using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet.Client.Publishing;
using Newtonsoft.Json;

//for image cutting
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;

//for telegram
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

using Microsoft.WindowsAPICodePack.Dialogs;
using Size = SixLabors.Primitives.Size;
using SizeF = SixLabors.Primitives.SizeF; //for file dialog
using static AITool.Global;
using System.Security.AccessControl;
using System.Drawing;

namespace AITool
{
    public static class AITOOL
    {
        // =============================================================
        // ALL FUNCTIONS HERE THAT MAY EVENTUALLY BE USED IN A SERVICE
        // NO direct UI interaction
        // =============================================================

        public static DeepStack DeepStackServerControl = null;
        public static RichTextBoxEx RTFLogger = null;
        public static LogFileWriter LogWriter = null;
        public static LogFileWriter HistoryWriter = null;
        public static BlueIris BlueIrisInfo = null;
        public static List<ClsURLItem> DeepStackURLList = new List<ClsURLItem>();

        //keep track of timing
        //moving average will be faster for long running process with 1000's of samples
        public static MovingCalcs tcalc = new MovingCalcs(250);
        public static MovingCalcs fcalc = new MovingCalcs(250);
        public static MovingCalcs qcalc = new MovingCalcs(250);
        public static MovingCalcs qsizecalc = new MovingCalcs(250);
        public static int errors = 0;
        public static ConcurrentQueue<ClsImageQueueItem> ImageProcessQueue = new ConcurrentQueue<ClsImageQueueItem>();

        //Instantiate a Singleton of the Semaphore with a value of 1. This means that only 1 thread can be granted access at a time.
        //public static SemaphoreSlim semaphore_detection_running = new SemaphoreSlim(1, 1);

        public static object FileWatcherLockObject = new object();
        public static object ImageLoopLockObject = new object();

        //thread safe dictionary to prevent more than one file being processed at one time
        public static ConcurrentDictionary<string, ClsImageQueueItem> detection_dictionary = new ConcurrentDictionary<string, ClsImageQueueItem>();

        

        public static Dictionary<string, ClsFileSystemWatcher> watchers = new Dictionary<string, ClsFileSystemWatcher>();

        public static async Task ImageQueueLoop()
        {
            //This runs in another thread, waiting for items to appear in the queue and process them one at a time
            try
            {
                //Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;

                ClsImageQueueItem CurImg;

                string LastURLS = AppSettings.Settings.deepstack_url;

                DateTime LastURLCheckTime = DateTime.MinValue;
                DateTime LastCleanDupesTime = DateTime.MinValue;
                bool HasDisabledURLs = false;
                int DisabledLogCount = 0;

                //Start infinite loop waiting for images to come into queue
                ConcurrentQueue<ClsURLItem> DSURLQueue = new ConcurrentQueue<ClsURLItem>();

                while (true)
                {
                    while (!ImageProcessQueue.IsEmpty)
                    {

                        //lock this section so more than one thread will keep the variables in sync
                        lock (ImageLoopLockObject)
                        {
                            //Check to see if we need to get updated URL list
                            if (DeepStackURLList.Count == 0 || LastURLS != AppSettings.Settings.deepstack_url)
                            {
                                Log("Updating/Resetting AI URL list...");
                                List<string> tmp = Global.Split(AppSettings.Settings.deepstack_url, "|;,");
                                DeepStackURLList.Clear();

                                //check to see if any need updating with http or path
                                foreach (string url in tmp)
                                {
                                    DeepStackURLList.Add(new ClsURLItem(url));
                                }
                                Log($"...Found {DeepStackURLList.Count} AI URL's in settings.");

                                LastURLS = AppSettings.Settings.deepstack_url;
                                LastURLCheckTime = DateTime.Now;
                                HasDisabledURLs = false;
                                DisabledLogCount = 0;

                                DSURLQueue = new ConcurrentQueue<ClsURLItem>();

                                foreach (ClsURLItem url in DeepStackURLList)
                                {
                                    if (url.Enabled)
                                        DSURLQueue.Enqueue(url);
                                }
                            }

                            //check to see if we need to reenable all the URLs after they have been disabled for a while
                            if (HasDisabledURLs && (DateTime.Now - LastURLCheckTime).TotalMinutes >= AppSettings.Settings.URLResetAfterDisabledMinutes)
                            {
                                LastURLCheckTime = DateTime.Now;
                                HasDisabledURLs = false;
                                DisabledLogCount = 0;

                                DSURLQueue = new ConcurrentQueue<ClsURLItem>();

                                foreach (ClsURLItem url in DeepStackURLList)
                                {
                                    if (!url.Enabled)
                                    {
                                        url.Enabled = true;
                                        url.ErrCount = 0;
                                        Log("---- Re-enabling disabled URL: " + url);
                                    }

                                    DSURLQueue.Enqueue(url);
                                }
                            }

                        }

                        int ProcImgCnt = 0;
                        int ErrCnt = 0;
                        int TskCnt = 0;

                        while (!ImageProcessQueue.IsEmpty)
                        {
                            //tiny delay to conserve cpu and allow more images to come in the queue if needed
                            await Task.Delay(100);

                            //get the next image
                            if (ImageProcessQueue.TryDequeue(out CurImg))
                            {

                                //get the next url
                                ClsURLItem url = null;
                                if (DSURLQueue.TryDequeue(out url))
                                {
                                    if (url != null && url.Enabled)
                                    {
                                        double lastsecs = Math.Round((DateTime.Now - url.LastUsedTime).TotalSeconds, 0);

                                        if (url.ErrCount == 0 || (url.ErrCount > 0 && (lastsecs >= AppSettings.Settings.MinSecondsBetweenFailedURLRetry)))
                                        {
                                            Log($"Adding task for file '{Path.GetFileName(CurImg.image_path)}' on URL '{url}'");

                                            Interlocked.Increment(ref TskCnt);

                                            url.LastUsedTime = DateTime.Now;

                                            Task.Run(async () =>
                                            {

                                                Global.SendMessage(MessageType.BeginProcessImage, CurImg.image_path);

                                                bool success = await DetectObjects(CurImg, url); //ai process image

                                                Global.SendMessage(MessageType.EndProcessImage, CurImg.image_path);
                                                

                                                if (!success)
                                                {
                                                    Interlocked.Increment(ref ErrCnt);

                                                    if (url.ErrCount > 0)
                                                    {
                                                        if (url.ErrCount < AppSettings.Settings.MaxQueueItemRetries)
                                                        {
                                                            //put url back in queue when done
                                                            Log($"...Problem with AI URL: '{url}' (URL ErrCount={url.ErrCount}, max allowed of {AppSettings.Settings.MaxQueueItemRetries}), URLQueue.Count={DSURLQueue.Count}");
                                                        }
                                                        else
                                                        {
                                                            HasDisabledURLs = true;
                                                            url.Enabled = false;
                                                            Log($"...Error: AI URL for '{url.Type}' failed '{url.ErrCount}' times.  Disabling: '{url}', URLQueue.Count={DSURLQueue.Count}");
                                                        }

                                                    }

                                                    CurImg.IncrementRetryCount();  //even if there was not an error directly accessing the image

                                                    if (CurImg.ErrCount <= AppSettings.Settings.MaxQueueItemRetries && CurImg.RetryCount <= AppSettings.Settings.MaxQueueItemRetries)
                                                    {
                                                        //put back in queue to be processed by another deepstack server
                                                        Log($"...Putting image back in queue due to URL '{url}' problem (Image ErrCount={CurImg.ErrCount}, Image RetryCount={CurImg.RetryCount}, URL ErrCount={url.ErrCount}): '{CurImg.image_path}', ImageProcessQueue.Count={ImageProcessQueue.Count}");
                                                        ImageProcessQueue.Enqueue(CurImg);
                                                    }
                                                    else
                                                    {
                                                        Log($"...Error: Removing image from queue. Image RetryCount={CurImg.RetryCount}, URL ErrCount='{url.ErrCount}': {url}', Image: '{CurImg.image_path}', ImageProcessQueue.Count={ImageProcessQueue.Count}");
                                                    }
                                                }
                                                else
                                                {
                                                    Interlocked.Increment(ref ProcImgCnt);

                                                }


                                                Interlocked.Decrement(ref TskCnt);

                                            });

                                        }
                                        else
                                        {
                                            //lets not fill up the log if there are no more enabled URL's...
                                            if (DisabledLogCount < 10)
                                            {
                                                DisabledLogCount++;
                                                Log($"Skipping AI URL because of previous problem; minimum seconds between attempts has not been reached ({lastsecs} of {AppSettings.Settings.MinSecondsBetweenFailedURLRetry} secs, ErrCnt={url.ErrCount}): {url}");
                                            }
                                            else if (DisabledLogCount == 10)
                                            {
                                                Log("***** Not showing further warnings about skipping disabled URL: " + url);
                                            }


                                            ImageProcessQueue.Enqueue(CurImg);
                                        }

                                    }
                                    else
                                    {
                                        Log($"Skipping disabled AI URL: {url}, URLQueue.Count={DSURLQueue.Count}, ImageProcessQueue.Count={ImageProcessQueue.Count}");
                                        ImageProcessQueue.Enqueue(CurImg);
                                    }

                                    //lets always put the URL back in the queue even if failed.  Rely on disable property
                                    DSURLQueue.Enqueue(url);

                                }
                                else
                                {
                                    //lets not fill up the log if there are no more enabled URL's...
                                    if (DisabledLogCount < 10)
                                    {
                                        DisabledLogCount++;
                                        Log($"(No AI URLs left in the queue, waiting... ImageProcessQueue.Count={ImageProcessQueue.Count})");
                                    }
                                    else if (DisabledLogCount == 10)
                                    {
                                        Log("***** Not showing further warnings about skipping disabled URL: " + url);
                                    }

                                    ImageProcessQueue.Enqueue(CurImg);
                                    break;
                                }

                            }
                            else
                            {
                                //Log("No Images left in the queue!");
                                break;
                            }

                        }

                        if (TskCnt > 0)
                        {
                            Log($"Done adding {TskCnt} total threads, ErrCnt={ErrCnt}, URLQueue.Count={DSURLQueue.Count}, ImageProcessQueue.Count={ImageProcessQueue.Count}");
                        }

                        //Clean up old images in the dupe check dic
                        if ((DateTime.Now - LastCleanDupesTime).TotalMinutes >= 30)
                        {
                            int cnt = 0;
                            foreach (KeyValuePair<string, ClsImageQueueItem> kvPair in detection_dictionary)
                            {
                                if ((DateTime.Now - kvPair.Value.TimeAdded).TotalMinutes >= 30)
                                {   // Remove expired item.
                                    cnt++;
                                    ClsImageQueueItem removedItem;
                                    detection_dictionary.TryRemove(kvPair.Key, out removedItem);
                                }
                            }
                            
                        }

                    }

                    //Only loop 10 times a second conserve cpu
                    await Task.Delay(100);
                }
            }
            catch (Exception ex)
            {
                //if we get here its the end of the world as we know it
                Log("Error: * '...Human sacrifice, dogs and cats living together – mass hysteria!' * - " + Global.ExMsg(ex));
            }
        }

