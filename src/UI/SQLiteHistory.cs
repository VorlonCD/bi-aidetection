using SQLite;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Telegram.Bot.Requests;

using static AITool.AITOOL;


namespace AITool
{
    //this is mainly so we only need one thread for adding and deleting
    public class DBQueueHistoryItem
    {
        public bool add = false;  //otherwise delete
        public History hist = new History();
        public DBQueueHistoryItem(History hist, bool add)
        {
            this.add = add;
            this.hist = hist;
        }
    }
    public class SQLiteHistory:IDisposable
    {
        private bool disposedValue;

        public string Filename { get; } = "";
        public bool ReadOnly { get; } = false;
        public bool IsNew { get; } = false;
        public ConcurrentDictionary<string, History> HistoryDic { get; } = new ConcurrentDictionary<string, History>();
        public DateTime InitializeTime { get; } = DateTime.Now;
        public DateTime LastUpdateTime { get; set; } = DateTime.MinValue;
        public ThreadSafe.Boolean HasInitialized { get; set; } = new ThreadSafe.Boolean(false);
        private SQLiteConnection sqlite_conn { get; set; } = null;
        public ConcurrentQueue<History> RecentlyAdded { get; set; } = new ConcurrentQueue<History>();
        public ConcurrentQueue<History> RecentlyDeleted { get; set; } = new ConcurrentQueue<History>();
        public ThreadSafe.Integer AddedCount { get; set; } = new ThreadSafe.Integer(0);
        public ThreadSafe.Integer DeletedCount { get; set; } = new ThreadSafe.Integer(0);
        //private ThreadSafe.Boolean IsUpdating { get; set; } = new ThreadSafe.Boolean(false);
        private BlockingCollection<DBQueueHistoryItem> DBQueueHistory = new BlockingCollection<DBQueueHistoryItem>();
        public MovingCalcs AddTimeCalc { get; set; } = new MovingCalcs(1000, "DB Items", true);
        public MovingCalcs DeleteTimeCalc { get; set; } = new MovingCalcs(1000, "DB Items", true);

        public static object DBLock = new object();

        private DateTime LastCleanTime = DateTime.MinValue;
        public SQLiteHistory(string Filename, bool ReadOnly)
        {
            if (string.IsNullOrEmpty(Filename))
                throw new System.ArgumentException("Parameter cannot be empty", nameof(Filename));

            this.Filename = Filename;
            this.ReadOnly = ReadOnly;
            this.IsNew = !File.Exists(Filename);

        }
        public async Task Initialize()
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            try
            {
                await this.UpdateHistoryListAsync(true);

                if (this.HistoryDic.Count == 0)
                    this.MigrateHistoryCSV(AppSettings.Settings.HistoryFileName);

            }
            catch (Exception ex)
            {
                Log("Error: " + ex.Msg());
            }
            finally
            {
                Task.Run(this.HistoryJobQueueLoop);
                this.HasInitialized = true;
                Global.SendMessage(MessageType.DatabaseInitialized);
            }

        }

        private void HistoryJobQueueLoop()
        {
            //this runs forever and blocks if nothing is in the queue

            try
            {
                foreach (DBQueueHistoryItem hitm in this.DBQueueHistory.GetConsumingEnumerable(MasterCTS.Token))
                {
                    if (MasterCTS.IsCancellationRequested)
                        break;

                    string file = "";
                    try
                    {
                        if (hitm == null || hitm.hist == null || string.IsNullOrEmpty(hitm.hist.Filename))
                        {
                            Log("Error: hist should not be null?");
                        }
                        else
                        {
                            file = hitm.hist.Filename;
                            if (hitm.add)
                                this.InsertHistoryItem(hitm.hist);
                            else
                                this.DeleteHistoryItem(hitm.hist);  //Getting nullreferenceexception here and cant figure out why
                        }
                    }
                    catch (Exception ex)
                    {

                        Log($"Error: ({file})" + ex.ToString());
                    }

                }
            }
            catch (Exception ex)
            {

                Log($"Debug: HistoryJobQueueLoop canceled: {ex.Message}");
            }

            Log($"Debug: HistoryJobQueueLoop exited.");

            this.Dispose();

        }

