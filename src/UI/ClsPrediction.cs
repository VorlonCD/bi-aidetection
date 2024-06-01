using Amazon.Rekognition.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static AITool.AITOOL;
using Newtonsoft.Json;

namespace AITool
{
    public enum ObjectType
    {
        Object,
        Person,
        Animal,
        Vehicle,
        Face,
        Unknown,
        LicensePlate
    }
    public enum ServerType
    {
        Primary,
        Linked,
        Refine,
        Unknown
    }

    public enum ResultType
    {
        Relevant = 1,
        DynamicMasked = 2,
        StaticMasked = 3,
        ImageMasked = 4,
        NoMask = 5,
        NoConfidence = 6,
        UnwantedObject = 7,
        DuplicateObject = 8,
        RefinementObject = 9,
        CameraNotEnabled = 10,
        Error = 11,
        Unknown = 12,
        TooSmallPercent = 13,
        TooLargePercent = 14,
        TooSmallWidth = 15,
        TooSmallHeight = 16,
        TooLargeWidth = 17,
        TooLargeHeight = 18,
        IgnoredObject = 19,
        RelevantDuplicateObject = 20,
        NotInTimeRange = 21,
        NotEnabled = 22,
        UnknownObject = 23,

    }
    public class ClsPrediction:IEquatable<ClsPrediction>
    {
        private ObjectType _defaultObjType;
        private Camera _cam;
        private ClsImageQueueItem _curimg;
        private ClsURLItem _cururl;

        public string Label { get; set; } = "";
        public string Detail { get; set; } = "";
        public ResultType Result { get; set; } = ResultType.Unknown;
        public ResultType ObjectResult { get; set; } = ResultType.Unknown;
        public double Confidence { get; set; } = 0;
        public double PercentOfImage { get; set; } = 0;

        public ObjectType ObjType { get; set; } = ObjectType.Unknown;
        public ServerType ServerType { get; set; } = ServerType.Unknown;
        public string Server { get; set; } = "";
        public MaskType DynMaskType { get; set; } = MaskType.Unknown;
        public MaskResult DynMaskResult { get; set; } = MaskResult.Unknown;
        public double PercentMatch { get; set; } = 0;
        public double PercentMatchRefinement { get; set; } = 0;
        public MaskType ImgMaskType { get; set; } = MaskType.Unknown;
        public MaskResult ImgMaskResult { get; set; } = MaskResult.Unknown;
        public int DynamicThresholdCount { get; set; } = 0;
        public int ImagePointsOutsideMask { get; set; } = 0;
        public double YMin { get; set; } = 0;
        public double XMin { get; set; } = 0;
        public double YMax { get; set; } = 0;
        public double XMax { get; set; } = 0;
        public string Camera { get; set; } = "";
        public string BICamName { get; set; } = "";
        public double RectWidth { get; set; } = 0;
        public double RectHeight { get; set; } = 0;
        public double RectArea { get; set; } = 0;
        public double AIWidth { get; set; } = 0;
        public double AIHeight { get; set; } = 0;
        public double ImageWidth { get; set; } = 0;
        public double ImageHeight { get; set; } = 0;
        public double ImageArea { get; set; } = 0;
        public int ObjectPriority { get; set; } = 0;
        public string Filename { get; set; } = "";
        public int OriginalOrder { get; set; } = 0;
        public int DupeCount { get; set; } = 0;
        public int RefineMergedCount { get; set; } = 0;
        public DateTime Time { get; set; } = DateTime.MinValue;
        public ClsDeepstackDetection ToDeepstackDetection()
        {
            ClsDeepstackDetection ret = new ClsDeepstackDetection();
            ret.label = this.Label;
            ret.confidence = this.Confidence;
            ret.x_min = this.XMin;
            ret.y_min = this.YMin;
            ret.x_max = this.XMax;
            ret.y_max = this.YMax;
            ret.Detail = this.Detail;
            ret.Server = this.Server;
            return ret;
        }
        private void UpdateImageInfo(ClsImageQueueItem curImg)
        {
            this._curimg = curImg;
            this.ImageHeight = curImg.Height;
            this.ImageWidth = curImg.Width;
            this.Filename = curImg.image_path;
            this.UpdatePercent();
        }

