using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using NLog;
using NLog.Targets;
using NLog.Targets.Wrappers;

namespace AITool
{

    //this is for UI logging only, not file logging directly
    public class ClsLogManager:IDisposable
    {
        public bool Enabled { get; set; } = true;
        public List<ClsLogItm> Values { get; set; } = new List<ClsLogItm>();
        public ConcurrentQueue<ClsLogItm> RecentlyAdded { get; set; } = new ConcurrentQueue<ClsLogItm>();
        public ConcurrentQueue<ClsLogItm> RecentlyDeleted { get; set; } = new ConcurrentQueue<ClsLogItm>();
        public ThreadSafe.Integer ErrorCount { get; set; } = new ThreadSafe.Integer(0);
        public ClsLogItm LastLogItm = new ClsLogItm();
        public int MaxGUILogItems { get; set; } = 10000;
        public long LastLoadTimeMS { get; set; } = 0;
        //public List<string> LastLoadMessages = new List<string>();
        private ThreadSafe.Integer _LastIDX { get; set; } = new ThreadSafe.Integer(0);
        private bool _Store;
        private ThreadSafe.Integer _CurDepth = new ThreadSafe.Integer(0);
        private object _LockObj = new object();

        private LogLevel MinLevel = LogLevel.Debug;
        private string _Filename = "";
        private long _MaxSize = 0;
        private int _MaxAgeDays = 0;
        private string _LastSource = "None";
        private string _LastCamera = "None";
        private string _LastAIServer = "None";
        private string _LastImage = "None";

        private NLog.Logger NLogFileWriter = null;
        AsyncTargetWrapper NLogAsyncWrapper = null;


        public ClsLogManager(bool Store, string DefaultSource)
        {
            this._Store = Store;  //we wont store log entries when running as a service, its only for the GUI
            this._LastSource = DefaultSource;
            //this.UpdateNLog(MinLevel, Filename, MaxSize, MaxAgeDays, MaxGUILogItems);
        }

        public string GetCurrentLogFileName()
        {
            string fileName = null;

            if (LogManager.Configuration != null && LogManager.Configuration.ConfiguredNamedTargets.Count != 0)
            {
                Target target = LogManager.Configuration.FindTargetByName("NLogAsyncWrapper");
                if (target == null)
                {
                    throw new Exception("Could not find target named: " + "NLogAsyncWrapper");
                }

                FileTarget fileTarget = null;
                WrapperTargetBase wrapperTarget = target as WrapperTargetBase;

                // Unwrap the target if necessary.
                if (wrapperTarget == null)
                {
                    fileTarget = target as FileTarget;
                }
                else
                {
                    fileTarget = wrapperTarget.WrappedTarget as FileTarget;
                }

                if (fileTarget == null)
                {
                    throw new Exception("Could not get a FileTarget from " + target.GetType());
                }

                var logEventInfo = new LogEventInfo { TimeStamp = DateTime.Now };
                fileName = fileTarget.FileName.Render(logEventInfo);
            }
            else
            {
                throw new Exception("LogManager contains no Configuration or there are no named targets");
            }

            this._Filename = fileName; //refresh

            return fileName;
        }

