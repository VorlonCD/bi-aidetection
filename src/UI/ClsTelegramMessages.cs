using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Amazon.Rekognition.Model;

using Newtonsoft.Json.Linq;

using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
//using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
//using Telegram.Bot.Types.InputFiles;

using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using static AITool.AITOOL;

using User = Telegram.Bot.Types.User;

namespace AITool
{
    public class ClsTelegramMessages:IDisposable
    {
        public TelegramBotClient telegramBot { get; set; } = null;

        private HttpClient telegramHttpClient = null;
        private User BotUser = null;
        private ThreadSafe.Boolean Started = new ThreadSafe.Boolean(false);
        private ThreadSafe.Boolean StartingReceive = new ThreadSafe.Boolean(false);
        private CancellationTokenSource TelegramCTS = new CancellationTokenSource();
        private bool disposedValue;
        public ClsTelegramMessages()
        {
            TryStartTelegram();
        }

        public async Task<bool> TryStartTelegram()
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            bool ret = true;
            //
            try
            {
                if (AppSettings.Settings.telegram_token.IsNotNull() && AppSettings.Settings.telegram_chatids.IsNotEmpty())
                {
                    if (!this.Started)
                    {
                        Log("Debug: Initializing Telegram Bot...");

                        Stopwatch sw = Stopwatch.StartNew();

                        //try to prevent telegram Could not create SSL/TLS secure channel exception on Win7.
                        //This may also need TLS 1.2 and 1.3 checked in Internet Options > Advanced tab....
                        ServicePointManager.Expect100Continue = true;
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls; //| SecurityProtocolType.Tls13;

                        this.telegramHttpClient = new System.Net.Http.HttpClient();
                        this.telegramHttpClient.Timeout = TimeSpan.FromSeconds(AppSettings.Settings.HTTPClientRemoteTimeoutSeconds);

                        this.telegramBot = new TelegramBotClient(AppSettings.Settings.telegram_token, this.telegramHttpClient);

                        //this may be redundant:
                        this.telegramBot.Timeout = TimeSpan.FromSeconds(AppSettings.Settings.HTTPClientRemoteTimeoutSeconds);

                        //for debugging, remove later...
                        this.telegramBot.OnApiResponseReceived += TelegramBot_OnApiResponseReceived;
                        this.telegramBot.OnMakingApiRequest += TelegramBot_OnMakingApiRequest;

                        //start another thread to initialize listening for commands (said it was non blocking but was having weird issues with it)
                        if (!this.StartingReceive)
                        {
                            //Only start listening if another instance is not running
                            if (!AppSettings.AlreadyRunning && AppSettings.Settings.telegram_monitor_commands)
                                Task.Run(TelegramStartReceiving);
                        }
                        else
                            Log($"Debug: Already initializing StartReceiving?");

                        Log($"Debug: ...Done in {sw.ElapsedMilliseconds}ms.");

                        this.Started= true;
                    }

                }
                else
                {
                    Log("Debug: Telegram Bot cannot initialize because of missing token or chatid.");
                }

            }
            catch (Exception ex)
            {
                ret = false;
                this.Started = false;
                throw;
            }

            return ret;
        }

        private async void TelegramStartReceiving()
        {
            //put this on another thread because it seemed to freeze even though it was supposed to be non blocking?

            try
            {
                this.StartingReceive= true;

                Log("Debug: (Initializing StartReceiving)...");

                Stopwatch sw = Stopwatch.StartNew();

                // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
                var receiverOptions = new ReceiverOptions { AllowedUpdates = { } }; // receive all update types 

                this.telegramBot.StartReceiving(
                    TelegramHandleUpdateAsync,
                    TelegramHandleErrorAsync,
                    receiverOptions,
                    cancellationToken: this.TelegramCTS.Token);

                Log("Debug: (Getting User Info)...");

                BotUser = await this.telegramBot.GetMeAsync();

                if (AppSettings.Settings.telegram_chatids.GetStrAtIndex(0).IsNotEmpty())
                {
                    Log("Debug: (Sending intro message)...");
                    string AssemVer = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                    Message message = await this.telegramBot.SendTextMessageAsync(AppSettings.Settings.telegram_chatids.GetStrAtIndex(0),
                        $"AITOOL {AssemVer} Initialized.  " +
                        $"\nTo send Telegram commands, first ask BotFather to disable '/setprivacy'.  " +
                        $"\nCommand Usage: " +
                        $"\nPAUSE|STOP|START|RESUME [CAMNAME] [MINUTES]" +
                        $"\n'PAUSE 1' will pause all cams for 1 min." +
                        $"\nMUTE, UNMUTE, VOLUMEUP, VOLUMEDOWN, " +
                        $"\nVOLUMESET Level" +
                        $"\nSHUTDOWNCOMPUTER\nRESTARTCOMPUTER\nRESTARTAITOOL\nSCREENSHOT");
                }
                else
                {
                    Log("Debug: (Skipping intro message because no ChatID configured)...");
                }

                Log($"Debug: Ready to receive control messages. Done in {sw.ElapsedMilliseconds}ms.  Listening for Telegram messages from {BotUser.Username} ({BotUser.Id}, {BotUser.FirstName} {BotUser.LastName}), IsBot={BotUser.IsBot}, CanJoinGroups={BotUser.CanJoinGroups}, CanReadAllGroupMessages={BotUser.CanReadAllGroupMessages}...");

            }
            catch (Exception ex)
            {
                Log($"Error: {ex.Msg()}");
            }
            finally
            {
                this.StartingReceive = false;
            }

        }

