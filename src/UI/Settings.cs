using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

using Newtonsoft.Json;

using static AITool.AITOOL;

namespace AITool
{

    public static class AppSettings
    {
        public static ClsSettings Settings = new ClsSettings();
        public static bool AlreadyRunning = false;
        public static string LastShutdownState = "";
        public static string LastLogEntry = "";
        private static string LastSettingsJSON = "";
        private static ClsDeepstackDetection ThreadLock = new ClsDeepstackDetection();
        private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        private static ThreadSafe.DateTime _LastSaveTime = new ThreadSafe.DateTime(DateTime.MinValue, AppSettings.Settings.DateFormat);
        private static ThreadSafe.Boolean _AlreadySaving = new ThreadSafe.Boolean(false);
        public class ClsSettings
        {
            [JsonIgnore]
            public string SettingsFileName = ""; //Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location) + ".Settings.JSON");
            [JsonIgnore]
            public string LogFileName = ""; //Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location) + ".LOG");
            public string CustomLogFilePath = ""; //Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location) + ".LOG");
            public string FacesPath = "";
            [JsonIgnore]
            public string HistoryFileName = ""; //Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cameras\\history.csv");
            [JsonIgnore]
            public string HistoryDBFileName = ""; //Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location) + ".Database.SQLITE3");

            public bool SettingsPortable = true;  //portable means the settings stay under EXE folder, otherwise they get dropped in %appdata%
            public string telegram_token = "";
            public double telegram_cooldown_minutes = -1;  //Default to no more often than 5 seconds.   In minutes (How many minutes must have passed since the last detection. Used to separate event to ensure that every event only causes one telegram message.)
            public int telegram_cooldown_seconds = 5;
            public bool telegram_monitor_commands = true;
            public int Telegram_RetryAfterFailSeconds = 300;  //default to 5 minutes if telegram exception
            public string input_path = "";
            public bool input_path_includesubfolders = false;
            public List<string> telegram_chatids = new List<string>();
            public bool log_everything = false;
            public string LogLevel = "Debug";
            [JsonProperty("send_errors")]
            public bool send_telegram_errors = true;
            public bool send_pushover_errors = true;

            public bool startwithwindows = false;
            public int close_instantly = -1;
            public List<Camera> CameraList = new List<Camera>();
            public string deepstack_url = "";
            public string deepstack_adminkey = "";
            public string deepstack_apikey = "";
            public string deepstack_installfolder = "C:\\DeepStack";
            public string deepstack_port = "81";
            public string deepstack_mode = "Medium";
            public string deepstack_customModelPath = "";
            public string deepstack_customModelName = "";
            public string deepstack_customModelPort = "82";
            public string deepstack_customModelMode = "Medium";  //might be case sensitive
            public bool deepstack_stopbeforestart = true;
            public bool deepstack_urls_are_queued = true;
            public bool deepstack_autostart = false;
            public bool deepstack_autoadd = true;
            public bool deepstack_debug = false;
            public bool deepstack_highpriority = true;
            public bool deepstack_sceneapienabled = false;
            public bool deepstack_faceapienabled = false;
            public bool deepstack_detectionapienabled = false;
            public bool deepstack_customModelApiEnabled = false;
            public bool deepstack_autorestart = true;
            public int deepstack_autorestart_fail_count = 3;  //this many fails in a row will trigger restart
            public double deepstack_autorestart_minutes_between_restart_attempts = 10.0;

            public int loop_delay_ms = 50;  //the small delay applied in a loop after a failed file access check
            public bool SettingsValid = false;
            public int MaxLogFileAgeDays = 14;
            public long MaxLogFileSize = ((1024 * 1024) * 10);  //10mb in bytes

            public int MaxImageQueueSize = 100;
            public int MaxActionQueueSize = 100;
            public double MaxImageQueueTimeMinutes = 30;  //Take an image out of the queue if it sits in there over this time
            public int MaxQueueItemRetries = 5;  //will be disabled if fails this many times - Also applies to individual image failures
            public int URLResetAfterDisabledMinutes = 60;  //If any AI/Deepstack URL's have been disabled for over this time, all URLs will be reset to try again
            public int MinSecondsBetweenFailedURLRetry = 30;   //if a URL has failed, dont retry try more often than xx seconds
            public int MaxErrorsInARowBeforeDisable = 60;
            [JsonProperty("HTTPClientTimeoutSeconds")]
            public int HTTPClientLocalTimeoutSeconds = 120;    //httpclient.timeout - https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient.timeout?view=netcore-3.1
            public int HTTPClientRemoteTimeoutSeconds = 120;    //httpclient.timeout - https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient.timeout?view=netcore-3.1

            public int RectRelevantColorAlpha = 150;    //255=solid, 127 half transparent
            public int RectIrrelevantColorAlpha = 150;
            public int RectMaskedColorAlpha = 150;

