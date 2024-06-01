using Microsoft.Win32;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using static AITool.AITOOL;


namespace AITool
{
    public class DeepstackPlatformJson
    {
        public string PROFILE = "";
        public string CUDA_MODE = "";
    }
    public enum DeepStackTypeEnum
    {
        CPU,
        GPU,
        Unknown
    }
    public class DeepStack
    {

        public string DisplayName = "Unknown";
        public string DisplayVersion = "Unknown";
        public DeepStackTypeEnum Type = DeepStackTypeEnum.Unknown;
        public bool IsNewVersion = false;
        public bool Is2022OrLaterVersion = false;
        public string AdminKey = "";
        public string APIKey = "";
        public string Port = "81";
        public string Mode = "Medium";
        public int Count = 1;
        public string DeepStackFolder = @"C:\DeepStack";
        public string DeepStackEXE = @"C:\DeepStack\DeepStack.exe";
        public string ServerEXE = @"C:\DeepStack\server\DeepStack.exe";
        public string PythonEXE = @"C:\DeepStack\interpreter\python.exe";
        public string RedisEXE = @"C:\DeepStack\redis\redis-server.exe";
        public bool SceneAPIEnabled = false;
        public bool FaceAPIEnabled = false;
        public bool DetectionAPIEnabled = true;
        public bool CustomModelEnabled = false;
        public string CustomModelPath = "";
        public string CustomModelName = "";
        public string CustomModelPort = "";
        public string CustomModelMode = "";
        public bool IsStarted = false;
        public bool HasError = false;
        public bool IsInstalled = false;
        public bool IsActivated = false;
        public bool VisionDetectionRunning = false;
        public bool StopBeforeStart = true;
        public bool NeedsSaving = false;
        public string CommandLine = "";
        public List<Global.ClsProcess> DeepStackProc = new List<Global.ClsProcess>();
        public List<Global.ClsProcess> ServerProc = new List<Global.ClsProcess>();
        public List<Global.ClsProcess> PythonProc = new List<Global.ClsProcess>();
        public List<Global.ClsProcess> RedisProc = new List<Global.ClsProcess>();
        public List<double> ResponseTimeList = new List<double>();  //From this you can get min/max/avg
        public string URLS = "";

        public ThreadSafe.Boolean Starting = new ThreadSafe.Boolean(false);
        public ThreadSafe.Boolean Stopping = new ThreadSafe.Boolean(false);
        public int ExpectedPythonCnt = 0;

        public DeepStack(string AdminKey, string APIKey, string Mode, bool SceneAPIEnabled, bool FaceAPIEnabled, bool DetectionAPIEnabled, string Port, string CustomModelPath, bool StopBeforeStart, string CustomModelName, string CustomModelPort, string CustomModelMode, bool CustomModelEnabled)
        {

            this.Update(AdminKey, APIKey, Mode, SceneAPIEnabled, FaceAPIEnabled, DetectionAPIEnabled, Port, CustomModelPath, StopBeforeStart, CustomModelName, CustomModelPort, CustomModelMode, CustomModelEnabled);

        }

        public void Update(string AdminKey, string APIKey, string Mode, bool SceneAPIEnabled, bool FaceAPIEnabled, bool DetectionAPIEnabled, string Port, string CustomModelPath, bool StopBeforeStart, string CustomModelName, string CustomModelPort, string CustomModelMode, bool CustomModelEnabled)
        {
            this.AdminKey = AdminKey.Trim();
            this.APIKey = APIKey.Trim();
            this.SceneAPIEnabled = SceneAPIEnabled;
            this.FaceAPIEnabled = FaceAPIEnabled;
            this.DetectionAPIEnabled = DetectionAPIEnabled;
            this.CustomModelPath = CustomModelPath.Trim();
            this.CustomModelName = CustomModelName.Trim();
            this.CustomModelEnabled = CustomModelEnabled;
            this.CustomModelPort = CustomModelPort;
            this.CustomModelMode = CustomModelMode;
            this.Port = Port;
            this.Mode = Mode;
            this.Count = this.Port.SplitStr(",|").Count;
            this.StopBeforeStart = StopBeforeStart;

            bool found = this.RefreshDeepstackInfo();

        }
        public bool GetDeepStackRun()
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            bool Ret = false;