        public bool IsValid()
        {
            return !this.Label.IsEmpty() && this.XMax > 0 && this.YMax > 0 && this.ImageHeight > 0 && this.ImageWidth > 0;
        }
        public void UpdatePercent()
        {
            if (this.IsValid())
            {

                //calculate the percentage of the size of the prediction compared to the image size

                //First update width and height due to past miscalculation
                Rectangle PredRect = this.GetRectangle();

                this.RectWidth = PredRect.Width;
                this.RectHeight = PredRect.Height;
                this.RectArea = PredRect.Area();

                Rectangle img = new Rectangle(0, 0, this.ImageWidth.ToInt(), this.ImageHeight.ToInt());
                this.ImageArea = img.Area();

                this.PercentOfImage = img.PercentOfSize(PredRect);

                if (this.PercentOfImage <= 0)
                    Log($"Error: PercentOfImage is {this.PercentOfImage.ToPercent()}? ImageArea={ImageArea}, PredArea={this.RectArea}, pred.width={this.RectWidth}, pred.height={this.RectHeight}, pred.Xmin={this.XMin}, pred.Ymin={this.YMin}, pred.Xmax={this.XMax}, pred.Ymax={this.YMax}, ImageWidth={this.ImageWidth}, ImageHeight={this.ImageHeight}");

            }
            else
            {
                //Log($"Error: Updated prediction PercentOfImage too soon? Xmax={this.XMax}, Ymax={this.YMax}, ImageWidth={this.ImageWidth}, ImageHeight={this.ImageHeight}");
            }

        }
        public Rectangle GetRectangle()
        {
            return Rectangle.FromLTRB(this.XMin.ToInt(true), this.YMin.ToInt(true), this.XMax.ToInt(true), this.YMax.ToInt(true));
        }
        public RectangleF GetRectangleF()
        {
            return RectangleF.FromLTRB(this.XMin.ToFloat(), this.YMin.ToFloat(true), this.XMax.ToFloat(true), this.YMax.ToFloat(true));
        }
        [JsonConstructor]
        public ClsPrediction()
        {
            this.UpdatePercent();
        }
        public ClsPrediction(ObjectType defaultObjType, Camera cam, Amazon.Rekognition.Model.FaceDetail AiDetectionObject, ClsImageQueueItem curImg, ClsURLItem curURL)
        {
            this._defaultObjType = defaultObjType;
            this._cam = cam;
            this._cururl = curURL;
            this.Camera = cam.Name;
            this.BICamName = cam.BICamName;
            this.Server = curURL.CurSrv;
            this.Time = DateTime.Now;
            this.UpdateImageInfo(curImg);

            if (AiDetectionObject == null || cam == null)
            {
                Log("Error: Prediction or Camera was null?", "", this._cam.Name);
                this.Result = ResultType.Error;
                return;
            }

            this.AIWidth = AiDetectionObject.BoundingBox.Width;
            this.AIHeight = AiDetectionObject.BoundingBox.Height;

            //aws returns a percentage of the image width and height rather than actual pixels
            this.RectHeight = curImg.Height * AiDetectionObject.BoundingBox.Height;
            this.RectWidth = curImg.Width * AiDetectionObject.BoundingBox.Width;


            double right = (curImg.Width * AiDetectionObject.BoundingBox.Left) + this.RectWidth;
            double left = curImg.Width * AiDetectionObject.BoundingBox.Left;

            double top = curImg.Height * AiDetectionObject.BoundingBox.Top;
            double bottom = (curImg.Height * AiDetectionObject.BoundingBox.Top) + this.RectHeight;

            this.XMin = left;
            this.YMin = top;

            this.XMax = right;
            this.YMax = bottom;

            //[{Global.UpperFirst(AiDetectionObject.Attributes.Gender)}, {AiDetectionObject.Attributes.Age}, {Global.UpperFirst(AiDetectionObject.Attributes.Emotion)}] 
            string emotions = "";
            List<Emotion> emotionslist = new List<Emotion>();

            if (AiDetectionObject.Emotions != null && AiDetectionObject.Emotions.Count > 0)
            {
                foreach (Emotion em in AiDetectionObject.Emotions)
                {
                    if (em.Confidence >= 25)
                        emotionslist.Add(em);
                }
                if (emotionslist.Count > 0)
                {
                    //sort so highest conf is first
                    emotionslist = emotionslist.OrderByDescending(a => a.Confidence).ToList();
                    emotions = ", ";
                    int cnt = 0;
                    foreach (Emotion em in emotionslist)
                    {
                        emotions += $"{em.Type.ToString().ToLower().UpperFirst()};";
                        cnt++;
                        if (cnt > 3)
                            break;
                    }
                    emotions = emotions.Trim("; ".ToCharArray());
                }
            }

            string gender = "";
            if (AiDetectionObject.Gender != null)
                gender = $", {AiDetectionObject.Gender.Value}";

            string age = "";
            if (AiDetectionObject.AgeRange != null)
                age = $", {AiDetectionObject.AgeRange.Low}-{AiDetectionObject.AgeRange.High}";

            string smile = "";
            if (AiDetectionObject.Smile.Value)
                smile = ", Smile";

            string eyeglasses = "";
            if (AiDetectionObject.Eyeglasses.Value)
                eyeglasses = ", Eyeglasses";

            string sunglasses = "";
            if (AiDetectionObject.Sunglasses.Value)
                sunglasses = ", Sunglasses";

            string beard = "";
            if (AiDetectionObject.Beard.Value)
                beard = ", Beard";

            string mustache = "";
            if (AiDetectionObject.Mustache.Value)
                mustache = ", Mustache";

            string mouthopen = "";
            if (AiDetectionObject.MouthOpen.Value)
                mouthopen = ", MouthOpen";


            this.Label = "Face";

            this.Detail = $"{gender}{age}{emotions}{smile}{eyeglasses}{sunglasses}{beard}{mustache}{mouthopen}".Trim(", ".ToCharArray());

            this.Confidence = AiDetectionObject.Confidence;

            this.GetObjectType();
            this.UpdateImageInfo(curImg);
        }