            public int RectBorderWidth = 3;

            public int RectDetectionTextSize = 14;
            public string RectDetectionTextFont = "Segoe UI";

            public string DefaultFont = "Segoe UI, 8.25pt";

            public System.Drawing.Color RectDetectionTextBackColor = System.Drawing.Color.Gainsboro;  //a magic color randomly picked that means "Use Relevant or Irrelevant colors"
            public System.Drawing.Color RectDetectionTextForeColor = System.Drawing.Color.Black;
            public System.Drawing.Color RectRelevantColor = System.Drawing.Color.Red;
            public System.Drawing.Color RectIrrelevantColor = System.Drawing.Color.Silver;
            public System.Drawing.Color RectMaskedColor = System.Drawing.Color.DarkSlateGray;

            public string image_copy_folder = "";

            public string mqtt_serverandport = "mqtt:1883";
            public string mqtt_username = "user";
            public bool mqtt_UseTLS = false;
            public string mqtt_password = "password";
            public string mqtt_clientid = "AITool";
            public string mqtt_LastWillTopic = "AITool/status";
            public string mqtt_LastWillPayload = "Offline";
            public string mqtt_OnlinePayload = "Online";


            public string pushover_APIKey = "";
            public string pushover_UserKey = "";
            public int pushover_cooldown_seconds = 5;
            public int Pushover_RetryAfterFailSeconds = 300;  //default to 5 minutes if telegram exception

            public bool Autoscroll_log = true;
            public bool log_mnu_Filter = true;
            public bool log_mnu_Highlight = false;
            public int MaxGUILogItems = 5000; //makes to slow to work with if too high
            public string DateFormat = "yyyy-MM-dd hh:mm:ss"; //"dd.MM.yy, HH:mm:ss";
            public string DisplayPercentageFormat = "({0:0}%)";
            public int TimeBetweenListRefreshsMS = 5000;

            public bool HistoryShowMask = true;
            public bool HistoryShowObjects = true;
            public bool HistoryOnlyDisplayRelevantObjects = true;
            public bool HistoryFollow = true;
            public bool HistoryAutoRefresh = true;
            public bool HistoryStoreFalseAlerts = true;
            public bool HistoryStoreMaskedAlerts = true;
            public bool HistoryRestrictMinThresholdAtSource = true;
            public bool HistoryFilterRelevant = false;
            public bool HistoryFilterNoSuccess = false;
            public bool HistoryFilterPeople = false;
            public bool HistoryFilterAnimals = false;
            public bool HistoryFilterVehicles = false;
            public bool HistoryFilterSkipped = false;
            public bool HistoryFilterMasked = false;
            public bool HistoryMergeDuplicatePredictions = true;
            public int HistoryHoursBetweenCleaning = 24;

            public int MaxHistoryAgeDays = 14;

            public string ObjectPriority = "Ambulance, person, people, bear, fox, elephant, car, truck, pickup truck, SUV, van, bicycle, motorcycle, motorbike, bus, license plate, dog, horse, kangaroo, boat, train, zebra, giraffe, cow, pig, skunk, raccoon, sheep, rabbit, cat, bird, squirrel";
            public string ObjectsExcluded = "Airplane, apple, backpack, banana, baseball bat, baseball glove, bed, bench, book, bottle, bowl, broccoli, cake, carrot, cell phone, chair, clock, couch, cup, dining table, donut, fire hydrant, fork, frisbee, handbag, hot dog, keyboard, kite, laptop, microwave, mouse, orange, oven, parking meter, pizza, potted plant, refrigerator, remote, sandwich, scissors, sink, skateboard, skis, snowboard, spoon, sports ball, stop sign, suitcase, surfboard, table, teddy bear, tennis racket, tie, toilet, toothbrush, traffic light, tv, umbrella,vase, wine glass";
            [JsonIgnore]
            //make the following dictionary case insensitive, and do not store it
            public Dictionary<string, string> ObjectsExcludedDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            public string DefaultUserName = "Username";
            public string DefaultPasswordEncrypted = "";

            public string BlueIrisServer = "127.0.0.1";

            public string DOODSDetectorName = "default";
            public bool ScrewPutinTrumpAndWinniethePooh = true;
            public int FileSystemWatcherRetryOnErrorTimeMS = 300000;  //5 mins default

            public string AmazonAccessKeyId = "";
            public string AmazonSecretKey = "";
            public string AmazonRegionEndpoint = "";  //https://docs.aws.amazon.com/general/latest/gr/rande.html
            public int AmazonMaxLabels = 15;
            public int AmazonMinConfidence = 25;

            public string SightHoundAPIKey = "";   //https://accounts.sighthound.com/#/sighthound-cloud