            if (this.IsNewVersion)
            {
                this.Count = Global.GetProcessesByPath(this.ServerEXE).Count;


                if (!Global.ProcessValid(this.ServerProc))
                    this.ServerProc = Global.GetProcessesByPath(this.ServerEXE);
                if (!Global.ProcessValid(this.PythonProc))
                    this.PythonProc = Global.GetProcessesByPath(this.PythonEXE);
                if (!Global.ProcessValid(this.RedisProc))
                    this.RedisProc = Global.GetProcessesByPath(this.RedisEXE);

                List<Global.ClsProcess> montys = Global.GetProcessesByPath(this.PythonEXE);

                bool srvvalid = Global.ProcessValid(this.ServerProc);
                bool redvalid = Global.ProcessValid(this.RedisProc);
                bool pytvalid = false;

                if (this.ExpectedPythonCnt == 0)
                {

                    if (this.Is2022OrLaterVersion)
                    {
                        if (this.FaceAPIEnabled)
                            ExpectedPythonCnt = ExpectedPythonCnt + (this.Count * 2);
                        if (this.CustomModelEnabled)
                            ExpectedPythonCnt = ExpectedPythonCnt + (this.Count * 1);
                        if (this.DetectionAPIEnabled)
                            ExpectedPythonCnt = ExpectedPythonCnt + (this.Count * 1);
                        if (this.SceneAPIEnabled)
                            ExpectedPythonCnt = ExpectedPythonCnt + (this.Count * 1);

                    }
                    else
                    {
                        if (this.FaceAPIEnabled)
                            ExpectedPythonCnt = ExpectedPythonCnt + (this.Count * 3);
                        if (this.CustomModelEnabled)
                            ExpectedPythonCnt = ExpectedPythonCnt + (this.Count * 2);
                        if (this.DetectionAPIEnabled)
                            ExpectedPythonCnt = ExpectedPythonCnt + (this.Count * 2);
                        if (this.SceneAPIEnabled)
                            ExpectedPythonCnt = ExpectedPythonCnt + (this.Count * 2);

                    }


                }

                pytvalid = srvvalid && montys.Count >= ExpectedPythonCnt;


                bool allvalid = srvvalid && redvalid && pytvalid;

                bool partvalid = (srvvalid || redvalid || pytvalid || Global.ProcessValid(this.PythonProc));

                if (allvalid)
                {
                    this.HasError = false;
                    this.IsStarted = true;
                    Log("Debug: DeepStack Desktop IS running from " + this.ServerEXE + $": {this.Count} copies + {montys.Count} copies of python.exe");
                }
                else if (partvalid)
                {
                    this.HasError = true;
                    this.IsStarted = true;
                    Log($"Deepstack partially running. Only {montys.Count} out of {ExpectedPythonCnt} copies of python.exe are running.");

                }
                else
                {
                    this.IsStarted = false;
                    this.HasError = false;
                    Log("Debug: DeepStack Desktop NOT running.");
                }
            }
            else if (this.IsInstalled)
            {
                Log("Error: Deepstack v3.4 for Windows is not supported.  Install latest version, or uninstall the windows version on this machine to stop seeing this message.   https://docs.deepstack.cc/windows/index.html");
            }