        public ClsPrediction(ObjectType defaultObjType, Camera cam, Amazon.Rekognition.Model.Label AiDetectionObject, int InstanceIdx, ClsImageQueueItem curImg, ClsURLItem curURL)
        {
            this._defaultObjType = defaultObjType;
            this._cam = cam;
            this._cururl = curURL;
            this.Camera = cam.Name;
            this.BICamName = cam.BICamName;
            this.Server = curURL.CurSrv;
            this.Time = DateTime.Now;
            this.UpdateImageInfo(curImg);

            if (AiDetectionObject == null || cam == null || string.IsNullOrWhiteSpace(AiDetectionObject.Name))
            {
                Log("Error: Prediction or Camera was null?", "", this._cam.Name);
                this.Result = ResultType.Error;
                return;
            }
            //"Name": "Car",
            //            "Confidence": 98.87621307373047,
            //            "Instances": [
            //                {
            //                    "BoundingBox": {
            //                        "Width": 0.10527367144823074,
            //                        "Height": 0.18472492694854736,
            //                        "Left": 0.0042892382480204105,
            //                        "Top": 0.5051581859588623
            //                    },
            //                    "Confidence": 98.87621307373047
            //                },

            //Rectangle(xmin, ymin, xmax - xmin, ymax - ymin)
            //          x,    y     Width,       Height

            this.AIWidth = AiDetectionObject.Instances[InstanceIdx].BoundingBox.Width;
            this.AIHeight = AiDetectionObject.Instances[InstanceIdx].BoundingBox.Height;

            this.RectHeight = curImg.Height * AiDetectionObject.Instances[InstanceIdx].BoundingBox.Height;
            this.RectWidth = curImg.Width * AiDetectionObject.Instances[InstanceIdx].BoundingBox.Width;

            double right = (curImg.Width * AiDetectionObject.Instances[InstanceIdx].BoundingBox.Left) + this.RectWidth;
            double left = curImg.Width * AiDetectionObject.Instances[InstanceIdx].BoundingBox.Left;

            double top = curImg.Height * AiDetectionObject.Instances[InstanceIdx].BoundingBox.Top;
            double bottom = (curImg.Height * AiDetectionObject.Instances[InstanceIdx].BoundingBox.Top) + this.RectHeight;

            this.XMin = left;
            this.YMin = top;

            this.XMax = right;
            this.YMax = bottom;

            //force first letter to always be capitalized 
            this.Label = AiDetectionObject.Name.UpperFirst();
            //if (AiDetectionObject.Parents != null && AiDetectionObject.Parents.Count > 0)
            //{
            //    foreach (var parent in AiDetectionObject.Parents)
            //    {
            //        this.Detail += $", {parent.Name.UpperFirst()}";
            //    }
            //    this.Detail = this.Detail.Trim(", ".ToCharArray());
            //}

            this.Confidence = AiDetectionObject.Confidence; //AiDetectionObject.Instances[InstanceIdx].Confidence;

            this.GetObjectType();
            this.UpdateImageInfo(curImg);

        }