        public async Task UpdateNLog(LogLevel MinLevel, string Filename, long MaxSize, int MaxAgeDays, int MaxGUILogItems)
        {

            try
            {
                this.MaxGUILogItems = MaxGUILogItems;

                bool needsupdating = this.NLogFileWriter == null || this.NLogAsyncWrapper == null || MinLevel != this.MinLevel || Filename != this._Filename || MaxSize != this._MaxSize || MaxAgeDays != this._MaxAgeDays;

                if (!needsupdating)
                    return;

                //if we change the logging level only, I dont want to re-initialize everything else...

                bool onlylevel = this.NLogFileWriter != null && this.NLogAsyncWrapper != null && MinLevel != this.MinLevel && Filename == this._Filename && MaxSize == this._MaxSize && MaxAgeDays == this._MaxAgeDays;

                if (onlylevel)
                {
                    this.MinLevel = MinLevel;
                    foreach (var rule in LogManager.Configuration.LoggingRules)
                    {
                        rule.EnableLoggingForLevel(this.MinLevel);
                    }

                    //Call to update existing Loggers created with GetLogger() or 
                    //GetCurrentClassLogger()
                    LogManager.ReconfigExistingLoggers();

                    this._Filename = this.GetCurrentLogFileName();

                    return;
                }

                if (this.NLogAsyncWrapper == null)
                    this.NLogAsyncWrapper = new AsyncTargetWrapper();

                this.MinLevel = MinLevel;
                this._MaxAgeDays = MaxAgeDays;
                this._MaxSize = MaxSize;

                // Targets where to log to: File and Console
                var FileTarget = new NLog.Targets.FileTarget("logfile"); // { FileName = AppSettings.Settings.LogFileName };
                string dir = Path.GetDirectoryName(Filename);
                string justfile = Path.GetFileNameWithoutExtension(Filename);
                //${basedir}/${shortdate}.log

                FileTarget.FileName = dir + "\\" + justfile + ".[${shortdate}].log";

                FileTarget.ArchiveAboveSize = MaxSize;
                FileTarget.ArchiveEvery = NLog.Targets.FileArchivePeriod.Day;
                FileTarget.MaxArchiveDays = MaxAgeDays;
                FileTarget.ArchiveNumbering = NLog.Targets.ArchiveNumberingMode.DateAndSequence;
                FileTarget.ArchiveOldFileOnStartup = false;
                FileTarget.ArchiveDateFormat = "yyyy-MM-dd";
                FileTarget.ArchiveFileName = dir + "\\" + justfile + ".[{#}].log.zip";


                FileTarget.KeepFileOpen = false;
                FileTarget.CreateDirs = true;
                FileTarget.Header = "Date|Level|Source|Func|AIServer|Camera|Image|Detail|Idx|Depth|Color|ThreadID";
                FileTarget.EnableArchiveFileCompression = true;
                FileTarget.Layout = "${message}";  //nothing fancy we are doing it ourselves

                this.NLogAsyncWrapper.WrappedTarget = FileTarget;
                this.NLogAsyncWrapper.QueueLimit = 100;
                this.NLogAsyncWrapper.OverflowAction = AsyncTargetWrapperOverflowAction.Discard;
                this.NLogAsyncWrapper.Name = "NLogAsyncWrapper";

                // Rules for mapping loggers to targets            
                NLog.Config.SimpleConfigurator.ConfigureForTargetLogging(this.NLogAsyncWrapper, this.MinLevel);
                NLog.LogManager.AutoShutdown = true;

                this.NLogFileWriter = NLog.LogManager.GetCurrentClassLogger();

                //this.NLogAsyncWrapper.EventQueueGrow
                //this.NLogAsyncWrapper.LogEventDropped

                if (this.Values.Count == 0)
                    this.GetCurrentLogFileName();

                //load the current log file into memory
                //await this.LoadLogFileAsync(this._Filename, true, false);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Msg());
            }

        }

        public List<ClsLogItm> GetRecentlyDeleted()
        {
            List<ClsLogItm> ret = new List<ClsLogItm>();
            while (!this.RecentlyDeleted.IsEmpty)
            {
                if (this.RecentlyDeleted.TryDequeue(out ClsLogItm CLI))
                    ret.Add(CLI);
            }
            return ret;
        }
        public List<ClsLogItm> GetRecentlyAdded()
        {
            List<ClsLogItm> ret = new List<ClsLogItm>();
            while (!this.RecentlyAdded.IsEmpty)
            {
                if (this.RecentlyAdded.TryDequeue(out ClsLogItm CLI))
                    ret.Add(CLI);
            }
            return ret;
        }
        public void Clear()
        {
            lock (this._LockObj)
            {
                NLog.LogManager.Flush();
                this.Values.Clear();
                this.LastLogItm = new ClsLogItm();
                this._LastIDX = 0;
                this._LastAIServer = "";
                this._LastCamera = "";
                this._LastImage = "";
                this.RecentlyAdded = new ConcurrentQueue<ClsLogItm>();
                this.RecentlyDeleted = new ConcurrentQueue<ClsLogItm>();
                this.ErrorCount = 0;
                this._Filename = this.GetCurrentLogFileName();

            }
        }