            return Ret;
        }
        public bool RefreshDeepstackInfo()
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            bool Ret = false;
            this.IsInstalled = false;
            RegistryKey key = null;
            List<string> reglocs = new List<string>();
            //                                                                            {0E2C3125-3440-4622-A82A-3B1E07310EF2}_is1
            reglocs.Add(@"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\{0E2C3125-3440-4622-A82A-3B1E07310EF2}_is1");  //new 2020 beta 
            reglocs.Add(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{0E2C3125-3440-4622-A82A-3B1E07310EF2}_is1");              //check for 64 bit install but I dont think it exists 
            reglocs.Add(@"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\{B976B0A1-C83C-4735-AC7F-196922A2748B}_is1");  //old 32 bit version
            reglocs.Add(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{B976B0A1-C83C-4735-AC7F-196922A2748B}_is1");              //check for 64 bit install but I dont think it exists 


            try
            {

                foreach (string keystr in reglocs)
                {
                    key = Registry.LocalMachine.OpenSubKey(keystr);
                    if (key != null)
                        break;
                }


                if (key != null)
                {
                    this.DisplayName = (string)key.GetValue("DisplayName");
                    this.DisplayVersion = (string)key.GetValue("DisplayVersion");
                    this.IsNewVersion = this.DisplayName.Contains("202") || this.DisplayVersion.Contains("202");
                    int year = this.DisplayVersion.GetWord("", ".").ToInt();
                    this.Is2022OrLaterVersion = year >= 2022;

                    string dspath = (string)key.GetValue("Inno Setup: App Path");
                    if (!string.IsNullOrWhiteSpace(dspath))
                    {
                        Log("Debug: Deepstack Desktop install path found in Uninstall registry: " + dspath);

                        string exepth = Path.Combine(dspath, "server", "DeepStack.exe");
                        if (File.Exists(exepth))
                        {
                            this.IsInstalled = true;
                            this.DeepStackFolder = dspath;
                            this.DeepStackEXE = exepth;

                            this.PythonEXE = Path.Combine(dspath, @"interpreter\python.exe");
                            this.RedisEXE = Path.Combine(dspath, @"redis\redis-server.exe");

                            if (this.IsNewVersion)
                            {
                                this.ServerEXE = exepth;
                            }
                            else
                            {
                                this.ServerEXE = Path.Combine(dspath, @"server\server.exe");
                            }


                            if (!string.Equals(dspath, this.DeepStackFolder, StringComparison.OrdinalIgnoreCase))
                            {
                                Log("Debug: Deepstack running from non-default path: " + dspath);
                                this.NeedsSaving = true;
                            }
                        }
                        else
                        {
                            Log("debug: DeepStack File not found " + exepth);
                        }
                    }
                    else
                    {
                        Log("Error: DeepStack registry App Path not found? 'Inno Setup: App Path'");
                    }

                }


                if (!this.IsInstalled)
                {
                    Log("Debug: DeepStack does not appear to be installed in add/remove programs.");

                    if (File.Exists(this.DeepStackEXE))
                    {
                        //this.DeepStackFolder = Path.GetDirectoryName(this.DeepStackEXE);
                        this.IsInstalled = true;

                    }
                    else
                    {
                        this.IsInstalled = false;
                        Log("Debug: DeepStack NOT installed");
                    }
                }

                if (this.IsInstalled)
                {
                    //get type and version

                    //this file exists with 2020 version:
                    string platformfile = Path.Combine(this.DeepStackFolder, "server", "platform.json");
                    //New 2021 platform.json:
                    //{
                    //    "PROFILE":"windows_native",
                    //    "CUDA_MODE":true
                    //}

                    //OLD 2020 beta platform.json (which did not change between cpu and gpu
                    //{
                    //    "PROFILE":"windows_native",
                    //    "GPU":false
                    //}

                    //For old 2020 beta we have to read SERVER.GO:
                    //   os.Setenv("CUDA_MODE", "True")

                    string servergofile = Path.Combine(this.DeepStackFolder, "server", "server.go");

                    if (File.Exists(platformfile))
                    {
                        this.IsNewVersion = true;
                        this.IsActivated = true;
                        string platcontents = File.ReadAllText(platformfile);

                        if (platcontents.IndexOf("CUDA_MODE", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            //2021 version
                            DeepstackPlatformJson dp = Global.SetJSONString<DeepstackPlatformJson>(platcontents);
                            if (string.Equals(dp.CUDA_MODE, "true", StringComparison.OrdinalIgnoreCase))
                                this.Type = DeepStackTypeEnum.GPU;
                            else
                                this.Type = DeepStackTypeEnum.CPU;
                        }
                        else
                        {
                            //2020 version - server.go file
                            //   os.Setenv("CUDA_MODE", "True")
                            string gocontents = File.ReadAllText(servergofile);
                            if (gocontents.IndexOf("\"CUDA_MODE\", \"True\"", StringComparison.OrdinalIgnoreCase) >= 0 || gocontents.IndexOf("\"CUDA_MODE\",\"True\"", StringComparison.OrdinalIgnoreCase) >= 0)
                                this.Type = DeepStackTypeEnum.GPU;
                            else
                                this.Type = DeepStackTypeEnum.CPU;
                        }


                        //get the version if not already found  (Not always in sync with the registry installer key?):
                        if (string.IsNullOrWhiteSpace(this.DisplayVersion))
                        {
                            List<FileInfo> files = Global.GetFiles(this.DeepStackFolder, "*.iss", SearchOption.TopDirectoryOnly);
                            if (files.Count > 0)
                            {
                                string isscontents = File.ReadAllText(files[0].FullName);
                                //#define MyAppVersion "2020.12.beta"
                                this.DisplayVersion = isscontents.GetWord("MyAppVersion \"", "\"");
                            }
                            else
                            {
                                Log($"Error: Could not find .ISS file in Deepstack folder?");
                            }

                        }

                        Log($"Debug: DeepStack v'{this.DisplayVersion}' ({this.Type}) is installed: " + this.DeepStackEXE);
                        //Try to get running processes in any case
                        bool success = this.GetDeepStackRun();

                        Ret = true;



                    }
                    else
                    {
                        this.IsNewVersion = false;
                        this.Type = DeepStackTypeEnum.CPU;
                        this.DisplayVersion = "3.4";
                        Log("Error: Deepstack v3.4 not supported.  Install version 2020+");


                    }

                }

            }
            catch (Exception ex)
            {

                Log("Error: While detecting DeepStack, got: " + ex.Msg());
            }

            if (key != null)
                key.Dispose();

            return Ret;

        }

        private string LastStdErr = "";
        public string GetStdErr()
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.
            string ret = "";
            try
            {
                string errfile = Path.Combine(Environment.GetEnvironmentVariable("LOCALAPPDATA"), "DeepStack", "logs", "stderr.txt");

                if (File.Exists(errfile))
                {
                    string contents = Global.SafeLoadTextFile(errfile);
                    if (!string.IsNullOrWhiteSpace(contents))
                    {
                        List<string> lines = contents.SplitStr("\r\n", TrimStr: false);
                        for (int i = lines.Count - 1; i >= 0; i--)
                        {
                            if (!string.IsNullOrWhiteSpace(lines[i]) && lines[i].Substring(0) != " " && !lines[i].Contains("Traceback"))
                            {
                                //stderr.txt
                                //  File "C://DeepStack\windows_packages\torch\cuda\__init__.py", line 480, in _lazy_new
                                //    return super(_CudaBase, cls).__new__(cls, *args, **kwargs)
                                //RuntimeError: CUDA out of memory. Tried to allocate 20.00 MiB (GPU 0; 2.00 GiB total capacity; 35.77 MiB already allocated; 0 bytes free; 38.00 MiB reserved in total by PyTorch)

                                //this error happens after sending an image to deepstack - I believe it is still running out of video memory:
                                //  File "C:\DeepStack\intelligencelayer\shared\detection.py", line 138, in objectdetection
                                //    os.remove(img_path)
                                //FileNotFoundError: [WinError 2] The system cannot find the file specified: 'C:\\Users\\Vorlon\\AppData\\Local\\Temp\\DeepStack\\83e9c5b0-d698-44f3-a8df-d19655d9f7da'

                                if (lines[i].Trim() != LastStdErr)
                                {
                                    ret = lines[i].Trim();
                                    LastStdErr = ret;
                                }
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"Error: {ex.Msg()}");
            }

            return ret;
        }

        public void PrintDeepStackError()
        {
            if (!this.IsInstalled)
                return;

            string err = this.GetStdErr();

            if (!string.IsNullOrEmpty(err))
            {
                if (!string.IsNullOrEmpty(err))
                    Log($"Error: Last Deepstack STDERR.TXT error is '{err}'");
            }
        }
        public async Task<bool> StartDeepstackAsync(bool ForceRestart = false)
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.
            return await Task.Run(() => this.StartDeepstack(ForceRestart));
        }
        public bool StartDeepstack(bool ForceRestart = false)
        {
            //This error happens when you run out of video memory:
            //stderr.txt
            //  File "C://DeepStack\windows_packages\torch\cuda\__init__.py", line 480, in _lazy_new
            //    return super(_CudaBase, cls).__new__(cls, *args, **kwargs)
            //RuntimeError: CUDA out of memory. Tried to allocate 20.00 MiB (GPU 0; 2.00 GiB total capacity; 35.77 MiB already allocated; 0 bytes free; 38.00 MiB reserved in total by PyTorch)

            //this error happens after sending an image to deepstack - I believe it is still running out of video memory:
            //  File "C:\DeepStack\intelligencelayer\shared\detection.py", line 138, in objectdetection
            //    os.remove(img_path)
            //FileNotFoundError: [WinError 2] The system cannot find the file specified: 'C:\\Users\\Vorlon\\AppData\\Local\\Temp\\DeepStack\\83e9c5b0-d698-44f3-a8df-d19655d9f7da'



            if (this.Starting)
            {
                Log("Already starting?");
                return false;
            }

            this.Starting= true;

            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            bool Ret = false;

            try
            {

                if (!this.IsInstalled)
                {
                    Log("Error: Cannot start because not installed.");
                    this.IsStarted = false;
                    return Ret;
                }
                else
                {
                    if (this.IsStarted)
                    {
                        if (this.StopBeforeStart || ForceRestart)
                        {
                            Log("Debug: Stopping already running DeepStack instance...");
                            Global.UpdateProgressBar("Stopping Deepstack...", 1, 1, 1);
                            this.StopDeepstack();
                            Thread.Sleep(250);
                        }
                        else
                        {
                            Log("Debug: Deepstack is already running, not re-starting due to 'deepstack_stopbeforestart' setting = false in aitool.settings.json file.");
                            return Ret;
                        }
                    }

                    Log("Starting DeepStack...");
                    Global.UpdateProgressBar("Starting Deepstack...", 1, 1, 1);

                }

                Stopwatch SW = Stopwatch.StartNew();
                this.URLS = "";
                this.CommandLine = "";
                int ccnt = 0;
                int dcnt = 0;
                ExpectedPythonCnt = 0;

                if (this.IsNewVersion)
                {
                    //start the custom model(s)
                    if (this.CustomModelEnabled)
                    {
                        List<string> cports = this.CustomModelPort.SplitStr(",;|");
                        List<string> cpaths = this.CustomModelPath.SplitStr(",;|");
                        List<string> cnames = this.CustomModelName.SplitStr(",;|");
                        List<string> cmodes = this.CustomModelMode.SplitStr(",;|");

                        this.Count = cports.Count;

                        if (cports.Count > 0 && cpaths.Count > 0 && cnames.Count > 0 && cports.Count == cpaths.Count && cports.Count == cnames.Count && cmodes.Count == cnames.Count)
                        {
                            for (int i = 0; i < cports.Count; i++)
                            {

                                if (!Directory.Exists(cpaths[i]))
                                {
                                    Log($"Error: Path '{cpaths[i]}' does not exist.");
                                    continue;
                                }

                                if (Global.IsLocalPortInUse(Convert.ToInt32(cports[i])))
                                {
                                    Log($"Error: Port {cports[i]} is already open, so cannot start deepstack.exe using that port.");
                                    continue;
                                }


                                Global.ClsProcess prc = new Global.ClsProcess();
                                prc.process.StartInfo.FileName = this.DeepStackEXE;
                                prc.process.StartInfo.WorkingDirectory = Path.GetDirectoryName(this.DeepStackEXE);
                                string mode = cmodes.GetStrAtIndex(i).ToLower().UpperFirst();   //mode appears to be case sensitive in the Sept 21 version of deepstack
                                if (mode.IsNotNull())
                                    mode = "--MODE " + mode + " ";

                                prc.process.StartInfo.Arguments = $"--MODELSTORE-DETECTION \"{cpaths[i].Replace("\\", "/")}\" {mode}--PORT {cports[i]}";

                                if (!AppSettings.Settings.deepstack_debug)
                                {
                                    prc.process.StartInfo.CreateNoWindow = true;
                                    prc.process.StartInfo.UseShellExecute = false;
                                    prc.process.StartInfo.RedirectStandardOutput = true;
                                    prc.process.StartInfo.RedirectStandardError = true;
                                    prc.process.EnableRaisingEvents = true;
                                    prc.process.OutputDataReceived += this.DSHandleServerProcMSG;
                                    prc.process.ErrorDataReceived += this.DSHandleServerProcERROR;
                                }
                                else
                                {
                                    prc.process.StartInfo.UseShellExecute = false;
                                }


                                prc.process.Exited += (sender, e) => this.DSProcess_Exited(sender, e, "deepstack.exe"); //new EventHandler(myProcess_Exited);
                                prc.FileName = this.DeepStackEXE;
                                prc.CommandLine = prc.process.StartInfo.Arguments;

                                ccnt++;
                                Log($"Starting Deepstack process Type='DeepStack_Custom' - {ccnt} of {this.Count}: {prc.process.StartInfo.FileName} {prc.process.StartInfo.Arguments}...");
                                prc.process.Start();

                                Global.WaitForProcessToStart(prc.process, 4000, this.DeepStackEXE);

                                if (AppSettings.Settings.deepstack_highpriority)
                                {
                                    prc.process.PriorityClass = ProcessPriorityClass.High;
                                }

                                if (!AppSettings.Settings.deepstack_debug)
                                {
                                    prc.process.BeginOutputReadLine();
                                    prc.process.BeginErrorReadLine();
                                }

                                this.ServerProc.Add(prc);

                                this.CommandLine += $"{prc.FileName} {prc.CommandLine}\r\n";

                                ClsURLItem url = new ClsURLItem($"http://127.0.0.1:{cports[i]}/v1/vision/custom/{cnames[i]}", AppSettings.Settings.AIURLList.Count + 1, URLTypeEnum.DeepStack_Custom);
                                this.URLS += $"{url.ToString()}\r\n";
                                if (AppSettings.Settings.deepstack_autoadd && AppSettings.Settings.deepstack_autoadd && !AppSettings.Settings.AIURLList.Contains(url))
                                {
                                    Log($"Debug: Automatically adding local Windows Deepstack URL Type='{url.Type.ToString()}': " + url.ToString());
                                    AppSettings.Settings.AIURLList.Add(url);
                                }

                                if (this.Is2022OrLaterVersion)
                                    ExpectedPythonCnt += 1;  //1 per port 
                                else
                                    ExpectedPythonCnt += 2;

                                Thread.Sleep(100);

                            }
                        }
                        else
                        {
                            this.CustomModelEnabled = false;
                            Log($"Error: Invalid custom model configuration.  All settings must be configured and have same number of items.");
                        }

                    }

                    List<string> ports = this.Port.SplitStr(",;|");
                    this.Count = ports.Count;

                    if (this.FaceAPIEnabled || this.SceneAPIEnabled || this.DetectionAPIEnabled && ports.Count > 0)
                    {
                        foreach (string CurPort in ports)
                        {

                            if (Global.IsLocalPortInUse(Convert.ToInt32(CurPort)))
                            {
                                Log($"Error: Port {CurPort} is already open, so cannot start deepstack.exe using that port.");
                                continue;
                            }


                            Global.ClsProcess prc = new Global.ClsProcess();
                            prc.process.StartInfo.FileName = this.DeepStackEXE;
                            prc.process.StartInfo.WorkingDirectory = Path.GetDirectoryName(this.DeepStackEXE);
                            string face = "";
                            string scene = "";
                            string detect = "";
                            string admin = "";
                            string api = "";
                            string mode = "";

                            if (this.FaceAPIEnabled)
                                face = $"--VISION-FACE {this.FaceAPIEnabled} ";
                            if (this.SceneAPIEnabled)
                                scene = $"--VISION-SCENE {this.SceneAPIEnabled} ";
                            if (this.DetectionAPIEnabled)
                                detect = $"--VISION-DETECTION {this.DetectionAPIEnabled} ";
                            if (!string.IsNullOrEmpty(this.AdminKey))
                                admin = $"--ADMIN-KEY {this.AdminKey} ";
                            if (!string.IsNullOrEmpty(this.APIKey))
                                api = $"--API-KEY {this.APIKey} ";

                            if (!string.IsNullOrEmpty(this.Mode) && !this.DisplayVersion.Contains("2020"))
                                mode = $"--MODE {this.Mode} ";

                            prc.process.StartInfo.Arguments = $"{face}{scene}{detect}{admin}{api}{mode}--PORT {CurPort}";

                            if (!AppSettings.Settings.deepstack_debug)
                            {
                                prc.process.StartInfo.CreateNoWindow = true;
                                prc.process.StartInfo.UseShellExecute = false;
                                prc.process.StartInfo.RedirectStandardOutput = true;
                                prc.process.StartInfo.RedirectStandardError = true;
                                prc.process.EnableRaisingEvents = true;
                                prc.process.OutputDataReceived += this.DSHandleServerProcMSG;
                                prc.process.ErrorDataReceived += this.DSHandleServerProcERROR;
                            }
                            else
                            {
                                prc.process.StartInfo.UseShellExecute = false;
                            }

                            if (this.DisplayVersion.Contains("2020") && !string.Equals(this.Mode, "medium", StringComparison.OrdinalIgnoreCase))
                                prc.process.StartInfo.EnvironmentVariables["MODE"] = this.Mode;

                            prc.process.Exited += (sender, e) => this.DSProcess_Exited(sender, e, "deepstack.exe"); //new EventHandler(myProcess_Exited);
                            prc.FileName = this.DeepStackEXE;
                            prc.CommandLine = prc.process.StartInfo.Arguments;

                            dcnt++;
                            Log($"Starting {dcnt} of {ports.Count}: {prc.process.StartInfo.FileName} {prc.process.StartInfo.Arguments}...");
                            prc.process.Start();

                            Global.WaitForProcessToStart(prc.process, 4000, this.DeepStackEXE);

                            if (AppSettings.Settings.deepstack_highpriority)
                            {
                                prc.process.PriorityClass = ProcessPriorityClass.High;
                            }

                            if (!AppSettings.Settings.deepstack_debug)
                            {
                                prc.process.BeginOutputReadLine();
                                prc.process.BeginErrorReadLine();
                            }

                            this.ServerProc.Add(prc);

                            this.CommandLine += $"{prc.FileName} {prc.CommandLine}\r\n";

                            if (this.DetectionAPIEnabled)
                            {
                                ClsURLItem url = new ClsURLItem($"http://127.0.0.1:{CurPort}/v1/vision/detection", AppSettings.Settings.AIURLList.Count + 1, URLTypeEnum.DeepStack);
                                this.URLS += $"{url.ToString()}\r\n";
                                if (AppSettings.Settings.deepstack_autoadd && !AppSettings.Settings.AIURLList.Contains(url))
                                {
                                    Log($"Debug: Automatically adding local Windows Deepstack URL Type='{url.Type.ToString()}': " + url.ToString());
                                    AppSettings.Settings.AIURLList.Add(url);
                                }
                                if (this.Is2022OrLaterVersion)
                                    ExpectedPythonCnt = ExpectedPythonCnt + 1;
                                else
                                    ExpectedPythonCnt = ExpectedPythonCnt + 2;


                            }

                            if (this.FaceAPIEnabled)
                            {
                                ClsURLItem url = new ClsURLItem($"http://127.0.0.1:{CurPort}/v1/vision/face/recognize", AppSettings.Settings.AIURLList.Count + 1, URLTypeEnum.DeepStack_Faces);
                                this.URLS += $"{url.ToString()}\r\n";
                                if (AppSettings.Settings.deepstack_autoadd && !AppSettings.Settings.AIURLList.Contains(url))
                                {
                                    Log($"Debug: Automatically adding local Windows Deepstack URL Type='{url.Type.ToString()}': " + url.ToString());
                                    AppSettings.Settings.AIURLList.Add(url);
                                }
                                if (this.Is2022OrLaterVersion)
                                    ExpectedPythonCnt = ExpectedPythonCnt + 2;
                                else
                                    ExpectedPythonCnt = ExpectedPythonCnt + 3;

                            }

                            if (this.SceneAPIEnabled)
                            {
                                ClsURLItem url = new ClsURLItem($"http://127.0.0.1:{CurPort}/v1/vision/scene", AppSettings.Settings.AIURLList.Count + 1, URLTypeEnum.DeepStack_Scene);
                                this.URLS += $"{url.ToString()}\r\n";
                                if (AppSettings.Settings.deepstack_autoadd && !AppSettings.Settings.AIURLList.Contains(url))
                                {
                                    Log($"Debug: Automatically adding local Windows Deepstack URL Type='{url.Type.ToString()}': " + url.ToString());
                                    AppSettings.Settings.AIURLList.Add(url);
                                }
                                if (this.Is2022OrLaterVersion)
                                    ExpectedPythonCnt = ExpectedPythonCnt + 1;
                                else
                                    ExpectedPythonCnt = ExpectedPythonCnt + 2;
                            }

                            Thread.Sleep(100);
                        }

                        this.Count = this.ServerProc.Count;

                        this.IsStarted = true;
                        this.HasError = false;
                        Ret = true;

                        //Lets wait for the rest of the python.exe processes to spawn and set their priority too (otherwise they are normal)

                        int PythonCnt = 0;
                        int cc = 0;

                        do
                        {

                            List<Global.ClsProcess> montys = Global.GetProcessesByPath(this.PythonEXE);
                            PythonCnt = montys.Count;


                            if (montys.Count >= ExpectedPythonCnt)
                            {
                                //when deepstack is running normaly there will be 2 python.exe processes running for each deepstack.exe
                                //Set priority for each this way since we didnt start them in the first place...
                                if (AppSettings.Settings.deepstack_highpriority)
                                {
                                    foreach (Global.ClsProcess prc in montys)
                                    {
                                        cc++;
                                        if (Global.ProcessValid(prc))
                                        {
                                            try
                                            {
                                                prc.process.PriorityClass = ProcessPriorityClass.High;
                                                prc.process.StartInfo.UseShellExecute = false;
                                                prc.process.StartInfo.RedirectStandardOutput = true;
                                                prc.process.StartInfo.RedirectStandardError = true;
                                                prc.process.EnableRaisingEvents = true;
                                                prc.process.OutputDataReceived += this.DSHandlePythonProcMSG;
                                                prc.process.ErrorDataReceived += this.DSHandlePythonProcERROR;
                                                prc.process.Exited += (sender, e) => this.DSProcess_Exited(sender, e, "python.exe"); //new EventHandler(myProcess_Exited);
                                            }
                                            catch { }

                                        }
                                    }
                                }

                                this.PythonProc = montys;

                                break;
                            }
                            else
                            {
                                Log($"Debug: ...Waiting for {ExpectedPythonCnt} copies of {this.PythonEXE} to start (now={montys.Count})...");
                                Thread.Sleep(250);
                            }

                        } while (SW.ElapsedMilliseconds < 30000);  //wait 30 seconds max

                        this.RedisProc = Global.GetProcessesByPath(this.RedisEXE);

                        if (Global.ProcessValid(this.RedisProc))
                        {
                            if (AppSettings.Settings.deepstack_highpriority)
                            {

                                this.RedisProc[0].process.PriorityClass = ProcessPriorityClass.High;

                            }
                        }
                        else
                        {
                            this.HasError = true;
                            Log($"Error: 1 'redis-server.exe' processes did not start within " + SW.ElapsedMilliseconds + "ms");
                        }

                        if (PythonCnt >= ExpectedPythonCnt)
                        {

                            Log("Debug: Started in " + SW.ElapsedMilliseconds + "ms");
                        }
                        else
                        {
                            this.HasError = true;
                            Log($"Error: Only {PythonCnt} out of {ExpectedPythonCnt} 'python.exe' processes did not start within " + SW.ElapsedMilliseconds + "ms");
                        }

                        if (!this.HasError)
                        {
                            this.IsActivated = true;
                            this.IsStarted = true;
                        }

                    }
                    else
                    {
                        //nothing to enable
                        Log("Debug: DeepStack config is invalid, or only a custom model is enabled.");

                    }
                }
                else
                {
                    Log("Error: DeepStack for Windows v2020 or higher is not installed. https://github.com/johnolafenwa/DeepStack/releases");
                }
            }
            catch (Exception ex)
            {
                this.IsStarted = false;
                this.HasError = true;
                Log("Error: Cannot start: " + ex.Msg());
            }
            finally
            {
                this.Starting = false;
                //this.PrintDeepStackError();
            }

            Global.SendMessage(MessageType.UpdateDeepstackStatus, "Manual start");


            return Ret;

        }

        public async Task<bool> StopDeepstackAsync()
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.
            return await Task.Run(() => this.StopDeepstack());
        }


        public bool StopDeepstack()
        {

            if (this.Stopping)
            {
                Log("Already stopping?");
                return false;
            }

            this.Stopping= true;

            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            bool Ret = false;
            bool err = false;

            Log("Stopping Deepstack...");
            Stopwatch sw = Stopwatch.StartNew();

            //Try to get current running processes in any case
            bool success = this.GetDeepStackRun();

            //more than one python process we need to take care of...  Sometimes MANY 
            bool perr = Global.KillProcesses(this.PythonProc);
            bool rerr = Global.KillProcesses(this.RedisProc);
            bool serr = Global.KillProcesses(this.ServerProc);
            if (serr)
                serr = Global.KillProcesses("C:\\DeepStack\\DeepStack.exe");  //force the old location to be killed also

            err = !perr || !rerr || !serr;

            if (!err)
            {
                this.DeepStackProc = new List<Global.ClsProcess>();
                this.ServerProc = new List<Global.ClsProcess>();
                this.PythonProc = new List<Global.ClsProcess>();
                this.RedisProc = new List<Global.ClsProcess>();
                //this.DeepStackProc = null;
                Log("Debug: Stopped DeepStack in " + sw.ElapsedMilliseconds + "ms");
                Ret = true;
            }
            else
            {
                Log("Could not fully stop - This can happen for a few reasons: 1) This tool did not originally START deepstack.  2) If this tool is 32 bit it cannot stop 64 bit Deepstack process.  Kill manually via task manager - Server.exe, python.exe, redis-server.exe.");
            }

            this.IsStarted = false;

            this.Stopping = false;

            this.HasError = !Ret;

            Global.SendMessage(MessageType.UpdateDeepstackStatus, "Manual stop");

            return Ret;

        }
        public void ResetDeepstack()
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            try
            {
                if (this.IsStarted)
                {
                    Log("Stopping already running DeepStack instance...");
                    this.StopDeepstack();
                }

                Thread.Sleep(250);

                Log("Resetting DeepStack...");

                String curdir = Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE"), ".deepstack");
                if (Directory.Exists(curdir))
                {
                    Log($"Removing {curdir}...");
                    Directory.Delete(curdir, true);
                }
                curdir = Path.Combine(Environment.GetEnvironmentVariable("TEMP"), "DeepStack");
                if (Directory.Exists(curdir))
                {
                    Log($"Removing {curdir}...");
                    Directory.Delete(curdir, true);
                }
                curdir = Path.Combine(Environment.GetEnvironmentVariable("LOCALAPPDATA"), "DeepStack");
                if (Directory.Exists(curdir))
                {
                    Log($"Removing {curdir}...");
                    Directory.Delete(curdir, true);
                }

                List<FileInfo> files = Global.GetFiles(this.DeepStackFolder, "*.pyc");
                if (files.Count > 0)
                {
                    Log($"Removing {files.Count} compiled Python files {this.DeepStackFolder}\\*.PYC...");
                    foreach (FileInfo fi in files)
                    {
                        fi.Delete();
                    }
                }

            }
            catch (Exception ex)
            {

                Log($"Error: {ex.Msg()}");
            }
        }
        private void DSProcess_Exited(object sender, System.EventArgs e, string Name)
        {

            string output = "Debug: DeepStack>> Process exited: ";
            if (sender != null)
            {
                Process prc = (Process)sender;
                try
                {
                    //if (!prc.HasExited)
                    //{
                    output += $"Debug: Name='{Name}'";
                    //}
                    string err = prc.ExitCode.ToString();
                    if (prc.ExitCode == 0)
                    {
                        err += " (Normal exit)";
                    }
                    else
                    {
                        err += " (Process killed or exited with error)";
                    }
                    output += $", ExitCode='{err}', Runtime='{Math.Round((prc.ExitTime - prc.StartTime).TotalMilliseconds)}'ms";
                }
                catch
                {

                    output += " (error accessing process properties)";
                }
            }
            if (!Name.Contains("Init:"))
            {
                this.IsStarted = false;
                this.HasError = true;
            }
            Log(output, "");
            Global.SendMessage(MessageType.UpdateDeepstackStatus, "Process exited");


        }

        private void DSHandleRedisProcERROR(object sender, DataReceivedEventArgs line)
        {

            try
            {
                if (string.IsNullOrWhiteSpace(line.Data) || Global.progress == null)
                {
                    return;
                }

                if (AppSettings.Settings.deepstack_debug)
                    Log($"Debug: DeepStack>> STDERR: {line.Data}", "", "", "REDIS-SERVER.EXE");

            }
            catch (Exception)
            {

                throw;
            }
        }
        private void DSHandleRedisProcMSG(object sender, DataReceivedEventArgs line)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(line.Data) || Global.progress == null)
                {
                    return;
                }

