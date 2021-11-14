﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AITool.Global;
using static AITool.AITOOL;
using Amazon.Rekognition.Model;
using Newtonsoft.Json;
using AITool.Properties;
using System.Diagnostics;

namespace AITool
{
    public class ClsFace : IEquatable<ClsFace>
    {
        public string Name { get; set; } = "";
        public long Hits { get; set; } = 0;
        public string FaceStoragePath { get; set; } = "";
        public List<ClsFaceFile> Files { get; set; } = new List<ClsFaceFile>();
        [JsonIgnore]
        public Dictionary<string, ClsFaceFile> FilesDic { get; set; } = new Dictionary<string, ClsFaceFile>();
        //public List<ClsFaceFile> Files = new List<ClsFaceFile>();

        [JsonConstructor]
        public ClsFace()
        {
            this.Update();
        }

        public ClsFace(string name)
        {
            Name = name;
            this.Update();
        }

        public void Update()
        {
            this.FaceStoragePath = Path.Combine(AppSettings.Settings.FacesPath, this.Name);
            if (!Directory.Exists(this.FaceStoragePath))
                Directory.CreateDirectory(this.FaceStoragePath);

        }


        public bool TryAddFaceFile(ClsImageQueueItem CurImg, out string Newfilename, int MaxFilesPerFace, int MaxFileAgeDays)
        {
            Newfilename = "";

            if (!CurImg.IsValid())
                return false;

            string fname = Path.GetFileName(CurImg.image_path).ToLower();
            Newfilename = Path.Combine(this.FaceStoragePath, fname);

            if (!this.FilesDic.ContainsKey(fname))
            {
                if (this.Files.Count > MaxFilesPerFace)
                {
                    //remove the first one
                    string firstname = Path.GetFileName(this.Files[0].FilePath).ToLower();
                    this.FilesDic.Remove(firstname);
                    this.Files.RemoveAt(0);
                    if (this.Files[0].Exists)
                        File.Delete(this.Files[0].FilePath);
                }

                if (this.Files.Count > 1 && (DateTime.Now - this.Files[0].DateFileModified).TotalDays > MaxFileAgeDays)
                {
                    //remove the first one
                    string firstname = Path.GetFileName(this.Files[0].FilePath).ToLower();
                    this.FilesDic.Remove(firstname);
                    this.Files.RemoveAt(0);
                    if (this.Files[0].Exists)
                        File.Delete(this.Files[0].FilePath);

                }

                //if in different location, copy it in
                if (!CurImg.image_path.EqualsIgnoreCase(Newfilename))
                {
                    if (!CurImg.CopyFileTo(Newfilename))
                        return false;
                }

                ClsFaceFile ff = new ClsFaceFile(Newfilename, this);

                ff.OriginalPath = CurImg.image_path;

                ff.OriginalCamera = GetCamera(ff.OriginalPath, true).Name;

                this.Files.Add(ff);
                this.FilesDic.Add(fname, ff);

                return true;
            }

            return false;

        }
        public override bool Equals(object obj)
        {
            return Equals(obj as ClsFace);
        }

        public bool Equals(ClsFace other)
        {
            return other != null &&
                   Name.EqualsIgnoreCase(other.Name);
        }

        public override int GetHashCode()
        {
            return 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);
        }

        public static bool operator ==(ClsFace left, ClsFace right)
        {
            return EqualityComparer<ClsFace>.Default.Equals(left, right);
        }