        public ClsPrediction(ObjectType defaultObjType, Camera cam, SightHoundVehicleObject AiDetectionObject, ClsImageQueueItem curImg, ClsURLItem curURL, SightHoundImage SHImg, bool GetPlate)
        {
            //https://docs.sighthound.com/cloud/recognition/

            this._defaultObjType = defaultObjType;
            this._cam = cam;
            this._cururl = curURL;
            this.Camera = cam.Name;
            this.BICamName = cam.BICamName;
            this.Server = curURL.CurSrv;
            this.Time = DateTime.Now;
            this.UpdateImageInfo(curImg);

            if (AiDetectionObject == null || cam == null || string.IsNullOrWhiteSpace(AiDetectionObject.ObjectType))
            {
                Log("Error: Prediction or Camera was null?", "", this._cam.Name);
                this.Result = ResultType.Error;
                return;
            }

            this.ImageHeight = SHImg.Height; //curImg.Height;
            this.ImageWidth = SHImg.Width; //curImg.Width;

            if (curImg.Height != SHImg.Height)
            {
                Log($"Debug: Original image Height does not match returned height: Original={curImg.Height}, Returned={SHImg.Height}");
            }
            if (curImg.Width != SHImg.Width)
            {
                Log($"Debug: Original image Width does not match returned Width: Original={curImg.Width}, Returned={SHImg.Width}");
            }

            //{
            //    "image": { 
            //    "width":2016,
            //    "orientation":1,
            //    "height":1512
            //},
            //"objects":[  
            //    {  
            //        "vehicleAnnotation":{  
            //            "bounding":{  
            //                "vertices":[  
            //                    {  "x":430, "y":286 },
            //                    {  "x":835, "y":286 },
            //                    {  "x":835, "y":559 },
            //                    {  "x":430, "y":559 }
            //                ]
            //            },
            //            "attributes":{  
            //            "system":{  
            //                "color":{  
            //                    "confidence":0.9968,
            //                    "name":"silver/grey"
            //                },
            //                "make":{  
            //                    "confidence":0.8508,
            //                    "name":"BMW"
            //                },
            //                "model":{  
            //                    "confidence":0.8508,
            //                    "name":"3 Series"
            //                },
            //                "vehicleType":"car"
            //            }
            //            },
            //            "recognitionConfidence":0.8508
            //            "licenseplate":{
            //                "bounding":{
            //                    "vertices":[
            //                        { "x":617, "y":452 },
            //                        { "x":743, "y":452 },
            //                        { "x":743, "y":482 },
            //                        { "x":617, "y":482 }
            //                    ]
            //                },
            //                "attributes":{
            //                    "system":{
            //                        "region":{
            //                            "name":"Florida",
            //                            "confidence":0.9994
            //                        },
            //                        "string":{
            //                            "name":"RTB2",
            //                            "confidence":0.999
            //                        },
            //                        "characters":[
            //                            {
            //                                "bounding":{
            //                                    "vertices":[
            //                                        { "y":455, "x":637 },
            //                                        { "y":455, "x":649 },
            //                                        { "y":473, "x":649 },
            //                                        { "y":473, "x":637 }
            //                                    ]
            //                                },
            //                                "index":0,
            //                                "confidence":0.9999,
            //                                "character":"R"
            //                            },
            //                            {
            //                                "bounding":{
            //                                    "vertices":[
            //                                        { "y":455, "x":648 },
            //                                        { "y":455, "x":661 },
            //                                        { "y":473, "x":661 },
            //                                        { "y":473, "x":648 }
            //                                    ]
            //                                },
            //                                "index":1,
            //                                "confidence":0.9999,
            //                                "character":"T"
            //                            },
            //                            {
            //                                "bounding":{  
            //                                    "vertices":[  
            //                                        { "y":455, "x":671 },
            //                                        { "y":455, "x":684 },
            //                                        { "y":474, "x":684 },
            //                                        { "y":474, "x":671 }
            //                                    ]
            //                                },
            //                                "index":2,
            //                                "confidence":0.9996,
            //                                "character":"B"
            //                            },
            //                            {
            //                                "bounding":{
            //                                    "vertices":[
            //                                        { "y":456, "x":683 },
            //                                        { "y":456, "x":696 },
            //                                        { "y":474, "x":696 },
            //                                        { "y":474, "x":683 }
            //                                    ]
            //                                },
            //                                "index":3,
            //                                "confidence":0.9995,
            //                                "character":"2"
            //                            }
            //                        ]
            //                    }
            //                }
            //            },
            //        },
            //        "objectId":"_vehicle_f3c3d26b-c568-4d98-b6db-b96659fd7766",
            //        "objectType":"vehicle"
            //    }
            //],
            //"requestId":"d25b5e5d22f6431498065e9a25134d59"
            //}



            //Rectangle(xmin, ymin, xmax - xmin, ymax - ymin)
            //          x,    y     Width,       Height


            //
            //    vertices (array): A list of objects that define coordinates of the vertices that surround the Object
            //        x (integer): Horizontal pixel position of the vertex
            //        y (integer): Vertical pixel position of the vertex


            if (!GetPlate)
            {
                // get the bounding box from vertices's 
                List<System.Drawing.Point> pts = new List<System.Drawing.Point>();
                foreach (var pt in AiDetectionObject.VehicleAnnotation.Bounding.Vertices)
                    pts.Add(new System.Drawing.Point(pt.X, pt.Y));

                Rectangle rect = new Rectangle().FromVertices(pts);

                this.AIHeight = rect.Height;
                this.AIWidth = rect.Width;

                this.RectHeight = rect.Height;
                this.RectWidth = rect.Width;

                double right = rect.X + this.RectWidth;
                double left = rect.X;

                double top = rect.Y;
                double bottom = rect.Y + this.RectHeight;

                this.XMin = left;
                this.YMin = top;

                this.XMax = right;
                this.YMax = bottom;

                if (AiDetectionObject.VehicleAnnotation != null && !string.IsNullOrEmpty(AiDetectionObject.VehicleAnnotation.Attributes.System.VehicleType))
                {
                    string type = AiDetectionObject.VehicleAnnotation.Attributes.System.VehicleType.UpperFirst();
                    string color = AiDetectionObject.VehicleAnnotation.Attributes.System.Color.Name.UpperFirst();
                    string make = AiDetectionObject.VehicleAnnotation.Attributes.System.Make.Name.UpperFirst();
                    string model = AiDetectionObject.VehicleAnnotation.Attributes.System.Model.Name.UpperFirst();

                    this.Label = type;
                    this.Detail = $"{color}, {make}, {model}";
                    this.ObjType = ObjectType.Vehicle;

                    //this isnt exactly right, but sighthound doesnt give confidence for person/face, only gender, age
                    this.Confidence = AiDetectionObject.VehicleAnnotation.RecognitionConfidence * 100;
                }
                else
                {
                    this.Confidence = 100;
                }

            }
            else if (AiDetectionObject.VehicleAnnotation.Licenseplate != null && !string.IsNullOrEmpty(AiDetectionObject.VehicleAnnotation.Licenseplate.Attributes.System.String.Name))
            {
                //get the license plate object
                // get the bounding box from vertices's 
                List<System.Drawing.Point> pts = new List<System.Drawing.Point>();
                foreach (var pt in AiDetectionObject.VehicleAnnotation.Licenseplate.Bounding.Vertices)
                    pts.Add(new System.Drawing.Point(pt.X, pt.Y));

                Rectangle rect = new Rectangle().FromVertices(pts); //Global.RectFromVertices(pts);

                this.AIHeight = rect.Height;
                this.AIWidth = rect.Width;

                this.RectHeight = rect.Height;
                this.RectWidth = rect.Width;

                double right = rect.X + this.RectWidth;
                double left = rect.X;

                double top = rect.Y;
                double bottom = rect.Y + this.RectHeight;

                this.XMin = left;
                this.YMin = top;
                this.XMax = right;
                this.YMax = bottom;

                this.Label = "License Plate";
                this.ObjType = ObjectType.LicensePlate;
                this.Detail = $"Plate={AiDetectionObject.VehicleAnnotation.Licenseplate.Attributes.System.Region.Name} {AiDetectionObject.VehicleAnnotation.Licenseplate.Attributes.System.String.Name}";

                //this isnt exactly right, but sighthound doesnt give confidence for person/face, only gender, age
                this.Confidence = AiDetectionObject.VehicleAnnotation.Licenseplate.Attributes.System.String.Confidence * 100;

            }


            this.GetObjectType();
            this.UpdateImageInfo(curImg);

        }
        public ClsPrediction(ObjectType defaultObjType, Camera cam, SightHoundPersonObject AiDetectionObject, ClsImageQueueItem curImg, ClsURLItem curURL, SightHoundImage SHImg)
        {
            this._defaultObjType = defaultObjType;
            this._cam = cam;
            this._cururl = curURL;
            this.Camera = cam.Name;
            this.BICamName = cam.BICamName;
            this.Server = curURL.CurSrv;
            this.Time = DateTime.Now;
            this.UpdateImageInfo(curImg);

            if (AiDetectionObject == null || cam == null || string.IsNullOrWhiteSpace(AiDetectionObject.Type))
            {
                Log("Error: Prediction or Camera was null?", "", this._cam.Name);
                this.Result = ResultType.Error;
                return;
            }

            this.ImageHeight = SHImg.Height; //curImg.Height;
            this.ImageWidth = SHImg.Width; //curImg.Width;

            if (curImg.Height != SHImg.Height)
            {
                Log($"Debug: Original image Height does not match returned height: Original={curImg.Height}, Returned={SHImg.Height}");
            }
            if (curImg.Width != SHImg.Width)
            {
                Log($"Debug: Original image Width does not match returned Width: Original={curImg.Width}, Returned={SHImg.Width}");
            }

            //{
            //    "image": {
            //        "width": 1280, 
            //        "height": 960, 
            //        "orientation": 1
            //    },

            //    "objects": [

            //        {
            //            "type": "person",
            //            "boundingBox": { 
            //                "x": 363, 
            //                "y": 182, 
            //                "height": 778, 
            //                "width": 723
            //            } 
            //        },

            //        {
            //            "type": "face",
            //            "boundingBox": {
            //                "x": 508, 
            //                "y": 305, 
            //                "height": 406, 
            //                "width": 406
            //            },
            //            "attributes": {
            //                "gender": "male", 
            //                "genderConfidence": 0.9883, 
            //                "age":25,
            //                "ageConfidence": 0.2599,
            //                "emotion": "happiness",
            //                "emotionConfidence": 0.9943,
            //                "emotionsAll": {
            //                    "neutral": 0.0018,
            //                    "sadness": 0.0009,
            //                    "disgust": 0.0002,
            //                    "anger": 0.0003,
            //                    "surprise": 0,
            //                    "fear": 0.0022,
            //                    "happiness": 0.9943
            //                },
            //                "pose":{
            //                    "pitch":-18.4849,
            //                    "roll":0.854,
            //                    "yaw":-4.2123
            //                },
            //                "frontal": true
            //            },
            //            "landmarks": {
            //                "faceContour": [[515,447],[517,491]...[872,436]],
            //                "noseBridge": [[710,419],[711,441]...[712,487]],
            //                "noseBall": [[680,519],[696,522]...[742,518]],
            //                "eyebrowRight": [[736,387],[768,376]...[854,394]],
            //                "eyebrowLeft": [[555,413],[578,391]...[679,391]],
            //                "eyeRight": [[753,428],[774,414]...[777,432]],
            //                "eyeRightCenter": [[786,423]],
            //                "eyeLeft": [[597,435],[617,423]...[619,442]],
            //                "eyeLeftCenter": [[630,432]],
            //                "mouthOuter": [[650,590],[674,572]...[675,600]],
            //                "mouthInner": [[661,587],[697,580]...[697,584]]
            //            }
            //        }
            //    ]
            //}


            //Rectangle(xmin, ymin, xmax - xmin, ymax - ymin)
            //          x,    y     Width,       Height


            //boundingBox (object): An object containing x, y, width, and height values 
            //defining the location of the object in the image. The top left pixel of 
            //the image represents coordinate (0,0).

            this.AIHeight = AiDetectionObject.BoundingBox.Height;
            this.AIWidth = AiDetectionObject.BoundingBox.Width;

            this.RectHeight = AiDetectionObject.BoundingBox.Height;
            this.RectWidth = AiDetectionObject.BoundingBox.Width;

            double right = AiDetectionObject.BoundingBox.X + this.RectWidth;
            double left = AiDetectionObject.BoundingBox.X;

            double top = AiDetectionObject.BoundingBox.Y;
            double bottom = AiDetectionObject.BoundingBox.Y + this.RectHeight;

            this.XMin = left;
            this.YMin = top;
            this.XMax = right;
            this.YMax = bottom;

            //force first letter to always be capitalized 
            this.Label = AiDetectionObject.Type.UpperFirst();

            if (AiDetectionObject.Attributes != null && !string.IsNullOrEmpty(AiDetectionObject.Attributes.Gender))
            {
                string emotions = AiDetectionObject.Attributes.Emotion.UpperFirst();  //this is the highest confidence emotion 

                if (AiDetectionObject.Attributes.EmotionsAll != null)
                {
                    List<SightHoundEmotionItem> emotionslist = new List<SightHoundEmotionItem>();
                    if (AiDetectionObject.Attributes.EmotionsAll.Anger > .25)
                        emotionslist.Add(new SightHoundEmotionItem("Anger", AiDetectionObject.Attributes.EmotionsAll.Anger));
                    if (AiDetectionObject.Attributes.EmotionsAll.Disgust > .25)
                        emotionslist.Add(new SightHoundEmotionItem("Disgust", AiDetectionObject.Attributes.EmotionsAll.Disgust));
                    if (AiDetectionObject.Attributes.EmotionsAll.Fear > .25)
                        emotionslist.Add(new SightHoundEmotionItem("Fear", AiDetectionObject.Attributes.EmotionsAll.Fear));
                    if (AiDetectionObject.Attributes.EmotionsAll.Happiness > .25)
                        emotionslist.Add(new SightHoundEmotionItem("Happiness", AiDetectionObject.Attributes.EmotionsAll.Happiness));
                    if (AiDetectionObject.Attributes.EmotionsAll.Neutral > .25)
                        emotionslist.Add(new SightHoundEmotionItem("Neutral", AiDetectionObject.Attributes.EmotionsAll.Neutral));
                    if (AiDetectionObject.Attributes.EmotionsAll.Sadness > .25)
                        emotionslist.Add(new SightHoundEmotionItem("Sadness", AiDetectionObject.Attributes.EmotionsAll.Sadness));
                    if (AiDetectionObject.Attributes.EmotionsAll.Surprise > .25)
                        emotionslist.Add(new SightHoundEmotionItem("Surprise", AiDetectionObject.Attributes.EmotionsAll.Surprise));

                    //sort so highest conf is first
                    emotionslist = emotionslist.OrderByDescending(a => a.Confidence).ToList();
                    emotions = "";
                    int cnt = 0;
                    foreach (SightHoundEmotionItem em in emotionslist)
                    {
                        emotions += $"{em.ToString()};";
                        cnt++;
                        if (cnt > 3)
                            break;
                    }
                    emotions = emotions.Trim("; ".ToCharArray());
                    if (string.IsNullOrEmpty(emotions))
                        emotions = "UnknownEmotions";
                }

                string frontal = "";
                if (AiDetectionObject.Attributes.Frontal)
                    frontal = ", Frontal";

                this.Label = AiDetectionObject.Type.UpperFirst();
                this.Detail = $"{AiDetectionObject.Attributes.Gender.UpperFirst()}, {AiDetectionObject.Attributes.Age}, {emotions}{frontal}";

                //this isnt exactly right, but sighthound doesnt give confidence for person/face, only gender, age
                this.Confidence = AiDetectionObject.Attributes.GenderConfidence * 100;
            }
            else
            {
                this.Confidence = 100;
            }

            this.GetObjectType();
            this.UpdateImageInfo(curImg);

        }
        public ClsPrediction(ObjectType defaultObjType, Camera cam, ClsDeepstackDetection AiDetectionObject, ClsImageQueueItem curImg, ClsURLItem curURL)
        {
            this._defaultObjType = defaultObjType;
            this._cam = cam;
            this._cururl = curURL;
            this.Server = curURL.CurSrv;
            this.Time = DateTime.Now;
            this.Camera = cam.Name;
            this.BICamName = cam.BICamName;

            this.UpdateImageInfo(curImg);


            if (AiDetectionObject == null || cam == null || (string.IsNullOrWhiteSpace(AiDetectionObject.label) && string.IsNullOrWhiteSpace(AiDetectionObject.UserID)))
            {
                Log("Error: Prediction or Camera was null?", "", this._cam.Name);
                this.Result = ResultType.Error;
                return;
            }

            //if running face detection:
            if (!string.IsNullOrWhiteSpace(AiDetectionObject.UserID))
            {
                this.ObjType = ObjectType.Face;
                this.Label = AiDetectionObject.UserID.UpperFirst();
                this.Detail = "";  //"Face, " + Global.UpperFirst(AiDetectionObject.UserID);
            }
            else
            {
                //force first letter to always be capitalized 
                this.Label = AiDetectionObject.label.UpperFirst();
            }

            if (!string.IsNullOrEmpty(AiDetectionObject.Detail))
                this.Detail = AiDetectionObject.Detail.UpperFirst();

            this.XMax = AiDetectionObject.x_max;
            this.YMax = AiDetectionObject.y_max;
            this.XMin = AiDetectionObject.x_min;
            this.YMin = AiDetectionObject.y_min;
            this.Confidence = AiDetectionObject.confidence * 100;  //store as whole number percent 
            this.Filename = curImg.image_path;

            this.AIWidth = this.XMax - this.XMin;
            this.AIHeight = this.YMax - this.YMin;  //TODO:  Bug?  Deepstack returning higher YMIN than YMAX sometimes.  

            this.RectWidth = this.AIWidth;
            this.RectHeight = this.AIHeight;

            this.GetObjectType();
            this.UpdateImageInfo(curImg);

        }

