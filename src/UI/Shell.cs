using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

using BrightIdeasSoftware;

using Microsoft.WindowsAPICodePack.Dialogs;

using Newtonsoft.Json; //deserialize DeepquestAI response

using NLog;

using static AITool.AITOOL;
using static AITool.Global;

namespace AITool
{

    public partial class Shell:Form
    {
        private ThreadSafe.DateTime LastListUpdate = new ThreadSafe.DateTime(DateTime.MinValue, AppSettings.Settings.DateFormat);

        private ThreadSafe.Boolean DatabaseInitialized = new ThreadSafe.Boolean(false);

        private ThreadSafe.Boolean IsHistoryListUpdating = new ThreadSafe.Boolean(false);
        private ThreadSafe.Boolean IsLogListUpdating = new ThreadSafe.Boolean(false);
        private ThreadSafe.Boolean IsStatsUpdating = new ThreadSafe.Boolean(false);

        //Dictionary<string, History> HistoryDic = new Dictionary<string, History>();
        //private ThreadSafe.Boolean FilterChanged = new ThreadSafe.Boolean(true);

        //Instantiate a Singleton of the Semaphore with a value of 1. This means that only 1 thread can be granted access at a time.
        public static SemaphoreSlim Semaphore_List_Updating = new SemaphoreSlim(1, 1);

        //public static ConcurrentQueue<History> AddedHistoryItems = new ConcurrentQueue<History>();
        //public static ConcurrentQueue<History> DeletedHistoryItems = new ConcurrentQueue<History>();

        //for searching log tab:
        System.Timers.Timer tmr;
        DateTime TimeSinceType = DateTime.MinValue;

        FrmSplash SplashScreen = null;

        Stopwatch StartupSW = null;




        public Shell()
        {


            this.StartupSW = Stopwatch.StartNew();

            this.InitializeComponent();

            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            //this is to log and status messages from other classes...
            Global.progress = new Progress<ClsMessage>(this.EventMessage);

            this.SplashScreen = new FrmSplash();

            if (Debugger.IsAttached)
                this.SplashScreen.Hide();
            else
                this.SplashScreen.Show();

            HideForm();

            Application.DoEvents();
            Debug.Print("Init tid=" + Thread.CurrentThread.ManagedThreadId);

        }


        private async void Shell_Load(object sender, EventArgs e)
        {
            //PlayOOG("D:\\TEMP\\AwACAgEAAxkBAAI7OGGhP0AhP-aBlubv3Vbq4OahTbkAAy4CAAJ59AlFMssC4x5Yy3wiBA.ogg");

            //using var synth = new SpeechSynthesizer();
            //foreach (InstalledVoice voice in synth.GetInstalledVoices())
            //{
            //    VoiceInfo info = voice.VoiceInfo;
            //    Console.WriteLine(" Voice Name: " + info.Name);
            //}
            //string test = CompressToBase64String(" er 2 35464353 4t f fg wefg wer gwe gw t345 y4 t4 rtw w egfgkfkkkk");
            //bool isbase = IsBase64String(test);
            //TimeSpan ts = TimeSpan.FromDays(1.2);
            //Debug.Print(ts.FormatTS(true));
            //Debug.Print(ts);

            //Global.IsTimeBetween(new DateTime(2021, 6, 13, 06, 00, 00), "dusk-dawn");
            //Global.IsTimeBetween(new DateTime(2021, 6, 13, 23, 00, 00), "dusk-dawn");
            //Global.IsTimeBetween(new DateTime(2021, 6, 13, 07, 00, 00), "dusk-dawn");
            //Global.IsTimeBetween(new DateTime(2021, 6, 13, 21, 00, 00), "dusk-dawn");

            //Global.IsTimeBetween(new DateTime(2021, 6, 13, 06, 00, 00), "dawn-dusk");
            //Global.IsTimeBetween(new DateTime(2021, 6, 13, 23, 00, 00), "dawn-dusk");
            //Global.IsTimeBetween(new DateTime(2021, 6, 13, 07, 00, 00), "dawn-dusk");
            //Global.IsTimeBetween(new DateTime(2021, 6, 13, 21, 00, 00), "dawn-dusk");

            //Rectangle img = new Rectangle(0, 0, 3840, 2160);
            //Rectangle prd = new Rectangle(0, 0, 155, 204);  // .38% of the original image
            //Rectangle prd = new Rectangle(0, 0, img.Width / 2, img.Height);  //half of the original image = 50%
            //Rectangle prd = new Rectangle(0, 0, img.Width, img.Height / 2);  //half of the original image = 50%
            //Rectangle prd = new Rectangle(0, 0, img.Width / 2, img.Height / 2);  //a quarter of the original image = 25%

            //double percent = img.PercentOfSize(prd);

            Debug.Print("load tid=" + Thread.CurrentThread.ManagedThreadId);

            //Uri pth = new Uri("\\\\[2600:6c64:6b7f:f8d8::1d4]\\c$");

            //ClsDoodsRequest cdr = new ClsDoodsRequest();

            //cdr.Detect.MinPercentMatch = 50;

            //string testjson = JsonConvert.SerializeObject(cdr);


            using var cw = new Global_GUI.CursorWait(true);

            //---------------------------------------------------------------------------
            //INITIALIZE HISTORY DB, load settings, ETC

            if (AppSettings.AlreadyRunning && !Global.IsService)
            {
                if (MessageBox.Show("AITOOL.EXE is already running.  If you need to modify settings:\r\n\r\n1) Close this copy.\r\n2) Stop the other service/instance. \r\n3) Restart AITOOL.\r\n\r\nCLOSE THIS COPY?", "Already running", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    this.SplashScreen.Close();
                    this.SplashScreen = null;
                    CloseImmediately = true;
                    Application.Exit();
                }
            }

            Stopwatch sw = Stopwatch.StartNew();

            await AITOOL.InitializeBackend();


            //Camera testcam = GetCamera("c:\\test\\CAMNAME.1.123456.jpg");

            //ClsImageQueueItem cli = new ClsImageQueueItem("C:\\Downloads\\TestImage.jpg", 0);
            //NPushover.RequestObjects.Message msg = new NPushover.RequestObjects.Message();
            //msg.Timestamp = DateTime.Now;
            //msg.Title = "Test title";
            //msg.Body = "Some message";

            //AITOOL.pushoverClient = new NPushover.Pushover(AppSettings.Settings.pushover_APIKey);
            //NPushover.ResponseObjects.PushoverUserResponse usr = await AITOOL.pushoverClient.SendPushoverMessageAsync(msg, "", cli);

            Log($"Debug: Back end initialization completed in {sw.ElapsedMilliseconds}ms.");


            //since async stuff continues on another thread, we have to do this:

            Global_GUI.InvokeIFRequired(this, () =>
            {
                //---------------------------------------------------------------------------
                //HISTORY TAB
                Global_GUI.ConfigureFOLV(this.folv_history, typeof(History), new Font("Segoe UI", (float)9.75, FontStyle.Regular), this.HistoryImageList, GridLines: false);
                //this.folv_history.EmptyListMsg = "Initializing database";
                this.cb_showMask.Checked = AppSettings.Settings.HistoryShowMask;
                this.cb_showObjects.Checked = AppSettings.Settings.HistoryShowObjects;
                this.cb_follow.Checked = AppSettings.Settings.HistoryFollow;
                this.automaticallyRefreshToolStripMenuItem.Checked = AppSettings.Settings.HistoryAutoRefresh;
                this.storeFalseAlertsToolStripMenuItem.Checked = AppSettings.Settings.HistoryStoreFalseAlerts;
                this.storeMaskedAlertsToolStripMenuItem.Checked = AppSettings.Settings.HistoryStoreMaskedAlerts;
                this.showOnlyRelevantObjectsToolStripMenuItem.Checked = AppSettings.Settings.HistoryOnlyDisplayRelevantObjects;
                this.restrictThresholdAtSourceToolStripMenuItem.Checked = AppSettings.Settings.HistoryRestrictMinThresholdAtSource;
                this.mergeDuplicatePredictionsToolStripMenuItem.Checked = AppSettings.Settings.HistoryMergeDuplicatePredictions;
                this.cb_filter_animal.Checked = AppSettings.Settings.HistoryFilterAnimals;
                this.cb_filter_masked.Checked = AppSettings.Settings.HistoryFilterMasked;
                this.cb_filter_nosuccess.Checked = AppSettings.Settings.HistoryFilterNoSuccess;
                this.cb_filter_person.Checked = AppSettings.Settings.HistoryFilterPeople;
                this.cb_filter_skipped.Checked = AppSettings.Settings.HistoryFilterSkipped;
                this.cb_filter_success.Checked = AppSettings.Settings.HistoryFilterRelevant;
                this.cb_filter_vehicle.Checked = AppSettings.Settings.HistoryFilterVehicles;
                this.HistoryUpdateListTimer.Interval = AppSettings.Settings.TimeBetweenListRefreshsMS;

                //---------------------------------------------------------------------------
                //CAMERAS TAB

                Global_GUI.ConfigureFOLV(this.FOLV_Cameras, typeof(Camera), new Font("Segoe UI", (float)9.75, FontStyle.Regular), CameraImageList, GridLines: false);


                this.comboBox_filter_camera.SelectedIndex = this.comboBox_filter_camera.FindStringExact("All Cameras"); //select all cameras entry

                //---------------------------------------------------------------------------
                //SETTINGS TAB

                this.LoadSettingsTab();

                //---------------------------------------------------------------------------
                //STATS TAB
                this.comboBox1.Items.Add("All Cameras"); //add all cameras stats entry
                this.comboBox1.SelectedIndex = this.comboBox1.FindStringExact("All Cameras"); //select all cameras entry


                //---------------------------------------------------------------------------
                //Deepstack server TAB


                if (!DeepStackServerControl.IsInstalled)
                {
                    //remove deepstack tab if not installed
                    //Log("Removing DeepStack tab since it not installed as a Windows app (No docker support yet)");
                    //this.tabControl1.TabPages.Remove(this.tabControl1.TabPages[""]);
                }
                else
                {
                    if (DeepStackServerControl.NeedsSaving)
                    {
                        //this may happen if the already running instance has a different port, etc, so we update the config
                        this.SaveDeepStackTabAsync();
                    }
                    this.LoadDeepStackTab();
                }

                //---------------------------------------------------------------------------
                //LOG TAB

                Global_GUI.ConfigureFOLV(this.folv_log, typeof(ClsLogItm), null, null, GridLines: false);

                this.UpdateLogAddedRemovedAsync(true);
                this.LogUpdateListTimer.Interval = AppSettings.Settings.TimeBetweenListRefreshsMS;

                if (this.toolStripButtonPauseLog.Checked)
                {
                    this.LogUpdateListTimer.Enabled = false;
                    this.LogUpdateListTimer.Stop();
                }
                else
                {
                    this.LogUpdateListTimer.Enabled = true;
                    this.LogUpdateListTimer.Start();
                }

                this.UpdateLogAddedRemovedAsync(true);

                this.tmr = new System.Timers.Timer();
                this.tmr.Interval = 300;
                this.tmr.Elapsed += new System.Timers.ElapsedEventHandler(this.tmr_Elapsed);
                this.tmr.Stop();

                this.ToolStripComboBoxSearch.Text = Global.GetRegSetting("SearchText", "");
                this.mnu_Filter.Checked = AppSettings.Settings.log_mnu_Filter;
                this.mnu_Highlight.Checked = AppSettings.Settings.log_mnu_Highlight;

                if (string.Equals(AppSettings.Settings.LogLevel, "off", StringComparison.OrdinalIgnoreCase))
                {
                    this.mnu_log_filter_off.Checked = true;
                }
                else if (string.Equals(AppSettings.Settings.LogLevel, "fatal", StringComparison.OrdinalIgnoreCase))
                {
                    this.mnu_log_filter_fatal.Checked = true;
                }
                else if (string.Equals(AppSettings.Settings.LogLevel, "error", StringComparison.OrdinalIgnoreCase))
                {
                    this.mnu_log_filter_error.Checked = true;
                }
                else if (string.Equals(AppSettings.Settings.LogLevel, "warn", StringComparison.OrdinalIgnoreCase))
                {
                    this.mnu_log_filter_warn.Checked = true;
                }
                else if (string.Equals(AppSettings.Settings.LogLevel, "info", StringComparison.OrdinalIgnoreCase))
                {
                    this.mnu_log_filter_info.Checked = true;
                }
                else if (string.Equals(AppSettings.Settings.LogLevel, "debug", StringComparison.OrdinalIgnoreCase))
                {
                    this.mnu_log_filter_debug.Checked = true;
                }
                else if (string.Equals(AppSettings.Settings.LogLevel, "trace", StringComparison.OrdinalIgnoreCase))
                {
                    this.mnu_log_filter_trace.Checked = true;
                }

                folv_log.MouseWheel += ListScrollZoom;
                folv_history.MouseWheel += ListScrollZoom;

                //---------------------------------------------------------------------------
                // finish up

                string AssemVer = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                this.lbl_version.Text = $"Version {AssemVer} built on {Global.RetrieveLinkerTimestamp()}";

                //---------------------------------------------------------------------------------------------------------

                IsLoading = false;

                this.Resize += new System.EventHandler(this.Form1_Resize); //resize event to enable 'minimize to tray'

                Log($"{{yellow}}APP START complete.  Initialized in {this.StartupSW.Elapsed.TotalSeconds.ToString("##0.0")} seconds ({this.StartupSW.ElapsedMilliseconds}ms)");

                this.SplashScreen.Close();
                this.SplashScreen = null;

                Global_GUI.RestoreWindowState(this);

                this.LoadCameras(); //load camera list

                if (Environment.CommandLine.IndexOf("/min", StringComparison.OrdinalIgnoreCase) == -1)
                {
                    this.ShowForm();
                }


            });


        }

        private void ListScrollZoom(object sender, MouseEventArgs e)
        {
            if (Control.ModifierKeys != Keys.Control)
                return;
            float delta = (e.Delta > 0 ? 2f : -2f);
            FastObjectListView folv = ((FastObjectListView)sender);
            folv.Font = new Font(folv.Font.FontFamily, folv.Font.Size + delta);
            folv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }
        private void LoadSettingsTab()
        {

            // fill settings tab with stored settings

            this.cmbInput.Text = AppSettings.Settings.input_path;
            this.cb_inputpathsubfolders.Checked = AppSettings.Settings.input_path_includesubfolders;
            this.cmbInput.Items.Clear();
            foreach (string pth in AITOOL.BlueIrisInfo.ClipPaths)
            {
                this.cmbInput.Items.Add(pth);

            }

            //this.tbDeepstackUrl.Text = AppSettings.Settings.deepstack_url;
            this.cb_DeepStackURLsQueued.Checked = AppSettings.Settings.deepstack_urls_are_queued;

            this.Chk_AutoScroll.Checked = AppSettings.Settings.Autoscroll_log;

            this.tb_telegram_chatid.Text = String.Join(",", AppSettings.Settings.telegram_chatids);
            this.tb_telegram_token.Text = AppSettings.Settings.telegram_token;
            this.tb_telegram_cooldown.Text = AppSettings.Settings.telegram_cooldown_seconds.ToString();

            this.cb_send_telegram_errors.Checked = AppSettings.Settings.send_telegram_errors;
            this.cb_send_pushover_errors.Checked = AppSettings.Settings.send_pushover_errors;
            this.cbStartWithWindows.Checked = AppSettings.Settings.startwithwindows;
            this.cbMinimizeToTray.Checked = AppSettings.Settings.MinimizeToTray;

            this.tb_username.Text = AppSettings.Settings.DefaultUserName;
            this.tb_password.Text = AppSettings.Settings.DefaultPasswordEncrypted.Decrypt();

            this.tb_BlueIrisServer.Text = AppSettings.Settings.BlueIrisServer;


            if (AITOOL.BlueIrisInfo.Result == BlueIrisResult.Valid)
            {
                lbl_blueirisserver.Text = "BI Config: " + AITOOL.BlueIrisInfo.Result;
                lbl_blueirisserver.Text += $";  WebServer is configured for {AITOOL.BlueIrisInfo.URL}";
                lbl_blueirisserver.ForeColor = Color.DodgerBlue;
                if (Global.IsValidIPAddress(AppSettings.Settings.BlueIrisServer, out IPAddress foundip) && !AppSettings.Settings.BlueIrisServer.EqualsIgnoreCase(AITOOL.BlueIrisInfo.ServerName) && AppSettings.Settings.BlueIrisServer != "127.0.0.1")
                {
                    Log($"Warning: BlueIris Settings > Web Server > Local IP address is set to a different IP: AITOOL={AppSettings.Settings.BlueIrisServer}, BI={AITOOL.BlueIrisInfo.ServerName}");
                }
            }
            else if (!AppSettings.Settings.BlueIrisServer.IsEmpty())
            {
                lbl_blueirisserver.Text = "BI Config: Error - " + AITOOL.BlueIrisInfo.Result;
                lbl_blueirisserver.ForeColor = Color.MediumPurple;
            }
            else
            {
                lbl_blueirisserver.Text = "";
            }

            this.tb_Pushover_APIKey.Text = AppSettings.Settings.pushover_APIKey;
            this.tb_Pushover_UserKey.Text = AppSettings.Settings.pushover_UserKey;
            this.tb_Pushover_Cooldown.Text = AppSettings.Settings.pushover_cooldown_seconds.ToString();

        }
        private void tmr_Elapsed(object sender, ElapsedEventArgs e)
        {
            if ((DateTime.Now - this.TimeSinceType).Milliseconds >= 600)
            {
                Global_GUI.InvokeIFRequired(this.toolStripStatusLabelHistoryItems.GetCurrentParent(), () =>
                {
                    this.tmr.Stop();
                    this.tmr.Enabled = false;
                    if (Global.IsRegexPatternValid(this.ToolStripComboBoxSearch.Text) || this.ToolStripComboBoxSearch.Text.Length == 0)
                    {
                        this.ToolStripComboBoxSearch.ForeColor = Color.Blue;
                        if (this.ToolStripComboBoxSearch.FindStringExact(this.ToolStripComboBoxSearch.Text) == -1)
                            this.ToolStripComboBoxSearch.Items.Add(this.ToolStripComboBoxSearch.Text);

                        Global_GUI.FilterFOLV(this.folv_log, this.ToolStripComboBoxSearch.Text, this.mnu_Filter.Checked);
                    }
                    else
                    {
                        this.ToolStripComboBoxSearch.ForeColor = Color.Red;
                    }
                });
            }
        }

        private async Task UpdateLogAddedRemovedAsync(bool Follow = false, bool Force = false)
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            if (IsLoading)
                return;

            if (!this.IsLogListUpdating && Force ||
                !this.IsLogListUpdating &&
                (this.tabControl1.SelectedTab == this.tabControl1.TabPages["tabLog"]) &&
                this.Visible &&
                !(this.WindowState == FormWindowState.Minimized) &&
                LogMan != null)
            {
                //run in another thread so gui doesnt freeze
                //await Task.Run(() =>
                //{
                Stopwatch sw = Stopwatch.StartNew();

                this.IsLogListUpdating = true;

                Global_GUI.InvokeIFRequired(this.folv_log, async () =>
                {
                    if (this.folv_log.Items.Count == 0)
                        this.folv_log.EmptyListMsg = "Loading log...";

                    List<ClsLogItm> added = LogMan.GetRecentlyAdded();
                    List<ClsLogItm> removed = LogMan.GetRecentlyDeleted();

                    if (added.Count > 2000 || this.folv_log.Items.Count == 0)
                    {
                        //do it all in one update so it is faster:
                        using var cw = new Global_GUI.CursorWait();
                        // run in another thread so gui doesnt freeze
                        await Task.Run(() =>
                        {
                            Global_GUI.UpdateFOLV(this.folv_log, LogMan.Values, (Follow || AppSettings.Settings.Autoscroll_log), FullRefresh: true);
                        });
                    }
                    else
                    {
                        if (removed.Count > 0)
                            this.folv_log.RemoveObjects(removed);

                        if (added.Count > 0)
                            Global_GUI.UpdateFOLV(this.folv_log, added, (Follow || AppSettings.Settings.Autoscroll_log));
                    }


                    if (sw.ElapsedMilliseconds > 5000 && sw.ElapsedMilliseconds < 10000)
                    {
                        Log($"Debug: ---- Log window update took {sw.ElapsedMilliseconds}ms to load. {added.Count} added, {removed.Count} removed, {this.folv_history.Items.Count} total. Consider lowering the '{nameof(AppSettings.Settings.MaxGUILogItems)}' setting in JSON file (Currently {AppSettings.Settings.MaxGUILogItems}) ");
                    }
                    else if (sw.ElapsedMilliseconds > 10000)
                    {
                        Log($"Warn: ---- Log window update took {sw.ElapsedMilliseconds}ms to load. {added.Count} added, {removed.Count} removed, {this.folv_history.Items.Count} total. Consider lowering the '{nameof(AppSettings.Settings.MaxGUILogItems)}' setting in JSON file (Currently {AppSettings.Settings.MaxGUILogItems}) ");
                    }

                    this.UpdateStats();


                });

                //});

                this.IsLogListUpdating = false;

            }
            else
            {

            }
        }
        async Task UpdateHistoryAddedRemoved()
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.
            //Log("===Enter");
            //this should be a quicker full list update
            if (AppSettings.Settings.HistoryAutoRefresh &&
                !this.IsHistoryListUpdating &&
                this.tabControl1.SelectedIndex == 2 &&
                this.Visible &&
                !(this.WindowState == FormWindowState.Minimized) &&
                (DateTime.Now - this.LastListUpdate).TotalMilliseconds >= AppSettings.Settings.TimeBetweenListRefreshsMS &&
                this.DatabaseInitialized &&
                !IsLoading &&
                await HistoryDB.HasUpdates())
            {
                // run in another thread so gui doesnt freeze
                await Task.Run(() =>
                {

                    this.IsHistoryListUpdating = true;

                    Global_GUI.InvokeIFRequired(this.folv_history, () =>
                    {

                        //Log($"Debug:  Updating list...({AddedHistoryItems.Count} added, {DeletedHistoryItems.Count} deleted)");

                        //UpdateToolstrip("Updating list...");

                        List<History> added = HistoryDB.GetRecentlyAdded();
                        List<History> removed = HistoryDB.GetRecentlyDeleted();

                        if (removed.Count > 0)
                            this.folv_history.RemoveObjects(removed);

                        if (added.Count > 0)
                        {
                            //find the last item that is unfiltered:
                            History lasthist = null;
                            for (int i = added.Count - 1; i >= 0; i--)
                            {
                                if (this.checkListFilters(added[i]))
                                {
                                    lasthist = added[i];
                                    break;
                                }
                            }

                            Global_GUI.UpdateFOLV(this.folv_history, added, AppSettings.Settings.HistoryFollow, UseSelected: true, SelectObject: lasthist);

                        }

                        if (AppSettings.Settings.HistoryFollow && this.folv_history.SelectedObject == null && this.folv_history.GetItemCount() > 0)
                        {
                            if (this.folv_history.IsFiltering)
                            {
                                //use the last filtered object as the selected object
                                this.folv_history.SelectedObject = this.folv_history.FilteredObjects.Cast<object>().Last();
                            }
                            else if (!this.folv_history.IsFiltering)
                            {
                                this.folv_history.SelectedObject = this.folv_history.Objects.Cast<object>().Last();
                            }

                            if (this.folv_history.SelectedObject != null)
                                this.folv_history.EnsureModelVisible(this.folv_history.SelectedObject);

                        }

                        this.LastListUpdate = DateTime.Now;

                        this.IsHistoryListUpdating = false;

                        //UpdateToolstrip("");

                    });

                });

            }
            else
            {
                //Log($"Debug: List not updated - Refresh={AppSettings.Settings.HistoryAutoRefresh}, Visible={tabControl1.SelectedIndex == 2 && this.Visible && !(this.WindowState == FormWindowState.Minimized)}, IsListUpdating={this.IsListUpdating}, LastListUpdateMS={(DateTime.Now - this.LastListUpdate).TotalMilliseconds}");
            }
            //Log("===Exit");


        }

        private void HistoryStartStop()
        {
            if (AppSettings.Settings.HistoryAutoRefresh)
            {
                this.HistoryUpdateListTimer.Enabled = true;
                this.HistoryUpdateListTimer.Start();
                Log("Debug: History update timer started.");
            }
            else
            {
                this.HistoryUpdateListTimer.Enabled = false;
                this.HistoryUpdateListTimer.Stop();
                Log("Debug: History update timer stopped.");
            }
        }