        public void Add(ClsLogItm CDI)
        {
            lock (this._LockObj)
            {
                this._LastIDX = CDI.Idx;
                this.LastLogItm = CDI;
                this.Values.Add(this.LastLogItm);
            }
        }

        public void AddRange(List<ClsLogItm> CDIList)
        {
            lock (this._LockObj)
            {
                foreach (ClsLogItm CDI in CDIList)
                {
                    this._LastIDX = CDI.Idx;
                    this.LastLogItm = CDI;
                    this.Values.Add(this.LastLogItm);
                }
            }
        }

        public void Enter([CallerMemberName()] string memberName = null)
        {
            this._CurDepth++;

            if (this._CurDepth > 10)
                this._CurDepth = 10;  //just in case something weird is going on

            if (this.MinLevel == LogLevel.Trace)
                this.Log($"---->ENTER {memberName}, Depth={this._CurDepth}", "Trace-Enter", "Trace-Enter", "", "", 0, LogLevel.Trace, DateTime.Now, memberName);

        }
        public void Exit([CallerMemberName()] string memberName = null, long timems = 0)
        {
            this._CurDepth.Decrement(0);
            if (this._CurDepth < 0)
                this._CurDepth = 0;

            if (this.MinLevel == LogLevel.Trace)
                this.Log($"----<EXIT {memberName}, Time={timems}ms, Depth={this._CurDepth}", "Trace-Exit", "Trace-Exit", "", "", 0, LogLevel.Trace, DateTime.Now, memberName);
        }
        public ClsLogItm Log(string Detail, string AIServer = "", string Camera = "", string Image = "", string Source = "", int Depth = 0, LogLevel Level = null, Nullable<DateTime> Time = default(DateTime?), [CallerMemberName()] string memberName = null)
        {
            if (!this.Enabled)
                return null;

            lock (this._LockObj)
            {
                this._LastIDX++;
                this.LastLogItm = new ClsLogItm();
                this.LastLogItm.Detail = Detail.Replace("|", ";");
                this.LastLogItm.Filename = Path.GetFileName(this._Filename);

                this.LastLogItm.ThreadID = Thread.CurrentThread.ManagedThreadId;

                if (Source == null || string.IsNullOrWhiteSpace(Source))
                    this.LastLogItm.Source = this._LastSource;
                else
                    this.LastLogItm.Source = Source;

                if (Camera == null || string.IsNullOrWhiteSpace(Camera))
                    this.LastLogItm.Camera = this._LastCamera;
                else
                {
                    this.LastLogItm.Camera = Camera;
                    if (!Camera.StartsWith("Trace-"))
                        this._LastCamera = Camera;
                }

                if (AIServer == null || string.IsNullOrWhiteSpace(AIServer))
                    this.LastLogItm.AIServer = this._LastAIServer;
                else
                {
                    this.LastLogItm.AIServer = AIServer;
                    if (!AIServer.StartsWith("Trace-"))
                        this._LastAIServer = AIServer;
                }

                if (Image == null || string.IsNullOrWhiteSpace(Image))
                    this.LastLogItm.Image = this._LastImage;
                else
                {
                    this.LastLogItm.Image = Path.GetFileName(Image);
                }

                if (Time.HasValue)
                    this.LastLogItm.Date = Time.Value;
                else
                    this.LastLogItm.Date = DateTime.Now;

                if (memberName == ".ctor")
                    memberName = "Constructor";

                this.LastLogItm.Func = memberName.Replace("AITool.", "");

                //deepstack messages spam us...
                bool DeepstackDebug = !string.Equals(this.LastLogItm.Func, "handleredisprocmsg", StringComparison.OrdinalIgnoreCase) || string.Equals(this.LastLogItm.Func, "handleredisprocmsg", StringComparison.OrdinalIgnoreCase) && AppSettings.Settings.deepstack_debug;

                if (!DeepstackDebug)
                    return this.LastLogItm;

                if (Level == null)
                {
                    if (this.LastLogItm.Detail.Within("fatal:", 6))
                    {
                        Level = LogLevel.Fatal;
                    }
                    else if (this.LastLogItm.Detail.Within("error:", 6))
                        Level = LogLevel.Error;
                    else if (this.LastLogItm.Detail.Within("warning:", 6) || this.LastLogItm.Detail.Within("warn:", 6))
                        Level = LogLevel.Warn;
                    else if (this.LastLogItm.Detail.Within("info:", 6))
                        Level = LogLevel.Info;
                    else if (this.LastLogItm.Detail.Within("debug:", 6))
                        Level = LogLevel.Debug;
                    else if (this.LastLogItm.Detail.Within("trace:", 6))
                        Level = LogLevel.Trace;
                    else
                        Level = LogLevel.Info;
                }

                bool HasError = false;

                //remove tags
                if (Level == LogLevel.Error)
                {
                    this.ErrorCount++;
                    HasError = true;
                    this.LastLogItm.Detail = Global.ReplaceCaseInsensitive(this.LastLogItm.Detail, "error:", "");
                }
                else if (Level == LogLevel.Fatal)
                {
                    this.ErrorCount++;
                    HasError = true;
                    this.LastLogItm.Detail = Global.ReplaceCaseInsensitive(this.LastLogItm.Detail, "fatal:", "");
                }
                else if (Level == LogLevel.Warn)
                {
                    this.ErrorCount++;
                    HasError = true;
                    this.LastLogItm.Detail = Global.ReplaceCaseInsensitive(this.LastLogItm.Detail, "warn:", "");
                    this.LastLogItm.Detail = Global.ReplaceCaseInsensitive(this.LastLogItm.Detail, "warning:", "");
                }
                else if (Level == LogLevel.Info)
                {
                    this.LastLogItm.Detail = Global.ReplaceCaseInsensitive(this.LastLogItm.Detail, "info:", "");
                }
                else if (Level == LogLevel.Trace)
                {
                    this.LastLogItm.Detail = Global.ReplaceCaseInsensitive(this.LastLogItm.Detail, "trace:", "");
                }
                else if (Level == LogLevel.Debug)
                {
                    this.LastLogItm.Detail = Global.ReplaceCaseInsensitive(this.LastLogItm.Detail, "debug:", "");
                }

                if (this.LastLogItm.Detail.TrimStart().StartsWith("{"))
                {
                    this.LastLogItm.Color = this.LastLogItm.Detail.GetWord("{", "}");
                    //strip out the color definition
                    this.LastLogItm.Detail = Global.ReplaceCaseInsensitive(this.LastLogItm.Detail, "{" + this.LastLogItm.Color + "}", "");
                }

                //clean out any whitespace
                //this.LastLogItm.Detail = this.LastLogItm.Detail.TrimStart();

                if (this._CurDepth + Depth > 0)
                    this.LastLogItm.Detail = new string(' ', (this._CurDepth + Depth * 2)) + this.LastLogItm.Detail;

                this.LastLogItm.Depth = this._CurDepth + Depth;
                this.LastLogItm.Level = Level;
                this.LastLogItm.Idx = this._LastIDX;


                if (this._Store)
                {
                    if (Level >= this.MinLevel)
                    {
                        this.Values.Add(this.LastLogItm);
                        this.RecentlyAdded.Enqueue(this.LastLogItm);
                        //keep the log list size down
                        if (this.Values.Count > this.MaxGUILogItems)
                        {
                            this.RecentlyDeleted.Enqueue(this.Values[0]);
                            this.Values.RemoveAt(0);
                        }
                    }
                }

                this.NLogFileWriter.Log(Level, this.LastLogItm.ToString());

                if (Debugger.IsAttached)
                    Debug.WriteLine(this.LastLogItm.ToDetailString());  //change from Console. to Debug. so it would appear in Immediate window?

                string itm = this.LastLogItm.ToString();

                if (AppSettings.Settings.IsNotNull())
                {
                    //Send telegram error message
                    if (AppSettings.Settings.send_telegram_errors &&
                        (HasError) &&
                        AppSettings.Settings.telegram_chatids.Count > 0 &&
                        AITOOL.TriggerActionQueue != null &&
                        !(itm.IndexOf("telegram", StringComparison.OrdinalIgnoreCase) >= 0) &&
                        !(itm.IndexOf("addtriggeraction", StringComparison.OrdinalIgnoreCase) >= 0))

                    {
                        //await TelegramText($"[{time}]: {text}"); //upload text to Telegram
                        Camera cam = AITOOL.GetCamera(this._LastCamera);
                        AITOOL.TriggerActionQueue.AddTriggerActionAsync(TriggerType.TelegramText, cam, null, null, true, false, null, this.LastLogItm.ToDetailString());
                    }

                    //Send pushover error message
                    if (AppSettings.Settings.send_pushover_errors &&
                        (HasError) &&
                        !string.IsNullOrEmpty(AppSettings.Settings.pushover_APIKey) &&
                        !string.IsNullOrEmpty(AppSettings.Settings.pushover_UserKey) &&
                        AITOOL.TriggerActionQueue != null &&
                        !(itm.IndexOf("pushover", StringComparison.OrdinalIgnoreCase) >= 0) &&
                        !(itm.IndexOf("addtriggeraction", StringComparison.OrdinalIgnoreCase) >= 0))
                    {
                        Camera cam = AITOOL.GetCamera(this._LastCamera);
                        AITOOL.TriggerActionQueue.AddTriggerActionAsync(TriggerType.Pushover, cam, null, null, true, false, null, this.LastLogItm.ToDetailString());
                    }

                }

                if (HasError)
                    NLog.LogManager.Flush();

            }

            return this.LastLogItm;
        }
        public void Sort()
        {
            lock (this._LockObj)
                this.Values = this.Values.OrderBy(c => c.Date).ThenBy(c => c.Idx).ToList();
        }

