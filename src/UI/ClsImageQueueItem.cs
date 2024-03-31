using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;

namespace AITool
{

    public class ClsImageQueueItem:IDisposable
    {
        private bool disposedValue;

        public string image_path { get; set; } = "";
        public DateTime TimeAdded { get; set; } = DateTime.MinValue;
        public DateTime TimeCreated { get; set; } = DateTime.MinValue;
        public DateTime TimeCreatedUTC { get; set; } = DateTime.MinValue;
        public long QueueWaitMS { get; set; } = 0;
        public long TotalTimeMS { get; set; } = 0;
        public long LifeTimeMS { get; set; } = 0;
        public long DeepStackTimeMS { get; set; } = 0;
        public long FileLockMS { get; set; } = 0;
        public long FileLoadMS { get; set; } = 0;
        public long FileLockErrRetryCnt { get; set; } = 0;
        public long CurQueueSize { get; set; } = 0;
        public ThreadSafe.Integer ErrCount { get; set; } = new ThreadSafe.Integer(0);
        public ThreadSafe.Integer RetryCount { get; set; } = new ThreadSafe.Integer(0);
        public string ResultMessage { get; set; } = "";
        public string LastError { get; set; } = "";
        public int Width { get; set; } = 0;
        public int Height { get; set; } = 0;
        public float DPI { get; set; } = 0;
        public long FileSize { get; set; } = 0;
        private byte[] _imageByteArray = null;
        private bool _valid { get; set; } = false;
        private bool _loaded { get; set; } = false;
        private bool _Temp { get; set; } = false;
        public bool IsValid()
        {
            if (!this._loaded)
                this.LoadImage();
            return this._valid;
        }

        public bool CopyFileTo(string outputFilePath)
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            bool ret = false;

            int bufferSize = 1024 * 1024;
            string copydir = "";

            try
            {
                if (!outputFilePath.Contains("\\"))
                {
                    AITOOL.Log($"Error: Must specify a full path: {outputFilePath}");
                    return false;
                }

                if (this.IsValid())  //loads into memory if not already loaded
                {

                    copydir = Path.GetDirectoryName(outputFilePath);

                    DirectoryInfo d = new DirectoryInfo(copydir);
                    if (d.Root != null && !d.Exists)
                    {
                        //dont try to create if working off root drive
                        d.Create();
                    }


                    //If the destination file exists, wait for exclusive access
                    Global.WaitFileAccessResult result2 = new Global.WaitFileAccessResult();
                    if (File.Exists(outputFilePath))
                    {
                        result2 = Global.WaitForFileAccess(outputFilePath, FileAccess.ReadWrite, FileShare.None, 3000, MinFileSize: 0);
                        if (result2.Success)
                            File.Delete(outputFilePath);
                    }
                    else
                        result2.Success = true;

                    if (result2.Success)
                    {
                        //using (FileStream fileStream = new FileStream(outputFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                        //{
                        //    this._imageMemStream.Position = 0;
                        //    fileStream.SetLength(this._imageMemStream.Length);
                        //    int bytesRead = -1;
                        //    byte[] bytes = new byte[bufferSize];

                        //    while ((bytesRead = this._imageMemStream.Read(bytes, 0, bufferSize)) > 0)
                        //    {
                        //        fileStream.Write(bytes, 0, bytesRead);
                        //    }
                        //}

                        using (FileStream fileStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            fileStream.Write(_imageByteArray, 0, _imageByteArray.Length);
                        }

                        ret = true;
                    }
                    else
                    {
                        AITOOL.Log($"Error: Could not gain access to destination file ({result2.TimeMS}ms, '{result2.ResultString}') {outputFilePath}");
                    }
                }
                else
                {
                    AITOOL.Log($"Error: File not valid: {this.image_path}");
                }

            }
            catch (Exception ex)
            {
                AITOOL.Log($"Error: Copying to {outputFilePath}: {ex.Msg()}");
            }

            return ret;

        }
        public ClsImageQueueItem(String FileName, long CurQueueSize, bool Temp = false)
        {
            this.image_path = FileName;
            this.TimeAdded = DateTime.Now;
            this.CurQueueSize = CurQueueSize;
            this._Temp = Temp;
            FileInfo fi = new FileInfo(this.image_path);
            if (fi.Exists)
            {
                this.TimeCreated = fi.CreationTime;
                this.TimeCreatedUTC = fi.CreationTimeUtc;
                this.FileSize = fi.Length;
            }

        }
        public Bitmap ToBitmap()
        {
            return new Bitmap(this.ToMemStream());
        }

        public Image ToImage()
        {
            return Image.FromStream(this.ToMemStream());
        }