        async void EventMessage(ClsMessage msg)
        {
            if (IsClosing)
                return;

            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            //output messages from the deepstack, blueiris, etc class to the text log window and log file
            if (msg.MessageType == MessageType.LogEntry)
            {
                Log(msg.Description, "");
            }
            else if (msg.MessageType == MessageType.DatabaseInitialized)
            {

                Log("debug: Database initialized.");

                this.folv_history.EmptyListMsg = "No History";

                this.DatabaseInitialized = true;
                await this.LoadHistoryAsync(true, true);
                this.HistoryStartStop();

            }
            else if (msg.MessageType == MessageType.UpdateDeepstackStatus)
            {

                Log($"debug: Deepstack control message '{msg.Description}'");

                LoadDeepStackTab();

            }
            else if (msg.MessageType == MessageType.CreateHistoryItem)
            {
                JsonSerializerSettings jset = new JsonSerializerSettings { };
                jset.TypeNameHandling = TypeNameHandling.All;
                jset.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                jset.ContractResolver = Global.JSONContractResolver;

                History hist = JsonConvert.DeserializeObject<History>(msg.JSONPayload, jset);


                if (!HistoryDB.ReadOnly)
                {
                    bool StoreMasked = hist.Success || !hist.WasMasked || (hist.WasMasked && AppSettings.Settings.HistoryStoreMaskedAlerts);
                    bool StoreFalse = hist.Success || !(hist.Detections.IndexOf("false alert", StringComparison.OrdinalIgnoreCase) >= 0) || (hist.Detections.IndexOf("false alert", StringComparison.OrdinalIgnoreCase) >= 0 && AppSettings.Settings.HistoryStoreFalseAlerts);
                    bool Save = true;
                    if (!StoreMasked || !StoreFalse)
                        Save = false;

                    if (Save)
                    {
                        HistoryDB.InsertHistoryQueue(hist);
                    }
                    else
                    {
                        Log($"debug: Not storing item in db - StoreMasked={StoreMasked}, StoreFalse={StoreFalse}: {hist.Detections}");
                    }
                }

                //UpdateHistoryAddedRemoved();

                this.UpdateStats();
            }
            else if (msg.MessageType == MessageType.DeleteHistoryItem)
            {

                if (!HistoryDB.ReadOnly)  //assume service or other instance will be handling 
                {
                    HistoryDB.DeleteHistoryQueue(msg.Description);
                }

                //UpdateHistoryAddedRemoved();

                this.UpdateStats();

            }
            else if (msg.MessageType == MessageType.ImageAddedToQueue)
            {
                this.UpdateStats();
            }
            else if (msg.MessageType == MessageType.UpdateStatus)
            {
                this.UpdateStats();
            }
            else if (msg.MessageType == MessageType.UpdateProgressBar)
            {

                this.UpdateProgressBar(msg.Description, msg.CurVal, msg.MinVal, msg.MaxVal);

            }
            else if (msg.MessageType == MessageType.BeginProcessImage)
            {
                this.BeginProcessImage(msg.Description);
            }
            else if (msg.MessageType == MessageType.EndProcessImage)
            {
                this.EndProcessImage(msg.Description);
                this.UpdateStats();
            }
            else if (msg.MessageType == MessageType.UpdateLabel)
            {
                string lblcontrolname = (string)Global.SetJSONString<object>(msg.JSONPayload);

                if (!string.IsNullOrWhiteSpace(lblcontrolname))
                {
                    Label lbl = null;
                    try
                    {
                        lbl = this.Controls.Find(lblcontrolname, true).FirstOrDefault() as Label;
                    }
                    catch (Exception ex)
                    {

                        Log($"Error: Could not find label '{lblcontrolname}': {ex.Msg()}");
                    }

                    if (lbl != null)
                    {
                        Global_GUI.InvokeIFRequired(lbl, () =>
                        {
                            lbl.Show();
                            lbl.Text = msg.Description;
                        });

                    }
                    else
                    {
                        Log($"Error: Could not find label '{lblcontrolname}'?");

                    }

                }
                else
                {
                    Log($"Error: No label name passed - '{msg.Description}'");

                }

                this.UpdateStats();
            }
            else
            {
                Log($"Error: Unhandled message type '{msg.MessageType}'");
            }
        }

        private void UpdateProgressBar(string Message, int CurVal = -1, int MinVal = -1, int MaxVal = -1)
        {

            if (IsClosing)
                return;

            if (this.SplashScreen != null)
            {
                Global_GUI.InvokeIFRequired(this.SplashScreen.progressBar, () =>
                {

                    if (MaxVal != -1 && this.SplashScreen.progressBar.Maximum != MaxVal)
                        this.SplashScreen.progressBar.Maximum = MaxVal;

                    if (MinVal != -1 && this.SplashScreen.progressBar.Minimum != MinVal)
                        this.SplashScreen.progressBar.Minimum = MinVal;

                    if (this.SplashScreen.progressBar.Style != ProgressBarStyle.Continuous)
                        this.SplashScreen.progressBar.Style = ProgressBarStyle.Continuous;

                    if (CurVal == 1 && MinVal == 1 && MaxVal == 1)
                    {
                        this.SplashScreen.progressBar.Maximum = 2;
                        this.SplashScreen.progressBar.Value = this.SplashScreen.progressBar.Maximum;

                    }
                    else
                    {

                        if (CurVal > -1)
                        {
                            if (CurVal >= this.SplashScreen.progressBar.Minimum && CurVal <= this.SplashScreen.progressBar.Maximum)
                                this.SplashScreen.progressBar.Value = CurVal;
                            if (CurVal < this.SplashScreen.progressBar.Minimum)
                                this.SplashScreen.progressBar.Value = this.SplashScreen.progressBar.Minimum;
                            if (CurVal > this.SplashScreen.progressBar.Maximum)
                                this.SplashScreen.progressBar.Value = this.SplashScreen.progressBar.Maximum;
                        }
                    }

                    string msg = Global.ReplaceCaseInsensitive(Message, "debug:", "").Trim();
                    if (msg != this.SplashScreen.lbl_status.Text)
                    {
                        this.SplashScreen.lbl_status.Text = msg;
                        //SplashScreen.Refresh();
                    }

                    //SplashScreen.progressBar.Refresh();
                    //Application.DoEvents();  //causes all sorts of issues

                });

            }
            else
            {
                Global_GUI.InvokeIFRequired(this.toolStripProgressBar1.GetCurrentParent(), () =>
                {

                    if (MaxVal != -1 && this.toolStripProgressBar1.Maximum != MaxVal)
                        this.toolStripProgressBar1.Maximum = MaxVal;

                    if (MinVal != -1 && this.toolStripProgressBar1.Minimum != MinVal)
                        this.toolStripProgressBar1.Minimum = MinVal;

                    if (this.toolStripProgressBar1.Style != ProgressBarStyle.Continuous)
                        this.toolStripProgressBar1.Style = ProgressBarStyle.Continuous;

                    if (CurVal == 1 && MinVal == 1 && MaxVal == 1)
                    {
                        this.toolStripProgressBar1.Maximum = 2;
                        this.toolStripProgressBar1.Value = this.toolStripProgressBar1.Maximum;

                    }
                    else
                    {

                        if (CurVal > -1)
                        {
                            if (CurVal >= this.toolStripProgressBar1.Minimum && CurVal <= this.toolStripProgressBar1.Maximum)
                                this.toolStripProgressBar1.Value = CurVal;
                            if (CurVal < this.toolStripProgressBar1.Minimum)
                                this.toolStripProgressBar1.Value = this.toolStripProgressBar1.Minimum;
                            if (CurVal > this.toolStripProgressBar1.Maximum)
                                this.toolStripProgressBar1.Value = this.toolStripProgressBar1.Maximum;
                        }
                    }

                    if (this.toolStripProgressBar1.Value > 0 && !this.Visible)
                    {
                        this.Visible = true;
                    }
                    else if (this.toolStripProgressBar1.Value == 0 && this.Visible)
                    {
                        this.Visible = false;
                    }

                    this.toolStripProgressBar1.GetCurrentParent().Refresh();
                    //Application.DoEvents();  //causes all sorts of issues

                });


                this.UpdateStats(Message);

            }

        }

        //----------------------------------------------------------------------------------------------------------
        //CORE
        //----------------------------------------------------------------------------------------------------------


        //add text to log
        //public async void Log(string text, [CallerMemberName] string memberName = null)
        //{

        //    if (IsClosing)
        //        return;

        //    try
        //    {

        //        //get current date and time

        //        string time = DateTime.Now.ToString("dd.MM.yyyy, HH:mm:ss");
        //        string rtftime = DateTime.Now.ToString("dHH:mm:ss");  //no need for date in log tab
        //        string ModName = "";
        //        if (memberName == ".ctor")
        //            memberName = "Constructor";

        //        if (AppSettings.Settings.log_everything == true || AppSettings.Settings.deepstack_debug)
        //        {
        //            time = DateTime.Now.ToString("dd.MM.yyyy, HH:mm:ss.fff");
        //            rtftime = DateTime.Now.ToString("HH:mm:ss.fff");
        //            if (memberName != null && !string.IsNullOrEmpty(memberName))
        //                ModName = memberName.PadLeft(24) + "> ";

        //            //when the global logger reports back to the progress logger we cant use CallerMemberName, so extract the member name from text

        //            int gg = text.IndexOf(">> ");

        //            if (gg > 0 && gg <= 24)
        //            {
        //                string modfromglobal = Global.GetWordBetween(text, "", ">> ");
        //                if (!string.IsNullOrEmpty(modfromglobal))
        //                {
        //                    ModName = modfromglobal.PadLeft(24) + "> ";
        //                    text = Global.GetWordBetween(text, ">> ", "");
        //                }

        //            }
        //        }

        //        //check for messages coming from deepstack processes and kill them if we didnt ask for debugging messages
        //        if (!AppSettings.Settings.deepstack_debug)
        //        {
        //            if (text.ToLower().Contains("redis-server.exe>") || text.ToLower().Contains("python.exe>"))
        //            {
        //                return;
        //            }
        //        }

        //        //make the error and warning detection case insensitive:
        //        bool HasError = (text.IndexOf("error", StringComparison.InvariantCultureIgnoreCase) > -1) || (text.IndexOf("exception", StringComparison.InvariantCultureIgnoreCase) > -1);
        //        bool HasWarning = (text.IndexOf("warning:", StringComparison.InvariantCultureIgnoreCase) > -1);
        //        bool HasInfo = (text.IndexOf("info:", StringComparison.InvariantCultureIgnoreCase) > -1);
        //        bool HasDebug = (text.IndexOf("debug:", StringComparison.InvariantCultureIgnoreCase) > -1);
        //        bool IsDeepStackMsg = (memberName.IndexOf("deepstack", StringComparison.InvariantCultureIgnoreCase) > -1) || (text.IndexOf("deepstack", StringComparison.InvariantCultureIgnoreCase) > -1) || (ModName.IndexOf("deepstack", StringComparison.InvariantCultureIgnoreCase) > -1);

        //        string RTFText = "";

        //        //set the color for RTF text window:
        //        if (HasError)
        //        {
        //            RTFText = $"{{gray}}[{rtftime}]: {ModName}{{red}}{text}";
        //        }
        //        else if (HasWarning)
        //        {
        //            RTFText = $"{{gray}}[{rtftime}]: {ModName}{{mediumorchid}}{text}";
        //        }
        //        else if (IsDeepStackMsg)
        //        {
        //            RTFText = $"{{gray}}[{rtftime}]: {ModName}{{lime}}{text}";
        //        }
        //        else if (HasInfo)
        //        {
        //            RTFText = $"{{gray}}[{rtftime}]: {ModName}{{yellow}}{text}";
        //        }
        //        else if (HasDebug)
        //        {
        //            RTFText = $"{{gray}}[{rtftime}]: {ModName}{text}";
        //        }
        //        else
        //        {
        //            RTFText = $"{{gray}}[{rtftime}]: {ModName}{{white}}{text}";
        //        }

        //        if (!AppSettings.AlreadyRunning)
        //        {
        //            Global.SaveSetting("LastLogEntry", RTFText);
        //            Global.SaveSetting("LastShutdownState", $"checkpoint: GUI.Log: {DateTime.Now}");
        //        }

        //        //get rid of any common color coding before logging to file or console
        //        text = text.Replace("{yellow}", "").Replace("{red}", "").Replace("{white}", "").Replace("{orange}", "").Replace("{lime}", "").Replace("{orange}", "mediumorchid");

        //        //if log everything is disabled and the text is neither an ERROR, nor a WARNING: write only to console and ABORT
        //        if (AppSettings.Settings.log_everything == false && !HasError && !HasWarning)
        //        {
        //            //Creates a lot of extra text in immediate window while debugging, disabling -Vorlon
        //            //text += "Enabling \'Log everything\' might give more information.";
        //            Console.WriteLine($"[{rtftime}]: {ModName}{text}");

        //            return;
        //        }



        //        RTFLogger.LogToRTF(RTFText);
        //        LogWriter.WriteToLog($"[{time}]:  {ModName}{text}", HasError);



        //                        //add log text to console


        //    }
        //    catch (Exception ex)
        //    {

        //        Console.WriteLine("Error: In LOG, got: " + ex.Message);
        //    }

        //}



        //----------------------------------------------------------------------------------------------------------
        //GUI
        //----------------------------------------------------------------------------------------------------------

        //minimize to tray
        private void Form1_Resize(object sender, EventArgs e)
        {

            if (IsLoading)
                return;

            if (IsClosing)
                return;

            if (this.WindowState == FormWindowState.Minimized)
            {
                HideForm();
            }

            UpdateErrorIcon();

        }

        //open from tray
        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ShowForm();
        }

        //protected override void SetVisibleCore(bool value)
        //{
        //    base.SetVisibleCore(AllowShowDisplay ? value : AllowShowDisplay);
        //}

        private async void ShowForm()
        {

            if (IsClosing)
                return;

            try
            {
                //this.AllowShowDisplay = true;
                this.Opacity = 100;
                this.TopMost = true;
                this.TopMost = false;
                this.Show();
                this.WindowState = FormWindowState.Normal;
                this.ShowInTaskbar = true;
                this.notifyIcon.Visible = false;
                this.LoadHistoryAsync(true, true);
                this.UpdateLogAddedRemovedAsync(true, true);
                this.Activate();
                Application.DoEvents();

            }
            catch { }
        }

        private async void HideForm()
        {

            if (IsClosing)
                return;

            if (!AppSettings.Settings.MinimizeToTray)
                return;

            try
            {
                this.Hide();
                this.Opacity = 0;
                this.notifyIcon.Visible = true;
                //Log($"Debug: Hiding app. Visible={this.Visible}, state={this.WindowState}, tbicon={this.ShowInTaskbar}, trayicon={this.notifyIcon.Visible}");
                Application.DoEvents();

            }
            catch { }

        }
        //open Log when clicking or error message
        private void lbl_errors_Click(object sender, EventArgs e)
        {
            this.ShowErrors();

        }

        private void ShowErrors()
        {

            //filter the main log list for errors
            this.chk_filterErrors.Checked = true;
            this.tabControl1.SelectedTab = this.tabLog;
            this.FilterLogErrors();
        }


        //EVENTS:

        //event: mouse click on tab control
        private void tabControl1_MouseDown(object sender, MouseEventArgs e)
        {
        }

        //event: another tab selected (Only load certain things in tabs if they are actually open)
        private async void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Application.DoEvents();
            using var Trace = new Trace();

