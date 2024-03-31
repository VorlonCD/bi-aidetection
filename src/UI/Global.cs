using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Management;
using System.Media;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

using Innovative.SolarCalculator;

using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;

using NAudio.Wave;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

using static AITool.AITOOL;

namespace AITool
{
    // =============================================================
    // ALL FUNCTIONS HERE ARE GENERIC/SHARED AND NOT UNIQUE TO AITOOL
    // NO direct UI interaction
    // =============================================================

    public static class Global
    {
        public static IProgress<ClsMessage> progress = null;

        //this may speed up json serialization
        public static readonly DefaultContractResolver JSONContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy
            {
                ProcessDictionaryKeys = false,
                OverrideSpecifiedNames = false,
                ProcessExtensionDataNames = false
            }

            //Global.JSONContractResolver.NamingStrategy = new CamelCaseNamingStrategy();
        };
        public static readonly JsonSerializerSettings JSONSettingsPretty = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.All,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            ContractResolver = JSONContractResolver,
            //NullValueHandling = NullValueHandling.Ignore,
            //DefaultValueHandling = DefaultValueHandling.Ignore,
            //ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            // Add other settings that may improve performance for your specific case
        };

        public static readonly JsonSerializerSettings JSONSettingsPerformance = new JsonSerializerSettings
        {
            Formatting = Formatting.None,
            TypeNameHandling = TypeNameHandling.Auto,
            PreserveReferencesHandling = PreserveReferencesHandling.None,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ConstructorHandling = ConstructorHandling.Default,
            ObjectCreationHandling = ObjectCreationHandling.Auto,
            NullValueHandling = NullValueHandling.Ignore,
            MetadataPropertyHandling = MetadataPropertyHandling.Default,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            //NullValueHandling = NullValueHandling.Ignore,
            //DefaultValueHandling = DefaultValueHandling.Ignore,
            //ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            // Add other settings that may improve performance for your specific case
        };

        /// <summary>
        ///     ''' Gets a value indicating whether the application is a windows service.
        ///     ''' </summary>
        ///     ''' <value>
        ///     ''' <c>true</c> if this instance is service; otherwise, <c>false</c>.

        ///     ''' </value>
        public static bool IsService
        {
            get
            {
                // Determining whether or not the host application is a service is
                // an expensive operation (it uses reflection), so we cache the
                // result of the first call to this method so that we don't have to
                // recalculate it every call.

                // If we have not already determined whether or not the application
                // is running as a service...
                if (!_isService.HasValue)
                {

                    // Get details of the host assembly.
                    System.Reflection.Assembly entryAssembly = System.Reflection.Assembly.GetEntryAssembly();

                    // Get the method that was called to enter the host assembly.
                    System.Reflection.MethodInfo entryPoint = entryAssembly.EntryPoint;

                    // If the base type of the host assembly inherits from the
                    // "ServiceBase" class, it must be a windows service. We store
                    // the result ready for the next caller of this method.
                    _isService = (entryPoint.ReflectedType.BaseType.FullName == "System.ServiceProcess.ServiceBase");
                }

                // Return the cached result.
                return System.Convert.ToBoolean(_isService);
            }
        }

        private static Nullable<bool> _isService = default(Boolean?);

        public static async Task<bool> SafeFileDeleteAsync(string Filename, string Source, long MaxWaitMS = 30000, bool Quiet = false)
        {
            //run the function in another thread
            return await Task.Run(() => SafeFileDelete(Filename, Source, MaxWaitMS, Quiet));
        }
        public static bool SafeFileDelete(string Filename, string Source, long MaxWaitMS = 30000, bool Quiet = false)
        {
            bool ret = false;

            if (!File.Exists(Filename))
                return true;

            WaitFileAccessResult result = Global.WaitForFileAccess(Filename, FileAccess.ReadWrite, FileShare.None, MaxWaitMS, AppSettings.Settings.loop_delay_ms, true, 0);
            if (result.Success)
            {
                try
                {
                    File.Delete(Filename);
                    ret = true;
                }
                catch (Exception ex)
                {
                    if (!Quiet)
                        Log($"Error: Could not delete file after {result.TimeMS} ms. Source='{Source}': {ex.Msg()}");
                }
            }
            else
            {
                if (!Quiet)
                    Log($"Error: Could not delete file after {result.TimeMS} ms. Source='{Source}': {Filename}");
            }

            return ret;

        }

        public static string FindSoundFile(string InputFile)
        {
            //only return the sound file if we found it
            string ret = "";
            try
            {
                //If the file passed contains a full path, just verify it exists
                if (InputFile.Contains("\\"))
                {
                    if (File.Exists(InputFile))
                    {
                        ret = InputFile;
                    }
                }
                else  //Assume it is just a filename and look for it...
                {
                    //add extension if not found
                    if (!Path.HasExtension(InputFile))
                        InputFile = InputFile.Trim() + ".wav";

                    //generate a list of known media paths
                    List<string> paths = new List<string>();
                    paths.Add(AppDomain.CurrentDomain.BaseDirectory);
                    paths.Add(Path.GetDirectoryName(AppSettings.Settings.SettingsFileName));

                    if (AITOOL.BlueIrisInfo.IsNotNull() && AITOOL.BlueIrisInfo.AppPath.IsNotNull() && Directory.Exists(AITOOL.BlueIrisInfo.AppPath))
                        paths.Add(Path.Combine(AITOOL.BlueIrisInfo.AppPath, "sounds"));

                    paths.Add(Environment.ExpandEnvironmentVariables("%SYSTEMROOT%\\Media"));

                    if (Directory.Exists(Environment.ExpandEnvironmentVariables("%PROGRAMFILES%\\Microsoft Office\\root\\Office16\\Media")))
                        paths.Add(Environment.ExpandEnvironmentVariables("%PROGRAMFILES%\\Microsoft Office\\root\\Office16\\Media"));

                    if (Directory.Exists(Environment.ExpandEnvironmentVariables("%PROGRAMFILES%\\Microsoft Office\\root\\Office15\\Media")))
                        paths.Add(Environment.ExpandEnvironmentVariables("%PROGRAMFILES%\\Microsoft Office\\root\\Office15\\Media"));

                    //search each path for the sound file
                    foreach (var fldr in paths)
                    {
                        string file = Path.Combine(fldr, InputFile);
                        if (File.Exists(file))
                        {
                            ret = file;
                            break;
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

        public static string GetTempFolder(bool Clear = false)
        {
            string ret = Path.GetTempPath();

            try
            {
                ret = Path.Combine(Path.GetTempPath(), "_AITOOL");

                if (Clear && Directory.Exists(ret))
                    try { Directory.Delete(ret, true); } catch { }

                if (!Directory.Exists(ret))
                    Directory.CreateDirectory(ret);
            }
            catch (Exception ex)
            {
                Log("Error: Could not get temp folder? " + ex.Msg());
            }

            return ret;
        }

        public static string SafeLoadTextFile(string Filename)
        {

            string ret = "";

            //dont wait very long for access, only grab text if the file is not being used (for stderr.txt, etc)
            WaitFileAccessResult result = WaitForFileAccess(Filename, FileAccess.Read, FileShare.Read, 100, AppSettings.Settings.loop_delay_ms, true, 4, MaxErrRetryCnt: 4);

            try
            {
                if (result.Success)
                {
                    // Open a FileStream object using the passed in safe file handle.
                    using FileStream fileStream = new FileStream(result.Handle, FileAccess.Read);
                    using StreamReader sr = new StreamReader(fileStream, Encoding.UTF8);
                    ret = sr.ReadToEnd();
                }
            }
            catch { }
            finally
            {
                if (result.Handle != null)
                {
                    if (!result.Handle.IsClosed)
                        result.Handle.Close();

                    result.Handle.Dispose();
                }
            }

            return ret;

        }

        /// <summary>
        /// Perform a deep Copy of the object, using Json as a serialisation method.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T CloneJson<T>(this T source)
        {
            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source, JSONSettingsPerformance));

        }

        public static async Task<bool> IsPortOpenAsync(string Host, int port)
        {
            Socket socket = null;

            try
            {
                // make a TCP based socket
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // connect
                await Task.Run(() => socket.Connect(Host, port));

                return true;
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode == SocketError.ConnectionRefused)
                {
                    return false;
                }

                //An error occurred when attempting to access the socket
                Debug.WriteLine(ex.ToString());
                Console.WriteLine(ex);
            }
            finally
            {
                if (socket?.Connected ?? false)
                {
                    socket?.Disconnect(false);
                }
                socket?.Close();
            }

            return false;
        }

        public static bool IsPortOpen(string Host, int port)
        {
            Socket socket = null;

            try
            {
                // make a TCP based socket
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // connect
                socket.Connect(Host, port);

                return true;
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode == SocketError.ConnectionRefused)
                {
                    return false;
                }

                //An error occurred when attempting to access the socket
                //Debug.WriteLine(ex.ToString());
                //Console.WriteLine(ex);
            }
            finally
            {
                if (socket?.Connected ?? false)
                {
                    socket?.Disconnect(false);
                }
                socket?.Close();
            }

            return false;
        }

        public static bool IsLocalPortInUse(int port)
        {

            try
            {
                // Evaluate current system tcp connections. This is the same information provided
                // by the netstat command line application, just in .Net strongly-typed object
                // form.  We will look through the list, and if our port we would like to use
                // in our TcpClient is occupied, we will set isAvailable to false.
                IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();

                TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

                foreach (TcpConnectionInformation tcpi in tcpConnInfoArray)
                {
                    if (tcpi.LocalEndPoint.Port == port)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"Error: {ex.Msg()}");
                return true;
            }

            return false;
        }



        public static string UpdateURL(string InURL, int DefaultPort, string DefaultPath, string Host, ref bool WasFixed, ref bool HadError)
        {
            string ret = InURL;
            UriBuilder uriBuilder = new UriBuilder();
            try
            {
                Uri url = new Uri(InURL);

                if (url != null)
                {

                    uriBuilder.Scheme = url.Scheme;
                    uriBuilder.Host = url.Host;
                    uriBuilder.Path = url.PathAndQuery;
                    uriBuilder.Port = url.Port;

                    if (url.HostNameType == UriHostNameType.IPv6 && url.Host.Contains("[") && !InURL.Contains("["))
                    {
                        //it adds [ ] around an ipv6 address
                        Log($"Debug: Placed square brackets around IPV6 address: '{url.Host}' for {InURL}");
                        WasFixed = true;
                    }


                    if (!string.IsNullOrEmpty(Host) && !Host.Equals(url.Host, StringComparison.OrdinalIgnoreCase))
                    {
                        uriBuilder.Host = Host;
                        Log($"Debug: Changed host from '{url.Host}' to '{Host}' for {InURL}");
                        WasFixed = true;
                    }

                    if (DefaultPort > 0 && url.Port != DefaultPort)
                    {
                        uriBuilder.Port = DefaultPort;
                        Log($"Debug: Changed port from '{url.Port}' to '{DefaultPort}' for {InURL}");
                        WasFixed = true;
                    }

                    //scheme=http or https
                    if (DefaultPort == 443 && url.Scheme != "https")
                    {
                        Log($"Debug: Changed scheme from '{url.Scheme}' to 'https' for {InURL}");
                        uriBuilder.Scheme = "https";
                        WasFixed = true;
                    }
                    else if (DefaultPort == 80 && url.Scheme == "https")
                    {
                        Log($"Debug: Changed scheme from '{url.Scheme}' to 'http' for {InURL}");
                        uriBuilder.Scheme = "http";
                        WasFixed = true;
                    }

                    //if (!InURL.Contains($":{uriBuilder.Port}"))
                    //{
                    //    //this doesnt work because URI.ToString does not include the port if it is the default port for the scheme
                    //    Log($"Debug: Port added to URL: '{uriBuilder.Port}' for {InURL}");
                    //    WasFixed = true;
                    //}

                    if (!string.IsNullOrEmpty(DefaultPath) && string.IsNullOrEmpty(url.PathAndQuery) || url.PathAndQuery == "/" || url.PathAndQuery == "\\" || !url.PathAndQuery.StartsWith(DefaultPath, StringComparison.OrdinalIgnoreCase))
                    {
                        Log($"Debug: Added correct path of '{DefaultPath}' to {InURL}");
                        uriBuilder.Path = DefaultPath;
                        WasFixed = true;
                    }


                    if (!WasFixed)
                    {
                        string newurl = uriBuilder.Uri.GetComponents(UriComponents.AbsoluteUri, UriFormat.Unescaped);
                        if (!newurl.Equals(ret, StringComparison.OrdinalIgnoreCase))
                        {
                            Log($"Debug: Updated URL '{InURL}' to '{newurl}'");
                            WasFixed = true;
                        }

                    }

                }
                else
                {
                    HadError = true;
                    Log($"Error: Bad url '{InURL}'");
                }

            }
            catch (Exception ex)
            {
                HadError = true;
                Log($"Error: {InURL}: {ex.Message}");
            }

            if (WasFixed)
            {
                //ret = uriBuilder.Uri.ToString();
                ret = uriBuilder.Uri.GetComponents(UriComponents.AbsoluteUri, UriFormat.SafeUnescaped);
            }

            return ret;
        }

        public static string GetURLPath(string InURL)
        {
            string ret = "";
            try
            {
                Uri url = new Uri(InURL);
                if (url != null && !string.IsNullOrEmpty(url.PathAndQuery) && url.PathAndQuery != "\\" && url.PathAndQuery != "/")
                    ret = url.PathAndQuery;
            }
            catch (Exception ex)
            {
                Log($"Error: {InURL}: {ex.Message}");
            }
            return ret;
        }


        public static bool IsValidURL(string InURL)
        {
            bool ret = false;
            try
            {
                Uri url = new Uri(InURL);
                if (url != null &&
                    !string.IsNullOrEmpty(url.PathAndQuery) &&
                    url.PathAndQuery != "/" &&
                    !url.PathAndQuery.Contains("|") &&
                    !url.PathAndQuery.Contains(";") &&
                    url.Port != 0)
                    ret = true;
            }
            catch (Exception ex)
            {
                //Log($"Error: {InURL}: {ex.Message}");
            }
            return ret;
        }
        [DllImport("Shlwapi.dll", CharSet = CharSet.Auto)]
        public static extern long StrFormatByteSize(long fileSize, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder buffer, int bufferSize);

        public static string FormatBytes(long filesize)
        {
            StringBuilder sb = new StringBuilder();
            StrFormatByteSize(filesize, sb, sb.Capacity);
            return sb.ToString();
        }
        public static bool IsLatLongValid(string lat, string lng)
        {
            return IsLatLongValid(lat.ToDouble(), lng.ToDouble());
        }
        public static bool IsLatLongValid(double lat, double lng)
        {
            bool ret = false;
            if (lat == 0 || lat == 39.809734 || lat < -90 || lat > 90 || lng == 0 || lng == -98.555620 || lng < -180 || lng > 180)
                ret = false;
            else
                ret = true;
            return ret;
        }
        private static DateTime lastlatwarn = DateTime.Now;
        [DebuggerStepThrough]
        public static bool IsTimeBetween(DateTime time, string span)
        {
            if (span.IsEmpty())
                return true;  //if span is not set, assume its always true

            bool ret = false;

            try
            {
                // convert datetime to a TimeSpan
                TimeSpan now = time.TimeOfDay;

                //first split up multiple ranges:
                List<string> spans = span.SplitStr(",");
                foreach (var spn in spans)
                {

                    //support simple format hour1;hour2;hour3  12;1;2;3;4;5;6
                    if (spn.Contains(";"))
                    {
                        List<string> semsplt = spn.SplitStr(";");
                        foreach (string hr in semsplt)
                        {
                            //The value of the Hour property is always expressed using a 24-hour clock.
                            if (hr == time.Hour.ToString())
                            {
                                ret = true;
                                break;
                            }
                        }

                        if (ret)
                            break;
                    }
                    else
                    {
                        List<string> splt = spn.SplitStr("-", RemoveEmpty: false);

                        TimeSpan BeginSpan = now;
                        TimeSpan EndSpan = now;

                        //sunset-sunrise
                        //sunrise-sunset
                        //sunset_5-sunrise+5

                        if (splt[0].EqualsIgnoreCase("sunset") || splt[0].EqualsIgnoreCase("sunrise") || splt[0].Has("dusk") || splt[0].Has("dawn"))
                        {
                            if (!IsLatLongValid(AppSettings.Settings.LocalLatitude, AppSettings.Settings.LocalLongitude))
                            {
                                if ((DateTime.Now - lastlatwarn).TotalMinutes >= 30)
                                {
                                    Log($"Warn: The 'LocalLatitude' and 'LocalLongitude' settings in AITOOL.Settings.JSON file is incorrect and needs to be set to your local area.  Latitude is currently '{AppSettings.Settings.LocalLatitude}' and Longitude is '{AppSettings.Settings.LocalLongitude}'.  If you set those settings in a locally running copy of BlueIris > Settings > Schedule tab, they will be AUTOMATICALLY used.");
                                    lastlatwarn = DateTime.Now;
                                }
                            }
                            else
                            {
                                TimeZoneInfo localZone = TimeZoneInfo.Local;
                                SolarTimes solarTimes = new SolarTimes(time, AppSettings.Settings.LocalLatitude, AppSettings.Settings.LocalLongitude);

                                if (splt[0].Has("dusk"))
                                {
                                    BeginSpan = TimeZoneInfo.ConvertTimeFromUtc(solarTimes.DuskCivil.ToUniversalTime(), localZone).TimeOfDay;
                                }
                                else if (splt[0].EqualsIgnoreCase("sunset"))
                                {
                                    BeginSpan = TimeZoneInfo.ConvertTimeFromUtc(solarTimes.Sunset.ToUniversalTime(), localZone).TimeOfDay;
                                }
                                else if (splt[0].EqualsIgnoreCase("sunrise"))
                                {
                                    BeginSpan = TimeZoneInfo.ConvertTimeFromUtc(solarTimes.Sunrise.ToUniversalTime(), localZone).TimeOfDay;
                                }
                                else if (splt[0].Has("dawn"))
                                {
                                    BeginSpan = TimeZoneInfo.ConvertTimeFromUtc(solarTimes.DawnCivil.ToUniversalTime(), localZone).TimeOfDay;
                                }

                                if (splt[1].Has("dusk"))
                                {
                                    EndSpan = TimeZoneInfo.ConvertTimeFromUtc(solarTimes.DuskCivil.ToUniversalTime(), localZone).TimeOfDay;
                                }
                                else if (splt[1].EqualsIgnoreCase("sunset"))
                                {
                                    EndSpan = TimeZoneInfo.ConvertTimeFromUtc(solarTimes.Sunset.ToUniversalTime(), localZone).TimeOfDay;
                                }
                                else if (splt[1].EqualsIgnoreCase("sunrise"))
                                {
                                    EndSpan = TimeZoneInfo.ConvertTimeFromUtc(solarTimes.Sunrise.ToUniversalTime(), localZone).TimeOfDay;
                                }
                                else if (splt[1].Has("dawn"))
                                {
                                    EndSpan = TimeZoneInfo.ConvertTimeFromUtc(solarTimes.DawnCivil.ToUniversalTime(), localZone).TimeOfDay;
                                }

                            }

                        }
                        else
                        {
                            BeginSpan = TimeSpan.Parse(splt[0]);
                            EndSpan = TimeSpan.Parse(splt[1]);

                        }

                        // see if start comes before end
                        if (BeginSpan < EndSpan)
                        {
                            ret = BeginSpan <= now && now <= EndSpan;
                            //if (ret)
                            //    Console.WriteLine($"Time ({now.TotalHours.Round()}) IS       BETWEEN [{span}] BeginSpan ({BeginSpan.TotalHours.Round()}) is LESS THAN EndSpan ({EndSpan.TotalHours.Round()})");
                            //else
                            //    Console.WriteLine($"Time ({now.TotalHours.Round()}) IS *NOT* BETWEEN [{span}] BeginSpan ({BeginSpan.TotalHours.Round()}) is LESS THAN EndSpan ({EndSpan.TotalHours.Round()})");

                        }
                        else
                        {
                            // start is after end, so do the inverse comparison
                            ret = !(EndSpan < now && now < BeginSpan);
                            //if (ret)
                            //    Console.WriteLine($"Time ({now.TotalHours.Round()}) IS       BETWEEN [{span}] BeginSpan ({BeginSpan.TotalHours.Round()}) is GREATER THAN EndSpan ({EndSpan.TotalHours.Round()})");
                            //else
                            //    Console.WriteLine($"Time ({now.TotalHours.Round()}) IS *NOT* BETWEEN [{span}] BeginSpan ({BeginSpan.TotalHours.Round()}) is GREATER THAN EndSpan ({EndSpan.TotalHours.Round()})");

                        }

                        if (ret)
                            break;
                    }
                }

            }
            catch (Exception ex)
            {

                Log($"Error: Range Invalid: '{span}'. Lat='{AppSettings.Settings.LocalLatitude}', Long='{AppSettings.Settings.LocalLongitude}': " + ex.Msg());
            }

            return ret;
        }

        public static async Task<bool> DirectoryExistsAsync(string directory, int TimeoutMS = 20000)
        {
            //run the function in another thread
            CancellationTokenSource cts = new CancellationTokenSource(TimeoutMS);
            try
            {
                Stopwatch sw = Stopwatch.StartNew();
                bool result = await Task.Run(() => Directory.Exists(directory), cts.Token);
                Log($"Trace: Directory exists for '{directory}' took {sw.ElapsedMilliseconds}ms");
                return result;
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        public static string MappedDriveToUNCPath(string path)
        {
            if (!path.StartsWith(@"\\"))
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey("Network\\" + path[0]))
                {
                    if (key != null)
                    {
                        return key.GetValue("RemotePath").ToString() + path.Remove(0, 2).ToString();
                    }
                }
            }
            return path;
        }

        public class MappedDrive
        {
            public string DriveLetter = "";
            public string Path = "";
        }

        public static async Task<string> GetBestRemotePathAsync(string RemoteLocalPath, string RemoteMachineNameOrIP)
        {
            //We are taking a path like C:\BlueIris\clips\blah read from a remote computer's registry
            //and trying to convert it to an accessible path on the current computer.
            string ret = RemoteLocalPath;
            string lastremotepathpart = RemoteLocalPath.SplitStr(@"\").Last();
            string ip = "";
            string hostname = "";
            Stopwatch sw = Stopwatch.StartNew();

            if (!RemoteLocalPath.StartsWith(@"\\"))
            {
                //Make sure we have the IP address
                IPAddress ipa = await GetIPAddressFromHostnameAsync(RemoteMachineNameOrIP);
                ip = Global.IP2Str(ipa, IPType.Path);
                hostname = await GetHostNameAsync(RemoteMachineNameOrIP);

                //first look for a mapped drive letter:
                string letter = RemoteLocalPath.Substring(0, 1);
                List<MappedDrive> mapped = new List<MappedDrive>();
                using RegistryKey key = Registry.CurrentUser.OpenSubKey("Network");
                if (key != null)
                {
                    List<string> drives = key.GetSubKeyNames().ToList();
                    foreach (string drv in drives)
                    {
                        using RegistryKey drvkey = key.OpenSubKey(drv);
                        if (drvkey != null)
                        {
                            string remotepath = drvkey.GetValue("RemotePath", "").ToString();
                            if (!string.IsNullOrEmpty(remotepath) && remotepath.StartsWith(@"\\"))
                            {
                                string mappedserver = remotepath.GetWord(@"\\", @"\");

                                if (mappedserver.EqualsIgnoreCase(ip) ||
                                    mappedserver.EqualsIgnoreCase(hostname) ||
                                    mappedserver.EqualsIgnoreCase(hostname.GetWord("", ".")))  //lop off the SERVER.DOMAIN
                                {
                                    MappedDrive md = new MappedDrive();
                                    md.DriveLetter = drv;
                                    md.Path = remotepath;
                                    mapped.Add(md);
                                }
                            }
                        }
                    }
                }

                //first pass, try to get any matches where some of the UNC path matches...
                //there would have to be a shared part of the path
                //            C:\clips\myclippath
                //\\server\share\clips\myclippath
                foreach (MappedDrive md in mapped)
                {
                    string sharedpath = GetSharedPath(md.Path, RemoteLocalPath, false).TrimEnd(@"\".ToCharArray());
                    if (!string.IsNullOrEmpty(sharedpath))
                    {
                        Log($"Debug: Found shared path in {sw.ElapsedMilliseconds}ms on '{RemoteMachineNameOrIP}' for path '{RemoteLocalPath}': {sharedpath}");
                        return sharedpath;
                    }
                }

                List<string> spltpth = RemoteLocalPath.SplitStr("\\");

                //search for last two parts of the path UNDER each of the shares
                string lastpath = spltpth[spltpth.Count - 1];
                string nexttolast = "";

                if (spltpth.Count - 2 > 0)
                {
                    nexttolast = spltpth[spltpth.Count - 2];
                    string searchpath = $"{nexttolast}\\{lastpath}";
                    foreach (MappedDrive md in mapped)
                    {
                        string checkpath = Path.Combine(md.Path, searchpath);
                        if (await Global.DirectoryExistsAsync(checkpath))
                        {
                            Log($"Debug: Found remote path in {sw.ElapsedMilliseconds}ms on '{RemoteMachineNameOrIP}' for path '{RemoteLocalPath}': {checkpath}");
                            return checkpath;
                        }
                    }
                }


                //            C:\clips\myclippath
                //\\server\share\clips\myclippath

                //C:\BlueIrisStorage\Alerts
                //\\server\BlueIrisStorage

                foreach (MappedDrive md in mapped)
                {
                    string checkpath = Path.Combine(md.Path, lastpath);
                    if (await Global.DirectoryExistsAsync(checkpath))
                    {
                        Log($"Debug: Found remote path in {sw.ElapsedMilliseconds}ms on '{RemoteMachineNameOrIP}' for path '{RemoteLocalPath}': {checkpath}");
                        return checkpath;
                    }
                }


                ret = $"\\\\{Global.IP2Str(RemoteMachineNameOrIP, IPType.Path)}\\{RemoteLocalPath.Replace(":", "$")}";
                //resort to using admin shares (have to be enabled through group policy in newer versions of windows)
                Log($"Debug: Found ADMIN share in {sw.ElapsedMilliseconds}ms '{RemoteMachineNameOrIP}' for path '{RemoteLocalPath}': {ret}");

            }
            return ret;
        }

        public static string GetSharedPath(string BasePath, string SrcPath, bool OnlyPartial)
        {
            // Dim NetRootPth As String = "\\server\admlibrary\Software\AutoDesk\Test_CAD_State_Kit"
            // Dim SrcPth As String     = "                              C:\Test\Test_CAD_State_Kit\C3D 2021\Utilities"
            // Output=                     \\server\admlibrary\Software\AutoDesk\Test_CAD_State_Kit\C3D 2021\Utilities
            string Ret = "";
            try
            {
                // Dim com As String = FindCommonPath(pths)
                string[] nps = BasePath.Trim().Split('\\');
                string SharedPth = "";
                bool Fnd = false;
                foreach (string pp in nps)
                {
                    if (!OnlyPartial)
                    {
                        if (pp == "")
                            SharedPth = SharedPth + @"\";
                        else
                            SharedPth = SharedPth + pp + @"\";
                    }

                    string[] rps = SrcPath.Trim().Split('\\');
                    foreach (string rp in rps)
                    {
                        if (Fnd)
                        {
                            if (rp == "")
                                SharedPth = SharedPth + @"\";
                            else
                                SharedPth = SharedPth + rp + @"\";
                        }
                        else if (pp != "" && string.Equals(pp, rp, StringComparison.OrdinalIgnoreCase))
                        {
                            Fnd = true;
                            if (OnlyPartial)
                                SharedPth = SharedPth + rp + @"\";
                        }
                    }
                    if (Fnd)
                        break;
                }
                if (Fnd)
                    Ret = SharedPth;
            }
            catch (Exception ex)
            {
                Ret = "";
                Log("Error: " + ex.Message);
            }
            return Ret;
        }

        public static bool IsRegexPatternValid(string pattern)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(pattern) && pattern.Length > 2)
                {
                    System.Text.RegularExpressions.Regex test = new System.Text.RegularExpressions.Regex(pattern);
                    return true;
                }
            }
            catch { }
            return false;
        }

        public static bool OnlyHexInString(string test)
        {
            if (test.IsEmpty())
                return false;
            // For C-style hex notation (0xFF) you can use @"\A\b(0[xX])?[0-9a-fA-F]+\b\Z"
            return System.Text.RegularExpressions.Regex.IsMatch(test, @"\A\b[0-9a-fA-F]+\b\Z");
        }

        public static Color ConvertStringToColor(string InString, string InAlphaString = "")
        {
            Color Ret = Color.FromKnownColor(KnownColor.White);
            try
            {
                if (InString.IsNotEmpty())
                {
                    int Alpha = 255;
                    int Red = Ret.R;
                    int Green = Ret.G;
                    int Blue = Ret.B;
                    string[] splt;
                    if (InString.Contains(","))
                    {
                        splt = InString.Trim().Split(',');
                        if (splt.Count() == 3)
                        {
                            Red = splt[0].ToInt();
                            Green = splt[1].ToInt();
                            Blue = splt[2].ToInt();
                            Ret = Color.FromArgb(Red, Green, Blue);
                        }
                        else if (splt.Count() == 4)
                        {
                            Alpha = splt[0].ToInt();
                            Red = splt[1].ToInt();
                            Green = splt[2].ToInt();
                            Blue = splt[3].ToInt();
                            Ret = Color.FromArgb(Alpha, Red, Green, Blue);
                        }
                        else
                            Log("Error: Problem converting color, not 3 RGB numbers or 4 ARGB: '" + InString + ",");

                    }
                    else
                    {
                        if (InString.StartsWith("#") || OnlyHexInString(InString.Trim()))
                        {
                            int argb = Int32.Parse(InString.Replace("#", "").Trim(), NumberStyles.HexNumber);
                            Ret = Color.FromArgb(argb);
                        }
                        else
                        {
                            Ret = Color.FromName(InString.Trim());
                        }
                    }

                    if (InAlphaString.IsNotEmpty())
                    {
                        Ret = Color.FromArgb(InAlphaString.ToInt(), Ret);
                    }

                }
            }
            catch (Exception ex)
            {
                Log($"Error: InString='{InString}': {ex.Message}");
            }
            return Ret;
        }

        private static string CachedMacAddress = "";
        /// <summary>
        /// Finds the MAC address of the NIC with maximum speed.
        /// </summary>
        /// <returns>The MAC address.</returns>
        public static string GetMacAddress()
        {

            if (CachedMacAddress.IsNotNull())
                return CachedMacAddress;

            const int MIN_MAC_ADDR_LENGTH = 12;
            string macAddress = string.Empty;
            long maxSpeed = -1;
            Stopwatch sw = Stopwatch.StartNew();

            try
            {
                foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                    {
                        string tempMac = nic.GetPhysicalAddress().ToString();

                        if (nic.Speed > maxSpeed && !string.IsNullOrEmpty(tempMac) && tempMac.Length >= MIN_MAC_ADDR_LENGTH)
                        {
                            Log("Trace: New Max Speed = " + nic.Speed + ", MAC: " + tempMac);
                            maxSpeed = nic.Speed;
                            macAddress = tempMac;
                        }

                    }

                }

            }
            catch (Exception ex)
            {
                Log("Error: " + ex.Msg());
            }

            if (sw.ElapsedMilliseconds > 500)  //should it really take this long??
                Log($"Warn: It tool {sw.ElapsedMilliseconds}ms to get the MAC address?");

            CachedMacAddress = macAddress;

            return macAddress;
        }
        public static void ResponsiveSleep(int SleepMS)
        {
            Stopwatch sw = Stopwatch.StartNew();
            do
            {
                Thread.Sleep(50);
                //I've seen threading errors happen with doevents calls, so wrap in try/catch...
                try
                { Application.DoEvents(); }  //Cover your eyes, nothing to see here.  DoEvents isnt that bad.  Really.  Ok, bye.
                catch { }

            } while (sw.ElapsedMilliseconds <= SleepMS);
        }

        public static bool DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs, bool nolog)
        {

            bool ret = true;
            try
            {
                // Get the subdirectories for the specified directory.
                DirectoryInfo dir = new DirectoryInfo(sourceDirName);

                if (!dir.Exists)
                {
                    ret = false;
                    if (!nolog) Log($"Error: Source folder does not exist? {sourceDirName}");
                    return ret;
                }

                DirectoryInfo[] dirs = dir.GetDirectories();

                // If the destination directory doesn't exist, create it.       
                Directory.CreateDirectory(destDirName);

                // Get the files in the directory and copy them to the new location.
                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo file in files)
                {
                    string tempPath = Path.Combine(destDirName, file.Name);
                    try
                    {
                        file.CopyTo(tempPath, false);
                    }
                    catch (Exception ex)
                    {
                        ret = false;
                        if (!nolog) Log($"Error: {tempPath} - {ex.Message}");
                    }
                }

                // If copying subdirectories, copy them and their contents to new location.
                if (copySubDirs)
                {
                    foreach (DirectoryInfo subdir in dirs)
                    {
                        string tempPath = Path.Combine(destDirName, subdir.Name);
                        if (!DirectoryCopy(subdir.FullName, tempPath, copySubDirs, nolog))
                            ret = false;
                    }
                }

            }
            catch (Exception ex)
            {
                ret = false;
                if (!nolog) Log($"Error: {ex.Message}");
            }

            return ret;
        }

        private static Dictionary<string, string> HostNameCache = new Dictionary<string, string>();
        public static async Task<string> GetHostNameAsync(string IPAddressOrHostName)
        {

            if (HostNameCache.ContainsKey(IPAddressOrHostName.ToLower()))
                return HostNameCache[IPAddressOrHostName.ToLower()];

            Stopwatch sw = Stopwatch.StartNew();
            string ret = IPAddressOrHostName;

            try
            {
                if (!IsValidIPAddress(IPAddressOrHostName, out IPAddress FoundIP))
                    return IPAddressOrHostName;  //assume valid hostname if it doesnt look like an ip address

                //why is this taking close to 5 seconds for an internal network dns call??

                IPHostEntry entry = await Dns.GetHostEntryAsync(IPAddressOrHostName);
                if (entry != null)
                {
                    ret = entry.HostName;
                }
            }
            catch (Exception ex)
            {
                Log("Error: " + ex.Message);
            }
            finally
            {
                Log($"Trace: Resolved Host name '{IPAddressOrHostName}' to '{ret}' in {sw.ElapsedMilliseconds}ms");
            }

            HostNameCache.Add(IPAddressOrHostName.ToLower(), ret);

            return ret;

        }

        private static string CurrentIP = "";
        private static string CurrentHost = "";
        public static bool IsLocalHost(string HostNameOrIPAddress)
        {
            try
            {
                bool ret = false;

                if (CurrentIP.IsEmpty())
                    CurrentIP = GetAllLocalIPs(NetworkInterfaceType.Ethernet)[0].ToString();

                if (CurrentHost.IsEmpty())
                    CurrentHost = Dns.GetHostName();

                if (string.IsNullOrEmpty(HostNameOrIPAddress) ||
                    HostNameOrIPAddress == "." ||
                    HostNameOrIPAddress.EqualsIgnoreCase("localhost") ||
                    HostNameOrIPAddress == "127.0.0.1" ||
                    HostNameOrIPAddress == "0.0.0.0" ||
                    HostNameOrIPAddress == "::1:" ||
                    HostNameOrIPAddress == "[::1:]" ||
                    HostNameOrIPAddress == "0:0:0:0:0:0:0:1" ||
                    HostNameOrIPAddress == "[0:0:0:0:0:0:0:1]" ||
                    HostNameOrIPAddress.EqualsIgnoreCase(CurrentIP) ||
                    HostNameOrIPAddress.EqualsIgnoreCase(CurrentHost))
                {
                    ret = true;
                }
                else
                {
                    IPAddress ip = GetIPAddressFromHostname(HostNameOrIPAddress);
                    if (IPAddress.IsLoopback(ip))
                    {
                        ret = true;
                    }
                    else if (HostNameOrIPAddress.EqualsIgnoreCase(CurrentIP) ||
                             HostNameOrIPAddress.EqualsIgnoreCase(CurrentHost))
                    {
                        ret = true;
                    }
                }

                ////if (!ret)
                ////    Log($"Host or IP not detected as 'localhost': '{HostNameOrIPAddress}' ThisHost='{CurrentHost}', ThisIP='{CurrentIP}'");

                return ret;

            }
            catch (Exception ex)
            {
                Log($"Error: {ex.Msg()}");
                return false;
            }


        }


        public static async Task<bool> IsLocalHostAsync(string HostNameOrIPAddress)
        {
            try
            {
                bool ret = false;

                if (CurrentIP.IsEmpty())
                {
                    List<IPAddress> ips = GetAllLocalIPs(NetworkInterfaceType.Ethernet);
                    //was getting index out of bounds on one machine that has never had AITOOL installed
                    if (ips.Count > 0)
                        CurrentIP = ips[0].ToString();
                    else
                    {
                        CurrentIP = "127.0.0.1";  //this should not happen - antivirus/firewall blocking?
                    }
                }


                if (CurrentHost.IsEmpty())
                    CurrentHost = Dns.GetHostName();

                if (string.IsNullOrEmpty(HostNameOrIPAddress) ||
                    HostNameOrIPAddress == "." ||
                    HostNameOrIPAddress.EqualsIgnoreCase("localhost") ||
                    HostNameOrIPAddress == "127.0.0.1" ||
                    HostNameOrIPAddress == "0.0.0.0" ||
                    HostNameOrIPAddress == "::1:" ||
                    HostNameOrIPAddress == "[::1:]" ||
                    HostNameOrIPAddress == "0:0:0:0:0:0:0:1" ||
                    HostNameOrIPAddress == "[0:0:0:0:0:0:0:1]" ||
                    HostNameOrIPAddress.EqualsIgnoreCase(CurrentIP) ||
                    HostNameOrIPAddress.EqualsIgnoreCase(CurrentHost))
                {
                    ret = true;
                }
                else
                {
                    IPAddress ip = await GetIPAddressFromHostnameAsync(HostNameOrIPAddress);
                    if (IPAddress.IsLoopback(ip))
                    {
                        ret = true;
                    }
                    else if (HostNameOrIPAddress.EqualsIgnoreCase(CurrentIP) ||
                             HostNameOrIPAddress.EqualsIgnoreCase(CurrentHost))
                    {
                        ret = true;
                    }
                }

                ////if (!ret)
                ////    Log($"Host or IP not detected as 'localhost': '{HostNameOrIPAddress}' ThisHost='{CurrentHost}', ThisIP='{CurrentIP}'");

                return ret;

            }
            catch (Exception ex)
            {
                Log($"Error: {ex.Msg()}");
                return false;
            }


        }


        public static bool IsLocalNetwork(string HostNameOrIPAddress)
        {

            if (IsLocalHost(HostNameOrIPAddress))
                return true;

            IPAddress ip = GetIPAddressFromHostname(HostNameOrIPAddress);

            if (ip != IPAddress.None)
            {
                if (IPAddress.IsLoopback(ip))
                    return true;

                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    byte[] bytes = ip.GetAddressBytes();
                    switch (bytes[0])
                    {
                        case 10:
                            return true;
                        case 172:
                            return bytes[1] < 32 && bytes[1] >= 16;
                        case 192:
                            return bytes[1] == 168;
                        default:
                            return false;
                    }
                }
                else if (ip.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    var addressAsString = ip.ToString();
                    var firstWord = addressAsString.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[0];

                    // Make sure we are dealing with an IPv6 address
                    if (ip.AddressFamily != AddressFamily.InterNetworkV6) return false;

                    // The original IPv6 Site Local addresses (fec0::/10) are deprecated. Unfortunately IsIPv6SiteLocal only checks for the original deprecated version:
                    else if (ip.IsIPv6SiteLocal) return true;

                    // These days Unique Local Addresses (ULA) are used in place of Site Local. 
                    // ULA has two variants: 
                    //      fc00::/8 is not defined yet, but might be used in the future for internal-use addresses that are registered in a central place (ULA Central). 
                    //      fd00::/8 is in use and does not have to registered anywhere.
                    else if (firstWord.Substring(0, 2) == "fc" && firstWord.Length >= 4) return true;
                    else if (firstWord.Substring(0, 2) == "fd" && firstWord.Length >= 4) return true;

                    // Link local addresses (prefixed with fe80) are not routable
                    else if (firstWord == "fe80") return true;

                    // Discard Prefix
                    else if (firstWord == "100") return true;

                    // Any other IP address is not Unique Local Address (ULA)
                    else return false;
                }
            }

            return false;


        }

        public enum IPType
        {
            Path,
            URL
        }

        public static string IP2Str(IPAddress ip, IPType type)
        {
            if (ip == IPAddress.None)
                return ip.ToString(); //??

            if (ip.AddressFamily == AddressFamily.InterNetwork)
                return ip.ToString();

            if (ip.AddressFamily == AddressFamily.InterNetworkV6)
            {

                if (type == IPType.Path)
                {
                    return $"{ip.ToString().Replace(":::", "::").Replace(":", "-").Replace("%", "s")}.ipv6-literal.net";  //https://devblogs.microsoft.com/oldnewthing/20100915-00/?p=12863

                }
                else if (type == IPType.URL)
                {
                    return $"[{ip.ToString().Replace(":::", "::")}]";  //add square brackets to make the URL valid.  Fix where BlueIris returns 3 colons vs 2 correct.
                }
            }

            return ip.ToString();

        }

        public static string IP2Str(string HostNameOrIPAddress, IPType type)
        {
            IPAddress ip = GetIPAddressFromIPString(HostNameOrIPAddress.Replace(":::", "::"));
            if (ip == IPAddress.None)
                return HostNameOrIPAddress;

            return IP2Str(ip, type);


        }

        public static string CleanIPV6Address(string IP)
        {
            //2600-6c64-6b7f-f8d8--1d4.ipv6-literal.net
            //2600:6c64:6b7f:f8d8::1d4
            //[2600:6c64:6b7f:f8d8::1d4]
            //[2600:6c64:6b7f:f8d8:::1d4] - bad, 3 colons
            IP = IP.Trim("[] ".ToCharArray());
            if (IP.IndexOf(".ipv6-literal.net", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                IP = IP.Replace(".ipv6-literal.net", "").Replace(":::", "::").Replace("-", ":").Replace("s", "%").Trim();
            }
            return IP;
        }

        static Dictionary<string, string> DNSErrCache = new Dictionary<string, string>();
        public static IPAddress GetIPAddressFromHostname(string HostNameOrIPAddress = "")
        {
            IPAddress ret = IPAddress.None;
            try
            {
                //stop any errors from happening more than once since it may take a long time to resolve an invalid host:
                if (DNSErrCache.ContainsKey(HostNameOrIPAddress.ToLower()))
                    return ret;

                if (IsValidIPAddress(HostNameOrIPAddress, out IPAddress FoundIP))
                    return FoundIP;

                IPHostEntry Host;
                if (string.IsNullOrWhiteSpace(HostNameOrIPAddress))
                    Host = Dns.GetHostEntry(Dns.GetHostName());
                else
                    Host = Dns.GetHostEntry(HostNameOrIPAddress);

                foreach (IPAddress IP in Host.AddressList)
                {
                    if (IP.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        // just return the first one ipv4 address
                        ret = IP;
                        break;
                    }
                }
                if (!IsValidIPAddress(ret, out FoundIP))
                {
                    // fall back to ipv6
                    foreach (IPAddress IP in Host.AddressList)
                    {
                        if (IP.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                        {
                            // just return the first ipv6 address
                            ret = IP;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ret = IPAddress.None;
                if (!DNSErrCache.ContainsKey(HostNameOrIPAddress.ToLower()))
                    DNSErrCache.Add(HostNameOrIPAddress.ToLower(), ex.Message);

                Log($"Error: Hostname '{HostNameOrIPAddress}': {ex.Msg()}");
            }

            return ret;
        }

        public static List<IPAddress> GetAllLocalIPs(NetworkInterfaceType _type)
        {
            List<IPAddress> ipAddrList = new List<IPAddress>();
            try
            {
                foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (item.NetworkInterfaceType == _type && item.OperationalStatus == OperationalStatus.Up)
                    {
                        foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                ipAddrList.Add(ip.Address);
                            }
                        }
                    }
                }
                //fall back to IPV6 if needed
                if (ipAddrList.Count == 0)
                {
                    foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
                    {
                        if (item.NetworkInterfaceType == _type && item.OperationalStatus == OperationalStatus.Up)
                        {
                            foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                            {
                                if (ip.Address.AddressFamily == AddressFamily.InterNetworkV6)
                                {
                                    ipAddrList.Add(ip.Address);
                                }
                            }
                        }
                    }

                }

                if (ipAddrList.Count == 0)
                    Log($"Error: No IP addresses found for NetworkInterfaceType '{_type}' with Operational status = UP????");

            }
            catch (Exception ex)
            {

                Log("Error: " + ex.Msg());
            }

            return ipAddrList;
        }

        public static async Task<IPAddress> GetIPAddressFromHostnameAsync(string HostNameOrIPAddress = "")
        {
            IPAddress ret = IPAddress.None;
            Stopwatch sw = Stopwatch.StartNew();

            try
            {
                //stop any errors from happening more than once:
                if (DNSErrCache.ContainsKey(HostNameOrIPAddress.ToLower()))
                    return ret;

                if (IsValidIPAddress(HostNameOrIPAddress, out IPAddress FoundIP))
                    return FoundIP;

                IPHostEntry Host;
                if (string.IsNullOrWhiteSpace(HostNameOrIPAddress))
                    Host = await Dns.GetHostEntryAsync(Dns.GetHostName());
                else
                    Host = await Dns.GetHostEntryAsync(HostNameOrIPAddress);

                //prefer ipv4 address
                foreach (IPAddress IP in Host.AddressList)
                {
                    if (IP.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        // just return the first ipv4 found
                        ret = IP;
                        break;
                    }
                }
                if (!IsValidIPAddress(ret, out FoundIP))
                {
                    // fall back to ipv6
                    foreach (IPAddress IP in Host.AddressList)
                    {
                        if (IP.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                        {
                            // just return the first ipv6
                            ret = IP;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ret = IPAddress.None;
                if (!DNSErrCache.ContainsKey(HostNameOrIPAddress.ToLower()))
                    DNSErrCache.Add(HostNameOrIPAddress.ToLower(), ex.Message);

                Log($"Error: Hostname '{HostNameOrIPAddress}': {ex.Msg()}");
            }
            finally
            {
                if (ret != IPAddress.None)
                    Log($"Trace: Resolved host '{HostNameOrIPAddress}' to IP {ret.ToString()} in {sw.ElapsedMilliseconds}ms");
            }

            return ret;
        }
        public static IPAddress GetIPAddressFromIPString(string IP)
        {
            IPAddress ret = IPAddress.None;
            if (IsValidIPAddress(IP, out IPAddress FoundIP))
                return FoundIP;

            return ret;
        }

        public static bool IsValidIPAddress(string IP, out IPAddress FoundIP)
        {
            FoundIP = IPAddress.None;
            if (!string.IsNullOrEmpty(IP))
            {
                IP = CleanIPV6Address(IP);
                if (IPAddress.TryParse(IP, out FoundIP) && !FoundIP.Equals(IPAddress.None) && (FoundIP.AddressFamily == AddressFamily.InterNetwork || FoundIP.AddressFamily == AddressFamily.InterNetworkV6))
                    return true;
            }
            return false;
        }
        public static bool IsValidIPAddress(IPAddress IP, out IPAddress FoundIP)
        {
            FoundIP = IPAddress.None;
            if (IP != null && !IP.Equals(IPAddress.None))
            {
                if (IsValidIPAddress(IP.ToString(), out FoundIP))
                    return true;
            }
            return false;
        }

        public class ClsPingOut
        {
            public bool Success = false;
            public PingReply PingReply = null;
            public long DNSResolveMS = 0;
            public string PingError = "";
            public int Hops = 0;
            public int Retries = 0;
            public long AvgTimeMS = 0;
            public long MaxTimeMS = 0;
            public long MinTimeMS = 0;
            public long TotalTimeMS = 0;
            public List<long> Pings = new List<long>();
        }

        public static async Task<ClsPingOut> IsConnected(string HostOrIPToPing = "www.google.com", int TimeoutMS = 2000, int RetryCount = 3, int DelayMS = 25, bool AlwaysRetry = false)
        {
            ClsPingOut ret = new ClsPingOut();
            Stopwatch SW = Stopwatch.StartNew();

            try
            {
                IPAddress IP = null;

                if (!IsValidIPAddress(HostOrIPToPing, out IP))
                    IP = await GetIPAddressFromHostnameAsync(HostOrIPToPing);

                ret.DNSResolveMS = SW.ElapsedMilliseconds;

                if (!IsValidIPAddress(IP, out IP))
                    return ret;

                Log($"Trace: Pinging {HostOrIPToPing} ({IP.ToString()}) With timeout:{TimeoutMS}ms And Ping Retry Count:{RetryCount}...");
                for (int Tries = 1; Tries <= RetryCount; Tries++)
                {
                    ret.Retries = Tries;
                    byte[] buffer = new byte[32];
                    PingOptions pingOptions = new PingOptions(128, false);

                    int TTLBefore = pingOptions.Ttl;
                    try
                    {
                        using (Ping Myping = new Ping())
                        {
                            ret.PingReply = await Myping.SendPingAsync(IP, TimeoutMS, buffer, pingOptions);
                        }
                    }
                    catch (Exception ex)
                    {
                        ret.PingError = ex.GetBaseException().Message;
                    }
                    if (ret.PingReply != null)
                    {
                        ret.Success = (ret.PingReply.Status == IPStatus.Success);
                        ret.PingError = ret.PingReply.Status.ToString();
                        if (ret.PingReply.Options != null) //ipv6 returns null
                            ret.Hops = TTLBefore - ret.PingReply.Options.Ttl;
                        if (ret.Success)
                        {
                            ret.Pings.Add(ret.PingReply.RoundtripTime);

                            if (!AlwaysRetry) //If we want to get a true ping average, dont break out of the loop yet
                                break;
                        }
                    }

                    // wait before next try
                    await Task.Delay(DelayMS);
                }
            }
            catch (Exception ex)
            {
                ret.PingError = $"{ex.Msg()} (Site={HostOrIPToPing} Timeout was {TimeoutMS}ms)";
                Log($"Error: {ret.PingError}");
            }
            finally
            {
            }

            if (ret.Pings.Count > 0)
            {
                ret.AvgTimeMS = System.Convert.ToInt64(ret.Pings.Average());
                ret.MaxTimeMS = System.Convert.ToInt64(ret.Pings.Max());
                ret.MinTimeMS = System.Convert.ToInt64(ret.Pings.Min());
            }

            SW.Stop();
            ret.TotalTimeMS = SW.ElapsedMilliseconds;

            Log($"Trace: ...Result={ret.Success}, {ret.TotalTimeMS}ms, {ret.PingError}");

            return ret;
        }


        public static void MoveFiles(string FromFolder, string ToFolder, string FileSpec, bool OnlyIfNewer, bool OnlyCopy = false)
        {
            //Let us pass a filename so we can be lazy
            if (Path.HasExtension(FromFolder))
                FromFolder = Path.GetDirectoryName(FromFolder);

            if (Path.HasExtension(ToFolder))
                ToFolder = Path.GetDirectoryName(ToFolder);

            List<FileInfo> files = GetFiles(FromFolder, FileSpec, SearchOption.TopDirectoryOnly);

            int cnt = 0;

            if (files.Count > 0)
            {
                if (!Directory.Exists(ToFolder))
                    Directory.CreateDirectory(ToFolder);

            }

            foreach (FileInfo fi in files)
            {
                string newfile = Path.Combine(ToFolder, fi.Name);
                try
                {
                    bool move = true;
                    FileInfo nfi = new FileInfo(newfile);
                    if (nfi.Exists)
                    {
                        if (fi.LastWriteTime < nfi.LastWriteTime)
                        {
                            //just delete the older file rather than moving it
                            move = false;
                            if (!OnlyCopy)
                                fi.Delete();
                        }
                    }

                    if (move)
                    {
                        if (!OnlyCopy)
                            fi.MoveTo(newfile);
                        else
                            fi.CopyTo(newfile, true);

                    }

                    cnt++;
                }
                catch (Exception ex)
                {

                    Log($"Error: Could not move {fi.FullName} to {newfile}: {ex.Msg()}");
                }
            }

            Log($"Debug: Moved {cnt} '{FileSpec}' files from {FromFolder} to {ToFolder}.");

        }

        public static string GetFrameworkVersion()
        {
            using (RegistryKey ndpKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full"))
            {
                if (ndpKey != null)
                {
                    int value = (int)(ndpKey.GetValue("Release") ?? 0);
                    string ver = ndpKey.GetValue("Version", "Unknown").ToString();
                    if (value >= 528040)
                        return new Version(4, 8, 0).ToString() + $" (v{ver}, Release {value.ToString()})";

                    if (value >= 461808)
                        return new Version(4, 7, 2).ToString() + $" (v{ver}, Release {value.ToString()})";

                    if (value >= 461308)
                        return new Version(4, 7, 1).ToString() + $" (v{ver}, Release {value.ToString()})";

                    if (value >= 460798)
                        return new Version(4, 7, 0).ToString() + $" (v{ver}, Release {value.ToString()})";

                    if (value >= 394802)
                        return new Version(4, 6, 2).ToString() + $" (v{ver}, Release {value.ToString()})";

                    if (value >= 394254)
                        return new Version(4, 6, 1).ToString() + $" (v{ver}, Release {value.ToString()})";

                    if (value >= 393295)
                        return new Version(4, 6, 0).ToString() + $" (v{ver}, Release {value.ToString()})";

                    if (value >= 379893)
                        return new Version(4, 5, 2).ToString() + $" (v{ver}, Release {value.ToString()})";

                    if (value >= 378675)
                        return new Version(4, 5, 1).ToString() + $" (v{ver}, Release {value.ToString()})";

                    if (value >= 378389)
                        return new Version(4, 5, 0).ToString() + $" (v{ver}, Release {value.ToString()})";

                    return $"Unknown release {value}";
                }

                throw new NotSupportedException(@"No registry key found under 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full' to determine running framework version");
            }
        }


        public static bool DeleteRegSetting(string Name)
        {

            //regkey is built from CompanyName\ProductName\MajorVersion.MinorVersion
            Version AN = Assembly.GetExecutingAssembly().GetName().Version;
            string Cname = System.Windows.Forms.Application.CompanyName;
            string Pname = System.Windows.Forms.Application.ProductName;
            string version = AN.Major + "." + AN.Minor;
            string SKey = "";
            bool ret = false;
            try
            {
                string RKey = $"Software\\{Cname}\\{Pname}\\{version}{SKey}";

                using RegistryKey reg = Registry.CurrentUser.OpenSubKey(RKey, false);

                if (reg != null)
                {
                    bool Found = false;
                    string[] Values = reg.GetValueNames();
                    foreach (string valu in Values)
                        if (string.Equals(valu, Name, StringComparison.OrdinalIgnoreCase))
                        {
                            Found = true;
                            break;
                        }
                    if (Found)
                    {
                        reg.DeleteValue(Name, false);
                        ret = true;
                    }
                }


            }
            catch (Exception)
            {
                //Log($"Error: {ex.Msg()}");
            }

            return ret;

        }
        public static bool DeleteRegSettings()
        {

            //regkey is built from CompanyName\ProductName\MajorVersion.MinorVersion
            Version AN = Assembly.GetExecutingAssembly().GetName().Version;
            string Cname = System.Windows.Forms.Application.CompanyName;
            string Pname = System.Windows.Forms.Application.ProductName;
            string version = AN.Major + "." + AN.Minor;
            bool ret = false;
            try
            {
                string RKey = $"Software\\{Cname}\\{Pname}\\{version}";

                Registry.CurrentUser.DeleteSubKeyTree(RKey, false);

                ret = true;

            }
            catch (Exception)
            {
                //Log($"Error: {ex.Msg()}");
            }

            return ret;

        }

        public static dynamic GetRegSetting(string Name, object DefaultValue = null, string SubKey = "")
        {

            //regkey is built from CompanyName\ProductName\MajorVersion.MinorVersion
            Version AN = Assembly.GetExecutingAssembly().GetName().Version;
            string Cname = System.Windows.Forms.Application.CompanyName;
            string Pname = System.Windows.Forms.Application.ProductName;
            string version = AN.Major + "." + AN.Minor;
            object RetVal = DefaultValue;
            string SKey = "";
            if (!string.IsNullOrWhiteSpace(SubKey))
                SKey = "\\" + SubKey.Trim();
            try
            {
                string RKey = $"Software\\{Cname}\\{Pname}\\{version}{SKey}";

                using RegistryKey reg = Registry.CurrentUser.OpenSubKey(RKey, false);
                if (reg != null)
                {
                    bool Found = false;
                    string[] Values = reg.GetValueNames();
                    foreach (string valu in Values)
                        if (string.Equals(valu, Name, StringComparison.OrdinalIgnoreCase))
                        {
                            Found = true;
                            RetVal = reg.GetValue(Name, DefaultValue);
                            break;
                        }
                    if (Found)
                    {
                        if (reg.GetValueKind(Name) == RegistryValueKind.MultiString)
                        {
                            if (DefaultValue is List<string>)
                                RetVal = ((string[])RetVal).ToList();
                            else if (DefaultValue is object[])
                                RetVal = (string[])RetVal;
                            else if (DefaultValue is string[])
                                RetVal = (string[])RetVal;
                        }
                        else if (RetVal is string && DefaultValue is Point)
                        {
                            //{X=965,Y=399}
                            int X = GetNumberInt(RetVal.ToString().GetWord("X=", ","));
                            int Y = GetNumberInt(RetVal.ToString().GetWord("Y=", "}"));
                            RetVal = new Point(X, Y);

                        }
                        else if (RetVal is string && DefaultValue is Size)
                        {
                            //{Width=931, Height=592}
                            int Wid = GetNumberInt(RetVal.ToString().GetWord("Width=", ","));
                            int Hei = GetNumberInt(RetVal.ToString().GetWord("Height=", "}"));
                            RetVal = new Size(Wid, Hei);
                        }
                        else if (RetVal is string)
                        {
                            if (RetVal.ToString().Length > 256 && IsBase64String(RetVal.ToString()))
                            {
                                string base64 = "";
                                try
                                {
                                    base64 = DeCompressFromBase64String(RetVal.ToString());
                                }
                                catch (Exception)
                                {
                                }

                                if (base64.IsEmpty())  //maybe its not really base 64 + gzip compressed?
                                    base64 = RetVal.ToString();

                                RetVal = Convert.ChangeType(base64, DefaultValue.GetType());
                            }
                            else
                            {
                                RetVal = Convert.ChangeType(RetVal, DefaultValue.GetType());
                            }
                        }
                        else if (DefaultValue != null)
                            RetVal = Convert.ChangeType(RetVal, DefaultValue.GetType());
                        //Else
                        //    RetVal = Convert.ChangeType(RetVal, DefaultValue.GetType)


                    }
                }


            }
            catch (Exception)
            {
                //Log($"Error: {ex.Msg()}");
            }

            return RetVal;

        }
        public static Double GetNumberDbl(object Obj)
        {
            //gets a number from anywhere within a string
            double Ret = 0;
            if (!Obj.IsNull())
            {
                if (Obj is string)
                {
                    string o = System.Convert.ToString(Obj).Trim();
                    double outdbl = 0;

                    //Take into account that some countries may use 123,45 vs 123.45
                    if (double.TryParse(o, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out outdbl))
                        Ret = outdbl;
                    else //try to extract the number from a larger string
                    {
                        try
                        {
                            //this can grab anything even "The number is 69,9 dude"
                            string outstrnum = Regex.Match(o.Replace(",", "."), @"[-+]?(?:\b[0-9]+(?:\.[0-9]*)?|\.[0-9]+\b)(?:[eE][-+]?[0-9]+\b)?").Value;
                            if (double.TryParse(outstrnum, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out outdbl))
                                Ret = outdbl;
                            else
                            {
                                Log($"Error: Could not parse to double? '{o}'");
                            }
                        }
                        catch { }

                    }


                    //if (OnlyNums != o)
                    //{
                    //    //debug
                    //    int brkpt = 0;
                    //}

                }
                else if (Obj is int)
                    Ret = Convert.ToDouble(Obj);
                else if (Obj is double)
                    Ret = ((double)Obj);
                else if (Obj is float)
                    Ret = (float)Obj;
            }
            return Ret;

        }
        public static int GetNumberInt(object Obj)
        {
            //gets a number from anywhere within a string
            int Ret = 0;
            if (Obj != null)
            {
                if (Obj is string && !string.IsNullOrWhiteSpace((string)Obj))
                {
                    string o = System.Convert.ToString(Obj).Trim();
                    double outdbl = 0;
                    string OnlyNums = "";

                    try
                    {
                        //this can grab anything even "The number is 69"
                        OnlyNums = Regex.Match(o, @"[-+]?(?:\b[0-9]+(?:\.[0-9]*)?|\.[0-9]+\b)(?:[eE][-+]?[0-9]+\b)?").Value;
                    }
                    catch { }

                    if (OnlyNums != o)
                    {
                        //debug
                        int brkpt = 0;
                    }

                    if (double.TryParse(OnlyNums, out outdbl))
                        Ret = Convert.ToInt32(Math.Round(outdbl));
                }
                else if (Obj is int)
                    Ret = (int)Obj;
                else if (Obj is double)
                    Ret = ((double)Obj).ToInt();
                else if (Obj is float)
                    Ret = Convert.ToInt32(Math.Round((float)Obj));
            }
            return Ret;

        }
        public static bool SaveRegSetting(string name, object value, string SubKey = "")
        {
            bool ret = false;
            //regkey is built from CompanyName\ProductName\MajorVersion.MinorVersion
            Version AN = Assembly.GetExecutingAssembly().GetName().Version;
            string Cname = System.Windows.Forms.Application.CompanyName;
            string Pname = System.Windows.Forms.Application.ProductName;
            string version = AN.Major + "." + AN.Minor;
            string SKey = "";
            if (!string.IsNullOrWhiteSpace(SubKey))
                SKey = "\\" + SubKey.Trim();
            try
            {
                string RKey = $"Software\\{Cname}\\{Pname}\\{version}{SKey}";
                using RegistryKey reg = Registry.CurrentUser.CreateSubKey(RKey, RegistryKeyPermissionCheck.ReadWriteSubTree);
                if (reg != null)
                {
                    if (value is List<string>)
                    {
                        List<string> strlist = (List<string>)value;
                        reg.SetValue(name, strlist.ToArray(), RegistryValueKind.MultiString);
                    }
                    else if (value is object[])
                    {
                        List<string> strlist = new List<string>();
                        object[] objects = (object[])value;
                        foreach (object obj in objects)
                            strlist.Add(obj.ToString());
                        reg.SetValue(name, strlist.ToArray(), RegistryValueKind.MultiString);
                    }
                    else if (value is string)
                    {
                        //large strings may cause, so compress and base 64 encode
                        //Insufficient system resources exist to complete the requested service.; [IOException]
                        string compressed = value.ToString();

                        if (compressed.Length > 2048)
                            compressed = CompressToBase64String(compressed);

                        reg.SetValue(name, compressed);

                    }
                    else
                    {
                        reg.SetValue(name, value);

                    }
                    ret = true;
                }


            }
            catch (Exception ex)
            {
                Log($"Error: ({name}={value.ToString().Length} bytes) {ex.Msg()}");
            }
            finally
            {

            }

            return ret;
        }

        public static string CompressToBase64String(string strdata)
        {
            return Convert.ToBase64String(CompressGzip(Encoding.UTF8.GetBytes(strdata)));
        }
        public static string DeCompressFromBase64String(string strdata)
        {
            return Encoding.UTF8.GetString(DecompressGZ(Convert.FromBase64String(strdata)));
        }

        public static bool IsBase64String(string value)
        {
            if (value == null || value.Length == 0 || value.Length % 4 != 0
                || value.Contains(' ') || value.Contains('\t') || value.Contains('\r') || value.Contains('\n'))
                return false;
            var index = value.Length - 1;
            if (value[index] == '=')
                index--;
            if (value[index] == '=')
                index--;
            for (var i = 0; i <= index; i++)
                if (IsInvalid(value[i]))
                    return false;
            return true;
        }
        // Make it private as there is the name makes no sense for an outside caller
        private static bool IsInvalid(char value)
        {
            var intValue = (Int32)value;
            if (intValue >= 48 && intValue <= 57)
                return false;
            if (intValue >= 65 && intValue <= 90)
                return false;
            if (intValue >= 97 && intValue <= 122)
                return false;
            return intValue != 43 && intValue != 47;
        }
        public static byte[] CompressGzip(byte[] data)
        {
            try
            {
                using (MemoryStream output = new MemoryStream())
                {
                    using (GZipStream gzip = new GZipStream(output, CompressionMode.Compress, false))
                    {
                        gzip.Write(data, 0, data.Length);
                        gzip.Close();
                        return output.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Log("Error: " + ex.Msg());
            }

            return null;

        }

        public static byte[] DecompressGZ(byte[] data)
        {
            try
            {
                Stopwatch sw1 = new Stopwatch();
                sw1.Start();
                using (MemoryStream input = new MemoryStream())
                {
                    input.Write(data, 0, data.Length);
                    input.Position = 0;
                    using (GZipStream gzip = new GZipStream(input, CompressionMode.Decompress, false))
                    {
                        using (MemoryStream output = new MemoryStream())
                        {
                            byte[] buff = new byte[4097];
                            int read = -1;
                            read = gzip.Read(buff, 0, buff.Length);
                            while (read > 0)
                            {
                                output.Write(buff, 0, read);
                                read = gzip.Read(buff, 0, buff.Length);
                            }
                            gzip.Close();
                            sw1.Stop();
                            //Debug.Print(" (gz: " + sw1.ElapsedMilliseconds + "ms)");
                            return output.ToArray();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log("Error: " + ex.Msg());
            }

            return null;

        }

        public static string ReplaceCaseInsensitive(string input, string search, string replacement)
        {
            string result = Regex.Replace(
                input,
                Regex.Escape(search),
                replacement.Replace("$", "$$"),
                RegexOptions.IgnoreCase
            );
            return result;
        }


        public static String WildCardToRegular(String value)
        {
            return "^" + Regex.Escape(value).Replace("\\?", ".").Replace("\\*", ".*") + "$";
        }

        public static void SendMessage(MessageType MT, string Descript = "", object Payload = null, [CallerMemberName] string memberName = null)
        {
            if (progress == null)
            {
                //TODO: trying to catch a logging failure, should take out messagebox later
                if (!warnedlog && !Global.IsService)
                {
                    warnedlog = true;
                    string member = "unknown";
                    if (memberName != null)
                        member = memberName;

                    MessageBox.Show($"SendMessage {MT.ToString()} Debug warning: Progress event logger is null? calling function='{member}'");
                }
                return;
            }

            ClsMessage msg = new ClsMessage(MT, Descript, Payload, memberName);

            progress.Report(msg);

        }

        public static async void TelegramControlMessage(string Message, [CallerMemberName] string memberName = null)
        {

            try
            {
                //PAUSE|STOP [CAMNAME] [MINUTES]
                //STOP 30   <<---Stops/pauses all cameras for 30 minutes
                //PAUSE CAMERANAME 30
                //START|RESUME [CAMNAME]
                //RESUME
                //RESUME CAMERANAME

                if (Message.StartsWith("play:", StringComparison.OrdinalIgnoreCase))
                {
                    string file = Message.GetWord("play:", "");
                    string soundfile = Global.FindSoundFile(file);
                    if (soundfile.IsNotNull())
                    {
                        AITOOL.Telegram.SendTextMessageAsync(AppSettings.Settings.telegram_chatids.GetStrAtIndex(0), $"Playing voice file {soundfile}...");

                        if (soundfile.EndsWith(".ogg", StringComparison.OrdinalIgnoreCase))
                        {
                            PlayOOG(soundfile);
                        }
                        else
                        {
                            Log($"Debug:   Playing sound: {soundfile}...");
                            using SoundPlayer sp = new SoundPlayer(soundfile);
                            sp.PlaySync();
                        }
                    }
                    else
                    {
                        Log($"Error: Sound file not found: {soundfile}");
                        AITOOL.Telegram.SendTextMessageAsync(AppSettings.Settings.telegram_chatids.GetStrAtIndex(0), $"Error: Could not find {soundfile}...");
                    }
                    return;
                }

                List<string> parts = Message.SplitStr(" ", TrimChars: " []");

                bool pause = false;
                bool resume = false;
                double howlong = 525600;  //let it default to the number of minutes in a year
                string camname = "";

                if (parts.Count > 0)
                {
                    if (parts[0].EqualsIgnoreCase("pause") || parts[0].EqualsIgnoreCase("stop"))
                        pause = true;
                    else if (parts[0].EqualsIgnoreCase("resume") || parts[0].EqualsIgnoreCase("start"))
                        resume = true;
                    else if (parts[0].EqualsIgnoreCase("restartcomputer") || parts[0].EqualsIgnoreCase("restartpc") || parts[0].EqualsIgnoreCase("reboot") || parts[0].EqualsIgnoreCase("rebootcomputer") || parts[0].EqualsIgnoreCase("rebootpc"))
                    {
                        await AITOOL.Telegram.SendTextMessageAsync(AppSettings.Settings.telegram_chatids.GetStrAtIndex(0), $"Computer restarting in 10 seconds...");

                        using (Process prc = System.Diagnostics.Process.Start("shutdown.exe", "-r -f -t 10")) { }
                        Application.Exit();
                    }
                    else if (parts[0].EqualsIgnoreCase("shutdowncomputer") || parts[0].EqualsIgnoreCase("shutdownpc"))
                    {
                        await AITOOL.Telegram.SendTextMessageAsync(AppSettings.Settings.telegram_chatids.GetStrAtIndex(0), $"Computer shutting down in 10 seconds...");
                        using (Process prc = System.Diagnostics.Process.Start("shutdown.exe", "-s -f -t 10")) { }

                        Application.Exit();
                    }
                    else if (parts[0].EqualsIgnoreCase("restart") || parts[0].EqualsIgnoreCase("restartaitool"))
                    {
                        await AITOOL.Telegram.SendTextMessageAsync(AppSettings.Settings.telegram_chatids.GetStrAtIndex(0), $"Restarting AITOOL...");
                        Restart.WriteFullFence(true);
                        Application.Exit();

                    }
                    else if (parts[0].EqualsIgnoreCase("mute"))
                    {
                        await AITOOL.Telegram.SendTextMessageAsync(AppSettings.Settings.telegram_chatids.GetStrAtIndex(0), $"Muting.");
                        Log("Muting.");
                        Mute();
                    }
                    else if (parts[0].EqualsIgnoreCase("unmute"))
                    {
                        await AITOOL.Telegram.SendTextMessageAsync(AppSettings.Settings.telegram_chatids.GetStrAtIndex(0), $"UnMuting.");
                        Log("UnMuting");
                        UnMute();
                    }
                    else if (parts[0].EqualsIgnoreCase("volumeup"))
                    {
                        await AITOOL.Telegram.SendTextMessageAsync(AppSettings.Settings.telegram_chatids.GetStrAtIndex(0), $"VolumeUp.");
                        Log("Volume Up");
                        VolUp();
                    }
                    else if (parts[0].EqualsIgnoreCase("volumedown"))
                    {
                        await AITOOL.Telegram.SendTextMessageAsync(AppSettings.Settings.telegram_chatids.GetStrAtIndex(0), $"VolumeDown.");
                        Log("Volume Down");
                        VolDown();
                    }
                    else if (parts[0].EqualsIgnoreCase("volumeset"))
                    {
                        float num = parts.GetStrAtIndex(1).ToFloat();
                        await AITOOL.Telegram.SendTextMessageAsync(AppSettings.Settings.telegram_chatids.GetStrAtIndex(0), $"VolumeSet {num}");
                        Log($"Volume set {num}");
                        VolSet(num);
                    }
                    else if (parts[0].EqualsIgnoreCase("screenshot"))
                    {
                        await AITOOL.Telegram.SendTextMessageAsync(AppSettings.Settings.telegram_chatids.GetStrAtIndex(0), $"Taking screenshot...");
                        Log($"Debug: Taking screenshot of primary screen @ {Screen.PrimaryScreen.Bounds.Width}x{Screen.PrimaryScreen.Bounds.Height}");

                        using (Bitmap bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height))
                        {
                            using (Graphics g = Graphics.FromImage(bitmap))
                            {
                                g.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
                            }

                            //string outfile = Path.Combine(GetTempFolder(), $"Screenshot_{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.jpg");


                            using (MemoryStream ms = new MemoryStream())
                            {
                                bitmap.Save(ms, ImageFormat.Jpeg);
                                ms.Seek(0, SeekOrigin.Begin); //go back to start
                                await AITOOL.Telegram.SendPhotoAsync(AppSettings.Settings.telegram_chatids.GetStrAtIndex(0), ms, "", "screenshot.jpg", "");
                            }

                            //ClsImageQueueItem img = new ClsImageQueueItem(outfile, 0, false);

                        }
                    }
                    else
                    {
                        Log($"Debug: Unknown AITOOL Telegram control command '{parts[0]}'");
                        AITOOL.Telegram.SendTextMessageAsync(AppSettings.Settings.telegram_chatids.GetStrAtIndex(0), $"Debug: Unknown Telegram control command '{parts[0]}'");
                    }

                    if (pause || resume)
                    {
                        //check if the second parameter is a camera name or number
                        if (parts.Count > 1)
                        {
                            if (parts[1].IsNumeric())
                            {
                                howlong = parts[1].ToDouble();
                            }
                            else
                            {
                                camname = parts[1];
                            }
                        }

                        if (parts.Count > 2 && parts[2].IsNumeric())
                        {
                            howlong = parts[2].ToDouble();
                        }

                        if (camname.IsNotEmpty())
                        {
                            Camera cam = AITOOL.GetCamera(camname);
                            if (cam != null)
                            {
                                if (pause)
                                {
                                    cam.PauseMinutes = howlong;
                                    cam.PauseFileMon = true;
                                    cam.PauseMQTT = true;
                                    cam.PausePushover = true;
                                    cam.PauseTelegram = true;
                                    cam.PauseURL = true;
                                    cam.Pause();
                                    AITOOL.Telegram.SendTextMessageAsync(AppSettings.Settings.telegram_chatids.GetStrAtIndex(0), $"Camera '{camname}' paused {howlong} minutes.");
                                }
                                else
                                {
                                    cam.PauseFileMon = false;
                                    cam.PauseMQTT = false;
                                    cam.PausePushover = false;
                                    cam.PauseTelegram = false;
                                    cam.PauseURL = false;
                                    cam.Resume();
                                    AITOOL.Telegram.SendTextMessageAsync(AppSettings.Settings.telegram_chatids.GetStrAtIndex(0), $"Camera '{camname}' resumed.");
                                }
                            }
                            else
                            {
                                Log($"Error: Camera '{camname}' not found.");
                                AITOOL.Telegram.SendTextMessageAsync(AppSettings.Settings.telegram_chatids.GetStrAtIndex(0), $"Error: Camera '{camname}' not found.");
                            }
                        }
                        else
                        {
                            string pausemsg = "";
                            if (pause)
                                pausemsg = $"All cameras paused {howlong} minutes.";
                            else
                                pausemsg = $"All cameras resumed.";


                            foreach (var cam in AppSettings.Settings.CameraList)
                            {
                                if (pause)
                                {
                                    cam.PauseMinutes = howlong;
                                    cam.PauseFileMon = true;
                                    cam.PauseMQTT = true;
                                    cam.PausePushover = true;
                                    cam.PauseTelegram = true;
                                    cam.PauseURL = true;
                                    cam.Pause();
                                }
                                else
                                {
                                    cam.PauseFileMon = false;
                                    cam.PauseMQTT = false;
                                    cam.PausePushover = false;
                                    cam.PauseTelegram = false;
                                    cam.PauseURL = false;
                                    cam.Resume();
                                }
                            }

                            AITOOL.Telegram.SendTextMessageAsync(AppSettings.Settings.telegram_chatids.GetStrAtIndex(0), pausemsg);

                        }

                    }

                }
                else
                {
                    Log("Debug: empty Telegram control message?");
                }

            }
            catch (Exception ex)
            {
                Log($"Error: {ex.Msg()}");
            }


        }
        public static void DeleteHistoryItem(string filename, [CallerMemberName] string memberName = null)
        {
            if (progress == null)
            {
                //TODO: trying to catch a logging failure, should take out messagebox later
                if (!warnedlog && !Global.IsService)
                {
                    warnedlog = true;
                    string member = "unknown";
                    if (memberName != null)
                        member = memberName;

                    MessageBox.Show($"DeleteHistoryItem Debug warning: Progress event logger is null? calling function='{member}'");
                }
                return;
            }

            ClsMessage msg = new ClsMessage(MessageType.DeleteHistoryItem, filename, null, memberName);

            progress.Report(msg);

        }

        public static void PlayOOG(string filename)
        {
            //This cannot play .OGG files created by the Telegram.Bot engine?
            //Could not load stream 1483939711 due to error: Found OPUS bitstream.
            //'System.ArgumentException' in NAudio.Vorbis.dll
            //Could not initialize container!
            using NAudio.Vorbis.VorbisWaveReader vorbis = new NAudio.Vorbis.VorbisWaveReader(filename);
            using NAudio.Wave.WaveOut waveOut = new NAudio.Wave.WaveOut();
            waveOut.Init(vorbis);
            waveOut.Play();
            while (waveOut.PlaybackState == PlaybackState.Playing)
            {
                if (MasterCTS.IsNotNull() && MasterCTS.Token.IsCancellationRequested)
                    break;

                Task.Delay(100, MasterCTS.Token);
            }

        }

        public static void Mute()
        {

            //SendMessageW(hand, WM_APPCOMMAND, hand, (IntPtr)APPCOMMAND_VOLUME_MUTE);

            using (var enumerator = new NAudio.CoreAudioApi.MMDeviceEnumerator())
            {
                foreach (var device in enumerator.EnumerateAudioEndPoints(NAudio.CoreAudioApi.DataFlow.Render, NAudio.CoreAudioApi.DeviceState.Active))
                {
                    if (device.AudioEndpointVolume?.HardwareSupport.HasFlag(NAudio.CoreAudioApi.EEndpointHardwareSupport.Mute) == true)
                    {
                        Console.WriteLine(device.FriendlyName);
                        device.AudioEndpointVolume.Mute = true;
                    }
                }
            }
        }
        public static void UnMute()
        {
            using (var enumerator = new NAudio.CoreAudioApi.MMDeviceEnumerator())
            {
                foreach (var device in enumerator.EnumerateAudioEndPoints(NAudio.CoreAudioApi.DataFlow.Render, NAudio.CoreAudioApi.DeviceState.Active))
                {
                    if (device.AudioEndpointVolume?.HardwareSupport.HasFlag(NAudio.CoreAudioApi.EEndpointHardwareSupport.Mute) == true)
                    {
                        device.AudioEndpointVolume.Mute = false;
                    }
                }
            }
        }

        public static void VolDown()
        {
            //SendMessageW(hand, WM_APPCOMMAND, hand, (IntPtr)APPCOMMAND_VOLUME_DOWN);

            using (var enumerator = new NAudio.CoreAudioApi.MMDeviceEnumerator())
            {
                foreach (var device in enumerator.EnumerateAudioEndPoints(NAudio.CoreAudioApi.DataFlow.Render, NAudio.CoreAudioApi.DeviceState.Active))
                {
                    if (device.AudioEndpointVolume?.HardwareSupport.HasFlag(NAudio.CoreAudioApi.EEndpointHardwareSupport.Volume) == true)
                    {
                        device.AudioEndpointVolume.VolumeStepDown();
                    }
                }
            }

        }

        public static void VolUp()
        {
            //SendMessageW(hand, WM_APPCOMMAND, hand, (IntPtr)APPCOMMAND_VOLUME_UP);
            using (var enumerator = new NAudio.CoreAudioApi.MMDeviceEnumerator())
            {
                foreach (var device in enumerator.EnumerateAudioEndPoints(NAudio.CoreAudioApi.DataFlow.Render, NAudio.CoreAudioApi.DeviceState.Active))
                {
                    if (device.AudioEndpointVolume?.HardwareSupport.HasFlag(NAudio.CoreAudioApi.EEndpointHardwareSupport.Volume) == true)
                    {
                        device.AudioEndpointVolume.VolumeStepUp();
                    }
                }
            }

        }

        public static void VolSet(float Level)
        {
            //SendMessageW(hand, WM_APPCOMMAND, hand, (IntPtr)APPCOMMAND_VOLUME_UP);
            using (var enumerator = new NAudio.CoreAudioApi.MMDeviceEnumerator())
            {
                foreach (var device in enumerator.EnumerateAudioEndPoints(NAudio.CoreAudioApi.DataFlow.Render, NAudio.CoreAudioApi.DeviceState.Active))
                {
                    if (device.AudioEndpointVolume?.HardwareSupport.HasFlag(NAudio.CoreAudioApi.EEndpointHardwareSupport.Volume) == true)
                    {
                        //device.AudioEndpointVolume.VolumeRange.MaxDecibels
                        //device.AudioEndpointVolume.VolumeRange.MinDecibels
                        //The new master volume level. The level is expressed as a normalized value in the range from 0.0 to 1.0.
                        device.AudioEndpointVolume.MasterVolumeLevelScalar = Level / 100f;
                    }
                }
            }

        }
        public static void UpdateProgressBar(string label, int CurVal = -1, int MinVal = -1, int MaxVal = -1, [CallerMemberName] string memberName = null)
        {
            if (progress == null)
            {
                //TODO: trying to catch a logging failure, should take out messagebox later
                if (!warnedlog && !Global.IsService)
                {
                    warnedlog = true;
                    string member = "unknown";
                    if (memberName != null)
                        member = memberName;

                    MessageBox.Show($"UpdateProgressBar Debug warning: Progress event logger is null? calling function='{member}'");
                }
                return;
            }

            ClsMessage msg = new ClsMessage(MessageType.UpdateProgressBar, label, null, memberName, CurVal, MinVal, MaxVal);

            progress.Report(msg);

        }

        public static void CreateHistoryItem(History hist, [CallerMemberName] string memberName = null)
        {
            if (progress == null)
            {
                //TODO: trying to catch a logging failure, should take out messagebox later
                if (!warnedlog && !Global.IsService)
                {
                    warnedlog = true;
                    string member = "unknown";
                    if (memberName != null)
                        member = memberName;

                    MessageBox.Show($"CreateHistoryItem Debug warning: Progress event logger is null? calling function='{member}'");
                }
                return;
            }

            ClsMessage msg = new ClsMessage(MessageType.CreateHistoryItem, "", hist, memberName);

            progress.Report(msg);

        }

        public static void UpdateLabel(string Message, string LabelControlName, [CallerMemberName] string memberName = null)
        {
            if (progress == null)
            {
                //TODO: trying to catch a logging failure, should take out messagebox later
                if (!warnedlog && !Global.IsService)
                {
                    warnedlog = true;
                    string member = "unknown";
                    if (memberName != null)
                        member = memberName;

                    MessageBox.Show($"UpdateLabel Debug warning: Progress event logger is null? calling function='{member}'");
                }
                return;
            }

            ClsMessage msg = new ClsMessage(MessageType.UpdateLabel, Message, LabelControlName, memberName);

            progress.Report(msg);

        }
        private static bool warnedlog = false;
        public static void LogMessage(string Message, [CallerMemberName] string memberName = null)
        {
            if (progress == null)
            {
                //TODO: trying to catch a logging failure, should take out messagebox later
                if (!warnedlog && !Global.IsService)
                {
                    warnedlog = true;
                    string member = "unknown";
                    if (memberName != null)
                        member = memberName;

                    MessageBox.Show($"Log Debug warning: Progress event logger is null? calling function='{member}'");
                }
                return;
            }

            ClsMessage msg = new ClsMessage(MessageType.LogEntry, "", null, memberName);

            //this is for logging in non-gui classes.  Reports back to real logger
            //progress needs to be subscribed to in main gui
            string mn = "";
            if (memberName != null && !string.IsNullOrEmpty(memberName))
            {
                mn = $"{memberName}>> ";
            }
            msg.Description = $"{mn}{Message}";

            SaveRegSetting("LastLogEntry", msg.Description);
            Global.SaveRegSetting("LastShutdownState", $"checkpoint: Global.Log: {DateTime.Now}");


            progress.Report(msg);

        }
        public static Regex RegEx_ValidDate = new Regex("(19|20)[0-9]{2}-(0[1-9]|1[012])-(0[1-9]|[12][0-9]|3[01])_[0-9][0-9]_[0-9][0-9]_[0-9][0-9]", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        //[DllImport("ntdll.dll")]
        //public static extern int RtlNtStatusToDosError(int status);

        /// <summary>
        /// Flags used by <see cref="WinError.FormatMessage"/> method.
        /// </summary>
        [Flags]
        public enum FormatMessageFlags:uint
        {
            /// <summary>
            /// The function allocates a buffer large enough to hold the formatted message, and places a pointer to the
            /// allocated buffer at the address specified by <c>lpBuffer</c>. The <c>lpBuffer</c> parameter is a pointer
            /// to an <c>LPTSTR</c>. The <c>nSize</c> parameter specifies the minimum number of <c>TCHARs</c> to allocate
            /// for an output message buffer. The caller should use the <c>LocalFree</c> function to free the buffer when
            /// it is no longer needed.
            /// If the length of the formatted message exceeds 128K bytes, then <c>FormatMessage</c> will fail and a
            /// subsequent call to <c>GetLastError</c> will return <c>ERROR_MORE_DATA</c>.
            /// This value is not available for use when compiling Windows Store apps.
            /// </summary>
            FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x00000100,

            /// <summary>
            /// Insert sequences in the message definition are to be ignored and passed through to the output buffer
            /// unchanged. This flag is useful for fetching a message for later formatting. If this flag is set, the
            /// <c>Arguments</c> parameter is ignored.
            /// </summary>
            FORMAT_MESSAGE_IGNORE_INSERTS = 0x00000200,

            /// <summary>
            /// The <c>lpSource</c> parameter is a pointer to a null-terminated string that contains a message definition.
            /// The message definition may contain insert sequences, just as the message text in a message table resource
            /// may. This flag cannot be used with <see cref="FORMAT_MESSAGE_FROM_HMODULE"/> or
            /// <see cref="FORMAT_MESSAGE_FROM_SYSTEM"/>.
            /// </summary>
            FORMAT_MESSAGE_FROM_STRING = 0x00000400,

            /// <summary>
            /// The <c>lpSource</c> parameter is a module handle containing the message-table resource(s) to search. If
            /// this <c>lpSource</c> handle is <c>null</c>, the current process's application image file will be searched.
            /// This flag cannot be used with <see cref="FORMAT_MESSAGE_FROM_STRING"/>.
            /// If the module has no message table resource, the function fails with <c>ERROR_RESOURCE_TYPE_NOT_FOUND</c>.
            /// </summary>
            FORMAT_MESSAGE_FROM_HMODULE = 0x00000800,

            /// <summary>
            /// The function should search the system message-table resource(s) for the requested message. If this flag is
            /// specified with <see cref="FORMAT_MESSAGE_FROM_HMODULE"/>, the function searches the system message table
            /// if the message is not found in the module specified by <c>lpSource</c>. This flag cannot be used with
            /// <see cref="FORMAT_MESSAGE_FROM_STRING"/>.
            /// If this flag is specified, an application can pass the result of the <c>GetLastError</c> function to
            /// retrieve the message text for a system-defined error.
            /// </summary>
            FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000,

            /// <summary>
            /// The Arguments parameter is not a <c>va_list</c> structure, but is a pointer to an array of values that
            /// represent the arguments. This flag cannot be used with 64-bit integer values. If you are using a 64-bit
            /// integer, you must use the <c>va_list</c> structure.
            /// </summary>
            FORMAT_MESSAGE_ARGUMENT_ARRAY = 0x00002000
        }
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern uint FormatMessage(
                FormatMessageFlags dwFlags,
                IntPtr lpSource,
                uint dwMessageId,
                uint dwLanguageId,
                StringBuilder lpBuffer,
                uint nSize,
                IntPtr arguments);
        //[DllImport("Kernel32.dll", SetLastError = true)]
        //static extern uint FormatMessage(uint dwFlags, IntPtr lpSource, uint dwMessageId, uint dwLanguageId, ref IntPtr lpBuffer, uint nSize, IntPtr pArguments);
        //[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        //static extern uint FormatMessage(uint dwFlags, IntPtr lpSource, uint dwMessageId, uint dwLanguageId, StringBuilder lpBuffer, uint nSize, IntPtr Arguments);
        public static string FormatMessageFromHRESULT(int errorcode)
        {
            const int nCapacity = 1024; // max error length
                                        //const uint FORMAT_MSG_FROM_SYS = 0x01000;

            //const uint FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x00000100;
            //const uint FORMAT_MESSAGE_IGNORE_INSERTS = 0x00000200;
            //const uint FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;
            //const uint FORMAT_MESSAGE_ARGUMENT_ARRAY = 0x00002000;
            //const uint FORMAT_MESSAGE_FROM_HMODULE = 0x00000800;
            //const uint FORMAT_MESSAGE_FROM_STRING = 0x00000400;
            //const int HresultWin32Prefix = unchecked((int)0x80070000);
            StringBuilder defSb = new StringBuilder(nCapacity);

            const FormatMessageFlags Flags = FormatMessageFlags.FORMAT_MESSAGE_FROM_SYSTEM
                | FormatMessageFlags.FORMAT_MESSAGE_IGNORE_INSERTS
                | FormatMessageFlags.FORMAT_MESSAGE_ARGUMENT_ARRAY;

            var buffer = new StringBuilder(nCapacity);
            uint result = FormatMessage(Flags, IntPtr.Zero, (uint)errorcode, 0, buffer, nCapacity, IntPtr.Zero);
            string ret = "";
            if (result != 0)
            {
                ret = buffer.ToString().Trim();
            }
            return ret;
            //uint dwChars = FormatMessage(
            //    FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS | FORMAT_MESSAGE_FROM_HMODULE,
            //    IntPtr.Zero,
            //    (uint)hresult,
            //    0, // Default language
            //    ref lpMsgBuf,
            //    0,
            //    IntPtr.Zero);
            //must specify the FORMAT_MESSAGE_ARGUMENT_ARRAY flag when pass an array
            //uint length = FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM, IntPtr.Zero, (uint)code, 0, defSb, nCapacity, IntPtr.Zero);



            //string sRet = "(unknown)";
            //if (dwChars > 0)
            //{
            //    sRet = Marshal.PtrToStringAnsi(lpMsgBuf).TrimEnd(' ', '.', '\r', '\n');
            //}

            //string sDefMsg = defSb.ToString().TrimEnd(' ', '.', '\r', '\n');
            ////nothing left to do:
            //return sDefMsg;
        }

        //[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        //private extern static SafeFileHandle CreateFile(
        //    string lpFileName,
        //    FileSystemRights dwDesiredAccess,
        //    FileShare dwShareMode,
        //    IntPtr securityAttrs,
        //    FileMode dwCreationDisposition,
        //    FileOptions dwFlagsAndAttributes,
        //    IntPtr hTemplateFile);


        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private extern static SafeFileHandle CreateFile(
            string lpFileName,
            [MarshalAs(UnmanagedType.U4)] FileAccess dwDesiredAccess,
            [MarshalAs(UnmanagedType.U4)] FileShare dwShareMode,
            IntPtr lpSecurityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode dwCreationDisposition,
            [MarshalAs(UnmanagedType.U4)] FileAttributes dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        private const int ERROR_ACCESS_DENIED = 5;
        private const int ERROR_SHARING_VIOLATION = 32;
        private const int ERROR_LOCK_VIOLATION = 33;
        public static async Task<WaitFileAccessResult> WaitForFileAccessAsync(string filename, FileAccess rights = FileAccess.Read, FileShare share = FileShare.Read, long WaitMS = 30000, int RetryDelayMS = 0)
        {
            //run the function in another thread
            return await Task.Run(() => WaitForFileAccess(filename, rights, share, WaitMS, RetryDelayMS));
        }

        public class WaitFileAccessResult
        {
            public bool Success = false;
            public long TimeMS = 0;
            public int ErrRetryCnt = 0;
            public string ResultString = "";
            public SafeFileHandle Handle = default(SafeFileHandle);

        }

        public static WaitFileAccessResult WaitForFileAccess(string filename,
                                                              FileAccess rights = FileAccess.Read,
                                                              FileShare share = FileShare.None,
                                                              long MaxWaitMS = 30000,
                                                              int RetryDelayMS = 0,
                                                              bool ReturnHandle = false,
                                                              long MinFileSize = 1,
                                                              int MaxErrRetryCnt = 2000)
        {

            //using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            WaitFileAccessResult ret = new WaitFileAccessResult();
            string LastFailReason = "";
            Stopwatch SW = Stopwatch.StartNew();

            if (RetryDelayMS == 0)
                RetryDelayMS = AppSettings.Settings.loop_delay_ms;

            try
            {
                //lets give it an initial tiny wait
                //await Task.Delay(RetryDelayMS);

                FileInfo FI = new FileInfo(filename);

                bool NeedsToWrite = rights == FileAccess.ReadWrite || rights == FileAccess.Write;

                if (FI.Exists)
                {

                    if (NeedsToWrite && FI.IsReadOnly)
                    {
                        ret.Success = false;
                        ret.ResultString = "(ReadOnly)";
                        return ret;
                    }

                    long LastLength = FI.Length;  //if the file is growing, try to wait for it

                    while ((ret.ErrRetryCnt <= MaxErrRetryCnt) && (SW.ElapsedMilliseconds <= MaxWaitMS))
                    {
                        if (FI.Length >= MinFileSize && LastLength == FI.Length)
                        {

                            //SafeFileHandle fileHandle = CreateFile(fileName,FileSystemRights.Modify, FileShare.Write,IntPtr.Zero, FileMode.OpenOrCreate,FileOptions.None, IntPtr.Zero);
                            ret.Handle = CreateFile(filename, rights, share, IntPtr.Zero, FileMode.Open, FileAttributes.Normal, IntPtr.Zero);

                            if (ret.Handle.IsInvalid)
                            {
                                int LastErr = Marshal.GetLastWin32Error();

                                if (LastErr == ERROR_SHARING_VIOLATION)
                                    LastFailReason = "(SharingViolation)";
                                else if (LastErr == ERROR_LOCK_VIOLATION)
                                    LastFailReason = "(LockViolation)";



                                if (LastErr != ERROR_SHARING_VIOLATION && LastErr != ERROR_LOCK_VIOLATION)
                                {
                                    LastFailReason = $"(Unexpected-{LastErr})";
                                    //unexpected error, break out
                                    Log($"Error: Unexpected Win32Error waiting for access to {filename}: {LastErr}: {new Win32Exception(LastErr)}");
                                    break;
                                }

                            }
                            else
                            {
                                //passed the first test
                                if (NeedsToWrite)
                                {
                                    //make sure we can read a byte and write the same byte back to verify access
                                    if (IsFileWritable(filename, ref ret.Handle, ref FI, out LastFailReason))
                                    {
                                        ret.Success = true;
                                        break;
                                    }
                                }
                                else
                                {
                                    ret.Success = true;
                                    break;
                                }
                            }

                            if (!ret.Handle.IsClosed)
                            {
                                ret.Handle.Close();
                                ret.Handle.Dispose();
                            }

                        }

                        LastLength = FI.Length;

                        ret.ErrRetryCnt += 1;

                        Thread.Sleep(RetryDelayMS);

                        FI.Refresh();
                    }
                    SW.Stop();

                    ret.TimeMS = SW.ElapsedMilliseconds;

                    if (!ret.Success)
                    {
                        ret.ResultString = $"Debug: LastFail={LastFailReason}, lock time: {ret.TimeMS}ms (max={MaxWaitMS}), {ret.ErrRetryCnt} retries (max={MaxErrRetryCnt}) with a {RetryDelayMS}ms retry delay: {Path.GetFileName(filename)}";
                        if (ret.ErrRetryCnt > 2 || ret.TimeMS > 1000)
                        {
                            Log(ret.ResultString);
                        }
                    }

                }
                else
                {
                    ret.ResultString = $"Error: File not found: " + filename;
                    Log(ret.ResultString);
                }

            }
            catch (Exception ex)
            {
                ret.TimeMS = SW.ElapsedMilliseconds;
                ret.ResultString = $"Error: {filename}: {ex.Msg()}";
                Log(ret.ResultString);
            }
            finally
            {
                if (!ReturnHandle && ret.Handle != null && !ret.Handle.IsClosed)
                {
                    ret.Handle.Close();
                    ret.Handle.Dispose();
                }
            }


            return ret;

        }

        public static bool IsFileWritable(string filename, ref SafeFileHandle handle, ref FileInfo FI, out string FailReason)
        {
            bool Ret = false;
            string err = "";
            FailReason = "";

            byte[] TestReadByte;
            try
            {
                if (FI == null)
                    FI = new FileInfo(filename);

                if (FI.IsReadOnly)
                {
                    FailReason = "(ReadOnly)";
                    return false;
                }

                bool Updated = false;

                using (FileStream fs = new FileStream(handle, FileAccess.ReadWrite))
                {

                    using (BinaryReader br = new BinaryReader(fs))
                    {

                        if (br.BaseStream.CanRead)
                        {
                            //read 1 byte
                            TestReadByte = br.ReadBytes(1);
                            fs.Position = 0;
                            using (BinaryWriter bw = new BinaryWriter(fs))
                            {

                                if (bw.BaseStream.CanWrite)
                                {
                                    //write same byte back - get exception if file is in use
                                    bw.Write(TestReadByte);
                                    bw.Flush();
                                    Updated = true;
                                }
                                else
                                {
                                    FailReason = "(CantWrite)";
                                }
                            }
                        }
                        else
                        {
                            FailReason = "(CantRead)";
                        }
                    }
                }

                if (Updated)
                {
                    //reset dates on the file back to what they were before the test write
                    FileInfo DFI = new FileInfo(filename);
                    if (DFI.CreationTime != FI.CreationTime)
                        DFI.CreationTime = FI.CreationTime;  //reset date the same as the old file
                    if (DFI.LastWriteTime != FI.LastWriteTime)
                        DFI.LastWriteTime = FI.LastWriteTime;
                }

                if (String.IsNullOrWhiteSpace(FailReason))
                    Ret = true;

            }
            catch (Exception ex)
            {
                err = "(" + ex.Message + ")";
            }
            FailReason = err;
            return Ret;
        }

        public static bool IsInList(List<string> FindStrList, string SearchList, string Separators = ",;|", bool TrueIfEmpty = true)
        {
            if (TrueIfEmpty && string.IsNullOrWhiteSpace(SearchList))
                return true;  //If there is no searchlist, always return true

            return IsInList(FindStrList, SearchList.SplitStr(Separators, true, true, true));
        }
        [DebuggerStepThrough]
        public static bool IsInList(string FindStr, List<string> SearchList, string Separators = ",;|", bool TrueIfEmpty = true)
        {
            if (TrueIfEmpty && SearchList.Count == 0)
                return true;  //If there is no searchlist, always return true

            return IsInList(FindStr.SplitStr(Separators, true, true, true), SearchList);
        }
        [DebuggerStepThrough]
        public static bool IsInList(string FindStr, string SearchList, string Separators = ",;|", bool TrueIfEmpty = true)
        {
            if (TrueIfEmpty && string.IsNullOrWhiteSpace(SearchList))
                return true;  //If there is no searchlist, always return true


            return IsInList(FindStr.SplitStr(Separators, true, true, true), SearchList.SplitStr(Separators, true, true, true));
        }
        [DebuggerStepThrough]
        public static bool IsInList(List<string> FindStrsList, List<string> SearchList)
        {
            foreach (string findstr in FindStrsList)
            {
                foreach (string searchstr in SearchList)
                {
                    if (findstr.Equals(searchstr, StringComparison.OrdinalIgnoreCase) || searchstr == "*")
                        return true;
                }
            }
            return false;
        }

        [DebuggerStepThrough]
        public static string ConvertToBase64(this Stream stream)
        {
            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
            }

            string base64 = Convert.ToBase64String(bytes);
            return base64;
        }



        public static IEnumerable<Exception> GetAllExceptions(this Exception exception)
        {
            yield return exception;

            if (exception is AggregateException aggrEx)
            {
                foreach (Exception innerEx in aggrEx.InnerExceptions.SelectMany(e => e.GetAllExceptions()))
                {
                    yield return innerEx;
                }
            }
            else if (exception.InnerException != null)
            {
                foreach (Exception innerEx in exception.InnerException.GetAllExceptions())
                {
                    yield return innerEx;
                }
            }
        }

        public static void Startup(bool Enable)
        {
            try
            {

                using (RegistryKey RK = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                {

                    string AppName = Path.GetFileNameWithoutExtension(Application.ExecutablePath);
                    string AppCmd = Application.ExecutablePath + " /min";
                    bool Enabled = false;
                    object CurVal = RK.GetValue(AppName, null);

                    if (CurVal == null || string.IsNullOrWhiteSpace(CurVal.ToString()))
                    {
                        Log("Application is NOT set to start with Windows: HKCU\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");

                        Enabled = false;
                    }
                    else
                    {
                        if (string.Equals(CurVal.ToString(), AppCmd, StringComparison.OrdinalIgnoreCase))
                        {
                            Log("Application is already set to start with Windows: HKCU\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
                            Enabled = true;
                        }
                        else
                        {
                            Log($"Application is NOT set to start with Windows (bad path={CurVal}): HKCU\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
                        }

                    }

                    if (Enable && !Enabled)
                    {
                        Log("Enabling Application startup: " + AppCmd);
                        if (!Debugger.IsAttached)
                        {
                            RK.SetValue(AppName, AppCmd);
                        }
                    }
                    else if (!Enable && Enabled)
                    {
                        Log("Disabling Application startup.");
                        if (!Debugger.IsAttached)
                        {
                            RK.DeleteValue(AppName);
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
            finally
            {
            }
        }




        public static void SerializeJsonIntoStream(object value, Stream stream)
        {
            using (var sw = new StreamWriter(stream, new UTF8Encoding(false), 32768, true))
            using (var jtw = new JsonTextWriter(sw) { Formatting = Formatting.None })
            {
                var js = new JsonSerializer();
                js.Serialize(jtw, value);
                jtw.Flush();
            }
        }

        public static HttpContent CreateHttpContentString(object content)
        {
            HttpContent httpContent = null;

            if (content != null)
            {
                var json = Global.GetJSONString(content); //JsonConvert.SerializeObject(content);
                httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            }

            return httpContent;
        }

        public static HttpContent CreateHttpContentStream(object content)
        {
            HttpContent httpContent = null;

            if (content != null)
            {
                var ms = new MemoryStream();
                SerializeJsonIntoStream(content, ms);
                ms.Seek(0, SeekOrigin.Begin);
                httpContent = new StreamContent(ms);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            }

            return httpContent;
        }



        public static bool IsDirWritable(string FolderName, bool CreateIfNotExist)
        {
            bool Ret = false;
            try
            {
                if (FolderName.IsEmpty())
                    return Ret;

                // if actually passed a filename, get folder
                string pth = FolderName;
                //if (PathEndsWithFilename(FolderName))
                //{
                //    pth = Path.GetDirectoryName(pth);
                //}

                if (!Directory.Exists(pth))
                {
                    if (CreateIfNotExist)
                    {
                        Directory.CreateDirectory(pth);
                        Ret = true;
                        return Ret;
                    }
                    else
                    {
                        return Ret;
                    }
                }

                string Filename = Path.Combine(pth, Path.GetRandomFileName());
                using (var fs = File.Create(Filename, 1, FileOptions.DeleteOnClose))
                {
                }
                // make sure temp file really did get deleted...
                if (File.Exists(Filename))
                {
                    Log("Trace: Failed to remove temp file? " + Filename);
                    File.Delete(Filename);
                }

                Ret = true;
            }
            catch
            {
            }

            return Ret;
        }

        /// <summary>
        /// Writes the given object instance to a Json file.
        /// <para>Object type must have a parameterless constructor.</para>
        /// <para>Only Public properties and variables will be written to the file. These can be any type though, even other classes.</para>
        /// <para>If there are public properties/variables that you do not want written to the file, decorate them with the [JsonIgnore] attribute.</para>
        /// </summary>
        /// <typeparam name="T">The type of object being written to the file.</typeparam>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the file.</param>
        /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
        public static string WriteToJsonFile<T>(string filePath, T objectToWrite, bool append = false) where T : new()
        {
            string Ret = "";
            TextWriter writer = null;
            try
            {

                Ret = JsonConvert.SerializeObject(objectToWrite, JSONSettingsPretty);
                if (JSONSettingsPretty.Error == null)
                {
                    if (Directory.Exists(Path.GetDirectoryName(filePath)))
                    {
                        writer = new StreamWriter(filePath, append);
                        writer.Write(Ret);
                    }
                }
                else
                {
                    Ret = "";
                    Log($"Error: While writing '{filePath}', got: " + JSONSettingsPretty.Error.ToString());
                }
            }
            catch (Exception ex)
            {
                Ret = "";
                Log($"Error: While writing '{filePath}', got: " + ex.Msg());
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
            return Ret;

        }

        /// <summary>
        /// Reads an object instance from an Json file.
        /// <para>Object type must have a parameterless constructor.</para>
        /// </summary>
        /// <typeparam name="T">The type of object to read from the file.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the Json file.</returns>
        public static T ReadFromJsonFile<T>(string filePath) where T : new()
        {


            T Ret = default(T);

            TextReader reader = null;
            try
            {
                reader = new StreamReader(filePath);
                var fileContents = reader.ReadToEnd();

                JsonSerializerSettings jset = new JsonSerializerSettings { };
                jset.TypeNameHandling = TypeNameHandling.All;
                jset.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                jset.ContractResolver = Global.JSONContractResolver;

                Ret = JsonConvert.DeserializeObject<T>(fileContents, jset);
            }
            catch (Exception ex)
            {
                Log($"Error: While reading '{filePath}', got: " + ex.Msg());
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }

            return Ret;

        }

        public static string GetJSONString(object cls2, Newtonsoft.Json.Formatting formatting = Formatting.Indented,
                                                        Newtonsoft.Json.TypeNameHandling handling = TypeNameHandling.All,
                                                        Newtonsoft.Json.PreserveReferencesHandling reference = PreserveReferencesHandling.Objects)
        {

            string Ret = "";
            try
            {

                JSONSettingsPretty.Formatting = formatting;
                JSONSettingsPretty.TypeNameHandling = handling;
                JSONSettingsPretty.PreserveReferencesHandling = reference;
                JSONSettingsPretty.Error = null;

                string contents2 = JsonConvert.SerializeObject(cls2, formatting, JSONSettingsPretty);

                if (JSONSettingsPretty.Error == null)
                {
                    Ret = contents2;
                }
                else
                {
                    Log($"Error: " + JSONSettingsPretty.Error.ToString());
                }

            }
            catch (Exception ex)
            {
                Log($"Error: " + ex.Msg());
            }
            finally
            {
            }

            return Ret;

        }

        public static T SetJSONString<T>(string JSONString) where T : new()
        {


            T Ret = default(T);

            try
            {
                //JsonSerializerSettings jset = new JsonSerializerSettings { };
                //jset.TypeNameHandling = TypeNameHandling.All;
                //jset.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                //jset.ContractResolver = Global.JSONContractResolver;

                Ret = JsonConvert.DeserializeObject<T>(JSONString, JSONSettingsPretty);
            }
            catch (Exception ex)
            {
                Log($"Error: While converting json string '{JSONString}', got: " + ex.Msg());
            }
            finally
            {
            }

            return Ret;

        }
        public static DateTime RetrieveLinkerTimestamp()
        {
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0);

            try
            {
                string filePath = System.Reflection.Assembly.GetCallingAssembly().Location;
                const int c_PeHeaderOffset = 60;
                const int c_LinkerTimestampOffset = 8;
                byte[] b = new byte[2048];
                using (System.IO.Stream s = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {

                    try
                    {
                        //s = New System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read)
                        s.Read(b, 0, 2048);
                    }
                    finally
                    {
                        if (s != null)
                        {
                            s.Close();
                        }
                    }

                    int i = System.BitConverter.ToInt32(b, c_PeHeaderOffset);
                    int secondsSince1970 = System.BitConverter.ToInt32(b, i + c_LinkerTimestampOffset);
                    dt = dt.AddSeconds(secondsSince1970);
                    dt = dt.AddHours(System.TimeZone.CurrentTimeZone.GetUtcOffset(dt).Hours);
                }

            }
            catch (System.Exception ex)
            {
                Log("Error: " + ex.Msg());
            }

            return dt;

        }


        public static DateTime GetTimeFromFileName(string FileName)
        {
            DateTime OutDate = DateTime.MinValue;

            try
            {
                if (string.IsNullOrWhiteSpace(FileName))
                {
                    return OutDate;
                }
                string Fil = Path.GetFileNameWithoutExtension(FileName);
                string StrDate = "";
                MatchCollection Matches = RegEx_ValidDate.Matches(Fil);
                if (Matches != null && Matches.Count > 0)
                {
                    StrDate = Matches[0].Value;
                    StrDate = StrDate.Replace("_", ":");
                    //pos 10=T
                    StrDate = StrDate.Remove(10, 1).Insert(10, "T");
                    if (!GetDateStrict(StrDate, ref OutDate))
                    {
                        Log("Error: There was a problem parsing '" + FileName + "' for a date.");
                        OutDate = DateTime.MinValue;
                    }
                }

            }
            catch (Exception ex)
            {
                Log("Error: " + ex.Msg());
            }

            return OutDate;

        }

        public class ClsDateFormat
        {
            public string Fmt = "";
            public long Cnt = 0;
            public override string ToString()
            {
                return $"Cnt='{this.Cnt}', Fmt='{this.Fmt}'";
            }
        }

        public static long DateFormatHitCnt = 0;

        public static List<ClsDateFormat> DateFormatList = new List<ClsDateFormat>();

        public static void CreateFormatList()
        {
            //28-Feb-2015 17:21:56.155
            //11/8/2012 09:28:33:941
            //15-May-2018 18:19:20.173
            //15-May-2018 18:05:28.457
            //dd-MMMM-yyyy HH:mm:ss.fff
            //7/15/2015 13:10:46:788
            //8/7/2017 13:00:15:330
            //6/14/2016 15:03:01:360
            //2018-04-10T14:32:26
            //yyyy-MM-ddTHH:mm:ss

            //most popular first
            DateFormatList.Add(new ClsDateFormat { Fmt = "dd-MMM-yyyy HH:mm:ss.fff" });
            DateFormatList.Add(new ClsDateFormat { Fmt = "yyyy-MM-dd HH:mm:ss.fff" });
            DateFormatList.Add(new ClsDateFormat { Fmt = "M/d/yyyy HH:mm:ss:fff" });
            DateFormatList.Add(new ClsDateFormat { Fmt = "dd-MMMM-yyyy HH:mm:ss.fff" });
            DateFormatList.Add(new ClsDateFormat { Fmt = "yyyy-MM-ddTHH:mm:ss" });

            DateFormatList.Add(new ClsDateFormat { Fmt = "M/d/yyyy H:mm:ss:fff" });
            DateFormatList.Add(new ClsDateFormat { Fmt = "M/dd/yyyy hh:mm:ss:fff" });
            DateFormatList.Add(new ClsDateFormat { Fmt = "M/dd/yyyy HH:mm:ss:fff" });
            DateFormatList.Add(new ClsDateFormat { Fmt = "M/dd/yyyy H:mm:ss:fff" });
            DateFormatList.Add(new ClsDateFormat { Fmt = "M/dd/yyyy hh:mm:ss:fff" });
            DateFormatList.Add(new ClsDateFormat { Fmt = "M/dd/yyyy HH:mm:ss:fff" });
            DateFormatList.Add(new ClsDateFormat { Fmt = "M/dd/yyyy H:mm:ss:fff" });
            DateFormatList.Add(new ClsDateFormat { Fmt = "MM/dd/yyyy hh:mm:ss:fff" });
            DateFormatList.Add(new ClsDateFormat { Fmt = "MM/dd/yyyy HH:mm:ss:fff" });
            DateFormatList.Add(new ClsDateFormat { Fmt = "MM/dd/yyyy H:mm:ss:fff" });
            DateFormatList.Add(new ClsDateFormat { Fmt = "d-MMMM-yyyy HH:mm:ss.fff" });
            DateFormatList.Add(new ClsDateFormat { Fmt = "d-MMM-yyyy HH:mm:ss.fff" });

        }
        public static bool GetDateStrict(string InpDate, ref DateTime OutDate, string format = "")
        {
            bool Ret = false;
            try
            {
                if (!string.IsNullOrEmpty(format))
                {
                    Ret = DateTime.TryParseExact(InpDate, format, null, System.Globalization.DateTimeStyles.None, out OutDate); //New CultureInfo("en-US")
                    if (Ret)
                        return Ret;
                }

                if (DateFormatList.Count == 0)
                {
                    CreateFormatList();
                }
                for (int i = 0; i < DateFormatList.Count; i++)
                {
                    ClsDateFormat df = DateFormatList[i];
                    Ret = DateTime.TryParseExact(InpDate, df.Fmt, null, System.Globalization.DateTimeStyles.None, out OutDate); //New CultureInfo("en-US")
                    if (Ret)
                    {
                        //First double check that date is in normal-ish range..
                        if (OutDate > new DateTime(2000, 1, 1) && OutDate <= DateTime.Now.AddHours(12))
                        {
                            df.Cnt = df.Cnt + 1;
                            DateFormatHitCnt = DateFormatHitCnt + 1;
                            //If DateFormatHitCnt < 15 OrElse (CurCnt > 1 AndAlso (DateFormatHitCnt Mod 25 = 0)) Then
                            //    'Sort the list by most frequent found cnt
                            //    DateFormatList = DateFormatList.OrderByDescending(Function(d) d.Cnt).ToList
                            //End If                        
                            break;
                        }
                        else
                        {
                            Ret = false;
                        }
                    }

                }

                if (!Ret)
                {
                    //last ditch
                    Ret = DateTime.TryParse(InpDate, out OutDate);
                    if (Ret)
                    {
                        if (OutDate > new DateTime(2010, 1, 1) && OutDate < new DateTime(2050, 1, 1))
                        {
                        }
                        else
                        {
                            Ret = false;
                            OutDate = DateTime.MinValue;
                        }
                    }
                }


            }
            catch (Exception)
            {
                Ret = false;
            }
            return Ret;
        }
        public static bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static Process GetaProcess(string processname)
        {
            try
            {
                if (Path.HasExtension(processname))
                    processname = Path.GetFileNameWithoutExtension(processname);
                Process[] aProc = Process.GetProcessesByName(processname);
                if (aProc.Length > 0)
                    return aProc[0];
                else
                    return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static Process[] GetProcesses(string processname)
        {
            try
            {
                if (Path.HasExtension(processname))
                    processname = Path.GetFileNameWithoutExtension(processname);
                Process[] aProc = Process.GetProcessesByName(processname);
                if (aProc.Length > 0)
                    return aProc;
                else
                    return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public enum ShowWindowEnum:int
        {
            /// <summary>
            ///        Hides the window and activates another window.
            /// </summary>
            SW_HIDE = 0,
            // <summary>
            ///        Activates and displays a window. If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when displaying the window for the first time.
            /// </summary>
            SW_SHOWNORMAL = 1,

            /// <summary>
            ///        Activates the window and displays it as a minimized window.
            /// </summary>
            SW_SHOWMINIMIZED = 2,

            /// <summary>
            ///        Activates the window and displays it as a maximized window.
            /// </summary>
            SW_SHOWMAXIMIZED = 3,

            /// <summary>
            ///        Maximizes the specified window.
            /// </summary>
            SW_MAXIMIZE = 3,

            /// <summary>
            ///        Displays a window in its most recent size and position. This value is similar to <see cref="ShowWindowCommands.SW_SHOWNORMAL"/>, except the window is not activated.
            /// </summary>
            SW_SHOWNOACTIVATE = 4,

            /// <summary>
            ///        Activates the window and displays it in its current size and position.
            /// </summary>
            SW_SHOW = 5,

            /// <summary>
            ///        Minimizes the specified window and activates the next top-level window in the z-order.
            /// </summary>
            SW_MINIMIZE = 6,

            /// <summary>
            ///        Displays the window as a minimized window. This value is similar to <see cref="ShowWindowCommands.SW_SHOWMINIMIZED"/>, except the window is not activated.
            /// </summary>
            SW_SHOWMINNOACTIVE = 7,

            /// <summary>
            ///        Displays the window in its current size and position. This value is similar to <see cref="ShowWindowCommands.SW_SHOW"/>, except the window is not activated.
            /// </summary>
            SW_SHOWNA = 8,

            /// <summary>
            ///        Activates and displays the window. If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when restoring a minimized window.
            /// </summary>
            SW_RESTORE = 9,

            /// <summary>
            ///        Items 10, 11 and 11 existed in the VB definition but not the c# definition - so I am assuming this was a mistake and have added them here.
            ///         Please forgive me if this is wrong!  I don't think it should have any negative impact.
            ///         According to what I have read elsewhere: The SW_SHOWDEFAULT makes sure the window is restored prior to showing, then activating.
            ///         And the 11's try to coerce a window to minimized or maximized.
            /// </summary>
            SW_SHOWDEFAULT = 10,
            SW_FORCEMINIMIZE = 11,
            SW_MAX = 11
        }
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool IsIconic(IntPtr handle);

        [DllImport("User32.dll", SetLastError = true)]
        static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        private extern static IntPtr FindWindow(string lpClassName, string lpWindowName);
        public static bool ShowProcessWindow(string processname, string WindowTitle, ShowWindowEnum WindowStyle)
        {
            bool ret = false;

            Process[] Procs = Global.GetProcesses(processname);
            if (Procs.IsNotEmpty())
            {
                foreach (var proc in Procs)
                {
                    try
                    {
                        if (!proc.HasExited)
                        {
                            bool RefreshFound = false;

                            if (proc.MainWindowHandle.IsNull())
                                proc.Refresh();

                            IntPtr hwnd = proc.MainWindowHandle;

                            if (hwnd.IsNull())
                            {
                                //get the window a different way
                                hwnd = FindWindow(null, WindowTitle);
                            }
                            else
                            {
                                RefreshFound = true;
                            }

                            if (hwnd.IsNotNull())
                            {
                                bool IsIconicResult = IsIconic(hwnd);

                                if (IsIconicResult)  //check if the window is minimized. If it is not, then you don't want to restore it, you only need to activate it.
                                    SwitchToThisWindow(hwnd, true);   //SendMessageW(p.MainWindowHandle, WM_SYSCOMMAND, SC_RESTORE, 0) 'restore the window from it's minimized state

                                //true or nonzero if the window was brought to the foreground,
                                //false or zero If the window was not
                                bool SetForegroundResult = SetForegroundWindow(hwnd);
                                //Return value
                                //If the window was previously visible, the return value is nonzero.
                                //If the window was previously hidden, the return value is zero.
                                bool ShowWindowResult = ShowWindow(hwnd, ((int)WindowStyle));
                                ret = true;
                                Log($"Debug: Set '{processname}' ({proc.Id}, {hwnd.ToString()}) to '{WindowStyle.ToString()}' - RefreshFound = '{RefreshFound}', IsIconicResult = '{IsIconicResult}', SetForgroundWindow result = '{SetForegroundResult}', ShowWindowResult='{ShowWindowResult}'");

                            }
                            else
                            {
                                Log($"Debug: Could not get MainWindowHandle for process '{processname}, {WindowTitle}' ({proc.Id}).  It may be running as a service without GUI?");
                            }

                        }
                        else
                        {
                            Log($"Debug: process not valid '{processname}'.");
                        }

                    }
                    catch (Exception ex)
                    {
                        Log($"Trace: Error working with process: {ex.Msg()}");
                    }

                }
            }
            else
            {
                Log($"Debug: Could not find running process? '{processname}'.");
            }

            return ret;
        }
        public static bool KillProcesses(string ProcessPath)
        {
            List<ClsProcess> prc = GetProcessesByPath(ProcessPath);
            return KillProcesses(prc);
        }

        public static bool KillProcesses(List<ClsProcess> prc)
        {

            int valid = 0;
            int ccnt = 0;
            if (prc != null && prc.Count > 0)
            {
                foreach (ClsProcess curprc in prc)
                {
                    ccnt++;
                    if (ProcessValid(curprc))
                    {
                        try
                        {
                            Log($"Debug: Killing {ccnt} of {prc.Count}: {curprc.FileName}");
                            curprc.process.Kill();
                            valid++;
                        }
                        catch (Exception ex)
                        {

                            Log($"Error: Could not kill process {curprc.FileName}");
                        }
                    }
                    else
                    {
                        Log($"Process no longer valid {curprc.FileName}");

                    }
                }
                if (prc.Count == valid)
                    return true;
                else
                    return false;
            }
            return true;
        }



        public static bool WaitForProcessToStart(Process prc, int TimeoutMS, string fullpath)
        {
            bool ret = false;
            Stopwatch sw = Stopwatch.StartNew();
            try
            {
                if (prc != null && !prc.HasExited)
                {
                    long LastMem = 0;
                    int cnt = 0;
                    //prc.WaitForInputIdle(TimeoutMS); //I think this only works with GUI app
                    prc.Refresh();
                    while (sw.ElapsedMilliseconds <= TimeoutMS && !prc.HasExited && LastMem != prc.PrivateMemorySize64)
                    {
                        cnt++;
                        LastMem = prc.PrivateMemorySize64;
                        System.Threading.Thread.Sleep(AppSettings.Settings.loop_delay_ms);
                        prc.Refresh();
                    }
                    sw.Stop();
                    ret = prc != null && !prc.HasExited;
                    if (ret)
                        Log($"Debug: Waited {sw.ElapsedMilliseconds}ms (cnt={cnt}) for {fullpath} to initialize.");
                    else
                        Log($"Error: Process failed to start in {sw.ElapsedMilliseconds}ms (cnt={cnt}) for {fullpath} to initialize.");

                }
            }
            catch (Exception ex)
            {
                Log($"Error: {ex.Msg()}");
            }
            return ret;

        }
        public static bool WaitForProcessToClose(Process prc, int TimeoutMS, string fullpath)
        {
            bool ret = false;
            Stopwatch sw = Stopwatch.StartNew();
            try
            {
                if (prc != null && !prc.HasExited)
                {
                    int cnt = 0;
                    //prc.WaitForInputIdle(TimeoutMS); //I think this only works with GUI app
                    prc.Refresh();
                    while (sw.ElapsedMilliseconds <= TimeoutMS && !prc.HasExited)
                    {
                        cnt++;
                        System.Threading.Thread.Sleep(AppSettings.Settings.loop_delay_ms);
                        prc.Refresh();
                    }
                    sw.Stop();
                    ret = prc == null || prc.HasExited;
                    int exitcode = 0;
                    if (prc.IsNotNull())
                        exitcode = prc.ExitCode;

                    if (ret)
                        Log($"Debug: Process closed after {sw.ElapsedMilliseconds}ms (cnt={cnt}) for {fullpath} to close with exit code '{exitcode}'");
                    else
                        Log($"Debug: Process was still open after {sw.ElapsedMilliseconds}ms (cnt={cnt}) for {fullpath}");

                }
                else
                {
                    ret = true;  //exited or didnt exist
                }
            }
            catch (Exception ex)
            {
                Log($"Error: {ex.Msg()}");
            }
            return ret;

        }

        public static bool ProcessValid(List<ClsProcess> prc)
        {

            int valid = 0;
            if (prc != null && prc.Count > 0)
            {
                foreach (ClsProcess curprc in prc)
                {
                    if (ProcessValid(curprc))
                        valid++;
                    else
                        break;
                }
                if (prc.Count == valid)
                    return true;
            }
            return false;
        }
        public static bool ProcessValid(ClsProcess prc)
        {

            if (prc != null && prc.process != null)
            {
                try
                {
                    if (!prc.process.HasExited)
                    {
                        //if (!string.IsNullOrEmpty(prc.CommandLine) || !string.IsNullOrEmpty(prc.process.StartInfo.Arguments))
                        //{
                        return true;
                        //}
                    }
                }
                catch { }
            }
            return false;
        }

        public static List<ClsProcess> GetProcessesByPath(string processname, bool ExcludeCurrentProcess = false)
        {
            List<ClsProcess> Ret = new List<ClsProcess>();
            try
            {
                string pname = Path.GetFileNameWithoutExtension(processname);

                Process[] aProc = Process.GetProcessesByName(pname);

                Process cprc = null;
                if (ExcludeCurrentProcess)
                    cprc = Process.GetCurrentProcess();

                ProcessDetail PD = null;

                if (aProc.Length > 0)

                {
                    foreach (Process curproc in aProc)
                    {
                        if (ExcludeCurrentProcess && cprc.Id == curproc.Id)
                            continue;

                        //accessing 64 bit process from 32 bit app may not allow to get process properties, only name
                        //Stopwatch SW = Stopwatch.StartNew();
                        ClsProcess CurPrc = new ClsProcess();

                        try
                        {
                            //if (IsAdministrator())
                            //{
                            //    Process.EnterDebugMode();
                            //}
                            PD = new ProcessDetail(curproc.Id);
                            CurPrc.FileName = PD.Win32ProcessImagePath;
                            //Todo: This is not working for some reason, even with admin rights
                            //if (IsAdministrator())
                            //{
                            //    Ret.CommandLine = PD.CommandLine;  //.Replace((char)34,"");
                            //}
                        }
                        catch
                        {
                            CurPrc = null;
                        }

                        //asdf
                        //if (string.IsNullOrEmpty(Ret.CommandLine))
                        //{
                        //    //Having trouble obtaining the command line?
                        //    //Log($"Cannot get command line for '{curproc.ProcessName}', must be running as administrator.");
                        //}

                        if (Ret != null && CurPrc != null && !string.IsNullOrEmpty(CurPrc.FileName) && string.Equals(CurPrc.FileName, processname, StringComparison.OrdinalIgnoreCase))
                        {
                            CurPrc.process = curproc;
                            Ret.Add(CurPrc);
                        }
                        else
                        {
                            CurPrc = null;
                        }
                    }

                }
            }
            catch
            {
                Ret = new List<ClsProcess>();
            }

            return Ret;
        }

        public static ClsProcess GetaProcessByPath(string processname)
        {
            ClsProcess Ret = null;
            try
            {
                string pname = Path.GetFileNameWithoutExtension(processname);

                Process[] aProc = Process.GetProcessesByName(pname);

                ProcessDetail PD = null;

                if (aProc.Length > 0)

                {
                    foreach (Process curproc in aProc)
                    {
                        //accessing 64 bit process from 32 bit app may not allow to get process properties, only name
                        //Stopwatch SW = Stopwatch.StartNew();
                        Ret = new ClsProcess();

                        try
                        {
                            //if (IsAdministrator())
                            //{
                            //    Process.EnterDebugMode();
                            //}
                            PD = new ProcessDetail(curproc.Id);
                            Ret.FileName = PD.Win32ProcessImagePath;
                            //Todo: This is not working for some reason, even with admin rights
                            //if (IsAdministrator())
                            //{
                            //    Ret.CommandLine = PD.CommandLine;  //.Replace((char)34,"");
                            //}
                        }
                        catch
                        {
                            Ret = null;
                        }

                        //asdf
                        //if (string.IsNullOrEmpty(Ret.CommandLine))
                        //{
                        //    //Having trouble obtaining the command line?
                        //    //Log($"Cannot get command line for '{curproc.ProcessName}', must be running as administrator.");
                        //}

                        if (Ret != null && !string.IsNullOrEmpty(Ret.FileName) && Ret.FileName.ToLower() == processname.ToLower())
                        {
                            Ret.process = curproc;
                            break;
                        }
                        else
                        {
                            Ret = null;
                        }
                    }

                }
            }
            catch
            {
                Ret = null;
            }

            return Ret;
        }

        public static bool IsProcessRunning(string ProcessName)
        {
            bool Ret = false;

            try
            {
                Process Proc = GetaProcess(ProcessName);

                if (Proc == null)
                    Log($"Process is not running: '{ProcessName}'.");
                else
                {
                    Log($"Process IS running: '{ProcessName}'.");
                    Ret = true;
                }
            }
            catch (Exception ex)
            {
                Log("Error: " + ex.Msg());
            }

            return Ret;
        }



        public static string GetWMIPropertyFromProcess(Int32 PID, string PropName)
        {

            // THIS IS SLOW AS FUCK

            // .net PROCESS object cannot seem to get the command line from processes that we did not start
            // resort to using WMI
            //https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-process
            //
            //CommandLine, ExecutablePath, ProcessId
            string Ret = "";
            try
            {
                Stopwatch SW = Stopwatch.StartNew();

                //method 2 - 400ms faster:

                string wmiQuery = $"select ProcessId, CommandLine, ExecutablePath from Win32_Process where ProcessId='{Convert.ToUInt32(PID)}'";
                using (ManagementObjectSearcher search = new ManagementObjectSearcher(wmiQuery))
                {

                    // By definition, the query returns at most 1 match, because the process 
                    // is looked up by ID (which is unique by definition).
                    using (var matchEnum = search.Get().GetEnumerator())
                    {
                        if (matchEnum.MoveNext()) // Move to the 1st item.
                        {
                            object obj = matchEnum.Current[PropName];

                            if (obj == null)
                                obj = "";

                            Ret = obj.ToString();

                        }
                    }

                    //ManagementObjectCollection processList = search.Get();
                    //foreach (ManagementObject process in processList)
                    //{
                    //    //Log("{0,6} - {1} - {2}", process["ProcessId"], process["Name"], process["CommandLine"]);
                    //    object obj = process[PropName];

                    //    if (obj == null)
                    //        obj = "";

                    //    Ret = obj.ToString();

                    //    break;

                    //}
                }

                //Method 1 - 450 ms slowest
                //ManagementClass mgmtClass = new ManagementClass("Win32_Process");
                //foreach (ManagementObject process in mgmtClass.GetInstances())
                //{
                //    // Get pid
                //    UInt32 CurPID = (System.UInt32)process["ProcessId"];

                //    if (CurPID == Convert.ToUInt32(PID))
                //    {
                //        object obj = process[PropName];

                //        if (obj == null)
                //            obj = "";

                //        Ret = obj.ToString();

                //        break;
                //    }

                //}

                SW.Stop();
                Log($"Got '{PropName}' for PID '{PID}' in '{SW.ElapsedMilliseconds}'ms: {Ret}");

            }
            catch (Exception ex)
            {

                Log("Error: Could not get command line: " + ex.Msg());
            }
            return Ret;
        }

        //I had to make this class since win32 apps cant access 64 bit command line and module info
        public class ClsProcess:IEquatable<ClsProcess>
        {
            public Process process = null;
            public string FileName = "";
            public string CommandLine = "";
            public ClsProcess()
            {
                this.process = new Process();
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as ClsProcess);
            }

            public bool Equals(ClsProcess other)
            {
                return other != null &&
                       string.Equals(FileName, other.FileName, StringComparison.OrdinalIgnoreCase) &&
                       string.Equals(CommandLine, other.CommandLine, StringComparison.OrdinalIgnoreCase);
            }

            public override string ToString()
            {
                return $"{Path.GetFileName(this.FileName)} {this.CommandLine}";
            }

            public static bool operator ==(ClsProcess left, ClsProcess right)
            {
                return EqualityComparer<ClsProcess>.Default.Equals(left, right);
            }

            public static bool operator !=(ClsProcess left, ClsProcess right)
            {
                return !(left == right);
            }
        }





        public static string GetXValue(XElement XE, string Name, string AttributeName = "")
        {
            string Ret = "";
            try
            {
                if (XE.HasElements)
                {
                    if (XE.Element(Name) != null)
                    {
                        if (string.IsNullOrEmpty(AttributeName))
                        {
                            Ret = XE.Element(Name).Value;
                        }
                        else
                        {
                            if (XE.Element(Name).HasAttributes)
                            {
                                if (XE.Element(Name).Attribute(AttributeName) != null)
                                {
                                    Ret = XE.Element(Name).Attribute(AttributeName).Value;
                                }
                                else
                                {
                                    //M.DbgLog($"Warning: Attribute not found '{AttributeName}' for Element '{Name}'.");
                                }
                            }
                            else
                            {
                                //M.DbgLog($"Warning: Attribute not found '{AttributeName}' for Element '{Name}'.");
                            }
                        }
                    }
                    else
                    {
                        //M.DbgLog($"Warning: Elements not found '{Name}'.");
                    }
                }
                else
                {
                    //M.Log($"Error: No elements found While getting '{Name}'.");
                }
            }
            catch (Exception)
            {
                //M.Log($"Error: While getting '{Name}', got error '{ex.Message}'.");
            }
            return Ret;
        }

        public static List<System.IO.FileInfo> GetFiles(string CurDirectory, string FileName = "*", System.IO.SearchOption SearchOptions = System.IO.SearchOption.AllDirectories, DateTime? MinLastWriteTime = null, DateTime? MaxLastWriteTime = null, int MaxFiles = 99999)
        {

            List<System.IO.FileInfo> files = new List<System.IO.FileInfo>();
            try
            {
                //If Directory.Exists(CurDirectory) Then
                List<string> Folders = CurDirectory.SplitStr(";|");
                List<string> Names = FileName.SplitStr(";|");
                bool HasDate = MinLastWriteTime.HasValue;
                foreach (string fld in Folders)
                {
                    string fldr = fld;

                    //so we can be lazy and pass filenames...
                    if (Path.HasExtension(fldr))
                        fldr = Path.GetDirectoryName(fldr);

                    if (Directory.Exists(fldr))
                    {
                        DirectoryInfo DirInfo = new DirectoryInfo(fldr);
                        foreach (string nam in Names)
                        {
                            //M.DbgLog($"Getting '{nam}' files from folder '{fldr}'...");
                            try
                            {
                                foreach (FileInfo fi in DirInfo.EnumerateFiles(nam, SearchOptions))
                                {
                                    if (HasDate)
                                    {
                                        if (MaxLastWriteTime.HasValue)
                                        {
                                            if (fi.LastWriteTime >= MinLastWriteTime.Value && fi.LastWriteTime <= MaxLastWriteTime)
                                            {
                                                if (files.Count < MaxFiles)
                                                    files.Add(fi);
                                                else
                                                    break;
                                            }

                                        }
                                        else
                                        {
                                            if (fi.LastWriteTime == MinLastWriteTime.Value)
                                            {
                                                if (files.Count < MaxFiles)
                                                    files.Add(fi);
                                                else
                                                    break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (files.Count < MaxFiles)
                                            files.Add(fi);
                                        else
                                            break;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Log("Error: While getting file list, received error: " + ex.Msg());
                            }
                        }

                    }
                    else
                    {
                        Log($"Debug: Directory doesn't exist: '{fldr}' - ({FileName})");
                    }
                }
                //files.AddRange(IO.Directory.GetFiles(CurDirectory, nam, SearchOptions).Select(Function(p) New IO.FileInfo(p)).ToList)
                //M.DbgLog("Found " & files.Count & " " & FileName & " files in " & CurDirectory)
                //Else
                //M.DbgLog("Error: Folder does not exist: " & CurDirectory)
                //End If

            }
            catch (Exception ex)
            {
                Log("Error: While getting file list, received error: " + ex.Msg());
            }
            finally
            {
            }

            return files;

        }

    }

    internal static class Utility
    {
        public static string[] SplitArgs(string unsplitArgumentLine)
        {
            if (unsplitArgumentLine == null)
                return new string[0];

            int numberOfArgs;
            IntPtr ptrToSplitArgs;
            string[] splitArgs;

            ptrToSplitArgs = NativeMethods.CommandLineToArgvW(unsplitArgumentLine, out numberOfArgs);

            // CommandLineToArgvW returns NULL upon failure.
            if (ptrToSplitArgs == IntPtr.Zero)
                throw new ArgumentException("Unable to split argument.", new Win32Exception());

            // Make sure the memory ptrToSplitArgs to is freed, even upon failure.
            try
            {
                splitArgs = new string[numberOfArgs];

                // ptrToSplitArgs is an array of pointers to null terminated Unicode strings.
                // Copy each of these strings into our split argument array.
                for (int i = 0; i < numberOfArgs; i++)
                    splitArgs[i] = Marshal.PtrToStringUni(
                        Marshal.ReadIntPtr(ptrToSplitArgs, i * IntPtr.Size));

                return splitArgs;
            }
            finally
            {
                // Free memory obtained by CommandLineToArgW.
                NativeMethods.LocalFree(ptrToSplitArgs);
            }
        }

        public static T ReadUnmanagedStructFromProcess<T>(IntPtr processHandle,
                                                          IntPtr addressInProcess)
        {
            int bytesRead;
            int bytesToRead = Marshal.SizeOf(typeof(T));
            IntPtr buffer = Marshal.AllocHGlobal(bytesToRead);
            if (!NativeMethods.ReadProcessMemory(processHandle, addressInProcess, buffer, bytesToRead,
                    out bytesRead))
                throw new Win32Exception();
            T result = (T)Marshal.PtrToStructure(buffer, typeof(T));
            Marshal.FreeHGlobal(buffer);
            return result;
        }

        public static string ReadStringUniFromProcess(IntPtr processHandle,
                                                      IntPtr addressInProcess,
                                                      int NumChars)
        {
            int bytesRead;
            IntPtr outBuffer = Marshal.AllocHGlobal(NumChars * 2);

            bool bresult = NativeMethods.ReadProcessMemory(processHandle,
                                                           addressInProcess,
                                                           outBuffer,
                                                           NumChars * 2,
                                                           out bytesRead);
            if (!bresult)
                throw new Win32Exception();

            string result = Marshal.PtrToStringUni(outBuffer, bytesRead / 2);
            Marshal.FreeHGlobal(outBuffer);
            return result;
        }

        public static int UnmanagedStructSize<T>()
        {
            return Marshal.SizeOf(typeof(T));
        }
    }
    public static class LowLevelTypes
    {

        #region Constants and Enums
        // Represents the image format of a DLL or executable.
        public enum ImageFormat
        {
            NATIVE,
            MANAGED,
            UNKNOWN
        }

        // Flags used for opening a file handle (e.g. in a call to CreateFile), that determine the
        // requested permission level.
        [Flags]
        public enum FileAccessFlags:uint
        {
            GENERIC_WRITE = 0x40000000,
            GENERIC_READ = 0x80000000
        }

        // Value used for CreateFile to determine how to behave in the presence (or absence) of a
        // file with the requested name.  Used only for CreateFile.
        public enum FileCreationDisposition:uint
        {
            CREATE_NEW = 1,
            CREATE_ALWAYS = 2,
            OPEN_EXISTING = 3,
            OPEN_ALWAYS = 4,
            TRUNCATE_EXISTING = 5
        }

        // Flags that determine what level of sharing this application requests on the target file.
        // Used only for CreateFile.
        [Flags]
        public enum FileShareFlags:uint
        {
            EXCLUSIVE_ACCESS = 0x0,
            SHARE_READ = 0x1,
            SHARE_WRITE = 0x2,
            SHARE_DELETE = 0x4
        }

        // Flags that control caching and other behavior of the underlying file object.  Used only for
        // CreateFile.
        [Flags]
        public enum FileFlagsAndAttributes:uint
        {
            NORMAL = 0x80,
            OPEN_REPARSE_POINT = 0x200000,
            SEQUENTIAL_SCAN = 0x8000000,
            RANDOM_ACCESS = 0x10000000,
            NO_BUFFERING = 0x20000000,
            OVERLAPPED = 0x40000000
        }

        // The target architecture of a given executable image.  The various values correspond to the
        // magic numbers defined by the PE Executable Image File Format.
        // http://www.microsoft.com/whdc/system/platform/firmware/PECOFF.mspx
        public enum MachineType:ushort
        {
            UNKNOWN = 0x0,
            X64 = 0x8664,
            X86 = 0x14c,
            IA64 = 0x200
        }

        // A flag indicating the format of the path string that Windows returns from a call to
        // QueryFullProcessImageName().
        public enum ProcessQueryImageNameMode:uint
        {
            WIN32_FORMAT = 0,
            NATIVE_SYSTEM_FORMAT = 1
        }

        // Flags indicating the level of permission requested when opening a handle to an external
        // process.  Used by OpenProcess().
        [Flags]
        public enum ProcessAccessFlags:uint
        {
            NONE = 0x0,
            ALL = 0x001F0FFF,
            VM_OPERATION = 0x00000008,
            VM_READ = 0x00000010,
            QUERY_INFORMATION = 0x00000400,
            QUERY_LIMITED_INFORMATION = 0x00001000
        }

        // Defines return value codes used by various Win32 System APIs.
        public enum NTSTATUS:int
        {
            SUCCESS = 0,
        }

        // Determines the amount of information requested (and hence the type of structure returned)
        // by a call to NtQueryInformationProcess.
        public enum PROCESSINFOCLASS:int
        {
            PROCESS_BASIC_INFORMATION = 0
        };

        [Flags]
        public enum SHGFI:uint
        {
            Icon = 0x000000100,
            DisplayName = 0x000000200,
            TypeName = 0x000000400,
            Attributes = 0x000000800,
            IconLocation = 0x000001000,
            ExeType = 0x000002000,
            SysIconIndex = 0x000004000,
            LinkOverlay = 0x000008000,
            Selected = 0x000010000,
            Attr_Specified = 0x000020000,
            LargeIcon = 0x000000000,
            SmallIcon = 0x000000001,
            OpenIcon = 0x000000002,
            ShellIconSize = 0x000000004,
            PIDL = 0x000000008,
            UseFileAttributes = 0x000000010,
            AddOverlays = 0x000000020,
            OverlayIndex = 0x000000040,
        }
        #endregion

        #region Structures
        // In general, for all structures below which contains a pointer (represented here by IntPtr),
        // the pointers refer to memory in the address space of the process from which the original
        // structure was read.  While this seems obvious, it means we cannot provide an elegant
        // interface to the various fields in the structure due to the de-reference requiring a
        // handle to the target process.  Instead, that functionality needs to be provided at a
        // higher level.
        //
        // Additionally, since we usually explicitly define the fields that we're interested in along
        // with their respective offsets, we frequently specify the exact size of the native structure.

        // Win32 UNICODE_STRING structure.
        [StructLayout(LayoutKind.Sequential)]
        public struct UNICODE_STRING
        {
            // The length in bytes of the string pointed to by buffer, not including the null-terminator.
            private ushort length;
            // The total allocated size in memory pointed to by buffer.
            private ushort maximumLength;
            // A pointer to the buffer containing the string data.
            private IntPtr buffer;

            public ushort Length { get { return this.length; } }
            public ushort MaximumLength { get { return this.maximumLength; } }
            public IntPtr Buffer { get { return this.buffer; } }
        }

        // Win32 RTL_USER_PROCESS_PARAMETERS structure.
        [StructLayout(LayoutKind.Explicit, Size = 72)]
        public struct RTL_USER_PROCESS_PARAMETERS
        {
            [FieldOffset(56)]
            private UNICODE_STRING imagePathName;
            [FieldOffset(64)]
            private UNICODE_STRING commandLine;

            public UNICODE_STRING ImagePathName { get { return this.imagePathName; } }
            public UNICODE_STRING CommandLine { get { return this.commandLine; } }
        };

        // Win32 PEB structure.  Represents the process environment block of a process.
        [StructLayout(LayoutKind.Explicit, Size = 472)]
        public struct PEB
        {
            [FieldOffset(2), MarshalAs(UnmanagedType.U1)]
            private bool isBeingDebugged;
            [FieldOffset(12)]
            private IntPtr ldr;
            [FieldOffset(16)]
            private IntPtr processParameters;
            [FieldOffset(468)]
            private uint sessionId;

            public bool IsBeingDebugged { get { return this.isBeingDebugged; } }
            public IntPtr Ldr { get { return this.ldr; } }
            public IntPtr ProcessParameters { get { return this.processParameters; } }
            public uint SessionId { get { return this.sessionId; } }
        };

        // Win32 PROCESS_BASIC_INFORMATION.  Contains a pointer to the PEB, and various other
        // information about a process.
        [StructLayout(LayoutKind.Explicit, Size = 24)]
        public struct PROCESS_BASIC_INFORMATION
        {
            [FieldOffset(4)]
            private IntPtr pebBaseAddress;
            [FieldOffset(16)]
            private UIntPtr uniqueProcessId;

            public IntPtr PebBaseAddress { get { return this.pebBaseAddress; } }
            public UIntPtr UniqueProcessId { get { return this.uniqueProcessId; } }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SHFILEINFO
        {
            // C# doesn't support overriding the default constructor of value types, so we need to use
            // a dummy constructor.
            public SHFILEINFO(bool dummy)
            {
                this.hIcon = IntPtr.Zero;
                this.iIcon = 0;
                this.dwAttributes = 0;
                this.szDisplayName = "";
                this.szTypeName = "";
            }
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };
        #endregion
    }
    public static class NativeMethods
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ReadProcessMemory(IntPtr hProcess,
                                                    IntPtr lpBaseAddress,
                                                    IntPtr lpBuffer,
                                                    int dwSize,
                                                    out int lpNumberOfBytesRead);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern LowLevelTypes.NTSTATUS NtQueryInformationProcess(
            IntPtr hProcess,
            LowLevelTypes.PROCESSINFOCLASS pic,
            ref LowLevelTypes.PROCESS_BASIC_INFORMATION pbi,
            int cb,
            out int pSize);

        [DllImport("shell32.dll", SetLastError = true)]
        public static extern IntPtr CommandLineToArgvW(
            [MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine,
            out int pNumArgs);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LocalFree(IntPtr hMem);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(
            LowLevelTypes.ProcessAccessFlags dwDesiredAccess,
            [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle,
            int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.StdCall,
            CharSet = CharSet.Unicode)]
        public static extern uint QueryFullProcessImageName(
            IntPtr hProcess,
            [MarshalAs(UnmanagedType.U4)] LowLevelTypes.ProcessQueryImageNameMode flags,
            [Out] StringBuilder lpImageName, ref int size);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern SafeFileHandle CreateFile(string lpFileName,
                                                       LowLevelTypes.FileAccessFlags dwDesiredAccess,
                                                       LowLevelTypes.FileShareFlags dwShareMode,
                                                       IntPtr lpSecurityAttributes,
                                                       LowLevelTypes.FileCreationDisposition dwDisp,
                                                       LowLevelTypes.FileFlagsAndAttributes dwFlags,
                                                       IntPtr hTemplateFile);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr SHGetFileInfo(string pszPath,
                                                  uint dwFileAttributes,
                                                  ref LowLevelTypes.SHFILEINFO psfi,
                                                  uint cbFileInfo,
                                                  uint uFlags);
    }
    internal class ProcessDetail:IDisposable
    {


        public ProcessDetail(int pid)
        {
            // Initialize everything to null in case something fails.
            this.processId = pid;
            this.processHandleFlags = LowLevelTypes.ProcessAccessFlags.NONE;
            this.cachedProcessBasicInfo = null;
            this.machineTypeIsLoaded = false;
            this.machineType = LowLevelTypes.MachineType.UNKNOWN;
            this.cachedPeb = null;
            this.cachedProcessParams = null;
            this.cachedCommandLine = null;
            this.processHandle = IntPtr.Zero;

            this.OpenAndCacheProcessHandle();
        }

        // Returns the machine type (x86, x64, etc) of this process.  Uses lazy evaluation and caches
        // the result.
        public LowLevelTypes.MachineType MachineType
        {
            get
            {
                if (this.machineTypeIsLoaded)
                    return this.machineType;
                if (!this.CanQueryProcessInformation)
                    return LowLevelTypes.MachineType.UNKNOWN;

                this.CacheMachineType();
                return this.machineType;
            }
        }

        public string NativeProcessImagePath
        {
            get
            {
                if (this.nativeProcessImagePath == null)
                {
                    this.nativeProcessImagePath = this.QueryProcessImageName(
                        LowLevelTypes.ProcessQueryImageNameMode.NATIVE_SYSTEM_FORMAT);
                }
                return this.nativeProcessImagePath;
            }
        }

        public string Win32ProcessImagePath
        {
            get
            {
                if (this.win32ProcessImagePath == null)
                {
                    this.win32ProcessImagePath = this.QueryProcessImageName(
                        LowLevelTypes.ProcessQueryImageNameMode.WIN32_FORMAT);
                }
                return this.win32ProcessImagePath;
            }
        }

        //public Icon SmallIcon
        //{
        //    get
        //    {
        //        LowLevelTypes.SHFILEINFO info = new LowLevelTypes.SHFILEINFO(true);
        //        LowLevelTypes.SHGFI flags = LowLevelTypes.SHGFI.Icon
        //                                             | LowLevelTypes.SHGFI.SmallIcon
        //                                             | LowLevelTypes.SHGFI.OpenIcon
        //                                             | LowLevelTypes.SHGFI.UseFileAttributes;
        //        int cbFileInfo = Marshal.SizeOf(info);
        //        NativeMethods.SHGetFileInfo(Win32ProcessImagePath,
        //                                             256,
        //                                             ref info,
        //                                             (uint)cbFileInfo,
        //                                             (uint)flags);
        //        return Icon.FromHandle(info.hIcon);
        //    }
        //}

        // Returns the command line that this process was launched with.  Uses lazy evaluation and
        // caches the result.  Reads the command line from the PEB of the running process.
        public string CommandLine
        {
            get
            {
                if (!this.CanReadPeb)
                    return ""; //throw new InvalidOperationException();
                this.CacheProcessInformation();
                this.CachePeb();
                this.CacheProcessParams();
                this.CacheCommandLine();
                return this.cachedCommandLine;
            }
        }

        // Determines if we have permission to read the process's PEB.
        public bool CanReadPeb
        {
            get
            {
                LowLevelTypes.ProcessAccessFlags required_flags =
                    LowLevelTypes.ProcessAccessFlags.VM_READ
                  | LowLevelTypes.ProcessAccessFlags.QUERY_INFORMATION;

                // In order to read the PEB, we must have *both* of these flags.
                if ((this.processHandleFlags & required_flags) != required_flags)
                    return false;

                // If we're on a 64-bit OS, in a 32-bit process, and the target process is not 32-bit,
                // we can't read its PEB.
                if (Environment.Is64BitOperatingSystem && !Environment.Is64BitProcess
                    && (this.MachineType != LowLevelTypes.MachineType.X86))
                    return false;

                return true;
            }
        }

        // If we can't read the process's PEB, we may still be able to get other kinds of information
        // from the process.  This flag determines if we can get lesser information.
        private bool CanQueryProcessInformation
        {
            get
            {
                LowLevelTypes.ProcessAccessFlags required_flags =
                    LowLevelTypes.ProcessAccessFlags.QUERY_LIMITED_INFORMATION
                  | LowLevelTypes.ProcessAccessFlags.QUERY_INFORMATION;

                // In order to query the process, we need *either* of these flags.
                return (this.processHandleFlags & required_flags) != LowLevelTypes.ProcessAccessFlags.NONE;
            }
        }

        private string QueryProcessImageName(LowLevelTypes.ProcessQueryImageNameMode mode)
        {
            StringBuilder moduleBuffer = new StringBuilder(1024);
            int size = moduleBuffer.Capacity;
            NativeMethods.QueryFullProcessImageName(
                this.processHandle,
                mode,
                moduleBuffer,
                ref size);
            if (mode == LowLevelTypes.ProcessQueryImageNameMode.NATIVE_SYSTEM_FORMAT)
                moduleBuffer.Insert(0, "\\\\?\\GLOBALROOT");
            return moduleBuffer.ToString();
        }

        // Loads the top-level structure of the process's information block and caches it.
        private void CacheProcessInformation()
        {
            System.Diagnostics.Debug.Assert(this.CanReadPeb);

            // Fetch the process info and set the fields.
            LowLevelTypes.PROCESS_BASIC_INFORMATION temp = new LowLevelTypes.PROCESS_BASIC_INFORMATION();
            int size;
            LowLevelTypes.NTSTATUS status = NativeMethods.NtQueryInformationProcess(
                this.processHandle,
                LowLevelTypes.PROCESSINFOCLASS.PROCESS_BASIC_INFORMATION,
                ref temp,
                Utility.UnmanagedStructSize<LowLevelTypes.PROCESS_BASIC_INFORMATION>(),
                out size);

            if (status != LowLevelTypes.NTSTATUS.SUCCESS)
            {
                //-1073741820 = STATUS_INFO_LENGTH_MISMATCH  - Probably a 64 bit process accessing 32 bit process memory related error
                throw new Win32Exception(Convert.ToInt32(status));
            }

            this.cachedProcessBasicInfo = temp;
        }

        // Follows a pointer from the PROCESS_BASIC_INFORMATION structure in the target process's
        // address space to read the PEB.
        private void CachePeb()
        {
            System.Diagnostics.Debug.Assert(this.CanReadPeb);

            if (this.cachedPeb == null)
            {
                this.cachedPeb = Utility.ReadUnmanagedStructFromProcess<LowLevelTypes.PEB>(
                    this.processHandle,
                    this.cachedProcessBasicInfo.Value.PebBaseAddress);
            }
        }

        // Follows a pointer from the PEB structure in the target process's address space to read the
        // RTL_USER_PROCESS_PARAMETERS structure.
        private void CacheProcessParams()
        {
            System.Diagnostics.Debug.Assert(this.CanReadPeb);

            if (this.cachedProcessParams == null)
            {
                this.cachedProcessParams =
                    Utility.ReadUnmanagedStructFromProcess<LowLevelTypes.RTL_USER_PROCESS_PARAMETERS>(
                        this.processHandle, this.cachedPeb.Value.ProcessParameters);
            }
        }

        private void CacheCommandLine()
        {
            System.Diagnostics.Debug.Assert(this.CanReadPeb);

            if (this.cachedCommandLine == null)
            {
                this.cachedCommandLine = Utility.ReadStringUniFromProcess(
                    this.processHandle,
                    this.cachedProcessParams.Value.CommandLine.Buffer,
                    this.cachedProcessParams.Value.CommandLine.Length / 2);
            }
        }

        private void CacheMachineType()
        {
            System.Diagnostics.Debug.Assert(this.CanQueryProcessInformation);

            // If our extension is running in a 32-bit process (which it is), then attempts to access
            // files in C:\windows\system (and a few other files) will redirect to C:\Windows\SysWOW64
            // and we will mistakenly think that the image file is a 32-bit image.  The way around this
            // is to use a native system format path, of the form:
            //    \\?\GLOBALROOT\Device\HarddiskVolume0\Windows\System\foo.dat
            // NativeProcessImagePath gives us the full process image path in the desired format.
            string path = this.NativeProcessImagePath;

            // Open the PE File as a binary file, and parse just enough information to determine the
            // machine type.
            //http://www.microsoft.com/whdc/system/platform/firmware/PECOFF.mspx
            using (SafeFileHandle safeHandle = NativeMethods.CreateFile(
                       path,
                       LowLevelTypes.FileAccessFlags.GENERIC_READ,
                       LowLevelTypes.FileShareFlags.SHARE_READ,
                       IntPtr.Zero,
                       LowLevelTypes.FileCreationDisposition.OPEN_EXISTING,
                       LowLevelTypes.FileFlagsAndAttributes.NORMAL,
                       IntPtr.Zero))
            {
                FileStream fs = new FileStream(safeHandle, FileAccess.Read);
                using (BinaryReader br = new BinaryReader(fs))
                {
                    fs.Seek(0x3c, SeekOrigin.Begin);
                    Int32 peOffset = br.ReadInt32();
                    fs.Seek(peOffset, SeekOrigin.Begin);
                    UInt32 peHead = br.ReadUInt32();
                    if (peHead != 0x00004550) // "PE\0\0", little-endian
                        throw new Exception("Can't find PE header");
                    this.machineType = (LowLevelTypes.MachineType)br.ReadUInt16();
                    this.machineTypeIsLoaded = true;
                }
            }
        }

        private void OpenAndCacheProcessHandle()
        {
            // Try to open a handle to the process with the highest level of privilege, but if we can't
            // do that then fallback to requesting access with a lower privilege level.
            this.processHandleFlags = LowLevelTypes.ProcessAccessFlags.QUERY_INFORMATION
                               | LowLevelTypes.ProcessAccessFlags.VM_READ;
            this.processHandle = NativeMethods.OpenProcess(this.processHandleFlags, false, this.processId);
            if (this.processHandle == IntPtr.Zero)
            {
                this.processHandleFlags = LowLevelTypes.ProcessAccessFlags.QUERY_LIMITED_INFORMATION;
                this.processHandle = NativeMethods.OpenProcess(this.processHandleFlags, false, this.processId);
                if (this.processHandle == IntPtr.Zero)
                {
                    this.processHandleFlags = LowLevelTypes.ProcessAccessFlags.NONE;
                    throw new Win32Exception();
                }
            }
        }

        // An open handle to the process, along with the set of access flags that the handle was
        // open with.
        private int processId;
        private IntPtr processHandle;
        private LowLevelTypes.ProcessAccessFlags processHandleFlags;
        private string nativeProcessImagePath;
        private string win32ProcessImagePath;

        // The machine type is read by parsing the PE image file of the running process, so we cache
        // its value since the operation expensive.
        private bool machineTypeIsLoaded;
        private LowLevelTypes.MachineType machineType;

        // The following fields exist ultimately so that we can access the command line.  However,
        // each field must be read separately through a pointer into another process's address
        // space so the access is expensive, hence we cache the values.
        private Nullable<LowLevelTypes.PROCESS_BASIC_INFORMATION> cachedProcessBasicInfo;
        private Nullable<LowLevelTypes.PEB> cachedPeb;
        private Nullable<LowLevelTypes.RTL_USER_PROCESS_PARAMETERS> cachedProcessParams;
        private string cachedCommandLine;

        ~ProcessDetail()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            if (this.processHandle != IntPtr.Zero)
                NativeMethods.CloseHandle(this.processHandle);
            this.processHandle = IntPtr.Zero;
        }
    }



}