        public ClsPrediction(ObjectType defaultObjType, Camera cam, ClsDoodsDetection AiDetectionObject, ClsImageQueueItem curImg, ClsURLItem curURL)
        {
            this._defaultObjType = defaultObjType;
            this._cam = cam;
            this._cururl = curURL;
            this.Camera = cam.Name;
            this.BICamName = cam.BICamName;
            this.Server = curURL.CurSrv;
            this.Time = DateTime.Now;
            this.UpdateImageInfo(curImg);


            if (AiDetectionObject == null || cam == null || string.IsNullOrWhiteSpace(AiDetectionObject.Label))
            {
                Log("Error: Prediction or Camera was null?", "", this._cam.Name);
                this.Result = ResultType.Error;
                return;
            }

            //force first letter to always be capitalized 
            this.Label = AiDetectionObject.Label.UpperFirst();

            //{
            //         "top":0.09833781,
            //         "left":0.62415826,
            //         "bottom":0.14295554,
            //         "right":0.7029755,
            //         "label":"car",
            //         "confidence":63.28125
            //      }

            // convert percentage values from doods to pixels

            //System.Drawing.Rectangle rect = new System.Drawing.Rectangle(xmin, ymin, xmax - xmin, ymax - ymin);
            //System.Drawing.Rectangle rect = new System.Drawing.Rectangle(x,    y,    width,       height);

            //x,    y    = - coordinate of the UPPER LEFT corner of the rectangle
            //xmin, ymin

            double right = curImg.Width * AiDetectionObject.Right;
            double left = curImg.Width * AiDetectionObject.Left;

            double top = curImg.Height * AiDetectionObject.Top;
            double bottom = curImg.Height * AiDetectionObject.Bottom;

            this.XMin = left;
            this.YMin = top;

            this.XMax = right;
            this.YMax = bottom;

            this.AIWidth = AiDetectionObject.Right - AiDetectionObject.Left;
            this.AIHeight = AiDetectionObject.Bottom - AiDetectionObject.Top;

            this.RectWidth = this.XMax - this.XMin;
            this.RectHeight = this.YMax - this.YMin;

            this.Confidence = AiDetectionObject.Confidence;
            this.Filename = curImg.image_path;

            this.GetObjectType();
            this.UpdateImageInfo(curImg);

        }