        //EVENT: new image added to input_path -> START AI DETECTION
        private static async void OnCreatedAsync(object source, FileSystemEventArgs e)
        {

            lock (FileWatcherLockObject)
            {
                try
                {
                    //make sure we are not processing a duplicate file...
                    if (detection_dictionary.ContainsKey(e.FullPath.ToLower()))
                    {
                        Log("Skipping image because of duplicate Created File Event: " + e.FullPath);
                    }
                    else 
                    {
                        Camera cam = GetCamera(e.FullPath);
                        if (cam != null)  //only put in queue if we can match to camera (even default)
                        {

                            if (cam.enabled)
                            {
                                //Note:  Interwebz says ConCurrentQueue.Count may be slow for large number of items but I dont think we have to worry here in most cases
                                int qsize = ImageProcessQueue.Count + 1;
                                if (qsize > AppSettings.Settings.MaxImageQueueSize)
                                {
                                    Log("");
                                    Log($"Error: Skipping image because queue is greater than '{AppSettings.Settings.MaxImageQueueSize}'. (Adjust 'MaxImageQueueSize' in .JSON file if needed): " + e.FullPath);
                                }
                                else
                                {
                                    Log("");
                                    Log($"====================== Adding new image to queue (Count={ImageProcessQueue.Count + 1}): " + e.FullPath);
                                    ClsImageQueueItem CurImg = new ClsImageQueueItem(e.FullPath, qsize);
                                    detection_dictionary.TryAdd(e.FullPath.ToLower(), CurImg);
                                    ImageProcessQueue.Enqueue(CurImg);
                                    qsizecalc.AddToCalc(qsize);
                                    Global.SendMessage(MessageType.ImageAddedToQueue);
                                }

                            }
                            else
                            {
                                Log($"Error: Skipping image because camera '{cam.name}' is DISABLED " + e.FullPath);
                            }
                        }
                        else
                        {
                            Log("Error: Skipping image because no camera found for new image " + e.FullPath);
                        }


                    }

                }
                catch (Exception ex)
                {
                    Log("Error: " + Global.ExMsg(ex));
                }

            }

        }
        //event: image in input_path renamed
        private static void OnRenamed(object source, RenamedEventArgs e)
        {
            Global.DeleteHistoryItem(e.OldName);
        }

        //event: image in input path deleted
        private static void OnDeleted(object source, FileSystemEventArgs e)
        {
            Global.DeleteHistoryItem(e.Name);
        }

        private static void OnError(object sender, ErrorEventArgs e)
        {
            Log("Error: File watcher error: " + e.GetException().Message);
        }

        public static void UpdateWatchers()
        {

            try
            {

                if (AppSettings.AlreadyRunning)
                {
                    Log("*** Another instance is already running, skip watching for changed files ***");
                    return;
                }
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
                        names.Add($"{cam.name}|{pths}|{cam.input_path_includesubfolders}");
                    }
                }


