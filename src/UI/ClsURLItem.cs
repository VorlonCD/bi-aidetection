using Amazon;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AITool
{
    public enum URLTypeEnum
    {
        CodeProject_AI,
        CodeProject_AI_Faces,
        CodeProject_AI_Custom,
        CodeProject_AI_Scene,
        CodeProject_AI_Plate,
        CodeProject_AI_IPCAM_Animal,
        CodeProject_AI_IPCAM_Dark,
        CodeProject_AI_IPCAM_General,
        CodeProject_AI_IPCAM_Combined,
        DeepStack,
        DeepStack_Faces,
        DeepStack_Custom,
        DeepStack_Scene,
        DOODS,
        AWSRekognition_Objects,
        AWSRekognition_Faces,
        SightHound_Vehicle,
        SightHound_Person,
        Other,
        Unknown
    }
    [DebuggerDisplay("{this.Name}: {this.url}")]
    public class ClsURLItem:IEquatable<ClsURLItem>
    {

        public URLTypeEnum Type { get; set; } = URLTypeEnum.Unknown;
        public int Order { get; set; } = 0;

        public string Name { get; set; } = "";
        public string url { get; set; } = "";

        [JsonIgnore]
        public bool IsValid { get; set; } = false;
        public bool IsOnline { get; set; } = false;
        [JsonIgnore]
        public ThreadSafe.Boolean InUse { get; set; } = new ThreadSafe.Boolean(false);
        [JsonIgnore]
        public ThreadSafe.Integer AIQueueLength { get; set; } = new ThreadSafe.Integer(0);
        public int AIQueueTimeMS
        {
            get
            {
                if (this.QueueStartTime != DateTime.MinValue)
                {
                    return (int)(DateTime.Now - this.QueueStartTime).TotalMilliseconds;
                }
                else
                {
                    return 0;

                }
            }
        }
        public ThreadSafe.Integer AIQueueSkippedCount { get; set; } = new ThreadSafe.Integer(0);
        public string LastResultMessage { get; set; } = "";
        public string LastSkippedReason { get; set; } = "";
        [JsonIgnore]
        public string NotReadyReason { get; set; } = "";
        [JsonIgnore]
        public bool LastResultSuccess { get; set; } = false;
        [JsonIgnore]
        public ThreadSafe.DateTime LastUsedTime { get; set; } = new ThreadSafe.DateTime(DateTime.MinValue, AppSettings.Settings.DateFormat);
        [JsonIgnore]
        public ThreadSafe.DateTime QueueStartTime { get; set; } = new ThreadSafe.DateTime(DateTime.MinValue, AppSettings.Settings.DateFormat);

        public int LastTimeMS { get; set; } = 0;

        public int AvgTimeMS
        {
            get { return Convert.ToInt32(this.AITimeCalcs.Avg); }
        }
        public ThreadSafe.DateTime LastTestedTime { get; set; } = new ThreadSafe.DateTime(DateTime.MinValue, AppSettings.Settings.DateFormat);

        public MovingCalcs AITimeCalcs { get; set; } = new MovingCalcs(500, "TimeCalcs", true);   //store deepstack time calc in the url
        public MovingCalcs AIQueueLengthCalcs { get; set; } = new MovingCalcs(500, "AIQueueLengthCalcs", false);
        public DateTime LastMaxQueueLengthTime
        {
            get { return this.AIQueueLengthCalcs.LastMaxTime; }
        }
        public MovingCalcs AIQueueTimeCalcs { get; set; } = new MovingCalcs(500, "AIQueueTimeCalcs", true);
        public DateTime LastMaxQueueTimeTime
        {
            get { return this.AIQueueTimeCalcs.LastMaxTime; }
        }

        public ThreadSafe.Boolean Enabled { get; set; } = new ThreadSafe.Boolean(true);
        [JsonIgnore]
        public ThreadSafe.Integer CurErrCount { get; set; } = new ThreadSafe.Integer(0);
        public ThreadSafe.Boolean ErrDisabled { get; set; } = new ThreadSafe.Boolean(false);
        public ThreadSafe.Integer ErrCount { get; set; } = new ThreadSafe.Integer(0);
        public ThreadSafe.Integer ErrsInRowCount { get; set; } = new ThreadSafe.Integer(0);

        public string Cameras { get; set; } = "";
        public int MaxImagesPerMonth { get; set; } = 0;
        public int Threshold_Lower { get; set; } = 0;   //override the cameras threshold since different AI servers may need to be tuned to different values
        public int Threshold_Upper { get; set; } = 100;
        public bool UseAsRefinementServer { get; set; } = false;
        public string RefinementObjects { get; set; } = "";
        [JsonIgnore]
        public ThreadSafe.Boolean RefinementUseCurrentlyValid { get; set; } = new ThreadSafe.Boolean(false);
        [JsonIgnore]
        public ThreadSafe.Integer CurOrder { get; set; } = new ThreadSafe.Integer(0);
        public bool UseOnlyAsLinkedServer { get; set; } = false;
        public bool LinkServerResults { get; set; } = false;
        public string LinkedResultsServerList { get; set; } = "";
        public string ActiveTimeRange { get; set; } = "00:00:00-23:59:59";
        [JsonIgnore]
        public bool IsLocalHost { get; set; } = false;
        public bool IsLocalNetwork { get; set; } = false;
        public string CurSrv { get; set; } = "";
        public int Port { get; set; } = 0;
        public string Host { get; set; } = "";
        public int HttpClientTimeoutSeconds { get; set; } = 0;
        public string DefaultURL { get; set; } = "";
        public string HelpURL { get; set; } = "";
        //[JsonIgnore]
        //public Global.ClsProcess Process { get; set; } = null;
        [JsonIgnore]
        public HttpClient HttpClient { get; set; } = null;
        //public int Count { get; set; } = 0;
        public bool UrlFixed { get; set; } = false;
        public bool ExternalSettingsValid { get; set; } = false;
        public bool IgnoreOfflineError { get; set; } = false;  //if we cant even ping the server, we can skip it without giving an error.  This might be useful for servers that are only available at certain times of the day
        public bool AllowAIServerBasedQueue { get; set; } = false;
        public int AIMaxQueueLength { get; set; } = 16;
        public int SkipIfImgQueueLengthLarger { get; set; } = 8;
        public int SkipIfAIQueueTimeOverSecs { get; set; } = 300;  //300=5 mins
        public override string ToString()
        {
            return this.url;
        }

        public bool IsServerReady(int CurrentImgQueueLength, int ValidServerCnt)
        {
            if (!Enabled)
            {
                NotReadyReason = "Disabled";
                return false;
            }
            if (ErrDisabled)
            {
                NotReadyReason = "ErrDisabled";
                return false;
            }
            if (AllowAIServerBasedQueue)
            {
                //only return false if there is something in the queue and we actually have other AITOOL urls to use
                if (this.AIQueueLength > 1 && ValidServerCnt > 1)
                {
                    if (AIQueueLength > AIMaxQueueLength)
                    {
                        NotReadyReason = "AIServerQueueFull";
                        AIQueueSkippedCount++;
                        return false;
                    }
                    //if the AI server queue has anything in it and our regular image queue is above a certain length, then skip this server to give others a chance
                    //if there is only 1 server, then we will still use it since nothing to skip to.
                    if (CurrentImgQueueLength > this.SkipIfImgQueueLengthLarger)
                    {
                        NotReadyReason = "ImgQueueToHigh";
                        AIQueueSkippedCount++;
                        return false;
                    }

                    int queueTimeSecs = (int)(this.AIQueueTimeMS / 1000);
                    if (queueTimeSecs > SkipIfAIQueueTimeOverSecs)
                    {
                        NotReadyReason = "AIServerQueueTimeMaxed";
                        AIQueueSkippedCount++;
                        return false;
                    }
                }
            }
            if (!AllowAIServerBasedQueue && InUse)
            {
                NotReadyReason = "InUse";
                return false;
            }

            NotReadyReason = "[Ready]";
            return true;

        }
        /// <summary>
        /// This will reduce the queue count by 1 until it reaches 0
        /// </summary>
        /// <returns>True if still in use</returns>
        public bool DecrementQueue()
        {
            this.AIQueueLength.Decrement(0);
            bool ret = this.AIQueueLength == 0;
            if (ret)
            {
                this.AIQueueTimeCalcs.AddToCalc(this.AIQueueTimeMS);
                this.QueueStartTime = new ThreadSafe.DateTime(DateTime.MinValue, AppSettings.Settings.DateFormat);
                this.InUse = false;
            }
            return ret;
        }

        public void IncrementQueue(DateTime? time = null)
        {
            this.AIQueueLength++;
            this.InUse = true;
            if (AIQueueLength == 1)
            {
                if (time.HasValue)
                {
                    this.LastUsedTime = new ThreadSafe.DateTime(time.Value, AppSettings.Settings.DateFormat);
                }
                else
                {
                    this.LastUsedTime = new ThreadSafe.DateTime(DateTime.Now, AppSettings.Settings.DateFormat);
                }

                this.QueueStartTime = new ThreadSafe.DateTime(DateTime.Now, AppSettings.Settings.DateFormat);
            }

            this.AIQueueLengthCalcs.AddToCalc(this.AIQueueLength);
        }
        public void IncrementError()
        {
            this.CurErrCount++;
            this.ErrCount++;
        }

        public TimeSpan GetTimeout()
        {
            if (this.HttpClientTimeoutSeconds > 0)
                return TimeSpan.FromSeconds(this.HttpClientTimeoutSeconds);
            else if (this.IsLocalNetwork)
                return TimeSpan.FromSeconds(AppSettings.Settings.HTTPClientLocalTimeoutSeconds);
            else
                return TimeSpan.FromSeconds(AppSettings.Settings.HTTPClientRemoteTimeoutSeconds);

        }

        [JsonConstructor]
        public ClsURLItem() { }

        public ClsURLItem(String url, int Order, URLTypeEnum type)
        {
            //this.Name = name.Trim();
            this.UrlFixed = false;
            this.url = url.Trim();
            this.Type = type;
            this.Order = Order;
            this.Update(true);
        }

        public async Task<bool> CheckIfOnlineAsync()
        {

            bool IsAWS = this.Type == URLTypeEnum.AWSRekognition_Objects || this.Type == URLTypeEnum.AWSRekognition_Faces;


            if (this.IsValid && this.Host.IsNotEmpty() && !IsAWS)
            {
                if (this.IsLocalHost)
                {
                    this.IsOnline = true;  //assume true always for localhost
                }
                else
                {
                    Global.ClsPingOut po;

                    if (this.IsLocalNetwork)
                    {
                        po = await Global.IsConnected(this.Host, 15);
                    }
                    else
                    {
                        po = await Global.IsConnected(this.Host, AppSettings.Settings.MaxWaitForAIServerMS);  //5000ms - overkill?

                    }
                    this.IsOnline = po.Success;
                }
            }
            else
            {
                if (IsAWS)
                {
                    this.IsOnline = true;  //assume true for now
                }
                else
                {
                    this.IsOnline = false;
                }
            }

            return this.IsOnline;
        }

        public bool Update(bool Init)
        {
            bool ret = false;
            this.UrlFixed = false;
            bool WasFixed = false;
            bool HadError = false;

            Uri uri = null;
            //  CodeProject_AI,
            //  CodeProject_AI_Faces,
            //  CodeProject_AI_Custom,
            //  CodeProject_AI_Scene,
            //  CodeProject_AI_Plate,
            //  CodeProject_AI_IPCAM_Animal,
            //  CodeProject_AI_IPCAM_Dark,
            //  CodeProject_AI_IPCAM_General,
            //  CodeProject_AI_IPCAM_Combined,
            //  CodeProject_AI_IPCAM_Plate,

            bool HasCP = this.url.Has(":32168/v1/vision/detection");
            bool HasCPPlate = this.url.Has(":32168/v1/image/alpr");
            bool HasCPCustom = this.url.Has(":32168/v1/vision/custom");
            bool HasCPAnimal = this.url.Has(":32168/v1/vision/custom/ipcam-animal");
            bool HasCPDark = this.url.Has(":32168/v1/vision/custom/ipcam-dark");
            bool HasCPGeneral = this.url.Has(":32168/v1/vision/custom/ipcam-general");
            bool HasCPCombined = this.url.Has(":32168/v1/vision/custom/ipcam-combined");
            bool HasCPIPCamPlate = this.url.Has(":32168/v1/vision/custom/ipcam-license-plate");
            bool HasCPScene = this.url.Has(":32168/v1/vision/scene");
            bool HasCPFace = this.url.Has(":32168/v1/vision/face/recognize");


            bool HasDoods = this.url.EndsWith("/detect", StringComparison.OrdinalIgnoreCase);
            bool HasAWSObj = this.url.Equals("amazon", StringComparison.OrdinalIgnoreCase) || this.url.Equals("amazon_objects", StringComparison.OrdinalIgnoreCase);
            bool HasAWSFac = this.url.Equals("amazon_faces", StringComparison.OrdinalIgnoreCase);
            bool HasSHPer = this.url.IndexOf("/v1/detections", StringComparison.OrdinalIgnoreCase) >= 0;
            bool HasSHVeh = this.url.IndexOf("/v1/recognition", StringComparison.OrdinalIgnoreCase) >= 0;
            bool HasDSFacRec = this.url.IndexOf("/v1/vision/face/recognize", StringComparison.OrdinalIgnoreCase) >= 0;  //Face Recognition
            bool HasDSFacDet = this.url.IndexOf("/v1/vision/face", StringComparison.OrdinalIgnoreCase) >= 0;  //Face Detections
            bool HasDSCus = this.url.IndexOf("/v1/vision/custom", StringComparison.OrdinalIgnoreCase) >= 0;
            bool HasDSScn = this.url.IndexOf("/v1/vision/scene", StringComparison.OrdinalIgnoreCase) >= 0;
            bool HasDSDet = this.url.IndexOf("/v1/vision/detection", StringComparison.OrdinalIgnoreCase) >= 0;



            //bool ShouldInit = Init || !this.IsValid || string.IsNullOrWhiteSpace(this.url) || (!this.url.Contains("/") && !this.url.Contains("_")) || this.Type == URLTypeEnum.Unknown;
            bool ShouldInit = Init || string.IsNullOrWhiteSpace(this.url) || (!this.url.Contains("/") && !this.url.Contains("_")) || this.Type == URLTypeEnum.Unknown;

            if (ShouldInit)
            {
                if (this.Type == URLTypeEnum.DOODS || HasDoods)
                {
                    this.DefaultURL = "http://127.0.0.1:8080/detect";
                    this.HelpURL = "https://github.com/snowzach/doods";
                    this.Type = URLTypeEnum.DOODS;
                }
                else if (this.Type == URLTypeEnum.AWSRekognition_Objects || HasAWSObj)
                {
                    this.DefaultURL = "Amazon_Objects";
                    this.HelpURL = "https://docs.aws.amazon.com/rekognition/latest/dg/setting-up.html";
                    this.Type = URLTypeEnum.AWSRekognition_Objects;
                    this.MaxImagesPerMonth = 5000;
                }
                else if (this.Type == URLTypeEnum.AWSRekognition_Faces || HasAWSFac)
                {
                    this.DefaultURL = "Amazon_Faces";
                    this.HelpURL = "https://docs.aws.amazon.com/rekognition/latest/dg/setting-up.html";
                    this.Type = URLTypeEnum.AWSRekognition_Faces;
                    this.UseAsRefinementServer = true;
                    this.RefinementObjects = "Person, People, Face";
                    this.MaxImagesPerMonth = 5000;
                }
                else if (this.Type == URLTypeEnum.SightHound_Vehicle || HasSHVeh)
                {
                    this.DefaultURL = "https://dev.sighthoundapi.com/v1/recognition?objectType=vehicle,licenseplate";
                    this.HelpURL = "https://docs.sighthound.com/cloud/recognition/";
                    this.Type = URLTypeEnum.SightHound_Vehicle;
                    this.MaxImagesPerMonth = 5000;
                    this.UseAsRefinementServer = true;
                    this.RefinementObjects = "Ambulance, Car, Truck, Pickup Truck, Bus, SUV, Van, Motorcycle, Motorbike, License Plate, Plate";
                    this.IsLocalHost = false;
                    this.IsLocalNetwork = false;
                    this.HttpClient = null;
                }
                else if (this.Type == URLTypeEnum.SightHound_Person || HasSHPer)
                {
                    this.DefaultURL = "https://dev.sighthoundapi.com/v1/detections?type=face,person&faceOption=gender,age,emotion";
                    this.HelpURL = "https://docs.sighthound.com/cloud/detection/";
                    this.Type = URLTypeEnum.SightHound_Person;
                    this.UseAsRefinementServer = true;
                    this.RefinementObjects = "Person, People, Face";
                    this.IsLocalHost = false;
                    this.IsLocalNetwork = false;
                    this.HttpClient = null;
                    this.MaxImagesPerMonth = 5000;
                }
                else if (this.Type == URLTypeEnum.CodeProject_AI || HasCP)
                {
                    this.DefaultURL = "http://127.0.0.1:32168/v1/vision/detection";
                    this.HelpURL = "https://www.codeproject.com/AI/docs/api/api_reference.html";
                    this.Type = URLTypeEnum.CodeProject_AI;
                    this.AllowAIServerBasedQueue = true;
                }
                else if (this.Type == URLTypeEnum.CodeProject_AI_Faces || HasCPFace)
                {
                    this.DefaultURL = "http://127.0.0.1:32168/v1/vision/face/recognize";
                    this.HelpURL = "https://www.codeproject.com/AI/docs/api/api_reference.html";
                    this.Type = URLTypeEnum.CodeProject_AI_Faces;
                    this.UseAsRefinementServer = true;
                    this.RefinementObjects = "Person, People, Face";
                    this.AllowAIServerBasedQueue = true;
                }
                else if (this.Type == URLTypeEnum.CodeProject_AI_IPCAM_Animal || HasCPAnimal)
                {
                    this.DefaultURL = "http://127.0.0.1:32168/v1/vision/custom/ipcam-animal";
                    this.HelpURL = "https://www.codeproject.com/AI/docs/api/api_reference.html";
                    this.Type = URLTypeEnum.CodeProject_AI_IPCAM_Animal;
                    this.AllowAIServerBasedQueue = true;
                }
                else if (this.Type == URLTypeEnum.CodeProject_AI_IPCAM_Combined || HasCPCombined)
                {
                    this.DefaultURL = "http://127.0.0.1:32168/v1/vision/custom/ipcam-combined";
                    this.HelpURL = "https://www.codeproject.com/AI/docs/api/api_reference.html";
                    this.Type = URLTypeEnum.CodeProject_AI_IPCAM_Combined;
                    this.AllowAIServerBasedQueue = true;
                }
                else if (this.Type == URLTypeEnum.CodeProject_AI_IPCAM_Dark || HasCPDark)
                {
                    this.DefaultURL = "http://127.0.0.1:32168/v1/vision/custom/ipcam-dark";
                    this.HelpURL = "https://www.codeproject.com/AI/docs/api/api_reference.html";
                    this.Type = URLTypeEnum.CodeProject_AI_IPCAM_Dark;
                    this.AllowAIServerBasedQueue = true;
                }
                else if (this.Type == URLTypeEnum.CodeProject_AI_IPCAM_General || HasCPGeneral)
                {
                    this.DefaultURL = "http://127.0.0.1:32168/v1/vision/custom/ipcam-general";
                    this.HelpURL = "https://www.codeproject.com/AI/docs/api/api_reference.html";
                    this.Type = URLTypeEnum.CodeProject_AI_IPCAM_General;
                    this.AllowAIServerBasedQueue = true;
                }
                else if (this.Type == URLTypeEnum.CodeProject_AI_Plate || HasCPPlate)
                {
                    this.DefaultURL = "http://127.0.0.1:32168/v1/image/alpr";
                    this.HelpURL = "https://www.codeproject.com/AI/docs/api/api_reference.html";
                    this.Type = URLTypeEnum.CodeProject_AI_Plate;
                    this.UseAsRefinementServer = true;
                    this.RefinementObjects = "Ambulance, Car, Truck, Pickup Truck, Bus, SUV, Van, Motorcycle, Motorbike, License Plate, Plate";
                    this.AllowAIServerBasedQueue = true;

                }
                else if (this.Type == URLTypeEnum.CodeProject_AI_Scene || HasCPScene)
                {
                    this.DefaultURL = "http://127.0.0.1:32168/v1/vision/scene";
                    this.HelpURL = "https://www.codeproject.com/AI/docs/api/api_reference.html";
                    this.Type = URLTypeEnum.CodeProject_AI_Scene;
                    this.AllowAIServerBasedQueue = true;
                }
                else if (this.Type == URLTypeEnum.CodeProject_AI_Custom || HasCPCustom)
                {
                    this.DefaultURL = "http://127.0.0.1:32168/v1/vision/custom";
                    this.HelpURL = "https://www.codeproject.com/AI/docs/api/api_reference.html";
                    this.Type = URLTypeEnum.CodeProject_AI_Custom;
                    this.AllowAIServerBasedQueue = true;
                }



                else if (this.Type == URLTypeEnum.DeepStack_Faces || HasDSFacRec)
                {
                    this.DefaultURL = "http://127.0.0.1:80/v1/vision/face/recognize";
                    this.HelpURL = "https://docs.deepstack.cc/face-recognition/index.html";
                    this.Type = URLTypeEnum.DeepStack_Faces;
                    this.UseAsRefinementServer = true;
                    this.RefinementObjects = "Person, People, Face";
                }
                else if (this.Type == URLTypeEnum.DeepStack_Scene || HasDSScn)
                {
                    this.DefaultURL = "http://127.0.0.1:80/v1/vision/scene";
                    this.HelpURL = "https://docs.deepstack.cc/face-recognition/index.html";
                    this.Type = URLTypeEnum.DeepStack_Scene;
                    this.UseAsRefinementServer = true;
                    this.RefinementObjects = "*";
                }
                else if (this.Type == URLTypeEnum.DeepStack_Custom || HasDSCus) // assume deepstack //if (this.Type == URLTypeEnum.DeepStack || this.url.IndexOf("/v1/vision/detection", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    this.DefaultURL = "http://127.0.0.1:80/v1/vision/custom/YOUR_CUSTOM_MODEL_NAME_HERE";
                    this.HelpURL = "https://docs.deepstack.cc/custom-models/index.html";
                    this.Type = URLTypeEnum.DeepStack_Custom;
                    if (this.url.Has("dark"))
                        this.UseOnlyAsLinkedServer = false;
                    else
                        this.UseOnlyAsLinkedServer = true;

                }
                else // assume deepstack //if (this.Type == URLTypeEnum.DeepStack || this.url.IndexOf("/v1/vision/detection", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    this.DefaultURL = "http://127.0.0.1:80/v1/vision/detection";
                    this.HelpURL = "https://docs.deepstack.cc/object-detection/index.html";
                    this.Type = URLTypeEnum.DeepStack;
                }

            }

            if (string.IsNullOrWhiteSpace(this.url))
                this.url = this.DefaultURL;

            HasCP = this.url.Has(":32168/v1/vision/detection");
            HasCPPlate = this.url.Has(":32168/v1/image/alpr");
            HasCPAnimal = this.url.Has(":32168/v1/vision/custom/ipcam-animal");
            HasCPDark = this.url.Has(":32168/v1/vision/custom/ipcam-dark");
            HasCPGeneral = this.url.Has(":32168/v1/vision/custom/ipcam-general");
            HasCPCombined = this.url.Has(":32168/v1/vision/custom/ipcam-combined");
            HasCPIPCamPlate = this.url.Has(":32168/v1/vision/custom/ipcam-license-plate");
            HasCPCustom = this.url.Has(":32168/v1/vision/custom");
            HasCPScene = this.url.Has(":32168/v1/vision/scene");
            HasCPFace = this.url.Has(":32168/v1/vision/face/recognize");

            HasDoods = this.url.EndsWith("/detect", StringComparison.OrdinalIgnoreCase);
            HasAWSObj = this.url.Equals("amazon", StringComparison.OrdinalIgnoreCase) || this.url.Equals("amazon_objects", StringComparison.OrdinalIgnoreCase);
            HasAWSFac = this.url.Equals("amazon_faces", StringComparison.OrdinalIgnoreCase);
            HasSHPer = this.url.IndexOf("/v1/detections", StringComparison.OrdinalIgnoreCase) >= 0;
            HasSHVeh = this.url.IndexOf("/v1/recognition", StringComparison.OrdinalIgnoreCase) >= 0;
            HasDSFacRec = this.url.IndexOf("/v1/vision/face/recognize", StringComparison.OrdinalIgnoreCase) >= 0;  //Face Recognition
            HasDSFacDet = this.url.IndexOf("/v1/vision/face", StringComparison.OrdinalIgnoreCase) >= 0;  //Face Detections - Not using this for now
            HasDSCus = this.url.IndexOf("/v1/vision/custom", StringComparison.OrdinalIgnoreCase) >= 0;
            HasDSDet = this.url.IndexOf("/v1/vision/detection", StringComparison.OrdinalIgnoreCase) >= 0;
            HasDSScn = this.url.IndexOf("/v1/vision/scene", StringComparison.OrdinalIgnoreCase) >= 0;

            //================================================================================
            // Try to correct any servers without a full URL
            //================================================================================
            if (this.url.Has(":32168"))
            {
                //do nothing for now - this is a codeproject server
            }
            else if (this.Type == URLTypeEnum.DOODS || HasDoods)
            {


                this.url = Global.UpdateURL(this.url, 0, "/detect", "", ref WasFixed, ref HadError);
                this.Type = URLTypeEnum.DOODS;
            }
            else if (this.Type == URLTypeEnum.AWSRekognition_Objects || HasAWSObj)
            {

                this.url = "Amazon_Objects";
                this.Type = URLTypeEnum.AWSRekognition_Objects;
                this.UrlFixed = true;

            }
            else if (this.Type == URLTypeEnum.AWSRekognition_Faces || HasAWSFac)
            {

                this.Type = URLTypeEnum.AWSRekognition_Faces;
                this.url = "Amazon_Faces";
                this.UrlFixed = true;
            }
            else if (this.Type == URLTypeEnum.SightHound_Vehicle || HasSHVeh)
            {

                this.url = Global.UpdateURL(this.url, 443, "/v1/recognition", "", ref WasFixed, ref HadError);
                this.Type = URLTypeEnum.SightHound_Vehicle;

            }
            else if (this.Type == URLTypeEnum.SightHound_Person || HasSHPer)
            {
                this.url = Global.UpdateURL(this.url, 443, "/v1/detections", "", ref WasFixed, ref HadError);
                this.Type = URLTypeEnum.SightHound_Person;
            }
            else  // assume deepstack, default to detection
            {

                if (!HasDSCus && !HasDSDet && !HasDSScn && !HasDSFacRec && !HasDSFacDet)  //default to regular detection
                {
                    this.url = Global.UpdateURL(this.url, 0, "/v1/vision/detection", "", ref WasFixed, ref HadError);
                    this.Type = URLTypeEnum.DeepStack;
                }
                else if (this.Type == URLTypeEnum.DeepStack_Custom && !HasDSCus)
                {
                    this.url = Global.UpdateURL(this.url, 0, "/v1/vision/custom/", "", ref WasFixed, ref HadError);
                }
                else if (this.Type != URLTypeEnum.DeepStack_Custom && HasDSCus)
                {
                    this.Type = URLTypeEnum.DeepStack_Custom;
                }
                else if (this.Type == URLTypeEnum.DeepStack_Faces && !HasDSFacRec && !HasDSFacDet)
                {
                    this.url = Global.UpdateURL(this.url, 0, "/v1/vision/face/recognize", "", ref WasFixed, ref HadError);
                }
                else if (this.Type != URLTypeEnum.DeepStack_Faces && HasDSFacRec)
                {
                    this.Type = URLTypeEnum.DeepStack_Faces;
                }
                else if (this.Type == URLTypeEnum.DeepStack && !HasDSDet)
                {
                    this.url = Global.UpdateURL(this.url, 0, "/v1/vision/detection", "", ref WasFixed, ref HadError);
                }
                else if (this.Type != URLTypeEnum.DeepStack && HasDSDet)
                {
                    this.Type = URLTypeEnum.DeepStack;
                }

                else if (this.Type == URLTypeEnum.DeepStack_Scene && !HasDSScn)
                {
                    this.url = Global.UpdateURL(this.url, 0, "/v1/vision/scene", "", ref WasFixed, ref HadError);
                }
                else if (this.Type != URLTypeEnum.DeepStack_Scene && HasDSScn)
                {
                    this.Type = URLTypeEnum.DeepStack_Scene;
                }


            }


            //================================================================================
            // Do final validation tests
            //================================================================================

            bool IsAWS = this.Type == URLTypeEnum.AWSRekognition_Objects || this.Type == URLTypeEnum.AWSRekognition_Faces;

            if (!IsAWS)
            {
                if (Global.IsValidURL(this.url) && !HadError)
                {
                    uri = new Uri(this.url);


                    this.Port = uri.Port;
                    this.Host = uri.Host;
                    this.IsLocalHost = Global.IsLocalHost(uri.Host);
                    this.IsLocalNetwork = Global.IsLocalNetwork(uri.Host);

                    if (this.IsLocalHost && !uri.Host.Contains("127."))
                    {
                        //force it to always be 127.0.0.1 for localhost
                        AITOOL.Log($"Debug: Converting localhost from '{uri.Host}' to '127.0.0.1'.  Localhost and 0.0.0.0 do not seem to be reliable.");
                        this.url = Global.UpdateURL(this.url, 0, "", "127.0.0.1", ref WasFixed, ref HadError);
                    }

                    if (url.Has(":32168"))
                    {
                        this.CurSrv = this.Type.ToString() + ":" + this.Name + ":" + uri.Host + ":" + uri.Port;
                    }
                    else if (this.Type == URLTypeEnum.DeepStack)
                    {
                        this.CurSrv = "Deepstack_Objects:" + uri.Host + ":" + uri.Port;
                    }
                    else if (this.Type == URLTypeEnum.DeepStack_Custom)
                    {
                        this.CurSrv = "Deepstack_Custom:" + uri.Host + ":" + uri.Port;
                    }
                    else if (this.Type == URLTypeEnum.DeepStack_Faces)
                    {
                        this.CurSrv = "Deepstack_Faces:" + uri.Host + ":" + uri.Port;
                    }
                    else if (this.Type == URLTypeEnum.DeepStack_Scene)
                    {
                        this.CurSrv = "Deepstack_Scene:" + uri.Host + ":" + uri.Port;
                    }
                    else if (this.Type == URLTypeEnum.DOODS)
                    {
                        this.CurSrv = "DOODS:" + uri.Host + ":" + uri.Port;
                    }
                    else if (this.Type == URLTypeEnum.SightHound_Person)
                    {
                        this.CurSrv = "SightHound_Person:" + uri.Host + ":" + uri.Port; this.IsLocalHost = false; this.IsLocalNetwork = false;
                    }
                    else if (this.Type == URLTypeEnum.SightHound_Vehicle)
                    {
                        this.CurSrv = "SightHound_Vehicle:" + uri.Host + ":" + uri.Port; this.IsLocalHost = false; this.IsLocalNetwork = false;
                    }
                    else
                    {
                        this.CurSrv = "Unknown:" + uri.Host + ":" + uri.Port; this.IsLocalHost = false; this.IsLocalNetwork = false;
                    }

                    ret = (this.Type != URLTypeEnum.Unknown && !string.IsNullOrEmpty(this.CurSrv) && !this.CurSrv.StartsWith("Unknown"));
                }
                else
                {
                    ret = false;
                }
            }
            else
            {
                if (IsAWS)
                {
                    this.CurSrv = this.url + ":" + AppSettings.Settings.AmazonRegionEndpoint;
                    this.IsLocalHost = false; this.IsLocalNetwork = false;
                    if (!this.ExternalSettingsValid || ShouldInit)
                    {
                        string error = AITOOL.UpdateAmazonSettings();
                        if (string.IsNullOrEmpty(error))
                        {
                            this.IsValid = true;
                            this.ExternalSettingsValid = true;
                            ret = true;
                        }
                        else
                        {
                            AITOOL.Log($"Error: {error}");
                            this.IsValid = false;
                            this.ExternalSettingsValid = false;
                            ret = false;
                            this.Enabled = false;
                        }
                    }
                    else
                    {
                        ret = true;
                    }

                }

            }

            this.IsValid = ret;

            this.CheckIfOnlineAsync();

            //disable if needed, but never try reenable if the user disabled by themselves 
            if (!this.IsValid)
            {
                this.Enabled = false;
                AITOOL.Log($"Error: '{this.Type.ToString()}' URL is not known/valid: '{this.url}'");
            }

            if (!IsAWS && this.IsValid && this.HttpClient == null)
            {
                this.HttpClient = new HttpClient();
                this.HttpClient.Timeout = this.GetTimeout();
            }

            //remove duplicates from this.LinkedResultsServerList, assuming they are separated by " ,"
            if (this.LinkedResultsServerList.IsNotEmpty())
            {
                List<string> servers = this.LinkedResultsServerList.SplitStr(",");
                //use linq to remove duplicates from servers in a case insensitive way using distinct:
                servers = servers.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                //loop through servers and add them back to the string as long as AITOOL.GetServer() returns a match that is enabled
                foreach (string server in servers)
                {
                    ClsURLItem srv = AITOOL.GetURL(server, false, true);
                    if (srv != null && srv.Enabled && srv.UseOnlyAsLinkedServer)
                    {
                        this.LinkedResultsServerList = server + " ,";
                    }
                }

                this.LinkedResultsServerList = LinkedResultsServerList.Trim(' ', ',');
            }

            if (WasFixed)
                this.UrlFixed = true;


            return ret;
        }

        public static bool operator ==(ClsURLItem left, ClsURLItem right)
        {
            return EqualityComparer<ClsURLItem>.Default.Equals(left, right);
        }

        public static bool operator !=(ClsURLItem left, ClsURLItem right)
        {
            return !(left == right);


        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ClsURLItem);
        }

        public bool Equals(ClsURLItem other)
        {
            return other != null && this.url.EqualsIgnoreCase(other.url) && this.Name.EqualsIgnoreCase(other.Name) && this.Type == other.Type;
        }

    }
}