            public int ActionCancelSeconds = 30;
            public int ActionDelayMS = 250;

            public bool MinimizeToTray = true;

            public int MaxWaitForAIServerMS = 5000;
            public bool MaxWaitForAIServerTimeoutError = true;

            public double LocalLatitude = 39.809734;  //mid USA lat long used as default value since BI also does this
            public double LocalLongitude = -98.555620;

            public List<ClsURLItem> AIURLList = new List<ClsURLItem>();

            public List<ClsImageAdjust> ImageAdjustProfiles = new List<ClsImageAdjust> { new ClsImageAdjust("Default") };

            public int SaveSettingsIntervalSeconds = 30;

            public bool Tick = false;
            public string TickImageAddedSoundFile = "tick.wav";

            //public bool TopMost = true;   //keep the main form on top of all other windows
        }

        public static ClsURLItem GetURL(string url = "", URLTypeEnum type = URLTypeEnum.Unknown)
        {
            ClsURLItem ret = null;
            foreach (var cu in Settings.AIURLList)
            {
                if (string.Equals(cu.url, url, StringComparison.OrdinalIgnoreCase) || cu.Type == type)
                {
                    return cu;
                }
            }
            return ret;

        }


        public static async Task<bool> SaveAsync(bool force = false)
        {
            //using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            double lastsecs = (DateTime.Now - _LastSaveTime).TotalSeconds;

            if (!force && lastsecs < AppSettings.Settings.SaveSettingsIntervalSeconds)
                return false;


            if (_AlreadySaving)
            {
                Log("Trace: *** Had to exit from save? ***");
                return false;
            }

            //await semaphoreSlim.WaitAsync();
            _AlreadySaving = true;

            bool Ret = false;
            try
            {
                //if (!Global.IsClassEqual(AppSettings.LastSettingsJSON, AppSettings.Settings))
                //{
                //keep a backup file in case of corruption
                if (await IsFileValidAsync(AppSettings.Settings.SettingsFileName))
                {
                    // Only create the backup if we are forcing settings to be saved (like a regular setting is changed)
                    if (force)
                    {
                        if (File.Exists(AppSettings.Settings.SettingsFileName + ".bak"))
                            File.Delete(AppSettings.Settings.SettingsFileName + ".bak");
                        if (File.Exists(AppSettings.Settings.SettingsFileName))
                            File.Move(AppSettings.Settings.SettingsFileName, AppSettings.Settings.SettingsFileName + ".bak");
                    }
                }
                else
                {
                    //file corrupt or doesnt exist
                    if (File.Exists(AppSettings.Settings.SettingsFileName))
                    {
                        Log("Error: Deleting corrupt settings file before resave: " + AppSettings.Settings.SettingsFileName);
                        File.Delete(AppSettings.Settings.SettingsFileName);
                    }
                }

                if (GetCamera("default", ReturnDefault: true) == null)
                {
                    //add a default camera
                    Camera cam = new Camera("Default");
                    Settings.CameraList.Add(cam);
                }
                //update threshold in all masks if changed during session
                foreach (Camera cam in AppSettings.Settings.CameraList)
                {
                    cam.UpdateCamera();
                }

                Settings.SettingsValid = true;
                String CurSettingsJSON = Global.WriteToJsonFile<ClsSettings>(AppSettings.Settings.SettingsFileName, Settings);

                if (!string.IsNullOrEmpty(CurSettingsJSON) && await IsFileValidAsync(AppSettings.Settings.SettingsFileName))
                {
                    Settings.SettingsValid = true;
                    Ret = true;
                    AppSettings.LastSettingsJSON = CurSettingsJSON;
                    //save a backup of settings to the registry since I've had a few times my raid array was going bad and I lost both backup and json files
                    Global.SaveRegSetting("BackupSettingsJSON", CurSettingsJSON);
                    Log($"Trace: JSON Settings saved to REGISTRY and {AppSettings.Settings.SettingsFileName}");
                }
                else
                {
                    Settings.SettingsValid = false;
                    Log($"Error: Failed to save Settings to {AppSettings.Settings.SettingsFileName}");
                }


                if (!AITOOL.FaceMan.IsNull())
                {
                    AITOOL.FaceMan.SaveFaces();
                }

                //}
                //else
                //{
                //    //does not need saving
                //    //Log("Settings have not changed, skipping save.");
                //    Ret = true;
                //    Settings.SettingsValid = true;
                //}


            }
            catch (Exception ex)
            {

                Log("Error: Could not save settings: " + ex.Msg());
            }

            if (!Ret)
            {
                try
                {
                    //Revert to backup copy if exists AND is not corrupt
                    if (await IsFileValidAsync(AppSettings.Settings.SettingsFileName + ".bak"))
                    {
                        Log("Error: Settings save failed, reverting to backup copy: " + AppSettings.Settings.SettingsFileName + ".bak");

                        if (File.Exists(AppSettings.Settings.SettingsFileName))
                            File.Delete(AppSettings.Settings.SettingsFileName);

                        File.Move(AppSettings.Settings.SettingsFileName + ".bak", AppSettings.Settings.SettingsFileName);
                    }
                    else
                    {
                        Log("Error: Settings save failed, Backup copy is not found or is corrupt: " + AppSettings.Settings.SettingsFileName + ".bak");
                    }

                }
                catch (Exception ex)
                {

                    Log("Error: Could not save settings: " + ex.Msg());
                }
            }

            //semaphoreSlim.Release();

            _AlreadySaving = false;

            _LastSaveTime = DateTime.Now;

            return Ret;
        }