                if (AppSettings.Settings.deepstack_debug)
                    Log($"Debug: DeepStack>> {line.Data}", "", "", "REDIS-SERVER.EXE");

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void DSHandleInitProcERROR(object sender, DataReceivedEventArgs line)
        {

            try
            {
                if (string.IsNullOrWhiteSpace(line.Data) || Global.progress == null)
                {
                    return;
                }

                if (AppSettings.Settings.deepstack_debug)
                    Log($"Debug: DeepStack>> Init: STDERR: {line.Data}", "", "PYTHON.EXE");

            }
            catch (Exception)
            {

                throw;
            }
        }
        private void DSHandleInitProcMSG(object sender, DataReceivedEventArgs line)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(line.Data) || Global.progress == null)
                {
                    return;
                }

                if (AppSettings.Settings.deepstack_debug)
                    Log($"Debug: DeepStack>> Init: {line.Data}", "", "", "PYTHON.EXE");

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void DSHandlePythonProcERROR(object sender, DataReceivedEventArgs line)
        {

            try
            {
                if (string.IsNullOrWhiteSpace(line.Data) || Global.progress == null)
                {
                    return;
                }

                if (AppSettings.Settings.deepstack_debug)
                    Log($"Debug: DeepStack>> STDERR: {line.Data}", "", "", "PYTHON.EXE");

            }
            catch (Exception)
            {

                throw;
            }
        }
        private void DSHandlePythonProcMSG(object sender, DataReceivedEventArgs line)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(line.Data) || Global.progress == null)
                {
                    return;
                }

                if (AppSettings.Settings.deepstack_debug)
                    Log($"Debug: DeepStack>> {line.Data}", "", "", "PYTHON.EXE");

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void DSHandleServerProcERROR(object sender, DataReceivedEventArgs line)
        {

            try
            {
                if (string.IsNullOrWhiteSpace(line.Data) || Global.progress == null)
                {
                    return;
                }

                if (AppSettings.Settings.deepstack_debug)
                    Log($"Debug: DeepStack>> STDERR: {line.Data}", "", "", "SERVER.EXE");

            }
            catch (Exception)
            {

                throw;
            }
        }
        private void DSHandleServerProcMSG(object sender, DataReceivedEventArgs line)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(line.Data) || Global.progress == null)
                {
                    return;
                }

                //--VISION-DETECTION True --PORT 84
                Process prc = (Process)sender;
                string dsinfo = $"DeepStack:{prc.StartInfo.Arguments.GetWord("PORT ", " |")}>>";

                //Console.WriteLine(Message);
                if (line.Data.IndexOf("visit localhost to activate deepstack", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    this.IsActivated = false;
                }
                else if (line.Data.IndexOf("deepstack is active", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    this.IsActivated = true;
                }

                if (line.Data.IndexOf("vision/detection", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    this.VisionDetectionRunning = true;
                }

                Log($"Debug: {dsinfo} {line.Data}", "", "", "DEEPSTACK.EXE");

            }
            catch { }
        }


    }
}