        public static bool operator !=(ClsFace left, ClsFace right)
        {
            return !(left == right);
        }
    }

    public class ClsFaceFile : IEquatable<ClsFaceFile>
    {
        [JsonConstructor]
        public ClsFaceFile()
        {

        }
        public ClsFaceFile(string filepath, ClsFace face)
        {
            this.FilePath = filepath;
            this.Name = Path.GetFileName(filepath);
            this.DateAdded = DateTime.Now;
            this.Update(face);
        }

        public void Update(ClsFace face)
        {
            //first make sure path is correct
            string curpath = Path.GetDirectoryName(this.FilePath);
            if (!curpath.EqualsIgnoreCase(face.FaceStoragePath))
            {
                string filename = Path.GetFileName(this.FilePath);
                this.FilePath = Path.Combine(face.FaceStoragePath, filename);
            }

            if (File.Exists(this.FilePath))
            {
                this.Exists = true;
                this.DateFileModified = new FileInfo(this.FilePath).LastWriteTime;
            }
            else
            {
                this.Exists = false;
            }
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ClsFaceFile);
        }

        public bool Equals(ClsFaceFile other)
        {
            return other != null &&
                   Name.EqualsIgnoreCase(other.Name);
        }

        public override int GetHashCode()
        {
            return 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);
        }

        public string Name { get; set; } = "";
        public int RegisterTimeMS { get; set; } = 0;
        public string FilePath { get; set; } = "";
        public bool Exists { get; set; } = false;
        public bool Keep { get; set; } = false;
        public DateTime DateRegistered { get; set; } = DateTime.MinValue;
        public DateTime DateAdded { get; set; } = DateTime.MinValue;
        public DateTime DateFileModified { get; set; } = DateTime.MinValue;
        public string OriginalPath { get; set; } = "";
        public string OriginalCamera { get; set; } = "Unknown";

        public static bool operator ==(ClsFaceFile left, ClsFaceFile right)
        {
            return EqualityComparer<ClsFaceFile>.Default.Equals(left, right);
        }

        public static bool operator !=(ClsFaceFile left, ClsFaceFile right)
        {
            return !(left == right);
        }
    }
    public class ClsFaceManager
    {
        public int MaxFilesPerFace { get; set; } = 1000;
        public int MaxFileAgeDays { get; set; } = 182;
        public bool SaveUnknownFaces { get; set; } = true;
        public bool SaveKnownFaces { get; set; } = true;
        public string FaceFile { get; set; } = "";
        public List<ClsFace> Faces { get; set; } = new List<ClsFace>();
        private bool NeedsSaving { get; set; } = true;
        private object FaceLock = new object();
        public ClsFaceManager()
        {
            this.FaceFile = Path.Combine(AppSettings.Settings.FacesPath, "Faces.JSON");
            //update in background thread
            Task.Run(this.UpdateFaces);
        }
        public void SaveFaces()
        {
            using var Trace = new Trace();

            lock (FaceLock)
            {
                if (NeedsSaving && this.Faces.IsNotEmpty())
                {
                    Global.WriteToJsonFile<ClsFaceManager>(this.FaceFile, this);
                    NeedsSaving = false;
                }
            }
        }
        public void UpdateFaces()
        {
            using var Trace = new Trace();

            int missingfiles = 0;
            int oldfiles = 0;
            int addedfiles = 0;
            int totalfiles = 0;
            int missingfaces = 0;
            Stopwatch sw = Stopwatch.StartNew();

            try
            {

                lock (FaceLock)
                {

                    Log("Debug: Updating faces...");


                    if (!Directory.Exists(AppSettings.Settings.FacesPath))
                        Directory.CreateDirectory(AppSettings.Settings.FacesPath);

                    this.TryAddFace("Unknown");

                    //Add any existing subfolders as new faces if not already in the list
                    string[] facedirs = Directory.GetDirectories(AppSettings.Settings.FacesPath, "*", SearchOption.TopDirectoryOnly);
                    foreach (var facedir in facedirs)
                        this.TryAddFace(facedir);

                    //delete any faces that dont have a folder
                    for (int i = this.Faces.Count - 1; i >= 0; i--)
                    {
                        if (!this.Faces[i].Name.EqualsIgnoreCase("unknown") && !Directory.Exists(this.Faces[i].FaceStoragePath))
                        {
                            missingfaces++;
                            this.Faces.RemoveAt(i);
                        }
                    }


                    for (int i = 0; i < this.Faces.Count; i++)
                    {

                        foreach (var file in this.Faces[i].Files)
                            file.Update(this.Faces[i]);

                        //remove any missing files
                        var itemsToRemove = this.Faces[i].Files.Where(f => !f.Exists).ToArray();
                        foreach (var item in itemsToRemove)
                        {
                            string firstname = Path.GetFileName(item.FilePath).ToLower();
                            this.Faces[i].Files.Remove(item);
                            this.Faces[i].FilesDic.Remove(firstname);

                        }
                        missingfiles += itemsToRemove.Length;

                        //remove old files
                        itemsToRemove = this.Faces[i].Files.Where(f => (DateTime.Now - f.DateFileModified).TotalDays > this.MaxFileAgeDays && !f.Keep).ToArray();
                        foreach (var item in itemsToRemove)
                        {
                            string firstname = Path.GetFileName(item.FilePath).ToLower();
                            this.Faces[i].Files.Remove(item);
                            this.Faces[i].FilesDic.Remove(firstname);
                            //actually delete the file:
                            File.Delete(item.FilePath);

                        }
                        oldfiles += itemsToRemove.Length;

                        totalfiles = this.Faces[i].Files.Count;

                        //scan the folder for new files
                        List<FileInfo> newfiles = Global.GetFiles(this.Faces[i].FaceStoragePath, "*.jpg;*.jpeg;*.bmp;*.png", SearchOption.TopDirectoryOnly);
                        foreach (FileInfo fi in newfiles)
                        {
                            if ((DateTime.Now - fi.CreationTime).TotalDays > this.MaxFileAgeDays)
                            {
                                fi.Delete();
                            }
                            else if (this.Faces[i].TryAddFaceFile(new ClsImageQueueItem(fi.FullName, 0), out string newfilename, this.MaxFilesPerFace, this.MaxFileAgeDays))
                            {
                                addedfiles++;
                            }
                        }

                    }

                }



            }
            catch (Exception ex)
            {

                Log($"Error: {ex.Msg()}");
            }

            Log($"Updated {this.Faces.Count} faces in {sw.ElapsedMilliseconds}ms. {missingfaces} faces removed. {totalfiles} files. Added {addedfiles} new files, Removed {missingfiles} missing files and {oldfiles} files that were too old.");


        }

        public bool TryAddFaceFile(ClsImageQueueItem CurImg, string face = "")
        {
            if (!CurImg.IsValid())
                return false;

            if (face.IsEmpty())
                face = "Unknown";

            if (face.EqualsIgnoreCase("face"))
                face = "Unknown";

            if (face.EqualsIgnoreCase("unknown"))
            {
                if (!this.SaveUnknownFaces)
                    return false;
            }
            else
            {
                if (!this.SaveKnownFaces)
                    return false;
            }

            ClsFace FoundFace = this.TryAddFace(face);

            bool added = FoundFace.TryAddFaceFile(CurImg, out string Outfilename, this.MaxFilesPerFace, this.MaxFileAgeDays);

            //delete from unknown folder if it was originally from there and it moved
            if (added)
            {
                this.NeedsSaving = true;

                if (CurImg.image_path.Has("\\unknown\\") && !Outfilename.Has("\\unknown\\") &&
                    CurImg.image_path.Has("\\face\\") && !Outfilename.Has("\\face\\"))
                    Global.SafeFileDelete(CurImg.image_path, "TryAddFaceFile");

            }

            return added;
        }

        public ClsFace TryAddFace(string face)
        {
            ClsFace FoundFace = new ClsFace(face);

            int fnd = this.Faces.IndexOf(FoundFace);

            if (fnd > -1)
                FoundFace = this.Faces[fnd];
            else
            {
                lock (FaceLock)
                {
                    this.Faces.Add(FoundFace);
                    this.NeedsSaving = true;
                }
            }

            return FoundFace;
        }

    }
}