        public MemoryStream ToMemStream()
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            if (this.IsValid())
            {
                try
                {
                    return new MemoryStream(this._imageByteArray);
                }
                catch (Exception ex)
                {
                    AITOOL.Log($"Error: Cannot convert to MemoryStream: {ex.Message}");
                }
            }
            else
            {
                AITOOL.Log($"Error: Cannot convert to MemoryStream because image is not valid.");
            }
            return new MemoryStream();
        }
        public void LoadImage()
        {
            //using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            //since having a lot of trouble with image access problems, try to wait for image to become available, validate the image and load
            //a single time rather than multiple
            Global.WaitFileAccessResult result = new Global.WaitFileAccessResult();
            this._valid = false;
            bool validate = !this._Temp;

            try
            {
                if (!string.IsNullOrEmpty(this.image_path) && File.Exists(this.image_path))
                {
                    Stopwatch sw = Stopwatch.StartNew();
                    do
                    {
                        int MaxWaitMS = 0;
                        int MaxRetries = 0;
                        if (this._Temp)
                        {
                            MaxWaitMS = 500;
                            MaxRetries = 2;
                        }
                        else
                        {
                            MaxWaitMS = 10000;
                            MaxRetries = 100;
                        }

                        result = Global.WaitForFileAccess(this.image_path, FileAccess.Read, FileShare.None, MaxWaitMS, AppSettings.Settings.loop_delay_ms, true, 4096, MaxRetries);

                        this.FileLockMS = sw.ElapsedMilliseconds;
                        this.FileLockErrRetryCnt += result.ErrRetryCnt;

                        if (result.Success)
                        {

                            try
                            {
                                sw.Restart();
                                // Open a FileStream object using the passed in safe file handle.
                                using (FileStream fileStream = new FileStream(result.Handle, FileAccess.Read))
                                {

                                    using System.Drawing.Image img = System.Drawing.Image.FromStream(fileStream, true, validate);

                                    this._valid = img != null && img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg) && img.Width > 0 && img.Height > 0;

                                    this.FileLoadMS = sw.ElapsedMilliseconds;

                                    if (!this._valid)
                                    {
                                        LastError = $"Error: Image file is not jpeg? {this.image_path}";
                                        AITOOL.Log(LastError);
                                        break;
                                    }
                                    else
                                    {
                                        this.Width = img.Width;
                                        this.Height = img.Height;
                                        this.DPI = img.HorizontalResolution;
                                        if (this._Temp)
                                        {
                                            this.FileLoadMS = sw.ElapsedMilliseconds;
                                        }
                                        else
                                        {
                                            //using MemoryStream ms = new MemoryStream();
                                            //fileStream.CopyTo(ms);
                                            using MemoryStream ms = new MemoryStream();
                                            img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                                            this._imageByteArray = ms.ToArray();
                                            this.FileSize = this._imageByteArray.Length;
                                            this.FileLoadMS = sw.ElapsedMilliseconds;
                                            this._valid = true;
                                            AITOOL.Log($"Trace: Image file is valid. Resolution={this.Width}x{this.Height}, LockMS={this.FileLockMS}ms (max={MaxWaitMS}ms), retries={this.FileLockErrRetryCnt}, size={Global.FormatBytes(this._imageByteArray.Length)}: {Path.GetFileName(this.image_path)}");
                                        }
                                        break;
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                this._valid = false;
                                LastError = $"Error: Image is corrupt. LockMS={this.FileLockMS}ms (max={MaxWaitMS}ms), retries={this.FileLockErrRetryCnt}: {Path.GetFileName(this.image_path)} - {ex.Msg()}";
                            }
                            finally
                            {
                                this._loaded = true;

                                if (!result.Handle.IsClosed)
                                {
                                    result.Handle.Close();
                                    result.Handle.Dispose();
                                }
                            }

                        }
                        else
                        {
                            if (this._Temp)
                                LastError = $"Debug: Could not gain access to the image in {result.TimeMS}ms, retries={result.ErrRetryCnt}: {Path.GetFileName(this.image_path)}";
                            else
                                LastError = $"Error: Could not gain access to the image in {result.TimeMS}ms, retries={result.ErrRetryCnt}: {Path.GetFileName(this.image_path)}";
                        }

                        if (this._Temp) //only one loop
                            break;

                    } while ((!result.Success || !this._valid) && sw.ElapsedMilliseconds < 30000);

                }
                else
                {
                    LastError = "Debug: File has been deleted: " + this.image_path;
                }
            }
            catch (Exception ex)
            {

                LastError = $"Error: {ex.Msg()}";
            }
            finally
            {
                if (result.Handle != null && !result.Handle.IsInvalid && !result.Handle.IsClosed)
                {
                    result.Handle.Close();
                    result.Handle.Dispose();
                }
                if (!this._valid && !string.IsNullOrEmpty(LastError))
                    AITOOL.Log(LastError);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    if (this._imageByteArray != null)
                    {
                        this._imageByteArray = null;
                        AITOOL.Log($"Trace: {Path.GetFileName(this.image_path)} Lifetime was {(DateTime.Now - this.TimeCreated).TotalMilliseconds}");
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ClsImageQueueItem()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