                //check each one to see if needs to be added
                foreach (string item in names)
                {
                    List<string> splt = Global.Split(item, "|", false);
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
                                FileSystemWatcher curwatch = MyWatcherFatory(path, include);
                                ClsFileSystemWatcher mywtc = new ClsFileSystemWatcher(name, path, curwatch, include);
                                //add even if null to keep track of things
                                watchers.Add(name.ToLower(), mywtc);
                            }
                            else
                            {
                                //update path if needed, even to empty
                                watchers[name.ToLower()].Path = path;
                                if (watchers[name.ToLower()].watcher == null)
                                {
                                    //could be null if path is bad
                                    watchers[name.ToLower()].watcher = MyWatcherFatory(path, include);
                                }
                            }
                        }
                        else
                        {
                            Log($"Skipping duplicate path for '{name}': '{path}'");
                        }

                    }


                }

                //check to see if any need disabling - a camera was deleted
                foreach (ClsFileSystemWatcher watcher1 in watchers.Values)
                {
                    bool fnd = false;
                    foreach (string item in names)
                    {
                        List<string> splt = Global.Split(item);
                        string name = splt[0];
                        if (name.ToLower() == watcher1.Name.ToLower())
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
                                    Log($"Watcher '{watcher.Name}' changed from '{watcher.watcher.Path}' to '{watcher.Path}'.");
                                }

                                if (watcher.IncludeSubdirectories != watcher.watcher.IncludeSubdirectories)
                                {
                                    watcher.watcher.IncludeSubdirectories = watcher.IncludeSubdirectories;
                                    Log($"Watcher '{watcher.Name}' IncludeSubdirectories changed from '{watcher.watcher.IncludeSubdirectories}' to '{watcher.IncludeSubdirectories}'.");
                                }

                                if (watcher.watcher.EnableRaisingEvents != true)
                                {
                                    enabledcnt++;
                                    watcher.watcher.EnableRaisingEvents = true;
                                    dupes.Add(watcher.Path.ToLower(), watcher.Path);
                                    Log($"Watcher '{watcher.Name}' is now watching '{watcher.Path}'");
                                }

                            }
                            else
                            {
                                Log($"Watcher '{watcher.Name}' has a duplicate path, skipping '{watcher.Path}'");
                            }
                        }
                        else
                        {
                            //make sure it is disabled
                            disabledcnt++;

                            watcher.watcher.EnableRaisingEvents = false;
                            watcher.watcher.Dispose();
                            watcher.watcher = null;
                            Log($"Watcher '{watcher.Name}' has an empty path, just disabled.");
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

                if (watchers.Count() == 0)
                {
                    Log("No FileSystemWatcher input folders defined yet.");
                }
                else
                {
                    if (enabledcnt == 0)
                    {
                        Log("No NEW FileSystemWatcher input folders found.");
                    }
                    else
                    {
                        Log($"Enabled {enabledcnt} FileSystemWatchers.");
                    }
                }

            }
            catch (Exception ex)
            {

                Log($"Error: {Global.ExMsg(ex)}");
            }

        }

        public static FileSystemWatcher MyWatcherFatory(string path, bool IncludeSubdirectories = false, string filter = "*.jpg")
        {
            FileSystemWatcher watcher = null;

            try
            {
                // Be aware: https://stackoverflow.com/questions/1764809/filesystemwatcher-changed-event-is-raised-twice

                if (!String.IsNullOrWhiteSpace(path) && Directory.Exists(path))
                {
                    watcher = new FileSystemWatcher(path);
                    watcher.Path = path;
                    watcher.Filter = filter;
                    watcher.IncludeSubdirectories = IncludeSubdirectories;

                    //The 'default' is the bitwise OR combination of LastWrite, FileName, and DirectoryName'
                    watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;

                    //fswatcher events
                    watcher.Created += new FileSystemEventHandler(OnCreatedAsync);
                    watcher.Renamed += new RenamedEventHandler(OnRenamed);
                    watcher.Deleted += new FileSystemEventHandler(OnDeleted);
                    watcher.Error += new ErrorEventHandler(OnError);

                }
            }
            catch (Exception ex)
            {
                Log($"Error: {Global.ExMsg(ex)}");
            }

            return watcher;
        }


        //analyze image with AI
        public static async Task<bool> DetectObjects(ClsImageQueueItem CurImg, ClsURLItem DeepStackURL)
        {


            bool ret = false;

            //Only set error when there IS an error...
            string error = ""; //if code fails at some point, the last text of the error string will be posted in the log

            string filename = Path.GetFileName(CurImg.image_path);

            CurImg.QueueWaitMS = (long)(DateTime.Now - CurImg.TimeAdded).TotalMilliseconds;

            Stopwatch sw = Stopwatch.StartNew();
            Stopwatch swposttime = Stopwatch.StartNew();

            Uri url = new Uri(DeepStackURL.url);
            String CurSrv = url.Host + ":" + url.Port;

            Camera cam = AITOOL.GetCamera(CurImg.image_path);
            cam.last_image_file = CurImg.image_path;

            // check if camera is still in the first half of the cooldown. If yes, don't analyze to minimize cpu load.
            //only analyze if 50% of the cameras cooldown time since last detection has passed
            double mins = (DateTime.Now - cam.last_trigger_time).TotalMinutes;
            double halfcool = cam.cooldown_time / 2;
            if (mins >= halfcool)
            {
                try
                {
                    Log($"{CurSrv} - Starting analysis of {CurImg.image_path}...");

                    // Wait up to 30 seconds to gain access to the file that was just created.This should
                    //prevent the need to retry in the detection routine
                    sw.Restart();

                    bool success = await Global.WaitForFileAccessAsync(CurImg.image_path, FileSystemRights.Read, FileShare.Read, 30000, 20);

                    sw.Stop();

                    CurImg.FileLockMS = sw.ElapsedMilliseconds;

                    if (success)
                    {

                        string jsonString = "";

                        using (FileStream image_data = System.IO.File.OpenRead(CurImg.image_path))
                        {

                            //error = $"Can't reach DeepQuestAI Server at {fullDeepstackUrl}.";

                            MultipartFormDataContent request = new MultipartFormDataContent();
                            request.Add(new StreamContent(image_data), "image", Path.GetFileName(CurImg.image_path));

                            //Be aware Microsoft recommends creating an instance once per session, but since
                            //we may be operating in more than one thread at the same time, there is some
                            //speculation that it may not be TOALLY thread safe.   To be on safe side, I'm
                            //going to try to initialize once per thread, but maybe not needed and adds a
                            //bit of overhead.....  -Vorlon

                            using (HttpClient client = new HttpClient())
                            {
                                //set httpclient timeout:
                                client.Timeout = TimeSpan.FromSeconds(AppSettings.Settings.HTTPClientTimeoutSeconds);

                                //I'm not sure if we need both httpclient.timeout and CancellationTokenSource timeout...
                                using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(AppSettings.Settings.AIDetectionTimeoutSeconds)))
                                {
                                    Log($"{CurSrv} - (1/6) Uploading image to DeepQuestAI Server at {DeepStackURL}");

                                    swposttime = Stopwatch.StartNew();

                                    using (HttpResponseMessage output = await client.PostAsync(url, request, cts.Token))
                                    {
                                        swposttime.Stop();

                                        if (output.IsSuccessStatusCode)
                                        {
                                            jsonString = await output.Content.ReadAsStringAsync();
                                        }
                                        else
                                        {
                                            error = $"{CurSrv} - ERROR: Got http status code '{Convert.ToInt32(output.StatusCode)}' in {{yellow}}{swposttime.ElapsedMilliseconds}ms{{red}}: {output.ReasonPhrase}";
                                            DeepStackURL.IncrementErrCount();
                                            DeepStackURL.ResultMessage = error;
                                            Log(error);
                                        }
                                    }
                                }
                            }

                        }

                        if (jsonString != null && !string.IsNullOrWhiteSpace(jsonString))
                        {
                            string cleanjsonString = Global.CleanString(jsonString);

                            Log($"{CurSrv} - (2/6) Posted in {{yellow}}{swposttime.ElapsedMilliseconds}ms{{white}}, Received a {jsonString.Length} byte response.");
                            Log($"{CurSrv} - (3/6) Processing results...");

                            Response response = null;

                            try
                            {
                                //This can throw an exception
                                response = JsonConvert.DeserializeObject<Response>(jsonString);
                            }
                            catch (Exception ex)
                            {
                                error = $"{CurSrv} - ERROR: Deserialization of 'Response' from DeepStack failed: {Global.ExMsg(ex)}, JSON: '{cleanjsonString}'";
                                DeepStackURL.IncrementErrCount();
                                DeepStackURL.ResultMessage = error;
                                Log(error);
                            }

                            if (response != null)
                            {

                                //error = $"Failure in DeepStack processing the image.";
                                //print every detected object with the according confidence-level
                                string outputtext = $"{CurSrv} -    Detected objects:";


                                if (response.predictions != null)
                                {
                                    //if we are not using the local deepstack windows version, this means nothing:
                                    DeepStackServerControl.IsActivated = true;

                                    if (response.predictions.Count() > 0)
                                    {
                                        foreach (Object user in response.predictions)
                                        {
                                            if (user != null && !string.IsNullOrEmpty(user.label))
                                            {
                                                DeepStackServerControl.VisionDetectionRunning = true;
                                                outputtext += $" {user.label.ToString()} ({Math.Round((user.confidence * 100), 2).ToString() }%), ";
                                            }
                                            else
                                            {
                                                outputtext += " (Error: null prediction? DeepStack may not be started with correct switches.), ";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        outputtext += " ((NONE))";
                                    }
                                }
                                else
                                {
                                    outputtext = $"{CurSrv} - (Error: No predictions?  JSON: '{cleanjsonString}')";
                                }

                                Log(outputtext);

                                if (response.success == true)
                                {

                                    //if camera is enabled
                                    if (cam.enabled == true)
                                    {

                                        List<string> objects = new List<string>(); //list that will be filled with all objects that were detected and are triggering_objects for the camera
                                        List<float> objects_confidence = new List<float>(); //list containing ai confidence value of object at same position in List objects
                                        List<string> objects_position = new List<string>(); //list containing object positions (xmin, ymin, xmax, ymax)

                                        List<string> irrelevant_objects = new List<string>(); //list that will be filled with all irrelevant objects
                                        List<float> irrelevant_objects_confidence = new List<float>(); //list containing ai confidence value of irrelevant object at same position in List objects
                                        List<string> irrelevant_objects_position = new List<string>(); //list containing irrelevant object positions (xmin, ymin, xmax, ymax)


                                        int masked_counter = 0; //this value is incremented if an object is in a masked area
                                        int threshold_counter = 0; // this value is incremented if an object does not satisfy the confidence limit requirements
                                        int irrelevant_counter = 0; // this value is incremented if an irrelevant (but not masked or out of range) object is detected

                                        //if something was detected
                                        if (response.predictions.Length > 0)
                                        {

                                            Log($"{CurSrv} - (4/6) Checking if detected object is relevant and within confidence limits:");
                                            //add all triggering_objects of the specific camera into a list and the correlating confidence levels into a second list
                                            foreach (Object user in response.predictions)
                                            {
                                                // just extra log lines - Log($"   {user.label.ToString()} ({Math.Round((user.confidence * 100), 2).ToString() }%):");

                                                using (var img = new Bitmap(CurImg.image_path))
                                                {
                                                    bool irrelevant_object = false;

                                                    //if object detected is one of the objects that is relevant
                                                    if (cam.triggering_objects_as_string.Contains(user.label))
                                                    {
                                                        // -> OBJECT IS RELEVANT

                                                        //if confidence limits are satisfied
                                                        if (user.confidence * 100 >= cam.threshold_lower && user.confidence * 100 <= cam.threshold_upper)
                                                        {
                                                            // -> OBJECT IS WITHIN CONFIDENCE LIMITS

                                                            ObjectPosition currentObject = new ObjectPosition(user.x_min, user.y_min, user.x_max, user.y_max, user.label,
                                                                                                              img.Width, img.Height, cam.name, CurImg.image_path);

                                                            bool maskExists = false;
                                                            if (cam.maskManager.masking_enabled)
                                                            {
                                                                //creates history and masked lists for objects returned
                                                                maskExists = cam.maskManager.CreateDynamicMask(currentObject);
                                                            }

                                                            //only if the object is outside of the masked area
                                                            if (Outsidemask(cam.name, user.x_min, user.x_max, user.y_min, user.y_max, img.Width, img.Height)
                                                                && !maskExists)
                                                            {
                                                                // -> OBJECT IS OUTSIDE OF MASKED AREAS

                                                                objects.Add(user.label);
                                                                objects_confidence.Add(user.confidence);
                                                                string position = $"{user.x_min},{user.y_min},{user.x_max},{user.y_max}";
                                                                objects_position.Add(position);
                                                                Log($"{CurSrv} -    {{orange}}{ user.label.ToString()} ({ Math.Round((user.confidence * 100), 2).ToString() }%) confirmed.");
                                                            }
                                                            else //if the object is in a masked area
                                                            {
                                                                masked_counter++;
                                                                irrelevant_object = true;
                                                            }
                                                        }
                                                        else //if confidence limits are not satisfied
                                                        {
                                                            threshold_counter++;
                                                            irrelevant_object = true;
                                                        }
                                                    }
                                                    else //if object is not relevant
                                                    {
                                                        irrelevant_counter++;
                                                        irrelevant_object = true;
                                                    }

                                                    if (irrelevant_object == true)
                                                    {
                                                        irrelevant_objects.Add(user.label);
                                                        irrelevant_objects_confidence.Add(user.confidence);
                                                        string position = $"{user.x_min},{user.y_min},{user.x_max},{user.y_max}";
                                                        irrelevant_objects_position.Add(position);
                                                        Log($"{CurSrv} -    { user.label.ToString()} ({ Math.Round((user.confidence * 100), 2).ToString() }%) is irrelevant.");
                                                    }
                                                }

                                            }  //end loop over current object list

                                            if (cam.maskManager.masking_enabled)
                                            {
                                                //remove objects from history if they have not been detected in the history_save_mins and hit counter < history_threshold_count
                                                cam.maskManager.CleanUpExpiredMasks();
                                            }

                                            //if one or more objects were detected, that are 1. relevant, 2. within confidence limits and 3. outside of masked areas
                                            if (objects.Count() > 0)
                                            {
                                                //store these last detections for the specific camera
                                                cam.last_detections = objects;
                                                cam.last_confidences = objects_confidence;
                                                cam.last_positions = objects_position;
                                                cam.last_image_file_with_detections = CurImg.image_path;

                                                //create summary string for this detection
                                                StringBuilder detectionsTextSb = new StringBuilder();
                                                for (int i = 0; i < objects.Count(); i++)
                                                {
                                                    detectionsTextSb.Append(String.Format("{0} ({1}%) | ", objects[i], Math.Round((objects_confidence[i] * 100), 2)));
                                                }
                                                if (detectionsTextSb.Length >= 3)
                                                {
                                                    detectionsTextSb.Remove(detectionsTextSb.Length - 3, 3);
                                                }
                                                cam.last_detections_summary = detectionsTextSb.ToString();
                                                Log($"{CurSrv} - The summary:" + cam.last_detections_summary);


                                                if (!cam.trigger_url_cancels)
                                                {
                                                    Log($"{CurSrv} - (5/6) Performing alert actions:");
                                                    await Trigger(cam, CurImg); //make TRIGGER
                                                }
                                                cam.IncrementAlerts(); //stats update
                                                Log($"{CurSrv} - (6/6) SUCCESS.");


                                                //create text string objects and confidences
                                                string objects_and_confidences = "";
                                                string object_positions_as_string = "";
                                                for (int i = 0; i < objects.Count; i++)
                                                {
                                                    objects_and_confidences += $"{objects[i]} ({Math.Round((objects_confidence[i] * 100), 0)}%); ";
                                                    object_positions_as_string += $"{objects_position[i]};";
                                                }

                                                //add to history list
                                                Log($"{CurSrv} - Adding detection to history list.");
                                                Global.CreateHistoryItem(new ClsHistoryItem(CurImg.image_path, DateTime.Now, cam.name, objects_and_confidences, object_positions_as_string));

                                            }
                                            //if no object fulfills all 3 requirements but there are other objects: 
                                            else if (irrelevant_objects.Count() > 0)
                                            {
                                                //IRRELEVANT ALERT

                                                if (cam.trigger_url_cancels)
                                                {
                                                    Log($"{CurSrv} - (5/6) Performing alert CANCEL actions:");
                                                    await Trigger(cam, CurImg); //make TRIGGER
                                                }

                                                cam.IncrementIrrelevantAlerts(); //stats update
                                                Log($"{CurSrv} - (6/6) Camera {cam.name} caused an irrelevant alert.");
                                                //Log("Adding irrelevant detection to history list.");

                                                //retrieve confidences and positions
                                                string objects_and_confidences = "";
                                                string object_positions_as_string = "";
                                                for (int i = 0; i < irrelevant_objects.Count; i++)
                                                {
                                                    objects_and_confidences += $"{irrelevant_objects[i]} ({Math.Round((irrelevant_objects_confidence[i] * 100), 0)}%); ";
                                                    object_positions_as_string += $"{irrelevant_objects_position[i]};";
                                                }

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

                                                if (text != "") //remove last ";"
                                                {
                                                    text = text.Remove(text.Length - 2);
                                                }

                                                Log($"{CurSrv} - {text}, so it's an irrelevant alert.");
                                                //add to history list
                                                Global.CreateHistoryItem(new ClsHistoryItem(CurImg.image_path, DateTime.Now, cam.name, $"{text} : {objects_and_confidences}", object_positions_as_string));
                                            }
                                        }
                                        //if no object was detected
                                        else if (response.predictions.Length == 0)
                                        {
                                            // FALSE ALERT

                                            if (cam.trigger_url_cancels)
                                            {
                                                Log($"{CurSrv} - (5/6) Performing alert CANCEL actions:");
                                                await Trigger(cam, CurImg); //make TRIGGER
                                            }

                                            cam.IncrementFalseAlerts(); //stats update
                                            Log($"{CurSrv} - (6/6) Camera {cam.name} caused a false alert, nothing detected.");

                                            //add to history list
                                            //Log($"{CurSrv} - Adding false to history list.");
                                            Global.CreateHistoryItem(new ClsHistoryItem(CurImg.image_path, DateTime.Now, cam.name, "false alert", ""));
                                        }
                                    }

                                    //if camera is disabled.
                                    else if (cam.enabled == false)
                                    {
                                        Log($"{CurSrv} - (6/6) Selected camera is disabled.");
                                    }

                                }
                                else if (response.success == false) //if nothing was detected
                                {
                                    error = $"{CurSrv} - ERROR: Failure response from DeepStack. JSON: '{cleanjsonString}'";
                                    Log(error);
                                }

                            }
                            else if (string.IsNullOrEmpty(error))
                            {
                                //deserialization did not cause exception, it just gave a null response in the object?
                                //probably wont happen but just making sure
                                error = $"{CurSrv} - ERROR: Deserialization of 'Response' from DeepStack failed. response is null. JSON: '{cleanjsonString}'";
                                DeepStackURL.IncrementErrCount();
                                DeepStackURL.ResultMessage = error;
                                Log(error);
                            }


                        }
                        else
                        {
                            error = $"{CurSrv} - ERROR: Empty string returned from HTTP post.";
                            DeepStackURL.IncrementErrCount();
                            DeepStackURL.ResultMessage = error;
                            Log(error);
                        }



                    }
                    else
                    {
                        //could not access the file for 30 seconds??   Or unexpected error
                        error = $"Error: Could not gain access to {CurImg.image_path} for {{yellow}}{sw.Elapsed.TotalSeconds}{{red}} seconds, giving up.";
                        CurImg.IncrementErrCount();
                        CurImg.ResultMessage = error;
                        Log(error);
                    }


                    //break; //end retries if code was successful
                }
                catch (Exception ex)
                {

                    //We should almost never get here due to all the null checks and function to wait for file to become available...
                    //When the connection to deepstack fails we will get here
                    //exception.tostring should give the line number and ALL detail - but maybe only if PDB is in same folder as exe?
                    swposttime.Stop();

                    error = $"{CurSrv} - ERROR: {Global.ExMsg(ex)}";
                    DeepStackURL.IncrementErrCount();
                    DeepStackURL.ResultMessage = error;
                    Log(error);
                }

                if (!string.IsNullOrEmpty(error) && AppSettings.Settings.send_errors == true)
                {
                    //upload the alert image which could not be analyzed to Telegram
                    if (AppSettings.Settings.send_errors == true)
                    {
                        bool success = await TelegramUpload(CurImg, "Error");
                    }

                }


                //I notice deepstack takes a lot longer the very first run?

                CurImg.TotalTimeMS = (long)(DateTime.Now - CurImg.TimeAdded).TotalMilliseconds; //sw.ElapsedMilliseconds + CurImg.QueueWaitMS + CurImg.FileLockMS;
                CurImg.DeepStackTimeMS = swposttime.ElapsedMilliseconds;
                DeepStackURL.DeepStackTimeMS = swposttime.ElapsedMilliseconds;
                tcalc.AddToCalc(CurImg.TotalTimeMS);
                DeepStackURL.dscalc.AddToCalc(CurImg.DeepStackTimeMS);
                qcalc.AddToCalc(CurImg.QueueWaitMS);
                fcalc.AddToCalc(CurImg.FileLockMS);


                //Log($"{CurSrv} - ...Object detection finished: ");
                Log($"{CurSrv} -          Total Time:   {{yellow}}{CurImg.TotalTimeMS}ms{{white}} (Count={tcalc.Count}, Min={tcalc.Min}ms, Max={tcalc.Max}ms, Avg={tcalc.Average.ToString("#####")}ms)");
                Log($"{CurSrv} -DeepStack (URL) Time:   {{yellow}}{CurImg.DeepStackTimeMS}ms{{white}} (Count={DeepStackURL.dscalc.Count}, Min={DeepStackURL.dscalc.Min}ms, Max={DeepStackURL.dscalc.Max}ms, Avg={DeepStackURL.dscalc.Average.ToString("#####")}ms)");
                Log($"{CurSrv} -      File lock Time:   {{yellow}}{CurImg.FileLockMS}ms{{white}} (Count={fcalc.Count}, Min={fcalc.Min}ms, Max={fcalc.Max}ms, Avg={fcalc.Average.ToString("#####")}ms)");

                //I want to highlight when we have to wait for the last detection (or for the file to become readable) too long
                if (CurImg.QueueWaitMS + CurImg.FileLockMS >= 500)
                {
                    Log($"{CurSrv} -    {{red}}Image Queue Time:   {{yellow}}{CurImg.QueueWaitMS}ms{{red}} (Count={qcalc.Count}, Min={qcalc.Min}ms, Max={qcalc.Max}ms, Avg={qcalc.Average.ToString("#####")}ms)");
                }
                else
                {
                    Log($"{CurSrv} -    {{white}}Image Queue Time:   {{yellow}}{CurImg.QueueWaitMS}ms{{white}} (Count={qcalc.Count}, Min={qcalc.Min}ms, Max={qcalc.Max}ms, Avg={qcalc.Average.ToString("#####")}ms)");
                }

                Log($"{CurSrv} -   Image Queue Depth:   {{yellow}}{CurImg.CurQueueSize}{{white}} (Count={qsizecalc.Count}, Min={qsizecalc.Min}, Max={qsizecalc.Max}, Avg={qsizecalc.Average.ToString("#####")})");

                //}

            }
            else
            {
                Log($"{CurSrv} - Skipping detection. Found='{cam.name}', Mins since last submission='{mins}', halfcool={halfcool}");
            }

            return (error == "");


        }

        //call trigger urls
        public static void CallTriggerURLs(List<string> trigger_urls)
        {

            var client = new WebClient();
            foreach (string x in trigger_urls)
            {
                try
                {
                    string content = client.DownloadString(x);
                    Log($"   -> trigger URL called: {x}, response: '{content.Replace("\r\n", "\n").Replace("\n", " ")}'");
                }
                catch (Exception ex)
                {
                    Log($"ERROR: Could not trigger URL '{x}', please check if '{x}' is correct and reachable: {Global.ExMsg(ex)}");
                }

            }


        }

        //send image to Telegram
        public static async Task<bool> TelegramUpload(ClsImageQueueItem CurImg, string message)
        {
            bool ret = false;

            if (AppSettings.Settings.telegram_chatids.Count > 0 && AppSettings.Settings.telegram_token != "")
            {
                //telegram upload sometimes fails
                Stopwatch sw = Stopwatch.StartNew();
                try
                {
                    using (var image_telegram = System.IO.File.OpenRead(CurImg.image_path))
                    {
                        var bot = new TelegramBotClient(AppSettings.Settings.telegram_token);

                        //upload image to Telegram servers and send to first chat
                        Log($"      uploading image to chat \"{AppSettings.Settings.telegram_chatids[0]}\"");
                        var message = await bot.SendPhotoAsync(AppSettings.Settings.telegram_chatids[0], new InputOnlineFile(image_telegram, "image.jpg"), message);
                        string file_id = message.Photo[0].FileId; //get file_id of uploaded image

                        //share uploaded image with all remaining telegram chats (if multiple chat_ids given) using file_id 
                        foreach (string chatid in AppSettings.Settings.telegram_chatids.Skip(1))
                        {
                            Log($"      uploading image to chat \"{chatid}\"");
                            await bot.SendPhotoAsync(chatid, file_id, message);
                        }
                        ret = true;
                    }
                }
                catch
                {
                    Log($"ERROR: Could not upload image {CurImg.image_path} to Telegram.");
                    //store image that caused an error in ./errors/
                    if (!Directory.Exists("./errors/")) //if folder does not exist, create the folder
                    {
                        //create folder
                        DirectoryInfo di = Directory.CreateDirectory("./errors");
                        Log("./errors/" + " dir created.");
                    }
                    //save error image
                    using (var image = SixLabors.ImageSharp.Image.Load(CurImg.image_path))
                    {
                        image.Save("./errors/" + "TELEGRAM-ERROR-" + Path.GetFileName(CurImg.image_path) + ".jpg");
                    }
                }


                Log($"...Finished in {{yellow}}{sw.ElapsedMilliseconds}ms{{white}}");

            }

            return ret;

        }

        //send text to Telegram
        public static async Task TelegramText(string text)
        {
            if (AppSettings.Settings.telegram_chatids.Count > 0 && AppSettings.Settings.telegram_token != "")
            {
                //telegram upload sometimes fails
                try
                {
                    var bot = new Telegram.Bot.TelegramBotClient(AppSettings.Settings.telegram_token);
                    foreach (string chatid in AppSettings.Settings.telegram_chatids)
                    {
                        await bot.SendTextMessageAsync(chatid, text);
                    }

                }
                catch (Exception ex)
                {
                    if (AppSettings.Settings.send_errors == true && text.ToUpper().Contains("ERROR") || text.ToUpper().Contains("WARNING")) //if Error message originating from Log() methods can't be uploaded
                    {
                        AppSettings.Settings.send_errors = false; //shortly disable send_errors to ensure that the Log() does not try to send the 'Telegram upload failed' message via Telegram again (causing a loop)
                        Log($"ERROR: Could not send text \"{text}\" to Telegram: {Global.ExMsg(ex)}");
                        AppSettings.Settings.send_errors = true;

                        //inform on main tab that Telegram upload failed
                        Global.UpdateLabel("Can't upload error message to Telegram!", "lbl_errors");
                    }
                    else
                    {
                        Log($"ERROR: Could not send text \"{text}\" to Telegram: {Global.ExMsg(ex)}");
                    }
                }

            }
        }

        //trigger actions
        public static async Task<bool> Trigger(Camera cam, ClsImageQueueItem CurImg)
        {
            bool ret = true;

            //mostly for testing when we dont have a current image...
            if (CurImg == null)
            {
                if (!string.IsNullOrEmpty(cam.last_image_file_with_detections))
                {
                    CurImg = new ClsImageQueueItem(cam.last_image_file_with_detections, 1);
                }
                else if (!string.IsNullOrEmpty(cam.last_image_file))
                {
                    CurImg = new ClsImageQueueItem(cam.last_image_file, 1);
                }
                else
                {
                    Log("Error: No image to process?");
                    return false;
                }
            }

            try
            {
                //only trigger if cameras cooldown time since last detection has passed
                if ((DateTime.Now - cam.last_trigger_time).TotalMinutes >= cam.cooldown_time)
                {
                    //call trigger urls
                    if (cam.trigger_urls.Count() > 0)
                    {
                        //replace url paramters with according values
                        List<string> urls = new List<string>();
                        //call urls
                        foreach (string url in cam.trigger_urls)
                        {
                            try
                            {

                                string tmp = AITOOL.ReplaceParams(cam, CurImg, url);
                                urls.Add(tmp);
                            }
                            catch (Exception ex)
                            {
                                ret = false;
                                Log($"{Global.ExMsg(ex)}");
                            }

                        }

                        CallTriggerURLs(urls);
                    }

                    if (!cam.trigger_url_cancels)
                    {
                        //upload to telegram
                        if (cam.telegram_enabled)
                        {
                            if ((DateTime.Now - cam.last_trigger_time).TotalMinutes >= AppSettings.Settings.telegram_cooldown_minutes)
                            {
                                Log("   Uploading image to Telegram...");
                                
                                if (!await TelegramUpload(CurImg, cam.name + " - " + cam.last_detections_summary))
                                {
                                    ret = false;
                                    Log("   -> ERROR sending image to Telegram.");
                                }
                                else
                                {
                                    Log("   -> Sent image to Telegram.");
                                }
                            }
                            else
                            {
                                //log that nothing was done
                                Log($"   Camera {cam.name} is still in TELEGRAM cooldown. No image will be uploaded to Telegram.");
                            }
                        }

                        //run external program
                        if (cam.Action_RunProgram)
                        {
                            try
                            {
                                string run = AITOOL.ReplaceParams(cam, CurImg, cam.Action_RunProgramString);
                                string param = AITOOL.ReplaceParams(cam, CurImg, cam.Action_RunProgramArgsString);
                                Log($"   Starting external app {run} {param}");
                                Process.Start(run, param);
                            }
                            catch (Exception ex)
                            {

                                ret = false;
                                Log($"Error: while running '{cam.Action_RunProgramString}', got: {Global.ExMsg(ex)}");
                            }
                        }

                        //Play sounds
                        if (cam.Action_PlaySounds)
                        {
                            try
                            {

                                //object1, object2 ; soundfile.wav | object1, object2 ; anotherfile.wav | * ; defaultsound.wav
                                string snds = AITOOL.ReplaceParams(cam, CurImg, cam.Action_Sounds);

                                List<string> items = Global.Split(snds, "|");

                                foreach (string itm in items)
                                {
                                    //object1, object2 ; soundfile.wav
                                    int played = 0;
                                    List<string> prms = Global.Split(itm, "|");
                                    foreach (string prm in prms)
                                    {
                                        //prm0 - object1, object2
                                        //prm1 - soundfile.wav
                                        List<string> splt = Global.Split(prm, ";");
                                        string soundfile = splt[1];
                                        List<string> objects = Global.Split(splt[0], ",");
                                        foreach (string objname in objects)
                                        {
                                            foreach (string detection in cam.last_detections)
                                            {
                                                if (detection.ToLower().Contains(objname.ToLower()) || (objname == "*"))
                                                {
                                                    Log($"   Playing sound because '{objname}' was detected: {soundfile}...");
                                                    SoundPlayer sp = new SoundPlayer(soundfile);
                                                    sp.Play();
                                                    played++;
                                                }
                                            }
                                        }
                                    }
                                    if (played == 0)
                                    {
                                        Log("No object matched sound to play or no detections.");
                                    }
                                }

                            }
                            catch (Exception ex)
                            {

                                ret = false;
                                Log($"Error: while calling sound '{cam.Action_Sounds}', got: {Global.ExMsg(ex)}");
                            }
                        }



                        if (cam.Action_image_copy_enabled)
                        {
                            Log("   Copying image to network folder...");
                            
                            if (!AITOOL.CopyImage(cam, CurImg))
                                ret = false;

                            Log("   -> Image copied to network folder.");
                        }


                        if (cam.Action_mqtt_enabled)
                        {
                            string topic = AITOOL.ReplaceParams(cam, CurImg, cam.Action_mqtt_topic);
                            string payload = AITOOL.ReplaceParams(cam, CurImg, cam.Action_mqtt_payload);

                            MQTTClient mq = new MQTTClient();
                            MqttClientPublishResult pr = await mq.PublishAsync(topic, payload);
                            if (pr == null || pr.ReasonCode != MqttClientPublishReasonCode.Success)
                                ret = false;
                        }

                    }
                    else
                    {
                        Log($"   (Skipping all other actions due to 'Trigger cancels' camera action setting)");
                    }

                }
                else
                {
                    //log that nothing was done
                    Log($"   Camera {cam.name} is still in cooldown. Trigger URL wasn't called and no image will be uploaded to Telegram.");
                }

                cam.last_trigger_time = DateTime.Now; //reset cooldown time every time an image contains something, even if no trigger was called (still in cooldown time)

                //Task ignoredAwaitableResult = LastTriggerInfo(cam); //write info to label
                Global.UpdateLabel($"{cam.name} last triggered at {cam.last_trigger_time}.", "lbl_info");

            }
            catch (Exception ex)
            {

                Log("Error: " + ExMsg(ex));
            }


            return ret;

        }



        //check if detected object is outside the mask for the specific camera
        //TODO: refacotor png, bmp mask logic later. This is just a starting point. 
        public static bool Outsidemask(string cameraname, double xmin, double xmax, double ymin, double ymax, int width, int height)
        {
            //Log($"      Checking if object is outside privacy mask of {cameraname}:");
            //Log("         Loading mask file...");
            string fileType;
            try
            {
                if (System.IO.File.Exists("./cameras/" + cameraname + ".bmp")) //only check if mask image exists (.png or .bmp)
                {
                    fileType = ".bmp";
                }
                else if (System.IO.File.Exists("./cameras/" + cameraname + ".png"))
                {
                    fileType = ".png";
                }
                else //if mask image does not exist, object is outside the non-existing masked area
                {
                    Log("     ->Camera has no mask, the object is OUTSIDE of the masked area.");
                    return true;
                }

                //load mask file (in the image all places that have color (transparency > 9 [0-255 scale]) are masked)
                using (var mask_img = new Bitmap($"./cameras/{cameraname}" + fileType))
                {
                    //if any coordinates of the object are outside of the mask image, th mask image must be too small.
                    if (mask_img.Width != width || mask_img.Height != height)
                    {
                        Log($"ERROR: The resolution of the mask './camera/{cameraname}" + fileType + "' does not equal the resolution of the processed image. Skipping privacy mask feature. Image: {width}x{height}, Mask: {mask_img.Width}x{mask_img.Height}");
                        return true;
                    }

                    Log("         Checking if the object is in a masked area...");

                    //relative x and y locations of the 9 detection points
                    double[] x_factor = new double[] { 0.25, 0.5, 0.75, 0.25, 0.5, 0.75, 0.25, 0.5, 0.75 };
                    double[] y_factor = new double[] { 0.25, 0.25, 0.25, 0.5, 0.5, 0.5, 0.75, 0.75, 0.75 };

                    int result = 0; //counts how many of the 9 points are outside of masked area(s)

                    //check the transparency of the mask image in all 9 detection points
                    for (int i = 0; i < 9; i++)
                    {
                        //get image point coordinates (and converting double to int)
                        int x = (int)(xmin + (xmax - xmin) * x_factor[i]);
                        int y = (int)(ymin + (ymax - ymin) * y_factor[i]);

                        // Get the color of the pixel
                        System.Drawing.Color pixelColor = mask_img.GetPixel(x, y);

                        if (fileType == ".png")
                        {
                            //if the pixel is transparent (A refers to the alpha channel), the point is outside of masked area(s)
                            if (pixelColor.A < 10)
                            {
                                result++;
                            }
                        }
                        else
                        {
                            if (pixelColor.A == 0)  // object is in a transparent section of the image (not masked)
                            {
                                result++;
                            }
                        }

                    }

                    Log($"         { result.ToString() } of 9 detection points are outside of masked areas."); //print how many of the 9 detection points are outside of masked areas.

                    if (result > 4) //if 5 or more of the 9 detection points are outside of masked areas, the majority of the object is outside of masked area(s)
                    {
                        Log("      ->The object is OUTSIDE of masked area(s).");
                        return true;
                    }
                    else //if 4 or less of 9 detection points are outside, then 5 or more points are in masked areas and the majority of the object is so too
                    {
                        Log("      ->The object is INSIDE a masked area.");
                        return false;
                    }

                }



            }
            catch
            {
                Log($"ERROR while loading the mask file ./cameras/{cameraname}.png.");
                return true;
            }

        }


        public static string ReplaceParams(Camera cam, ClsImageQueueItem CurImg, string instr)
        {
            string ret = instr;

            try
            {
                string camname = "TESTCAMERANAME";
                string imgpath = "C:\\TESTFILE.jpg";

                if (cam != null)
                    camname = cam.name;

                if (CurImg == null && cam != null)
                {
                    imgpath = cam.last_image_file;
                }
                else if (CurImg != null)
                {
                    imgpath = CurImg.image_path;
                }

                //handle environment variables too:
                ret = Environment.ExpandEnvironmentVariables(ret);

                //handle custom variables
                ret = Global.ReplaceCaseInsensitive(ret, "[camera]", camname);
                ret = Global.ReplaceCaseInsensitive(ret, "[imagepath]", imgpath); //gives the full path of the image that caused the trigger
                ret = Global.ReplaceCaseInsensitive(ret, "[imagefilename]", Path.GetFileName(imgpath)); //gives the image name of the image that caused the trigger

                if (cam !=null)
                {
                    if (cam.last_detections != null && cam.last_detections.Count > 0)
                    {
                        ret = Global.ReplaceCaseInsensitive(ret, "[summary]", Uri.EscapeUriString(cam.last_detections_summary)); //summary text including all detections and confidences, p.e."person (91,53%)"
                        ret = Global.ReplaceCaseInsensitive(ret, "[detection]", cam.last_detections.ElementAt(0)); //only gives first detection (maybe not most relevant one)
                        ret = Global.ReplaceCaseInsensitive(ret, "[position]", cam.last_positions.ElementAt(0));
                        ret = Global.ReplaceCaseInsensitive(ret, "[confidence]", cam.last_confidences.ElementAt(0).ToString());
                        ret = Global.ReplaceCaseInsensitive(ret, "[detections]", string.Join(",", cam.last_detections));
                        ret = Global.ReplaceCaseInsensitive(ret, "[confidences]", string.Join(",", cam.last_confidences.ToString()));
                    }
                    else
                    {
                        ret = Global.ReplaceCaseInsensitive(ret, "[summary]", "No Summary Found"); //summary text including all detections and confidences, p.e."person (91,53%)"
                        ret = Global.ReplaceCaseInsensitive(ret, "[detection]", "No Detection Found"); //only gives first detection (maybe not most relevant one)
                        ret = Global.ReplaceCaseInsensitive(ret, "[position]", "No Position Found");
                        ret = Global.ReplaceCaseInsensitive(ret, "[confidence]", "No Confidence Found");
                        ret = Global.ReplaceCaseInsensitive(ret, "[detections]", "No Detections Found");
                        ret = Global.ReplaceCaseInsensitive(ret, "[confidences]", "No Confidences Found");
                    }

                }

            }
            catch (Exception ex)
            {

                Global.Log($"Error: {Global.ExMsg(ex)}");
            }

            return ret;

        }
        public static bool CopyImage(Camera cam, ClsImageQueueItem CurImg)
        {
            bool ret = false;

            string extension = "";
            string dest_path = "";
            try
            {
                string netfld = AITOOL.ReplaceParams(cam, CurImg, cam.Action_network_folder);

                if (!Directory.Exists(netfld))
                {
                    Directory.CreateDirectory(netfld);
                }
                if (cam.Action_image_copy_original_name)
                {
                    dest_path = System.IO.Path.Combine(netfld, Path.GetFileName(CurImg.image_path));
                    Global.Log($"  File copying from {CurImg.image_path} to {dest_path}");
                    System.IO.File.Copy(CurImg.image_path, dest_path, true);
                    ret = true;

                }
                else
                {
                    extension = System.IO.Path.GetExtension(CurImg.image_path);
                    dest_path = System.IO.Path.Combine(netfld, cam.name + extension);
                    Global.Log($"  File copying from {CurImg.image_path} to {dest_path}");
                    System.IO.File.Copy(CurImg.image_path, dest_path, true);
                    ret = true;
                }
            }
            catch (Exception ex)
            {
                Global.Log($"ERROR: Could not copy image {CurImg.image_path} to network path {dest_path}: {Global.ExMsg(ex)}");
            }

            return ret;

        }

        public static Camera GetCamera(String ImageOrNameOrPrefix, bool ReturnDefault = true)
        {
            Camera cam = null;
            try
            {
                ImageOrNameOrPrefix = ImageOrNameOrPrefix.Trim();

                //search by path or filename prefix if we are passed a full path to image file
                if (ImageOrNameOrPrefix.Contains("\\"))
                {
                    string pth = Path.GetDirectoryName(ImageOrNameOrPrefix);
                    string fname = Path.GetFileNameWithoutExtension(ImageOrNameOrPrefix);
                    int index = -1;
                    //&CAM.%Y%m%d_%H%M%S
                    //AIFOSCAMDRIVEWAY.20200827_131840312.jpg
                    //sgrtgrdg - Kopie (2).jpg
                    if (fname.Contains(".") || fname.Contains("-"))
                    {

                        string fileprefix = "";

                        if (fname.Contains("."))
                        {
                            fileprefix = Path.GetFileNameWithoutExtension(ImageOrNameOrPrefix).Split('.')[0].Trim(); //get prefix of inputted file
                        }
                        else if (fname.Contains("-"))
                        {
                            fileprefix = Path.GetFileNameWithoutExtension(ImageOrNameOrPrefix).Split('-')[0].Trim(); //get prefix of inputted file
                        }

                        index = AppSettings.Settings.CameraList.FindIndex(x => x.prefix.ToLower() == fileprefix.ToLower()); //get index of camera with same prefix, is =-1 if no camera has the same prefix 

                        if (index > -1)
                        {
                            //found
                            cam = AppSettings.Settings.CameraList[index];
                        }
                        else
                        {
                            //fall back to camera name
                            //TODO:  Is this right?   What problems will it cause?   This fixes an issue I had were I renamed a camera and it wasnt finding the images.
                            index = AppSettings.Settings.CameraList.FindIndex(x => x.name.Trim().ToLower() == fileprefix.Trim().ToLower()); //get index of camera with same prefix, is =-1 if no camera has the same prefix 
                            if (index > -1)
                            {
                                //found
                                cam = AppSettings.Settings.CameraList[index];
                            }
                        }
                    }

                    //if it is not found, search by the input path
                    if (index == -1)
                    {
                        foreach (Camera ccam in AppSettings.Settings.CameraList)
                        {
                            //If the watched path is c:\bi\cameraname but the full path of found file is 
                            //                       c:\bi\cameraname\date\time\randomefilename.jpg 
                            //we just check the beginning of the path
                            if (!String.IsNullOrWhiteSpace(ccam.input_path) && ccam.input_path.Trim().ToLower().StartsWith(pth.ToLower()))
                            {
                                //found
                                cam = ccam;
                                break;

                            }
                        }

                    }
                    else
                    {
                        //found
                        cam = AppSettings.Settings.CameraList[index];
                    }

                }
                else
                {
                    //find by name or prefix
                    //allow to use wildcards
                    if (ImageOrNameOrPrefix.Contains("*") || ImageOrNameOrPrefix.Contains("?"))
                    {
                        foreach (Camera ccam in AppSettings.Settings.CameraList)
                        {
                            if (Regex.IsMatch(ccam.name, Global.WildCardToRegular(ImageOrNameOrPrefix)) || Regex.IsMatch(ccam.prefix, Global.WildCardToRegular(ImageOrNameOrPrefix)))
                            {
                                cam = ccam;
                                break;
                            }
                        }

                    }
                    else  //find by exact name or prefix
                    {
                        foreach (Camera ccam in AppSettings.Settings.CameraList)
                        {
                            if (ccam.name.ToLower() == ImageOrNameOrPrefix.ToLower() || ccam.prefix.ToLower() == ImageOrNameOrPrefix.ToLower())
                            {
                                cam = ccam;
                                break;
                            }
                        }

                    }

                }

                //if we didnt find a camera see if there is a default camera name we can use without a prefix
                if (cam == null)
                {
                    Global.Log($"WARNING: No enabled camera with the same filename, cameraname, or prefix found for '{ImageOrNameOrPrefix}'");
                    //check if there is a default camera which accepts any prefix, select it
                    if (ReturnDefault)
                    {
                        if (AppSettings.Settings.CameraList.Exists(x => x.prefix.Trim() == ""))
                        {
                            int i = AppSettings.Settings.CameraList.FindIndex(x => x.prefix.Trim() == "");
                            cam = AppSettings.Settings.CameraList[i];
                            Global.Log($"(   Found a default camera: '{cam.name}')");
                        }
                        else
                        {
                            Global.Log("WARNING: No default camera found. Aborting.");
                        }
                    }

                }

            }
            catch (Exception ex)
            {

                Global.Log(Global.ExMsg(ex));
            }

            if (cam == null)
            {
                Global.Log($"Error: Cannot match '{ImageOrNameOrPrefix}' to an existing camera.");
            }

            return cam;

        }

    }
}