        public static async Task<bool> IsFileValidAsync(string Filename, long MinSize = 800)
        {
            return await Task.Run(() => IsFileValidInternal(Filename, MinSize));
        }

        public static bool IsFileValidInternal(string Filename, long MinSize = 800)
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            bool Ret = false;
            try
            {
                if (File.Exists(Filename))
                {
                    FileInfo fi = new FileInfo(Filename);
                    if (fi.Length > MinSize)
                    {
                        //try to prevent multiple threads from erroring out writing the json file...
                        Global.WaitFileAccessResult result = Global.WaitForFileAccess(Filename, FileAccess.Read, FileShare.ReadWrite, 5000);
                        if (result.Success)
                        {
                            //check its contents, 0 bytes indicate corruption
                            string contents = File.ReadAllText(Filename);
                            if (!contents.Contains("\0"))
                            {
                                if (contents.TrimStart().StartsWith("{") && contents.TrimEnd().EndsWith("}"))
                                {
                                    Ret = true;
                                }
                                else
                                {
                                    Log($"Error: Settings file does not look like JSON (size={fi.Length} bytes): {Filename}");
                                }
                            }
                            else
                            {
                                Log($"Error: Settings file contains null bytes, corrupt: (size={fi.Length} bytes): " + Filename);
                            }
                        }
                        else
                        {
                            Log($"Error: Could not gain access to file for {result.TimeMS}ms - {Filename}");
                        }

                    }
                    else
                    {
                        Log($"Error: Settings file is too small at {fi.Length} bytes: {Filename}");
                    }
                }

                else
                {
                    Log("Settings file does not exist yet: " + Filename);
                }
            }
            catch (Exception ex)
            {

                Log($"Error: While validating settings file '{Filename}' got error '{ex.Message}'.");
            }
            return Ret;
        }

