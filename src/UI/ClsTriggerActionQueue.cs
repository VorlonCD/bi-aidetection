using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Speech.Synthesis;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

using MQTTnet.Client;

using NPushover.RequestObjects;
using NPushover.ResponseObjects;

using SixLabors.ImageSharp;

using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
//using Telegram.Bot.Types.InputFiles;

using static AITool.AITOOL;
//using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;

namespace AITool
{
    public class ClsTriggerActionQueueItem
    {
        public TriggerType TType { get; set; } = TriggerType.Unknown;
        public Camera cam { get; set; } = null;
        public string camname { get; set; } = "None";
        public History Hist { get; set; } = null;
        public ClsImageQueueItem CurImg { get; set; } = null;
        public bool Trigger { get; set; } = true;
        public string Text { get; set; } = "";
        public DateTime AddedTime { get; set; } = DateTime.MinValue;
        public long QueueCount { get; set; } = 0;
        public long QueueWaitMS { get; set; } = 0;
        public bool IsQueued { get; set; } = false;
        public long ActionTimeMS { get; set; } = 0;
        public long TotalTimeMS { get; set; } = 0;
        public ClsTriggerActionQueueItem(TriggerType ttype, Camera cam, ClsImageQueueItem CurImg, History hist, bool Trigger, string Text, bool IsQueued)
        {
            this.cam = cam;
            this.Hist = hist;
            this.CurImg = CurImg;

            //if they are null must be for testing?
            if (this.cam.IsNull())
                this.cam = new Camera("None");

            if (this.CurImg.IsNull())
                this.CurImg = new ClsImageQueueItem(Path.Combine(Global.GetTempFolder(), "test.jpg"), 0);

            if (this.Hist.IsNull())
                this.Hist = new History().Create(this.CurImg.image_path, DateTime.Now, this.cam.Name, Text, "", Trigger, "", "None", 0, Trigger);

            if (!this.cam.IsNull())
                this.camname = this.cam.Name;

            this.TType = ttype;
            this.Trigger = Trigger;
            this.AddedTime = DateTime.Now;
            this.Text = Text;
            this.IsQueued = IsQueued;
        }
    }

    public class ClsTriggerActionQueue
    {
        BlockingCollection<ClsTriggerActionQueueItem> TriggerActionQueue = new BlockingCollection<ClsTriggerActionQueueItem>();
        ConcurrentDictionary<string, ClsTriggerActionQueueItem> CancelActionDict = new ConcurrentDictionary<string, ClsTriggerActionQueueItem>();
        ConcurrentDictionary<string, ThreadSafe.Datetime> GroupsLastTriggerDict = new ConcurrentDictionary<string, ThreadSafe.Datetime>();
        public ThreadSafe.Datetime last_telegram_trigger_time { get; set; } = new ThreadSafe.Datetime(DateTime.MinValue);
        public ThreadSafe.Datetime last_Pushover_trigger_time { get; set; } = new ThreadSafe.Datetime(DateTime.MinValue);
        public ThreadSafe.Datetime TelegramRetryTime { get; set; } = new ThreadSafe.Datetime(DateTime.MinValue);
        public ThreadSafe.Datetime PushoverRetryTime { get; set; } = new ThreadSafe.Datetime(DateTime.MinValue);
        //public ClsURLItem _url { get; set; } = null;
        private String CurSrv = "";
        string ImgPath = "NoImage";
        public ThreadSafe.Integer Count { get; set; } = new ThreadSafe.Integer(0);
        public MovingCalcs QCountCalc { get; set; } = new MovingCalcs(250, "Action Queue Sizes", false);
        public MovingCalcs QTimeCalc { get; set; } = new MovingCalcs(250, "Action Queue Times", true);
        public MovingCalcs ActionTimeCalc { get; set; } = new MovingCalcs(250, "Actions", true);
        public MovingCalcs TotalTimeCalc { get; set; } = new MovingCalcs(250, "Actions", true);
        public ClsTriggerActionQueue()
        {
            Task.Run(this.TriggerActionJobQueueLoop);
            Task.Run(this.CancelActionJobQueueLoop);
        }

        public async Task<bool> AddTriggerActionAsync(TriggerType ttype, Camera cam, ClsImageQueueItem CurImg, History hist, bool Trigger, bool Wait, string CurSrv, string Text)
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            bool ret = false;
            this.CurSrv = CurSrv;

            if (CurImg != null)
            {
                this.ImgPath = CurImg.image_path;
            }


            ClsTriggerActionQueueItem AQI = new ClsTriggerActionQueueItem(ttype, cam, CurImg, hist, Trigger, Text, !Wait);

            //Make sure not to put cancel items in the queue if no cancel triggers are defined...

            bool HasCancel = ((AQI.cam.Action_mqtt_enabled && AQI.cam.Action_mqtt_payload_cancel.IsNotEmpty()) ||
                               (AQI.cam.Action_CancelURL_Enabled && AQI.cam.cancel_urls.Length > 0));

            bool IsCancel = ttype == TriggerType.Cancel || !Trigger;

            //bool DoIt = (Trigger || (!Trigger && cam.cancel_urls.Count > 0 || (cam.Action_mqtt_enabled && !string.IsNullOrEmpty(cam.Action_mqtt_payload_cancel))));
            bool DoIt = (Trigger || (IsCancel && HasCancel));

            if (DoIt)
            {
                if (Wait)  //not queued
                {
                    ret = await this.RunTriggers(AQI);
                }
                else
                {
                    if (this.TriggerActionQueue.Count <= AppSettings.Settings.MaxActionQueueSize)
                    {
                        if (!this.TriggerActionQueue.TryAdd(AQI))
                        {
                            Log($"Error: Action '{AQI.TType}' could not be added? {this.ImgPath}", this.CurSrv, AQI.cam, AQI.CurImg);
                        }
                        else
                        {
                            this.Count.WriteFullFence(this.TriggerActionQueue.Count);
                            AQI.QueueCount = this.Count.ReadFullFence();

                            ret = true;
                            Log($"Debug: Action '{AQI.TType}' ADDED to queue. Trigger={AQI.Trigger}, Queued={AQI.IsQueued}, Queue Count={AQI.QueueCount}, Image={this.ImgPath}", this.CurSrv, AQI.cam, AQI.CurImg);

                        }
                    }
                    else
                    {
                        Log($"Error: Action '{AQI.TType}' could not be added because queue size is {this.TriggerActionQueue.Count} and the max is {AppSettings.Settings.MaxActionQueueSize} (MaxActionQueueSize) - {this.ImgPath}", this.CurSrv, AQI.cam, AQI.CurImg);
                    }

                }
            }
            else
            {
                Log($"Debug: Action '{AQI.TType}' could not be added because there are no CANCEL actions configured.", this.CurSrv, AQI.cam, AQI.CurImg);
            }

            return ret;
        }

        private async Task TriggerActionJobQueueLoop()
        {

            try
            {
                //this runs forever and blocks if nothing is in the queue
                foreach (ClsTriggerActionQueueItem AQI in this.TriggerActionQueue.GetConsumingEnumerable(MasterCTS.Token))
                {
                    if (MasterCTS.IsCancellationRequested)
                        break;

                    try
                    {
                        await this.RunTriggers(AQI);
                        await Task.Delay(50); //very short wait between trigger queue events
                    }
                    catch (Exception ex)
                    {

                        Log($"Error: " + ex.ToString(), this.CurSrv, AQI.cam, AQI.CurImg);
                    }

                }

            }
            catch (Exception ex)
            {

                Log($"Exit ActionQueueLoop: {ex.Message}");
            }

            Log($"Exit ActionQueueLoop?");

        }

        private async Task CancelActionJobQueueLoop()
        {
            while (true)
            {
                if (MasterCTS.IsCancellationRequested)
                    break;

                if (!this.CancelActionDict.IsEmpty)
                {

                    foreach (ClsTriggerActionQueueItem AQI in CancelActionDict.Values)
                    {

                        try
                        {
                            if (AQI.cam.Action_Cancel_Timer_Enabled.ReadFullFence())
                            {
                                if ((DateTime.Now - AQI.cam.Action_Cancel_Start_Time.Read()).TotalSeconds >= AppSettings.Settings.ActionCancelSeconds)
                                {
                                    Log($"Debug: Running cancel Action '{AQI.TType}' in queue for event '{AQI.Hist.Detections}' for camera '{AQI.camname}', after {(DateTime.Now - AQI.cam.Action_Cancel_Start_Time.Read()).TotalSeconds.Round()} seconds...", this.CurSrv, AQI.cam, AQI.CurImg);
                                    await this.RunTriggers(AQI);
                                    AQI.cam.Action_Cancel_Timer_Enabled.WriteFullFence(false);  // will be deleted next time the loop goes through
                                }
                            }
                            else
                            {
                                CancelActionDict.TryRemove(AQI.camname.ToLower(), out ClsTriggerActionQueueItem removedItem);
                                Log($"Debug: Removed cancel Action '{AQI.TType}' in queue for event '{AQI.Hist.Detections}' for camera '{AQI.camname}', after {(DateTime.Now - AQI.cam.Action_Cancel_Start_Time.Read()).TotalSeconds.Round()} seconds", this.CurSrv, AQI.cam, AQI.CurImg);

                            }

                        }
                        catch (Exception ex)
                        {

                            Log($"Error: " + ex.ToString(), this.CurSrv, AQI.cam, AQI.CurImg);
                        }
                    }

                }

                await Task.Delay(1000); //Only check every second
            }

        }

        public async Task<bool> RunTriggers(ClsTriggerActionQueueItem AQI)
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            bool res = false;