            try
            {
                if (this.tabControl1.SelectedIndex == 1)
                {
                    this.UpdatePieChart(); this.UpdateTimeline(); this.UpdateConfidenceChart();
                }
                else if (this.tabControl1.SelectedIndex == 2)
                {
                    await this.LoadHistoryAsync(true, true);
                }
                else if (this.tabControl1.SelectedIndex == 3)
                {
                    this.LoadCameras();
                }
                else if (this.tabControl1.SelectedIndex == 4)
                {
                    this.LoadSettingsTab();
                }
                else if (this.tabControl1.SelectedTab == this.tabControl1.TabPages["tabDeepStack"])
                {
                    this.LoadDeepStackTab();
                }
                else if (this.tabControl1.SelectedTab == this.tabControl1.TabPages["tabLog"])
                {
                    //scroll to bottom, only when tab is active for better performance 
                    if (!this.toolStripButtonPauseLog.Checked)
                    {
                        await this.UpdateLogAddedRemovedAsync(true, true);
                    }
                }
                UpdateStats();

            }
            catch (Exception ex)
            {
                string err = $"Error: While changing to tab {this.tabControl1.SelectedTab.Name}, got error: {ex.Msg()}";
                Log(err);
                MessageBox.Show(err);

            }

        }


        //----------------------------------------------------------------------------------------------------------
        //STATS TAB
        //----------------------------------------------------------------------------------------------------------

        //other camera in combobox selected, display according PieChart
        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        //update pie chart
        public void UpdatePieChart()
        {

            if (this.tabControl1.SelectedIndex != 1 || !this.Visible || this.WindowState == FormWindowState.Minimized || string.IsNullOrEmpty(this.comboBox1.Text))
                return;

            int alerts = 0;
            int irrelevantalerts = 0;
            int falsealerts = 0;
            int skipped = 0;
            Series ser = this.chart1.Series[0];


            if (this.comboBox1.Text == "All Cameras")
            {
                foreach (Camera cam in AppSettings.Settings.CameraList)
                {
                    alerts += cam.stats_alerts;
                    irrelevantalerts += cam.stats_irrelevant_alerts;
                    falsealerts += cam.stats_false_alerts;
                    skipped += cam.stats_skipped_images;
                }
            }
            else
            {

                Camera cam = AITOOL.GetCamera(this.comboBox1.Text);  //int i = AppSettings.Settings.CameraList.FindIndex(x => x.name.ToLower().Trim() == comboBox1.Text.ToLower().Trim());
                if (cam != null)
                {
                    alerts = cam.stats_alerts;
                    irrelevantalerts = cam.stats_irrelevant_alerts;
                    falsealerts = cam.stats_false_alerts;
                    skipped = cam.stats_skipped_images;
                }
                else
                {
                    alerts = 0;
                    irrelevantalerts = 0;
                    falsealerts = 0;
                    skipped = 0;
                    Log($"Error: Could not match combobox dropdown '{this.comboBox1.Text}' to a known camera name?");
                }
            }

            ser.Points.Clear();
            ser.IsVisibleInLegend = true;


            ser.LegendText = "#LABEL"; //"#VALY"; //"#VALY #VALX"
            //ser["PieLabelStyle"] = "Disabled";

            int index = -1;

            //show Alerts label
            index = ser.Points.AddXY("Alerts", alerts);
            ser.Points[index].Color = System.Drawing.Color.Green;
            ser.Points[index].LegendText = $"{alerts.ToString("00000")} - Alerts";
            ser.Points[index].Label = "Alerts";

            //show irrelevant Alerts label
            index = ser.Points.AddXY("Irrelevant Alerts", irrelevantalerts);
            ser.Points[index].Color = System.Drawing.Color.Orange;
            ser.Points[index].LegendText = $"{irrelevantalerts.ToString("00000")} - Irrelevant Alerts";
            ser.Points[index].Label = "Irrelevant";

            //show false Alerts label
            index = ser.Points.AddXY("False Alerts", falsealerts);
            ser.Points[index].Color = System.Drawing.Color.OrangeRed;
            ser.Points[index].LegendText = $"{falsealerts.ToString("00000")} - False Alerts";
            ser.Points[index].Label = "False";

            //show skipped label
            index = ser.Points.AddXY("Skipped Images", skipped);
            ser.Points[index].Color = System.Drawing.Color.Purple;
            ser.Points[index].LegendText = $"{skipped.ToString("00000")} - Skipped Images";
            ser.Points[index].Label = "Skipped";



        }

        //update timeline
        public void UpdateTimeline()
        {

            if (this.tabControl1.SelectedIndex != 1 || !this.Visible || this.WindowState == FormWindowState.Minimized || string.IsNullOrEmpty(this.comboBox1.Text))
                return;

            Log("Debug: Loading time line from history database ...");

            //clear previous values
            this.timeline.Series[0].Points.Clear();
            this.timeline.Series[1].Points.Clear();
            this.timeline.Series[2].Points.Clear();
            this.timeline.Series[3].Points.Clear();
            this.timeline.Series[4].Points.Clear();

            Stopwatch SW = Stopwatch.StartNew();

            try
            {
                List<History> result = HistoryDB.GetAllValues();

                if (!string.Equals(this.comboBox1.Text.Trim(), "All Cameras", StringComparison.OrdinalIgnoreCase)) //all cameras selected
                {
                    result = result.Where(hist => hist.Camera.StartsWith(this.comboBox1.Text.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();
                }

                //every int represents the number of ai calls in successive half hours (p.e. relevant[0] is 0:00-0:30 o'clock, relevant[1] is 0:30-1:00 o'clock) 
                int[] all = new int[48];
                int[] falses = new int[48];
                int[] irrelevant = new int[48];
                int[] relevant = new int[48];
                int[] skipped = new int[48];

                //fill arrays with amount of calls/half hour
                foreach (History hist in result)
                {               //example of time column entry: 23.08.19, 18:31:09
                                //get hour
                    int hour = hist.Date.Hour;

                    //get minute
                    int minute = hist.Date.Minute;

                    int halfhour; //stores the half hour in which the alert occurred

                    //add +1 to counter for corresponding half-hour
                    if (minute >= 30) //if alert occurred after half o clock
                    {
                        halfhour = hour * 2 + 1;
                    }
                    else //if alert occured before half o clock
                    {
                        halfhour = hour * 2;
                    }

                    //if detection was successful
                    if (hist.Success)
                    {
                        relevant[halfhour]++;
                    }
                    //if it was a false alert
                    else if (string.Equals(hist.Detections, "false alert", StringComparison.OrdinalIgnoreCase))
                    {
                        falses[halfhour]++;
                    }
                    //if something irrelevant was detected
                    else
                    {
                        irrelevant[halfhour]++;
                    }

                    if (hist.WasSkipped)
                        skipped[halfhour]++;

                    all[halfhour]++;
                }

                //add to graph "all":

                /*the graph will have a gap at the end and at the beginning if we don'f specify a value
                * with an x value outside the visible area at the end and before the first visible point. 
                * So the first point is at -0.25 and has the value of the last visible point and the 
                * last point is at 24.25 and has the value of the first visible point. */

                this.timeline.Series[0].Points.AddXY(-0.25, all[47]); // beginning point with value of last visible point

                //and now add all visible points 
                double x = 0.25;
                foreach (int halfhour in all)
                {
                    int index = this.timeline.Series[0].Points.AddXY(x, halfhour);
                    x = x + 0.5;
                }

                this.timeline.Series[0].Points.AddXY(24.25, all[0]); // finally add last point with value of first visible point

                //add to graph "falses":

                this.timeline.Series[1].Points.AddXY(-0.25, falses[47]); // beginning point with value of last visible point
                                                                         //and now add all visible points 
                x = 0.25;
                foreach (int halfhour in falses)
                {
                    int index = this.timeline.Series[1].Points.AddXY(x, halfhour);
                    x = x + 0.5;
                }
                this.timeline.Series[1].Points.AddXY(24.25, falses[0]); // finally add last point with value of first visible point

                //add to graph "irrelevant":

                this.timeline.Series[2].Points.AddXY(-0.25, irrelevant[47]); // beginning point with value of last visible point
                                                                             //and now add all visible points 
                x = 0.25;
                foreach (int halfhour in irrelevant)
                {
                    int index = this.timeline.Series[2].Points.AddXY(x, halfhour);
                    x = x + 0.5;
                }
                this.timeline.Series[2].Points.AddXY(24.25, irrelevant[0]); // finally add last point with value of first visible point

                //add to graph "relevant":

                this.timeline.Series[3].Points.AddXY(-0.25, relevant[47]); // beginning point with value of last visible point
                                                                           //and now add all visible points 
                x = 0.25;
                foreach (int halfhour in relevant)
                {
                    int index = this.timeline.Series[3].Points.AddXY(x, halfhour);
                    x = x + 0.5;
                }
                this.timeline.Series[3].Points.AddXY(24.25, relevant[0]); // finally add last point with value of first visible point


                //add to graph "skipped":

                this.timeline.Series[4].Points.AddXY(-0.25, skipped[47]); // beginning point with value of last visible point
                                                                          //and now add all visible points 
                x = 0.25;
                foreach (int halfhour in skipped)
                {
                    int index = this.timeline.Series[4].Points.AddXY(x, halfhour);
                    x = x + 0.5;
                }
                this.timeline.Series[4].Points.AddXY(24.25, skipped[0]); // finally add last point with value of first visible point



            }
            catch (Exception ex)
            {

                Log("Error: " + ex.Msg());
            }




        }

        //update confidence_frequency chart
        public void UpdateConfidenceChart()
        {

            if (this.tabControl1.SelectedIndex != 1 || !this.Visible || this.WindowState == FormWindowState.Minimized || string.IsNullOrEmpty(this.comboBox1.Text))
                return;


            Log("Debug: Loading confidence-frequency chart from history database ...");

            //clear previous values
            this.chart_confidence.Series[0].Points.Clear();
            this.chart_confidence.Series[1].Points.Clear();


            Stopwatch SW = Stopwatch.StartNew();

            try
            {
                List<History> result = HistoryDB.GetAllValues();

                if (!string.Equals(this.comboBox1.Text.Trim(), "All Cameras", StringComparison.OrdinalIgnoreCase)) //all cameras selected
                {
                    result = result.Where(hist => hist.Camera.StartsWith(this.comboBox1.Text.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();
                }

                //this array stores the Absolute frequencies of all possible confidence values (0%-100%)
                int[] green_values = new int[101];
                int[] orange_values = new int[101];

                //fill array with frequencies
                foreach (History hist in result)
                {
                    //example of detections column entry: "person (41%); person (97%);" or "masked: person (41%); person (97%);"
                    string detections_column = hist.Detections;
                    if (detections_column.Contains(':'))
                    {
                        detections_column = detections_column.Split(':')[1];

                        string[] detections = detections_column.Split(';');

                        //write the confidence of every detection into the green_values string
                        foreach (string detection in detections)
                        {
                            int x_value = Global.GetNumberInt(detection);  // gets a number anywhere in the string

                            //fix temp bug from earlier where whole number percentages were multiplied by 100 when they shouldnt have been.
                            if (x_value > 500)
                                x_value = x_value / 100;

                            if (x_value > 0 && x_value <= orange_values.Count() - 1)
                            {
                                //example: -> "person (41%)"
                                //Int32.TryParse(detection.Split('(')[1].Split('%')[0], out int x_value); //example: -> "41"
                                orange_values[x_value]++;
                            }
                        }
                    }
                    else
                    {
                        string[] detections = detections_column.Split(';');

                        //write the confidence of every detection into the green_values string
                        foreach (string detection in detections)
                        {
                            int x_value = Global.GetNumberInt(detection);  // gets first number anywhere in the string - TODO: May not be correct
                            if (x_value > 0 && x_value <= green_values.Count() - 1)
                            {
                                //example: -> "person (41%)"
                                //Int32.TryParse(detection.Split('(')[1].Split('%')[0], out int x_value); //example: -> "41"
                                green_values[x_value]++;
                            }
                        }
                    }
                }


                //write green series in chart
                int i = 0;
                foreach (int y_value in green_values)
                {
                    this.chart_confidence.Series[1].Points.AddXY(i, y_value);
                    i++;
                }

                //write orange series in chart
                i = 0;
                foreach (int y_value in orange_values)
                {
                    this.chart_confidence.Series[0].Points.AddXY(i, y_value);
                    i++;
                }



            }
            catch (Exception ex)
            {

                Log("Error: " + ex.Msg());
            }


        }


        private void showHideMask()
        {
            if (this.cb_showMask.Checked == true) //show overlay
            {
                //Log("Show mask toggled.");

                if (this.folv_history.SelectedObjects != null && this.folv_history.SelectedObjects.Count > 0)
                {
                    History hist = (History)this.folv_history.SelectedObjects[0];

                    string imagefile = AITOOL.GetMaskFile(hist.Camera);

                    if (File.Exists(imagefile))
                    {
                        using (var img = new Bitmap(imagefile))
                        {
                            this.pictureBox1.Image = new Bitmap(img); //load mask as overlay
                        }
                    }
                    else
                    {
                        this.pictureBox1.Image = null; //if file does not exist, empty mask overlay (from possible overlays of previous images)
                    }

                }

            }
            else //if showmask toggle-button is not checked, hide the mask overlay
            {
                this.pictureBox1.Image = null;
            }

        }

        //show rectangle overlay
        //        private void showObject(PaintEventArgs e, double _xmin, double _ymin, double _xmax, double _ymax, string text, ResultType result)
        //        {
        //            try
        //            {
        //                if (this.folv_history.SelectedObjects != null && this.folv_history.SelectedObjects.Count > 0 && (this.pictureBox1 != null) && (this.pictureBox1.BackgroundImage != null))
        //                {

        //                    System.Drawing.Color color = new System.Drawing.Color();
        //                    int BorderWidth = AppSettings.Settings.RectBorderWidth
        //;

        //                    if (result == ResultType.Relevant)
        //                    {
        //                        color = System.Drawing.Color.FromArgb(AppSettings.Settings.RectRelevantColorAlpha, AppSettings.Settings.RectRelevantColor);
        //                    }
        //                    else if (result == ResultType.DynamicMasked || result == ResultType.ImageMasked || result == ResultType.StaticMasked)
        //                    {
        //                        color = System.Drawing.Color.FromArgb(AppSettings.Settings.RectMaskedColorAlpha, AppSettings.Settings.RectMaskedColor);
        //                    }
        //                    else
        //                    {
        //                        color = System.Drawing.Color.FromArgb(AppSettings.Settings.RectIrrelevantColorAlpha, AppSettings.Settings.RectIrrelevantColor);
        //                    }

        //                    //1. get the padding between the image and the picturebox border

        //                    //get dimensions of the image and the picturebox
        //                    double imgWidth = this.pictureBox1.BackgroundImage.Width;
        //                    double imgHeight = this.pictureBox1.BackgroundImage.Height;
        //                    double boxWidth = this.pictureBox1.Width;
        //                    double boxHeight = this.pictureBox1.Height;
        //                    //double clnWidth = this.pictureBox1.ClientSize.Width;
        //                    //double clnHeight = this.pictureBox1.ClientSize.Height;
        //                    //double rctWidth = this.pictureBox1.ClientRectangle.Width;
        //                    //double rctHeight = this.pictureBox1.ClientRectangle.Height;

        //                    //these variables store the padding between image border and picturebox border
        //                    double absX = 0;
        //                    double absY = 0;

        //                    //because the sizemode of the picturebox is set to 'zoom', the image is scaled down
        //                    double scale = 1;


        //                    //Comparing the aspect ratio of both the control and the image itself.
        //                    if (imgWidth / imgHeight > boxWidth / boxHeight) //if the image is p.e. 16:9 and the picturebox is 4:3
        //                    {
        //                        scale = boxWidth / imgWidth; //get scale factor
        //                        absY = (boxHeight - scale * imgHeight) / 2; //padding on top and below the image
        //                    }
        //                    else //if the image is p.e. 4:3 and the picturebox is widescreen 16:9
        //                    {
        //                        scale = boxHeight / imgHeight; //get scale factor
        //                        absX = (boxWidth - scale * imgWidth) / 2; //padding left and right of the image
        //                    }

        //                    //2. inputted position values are for the original image size. As the image is probably smaller in the picturebox, the positions must be adapted. 
        //                    double xmin = (scale * _xmin) + absX;
        //                    double xmax = (scale * _xmax) + absX;
        //                    double ymin = (scale * _ymin) + absY;
        //                    double ymax = (scale * _ymax) + absY;

        //                    double sclWidth = xmax - xmin;
        //                    double sclHeight = ymax - ymin;

        //                    double sclxmax = boxWidth - (absX * 2);
        //                    double sclymax = boxHeight - (absY * 2);
        //                    double sclxmin = absX;
        //                    double sclymin = absY;

        //                    //3. paint rectangle
        //                    System.Drawing.Rectangle rect = new System.Drawing.Rectangle(xmin.ToInt(),
        //                                                                                 ymin.ToInt(),
        //                                                                                 sclWidth.ToInt(),
        //                                                                                 sclHeight.ToInt());
        //                    using (Pen pen = new Pen(color, BorderWidth))
        //                    {
        //                        e.Graphics.DrawRectangle(pen, rect); //draw rectangle
        //                    }


        //                    ///testing=================================================
        //                    //3. paint rectangle
        //                    //rect = new System.Drawing.Rectangle(absX + 5,
        //                    //                                    absY + 5,
        //                    //                                    sclxmax - 10,
        //                    //                                    sclymax - 10);

        //                    //using (Pen pen = new Pen(Color.Red, BorderWidth))
        //                    //{
        //                    //    e.Graphics.DrawRectangle(pen, rect); //draw rectangle
        //                    //}
        //                    ///testing=================================================

        //                    //we need this since people can change the border width in the json file
        //                    double halfbrd = BorderWidth / 2;


        //                    System.Drawing.SizeF TextSize = e.Graphics.MeasureString(text, new Font(AppSettings.Settings.RectDetectionTextFont, AppSettings.Settings.RectDetectionTextSize)); //finds size of text to draw the background rectangle


        //                    //object name text below rectangle

        //                    double x = xmin - halfbrd;
        //                    double y = ymax + halfbrd;


        //                    //adjust the x / width label so it doesnt go off screen
        //                    double EndX = x + TextSize.Width;
        //                    if (EndX > sclxmax)
        //                    {
        //                        //int diffx = x - sclxmax;
        //                        x = xmax - TextSize.Width + halfbrd;
        //                    }

        //                    if (x < sclxmin)
        //                        x = sclxmin;

        //                    if (x < 0)
        //                        x = 0;

        //                    //adjust the y / height label so it doesnt go off screen
        //                    double EndY = y + TextSize.Height;
        //                    if (EndY > sclymax)
        //                    {
        //                        //float diffy = EndY - sclymax;
        //                        y = ymax - TextSize.Height - halfbrd;
        //                    }

        //                    if (y < 0)
        //                        y = 0;


        //                    rect = new System.Drawing.Rectangle(x.ToInt(),
        //                                                        y.ToInt(),
        //                                                        boxWidth.ToInt(),
        //                                                        boxHeight.ToInt()); //sets bounding box for drawn text


        //                    Brush brush = new SolidBrush(color); //sets background rectangle color
        //                    if (AppSettings.Settings.RectDetectionTextBackColor != Color.Gainsboro)
        //                        brush = new SolidBrush(AppSettings.Settings.RectDetectionTextBackColor);

        //                    Brush forecolor = Brushes.Black;
        //                    if (AppSettings.Settings.RectDetectionTextForeColor != Color.Gainsboro)
        //                        forecolor = new SolidBrush(AppSettings.Settings.RectDetectionTextForeColor);

        //                    e.Graphics.FillRectangle(brush,
        //                                             x.ToInt(),
        //                                             y.ToInt(),
        //                                             TextSize.Width,
        //                                             TextSize.Height); //draw grey background rectangle for detection text

        //                    e.Graphics.DrawString(text,
        //                                          new Font(AppSettings.Settings.RectDetectionTextFont,
        //                                          AppSettings.Settings.RectDetectionTextSize),
        //                                          forecolor,
        //                                          rect); //draw detection text


        //                }

        //            }
        //            catch (Exception ex)
        //            {

        //                Log("Error: " + ex.Msg());
        //            }
        //        }

        //load object rectangle overlays
        //TODO: refactor detections
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (this.pictureBox1.BackgroundImage.IsNull())
                return;

            if (AppSettings.Settings.HistoryShowObjects && this.folv_history.SelectedObjects != null && this.folv_history.SelectedObjects.Count > 0) //if checkbox button is enabled
            {
                //Log("Loading object rectangles...");
                History hist = (History)this.folv_history.SelectedObjects[0];

                if (hist == null)
                {
                    Log("Warn: Selected history item is null?");
                    return;
                }

                string positions = hist.Positions;
                string detections = hist.Detections;

                //int XOffset = 0;
                //int YOffset = 0;

                //Camera cam = AITOOL.GetCamera(hist.Camera);
                //if (cam != null)
                //{
                //    //apply offset if one is defined by user in json file
                //    XOffset = cam.XOffset;
                //    YOffset = cam.YOffset;
                //}

                try
                {

                    if (!string.IsNullOrEmpty(hist.PredictionsJSON))
                    {
                        List<ClsPrediction> predictions = hist.Predictions();

                        if (predictions.Count > 0)
                        {
                            //draw in reverse, assuming most important should be on top
                            for (int i = predictions.Count - 1; i >= 0; i--)
                            {
                                ClsPrediction pred = predictions[i];
                                if (pred != null)
                                {

                                    AITOOL.DrawAnnotation(e.Graphics,
                                                          pred,
                                                          this.pictureBox1.BackgroundImage.Width,
                                                          this.pictureBox1.BackgroundImage.Height,
                                                          this.pictureBox1.Width,
                                                          this.pictureBox1.Height);

                                    //if (AppSettings.Settings.HistoryOnlyDisplayRelevantObjects && pred.Result == ResultType.Relevant)
                                    //{
                                    //    this.showObject(e, pred.XMin + XOffset, pred.YMin + YOffset, pred.XMax, pred.YMax, pred.ToString(), pred.Result); //call rectangle drawing method, calls appropriate detection text
                                    //}
                                    //else if (!AppSettings.Settings.HistoryOnlyDisplayRelevantObjects)
                                    //{
                                    //    this.showObject(e, pred.XMin + XOffset, pred.YMin + YOffset, pred.XMax, pred.YMax, pred.ToString(), pred.Result); //call rectangle drawing method, calls appropriate detection text
                                    //}
                                }
                                else
                                {
                                    Log($"Warn: Prediction #{i + 1} of {predictions.Count} is empty?");

                                }

                            }
                        }
                        else
                        {
                            Log("Warn: No predictions for the selected history item.");
                        }

                    }
                    //else
                    //{

                    //    //we should never get here after all old AITOOL entries have been deleted
                    //    List<string> positionssArray = positions.SplitStr(";");//creates array of detected objects, used for adding text overlay

                    //    int countr = positionssArray.Count;

                    //    ResultType result = ResultType.Unknown;

                    //    if (detections.IndexOf("irrelevant", StringComparison.OrdinalIgnoreCase) >= 0 || detections.IndexOf("masked", StringComparison.OrdinalIgnoreCase) >= 0 || detections.IndexOf("confidence", StringComparison.OrdinalIgnoreCase) >= 0)
                    //    {
                    //        detections = detections.Split(':')[1]; //removes the "1x masked, 3x irrelevant:" before the actual detection, otherwise this would be displayed in the detection tags
                    //        if (detections.Contains("masked"))
                    //        {
                    //            result = ResultType.ImageMasked;
                    //        }
                    //        else
                    //        {
                    //            result = ResultType.Unknown;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        result = ResultType.Relevant;
                    //    }

                    //    List<string> detectionsArray = detections.SplitStr(";");//creates array of detected objects, used for adding text overlay

                    //    if (cam != null)
                    //    {
                    //        //apply offset if one is defined by user in json file
                    //        XOffset = cam.XOffset;
                    //        YOffset = cam.YOffset;
                    //    }

                    //    //display a rectangle around each relevant object
                    //    for (int i = 0; i < countr; i++)
                    //    {

                    //        //load 'xmin,ymin,xmax,ymax' from third column into a string
                    //        List<string> positionsplt = positionssArray[i].SplitStr(",");

                    //        //store xmin, ymin, xmax, ymax in separate variables
                    //        Int32.TryParse(positionsplt[0], out int xmin);
                    //        Int32.TryParse(positionsplt[1], out int ymin);
                    //        Int32.TryParse(positionsplt[2], out int xmax);
                    //        Int32.TryParse(positionsplt[3], out int ymax);


                    //        this.showObject(e, xmin + XOffset, ymin + YOffset, xmax, ymax, detectionsArray[i], result); //call rectangle drawing method, calls appropriate detection text

                    //    }

                    //}


                }
                catch (Exception ex)
                {

                    Log($"Error: Positions (subitem4) ='{positions}', Detections (subitem3) ='{detections}': {ex.Msg()}");
                }

            }
        }

        // add new entry in left list
        private bool showingtrayerr = false;
        private bool showingtrayok = false;

        private void UpdateErrorIcon()
        {
            try
            {
                //make tray icon red if there are errors
                if (LogMan.ErrorCount == 0 && !showingtrayok)
                {
                    this.notifyIcon.Icon = AITool.Properties.Resources.Logo_old;
                    this.Icon = AITool.Properties.Resources.Logo_old;
                    this.Refresh();
                    showingtrayok = true;
                    showingtrayerr = false;
                }
                else if (LogMan.ErrorCount > 0 && !showingtrayerr)
                {
                    this.notifyIcon.Icon = AITool.Properties.Resources.Logo_Error_old;
                    this.Icon = AITool.Properties.Resources.Logo_Error_old;
                    this.Refresh();
                    showingtrayok = false;
                    showingtrayerr = true;
                }

            }
            catch { }
        }



        private void UpdateStats(string Message = "")
        {
            try
            {
                UpdateErrorIcon();


                if (!this.Visible || (this.WindowState == FormWindowState.Minimized) || this.IsStatsUpdating || IsClosing)
                    return;  //save a tree

                this.IsStatsUpdating = true;

                int alerts = 0;
                int irrelevantalerts = 0;
                int falsealerts = 0;
                int newskipped = 0;
                int skipped = 0;
                foreach (Camera cam in AppSettings.Settings.CameraList)
                {
                    alerts += cam.stats_alerts;
                    irrelevantalerts += cam.stats_irrelevant_alerts;
                    falsealerts += cam.stats_false_alerts;
                    skipped += cam.stats_skipped_images;
                    newskipped += cam.stats_skipped_images_session;
                }

                Global_GUI.InvokeIFRequired(this.toolStripStatusLabelHistoryItems.GetCurrentParent(), () =>
                {

                    double hpm = 0;
                    int items = 0;
                    int removed = 0;

                    if (HistoryDB != null)
                    {
                        items = HistoryDB.HistoryDic.Count;
                        removed = HistoryDB.DeletedCount;
                    }

                    if (HistoryDB != null && HistoryDB.AddedCount > 0)
                        hpm = HistoryDB.AddedCount / (DateTime.Now - HistoryDB.InitializeTime).TotalMinutes;

                    this.toolStripStatusLabelHistoryItems.Text = $"{items} history items ({hpm.ToString("###0")}/MIN) | {removed} removed |";
                    int TriggerActionQueueCount = 0;
                    if (TriggerActionQueue != null)
                        TriggerActionQueueCount = TriggerActionQueue.Count;

                    this.toolStripStatusLabel1.Text = $"| {alerts} Alerts | {irrelevantalerts} Irrelevant | {falsealerts} False | {skipped} Skipped ({newskipped} new) | {qcalc.Count} ImgProcessed ({qcalc.ItemsPerMinute().ToString("###0")}/MIN) | {ImageProcessQueue.Count} ImgQueued (Max={scalc.Max},Avg={Math.Round(scalc.Avg, 0)}) | {TriggerActionQueueCount} ActQueued (Min={TriggerActionQueue.ActionTimeCalc.Min.Round(0)}ms/Max={TriggerActionQueue.ActionTimeCalc.Max.Round(0)}ms)";

                    this.toolStripStatusErrors.Text = $"{LogMan.ErrorCount} Errors";

                    if (!string.IsNullOrEmpty(Message))
                    {
                        this.toolStripStatusLabelInfo.Text = Message;
                    }
                    else if (ImageProcessQueue.Count > 0 && this.toolStripProgressBar1.Value == 0)
                    {
                        this.toolStripStatusLabelInfo.Text = "Working...";
                    }
                    else if (this.toolStripProgressBar1.Value == 0)
                    {
                        this.toolStripStatusLabelInfo.Text = "Idle.";
                    }

                    //if (toolStripProgressBar1.Style == ProgressBarStyle.Marquee && toolStripStatusLabelInfo.Text == "Idle.")
                    //{
                    //    toolStripProgressBar1.Style = ProgressBarStyle.Continuous;
                    //}


                    if (LogMan.ErrorCount > 0)
                    {
                        this.toolStripStatusErrors.ForeColor = Color.Black;
                        this.toolStripStatusErrors.BackColor = Color.Red;
                    }
                    else
                    {
                        this.toolStripStatusErrors.ForeColor = this.toolStripStatusLabelHistoryItems.GetCurrentParent().ForeColor;
                        this.toolStripStatusErrors.BackColor = this.toolStripStatusLabelHistoryItems.GetCurrentParent().BackColor;
                    }

                    this.toolStripStatusErrors.GetCurrentParent().Refresh();

                });

                Global_GUI.InvokeIFRequired(this.lblQueue, () =>
                {
                    this.lblQueue.Text = $"Images in queue: {ImageProcessQueue.Count}, Max: {scalc.Max} ({qcalc.Max}ms), Average: {scalc.Avg.ToString("#####")} ({qcalc.Avg.ToString("#####")}ms queue wait time, Trigger Actions Min={TriggerActionQueue.ActionTimeCalc.Min.Round(0)}ms/Max={TriggerActionQueue.ActionTimeCalc.Max.Round(0)}ms/Avg={TriggerActionQueue.ActionTimeCalc.Avg.Round(0)}ms)";

                });
                Global_GUI.InvokeIFRequired(this.lbl_errors, () =>
                {
                    if (LogMan.ErrorCount > 0)
                        this.lbl_errors.Text = $"{LogMan.ErrorCount} error(s) occurred. Click to open Log."; //update error counter label
                    else
                        this.lbl_errors.Text = "";

                });



            }
            catch (Exception ex)
            {

                Log("Error: " + ex.Msg());
            }
            finally
            {
                this.IsStatsUpdating = false;
            }
        }
        //load stored entries in history CSV into history ListView
        private async Task LoadHistoryAsync(bool FilterChanged, bool Follow)
        {

            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.


            //if (IsLoading)  //when you set checkboxes during init, it may trigger the event to load the history
            //{
            //    //Log("---Exit (still loading)");
            //    return;
            //}

            //make sure only one thread updating at a time
            //await Semaphore_List_Updating.WaitAsync();

            if (this.IsHistoryListUpdating)
            {
                Log("Trace: ---Exit (already updating)");
                return;
            }

            Stopwatch semsw = Stopwatch.StartNew();

            this.IsHistoryListUpdating = true;
            this.LastListUpdate = DateTime.Now;

            Global_GUI.CursorWait cw = null;

            try
            {
                if (semsw.ElapsedMilliseconds >= AppSettings.Settings.loop_delay_ms)
                    Log($"debug: Waited {semsw.ElapsedMilliseconds}ms while waiting for other threads to finish.");

                //wait a bit for the list to be available
                Stopwatch sw = Stopwatch.StartNew();
                bool displayed = false;
                do
                {
                    if (HistoryDB != null && HistoryDB.HistoryDic != null && this.folv_history != null && this.DatabaseInitialized)
                        break;
                    else if (!displayed)
                    {

                        Log("debug: Waiting for database to finish initializing...");
                        displayed = true;
                    }
                    await Task.Delay(AppSettings.Settings.loop_delay_ms);

                } while (sw.ElapsedMilliseconds < 60000);

                if (displayed)
                    Log($"...debug: Waited {sw.ElapsedMilliseconds}ms for the database to finish initializing/cleaning.");

                //Dont update list unless we are on the tab and it is visible for performance reasons.
                if (FilterChanged || this.folv_history.Items.Count == 0 || (this.tabControl1.SelectedIndex == 2 && this.Visible && !(this.WindowState == FormWindowState.Minimized)))
                {


                    if (this.Visible && !(this.WindowState == FormWindowState.Minimized))
                        this.UpdateStats("Updating History List...");

                    if (FilterChanged)
                        cw = new Global_GUI.CursorWait();

                    if (await HistoryDB.HasUpdates() || FilterChanged)
                    {

                        Global_GUI.InvokeIFRequired(this.folv_history, async () =>
                        {

                            List<History> histlist = HistoryDB.GetAllValues();
                            //find the last item that is unfiltered:
                            History lasthist = null;
                            for (int i = histlist.Count - 1; i >= 0; i--)
                            {
                                if (this.checkListFilters(histlist[i]))
                                {
                                    lasthist = histlist[i];
                                    break;
                                }
                            }
                            // run in another thread so gui doesnt freeze
                            await Task.Run(() =>
                            {
                                Global_GUI.UpdateFOLV(this.folv_history, histlist, Follow || AppSettings.Settings.HistoryFollow, UseSelected: true, SelectObject: lasthist, FullRefresh: true);
                            });

                            //reset any that snuck in while waiting since we just did a full list update
                            HistoryDB.GetRecentlyAdded();
                            HistoryDB.GetRecentlyDeleted();

                            if (FilterChanged)
                            {

                                if (this.comboBox_filter_camera.Text != "All Cameras" || this.cb_filter_animal.Checked || this.cb_filter_nosuccess.Checked || this.cb_filter_person.Checked || this.cb_filter_success.Checked || this.cb_filter_vehicle.Checked || this.cb_filter_skipped.Checked)
                                {
                                    //filter
                                    this.folv_history.ModelFilter = new BrightIdeasSoftware.ModelFilter((object x) =>
                                {
                                    History hist = (History)x;
                                    return this.checkListFilters(hist);
                                });
                                }
                                else
                                {
                                    this.folv_history.ModelFilter = null;
                                }


                            }

                            if (Follow && this.folv_history.SelectedObject == null && this.folv_history.GetItemCount() > 0)
                            {
                                if (this.folv_history.IsFiltering)
                                {
                                    //use the last filtered object as the selected object
                                    this.folv_history.SelectedObject = this.folv_history.FilteredObjects.Cast<object>().Last();
                                }
                                else if (!this.folv_history.IsFiltering)
                                {
                                    this.folv_history.SelectedObject = this.folv_history.Objects.Cast<object>().Last();
                                }

                                if (this.folv_history.SelectedObject != null)
                                    this.folv_history.EnsureModelVisible(this.folv_history.SelectedObject);

                            }

                        });
                    }
                    else
                    {
                        //Log("debug: No history file updates.");
                    }

                    if (this.Visible && !(this.WindowState == FormWindowState.Minimized))
                        this.UpdateStats("Idle.");

                }
                else
                {
                    //Log("debug: Not updating history, window not visible or history tab not selected.");
                }

            }
            catch (Exception ex)
            {
                Log("Error: " + ex.Msg());
            }
            finally
            {
                if (cw != null)
                    cw.Dispose();
                this.LastListUpdate = DateTime.Now;
                this.IsHistoryListUpdating = false;
                //Semaphore_List_Updating.Release();
                //Log("---Exit");

            }

        }

        //check if a filter applies on given string of history list entry 
        private bool checkListFilters(History hist)   //string cameraname, string success, string objects_and_confidence
        {

            bool ret = true;

            if (!hist.Success && this.cb_filter_success.Checked)
                ret = false;

            if (hist.Success && this.cb_filter_nosuccess.Checked)
                ret = false;

            if (!hist.WasSkipped && this.cb_filter_skipped.Checked)
                ret = false;

            if (!hist.WasMasked && this.cb_filter_masked.Checked)
                ret = false;

            if (!hist.IsPerson && this.cb_filter_person.Checked)
                ret = false;

            if (!hist.IsVehicle && this.cb_filter_vehicle.Checked)
                ret = false;

            if (!hist.IsAnimal && this.cb_filter_animal.Checked)
                ret = false;

            bool CameraValid = ((string.Equals(this.comboBox_filter_camera.Text.Trim(), "All Cameras", StringComparison.OrdinalIgnoreCase)) || string.Equals(hist.Camera.Trim(), this.comboBox_filter_camera.Text.Trim(), StringComparison.OrdinalIgnoreCase));

            if (!CameraValid)
                ret = false;

            return ret;


        }



        //EVENTS

        private void BeginProcessImage(string image_path)
        {

            string filename = Path.GetFileName(image_path);

            Global_GUI.InvokeIFRequired(this.label2, () => { this.label2.Text = $"Processing {filename}..."; });

            this.UpdateStats();

        }

        private void EndProcessImage(string image_path)
        {

            string filename = Path.GetFileName(image_path);


            //output Running on Overview Tab
            Global_GUI.InvokeIFRequired(this.label2, () => { this.label2.Text = $"Idle"; });

            //only update charts if stats tab is open

            if (this.tabControl1.SelectedIndex == 1)
            {
                Global_GUI.InvokeIFRequired(this, () => { this.UpdatePieChart(); this.UpdateTimeline(); this.UpdateConfidenceChart(); });
            }

            //load updated cameara stats info in camera tab if a camera is selected
            Global_GUI.InvokeIFRequired(this, () =>
            {
                if (this.FOLV_Cameras.SelectedObjects != null && this.FOLV_Cameras.SelectedObjects.Count > 0)
                {


                    //load only stats from Camera.cs object
                    //all camera objects are stored in the list CameraList, so firstly the position (stored in the second column for each entry) is gathered
                    Camera cam = AITOOL.GetCamera(((Camera)this.FOLV_Cameras.SelectedObjects[0]).Name);

                    //load cameras stats
                    string stats = $"Alerts: {cam.stats_alerts.ToString()} | Irrelevant Alerts: {cam.stats_irrelevant_alerts.ToString()} | False Alerts: {cam.stats_false_alerts.ToString()}";
                    if (cam.maskManager.MaskingEnabled)
                    {
                        stats += $" | Mask History Count: {cam.maskManager.LastPositionsHistory.Count} | Current Dynamic Masks: {cam.maskManager.MaskedPositions.Count}";
                    }
                    this.lbl_camstats.Text = stats;
                }

            });


            this.UpdateStats();

        }


        //event: load selected image to picturebox


        //event: show mask button clicked
        private void cb_showMask_CheckedChanged(object sender, EventArgs e)
        {
            if (this.folv_history.SelectedObjects != null && this.folv_history.SelectedObjects.Count > 0)
            {
                this.showHideMask();
            }
        }

        //event: show objects button clicked
        private void cb_showObjects_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.folv_history.SelectedObjects != null && this.folv_history.SelectedObjects.Count > 0)
            {
                this.pictureBox1.Refresh();
            }

        }

        //event: show history list filters button clicked
        //private void cb_showFilters_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (cb_showFilters.Checked)
        //    {
        //        cb_showFilters.Text = "˅ Filter";
        //        splitContainer1.Panel2Collapsed = false;
        //    }
        //    else
        //    {
        //        cb_showFilters.Text = "˄ Filter";
        //        splitContainer1.Panel2Collapsed = true;
        //    }

        //    ResizeListViews();

        //}

        //event: filter "only revelant alerts" checked or unchecked



        //----------------------------------------------------------------------------------------------------------
        //CAMERAS TAB
        //----------------------------------------------------------------------------------------------------------

        //BASIC METHODS

        // load cameras to camera list
        public void LoadCameras()
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            try
            {
                //Global_GUI.InvokeIFRequired(this.FOLV_Cameras, () =>
                //{
                //start by getting last selected camera if any
                string oldnamecameras = "";
                if (this.FOLV_Cameras.SelectedObjects != null && this.FOLV_Cameras.SelectedObjects.Count > 0)
                    oldnamecameras = ((Camera)this.FOLV_Cameras.SelectedObjects[0]).Name;

                //if nothing was selected, then select the last saved camera:
                if (string.IsNullOrEmpty(oldnamecameras))
                    oldnamecameras = Global.GetRegSetting("LastSelectedCamera", "");

                if (string.IsNullOrEmpty(oldnamecameras))
                    oldnamecameras = "default";

                string oldnamefilters = "";
                if (this.comboBox_filter_camera.Items.Count > 0)
                    oldnamefilters = this.comboBox_filter_camera.Text;

                string oldnamestats = "";
                if (this.comboBox1.Items.Count > 0)
                    oldnamestats = this.comboBox1.Text;

                this.comboBox1.Items.Clear();
                this.comboBox1.Items.Add("All Cameras");
                this.comboBox_filter_camera.Items.Clear();
                this.comboBox_filter_camera.Items.Add("All Cameras");

                int i = 0;
                int oldidxcameras = 0;
                int oldidxfilters = 0;
                int oldidxstats = 0;
                Camera selectedcam = null;
                foreach (Camera cam in AppSettings.Settings.CameraList)
                {
                    //item.Tag = file; //tag is not used anywhere I can see
                    //add camera to combobox on overview tab and to camera filter combobox in the History tab 
                    this.comboBox1.Items.Add($"   {cam.Name}");
                    this.comboBox_filter_camera.Items.Add($"   {cam.Name}");
                    if (string.Equals(oldnamecameras.Trim(), cam.Name.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        oldidxcameras = i;
                        selectedcam = cam;
                    }
                    if (string.Equals(oldnamefilters.Trim(), cam.Name.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        oldidxfilters = i + 1;
                    }
                    if (string.Equals(oldnamestats.Trim(), cam.Name.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        oldidxstats = i + 1;
                    }
                    i++;

                }

                if (selectedcam == null && AppSettings.Settings.CameraList.Count > 0)
                    selectedcam = AppSettings.Settings.CameraList[0];

                Global_GUI.UpdateFOLV(FOLV_Cameras, AppSettings.Settings.CameraList, false, ColumnHeaderAutoResizeStyle.ColumnContent, true, true, selectedcam);

                if (this.comboBox_filter_camera.Items.Count > 0)
                    this.comboBox_filter_camera.SelectedIndex = oldidxfilters;

                if (this.comboBox1.Items.Count > 0)
                    this.comboBox1.SelectedIndex = oldidxstats;

                //});

            }
            catch (Exception ex)
            {
                Log("ERROR: LoadCameras() failed: " + ex.Msg());
                MessageBox.Show("ERROR: LoadCameras() failed: " + ex.Msg());
            }

        }

        //load existing camera (settings file exists) into CameraList, into Stats dropdown and into History filter dropdown 
        //private string LoadCamera(string config_path)
        //{
        //    //check if camera with specified name or its prefix already exists. If yes, then abort.
        //    foreach (Camera c in AppSettings.Settings.CameraList)
        //    {
        //        if (c.name == Path.GetFileNameWithoutExtension(config_path))
        //        {
        //            return ($"ERROR: Camera name must be unique,{Path.GetFileNameWithoutExtension(config_path)} already exists.");
        //        }
        //        if (c.prefix == System.IO.File.ReadAllLines(config_path)[2].Split('"')[1])
        //        {
        //            return ($"ERROR: Every camera must have a unique prefix ('Input file begins with'), but the prefix of {Path.GetFileNameWithoutExtension(config_path)} equals the prefix of the existing camera {c.name} .");
        //        }
        //    }
        //    Camera cam = new Camera(); //create new camera object
        //    Log("read config");
        //    cam.ReadConfig(config_path); //read camera's config from file
        //    Log("add");
        //    AppSettings.Settings.CameraList.Add(cam); //add created camera object to CameraList

        //    //add camera to combobox on overview tab and to camera filter combobox in the History tab 
        //    comboBox1.Items.Add($"   {cam.name}");
        //    comboBox_filter_camera.Items.Add($"   {cam.name}");

        //    return ($"SUCCESS: {Path.GetFileNameWithoutExtension(config_path)} loaded.");
        //}

        //add camera
        private string AddCamera(Camera cam) //, int history_mins, int mask_create_counter, int mask_remove_counter, double percent_variance)
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            //check if camera with specified name already exists. If yes, then abort.
            foreach (Camera c in AppSettings.Settings.CameraList)
            {
                if (string.Equals(c.Name.Trim(), cam.Name.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show($"ERROR: Camera name must be unique,{cam.Name} already exists.");
                    return ($"ERROR: Camera name must be unique,{cam.Name} already exists.");
                }
            }

            //check if name is empty
            if (cam.Name == "")
            {
                MessageBox.Show($"ERROR: Camera name may not be empty.");
                return ($"ERROR: Camera name may not be empty.");
            }


            if (AITOOL.BlueIrisInfo.Result == BlueIrisResult.Valid)
            {
                //http://10.0.1.99:81/admin?trigger&camera=BACKFOSCAM&user=AITools&pw=haha&memo=[summary]
                cam.trigger_urls_as_string = "[BlueIrisURL]/admin?camera=[camera]&trigger&user=[Username]&pw=[Password]&flagalert=2&memo=[summary]&jpeg=[ImagePathEscaped]";
            }

            //I dont think this is used anywhere
            cam.triggering_objects = cam.triggering_objects_as_string.SplitStr(",").ToArray();   //triggering_objects_as_string.Split(','); //split the row of triggering objects between every ','

            //Split by cr/lf or other common delimiters
            cam.trigger_urls = cam.trigger_urls_as_string.SplitStr("\r\n|").ToArray();  //all trigger urls in an array
            cam.cancel_urls = cam.cancel_urls_as_string.SplitStr("\r\n|").ToArray();  //all trigger urls in an array

            cam.BICamName = cam.Name;

            cam.MaskFileName = $"{cam.Name}.bmp";

            AppSettings.Settings.CameraList.Add(cam); //add created camera object to CameraList

            this.LoadCameras();

            return ($"SUCCESS: {cam.Name} created.");
        }



        //remove camera
        private void RemoveCamera(System.Collections.IList objs)
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            for (int i = 0; i < objs.Count; i++)
            {
                Log($"Removing camera {((Camera)objs[i]).Name}...");

                AppSettings.Settings.CameraList.Remove((Camera)objs[i]);

            }
            if (this.FOLV_Cameras.Items.Count > 0)
                this.FOLV_Cameras.SelectedIndex = 0;
            AppSettings.SaveAsync(true);
            this.LoadCameras();

        }

        //display camera settings for selected camera
        private void DisplayCameraSettings()
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            try
            {
                if (this.FOLV_Cameras.SelectedObjects != null && this.FOLV_Cameras.SelectedObjects.Count > 0)
                {
                    Camera cam = ((Camera)this.FOLV_Cameras.SelectedObjects[0]);

                    if (cam.last_image_file.IsNotEmpty() && File.Exists(cam.last_image_file))
                    {
                        using (var img = new Bitmap(cam.last_image_file))
                        {
                            this.pictureBoxCamera.Image = new Bitmap(img); //load mask as overlay
                        }
                    }
                    else
                    {
                        this.pictureBoxCamera.Image = null; //if file does not exist, empty mask overlay (from possible overlays of previous images)
                    }

                    UpdateActionsLabel(cam);

                    Lbl_PredictionTolerances.Text = $"Threshold: {cam.threshold_lower}-{cam.threshold_upper}, Size: {cam.PredSizeMinPercentOfImage.ToPercent()}-{cam.PredSizeMaxPercentOfImage.ToPercent()} ; Width: {cam.PredSizeMinWidth}-{cam.PredSizeMaxWidth}, Height: {cam.PredSizeMinHeight}-{cam.PredSizeMaxHeight}, PredictionMatch: {cam.MergePredictionsMinMatchPercent.ToPercent()}";

                    this.tableLayoutPanel6.Enabled = true;

                    this.tbName.Text = cam.Name; //load name textbox from name in list2
                    this.tbBiCamName.Text = cam.BICamName;
                    this.tbCustomMaskFile.Text = cam.MaskFileName;

                    //load cameras stats
                    string stats = $"Alerts: {cam.stats_alerts.ToString()} | Irrelevant Alerts: {cam.stats_irrelevant_alerts.ToString()} | False Alerts: {cam.stats_false_alerts.ToString()}";

                    if (cam.maskManager.MaskingEnabled)
                    {
                        stats += $" | Mask History Count: {cam.maskManager.LastPositionsHistory.Count} | Current Dynamic Masks: {cam.maskManager.MaskedPositions.Count}";
                    }
                    this.lbl_camstats.Text = stats;

                    //load if ai detection is active for the camera
                    if (cam.enabled == true)
                    {
                        this.cb_enabled.Checked = true;
                    }
                    else
                    {
                        this.cb_enabled.Checked = false;
                    }
                    this.tbPrefix.Text = cam.Prefix; //load 'input file begins with'
                    this.lbl_prefix.Text = this.tbPrefix.Text + ".××××××.jpg"; //prefix live preview

                    this.cmbcaminput.Text = cam.input_path;
                    this.cmbcaminput.Items.Clear();
                    foreach (string pth in AITOOL.BlueIrisInfo.ClipPaths)
                    {
                        this.cmbcaminput.Items.Add(pth);
                    }

                    this.cb_monitorCamInputfolder.Checked = cam.input_path_includesubfolders;

                    this.tb_camera_telegram_chatid.Text = cam.telegram_chatid;

                    //load is masking enabled 
                    this.cb_masking_enabled.Checked = cam.maskManager.MaskingEnabled;


                    this.lbl_RelevantObjects.Text = cam.DefaultTriggeringObjects.ToString();
                    ////load triggering objects
                    ////first create arrays with all checkboxes stored in
                    //CheckBox[] cbarray = new CheckBox[] { this.cb_airplane, this.cb_bear, this.cb_bicycle, this.cb_bird, this.cb_boat, this.cb_bus, this.cb_car, this.cb_cat, this.cb_cow, this.cb_dog, this.cb_horse, this.cb_motorcycle, this.cb_person, this.cb_sheep, this.cb_truck };
                    ////create array with strings of the triggering_objects related to the checkboxes in the same order
                    //string[] cbstringarray = new string[] { "Airplane", "Bear", "Bicycle", "Bird", "Boat", "Bus", "Car", "Cat", "Cow", "Dog", "Horse", "Motorcycle", "Person", "Sheep", "Truck" };

                    ////clear all checkmarks
                    //foreach (CheckBox cb in cbarray)
                    //{
                    //    cb.Checked = false;
                    //}

                    ////check for every triggering_object string if it is active in the settings file. If yes, check according checkbox
                    //for (int j = 0; j < cbarray.Length; j++)
                    //{
                    //    if (cam.triggering_objects_as_string.IndexOf(cbstringarray[j], StringComparison.OrdinalIgnoreCase) >= 0)
                    //    {
                    //        cbarray[j].Checked = true;
                    //    }
                    //}

                    //this.tbAdditionalRelevantObjects.Text = cam.additional_triggering_objects_as_string;


                }

            }
            catch (Exception ex)
            {

                string err = $"Error: While displaying camera settings, got error: {ex.Msg()}";
                Log(err);
                MessageBox.Show(err);
            }
        }



        // SPECIAL METHODS

        //input file begins with live preview
        private void tbPrefix_TextChanged(object sender, EventArgs e)
        {
            this.lbl_prefix.Text = this.tbPrefix.Text + ".××××××.jpg";
        }



        //event: camera list another item selected


        //event: camera add button
        private void btnCameraAdd_Click(object sender, EventArgs e)
        {

            using (Frm_CameraAdd frm = new Frm_CameraAdd())
            {

                //add only cameras not already installed
                foreach (string camstr in AITOOL.BlueIrisInfo.Cameras)
                {
                    if (GetCamera(camstr, false) == null)
                        frm.checkedListBoxCameras.Items.Add(camstr, false);

                }

                if (frm.ShowDialog() == DialogResult.OK)
                {
                    int added = 0;
                    List<string> cams = frm.tb_Cameras.Text.SplitStr("\r\n|;,");
                    Camera DefCam = GetCamera("default", true);

                    foreach (var cs in cams)
                    {

                        string cn = cs;

                        //if (cn.StartsWith("ai", StringComparison.OrdinalIgnoreCase))
                        //{
                        //    cn = Name.Substring(2).TrimStart(@"_-".ToCharArray()).Trim();  //if using dupe cam that may start with AICAMNAME or AI_CAMNAME
                        //    string maskfile = AITOOL.GetMaskFile(cn);
                        //    string newfile = AITOOL.GetMaskFile(cs);
                        //    if (File.Exists(maskfile) && !File.Exists(newfile))
                        //        File.Move(maskfile, newfile);
                        //}

                        if (GetCamera(cn, false) == null)
                        {

                            Camera cam = null;   // new Camera(cn);

                            //Try to use the default camera settings when creating a new camera
                            if (!DefCam.IsNull())
                            {
                                cam = DefCam.CloneJson();
                                cam.Name = cn;
                                cam.BICamName = cn;
                                cam.Prefix = cn;
                                cam.UpdateCamera();
                            }
                            else
                            {
                                cam = new Camera(cn);
                            }

                            string camresult = this.AddCamera(cam);

                            Log(camresult);
                            if (camresult.StartsWith("success", StringComparison.OrdinalIgnoreCase))
                            {
                                added++;
                                Global.SaveRegSetting("LastSelectedCamera", cam.Name);
                            }

                        }
                        else
                        {
                            Log($"Error: Camera already existed {cs}.");
                        }

                    }

                    MessageBox.Show($"Added {added} out of {cams.Count}.  See log for details.");

                }
            }

            //using (var form = new InputForm("Camera Name:", "New Camera", cbitems: BlueIrisInfo.Cameras))
            //{
            //    var result = form.ShowDialog();
            //    if (result == DialogResult.OK)
            //    {
            //        Camera cam = new Camera(form.text);

            //        string camresult = this.AddCamera(cam);

            //        // Old way...
            //        //string name, string prefix, string trigger_urls_as_string, string triggering_objects_as_string, bool telegram_enabled, bool enabled, double cooldown_time, int threshold_lower, int threshold_upper,
            //        //                                 string _input_path, bool _input_path_includesubfolders,
            //        //                                 bool masking_enabled,
            //        //                                 bool trigger_cancels

            //        MessageBox.Show(camresult);
            //    }
            //}
        }

        //event: save camera settings button
        private void btnCameraSave_Click_1(object sender, EventArgs e)
        {

            this.CameraSave(false);
        }

        private void CameraSave(bool SaveTo)
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            try
            {
                if (this.FOLV_Cameras.SelectedObjects.Count > 0)
                {
                    //check if name is empty
                    if (String.IsNullOrWhiteSpace(this.tbName.Text))
                    {
                        this.DisplayCameraSettings(); //reset displayed settings
                        MessageBox.Show($"WARNING: Camera name may not be empty.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (!string.Equals(((Camera)this.FOLV_Cameras.SelectedObjects[0]).Name.Trim(), this.tbName.Text.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        //camera renamed, make sure name doesnt exist
                        Camera CamCheck = AITOOL.GetCamera(this.tbName.Text, false);
                        if (CamCheck != null)
                        {
                            //Its a dupe
                            MessageBox.Show($"WARNING: Camera name must be unique, but new camera name '{this.tbName.Text}' already exists.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            this.DisplayCameraSettings(); //reset displayed settings
                            return;
                        }
                    }

                    Camera cam = AITOOL.GetCamera(((Camera)this.FOLV_Cameras.SelectedObjects[0]).Name, false);

                    if (cam == null)
                    {
                        //should not happen, but...
                        MessageBox.Show($"WARNING: Camera not found???  '{((Camera)this.FOLV_Cameras.SelectedObjects[0]).Name}'", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.DisplayCameraSettings(); //reset displayed settings
                        return;
                    }

                    if (cam.input_path.IsNotNull() && cam.Action_network_folder.IsNotNull() && cam.input_path.TrimEnd("\\".ToCharArray()).EqualsIgnoreCase(cam.Action_network_folder.TrimEnd("\\".ToCharArray())))
                    {
                        //You dont want to watch, then copy the same file back to the same folder
                        MessageBox.Show($"WARNING: Input path ({cam.input_path}) & 'Copy alert images to folder' path may not be the same for camera '{cam.Name}'", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.DisplayCameraSettings(); //reset displayed settings
                        return;
                    }

                    //1. GET SETTINGS INPUTTED
                    //all checkboxes in one array

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

                    //CheckBox[] cbarray = new CheckBox[] { this.cb_airplane, this.cb_bear, this.cb_bicycle, this.cb_bird, this.cb_boat, this.cb_bus, this.cb_car, this.cb_cat, this.cb_cow, this.cb_dog, this.cb_horse, this.cb_motorcycle, this.cb_person, this.cb_sheep, this.cb_truck };
                    ////create array with strings of the triggering_objects related to the checkboxes in the same order
                    //string[] cbstringarray = new string[] { "Airplane", "Bear", "Bicycle", "Bird", "Boat", "Bus", "Car", "Cat", "Cow", "Dog", "Horse", "Motorcycle", "Person", "Sheep", "Truck" };

                    ////go through all checkboxes and write all triggering_objects in one string
                    //cam.triggering_objects_as_string = "";
                    //for (int i = 0; i < cbarray.Length; i++)
                    //{
                    //    if (cbarray[i].Checked == true)
                    //    {
                    //        cam.triggering_objects_as_string += $"{cbstringarray[i].Trim()}, ";
                    //    }
                    //}


                    //cam.triggering_objects = cam.triggering_objects_as_string.Split(",").ToArray();   //triggering_objects_as_string.Split(','); //split the row of triggering objects between every ','

                    //cam.additional_triggering_objects_as_string = this.tbAdditionalRelevantObjects.Text.Trim();

                    cam.trigger_urls = cam.trigger_urls_as_string.SplitStr("\r\n|").ToArray();  //all trigger urls in an array
                    cam.cancel_urls = cam.cancel_urls_as_string.SplitStr("\r\n|").ToArray();

                    if (cam.Name != this.tbName.Text.Trim())
                    {
                        string Oldmaskfile = cam.GetMaskFile(false);
                        if (!string.IsNullOrEmpty(Oldmaskfile) && File.Exists(Oldmaskfile))
                        {
                            string ext = Path.GetExtension(Oldmaskfile);
                            string pth = Path.GetDirectoryName(Oldmaskfile);
                            string NewMaskFile = Path.Combine(pth, this.tbName.Text.Trim() + ext);
                            File.Move(Oldmaskfile, NewMaskFile);
                            cam.MaskFileName = NewMaskFile;
                        }
                        Log($"Renaming Camera '{cam.Name}' to '{this.tbName.Text}'");
                        cam.Name = this.tbName.Text.Trim();  //just in case we needed to rename it
                    }

                    cam.BICamName = this.tbBiCamName.Text.Trim();
                    cam.MaskFileName = this.tbCustomMaskFile.Text.Trim();

                    cam.Prefix = this.tbPrefix.Text.Trim();
                    cam.enabled = this.cb_enabled.Checked;
                    cam.maskManager.MaskingEnabled = this.cb_masking_enabled.Checked;
                    cam.input_path = this.cmbcaminput.Text.Trim();
                    cam.input_path_includesubfolders = this.cb_monitorCamInputfolder.Checked;

                    cam.telegram_chatid = this.tb_camera_telegram_chatid.Text.Trim();

                    int ccnt = 0;

                    if (SaveTo)
                    {
                        using (Frm_ApplyCameraTo frm = new Frm_ApplyCameraTo())
                        {
                            foreach (Camera ccam in AppSettings.Settings.CameraList)
                            {
                                if (ccam.Name != cam.Name)
                                    frm.checkedListBoxCameras.Items.Add(ccam.Name, false);
                            }

                            if (frm.ShowDialog() == DialogResult.OK)
                            {
                                for (int i = 0; i < frm.checkedListBoxCameras.Items.Count; i++)
                                {
                                    if (frm.checkedListBoxCameras.GetItemChecked(i))
                                    {
                                        Camera icam = AITOOL.GetCamera(frm.checkedListBoxCameras.Items[i].ToString(), false);
                                        if (icam != null)
                                        {
                                            ccnt++;

                                            Log($"Updating camera '{cam.Name}' with settings from '{icam.Name}'...");

                                            //icam.BICamName = cam.BICamName;


                                            if (frm.cb_apply_confidence_limits.Checked)
                                            {
                                                icam.threshold_lower = cam.threshold_lower;
                                                icam.threshold_upper = cam.threshold_upper;
                                                icam.MergePredictionsMinMatchPercent = cam.MergePredictionsMinMatchPercent;
                                                icam.PredSizeMinHeight = cam.PredSizeMinHeight;
                                                icam.PredSizeMinWidth = cam.PredSizeMinWidth;
                                                icam.PredSizeMaxHeight = cam.PredSizeMaxHeight;
                                                icam.PredSizeMaxWidth = cam.PredSizeMaxWidth;
                                                icam.PredSizeMaxPercentOfImage = cam.PredSizeMaxPercentOfImage;
                                                icam.PredSizeMinPercentOfImage = cam.PredSizeMinPercentOfImage;
                                            }
                                            if (frm.cb_apply_objects.Checked)
                                            {
                                                icam.triggering_objects_as_string = cam.triggering_objects_as_string;
                                                icam.additional_triggering_objects_as_string = cam.additional_triggering_objects_as_string;
                                                icam.triggering_objects = cam.triggering_objects_as_string.SplitStr(",").ToArray();   //triggering_objects_as_string.Split(','); //split the row of triggering objects between every ','

                                                icam.Action_pushover_triggering_objects = cam.Action_pushover_triggering_objects;
                                                icam.DefaultTriggeringObjects.ObjectList = cam.DefaultTriggeringObjects.CloneObjectList();
                                                icam.TelegramTriggeringObjects.ObjectList = cam.TelegramTriggeringObjects.CloneObjectList();
                                                icam.MQTTTriggeringObjects.ObjectList = cam.MQTTTriggeringObjects.CloneObjectList();
                                                icam.PushoverTriggeringObjects.ObjectList = cam.PushoverTriggeringObjects.CloneObjectList();
                                                icam.maskManager.MaskTriggeringObjects.ObjectList = cam.maskManager.MaskTriggeringObjects.CloneObjectList();

                                            }
                                            if (frm.cb_apply_actions.Checked)
                                            {
                                                icam.trigger_urls_as_string = cam.trigger_urls_as_string;
                                                icam.trigger_urls = cam.trigger_urls;
                                                icam.Action_TriggerURL_Enabled = cam.Action_TriggerURL_Enabled;
                                                icam.Action_CancelURL_Enabled = cam.Action_CancelURL_Enabled;
                                                icam.cancel_urls_as_string = cam.cancel_urls_as_string;
                                                icam.cancel_urls = cam.cancel_urls;
                                                icam.cooldown_time_seconds = cam.cooldown_time_seconds;

                                                icam.DetectionDisplayFormat = cam.DetectionDisplayFormat;


                                                icam.telegram_enabled = cam.telegram_enabled;
                                                icam.telegram_caption = cam.telegram_caption;
                                                icam.telegram_chatid = cam.telegram_chatid;
                                                icam.telegram_active_time_range = cam.telegram_active_time_range;

                                                icam.Action_image_copy_enabled = cam.Action_image_copy_enabled;
                                                icam.Action_network_folder = cam.Action_network_folder;
                                                icam.Action_network_folder_filename = cam.Action_network_folder_filename;
                                                icam.Action_RunProgram = cam.Action_RunProgram;
                                                icam.Action_RunProgramString = cam.Action_RunProgramString;
                                                icam.Action_RunProgramArgsString = cam.Action_RunProgramArgsString;
                                                icam.Action_PlaySounds = cam.Action_PlaySounds;
                                                icam.Action_Sounds = cam.Action_Sounds;

                                                icam.Action_mqtt_enabled = cam.Action_mqtt_enabled;
                                                icam.Action_mqtt_payload = cam.Action_mqtt_payload;
                                                icam.Action_mqtt_topic = cam.Action_mqtt_topic;
                                                icam.Action_mqtt_payload_cancel = cam.Action_mqtt_payload_cancel;
                                                icam.Action_mqtt_topic_cancel = cam.Action_mqtt_topic_cancel;

                                                icam.Action_image_merge_detections = cam.Action_image_merge_detections;
                                                icam.Action_image_merge_jpegquality = cam.Action_image_merge_jpegquality;

                                                icam.Action_pushover_enabled = cam.Action_pushover_enabled;
                                                icam.Action_pushover_title = cam.Action_pushover_title;
                                                icam.Action_pushover_message = cam.Action_pushover_message;
                                                icam.Action_pushover_device = cam.Action_pushover_device;
                                                icam.Action_pushover_Sound = cam.Action_pushover_Sound;
                                                icam.Action_pushover_Priority = cam.Action_pushover_Priority;
                                                icam.Action_pushover_retrycallback_url = cam.Action_pushover_retrycallback_url;
                                                icam.Action_pushover_SupplementaryUrl = cam.Action_pushover_SupplementaryUrl;
                                                icam.Action_pushover_expire_seconds = cam.Action_pushover_expire_seconds;
                                                icam.Action_pushover_retry_seconds = cam.Action_pushover_retry_seconds;

                                                icam.Action_pushover_active_time_range = cam.Action_pushover_active_time_range;

                                                icam.Action_queued = cam.Action_queued;
                                            }
                                            if (frm.cb_apply_mask_settings.Checked)
                                            {
                                                icam.maskManager.MaskingEnabled = cam.maskManager.MaskingEnabled;

                                                icam.maskManager.HistorySaveMins = cam.maskManager.HistorySaveMins;
                                                icam.maskManager.HistoryThresholdCount = cam.maskManager.HistoryThresholdCount;
                                                icam.maskManager.MaskRemoveThreshold = cam.maskManager.MaskRemoveThreshold;
                                                icam.maskManager.MaskRemoveMins = cam.maskManager.MaskRemoveMins;

                                                icam.maskManager.PercentMatch = cam.maskManager.PercentMatch;

                                                icam.maskManager.ScaleConfig.IsScaledObject = cam.maskManager.ScaleConfig.IsScaledObject;
                                                icam.maskManager.ScaleConfig.MediumObjectMatchPercent = cam.maskManager.ScaleConfig.MediumObjectMatchPercent;
                                                icam.maskManager.ScaleConfig.MediumObjectMaxPercent = cam.maskManager.ScaleConfig.MediumObjectMaxPercent;
                                                icam.maskManager.ScaleConfig.SmallObjectMatchPercent = cam.maskManager.ScaleConfig.SmallObjectMatchPercent;
                                                icam.maskManager.ScaleConfig.SmallObjectMaxPercent = cam.maskManager.ScaleConfig.SmallObjectMaxPercent;

                                            }

                                        }
                                    }

                                }

                            }
                        }
                    }
                    else
                    {
                        ccnt = 1;
                    }

                    this.LoadCameras();

                    AppSettings.SaveAsync(true);

                    UpdateWatchers(true);

                    string saved = $"{ccnt} Camera(s) saved.";
                    Log(saved);

                    MessageBox.Show(saved, "", MessageBoxButtons.OK, MessageBoxIcon.Information);


                    ////2. UPDATE SETTINGS
                    //// save new camera settings, display result in MessageBox
                    //string result = UpdateCamera(list2.SelectedItems[0].Text, tbName.Text, tbPrefix.Text, tbTriggerUrl.Text, triggering_objects_as_string, cb_telegram.Checked, cb_enabled.Checked, cooldown_time, threshold_lower, threshold_upper,
                    //                             cmbcaminput.Text, cb_monitorCamInputfolder.Checked,
                    //                             cb_masking_enabled.Checked,
                    //                             cb_TriggerCancels.Checked); //, history_mins, mask_create_counter, mask_remove_counter, percent_variance);



                    //1     2       3                             4                       5                 6        7              8                9
                    //name, prefix, triggering_objects_as_string, trigger_urls_as_string, telegram_enabled, enabled, cooldown_time, threshold_lower, threshold_upper,
                    //                                                               10           11  
                    //                                                               _input_path, _input_path_includesubfolders,
                    //                                                          12   masking_enabled,
                    //                                                          13   trigger_cancel



                }
                this.DisplayCameraSettings();

            }
            catch (Exception ex)
            {

                string err = $"Error: While saving camera, got error: {ex.Msg()}";
                Log(err);
                MessageBox.Show(err);
            }

        }
        //event: delete camera button
        private void btnCameraDel_Click(object sender, EventArgs e)
        {
            if (this.FOLV_Cameras.SelectedObjects.Count > 0)
            {
                using (var form = new InputForm($"Delete camera {((Camera)this.FOLV_Cameras.SelectedObjects[0]).Name} ?", "Delete Camera?", false))
                {
                    var result = form.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        //Log("about to del cam");
                        this.RemoveCamera(this.FOLV_Cameras.SelectedObjects);
                    }
                }
            }
        }

        //event: DELETE key pressed
        private void list2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (this.FOLV_Cameras.SelectedObjects.Count > 0)
                {
                    using (var form = new InputForm($"Delete camera {((Camera)this.FOLV_Cameras.SelectedObjects[0]).Name} ?", "Delete Camera?", false))
                    {
                        var result = form.ShowDialog();
                        if (result == DialogResult.OK)
                        {
                            this.RemoveCamera(this.FOLV_Cameras.SelectedObjects);
                        }
                    }
                }
            }
        }


        //----------------------------------------------------------------------------------------------------------
        //SETTING TAB
        //----------------------------------------------------------------------------------------------------------



        //settings save button
        private async void BtnSettingsSave_Click_1(object sender, EventArgs e)
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            Global_GUI.InvokeIFRequired(this.BtnSettingsSave, () =>
            {
                BtnSettingsSave.Enabled = false;
                BtnSettingsSave.Text = "Saving...";
            });


            Application.DoEvents();

            Log($"Saving settings to {AppSettings.Settings.SettingsFileName}");
            //save inputted settings into App.settings
            AppSettings.Settings.input_path = this.cmbInput.Text.Trim();
            AppSettings.Settings.input_path_includesubfolders = this.cb_inputpathsubfolders.Checked;
            AppSettings.Settings.deepstack_urls_are_queued = this.cb_DeepStackURLsQueued.Checked;
            AppSettings.Settings.telegram_chatids = this.tb_telegram_chatid.Text.SplitStr("|;,", true, true);
            AppSettings.Settings.telegram_token = this.tb_telegram_token.Text.Trim();
            AppSettings.Settings.telegram_cooldown_seconds = GetNumberInt(this.tb_telegram_cooldown.Text.Trim());
            AppSettings.Settings.send_telegram_errors = this.cb_send_telegram_errors.Checked;
            AppSettings.Settings.send_pushover_errors = this.cb_send_pushover_errors.Checked;
            AppSettings.Settings.startwithwindows = this.cbStartWithWindows.Checked;
            AppSettings.Settings.MinimizeToTray = this.cbMinimizeToTray.Checked;

            AppSettings.Settings.DefaultUserName = this.tb_username.Text.Trim();
            AppSettings.Settings.DefaultPasswordEncrypted = this.tb_password.Text.Trim().Encrypt();

            AppSettings.Settings.BlueIrisServer = this.tb_BlueIrisServer.Text.Trim();

            AppSettings.Settings.pushover_APIKey = this.tb_Pushover_APIKey.Text.Trim();
            AppSettings.Settings.pushover_UserKey = this.tb_Pushover_UserKey.Text.Trim();
            AppSettings.Settings.pushover_cooldown_seconds = GetNumberInt(this.tb_Pushover_Cooldown.Text.Trim());

            Global.Startup(AppSettings.Settings.startwithwindows);

            UpdateAIURLs();

            if (await AppSettings.SaveAsync(true))
            {
                Log("...Saved.");
            }
            else
            {
                Log("...Not saved.  No changes?");
            }
            //update variables
            //input_path = AppSettings.Settings.input_path;
            //deepstack_url = AppSettings.Settings.deepstack_url;
            //telegram_chatid = AppSettings.Settings.telegram_chatid;
            //telegram_chatids = telegram_chatid.Replace(" ", "").Split(','); //for multiple Telegram chats that receive alert images
            //telegram_token = AppSettings.Settings.telegram_token;
            //log_everything = AppSettings.Settings.log_everything;
            //send_errors = AppSettings.Settings.send_errors;

            Application.DoEvents();

            //update fswatcher to watch new input folder
            UpdateWatchers(true);

            //Update blue iris info
            Application.DoEvents();

            await AITOOL.BlueIrisInfo.RefreshBIInfoAsync(AppSettings.Settings.BlueIrisServer);

            AITOOL.UpdateLatLong();

            Application.DoEvents();

            await AITOOL.Telegram.TryStartTelegram();

            this.cmbInput.Items.Clear();
            foreach (string pth in AITOOL.BlueIrisInfo.ClipPaths)
                this.cmbInput.Items.Add(pth);


            this.LoadSettingsTab();

            if (AITOOL.BlueIrisInfo.Result != BlueIrisResult.Valid && AITOOL.BlueIrisInfo.Result != BlueIrisResult.NotInstalled)
                MessageBox.Show($"Error: Could not connect to BlueIris server: '{AITOOL.BlueIrisInfo.Result}'.  See log for more detail.", "Error", MessageBoxButtons.OK, icon: MessageBoxIcon.Error);

            Global_GUI.InvokeIFRequired(this.BtnSettingsSave, () =>
            {
                BtnSettingsSave.Enabled = true;
                BtnSettingsSave.Text = "Save";
            });

            Application.DoEvents();


        }

        //input path select dialog button
        private void btn_input_path_Click(object sender, EventArgs e)
        {
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
            {
                if (!string.IsNullOrEmpty(this.cmbInput.Text))
                {
                    dialog.InitialDirectory = this.cmbInput.Text;

                }
                dialog.IsFolderPicker = true;
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    this.cmbInput.Text = dialog.FileName;
                }
            }
        }

        //open log button
        private void btn_open_log_Click(object sender, EventArgs e)
        {
            OpenLogFile();

        }

        //ask before closing AI Tool to prevent accidentally closing
        private async void Shell_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (IsClosing)
                return;

            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            Log($"------Closing------- CloseReason: {e.CloseReason}");


            try
            {
                if (!Restart && !ResetSettings && !CloseImmediately && e.CloseReason != CloseReason.WindowsShutDown && AppSettings.Settings.close_instantly <= 0) //if it's either enabled or not set  -1 = not set | 0 = ask for confirmation | 1 = don't ask
                {
                    using (var form = new InputForm($"Stop and close AI Tool?", "AI Tool", false))
                    {
                        var result = form.ShowDialog();
                        if (AppSettings.Settings.close_instantly == -1)
                        {
                            //if it's the first time, ask if the confirmation dialog should ever appear again
                            using (var form1 = new InputForm($"Confirm closing AI Tool every time?", "AI Tool", false, "NO", "YES"))
                            {
                                var result1 = form1.ShowDialog();
                                if (result1 == DialogResult.Cancel)
                                {
                                    AppSettings.Settings.close_instantly = 0;
                                }
                                else
                                {
                                    AppSettings.Settings.close_instantly = 1;
                                }
                            }
                        }

                        e.Cancel = (result == DialogResult.Cancel);
                    }
                }


                if (!e.Cancel)
                {

                    if (!Restart)
                        Global_GUI.SaveWindowState(this);

                    AppSettings.SaveAsync(true);  //save settings in any case

                    //if (AITOOL.DeepStackServerControl.IsInstalled && AITOOL.DeepStackServerControl.IsStarted && AppSettings.Settings.deepstack_autostart)
                    //    await AITOOL.DeepStackServerControl.StopAsync();

                    IsClosing = true;

                    if (!AppSettings.AlreadyRunning)
                        Global.SaveRegSetting("LastShutdownState", "graceful shutdown");

                    MasterCTS.Cancel();

                    //wait a bit for the loops to cancel to avoid other threading errors on shutdown and to allow the logs to finish updating if we need to reset settings
                    Global.ResponsiveSleep(1000);

                    if (Restart)
                        Application.Restart();

                    if (ResetSettings)
                    {
                        //make a backup copy:
                        string cursettingsfolder = Path.GetDirectoryName(AppSettings.Settings.SettingsFileName);
                        string baksettingsfolder = cursettingsfolder + "_RESET_" + DateTime.Now.ToString("s").Replace(":", "_");
                        if (Global.DirectoryCopy(cursettingsfolder, baksettingsfolder, true, true))
                        {
                            MessageBox.Show($"Reset successful. Backup folder created.  Folder={baksettingsfolder}");
                            //delete the settings folder
                            try
                            { Directory.Delete(cursettingsfolder, true); }
                            catch { }
                            //restart so it creates all new settings
                            Global.DeleteRegSettings();
                            Application.Restart();
                        }
                        else
                        {
                            MessageBox.Show($"Error: Failed to fully copy to the backup folder.  NOT reset.  Folder={baksettingsfolder}", "Error", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
                        }
                    }

                }

            }
            catch { }



        }

        private async Task SaveDeepStackTabAsync()
        {

            Global_GUI.InvokeIFRequired(this, async () =>
            {

                if (DeepStackServerControl == null)
                    DeepStackServerControl = new DeepStack(AppSettings.Settings.deepstack_adminkey, AppSettings.Settings.deepstack_apikey, AppSettings.Settings.deepstack_mode, AppSettings.Settings.deepstack_sceneapienabled, AppSettings.Settings.deepstack_faceapienabled, AppSettings.Settings.deepstack_detectionapienabled, AppSettings.Settings.deepstack_port, AppSettings.Settings.deepstack_customModelPath, AppSettings.Settings.deepstack_stopbeforestart, AppSettings.Settings.deepstack_customModelName, AppSettings.Settings.deepstack_customModelPort, AppSettings.Settings.deepstack_customModelMode, AppSettings.Settings.deepstack_customModelApiEnabled);

                DeepStackServerControl.GetDeepStackRun();

                if (this.RB_Medium.Checked)
                    AppSettings.Settings.deepstack_mode = "Medium";
                if (this.RB_Low.Checked)
                    AppSettings.Settings.deepstack_mode = "Low";
                if (this.RB_High.Checked)
                    AppSettings.Settings.deepstack_mode = "High";

                AppSettings.Settings.deepstack_detectionapienabled = this.Chk_DetectionAPI.Checked;
                AppSettings.Settings.deepstack_faceapienabled = this.Chk_FaceAPI.Checked;
                AppSettings.Settings.deepstack_sceneapienabled = this.Chk_SceneAPI.Checked;
                AppSettings.Settings.deepstack_autostart = this.Chk_AutoStart.Checked;
                AppSettings.Settings.deepstack_autoadd = this.chk_AutoAdd.Checked;
                AppSettings.Settings.deepstack_debug = this.Chk_DSDebug.Checked;
                AppSettings.Settings.deepstack_highpriority = this.chk_HighPriority.Checked;
                //AppSettings.Settings.deepstack_adminkey = this.Txt_AdminKey.Text.Trim();
                //AppSettings.Settings.deepstack_apikey = this.Txt_APIKey.Text.Trim();
                AppSettings.Settings.deepstack_installfolder = this.Txt_DeepStackInstallFolder.Text.Trim();
                AppSettings.Settings.deepstack_port = this.Txt_Port.Text.Trim();
                AppSettings.Settings.deepstack_customModelPath = this.Txt_CustomModelPath.Text.Trim();
                AppSettings.Settings.deepstack_customModelName = this.Txt_CustomModelName.Text.Trim();
                AppSettings.Settings.deepstack_customModelPort = this.Txt_CustomModelPort.Text.Trim();
                AppSettings.Settings.deepstack_customModelMode = this.Txt_CustomModelMode.Text.Trim();
                AppSettings.Settings.deepstack_customModelApiEnabled = this.Chk_CustomModelAPI.Checked;

                AppSettings.Settings.deepstack_stopbeforestart = this.chk_stopbeforestart.Checked;

                AppSettings.Settings.deepstack_autorestart = this.Chk_AutoReStart.Checked;
                AppSettings.Settings.deepstack_autorestart_fail_count = GetNumberInt(this.txt_DeepstackRestartFailCount.Text);
                AppSettings.Settings.deepstack_autorestart_minutes_between_restart_attempts = this.txt_DeepstackNoMoreOftenThanMins.Text.ToDouble();

                if (AppSettings.Settings.deepstack_autorestart_fail_count >= AppSettings.Settings.MaxQueueItemRetries)
                {
                    MessageBox.Show($"Note: Deepstack restart fail count is '{AppSettings.Settings.deepstack_autorestart_fail_count}' but the maximum \r\nnumber of times a URL can fail before being disabled is '{AppSettings.Settings.MaxQueueItemRetries}'\r\nTo change, see 'MaxQueueItemRetries' in AITOOL.SETTINGS.JSON file.");
                }

                await AppSettings.SaveAsync(true);


                if (DeepStackServerControl.IsInstalled)
                {
                    if (DeepStackServerControl.IsStarted)
                    {


                        if (DeepStackServerControl.IsActivated)
                        {
                            this.Lbl_BlueStackRunning.Text = "*RUNNING*";
                            this.Lbl_BlueStackRunning.ForeColor = Color.Green;

                            this.Btn_Start.Enabled = false;
                            this.Btn_Stop.Enabled = true;
                        }
                        else
                        {
                            this.Lbl_BlueStackRunning.Text = "*NOT ACTIVATED, RUNNING*";

                            this.Btn_Start.Enabled = false;
                            this.Btn_Stop.Enabled = true;
                        }
                    }
                    else
                    {
                        if (DeepStackServerControl.Starting)
                        {
                            this.Lbl_BlueStackRunning.Text = "STARTING...";
                            this.Lbl_BlueStackRunning.ForeColor = Color.DodgerBlue;
                            this.Btn_Start.Enabled = false;
                            this.Btn_Stop.Enabled = true;

                        }
                        else if (DeepStackServerControl.Stopping)
                        {
                            this.Lbl_BlueStackRunning.Text = "STOPPING...";
                            this.Lbl_BlueStackRunning.ForeColor = Color.DodgerBlue;
                            this.Btn_Start.Enabled = false;
                            this.Btn_Stop.Enabled = false;

                        }
                        else
                        {
                            this.Lbl_BlueStackRunning.Text = "*NOT RUNNING*";
                            this.Lbl_BlueStackRunning.ForeColor = Color.Black;

                            this.Btn_Start.Enabled = true;
                            this.Btn_Stop.Enabled = false;

                        }
                    }
                }
                else
                {
                    this.Btn_Start.Enabled = false;
                    this.Btn_Stop.Enabled = false;
                    this.Lbl_BlueStackRunning.Text = "*NOT INSTALLED*";


                }

                DeepStackServerControl.Update(AppSettings.Settings.deepstack_adminkey, AppSettings.Settings.deepstack_apikey, AppSettings.Settings.deepstack_mode, AppSettings.Settings.deepstack_sceneapienabled, AppSettings.Settings.deepstack_faceapienabled, AppSettings.Settings.deepstack_detectionapienabled, AppSettings.Settings.deepstack_port, AppSettings.Settings.deepstack_customModelPath, AppSettings.Settings.deepstack_stopbeforestart, AppSettings.Settings.deepstack_customModelName, AppSettings.Settings.deepstack_customModelPort, AppSettings.Settings.deepstack_customModelMode, AppSettings.Settings.deepstack_customModelApiEnabled);


            });

        }

        private async Task LoadDeepStackTab()
        {

            try
            {

                Global_GUI.InvokeIFRequired(this, () =>
                {

                    if (DeepStackServerControl == null)
                        DeepStackServerControl = new DeepStack(AppSettings.Settings.deepstack_adminkey, AppSettings.Settings.deepstack_apikey, AppSettings.Settings.deepstack_mode, AppSettings.Settings.deepstack_sceneapienabled, AppSettings.Settings.deepstack_faceapienabled, AppSettings.Settings.deepstack_detectionapienabled, AppSettings.Settings.deepstack_port, AppSettings.Settings.deepstack_customModelPath, AppSettings.Settings.deepstack_stopbeforestart, AppSettings.Settings.deepstack_customModelName, AppSettings.Settings.deepstack_customModelPort, AppSettings.Settings.deepstack_customModelMode, AppSettings.Settings.deepstack_customModelApiEnabled);


                    //first update the port in the deepstack_url if found
                    //string prt = Global.GetWordBetween(AppSettings.Settings.deepstack_url, ":", " |/");
                    //if (!string.IsNullOrEmpty(prt) && (Convert.ToInt32(prt) > 0))
                    //{
                    //    DeepStackServerControl.Port = prt;
                    //}

                    //This will OVERRIDE the port if the deepstack processes found running already have a different port, mode, etc:
                    DeepStackServerControl.GetDeepStackRun();

                    if (string.Equals(DeepStackServerControl.Mode, "medium", StringComparison.OrdinalIgnoreCase))
                        this.RB_Medium.Checked = true;
                    if (string.Equals(DeepStackServerControl.Mode, "low", StringComparison.OrdinalIgnoreCase))
                        this.RB_Low.Checked = true;
                    if (string.Equals(DeepStackServerControl.Mode, "high", StringComparison.OrdinalIgnoreCase))
                        this.RB_High.Checked = true;

                    this.Chk_DetectionAPI.Checked = DeepStackServerControl.DetectionAPIEnabled;
                    this.Chk_FaceAPI.Checked = DeepStackServerControl.FaceAPIEnabled;
                    this.Chk_SceneAPI.Checked = DeepStackServerControl.SceneAPIEnabled;
                    this.Chk_CustomModelAPI.Checked = DeepStackServerControl.CustomModelEnabled;

                    Global_GUI.GroupboxEnableDisable(groupBoxCustomModel, Chk_CustomModelAPI);

                    //have seen a few cases nothing is checked but it is required
                    if (!this.Chk_DetectionAPI.Checked && !this.Chk_FaceAPI.Checked && !this.Chk_SceneAPI.Checked && !this.Chk_CustomModelAPI.Checked)
                    {
                        //If NOTHING else is enabled, force regular detection to be enabled
                        this.Chk_DetectionAPI.Checked = true;
                        DeepStackServerControl.DetectionAPIEnabled = true;
                    }

                    this.Chk_AutoStart.Checked = AppSettings.Settings.deepstack_autostart;
                    this.chk_AutoAdd.Checked = AppSettings.Settings.deepstack_autoadd;
                    this.Chk_DSDebug.Checked = AppSettings.Settings.deepstack_debug;
                    this.chk_HighPriority.Checked = AppSettings.Settings.deepstack_highpriority;
                    //this.Txt_AdminKey.Text = DeepStackServerControl.AdminKey;
                    //this.Txt_APIKey.Text = DeepStackServerControl.APIKey;
                    this.Txt_DeepStackInstallFolder.Text = DeepStackServerControl.DeepStackFolder;
                    this.Txt_Port.Text = DeepStackServerControl.Port;
                    this.Txt_CustomModelPath.Text = AppSettings.Settings.deepstack_customModelPath;
                    this.Txt_CustomModelName.Text = AppSettings.Settings.deepstack_customModelName;
                    this.Txt_CustomModelPort.Text = AppSettings.Settings.deepstack_customModelPort;
                    this.Txt_CustomModelMode.Text = AppSettings.Settings.deepstack_customModelMode;
                    this.chk_stopbeforestart.Checked = AppSettings.Settings.deepstack_stopbeforestart;

                    this.Chk_AutoReStart.Checked = AppSettings.Settings.deepstack_autorestart;
                    this.txt_DeepstackRestartFailCount.Text = AppSettings.Settings.deepstack_autorestart_fail_count.ToString();
                    this.txt_DeepstackNoMoreOftenThanMins.Text = AppSettings.Settings.deepstack_autorestart_minutes_between_restart_attempts.ToString();

                    if (!DeepStackServerControl.IsNewVersion)
                    {
                        this.Txt_CustomModelPath.Enabled = false;
                        this.Txt_CustomModelName.Enabled = false;
                        this.Txt_CustomModelPort.Enabled = false;
                    }

                    this.tb_DeepstackCommandLine.Text = DeepStackServerControl.CommandLine;
                    this.tb_DeepStackURLs.Text = DeepStackServerControl.URLS;

                    //if (prt != Txt_Port.Text)
                    //{
                    //    //server:port/maybe/more/path
                    //    string serv = Global.GetWordBetween(AppSettings.Settings.deepstack_url, "", ":");
                    //    if (!string.IsNullOrEmpty(serv))
                    //    {
                    //        tbDeepstackUrl.Text = serv + ":" + Txt_Port.Text;
                    //        //AppSettings.Settings.deepstack_url = serv + ":" + Txt_Port.Text;
                    //        //AppSettings.Settings.deepstack_url = tbDeepstackUrl.Text;
                    //        //AppSettings.Save();
                    //    }
                    //}

                    if (DeepStackServerControl.IsInstalled)
                    {
                        lbl_deepstackname.Text = DeepStackServerControl.DisplayName;
                        lbl_Deepstackversion.Text = DeepStackServerControl.DisplayVersion;
                        lbl_DeepstackType.Text = DeepStackServerControl.Type.ToString();

                        if (DeepStackServerControl.IsStarted && !DeepStackServerControl.HasError)
                        {
                            if (DeepStackServerControl.IsActivated && (DeepStackServerControl.VisionDetectionRunning || DeepStackServerControl.DetectionAPIEnabled || DeepStackServerControl.CustomModelEnabled || DeepStackServerControl.FaceAPIEnabled))
                            {

                                this.Lbl_BlueStackRunning.Text = "*RUNNING*";
                                this.Lbl_BlueStackRunning.ForeColor = Color.Green;

                                this.Btn_Start.Enabled = false;
                                this.Btn_Stop.Enabled = true;
                            }
                            else if (!DeepStackServerControl.IsActivated)
                            {
                                this.Lbl_BlueStackRunning.Text = "*NOT ACTIVATED, RUNNING*";

                                this.Btn_Start.Enabled = false;
                                this.Btn_Stop.Enabled = true;
                            }
                            else if (!DeepStackServerControl.VisionDetectionRunning || DeepStackServerControl.DetectionAPIEnabled)
                            {
                                this.Lbl_BlueStackRunning.Text = "*DETECTION API NOT RUNNING*";

                                this.Btn_Start.Enabled = false;
                                this.Btn_Stop.Enabled = true;
                            }

                        }
                        else if (DeepStackServerControl.HasError)
                        {
                            this.Lbl_BlueStackRunning.Text = "*ERROR*";
                            this.Lbl_BlueStackRunning.ForeColor = Color.Red;

                            this.Btn_Start.Enabled = true;
                            this.Btn_Stop.Enabled = true;
                        }
                        else
                        {
                            if (DeepStackServerControl.Starting)
                            {
                                this.Lbl_BlueStackRunning.Text = "STARTING...";
                                this.Lbl_BlueStackRunning.ForeColor = Color.DodgerBlue;
                                this.Btn_Start.Enabled = false;
                                this.Btn_Stop.Enabled = true;

                            }
                            else if (DeepStackServerControl.Stopping)
                            {
                                this.Lbl_BlueStackRunning.Text = "STOPPING...";
                                this.Lbl_BlueStackRunning.ForeColor = Color.DodgerBlue;
                                this.Btn_Start.Enabled = false;
                                this.Btn_Stop.Enabled = false;

                            }
                            else
                            {
                                this.Lbl_BlueStackRunning.Text = "*NOT RUNNING*";
                                this.Lbl_BlueStackRunning.ForeColor = Color.Black;

                                this.Btn_Start.Enabled = true;
                                this.Btn_Stop.Enabled = false;

                            }
                            //if (this.Chk_AutoStart.Checked && StartIfNeeded)
                            //{
                            //    if (await DeepStackServerControl.StartDeepstackAsync())
                            //    {
                            //        if (DeepStackServerControl.IsStarted && !DeepStackServerControl.HasError)
                            //        {
                            //            LabelUpdate = delegate { this.Lbl_BlueStackRunning.Text = "*RUNNING*"; };
                            //            this.Invoke(LabelUpdate);
                            //            this.Btn_Start.Enabled = false;
                            //            this.Btn_Stop.Enabled = true;
                            //        }
                            //        else if (DeepStackServerControl.HasError)
                            //        {
                            //            LabelUpdate = delegate { this.Lbl_BlueStackRunning.Text = "*ERROR*"; };
                            //            this.Invoke(LabelUpdate);

                            //            this.Btn_Start.Enabled = false;
                            //            this.Btn_Stop.Enabled = true;
                            //        }

                            //    }
                            //    else
                            //    {
                            //        LabelUpdate = delegate { this.Lbl_BlueStackRunning.Text = "*ERROR*"; };
                            //        this.Invoke(LabelUpdate);

                            //        this.Btn_Start.Enabled = false;
                            //        this.Btn_Stop.Enabled = true;
                            //    }
                            //}
                        }
                    }
                    else
                    {
                        this.Btn_Start.Enabled = false;
                        this.Btn_Stop.Enabled = false;
                        this.Lbl_BlueStackRunning.Text = "*NOT INSTALLED*";


                    }

                });


            }
            catch (Exception ex)
            {

                Log(ex.Msg());
            }
        }

        private async void Btn_Start_Click(object sender, EventArgs e)
        {
            Global_GUI.InvokeIFRequired(this, () =>
            {
                this.Lbl_BlueStackRunning.Text = "STARTING...";
                this.Btn_Start.Enabled = false;
                this.Btn_Stop.Enabled = false;

            });
            await this.SaveDeepStackTabAsync();
            await DeepStackServerControl.StartDeepstackAsync();
            //MessageBox.Show("Started");
            await this.LoadDeepStackTab();

        }

        private async void Btn_Save_Click(object sender, EventArgs e)
        {
            await this.SaveDeepStackTabAsync();
        }

        private async void Btn_Stop_Click(object sender, EventArgs e)
        {
            Global_GUI.InvokeIFRequired(this, () =>
            {
                this.Lbl_BlueStackRunning.Text = "STOPPING...";
                this.Btn_Start.Enabled = false;
                this.Btn_Stop.Enabled = false;

            });
            await this.SaveDeepStackTabAsync();
            await DeepStackServerControl.StopDeepstackAsync();
            //MessageBox.Show("Stopped");
            await this.LoadDeepStackTab();
        }


        private void btnViewLog_Click(object sender, EventArgs e)
        {
            OpenLogFile();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.ShowErrors();
        }

        private void Chk_AutoScroll_CheckedChanged(object sender, EventArgs e)
        {
            AppSettings.Settings.Autoscroll_log = this.Chk_AutoScroll.Checked;
        }

        private void tableLayoutPanel7_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
            {
                if (!string.IsNullOrEmpty(this.cmbcaminput.Text))
                {
                    dialog.InitialDirectory = this.cmbcaminput.Text;

                }
                dialog.IsFolderPicker = true;
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    this.cmbcaminput.Text = dialog.FileName;
                }
            }
        }


        private void BtnDynamicMaskingSettings_Click(object sender, EventArgs e)
        {
            if (this.FOLV_Cameras.SelectedObjects.Count == 0)
            {
                MessageBox.Show("Select a camera first");
                return;
            }

            using (Frm_DynamicMasking frm = new Frm_DynamicMasking())
            {
                Camera cam = AITOOL.GetCamera(((Camera)this.FOLV_Cameras.SelectedObjects[0]).Name);
                frm.cam = cam;
                frm.Text = "Dynamic Masking Settings - " + cam.Name;

                //Merge ClassObject's code
                frm.num_history_mins.Value = cam.maskManager.HistorySaveMins;//load minutes to retain history objects that have yet to become masks
                frm.num_mask_create.Value = cam.maskManager.HistoryThresholdCount; // load mask create counter
                frm.num_mask_remove.Value = cam.maskManager.MaskRemoveMins; //load mask remove counter
                frm.num_percent_var.Value = (decimal)cam.maskManager.PercentMatch;
                frm.numMaskThreshold.Value = cam.maskManager.MaskRemoveThreshold;

                frm.num_max_unused.Value = cam.maskManager.MaxMaskUnusedDays;

                frm.cb_enabled.Checked = this.cb_masking_enabled.Checked;

                frm.lbl_objects.Text = cam.maskManager.MaskTriggeringObjects.ToString();

                if (frm.ShowDialog() == DialogResult.OK)
                {
                    ////get masking values from textboxes

                    cam.maskManager.HistorySaveMins = frm.num_history_mins.Text.ToInt();
                    cam.maskManager.HistoryThresholdCount = frm.num_mask_create.Text.ToInt();
                    cam.maskManager.MaskRemoveMins = frm.num_mask_remove.Text.ToInt();
                    cam.maskManager.MaskRemoveThreshold = frm.numMaskThreshold.Text.ToInt();
                    cam.maskManager.PercentMatch = frm.num_percent_var.Text.ToDouble();
                    cam.maskManager.MaxMaskUnusedDays = frm.num_max_unused.Text.ToInt();

                    this.cb_masking_enabled.Checked = frm.cb_enabled.Checked;
                    cam.maskManager.MaskingEnabled = this.cb_masking_enabled.Checked;
                    //cam.maskManager.Objects = frm.tb_objects.Text.Trim();

                    AppSettings.SaveAsync(true);
                }
            }
        }

        private void btnDetails_Click(object sender, EventArgs e)
        {
            if (this.FOLV_Cameras.SelectedObjects.Count == 0)
            {
                MessageBox.Show("Select a camera first");
                return;
            }
            this.ShowMaskDetailsDialog(((Camera)this.FOLV_Cameras.SelectedObjects[0]).Name);
        }

        private void ShowMaskDetailsDialog(string cameraname)
        {
            using (Frm_DynamicMaskDetails frm = new Frm_DynamicMaskDetails())
            {

                Camera CurCam = GetCamera(cameraname);
                frm.cam = CurCam;

                frm.ShowDialog();

                this.cb_masking_enabled.Checked = CurCam.maskManager.MaskingEnabled;

            }

        }





        private void btnCustomMask_Click(object sender, EventArgs e)
        {
            if (this.FOLV_Cameras.SelectedObjects.Count == 0)
            {
                MessageBox.Show("Select a camera first");
                return;
            }
            this.ShowEditImageMaskDialog(((Camera)this.FOLV_Cameras.SelectedObjects[0]).Name);
        }

        private void ShowEditImageMaskDialog(string cameraname)
        {
            using (Frm_CustomMasking frm = new Frm_CustomMasking())
            {
                Camera cam = AITOOL.GetCamera(cameraname);
                frm.Cam = cam;

                if (frm.ShowDialog() == DialogResult.OK)
                {
                    cam.mask_brush_size = frm.BrushSize;
                }
            }

        }

        //private void btnActions_Click(object sender, EventArgs e)
        //{
        //    using (Frm_LegacyActions frm = new Frm_LegacyActions())
        //    {


        //        Camera cam = AITOOL.GetCamera(list2.SelectedItems[0].Text);
        //        frm.cam = cam;

        //        frm.tbTriggerUrl.Text = string.Join("\r\n", Global.Split(cam.trigger_urls_as_string, "\r\n|;,"));
        //        frm.tbCancelUrl.Text = string.Join("\r\n", Global.Split(cam.cancel_urls_as_string, "\r\n|;,"));
        //        frm.tb_cooldown.Text = cam.cooldown_time.ToString(); //load cooldown time
        //        //load telegram image sending on/off option
        //        frm.cb_telegram.Checked = cam.telegram_enabled;

        //        frm.cb_copyAlertImages.Checked = cam.Action_image_copy_enabled;
        //        frm.tb_network_folder_filename.Text = cam.Action_network_folder_filename;
        //        frm.tb_network_folder.Text = cam.Action_network_folder;
        //        frm.cb_RunProgram.Checked = cam.Action_RunProgram;

        //        if (frm.ShowDialog() == DialogResult.OK)
        //        {
        //            cam.trigger_urls_as_string = string.Join(",", Global.Split(frm.tbTriggerUrl.Text.Trim(), "\r\n|;,"));
        //            cam.cancel_urls_as_string = string.Join(",", Global.Split(frm.tbCancelUrl.Text.Trim(), "\r\n|;,"));
        //            cam.cancel_urls = Global.Split(cam.cancel_urls_as_string, "\r\n|;,").ToArray();
        //            cam.cooldown_time = Convert.ToDouble(frm.tb_cooldown.Text.Trim());
        //            cam.telegram_enabled = frm.cb_telegram.Checked;
        //            cam.Action_image_copy_enabled = frm.cb_copyAlertImages.Checked;
        //            cam.Action_network_folder = frm.tb_network_folder.Text.Trim();
        //            cam.Action_network_folder_filename = frm.tb_network_folder_filename.Text;
        //            cam.Action_RunProgram = frm.cb_RunProgram.Checked;
        //            cam.Action_RunProgramString = frm.tb_RunExternalProgram.Text;

        //            AppSettings.Save();

        //        }
        //    }
        //}

        private void btnActions_Click_1(object sender, EventArgs e)
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            if (this.FOLV_Cameras.SelectedObjects.Count == 0)
            {
                MessageBox.Show("Select a camera first");
                return;
            }

            using (Frm_LegacyActions frm = new Frm_LegacyActions())
            {

                Camera cam = AITOOL.GetCamera(((Camera)this.FOLV_Cameras.SelectedObjects[0]).Name);

                frm.cam = cam;

                frm.tb_DetectionFormat.Text = cam.DetectionDisplayFormat;
                frm.lbl_DetectionFormat.Text = AITOOL.ReplaceParams(cam, null, null, frm.tb_DetectionFormat.Text, Global.IPType.Path);

                frm.tb_ConfidenceFormat.Text = AppSettings.Settings.DisplayPercentageFormat;
                frm.lbl_Confidence.Text = string.Format(frm.tb_ConfidenceFormat.Text, 99.123);


                if (cam.cancel_urls_as_string.IsEmpty())
                    cam.Action_CancelURL_Enabled = false;
                if (cam.trigger_urls_as_string.IsEmpty())
                    cam.Action_TriggerURL_Enabled = false;

                frm.cb_UrlTriggerEnabled.Checked = cam.Action_TriggerURL_Enabled;
                frm.cb_UrlCancelEnabled.Checked = cam.Action_CancelURL_Enabled;

                frm.tbTriggerUrl.Text = cam.trigger_urls_as_string.SplitStr("\r\n|").JoinStr("\r\n");
                frm.tbCancelUrl.Text = cam.cancel_urls_as_string.SplitStr("\r\n|").JoinStr("\r\n");

                frm.tb_ActionDelayMS.Text = AppSettings.Settings.ActionDelayMS.ToString();
                frm.tb_cooldown.Text = cam.cooldown_time_seconds.ToString(); //load cooldown time
                frm.tb_sound_cooldown.Text = cam.sound_cooldown_time_seconds.ToString(); //load cooldown time

                frm.tb_NetworkFolderCleanupDays.Text = cam.Action_network_folder_purge_older_than_days.ToString();

                //load telegram image sending on/off option
                frm.cb_telegram.Checked = cam.telegram_enabled;
                frm.tb_telegram_caption.Text = cam.telegram_caption;
                //frm.tb_telegram_triggering_objects.Text = cam.telegram_triggering_objects;

                frm.cb_telegram_active_time.Text = cam.telegram_active_time_range;

                frm.cb_copyAlertImages.Checked = cam.Action_image_copy_enabled;
                frm.tb_network_folder_filename.Text = cam.Action_network_folder_filename;
                frm.tb_network_folder.Text = cam.Action_network_folder;

                frm.cb_RunProgram.Checked = cam.Action_RunProgram;
                frm.tb_RunExternalProgram.Text = cam.Action_RunProgramString;
                frm.tb_RunExternalProgramArgs.Text = cam.Action_RunProgramArgsString;

                frm.cb_PlaySound.Checked = cam.Action_PlaySounds;
                frm.tb_Sounds.Text = cam.Action_Sounds;

                frm.cb_MQTT_enabled.Checked = cam.Action_mqtt_enabled;
                frm.tb_MQTT_Payload.Text = cam.Action_mqtt_payload;
                frm.tb_MQTT_Topic.Text = cam.Action_mqtt_topic;
                frm.tb_MQTT_Payload_cancel.Text = cam.Action_mqtt_payload_cancel;
                frm.tb_MQTT_Topic_Cancel.Text = cam.Action_mqtt_topic_cancel;
                frm.cb_MQTT_SendImage.Checked = cam.Action_mqtt_send_image;

                frm.cb_Pushover_Enabled.Checked = cam.Action_pushover_enabled;
                frm.tb_Pushover_Title.Text = cam.Action_pushover_title;
                frm.tb_Pushover_Message.Text = cam.Action_pushover_message;
                frm.tb_Pushover_Device.Text = cam.Action_pushover_device;
                //frm.tb_pushover_triggering_objects.Text = cam.Action_pushover_triggering_objects;
                frm.tb_Pushover_Priority.Text = cam.Action_pushover_Priority;
                frm.tb_Pushover_sound.Text = cam.Action_pushover_Sound;
                frm.cb_pushover_active_time.Text = cam.Action_pushover_active_time_range;

                frm.cb_queue_actions.Checked = cam.Action_queued;

                frm.cb_ActivateBlueIrisWindow.Checked = cam.Action_ActivateBlueIrisWindow;

                frm.cb_mergeannotations.Checked = cam.Action_image_merge_detections;

                frm.tb_jpeg_merge_quality.Text = cam.Action_image_merge_jpegquality.ToString();

                Global_GUI.GroupboxEnableDisable(frm.groupBoxPushover, frm.cb_Pushover_Enabled);
                Global_GUI.GroupboxEnableDisable(frm.groupBoxTelegram, frm.cb_telegram);
                Global_GUI.GroupboxEnableDisable(frm.groupBoxMQTT, frm.cb_MQTT_enabled);
                Global_GUI.GroupboxEnableDisable(frm.groupBoxUrlTrigger, frm.cb_UrlTriggerEnabled);
                Global_GUI.GroupboxEnableDisable(frm.groupBoxUrlCancel, frm.cb_UrlCancelEnabled);

                frm.tb_Sounds.Enabled = frm.cb_PlaySound.Checked;
                frm.tb_RunExternalProgram.Enabled = frm.cb_RunProgram.Checked;
                frm.tb_RunExternalProgramArgs.Enabled = frm.cb_RunProgram.Checked;
                frm.tb_network_folder.Enabled = frm.cb_copyAlertImages.Checked;
                frm.tb_network_folder_filename.Enabled = frm.cb_copyAlertImages.Checked;

                frm.tb_ActionCancelSecs.Text = AppSettings.Settings.ActionCancelSeconds.ToString();

                frm.cb_ShowOnlyRelevant.Checked = AppSettings.Settings.HistoryOnlyDisplayRelevantObjects;

                if (frm.cb_mergeannotations.Checked)
                    frm.tb_jpeg_merge_quality.Enabled = true;
                else
                    frm.tb_jpeg_merge_quality.Enabled = false;

                if (frm.ShowDialog() == DialogResult.OK)
                {
                    cam.DetectionDisplayFormat = frm.tb_DetectionFormat.Text.Trim();
                    AppSettings.Settings.DisplayPercentageFormat = frm.tb_ConfidenceFormat.Text.Trim();

                    //clean up the trigger lists by splitting and re-joining 
                    cam.trigger_urls_as_string = frm.tbTriggerUrl.Text.Trim().SplitStr("\r\n|").JoinStr("|");
                    cam.trigger_urls = cam.trigger_urls_as_string.SplitStr("\r\n|").ToArray();

                    cam.cancel_urls_as_string = frm.tbCancelUrl.Text.Trim().SplitStr("\r\n|").JoinStr("|");
                    cam.cancel_urls = cam.cancel_urls_as_string.SplitStr("\r\n|").ToArray();

                    cam.Action_TriggerURL_Enabled = frm.cb_UrlTriggerEnabled.Checked;
                    cam.Action_CancelURL_Enabled = frm.cb_UrlCancelEnabled.Checked;

                    if (cam.cancel_urls.Count() == 0)
                        cam.Action_CancelURL_Enabled = false;
                    if (cam.trigger_urls.Count() == 0)
                        cam.Action_TriggerURL_Enabled = false;

                    AppSettings.Settings.ActionDelayMS = GetNumberInt(frm.tb_ActionDelayMS.Text.Trim());
                    cam.cooldown_time_seconds = GetNumberInt(frm.tb_cooldown.Text.Trim());
                    cam.sound_cooldown_time_seconds = GetNumberInt(frm.tb_sound_cooldown.Text.Trim());
                    cam.telegram_enabled = frm.cb_telegram.Checked;
                    cam.telegram_caption = frm.tb_telegram_caption.Text.Trim();
                    //cam.telegram_triggering_objects = frm.tb_telegram_triggering_objects.Text.Trim();

                    cam.Action_network_folder_purge_older_than_days = GetNumberInt(frm.tb_NetworkFolderCleanupDays.Text.Trim());

                    cam.telegram_active_time_range = frm.cb_telegram_active_time.Text.Trim();

                    cam.Action_image_copy_enabled = frm.cb_copyAlertImages.Checked;
                    cam.Action_network_folder = frm.tb_network_folder.Text.Trim();
                    cam.Action_network_folder_filename = frm.tb_network_folder_filename.Text;

                    cam.Action_RunProgram = frm.cb_RunProgram.Checked;
                    cam.Action_RunProgramString = frm.tb_RunExternalProgram.Text.Trim();
                    cam.Action_RunProgramArgsString = frm.tb_RunExternalProgramArgs.Text.Trim();

                    cam.Action_PlaySounds = frm.cb_PlaySound.Checked;
                    cam.Action_Sounds = frm.tb_Sounds.Text.Trim();

                    cam.Action_mqtt_enabled = frm.cb_MQTT_enabled.Checked;
                    cam.Action_mqtt_payload = frm.tb_MQTT_Payload.Text.Trim();
                    cam.Action_mqtt_topic = frm.tb_MQTT_Topic.Text.Trim();
                    cam.Action_mqtt_payload_cancel = frm.tb_MQTT_Payload_cancel.Text.Trim();
                    cam.Action_mqtt_topic_cancel = frm.tb_MQTT_Topic_Cancel.Text.Trim();
                    cam.Action_mqtt_send_image = frm.cb_MQTT_SendImage.Checked;

                    cam.Action_pushover_enabled = frm.cb_Pushover_Enabled.Checked;
                    cam.Action_pushover_title = frm.tb_Pushover_Title.Text.Trim();
                    cam.Action_pushover_message = frm.tb_Pushover_Message.Text.Trim();
                    cam.Action_pushover_device = frm.tb_Pushover_Device.Text.Trim();
                    //cam.Action_pushover_triggering_objects = frm.tb_pushover_triggering_objects.Text.Trim();
                    cam.Action_pushover_Sound = frm.tb_Pushover_sound.Text.Trim();
                    cam.Action_pushover_Priority = frm.tb_Pushover_Priority.Text.Trim();
                    cam.Action_pushover_active_time_range = frm.cb_pushover_active_time.Text.Trim();

                    cam.Action_image_merge_detections = frm.cb_mergeannotations.Checked;

                    cam.Action_image_merge_jpegquality = Convert.ToInt64(frm.tb_jpeg_merge_quality.Text);

                    cam.Action_queued = frm.cb_queue_actions.Checked;
                    cam.Action_ActivateBlueIrisWindow = frm.cb_ActivateBlueIrisWindow.Checked;

                    AppSettings.Settings.ActionCancelSeconds = GetNumberInt(frm.tb_ActionCancelSecs.Text);
                    AppSettings.Settings.HistoryOnlyDisplayRelevantObjects = frm.cb_ShowOnlyRelevant.Checked;


                    cam.UpdateCamera();

                    UpdateActionsLabel(cam);

                    AppSettings.SaveAsync(true);

                }
            }
        }

        private void UpdateActionsLabel(Camera cam)
        {
            Lbl_Actions.Text = "";
            if (cam.Action_TriggerURL_Enabled)
                Lbl_Actions.Text += "TriggerURL";
            if (cam.Action_CancelURL_Enabled)
                Lbl_Actions.Text += ", CancelURL";
            if (cam.telegram_enabled)
                Lbl_Actions.Text += ", Telegram";
            if (cam.Action_pushover_enabled)
                Lbl_Actions.Text += ", Pushover";
            if (cam.Action_mqtt_enabled)
                Lbl_Actions.Text += ", MQTT";
            if (cam.Action_RunProgram)
                Lbl_Actions.Text += ", Run";
            if (cam.Action_PlaySounds)
                Lbl_Actions.Text += ", Sound";
            if (cam.Action_image_copy_enabled)
                Lbl_Actions.Text += ", Copy";

            if (Lbl_Actions.Text.IsEmpty())
                Lbl_Actions.Text = "No Actions enabled.";
            else
                Lbl_Actions.Text = Lbl_Actions.Text.Trim(", ".ToCharArray());

        }

        private void tbDeepstackUrl_TextChanged(object sender, EventArgs e)
        {

        }

        private void tabLog_Click(object sender, EventArgs e)
        {

        }

        private void folv_history_SelectionChanged(object sender, EventArgs e)
        {

            if (IsClosing)
                return;

            string filename = "";

            try
            {
                if (this.folv_history.SelectedObjects != null && this.folv_history.SelectedObjects.Count > 0)
                {
                    History hist = (History)this.folv_history.SelectedObjects[0];

                    if (hist == null)
                        return;

                    filename = hist.Filename;

                    if (!filename.IsEmpty() && filename.Contains("\\") && File.Exists(filename))
                    {
                        this.lbl_objects.Text = $"Loading Image {filename}...";

                        using ClsImageQueueItem imgq = new ClsImageQueueItem(filename, 0);
                        if (imgq.IsValid())
                        {
                            this.pictureBox1.BackgroundImage = imgq.ToImage(); //load actual image as background, so that an overlay can be added as the image
                        }
                        this.showHideMask();
                        this.lbl_objects.Text = hist.Detections;
                    }
                    else
                    {
                        Log("Removing missing file from database: " + filename);
                        HistoryDB.DeleteHistoryQueue(filename);
                        this.lbl_objects.Text = "Image not found";
                        this.pictureBox1.BackgroundImage = null;
                    }

                    if (!string.IsNullOrEmpty(hist.PredictionsJSON))
                    {
                        this.toolStripButtonDetails.Enabled = true;
                    }
                    else
                    {
                        this.toolStripButtonDetails.Enabled = false;
                    }

                    this.toolStripButtonEditImageMask.Enabled = true;
                    this.toolStripButtonMaskDetails.Enabled = true;
                    this.toolStripButtonEditURL.Enabled = true;

                }
                else
                {
                    this.lbl_objects.Text = "No selection";
                    this.pictureBox1.BackgroundImage = null;
                    this.toolStripButtonDetails.Enabled = false;
                    this.toolStripButtonEditImageMask.Enabled = false;
                    this.toolStripButtonMaskDetails.Enabled = false;
                    this.toolStripButtonEditURL.Enabled = false;
                }

            }
            catch (Exception ex)
            {
                Log($"Error: {ex.Msg()} - Hist.Filename={filename}");

            }



        }

        private void folv_history_FormatRow(object sender, BrightIdeasSoftware.FormatRowEventArgs e)
        {
            this.FormatHistoryRow(sender, e);
        }

        private void FormatHistoryRow(object Sender, BrightIdeasSoftware.FormatRowEventArgs e)
        {
            try
            {
                History hist = (History)e.Model;

                // If SPI IsNot Nothing Then
                if (hist.Success && !hist.HasError)
                    e.Item.ForeColor = Color.Green;
                else if (hist.HasError)
                {
                    e.Item.ForeColor = Color.Black;
                    e.Item.BackColor = Color.Red;
                }
                else if (hist.WasSkipped)
                    e.Item.ForeColor = Color.Red;
                else if (hist.WasMasked)
                {
                    e.Item.ForeColor = Color.Black;
                    e.Item.BackColor = Color.LightGray;
                }
                else if (!hist.Success && hist.Detections.IndexOf("false alert", StringComparison.OrdinalIgnoreCase) >= 0)
                    e.Item.ForeColor = Color.Gray;
                else
                    e.Item.ForeColor = Color.Black;
            }


            catch (Exception)
            {
            }
            // Log("Error: " & ex.Msg())
            finally
            {
            }
        }

        private void FormatCameraRow(object Sender, BrightIdeasSoftware.FormatRowEventArgs e)
        {
            try
            {
                Camera cam = (Camera)e.Model;


                if (!cam.enabled)
                    e.Item.ForeColor = Color.Gray;
                else if (cam.Name.Equals("default", StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(cam.Prefix))
                    e.Item.ForeColor = Color.Brown;
                else
                    e.Item.ForeColor = Color.Black;
            }


            catch (Exception)
            {
            }
            // Log("Error: " & ex.Msg())
            finally
            {
            }
        }
        private void btn_resetstats_Click(object sender, EventArgs e)
        {

            if (string.Equals(this.comboBox1.Text.Trim(), "All Cameras", StringComparison.OrdinalIgnoreCase))
            {
                foreach (Camera cam in AppSettings.Settings.CameraList)
                {
                    cam.stats_alerts = 0;
                    cam.stats_irrelevant_alerts = 0;
                    cam.stats_false_alerts = 0;
                    cam.stats_skipped_images = 0;
                    cam.stats_skipped_images_session = 0;
                }
            }
            else
            {

                Camera cam = AITOOL.GetCamera(this.comboBox1.Text);  //int i = AppSettings.Settings.CameraList.FindIndex(x => x.name.ToLower().Trim() == comboBox1.Text.ToLower().Trim());
                if (cam != null)
                {
                    cam.stats_alerts = 0;
                    cam.stats_irrelevant_alerts = 0;
                    cam.stats_false_alerts = 0;
                    cam.stats_skipped_images = 0;
                    cam.stats_skipped_images_session = 0;
                }
            }

            fcalc.Clear();
            icalc.Clear();
            lcalc.Clear();
            qcalc.Clear();
            scalc.Clear();
            tcalc.Clear();

            LogMan.ErrorCount = 0;

            AppSettings.SaveAsync(true);

            this.UpdatePieChart(); this.UpdateTimeline(); this.UpdateConfidenceChart();

            this.UpdateStats();
        }

        private async void cb_filter_skipped_CheckedChanged(object sender, EventArgs e)
        {
            await this.LoadHistoryAsync(true, AppSettings.Settings.HistoryFollow).ConfigureAwait(false);
        }

        private async void HistoryUpdateListTimer_Tick(object sender, EventArgs e)
        {
            if (IsClosing)
                return;

            if (!AppSettings.AlreadyRunning)
                Global.SaveRegSetting("LastShutdownState", $"checkpoint: HistoryUpdateTimer: {DateTime.Now}");

            await this.UpdateHistoryAddedRemoved();

            this.UpdateStats();
        }

        private void folv_history_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void toolStripStatusErrors_Click(object sender, EventArgs e)
        {
            this.ShowErrors();
            LogMan.ErrorCount = 0;
        }

        private async void cb_follow_CheckedChanged(object sender, EventArgs e)
        {
            await this.LoadHistoryAsync(true, AppSettings.Settings.HistoryFollow).ConfigureAwait(false);
        }

        private async void comboBox_filter_camera_SelectionChangeCommitted(object sender, EventArgs e)
        {
            await this.LoadHistoryAsync(true, AppSettings.Settings.HistoryFollow).ConfigureAwait(false);
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (this.tabControl1.SelectedIndex == 1)
            {

                this.UpdatePieChart(); this.UpdateTimeline(); this.UpdateConfidenceChart();
            }
        }

        private void Shell_DpiChanged(object sender, DpiChangedEventArgs e)
        {
            //Controls get really messed up when DPI changes when app is open.   Just trying to figure out the right way to handle....
            Log($"[System DPI Changed from {e.DeviceDpiOld} to {e.DeviceDpiNew}]");

            //we need to figure out how to force a full resize of all components - something is preventing that from happening
            //automatically upon DPI change.   A suspicion is the custom DBLayoutPanel, but its a clusterfuck to try to 
            //revert back to TableLayoutPanel for everything.

        }

        private void SaveFilters()
        {
            AppSettings.Settings.HistoryFilterAnimals = cb_filter_animal.Checked;
            AppSettings.Settings.HistoryFilterMasked = cb_filter_masked.Checked;
            AppSettings.Settings.HistoryFilterNoSuccess = cb_filter_nosuccess.Checked;
            AppSettings.Settings.HistoryFilterPeople = cb_filter_person.Checked;
            AppSettings.Settings.HistoryFilterSkipped = cb_filter_skipped.Checked;
            AppSettings.Settings.HistoryFilterRelevant = cb_filter_success.Checked;
            AppSettings.Settings.HistoryFilterVehicles = cb_filter_vehicle.Checked;

            AppSettings.SaveAsync(true);
        }

        private async void cb_filter_success_Click(object sender, EventArgs e)
        {
            SaveFilters();
            await this.LoadHistoryAsync(true, AppSettings.Settings.HistoryFollow).ConfigureAwait(false);
        }

        private async void cb_filter_nosuccess_Click(object sender, EventArgs e)
        {
            SaveFilters();
            await this.LoadHistoryAsync(true, AppSettings.Settings.HistoryFollow).ConfigureAwait(false);
            AppSettings.SaveAsync(true);
        }

        private async void cb_filter_person_Click(object sender, EventArgs e)
        {
            SaveFilters();
            await this.LoadHistoryAsync(true, AppSettings.Settings.HistoryFollow).ConfigureAwait(false);
            AppSettings.SaveAsync(true);
        }

        private async void cb_filter_animal_Click(object sender, EventArgs e)
        {
            SaveFilters();
            await this.LoadHistoryAsync(true, AppSettings.Settings.HistoryFollow).ConfigureAwait(false);
        }

        private async void cb_filter_vehicle_Click(object sender, EventArgs e)
        {
            SaveFilters();
            await this.LoadHistoryAsync(true, AppSettings.Settings.HistoryFollow).ConfigureAwait(false);
        }

        private async void cb_filter_skipped_Click(object sender, EventArgs e)
        {
            SaveFilters();
            await this.LoadHistoryAsync(true, AppSettings.Settings.HistoryFollow).ConfigureAwait(false);
        }

        private async void cb_filter_masked_Click(object sender, EventArgs e)
        {
            SaveFilters();
            await this.LoadHistoryAsync(true, AppSettings.Settings.HistoryFollow).ConfigureAwait(false);
        }

        private async void comboBox_filter_camera_DropDownClosed(object sender, EventArgs e)
        {
            SaveFilters();
            await this.LoadHistoryAsync(true, AppSettings.Settings.HistoryFollow).ConfigureAwait(false);
        }

        private void cb_showMask_Click(object sender, EventArgs e)
        {
            AppSettings.Settings.HistoryShowMask = this.cb_showMask.Checked;
            AppSettings.SaveAsync(true);
            this.showHideMask();
        }

        private void cb_showObjects_CheckedChanged(object sender, EventArgs e)
        {
            if (this.folv_history.SelectedObjects != null && this.folv_history.SelectedObjects.Count > 0)
            {
                this.pictureBox1.Refresh();
            }
        }

        private void automaticallyRefreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AppSettings.Settings.HistoryAutoRefresh = this.automaticallyRefreshToolStripMenuItem.Checked;
            AppSettings.SaveAsync(true);
            this.HistoryStartStop();

        }

        private void cb_showObjects_Click(object sender, EventArgs e)
        {
            AppSettings.Settings.HistoryShowObjects = this.cb_showObjects.Checked;
            AppSettings.SaveAsync(true);
            this.pictureBox1.Refresh();
        }

        private void cb_follow_Click(object sender, EventArgs e)
        {
            AppSettings.Settings.HistoryFollow = this.cb_follow.Checked;
            AppSettings.SaveAsync(true);
        }

        private void toolStripButtonDetails_Click(object sender, EventArgs e)
        {
            this.ViewPredictionDetails();
        }

        private void ViewPredictionDetails()
        {
            try
            {
                List<ClsPrediction> allpredictions = new List<ClsPrediction>();
                string filename = "";
                foreach (History hist in this.folv_history.SelectedObjects)
                {
                    List<ClsPrediction> predictions = hist.Predictions();

                    if (predictions.Count > 0)
                    {
                        allpredictions.AddRange(predictions);
                        filename = hist.Filename;
                    }
                    else
                    {
                        Log($"debug: No predictions for image {hist.Filename}: Json='{hist.PredictionsJSON}'");
                    }

                }

                Frm_ObjectDetail frm = new Frm_ObjectDetail();
                frm.PredictionObjectDetailsList = allpredictions;
                frm.ImageFileName = filename;
                frm.Show();

            }
            catch (Exception ex)
            {

                Log("Error: " + ex.Msg());
            }
        }



        private void testDetectionAgainToolStripMenuItem_Click(object sender, EventArgs e)
        {


            if (this.folv_history.SelectedObjects != null && this.folv_history.SelectedObjects.Count > 0)
            {
                Log("----------------------- TESTING TRIGGERS ----------------------------");

                foreach (History hist in this.folv_history.SelectedObjects)
                {
                    if (!string.IsNullOrEmpty(hist.Filename) && File.Exists(hist.Filename))
                    {
                        //test by copying the file as a new file into the watched folder'
                        string folder = Path.GetDirectoryName(hist.Filename);
                        string filename = Path.GetFileNameWithoutExtension(hist.Filename);
                        //strip out anything after the first _AITOOLTEST_ in the filename
                        filename = filename.GetWord("", "_AITOOLTEST_");
                        string ext = Path.GetExtension(hist.Filename);
                        string testfile = Path.Combine(folder, $"{filename}_AITOOLTEST_{DateTime.Now.TimeOfDay.TotalSeconds.Round(0)}{ext}");
                        File.Copy(hist.Filename, testfile, true);
                        string str = "Created test image file based on last detected object for the camera: " + testfile;
                        Log(str);
                    }
                    else
                    {
                        Log("Error: File does not exist for testing: " + hist.Filename);

                    }


                }

                Log("---------------------- DONE TESTING TRIGGERS -------------------------");

            }

        }

        private void detailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ViewPredictionDetails();
        }

        private async void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await this.LoadHistoryAsync(false, AppSettings.Settings.HistoryFollow).ConfigureAwait(false);
        }

        private void folv_history_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.ViewPredictionDetails();
        }

        private void toolStripButtonMaskDetails_Click(object sender, EventArgs e)
        {

            if (this.folv_history.SelectedObjects != null && this.folv_history.SelectedObjects.Count > 0)
            {
                History hist = (History)this.folv_history.SelectedObjects[0];
                this.ShowMaskDetailsDialog(hist.Camera);

            }

        }

        private void dynamicMaskDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.folv_history.SelectedObjects != null && this.folv_history.SelectedObjects.Count > 0)
            {
                History hist = (History)this.folv_history.SelectedObjects[0];
                this.ShowMaskDetailsDialog(hist.Camera);

            }
        }

        private void toolStripButtonEditImageMask_Click(object sender, EventArgs e)
        {
            if (this.folv_history.SelectedObjects != null && this.folv_history.SelectedObjects.Count > 0)
            {
                History hist = (History)this.folv_history.SelectedObjects[0];
                this.ShowEditImageMaskDialog(hist.Camera);

            }
        }

        private void btn_enabletelegram_Click(object sender, EventArgs e)
        {
            AppSettings.Settings.send_telegram_errors = cb_send_telegram_errors.Checked;
            AppSettings.Settings.send_pushover_errors = cb_send_pushover_errors.Checked;

            foreach (Camera cam in AppSettings.Settings.CameraList)
            {
                if (AppSettings.Settings.send_telegram_errors && !cam.telegram_enabled)
                {
                    cam.telegram_enabled = true;
                    Log($"Enabled Telegram on camera '{cam.Name}'.");
                }
                if (AppSettings.Settings.send_pushover_errors && !cam.Action_pushover_enabled)
                {
                    cam.Action_pushover_enabled = true;
                    Log($"Enabled Pushover on camera '{cam.Name}'.");
                }
            }
            AppSettings.SaveAsync(true);
        }

        private void btn_disabletelegram_Click(object sender, EventArgs e)
        {
            AppSettings.Settings.send_telegram_errors = cb_send_telegram_errors.Checked;
            AppSettings.Settings.send_pushover_errors = cb_send_pushover_errors.Checked;

            foreach (Camera cam in AppSettings.Settings.CameraList)
            {
                if (AppSettings.Settings.send_telegram_errors && cam.telegram_enabled)
                {
                    cam.telegram_enabled = false;
                    Log($"Disabled Telegram on camera '{cam.Name}'.");
                }
                if (AppSettings.Settings.send_pushover_errors && cam.Action_pushover_enabled)
                {
                    cam.Action_pushover_enabled = false;
                    Log($"Disabled Pushover on camera '{cam.Name}'.");
                }
            }
            AppSettings.SaveAsync(true);
        }

        private void storeFalseAlertsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AppSettings.Settings.HistoryStoreFalseAlerts = this.storeFalseAlertsToolStripMenuItem.Checked;
            AppSettings.SaveAsync(true);
        }

        private void storeMaskedAlertsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AppSettings.Settings.HistoryStoreMaskedAlerts = this.storeMaskedAlertsToolStripMenuItem.Checked;
            AppSettings.SaveAsync(true);

        }

        private void showOnlyRelevantObjectsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AppSettings.Settings.HistoryOnlyDisplayRelevantObjects = this.showOnlyRelevantObjectsToolStripMenuItem.Checked;
            AppSettings.SaveAsync(true);
            this.pictureBox1.Refresh();

        }

        private void btnSaveTo_Click(object sender, EventArgs e)
        {
            this.CameraSave(true);
        }

        private async void LogUpdateListTimer_Tick(object sender, EventArgs e)
        {
            await this.UpdateLogAddedRemovedAsync();
        }

        private void Chk_AutoScroll_Click(object sender, EventArgs e)
        {
            AppSettings.Settings.Autoscroll_log = this.Chk_AutoScroll.Checked;
        }



        private void Chk_AutoScroll_Click_1(object sender, EventArgs e)
        {
            AppSettings.Settings.Autoscroll_log = this.Chk_AutoScroll.Checked;
        }

        private void chk_filterErrors_Click(object sender, EventArgs e)
        {
            this.FilterLogErrors();
        }

        private async void FilterLogErrors()
        {
            if (IsLoading)
                return;


            if (this.chk_filterErrors.Checked)
            {
                //filter
                Global_GUI.InvokeIFRequired(this.folv_log, () =>
                {
                    using var cw = new Global_GUI.CursorWait();
                    this.folv_log.ModelFilter = new BrightIdeasSoftware.ModelFilter((object x) =>
                    {
                        ClsLogItm CLI = (ClsLogItm)x;
                        return (CLI.Level == LogLevel.Error || CLI.Level == LogLevel.Warn || CLI.Level == LogLevel.Fatal);
                    });
                });
            }
            else
            {
                this.folv_log.ModelFilter = null;
                await this.UpdateLogAddedRemovedAsync(true);
            }

        }

        private async Task<bool> FilterHistItem(History hist)
        {
            if (IsLoading)
                return false;

            bool ret = false;

            try
            {

                this.toolStripButtonPauseLog.Checked = true; //pause for a bit, and stay paused if results found

                using var cw = new Global_GUI.CursorWait();

                Stopwatch sw = new Stopwatch();

                //first search directly through log files (the history entry may not be from todays log)
                string justfile = Path.GetFileName(hist.Filename);

                List<ClsLogItm> found = new List<ClsLogItm>();

                //AITool.[2020-10-19].log
                //AITool.[2020-10-19.1].log.zip
                List<FileInfo> files = Global.GetFiles(Path.GetDirectoryName(AppSettings.Settings.LogFileName), "AITOOL.[*].LOG|AITOOL.[*].LOG.ZIP", SearchOption.TopDirectoryOnly);

                //sort by date so newest files are searched first
                files = files.OrderByDescending((d) => d.LastWriteTime).ToList();

                string imagemaskkey = "";
                string matchedmaskkey = "";

                foreach (var fi in files)
                {
                    //AITool.[2020-10-19.1].log.zip
                    //        ------------
                    string date = fi.FullName.GetWord("[", "]");
                    //AITool.[2020-10-19.1].log.zip
                    //        ----------
                    date = date.GetWord("", ".");

                    if (string.IsNullOrEmpty(date))
                    {
                        Log("Error: Date in filename is unexpected: " + fi.FullName);
                        continue;
                    }

                    DateTime DATE = DateTime.MinValue;

                    if (Global.GetDateStrict(date, ref DATE, "yyyy-MM-dd"))
                    {
                        if (DATE.ToString("yyyy-MM-dd") == hist.Date.ToString("yyyy-MM-dd") || fi.LastWriteTime.ToString("yyyy-MM-dd") == hist.Date.ToString("yyyy-MM-dd") || fi.CreationTime.ToString("yyyy-MM-dd") == hist.Date.ToString("yyyy-MM-dd"))
                        {
                            //load into memory
                            Log($"Debug: Searching log file (namedate='{DATE.ToString("yyyy-MM-dd")}',moddate='{fi.LastWriteTime.ToString("yyyy-MM-dd")}',createdate='{fi.CreationTime.ToString("yyyy-MM-dd")}'): {fi.Name}...");

                            this.UpdateProgressBar($"Searching {fi.Name}...", 1, 1, 1);

                            List<ClsLogItm> curlist = await LogMan.LoadLogFileAsync(fi.FullName, false, false);

                            this.UpdateProgressBar($"Searching {fi.Name}...", 1, 1, curlist.Count);

                            DateTime FirstSeen = DateTime.MinValue;

                            int fnd = 0;
                            int cnt = 0;
                            foreach (var CLI in curlist)
                            {
                                cnt++;
                                //this.UpdateProgressBar($"Searching {fi.Name}...", cnt, 1, curlist.Count);

                                if (CLI.Detail.IndexOf(justfile, StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    if (FirstSeen == DateTime.MinValue)
                                        FirstSeen = CLI.Date;
                                    fnd++;

                                    if (!found.Contains(CLI))
                                        found.Add(CLI);

                                    // Current object detected: key=2249882, name=Person, xmin=1789,
                                    if (CLI.Detail.IndexOf("Current object detected:", StringComparison.OrdinalIgnoreCase) >= 0)
                                    {
                                        imagemaskkey = "key=" + CLI.Detail.GetWord("key=", ",| ");
                                        Log("Debug: " + imagemaskkey);
                                    }
                                    //   Found 'Person' (Key=191457) in last_positions_history: key=198932, name=Person, xmin=1108
                                    else if (CLI.Detail.IndexOf("last_positions_history: key=", StringComparison.OrdinalIgnoreCase) >= 0)
                                    {
                                        matchedmaskkey = "key=" + CLI.Detail.GetWord("history: key=", ",| ");
                                        Log("Debug: " + matchedmaskkey);
                                    }
                                }
                                else if (CLI.Detail.Contains(imagemaskkey) && ((CLI.Date - FirstSeen).TotalMinutes <= 20 || CLI.Func.StartsWith("CleanUpExpired")))
                                {
                                    if (!found.Contains(CLI))
                                    {
                                        fnd++;
                                        found.Add(CLI);
                                    }
                                }
                                else if (CLI.Detail.Contains(matchedmaskkey) && ((CLI.Date - FirstSeen).TotalMinutes <= 20 || CLI.Func.StartsWith("CleanUpExpired")))
                                {
                                    if (!found.Contains(CLI))
                                    {
                                        fnd++;
                                        found.Add(CLI);
                                    }
                                }
                                else if (string.Equals(CLI.Image, justfile, StringComparison.OrdinalIgnoreCase))
                                {
                                    if (!found.Contains(CLI))
                                    {
                                        fnd++;
                                        found.Add(CLI);
                                    }
                                }
                            }

                            if (curlist.Count > 0)
                                Log($"Debug: ...Found {fnd} out of {curlist.Count} line matches for a total of {found.Count} in {fi.Name}...");
                            else
                                Log("Error: Log may be corrupt or an old format since no lines where returned: " + fi.Name);

                        }
                        else
                        {
                            Log($"Debug: Skipping file because Dates dont match: file ('{DATE.ToString("yyyy-MM-dd")}' != history '{hist.Date.ToString("yyyy-MM-dd")}') :{fi.Name}");
                        }
                    }
                    else
                    {
                        Log($"Error: Could not parse date '{date}' in filename, Hist.Date='{hist.Date.ToString("yyyy-MM-dd")}': " + fi.FullName);
                    }

                }

                Log($"Found {found.Count} total matched lines in {sw.ElapsedMilliseconds}ms for image '{justfile}'.");

                if (found.Count > 0)
                {
                    LogMan.Clear();
                    LogMan.AddRange(found);
                    String search = justfile;
                    if (!string.IsNullOrEmpty(imagemaskkey))
                        search += "|" + imagemaskkey;
                    if (!string.IsNullOrEmpty(matchedmaskkey))
                        search += "|" + matchedmaskkey;
                    this.tabControl1.SelectedTab = this.tabLog;
                    this.ToolStripComboBoxSearch.Text = search;  //this should trigger textchanged and filer in a second.
                }
                else
                {
                    this.toolStripButtonPauseLog.Checked = false; //start
                    MessageBox.Show($"Could not find matching log entries for '{justfile}'?  See log for details. Note this function only works well if the DEBUG logging mode has been enabled.");
                }

                ret = true;
            }
            catch (Exception ex)
            {

                Log("Error: " + ex.Msg());
            }
            finally
            {
                this.UpdateProgressBar($"", 0, 0, 0);
            }

            return ret;
        }

        private void folv_log_FormatRow(object sender, BrightIdeasSoftware.FormatRowEventArgs e)
        {
            if (e.Model != null)
                this.FormatLogRow(sender, e);
        }
        private void FormatLogRow(object Sender, BrightIdeasSoftware.FormatRowEventArgs e)
        {
            try
            {
                ClsLogItm li = (ClsLogItm)e.Model;

                // If SPI IsNot Nothing Then
                if (li.FromFile)
                {
                    e.Item.BackColor = Color.Black;
                }
            }



            catch (Exception)
            {
            }
            finally
            {
            }
        }

        private void folv_log_FormatCell(object sender, BrightIdeasSoftware.FormatCellEventArgs e)
        {
            if (e.Model != null)
                this.FormatCellLog(sender, e);
        }

        private void FormatCellLog(object sender, BrightIdeasSoftware.FormatCellEventArgs e)
        {
            if (e.Column.Name == nameof(ClsLogItm.Detail))
            {
                ClsLogItm li = (ClsLogItm)e.Model;
                if (li.Level == LogLevel.Error || li.Level == LogLevel.Fatal)
                {
                    e.SubItem.ForeColor = Color.White;
                    e.SubItem.BackColor = Color.Red;
                }
                else if (li.Level == LogLevel.Warn)
                {
                    e.SubItem.ForeColor = Color.Red;
                    e.SubItem.BackColor = ((FastObjectListView)sender).BackColor;
                }
                else if (li.Level == LogLevel.Trace || li.Level == LogLevel.Debug)
                {
                    e.SubItem.ForeColor = Color.Gray;
                }
                else if (!string.IsNullOrEmpty(li.Color))
                {
                    e.SubItem.ForeColor = Color.FromName(li.Color);
                }
                else
                {
                    e.SubItem.ForeColor = Color.White;
                }
            }
            else
            {
                e.SubItem.ForeColor = Color.DarkGray;
            }
        }

        private void mnu_highlight_CheckStateChanged(object sender, EventArgs e)
        {
            this.filter_CheckStateChanged(sender, e);

        }

        private void filter_CheckStateChanged(object sender, EventArgs e)
        {

            if (IsLoading)
                return;

            ToolStripMenuItem currentItem = (ToolStripMenuItem)sender;
            ToolStripDropDownButton parentItem = (ToolStripDropDownButton)currentItem.OwnerItem;
            if (currentItem.Checked)
            {
                foreach (ToolStripMenuItem sibling in parentItem.DropDownItems)
                {
                    if (sibling != currentItem)
                    {
                        sibling.Checked = false;
                    }
                }

                if (!this.mnu_Filter.Checked || !this.mnu_Highlight.Checked)
                    this.mnu_Filter.Checked = true;

                AppSettings.Settings.log_mnu_Filter = this.mnu_Filter.Checked;
                AppSettings.Settings.log_mnu_Highlight = this.mnu_Highlight.Checked;

                if (!IsLoading && Global.IsRegexPatternValid(this.ToolStripComboBoxSearch.Text))
                {
                    bool Filter = false;
                    if (this.mnu_Filter.Checked && !this.mnu_Highlight.Checked)
                        Filter = true;
                    else
                        Filter = false;

                    Global_GUI.FilterFOLV(this.folv_log, this.ToolStripComboBoxSearch.Text, Filter);

                }
            }

        }

        private void Log_Filter_CheckStateChanged(object sender, EventArgs e)
        {

            if (IsLoading)
                return;

            ToolStripMenuItem currentItem = (ToolStripMenuItem)sender;
            ToolStripMenuItem parentItem = (ToolStripMenuItem)currentItem.OwnerItem;
            if (currentItem.Checked)
            {
                //uncheck everything else
                foreach (ToolStripMenuItem sibling in parentItem.DropDownItems)
                {
                    if (sibling != currentItem)
                    {
                        sibling.Checked = false;
                    }
                }

                if (!IsLoading)
                {
                    AppSettings.Settings.LogLevel = currentItem.Text;

                    LogMan.UpdateNLog(LogLevel.FromString(AppSettings.Settings.LogLevel), AppSettings.Settings.LogFileName, AppSettings.Settings.MaxLogFileSize, AppSettings.Settings.MaxLogFileAgeDays, AppSettings.Settings.MaxGUILogItems);
                }

                Log($"Debug: Logging level changed to '{currentItem.Text}'");
            }

        }
        private void mnu_Filter_CheckStateChanged(object sender, EventArgs e)
        {
            this.filter_CheckStateChanged(sender, e);
        }

        private void ToolStripComboBoxSearch_Leave(object sender, EventArgs e)
        {
        }

        private void ToolStripComboBoxSearch_TextChanged(object sender, EventArgs e)
        {
            if (IsLoading)
                return;

            if (!this.tmr.Enabled)
            {
                this.tmr.Enabled = true;
                this.tmr.Start();
            }

            this.TimeSinceType = DateTime.Now;

        }

        private void mnu_Filter_Click(object sender, EventArgs e)
        {

        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            OpenLogFile();
        }

        private void OpenLogFile()
        {
            if (System.IO.File.Exists(LogMan.GetCurrentLogFileName()))
            {
                System.Diagnostics.Process.Start(LogMan.GetCurrentLogFileName());
                this.lbl_errors.Text = "";
            }
            else
            {
                MessageBox.Show("log missing");
            }
        }

        private void mnu_log_filter_off_Click(object sender, EventArgs e)
        {

        }

        private void mnu_log_filter_off_CheckStateChanged(object sender, EventArgs e)
        {
            this.Log_Filter_CheckStateChanged(sender, e);
        }

        private void mnu_log_filter_fatal_CheckStateChanged(object sender, EventArgs e)
        {
            this.Log_Filter_CheckStateChanged(sender, e);

        }

        private void mnu_log_filter_error_CheckStateChanged(object sender, EventArgs e)
        {
            this.Log_Filter_CheckStateChanged(sender, e);

        }

        private void mnu_log_filter_warn_CheckStateChanged(object sender, EventArgs e)
        {
            this.Log_Filter_CheckStateChanged(sender, e);

        }

        private void mnu_log_filter_info_CheckStateChanged(object sender, EventArgs e)
        {
            this.Log_Filter_CheckStateChanged(sender, e);

        }

        private void mnu_log_filter_debug_CheckStateChanged(object sender, EventArgs e)
        {
            this.Log_Filter_CheckStateChanged(sender, e);

        }

        private void mnu_log_filter_trace_CheckStateChanged(object sender, EventArgs e)
        {
            this.Log_Filter_CheckStateChanged(sender, e);

        }

        private void clearRecentErrorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LogMan.ErrorCount = 0;
        }

        private async void locateInLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.folv_history.SelectedObjects != null && this.folv_history.SelectedObjects.Count > 0)
            {
                History hist = (History)this.folv_history.SelectedObjects[0];
                await this.FilterHistItem(hist);

            }
        }

        private void toolStripButtonPauseLog_Click(object sender, EventArgs e)
        {
            this.StartPauseLog();
        }

        private void StartPauseLog()
        {
            if (IsLoading)
                return;

            if (!this.toolStripButtonPauseLog.Checked)
            {
                this.LogUpdateListTimer.Enabled = true;
                this.LogUpdateListTimer.Start();
                Log("Started auto log refresh");
            }
            else
            {
                this.LogUpdateListTimer.Stop();
                this.LogUpdateListTimer.Enabled = false;
                Log("Stopped auto log refresh");
            }
        }

        private async void toolStripButtonReload_ClickAsync(object sender, EventArgs e)
        {
            this.ReloadLog();
        }

        private async void ReloadLog()
        {
            if (IsLoading)
                return;

            using var cw = new Global_GUI.CursorWait();
            this.chk_filterErrors.Checked = false;
            this.chk_filterErrorsAll.Checked = false;
            LogMan.Clear();
            this.folv_log.ClearObjects();
            this.folv_log.ModelFilter = null;
            await LogMan.LoadLogFileAsync(LogMan.GetCurrentLogFileName(), true, false);
            Log($"Loaded {LogMan.Values.Count} lines in {LogMan.LastLoadTimeMS}ms from {LogMan.GetCurrentLogFileName()}.");
            await this.UpdateLogAddedRemovedAsync(true);
            this.toolStripButtonPauseLog.Checked = false;

        }

        private void toolStripComboBoxFiles_Click(object sender, EventArgs e)
        {

        }

        private void toolStripComboBoxFiles_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private async void toolStripButtonLoad_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {

                string LastFile = Global.GetRegSetting("LastLoadedLogFile", LogMan.GetCurrentLogFileName());

                ofd.InitialDirectory = Path.GetDirectoryName(LastFile);
                ofd.FileName = LastFile;
                ofd.Title = "Browse for AITOOL Log Files";
                ofd.CheckFileExists = true;
                ofd.CheckPathExists = true;
                ofd.DefaultExt = "log";
                ofd.Filter = "LOG Files (AITOOL.[*.log;AITOOL.[*.zip)|AITOOL.[*.log;AITOOL.[*.zip";
                ofd.FilterIndex = 1;
                ofd.RestoreDirectory = true;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    using var cw = new Global_GUI.CursorWait();
                    this.toolStripButtonPauseLog.Checked = true;
                    this.chk_filterErrors.Checked = false;
                    Global.SaveRegSetting("LastLoadedLogFile", ofd.FileName);
                    LogMan.Clear();
                    this.folv_log.ClearObjects();
                    this.folv_log.ModelFilter = null;
                    await LogMan.LoadLogFileAsync(ofd.FileName, true, false);
                    Log($"Loaded {LogMan.Values.Count} lines in {LogMan.LastLoadTimeMS}ms from {ofd.FileName}.");
                    this.UpdateLogAddedRemovedAsync(true);
                }


            }

        }

        private void chk_filterErrors_Click_1(object sender, EventArgs e)
        {
            this.FilterLogErrors();
        }

        private async void chk_filterErrorsAll_Click(object sender, EventArgs e)
        {
            if (IsLoading)
                return;

            if (!this.chk_filterErrorsAll.Checked)
            {
                this.ReloadLog();
                return;
            }

            try
            {

                this.toolStripButtonPauseLog.Checked = true; //pause for a bit, and stay paused if results found

                this.folv_log.ClearObjects();
                this.folv_log.ModelFilter = null;
                this.folv_log.EmptyListMsg = "Searching...";

                this.StartPauseLog();

                using var cw = new Global_GUI.CursorWait();

                Stopwatch sw = new Stopwatch();

                List<ClsLogItm> found = new List<ClsLogItm>();

                //AITool.[2020-10-19].log
                //AITool.[2020-10-19.1].log.zip
                List<FileInfo> files = Global.GetFiles(AppSettings.Settings.LogFileName, "AITOOL.[*].LOG|AITOOL.[*].LOG.ZIP", SearchOption.TopDirectoryOnly, DateTime.Now.AddDays(-2), DateTime.Now.AddMinutes(1));

                //sort by date so newest files are searched first
                files = files.OrderByDescending((d) => d.LastWriteTime).ToList();

                int cur = 0;
                foreach (var fi in files)
                {
                    try
                    {

                        cur++;


                        //load into memory
                        Log($"Debug: Searching back 2 days - {cur} of {files.Count}: {fi.Name}...", "None", "None", "None");

                        this.UpdateProgressBar($"Searching {cur} of {files.Count}: {fi.Name}...", 1, 1, 1);

                        List<ClsLogItm> curlist = await LogMan.LoadLogFileAsync(fi.FullName, false, false);

                        this.UpdateProgressBar($"Searching {cur} of {files.Count}: {fi.Name}...", 1, 1, curlist.Count);


                        int fnd = 0;
                        int cnt = 0;
                        int CurListCount = curlist.Count;
                        int HalfList = CurListCount / 2;
                        long LastMS = sw.ElapsedMilliseconds;
                        foreach (var CLI in curlist)
                        {
                            cnt++;

                            //if (cnt == 1 || cnt == HalfList || cnt > (CurListCount - 5) || (sw.ElapsedMilliseconds - LastMS >= 500))
                            //{
                            //    Global.UpdateProgressBar($"Searching {cur} of {files.Count}: {fi.Name}...", cnt, 1, CurListCount);
                            //    LastMS = sw.ElapsedMilliseconds;
                            //}

                            if (CLI.Level == LogLevel.Error || CLI.Level == LogLevel.Warn || CLI.Level == LogLevel.Fatal)
                            {
                                fnd++;
                                found.Add(CLI);
                            }

                        }

                        Log($"Debug: ...Found {fnd} of {curlist.Count} lines that had an error for a total of {found.Count} lines in {fi.Name}...");

                    }
                    catch (Exception ex)
                    {

                        Log("Error: " + ex.Msg());
                    }

                }

                Log($"Found {found.Count} errors in {sw.ElapsedMilliseconds}ms");

                if (found.Count > 0)
                {
                    LogMan.Clear();
                    LogMan.AddRange(found);
                    this.UpdateLogAddedRemovedAsync(false);
                }
                else
                {
                    MessageBox.Show($"Could not find any error log entries in {files.Count} files.");
                    this.chk_filterErrorsAll.Checked = false;
                    this.toolStripButtonPauseLog.Checked = false; //start
                    this.StartPauseLog();
                }

            }
            catch (Exception ex)
            {

                Log("Error: " + ex.Msg());
            }
            finally
            {
                this.UpdateProgressBar($"", 0, 0, 0);
            }
        }

        private async void toolStripButton1_Click(object sender, EventArgs e)
        {
            Debug.Print("About to run");
            try
            {
                await Task.Run(() => longrunningtask()).CancelAfter(MasterCTS.Token, "my task canceled");
                Debug.Print("After run");

            }
            catch (Exception ex)
            {
                Debug.Print("Error: " + ex.ToString());
            }
            Debug.Print("Done.");

        }


        private void longrunningtask()
        {
            Debug.Print("Starting sleep...");
            while (true)
            {
                Debug.Print(DateTime.Now + ": working");
                Thread.Sleep(2000);
            }
            Debug.Print("After sleep");  //should never get here

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Debug.Print("Canceling...");
            MasterCTS.Cancel();
            MasterCTS.Dispose();
            MasterCTS = new CancellationTokenSource();
            Debug.Print("Canceled.");
        }

        private void cb_person_CheckedChanged(object sender, EventArgs e)
        {

        }

        private async void manuallyAddImagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {

                string LastFile = Global.GetRegSetting("LastLoadedImageFile", "");

                if (!string.IsNullOrEmpty(LastFile))
                    ofd.InitialDirectory = Path.GetDirectoryName(LastFile);

                ofd.Multiselect = true;
                ofd.FileName = LastFile;
                ofd.Title = "Browse for image files for AI to process";
                ofd.CheckFileExists = true;
                ofd.CheckPathExists = true;
                ofd.DefaultExt = "jpg";
                ofd.Filter = "Image Files (*.jpg)|*.jpg|All files (*.*)|*.*";
                ofd.FilterIndex = 1;
                ofd.RestoreDirectory = true;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    // Read the files
                    foreach (String file in ofd.FileNames)
                    {
                        Global.SaveRegSetting("LastLoadedImageFile", file);
                        AddImageToQueue(file);
                        //small delay
                        await Task.Delay(AppSettings.Settings.loop_delay_ms);
                    }
                }


            }
        }

        private void restrictThresholdAtSourceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AppSettings.Settings.HistoryRestrictMinThresholdAtSource = this.restrictThresholdAtSourceToolStripMenuItem.Checked;
            AppSettings.SaveAsync(true);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            UpdateAIURLs();

            using (Frm_AIServers frm = new Frm_AIServers())
            {

                frm.ShowDialog(this);
                //sort AIURLList so that all items enabled are at the top. Use OrderbyDescending so that the order is preserved
                AppSettings.Settings.AIURLList = AppSettings.Settings.AIURLList.OrderByDescending(x => x.Enabled).ToList();
            }

            UpdateAIURLs();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://docs.deepstack.cc/windows/index.html");
        }

        private void bt_DeepstackReset_Click(object sender, EventArgs e)
        {
            this.Lbl_BlueStackRunning.Text = "RESETTING...";
            this.Btn_Start.Enabled = false;
            this.Btn_Stop.Enabled = false;
            this.Btn_DeepstackReset.Enabled = false;
            this.SaveDeepStackTabAsync();
            DeepStackServerControl.ResetDeepstack();
            this.Btn_DeepstackReset.Enabled = true;
            this.LoadDeepStackTab();
        }

        private void Btn_ViewLog_Click(object sender, EventArgs e)
        {
            string errfile = Path.Combine(Environment.GetEnvironmentVariable("LOCALAPPDATA"), "DeepStack", "logs", "stderr.txt");
            if (File.Exists(errfile))
            {
                if (new FileInfo(errfile).Length > 4)
                {
                    try
                    {
                        Process.Start(errfile);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Error:  Please correct the WINDOWS File Association for .TXT files.");
                    }
                }
                else
                    MessageBox.Show("File has no lines " + errfile);
            }
            else
            {
                MessageBox.Show("Cannot find " + errfile);
            }
        }

        private void Txt_CustomModelName_TextChanged(object sender, EventArgs e)
        {

        }

        private void Chk_CustomModelAPI_CheckedChanged(object sender, EventArgs e)
        {
            Global_GUI.GroupboxEnableDisable(groupBoxCustomModel, Chk_CustomModelAPI);
        }

        private void toolStripButtonEditURL_Click(object sender, EventArgs e)
        {
            using (Frm_AIServerDeepstackEdit frm = new Frm_AIServerDeepstackEdit())
            {
                string srv = ((History)this.folv_history.SelectedObjects[0]).AIServer;
                ClsURLItem url = AITOOL.GetURL(srv, false, false);
                if (url != null)
                {
                    frm.CurURL = url;
                    frm.ShowDialog();
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

            if (MessageBox.Show("Are you sure you want to reset ALL settings?", "RESET?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                ResetSettings = true;
                this.Close();
            }


        }

        private void viewImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.folv_history.SelectedObjects != null && this.folv_history.SelectedObjects.Count > 0)
            {
                History hist = (History)this.folv_history.SelectedObjects[0];
                Process.Start(hist.Filename);

            }
        }

        private void jumpToImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.folv_history.SelectedObjects != null && this.folv_history.SelectedObjects.Count > 0)
            {
                History hist = (History)this.folv_history.SelectedObjects[0];

                // combine the arguments together
                // it doesn't matter if there is a space after ','
                string argument = "/select, \"" + hist.Filename + "\"";

                Process.Start("explorer.exe", argument);
            }

        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dbLayoutPanel11_Paint(object sender, PaintEventArgs e)
        {

        }

        private void BtnPredictionSize_Click(object sender, EventArgs e)
        {
            if (this.FOLV_Cameras.SelectedObjects.Count == 0)
            {
                MessageBox.Show("Select a camera first");
                return;
            }

            using (Frm_PredSizeLimits frm = new Frm_PredSizeLimits())
            {

                Camera cam = AITOOL.GetCamera(((Camera)this.FOLV_Cameras.SelectedObjects[0]).Name);

                frm.tb_ConfidenceLower.Text = cam.threshold_lower.ToString();
                frm.tb_ConfidenceUpper.Text = cam.threshold_upper.ToString();

                frm.tb_maxheight.Text = cam.PredSizeMaxHeight.ToString();
                frm.tb_maxwidth.Text = cam.PredSizeMaxWidth.ToString();
                frm.tb_minwidth.Text = cam.PredSizeMinWidth.ToString();
                frm.tb_minheight.Text = cam.PredSizeMinHeight.ToString();
                frm.tb_maxpercent.Text = cam.PredSizeMaxPercentOfImage.ToString();
                frm.tb_MinPercent.Text = cam.PredSizeMinPercentOfImage.ToString();

                frm.tb_duplicatepercent.Text = cam.MergePredictionsMinMatchPercent.ToString();

                if (frm.ShowDialog() == DialogResult.OK)
                {
                    cam.threshold_lower = GetNumberInt(frm.tb_ConfidenceLower.Text);
                    cam.threshold_upper = GetNumberInt(frm.tb_ConfidenceUpper.Text);
                    cam.PredSizeMaxHeight = GetNumberInt(frm.tb_maxheight.Text);
                    cam.PredSizeMaxWidth = GetNumberInt(frm.tb_maxwidth.Text);
                    cam.PredSizeMinWidth = GetNumberInt(frm.tb_minwidth.Text);
                    cam.PredSizeMinHeight = GetNumberInt(frm.tb_minheight.Text);
                    cam.PredSizeMaxPercentOfImage = frm.tb_maxpercent.Text.ToDouble();
                    cam.PredSizeMinPercentOfImage = frm.tb_MinPercent.Text.ToDouble();
                    cam.MergePredictionsMinMatchPercent = frm.tb_duplicatepercent.Text.ToDouble();

                    Lbl_PredictionTolerances.Text = $"Threshold: {cam.threshold_lower}-{cam.threshold_upper}, Size: {cam.PredSizeMinPercentOfImage.ToPercent()}-{cam.PredSizeMaxPercentOfImage.ToPercent()} ; Width: {cam.PredSizeMinWidth}-{cam.PredSizeMaxWidth}, Height: {cam.PredSizeMinHeight}-{cam.PredSizeMaxHeight}, PredictionMatch: {cam.MergePredictionsMinMatchPercent.ToPercent()}";

                    AppSettings.SaveAsync(true);
                }
            }

        }

        private void tb_threshold_lower_TextChanged(object sender, EventArgs e)
        {

        }

        private void FOLV_Cameras_SelectionChanged(object sender, EventArgs e)
        {
            if (FOLV_Cameras.SelectedObjects.Count > 0)
                Global.SaveRegSetting("LastSelectedCamera", ((Camera)FOLV_Cameras.SelectedObjects[0]).Name);

            DisplayCameraSettings();
        }

        private void FOLV_Cameras_FormatRow(object sender, FormatRowEventArgs e)
        {
            this.FormatCameraRow(sender, e);
        }

        private void BtnRelevantObjects_Click(object sender, EventArgs e)
        {
            if (this.FOLV_Cameras.SelectedObjects.Count == 0)
            {
                MessageBox.Show("Select a camera first");
                return;
            }

            using (Frm_RelevantObjects frm = new Frm_RelevantObjects())
            {
                Camera cam = AITOOL.GetCamera(((Camera)this.FOLV_Cameras.SelectedObjects[0]).Name);
                frm.ROMName = $"{cam.Name}\\{cam.DefaultTriggeringObjects.TypeName}";
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    DisplayCameraSettings();
                }
            }
        }

        private void toolStripButtonAdjustAnno_Click(object sender, EventArgs e)
        {
            using (Frm_AnnoAdjust frm = new Frm_AnnoAdjust())
            {
                frm.ShowDialog();
            }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            using (Frm_Pause frm = new Frm_Pause())
            {

                //Camera cam = AITOOL.GetCamera(((Camera)this.FOLV_Cameras.SelectedObjects[0]).Name);
                //frm.CurrentCam = cam;
                frm.ShowDialog(this);
            }
        }

        private void bt_CheckUpdates_Click(object sender, EventArgs e)
        {
            using (Frm_UpdateCheck frm = new Frm_UpdateCheck())
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    CloseImmediately = true;
                    Application.Exit();

                }
            }
        }

        private void mergeDuplicatePredictionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AppSettings.Settings.HistoryMergeDuplicatePredictions = this.mergeDuplicatePredictionsToolStripMenuItem.Checked;
            AppSettings.SaveAsync(true);
        }

        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.ShowForm();
            }
            else
            {
                this.notifyIcon.ContextMenuStrip.Show();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (Frm_Pause frm = new Frm_Pause())
            {

                //Camera cam = AITOOL.GetCamera(((Camera)this.FOLV_Cameras.SelectedObjects[0]).Name);
                //frm.CurrentCam = cam;
                frm.ShowDialog(this);
            }
        }

        private void pauseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double pausetime = 0;
            foreach (var cam in AppSettings.Settings.CameraList)
            {
                pausetime = cam.PauseMinutes;
                if (!cam.Paused)
                    cam.Pause();
            }
            MessageBox.Show($"Paused all cameras for {pausetime} minutes");
        }

        private void resumeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var cam in AppSettings.Settings.CameraList)
            {
                if (cam.Paused)
                    cam.Resume();
            }

            MessageBox.Show("Resumed all cameras.");
        }

        private void Shell_Activated(object sender, EventArgs e)
        {
            //bring this form to the top of all other windows:
            this.TopMost = true;
            this.TopMost = false;
            this.Show();
            Log($"Trace: App Activated. TopMost={this.TopMost}, Visible={this.Visible}, state={this.WindowState}, tbicon={this.ShowInTaskbar}, trayicon={this.notifyIcon.Visible}");


        }
    }

    //enhanced TableLayoutPanel loads faster
    public partial class DBLayoutPanel:TableLayoutPanel
    {
        public DBLayoutPanel()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
              ControlStyles.OptimizedDoubleBuffer |
              ControlStyles.UserPaint, true);
        }

        public DBLayoutPanel(IContainer container)
        {
            container.Add(this);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
              ControlStyles.OptimizedDoubleBuffer |
              ControlStyles.UserPaint, true);
        }
    }
}