        public async Task<List<ClsLogItm>> LoadLogFileAsync(string Filename, bool Import, bool LimitEntries)
        {
            return await Task.Run(async () => this.LoadLogFile(Filename, Import, LimitEntries));
        }

        private List<ClsLogItm> LoadLogFile(string Filename, bool Import, bool LimitEntries)
        {
            List<ClsLogItm> ret = new List<ClsLogItm>();

            //this.LastLoadMessages.Clear();

            if (Import)
                this.Enabled = false;  //disable while we import

            string ExtractZipPath = "";
            string file = Path.GetFileName(Filename);

            Stopwatch sw = Stopwatch.StartNew();

            if (File.Exists(Filename))
            {

                try
                {
                    Global.UpdateProgressBar($"Loading {Path.GetFileName(Filename)}...", 1, 1, 1);

                    Global.WaitFileAccessResult result = Global.WaitForFileAccess(Filename, FileAccess.Read, FileShare.Read, 30000);
                    if (result.Success)
                    {
                        //if its a zip file, extract that puppy...
                        string NewFilename = "";

                        if (Filename.EndsWith("zip", StringComparison.OrdinalIgnoreCase))
                        {
                            ExtractZipPath = Path.Combine(Global.GetTempFolder(), "_" + file);
                            if (!Directory.Exists(ExtractZipPath))
                                Directory.CreateDirectory(ExtractZipPath);

                            //just extract the first file in the archive
                            using (ZipArchive archive = ZipFile.OpenRead(Filename))
                            {
                                foreach (ZipArchiveEntry entry in archive.Entries)
                                {
                                    string destinationPath = Path.GetFullPath(Path.Combine(ExtractZipPath, entry.FullName));
                                    entry.ExtractToFile(destinationPath, true);
                                    NewFilename = destinationPath;
                                    break;
                                }
                            }

                        }
                        else
                        {
                            NewFilename = Filename;
                        }

                        lock (this._LockObj)
                        {
                            file = Path.GetFileName(NewFilename);
                            int Invalid = 0;
                            bool OldFile = false;
                            using (StreamReader sr = new StreamReader(NewFilename))
                            {
                                int cnt = 0;
                                while (!sr.EndOfStream)
                                {
                                    cnt++;
                                    if (cnt > 1)
                                    {
                                        string line = sr.ReadLine();

                                        if (!OldFile && line.TrimStart().StartsWith("["))  //old log format, ignore
                                        {
                                            OldFile = true;
                                            this._LastIDX = 0;
                                            break;
                                        }

                                        if (!Import)
                                        {
                                            //just spit out a list of log lines
                                            ClsLogItm CLI = new ClsLogItm(line);
                                            if (CLI.Level != LogLevel.Off)  //off indicates invalid - for example the old log format
                                            {
                                                CLI.FromFile = true;
                                                CLI.Filename = file;
                                                ret.Add(CLI);
                                            }
                                            else
                                            {
                                                Invalid++;
                                                if (Invalid > 50)
                                                {
                                                    this.Log($"Error: Too many invalid lines ({Invalid}) stopping load.");
                                                    ret.Clear();
                                                    break;
                                                }
                                                else
                                                {
                                                    this.Log($"Debug: {Invalid} line(s) in log file '{line}'");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            //load into current log manager
                                            if (this._Store)
                                            {
                                                ClsLogItm CLI = new ClsLogItm(line);
                                                if (CLI.Level != LogLevel.Off)  //off indicates invalid - for example the old log format
                                                {
                                                    this.LastLogItm = CLI;
                                                    if (this.LastLogItm.Level >= this.MinLevel)
                                                    {
                                                        CLI.FromFile = true;
                                                        CLI.Filename = file;
                                                        this.Values.Add(this.LastLogItm);
                                                        this.RecentlyAdded.Enqueue(this.LastLogItm);
                                                        //keep the log list size down
                                                        if (LimitEntries && this.Values.Count > this.MaxGUILogItems)
                                                        {
                                                            this.RecentlyDeleted.Enqueue(this.Values[0]);
                                                            this.Values.RemoveAt(0);
                                                        }
                                                    }

                                                }
                                                else
                                                {
                                                    Invalid++;
                                                    if (Invalid > 50)
                                                    {
                                                        this.Log($"Error: Too many invalid lines ({Invalid}) stopping load.");
                                                        ret.Clear();
                                                        this._LastIDX = 0;
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        this.Log($"Debug: {Invalid} line(s) in log file '{line}'");
                                                    }

                                                }
                                            }
                                        }
                                    }
                                }

                            }

                            if (OldFile)
                            {
                                //rename it to keep it out of our way next time
                                try
                                {
                                    this.Log($"Debug: File was in the old log format, renaming to OLDLOGFORMAT. {NewFilename}");
                                    File.Move(NewFilename, NewFilename + ".OLDLOGFORMAT");
                                }
                                catch (Exception ex)
                                {
                                    this.Log($"Error: While renaming log to OLDLOGFORMAT, got: {ex.Message}");
                                }
                            }
                        }
                    }
                    else
                    {
                        this.Log($"Error: Gave up waiting for exclusive file access after {result.TimeMS}ms with {result.ErrRetryCnt} retries for {Filename}");
                    }

                    if (Directory.Exists(ExtractZipPath))
                        Directory.Delete(ExtractZipPath, true);



                }
                catch (Exception ex)
                {

                    this.Log($"Error: {ex.Msg()}");
                }


            }

            if (Import)
                this.Enabled = true;  //enable after we import

            Global.UpdateProgressBar($"", 0, 0, 0);

            this.LastLoadTimeMS = sw.ElapsedMilliseconds;

            return ret;
        }

        public void Dispose()
        {
            NLog.LogManager.Flush();
            ((IDisposable)this.NLogAsyncWrapper).Dispose();
        }
    }

}