        public static void UpdateSettingsLocation()
        {
            try
            {
                //original:
                //public string SettingsFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location) + ".Settings.JSON");
                //public string LogFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location) + ".LOG");
                //public string HistoryDBFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location) + ".Database.SQLITE3");

                string OrigFolder = AppDomain.CurrentDomain.BaseDirectory.TrimEnd(@"\".ToCharArray());
                string OldFolder = !string.IsNullOrEmpty(Settings.SettingsFileName) ? Path.GetDirectoryName(Settings.SettingsFileName) : OrigFolder;
                string OldSettingsFile = Settings.SettingsFileName;

                string ExePath = AppDomain.CurrentDomain.BaseDirectory.TrimEnd(@"\".ToCharArray());
                string FullExePath = System.IO.Path.Combine(System.AppContext.BaseDirectory, System.Reflection.Assembly.GetEntryAssembly().GetName().Name + ".exe");

                //string OrigSettingsFilename = Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location) + ".Settings.JSON";
                string OrigSettingsFilename = Path.GetFileNameWithoutExtension(FullExePath) + ".Settings.JSON";
                string OrigLogFilename = Path.GetFileNameWithoutExtension(FullExePath) + ".LOG";
                string OrigHistoryDBFilename = Path.GetFileNameWithoutExtension(FullExePath) + ".Database.SQLITE3";

                //Just always look in original location for legacy import
                Settings.HistoryFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cameras\\history.csv");

                string NewFolder = "";
                string OrigSettingsFile = "";
                string NewSettingsFile = "";
                string NewLogFile = "";
                string NewHistoryFile = "";

                if (!Settings.SettingsPortable)
                {
                    //Set to appdata folder
                    NewFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AITOOL", "_Settings");
                }
                else
                {
                    //set to aitool subfolder
                    NewFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "_Settings");
                }


                bool NewDirExists = Directory.Exists(NewFolder);

                if (!NewDirExists)
                    Directory.CreateDirectory(NewFolder);

                OrigSettingsFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AITOOL.Settings.JSON");
                OldSettingsFile = Path.Combine(OldFolder, OrigSettingsFilename);
                NewSettingsFile = Path.Combine(NewFolder, OrigSettingsFilename);

                if (Settings.CustomLogFilePath.IsEmpty() || !Directory.Exists(Settings.CustomLogFilePath))
                    NewLogFile = Path.Combine(NewFolder, "Logs", OrigLogFilename);
                else
                    NewLogFile = Path.Combine(Settings.CustomLogFilePath, OrigLogFilename);


                NewHistoryFile = Path.Combine(NewFolder, OrigHistoryDBFilename);

                bool OrigSettingsFileExists = File.Exists(OrigSettingsFile);
                bool OldSettingsFileExists = File.Exists(OldSettingsFile);
                bool NewSettingFileExists = File.Exists(NewSettingsFile);
                bool ChangedLocation = !OldFolder.Equals(NewFolder, StringComparison.OrdinalIgnoreCase) || !NewSettingFileExists;

                Settings.SettingsFileName = NewSettingsFile;
                Settings.LogFileName = NewLogFile;
                Settings.HistoryDBFileName = NewHistoryFile;

                string OldCamFolder = Path.GetDirectoryName(Settings.HistoryFileName);
                string OldCamFolderWarn = Path.Combine(OldCamFolder, "_THIS_FOLDER_NOT_USED");
                if (Directory.Exists(OldCamFolder) && !File.Exists(OldCamFolderWarn))
                    File.WriteAllText(OldCamFolderWarn, "6, 28, 496, 8128 oh and 42");

                if (ChangedLocation)
                {

                    //make sure log folder exists

                    if (OldSettingsFileExists)
                    {
                        //this is only for when changing from portable and back... assume everything in \_settings subfolder and \_settings\Logs
                        Log($"Debug: Moving settings folder from old '{OldFolder}' to '{NewFolder}'...");

                        //Get from old camera folder first
                        Global.MoveFiles(Settings.HistoryFileName, NewFolder, "*.BMP|*.PNG", true);
                        Global.MoveFiles(OldFolder, NewFolder, "*.BMP|*.PNG", true);
                        Global.MoveFiles(OldFolder + "\\Logs", Settings.LogFileName, "AITOOL*.log|AITOOL.[*.ZIP", true);
                        Global.MoveFiles(OldFolder, Settings.LogFileName, "AITOOL*.log|AITOOL.[*.ZIP", true);
                        Global.MoveFiles(OldFolder, NewFolder, "*.JSON|*.JSON.BAK|*.BMP|*.PNG|*.SQLITE3|*.SQLITE3.BAK|*.CSV", true);
                    }
                    else if (OrigSettingsFileExists)
                    {
                        Log($"Debug: Moving settings folder from original '{OrigFolder}' to '{NewFolder}'...");
                        Global.MoveFiles(Settings.HistoryFileName, NewFolder, "*.BMP|*.PNG", true);
                        Global.MoveFiles(OrigFolder, NewFolder, "*.BMP|*.PNG", true);
                        Global.MoveFiles(OrigFolder, Settings.LogFileName, "AITOOL*.log|AITOOL.[*.ZIP", true);
                        Global.MoveFiles(OrigFolder + "\\Logs", Settings.LogFileName, "AITOOL*.log|AITOOL.[*.ZIP", true);
                        Global.MoveFiles(OrigFolder, NewFolder, "*.JSON|*.JSON.BAK|*.BMP|*.PNG|*.SQLITE3|*.SQLITE3.BAK|*.CSV", true);
                    }
                    else
                    {
                        Log($"Debug: Using settings folder '{NewFolder}'.");
                    }

                }
                else
                {
                    Log("Debug: Settings folder did not change.");
                }



            }
            catch (Exception ex)
            {

                Log("Error: " + ex.Msg());
            }

        }
        public static async Task<bool> LoadAsync()
        {
            await semaphoreSlim.WaitAsync();

            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            bool Ret = false;
            try
            {
                //////multiple threads may be trying to save at the same time
                ////lock (ThreadLock)
                //{
                Settings.SettingsValid = false;  //assume failure
                bool Resave = false;

                UpdateSettingsLocation();  //save to \settings folder or appdata\settings

                //get backup json from registry

                AppSettings.LastSettingsJSON = Global.GetRegSetting("BackupSettingsJSON", "");

                bool IsSettingsFileValid = await IsFileValidAsync(AppSettings.Settings.SettingsFileName);
                bool SettingsFileExists = File.Exists(AppSettings.Settings.SettingsFileName);
                bool IsSettingsBakFileValid = await IsFileValidAsync(AppSettings.Settings.SettingsFileName + ".bak");
                string LastRunPath = Global.GetRegSetting("LastRunPath", "");
                string LastSettingsFile = "";
                string LastSettingsFolder = "";

                if (!LastRunPath.IsEmpty() && !Directory.GetCurrentDirectory().EqualsIgnoreCase(LastRunPath) && Directory.Exists(LastRunPath))
                {
                    LastSettingsFolder = Path.Combine(LastRunPath, "_SETTINGS");
                    LastSettingsFile = Path.Combine(LastSettingsFolder, "AITOOL.Settings.JSON");
                    if (!File.Exists(LastSettingsFile))
                    {
                        LastSettingsFile = "";
                        LastSettingsFolder = "";
                    }
                }
                //read the old configuration file
                if (!IsSettingsFileValid && !IsSettingsBakFileValid && string.IsNullOrEmpty(AppSettings.LastSettingsJSON))
                {
                    //--------------------------------------------------------------------------------------------------------------------
                    //try to read in OLD aitool.exe.config or user.config files - they were
                    //unreliable because of strict versioning, etc
                    //
                    // NO NEED to add NEW settings to this area
                    //--------------------------------------------------------------------------------------------------------------------

                    List<FileInfo> filist = new List<FileInfo>();
                    FileInfo fi = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileName(Assembly.GetEntryAssembly().Location) + ".config"));
                    if (fi.Exists)
                        filist.Add(fi);
                    filist.AddRange(Global.GetFiles(Path.Combine(Environment.GetEnvironmentVariable("LOCALAPPDATA"), "WindowsFormsApp2"), "user.config"));
                    //sort by date
                    filist = filist.OrderByDescending((d) => d.LastWriteTime).ToList();

                    Log("First time load, reading old config file: " + filist[0].FullName);

                    XDocument xmlfile = XDocument.Load(filist[0].FullName);
                    //<configuration>
                    //    <userSettings>
                    //        <WindowsFormsApp2.Properties.Settings>
                    //            <setting name="telegram_token" serializeAs="String">
                    //                <value />
                    //            </setting>
                    //            <setting name="telegram_chatid" serializeAs="String">
                    //                <value />
                    //            </setting>
                    //            <setting name="input_path" serializeAs="String">
                    //                <value>D:\BlueIrisStorage\AIInput</value>
                    //            </setting>

                    IEnumerable<XElement> els = xmlfile.XPathSelectElements("/configuration/userSettings/WindowsFormsApp2.Properties.Settings/setting");
                    if (els == null || els.Count() == 0)
                        els = xmlfile.XPathSelectElements("/configuration/applicationSettings/WindowsFormsApp2.Properties.Settings/setting");

                    int cnt = 0;
                    foreach (XElement el in els)
                    {
                        string val = el.Value;
                        if (!string.IsNullOrEmpty(val))
                        {
                            if (el.ToString().Contains("telegram_token")) { Settings.telegram_token = val; cnt += 1; }
                            if (el.ToString().Contains("telegram_chatids")) { Settings.telegram_chatids = val.SplitStr(","); cnt += 1; }
                            if (el.ToString().Contains("input_path")) { Settings.input_path = val; cnt += 1; }
                            if (el.ToString().Contains("deepstack_url")) { Settings.deepstack_url = val; cnt += 1; }
                            if (el.ToString().Contains("log_everything")) { Settings.log_everything = Convert.ToBoolean(val); cnt += 1; }
                            if (el.ToString().Contains("send_errors")) { Settings.send_telegram_errors = Convert.ToBoolean(val); cnt += 1; }
                            if (el.ToString().Contains("close_instantly")) { Settings.close_instantly = Convert.ToInt32(val); cnt += 1; }

                        }
                    }

                    Resave = (cnt > 0);
                    Settings.SettingsValid = true;

                }
                else if (IsSettingsFileValid)
                {
                    //Load regular settings file
                    Log("Debug: Loading settings from " + AppSettings.Settings.SettingsFileName);
                    Settings = Global.ReadFromJsonFile<ClsSettings>(AppSettings.Settings.SettingsFileName);
                }
                else if (IsSettingsBakFileValid)
                {
                    //revert to backup if its good
                    Log("Error: Reverting to backup settings file: " + AppSettings.Settings.SettingsFileName + ".bak");
                    Log("Loading settings from " + AppSettings.Settings.SettingsFileName + ".bak");
                    Settings = Global.ReadFromJsonFile<ClsSettings>(AppSettings.Settings.SettingsFileName + ".bak");
                }
                else if (!string.IsNullOrEmpty(AppSettings.LastSettingsJSON) && SettingsFileExists)
                {
                    //revert to REGISTRY backup if its good AND the main settings file doesnt exist at all (so someone can delete the settings file to reset settings)
                    Log("Error: Reverting to REGISTRY backup settings...");
                    Settings = Global.SetJSONString<ClsSettings>(AppSettings.LastSettingsJSON);

                }
                else if (!LastSettingsFile.IsEmpty() && !SettingsFileExists && !Global.IsService)
                {
                    //revert to REGISTRY backup and look for the last folder to migrate settings from
                    string newfolder = Path.GetDirectoryName(Settings.SettingsFileName);
                    Log($"Debug: Previous settings found at {LastSettingsFolder} but not {newfolder}...");
                    if (System.Windows.Forms.MessageBox.Show($"Would you like to migrate settings from the last folder?\r\n\r\n{LastSettingsFolder}", "Migrate Settings?", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {
                        //copy the old settings folder:
                        Log($"Debug: Copying settings folder from original '{LastSettingsFolder}' to '{newfolder}'...");
                        Global.MoveFiles(LastSettingsFolder, newfolder, "*.*", true, true);
                        Log("Debug: Loading settings from " + AppSettings.Settings.SettingsFileName);
                        Settings = Global.ReadFromJsonFile<ClsSettings>(AppSettings.Settings.SettingsFileName);
                    }
                    else
                    {
                        Log("Debug: Did NOT import settings from folder.");
                    }

                }

                else if (!AppSettings.LastSettingsJSON.IsEmpty() && !SettingsFileExists && !Global.IsService)
                {
                    //revert to REGISTRY backup and look for the last folder to migrate settings from
                    string newfolder = Path.GetDirectoryName(Settings.SettingsFileName);
                    Log($"Debug: Previous settings found in the registry but not {newfolder}...");
                    if (System.Windows.Forms.MessageBox.Show($"Would you like to migrate settings from the last saved REGISTRY copy?", "Migrate Settings?", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {
                        //copy the old settings folder:
                        Log("Debug: Loading settings from previous REGISTRY...");
                        Settings = Global.SetJSONString<ClsSettings>(AppSettings.LastSettingsJSON);
                    }
                    else
                    {
                        Log("Debug: Did NOT import settings from previous registry copy.");
                    }

                }
                else
                {

                    //nothing valid
                    Log("Error: Settings file AND backup were missing or corrupt.");

                    if (File.Exists(AppSettings.Settings.SettingsFileName))
                        File.Delete(AppSettings.Settings.SettingsFileName);

                    if (File.Exists(AppSettings.Settings.SettingsFileName + ".bak"))
                        File.Delete(AppSettings.Settings.SettingsFileName + ".bak");

                }

                if (Settings != null)
                {

                    //The client identifier (ClientId) identifies each MQTT client that connects to an MQTT broker.
                    //The broker uses the ClientID to identify the client and the current state of the client.Therefore,
                    //this ID should be unique per client and broker. In MQTT 3.1.1 (the current standard), you can send
                    //an empty ClientId, if you don’t need a state to be held by the broker. The empty ClientID results
                    //in a connection without any state. In this case, the clean session flag must be set to true or
                    //the broker will reject the connection.

                    if (Settings.mqtt_clientid.EqualsIgnoreCase("aitool"))
                        Settings.mqtt_clientid = "AITool-BATMAN-" + Global.GetMacAddress();  //set the id to a unique number that should not *usually* change on a machine - the mac address of an active adapter
                                                                                             //but only set it once if the default clientid is set

                    if (Settings.telegram_cooldown_minutes > -1)
                    {
                        Settings.telegram_cooldown_seconds = Convert.ToInt32(Math.Round(TimeSpan.FromMinutes(Settings.telegram_cooldown_minutes).TotalSeconds, 0));
                        Settings.telegram_cooldown_minutes = -1;
                    }

                    UpdateSettingsLocation();  //save to \settings folder or appdata\settings


                    //load cameras the old way if needed
                    if (Settings.CameraList.Count == 0)
                    {
                        string camerafolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cameras");
                        Log("No cameras loaded in settings, trying to load old camera files from " + camerafolder);
                        List<FileInfo> files = Global.GetFiles(camerafolder, "*.txt"); //load all settings files in a string array
                                                                                       //Sort so more recent files are processed first - to make sure any dupes that are skipped are older
                                                                                       //I *think* this logic works?
                        files = files.OrderByDescending((d) => d.LastWriteTime).ToList();
                        //create a camera object for every camera settings file
                        int cnt = 0;
                        foreach (FileInfo file in files)
                        {

                            //check if camera with specified name or its prefix already exists. If yes, then abort.
                            bool fnd = false;
                            foreach (Camera c in AppSettings.Settings.CameraList)
                            {
                                if (string.Equals(c.Name, Path.GetFileNameWithoutExtension(file.FullName), StringComparison.OrdinalIgnoreCase))
                                {
                                    fnd = true;
                                }
                                else if (string.Equals(c.Prefix, System.IO.File.ReadAllLines(file.FullName)[2].Split('"')[1], StringComparison.OrdinalIgnoreCase))
                                {
                                    fnd = true;
                                }
                            }
                            if (!fnd)
                            {
                                cnt++;
                                Camera cam = new Camera(); //create new camera object
                                cam.ReadConfig(file.FullName); //read camera's config from file
                                AppSettings.Settings.CameraList.Add(cam); //add created camera object to CameraList
                            }
                            else
                            {
                                Log("Skipped duplicate camera: " + file);
                            }

                        }

                        if (cnt > 0)
                        {
                            Log($"...Loaded {cnt} camera files.");
                        }
                        else
                        {
                            Log($"...NO old camera txt files could be loaded.");
                        }

                        Resave = (cnt > 1);

                    }

                    Camera DefaultCam = GetCamera("default", ReturnDefault: true);
                    if (DefaultCam == null)
                    {
                        //add a default camera
                        Camera cam = new Camera("Default");
                        Settings.CameraList.Add(cam);
                    }
                    else if (DefaultCam.DefaultTriggeringObjects == null)
                    {
                        //replace the old default camera with a new one or else we have trouble with the triggering objects manager
                        Camera cam = new Camera(DefaultCam.Name);
                        Settings.CameraList.Remove(DefaultCam);
                        Settings.CameraList.Add(cam);
                    }

                    //make sure we have the updated default object list
                    if (!Settings.ObjectPriority.Has("suv") || !Settings.ObjectPriority.Has("van") || !Settings.ObjectPriority.Has("people") || !Settings.ObjectPriority.Has("fox") || !Settings.ObjectPriority.Has("raccoon") || !Settings.ObjectPriority.Has("license plate") || !Settings.ObjectPriority.Has("kangaroo"))
                        Settings.ObjectPriority = "Ambulance, person, people, bear, fox, elephant, car, truck, pickup truck, SUV, van, bicycle, motorcycle, motorbike, bus, license plate, dog, kangaroo, horse, boat, train, zebra, giraffe, cow, pig, skunk, raccoon, sheep, rabbit, cat, bird, squirrel";

                    //convert comma delimited list of objects stored in Settings.ObjectsExcluded to a dictionary
                    List<string> splt = Settings.ObjectsExcluded.SplitStr(",", true);
                    foreach (string obj in splt)
                    {
                        if (!Settings.ObjectsExcludedDict.ContainsKey(obj))
                            Settings.ObjectsExcludedDict.Add(obj, obj);
                    }


                    //make sure everything in the cameras look correct:
                    foreach (Camera cam in Settings.CameraList)
                    {
                        cam.UpdateCamera();
                    }


                    //sort the camera list:
                    AppSettings.Settings.CameraList = AppSettings.Settings.CameraList.OrderBy((d) => d.Name).ToList();

                    AITOOL.UpdateAIURLList(true);

                    //clean up image adjust list
                    List<ClsImageAdjust> iaps = AppSettings.Settings.ImageAdjustProfiles;

                    AppSettings.Settings.ImageAdjustProfiles.Clear();

                    for (int i = 0; i < iaps.Count; i++)
                    {
                        if (!AppSettings.Settings.ImageAdjustProfiles.Contains(iaps[i]))
                            Settings.ImageAdjustProfiles.Add(iaps[i]);
                    }

                    for (int i = 0; i < AppSettings.Settings.AIURLList.Count; i++)
                    {
                        AppSettings.Settings.AIURLList[i].Order = i + 1;
                        //if (!AITOOL.HasImageAdjustProfile(AppSettings.Settings.AIURLList[i].ImageAdjustProfile))
                        //{
                        //    AppSettings.Settings.AIURLList[i].ImageAdjustProfile = "Default";
                        //}
                    }

                    string facefile = Path.Combine(Settings.FacesPath, "Faces.JSON");

                    if (Settings.FacesPath.IsEmpty() || !Global.IsDirWritable(Settings.FacesPath, true) || !File.Exists(facefile))
                    {
                        string pth = Path.GetDirectoryName(Settings.SettingsFileName);
                        Settings.FacesPath = Path.Combine(pth, "FaceStorage");
                        facefile = Path.Combine(Settings.FacesPath, "Faces.JSON");
                    }


                    if (await IsFileValidAsync(facefile, 400))
                        AITOOL.FaceMan = Global.ReadFromJsonFile<ClsFaceManager>(facefile);
                    else
                        AITOOL.FaceMan = new ClsFaceManager();

                    if (AITOOL.FaceMan == null)
                        AITOOL.FaceMan = new ClsFaceManager();


                    Ret = true;
                }
                else
                {
                    Log("Error: Could not load settings?");
                }

                if (Resave)
                {
                    //we imported old settings, save them
                    SaveAsync(true);
                }

                //}

            }
            catch (Exception ex)
            {

                Log("Error: Could not save settings: " + ex.Msg());
            }
            finally
            {
                semaphoreSlim.Release();
            }


            return Ret;
        }


    }


}
