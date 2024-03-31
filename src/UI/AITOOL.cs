using Amazon;
using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Amazon.Runtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NLog;
using NPushover;
using OSVersionExtension;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telegram.Bot;
using Rectangle = System.Drawing.Rectangle;
using static AITool.Global;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace AITool
{
    public static class AITOOL
    {
        // =============================================================
        // ALL FUNCTIONS HERE THAT MAY EVENTUALLY BE USED IN A SERVICE
        // NO direct UI interaction
        // =============================================================

        public static DeepStack DeepStackServerControl = null;
        //public static RichTextBoxEx RTFLogger = null;
        //public static LogFileWriter LogWriter = null;
        //public static LogFileWriter HistoryWriter = null;

        public static BlueIrisInfo BlueIrisInfo = null;
        //public static List<ClsURLItem> DeepStackURLList = new List<ClsURLItem>();

        //keep track of timing
        //moving average will be faster for long running process with 1000's of samples
        public static MovingCalcs tcalc = new MovingCalcs(250, "Images", true);
        public static MovingCalcs fcalc = new MovingCalcs(250, "Images", true);
        public static MovingCalcs lcalc = new MovingCalcs(250, "Images", true);
        public static MovingCalcs icalc = new MovingCalcs(250, "Images", true);
        public static MovingCalcs qcalc = new MovingCalcs(250, "Images", true);
        public static MovingCalcs scalc = new MovingCalcs(250, "Queue Size", false);

        //public static ClsLogManager errors = new ClsLogManager();

        public static ClsLogManager LogMan = null;

        public static ClsFaceManager FaceMan = null;

        public static ConcurrentQueue<ClsImageQueueItem> ImageProcessQueue = new ConcurrentQueue<ClsImageQueueItem>();

        public static ConcurrentQueue<ClsLogItm> TmpHistQueue = new ConcurrentQueue<ClsLogItm>();  //For before the logger gets fully initialized

        //The sqlite db connection
        public static SQLiteHistory HistoryDB = null;
        public static ClsTriggerActionQueue TriggerActionQueue = null;

        public static object FileWatcherLockObject = new object();
        public static object ImageLoopLockObject = new object();

        //thread safe dictionary to prevent more than one file being processed at one time
        public static ConcurrentDictionary<string, DateTime> image_detection_dictionary = new ConcurrentDictionary<string, DateTime>();


        public static Dictionary<string, ClsFileSystemWatcher> watchers = new Dictionary<string, ClsFileSystemWatcher>();
        //public static ThreadSafe.Boolean AIURLSettingsChanged = new ThreadSafe.Boolean(true);


        public static ThreadSafe.Boolean IsClosing = new ThreadSafe.Boolean(false);
        public static ThreadSafe.Boolean IsLoading = new ThreadSafe.Boolean(true);
        public static ThreadSafe.Datetime LastImageBackupTime = new ThreadSafe.Datetime(DateTime.MinValue);

        public static CancellationTokenSource MasterCTS = new CancellationTokenSource();

        public static System.Timers.Timer FileSystemErrorCheckTimer = new System.Timers.Timer();

        public static MQTTClient mqttClient = new MQTTClient();

        public static Pushover pushoverClient = null;

        public static ClsTelegramMessages Telegram = null;
        public static ThrottledHttpClient triggerHttpClient = null;

        public static string srv = "";

        public static ThreadSafe.Integer AIURLListAvailableRefineServerCount = new ThreadSafe.Integer(0);

        public static ThreadSafe.Boolean CloseImmediately = new ThreadSafe.Boolean(false);

        public static ThreadSafe.Boolean ResetSettings = new ThreadSafe.Boolean(false);

        public static ThreadSafe.Boolean Restart = new ThreadSafe.Boolean(false);

        public static async Task InitializeBackend()
        {

            try
            {
                using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.



                //initialize log manager with basic settings so we can start getting output if needed
                if (Global.IsService)
                    srv = ".SERVICE.";
                else
                    srv = ".";

                string exe = $"AITOOLS{srv}EXE";

                //initialize logging as early as we can...  Write to the temp folder since we dont know the log location yet
                int TempDefSize = ((1024 * 1024) * 20); //20mb
                LogMan = new ClsLogManager(!Global.IsService, exe);

                await LogMan.UpdateNLog(LogLevel.Debug, Path.Combine(Global.GetTempFolder(true), Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location) + $"{srv}LOG"), TempDefSize, 120, AppSettings.Settings.MaxGUILogItems);

                //load settings
                await AppSettings.LoadAsync();

                //reset log settings if different:
                await LogMan.UpdateNLog(LogLevel.FromString(AppSettings.Settings.LogLevel), AppSettings.Settings.LogFileName, AppSettings.Settings.MaxLogFileSize, AppSettings.Settings.MaxLogFileAgeDays, AppSettings.Settings.MaxGUILogItems);


                Assembly CurAssm = Assembly.GetEntryAssembly();
                string AssemNam = CurAssm.GetName().Name;
                string AssemVer = CurAssm.GetName().Version.ToString();

                Log("");
                Log("");
                Log("");
                Log($"Starting {AssemNam} Version {AssemVer} built on {Global.RetrieveLinkerTimestamp()}");

                try  //just in case some weird issue comes up with older os version...
                {
                    OSVersionExt.VersionInfo vi = OSVersion.GetOSVersion();
                    OSVersionExtension.OperatingSystem ov = OSVersion.GetOperatingSystem();

                    Log($"Debug:   Installed NET Framework version '{Global.GetFrameworkVersion()}', Target version '{AppDomain.CurrentDomain.SetupInformation.TargetFrameworkName}'");
                    Log($"Debug:   Windows '{ov.ToString()}', version '{vi.Version.ToString()}' Release ID '{OSVersion.MajorVersion10Properties().ReleaseId}', 64Bit={OSVersion.Is64BitOperatingSystem}, Workstation={OSVersion.IsWorkstation}, Server={OSVersion.IsServer}, SERVICE={Global.IsService}");

                }
                catch (Exception ex)
                {

                    Log("Error: Problem getting OS version: " + ex.Msg());
                }


                if (AppSettings.AlreadyRunning)
                {
                    Log("*** Warning: Another instance is already running *** ");
                    Log(" --- Files will not be monitored from within this session ");
                    Log(" --- Log tab will not display output from service instance. You will need to directly open log file for that ");
                    Log(" --- Changes made here to settings will require that you stop/start the service ");
                }
                if (Global.IsAdministrator())
                {
                    Log("Debug: *** Running as administrator ***");
                }
                else
                {
                    Log("Debug: Not running as administrator.");
                }

                if (string.Equals(AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\'), Directory.GetCurrentDirectory().TrimEnd('\\'), StringComparison.OrdinalIgnoreCase))
                {
                    Log($"Debug: *** Start in/current directory is the same as where the EXE is running from: {Directory.GetCurrentDirectory()} ***");
                }
                else
                {
                    try
                    {
                        Log($"Debug: *** Changing Start in/current directory from '{Directory.GetCurrentDirectory().TrimEnd('\\')}' to '{AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\')}' ***");
                        Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
                    }
                    catch (Exception ex)
                    {

                        string msg = $"Error: The Start in/current directory is NOT the same as where the EXE is running from: \r\n{Directory.GetCurrentDirectory()}\r\n{AppDomain.CurrentDomain.BaseDirectory}";
                        Log(msg);
                        Log($"...this may prevent DLL files from loading from the wrong folder.  '{ex.Message}'");
                    }
                }

                Global.SaveRegSetting("LastRunPath", Directory.GetCurrentDirectory());

                //initialize blueiris info class to get camera names, clip paths, etc
                BlueIrisInfo = new BlueIrisInfo();

                await BlueIrisInfo.RefreshBIInfoAsync(AppSettings.Settings.BlueIrisServer);

                if (BlueIrisInfo.Result == BlueIrisResult.Valid)
                {
                    Log($"Debug: BlueIris path is '{BlueIrisInfo.AppPath}', with {BlueIrisInfo.Users.Count} users, {BlueIrisInfo.Cameras.Count} cameras and {BlueIrisInfo.ClipPaths.Count} clip folder paths configured.");
                    if (BlueIrisInfo.Users.Count > 0 && (string.IsNullOrEmpty(AppSettings.Settings.DefaultUserName) || string.Equals(AppSettings.Settings.DefaultUserName, "username", StringComparison.OrdinalIgnoreCase)))
                    {
                        AppSettings.Settings.DefaultUserName = BlueIrisInfo.Users[0].Name;
                        AppSettings.Settings.DefaultPasswordEncrypted = BlueIrisInfo.Users[0].Password.Encrypt();
                    }

                    UpdateLatLong();
                }
                else
                {
                    Log($"Debug: BlueIris not detected.");
                }


                //initialize the deepstack class - it collects info from running deepstack processes, detects install location, and
                //allows for stopping and starting of its service
                DeepStackServerControl = new DeepStack(AppSettings.Settings.deepstack_adminkey, AppSettings.Settings.deepstack_apikey, AppSettings.Settings.deepstack_mode, AppSettings.Settings.deepstack_sceneapienabled, AppSettings.Settings.deepstack_faceapienabled, AppSettings.Settings.deepstack_detectionapienabled, AppSettings.Settings.deepstack_port, AppSettings.Settings.deepstack_customModelPath, AppSettings.Settings.deepstack_stopbeforestart, AppSettings.Settings.deepstack_customModelName, AppSettings.Settings.deepstack_customModelPort, AppSettings.Settings.deepstack_customModelMode, AppSettings.Settings.deepstack_customModelApiEnabled);

                if (DeepStackServerControl.IsInstalled && AppSettings.Settings.deepstack_autostart)
                {
                    await DeepStackServerControl.StartDeepstackAsync();
                }

                //Load the database, and migrate any old csv lines if needed
                HistoryDB = new SQLiteHistory(AppSettings.Settings.HistoryDBFileName, AppSettings.AlreadyRunning);

                await HistoryDB.Initialize();

                TriggerActionQueue = new ClsTriggerActionQueue();


                await UpdateWatchers(false);

                FileSystemErrorCheckTimer.Elapsed += new System.Timers.ElapsedEventHandler(TimerCheckFileSystemWatchers);
                FileSystemErrorCheckTimer.Interval = AppSettings.Settings.FileSystemWatcherRetryOnErrorTimeMS;
                FileSystemErrorCheckTimer.Enabled = true;
                FileSystemErrorCheckTimer.Start();

                //Start the thread that watches for the file queue
                if (!AppSettings.AlreadyRunning)
                    Task.Run(ImageQueueLoop);


                Telegram = new ClsTelegramMessages();

                if (AppSettings.LastShutdownState.StartsWith("checkpoint") && !AppSettings.AlreadyRunning)
                    Log($"Error: Program did not shutdown gracefully.  Last log entry was '{AppSettings.LastLogEntry}', '{AppSettings.LastShutdownState}'");



            }
            catch (Exception ex)
            {

                Log("Error: " + ex.Msg());
            }

        }

        public static void UpdateLatLong()
        {
            //use blueiris lat/long setting if found, and not already set to something different in :
            if (BlueIrisInfo.Result == BlueIrisResult.Valid &&
                !Global.IsLatLongValid(AppSettings.Settings.LocalLatitude, AppSettings.Settings.LocalLongitude) &&
                Global.IsLatLongValid(BlueIrisInfo.Latitude, BlueIrisInfo.Longitude))  //default is middle of USA, assume that is not a valid lat/long
            {
                AppSettings.Settings.LocalLatitude = BlueIrisInfo.Latitude;
                AppSettings.Settings.LocalLongitude = BlueIrisInfo.Longitude;
            }



        }

        public static bool DrawAnnotation(Graphics g, ClsPrediction pred, double ImgWidth, double ImgHeight, double BoxWidth = 0, double BoxHeight = 0)
        {
            bool ret = false;

            try
            {

                if (BoxWidth == 0)
                    BoxWidth = ImgWidth;
                if (BoxHeight == 0)
                    BoxHeight = ImgHeight;

                string AnnoText = pred.ToString();

                bool Merge = false;

                if (AppSettings.Settings.HistoryOnlyDisplayRelevantObjects && pred.Result == ResultType.Relevant)
                    Merge = true;
                else if (!AppSettings.Settings.HistoryOnlyDisplayRelevantObjects)
                    Merge = true;

                if (Merge)
                {

                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    //http://csharphelper.com/blog/2014/09/understand-font-aliasing-issues-in-c/
                    g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

                    System.Drawing.Color color = new System.Drawing.Color();

                    double BorderWidth = AppSettings.Settings.RectBorderWidth;

                    if (pred.Result == ResultType.Relevant)
                    {
                        color = System.Drawing.Color.FromArgb(AppSettings.Settings.RectRelevantColorAlpha, AppSettings.Settings.RectRelevantColor);
                    }
                    else if (pred.Result == ResultType.DynamicMasked || pred.Result == ResultType.ImageMasked || pred.Result == ResultType.StaticMasked)
                    {
                        color = System.Drawing.Color.FromArgb(AppSettings.Settings.RectMaskedColorAlpha, AppSettings.Settings.RectMaskedColor);
                    }
                    else
                    {
                        color = System.Drawing.Color.FromArgb(AppSettings.Settings.RectIrrelevantColorAlpha, AppSettings.Settings.RectIrrelevantColor);
                    }

                    //Assume no scaling at first:
                    ///===================================================================================

                    System.Drawing.RectangleF rect = pred.GetRectangleF();

                    double xmin = pred.XMin;
                    double ymin = pred.YMin;
                    double xmax = pred.XMax;
                    double ymax = pred.YMax;

                    double sclxmin = pred.XMin;
                    double sclymin = pred.YMin;
                    double sclxmax = pred.XMax;
                    double sclymax = pred.YMax;

                    double TextSizePoints = AppSettings.Settings.RectDetectionTextSize;

                    ///===================================================================================
                    //check to see if we need to scale based on onscreen zoomed image from picturebox:
                    ///===================================================================================
                    if (ImgWidth != BoxWidth || ImgHeight != BoxHeight)
                    {
                        //these variables store the padding between image border and picturebox border
                        double absX = 0;
                        double absY = 0;

                        //because the sizemode of the picturebox is set to 'zoom', the image is scaled down
                        double scale = 1;

                        //Comparing the aspect ratio of both the control and the image itself.
                        if (ImgWidth / ImgHeight > BoxWidth / BoxHeight) //if the image is p.e. 16:9 and the picturebox is 4:3
                        {
                            scale = BoxWidth / ImgWidth; //get scale factor
                            absY = (BoxHeight - scale * ImgHeight) / 2; //padding on top and below the image
                        }
                        else //if the image is p.e. 4:3 and the picturebox is widescreen 16:9
                        {
                            scale = BoxHeight / ImgHeight; //get scale factor
                            absX = (BoxWidth - scale * ImgWidth) / 2; //padding left and right of the image
                        }

                        //2. inputted position values are for the original image size. As the image is probably smaller in the picturebox, the positions must be adapted. 
                        xmin = (scale * xmin) + absX;
                        xmax = (scale * xmax) + absX;
                        ymin = (scale * ymin) + absY;
                        ymax = (scale * ymax) + absY;

                        double sclWidth = xmax - xmin;
                        double sclHeight = ymax - ymin;

                        sclxmax = BoxWidth - (absX * 2);
                        sclymax = BoxHeight - (absY * 2);
                        sclxmin = absX;
                        sclymin = absY;

                        TextSizePoints = scale * TextSizePoints;

                        BorderWidth = scale * BorderWidth;

                        if (BorderWidth < 1)
                            BorderWidth = 1;

                        rect = new System.Drawing.RectangleF(xmin.ToFloat(),
                                             ymin.ToFloat(),
                                             sclWidth.ToFloat(),
                                             sclHeight.ToFloat());
                    }

                    ///===================================================================================

                    using (Pen pen = new Pen(color, BorderWidth.ToFloat()))
                    {
                        g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height); //draw rectangle
                    }

                    //we need this since people can change the border width in the json file
                    double halfbrd = BorderWidth / 2;

                    System.Drawing.SizeF TextSize = g.MeasureString(AnnoText, new Font(AppSettings.Settings.RectDetectionTextFont, TextSizePoints.ToFloat())); //finds size of text to draw the background rectangle

                    double x = xmin - halfbrd;
                    double y = ymax + halfbrd;

                    //adjust the x / width label so it doesnt go off screen
                    double EndX = x + TextSize.Width;
                    if (EndX > sclxmax)
                    {
                        //int diffx = x - sclxmax;
                        x = xmax - TextSize.Width + halfbrd;
                    }

                    if (x < sclxmin)
                        x = sclxmin;

                    if (x < 0)
                        x = 0;

                    //adjust the y / height label so it doesnt go off screen
                    double EndY = y + TextSize.Height;
                    if (EndY > sclymax)
                    {
                        //float diffy = EndY - sclymax;
                        y = ymax - TextSize.Height - halfbrd;
                    }


                    if (y < 0)
                        y = 0;

                    //object name text below rectangle
                    rect = new System.Drawing.RectangleF(x.ToFloat(),
                                                         y.ToFloat(),
                                                         BoxWidth.ToFloat(),
                                                         BoxHeight.ToFloat()); //sets bounding box for drawn text

                    Brush brush = new SolidBrush(color); //sets background rectangle color
                    if (AppSettings.Settings.RectDetectionTextBackColor != System.Drawing.Color.Gainsboro)
                    {
                        color = System.Drawing.Color.FromArgb(color.A, AppSettings.Settings.RectDetectionTextBackColor);
                        brush = new SolidBrush(color);
                    }

                    Brush forecolor = Brushes.Black;
                    if (AppSettings.Settings.RectDetectionTextForeColor != System.Drawing.Color.Gainsboro)
                        forecolor = new SolidBrush(AppSettings.Settings.RectDetectionTextForeColor);


                    g.FillRectangle(brush,
                                    x.ToFloat(),
                                    y.ToFloat(),
                                    TextSize.Width,
                                    TextSize.Height); //draw grey background rectangle for detection text

                    g.DrawString(AnnoText,
                                 new Font(AppSettings.Settings.RectDetectionTextFont, TextSizePoints.ToFloat()),
                                 forecolor,
                                 rect); //draw detection text

                    g.Flush(FlushIntention.Flush);

                    ret = true;

                }

            }
            catch (Exception ex)
            {

                Log($"Error: {ex.Msg()}");
            }


            return ret;

        }

        public static System.Drawing.Image CropImage(ClsImageQueueItem img, System.Drawing.Rectangle cropArea)
        {

            if (img.IsValid())
            {
                try
                {
                    DecoderOptions dc = new DecoderOptions();
                    using (SixLabors.ImageSharp.Image image = SixLabors.ImageSharp.Image.Load(img.ToMemStream()))  //, out IImageFormat format))
                    {
                        image.Mutate(i => i.Crop(SixLabors.ImageSharp.Rectangle.FromLTRB(cropArea.Left, cropArea.Top, cropArea.Right, cropArea.Bottom)));

                        using (MemoryStream ms = new MemoryStream())
                        {
                            image.Save(ms, image.Metadata.DecodedImageFormat);
                            System.Drawing.Image newimg = System.Drawing.Image.FromStream(ms);
                            return newimg;
                        }
                    }

                }
                catch (Exception ex)
                {

                    Log($"Error: {ex.Msg()}");
                }

            }

            return null;
            //using Bitmap bmpImage = new Bitmap(img);

            //if (cropArea.Right > bmpImage.Width || cropArea.Bottom > bmpImage.Height)
            //{
            //    cropArea.Intersect(new Rectangle(0, 0, bmpImage.Width, bmpImage.Height));
            //}

            //Bitmap bmpCrop = bmpImage.Clone(cropArea, bmpImage.PixelFormat);

            //return (Image)(bmpCrop);

        }
        //public static void Log(string Detail, string AIServer = "", Camera Camera = null, ClsImageQueueItem Image = null, string Source = "", int Depth = 0, LogLevel Level = null, Nullable<DateTime> Time = default(DateTime?), [CallerMemberName()] string memberName = null)
        //{
        //    string cam = Camera != null ? Camera.Name : "";
        //    string img = Image != null ? Image.image_path : "";
        //    Log(Detail, AIServer, cam, img, Source, Depth, Level, Time, memberName);
        //}
        //public static void Log(string Detail, bool fakeout = false, [CallerMemberName()] string memberName = null)
        //{

        //    Log(Detail, "", "", "", "", 0, null, null, memberName);
        //}

        //just an alias to make things easier
        [DebuggerStepThrough]
        public static void Log(string Detail, string AIServer = "", object Camera = null, object Image = null, string Source = "", int Depth = 0, LogLevel Level = null, Nullable<DateTime> Time = default(DateTime?), [CallerMemberName()] string memberName = null)
        {

            string cam = "";
            string img = "";

            if (Camera != null)
            {
                if (Camera is Camera)
                {
                    cam = ((Camera)Camera).Name;
                }
                else if (Camera is String)
                {
                    cam = Camera.ToString();
                }
                else
                {
                    cam = Camera.ToString();
                    //should not be here?
                }
            }

            if (Image != null)
            {
                if (Image is ClsImageQueueItem)
                {
                    img = ((ClsImageQueueItem)Image).image_path;
                }
                else if (Image is string)
                {
                    img = Image.ToString();
                }
                else
                {
                    img = Image.ToString();
                }
            }


            if (LogMan != null && LogMan.Enabled)
            {
                //flush any entries from before logman initialized
                while (!TmpHistQueue.IsEmpty)
                {
                    ClsLogItm cli;
                    if (TmpHistQueue.TryDequeue(out cli))
                        LogMan.Log(cli.Detail, cli.AIServer, cli.Camera, cli.Image, cli.Source, cli.Depth, cli.Level, cli.Date, cli.Func);
                }

                LogMan.Log(Detail, AIServer, cam, img, Source, Depth, Level, Time, memberName);
            }
            else
            {
                TmpHistQueue.Enqueue(new ClsLogItm(null, DateTime.Now, Source, memberName, AIServer, cam, img, Detail, 0, Depth, "", 0));
                //Console.WriteLine($"Error: Wrote to log before initialized? '{Detail}'");
            }
        }

        public static void UpdateAIURLs()
        {


            if (AppSettings.GetURL(type: URLTypeEnum.AWSRekognition_Objects) != null || AppSettings.GetURL(type: URLTypeEnum.AWSRekognition_Faces) != null) // || this.url.Equals("aws", StringComparison.OrdinalIgnoreCase) || this.url.Equals("rekognition", StringComparison.OrdinalIgnoreCase))
            {
                string error = AITOOL.UpdateAmazonSettings();

                if (!string.IsNullOrEmpty(error))
                {
                    AITOOL.Log($"Error: {error}");

                    if (error.IndexOf("endpoint", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        //hardcode the list for now:  https://docs.aws.amazon.com/general/latest/gr/rande.html
                        List<string> endpoints = new List<string>();
                        endpoints.Add("US East (N. Virginia)  \tus-east-1");
                        endpoints.Add("US East (Ohio)  \tus-east-2");
                        endpoints.Add("US West (N. California)  \tus-west-1");
                        endpoints.Add("US West (Oregon)  \tus-west-2");
                        endpoints.Add("Canada (Central)  \tca-central-1");
                        endpoints.Add("Europe (London)  \teu-west-2");
                        endpoints.Add("Europe (Frankfurt)  \teu-central-1");
                        endpoints.Add("Europe (Ireland)  \teu-west-1");
                        endpoints.Add("Europe (Milan)  \teu-south-1");
                        endpoints.Add("Europe (Paris)  \teu-west-3");
                        endpoints.Add("Europe (Stockholm)  \teu-north-1");
                        endpoints.Add("Africa (Cape Town)  \taf-south-1");
                        endpoints.Add("Middle East (Bahrain)  \tme-south-1");
                        endpoints.Add("South America (São Paulo)  \tsa-east-1");
                        endpoints.Add("China (Beijing)  \tcn-north-1");
                        endpoints.Add("China (Ningxia)  \tcn-northwest-1");
                        endpoints.Add("Asia Pacific (Hong Kong)  \tap-east-1");
                        endpoints.Add("Asia Pacific (Mumbai)  \tap-south-1");
                        endpoints.Add("Asia Pacific (Osaka-Local)  \tap-northeast-3");
                        endpoints.Add("Asia Pacific (Seoul)  \tap-northeast-2");
                        endpoints.Add("Asia Pacific (Singapore)  \tap-southeast-1");
                        endpoints.Add("Asia Pacific (Sydney)  \tap-southeast-2");
                        endpoints.Add("Asia Pacific (Tokyo)  \tap-northeast-1");

                        using (var form = new InputForm("Select Amazon AWS endpoint near you:", "Amazon AWS Endpoint", cbitems: endpoints))
                        {
                            var result = form.ShowDialog();
                            if (result == DialogResult.OK)
                            {
                                string region = "";
                                if (!string.IsNullOrEmpty(form.text))
                                {
                                    if (form.text.Contains("\t"))
                                    {
                                        region = form.text.GetWord("\t", "").Trim();
                                    }
                                    else if (form.text.Contains("-"))
                                    {
                                        region = form.text.Trim();
                                    }

                                }
                                if (string.IsNullOrEmpty(region))
                                {
                                    MessageBox.Show($"Error: No endpoint selected '{form.text}'");
                                }
                                else
                                {
                                    AppSettings.Settings.AmazonRegionEndpoint = region;
                                }
                            }
                        }
                    }

                    error = AITOOL.UpdateAmazonSettings();

                    if (!string.IsNullOrEmpty(error))
                    {
                        AITOOL.Log($"Error: {error}");
                        if (error.IndexOf("rootkey", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            MessageBox.Show(error, "Missing AWS credentials", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                }
            }

            if (AppSettings.GetURL(type: URLTypeEnum.SightHound_Person) != null || AppSettings.GetURL(type: URLTypeEnum.SightHound_Vehicle) != null)
            {
                if (string.IsNullOrWhiteSpace(AppSettings.Settings.SightHoundAPIKey))
                {
                    using (var form = new InputForm("Enter SightHound API Key", "SightHound API Key"))
                    {
                        var result = form.ShowDialog();
                        if (result == DialogResult.OK)
                        {
                            if (!string.IsNullOrEmpty(form.text) && form.text.Trim().Length > 30) //It looks like they are 36 chars
                            {
                                AppSettings.Settings.SightHoundAPIKey = form.text.Trim();
                            }
                            else
                            {
                                MessageBox.Show("Enter a valid key.");
                            }
                        }
                    }
                }
            }

            //let the image loop (running in another thread) know to recheck ai server url settings.
            //AIURLSettingsChanged.WriteFullFence(true);

            AITOOL.UpdateAIURLList(true);

        }

        public static string UpdateAmazonSettings()
        {

            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            if (AppSettings.GetURL(type: URLTypeEnum.AWSRekognition_Objects) == null && AppSettings.GetURL(type: URLTypeEnum.AWSRekognition_Faces) == null) // || this.url.Equals("aws", StringComparison.OrdinalIgnoreCase) || this.url.Equals("rekognition", StringComparison.OrdinalIgnoreCase))
                return "";

            string error = "";

            RegionEndpoint endpoint = null;
            try
            {
                if (!string.IsNullOrWhiteSpace(AppSettings.Settings.AmazonRegionEndpoint))
                    endpoint = RegionEndpoint.GetBySystemName(AppSettings.Settings.AmazonRegionEndpoint);
                else
                    error = "No AmazonRegionEndpoint set.";
            }
            catch (Exception ex)
            {
                error = $"Could not set Amazon region endpoint to '{AppSettings.Settings.AmazonRegionEndpoint}' (JSON setting=AmazonRegionEndpoint): {ex.Message}";
            }

            if (endpoint != null)
            {

                AITOOL.Log($"Debug: Amazon RegionEndpoint DisplayName={endpoint.DisplayName}, PartitionDnsSuffix={endpoint.PartitionDnsSuffix}, PartitionName={endpoint.PartitionName}, SystemName={endpoint.SystemName}");
                //always try to extract latest from rootkey.csv if found
                string pth = Path.Combine(Path.GetDirectoryName(AppSettings.Settings.SettingsFileName), "rootkey.csv");
                if (File.Exists(pth))
                {
                    string csv = File.ReadAllText(pth);
                    if (!string.IsNullOrWhiteSpace(csv))
                    {
                        AITOOL.Log($"Debug: Extracting AWSAccessKeyId and AWSSecretKey from {pth}");
                        if (csv.Contains(",") && !csv.Has("AWSAccessKeyId="))
                        {
                            //User name, Password,Access key ID,  Secret access key,          Console login link
                            //aitooluser,        ,XXXXXXXXXXXXX,  xxxxxxxxxxxxxxxxxxxxxxxxxx, https://xxxxxxxxx.signin.aws.amazon.com/console

                            List<string> lines = csv.SplitStr("\r\n", true);
                            if (lines.Count > 1)
                            {
                                List<string> header = lines[0].ToLower().SplitStr(",", false);
                                List<string> firstline = lines[0].SplitStr(",", false);
                                int idxID = header.IndexOf("access key id");
                                int idxSECRET = header.IndexOf("secret access key");
                                if (idxID > -1 && idxSECRET > -1)
                                {
                                    AppSettings.Settings.AmazonAccessKeyId = firstline[idxID];
                                    AppSettings.Settings.AmazonSecretKey = firstline[idxSECRET];
                                }
                                else
                                {
                                    error = $"Error: Could not find 'Access Key ID' or 'Secret Access key' columns in '{pth}'";
                                }
                            }
                            else
                            {
                                error = $"Error: Too few lines in '{pth}'";
                            }
                        }
                        else if (csv.Has("AWSAccessKeyId="))
                        {
                            //old format
                            string tid = csv.GetWord("AWSAccessKeyId=", "\r|\n");
                            string tsid = csv.GetWord("AWSSecretKey=", "\r|\n|");
                            if (!string.IsNullOrEmpty(tid) && tid != AppSettings.Settings.AmazonAccessKeyId)
                                AppSettings.Settings.AmazonAccessKeyId = tid;
                            if (!string.IsNullOrEmpty(tsid) && tsid != AppSettings.Settings.AmazonSecretKey)
                                AppSettings.Settings.AmazonSecretKey = tsid;

                        }
                        else
                        {
                            error = $"Error: File is an unknown format '{pth}'";
                        }

                    }
                    else
                    {
                        error = $"Error: Empty file '{pth}'";
                    }

                }
                else
                {
                    error = $"Could not find AWS credentials file '{pth}'";
                }

                if (string.IsNullOrWhiteSpace(AppSettings.Settings.AmazonAccessKeyId) || string.IsNullOrWhiteSpace(AppSettings.Settings.AmazonSecretKey))
                {
                    error = "Please download 'rootkey.csv' and place it in AITOOL _SETTINGS folder.  1) Sign up for AWS, 2) Create user 3) Export rootkey.csv when prompted.  https://docs.aws.amazon.com/rekognition/latest/dg/setting-up.html  Please note that you have to CREATE NEW in order to see rootkey.csv if one already exists: https://console.aws.amazon.com/iam/home#/security_credentials";
                }
            }
            else
            {
                error = error + "- Please close AITOOL and set 'AmazonRegionEndpoint' in AITOOL.SETTTINGS.JSON to a region code near you such as 'us-east-1': https://docs.aws.amazon.com/general/latest/gr/rande.html";
            }

            return error;

        }

        public static void UpdateAIURLList(bool Force = false)
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            AIURLListAvailableRefineServerCount.WriteFullFence(0);
            //double check all the URL's have a new httpclient
            foreach (ClsURLItem url in AppSettings.Settings.AIURLList)
            {
                url.Update(false);
                url.AITimeCalcs.UpdateDate(true);

                if (url.IsValid && url.UseAsRefinementServer)
                    AIURLListAvailableRefineServerCount.AtomicIncrementAndGet();

                if (url.Type != URLTypeEnum.AWSRekognition_Objects && url.Type != URLTypeEnum.AWSRekognition_Faces && url.HttpClient == null)
                {
                    url.HttpClient = new HttpClient();
                    url.HttpClient.Timeout = url.GetTimeout();
                }

            }

            //remove dupes
            List<ClsURLItem> newlist = new List<ClsURLItem>();
            foreach (ClsURLItem url in AppSettings.Settings.AIURLList)
            {
                if (!newlist.Contains(url))
                    newlist.Add(url);
            }
            AppSettings.Settings.AIURLList = newlist;



            //Check to see if we need to get updated URL list - In theory this should only happen once
            bool hasold = !string.IsNullOrEmpty(AppSettings.Settings.deepstack_url);
            if (((AppSettings.Settings.AIURLList.Count == 0 || Force) && hasold) || hasold)
            {
                int newcnt = 0;

                try
                {
                    Log("Debug: Updating/Resetting AI URL list...");
                    List<string> SpltURLs = AppSettings.Settings.deepstack_url.SplitStr("|;,");

                    //I want to reuse any object that already exists for the url but make sure to get the right order if it changes
                    Dictionary<string, ClsURLItem> tmpdic = new Dictionary<string, ClsURLItem>();

                    foreach (ClsURLItem url in AppSettings.Settings.AIURLList)
                    {
                        string ur = url.ToString().ToLower();
                        if (!tmpdic.ContainsKey(ur))
                        {
                            tmpdic.Add(ur, url);
                        }
                        else
                        {
                            Log($"Debug: ---- (duplicate url configured - {ur})");
                        }
                    }

                    AppSettings.Settings.AIURLList.Clear();


                    for (int i = 0; i < SpltURLs.Count; i++)
                    {
                        if (!SpltURLs[i].Contains(":"))
                        {
                            Log($"Error: Skipping old server name migration because it doesn't have a port: '{SpltURLs[i]}'");
                            continue;
                        }

                        ClsURLItem url = null;
                        try
                        {
                            url = new ClsURLItem(SpltURLs[i], i + 1, URLTypeEnum.Unknown);
                        }
                        catch (Exception ex) { Log($"Error: url='{SpltURLs[i]}': {ex.Msg()}"); }

                        if (url != null && url.IsValid)
                        {
                            //if it already exists, use it, otherwise add a new one
                            if (tmpdic.ContainsKey(url.ToString().ToLower()))
                            {
                                url = tmpdic[url.ToString().ToLower()];
                                AppSettings.Settings.AIURLList.Add(url);
                                url.Order = i + 1;
                                //url.InUse.WriteFullFence(false);
                                url.CurErrCount.WriteFullFence(0);
                                url.Enabled.WriteFullFence(true);
                                Log($"Debug: ----   #{url.Order}: Re-added known URL: '{url}'");
                            }
                            else
                            {
                                newcnt++;
                                AppSettings.Settings.AIURLList.Add(url);
                                Log($"Debug: ----   #{url.Order}: Added new URL: '{url}'");
                            }

                        }
                        else
                        {
                            Log($"Debug: ----   #{url.Order}: Skipped INVALID URL: '{SpltURLs[i]}'");

                        }
                    }

                }
                catch (Exception ex)
                {
                    Log($"Error: {ex.Msg()}");
                }

                Log($"Debug: ...{newcnt} new AI URL's migrated from old settings, with a total of {AppSettings.Settings.AIURLList.Count} AI URL's");

                AppSettings.Settings.deepstack_url = "";  //we are not going to use this any longer
                //AIURLSettingsChanged.WriteFullFence(false);

            }


            //add a default DeepStack server if none found
            //if (AppSettings.Settings.AIURLList.Count == 0)
            //{
            //    Log($"Debug: ----   Adding default Deepstack AI Server URL.");
            //    AppSettings.Settings.AIURLList.Add(new ClsURLItem("", 1, 1, URLTypeEnum.DeepStack));
            //}

        }

        public static async Task<List<ClsURLItem>> WaitForNextURL(Camera cam, bool GetRefinementServer, List<ClsPrediction> predictions = null, string RequiredLinkURLList = "", List<ClsURLItem> MainURLs = null)
        {
            //lets wait in here forever until a URL is available...  Unless trying to get a refinement server
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            List<ClsURLItem> ret = new List<ClsURLItem>();

            DateTime LastWaitingLog = DateTime.MinValue;
            bool displayedbad = false;
            bool displayedretry = false;
            List<string> CurrentURLs = new List<string>();
            List<string> LinkedRequiredURLs = RequiredLinkURLList.SplitStr(",;|", ToLower: true);
            bool GetLinkedRequiredList = LinkedRequiredURLs.Count > 0;
            int FoundRequiredCount = 0;
            int FoundRefinementCount = 0;
            int RefineNoMatchCount = 0;
            int RefineMatchCount = 0;
            string refinepreds = "";
            DateTime now = DateTime.Now;

            Stopwatch sw = Stopwatch.StartNew();

            try
            {
                AIURLListAvailableRefineServerCount.WriteFullFence(0);

                //preprocess a few things...
                for (int i = 0; i < AppSettings.Settings.AIURLList.Count; i++)
                {
                    AppSettings.Settings.AIURLList[i].RefinementUseCurrentlyValid.WriteFullFence(false);  //assume false at start of each loop

                    if (AppSettings.Settings.AIURLList[i].UseAsRefinementServer)
                    {
                        bool notenabled = !AppSettings.Settings.AIURLList[i].Enabled.ReadFullFence();
                        bool notintime = !Global.IsTimeBetween(now, AppSettings.Settings.AIURLList[i].ActiveTimeRange);
                        bool notinlist = !Global.IsInList(cam.Name, AppSettings.Settings.AIURLList[i].Cameras, TrueIfEmpty: true);
                        if (notenabled || notintime || notinlist)
                        {
                            AppSettings.Settings.AIURLList[i].LastTestedTime.Write(DateTime.Now);
                            AppSettings.Settings.AIURLList[i].LastSkippedReason = $"Refine: NotEnabled={notenabled}, NotInTime={notintime}, NotInCamList={notinlist}";
                            continue;
                        }

                        //dont let a refinement server be the same as the main server
                        if (MainURLs != null && MainURLs.Count > 0)
                        {
                            bool fnd = false;
                            foreach (ClsURLItem url in MainURLs)
                            {
                                if (string.Equals(AppSettings.Settings.AIURLList[i].ToString(), url.ToString(), StringComparison.OrdinalIgnoreCase))
                                {
                                    fnd = true;
                                    break;
                                }
                            }
                            if (fnd)
                            {
                                AppSettings.Settings.AIURLList[i].LastTestedTime.Write(DateTime.Now);
                                AppSettings.Settings.AIURLList[i].LastSkippedReason = $"Refine: FoundSameAsMainServer={fnd}";
                                continue;
                            }
                        }

                        AIURLListAvailableRefineServerCount.AtomicIncrementAndGet();

                        if (GetRefinementServer && predictions != null)
                        {
                            //set temp flag to indicate if the server can be CURRENTLY used as a refinement server
                            foreach (ClsPrediction pred in predictions)
                            {
                                bool fnd = false;

                                if (pred.Result == ResultType.Relevant)
                                {
                                    refinepreds += pred.Label + ",";
                                    if (AppSettings.Settings.AIURLList[i].RefinementObjects.Has("animal") && pred.ObjType == ObjectType.Animal)
                                        fnd = true;
                                    else if (AppSettings.Settings.AIURLList[i].RefinementObjects.Has("person") || AppSettings.Settings.AIURLList[i].RefinementObjects.Has("people") && pred.ObjType == ObjectType.Person)
                                        fnd = true;
                                    else if (AppSettings.Settings.AIURLList[i].RefinementObjects.Has("vehicle") && pred.ObjType == ObjectType.Vehicle)
                                        fnd = true;
                                    else if (Global.IsInList(pred.Label, AppSettings.Settings.AIURLList[i].RefinementObjects))
                                        fnd = true;

                                    if (fnd)
                                    {
                                        RefineMatchCount++;
                                        AppSettings.Settings.AIURLList[i].RefinementUseCurrentlyValid.WriteFullFence(true);
                                        break;
                                    }
                                }

                            }


                            if (!AppSettings.Settings.AIURLList[i].RefinementUseCurrentlyValid.ReadFullFence())
                            {
                                AppSettings.Settings.AIURLList[i].LastTestedTime.Write(DateTime.Now);
                                AppSettings.Settings.AIURLList[i].LastSkippedReason = $"Refine: No matched refine objects";
                                RefineNoMatchCount++;  //count number of failed tries to match refinement server objects
                            }
                        }

                    }
                }

                while (ret.Count == 0)
                {
                    int disabled = 0;
                    int inuse = 0;
                    int incorrectcam = 0;
                    int notintimerange = 0;
                    int maxpermonth = 0;
                    int notrefined = 0;
                    int notrequired = 0;
                    int onlylinked = 0;
                    int notonline = 0;

                    //If no refinement servers or less than should be available were returned
                    if (GetRefinementServer && RefineMatchCount == 0)
                    {
                        Log($"Debug: ---- Refinement server requested, but none were available for predictions '{refinepreds}'. ({sw.ElapsedMilliseconds}ms)");
                        break;
                    }

                    try
                    {
                        UpdateAIURLList();

                        List<ClsURLItem> sortedurls = new List<ClsURLItem>();

                        if (AppSettings.Settings.deepstack_urls_are_queued)
                        {
                            //always use oldest first
                            sortedurls = AppSettings.Settings.AIURLList.OrderBy((d) => d.LastUsedTime.Read()).ToList();
                        }
                        else
                        {
                            //use original order
                            sortedurls.AddRange(AppSettings.Settings.AIURLList);
                        }
                        //sort by oldest last used

                        for (int i = 0; i < sortedurls.Count; i++)
                        {
                            ClsURLItem url = sortedurls[i];

                            if (i > 0 && !GetRefinementServer && !GetLinkedRequiredList)
                            {
                                int debugging = 0;
                            }

                            now = DateTime.Now;

                            if (!url.Enabled.ReadFullFence())
                            {
                                url.LastTestedTime.Write(now);
                                url.LastSkippedReason = url.LastSkippedReason.Prepend("NotEnabled");
                                continue;
                            }

                            if (!url.ErrDisabled.ReadFullFence())
                            {
                                if (!url.InUse.ReadFullFence())
                                {
                                    if (Global.IsInList(cam.Name, url.Cameras, TrueIfEmpty: true))
                                    {
                                        if (GetRefinementServer && url.UseAsRefinementServer || !url.UseAsRefinementServer)
                                        {
                                            if (url.MaxImagesPerMonth == 0 || url.AITimeCalcs.CountMonth <= url.MaxImagesPerMonth)
                                            {

                                                if (Global.IsTimeBetween(now, url.ActiveTimeRange))
                                                {
                                                    //if set to ignoreconnection errors do an extra ping check to see if the server is available.  If not, skip it
                                                    if (url.IgnoreOfflineError)
                                                    {
                                                        if (!await url.CheckIfOnlineAsync())
                                                        {
                                                            url.LastTestedTime.Write(now);
                                                            url.LastSkippedReason = url.LastSkippedReason.Prepend("NotOnline");
                                                            notonline++;
                                                            continue;
                                                        }
                                                    }
                                                    if (url.CurErrCount.ReadFullFence() == 0)
                                                    {

                                                        if (url.UseOnlyAsLinkedServer && (GetRefinementServer || (!GetLinkedRequiredList)))
                                                        {
                                                            url.LastTestedTime.Write(now);
                                                            url.LastSkippedReason = url.LastSkippedReason.Prepend("UseOnlyAsLinked");
                                                            onlylinked++;
                                                            continue;
                                                        }

                                                        if (GetRefinementServer)
                                                        {
                                                            if (url.RefinementUseCurrentlyValid.ReadFullFence())
                                                            {
                                                                url.LastResultMessage = "Working [refinement]...";
                                                                url.CurOrder.WriteFullFence(i + 1);
                                                                url.InUse.WriteFullFence(true);
                                                                url.LastUsedTime.Write(now);
                                                                FoundRefinementCount++;
                                                                ret.Add(url);
                                                                // Dont break out of loop since we may need more than one refinement server
                                                            }
                                                            else
                                                            {
                                                                url.LastTestedTime.Write(now);
                                                                url.LastSkippedReason = url.LastSkippedReason.Prepend("RefineNotRequired");
                                                            }

                                                        }
                                                        else if (GetLinkedRequiredList)
                                                        {

                                                            if (Global.IsInList(url.ToString(), LinkedRequiredURLs, TrueIfEmpty: false))
                                                            {
                                                                url.LastResultMessage = "Working [Linked]...";
                                                                url.CurOrder.WriteFullFence(i + 1);
                                                                url.InUse.WriteFullFence(true);
                                                                url.LastUsedTime.Write(now);
                                                                FoundRequiredCount++;
                                                                ret.Add(url);
                                                                //dont break out of loop since we may need more than one linked/required URL
                                                            }
                                                            else
                                                            {
                                                                url.LastTestedTime.Write(now);
                                                                url.LastSkippedReason = url.LastSkippedReason.Prepend("RefineNotRequired");
                                                                notrequired++;
                                                            }
                                                        }
                                                        else if (!url.UseAsRefinementServer)
                                                        {
                                                            url.LastResultMessage = "Working...";
                                                            url.CurOrder.WriteFullFence(i + 1);
                                                            url.InUse.WriteFullFence(true);
                                                            url.LastUsedTime.Write(now);
                                                            ret.Add(url);
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            url.LastTestedTime.Write(now);
                                                            url.LastSkippedReason = url.LastSkippedReason.Prepend("NoConditionsMet?");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        double secs = Math.Round((now - url.LastUsedTime.Read()).TotalSeconds, 0);
                                                        if (secs >= AppSettings.Settings.MinSecondsBetweenFailedURLRetry)
                                                        {
                                                            url.LastResultMessage = "Working...";
                                                            url.CurOrder.WriteFullFence(i + 1);
                                                            url.InUse.WriteFullFence(true);
                                                            url.LastUsedTime.Write(now);
                                                            ret.Add(url);
                                                            if (!displayedretry)  //if we get in a long loop waiting for URL
                                                            {
                                                                Log($"---- Trying previously failed URL again after {secs} seconds. (ErrCount={url.CurErrCount.ReadFullFence()}, Setting 'MinSecondsBetweenFailedURLRetry'={AppSettings.Settings.MinSecondsBetweenFailedURLRetry}): {url}");
                                                                displayedretry = true;
                                                            }
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            url.LastTestedTime.Write(now);
                                                            url.LastSkippedReason = url.LastSkippedReason.Prepend("WaitingForRetry");
                                                            if (!displayedbad)  //if we get in a long loop waiting for URL
                                                            {
                                                                Log($"---- Waiting {AppSettings.Settings.MinSecondsBetweenFailedURLRetry - secs} seconds before retrying bad URL. (ErrCount={url.CurErrCount.ReadFullFence()} of {AppSettings.Settings.MaxQueueItemRetries}, Setting 'MinSecondsBetweenFailedURLRetry'={AppSettings.Settings.MinSecondsBetweenFailedURLRetry}): {url}");
                                                                displayedbad = true;
                                                            }
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    url.LastTestedTime.Write(now);
                                                    url.LastSkippedReason = url.LastSkippedReason.Prepend("NotInTimeRange");
                                                    notintimerange++;
                                                }

                                            }
                                            else
                                            {
                                                url.LastTestedTime.Write(now);
                                                url.LastSkippedReason = url.LastSkippedReason.Prepend("MaxImagesPerMonthMet");
                                                maxpermonth++;
                                            }

                                        }
                                        else
                                        {
                                            url.LastTestedTime.Write(now);
                                            url.LastSkippedReason = url.LastSkippedReason.Prepend("NotRefinementServer");
                                            notrefined++;
                                        }

                                    }
                                    else
                                    {
                                        url.LastTestedTime.Write(now);
                                        url.LastSkippedReason = url.LastSkippedReason.Prepend("NotInCamList");
                                        incorrectcam++;
                                    }

                                }
                                else
                                {
                                    url.LastTestedTime.Write(now);
                                    url.LastSkippedReason = url.LastSkippedReason.Prepend("InUse");
                                    inuse++;

                                    if (!GetRefinementServer && !GetLinkedRequiredList) { int debugging = 0; }

                                }
                            }
                            //disabled, but check to see if we need to reenable
                            else
                            {
                                double mins = (DateTime.Now - url.LastUsedTime.Read()).TotalMinutes;
                                url.LastTestedTime.Write(now);
                                url.LastSkippedReason = url.LastSkippedReason.Prepend("Disabled");

                                disabled++;
                                if (mins >= AppSettings.Settings.URLResetAfterDisabledMinutes)
                                {
                                    //check to see if can be re-enabled yet
                                    url.ErrDisabled.WriteFullFence(false);
                                    url.CurErrCount.WriteFullFence(0);
                                    url.InUse.WriteFullFence(false);
                                    url.LastResultMessage = "(Re-enabled)";

                                    Log($"---- Re-enabling disabled URL because {AppSettings.Settings.URLResetAfterDisabledMinutes} (URLResetAfterDisabledMinutes) minutes have passed: " + url);
                                    url.CurOrder.WriteFullFence(i + 1);
                                    url.InUse.WriteFullFence(true);
                                    url.LastUsedTime.Write(DateTime.Now);
                                    ret.Add(url);
                                    break;
                                }
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        Log("Error: getting next URL: " + ex.ToString());
                    }


                    if ((GetLinkedRequiredList && (FoundRequiredCount >= LinkedRequiredURLs.Count)) ||
                        (GetRefinementServer && (FoundRefinementCount >= RefineMatchCount)))
                    {
                        break;
                    }

                    if ((GetRefinementServer || GetLinkedRequiredList) && sw.ElapsedMilliseconds >= AppSettings.Settings.MaxWaitForAIServerMS)
                    {
                        string ew = "Debug:";
                        if (AppSettings.Settings.MaxWaitForAIServerTimeoutError)
                            ew = "Error:";

                        if (GetRefinementServer)
                            Log($"{ew} ---- URL request for REFINEMENT AI Server timed out, but only '{ret.Count}' of '{RefineMatchCount}' available. ({sw.ElapsedMilliseconds}ms - Setting in AITOOL.SETTINGS.JSON 'MaxWaitForAIServerMS' and is set to {AppSettings.Settings.MaxWaitForAIServerMS})");
                        else if (GetLinkedRequiredList)
                            Log($"{ew} ---- URL request for LINKED AI Server timed out, but only '{ret.Count}' of '{LinkedRequiredURLs.Count}' available. ({sw.ElapsedMilliseconds}ms) - Setting in AITOOL.SETTINGS.JSON 'MaxWaitForAIServerMS' and is set to {AppSettings.Settings.MaxWaitForAIServerMS})");
                        break;
                    }

                    //otherwise
                    if (ret.Count > 0)
                    {
                        break;
                    }

                    if ((DateTime.Now - LastWaitingLog).TotalMinutes >= 5)
                    {
                        Log($"---- All URL's are in use, disabled, camera name doesnt match or time range was not met.  ({inuse} inuse, {disabled} disabled, {incorrectcam} wrong camera, {notintimerange} not in time range, {maxpermonth} at max per month limit, {notrefined} not refinement server, {onlylinked} only use as linked server) Waiting...");
                        LastWaitingLog = DateTime.Now;
                    }


                    //short wait
                    await Task.Delay(AppSettings.Settings.loop_delay_ms);

                }

                //=====================================================================================================

                sw.Stop();


                //see if any servers have 'LINKED' servers
                if (!GetRefinementServer && !GetLinkedRequiredList && ret.Count > 0)
                {
                    List<ClsURLItem> linked = new List<ClsURLItem>();
                    foreach (ClsURLItem url in ret)
                    {
                        if (url.LinkServerResults && !string.IsNullOrEmpty(url.LinkedResultsServerList))
                        {
                            linked.AddRange(await WaitForNextURL(cam, false, null, url.LinkedResultsServerList));
                        }
                    }
                    if (linked.Count > 0)
                    {
                        Log($"Debug: ---- Found '{linked.Count}' linked AI URL's.");
                        ret.AddRange(linked);
                    }
                }


            }
            catch (Exception ex)
            {

                Log($"Error: {ex.Msg()}");
            }

            //remove any dupes just in case
            ret = ret.Distinct().ToList();


            return ret;


        }

        static ThreadSafe.Integer CurRunningDetectTasks = new ThreadSafe.Integer(0);

        public static async Task ImageQueueLoop()
        {
            //This runs in another thread, waiting for items to appear in the queue and process them one at a time
            try
            {
                //Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;

                ClsImageQueueItem CurImg;

                DateTime LastCleanDupesTime = DateTime.MinValue;


                //lets wait 5 seconds to let the UI settle down a bit
                await Task.Delay(5000);

                //Start infinite loop waiting for images to come into queue
                ThreadSafe.Integer MaxThreadCnt = new ThreadSafe.Integer(0);

                while (true)
                {
                    if (MasterCTS.IsCancellationRequested)
                        break;

                    ThreadSafe.Integer ThreadCnt = new ThreadSafe.Integer(0);

                    while (!ImageProcessQueue.IsEmpty)
                    {

                        int ProcImgCnt = 0;
                        int ErrCnt = 0;
                        long LastThreadInitTimeMS = 0;
                        ThreadSafe.Datetime NextDeepstackRestartTime = new ThreadSafe.Datetime(DateTime.MinValue);

                        while (!ImageProcessQueue.IsEmpty)
                        {
                            //tiny delay to conserve cpu and allow more images to come in the queue if needed
                            //await Task.Delay(250);

                            //get the next image

                            if (ImageProcessQueue.TryDequeue(out CurImg))
                            {
                                if (CurRunningDetectTasks.ReadFullFence() > 1)
                                {
                                    int testing = 0;
                                }

                                Camera cam = GetCamera(CurImg.image_path, true);

                                if (cam == null)
                                {
                                    Log($"Error: Camera could not be found to match this image: {CurImg.image_path}");
                                    continue;
                                }

                                //skip the image if its been in the queue too long
                                if ((DateTime.Now - CurImg.TimeAdded).TotalMinutes >= AppSettings.Settings.MaxImageQueueTimeMinutes)
                                {
                                    Log($"...Taking image OUT OF QUEUE because it has been in there over 'MaxImageQueueTimeMinutes'. (QueueTime={(DateTime.Now - CurImg.TimeAdded).TotalMinutes.ToString("###0.0")}, Image ErrCount={CurImg.ErrCount}, Image RetryCount={CurImg.RetryCount}, ImageProcessQueue.Count={ImageProcessQueue.Count}: '{CurImg.image_path}'", "None", cam, CurImg);
                                    continue;
                                }

                                Stopwatch sw = Stopwatch.StartNew();

                                //wait for the next url to become available...
                                List<ClsURLItem> urls = await WaitForNextURL(cam, false);

                                sw.Stop();

                                //If we have any linked servers there may be more than one server that we send at the same time
                                foreach (ClsURLItem url in urls)
                                {
                                    Log($"Debug: Adding task for file '{Path.GetFileName(CurImg.image_path)}' (Image QueueTime='{(DateTime.Now - CurImg.TimeAdded).TotalMinutes.ToString("###0.0")}' mins, CurRunningTasks={CurRunningDetectTasks.ReadFullFence()}, URL Queue wait='{sw.ElapsedMilliseconds}ms', URLOrder={url.CurOrder}, URLOriginalOrder={url.Order}) on URL '{url}'", url.CurSrv, cam, CurImg);

                                }

                                sw.Start();

                                //This *should* start detection on another thread and return control to this thread right away.
                                Task.Run(async () =>
                                {

                                    CurRunningDetectTasks.AtomicIncrementAndGet();
                                    Global.SendMessage(MessageType.BeginProcessImage, CurImg.image_path);

                                    DetectObjectsResult result = await DetectObjects(CurImg, urls); //ai process image

                                    Global.SendMessage(MessageType.EndProcessImage, CurImg.image_path);

                                    foreach (ClsURLItem url in result.OutURLs)
                                    {
                                        if (!url.LastResultSuccess)
                                        {
                                            Interlocked.Increment(ref ErrCnt);
                                            url.ErrsInRowCount.AtomicIncrementAndGet();

                                            if (url.CurErrCount.ReadFullFence() > 0)
                                            {
                                                if (url.CurErrCount.ReadFullFence() < AppSettings.Settings.MaxQueueItemRetries)
                                                {
                                                    //put url back in queue when done

                                                    //Only send error if MaxWaitForAIServerTimeoutError is true
                                                    string ew = "Debug:";
                                                    if (AppSettings.Settings.MaxWaitForAIServerTimeoutError)
                                                        ew = "Error:";
                                                    if (url.LastResultMessage.Has("request timed out"))
                                                    {
                                                        Log($"{ew}...Problem with AI URL: '{url.LastResultMessage}' - '{url}' (URL ErrCount={url.CurErrCount}, max allowed of {AppSettings.Settings.MaxQueueItemRetries})", url.CurSrv, cam);
                                                    }
                                                    else
                                                    {
                                                        Log($"...Problem with AI URL: '{url.LastResultMessage}' - '{url}' (URL ErrCount={url.CurErrCount}, max allowed of {AppSettings.Settings.MaxQueueItemRetries})", url.CurSrv, cam);
                                                    }
                                                }
                                                else
                                                {
                                                    url.ErrDisabled.WriteFullFence(false);
                                                    Log($"...Error: AI URL failed with '{url.LastResultMessage}' - for '{url.Type}' failed '{url.CurErrCount}' times.  Disabling: '{url}'", url.CurSrv, cam);
                                                }

                                            }

                                            if (url.ErrsInRowCount.ReadFullFence() > AppSettings.Settings.MaxErrorsInARowBeforeDisable)
                                            {
                                                Log($"...Error: AI URL failed {url.ErrsInRowCount.ReadFullFence()} times in a row.  DISABLING: '{url}'", url.CurSrv, cam);
                                                url.ErrDisabled.WriteFullFence(true);
                                            }

                                            if (url.ErrsInRowCount.ReadFullFence() >= AppSettings.Settings.deepstack_autorestart_fail_count &&
                                                AppSettings.Settings.deepstack_autostart &&
                                                DeepStackServerControl.IsInstalled &&
                                                DeepStackServerControl.URLS.IndexOf(url.ToString(), StringComparison.OrdinalIgnoreCase) >= 0)

                                            {
                                                if (NextDeepstackRestartTime.Read() == DateTime.MinValue)
                                                    NextDeepstackRestartTime.Write(DateTime.Now);

                                                double mins = (DateTime.Now - NextDeepstackRestartTime.Read()).TotalMinutes;
                                                double togo = (AppSettings.Settings.deepstack_autorestart_minutes_between_restart_attempts - mins);

                                                if (!DeepStackServerControl.Starting.ReadFullFence() &&
                                                   DateTime.Now >= NextDeepstackRestartTime.Read())
                                                {
                                                    Log($"Error: Locally installed deepstack instance failed {url.ErrsInRowCount.ReadFullFence()} times in a row. (autorestart_fail_count={AppSettings.Settings.deepstack_autorestart_fail_count}) Restarting Deepstack...");
                                                    //dont wait for it
                                                    await DeepStackServerControl.StartDeepstackAsync(true);
                                                    url.ErrsInRowCount.WriteFullFence(0);
                                                    NextDeepstackRestartTime.Write(DateTime.Now.AddSeconds(AppSettings.Settings.deepstack_autorestart_minutes_between_restart_attempts));
                                                }
                                                else
                                                {
                                                    Log($"Error: Locally installed deepstack instance failed {url.ErrsInRowCount.ReadUnfenced()} times in a row.  (autorestart_fail_count={AppSettings.Settings.deepstack_autorestart_fail_count}) Waiting {togo.ToString("##0.0")} mins before attempting restart...");
                                                }

                                            }

                                            CurImg.RetryCount.AtomicIncrementAndGet();  //even if there was not an error directly accessing the image

                                            if (CurImg.ErrCount.ReadFullFence() <= AppSettings.Settings.MaxQueueItemRetries && CurImg.RetryCount.ReadFullFence() <= AppSettings.Settings.MaxQueueItemRetries)
                                            {
                                                //put back in queue to be processed by another deepstack server
                                                Log($"...Putting image back in queue due to URL '{url}' problem (QueueTime={(DateTime.Now - CurImg.TimeAdded).TotalMinutes.ToString("###0.0")}, Image ErrCount={CurImg.ErrCount}, Image RetryCount={CurImg.RetryCount}, URL ErrCount={url.CurErrCount}): '{CurImg.image_path}', ImageProcessQueue.Count={ImageProcessQueue.Count}", url.CurSrv, cam, CurImg);
                                                ImageProcessQueue.Enqueue(CurImg);
                                            }
                                            else
                                            {
                                                cam.stats_skipped_images++;
                                                cam.stats_skipped_images_session++;
                                                int timems = (int)(DateTime.Now - CurImg.TimeAdded).TotalMilliseconds;

                                                Log($"...Error: Removing image from queue. Image RetryCount={CurImg.RetryCount}, URL ErrCount='{url.CurErrCount}': {url}', Image: '{CurImg.image_path}', ImageProcessQueue.Count={ImageProcessQueue.Count}, Skipped this session={cam.stats_skipped_images_session}", url.CurSrv, cam, CurImg);
                                                Global.CreateHistoryItem(new History().Create(CurImg.image_path, DateTime.Now, cam.Name, $"Skipped image, {CurImg.RetryCount.ReadFullFence()} errors processing.", "", false, "", url.CurSrv, timems, false));

                                            }
                                        }
                                        else
                                        {
                                            Interlocked.Increment(ref ProcImgCnt);
                                            //reset error count
                                            url.CurErrCount.WriteFullFence(0);
                                            url.ErrsInRowCount.WriteFullFence(0);
                                        }

                                        url.InUse.WriteFullFence(false);
                                        url.FullTimeMS = (int)(DateTime.Now - url.LastUsedTime.Read()).TotalMilliseconds;

                                    }
                                    CurRunningDetectTasks.AtomicDecrementAndGet();

                                });

                                sw.Stop();
                                LastThreadInitTimeMS = sw.ElapsedMilliseconds;
                            }
                            else
                            {
                                //Log("No Images left in the queue!");
                                break;
                            }

                        }

                        if (CurRunningDetectTasks.ReadFullFence() > 0)
                        {
                            Log($"Debug: Done adding. {CurRunningDetectTasks.ReadFullFence()} total threads running, LastThreadInitTimeMS={LastThreadInitTimeMS}, ErrCnt={ErrCnt}, ImageProcessQueue.Count={ImageProcessQueue.Count}");
                        }

                        //Clean up old images in the dupe check dic
                        if ((DateTime.Now - LastCleanDupesTime).TotalMinutes >= 60)
                        {
                            int cnt = 0;
                            foreach (KeyValuePair<string, DateTime> kvPair in image_detection_dictionary)
                            {
                                if ((DateTime.Now - kvPair.Value).TotalMinutes >= 30)
                                {   // Remove expired item.
                                    cnt++;
                                    //ClsImageQueueItem removedItem;
                                    image_detection_dictionary.TryRemove(kvPair.Key, out _);
                                }
                            }

                        }

                    }

                    if (MasterCTS.IsCancellationRequested)
                        break;

                    //Only loop 10 times a second conserve cpu
                    await Task.Delay(AppSettings.Settings.loop_delay_ms);
                }

                Log("Debug: ImageQueueLoop canceled.");

            }
            catch (Exception ex)
            {
                //if we get here its the end of the world as we know it
                Log("Error: * '...Human sacrifice, dogs and cats living together – mass hysteria!' * - " + ex.Msg());
            }
        }

        //EVENT: new image added to input_path -> START AI DETECTION
        private static void OnCreated(object source, FileSystemEventArgs e)
        {
            AddImageToQueue(e.FullPath);
        }

        public static void AddImageToQueue(string Filename)
        {

            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            lock (FileWatcherLockObject)
            {
                try
                {
                    //make sure we are not processing a duplicate file...
                    if (image_detection_dictionary.ContainsKey(Filename.ToLower()))
                    {
                        Log("Skipping image because of duplicate Created File Event: " + Filename);
                    }
                    else
                    {
                        Camera cam = GetCamera(Filename, true);
                        if (cam != null)  //only put in queue if we can match to camera (even default)
                        {

                            if (cam.enabled)
                            {
                                if (!(cam.Paused && cam.PauseFileMon))
                                {
                                    //Note:  Interwebz says ConCurrentQueue.Count may be slow for large number of items but I dont think we have to worry here in most cases
                                    int qsize = ImageProcessQueue.Count + 1;
                                    if (qsize > AppSettings.Settings.MaxImageQueueSize)
                                    {
                                        Log("");
                                        Log($"Error: Skipping image because queue ({qsize}) is greater than '{AppSettings.Settings.MaxImageQueueSize}'. (Adjust 'MaxImageQueueSize' in .JSON file if needed): " + Filename, "", cam, Filename);
                                    }
                                    else
                                    {
                                        Log("Debug: ");
                                        Log($"Debug: ====================== Adding new image to queue (Count={ImageProcessQueue.Count + 1}): " + Filename, "", cam, Filename);
                                        ClsImageQueueItem CurImg = new ClsImageQueueItem(Filename, qsize);
                                        image_detection_dictionary.TryAdd(Filename.ToLower(), DateTime.Now);
                                        ImageProcessQueue.Enqueue(CurImg);
                                        scalc.AddToCalc(qsize);
                                        Global.SendMessage(MessageType.ImageAddedToQueue);
                                    }

                                }
                                else
                                {
                                    Log($"Debug: Skipping image because camera '{cam}' file monitoring is PAUSED " + Filename, "", cam, Filename);
                                }

                            }
                            else
                            {
                                Log($"Debug: Skipping image because camera '{cam}' is DISABLED " + Filename, "", cam, Filename);
                            }
                        }
                        else
                        {
                            Log("Error: Skipping image because no camera found for new image " + Filename, "", cam, Filename);
                        }


                    }

                }
                catch (Exception ex)
                {
                    Log("Error: " + ex.Msg());
                }

            }


        }
        //event: image in input_path renamed
        private static void OnRenamed(object source, RenamedEventArgs e)
        {
            Global.DeleteHistoryItem(e.OldFullPath);
        }

        //event: image in input path deleted
        private static void OnDeleted(object source, FileSystemEventArgs e)
        {
            Global.DeleteHistoryItem(e.FullPath);
        }

        private static void OnError(object sender, System.IO.ErrorEventArgs e)
        {
            //Too many changes at once in directory:C:\BlueIris\aiinput.
            //File watcher  The specified network name is no longer available
            string path = ((FileSystemWatcher)sender).Path;
            Log("Error: File watcher error: " + e.GetException().Message + $" on path '{path}'");
            UpdateWatchers(true);

        }

        public static void TimerCheckFileSystemWatchers(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (FileWatcherHasError.ReadFullFence())
            {
                Log($"Debug: Re-checking bad File System Watcher Paths ('FileSystemWatcherRetryOnErrorTimeMS' = {AppSettings.Settings.FileSystemWatcherRetryOnErrorTimeMS}ms)...");
                UpdateWatchers(true);
            }
        }

        public static SemaphoreSlim Semaphore_Watcher_Updating = new SemaphoreSlim(1, 1);

        public static async Task UpdateWatchers(bool Reset)
        {
            await Semaphore_Watcher_Updating.WaitAsync();

            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            try
            {
                if (AppSettings.AlreadyRunning)
                {
                    Log("*** Another instance is already running, skip watching for changed files ***");
                    return;
                }

                FileWatcherHasError.WriteFullFence(false);

                Global.UpdateProgressBar($"Updating watched folders'...", 1, 1, 1);

                //first add all the names and paths to check...
                List<string> names = new List<string>();
                Dictionary<string, string> paths = new Dictionary<string, string>();

                string pths = AppSettings.Settings.input_path.Trim().TrimEnd(@"\".ToCharArray());
                names.Add($"INPUT_PATH|{pths}|{AppSettings.Settings.input_path_includesubfolders}");
                foreach (Camera cam in AppSettings.Settings.CameraList)
                {
                    if (cam.enabled && !String.IsNullOrWhiteSpace(cam.input_path))
                    {
                        pths = cam.input_path.Trim().TrimEnd(@"\".ToCharArray());
                        names.Add($"{cam.Name}|{pths}|{cam.input_path_includesubfolders}");
                    }
                }

                if (Reset)
                {
                    foreach (ClsFileSystemWatcher watcher1 in watchers.Values)
                    {
                        if (watcher1 != null && watcher1.watcher != null)
                        {
                            try
                            {
                                watcher1.watcher.EnableRaisingEvents = false;
                                watcher1.watcher.Dispose();
                                watcher1.watcher = null;
                            }
                            catch (Exception ex)
                            {

                                Log($"Error: Failed to reset/clear watcher for folder '{watcher1.Name}' - {watcher1.Path}: {ex.Message}");
                            }
                        }
                    }
                    watchers.Clear();
                }

                //check each one to see if needs to be added
                foreach (string item in names)
                {
                    List<string> splt = item.SplitStr("|", false);
                    string name = splt[0];
                    string path = splt[1];
                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        bool include = Convert.ToBoolean(splt[2]);
                        if (!paths.ContainsKey(path.ToLower()))
                        {
                            paths.Add(path.ToLower(), path);

                            if (!watchers.ContainsKey(name.ToLower()))
                            {
                                //this will return null if the path is invalid...
                                FileSystemWatcher curwatch = await CreateFileWatcherAsync(path, include);
                                if (curwatch != null)
                                {
                                    ClsFileSystemWatcher mywtc = new ClsFileSystemWatcher(name, path, curwatch, include);
                                    //add even if null to keep track of things
                                    watchers.Add(name.ToLower(), mywtc);
                                }
                            }
                            else
                            {
                                //update path if needed, even to empty
                                watchers[name.ToLower()].Path = path;
                                if (watchers[name.ToLower()].watcher == null)
                                {
                                    //could be null if path is bad
                                    watchers[name.ToLower()].watcher = await CreateFileWatcherAsync(path, include);
                                }
                            }
                        }
                        else
                        {
                            Log($"Debug: Skipping duplicate path for '{name}': '{path}'");
                        }

                    }


                }

                //check to see if any need disabling - a camera was deleted
                foreach (ClsFileSystemWatcher watcher1 in watchers.Values)
                {
                    bool fnd = false;
                    foreach (string item in names)
                    {
                        List<string> splt = item.SplitStr("|");
                        string name = splt[0];
                        if (string.Equals(name, watcher1.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            fnd = true;
                            break;
                        }
                    }
                    if (!fnd)
                    {
                        watcher1.Path = "";
                    }

                }


                //enable or disable watchers
                int enabledcnt = 0;
                int disabledcnt = 0;

                Dictionary<string, string> dupes = new Dictionary<string, string>();

                foreach (ClsFileSystemWatcher watcher in watchers.Values)
                {
                    if (watcher.watcher != null)
                    {
                        if (!String.IsNullOrWhiteSpace(watcher.Path))
                        {
                            if (!dupes.ContainsKey(watcher.Path.ToLower()))
                            {
                                if (watcher.Path != watcher.watcher.Path)
                                {
                                    watcher.watcher.Path = watcher.Path;
                                    Log($"Debug: Watcher '{watcher.Name}' changed from '{watcher.watcher.Path}' to '{watcher.Path}'.");
                                }

                                if (watcher.IncludeSubdirectories != watcher.watcher.IncludeSubdirectories)
                                {
                                    watcher.watcher.IncludeSubdirectories = watcher.IncludeSubdirectories;
                                    Log($"Debug: Watcher '{watcher.Name}' IncludeSubdirectories changed from '{watcher.watcher.IncludeSubdirectories}' to '{watcher.IncludeSubdirectories}'.");
                                }

                                if (watcher.watcher.EnableRaisingEvents != true)
                                {
                                    enabledcnt++;
                                    watcher.watcher.EnableRaisingEvents = true;
                                    dupes.Add(watcher.Path.ToLower(), watcher.Path);
                                    Log($"Debug: Watcher '{watcher.Name}' is now watching '{watcher.Path}'");
                                }

                            }
                            else
                            {
                                Log($"Debug: Watcher '{watcher.Name}' has a duplicate path, skipping '{watcher.Path}'");
                            }
                        }
                        else
                        {
                            //make sure it is disabled
                            disabledcnt++;

                            watcher.watcher.EnableRaisingEvents = false;
                            watcher.watcher.Dispose();
                            watcher.watcher = null;
                            Log($"Debug: Watcher '{watcher.Name}' has an empty path, just disabled.");
                        }

                    }
                    else if (!string.IsNullOrEmpty(watcher.Path))
                    {
                        Log($"Error: Watcher '{watcher.Name}' disabled. INVALID PATH='{watcher.Path}'");
                    }
                    else
                    {
                        //Log($"Watcher '{watcher.Name}' already disabled.");
                    }


                }

                if (watchers.Count == 0)
                {
                    Log("Debug: No FileSystemWatcher input folders defined yet.");
                }
                else
                {
                    if (enabledcnt == 0)
                    {
                        Log("Debug: No NEW FileSystemWatcher input folders found.");
                    }
                    else
                    {
                        Log($"Debug: Enabled {enabledcnt} FileSystemWatchers.");
                    }
                }


            }
            catch (Exception ex)
            {
                FileWatcherHasError.WriteFullFence(true);
                Log($"Error: {ex.Msg()}");
            }
            finally
            {
                Semaphore_Watcher_Updating.Release();
            }

        }

        static ThreadSafe.Boolean FileWatcherHasError = new ThreadSafe.Boolean(false);

        public static async Task<FileSystemWatcher> CreateFileWatcherAsync(string path, bool IncludeSubdirectories = false, string filter = "*.jpg")
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            FileSystemWatcher watcher = null;

            try
            {
                // Be aware: https://stackoverflow.com/questions/1764809/filesystemwatcher-changed-event-is-raised-twice

                if (!String.IsNullOrWhiteSpace(path))
                {
                    Stopwatch sw = Stopwatch.StartNew();

                    if (await Global.DirectoryExistsAsync(path, 10000))
                    {
                        watcher = new FileSystemWatcher(path);
                        watcher.Path = path;
                        watcher.Filter = filter;
                        watcher.IncludeSubdirectories = IncludeSubdirectories;
                        watcher.InternalBufferSize = 65536;  //defaults to 8k, we are going max it out to try to prevent "too many changes at once in directory"

                        //The 'default' is the bitwise OR combination of NotifyFilters.LastWrite | NotifyFilters.LastAccess | NotifyFilters.CreationTime | NotifyFilters.FileName | NotifyFilters.DirectoryName
                        watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;

                        //fswatcher events
                        watcher.Created += new FileSystemEventHandler(OnCreated);
                        watcher.Renamed += new RenamedEventHandler(OnRenamed);
                        watcher.Deleted += new FileSystemEventHandler(OnDeleted);
                        watcher.Error += new ErrorEventHandler(OnError);

                    }
                    else
                    {
                        FileWatcherHasError.WriteFullFence(true);
                        Log($"Error: Path does not exist. Time={sw.ElapsedMilliseconds}ms: " + path);
                    }
                }
            }
            catch (Exception ex)
            {
                FileWatcherHasError.WriteFullFence(true);
                Log($"Error: {ex.Msg()}");
            }

            return watcher;
        }

        public static ClsImageAdjust GetImageAdjustProfileByName(string name, bool ReturnDefault)
        {
            ClsImageAdjust ret = null;
            ClsImageAdjust def = null;

            foreach (ClsImageAdjust ia in AppSettings.Settings.ImageAdjustProfiles)
            {
                if (string.Equals(name.Trim(), ia.Name.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    ret = ia;
                }
                if (string.Equals("Default", ia.Name.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    def = ia;
                }
            }

            if (ret == null && ReturnDefault)
                ret = def;

            if (ret == null)
            {
                //ret = new ClsImageAdjust("Default");
                Log($"Error: Could not find Image Adjust profile that matches '{name}'");
            }

            return ret;
        }

        public static bool HasImageAdjustProfile(string name)
        {
            bool ret = false;

            foreach (ClsImageAdjust ia in AppSettings.Settings.ImageAdjustProfiles)
            {
                if (string.Equals(name.Trim(), ia.Name.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return ret;
        }

        public static async Task<System.Drawing.Image> ApplyImageAdjustProfileAsync(ClsImageAdjust IAProfile, string InputImageFile, string OutputImageFile)
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            System.Drawing.Image retimg = null;
            SixLabors.ImageSharp.Image ISImage = null;
            MemoryStream IStream = new System.IO.MemoryStream();
            try
            {
                if (!string.IsNullOrEmpty(InputImageFile) && File.Exists(InputImageFile))
                {
                    bool SaveToFile = (!string.IsNullOrEmpty(OutputImageFile));

                    //SixLabors.ImageSharp.Configuration config = new Configuration();

                    ISImage = await SixLabors.ImageSharp.Image.LoadAsync(InputImageFile);


                    if (IAProfile.ImageWidth != -1 && IAProfile.ImageHeight != -1 && ISImage.Width != IAProfile.ImageWidth || ISImage.Height != IAProfile.ImageHeight)  //hard coded size
                    {
                        Log($"Resizing image from {ISImage.Width},{ISImage.Height} to {IAProfile.ImageWidth},{IAProfile.ImageHeight}...");
                        ISImage.Mutate(i => i.Resize(IAProfile.ImageWidth, IAProfile.ImageHeight));
                    }
                    else if (IAProfile.ImageSizePercent > 0 && IAProfile.ImageSizePercent < 100)
                    {
                        double fractionalPercentage = (IAProfile.ImageSizePercent / 100.0);
                        double outputWidth = ISImage.Width * fractionalPercentage;
                        double outputHeight = ISImage.Height * fractionalPercentage;

                        Log($"Resizing image to {IAProfile.ImageSizePercent} from {ISImage.Width},{ISImage.Height} to {outputWidth},{outputHeight}...");
                        ISImage.Mutate(i => i.Resize(outputWidth.ToInt(), outputHeight.ToInt()));
                    }

                    if (IAProfile.Brightness > 1 && IAProfile.Brightness < 100)
                    {
                        //A value of 0 will create an image that is completely black. A value of 1 leaves the input unchanged. 
                        //Other values are linear multipliers on the effect. Values of an amount over 1 are allowed, providing brighter results
                        //amount - The proportion of the conversion.Must be greater than or equal to 0.
                        Log($"Changing brightness by amount {IAProfile.Brightness}...");
                        ISImage.Mutate(i => i.Brightness(IAProfile.Brightness));
                    }

                    if (IAProfile.Contrast > 1 && IAProfile.Contrast < 100)
                    {
                        //A value of 0 will create an image that is completely gray. A value of 1 leaves the input unchanged. 
                        //Other values are linear multipliers on the effect. Values of an amount over 1 are allowed, providing results with more contrast.
                        //amount - The proportion of the conversion. Must be greater than or equal to 0.
                        Log($"Changing contrast by amount {IAProfile.Contrast}...");
                        ISImage.Mutate(i => i.Contrast(IAProfile.Contrast));
                    }


                    //string tfile = Path.Combine(Environment.GetEnvironmentVariable("TEMP"), "_AITOOL\tmpimage.jpg");

                    //Save the image using the specified jpeg compression
                    Log($"Compressing jpeg to {IAProfile.JPEGQualityPercent}% quality...");
                    SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder encoder = new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder();
                    //encoder.Quality = IAProfile.JPEGQualityPercent;

                    // lets switch out the default encoder for jpeg to one
                    // that saves at 90 quality
                    Configuration.Default.ImageFormatsManager.SetEncoder(JpegFormat.Instance, new JpegEncoder()
                    {
                        Quality = IAProfile.JPEGQualityPercent
                    });

                    if (SaveToFile)
                    {
                        //save to file
                        await ISImage.SaveAsJpegAsync(OutputImageFile, encoder);

                    }
                    else  //assume we just need the image for viewing and send back an image
                    {
                        //save to stream
                        await ISImage.SaveAsJpegAsync(IStream, encoder);

                        //read back from stream
                        //ISImage = await SixLabors.ImageSharp.Image.LoadAsync(IStream);

                        retimg = System.Drawing.Image.FromStream(IStream);
                    }


                }
                else
                {
                    Log("File does not exist: " + InputImageFile);
                }

            }
            catch (Exception ex)
            {

                Log("Error: " + ex.Msg());
            }
            finally
            {
                if (IStream != null)
                    IStream.Dispose();

                if (ISImage != null)
                    ISImage.Dispose();

            }

            return retimg;

        }


        public class ClsAIServerResponse
        {
            public List<ClsPrediction> Predictions = new List<ClsPrediction>();
            public bool Success = false;
            public string JsonString = "";
            public string Message = "";
            public string Error = "";
            public long SWPostTime = 0;
            public HttpStatusCode StatusCode = HttpStatusCode.Unused;
            public ClsURLItem AIURL = null;
        }

        public static async Task<ClsAIServerResponse> GetDetectionsFromAIServer(ClsImageQueueItem CurImg, ClsURLItem AiUrl, Camera cam)
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            ClsAIServerResponse ret = new ClsAIServerResponse();

            ret.AIURL = AiUrl;
            AiUrl.LastResultMessage = "";
            AiUrl.LastResultSuccess = false;

            bool OverrideThreshold = AiUrl.Threshold_Lower > 0 || (AiUrl.Threshold_Upper > 0 && AiUrl.Threshold_Upper < 100);

            //==============================================================================================================
            //==============================================================================================================
            //==============================================================================================================
            if (AiUrl.Type.ToString().Has("codeproject") || AiUrl.Type == URLTypeEnum.DeepStack || AiUrl.Type == URLTypeEnum.DeepStack_Custom || AiUrl.Type == URLTypeEnum.DeepStack_Faces || AiUrl.Type == URLTypeEnum.DeepStack_Scene)
            {
                Stopwatch swposttime = new Stopwatch();

                try
                {
                    long FileSize = new FileInfo(CurImg.image_path).Length;

                    using MultipartFormDataContent request = new MultipartFormDataContent();

                    using StreamContent sc = new StreamContent(CurImg.ToMemStream());

                    request.Add(sc, "image", Path.GetFileName(CurImg.image_path));

                    string overr = "(NoLowerThresholdOverride)";

                    double minconf = 0;
                    if (AppSettings.Settings.HistoryRestrictMinThresholdAtSource && !OverrideThreshold)
                    {
                        overr = $"(CAM_LowerThresholdOverride={cam.threshold_lower},Upper={cam.threshold_upper})";
                        minconf = cam.threshold_lower;
                    }
                    else if (AppSettings.Settings.HistoryRestrictMinThresholdAtSource && OverrideThreshold)
                    {
                        overr = $"(URL_LowerThresholdOverride={AiUrl.Threshold_Lower},Upper={AiUrl.Threshold_Upper})";
                        minconf = AiUrl.Threshold_Lower;
                    }

                    double pc = 0;

                    if (minconf > 0)
                    {
                        pc = minconf / 100;
                        overr += $"({pc})";
                        StringContent scmc = new StringContent((pc).ToString().Replace(",", "."));  //replace comma with period in cases where the regional decimal symbol is a comma - Deepstack doesnt like that.
                        request.Add(scmc, "min_confidence");
                    }

                    Log($"Debug: (1/6) Uploading a {Global.FormatBytes(FileSize)} image to '{AiUrl.Type}' {overr} AI Server at {AiUrl}", AiUrl.CurSrv, cam, CurImg);

                    swposttime.Restart();


                    using HttpResponseMessage output = await AiUrl.HttpClient.PostAsync(AiUrl.ToString(), request, MasterCTS.Token);

                    swposttime.Stop();
                    ret.StatusCode = output.StatusCode;
                    ret.JsonString = await output.Content.ReadAsStringAsync();

                    ClsDeepStackResponse response = null;
                    string cleanjsonString = "";
                    if (ret.JsonString != null && !string.IsNullOrWhiteSpace(ret.JsonString))
                    {
                        cleanjsonString = ret.JsonString.CleanString();
                        try
                        {
                            response = JsonConvert.DeserializeObject<ClsDeepStackResponse>(ret.JsonString);
                        }
                        catch (Exception ex)
                        {
                            //deserialization did not cause exception, it just gave a null response in the object?
                            //probably wont happen but just making sure
                            ret.Error = $"ERROR: Deserialization of 'Response' from DeepStack failed. JSON: '{cleanjsonString}': {ex.Message}";
                            AiUrl.IncrementError();
                            AiUrl.LastResultMessage = ret.Error;
                        }
                    }
                    else
                    {
                        ret.Error = $"ERROR: Empty string returned from HTTP post?";
                        AiUrl.IncrementError();
                        AiUrl.LastResultMessage = ret.Error;
                    }

                    if (output.IsSuccessStatusCode)
                    {
                        try
                        {
                            if (response != null)
                            {
                                if (!response.success || !string.IsNullOrWhiteSpace(response.error)) // || )
                                {
                                    string err = "";
                                    if (!string.IsNullOrWhiteSpace(response.error))
                                        err = response.error;
                                    //if (response.message.EqualsIgnoreCase("no plates found"))
                                    //    err = response.message;
                                    ret.Error = $"ERROR: Failure response from '{AiUrl.Type.ToString()}'. Error='{err}'. JSON: '{cleanjsonString}'";
                                    AiUrl.IncrementError();
                                    AiUrl.LastResultMessage = ret.Error;
                                }
                                else
                                {
                                    List<ClsDeepstackDetection> addto = new List<ClsDeepstackDetection>();

                                    //intialize array if none returned so we can add a scene if needed
                                    if (response.predictions != null)
                                        addto = response.predictions.ToList();

                                    //check to see if we have a scene rather than normal detection and create a prediction from it
                                    if (!string.IsNullOrEmpty(response.label) && response.confidence > 0)
                                    {
                                        //{'success': True, 'confidence': 0.7373981, 'label': 'conference_room'
                                        ClsDeepstackDetection spred = new ClsDeepstackDetection();
                                        spred.label = $"Scene";
                                        spred.Detail = response.label;
                                        spred.confidence = response.confidence;
                                        //try to create a rectangle smaller than the image so the label will fit
                                        spred.x_min = 5;
                                        spred.y_min = 5;
                                        spred.x_max = CurImg.Width - 5;
                                        spred.y_max = CurImg.Height - 40;
                                        addto.Add(spred);
                                    }

                                    foreach (ClsDeepstackDetection DSObj in addto)
                                    {
                                        ClsPrediction pred = new ClsPrediction(ObjectType.Object, cam, DSObj, CurImg, AiUrl);

                                        ret.Predictions.Add(pred);

                                    }


                                    ret.Success = true;
                                    AiUrl.LastResultMessage = $"{ret.Predictions.Count} predictions found.";
                                    if (response.message.IsNotEmpty())
                                        AiUrl.LastResultMessage += $" Message: '{response.message}'";

                                }

                            }
                            else if (string.IsNullOrEmpty(ret.Error))
                            {
                                //deserialization did not cause exception, it just gave a null response in the object?
                                //probably wont happen but just making sure
                                ret.Error = $"ERROR: Deserialization of 'Response' from DeepStack failed. response is null. JSON: '{cleanjsonString}'";
                                AiUrl.IncrementError();
                                AiUrl.LastResultMessage = ret.Error;
                            }
                        }
                        catch (Exception ex)
                        {
                            ret.Error = $"ERROR: Deserialization of 'Response' from '{AiUrl.Type.ToString()}' failed: {ex.Msg()}, JSON: '{cleanjsonString}'";
                            AiUrl.IncrementError();
                            AiUrl.LastResultMessage = ret.Error;
                        }


                    }
                    else
                    {
                        if (response != null && !string.IsNullOrEmpty(response.error))
                            ret.Error = $"ERROR: Deepstack returned '{response.error}' - http status code '{output.StatusCode}' ({Convert.ToInt32(output.StatusCode)}) in {swposttime.ElapsedMilliseconds}ms: {output.ReasonPhrase}";
                        else
                            ret.Error = $"ERROR: Got http status code '{output.StatusCode}' ({Convert.ToInt32(output.StatusCode)}) in {swposttime.ElapsedMilliseconds}ms: {output.ReasonPhrase}";

                        AiUrl.IncrementError();
                        AiUrl.LastResultMessage = ret.Error;
                    }

                }
                catch (Exception ex)
                {
                    swposttime.Stop();
                    long seconds = swposttime.ElapsedMilliseconds / 1000;
                    if (seconds >= AiUrl.GetTimeout().TotalSeconds)
                    {
                        ret.Error = $"ERROR: HTTPClient timeout at {seconds} seconds ('HTTPClientTimeoutSeconds' is currently set to {AiUrl.GetTimeout().TotalSeconds} in AITOOL.Settings.JSON file.): {ex.Msg()}";
                    }
                    else
                    {
                        ret.Error = $"ERROR: {ex.Msg()}";
                    }
                    AiUrl.IncrementError();
                    AiUrl.LastResultMessage = ret.Error;
                }
                finally
                {
                    AiUrl.LastTimeMS = swposttime.ElapsedMilliseconds;
                    ret.SWPostTime = swposttime.ElapsedMilliseconds;
                    AiUrl.AITimeCalcs.AddToCalc(AiUrl.LastTimeMS);

                    if (!string.IsNullOrEmpty(ret.Error))
                        DeepStackServerControl.PrintDeepStackError();  //only prints error if we have locally installed windows deepstack and there is a new entry in stderr.txt

                }

            }

            //==============================================================================================================
            //==============================================================================================================
            //==============================================================================================================

            else if (AiUrl.Type.ToString().Has("sighthound"))
            {

                if (string.IsNullOrEmpty(AppSettings.Settings.SightHoundAPIKey))
                {
                    ret.Success = false;
                    ret.Error = $"ERROR: No SightHound API key set. (SightHoundAPIKey in AITOOL.SETTINGS.JSON).'";
                    AiUrl.IncrementError();
                    AiUrl.LastResultMessage = ret.Error;
                    return ret;
                }

                Stopwatch swposttime = new Stopwatch();

                //WebRequest request = null;
                //HttpWebResponse WebResponse = null;
                //Stream requestStream = null;

                MultipartFormDataContent request = null;
                //StreamContent stream = null;

                try
                {
                    long FileSize = new FileInfo(CurImg.image_path).Length;

                    request = new MultipartFormDataContent();

                    //stream = new StreamContent(CurImg.ToStream());
                    //string base64Img = CurImg.ToStream().ConvertToBase64();
                    //using StringContent imgstr = new StringContent(base64Img, Encoding.UTF8, "image/jpeg");  //Encoding.UTF8, "application/json"


                    //Dictionary<string, byte[]> dict = new Dictionary<string, byte[]>();
                    Dictionary<string, string> dict = new Dictionary<string, string>();
                    dict.Add("image", CurImg.ToMemStream().ConvertToBase64());
                    string json = Global.GetJSONString((object)dict); //JsonConvert.SerializeObject((object)dict);
                    //byte[] body = Encoding.UTF8.GetBytes(json);
                    //ByteArrayContent content = new ByteArrayContent(body);
                    //HttpContent content = Global.CreateHttpContentString(dict);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    //request.Add(content);

                    if (!AiUrl.HttpClient.DefaultRequestHeaders.Contains("X-Access-Token"))
                    {
                        AiUrl.HttpClient.DefaultRequestHeaders.Add("X-Access-Token", AppSettings.Settings.SightHoundAPIKey);
                    }

                    //Dictionary<string, byte[]> dict = new Dictionary<string, byte[]>();
                    //dict.Add("image", CurImg.ImageByteArray);
                    //string json = JsonConvert.SerializeObject((object)dict);
                    //byte[] body = Encoding.UTF8.GetBytes(json);

                    //request = WebRequest.Create(AiUrl.ToString());

                    //request.Timeout = AppSettings.Settings.HTTPClientLocalTimeoutSeconds * 1000;

                    //request.Method = "POST";
                    //request.ContentType = "application/json";
                    //request.ContentLength = json.Length;
                    //request.Headers["X-Access-Token"] = AppSettings.Settings.SightHoundAPIKey;

                    Log($"Debug: (1/6) Uploading a {Global.FormatBytes(FileSize)} image ({FileSize} bytes in request) to '{AiUrl.Type}' AI Server at {AiUrl}", AiUrl.CurSrv, cam, CurImg);

                    swposttime.Restart();

                    //requestStream = request.GetRequestStream();
                    //requestStream.Write(body, 0, body.Length);

                    //WebResponse = (HttpWebResponse)request.GetResponse();


                    using HttpResponseMessage output = await AiUrl.HttpClient.PostAsync(AiUrl.ToString(), content, MasterCTS.Token);

                    swposttime.Stop();
                    ret.StatusCode = output.StatusCode;
                    ret.JsonString = await output.Content.ReadAsStringAsync();

                    //ret.StatusCode = WebResponse.StatusCode;

                    //Successful queries will return a 200 (OK) response with a JSON body describing all detected objects and the attributes of the processed image.
                    if (output.IsSuccessStatusCode)
                    {

                        //using StreamReader reader = new StreamReader(WebResponse.GetResponseStream(), Encoding.UTF8);
                        //ret.JsonString = reader.ReadToEnd();

                        swposttime.Stop();

                        if (ret.JsonString != null && !string.IsNullOrWhiteSpace(ret.JsonString))
                        {
                            string cleanjsonString = ret.JsonString.CleanString();

                            try
                            {

                                JObject JOResult = JObject.Parse(ret.JsonString);

                                if (AiUrl.Type == URLTypeEnum.SightHound_Vehicle)
                                {
                                    //Vehicle Recognition

                                    //This can throw an exception
                                    SightHoundVehicleRoot SHObj = JsonConvert.DeserializeObject<SightHoundVehicleRoot>(ret.JsonString);

                                    if (SHObj != null)
                                    {
                                        if (SHObj.Objects != null)
                                        {


                                            if (SHObj.Objects.Count > 0)
                                            {
                                                foreach (SightHoundVehicleObject DSObj in SHObj.Objects)
                                                {
                                                    //Get the vehicle and plate as 2 separate predictions
                                                    ClsPrediction predv = new ClsPrediction(ObjectType.Object, cam, DSObj, CurImg, AiUrl, SHObj.Image, false);
                                                    if (!string.IsNullOrEmpty(predv.Label))
                                                        ret.Predictions.Add(predv);
                                                    ClsPrediction predp = new ClsPrediction(ObjectType.Object, cam, DSObj, CurImg, AiUrl, SHObj.Image, true);
                                                    if (!string.IsNullOrEmpty(predp.Label))
                                                        ret.Predictions.Add(predp);

                                                }
                                            }

                                            ret.Success = true;
                                            AiUrl.LastResultMessage = $"{ret.Predictions.Count} Vehicle predictions found.";
                                        }
                                        else
                                        {
                                            ret.Error = $"ERROR: No Vehicle predictions?  JSON: '{cleanjsonString}')";
                                            AiUrl.IncrementError();
                                            AiUrl.LastResultMessage = ret.Error;
                                        }


                                    }
                                    else if (string.IsNullOrEmpty(ret.Error))
                                    {
                                        //deserialization did not cause exception, it just gave a null response in the object?
                                        //probably wont happen but just making sure
                                        ret.Error = $"ERROR: Deserialization of 'Response' from DeepStack failed. response is null. JSON: '{cleanjsonString}'";
                                        AiUrl.IncrementError();
                                        AiUrl.LastResultMessage = ret.Error;
                                    }
                                }
                                else if (AiUrl.Type == URLTypeEnum.SightHound_Person)
                                {
                                    //face/person detection

                                    //This can throw an exception
                                    SightHoundPersonRoot SHObj = JsonConvert.DeserializeObject<SightHoundPersonRoot>(ret.JsonString);

                                    if (SHObj != null)
                                    {
                                        if (SHObj.Objects != null)
                                        {
                                            if (SHObj.Objects.Count > 0)
                                            {
                                                foreach (SightHoundPersonObject DSObj in SHObj.Objects)
                                                {
                                                    ClsPrediction pred = new ClsPrediction(ObjectType.Object, cam, DSObj, CurImg, AiUrl, SHObj.Image);
                                                    ret.Predictions.Add(pred);
                                                }
                                            }

                                            ret.Success = true;
                                            AiUrl.LastResultMessage = $"{ret.Predictions.Count} Person predictions found.";

                                        }
                                        else
                                        {
                                            ret.Error = $"ERROR: No Person predictions?  JSON: '{cleanjsonString}')";
                                            AiUrl.IncrementError();
                                            AiUrl.LastResultMessage = ret.Error;
                                        }


                                    }
                                    else if (string.IsNullOrEmpty(ret.Error))
                                    {
                                        //deserialization did not cause exception, it just gave a null response in the object?
                                        //probably wont happen but just making sure
                                        ret.Error = $"ERROR: Deserialization of 'Response' from DeepStack failed. response is null. JSON: '{cleanjsonString}'";
                                        AiUrl.IncrementError();
                                        AiUrl.LastResultMessage = ret.Error;
                                    }
                                }


                            }
                            catch (Exception ex)
                            {
                                ret.Error = $"ERROR: Deserialization of 'Response' from '{AiUrl.Type.ToString()}' failed: {ex.Msg()}, JSON: '{cleanjsonString}'";
                                AiUrl.IncrementError();
                                AiUrl.LastResultMessage = ret.Error;
                            }
                        }
                        else
                        {
                            ret.Error = $"ERROR: Empty string returned from HTTP post.";
                            AiUrl.IncrementError();
                            AiUrl.LastResultMessage = ret.Error;
                        }


                    }
                    else
                    {
                        //try to get the error response:
                        //{
                        //        "error": "ERROR MESSAGE"
                        //        "reason": "reason for error",
                        //        "reasonCode": 00000,
                        //        "details": {
                        //              "statusCode": 000,
                        //              "statusMessage": "reason for error",
                        //              "body": "description of error"
                        //             }
                        //    }

                        //{
                        //      "error": "ERROR_MEDIA_DATA",
                        //      "reason": "Cannot identify image format",
                        //      "reasonCode": 40012,
                        //      "details": {
                        //            "statusCode": 406,
                        //            "statusMessage": "Cannot identify image format",
                        //            "body": "image error cannot identify image file (-6)"
                        //      }
                        //}


                        string error = "";
                        swposttime.Stop();
                        if (ret.JsonString != null && !string.IsNullOrWhiteSpace(ret.JsonString))
                        {
                            SightHoundError SHErr = JsonConvert.DeserializeObject<SightHoundError>(ret.JsonString);
                            if (!string.IsNullOrEmpty(SHErr.Details.Body))
                                error = $"{SHErr.Error} (ReasonCode={SHErr.ReasonCode}): {SHErr.Details.Body}";
                            else
                                error = $"{SHErr.Error} (ReasonCode={SHErr.ReasonCode}): {SHErr.Reason}";
                        }
                        else
                        {
                            error = "(EmptyJsonResponse?)";
                        }


                        swposttime.Stop();
                        ret.Error = $"ERROR: Got http status code '{output.StatusCode}' ({Convert.ToInt32(output.StatusCode)}) - '{error}' in {swposttime.ElapsedMilliseconds}ms: {output.ReasonPhrase}";
                        AiUrl.IncrementError();
                        AiUrl.LastResultMessage = ret.Error;
                    }

                }
                catch (Exception ex)
                {

                    string error = "";

                    swposttime.Stop();
                    long seconds = swposttime.ElapsedMilliseconds / 1000;
                    if (seconds >= AiUrl.GetTimeout().TotalSeconds)
                    {
                        ret.Error = $"ERROR: HTTPClient timeout at {seconds} seconds (Max={AiUrl.GetTimeout().TotalSeconds} set in 'HTTPClientTimeoutSeconds' in aitool.settings.json): - '{error}': {ex.Msg()}";
                    }
                    else
                    {
                        ret.Error = $"ERROR: '{error}': {ex.Msg()}";
                    }
                    AiUrl.IncrementError();
                    AiUrl.LastResultMessage = ret.Error;
                }
                finally
                {
                    ret.SWPostTime = swposttime.ElapsedMilliseconds;
                    AiUrl.LastTimeMS = swposttime.ElapsedMilliseconds;
                    AiUrl.AITimeCalcs.AddToCalc(AiUrl.LastTimeMS);
                    if (request != null)
                        request.Dispose();
                }
            }

            //==============================================================================================================
            //==============================================================================================================
            //==============================================================================================================
            else if (AiUrl.Type == URLTypeEnum.DOODS)
            {
                Stopwatch swposttime = new Stopwatch();

                try
                {
                    ClsDoodsRequest cdr = new ClsDoodsRequest();

                    //We could prevent doods from giving back ALL of its results but then we couldnt fully see how it was working:
                    //So many things come back from Doods, cluttering up the db, we really need to limit at the source

                    if (AppSettings.Settings.HistoryRestrictMinThresholdAtSource && !OverrideThreshold)
                        cdr.Detect.MinPercentMatch = cam.threshold_lower;

                    if (AppSettings.Settings.HistoryRestrictMinThresholdAtSource && OverrideThreshold)
                        cdr.Detect.MinPercentMatch = AiUrl.Threshold_Lower;

                    cdr.DetectorName = AppSettings.Settings.DOODSDetectorName;

                    //string testjson = JsonConvert.SerializeObject(cdr);

                    long FileSize = new FileInfo(CurImg.image_path).Length;

                    cdr.Data = CurImg.ToMemStream().ConvertToBase64();

                    using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, AiUrl.ToString()))
                    {

                        using HttpContent httpContent = Global.CreateHttpContentString(cdr);

                        request.Content = httpContent;

                        Log($"Debug: (1/6) Uploading a {Global.FormatBytes(FileSize)} image to '{AiUrl.Type}' AI Server at {AiUrl}", AiUrl.CurSrv, cam, CurImg);


                        //  Got http status code 'RequestEntityTooLarge' (413) in 42ms: Request Entity Too Large
                        swposttime.Restart();

                        using HttpResponseMessage output = await AiUrl.HttpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, MasterCTS.Token);

                        swposttime.Stop();
                        ret.StatusCode = output.StatusCode;


                        if (output.IsSuccessStatusCode)
                        {
                            ret.JsonString = await output.Content.ReadAsStringAsync();

                            if (ret.JsonString != null && !string.IsNullOrWhiteSpace(ret.JsonString))
                            {
                                string cleanjsonString = ret.JsonString.CleanString();

                                ClsDoodsResponse response = null;

                                try
                                {
                                    //This can throw an exception
                                    response = JsonConvert.DeserializeObject<ClsDoodsResponse>(ret.JsonString);

                                    if (response != null)
                                    {
                                        if (response.Detections != null)
                                        {
                                            if (response.Detections.Count > 0)
                                            {

                                                foreach (ClsDoodsDetection DSObj in response.Detections)
                                                {
                                                    ClsPrediction pred = new ClsPrediction(ObjectType.Object, cam, DSObj, CurImg, AiUrl);

                                                    ret.Predictions.Add(pred);

                                                }


                                            }

                                            ret.Success = true;
                                            AiUrl.LastResultMessage = $"{ret.Predictions.Count} predictions found.";


                                        }
                                        else
                                        {
                                            ret.Error = $"ERROR: No predictions?  JSON: '{cleanjsonString}')";
                                            AiUrl.IncrementError();
                                            AiUrl.LastResultMessage = ret.Error;
                                        }


                                    }
                                    else if (string.IsNullOrEmpty(ret.Error))
                                    {
                                        //deserialization did not cause exception, it just gave a null response in the object?
                                        //probably wont happen but just making sure
                                        ret.Error = $"ERROR: Deserialization of 'Response' from DeepStack failed. response is null. JSON: '{cleanjsonString}'";
                                        AiUrl.IncrementError();
                                        AiUrl.LastResultMessage = ret.Error;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ret.Error = $"ERROR: Deserialization of 'Response' from '{AiUrl.Type.ToString()}' failed: {ex.Msg()}, JSON: '{cleanjsonString}'";
                                    AiUrl.IncrementError();
                                    AiUrl.LastResultMessage = ret.Error;
                                }
                            }
                            else
                            {
                                ret.Error = $"ERROR: Empty string returned from HTTP post.";
                                AiUrl.IncrementError();
                                AiUrl.LastResultMessage = ret.Error;
                            }


                        }
                        else
                        {
                            ret.Error = $"ERROR: Got http status code '{output.StatusCode}' ({Convert.ToInt32(output.StatusCode)}) in {swposttime.ElapsedMilliseconds}ms: {output.ReasonPhrase}";
                            AiUrl.IncrementError();
                            AiUrl.LastResultMessage = ret.Error;
                        }

                    }



                }
                catch (Exception ex)
                {
                    swposttime.Stop();

                    long seconds = swposttime.ElapsedMilliseconds / 1000;
                    if (seconds >= AiUrl.GetTimeout().TotalSeconds)
                    {
                        ret.Error = $"ERROR: HTTPClient timeout at {seconds} seconds (Max={AiUrl.GetTimeout().TotalSeconds} set in 'HTTPClientTimeoutSeconds' in aitool.settings.json): {ex.Msg()}";
                    }
                    else
                    {
                        ret.Error = $"ERROR: {ex.Msg()}";
                    }

                    ret.Error = $"ERROR: {ex.Msg()}";
                    AiUrl.IncrementError();
                    AiUrl.LastResultMessage = ret.Error;
                }
                finally
                {
                    ret.SWPostTime = swposttime.ElapsedMilliseconds;
                    AiUrl.LastTimeMS = swposttime.ElapsedMilliseconds;
                    AiUrl.AITimeCalcs.AddToCalc(AiUrl.LastTimeMS);

                }

            }
            //==============================================================================================================
            //==============================================================================================================
            //==============================================================================================================
            else if (AiUrl.Type == URLTypeEnum.AWSRekognition_Objects)
            {
                Stopwatch swposttime = new Stopwatch();

                try
                {

                    //string testjson = JsonConvert.SerializeObject(cdr);

                    long FileSize = new FileInfo(CurImg.image_path).Length;
                    //https://docs.aws.amazon.com/general/latest/gr/rande.html


                    RegionEndpoint endpoint = RegionEndpoint.GetBySystemName(AppSettings.Settings.AmazonRegionEndpoint);
                    AmazonRekognitionClient rekognitionClient = new AmazonRekognitionClient(new BasicAWSCredentials(AppSettings.Settings.AmazonAccessKeyId, AppSettings.Settings.AmazonSecretKey), endpoint);

                    DetectLabelsRequest dlr = new DetectLabelsRequest();

                    dlr.MaxLabels = AppSettings.Settings.AmazonMaxLabels;
                    dlr.MinConfidence = AppSettings.Settings.AmazonMinConfidence;

                    if (AppSettings.Settings.HistoryRestrictMinThresholdAtSource && !OverrideThreshold)
                        dlr.MinConfidence = cam.threshold_lower;

                    if (AppSettings.Settings.HistoryRestrictMinThresholdAtSource && OverrideThreshold)
                        dlr.MinConfidence = AiUrl.Threshold_Lower;

                    Amazon.Rekognition.Model.Image rekognitionImage = new Amazon.Rekognition.Model.Image();

                    //byte[] data = null;

                    //using (FileStream fileStream = new FileStream(CurImg.image_path, FileMode.Open, FileAccess.Read))
                    //{
                    //    data = new byte[fileStream.Length];
                    //    await fileStream.ReadAsync(data, 0, (int)fileStream.Length);
                    //}

                    rekognitionImage.Bytes = CurImg.ToMemStream();

                    dlr.Image = rekognitionImage;


                    Log($"Debug: (1/6) Uploading a {Global.FormatBytes(FileSize)} image to '{AiUrl.Type}' AI Server at {AiUrl}", AiUrl.CurSrv, cam, CurImg);

                    swposttime.Restart();

                    DetectLabelsResponse response = await rekognitionClient.DetectLabelsAsync(dlr);

                    swposttime.Stop();

                    if (response != null)
                    {
                        ret.StatusCode = response.HttpStatusCode;
                        if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                        {
                            if (response.Labels.Count > 0)
                            {

                                foreach (Amazon.Rekognition.Model.Label lbl in response.Labels)
                                {
                                    //not sure if there will ever be more than one instance
                                    for (int i = 0; i < lbl.Instances.Count; i++)
                                    {
                                        ClsPrediction pred = new ClsPrediction(ObjectType.Object, cam, lbl, i, CurImg, AiUrl);

                                        ret.Predictions.Add(pred);

                                    }

                                }


                            }

                            ret.Success = true;
                            AiUrl.LastResultMessage = $"{ret.Predictions.Count} predictions found.";
                        }
                        else
                        {
                            ret.Error = $"ERROR: Amazon Rekognition 'HttpStatusCode' is '{response.HttpStatusCode}' ({Convert.ToInt32(response.HttpStatusCode)}).";
                            AiUrl.IncrementError();
                            AiUrl.LastResultMessage = ret.Error;
                        }
                    }
                    else
                    {
                        ret.Error = $"ERROR: Amazon Rekognition 'Response' is null.";
                        AiUrl.IncrementError();
                        AiUrl.LastResultMessage = ret.Error;
                    }


                }
                catch (Exception ex)
                {
                    swposttime.Stop();

                    ret.Error = $"ERROR: {ex.Msg()}";
                    AiUrl.IncrementError();
                    AiUrl.LastResultMessage = ret.Error;
                }
                finally
                {
                    ret.SWPostTime = swposttime.ElapsedMilliseconds;
                    AiUrl.LastTimeMS = swposttime.ElapsedMilliseconds;
                    AiUrl.AITimeCalcs.AddToCalc(AiUrl.LastTimeMS);
                }

            }
            else if (AiUrl.Type == URLTypeEnum.AWSRekognition_Faces)
            {
                Stopwatch swposttime = new Stopwatch();

                try
                {

                    //string testjson = JsonConvert.SerializeObject(cdr);

                    long FileSize = new FileInfo(CurImg.image_path).Length;
                    //https://docs.aws.amazon.com/general/latest/gr/rande.html


                    RegionEndpoint endpoint = RegionEndpoint.GetBySystemName(AppSettings.Settings.AmazonRegionEndpoint);
                    AmazonRekognitionClient rekognitionClient = new AmazonRekognitionClient(new BasicAWSCredentials(AppSettings.Settings.AmazonAccessKeyId, AppSettings.Settings.AmazonSecretKey), endpoint);

                    DetectFacesRequest dlr = new DetectFacesRequest();

                    //dlr.MaxLabels = AppSettings.Settings.AmazonMaxLabels;
                    //dlr.MinConfidence = AppSettings.Settings.AmazonMinConfidence;

                    //if (AppSettings.Settings.HistoryRestrictMinThresholdAtSource && !OverrideThreshold)
                    //    dlr.MinConfidence = cam.threshold_lower;

                    //if (AppSettings.Settings.HistoryRestrictMinThresholdAtSource && OverrideThreshold)
                    //    dlr.MinConfidence = AiUrl.Threshold_Lower;

                    Amazon.Rekognition.Model.Image rekognitionImage = new Amazon.Rekognition.Model.Image();

                    //byte[] data = null;

                    //using (FileStream fileStream = new FileStream(CurImg.image_path, FileMode.Open, FileAccess.Read))
                    //{
                    //    data = new byte[fileStream.Length];
                    //    await fileStream.ReadAsync(data, 0, (int)fileStream.Length);
                    //}

                    rekognitionImage.Bytes = CurImg.ToMemStream();

                    dlr.Image = rekognitionImage;
                    dlr.Attributes.Add("ALL");

                    Log($"Debug: (1/6) Uploading a {Global.FormatBytes(FileSize)} image to '{AiUrl.Type}' AI Server at {AiUrl}", AiUrl.CurSrv, cam, CurImg);

                    swposttime.Restart();

                    DetectFacesResponse response = await rekognitionClient.DetectFacesAsync(dlr);

                    swposttime.Stop();

                    if (response != null)
                    {
                        ret.StatusCode = response.HttpStatusCode;
                        if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                        {
                            if (response.FaceDetails.Count > 0)
                            {

                                foreach (Amazon.Rekognition.Model.FaceDetail face in response.FaceDetails)
                                {
                                    ClsPrediction pred = new ClsPrediction(ObjectType.Object, cam, face, CurImg, AiUrl);

                                    ret.Predictions.Add(pred);

                                }


                            }

                            ret.Success = true;
                            AiUrl.LastResultMessage = $"{ret.Predictions.Count} predictions found.";
                        }
                        else
                        {
                            ret.Error = $"ERROR: Amazon Rekognition 'HttpStatusCode' is '{response.HttpStatusCode}' ({Convert.ToInt32(response.HttpStatusCode)}).";
                            AiUrl.IncrementError();
                            AiUrl.LastResultMessage = ret.Error;
                        }
                    }
                    else
                    {
                        ret.Error = $"ERROR: Amazon Rekognition 'Response' is null.";
                        AiUrl.IncrementError();
                        AiUrl.LastResultMessage = ret.Error;
                    }


                }
                catch (Exception ex)
                {
                    swposttime.Stop();

                    ret.Error = $"ERROR: {ex.Msg()}";
                    AiUrl.IncrementError();
                    AiUrl.LastResultMessage = ret.Error;
                }
                finally
                {
                    ret.SWPostTime = swposttime.ElapsedMilliseconds;
                    AiUrl.LastTimeMS = swposttime.ElapsedMilliseconds;
                    AiUrl.AITimeCalcs.AddToCalc(AiUrl.LastTimeMS);
                }

            }
            else
            {
                ret.Error = $"Error: URL type not supported yet: '{AiUrl.Type}'";
                AiUrl.LastResultMessage = ret.Error;
            }


            AiUrl.LastResultSuccess = ret.Success || !AiUrl.LastResultMessage.Has("error");

            return ret;

        }


        public static List<ClsPrediction> RemovePredictionDuplicates(List<ClsPrediction> items, Camera cam)
        {
            List<ClsPrediction> result = new List<ClsPrediction>();
            for (int i = 0; i < items.Count; i++)
            {
                // Assume not duplicate.
                bool duplicate = false;
                for (int z = 0; z < i; z++)
                {
                    if (items[z] == items[i] && items[z].ConfidenceString() == items[i].ConfidenceString())
                    {
                        ObjectPosition op1 = new ObjectPosition(items[z].XMin, items[z].XMax, items[z].YMin, items[z].YMax, items[z].Label, items[z].ImageWidth, items[z].ImageHeight, items[z].Camera, items[z].Filename);
                        op1.ScaleConfig = cam.maskManager.ScaleConfig;
                        op1.PercentMatch = cam.maskManager.PercentMatch;
                        ObjectPosition op2 = new ObjectPosition(items[i].XMin, items[i].XMax, items[i].YMin, items[i].YMax, items[i].Label, items[i].ImageWidth, items[i].ImageHeight, items[i].Camera, items[i].Filename);
                        op2.ScaleConfig = cam.maskManager.ScaleConfig;
                        op2.PercentMatch = cam.maskManager.PercentMatch;

                        if (op1 == op2)
                        {
                            // This is a duplicate.
                            duplicate = true;
                            break;
                        }
                    }
                }
                // If not duplicate, add to result.
                if (!duplicate)
                {
                    result.Add(items[i]);
                }
            }
            return result;
        }

        public class ClsPredMatch
        {
            public int Idx = -1;
            public double MatchPercent = 0;
            public List<ClsPrediction> preds = new List<ClsPrediction>();


            public ClsPredMatch(int idx, double matchPercent)
            {
                Idx = idx;
                MatchPercent = matchPercent;

            }
            public ClsPredMatch() { }
        }
        //search for position based on object position
        public static ClsPredMatch ContainsPrediction(ClsPrediction pred, List<ClsPrediction> predictions, Camera cam, bool ObjTypeMustMatch, bool TrueIfInsideOrPartiallyInside)
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.



            ClsPredMatch ret = new ClsPredMatch();

            if (predictions.Count == 0)
                return ret;

            ObjectPosition op1 = new ObjectPosition(pred.XMin, pred.XMax, pred.YMin, pred.YMax, pred.Label, pred.ImageWidth, pred.ImageHeight, pred.Camera, pred.Filename);
            op1.ScaleConfig = cam.maskManager.ScaleConfig;
            op1.PercentMatch = cam.maskManager.PercentMatch;

            for (int i = 0; i < predictions.Count; i++)
            {
                //nope out so we dont include ourself?
                //if (predictions[i].GetHashCode() == pred.GetHashCode())
                //    break;

                if (TrueIfInsideOrPartiallyInside)
                {
                    //for face matching, we dont care if it is not a closely matching rectangle size, in fact it should be smaller but mostly within
                    if (RectangleMatches(predictions[i].GetRectangle(), pred.GetRectangle(), cam.MergePredictionsMinMatchPercent, out double matched, TrueIfInsideOrPartiallyInside))
                    {
                        if (!ObjTypeMustMatch || ObjTypeMustMatch && (predictions[i].ObjType == pred.ObjType || predictions[i].Label.IndexOf(pred.Label, StringComparison.OrdinalIgnoreCase) >= 0))
                        {
                            predictions[i].PercentMatchRefinement = matched;
                            ret.preds.Add(predictions[i]);
                        }
                    }

                }
                else  // to see if we can find a similar sized rectangle using the maskmanager's technique of matching roughly the same size
                {
                    ObjectPosition op2 = new ObjectPosition(predictions[i].XMin, predictions[i].XMax, predictions[i].YMin, predictions[i].YMax, predictions[i].Label, predictions[i].ImageWidth, predictions[i].ImageHeight, predictions[i].Camera, predictions[i].Filename);
                    op2.ScaleConfig = cam.maskManager.ScaleConfig;
                    op2.PercentMatch = cam.MergePredictionsMinMatchPercent;  //cam.maskManager.PercentMatch;
                    if (op1 == op2 && (!ObjTypeMustMatch || ObjTypeMustMatch && (predictions[i].ObjType == pred.ObjType || predictions[i].Label.IndexOf(pred.Label, StringComparison.OrdinalIgnoreCase) >= 0)))
                    {
                        predictions[i].PercentMatchRefinement = op1.LastPercentMatch;
                        ret.preds.Add(predictions[i]);
                    }

                }
            }

            //send only best match
            if (ret.preds.Count > 0)
            {
                //sort so highest match is first:
                ret.preds = ret.preds.OrderByDescending(p => p.PercentMatchRefinement).ToList();
                ret.MatchPercent = ret.preds[0].PercentMatchRefinement;
                pred.PercentMatchRefinement = ret.MatchPercent;
            }


            return ret;
        }

        public static bool RectangleMatches(Rectangle MasterRect, Rectangle CompareRect, double PercentMatch, out double MatchedPercent, bool TrueIfInsideOrPartiallyInside)
        {

            MatchedPercent = MasterRect.IntersectPercent(CompareRect);

            if (TrueIfInsideOrPartiallyInside)
            {
                if (MasterRect.IntersectsWith(CompareRect) || MasterRect.Contains(CompareRect))
                {

                    //trying to match a face in a person rectangle.  The face rectangle can be partially outside the person rectangle so just using Contains doesnt always work.  May need to tweak lower and upper
                    if (MatchedPercent >= 5 && MatchedPercent <= 95)
                        return true;
                }

            }
            else
            {
                if (MatchedPercent >= PercentMatch)
                    return true;
            }

            return false;
        }





        public class DetectObjectsResult
        {
            public bool Success = false;
            public string Error = "";
            public string Message = "";
            public List<ClsURLItem> OutURLs = new List<ClsURLItem>();
            public List<ClsPrediction> OutPredictions = new List<ClsPrediction>();
        }
        //analyze image with AI
        public static async Task<DetectObjectsResult> DetectObjects(ClsImageQueueItem CurImg, List<ClsURLItem> InAiUrls, Camera cam = null)
        {

            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            DetectObjectsResult ret = new DetectObjectsResult();
            ret.OutURLs = InAiUrls;

            foreach (var url in ret.OutURLs)
            {
                url.InUse.WriteFullFence(true);
            }

            //Only set error when there IS an error...

            string filename = Path.GetFileName(CurImg.image_path);

            CurImg.QueueWaitMS = (long)(DateTime.Now - CurImg.TimeAdded).TotalMilliseconds;

            Stopwatch sw = Stopwatch.StartNew();

            long TotalSWPostTime = 0;

            if (cam == null)
                cam = AITOOL.GetCamera(CurImg.image_path);

            cam.last_image_file = CurImg.image_path;

            History hist = null;

            // check if camera is still in the first half of the cooldown. If yes, don't analyze to minimize cpu load.
            //only analyze if 50% of the cameras cooldown time since last detection has passed
            double secs = (DateTime.Now - cam.last_trigger_time.Read()).TotalSeconds;
            double halfcool = cam.cooldown_time_seconds / 2;

            //ClsAIServerResponse asr = new ClsAIServerResponse();
            ClsAIServerResponse[] asrs = new ClsAIServerResponse[] { };
            string AISRV = "";

            foreach (var url in ret.OutURLs)
                AISRV += url.CurSrv + ";";

            AISRV = AISRV.Trim(";".ToCharArray());

            ClsURLItem AiUrl = ret.OutURLs[0];  //default to first one just to have something set

            if (secs >= halfcool)
            {
                try
                {

                    Log($"Debug: Starting analysis of {CurImg.image_path}...", AISRV, cam, CurImg);

                    if (CurImg.IsValid())  //Waits for access and loads into memory if not already loaded
                    {
                        cam.UpdateImageResolutions(CurImg);

                        Log($"Debug: (Image resolution={CurImg.Width}x{CurImg.Height} @ {CurImg.DPI} DPI and {Global.FormatBytes(CurImg.FileSize)})", AISRV, cam, CurImg);

                        string fldr = Path.Combine(Path.GetDirectoryName(AppSettings.Settings.SettingsFileName), "LastCamImages");
                        string file = Path.Combine(fldr, $"{cam.Name}-Last.jpg");
                        //Create a copy of the current image for use in mask manager when the original image was deleted
                        if ((DateTime.Now - LastImageBackupTime.Read()).TotalMinutes >= 60 || !File.Exists(file))
                        {
                            //File.Copy(CurImg.image_path, file, true);
                            CurImg.CopyFileTo(file);
                            LastImageBackupTime.Write(DateTime.Now);
                        }


                        ///====================================================================================================================
                        ///Get the initial predictions=========================================================================================
                        ///====================================================================================================================

                        //Start processing all urls that may be linked at the same time in separate threads
                        List<Task<ClsAIServerResponse>> urltasks = new List<Task<ClsAIServerResponse>>();

                        bool HasLinked = false;
                        foreach (ClsURLItem url in ret.OutURLs)
                        {
                            if (url.LinkServerResults && !url.LinkedResultsServerList.IsEmpty())
                                HasLinked = true;

                            urltasks.Add(Task.Run(() => GetDetectionsFromAIServer(CurImg, url, cam)));
                        }

                        asrs = await Task.WhenAll(urltasks);

                        List<ClsPrediction> initialpredictions = new List<ClsPrediction>();

                        int order = 0;
                        int RelevantPredictionCount = 0;

                        foreach (ClsAIServerResponse asr in asrs)
                        {

                            TotalSWPostTime += asr.SWPostTime;

                            bool IsPrimary = (urltasks.Count == 1) || !HasLinked || (HasLinked && !asr.AIURL.LinkedResultsServerList.IsEmpty());
                            string primdet = "";
                            if (IsPrimary)
                                primdet = "[Primary]";
                            else
                                primdet = "[Linked]";

                            Log($"Debug: (2/6) {primdet} Posted in {asr.SWPostTime}ms, StatusCode='{asr.StatusCode}', Received a {asr.JsonString.Length} byte JSON response: '{asr.JsonString.Truncate()}'", AiUrl.CurSrv, cam, CurImg);
                            Log($"Debug: (3/6) {primdet} Processing {asr.Predictions.Count} results...", AiUrl.CurSrv, cam, CurImg);


                            if (asr.Success)
                            {

                                foreach (ClsPrediction pred in asr.Predictions)
                                {
                                    order++;
                                    pred.AnalyzePrediction(SkipDynamicMaskCheck: true);
                                    if (pred.Result == ResultType.Relevant)
                                        RelevantPredictionCount++;
                                    pred.ServerType = IsPrimary ? ServerType.Primary : ServerType.Linked;
                                    AiUrl = asr.AIURL;
                                    AiUrl.LastResultSuccess = asr.Success;
                                    pred.OriginalOrder = order;
                                    initialpredictions.Add(pred);
                                }
                            }
                            else
                            {
                                ret.Error = asr.Error;
                                //AiUrl.IncrementError();
                                //AiUrl.LastResultMessage = ret.Error;
                                //Only send error if MaxWaitForAIServerTimeoutError is true
                                string ew = "Debug:";
                                if (AppSettings.Settings.MaxWaitForAIServerTimeoutError)
                                    ew = "Error:";
                                if (ret.Error.Has("request timed out"))
                                {
                                    Log(ew + ret.Error, AiUrl.CurSrv, cam, CurImg);
                                }
                                else
                                {
                                    Log(ret.Error, AiUrl.CurSrv, cam, CurImg);
                                }
                            }
                        }


                        ///====================================================================================================================
                        ///Get the refinement predictions======================================================================================
                        ///====================================================================================================================

                        List<ClsPrediction> refinepredictions = new List<ClsPrediction>();


                        //First check and see if the main or linked servers contained any refinement objects, if so, add them to the list:
                        foreach (ClsPrediction pred in initialpredictions)
                        {
                            if (pred.ObjType == ObjectType.Face || pred.ObjType == ObjectType.LicensePlate)
                                refinepredictions.Add(pred);
                        }

                        //see if there are any refinement servers we need to ask for info...
                        if (AIURLListAvailableRefineServerCount.ReadFullFence() > 0 && RelevantPredictionCount > 0)
                        {
                            List<ClsURLItem> RefineURLs = await WaitForNextURL(cam, true, initialpredictions, "", ret.OutURLs);
                            if (RefineURLs.Count > 0)
                            {
                                //Start processing all refinement urls
                                urltasks = new List<Task<ClsAIServerResponse>>();

                                foreach (ClsURLItem url in RefineURLs)
                                    urltasks.Add(Task.Run(() => GetDetectionsFromAIServer(CurImg, url, cam)));

                                asrs = await Task.WhenAll(urltasks);

                                int refineorder = 0;

                                foreach (ClsAIServerResponse asr in asrs)
                                {
                                    AiUrl = asr.AIURL;
                                    AiUrl.LastResultSuccess = asr.Success;
                                    TotalSWPostTime += asr.SWPostTime;

                                    //add to the list of url's we called
                                    if (!ret.OutURLs.Contains(AiUrl))
                                        ret.OutURLs.Add(AiUrl);

                                    Log($"Debug: (2.1/6) [Refinement Server] Posted in {asr.SWPostTime}ms, StatusCode='{asr.StatusCode}', Received a {asr.JsonString.Length} byte JSON response: '{asr.JsonString.Truncate()}'", AiUrl.CurSrv, cam, CurImg);
                                    Log($"Debug: (3.1/6) [Refinement Server] Processing {asr.Predictions.Count} results...", AiUrl.CurSrv, cam, CurImg);


                                    if (asr.Success)
                                    {

                                        foreach (ClsPrediction pred in asr.Predictions)
                                        {
                                            refineorder++;
                                            pred.AnalyzePrediction(SkipDynamicMaskCheck: true);
                                            pred.ServerType = ServerType.Refine;
                                            pred.OriginalOrder = order;
                                            refinepredictions.Add(pred);
                                        }
                                    }
                                    else
                                    {
                                        ret.Error = asr.Error;
                                        //AiUrl.IncrementError();
                                        //AiUrl.LastResultMessage = ret.Error;
                                        Log(ret.Error, AiUrl.CurSrv, cam, CurImg);
                                    }
                                }


                            }
                        }

                        ///====================================================================================================================
                        ///merge the refinement predictions====================================================================================
                        ///====================================================================================================================


                        for (int i = 0; i < refinepredictions.Count; i++)
                        {
                            ClsPrediction RefinePred = refinepredictions[i];

                            Log($"Debug: [Refinement] Processing prediction #{i + 1} of {refinepredictions.Count} - {RefinePred.ToString()} [{RefinePred.Result}]...", AiUrl.CurSrv, cam, CurImg);

                            //See if there are any existing predictions that can be refined...
                            for (int r = 0; r < initialpredictions.Count; r++)
                            {
                                ClsPrediction ExistingPred = initialpredictions[r];
                                //sighthound and deepstack face detection is a rectangle around the face inside the person rectangle so it wont be an exact match.  Try to combine the face detail with person
                                if (RefinePred.ObjType == ObjectType.Face && ExistingPred.ObjType == ObjectType.Person)
                                {
                                    Rectangle personRect = ExistingPred.GetRectangle();
                                    Rectangle faceRect = RefinePred.GetRectangle();

                                    //check for at least an 80% match since various ai servers create different size rectangles 
                                    if (RectangleMatches(personRect, faceRect, cam.MergePredictionsMinMatchPercent, out double matched, true))
                                    {
                                        Log($"Debug: [Refinement]   Added face detail '{ExistingPred.Detail}' (r={r}) from {RefinePred.Server} for original {ExistingPred.Server} detection: {ExistingPred.ToString()} [{ExistingPred.Result}], Rectangle Match={matched.ToPercent()}");
                                        ExistingPred.PercentMatchRefinement = matched;
                                        ExistingPred.RefineMergedCount++;
                                        ExistingPred.Detail = ExistingPred.Detail.Append(RefinePred.Detail, "; ");
                                        //RefinePred.Result = ResultType.RefinementObject;
                                    }
                                    else
                                    {
                                        Log($"Debug: [Refinement]   Did *NOT* match face (r={r}) from {RefinePred.Server} for original {ExistingPred.Server} detection: {ExistingPred.ToString()} [{ExistingPred.Result}], Rectangle Match={matched}%, required={cam.MergePredictionsMinMatchPercent.ToPercent()} (Json CAM setting='MergePredictionsMinMatchPercent')");
                                    }
                                    RefinePred.PercentMatchRefinement = matched;

                                }
                                //try to match a license plate to a vehicle:
                                else if (RefinePred.ObjType == ObjectType.LicensePlate && ExistingPred.ObjType == ObjectType.Vehicle)
                                {
                                    Rectangle VehicleRect = ExistingPred.GetRectangle();
                                    Rectangle PlateRect = RefinePred.GetRectangle();

                                    //check for at least an 80% match since various ai servers create different size rectangles 
                                    if (RectangleMatches(VehicleRect, PlateRect, cam.MergePredictionsMinMatchPercent, out double matched, true))
                                    {
                                        Log($"Debug: [Refinement]   Added license plate detail '{ExistingPred.Detail}' (r={r}) from {RefinePred.Server} for original {ExistingPred.Server} detection: {ExistingPred.ToString()} [{ExistingPred.Result}], Rectangle Match={matched.ToPercent()}");
                                        ExistingPred.PercentMatchRefinement = matched;
                                        ExistingPred.RefineMergedCount++;
                                        ExistingPred.Detail = ExistingPred.Detail.Append(RefinePred.Detail, "; ");
                                    }
                                    else
                                    {
                                        Log($"Debug: [Refinement]   Did *NOT* match license plate (r={r}) from {RefinePred.Server} for original {ExistingPred.Server} detection: {ExistingPred.ToString()} [{ExistingPred.Result}], Rectangle Match={matched.ToPercent()}, required={cam.MergePredictionsMinMatchPercent.ToPercent()} (Json CAM setting='MergePredictionsMinMatchPercent')");
                                    }
                                    RefinePred.PercentMatchRefinement = matched;
                                }
                                else
                                {
                                    //lets just add it for now (new people, trucks, etc from refinement servers)
                                }

                            }

                            if (RefinePred.Result == ResultType.Relevant)
                                RelevantPredictionCount++;

                            RefinePred.OriginalOrder = initialpredictions.Count + 1;
                            initialpredictions.Add(RefinePred);


                        }


                        ///====================================================================================================================
                        ///De-duplicate + merge=================================================================================================
                        ///====================================================================================================================

                        ////lets sort the predictions so that lowest confidence are processed first so they are replaced with duplicates of higher confidence:
                        initialpredictions = initialpredictions.OrderBy(p => p.Result == ResultType.Relevant ? 1 : 999)
                                                               .ThenBy(p => p.ObjectPriority)
                                                               .ThenByDescending(p => p.Confidence).ToList();


                        List<ClsPrediction> predictions = new List<ClsPrediction>();

                        if (AppSettings.Settings.HistoryMergeDuplicatePredictions)
                        {
                            //take duplicates out of the queue as we go...
                            while (initialpredictions.Count > 0)
                            {
                                ClsPrediction TestPred = initialpredictions[0];
                                ClsPredMatch pm = ContainsPrediction(TestPred, initialpredictions, cam, ObjTypeMustMatch: true, TrueIfInsideOrPartiallyInside: false);
                                if (pm.preds.Count > 1) //if only 1, then it is itself
                                {
                                    //We only want to keep the first one of any that look alike, it will already be sorted with high confidence
                                    for (int i = 0; i < pm.preds.Count; i++)
                                    {
                                        if (i > 0)
                                        {
                                            pm.preds[i].Result = pm.preds[i].Result == ResultType.Relevant || pm.preds[i].Result == ResultType.RelevantDuplicateObject ? ResultType.RelevantDuplicateObject : ResultType.DuplicateObject;
                                            pm.preds[i].DupeCount++;
                                            pm.preds[0].Detail = pm.preds[0].Detail.Append(pm.preds[i].Detail, "; ");
                                            if (!pm.preds[0].Label.EqualsIgnoreCase(pm.preds[i].Label))
                                            {
                                                //add the dupe object name into the details column
                                                pm.preds[0].Detail = pm.preds[0].Detail.Append(pm.preds[i].Label, "; ");
                                            }
                                        }
                                        predictions.Add(pm.preds[i]);
                                        initialpredictions.Remove(pm.preds[i]);
                                    }

                                }
                                else
                                {
                                    predictions.Add(TestPred);
                                    initialpredictions.Remove(TestPred);
                                }
                            }


                        }
                        else
                        {
                            predictions.AddRange(initialpredictions);
                        }

                        ///====================================================================================================================
                        ///Run dynamic mask check last so that it does not increase mask counts of duplicate objects===========================
                        ///====================================================================================================================

                        foreach (ClsPrediction pred in predictions)
                        {
                            if (pred.Result == ResultType.Relevant)
                                pred.AnalyzePrediction(SkipDynamicMaskCheck: false);  //this may increase things like hitcount for relevant objects since it is run twice, may want to figure this out later
                        }


                        //sort predictions so most important are at the top
                        predictions = predictions.Distinct().OrderBy(p => p.Result == ResultType.Relevant ? 1 : 999)
                                                            .ThenBy(p => p.ObjectPriority)
                                                            .ThenByDescending(p => p.Confidence).ToList();

                        //save any images with faces
                        foreach (ClsPrediction pred in predictions)
                        {
                            if (pred.ObjType == ObjectType.Face)
                                FaceMan.TryAddFaceFile(CurImg, pred.Label);
                        }

                        string PredictionsJSON = Global.GetJSONString(predictions);

                        ret.OutPredictions = predictions;

                        ///====================================================================================================================
                        ///====================================================================================================================
                        ///====================================================================================================================

                        int cancelactions = 0;
                        if (cam.Action_mqtt_enabled && !cam.Action_mqtt_payload_cancel.IsEmpty())
                            cancelactions++;

                        if (cam.Action_CancelURL_Enabled && cam.cancel_urls.Length > 0)
                            cancelactions++;

                        //process the combined predictions
                        if (predictions.Count > 0)
                        {
                            List<string> relevant_objects = new List<string>(); //list that will be filled with all objects that were detected and are triggering_objects for the camera
                            List<double> objects_confidence = new List<double>(); //list containing ai confidence value of object at same position in List objects
                            List<string> objects_details = new List<string>(); //list containing ai confidence value of object at same position in List objects
                            List<string> objects_position = new List<string>(); //list containing object positions (xmin, ymin, xmax, ymax)

                            List<string> irrelevant_objects = new List<string>(); //list that will be filled with all irrelevant objects
                            List<double> irrelevant_objects_confidence = new List<double>(); //list containing ai confidence value of irrelevant object at same position in List objects
                            List<string> irrelevant_objects_details = new List<string>(); //list containing ai confidence value of irrelevant object at same position in List objects
                            List<string> irrelevant_objects_position = new List<string>(); //list containing irrelevant object positions (xmin, ymin, xmax, ymax)


                            int masked_counter = 0; //this value is incremented if an object is in a masked area
                            int threshold_counter = 0; // this value is incremented if an object does not satisfy the confidence limit requirements
                            int irrelevant_counter = 0; // this value is incremented if an irrelevant (but not masked or out of range) object is detected
                            int error_counter = 0;

                            //if we are not using the local deepstack windows version, this means nothing:
                            DeepStackServerControl.IsActivated = true;

                            bool HasIgnore = false;
                            //Find out if there is even one ignored object to prevent the trigger later
                            foreach (ClsPrediction pred in predictions)
                            {
                                if (pred.Result == ResultType.IgnoredObject)
                                    HasIgnore = true;
                            }

                            //print every detected object with the according confidence-level
                            if (HasIgnore)
                                Log($"Debug:    Detected objects ('Ignored' object found):", AISRV, cam, CurImg);
                            else
                                Log($"Debug:    Detected objects:", AISRV, cam, CurImg);

                            foreach (ClsPrediction pred in predictions)
                            {

                                string clr = "";
                                if (pred.Result != ResultType.Error)
                                    DeepStackServerControl.VisionDetectionRunning = true;

                                if (pred.Result == ResultType.Relevant && !HasIgnore)
                                {
                                    relevant_objects.Add(pred.Label);
                                    objects_confidence.Add(pred.Confidence);
                                    objects_details.Add(pred.Detail);
                                    objects_position.Add($"{pred.XMin.Round(0)},{pred.YMin.Round(0)},{pred.XMax.Round(0)},{pred.YMax.Round(0)}");
                                    clr = "{" + AppSettings.Settings.RectRelevantColor.Name + "}";
                                }
                                else
                                {
                                    clr = "{" + AppSettings.Settings.RectIrrelevantColor.Name + "}";
                                    irrelevant_objects.Add(pred.Label);
                                    irrelevant_objects_details.Add(pred.Detail);
                                    irrelevant_objects_confidence.Add(pred.Confidence);
                                    string position = $"{pred.XMin.Round(0)},{pred.YMin.Round(0)},{pred.XMax.Round(0)},{pred.YMax.Round(0)}";
                                    irrelevant_objects_position.Add(position);

                                    if (pred.Result == ResultType.NoConfidence)
                                    {
                                        threshold_counter++;
                                    }
                                    else if (pred.Result == ResultType.ImageMasked || pred.Result == ResultType.DynamicMasked || pred.Result == ResultType.StaticMasked)
                                    {
                                        clr = "{" + AppSettings.Settings.RectMaskedColor.Name + "}";
                                        masked_counter++;
                                    }
                                    else if (pred.Result == ResultType.Error)
                                    {
                                        clr = "{red}";
                                        error_counter++;
                                    }
                                    else
                                    {
                                        irrelevant_counter++;
                                    }
                                }

                                if (pred.Result == ResultType.Relevant || pred.Result == ResultType.Error)
                                    Log($"     {clr}Result='{pred.Result}', Detail='{pred.ToString()}', ObjType='{pred.ObjType}', ObjectResult={pred.ObjectResult}, DynMaskResult='{pred.DynMaskResult}', DynMaskType='{pred.DynMaskType}', ImgMaskResult='{pred.ImgMaskResult}', ImgMaskType='{pred.ImgMaskType}, PercentOfImage={pred.PercentOfImage.ToPercent()}", pred.Server, cam, CurImg);
                                else
                                    Log($"Debug:     {clr}Result='{pred.Result}', Detail='{pred.ToString()}', ObjType='{pred.ObjType}', ObjectResult={pred.ObjectResult}, DynMaskResult='{pred.DynMaskResult}', DynMaskType='{pred.DynMaskType}', ImgMaskResult='{pred.ImgMaskResult}', ImgMaskType='{pred.ImgMaskType}'", pred.Server, cam, CurImg);

                            }


                            //mark the end of AI detection for the current image
                            cam.maskManager.LastDetectionDate = DateTime.Now;


                            //if one or more objects were detected, that are 1. relevant, 2. within confidence limits and 3. outside of masked areas
                            if (relevant_objects.Count > 0)
                            {
                                //store these last detections for the specific camera
                                cam.last_detections = relevant_objects;
                                cam.last_confidences = objects_confidence;
                                cam.last_details = objects_details;
                                cam.last_positions = objects_position;
                                cam.last_image_file_with_detections = CurImg.image_path;

                                //the new way


                                //create summary string for this detection
                                StringBuilder detectionsTextSb = new StringBuilder();
                                for (int i = 0; i < relevant_objects.Count; i++)
                                {
                                    detectionsTextSb.Append($"{relevant_objects[i]} {String.Format(AppSettings.Settings.DisplayPercentageFormat, objects_confidence[i])}; "); // String.Format("{0} ({1}%) | ", objects[i], Math.Round((objects_confidence[i] * 100), 2)));
                                }

                                cam.last_detections_summary = detectionsTextSb.ToString().Trim("; ".ToCharArray());

                                //create text string objects and confidences
                                string objects_and_confidences = "";
                                string object_positions_as_string = "";
                                //for (int i = 0; i < objects.Count; i++)
                                //{
                                //    objects_and_confidences += $"{objects[i]} {String.Format(AppSettings.Settings.DisplayPercentageFormat, objects_confidence[i])}; ";
                                //    object_positions_as_string += $"{objects_position[i]};";
                                //}

                                foreach (ClsPrediction pred in predictions)
                                {
                                    if (pred.Result != ResultType.Relevant && AppSettings.Settings.HistoryOnlyDisplayRelevantObjects)
                                        continue;

                                    objects_and_confidences += $"{pred.ToString()}; ";
                                    object_positions_as_string += $"{pred.PositionString()};";
                                }

                                objects_and_confidences = objects_and_confidences.Trim("; ".ToCharArray());

                                Log($"Debug: The summary:" + cam.last_detections_summary, AISRV, cam, CurImg);

                                Log($"Debug: (5/6) Performing alert actions:", AISRV, cam, CurImg);


                                hist = new History().Create(CurImg.image_path, DateTime.Now, cam.Name, objects_and_confidences, object_positions_as_string, true, PredictionsJSON, AISRV, TotalSWPostTime, true);

                                await TriggerActionQueue.AddTriggerActionAsync(TriggerType.All, cam, CurImg, hist, true, !cam.Action_queued, AISRV, ""); //make TRIGGER

                                cam.IncrementAlerts(); //stats update
                                Log($"Debug: (6/6) SUCCESS.", AISRV, cam, CurImg);

                                //add to history list
                                //Log($"Debug: Adding detection to history list.", AiUrl.CurSrv, cam.name);
                                Global.CreateHistoryItem(hist);

                            }
                            //if no object fulfills all 3 requirements but there are other objects: 
                            else if (irrelevant_objects.Count > 0)
                            {
                                //IRRELEVANT ALERT

                                //retrieve confidences and positions
                                string objects_and_confidences = "";
                                string object_positions_as_string = "";

                                //for (int i = 0; i < irrelevant_objects.Count; i++)
                                //{
                                //    objects_and_confidences += $"{irrelevant_objects[i]} {String.Format(AppSettings.Settings.DisplayPercentageFormat, irrelevant_objects_confidence[i])}; "; // ({Math.Round((irrelevant_objects_confidence[i] * 100), 0)}%); ";
                                //    object_positions_as_string += $"{irrelevant_objects_position[i]};";
                                //}

                                foreach (ClsPrediction pred in predictions)
                                {
                                    //if (pred.Result != ResultType.Relevant)
                                    //{
                                    objects_and_confidences += $"{pred.ToString()}; ";
                                    object_positions_as_string += $"{pred.PositionString()};";
                                    //}
                                }


                                objects_and_confidences = objects_and_confidences.Trim("; ".ToCharArray());

                                //string text contains what is written in the log and in the history list
                                string text = "";
                                if (masked_counter > 0)//if masked objects, add them
                                {
                                    text += $"{masked_counter}x masked; ";
                                }
                                if (threshold_counter > 0)//if objects out of confidence range, add them
                                {
                                    text += $"{threshold_counter}x not in confidence range; ";
                                }
                                if (irrelevant_counter > 0) //if other irrelevant objects, add them
                                {
                                    text += $"{irrelevant_counter}x irrelevant; ";
                                }
                                if (error_counter > 0) //if other irrelevant objects, add them
                                {
                                    text += $"{error_counter}x errors; ";
                                }

                                if (text != "") //remove last ";"
                                {
                                    text = text.Remove(text.Length - 2);
                                }


                                Log($"Debug: {text}, so it's an irrelevant alert.", AISRV, cam, CurImg);


                                hist = new History().Create(CurImg.image_path, DateTime.Now, cam.Name, $"{text} : {objects_and_confidences}", object_positions_as_string, false, PredictionsJSON, AISRV, TotalSWPostTime, false);

                                if (cancelactions > 0)
                                {
                                    Log($"Debug: (5/6) Performing {cancelactions} CANCEL actions:", AISRV, cam, CurImg);
                                    await TriggerActionQueue.AddTriggerActionAsync(TriggerType.Cancel, cam, CurImg, hist, false, !cam.Action_queued, AISRV, ""); //make TRIGGER
                                }
                                else
                                    Log($"Debug: (5/6) NO CANCEL actions defined, skipping.", AISRV, cam, CurImg);


                                cam.IncrementIrrelevantAlerts(); //stats update
                                Log($"Debug: (6/6) Camera {cam.Name} caused an irrelevant alert.", AISRV, cam, CurImg);

                                //add to history list
                                Global.CreateHistoryItem(hist);
                            }
                        }
                        else
                        {
                            Log($"Debug:      ((NO DETECTED OBJECTS))", AISRV, cam, CurImg);
                            // FALSE ALERT

                            cam.IncrementFalseAlerts(); //stats update

                            hist = new History().Create(CurImg.image_path, DateTime.Now, cam.Name, "false alert", "", false, "", AISRV, TotalSWPostTime, false);

                            if (cancelactions > 0)
                            {
                                Log($"Debug: (5/6) Performing {cancelactions} CANCEL actions:", AISRV, cam, CurImg);
                                await TriggerActionQueue.AddTriggerActionAsync(TriggerType.Cancel, cam, CurImg, hist, false, !cam.Action_queued, AISRV, ""); //make TRIGGER
                            }
                            else
                                Log($"Debug: (5/6) NO CANCEL actions defined, skipping.", AISRV, cam, CurImg);


                            Log($"Debug: (6/6) Camera {cam.Name} caused a false alert, nothing detected.", AISRV, cam, CurImg);

                            //add to history list
                            Global.CreateHistoryItem(hist);
                        }


                    }
                    else
                    {
                        //could not access the file for 30 seconds??   Or unexpected error
                        ret.Error = $"Error: Last Image message: '{CurImg.LastError}'.  ({CurImg.FileLockMS}ms, with {CurImg.FileLockErrRetryCnt} retries)";
                        CurImg.ErrCount.AtomicIncrementAndGet();
                        CurImg.ResultMessage = ret.Error;
                        Log(ret.Error, AISRV, cam, CurImg);
                    }

                }
                catch (Exception ex)
                {

                    ret.Error = $"ERROR: {ex.Msg()}";
                    AiUrl.IncrementError();
                    AiUrl.LastResultMessage = ret.Error;
                    AiUrl.LastResultSuccess = false;
                    Log(ret.Error, AISRV, cam, CurImg);
                }
                //exitfunction:
                if (!string.IsNullOrEmpty(ret.Error) && AppSettings.Settings.send_telegram_errors && cam.telegram_enabled && !(cam.Paused && cam.PauseTelegram))
                {
                    //bool success = await TelegramUpload(CurImg, "Error");
                    if (hist == null)
                    {
                        hist = new History().Create(CurImg.image_path, DateTime.Now, cam.Name, "error", "", false, "", AiUrl.CurSrv, TotalSWPostTime, false);
                    }
                    await TriggerActionQueue.AddTriggerActionAsync(TriggerType.TelegramImageUpload, cam, CurImg, hist, false, !cam.Action_queued, AiUrl.CurSrv, "Error"); //make TRIGGER
                }


                //I notice deepstack takes a lot longer the very first run?

                CurImg.TotalTimeMS = (long)(DateTime.Now - CurImg.TimeAdded).TotalMilliseconds; //sw.ElapsedMilliseconds + CurImg.QueueWaitMS + CurImg.FileLockMS;
                CurImg.DeepStackTimeMS = TotalSWPostTime;
                CurImg.LifeTimeMS = (long)(DateTime.Now - CurImg.TimeCreated).TotalMilliseconds;
                tcalc.AddToCalc(CurImg.TotalTimeMS);
                qcalc.AddToCalc(CurImg.QueueWaitMS);
                fcalc.AddToCalc(CurImg.FileLockMS);
                lcalc.AddToCalc(CurImg.FileLoadMS);
                icalc.AddToCalc(CurImg.LifeTimeMS);

                Log($"Debug:          Total Time: {CurImg.TotalTimeMS.ToString().PadLeft(6)} ms (Count={tcalc.Count.ToString().PadLeft(6)}, Min={tcalc.MinS.PadLeft(6)} ms, Max={tcalc.MaxS.PadLeft(6)} ms, Avg={tcalc.AvgS.PadLeft(6)} ms)", AiUrl.CurSrv, cam, CurImg);

                foreach (ClsURLItem url in ret.OutURLs)
                {
                    Log($"Debug:       AI (URL) Time: {url.LastTimeMS.ToString().PadLeft(6)} ms (Count={url.AITimeCalcs.Count.ToString().PadLeft(6)}, Min={url.AITimeCalcs.MinS.PadLeft(6)} ms, Max={url.AITimeCalcs.MaxS.PadLeft(6)} ms, Avg={url.AITimeCalcs.AvgS.PadLeft(6)} ms)", url.CurSrv, cam, CurImg);
                }

                Log($"Debug:      File lock Time: {CurImg.FileLockMS.ToString().PadLeft(6)} ms (Count={fcalc.Count.ToString().PadLeft(6)}, Min={fcalc.MinS.PadLeft(6)} ms, Max={fcalc.MaxS.PadLeft(6)} ms, Avg={fcalc.AvgS.PadLeft(6)} ms)", AiUrl.CurSrv, cam, CurImg);
                Log($"Debug:      File load Time: {CurImg.FileLoadMS.ToString().PadLeft(6)} ms (Count={lcalc.Count.ToString().PadLeft(6)}, Min={lcalc.MinS.PadLeft(6)} ms, Max={lcalc.MaxS.PadLeft(6)} ms, Avg={lcalc.AvgS.PadLeft(6)} ms)", AiUrl.CurSrv, cam, CurImg);
                Log($"Debug:    Image Queue Time: {CurImg.QueueWaitMS.ToString().PadLeft(6)} ms (Count={qcalc.Count.ToString().PadLeft(6)}, Min={qcalc.MinS.PadLeft(6)} ms, Max={qcalc.MaxS.PadLeft(6)} ms, Avg={qcalc.AvgS.PadLeft(6)} ms)", AiUrl.CurSrv, cam, CurImg);
                Log($"Debug:     Image Life Time: {CurImg.LifeTimeMS.ToString().PadLeft(6)} ms (Count={icalc.Count.ToString().PadLeft(6)}, Min={icalc.MinS.PadLeft(6)} ms, Max={icalc.MaxS.PadLeft(6)} ms, Avg={icalc.AvgS.PadLeft(6)} ms)", AiUrl.CurSrv, cam, CurImg);
                Log($"Debug:   Image Queue Depth: {CurImg.CurQueueSize.ToString().PadLeft(6)}    (Count={scalc.Count.ToString().PadLeft(6)}, Min={scalc.MinS.PadLeft(6)},    Max={scalc.MaxS.PadLeft(6)},    Avg={scalc.AvgS.PadLeft(6)})", AiUrl.CurSrv, cam, CurImg);

            }
            else
            {
                cam.stats_skipped_images++;
                cam.stats_skipped_images_session++;
                Log($"Skipping detection for '{filename}' because cooldown has not been met for camera '{cam.Name}':  '{secs.Round()}' of '{halfcool.Round()}' seconds (half of trigger cooldown time), Session Skip Count={cam.stats_skipped_images_session}", AiUrl.CurSrv, cam, CurImg);
                Global.CreateHistoryItem(new History().Create(CurImg.image_path, DateTime.Now, cam.Name, $"Skipped image, cooldown was '{secs.Round()}' of '{halfcool.Round()}' seconds.", "", false, "", AiUrl.CurSrv, TotalSWPostTime, false));
            }

            ret.Success = (ret.Error == "");

            foreach (var url in ret.OutURLs)
            {
                if (url.InUse.ReadFullFence())
                {
                    url.InUse.WriteFullFence(false);
                    url.FullTimeMS = (int)(DateTime.Now - url.LastUsedTime.Read()).TotalMilliseconds;
                }
            }

            return ret;

        }


        public static string GetMaskFile(string cameraname)
        {
            Camera cam = GetCamera(cameraname, false);
            if (cam != null)
                return cam.GetMaskFile(true);
            else
                return "";
        }



        public static MaskResultInfo Outsidemask(Camera cam, double xmin, double xmax, double ymin, double ymax, int width, int height)
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            //Log($"      Checking if object is outside privacy mask of {cameraname}:");
            //Log("         Loading mask file...");
            MaskResultInfo ret = new MaskResultInfo();
            string fileType = "";
            string foundfile = "";
            try
            {

                foundfile = cam.GetMaskFile(true);

                if (!string.IsNullOrEmpty(foundfile) && System.IO.File.Exists(foundfile))
                {
                    Log($"Trace:     ->Using found mask file {foundfile}...");
                    fileType = Path.GetExtension(foundfile).ToLower();
                }
                else
                {
                    Log($"Trace:     ->Camera has no mask file yet");
                    ret.IsMasked = false;
                    ret.MaskType = MaskType.None;
                    ret.Result = MaskResult.NoMaskImageFile;
                    return ret;
                }

                //load mask file (in the image all places that have color (transparency > 9 [0-255 scale]) are masked)
                using (var mask_img = new Bitmap(foundfile))
                {
                    //if any coordinates of the object are outside of the mask image, th mask image must be too small.
                    if (mask_img.Width != width || mask_img.Height != height)
                    {
                        Log($"ERROR: The resolution of the mask '{foundfile}' does not equal the resolution of the processed image. Skipping privacy mask feature. Image: {width}x{height}, Mask: {mask_img.Width}x{mask_img.Height}");
                        ret.IsMasked = false;
                        ret.MaskType = MaskType.Image;
                        ret.Result = MaskResult.Error;
                        return ret;
                    }

                    //relative x and y locations of the 9 detection points
                    double[] x_factor = new double[] { 0.25, 0.5, 0.75, 0.25, 0.5, 0.75, 0.25, 0.5, 0.75 };
                    double[] y_factor = new double[] { 0.25, 0.25, 0.25, 0.5, 0.5, 0.5, 0.75, 0.75, 0.75 };

                    //int result = 0; //counts how many of the 9 points are outside of masked area(s)

                    //check the transparency of the mask image in all 9 detection points
                    for (int i = 0; i < 9; i++)
                    {
                        //get image point coordinates (and converting double to int)
                        double x = xmin + (xmax - xmin) * x_factor[i];
                        double y = ymin + (ymax - ymin) * y_factor[i];

                        // Get the color of the pixel
                        System.Drawing.Color pixelColor = mask_img.GetPixel(x.ToInt(), y.ToInt());

                        if (fileType == ".png")
                        {
                            //if the pixel is transparent (A refers to the alpha channel), the point is outside of masked area(s)
                            if (pixelColor.A < 10)
                            {
                                ret.ImagePointsOutsideMask++;
                            }
                        }
                        else
                        {
                            if (pixelColor.A == 0)  // object is in a transparent section of the image (not masked)
                            {
                                ret.ImagePointsOutsideMask++;
                            }
                        }

                    }

                    if (ret.ImagePointsOutsideMask > 4) //if 5 or more of the 9 detection points are outside of masked areas, the majority of the object is outside of masked area(s)
                    {
                        if (ret.ImagePointsOutsideMask == 9)
                        {
                            Log($"Trace:      ->ALL of the object is OUTSIDE of masked area(s). ({ret.ImagePointsOutsideMask} of 9 points)");
                            ret.IsMasked = false;
                            ret.MaskType = MaskType.Image;
                            ret.Result = MaskResult.MajorityOutsideMask;
                            return ret;
                        }
                        else
                        {
                            Log($"Trace:      ->Most of the object is OUTSIDE of masked area(s). ({ret.ImagePointsOutsideMask} of 9 points)");
                            ret.IsMasked = false;
                            ret.MaskType = MaskType.Image;
                            ret.Result = MaskResult.CompletlyOutsideMask;
                            return ret;
                        }
                    }

                    else //if 4 or less of 9 detection points are outside, then 5 or more points are in masked areas and the majority of the object is so too
                    {
                        if (ret.ImagePointsOutsideMask == 0)
                        {
                            Log($"Trace:      ->All of the object is INSIDE a masked area. ({ret.ImagePointsOutsideMask} of 9 points were outside)");
                            ret.IsMasked = true;
                            ret.MaskType = MaskType.Image;
                            ret.Result = MaskResult.CompletlyInsideMask;
                            return ret;

                        }
                        else
                        {
                            Log($"Trace:      ->Most of the object is INSIDE a masked area. ({ret.ImagePointsOutsideMask} of 9 points were outside)");
                            ret.IsMasked = true;
                            ret.MaskType = MaskType.Image;
                            ret.Result = MaskResult.MajorityInsideMask;
                            return ret;
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                Log($"ERROR: While loading the mask file {foundfile}: {ex.Msg()}");
                ret.IsMasked = false;
                ret.MaskType = MaskType.Image;
                ret.Result = MaskResult.Error;
                return ret;
            }

        }

        public static string ReplaceParams(Camera cam, History hist, ClsImageQueueItem CurImg, string instr, Global.IPType iptype, ClsPrediction curpred = null)
        {
            string ret = instr;

            try
            {
                string camname = "TESTCAMERANAME";
                string prefix = "TESTCAMERANAMEPREFIX";
                string imgpath = "C:\\TESTFILE.jpg";
                string caminputfolder = "C:\\TESTFOLDER";

                if (cam != null)
                {
                    camname = cam.BICamName;
                    prefix = cam.Prefix;
                    caminputfolder = cam.input_path;
                }

                if (CurImg != null)
                {
                    imgpath = CurImg.image_path;
                }
                else if (hist != null)
                {
                    imgpath = hist.Filename;
                }
                else if (cam != null)
                {
                    imgpath = cam.last_image_file;
                }

                //handle environment variables too:
                ret = Environment.ExpandEnvironmentVariables(ret);

                //handle date and time:
                ret = Global.ReplaceCaseInsensitive(ret, "%date%", DateTime.Now.ToString("d"));
                ret = Global.ReplaceCaseInsensitive(ret, "%time%", DateTime.Now.ToString("t"));
                ret = Global.ReplaceCaseInsensitive(ret, "%datetime%", DateTime.Now.ToString(AppSettings.Settings.DateFormat));


                //handle custom variables
                ret = Global.ReplaceCaseInsensitive(ret, "[camera]", camname);
                ret = Global.ReplaceCaseInsensitive(ret, "[prefix]", prefix);
                ret = Global.ReplaceCaseInsensitive(ret, "[caminputfolder]", caminputfolder);
                ret = Global.ReplaceCaseInsensitive(ret, "[inputfolder]", AppSettings.Settings.input_path);
                ret = Global.ReplaceCaseInsensitive(ret, "[imagepath]", imgpath); //gives the full path of the image that caused the trigger
                ret = Global.ReplaceCaseInsensitive(ret, "[imagepathescaped]", Uri.EscapeUriString(imgpath)); //gives the full path of the image that caused the trigger
                ret = Global.ReplaceCaseInsensitive(ret, "[imagefilename]", Path.GetFileName(imgpath)); //gives the image name of the image that caused the trigger
                ret = Global.ReplaceCaseInsensitive(ret, "[imagefilenamenoext]", Path.GetFileNameWithoutExtension(imgpath)); //gives the image name of the image that caused the trigger

                if (!AppSettings.Settings.DefaultUserName.IsEmpty())
                    ret = Global.ReplaceCaseInsensitive(ret, "[username]", AppSettings.Settings.DefaultUserName); //gives the image name of the image that caused the trigger

                string pw = AppSettings.Settings.DefaultPasswordEncrypted.Decrypt();
                if (!pw.IsEmpty())
                    ret = Global.ReplaceCaseInsensitive(ret, "[password]", pw); //gives the image name of the image that caused the trigger

                if (BlueIrisInfo.Result == BlueIrisResult.Valid)
                {
                    ret = Global.ReplaceCaseInsensitive(ret, "[blueirisserverip]", Global.IP2Str(AppSettings.Settings.BlueIrisServer.Trim(), iptype)); //gives the image name of the image that caused the trigger
                    ret = Global.ReplaceCaseInsensitive(ret, "[blueirisurl]", BlueIrisInfo.URL); //gives the image name of the image that caused the trigger
                }


                if (hist != null || curpred != null)
                {
                    List<ClsPrediction> preds = new List<ClsPrediction>();

                    if (hist != null)
                        preds = hist.Predictions();
                    else if (curpred != null)
                        preds.Add(curpred);

                    List<ClsDeepstackDetection> detectionslst = new List<ClsDeepstackDetection>();

                    if (preds != null && preds.Count > 0)
                    {
                        string detections = "";
                        string confidences = "";

                        foreach (ClsPrediction pred in preds)
                        {
                            if (pred.Result != ResultType.Relevant && AppSettings.Settings.HistoryOnlyDisplayRelevantObjects)
                                continue;

                            detectionslst.Add(pred.ToDeepstackDetection());

                            confidences += pred.ConfidenceString() + ",";
                            detections += pred.ToString() + ",";
                        }
                        ret = Global.ReplaceCaseInsensitive(ret, "[summarynonescaped]", hist.Detections); //summary text including all detections and confidences, p.e."person (91,53%)"
                        ret = Global.ReplaceCaseInsensitive(ret, "[summary]", Uri.EscapeUriString(hist.Detections)); //summary text including all detections and confidences, p.e."person (91,53%)"
                        ret = Global.ReplaceCaseInsensitive(ret, "[detection]", preds[0].ToString()); //only gives first detection 
                        ret = Global.ReplaceCaseInsensitive(ret, "[label]", preds[0].Label); //only gives first detection 
                        ret = Global.ReplaceCaseInsensitive(ret, "[detail]", preds[0].Detail);
                        ret = Global.ReplaceCaseInsensitive(ret, "[detailescaped]", Uri.EscapeUriString(preds[0].Detail));
                        ret = Global.ReplaceCaseInsensitive(ret, "[result]", preds[0].Result.ToString());
                        ret = Global.ReplaceCaseInsensitive(ret, "[percentofimage]", preds[0].PercentOfImage.Round().ToString());
                        ret = Global.ReplaceCaseInsensitive(ret, "[position]", preds[0].PositionString());
                        ret = Global.ReplaceCaseInsensitive(ret, "[confidence]", preds[0].ConfidenceString());
                        ret = Global.ReplaceCaseInsensitive(ret, "[detections]", detections.Trim(",".ToCharArray()));
                        ret = Global.ReplaceCaseInsensitive(ret, "[confidences]", confidences.Trim(",".ToCharArray()));

                    }
                    else
                    {
                        ret = Global.ReplaceCaseInsensitive(ret, "[summarynonescaped]", "Test Summary");
                        ret = Global.ReplaceCaseInsensitive(ret, "[summary]", "Test Summary"); //summary text including all detections and confidences, p.e."person (91,53%)"
                        ret = Global.ReplaceCaseInsensitive(ret, "[detection]", "Test Detection");
                        ret = Global.ReplaceCaseInsensitive(ret, "[label]", "Person");
                        ret = Global.ReplaceCaseInsensitive(ret, "[detail]", "Test Detail");
                        ret = Global.ReplaceCaseInsensitive(ret, "[detailescaped]", "Test Detail");
                        ret = Global.ReplaceCaseInsensitive(ret, "[result]", "Relevant");
                        ret = Global.ReplaceCaseInsensitive(ret, "[position]", "0,0,0,0");
                        ret = Global.ReplaceCaseInsensitive(ret, "[percentofimage]", "50.123");
                        ret = Global.ReplaceCaseInsensitive(ret, "[confidence]", string.Format(AppSettings.Settings.DisplayPercentageFormat, 99.123));
                        ret = Global.ReplaceCaseInsensitive(ret, "[detections]", "Detection1, Detection2");
                        ret = Global.ReplaceCaseInsensitive(ret, "[confidences]", string.Format(AppSettings.Settings.DisplayPercentageFormat, 99.123) + ", " + string.Format(AppSettings.Settings.DisplayPercentageFormat, 90.01));
                    }

                    if (ret.IndexOf("[Summaryjson]", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        ret = Global.ReplaceCaseInsensitive(ret, "[SummaryJson]", "{\"summary\": \"" + Uri.EscapeUriString(hist.Detections) + "\"}");
                    }

                    if (ret.IndexOf("[Detectionsjson]", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        string jsonstr = GetJSONString(detectionslst.ToArray(), Formatting.None).CleanString();
                        ret = Global.ReplaceCaseInsensitive(ret, "[DetectionsJson]", "{\"detections\": \"" + jsonstr + "\"}");
                    }

                    if (ret.IndexOf("[alljson]", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        AllJson json = new AllJson();
                        json.Time = hist.Date;
                        json.Camera = cam.Name;
                        json.analysisDurationMS = hist.analysisDurationMS;
                        json.fileName = hist.Filename;
                        json.baseName = Path.GetFileName(hist.Filename);
                        json.summary = hist.Detections;
                        json.state = hist.state;
                        json.predictions = detectionslst.ToArray();
                        string jsonstr = GetJSONString(json, Formatting.None, TypeNameHandling.None, PreserveReferencesHandling.None).CleanString();
                        ret = Global.ReplaceCaseInsensitive(ret, "[alljson]", jsonstr);
                    }

                }
                else if (cam != null)
                {
                    if (cam.last_detections != null && cam.last_detections.Count > 0)
                    {
                        ret = Global.ReplaceCaseInsensitive(ret, "[summarynonescaped]", cam.last_detections_summary); //summary text including all detections and confidences, p.e."person (91,53%)"
                        ret = Global.ReplaceCaseInsensitive(ret, "[summary]", Uri.EscapeUriString(cam.last_detections_summary)); //summary text including all detections and confidences, p.e."person (91,53%)"
                        ret = Global.ReplaceCaseInsensitive(ret, "[detection]", cam.last_detections.ElementAt(0)); //only gives first detection (maybe not most relevant one)
                        ret = Global.ReplaceCaseInsensitive(ret, "[label]", cam.last_detections.ElementAt(0)); //only gives first detection (maybe not most relevant one)
                        ret = Global.ReplaceCaseInsensitive(ret, "[detail]", cam.last_details.ElementAt(0)); //only gives first detection (maybe not most relevant one)
                        ret = Global.ReplaceCaseInsensitive(ret, "[detailescaped]", Uri.EscapeUriString(cam.last_details.ElementAt(0))); //only gives first detection (maybe not most relevant one)
                        ret = Global.ReplaceCaseInsensitive(ret, "[position]", cam.last_positions.ElementAt(0));
                        ret = Global.ReplaceCaseInsensitive(ret, "[confidence]", string.Format(AppSettings.Settings.DisplayPercentageFormat, cam.last_confidences.ElementAt(0)));
                        ret = Global.ReplaceCaseInsensitive(ret, "[result]", "Unknown");
                        ret = Global.ReplaceCaseInsensitive(ret, "[percentofimage]", "50.123");
                        ret = Global.ReplaceCaseInsensitive(ret, "[detections]", string.Join(",", cam.last_detections));
                        ret = Global.ReplaceCaseInsensitive(ret, "[confidences]", string.Join(",", cam.last_confidences.Select(x => x.ToString(AppSettings.Settings.DisplayPercentageFormat))));
                    }
                    else
                    {
                        ret = Global.ReplaceCaseInsensitive(ret, "[summarynonescaped]", "Test Summary");
                        ret = Global.ReplaceCaseInsensitive(ret, "[summary]", "Test Summary"); //summary text including all detections and confidences, p.e."person (91,53%)"
                        ret = Global.ReplaceCaseInsensitive(ret, "[detection]", "Test Detection"); //only gives first detection (maybe not most relevant one)
                        ret = Global.ReplaceCaseInsensitive(ret, "[detail]", "Test Detail");
                        ret = Global.ReplaceCaseInsensitive(ret, "[detailescaped]", "Test Detail");
                        ret = Global.ReplaceCaseInsensitive(ret, "[label]", "Person");
                        ret = Global.ReplaceCaseInsensitive(ret, "[result]", "Relevant");
                        ret = Global.ReplaceCaseInsensitive(ret, "[percentofimage]", "50.123");
                        ret = Global.ReplaceCaseInsensitive(ret, "[position]", "0,0,0,0");
                        ret = Global.ReplaceCaseInsensitive(ret, "[confidence]", string.Format(AppSettings.Settings.DisplayPercentageFormat, 99.123));
                        ret = Global.ReplaceCaseInsensitive(ret, "[detections]", "Detection1, Detection2");
                        ret = Global.ReplaceCaseInsensitive(ret, "[confidences]", string.Format(AppSettings.Settings.DisplayPercentageFormat, 99.123) + ", " + string.Format(AppSettings.Settings.DisplayPercentageFormat, 90.01));
                    }

                    if (ret.IndexOf("[Summaryjson]", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        ret = Global.ReplaceCaseInsensitive(ret, "[SummaryJson]", "{\"summary\": \"Test Summary\"}");
                    }

                    if (ret.IndexOf("[Detectionsjson]", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        ret = Global.ReplaceCaseInsensitive(ret, "[DetectionsJson]", "{\"detections\": \"[]\"}");
                    }


                    if (ret.EqualsIgnoreCase("[alljson]"))
                    {
                        AllJson json = new AllJson();
                        json.analysisDurationMS = 999;
                        json.fileName = imgpath;
                        json.Camera = cam.Name;
                        json.baseName = Path.GetFileName(imgpath);
                        if (cam.last_detections_summary.IsEmpty())
                            cam.last_detections_summary = "Test Detection";
                        json.summary = Uri.EscapeUriString(cam.last_detections_summary);
                        json.state = "on";
                        json.predictions = new List<ClsDeepstackDetection>().ToArray();
                        json.Time = DateTime.Now;
                        string jsonstr = GetJSONString(json, Formatting.None, TypeNameHandling.None, PreserveReferencesHandling.None).CleanString();
                        ret = Global.ReplaceCaseInsensitive(ret, "[alljson]", jsonstr);
                    }



                }

            }
            catch (Exception ex)
            {

                Log($"Error: {ex.Msg()}");
            }

            return ret;

        }

        public static ClsURLItem GetURL(string urlstring)
        {
            ClsURLItem ret = null;

            if (!urlstring.Contains("//"))
            {
                foreach (var url in AppSettings.Settings.AIURLList)
                {
                    if (urlstring.IndexOf(url.CurSrv, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        return url;
                    }
                }
                return ret;
            }

            //first look for exact match - where more than just the URL should match
            ClsURLItem test = new ClsURLItem(urlstring, 0, URLTypeEnum.Unknown);

            int fnd = AppSettings.Settings.AIURLList.IndexOf(test);

            if (fnd > -1)
            {
                ret = AppSettings.Settings.AIURLList[fnd];
            }
            else  //lets do a loose search for just the URL string 
            {
                for (int i = 0; i < AppSettings.Settings.AIURLList.Count; i++)
                {
                    if (!AppSettings.Settings.AIURLList[i].Enabled.ReadFullFence())
                        continue;

                    if (string.Equals(AppSettings.Settings.AIURLList[i].ToString(), test.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        ret = AppSettings.Settings.AIURLList[i];
                        break;
                    }
                }
            }

            return ret;

        }

        public static Camera GetCamera(String ImageOrNameOrPrefix, bool ReturnDefault = true)
        {
            //using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.


            //we got here too early, no warning messages
            if (AppSettings.Settings.CameraList.Count == 0)
                return null;

            List<Camera> cams = new List<Camera>();

            try
            {
                ImageOrNameOrPrefix = ImageOrNameOrPrefix.Trim();

                //search by path or filename prefix if we are passed a full path to image file
                if (ImageOrNameOrPrefix.Contains("\\"))
                {
                    string pth = Path.GetDirectoryName(ImageOrNameOrPrefix);
                    string fname = Path.GetFileNameWithoutExtension(ImageOrNameOrPrefix);

                    //&CAM.%Y%m%d_%H%M%S
                    //AIFOSCAMDRIVEWAY.20200827_131840312.jpg
                    //sgrtgrdg - Kopie (2).jpg

                    //Dont try to break the filename apart, just look at any characters matching the first part of the filename

                    //first look for . or - at end of prefix if not specified.   So CAM1 prefix wont match CAM12
                    foreach (Camera ccam in AppSettings.Settings.CameraList)
                    {
                        //if (!ccam.enabled)
                        //    continue;

                        if (ccam.Prefix.Contains("*") || ccam.Prefix.Contains("?"))
                        {
                            if (Regex.IsMatch(Global.WildCardToRegular(ccam.Prefix), ImageOrNameOrPrefix, RegexOptions.IgnoreCase) || Regex.IsMatch(Global.WildCardToRegular(ccam.Prefix), fname, RegexOptions.IgnoreCase))
                            {
                                if (!cams.Contains(ccam))
                                {
                                    ccam.LastGetCameraMatchResult = "(Regex)";
                                    cams.Add(ccam);
                                }
                            }
                        }
                        else if (ccam.Prefix.EndsWith("-") || ccam.Prefix.EndsWith("."))
                        {
                            if (fname.StartsWith(ccam.Prefix.Trim(), StringComparison.OrdinalIgnoreCase))
                            {
                                {
                                    if (!cams.Contains(ccam))
                                    {
                                        ccam.LastGetCameraMatchResult = "(StartsWith-Prefix-Dot)";
                                        cams.Add(ccam);
                                    }
                                }
                            }
                        }
                        else if (!string.IsNullOrWhiteSpace(ccam.Prefix))
                        {
                            //&CAM.A.%Y%m%d_%H%M%S
                            //AIFOSCAMDRIVEWAY.A.20200827_131840312

                            //loop through so that we always check a match with the LONGEST prefix (between 2 dots), before the shorter ones
                            string tmpfname = fname;
                            int meno = 0;
                            do
                            {
                                meno = tmpfname.LastIndexOf(".");
                                if (meno > 0)
                                {
                                    tmpfname = fname.Substring(0, meno);
                                    if (tmpfname.Equals(ccam.Prefix.Trim(), StringComparison.OrdinalIgnoreCase))
                                    {
                                        if (!cams.Contains(ccam))
                                        {
                                            ccam.LastGetCameraMatchResult = "(Equals-Prefix-AnyDot)";
                                            cams.Add(ccam);
                                        }
                                    }

                                }

                            } while (meno > 0);
                        }
                    }

                    //regular search
                    foreach (Camera ccam in AppSettings.Settings.CameraList)
                    {
                        //if (!ccam.enabled)
                        //    continue;

                        if (!string.IsNullOrWhiteSpace(ccam.Prefix) && fname.StartsWith(ccam.Prefix.Trim(), StringComparison.OrdinalIgnoreCase))
                        {
                            if (!cams.Contains(ccam))
                            {
                                ccam.LastGetCameraMatchResult = "(StartsWith-Prefix)";
                                cams.Add(ccam);
                            }
                        }
                    }


                    //if it is not found, search by the camera input path
                    foreach (Camera ccam in AppSettings.Settings.CameraList)
                    {
                        //if (!ccam.enabled)
                        //    continue;

                        //If the watched path is c:\bi\cameraname but the full path of found file is 
                        //                       c:\bi\cameraname\date\time\randomefilename.jpg 
                        //we just check the beginning of the path
                        if (!String.IsNullOrWhiteSpace(ccam.input_path) && ccam.input_path.Trim().StartsWith(pth, StringComparison.OrdinalIgnoreCase))
                        {
                            if (!cams.Contains(ccam))
                            {
                                ccam.LastGetCameraMatchResult = "(StartsWith-InputPath)";
                                cams.Add(ccam);
                            }
                        }
                    }


                }
                else
                {
                    //find by name - we dont care if the camera is disabled
                    foreach (Camera ccam in AppSettings.Settings.CameraList)
                    {
                        if (ImageOrNameOrPrefix.Equals(ccam.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            if (!cams.Contains(ccam))
                            {
                                ccam.LastGetCameraMatchResult = "(ByAICamName)";
                                cams.Add(ccam);
                            }
                        }
                    }
                    ////find by actual cam name if we have to
                    //foreach (Camera ccam in AppSettings.Settings.CameraList)
                    //{
                    //    if (ImageOrNameOrPrefix.Equals(ccam.BICamName, StringComparison.OrdinalIgnoreCase))
                    //    {
                    //        if (!cams.Contains(ccam))
                    //        {
                    //            ccam.LastGetCameraMatchResult = "(ByBICamName?)";
                    //            cams.Add(ccam);
                    //        }
                    //    }
                    //}

                }


                //if we didnt find a camera see if there is a default camera name we can use without a prefix
                if (cams.Count == 0)
                {
                    Log($"Debug: No enabled camera with the same filename, cameraname, or prefix found for '{ImageOrNameOrPrefix}'");
                    //check if there is a default camera which accepts any prefix, select it
                    if (ReturnDefault)
                    {

                        int i = AppSettings.Settings.CameraList.FindIndex(x => string.Equals(x.Name.Trim(), "default", StringComparison.OrdinalIgnoreCase));

                        if (i == -1)
                            i = AppSettings.Settings.CameraList.FindIndex(x => x.Prefix.Trim() == "");

                        if (i > -1)
                        {
                            if (!cams.Contains(AppSettings.Settings.CameraList[i]))
                                cams.Add(AppSettings.Settings.CameraList[i]);

                            if (!ImageOrNameOrPrefix.EqualsIgnoreCase("none"))
                                Log($"Trace:(Found a DEFAULT camera for '{ImageOrNameOrPrefix}': '{AppSettings.Settings.CameraList[i].Name}')");
                        }
                        else
                        {
                            Log($"Debug: No default camera found. Aborting. (Out of {AppSettings.Settings.CameraList.Count} cameras.)");
                        }
                    }

                }

            }
            catch (Exception ex)
            {

                Log(ex.Msg());
            }

            Camera cam = null;

            if (cams.Count == 0)
            {
                Log($"Debug: Cannot match '{ImageOrNameOrPrefix}' to an existing camera. (Out of {AppSettings.Settings.CameraList.Count} cameras.)");
            }
            else
            {
                cam = cams[0];
                if (cams.Count > 1)
                {
                    Log($"Trace: *** Note: More than one configured camera matched '{ImageOrNameOrPrefix}', using the first one matched: '{cams[0].Name}' {cams[0].LastGetCameraMatchResult} ***");
                    for (int i = 0; i < cams.Count; i++)
                    {
                        Log($"Trace:    ----{i + 1}: Name='{cams[i].Name}', MatchResult={cams[i].LastGetCameraMatchResult}, BICamName={cams[i].BICamName}, Prefix='{cams[i].Prefix}', InputPath='{cams[i].input_path}'");
                    }
                }
            }

            return cam;

        }



    }
}