        public void AnalyzePrediction(bool SkipDynamicMaskCheck)
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            MaskResultInfo result = new MaskResultInfo();

            try
            {
                if (this.ObjectPriority == 0)
                    this.GetObjectType();

                if (!this.IsValid())
                {
                    Log($"Error: prediction has not been processed yet? Label={this.Label}");
                    this.UpdateImageInfo(this._curimg);

                }

                if (this._cam.enabled)
                {
                    //this.Result = AITOOL.ArePredictionObjectsRelevant(this._cam.triggering_objects_as_string + "," + this._cam.additional_triggering_objects_as_string, "NEW", this, true);
                    this.Result = this._cam.DefaultTriggeringObjects.IsRelevant(this, true, out bool IgnoreImageMask, out bool IgnoreDynamicMask, "NEW");
                    if (this.Result == ResultType.Relevant)
                    {
                        this.Result = this._cam.IsRelevantSize(this);
                        if (this.Result == ResultType.Relevant)
                        {
                            // -> OBJECT IS RELEVANT

                            //if confidence limits are satisfied
                            bool OverrideThreshold = this._cururl != null && (this._cururl.Threshold_Lower > 0 || (this._cururl.Threshold_Upper > 0 && this._cururl.Threshold_Upper < 100));

                            if ((!OverrideThreshold && this.Confidence.Round() >= this._cam.threshold_lower && this.Confidence.Round() <= this._cam.threshold_upper) ||
                                (OverrideThreshold && this.Confidence.Round() >= this._cururl.Threshold_Lower && this.Confidence.Round() <= this._cururl.Threshold_Upper))
                            {
                                // -> OBJECT IS WITHIN CONFIDENCE LIMITS

                                if (!IgnoreImageMask)
                                {
                                    //only if the object is outside of the masked area
                                    result = AITOOL.Outsidemask(this._cam, this.XMin, this.XMax, this.YMin, this.YMax, this._curimg.Width, this._curimg.Height);
                                    this.ImgMaskResult = result.Result;
                                    this.ImgMaskType = result.MaskType;
                                    this.ImagePointsOutsideMask = result.ImagePointsOutsideMask;
                                }
                                else
                                {
                                    //a relevant object was flagged to ignore masks
                                    result.IsMasked = false;
                                    result.Result = MaskResult.ObjectIgnoredMask;
                                    result.MaskType = MaskType.Unknown;
                                    this.ImgMaskResult = result.Result;
                                    this.ImgMaskType = result.MaskType;

                                }

                                if (!result.IsMasked)
                                {
                                    this.Result = ResultType.Relevant;
                                }
                                else if (result.MaskType == MaskType.None) //if the object is in a masked area
                                {
                                    this.Result = ResultType.NoMask;
                                }
                                else if (result.MaskType == MaskType.Image) //if the object is in a masked area
                                {
                                    this.Result = ResultType.ImageMasked;
                                }

                                //check the mask manager if the image mask didnt flag anything
                                if (!result.IsMasked && !IgnoreDynamicMask)
                                {
                                    //check the dynamic or static masks
                                    if (this._cam.maskManager.MaskingEnabled && !SkipDynamicMaskCheck)
                                    {
                                        ObjectPosition currentObject = new ObjectPosition(this.XMin, this.XMax, this.YMin, this.YMax, this.Label,
                                                                                          this._curimg.Width, this._curimg.Height, this._cam.Name, this._curimg.image_path);
                                        //creates history and masked lists for objects returned
                                        result = this._cam.maskManager.CreateDynamicMask(currentObject);
                                        this.DynMaskResult = result.Result;
                                        this.DynMaskType = result.MaskType;
                                        this.DynamicThresholdCount = result.DynamicThresholdCount;
                                        this.PercentMatch = result.PercentMatch;
                                        //there is a dynamic or static mask
                                        if (result.MaskType == MaskType.Dynamic)
                                            this.Result = ResultType.DynamicMasked;
                                        else if (result.MaskType == MaskType.Static)
                                            this.Result = ResultType.StaticMasked;
                                    }
                                    else if (SkipDynamicMaskCheck)
                                    {
                                        this.DynMaskResult = MaskResult.SkippedDynamicMaskCheck;
                                        this.DynMaskType = MaskType.None;

                                    }
                                    else
                                    {
                                        this.DynMaskResult = MaskResult.NotEnabled;
                                    }

                                }

                                if (result.Result == MaskResult.Error || result.Result == MaskResult.Unknown)
                                {
                                    this.Result = ResultType.Error;
                                    Log($"Error: Masking error? '{this._cam.Name}' ('{this.Label}') - DynMaskResult={this.DynMaskResult}, ImgMaskResult={this.ImgMaskResult}", "", this._cam.Name, this._curimg.image_path);
                                }

                            }
                            else //if confidence limits are not satisfied
                            {
                                this.Result = ResultType.NoConfidence;
                            }


                        }
                    }


                }
                else
                {
                    Log($"Debug: Camera not enabled '{this._cam.Name}' ('{this.Label}')", "", this._cam.Name, this._curimg.image_path);
                    this.Result = ResultType.CameraNotEnabled;
                }

            }
            catch (Exception ex)
            {
                Log($"Error: Label '{this.Label}', Camera '{this._cam.Name}': {ex.Msg()}", "", this._cam.Name, this._curimg.image_path);
                this.Result = ResultType.Error;
            }

        }