        private bool IsSQLiteDBConnected()
        {

            //make sure only one thread updating at a time
            //await Semaphore_Updating.WaitAsync();

            try
            {
                return (this.sqlite_conn != null && !string.IsNullOrEmpty(this.sqlite_conn.DatabasePath));
            }
            catch (Exception ex)
            {
                Log("Error: " + ex.Msg(), "None", "None", "None");

                return false;
            }
            finally
            {
                //Semaphore_Updating.Release();
            }
        }

        private void DisposeConnection()
        {

            if (this.sqlite_conn == null)
                return;

            try
            {
                lock (DBLock)
                {
                    this.sqlite_conn.Close();
                    this.sqlite_conn = null;
                }

            }
            catch (Exception ex)
            {
                Log("Error: " + ex.Msg());

            }

        }
        private bool CreateConnection()
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            int retries = 0;
            bool restorebackup = false;

        RetryConnection:

            retries++;

            bool ret = false;
            string sflags = "SharedCache";

            try
            {
                Stopwatch sw = Stopwatch.StartNew();

                Global.UpdateProgressBar("Debug: Initializing history database...", 1, 1, 1);

                this.HistoryDic.Clear();
                this.RecentlyDeleted = new ConcurrentQueue<History>();
                this.RecentlyAdded = new ConcurrentQueue<History>();

                //https://www.sqlite.org/threadsafe.html
                SQLiteOpenFlags flags = SQLiteOpenFlags.SharedCache; // SQLiteOpenFlags.Create; // | SQLiteOpenFlags.NoMutex;  //| SQLiteOpenFlags.FullMutex;

                if (this.ReadOnly)
                {
                    flags = flags | SQLiteOpenFlags.ReadOnly;
                    sflags += ";ReadOnly";
                }
                else
                {
                    flags = flags | SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite;
                    sflags += ";Create;ReadWrite";
                }

                if (this.IsSQLiteDBConnected())
                {
                    this.DisposeConnection();
                }

                string fldr = Path.GetDirectoryName(this.Filename);

                if (!Directory.Exists(fldr))
                    Directory.CreateDirectory(fldr);

                string backupfile = this.Filename + ".bak";
                bool dbFileExisted = File.Exists(this.Filename);
                bool bakFileExisted = File.Exists(backupfile);

                if (restorebackup && bakFileExisted)
                {
                    Log($"Debug: Restoring backup database: {backupfile}...");
                    File.Copy(backupfile, this.Filename, true);
                    File.Delete(backupfile);
                }

                restorebackup = false;

                //If the database file doesn't exist, the default behavior is to create a new file
                this.sqlite_conn = new SQLiteConnection(this.Filename, flags, true);

                if (!this.ReadOnly)
                {
                    //make sure table exists:
                    CreateTableResult ctr = this.sqlite_conn.CreateTable<History>();
                }

                this.sqlite_conn.ExecuteScalar<int>(@"PRAGMA journal_mode = 'WAL';", new object[] { });
                this.sqlite_conn.ExecuteScalar<int>(@"PRAGMA busy_timeout = 30000;", new object[] { });

                if (!this.ReadOnly && dbFileExisted)
                {
                    //backup if we didnt just create the main db file
                    this.sqlite_conn.Backup(this.Filename + ".bak");
                }

                ret = true;

                sw.Stop();

                Log($"Debug: Created connection to SQLite db v{this.sqlite_conn.LibVersionNumber} in {sw.ElapsedMilliseconds}ms, Flags='{sflags}': {this.Filename}", "None", "None", "None");


            }
            catch (Exception ex)
            {

                Log($"Error: Flags='{sflags}', Error=" + ex.Msg(), "None", "None", "None");
            }

