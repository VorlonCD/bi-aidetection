using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
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
        public class ClsSettings
        {
            [JsonIgnore]
            public string SettingsFileName = ""; //Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location) + ".Settings.JSON");
            [JsonIgnore]
            public string LogFileName = ""; //Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location) + ".LOG");
            [JsonIgnore]
            public string HistoryFileName = ""; //Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cameras\\history.csv");
            [JsonIgnore]
            public string HistoryDBFileName = ""; //Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location) + ".Database.SQLITE3");

            public bool SettingsPortable = true;  //portable means the settings stay under EXE folder, otherwise they get dropped in %appdata%
            public string telegram_token = "";
            public double telegram_cooldown_minutes = 0.0833333;  //Default to no more often than 5 seconds.   In minutes (How many minutes must have passed since the last detection. Used to separate event to ensure that every event only causes one telegram message.)
            public int Telegram_RetryAfterFailSeconds = 300;  //default to 5 minutes if telegram exception
            public string input_path = "";
            public bool input_path_includesubfolders = false;
            public List<string> telegram_chatids = new List<string>();
            public bool log_everything = false;
            public string LogLevel = "Debug";
            public bool send_errors = true;
            public bool startwithwindows = false;
            public bool minimizetotray = true;
            public int close_instantly = -1;
            public List<Camera> CameraList = new List<Camera>();
            public string deepstack_url = "127.0.0.1:81";
            public string deepstack_adminkey = "";
            public string deepstack_apikey = "";
            public string deepstack_installfolder = "C:\\DeepStack";
            public string deepstack_port = "81";
            public string deepstack_mode = "Medium";
            public bool deepstack_urls_are_queued = true;
            public bool deepstack_autostart = false;
            public bool deepstack_debug = false;
            public bool deepstack_highpriority = true;
            public bool deepstack_sceneapienabled = false;
            public bool deepstack_faceapienabled = false;
            public bool deepstack_detectionapienabled = false;
            public int file_access_delay = 50;
            public int retry_delay = 10;
            public bool SettingsValid = false;
            public int MaxLogFileAgeDays = 14;
            public long MaxLogFileSize = ((1024 * 1024) * 10);  //10mb in bytes
            public int MaxImageQueueSize = 100;
            public int MaxActionQueueSize = 100;
            public double MaxImageQueueTimeMinutes = 30;  //Take an image out of the queue if it sits in there over this time
            public int MaxQueueItemRetries = 5;  //will be disabled if fails this many times - Also applies to individual image failures
            public int MaxHistoryAgeDays = 14;
            public int URLResetAfterDisabledMinutes = 60;  //If any AI/Deepstack URL's have been disabled for over this time, all URLs will be reset to try again
            public int MinSecondsBetweenFailedURLRetry = 30;   //if a URL has failed, dont retry try more often than xx seconds
            public int HTTPClientTimeoutSeconds = 55;    //httpclient.timeout - https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient.timeout?view=netcore-3.1
            public int AIDetectionTimeoutSeconds = 60;  //cancelationsource task timeout timeout
                                                        //public int MaxDeepStackProcessTimeSeconds = 120;

            public int RectRelevantColorAlpha = 150;    //255=solid, 127 half transparent
            public int RectIrrelevantColorAlpha = 150;
            public int RectMaskedColorAlpha = 150;

            public int RectBorderWidth = 3;

            public int RectDetectionTextSize = 14;
            public string RectDetectionTextFont = "Segoe UI Semibold";

            public System.Drawing.Color RectRelevantColor = System.Drawing.Color.Red;
            public System.Drawing.Color RectIrrelevantColor = System.Drawing.Color.Silver;
            public System.Drawing.Color RectMaskedColor = System.Drawing.Color.DarkSlateGray;

            public string image_copy_folder = "";

            public string mqtt_serverandport = "mqtt:1883";
            public string mqtt_username = "user";
            public bool mqtt_UseTLS = false;
            public string mqtt_password = "password";
            public string mqtt_clientid = "AITool";

            public bool Autoscroll_log = true;
            public bool log_mnu_Filter = true;
            public bool log_mnu_Highlight = false;
            public int MaxGUILogItems = 5000; //makes to slow to work with if too high
            public string DisplayPercentageFormat = "({0:0}%)";
            public string DateFormat = "dd.MM.yy, HH:mm:ss";
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

            public string ObjectPriority = "person, bear, elephant, car, truck, bicycle, motorcycle, bus, dog, horse, boat, train, airplane, zebra, giraffe, cow, sheep, cat, bird";

            public string DefaultUserName = "Username";
            public string DefaultPasswordEncrypted = "";

            public string BlueIrisServer = "127.0.0.1";

            public string DOODSDetectorName = "default";

            public int FileSystemWatcherRetryOnErrorTimeMS = 300000;  //5 mins default

        }

        static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        public static async Task<bool> SaveAsync()
        {

            await semaphoreSlim.WaitAsync();

            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            bool Ret = false;
            try
            {
                //if (!Global.IsClassEqual(AppSettings.LastSettingsJSON, AppSettings.Settings))
                //{
                //keep a backup file in case of corruption
                if (await IsFileValidAsync(AppSettings.Settings.SettingsFileName))
                {
                    if (File.Exists(AppSettings.Settings.SettingsFileName + ".bak"))
                        File.Delete(AppSettings.Settings.SettingsFileName + ".bak");
                    if (File.Exists(AppSettings.Settings.SettingsFileName))
                        File.Move(AppSettings.Settings.SettingsFileName, AppSettings.Settings.SettingsFileName + ".bak");
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

                //update threshold in all masks if changed during session
                foreach (Camera cam in AppSettings.Settings.CameraList)
                {
                    cam.maskManager.Update(cam);
                }

                Settings.SettingsValid = true;
                String CurSettingsJSON = Global.WriteToJsonFile<ClsSettings>(AppSettings.Settings.SettingsFileName, Settings);

                if (!string.IsNullOrEmpty(CurSettingsJSON) && await IsFileValidAsync(AppSettings.Settings.SettingsFileName))
                {
                    Settings.SettingsValid = true;
                    Ret = true;
                    AppSettings.LastSettingsJSON = CurSettingsJSON;
                    //save a backup of settings to the registry since I've had a few times my raid array was going bad and I lost both backup and json files
                    Global.SaveSetting("BackupSettingsJSON", CurSettingsJSON);
                    Log($"Debug: JSON Settings saved to REGISTRY and {AppSettings.Settings.SettingsFileName}");
                }
                else
                {
                    Settings.SettingsValid = false;
                    Log($"Error: Failed to save Settings to {AppSettings.Settings.SettingsFileName}");
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

                Log("Error: Could not save settings: " + Global.ExMsg(ex));
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

                    Log("Error: Could not save settings: " + Global.ExMsg(ex));
                }
            }

            semaphoreSlim.Release();

            return Ret;
        }

        public static async Task<bool> IsFileValidAsync(string Filename)
        {
            return await Task.Run(() => IsFileValidInternal(Filename));
        }

        private static bool IsFileValidInternal(string Filename)
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            bool Ret = false;
            try
            {
                if (File.Exists(Filename))
                {
                    FileInfo fi = new FileInfo(Filename);
                    if (fi.Length > 800)
                    {
                        //try to prevent multiple threads from erroring out writing the json file...
                        Global.WaitFileAccessResult result = Global.WaitForFileAccess(Filename, FileSystemRights.Read, FileShare.ReadWrite, 5000);
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
                                Log("Error: Settings file contains null bytes, corrupt: (size={fi.Length} bytes)" + Filename);
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

                string OrigSettingsFilename = Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location) + ".Settings.JSON";
                string OrigLogFilename = Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location) + ".LOG";
                string OrigHistoryDBFilename = Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location) + ".Database.SQLITE3";

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

                NewLogFile = Path.Combine(NewFolder, "Logs", OrigLogFilename);
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
                        Global.MoveFiles(OldFolder, NewFolder, "*.JSON|*.JSON.BAK|*.BMP|*.PNG|*.SQLITE3|*.SQLITE3.BAK", true);
                    }
                    else if (OrigSettingsFileExists)
                    {
                        Log($"Debug: Moving settings folder from original '{OrigFolder}' to '{NewFolder}'...");
                        Global.MoveFiles(Settings.HistoryFileName, NewFolder, "*.BMP|*.PNG", true);
                        Global.MoveFiles(OrigFolder, NewFolder, "*.BMP|*.PNG", true);
                        Global.MoveFiles(OrigFolder, Settings.LogFileName, "AITOOL*.log|AITOOL.[*.ZIP", true);
                        Global.MoveFiles(OrigFolder + "\\Logs", Settings.LogFileName, "AITOOL*.log|AITOOL.[*.ZIP", true);
                        Global.MoveFiles(OrigFolder, NewFolder, "*.JSON|*.JSON.BAK|*.BMP|*.PNG|*.SQLITE3|*.SQLITE3.BAK", true);
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

                Log("Error: " + Global.ExMsg(ex));
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

                AppSettings.LastSettingsJSON = Global.GetSetting("BackupSettingsJSON", "");

                bool IsSettingsFileValid = await IsFileValidAsync(AppSettings.Settings.SettingsFileName);
                bool IsSettingsBakFileValid = await IsFileValidAsync(AppSettings.Settings.SettingsFileName + ".bak");
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
                            if (el.ToString().Contains("telegram_chatids")) { Settings.telegram_chatids = Global.Split(val, ","); cnt += 1; }
                            if (el.ToString().Contains("input_path")) { Settings.input_path = val; cnt += 1; }
                            if (el.ToString().Contains("deepstack_url")) { Settings.deepstack_url = val; cnt += 1; }
                            if (el.ToString().Contains("log_everything")) { Settings.log_everything = Convert.ToBoolean(val); cnt += 1; }
                            if (el.ToString().Contains("send_errors")) { Settings.send_errors = Convert.ToBoolean(val); cnt += 1; }
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
                else if (!string.IsNullOrEmpty(AppSettings.LastSettingsJSON) && !File.Exists(AppSettings.Settings.SettingsFileName))
                {
                    //revert to REGISTRY backup if its good AND the main settings file doesnt exist at all (so someone can delete the settings file to reset settings)
                    Log("Error: Reverting to REGISTRY backup settings...");
                    Settings = Global.SetJSONString<ClsSettings>(AppSettings.LastSettingsJSON);

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

                    UpdateSettingsLocation();  //save to \settings folder or appdata\settings

                    //Ive had a case where MaskManager was null/corrupt to double check:
                    foreach (Camera cam in Settings.CameraList)
                    {
                        if (cam.maskManager == null)
                        {
                            cam.maskManager = new MaskManager();
                            Log("Warning: Had to reset MaskManager for camera " + cam.name);
                        }

                        //update threshold in all masks if changed during session
                        cam.maskManager.Update(cam);

                        ///this was an old setting we dont want to use any longer, but pull it over if someone enabled it before
                        if (cam.trigger_url_cancels && !string.IsNullOrWhiteSpace(cam.cancel_urls_as_string))
                        {
                            cam.cancel_urls_as_string = cam.trigger_urls_as_string;
                            cam.trigger_url_cancels = false;
                        }

                        cam.trigger_urls = Global.Split(cam.trigger_urls_as_string, "\r\n|;,").ToArray();
                        cam.cancel_urls = Global.Split(cam.cancel_urls_as_string, "\r\n|;,").ToArray();

                        if (cam.Action_image_copy_enabled && !string.IsNullOrWhiteSpace(cam.Action_network_folder) && cam.Action_network_folder_purge_older_than_days > 0 && Directory.Exists(cam.Action_network_folder))
                        {
                            Log($"Debug: Cleaning out jpg files older than '{cam.Action_network_folder_purge_older_than_days}' days in '{cam.Action_network_folder}'...");

                            List<FileInfo> filist = new List<FileInfo>(Global.GetFiles(cam.Action_network_folder, "*.jpg"));
                            int deleted = 0;
                            int errs = 0;
                            foreach (FileInfo fi in filist)
                            {
                                if ((DateTime.Now - fi.LastWriteTime).TotalDays > cam.Action_network_folder_purge_older_than_days)
                                {
                                    try { fi.Delete(); deleted++; }
                                    catch { errs++; }
                                }
                            }
                            if (errs == 0)
                                Log($"Debug: ...Deleted {deleted} out of {filist.Count} files");
                            else
                                Log($"Debug: ...Deleted {deleted} out of {filist.Count} files with {errs} errors.");



                        }

                    }

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
                                if (string.Equals(c.name, Path.GetFileNameWithoutExtension(file.FullName), StringComparison.OrdinalIgnoreCase))
                                {
                                    fnd = true;
                                }
                                else if (c.prefix.ToLower() == System.IO.File.ReadAllLines(file.FullName)[2].Split('"')[1].ToLower())
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

                    //sort the camera list:
                    AppSettings.Settings.CameraList = AppSettings.Settings.CameraList.OrderBy((d) => d.name).ToList();


                    Ret = true;
                }
                else
                {
                    Log("Error: Could not load settings?");
                }

                if (Resave)
                {
                    //we imported old settings, save them
                    SaveAsync();
                }

                //}

            }
            catch (Exception ex)
            {

                Log("Error: Could not save settings: " + Global.ExMsg(ex));
            }
            finally
            {
                semaphoreSlim.Release();
            }


            return Ret;
        }

    }


}