        public override string ToString()
        {
            //LabelDisplayFormat = "[Detection] [[Detail]] ({0:0}%)"

            if (this._cam == null && !string.IsNullOrEmpty(this.Camera))
                this._cam = AITOOL.GetCamera(this.Camera);

            string DetectionDisplayFormat = "[label] [confidence]";

            if (this._cam == null)
            {
                Log($"Could not match camera '{this.Camera}' to an existing camera. Has it been renamed?");
            }
            else
            {
                if (!string.Equals(this._cam.Name, this.Camera, StringComparison.OrdinalIgnoreCase))
                {
                    Log($"Prediction camera '{this.Camera}' does not match found camera '{this._cam.Name}'.  Has it been renamed?");
                }

                DetectionDisplayFormat = this._cam.DetectionDisplayFormat;
            }
            //Dont call this or we get a stack overflow!
            //string ret = AITOOL.ReplaceParams(this._cam, null, this._curimg, this._cam.DetectionDisplayFormat, this);

            string ret = DetectionDisplayFormat;

            ret = Global.ReplaceCaseInsensitive(ret, "[label]", this.Label); //only gives first detection 
            ret = Global.ReplaceCaseInsensitive(ret, "[detail]", this.Detail);
            ret = Global.ReplaceCaseInsensitive(ret, "[result]", this.Result.ToString());
            ret = Global.ReplaceCaseInsensitive(ret, "[position]", this.PositionString());
            ret = Global.ReplaceCaseInsensitive(ret, "[confidence]", this.ConfidenceString());
            ret = Global.ReplaceCaseInsensitive(ret, "[percentofimage]", this.PercentOfImage.Round().ToString());
            //if there was no detail string, clean up:
            ret = ret.Replace("[]", "").Replace("()", "").Replace("   ", " ").Replace("  ", " ");

            return ret;

        }
        public string PositionString()
        {
            return $"{this.XMin},{this.YMin},{this.XMax},{this.YMax}";
        }
        public string ConfidenceString()
        {
            return $"{String.Format(AppSettings.Settings.DisplayPercentageFormat, this.Confidence)}";
        }