            if (!ret)
            {
                if (retries > 3)
                {
                    Log($"Error: Retries exceeded, could not create history database? {this.Filename}");
                    return false;
                }

                Log($"Debug: Retrying database connection #{retries} of 3...");

                //close the database if open
                DisposeConnection();

                //first try to restore the backup
                if (retries == 1)
                {
                    restorebackup = true;
                }

                //delete the database file and try again
                Global.SafeFileDelete(this.Filename, "SQLiteHistoryError");
                Global.SafeFileDelete(this.Filename + "-shm", "SQLiteHistoryError");
                Global.SafeFileDelete(this.Filename + "-wal", "SQLiteHistoryError");

                goto RetryConnection;
            }

            return ret;
        }

        public bool InsertHistoryQueue(History hist)
        {
            //simply add to the queue
            DBQueueHistoryItem ditm = new DBQueueHistoryItem(hist, true);
            return this.DBQueueHistory.TryAdd(ditm);
        }

        private bool InsertHistoryItem(History hist)
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            bool ret = false;
            int iret = 0;

            //make sure only one thread updating at a time
            //await Semaphore_Updating.WaitAsync();

            //this.IsUpdating= true;
            lock (DBLock)
            {

                try
                {
                    ret = this.HistoryDic.TryAdd(hist.Filename.ToLower(), hist);

                    if (!ret)
                    {
                        Log($"debug: File already existed in dictionary: {hist.Filename}", hist.AIServer, hist.Camera, hist.Filename);
                    }

                    Stopwatch sw = Stopwatch.StartNew();

                    iret = this.sqlite_conn.Insert(hist);

                    this.AddTimeCalc.AddToCalc(sw.ElapsedMilliseconds);

                    if (iret != 1)
                    {
                        Log($"Error: When trying to insert database entry, RowsAdded count was {ret} but we expected 1. StackDepth={new StackTrace().FrameCount}, TID={Thread.CurrentThread.ManagedThreadId}, TCNT={Process.GetCurrentProcess().Threads.Count}", hist.AIServer, hist.Camera, hist.Filename);
                    }

                    if (sw.ElapsedMilliseconds > 3000)
                        Log($"Debug: It took a long time to add a history item @ {sw.ElapsedMilliseconds}ms, (Count={this.AddTimeCalc.Count}, Min={this.AddTimeCalc.Min}ms, Max={this.AddTimeCalc.Max}ms, Avg={this.AddTimeCalc.Avg.ToString("#####")}ms), StackDepth={new StackTrace().FrameCount}, TID={Thread.CurrentThread.ManagedThreadId}, TCNT={Process.GetCurrentProcess().Threads.Count}: {this.Filename}", hist.AIServer, hist.Camera, hist.Filename);

                }
                catch (Exception ex)
                {

                    if (ex.Message.IndexOf("UNIQUE constraint failed", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        Log($"Debug: File was already in the database: {hist.Filename}", hist.AIServer, hist.Camera, hist.Filename);
                    }
                    else
                    {
                        Log($"Error: StackDepth={new StackTrace().FrameCount}, TID={Thread.CurrentThread.ManagedThreadId}, TCNT={Process.GetCurrentProcess().Threads.Count}: {hist.Filename}: " + ex.Msg(), hist.AIServer, hist.Camera, hist.Filename);
                    }
                }

                if (ret || iret > 0)
                {
                    this.RecentlyAdded.Enqueue(hist);
                    this.AddedCount++;
                    this.LastUpdateTime = DateTime.Now;
                }

            }

            //Semaphore_Updating.Release();

            //this.IsUpdating = false;

            return ret;

        }

        public bool DeleteHistoryQueue(History hist)
        {
            //simply add to the queue
            DBQueueHistoryItem ditm = new DBQueueHistoryItem(hist, false);
            return this.DBQueueHistory.TryAdd(ditm);
        }
        public bool DeleteHistoryQueue(string Filename)
        {
            //simply add to the queue
            History hist;
            this.HistoryDic.TryGetValue(Filename.ToLower(), out hist);

            if (hist == null)
                hist = new History().Create(Filename, DateTime.Now, "unknown", "", "", false, "", "", 0, false);

            DBQueueHistoryItem ditm = new DBQueueHistoryItem(hist, false);

            return this.DBQueueHistory.TryAdd(ditm);

        }

        private bool DeleteHistoryItem(History hist)
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            bool ret = false;
            int dret = 0;
            lock (DBLock)
            {
                try
                {

                    History thist = null;
                    ret = this.HistoryDic.TryRemove(hist.Filename.ToLower(), out thist);

                    Stopwatch sw = Stopwatch.StartNew();
                    //Error: Cannot delete String: it has no PK [NotSupportedException] Mod: <DeleteHistoryItem>d__18 Line:150:5
                    //trying to use object rather than primarykey to delete

                    if (thist != null)
                        hist = thist;  //probably dont have to do this in order to delete from db

                    dret = this.sqlite_conn.Delete(hist);

                    this.DeleteTimeCalc.AddToCalc(sw.ElapsedMilliseconds);

                    //if (dret != 1)
                    //{
                    //    Log($"Error: When trying to delete database entry '{Filename}', RowsDeleted count was {ret} but we expected 1. StackDepth={new StackTrace().FrameCount}, TID={Thread.CurrentThread.ManagedThreadId}, TCNT={Process.GetCurrentProcess().Threads.Count}");
                    //}
                    //else
                    //{
                    //    Log($"Removed {dret} database entry for '{Filename}'");
                    //}

                    if (sw.ElapsedMilliseconds > 2000)
                        Log($"Debug: It took a long time to delete a history item @ {sw.ElapsedMilliseconds}ms, (Count={this.DeleteTimeCalc.Count}, Min={this.DeleteTimeCalc.Min}ms, Max={this.DeleteTimeCalc.Max}ms, Avg={this.DeleteTimeCalc.Avg.ToString("#####")}ms), StackDepth={new StackTrace().FrameCount}, TID={Thread.CurrentThread.ManagedThreadId}, TCNT={Process.GetCurrentProcess().Threads.Count}: {this.Filename}", hist.AIServer, hist.Camera, hist.Filename);

                }
                catch (Exception ex)
                {

                    Log($"Error: File='{hist.Filename}' - " + ex.Msg(), hist.AIServer, hist.Camera, hist.Filename);
                }

                if (ret || dret > 0)
                {
                    this.DeletedCount++;
                    this.LastUpdateTime = DateTime.Now;
                    this.RecentlyDeleted.Enqueue(hist);
                }

                //Semaphore_Updating.Release();

                //this.IsUpdating = false;

            }

            return ret;

        }

        public bool MigrateHistoryCSV(string Filename)
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            bool ret = false;
            lock (DBLock)
            {
                try
                {

                    //if (!await this.IsSQLiteDBConnectedAsync())
                    //    await this.CreateConnectionAsync();

                    //run in another thread so we dont block UI
                    //await Task.Run(async () =>
                    //{

                    if (System.IO.File.Exists(Filename))
                    {
                        Global.UpdateProgressBar("Migrating history.csv...", 1, 1, 1);

                        Log($"Debug: Migrating history list from {Filename} ...");

                        Stopwatch SW = Stopwatch.StartNew();

                        //delete obsolete entries from history.csv
                        //CleanCSVList(); //removed to load the history list faster

                        List<string> result = new List<string>(); //List that later on will be containing all lines of the csv file

                        Global.WaitFileAccessResult wresult = Global.WaitForFileAccess(Filename);

                        if (wresult.Success)
                        {
                            //load all lines except the first line into List (the first line is the table heading and not an alert entry)
                            foreach (string line in System.IO.File.ReadAllLines(Filename).Skip(1))
                            {
                                result.Add(line);
                            }

                            Log($"...Found {result.Count} lines.");

                            List<string> itemsToDelete = new List<string>(); //stores all filenames of history.csv entries that need to be removed

                            //load all List elements into the ListView for each row
                            int added = 0;
                            int removed = 0;
                            int cnt = 0;
                            foreach (var val in result)
                            {
                                cnt++;

                                //if (cnt == 1 || cnt == result.Count || (cnt % (result.Count / 10) > 0))
                                //{
                                //    Global.UpdateProgressBar("Migrating history.csv...", cnt, 1, result.Count);
                                //}

                                History hist = new History().CreateFromCSV(val);
                                if (File.Exists(hist.Filename))
                                {
                                    if (this.InsertHistoryItem(hist))
                                    {
                                        added++;
                                        //this.AddedCount++;
                                    }
                                    else
                                    {
                                        removed++;
                                    }
                                }
                                else
                                {
                                    removed++;
                                }
                            }


                            ret = (added > 0);

                            //this.AddedCount.AtomicAddAndGet(added);
                            //this.DeletedCount.AtomicAddAndGet(removed);

                            //try to get a better feel how much time this function consumes - Vorlon
                            Log($"Debug: ...Added {added} out of {result.Count} history items ({removed} removed) in {SW.ElapsedMilliseconds}ms, {this.HistoryDic.Count} lines.");

                        }
                        else
                        {
                            Log($"Error: Could not gain access to history file for {wresult.TimeMS}ms with {wresult.ErrRetryCnt} retries - {AppSettings.Settings.HistoryFileName}");

                        }

                    }
                    else
                    {
                        Log($"Debug: Old history file does not exist, could not migrate: {Filename}");
                    }


                    //});


                }
                catch (Exception ex)
                {

                    Log("Error: " + ex.Msg());
                }

            }

            Global.UpdateProgressBar("", 0, 0, 0);


            return ret;


        }

        public List<History> GetRecentlyDeleted()
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            List<History> ret = new List<History>();
            while (!this.RecentlyDeleted.IsEmpty)
            {
                History hist;
                if (this.RecentlyDeleted.TryDequeue(out hist))
                    ret.Add(hist);
            }
            return ret;
        }
        public List<History> GetAllValues()
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            //because the dictionary doesnt give a proper sorted list
            if ((DateTime.Now - this.LastCleanTime).Hours >= AppSettings.Settings.HistoryHoursBetweenCleaning)
                this.CleanHistoryList();

            return this.HistoryDic.Values.OrderBy(c => c.Date).ToList();
        }

        public List<History> GetRecentlyAdded()
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            if ((DateTime.Now - this.LastCleanTime).Hours >= AppSettings.Settings.HistoryHoursBetweenCleaning)
                this.CleanHistoryList();

            List<History> ret = new List<History>();
            while (!this.RecentlyAdded.IsEmpty)
            {
                History hist;
                if (this.RecentlyAdded.TryDequeue(out hist))
                    ret.Add(hist);
            }


            return ret;
        }

        public async Task<bool> HasUpdates()
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            try
            {

                if (!this.ReadOnly)  //assume if we had to wait the database was just updating
                {
                    //we are running in the same process as the database updater
                    //no need to re-check sqlite db every time

                    if (!this.RecentlyAdded.IsEmpty || !this.RecentlyDeleted.IsEmpty)
                    {
                        return true;
                    }
                }
                else
                {
                    //do a full update for now, later figure out best way to communicate updates from service to gui
                    return await this.UpdateHistoryListAsync(false);
                }

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                //Semaphore_Updating.Release();
            }

            return false;

        }

        public async Task<bool> UpdateHistoryListAsync(bool Clean)
        {
            return await Task.Run(() => this.UpdateHistoryList(Clean));
        }

        public bool UpdateHistoryList(bool Clean)
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            bool ret = false;

            lock (DBLock)
            {
                try
                {

                    Global.UpdateProgressBar("Reading database...", 1, 1, 1);

                    Stopwatch sw = Stopwatch.StartNew();

                    if (!this.IsSQLiteDBConnected())
                    {
                        if (!this.CreateConnection())
                            return false;
                    }

                    TableQuery<History> query = this.sqlite_conn.Table<History>();
                    List<History> CurList = query.ToList();

                    int added = 0;
                    int removed = 0;
                    bool isnew = (this.HistoryDic.Count == 0);

                    Dictionary<string, String> tmpdic = new Dictionary<string, String>();

                    //lets try to keep the original objects in memory and not create them new
                    int cnt = 0;
                    long LastMS = sw.ElapsedMilliseconds;
                    int CurListCount = CurList.Count;
                    int HalfList = CurListCount / 2;

                    foreach (History hist in CurList)
                    {
                        cnt++;

                        if (cnt == 1 || cnt == HalfList || cnt > (CurListCount - 5) || (sw.ElapsedMilliseconds - LastMS >= 500))
                        {
                            Global.UpdateProgressBar("Reading database...", cnt, 1, CurList.Count);
                            LastMS = sw.ElapsedMilliseconds;
                        }

                        if (!isnew)
                        {
                            //add missing
                            if (this.HistoryDic.TryAdd(hist.Filename.ToLower(), hist))
                            {
                                added++;
                            }
                            tmpdic.Add(hist.Filename.ToLower(), hist.Filename);
                        }
                        else
                        {
                            this.HistoryDic.TryAdd(hist.Filename.ToLower(), hist);
                            added++;
                        }
                    }

                    if (!isnew)
                    {
                        //subtract
                        foreach (History hist in this.HistoryDic.Values)
                        {
                            if (!tmpdic.ContainsKey(hist.Filename.ToLower()))
                            {
                                History th;
                                this.HistoryDic.TryRemove(hist.Filename.ToLower(), out th);
                                removed++;
                            }

                        }

                    }

                    if (Clean && !this.ReadOnly)
                        this.CleanHistoryList();

                    ret = (added > 0 || removed > 0);

                    if (ret)
                    {
                        //this.DeletedCount.AtomicAddAndGet(removed);
                        if (!isnew)
                            this.AddedCount = added;

                        this.LastUpdateTime = DateTime.Now;
                    }

                    Log($"Debug: Update History Database: Added={added}, removed={removed}, total={this.HistoryDic.Count}, StackDepth={new StackTrace().FrameCount}, TID={Thread.CurrentThread.ManagedThreadId}, TCNT={Process.GetCurrentProcess().Threads.Count} in {sw.ElapsedMilliseconds}ms");

                }
                catch (Exception ex)
                {

                    Log("Error: " + ex.Msg());
                }

            }

            Global.UpdateProgressBar("", 0, 0, 0);


            return ret;

        }

        private async Task<bool> CleanHistoryList()
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            bool ret = false;

            try
            {
                Stopwatch sw = Stopwatch.StartNew();


                if (!this.IsSQLiteDBConnected())
                {
                    if (!this.CreateConnection())
                        return false;
                }

                ConcurrentBag<History> removed = new ConcurrentBag<History>();   //this may only need to be a list, but just being safe in threading

                //run the file exists check in another thread so we dont freeze the UI, but WAIT for it
                await Task.Run(async () =>
                {
                    Log("Debug: Removing missing files from in-memory database...");

                    try
                    {
                        int missing = 0;
                        int tooold = 0;
                        int failedcnt = 0;

                        int cnt = 0;
                        int HistCount = this.HistoryDic.Count;
                        long LastMS = sw.ElapsedMilliseconds;
                        int HalfCount = HistCount / 2;
                        //we are allowed to do this with a ConcurrentDictionary...
                        foreach (History hist in this.HistoryDic.Values)
                        {
                            cnt++;

                            if (cnt == 1 || cnt == HalfCount || cnt > (HistCount - 5) || (sw.ElapsedMilliseconds - LastMS >= 500))
                            {
                                Global.UpdateProgressBar("Cleaning database (1 of 2)...", cnt, 1, HistCount);
                                LastMS = sw.ElapsedMilliseconds;
                            }

                            bool IsTooOld = (DateTime.Now - hist.Date).TotalDays >= AppSettings.Settings.MaxHistoryAgeDays;

                            if (IsTooOld || !File.Exists(hist.Filename))
                            {
                                if (IsTooOld)
                                    tooold++;
                                else
                                    missing++;

                                removed.Add(hist);
                                this.RecentlyDeleted.Enqueue(hist);

                                History rhist;

                                if (!this.HistoryDic.TryRemove(hist.Filename.ToLower(), out rhist))
                                {
                                    failedcnt++;
                                    Log($"Trace: Could not remove from in-memory database: {hist.Filename}");
                                }
                            }

                        }

                        if (removed.Count > 0)
                        {
                            Log($"Debug: Removing {missing} files and {tooold} database entries that are older than {AppSettings.Settings.MaxHistoryAgeDays} days (MaxHistoryAgeDays) from file database...");
                            //start another thread to finish cleaning the database so that the app gets the list faster...
                            //the db should be thread safe due to opening with fullmutex flag
                            Stopwatch swr = Stopwatch.StartNew();
                            int rcnt = 0;
                            cnt = 0;
                            LastMS = sw.ElapsedMilliseconds;
                            int RemovedCount = removed.Count;
                            int HalfRemoved = RemovedCount / 2;

                            foreach (History hist in removed)
                            {
                                cnt++;

                                if (cnt == 1 || cnt == HalfRemoved || cnt > (RemovedCount - 5) || (sw.ElapsedMilliseconds - LastMS >= 500))
                                {
                                    Global.UpdateProgressBar("Cleaning database (2 of 2)...", cnt, 1, removed.Count);
                                    LastMS = sw.ElapsedMilliseconds;
                                }

                                int rowsdeleted = 0;

                                try
                                {
                                    Stopwatch dsw = Stopwatch.StartNew();

                                    rowsdeleted = this.sqlite_conn.Delete(hist);

                                    this.DeleteTimeCalc.AddToCalc(dsw.ElapsedMilliseconds);

                                    this.RecentlyDeleted.Enqueue(hist);

                                    if (rowsdeleted != 1)
                                    {
                                        failedcnt++;
                                        Log($"Trace: When trying to delete database entry, RowsDeleted count was {rowsdeleted} but we expected 1.");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    failedcnt++;
                                    Log($"Error: StackDepth={new StackTrace().FrameCount}, TID={Thread.CurrentThread.ManagedThreadId}, TCNT={Process.GetCurrentProcess().Threads.Count}: '{this.Filename}' - " + ex.Msg());
                                }
                                rcnt += rowsdeleted;
                            }


                            //this.DeletedCount.AtomicAddAndGet(rcnt);

                            Log($"Debug: ...Cleaned {rcnt} of {removed.Count} history file database items. {failedcnt} failed, {missing} missing, {tooold} too old.  Time={swr.ElapsedMilliseconds}ms (Count={this.DeleteTimeCalc.Count}, Min={this.DeleteTimeCalc.Min}ms, Max={this.DeleteTimeCalc.Max}ms, Avg={this.DeleteTimeCalc.Avg.ToString("#####")}ms)");

                        }
                        else
                        {
                            Log("debug: No missing or old files to clean from database.");
                        }

                    }
                    catch (Exception ex)
                    {

                        Log("Error: " + ex.ToString());
                    }


                });


                Log($"Debug: ...Cleaned {removed.Count} items in {sw.ElapsedMilliseconds}ms");


                ret = removed.Count > 0;


            }
            catch (Exception ex)
            {

                Log("Error: " + ex.ToString());
            }
            finally
            {
                Global.UpdateProgressBar("", 0, 0, 0);
                this.LastCleanTime = DateTime.Now;
            }

            return ret;

        }


        protected virtual async void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    if (this.IsSQLiteDBConnected())
                    {
                        this.DisposeConnection();
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                this.disposedValue = true;
            }
        }


        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