            try
            {
                AQI.QueueWaitMS = Convert.ToInt64((DateTime.Now - AQI.AddedTime).TotalMilliseconds);
                this.QTimeCalc.AddToCalc(AQI.QueueWaitMS);
                bool WasSkipped = false;

                Stopwatch sw = Stopwatch.StartNew();

                this.Count.WriteFullFence(this.TriggerActionQueue.Count);

                this.QCountCalc.AddToCalc(AQI.QueueCount);

                Global.SendMessage(MessageType.UpdateStatus);

                if (AQI.TType == TriggerType.TelegramText)
                {
                    if (AppSettings.Settings.telegram_chatids.Count > 0 && AppSettings.Settings.telegram_token != "" && !(AQI.cam.Paused && AQI.cam.PauseTelegram))
                        res = await this.TelegramText(AQI);
                    else
                        WasSkipped = true;
                }
                else if (AQI.TType == TriggerType.TelegramImageUpload)
                {
                    if (AppSettings.Settings.telegram_chatids.Count > 0 && AppSettings.Settings.telegram_token != "" && !(AQI.cam.Paused && AQI.cam.PauseTelegram))
                        res = await this.TelegramUpload(AQI);
                    else
                        WasSkipped = true;
                }
                else if (AQI.TType == TriggerType.Pushover)
                {
                    if (!string.IsNullOrEmpty(AppSettings.Settings.pushover_APIKey) && !string.IsNullOrEmpty(AppSettings.Settings.pushover_UserKey) && !(AQI.cam.Paused && AQI.cam.PausePushover))
                        res = await this.PushoverUpload(AQI);
                    else
                        WasSkipped = true;
                }
                else
                {
                    res = await this.Trigger(AQI);
                }


                bool HasCancelAction = ((AQI.cam.Action_mqtt_enabled && !AQI.cam.Action_mqtt_payload_cancel.IsEmpty()) ||
                                       (AQI.cam.Action_CancelURL_Enabled && AQI.cam.cancel_urls.Length > 0));

                if (HasCancelAction)
                {
                    if (AQI.TType == TriggerType.Cancel || AQI.Trigger == false)  //If this is a CANCEL anyway...
                    {
                        //if we already did a cancel, set flag to delete the queued cancel item if exists and log that we are removing from queue
                        if (AQI.cam.Action_Cancel_Timer_Enabled.ReadFullFence() || this.CancelActionDict.ContainsKey(AQI.camname.ToLower()))
                        {
                            AQI.cam.Action_Cancel_Timer_Enabled.WriteFullFence(false);
                            if (this.CancelActionDict.ContainsKey(AQI.camname.ToLower()))
                                this.CancelActionDict.TryRemove(AQI.camname.ToLower(), out ClsTriggerActionQueueItem ignoreme);

                            Log($"Debug: Cancel action '{AQI.TType}' has been removed due to event '{AQI.Hist.Detections}' for camera '{AQI.camname}'", this.CurSrv, AQI.cam, AQI.CurImg);

                        }
                    }
                    else //If this is another TRIGGER...
                    {
                        if (this.CancelActionDict.ContainsKey(AQI.camname.ToLower()))
                        {
                            //if already in queue, update date
                            Log($"Debug: EXTENDING cancel action '{AQI.TType}' time due to event '{AQI.Hist.Detections}' for camera '{AQI.camname}', waiting {AppSettings.Settings.ActionCancelSeconds} seconds...", this.CurSrv, AQI.cam, AQI.CurImg);
                            AQI.cam.Action_Cancel_Start_Time.Write(DateTime.Now);
                        }
                        else  //add it to the queue
                        {
                            Log($"Debug: Cancel action '{AQI.TType}' queued due to event '{AQI.Hist.Detections}' for camera '{AQI.camname}', waiting {AppSettings.Settings.ActionCancelSeconds} seconds...", this.CurSrv, AQI.cam, AQI.CurImg);
                            AQI.cam.Action_Cancel_Start_Time.Write(DateTime.Now);
                            AQI.cam.Action_Cancel_Timer_Enabled.WriteFullFence(true);
                            AQI.Trigger = false;  //set to be a cancel
                            AQI.TType = TriggerType.Cancel;
                            this.CancelActionDict.TryAdd(AQI.camname.ToLower(), AQI);
                        }
                    }
                }
                else
                {
                    Log($"Debug: Cancel action '{AQI.TType}' could not be queued because there are no CANCEL actions configured.   Event='{AQI.Hist.Detections}' for camera '{AQI.camname}'", this.CurSrv, AQI.cam, AQI.CurImg);
                }

                this.Count.WriteFullFence(this.TriggerActionQueue.Count);

                AQI.ActionTimeMS = sw.ElapsedMilliseconds;
                AQI.TotalTimeMS = Convert.ToInt64((DateTime.Now - AQI.AddedTime).TotalMilliseconds);
                this.TotalTimeCalc.AddToCalc(AQI.TotalTimeMS);
                this.ActionTimeCalc.AddToCalc(AQI.ActionTimeMS);

                if (!WasSkipped)
                {
                    Log($"Debug: Action '{AQI.TType}' done. Succeeded={res}, Trigger={AQI.Trigger}, Queued={AQI.IsQueued}, Queue Count={AQI.QueueCount} (Min={this.QCountCalc.MinS},Max={this.QCountCalc.MaxS},Avg={this.QCountCalc.AvgS}), Total time={AQI.TotalTimeMS}ms (Min={this.TotalTimeCalc.MinS}ms,Max={this.TotalTimeCalc.MaxS}ms,Avg={Convert.ToInt64(this.TotalTimeCalc.AvgS)}ms), Queue time={AQI.QueueWaitMS} (Min={this.QTimeCalc.MinS}ms,Max={this.QTimeCalc.MaxS}ms,Avg={Convert.ToInt64(this.QTimeCalc.AvgS)}ms), Action Time={AQI.ActionTimeMS}ms (Min={this.ActionTimeCalc.MinS}ms,Max={this.ActionTimeCalc.MaxS}ms,Avg={Convert.ToInt64(this.ActionTimeCalc.AvgS)}ms), Image={this.ImgPath}", this.CurSrv, AQI.cam, AQI.CurImg);
                }

                Global.SendMessage(MessageType.UpdateStatus);

            }
            catch (Exception ex)
            {
                res = false;
                Log("Error: " + ex.Msg(), this.CurSrv, AQI.cam, AQI.CurImg);
            }