        private void GetObjectType()
        {
            //just to make it so truck and pickup truck are the same object priority
            //eventually we may have to allow for more than one object with the same priority but this is a quick fix
            if (this.Label.EqualsIgnoreCase("pickup truck"))
            {
                this.Label = "Truck";
            }

            string tmp = this.Label.Trim().ToLower();

            //person,   bicycle,   car,   motorcycle,   airplane,
            //bus,   train,   truck,   boat,   traffic light,   fire hydrant,   stop_sign,
            //parking meter,   bench,   bird,   cat,   dog,   horse,   sheep,   cow,   elephant,
            //bear,   zebra, giraffe,   backpack,   umbrella,   handbag,   tie,   suitcase,
            //frisbee,   skis,   snowboard, sports ball,   kite,   baseball bat,   baseball glove,
            //skateboard,   surfboard,   tennis racket, bottle,   wine glass,   cup,   fork,
            //knife,   spoon,   bowl,   banana,   apple,   sandwich,   orange, broccoli,   carrot,
            //hot dog,   pizza,   donot,   cake,   chair,   couch,   potted plant,   bed, dining table,
            //toilet,   tv,   laptop,   mouse,   remote,   keyboard,   cell phone,   microwave,
            //oven,   toaster,   sink,   refrigerator,   book,   clock,   vase,   scissors,   teddy bear,
            //hair dryer, toothbrush.


            if (this.ObjType != ObjectType.Unknown)  //because we can set this in deepstack face detection for example
            {
                this.GetPriority();
                return;
            }

            if (tmp.Equals("person") || tmp.Equals("people") || tmp.Equals("male") || tmp.Equals("female"))
                this.ObjType = ObjectType.Person;
            else if (tmp.Equals("face") || this.Detail.IndexOf("face", StringComparison.OrdinalIgnoreCase) >= 0)
                this.ObjType = ObjectType.Face;
            else if (tmp.Contains("license") || tmp.Contains("plate"))
            {
                this.ObjType = ObjectType.LicensePlate;
                //Plate: ddddddddd
                if (tmp.StartsWith("plate:"))
                {
                    this.Detail = this.Label.GetWord("plate:", "");
                    this.Label = "License Plate";
                }
            }
            else if (tmp.Equals("dog") ||
                     tmp.Equals("cat") ||
                     tmp.Equals("bird") ||
                     tmp.Equals("bear") ||
                     tmp.Equals("sheep") ||
                     tmp.Equals("kangaroo") ||
                     tmp.Equals("deer") ||
                     tmp.Equals("cow") ||
                     tmp.Equals("rabbit") ||
                     tmp.Equals("raccoon") ||
                     tmp.Equals("racoon") ||
                     tmp.Equals("fox") ||
                     tmp.Equals("skunk") ||
                     tmp.Equals("squirrel") ||
                     tmp.Equals("pig") ||
                     tmp.Equals("horse") ||
                     tmp.Equals("elephant") ||
                     tmp.Equals("zebra") ||
                     tmp.Equals("giraffe"))
                this.ObjType = ObjectType.Animal;
            else if (tmp.Equals("car") ||
                     tmp.Contains("Ambulance") ||
                     tmp.Contains("truck") ||
                     tmp.Equals("bus") ||
                     tmp.Equals("van") ||
                     tmp.Equals("suv") ||
                     tmp.Equals("bicycle") ||
                     tmp.Equals("motorcycle") ||
                     tmp.Equals("boat") ||
                     tmp.Equals("train") ||
                     tmp.Equals("airplane"))
            {
                this.ObjType = ObjectType.Vehicle;
            }
            else
                this.ObjType = this._defaultObjType;

            this.GetPriority();

        }

        public void GetPriority()
        {
            //List<string> pris = AppSettings.Settings.ObjectPriority.Split(",", ToLower: true);
            ClsRelevantObject ro = this._cam.DefaultTriggeringObjects.Get(this.Label, AllowEverything: false, out int FoundIDX);
            if (ro != null && ro.Priority > 0)
                this.ObjectPriority = ro.Priority; //pris.IndexOf(tmp);
            else
                this.ObjectPriority = 999;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ClsPrediction);
        }

        public bool Equals(ClsPrediction other)
        {
            if (other == null)
                return false;

            //string tmp1 = this.Label.Trim().ToLower();
            //string tmp2 = other.Label.Trim().ToLower();

            //if (tmp1.Contains("["))
            //    tmp1 = Global.GetWordBetween(tmp1, "", "[").Trim();

            //if (tmp2.Contains("["))
            //    tmp2 = Global.GetWordBetween(tmp2, "", "[").Trim();

            return this.GetHashCode() == other.GetHashCode();
        }

        public override int GetHashCode()
        {
            int hashCode = 1877525906;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Label);
            hashCode = hashCode * -1521134295 + Confidence.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Server);
            hashCode = hashCode * -1521134295 + YMin.GetHashCode();
            hashCode = hashCode * -1521134295 + XMin.GetHashCode();
            hashCode = hashCode * -1521134295 + YMax.GetHashCode();
            hashCode = hashCode * -1521134295 + XMax.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(ClsPrediction left, ClsPrediction right)
        {
            return EqualityComparer<ClsPrediction>.Default.Equals(left, right);
        }

        public static bool operator !=(ClsPrediction left, ClsPrediction right)
        {
            return !(left == right);
        }
    }
}