        private ValueTask TelegramBot_OnMakingApiRequest(ITelegramBotClient botClient, Telegram.Bot.Args.ApiRequestEventArgs args, CancellationToken cancellationToken = default)
        {
            Log($"Trace: API request: {args.Request.MethodName}");
            return default;
        }

        private ValueTask TelegramBot_OnApiResponseReceived(ITelegramBotClient botClient, Telegram.Bot.Args.ApiResponseEventArgs args, CancellationToken cancellationToken = default)
        {
            Log($"Trace: API response: {args.ResponseMessage.ToString().CleanString(",")}");
            return default;
        }

        public async Task<Message> SendTextMessageAsync(string ChatID, string Caption)
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            Message message = null;

            await this.TryStartTelegram();

            if (Started)
            {
                message = await this.telegramBot.SendTextMessageAsync(ChatID, Caption);
            }

            return message;

        }
        public async Task<Message> SendPhotoAsync(string ChatID, Stream FileStream, string file_id, string Filename, string Caption)
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            Message message = null;

            await this.TryStartTelegram();

            if (Started)
            {
                await this.telegramBot.SendChatActionAsync(ChatID, ChatAction.UploadPhoto);

                if (Filename.Contains("\\"))
                    Filename = Path.GetFileName(Filename);

                if (file_id.IsNull())
                    //message = await this.telegramBot.SendPhotoAsync(ChatID, new InputOnlineFile(FileStream, Filename), Caption);
                    message = await this.telegramBot.SendPhotoAsync(ChatID, new InputFileStream(FileStream, Filename), caption: Caption);
                else
                    message = await this.telegramBot.SendPhotoAsync(ChatID, InputFile.FromFileId(file_id), caption: Caption);
            }

            return message;

        }

        async Task TelegramHandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            try
            {
                // Only process Message updates: https://core.telegram.org/bots/api#message
                if (update.Type != UpdateType.Message)
                {
                    Log($"Debug: Telegram: Received an unsupported update.type '{update.Type}'");
                    return;
                }

                // Only process text messages
                if (update.Message!.Type != Telegram.Bot.Types.Enums.MessageType.Text && update.Message!.Type != Telegram.Bot.Types.Enums.MessageType.Voice)
                {
                    Log($"Debug: Telegram: Received an unsupported update.Message.type '{update.Message!.Type}'");
                    return;
                }


                var chatId = update.Message.Chat.Id;
                var messageText = update.Message.Text.IsNotEmpty() ? update.Message.Text : "";

                Log($"Debug: Telegram: Received a '{update.Type}' with content '{messageText}' in chat {chatId}.");

                if (update.Message.Type == Telegram.Bot.Types.Enums.MessageType.Voice)
                {
                    var filePath = Path.Combine(Global.GetTempFolder(), update.Message.Voice.FileId + ".ogg");
                    Log($"Debug: Telegram: Downloading a {update.Message.Voice.FileSize} byte Voice recording to {filePath}...");

                    Stopwatch sw = Stopwatch.StartNew();
                    using (FileStream fileStream = System.IO.File.OpenWrite(filePath))
                    {
                        Telegram.Bot.Types.File file = await this.telegramBot.GetInfoAndDownloadFileAsync(fileId: update.Message.Voice.FileId, destination: fileStream);
                    }
                    Log($"Debug: ...Done in {sw.ElapsedMilliseconds}ms");
                    Global.TelegramControlMessage($"play:{filePath}");
                }
                else
                {
                    Global.TelegramControlMessage(messageText);
                }


                // Echo received message text
                //Telegram.Bot.Types.Message sentMessage = await botClient.SendTextMessageAsync(
                //    chatId: chatId,
                //    text: "You said:\n" + messageText,
                //    cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                Log($"Error: {ex.Msg()}");
            }
        }

        string lasterr = "";
        DateTime lasterrtime = DateTime.MinValue;
        int repeated = 0;
        int inuse = 0;
        Task TelegramHandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {

            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error: [{apiRequestException.ErrorCode}] {apiRequestException.Message}",
                _ => exception.Msg()
            };

            //rate limit since I've seen 100 of these in a few seconds?
            double lastsecs = Math.Abs((DateTime.Now - lasterrtime).TotalSeconds);

            if (lasterr.Contains("409"))
                inuse++;

            if (!ErrorMessage.EqualsIgnoreCase(lasterr) || ErrorMessage.EqualsIgnoreCase(lasterr) && lastsecs >= 120)
            {
                string rep = "";
                if (this.repeated > 0)
                    rep = $"(Repeated {this.repeated} times)";
                Log($"Error: {rep}{ErrorMessage}");
                this.lasterr = ErrorMessage;
                this.lasterrtime = DateTime.Now;
                this.repeated = 0;
            }
            else
            {
                repeated++;
            }

            //this happens if more than one client is listening at a time
            //Telegram API [409] Conflict: terminated by other getUpdates request; make sure that only one bot instance is running
            if (inuse >= 6)
            {
                //give up and stop listening after three repeat errors
                Log("Error: Canceling receive Updates because another Telegram client is using the same token.");
                this.TelegramCTS.Cancel();
            }

            return Task.CompletedTask;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    if (this.telegramBot.IsNotNull())
                        this.telegramBot = null;

                    if (this.telegramHttpClient.IsNotNull())
                        this.telegramHttpClient.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ClsTelegramMessages()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