            return res;
        }

        //public bool IsNotInCooldown(Camera cam, out double cooltime)
        //{
        //    if (cam.CameraGroup.IsEmpty())
        //    {
        //        cooltime = (DateTime.Now - cam.last_trigger_time.Read()).TotalSeconds;
        //        return cooltime >= cam.cooldown_time_seconds;
        //    }
        //    else
        //    {
        //        ThreadSafe.Datetime last_trigger_time = new ThreadSafe.Datetime(DateTime.MinValue);

        //        if (GroupsLastTriggerDict.)
        //    }
        //}

        //trigger actions
        public async Task<bool> Trigger(ClsTriggerActionQueueItem AQI)
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            bool ret = true;

            //mostly for testing when we dont have a current image...
            if (AQI.CurImg == null)
            {
                if (!string.IsNullOrEmpty(AQI.cam.last_image_file_with_detections))
                {
                    AQI.CurImg = new ClsImageQueueItem(AQI.cam.last_image_file_with_detections, 1);
                }
                else if (!string.IsNullOrEmpty(AQI.cam.last_image_file))
                {
                    AQI.CurImg = new ClsImageQueueItem(AQI.cam.last_image_file, 1);
                }
                else
                {
                    Log($"Error: No image to process?", this.CurSrv, AQI.camname);
                    return false;
                }
            }

            try
            {
                double cooltime = (DateTime.Now - AQI.cam.last_trigger_time.Read()).TotalSeconds;
                string tmpfile = "";

                //only trigger if cameras cooldown time since last detection has passed
                if (cooltime >= AQI.cam.cooldown_time_seconds)
                {

                    //merge annotations
                    if (AQI.cam.Action_image_merge_detections && AQI.Trigger)
                    {
                        tmpfile = await this.MergeImageAnnotations(AQI);

                        if (!string.Equals(AQI.CurImg.image_path, tmpfile, StringComparison.OrdinalIgnoreCase) && System.IO.File.Exists(tmpfile))  //it wont exist if no detections or failure...
                        {
                            AQI.CurImg = new ClsImageQueueItem(tmpfile, 1);
                            //force the image to load right away to try to avoid BI showing blank images when given a jpg file for the thumbnail
                            AQI.CurImg.LoadImage();
                        }
                    }

                    //copy image
                    if (AQI.cam.Action_image_copy_enabled && AQI.Trigger)
                    {
                        Log($"Debug:   Copying image to network folder...", this.CurSrv, AQI.cam, AQI.CurImg);
                        string newimagepath = "";
                        if (!this.CopyImage(AQI, ref newimagepath))
                        {
                            ret = false;
                            Log($"Warn:   -> Warning: Image could not be copied to network folder.", this.CurSrv, AQI.cam, AQI.CurImg);
                        }
                        else
                        {
                            Log($"Debug:   -> Image copied to network folder {newimagepath}", this.CurSrv, AQI.cam, AQI.CurImg);
                            //set the image path to the new path so all imagename variable works
                            AQI.CurImg = new ClsImageQueueItem(newimagepath, 1);
                            //force the image to load right away to try to avoid BI showing blank images when given a jpg file for the thumbnail
                            AQI.CurImg.LoadImage();
                        }

                    }

                    // Activate BI window
                    if (AQI.cam.Action_ActivateBlueIrisWindow && AQI.Trigger)
                        Global.ShowProcessWindow("blueiris.exe", "Blue Iris", Global.ShowWindowEnum.SW_SHOWMAXIMIZED);

                    //Play sounds
                    if (AQI.cam.Action_PlaySounds && AQI.Trigger)
                    {
                        double soundcooltime = (DateTime.Now - AQI.cam.last_sound_time.Read()).TotalSeconds;

                        if (soundcooltime >= AQI.cam.sound_cooldown_time_seconds)
                        {
                            try
                            {

                                //Examples:
                                //Simple sound play:
                                //    C:\BlueIris\sounds\are-you-kidding.wav
                                //    are-you-kidding.wav   <-- No need to specify path if in AITOOL, BlueIris folder or Windows Media folder
                                //    are-you-kidding
                                //    C:\Windows\Media\Ring10.wav
                                //    Ring10.wav
                                //Conditional:
                                //    cat ; catsound.wav
                                //    cat,dog,sheep ; animalsound.wav
                                //    bear ; fuuuuck.wav
                                //Talk:
                                //    Talk:There is a [Label] outside
                                //    person ; talk:There is a mother f'in person in the driveway
                                //Combine any with pipe symbols
                                //    Talk:There is a [Label] outside | object1, object2 ; soundfile.wav | object1, object2 ; anotherfile.wav | * ; defaultsound.wav
                                string snds = AITOOL.ReplaceParams(AQI.cam, AQI.Hist, AQI.CurImg, AQI.cam.Action_Sounds, Global.IPType.Path);

                                bool wasplayed = false;

                                List<string> prms = snds.SplitStr("|");
                                foreach (string prm in prms)
                                {
                                    if (prm.Contains(";"))
                                    {
                                        //prm0 - object1, object2
                                        //prm1 - soundfile.wav
                                        List<string> splt = prm.SplitStr(";");

                                        ClsRelevantObjectManager rom = new ClsRelevantObjectManager(splt[0], "Sound", AQI.cam);

                                        if (!AQI.Hist.IsNull() && rom.IsRelevant(AQI.Hist.Predictions(), false, out bool IgnoreImageMask, out bool IgnoreDynamicMask) == ResultType.Relevant)
                                        {

                                            if (splt[1].StartsWith("talk:", StringComparison.OrdinalIgnoreCase))
                                            {
                                                string speech = splt[1].GetWord("talk:", "");

                                                if (speech.IsNotNull())
                                                {
                                                    //if you would like to change the default voice used, you have to change it in the OLD control panel, not the new Windows 10/11 version?
                                                    //Start menu > type 'Control Panel' > Easy of use > Speech Recognition > Advanced speech options > Text to speech tab.

                                                    using var synth = new SpeechSynthesizer();

                                                    synth.SetOutputToDefaultAudioDevice();
                                                    Log($"Debug:   Talking using system default Windows voice '{synth.Voice.Name}': '{speech}'...", this.CurSrv, AQI.cam, AQI.CurImg);
                                                    synth.Speak(speech);
                                                    wasplayed = true;
                                                }

                                            }
                                            else
                                            {
                                                string soundfile = Global.FindSoundFile(splt[1].Trim());
                                                if (soundfile.IsNotNull())
                                                {

                                                    Log($"Debug:   Playing sound: {soundfile}...", this.CurSrv, AQI.cam, AQI.CurImg);
                                                    using SoundPlayer sp = new SoundPlayer(soundfile);
                                                    sp.PlaySync();
                                                    wasplayed = true;
                                                }
                                                else
                                                {
                                                    Log($"Error: Sound file not found: {soundfile}");
                                                }

                                            }
                                        }
                                    }
                                    else if (prm.StartsWith("talk:", StringComparison.OrdinalIgnoreCase))
                                    {
                                        string speech = prm.GetWord("talk:", "");

                                        if (speech.IsNotNull())
                                        {
                                            using var synth = new SpeechSynthesizer();

                                            synth.SetOutputToDefaultAudioDevice();
                                            Log($"Debug:   Talking using system default Windows voice '{synth.Voice.Name}': '{speech}'...", this.CurSrv, AQI.cam, AQI.CurImg);
                                            synth.Speak(speech);
                                            wasplayed = true;
                                        }
                                    }
                                    else   //assume it is JUST a sound file
                                    {
                                        string soundfile = Global.FindSoundFile(prm);
                                        if (soundfile.IsNotNull())
                                        {

                                            Log($"Debug:   Playing sound: {soundfile}...", this.CurSrv, AQI.cam, AQI.CurImg);
                                            using SoundPlayer sp = new SoundPlayer(soundfile);
                                            sp.PlaySync();
                                            wasplayed = true;
                                        }
                                        else
                                        {
                                            Log($"Error: Sound file not found: {prm}");
                                        }
                                    }

                                    if (wasplayed && prms.Count > 1)
                                        await Task.Delay(50); //very short wait between sound events

                                }

                                if (wasplayed)
                                {

                                    AQI.cam.last_sound_time.Write(DateTime.Now);

                                    if (AppSettings.Settings.ActionDelayMS >= 100)  //dont show for tiny delays
                                        Log($"Debug:  ...Applying 'ActionDelayMS' delay of {AppSettings.Settings.ActionDelayMS}ms.");

                                    //lets not wait after each sound
                                    //await Task.Delay(AppSettings.Settings.ActionDelayMS); //very short wait between trigger events

                                }
                                else
                                {
                                    Log($"Debug: No object matched sound to play.", this.CurSrv, AQI.cam, AQI.CurImg);
                                }

                            }
                            catch (Exception ex)
                            {

                                ret = false;
                                Log($"Error: while calling sound '{AQI.cam.Action_Sounds}', got: {ex.Msg()}", this.CurSrv, AQI.cam, AQI.CurImg);
                            }

                        }
                        else
                        {
                            Log($"   Camera {AQI.camname} is still in SOUND cooldown. Sound was not played. ({soundcooltime} of {AQI.cam.sound_cooldown_time_seconds} seconds - See Cameras 'sound_cooldown_time_seconds' in settings file)", this.CurSrv, AQI.cam, AQI.CurImg);

                        }
                    }

                    //run external program
                    if (AQI.cam.Action_RunProgram && AQI.Trigger)
                    {
                        string run = "";
                        string param = "";
                        try
                        {
                            run = AITOOL.ReplaceParams(AQI.cam, AQI.Hist, AQI.CurImg, AQI.cam.Action_RunProgramString, Global.IPType.Path);
                            param = AITOOL.ReplaceParams(AQI.cam, AQI.Hist, AQI.CurImg, AQI.cam.Action_RunProgramArgsString, Global.IPType.Path);
                            Log($"Debug:   Starting external app - Camera={AQI.camname} run='{run}', param='{param}'", this.CurSrv, AQI.cam, AQI.CurImg);

                            Process.Start(run, param);

                            if (AppSettings.Settings.ActionDelayMS >= 100)  //dont show for tiny delays
                                Log($"Debug: ...Applying 'ActionDelayMS' delay of {AppSettings.Settings.ActionDelayMS}ms.");

                            //await Task.Delay(AppSettings.Settings.ActionDelayMS); //very short wait between trigger events
                        }
                        catch (Exception ex)
                        {

                            ret = false;
                            Log($"Error: while running program '{run}' with params '{param}', got: {ex.Msg()}", this.CurSrv, AQI.cam, AQI.CurImg);
                        }
                    }

                    //call trigger urls
                    if (!(AQI.cam.Paused && AQI.cam.PauseURL))
                    {
                        if (AQI.Trigger && AQI.cam.Action_TriggerURL_Enabled && AQI.cam.trigger_urls.Length > 0)
                        {
                            //replace url parameters with according values
                            List<string> urls = new List<string>();
                            //call urls
                            foreach (string url in AQI.cam.trigger_urls)
                            {
                                string tmp = AITOOL.ReplaceParams(AQI.cam, AQI.Hist, AQI.CurImg, url, Global.IPType.URL);
                                urls.Add(tmp);

                            }

                            bool result = await this.CallTriggerURLs(urls, AQI);
                        }
                        else if (!AQI.Trigger && AQI.cam.Action_CancelURL_Enabled && AQI.cam.cancel_urls.Length > 0)
                        {
                            //replace url parameters with according values
                            List<string> urls = new List<string>();
                            //call urls
                            foreach (string url in AQI.cam.cancel_urls)
                            {
                                string tmp = AITOOL.ReplaceParams(AQI.cam, AQI.Hist, AQI.CurImg, url, Global.IPType.URL);
                                urls.Add(tmp);

                            }

                            bool result = await this.CallTriggerURLs(urls, AQI);

                        }

                    }


                    if (AQI.cam.Action_mqtt_enabled && !(AQI.cam.Paused && AQI.cam.PauseMQTT))
                    {

                        //make sure it is a matching object, but call MQTT in any case if it is a canceled event
                        if (!AQI.Hist.IsNull() && (!AQI.Trigger || AQI.cam.MQTTTriggeringObjects.IsRelevant(AQI.Hist.Predictions(), false, out bool IgnoreImageMask, out bool IgnoreDynamicMask) == ResultType.Relevant))
                        {
                            string topic = "";
                            string payload = "";

                            if (AQI.Trigger)
                            {
                                topic = AITOOL.ReplaceParams(AQI.cam, AQI.Hist, AQI.CurImg, AQI.cam.Action_mqtt_topic, Global.IPType.URL);
                                payload = AITOOL.ReplaceParams(AQI.cam, AQI.Hist, AQI.CurImg, AQI.cam.Action_mqtt_payload, Global.IPType.URL);
                                Log($"Debug: MQTT Trigger event - [SummaryNonEscaped]='{AQI.Hist.Detections}', After replacement Topic='{topic}', Payload='{payload}'");
                            }
                            else
                            {
                                topic = AITOOL.ReplaceParams(AQI.cam, AQI.Hist, AQI.CurImg, AQI.cam.Action_mqtt_topic_cancel, Global.IPType.URL);
                                payload = AITOOL.ReplaceParams(AQI.cam, AQI.Hist, AQI.CurImg, AQI.cam.Action_mqtt_payload_cancel, Global.IPType.URL);
                                Log($"Debug: MQTT Cancel event - [SummaryNonEscaped]='{AQI.Hist.Detections}', After replacement Topic='{topic}', Payload='{payload}'");
                            }


                            List<string> topics = topic.SplitStr("|");
                            List<string> payloads = payload.SplitStr("|");
                            if (topics.Count == payloads.Count)
                            {
                                ClsImageQueueItem ci = null;

                                for (int i = 0; i < topics.Count; i++)
                                {
                                    if (AQI.cam.Action_mqtt_send_image && topics[i].IndexOf("/image", StringComparison.OrdinalIgnoreCase) >= 0)
                                        ci = AQI.CurImg;
                                    else
                                        ci = null;
                                    MqttClientPublishResult pr = await AITOOL.mqttClient.PublishAsync(topics[i], payloads[i], AQI.cam.Action_mqtt_retain_message, ci);
                                    if (pr == null || pr.ReasonCode != MqttClientPublishReasonCode.Success)
                                        ret = false;

                                    if (AppSettings.Settings.ActionDelayMS >= 100)  //dont show for tiny delays
                                        Log($"Debug:  ...Applying 'ActionDelayMS' delay of {AppSettings.Settings.ActionDelayMS}ms.");

                                    await Task.Delay(AppSettings.Settings.ActionDelayMS); //very short wait between trigger events
                                }

                            }
                            else
                            {
                                Log($"Error: You must have an equal number of MQTT topics and payloads. (separated by | pipe symbol).  Topics='{topic}', Payloads='{payloads}'");
                                ret = false;
                            }

                        }
                        else
                        {
                            Log("Trace: Skipping MQTT call.");
                            ret = true;   //dont return false unless actual error
                        }

                    }

                    //upload to pushover
                    if (AQI.cam.Action_pushover_enabled && AQI.Trigger && !(AQI.cam.Paused && AQI.cam.PausePushover))
                    {

                        if (!await this.PushoverUpload(AQI))
                        {
                            ret = false;
                            Log($"Error:   -> ERROR sending message or image to Pushover", this.CurSrv, AQI.cam, AQI.CurImg);
                        }
                        else
                        {
                            Log($"Debug:   -> Sent message or image to Pushover.", this.CurSrv, AQI.cam, AQI.CurImg);
                        }

                        if (AppSettings.Settings.ActionDelayMS >= 100)  //dont show for tiny delays
                            Log($"Debug:  ...Applying 'ActionDelayMS' delay of {AppSettings.Settings.ActionDelayMS}ms.");

                        await Task.Delay(AppSettings.Settings.ActionDelayMS); //very short wait between trigger events

                    }


                    //upload to telegram
                    if (AQI.cam.telegram_enabled && AQI.Trigger && !(AQI.cam.Paused && AQI.cam.PauseTelegram))
                    {

                        if (!await this.TelegramUpload(AQI))
                        {
                            ret = false;
                            Log($"Error:   -> ERROR sending image to Telegram.", this.CurSrv, AQI.cam, AQI.CurImg);
                        }
                        else
                        {
                            Log($"Debug:   -> Sent image to Telegram.", this.CurSrv, AQI.cam, AQI.CurImg);
                        }

                        if (AppSettings.Settings.ActionDelayMS >= 100)  //dont show for tiny delays
                            Log($"Debug:  ...Applying 'ActionDelayMS' delay of {AppSettings.Settings.ActionDelayMS}ms.");

                        await Task.Delay(AppSettings.Settings.ActionDelayMS); //very short wait between trigger events
                    }

                    if (AQI.Trigger)
                    {
                        AQI.cam.last_trigger_time.Write(DateTime.Now); //reset cooldown time every time an image contains something, even if no trigger was called (still in cooldown time)
                        Log($"Debug: {AQI.camname} last triggered at {AQI.cam.last_trigger_time.Read()}.", this.CurSrv, AQI.cam, AQI.CurImg);
                        Global.UpdateLabel($"{AQI.camname} last triggered at {AQI.cam.last_trigger_time.Read()}.", "lbl_info");
                    }

                }
                else
                {
                    //log that nothing was done
                    Log($"   Camera {AQI.camname} is still in cooldown. ({cooltime.Round()} of {AQI.cam.cooldown_time_seconds} seconds - See Cameras 'cooldown_time_seconds' in settings file)", this.CurSrv, AQI.cam, AQI.CurImg);
                }


                if (AQI.cam.Action_image_merge_detections && AQI.Trigger && !string.IsNullOrEmpty(tmpfile) && tmpfile.IndexOf(Global.GetTempFolder(), StringComparison.OrdinalIgnoreCase) >= 0 && System.IO.File.Exists(tmpfile))
                {
                    Global.SafeFileDeleteAsync(tmpfile, "TriggerActionQueue");
                }


            }
            catch (Exception ex)
            {

                Log($"Error: " + ex.Msg(), this.CurSrv, AQI.cam, AQI.CurImg);
            }


            return ret;

        }


        public async Task<string> MergeImageAnnotations(ClsTriggerActionQueueItem AQI)
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            int countr = 0;
            string detections = "";
            string lasttext = "";
            string lastposition = "";
            string OutputImageFile = "";

            try
            {
                Log($"Debug: Merging image annotations: " + AQI.CurImg.image_path, "", "", AQI.CurImg);

                if (AQI.CurImg.IsValid())
                {
                    AQI.cam.UpdateImageResolutions(AQI.CurImg);

                    Stopwatch sw = Stopwatch.StartNew();

                    //using MemoryStream ms = AQI.CurImg.ToStream();

                    using (Bitmap img = AQI.CurImg.ToBitmap())
                    {
                        using (Graphics g = Graphics.FromImage(img))
                        {
                            if (AQI.Hist != null && !string.IsNullOrEmpty(AQI.Hist.PredictionsJSON))
                            {
                                List<ClsPrediction> predictions = AQI.Hist.Predictions();

                                for (int i = predictions.Count - 1; i >= 0; i--)
                                {
                                    ClsPrediction pred = predictions[i];

                                    if (AITOOL.DrawAnnotation(g,
                                                          pred,
                                                          img.Width,
                                                          img.Height))
                                    {
                                        countr++;
                                    }

                                    //bool Merge = false;

                                    //if (AppSettings.Settings.HistoryOnlyDisplayRelevantObjects && pred.Result == ResultType.Relevant)
                                    //    Merge = true;
                                    //else if (!AppSettings.Settings.HistoryOnlyDisplayRelevantObjects)
                                    //    Merge = true;

                                    //if (Merge)
                                    //{
                                    //    if (pred.Result == ResultType.Relevant)
                                    //    {
                                    //        color = System.Drawing.Color.FromArgb(AppSettings.Settings.RectRelevantColorAlpha, AppSettings.Settings.RectRelevantColor);
                                    //    }
                                    //    else if (pred.Result == ResultType.DynamicMasked || pred.Result == ResultType.ImageMasked || pred.Result == ResultType.StaticMasked)
                                    //    {
                                    //        color = System.Drawing.Color.FromArgb(AppSettings.Settings.RectMaskedColorAlpha, AppSettings.Settings.RectMaskedColor);
                                    //    }
                                    //    else
                                    //    {
                                    //        color = System.Drawing.Color.FromArgb(AppSettings.Settings.RectIrrelevantColorAlpha, AppSettings.Settings.RectIrrelevantColor);
                                    //    }

                                    //    double xmin = pred.XMin;
                                    //    double ymin = pred.YMin;
                                    //    double xmax = pred.XMax;
                                    //    double ymax = pred.YMax;

                                    //    System.Drawing.Rectangle rect = pred.GetRectangle();  //new System.Drawing.Rectangle(xmin.ToInt(), ymin.ToInt(), (xmax - xmin).ToInt(), (ymax - ymin).ToInt());

                                    //    using (Pen pen = new Pen(color, AppSettings.Settings.RectBorderWidth))
                                    //    {
                                    //        g.DrawRectangle(pen, rect); //draw rectangle
                                    //    }

                                    //    //we need this since people can change the border width in the json file
                                    //    double halfbrd = AppSettings.Settings.RectBorderWidth / 2;

                                    //    System.Drawing.SizeF TextSize = g.MeasureString(lasttext, new Font(AppSettings.Settings.RectDetectionTextFont, AppSettings.Settings.RectDetectionTextSize)); //finds size of text to draw the background rectangle

                                    //    double x = xmin - halfbrd;
                                    //    double y = ymax + halfbrd;


                                    //    //adjust the x / width label so it doesnt go off screen
                                    //    double EndX = x + TextSize.Width;
                                    //    if (EndX > xmax)
                                    //    {
                                    //        //int diffx = x - sclxmax;
                                    //        x = xmax - TextSize.Width + halfbrd;
                                    //    }

                                    //    if (x < xmin)
                                    //        x = xmin;

                                    //    if (x < 0)
                                    //        x = 0;

                                    //    //adjust the y / height label so it doesnt go off screen
                                    //    double EndY = y + TextSize.Height;
                                    //    if (EndY > ymax)
                                    //    {
                                    //        //float diffy = EndY - sclymax;
                                    //        y = ymax - TextSize.Height - halfbrd;
                                    //    }


                                    //    if (y < 0)
                                    //        y = 0;

                                    //    //object name text below rectangle
                                    //    rect = new System.Drawing.Rectangle(x.ToInt(),
                                    //                                        y.ToInt(),
                                    //                                        img.Width,
                                    //                                        img.Height); //sets bounding box for drawn text

                                    //    Brush brush = new SolidBrush(color); //sets background rectangle color
                                    //    if (AppSettings.Settings.RectDetectionTextBackColor != System.Drawing.Color.Gainsboro)
                                    //        brush = new SolidBrush(AppSettings.Settings.RectDetectionTextBackColor);

                                    //    Brush forecolor = Brushes.Black;
                                    //    if (AppSettings.Settings.RectDetectionTextForeColor != System.Drawing.Color.Gainsboro)
                                    //        forecolor = new SolidBrush(AppSettings.Settings.RectDetectionTextForeColor);

                                    //    lasttext = pred.ToString();

                                    //    g.FillRectangle(brush,
                                    //                   x.ToInt(),
                                    //                   y.ToInt(),
                                    //                   TextSize.Width,
                                    //                   TextSize.Height); //draw grey background rectangle for detection text

                                    //    g.DrawString(lasttext,
                                    //                 new Font(AppSettings.Settings.RectDetectionTextFont,
                                    //                 AppSettings.Settings.RectDetectionTextSize),
                                    //                 forecolor,
                                    //                 rect); //draw detection text

                                    //    g.Flush();

                                    //    countr++;
                                    //}


                                }

                            }
                            //else
                            //{
                            //    //Use the old way -this code really doesnt need to be here but leaving just to make sure
                            //    detections = AQI.cam.last_detections_summary;
                            //    if (string.IsNullOrEmpty(detections))
                            //        detections = "";

                            //    string label = detections.GetWord("", ":");

                            //    if (label.IndexOf("irrelevant", StringComparison.OrdinalIgnoreCase) >= 0 || label.IndexOf("confidence", StringComparison.OrdinalIgnoreCase) >= 0 || label.IndexOf("masked", StringComparison.OrdinalIgnoreCase) >= 0 || label.IndexOf("errors", StringComparison.OrdinalIgnoreCase) >= 0)
                            //    {
                            //        detections = detections.SplitStr(":")[1]; //removes the "1x masked, 3x irrelevant:" before the actual detection, otherwise this would be displayed in the detection tags

                            //        if (label.IndexOf("masked", StringComparison.OrdinalIgnoreCase) >= 0)
                            //        {
                            //            color = System.Drawing.Color.FromArgb(AppSettings.Settings.RectMaskedColorAlpha, AppSettings.Settings.RectMaskedColor);
                            //        }
                            //        else
                            //        {
                            //            color = System.Drawing.Color.FromArgb(AppSettings.Settings.RectIrrelevantColorAlpha, AppSettings.Settings.RectIrrelevantColor);
                            //        }
                            //    }
                            //    else
                            //    {
                            //        color = System.Drawing.Color.FromArgb(AppSettings.Settings.RectRelevantColorAlpha, AppSettings.Settings.RectRelevantColor);
                            //    }

                            //    //List<string> detectlist = Global.Split(detections, "|;");
                            //    countr = AQI.cam.last_detections.Count;

                            //    //display a rectangle around each relevant object


                            //    for (int i = 0; i < countr; i++)
                            //    {
                            //        //({ Math.Round((user.confidence * 100), 2).ToString() }%)
                            //        lasttext = $"{AQI.cam.last_detections[i]} {String.Format(AppSettings.Settings.DisplayPercentageFormat, AQI.cam.last_confidences[i])}";
                            //        lastposition = AQI.cam.last_positions[i];  //load 'xmin,ymin,xmax,ymax' from third column into a string

                            //        //store xmin, ymin, xmax, ymax in separate variables
                            //        Int32.TryParse(lastposition.Split(',')[0], out int xmin);
                            //        Int32.TryParse(lastposition.Split(',')[1], out int ymin);
                            //        Int32.TryParse(lastposition.Split(',')[2], out int xmax);
                            //        Int32.TryParse(lastposition.Split(',')[3], out int ymax);

                            //        xmin = xmin + AQI.cam.XOffset;
                            //        ymin = ymin + AQI.cam.YOffset;

                            //        System.Drawing.Rectangle rect = new System.Drawing.Rectangle(xmin, ymin, xmax - xmin, ymax - ymin);


                            //        using (Pen pen = new Pen(color, AppSettings.Settings.RectBorderWidth))
                            //        {
                            //            g.DrawRectangle(pen, rect); //draw rectangle
                            //        }

                            //        //we need this since people can change the border width in the json file
                            //        int halfbrd = AppSettings.Settings.RectBorderWidth / 2;

                            //        //object name text below rectangle
                            //        rect = new System.Drawing.Rectangle(xmin - halfbrd, ymax + halfbrd, img.Width, img.Height); //sets bounding box for drawn text


                            //        Brush brush = new SolidBrush(color); //sets background rectangle color
                            //        if (AppSettings.Settings.RectDetectionTextBackColor != System.Drawing.Color.Gainsboro)
                            //            brush = new SolidBrush(AppSettings.Settings.RectDetectionTextBackColor);

                            //        Brush forecolor = Brushes.Black;
                            //        if (AppSettings.Settings.RectDetectionTextForeColor != System.Drawing.Color.Gainsboro)
                            //            forecolor = new SolidBrush(AppSettings.Settings.RectDetectionTextForeColor);

                            //        System.Drawing.SizeF size = g.MeasureString(lasttext, new Font(AppSettings.Settings.RectDetectionTextFont, AppSettings.Settings.RectDetectionTextSize)); //finds size of text to draw the background rectangle
                            //        g.FillRectangle(brush, xmin - halfbrd, ymax + halfbrd, size.Width, size.Height); //draw grey background rectangle for detection text
                            //        g.DrawString(lasttext, new Font(AppSettings.Settings.RectDetectionTextFont, AppSettings.Settings.RectDetectionTextSize), forecolor, rect); //draw detection text

                            //        g.Flush();

                            //        //Log($"...{i}, LastText='{lasttext}' - LastPosition='{lastposition}'");
                            //    }

                            //}


                            if (countr > 0)
                            {

                                GraphicsState gs = g.Save();

                                ImageCodecInfo jpgEncoder = this.GetEncoder(ImageFormat.Jpeg);

                                // Create an Encoder object based on the GUID  
                                // for the Quality parameter category.  
                                System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;

                                // Create an EncoderParameters object.  
                                // An EncoderParameters object has an array of EncoderParameter  
                                // objects. In this case, there is only one  
                                // EncoderParameter object in the array.  
                                EncoderParameters myEncoderParameters = new EncoderParameters(1);

                                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, AQI.cam.Action_image_merge_jpegquality);  //100=least compression, largest file size, best quality
                                myEncoderParameters.Param[0] = myEncoderParameter;

                                Global.WaitFileAccessResult result = new Global.WaitFileAccessResult();
                                result.Success = true; //assume true
                                string tmpfolder = Global.GetTempFolder();  //Path.GetTempPath();
                                if (AQI.cam.Action_image_merge_detections_makecopy && !(AQI.CurImg.image_path.IndexOf(tmpfolder, StringComparison.OrdinalIgnoreCase) >= 0))
                                    OutputImageFile = Path.Combine(tmpfolder, Path.GetFileName(AQI.CurImg.image_path));
                                else
                                    OutputImageFile = AQI.CurImg.image_path;

                                if (System.IO.File.Exists(OutputImageFile))
                                {
                                    result = await Global.WaitForFileAccessAsync(OutputImageFile, FileAccess.ReadWrite, FileShare.None);
                                }

                                if (result.Success)
                                {
                                    img.Save(OutputImageFile, jpgEncoder, myEncoderParameters);
                                    Log($"Debug: Merged {countr} detections in {sw.ElapsedMilliseconds}ms into image {OutputImageFile}", this.CurSrv, AQI.cam, AQI.CurImg);
                                }
                                else
                                {
                                    Log($"Error: Could not gain access to write merged file {OutputImageFile}", this.CurSrv, AQI.cam, AQI.CurImg);
                                }

                            }
                            else
                            {
                                Log($"Debug: No detections to merge.  Time={sw.ElapsedMilliseconds}ms, {OutputImageFile}", this.CurSrv, AQI.cam, AQI.CurImg);

                            }

                        }

                    }

                }
                else
                {
                    Log($"Error: could not find last image with detections: " + AQI.CurImg.image_path, this.CurSrv, AQI.cam, AQI.CurImg);
                }
            }
            catch (Exception ex)
            {

                Log($"Error: Detections='{detections}', LastText='{lasttext}', LastPostions='{lastposition}' - " + ex.Msg(), this.CurSrv, AQI.cam, AQI.CurImg);
            }

            return OutputImageFile;
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        public bool CopyImage(ClsTriggerActionQueueItem AQI, ref string dest_path)
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            bool ret = false;

            try
            {
                if (string.IsNullOrWhiteSpace(AQI.cam.Action_network_folder) || string.IsNullOrWhiteSpace(AQI.cam.Action_network_folder_filename))
                {
                    AITOOL.Log($"Error: Camera settings > 'Copy alert images to folder' path or 'Filename' is empty.: {AQI.cam.Action_network_folder} -- {AQI.cam.Action_network_folder_filename}");
                    return false;
                }

                string netfld = AITOOL.ReplaceParams(AQI.cam, AQI.Hist, AQI.CurImg, AQI.cam.Action_network_folder, Global.IPType.Path);

                if (string.IsNullOrWhiteSpace(netfld) || !netfld.Contains("\\"))
                {
                    AITOOL.Log($"Error: Camera settings > Copy alert images to folder is not a valid path: {netfld}");
                    return false;
                }

                if (!Directory.Exists(netfld))
                    Directory.CreateDirectory(netfld);

                //check to see if we need to clean the network folder out yet:
                //It will only check once a day, and will only clean if it has been over cam.Action_network_folder_purge_older_than_days  (defaults to 30 days)
                AQI.cam.CleanActionNetworkFolder();

                string ext = Path.GetExtension(AQI.CurImg.image_path);
                string filename = AITOOL.ReplaceParams(AQI.cam, AQI.Hist, AQI.CurImg, AQI.cam.Action_network_folder_filename, Global.IPType.Path).Trim().Replace(ext, "") + ext;

                dest_path = System.IO.Path.Combine(netfld, filename);

                Log($"Debug:  File copying from {AQI.CurImg.image_path} to {dest_path}", this.CurSrv, AQI.cam, AQI.CurImg);


                if (AQI.CurImg.CopyFileTo(dest_path))
                {
                    ret = true;
                }
            }
            catch (Exception ex)
            {
                ret = false;
                Log($"ERROR: Could not copy image {AQI.CurImg.image_path} to network path {dest_path}: {ex.Msg()}", this.CurSrv, AQI.cam, AQI.CurImg);
            }

            return ret;

        }
        //call trigger urls
        public async Task<bool> CallTriggerURLs(List<string> trigger_urls, ClsTriggerActionQueueItem AQI)
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            bool ret = true;
            string url = "";
            string type = "trigger";
            if (!AQI.Trigger)
                type = "cancel";

            if (AITOOL.triggerHttpClient == null)
            {
                AITOOL.triggerHttpClient = new ThrottledHttpClient(TimeSpan.FromSeconds(AppSettings.Settings.HTTPClientLocalTimeoutSeconds));  //new System.Net.Http.HttpClient();
                AITOOL.triggerHttpClient.Timeout = TimeSpan.FromSeconds(AppSettings.Settings.HTTPClientLocalTimeoutSeconds);

                //lets give it a user agent unique to this machine and product version...
                AssemblyName ASN = Assembly.GetExecutingAssembly().GetName();
                string Version = ASN.Version.ToString();
                ProductInfoHeaderValue PIH = new ProductInfoHeaderValue("AI_Tool_MANBAT_" + Global.GetMacAddress(), Version);
                AITOOL.triggerHttpClient.DefaultRequestHeaders.UserAgent.Add(PIH);
            }


            for (int i = 0; i < trigger_urls.Count; i++)
            {
                url = trigger_urls[i];

                if (url.IsStringBefore(";", ":"))
                {
                    //prm0 - object1, object2
                    //prm1 - soundfile.wav or URL
                    string objects = url.GetWord("", ";");
                    url = url.GetWord(";", "");
                    //make sure it is a matching object
                    //if (AITOOL.ArePredictionObjectsRelevant(objects, "TriggerURL", AQI.Hist.Predictions(), false) != ResultType.Relevant) ;

                    ClsRelevantObjectManager rom = new ClsRelevantObjectManager(objects, "TriggerURL", AQI.cam);

                    if (!AQI.Hist.IsNull() && rom.IsRelevant(AQI.Hist.Predictions(), false, out bool IgnoreImageMask, out bool IgnoreDynamicMask) != ResultType.Relevant)
                        continue;

                }
                else
                {
                    //Log($"Debug: No conditional objects found in URL: {url}");
                }

                Stopwatch sw = Stopwatch.StartNew();
                try
                {
                    Log($"Debug:   -> {type} URL is being triggered... {url}");

                    HttpResponseMessage response = await triggerHttpClient.GetAsync(url);

                    //If we get a null response it means the host+port was already in use.  In that case, we are just going to skip this URL call and call it good.
                    if (response == null)
                    {
                        Log($"Debug:   -> {type} URL called in {sw.ElapsedMilliseconds}ms: {url}, response: 'URL was in use, skipping.  Turn off 'queue actions' in camera settings to avoid this happening.'");
                    }
                    else
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            string content = await response.Content.ReadAsStringAsync();
                            Log($"Debug:   -> {type} URL called in {sw.ElapsedMilliseconds}ms: {url}, response: '{content.CleanString().Truncate()}'");
                        }
                        else
                        {
                            ret = false;
                            Log($"ERROR: {type}: In {sw.ElapsedMilliseconds}ms, got StatusCode='{response.StatusCode}', Reason='{response.ReasonPhrase}: Could not {type} URL '{url}', please check if correct");
                        }
                    }

                }
                catch (Exception ex)
                {
                    ret = false;
                    Log($"ERROR: {type}: In {sw.ElapsedMilliseconds}ms, Could not {type} Error='{ex.Msg()}', URL='{url}'");
                }

                if (AppSettings.Settings.ActionDelayMS >= 100)  //dont show for tiny delays
                    Log($"Debug: ...Applying 'ActionDelayMS' delay of {AppSettings.Settings.ActionDelayMS}ms.");

                await Task.Delay(AppSettings.Settings.ActionDelayMS); //very short wait between trigger events
            }


            return ret;


        }

        public async Task<bool> PushoverUpload(ClsTriggerActionQueueItem AQI)
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            bool ret = true;

            if (!string.IsNullOrEmpty(AppSettings.Settings.pushover_APIKey) && !string.IsNullOrEmpty(AppSettings.Settings.pushover_UserKey))
            {
                try
                {
                    //make sure it is a matching object
                    if (!AQI.Hist.IsNull() && AQI.cam.PushoverTriggeringObjects.IsRelevant(AQI.Hist.Predictions(), false, out bool IgnoreImageMask, out bool IgnoreDynamicMask) != ResultType.Relevant)
                        return true;

                    if (AppSettings.Settings.pushover_cooldown_seconds < 2)
                        AppSettings.Settings.pushover_cooldown_seconds = 2;  //force to be at least 2 seconds

                    DateTime now = DateTime.Now;

                    if (this.PushoverRetryTime.Read() == DateTime.MinValue || now >= this.PushoverRetryTime.Read())
                    {
                        double cooltime = Math.Round((now - this.last_Pushover_trigger_time.Read()).TotalSeconds, 4);
                        if (cooltime >= AppSettings.Settings.pushover_cooldown_seconds)
                        {
                            string title = "";
                            string message = "";
                            string device = "";

                            if (AQI.Trigger)
                            {

                                if (!string.IsNullOrEmpty(AQI.Text))
                                {
                                    if (AQI.Text.IndexOf("error", StringComparison.OrdinalIgnoreCase) >= 0)
                                        title = "Error";
                                    else
                                        title = AITOOL.ReplaceParams(AQI.cam, AQI.Hist, AQI.CurImg, AQI.cam.Action_pushover_title, Global.IPType.Path);

                                    message = AITOOL.ReplaceParams(AQI.cam, AQI.Hist, AQI.CurImg, AQI.Text, Global.IPType.Path);

                                }
                                else
                                {
                                    title = AITOOL.ReplaceParams(AQI.cam, AQI.Hist, AQI.CurImg, AQI.cam.Action_pushover_title, Global.IPType.Path);
                                    message = AITOOL.ReplaceParams(AQI.cam, AQI.Hist, AQI.CurImg, AQI.cam.Action_pushover_message, Global.IPType.Path);
                                }

                                device = AITOOL.ReplaceParams(AQI.cam, AQI.Hist, AQI.CurImg, AQI.cam.Action_pushover_device, Global.IPType.Path);
                            }
                            else  //TODO: Add cancel if requested
                            {
                                title = AITOOL.ReplaceParams(AQI.cam, AQI.Hist, AQI.CurImg, AQI.cam.Action_pushover_title, Global.IPType.Path);
                                message = AITOOL.ReplaceParams(AQI.cam, AQI.Hist, AQI.CurImg, AQI.cam.Action_pushover_message, Global.IPType.Path);
                                device = AITOOL.ReplaceParams(AQI.cam, AQI.Hist, AQI.CurImg, AQI.cam.Action_pushover_device, Global.IPType.Path);
                            }


                            List<string> titles = title.SplitStr("|");
                            List<string> messages = message.SplitStr("|");
                            List<string> devices = device.SplitStr("|");
                            List<string> sounds = AQI.cam.Action_pushover_Sound.SplitStr("|");
                            List<string> priorities = AQI.cam.Action_pushover_Priority.SplitStr("|");
                            List<string> times = AQI.cam.Action_pushover_active_time_range.SplitStr("|");

                            for (int t = 0; t < times.Count; t++)
                            {
                                string time = times.GetStrAtIndex(t);

                                if (Global.IsTimeBetween(now, time))
                                {

                                    if (AITOOL.pushoverClient == null)
                                        AITOOL.pushoverClient = new NPushover.Pushover(AppSettings.Settings.pushover_APIKey); //new PushoverClient.Pushover(, AppSettings.Settings.pushover_UserKey);

                                    string imginfo = "";
                                    if (AQI.CurImg != null && AQI.CurImg.IsValid())
                                    {
                                        imginfo = $"Attached Image: {Path.GetFileName(AQI.CurImg.image_path)}";
                                    }

                                    PushoverUserResponse response = null;

                                    Stopwatch sw = Stopwatch.StartNew();

                                    try
                                    {
                                        string pushtitle = titles.GetStrAtIndex(t);
                                        string pushmessage = messages.GetStrAtIndex(t);
                                        string pushsound = sounds.GetStrAtIndex(t);
                                        string pushdevice = devices.GetStrAtIndex(t);

                                        if (times.Count != priorities.Count)
                                            Log($"Warn: You should have the same number of Pushover priorities and times specified.");

                                        NPushover.RequestObjects.Priority pri = (NPushover.RequestObjects.Priority)Enum.Parse(typeof(NPushover.RequestObjects.Priority), priorities.GetStrAtIndex(t));

                                        //fix a bug where pushover expire was set to hours rather than seconds
                                        if (AQI.cam.Action_pushover_expire_seconds >= TimeSpan.FromHours(24).TotalSeconds)
                                            AQI.cam.Action_pushover_expire_seconds = 10800; //3 hours

                                        NPushover.RequestObjects.Message msg = new NPushover.RequestObjects.Message()
                                        {
                                            Title = pushtitle,
                                            Body = pushmessage,
                                            Timestamp = AQI.CurImg != null ? AQI.CurImg.TimeCreated : DateTime.Now,
                                            Priority = pri,
                                            Sound = pushsound,

                                            RetryOptions = pri == Priority.Emergency ? new RetryOptions
                                            {
                                                RetryEvery = TimeSpan.FromSeconds(AQI.cam.Action_pushover_retry_seconds),
                                                RetryPeriod = TimeSpan.FromSeconds(AQI.cam.Action_pushover_expire_seconds),
                                                CallBackUrl = !string.IsNullOrEmpty(AQI.cam.Action_pushover_retrycallback_url) ? new Uri(AQI.cam.Action_pushover_retrycallback_url) : null,
                                            } : null,
                                            SupplementaryUrl = !string.IsNullOrEmpty(AQI.cam.Action_pushover_SupplementaryUrl) ? new SupplementaryURL { Uri = new Uri(AQI.cam.Action_pushover_SupplementaryUrl), Title = "42" } : null,
                                        };

                                        sw.Restart();

                                        List<string> userkeys = AppSettings.Settings.pushover_UserKey.SplitStr("|,;");
                                        foreach (string userkey in userkeys)
                                        {
                                            Log($"Debug: Sending pushover message '{pushmessage}', priority '{pri.ToString()}', sound '{pushsound}' to user '{userkey}' {imginfo}...");
                                            response = await AITOOL.pushoverClient.SendPushoverMessageAsync(msg, userkey, pushdevice, AQI.CurImg);
                                            await Task.Delay(AppSettings.Settings.loop_delay_ms);
                                        }
                                        this.last_Pushover_trigger_time.Write(now);
                                        sw.Stop();
                                    }
                                    catch (Exception ex)
                                    {

                                        sw.Stop();
                                        ret = false;
                                        Log($"Error: Pushover: After {sw.ElapsedMilliseconds}ms, got: " + ex.Msg(), this.CurSrv, AQI.cam, AQI.CurImg);
                                    }

                                    if (response != null)
                                    {
                                        string rateinfo = "";
                                        if (response.RateLimitInfo != null)
                                        {
                                            rateinfo = $"(Monthly Limit={response.RateLimitInfo.Limit}, Remaining={response.RateLimitInfo.Remaining}, ResetDate={response.RateLimitInfo.Reset})";
                                        }

                                        if (response.IsOk)
                                        {
                                            ret = true;
                                            Log($"Debug: ...Pushover success in {sw.ElapsedMilliseconds}ms {rateinfo}");
                                        }
                                        else
                                        {
                                            string errs = "";
                                            if (response.HasErrors)
                                                errs = string.Join(";", response.Errors);
                                            ret = false;
                                            Log($"Error: Pushover response code={response.Status} in {sw.ElapsedMilliseconds}ms, Errs='{errs}' {rateinfo}");
                                        }
                                    }
                                    else
                                    {
                                        ret = false;
                                        Log($"Error: Pushover failed to return a response in {sw.ElapsedMilliseconds}ms?", this.CurSrv, AQI.cam, AQI.CurImg);
                                    }

                                    if (!ret)
                                        this.PushoverRetryTime.Write(DateTime.Now.AddSeconds(AppSettings.Settings.Pushover_RetryAfterFailSeconds));
                                    else
                                        this.PushoverRetryTime.Write(DateTime.MinValue);



                                }
                                else
                                {
                                    Log($"Debug: Skipping pushover because time is not between {time}");
                                }

                            }


                        }
                        else
                        {
                            //log that nothing was done
                            Log($"Debug:   Still in PUSHOVER cooldown. No image will be uploaded to Pushover.  ({cooltime} of {AppSettings.Settings.pushover_cooldown_seconds} seconds - See 'pushover_cooldown_seconds' in settings file)", this.CurSrv, AQI.cam, AQI.CurImg);

                        }
                    }
                    else
                    {
                        Log($"Debug:   Waiting {Math.Round((this.PushoverRetryTime.Read() - DateTime.Now).TotalSeconds, 1)} seconds ({this.PushoverRetryTime.Read()}) to retry PUSHOVER connection.  This is due to a previous pushover send error.", this.CurSrv, AQI.cam, AQI.CurImg);
                    }


                }
                catch (Exception ex)
                {

                    ret = false;
                    Log($"Error: {ex.Msg()}", this.CurSrv, AQI.cam, AQI.CurImg);
                }
            }
            else
            {
                ret = false;
                Log("Error: Pushover API key or User Key not set.");
            }


            return ret;
        }
        //send image to Telegram
        public async Task<bool> TelegramUpload(ClsTriggerActionQueueItem AQI)
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            bool ret = true;
            string lastchatid = "";

            if ((!string.IsNullOrWhiteSpace(AQI.cam.telegram_chatid) || AppSettings.Settings.telegram_chatids.Count > 0) && AppSettings.Settings.telegram_token != "")
            {
                //telegram upload sometimes fails
                Stopwatch sw = Stopwatch.StartNew();
                try
                {

                    if (AppSettings.Settings.telegram_cooldown_seconds < 2)
                        AppSettings.Settings.telegram_cooldown_seconds = 2;  //force to be at least 2 seconds

                    string Caption = "";

                    if (!string.IsNullOrEmpty(AQI.Text))
                        Caption = AITOOL.ReplaceParams(AQI.cam, AQI.Hist, AQI.CurImg, AQI.Text, Global.IPType.Path);
                    else
                        Caption = AITOOL.ReplaceParams(AQI.cam, AQI.Hist, AQI.CurImg, AQI.cam.telegram_caption, Global.IPType.Path);

                    //make sure it is a matching object
                    //if (AITOOL.ArePredictionObjectsRelevant(AQI.cam.telegram_triggering_objects, "Telegram", AQI.Hist.Predictions(), false) != ResultType.Relevant)
                    if (!AQI.Hist.IsNull())
                    {
                        if (AQI.cam.TelegramTriggeringObjects.IsRelevant(AQI.Hist.Predictions(), false, out bool IgnoreImageMask, out bool IgnoreDynamicMask) != ResultType.Relevant)
                        {
                            AITOOL.Log("Debug: No relevant objects, skipping TelegramUpload.");
                            return true;
                        }
                    }
                    else
                    {
                        AITOOL.Log("Warn: Hist is null?");
                    }


                    DateTime now = DateTime.Now;

                    if (this.TelegramRetryTime.Read() == DateTime.MinValue || now >= this.TelegramRetryTime.Read())
                    {
                        double cooltime = Math.Round((now - this.last_telegram_trigger_time.Read()).TotalSeconds, 4);
                        if (cooltime >= AppSettings.Settings.telegram_cooldown_seconds)
                        {
                            //in order to avoid hitting our limits when sending out mass notifications, consider spreading them over longer intervals, e.g. 8-12 hours. The API will not allow bulk notifications to more than ~30 users per second, if you go over that, you'll start getting 429 errors.

                            if (Global.IsTimeBetween(now, AQI.cam.telegram_active_time_range))
                            {
                                string chatid = "";
                                bool overrideid = (!string.IsNullOrWhiteSpace(AQI.cam.telegram_chatid));
                                if (overrideid)
                                    chatid = AQI.cam.telegram_chatid.Trim();
                                else
                                    chatid = AppSettings.Settings.telegram_chatids[0];

                                //upload image to Telegram servers and send to first chat
                                Log($"Debug:      uploading image to chat \"{chatid.ReplaceChars('*')}\"", this.CurSrv, AQI.cam, AQI.CurImg);
                                lastchatid = chatid;

                                Telegram.Bot.Types.Message message = await AITOOL.Telegram.SendPhotoAsync(chatid, AQI.CurImg.ToMemStream(), "", AQI.CurImg.image_path, Caption);

                                string file_id = message.Photo[0].FileId; //get file_id of uploaded image

                                if (!overrideid)
                                {
                                    //share uploaded image with all remaining telegram chats (if multiple chat_ids given) using file_id 
                                    foreach (string curchatid in AppSettings.Settings.telegram_chatids.Skip(1))
                                    {
                                        Log($"Debug:      uploading image to chat \"{curchatid.ReplaceChars('*')}\"...", this.CurSrv, AQI.cam, AQI.CurImg);
                                        lastchatid = curchatid;
                                        message = await AITOOL.Telegram.SendPhotoAsync(curchatid, null, file_id, "", Caption);
                                    }
                                }
                                ret = message != null;

                                this.last_telegram_trigger_time.Write(DateTime.Now);
                                this.TelegramRetryTime.Write(DateTime.MinValue);

                                if (AQI.IsQueued)
                                {
                                    //add a minimum delay if we are in a queue to prevent minimum cooldown error
                                    Log($"Debug: Waiting {AppSettings.Settings.telegram_cooldown_seconds} seconds (telegram_cooldown_seconds)...", this.CurSrv, AQI.cam, AQI.CurImg);
                                    await Task.Delay(TimeSpan.FromSeconds(AppSettings.Settings.telegram_cooldown_seconds));
                                }

                            }
                            else
                            {
                                Log($"Debug: Skipping Telegram because time is not between {AQI.cam.telegram_active_time_range}");
                            }
                        }
                        else
                        {
                            //log that nothing was done
                            Log($"Debug:   Still in TELEGRAM cooldown. No image will be uploaded to Telegram.  ({cooltime} of {AppSettings.Settings.telegram_cooldown_seconds} seconds - See 'telegram_cooldown_seconds' in settings file)", this.CurSrv, AQI.cam, AQI.CurImg);

                        }

                    }
                    else
                    {
                        Log($"Debug:   Waiting {Math.Round((this.TelegramRetryTime.Read() - DateTime.Now).TotalSeconds, 1)} seconds ({this.TelegramRetryTime}) to retry TELEGRAM connection.  This is due to a previous telegram send error.", this.CurSrv, AQI.cam, AQI.CurImg);
                    }


                }
                catch (ApiRequestException ex)  //current version only gives webexception NOT this exception!  https://github.com/TelegramBots/Telegram.Bot/issues/891
                {
                    bool se = AppSettings.Settings.send_telegram_errors;
                    AppSettings.Settings.send_telegram_errors = false;
                    Log($"ERROR: Could not upload image {AQI.CurImg.image_path} with chatid '{lastchatid.ReplaceChars('*')}' to Telegram: {ex.Msg()}", this.CurSrv, AQI.cam, AQI.CurImg);

                    if (!ex.Parameters.IsNull() && !ex.Parameters.RetryAfter.IsNull())
                    {
                        this.TelegramRetryTime.Write(DateTime.Now.AddSeconds(Convert.ToDouble(ex.Parameters.RetryAfter)));
                        Log($"...BOT API returned 'RetryAfter' value '{ex.Parameters.RetryAfter} seconds', so not retrying until {this.TelegramRetryTime.Read()}", this.CurSrv, AQI.cam, AQI.CurImg);
                    }

                    AppSettings.Settings.send_telegram_errors = se;
                    //store image that caused an error in ./errors/
                    if (!Directory.Exists("./errors/")) //if folder does not exist, create the folder
                    {
                        //create folder
                        DirectoryInfo di = Directory.CreateDirectory("./errors");
                        Log($"./errors/" + " dir created.");
                    }
                    //save error image
                    using (var image = await SixLabors.ImageSharp.Image.LoadAsync(AQI.CurImg.image_path))
                    {
                        await image.SaveAsync($"./errors/" + "TELEGRAM-ERROR-" + Path.GetFileName(AQI.CurImg.image_path) + ".jpg");
                    }
                    Global.UpdateLabel($"Can't upload error message to Telegram!", "lbl_errors");
                    ret = false;

                }
                catch (Exception ex)  //As of version 
                {
                    bool se = AppSettings.Settings.send_telegram_errors;
                    AppSettings.Settings.send_telegram_errors = false;
                    Log($"ERROR: Could not upload image {AQI.CurImg.image_path} to Telegram with chatid '{lastchatid.ReplaceChars('*')}': {ex.Msg()}", this.CurSrv, AQI.cam, AQI.CurImg);
                    this.TelegramRetryTime.Write(DateTime.Now.AddSeconds(AppSettings.Settings.Telegram_RetryAfterFailSeconds));
                    Log($"Debug: ...'Default' 'Telegram_RetryAfterFailSeconds' value was set to '{AppSettings.Settings.Telegram_RetryAfterFailSeconds}' seconds, so not retrying until {this.TelegramRetryTime}", this.CurSrv, AQI.cam, AQI.CurImg);
                    AppSettings.Settings.send_telegram_errors = se;
                    //store image that caused an error in ./errors/
                    if (!Directory.Exists("./errors/")) //if folder does not exist, create the folder
                    {
                        //create folder
                        DirectoryInfo di = Directory.CreateDirectory("./errors");
                        Log($"./errors/" + " dir created.");
                    }
                    //save error image
                    using (var image = await SixLabors.ImageSharp.Image.LoadAsync(AQI.CurImg.image_path))
                    {
                        await image.SaveAsync("./errors/" + "TELEGRAM-ERROR-" + Path.GetFileName(AQI.CurImg.image_path) + ".jpg");
                    }
                    Global.UpdateLabel($"Can't upload error message to Telegram!", "lbl_errors");
                    ret = false;

                }


                Log($"Debug: ...Finished in {sw.ElapsedMilliseconds}ms", this.CurSrv, AQI.cam, AQI.CurImg);

            }
            else
            {
                Log($"Error:  Telegram settings mis-configured. telegram_chatids.Count={AppSettings.Settings.telegram_chatids.Count} ({string.Join(",", AppSettings.Settings.telegram_chatids).ReplaceChars('*')}), telegram_token='{AppSettings.Settings.telegram_token.ReplaceChars('*')}'", this.CurSrv, AQI.cam, AQI.CurImg);
                ret = false;
            }

            return ret;

        }

        //send text to Telegram
        public async Task<bool> TelegramText(ClsTriggerActionQueueItem AQI)
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            bool ret = false;
            string lastchatid = "";
            if (AppSettings.Settings.telegram_chatids.Count > 0 && AppSettings.Settings.telegram_token != "")
            {
                try
                {

                    if (AppSettings.Settings.telegram_cooldown_seconds < 2)
                        AppSettings.Settings.telegram_cooldown_seconds = 2;  //force to be at least 2 second

                    string Caption = AITOOL.ReplaceParams(AQI.cam, AQI.Hist, AQI.CurImg, AQI.Text, Global.IPType.Path);

                    DateTime now = DateTime.Now;

                    if (this.TelegramRetryTime.Read() == DateTime.MinValue || now >= this.TelegramRetryTime.Read())
                    {
                        double cooltime = Math.Round((now - this.last_telegram_trigger_time.Read()).TotalSeconds, 4);
                        if (cooltime >= AppSettings.Settings.telegram_cooldown_seconds)
                        {
                            if (Global.IsTimeBetween(now, AQI.cam.telegram_active_time_range))
                            {
                                string chatid = "";
                                bool overrideid = (!string.IsNullOrWhiteSpace(AQI.cam.telegram_chatid));
                                if (overrideid)
                                    chatid = AQI.cam.telegram_chatid.Trim();
                                else
                                    chatid = AppSettings.Settings.telegram_chatids[0];


                                if (overrideid)
                                {
                                    lastchatid = chatid;
                                    Telegram.Bot.Types.Message msg = await AITOOL.Telegram.SendTextMessageAsync(chatid, Caption);

                                }
                                else
                                {
                                    foreach (string curchatid in AppSettings.Settings.telegram_chatids)
                                    {
                                        lastchatid = curchatid;
                                        Telegram.Bot.Types.Message msg = await AITOOL.Telegram.SendTextMessageAsync(curchatid, Caption);

                                    }

                                }
                                this.last_telegram_trigger_time.Write(DateTime.Now);
                                this.TelegramRetryTime.Write(DateTime.MinValue);

                                if (AQI.IsQueued)
                                {
                                    //add a minimum delay if we are in a queue to prevent minimum cooldown error
                                    Log($"Waiting {AppSettings.Settings.telegram_cooldown_seconds} seconds (telegram_cooldown_seconds)...", this.CurSrv, AQI.cam, AQI.CurImg);
                                    await Task.Delay(TimeSpan.FromSeconds(AppSettings.Settings.telegram_cooldown_seconds));
                                }

                                ret = true;
                            }
                            else
                            {
                                Log($"Debug: Skipping Telegram because time is not between {AQI.cam.telegram_active_time_range}");
                            }
                        }
                        else
                        {
                            //log that nothing was done
                            Log($"   Still in TELEGRAM cooldown. No image will be uploaded to Telegram.  ({cooltime} of {AppSettings.Settings.telegram_cooldown_seconds} seconds - See 'telegram_cooldown_seconds' in settings file)", this.CurSrv, AQI.cam, AQI.CurImg);

                        }

                    }
                    else
                    {
                        Log($"   Waiting {Math.Round((this.TelegramRetryTime.Read() - DateTime.Now).TotalSeconds, 1)} seconds ({this.TelegramRetryTime}) to retry TELEGRAM connection.  This is due to a previous telegram send error.", this.CurSrv, AQI.cam, AQI.CurImg);
                    }



                }
                catch (ApiRequestException ex)  //current version only gives webexception NOT this exception!  https://github.com/TelegramBots/Telegram.Bot/issues/891
                {
                    bool se = AppSettings.Settings.send_telegram_errors;
                    AppSettings.Settings.send_telegram_errors = false;
                    Log($"ERROR: Could not upload text '{AQI.Text}' with chatid '{lastchatid}' to Telegram: {ex.Msg()}", this.CurSrv, AQI.cam, AQI.CurImg);
                    this.TelegramRetryTime.Write(DateTime.Now.AddSeconds(Convert.ToDouble(ex.Parameters.RetryAfter)));
                    Log($"...BOT API returned 'RetryAfter' value '{ex.Parameters.RetryAfter} seconds', so not retrying until {this.TelegramRetryTime}", this.CurSrv, AQI.cam, AQI.CurImg);
                    AppSettings.Settings.send_telegram_errors = se;
                    Global.UpdateLabel($"Can't upload error message to Telegram!", "lbl_errors");

                }
                catch (Exception ex)
                {
                    bool se = AppSettings.Settings.send_telegram_errors;
                    AppSettings.Settings.send_telegram_errors = false;
                    Log($"ERROR: Could not upload image '{AQI.Text}' with chatid '{lastchatid}' to Telegram: {ex.Msg()}", this.CurSrv, AQI.cam, AQI.CurImg);
                    this.TelegramRetryTime.Write(DateTime.Now.AddSeconds(AppSettings.Settings.Telegram_RetryAfterFailSeconds));
                    Log($"...'Default' 'Telegram_RetryAfterFailSeconds' value was set to '{AppSettings.Settings.Telegram_RetryAfterFailSeconds}' seconds, so not retrying until {this.TelegramRetryTime}", this.CurSrv, AQI.cam, AQI.CurImg);
                    AppSettings.Settings.send_telegram_errors = se;
                    Global.UpdateLabel($"Can't upload error message to Telegram!", "lbl_errors");
                }

            }
            else
            {
                Log($"Error:  Telegram settings misconfigured. telegram_chatids.Count={AppSettings.Settings.telegram_chatids.Count} ({string.Join(",", AppSettings.Settings.telegram_chatids)}), telegram_token='{AppSettings.Settings.telegram_token}'", this.CurSrv, AQI.cam, AQI.CurImg);
            }

            return ret;
        }

        //Handle telegram return messages...



    }
}
