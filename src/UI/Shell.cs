using BrightIdeasSoftware;
using EasyAsyncCancel;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json; //deserialize DeepquestAI response
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static AITool.AITOOL;

namespace AITool
{

    public partial class Shell : Form
    {
        private ThreadSafe.Datetime LastListUpdate = new ThreadSafe.Datetime(DateTime.MinValue);

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

        private bool AllowShowDisplay = false;

        public Shell()
        {
            this.StartupSW = Stopwatch.StartNew();

            this.InitializeComponent();

            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            //this is to log and status messages from other classes...
            Global.progress = new Progress<ClsMessage>(this.EventMessage);

            this.SplashScreen = new FrmSplash();
            this.SplashScreen.Show();

            HideForm();

            Application.DoEvents();
            Debug.Print("Init tid=" + Thread.CurrentThread.ManagedThreadId);

        }


        private async void Shell_Load(object sender, EventArgs e)
        {

            //ClsDoodsRequest cdr = new ClsDoodsRequest();

            //cdr.Detect.MinPercentMatch = 50;

            //string testjson = JsonConvert.SerializeObject(cdr);

            Debug.Print("load tid=" + Thread.CurrentThread.ManagedThreadId);

            using var cw = new Global_GUI.CursorWait(true);

            //---------------------------------------------------------------------------
            //INITIALIZE HISTORY DB, load settings, ETC

            Stopwatch sw = Stopwatch.StartNew();

            await AITOOL.InitializeBackend();

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

                //left list column setup
                this.list2.Columns.Add("Camera");

                //set left list column width segmentation (because of some bug -4 is necessary to achieve the correct width)
                this.list2.Columns[0].Width = this.list2.Width - 4;
                this.list2.FullRowSelect = true; //make all columns clickable

                this.LoadCameras(); //load camera list

                this.comboBox_filter_camera.SelectedIndex = this.comboBox_filter_camera.FindStringExact("All Cameras"); //select all cameras entry

                //---------------------------------------------------------------------------
                //SETTINGS TAB

                //fill settings tab with stored settings 

                this.cmbInput.Text = AppSettings.Settings.input_path;
                this.cb_inputpathsubfolders.Checked = AppSettings.Settings.input_path_includesubfolders;
                this.cmbInput.Items.Clear();
                foreach (string pth in BlueIrisInfo.ClipPaths)
                {
                    this.cmbInput.Items.Add(pth);

                }

                this.tbDeepstackUrl.Text = AppSettings.Settings.deepstack_url;
                this.cb_DeepStackURLsQueued.Checked = AppSettings.Settings.deepstack_urls_are_queued;

                Debug.Print("load tid=" + Thread.CurrentThread.ManagedThreadId);

                this.Chk_AutoScroll.Checked = AppSettings.Settings.Autoscroll_log;

                this.tb_telegram_chatid.Text = String.Join(",", AppSettings.Settings.telegram_chatids);
                this.tb_telegram_token.Text = AppSettings.Settings.telegram_token;
                this.tb_telegram_cooldown.Text = AppSettings.Settings.telegram_cooldown_minutes.ToString();

                this.cb_send_errors.Checked = AppSettings.Settings.send_errors;
                this.cbStartWithWindows.Checked = AppSettings.Settings.startwithwindows;
                this.minimizeToTrayCheckbox.Checked = AppSettings.Settings.minimizetotray;

                this.tb_username.Text = AppSettings.Settings.DefaultUserName.Trim();
                this.tb_password.Text = Global.DecryptString(AppSettings.Settings.DefaultPasswordEncrypted);

                this.tb_BlueIrisServer.Text = AppSettings.Settings.BlueIrisServer.Trim();

                //---------------------------------------------------------------------------
                //STATS TAB
                this.comboBox1.Items.Add("All Cameras"); //add all cameras stats entry
                this.comboBox1.SelectedIndex = this.comboBox1.FindStringExact("All Cameras"); //select all cameras entry


                //---------------------------------------------------------------------------
                //Deepstack server TAB


                if (!DeepStackServerControl.IsInstalled)
                {
                    //remove deepstack tab if not installed
                    Log("Removing DeepStack tab since it not installed as a Windows app (No docker support yet)");
                    this.tabControl1.TabPages.Remove(this.tabControl1.TabPages["tabDeepStack"]);
                }
                else
                {
                    if (DeepStackServerControl.NeedsSaving)
                    {
                        //this may happen if the already running instance has a different port, etc, so we update the config
                        this.SaveDeepStackTab();
                    }
                    this.LoadDeepStackTab(true);
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

                this.ToolStripComboBoxSearch.Text = Global.GetSetting("SearchText", "");
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


                //---------------------------------------------------------------------------
                // finish up

                string AssemVer = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                this.lbl_version.Text = $"Version {AssemVer} built on {Global.RetrieveLinkerTimestamp()}";

                //---------------------------------------------------------------------------------------------------------

                IsLoading.WriteFullFence(false);

                Resize += new System.EventHandler(this.Form1_Resize); //resize event to enable 'minimize to tray'

                Log($"{{yellow}}APP START complete.  Initialized in {this.StartupSW.Elapsed.TotalSeconds.ToString("##0.0")} seconds ({this.StartupSW.ElapsedMilliseconds}ms)");

                this.SplashScreen.Close();
                this.SplashScreen = null;

                Global_GUI.RestoreWindowState(this);

                if (Environment.CommandLine.IndexOf("/min", StringComparison.OrdinalIgnoreCase) == -1)
                {
                    ShowForm();
                }

                Application.DoEvents();

            });

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

            if (IsLoading.ReadFullFence())
                return;

            if (!this.IsLogListUpdating.ReadFullFence() && Force ||
                !this.IsLogListUpdating.ReadFullFence() &&
                (this.tabControl1.SelectedTab == this.tabControl1.TabPages["tabLog"]) &&
                this.Visible &&
                !(this.WindowState == FormWindowState.Minimized) &&
                LogMan != null)
            {
                //run in another thread so gui doesnt freeze
                //await Task.Run(() =>
                //{
                Stopwatch sw = Stopwatch.StartNew();

                this.IsLogListUpdating.WriteFullFence(true);

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

                this.IsLogListUpdating.WriteFullFence(false);

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
                !this.IsHistoryListUpdating.ReadFullFence() &&
                this.tabControl1.SelectedIndex == 2 &&
                this.Visible &&
                !(this.WindowState == FormWindowState.Minimized) &&
                (DateTime.Now - this.LastListUpdate.Read()).TotalMilliseconds >= AppSettings.Settings.TimeBetweenListRefreshsMS &&
                this.DatabaseInitialized.ReadFullFence() &&
                !IsLoading.ReadFullFence() &&
                await HistoryDB.HasUpdates())
            {
                // run in another thread so gui doesnt freeze
                await Task.Run(() =>
                {

                    this.IsHistoryListUpdating.WriteFullFence(true);

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

                        this.LastListUpdate.Write(DateTime.Now);

                        this.IsHistoryListUpdating.WriteFullFence(false);

                        //UpdateToolstrip("");

                    });

                });

            }
            else
            {
                //Log($"Debug: List not updated - Refresh={AppSettings.Settings.HistoryAutoRefresh}, Visible={tabControl1.SelectedIndex == 2 && this.Visible && !(this.WindowState == FormWindowState.Minimized)}, IsListUpdating={this.IsListUpdating.ReadFullFence()}, LastListUpdateMS={(DateTime.Now - this.LastListUpdate.Read()).TotalMilliseconds}");
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
            if (IsClosing.ReadFullFence())
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

                this.DatabaseInitialized.WriteFullFence(true);
                await this.LoadHistoryAsync(true, true);
                this.HistoryStartStop();

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

                        Log($"Error: Could not find label '{lblcontrolname}': {Global.ExMsg(ex)}");
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

            if (IsClosing.ReadFullFence())
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

        //    if (IsClosing.ReadFullFence())
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

            if (IsLoading.ReadFullFence())
                return;

            if (IsClosing.ReadFullFence())
                return;

            if (this.WindowState == FormWindowState.Minimized)
            {
                if(AppSettings.Settings.minimizetotray){
                    HideForm();
                };
            }
            else
            {
                this.ResizeListViews();
                //Log($"Debug: Form_Resize. Visible={this.Visible}, state={this.WindowState}, tbicon={this.ShowInTaskbar}, tryicon={this.notifyIcon.Visible}");
            }
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

            if (IsClosing.ReadFullFence())
                return;

            try
            {
                //this.AllowShowDisplay = true;
                this.Opacity = 100;
                this.Show();
                this.WindowState = FormWindowState.Normal;
                this.ShowInTaskbar = true;
                this.notifyIcon.Visible = false;
                this.LoadHistoryAsync(true, true);
                this.UpdateLogAddedRemovedAsync(true, true);
                //Log($"Debug: Showing app. Visible={this.Visible}, state={this.WindowState}, tbicon={this.ShowInTaskbar}, trayicon={this.notifyIcon.Visible}");
                this.Activate();
                Application.DoEvents();

            }
            catch { }
        }

        private async void HideForm()
        {

            if (IsClosing.ReadFullFence())
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

        //adapt list views (history tab and cameras tab) to window size while considering scrollbar influence
        private void ResizeListViews()
        {
            //suspend layout of most complex tablelayout elements (gives a few milliseconds)
            this.tableLayoutPanel7.SuspendLayout();
            this.tableLayoutPanel8.SuspendLayout();
            //tableLayoutPanel9.SuspendLayout();


            if (this.list2.Columns.Count > 0)
                this.list2.Columns[0].Width = this.list2.Width - 4; //resize camera list column

            //resume layout again
            this.tableLayoutPanel7.ResumeLayout();
            this.tableLayoutPanel8.ResumeLayout();
            //tableLayoutPanel9.ResumeLayout();
        }


        //EVENTS:

        //event: mouse click on tab control
        private void tabControl1_MouseDown(object sender, MouseEventArgs e)
        {
            this.ResizeListViews();
        }

        //event: another tab selected (Only load certain things in tabs if they are actually open)
        private async void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Application.DoEvents();

            if (this.tabControl1.SelectedIndex == 1)
            {
                this.UpdatePieChart(); this.UpdateTimeline(); this.UpdateConfidenceChart();
            }
            else if (this.tabControl1.SelectedIndex == 2)
            {
                await this.LoadHistoryAsync(true, true);
            }
            else if (this.tabControl1.SelectedTab == this.tabControl1.TabPages["tabDeepStack"])
            {
                this.LoadDeepStackTab(true);
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
                    if (minute > 30) //if alert occurred after half o clock
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

                Log("Error: " + Global.ExMsg(ex));
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

                            if (x_value > 0)
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
                            int x_value = Global.GetNumberInt(detection);  // gets a number anywhere in the string
                            if (x_value > 0)
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

                Log("Error: " + Global.ExMsg(ex));
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
        private void showObject(PaintEventArgs e, int _xmin, int _ymin, int _xmax, int _ymax, string text, ResultType result)
        {
            try
            {
                if (this.folv_history.SelectedObjects != null && this.folv_history.SelectedObjects.Count > 0 && (this.pictureBox1 != null) && (this.pictureBox1.BackgroundImage != null))
                {

                    System.Drawing.Color color = new System.Drawing.Color();
                    int BorderWidth = AppSettings.Settings.RectBorderWidth
;

                    if (result == ResultType.Relevant)
                    {
                        color = System.Drawing.Color.FromArgb(AppSettings.Settings.RectRelevantColorAlpha, AppSettings.Settings.RectRelevantColor);
                    }
                    else if (result == ResultType.DynamicMasked || result == ResultType.ImageMasked || result == ResultType.StaticMasked)
                    {
                        color = System.Drawing.Color.FromArgb(AppSettings.Settings.RectMaskedColorAlpha, AppSettings.Settings.RectMaskedColor);
                    }
                    else
                    {
                        color = System.Drawing.Color.FromArgb(AppSettings.Settings.RectIrrelevantColorAlpha, AppSettings.Settings.RectIrrelevantColor);
                    }

                    //1. get the padding between the image and the picturebox border

                    //get dimensions of the image and the picturebox
                    float imgWidth = this.pictureBox1.BackgroundImage.Width;
                    float imgHeight = this.pictureBox1.BackgroundImage.Height;
                    float boxWidth = this.pictureBox1.Width;
                    float boxHeight = this.pictureBox1.Height;

                    //these variables store the padding between image border and picturebox border
                    int absX = 0;
                    int absY = 0;

                    //because the sizemode of the picturebox is set to 'zoom', the image is scaled down
                    float scale = 1;


                    //Comparing the aspect ratio of both the control and the image itself.
                    if (imgWidth / imgHeight > boxWidth / boxHeight) //if the image is p.e. 16:9 and the picturebox is 4:3
                    {
                        scale = boxWidth / imgWidth; //get scale factor
                        absY = (int)(boxHeight - scale * imgHeight) / 2; //padding on top and below the image
                    }
                    else //if the image is p.e. 4:3 and the picturebox is widescreen 16:9
                    {
                        scale = boxHeight / imgHeight; //get scale factor
                        absX = (int)(boxWidth - scale * imgWidth) / 2; //padding left and right of the image
                    }

                    //2. inputted position values are for the original image size. As the image is probably smaller in the picturebox, the positions must be adapted. 
                    int xmin = (int)(scale * _xmin) + absX;
                    int xmax = (int)(scale * _xmax) + absX;
                    int ymin = (int)(scale * _ymin) + absY;
                    int ymax = (int)(scale * _ymax) + absY;


                    //3. paint rectangle
                    System.Drawing.Rectangle rect = new System.Drawing.Rectangle(xmin, ymin, xmax - xmin, ymax - ymin);
                    using (Pen pen = new Pen(color, BorderWidth))
                    {
                        e.Graphics.DrawRectangle(pen, rect); //draw rectangle
                    }

                    //we need this since people can change the border width in the json file
                    int halfbrd = BorderWidth / 2;

                    //object name text below rectangle
                    rect = new System.Drawing.Rectangle(xmin - halfbrd, ymax + halfbrd, (int)boxWidth, (int)boxHeight); //sets bounding box for drawn text

                    Brush brush = new SolidBrush(color); //sets background rectangle color

                    System.Drawing.SizeF size = e.Graphics.MeasureString(text, new Font(AppSettings.Settings.RectDetectionTextFont, AppSettings.Settings.RectDetectionTextSize)); //finds size of text to draw the background rectangle
                    e.Graphics.FillRectangle(brush, xmin - halfbrd, ymax + halfbrd, size.Width, size.Height); //draw grey background rectangle for detection text
                    e.Graphics.DrawString(text, new Font(AppSettings.Settings.RectDetectionTextFont, AppSettings.Settings.RectDetectionTextSize), Brushes.Black, rect); //draw detection text



                }

            }
            catch (Exception ex)
            {

                Log("Error: " + Global.ExMsg(ex));
            }
        }

        //load object rectangle overlays
        //TODO: refactor detections
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (AppSettings.Settings.HistoryShowObjects && this.folv_history.SelectedObjects != null && this.folv_history.SelectedObjects.Count > 0) //if checkbox button is enabled
            {
                //Log("Loading object rectangles...");
                History hist = (History)this.folv_history.SelectedObjects[0];

                if (hist == null)
                    return;

                string positions = hist.Positions;
                string detections = hist.Detections;

                int XOffset = 0;
                int YOffset = 0;

                Camera cam = AITOOL.GetCamera(hist.Camera);
                if (cam != null)
                {
                    //apply offset if one is defined by user in json file
                    XOffset = cam.XOffset;
                    YOffset = cam.YOffset;
                }

                try
                {

                    if (!string.IsNullOrEmpty(hist.PredictionsJSON))
                    {
                        List<ClsPrediction> predictions = new List<ClsPrediction>();

                        predictions = Global.SetJSONString<List<ClsPrediction>>(hist.PredictionsJSON);

                        foreach (var pred in predictions)
                        {
                            if (AppSettings.Settings.HistoryOnlyDisplayRelevantObjects && pred.Result == ResultType.Relevant)
                            {
                                this.showObject(e, pred.XMin + XOffset, pred.YMin + YOffset, pred.XMax, pred.YMax, pred.ToString(), pred.Result); //call rectangle drawing method, calls appropriate detection text
                            }
                            else if (!AppSettings.Settings.HistoryOnlyDisplayRelevantObjects)
                            {
                                this.showObject(e, pred.XMin + XOffset, pred.YMin + YOffset, pred.XMax, pred.YMax, pred.ToString(), pred.Result); //call rectangle drawing method, calls appropriate detection text
                            }

                        }

                    }
                    else
                    {
                        List<string> positionssArray = Global.Split(positions, ";");//creates array of detected objects, used for adding text overlay

                        int countr = positionssArray.Count();

                        ResultType result = ResultType.Unknown;

                        if (detections.ToLower().Contains("irrelevant") || detections.ToLower().Contains("masked") || detections.ToLower().Contains("confidence"))
                        {
                            detections = detections.Split(':')[1]; //removes the "1x masked, 3x irrelevant:" before the actual detection, otherwise this would be displayed in the detection tags
                            if (detections.Contains("masked"))
                            {
                                result = ResultType.ImageMasked;
                            }
                            else
                            {
                                result = ResultType.Unknown;
                            }
                        }
                        else
                        {
                            result = ResultType.Relevant;
                        }

                        List<string> detectionsArray = Global.Split(detections, ";");//creates array of detected objects, used for adding text overlay

                        if (cam != null)
                        {
                            //apply offset if one is defined by user in json file
                            XOffset = cam.XOffset;
                            YOffset = cam.YOffset;
                        }

                        //display a rectangle around each relevant object
                        for (int i = 0; i < countr; i++)
                        {

                            //load 'xmin,ymin,xmax,ymax' from third column into a string
                            List<string> positionsplt = Global.Split(positionssArray[i], ",");

                            //store xmin, ymin, xmax, ymax in separate variables
                            Int32.TryParse(positionsplt[0], out int xmin);
                            Int32.TryParse(positionsplt[1], out int ymin);
                            Int32.TryParse(positionsplt[2], out int xmax);
                            Int32.TryParse(positionsplt[3], out int ymax);


                            this.showObject(e, xmin + XOffset, ymin + YOffset, xmax, ymax, detectionsArray[i], result); //call rectangle drawing method, calls appropriate detection text

                        }

                    }


                }
                catch (Exception ex)
                {

                    Log($"Error: Positions (subitem4) ='{positions}', Detections (subitem3) ='{detections}': {Global.ExMsg(ex)}");
                }

            }
        }

        // add new entry in left list


        private void UpdateStats(string Message = "")
        {
            try
            {


                if (!this.Visible || (this.WindowState == FormWindowState.Minimized) || this.IsStatsUpdating.ReadFullFence() || IsClosing.ReadFullFence())
                    return;  //save a tree

                this.IsStatsUpdating.WriteFullFence(true);

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
                        items = HistoryDB.HistoryDic.Count();
                        removed = HistoryDB.DeletedCount.ReadFullFence();
                    }

                    if (HistoryDB != null && HistoryDB.AddedCount.ReadFullFence() > 0)
                        hpm = HistoryDB.AddedCount.ReadFullFence() / (DateTime.Now - HistoryDB.InitializeTime).TotalMinutes;

                    this.toolStripStatusLabelHistoryItems.Text = $"{items} history items ({hpm.ToString("###0")}/MIN) | {removed} removed |";
                    int TriggerActionQueueCount = 0;
                    if (TriggerActionQueue != null)
                        TriggerActionQueueCount = TriggerActionQueue.Count.ReadFullFence();

                    this.toolStripStatusLabel1.Text = $"| {alerts} Alerts | {irrelevantalerts} Irrelevant | {falsealerts} False | {skipped} Skipped ({newskipped} new) | {qcalc.Count} ImgProcessed ({qcalc.ItemsPerMinute().ToString("###0")}/MIN) | {ImageProcessQueue.Count} ImgQueued (Max={qsizecalc.Max},Avg={Math.Round(qsizecalc.Average, 0)}) | {TriggerActionQueueCount} Actions Queued";

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


                    if (LogMan.ErrorCount.ReadFullFence() > 0)
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
                    this.lblQueue.Text = $"Images in queue: {ImageProcessQueue.Count}, Max: {qsizecalc.Max} ({qcalc.Max}ms), Average: {qsizecalc.Average.ToString("#####")} ({qcalc.Average.ToString("#####")}ms queue wait time)";

                });
                Global_GUI.InvokeIFRequired(this.lbl_errors, () =>
                {
                    if (LogMan.ErrorCount.ReadFullFence() > 0)
                        this.lbl_errors.Text = $"{LogMan.ErrorCount} error(s) occurred. Click to open Log."; //update error counter label
                    else
                        this.lbl_errors.Text = "";

                });



            }
            catch (Exception ex)
            {

                Log("Error: " + Global.ExMsg(ex));
            }
            finally
            {
                this.IsStatsUpdating.WriteFullFence(false);
            }
        }
        //load stored entries in history CSV into history ListView
        private async Task LoadHistoryAsync(bool FilterChanged, bool Follow)
        {

            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.


            //if (IsLoading.ReadFullFence())  //when you set checkboxes during init, it may trigger the event to load the history
            //{
            //    //Log("---Exit (still loading)");
            //    return;
            //}

            //make sure only one thread updating at a time
            //await Semaphore_List_Updating.WaitAsync();

            if (this.IsHistoryListUpdating.ReadFullFence())
            {
                Log("Trace: ---Exit (already updating)");
                return;
            }

            Stopwatch semsw = Stopwatch.StartNew();

            this.IsHistoryListUpdating.WriteFullFence(true);
            this.LastListUpdate.Write(DateTime.Now);

            Global_GUI.CursorWait cw = null;

            try
            {
                if (semsw.ElapsedMilliseconds > 50)
                    Log($"debug: Waited {semsw.ElapsedMilliseconds}ms while waiting for other threads to finish.");

                //wait a bit for the list to be available
                Stopwatch sw = Stopwatch.StartNew();
                bool displayed = false;
                do
                {
                    if (HistoryDB != null && HistoryDB.HistoryDic != null && this.folv_history != null && this.DatabaseInitialized.ReadFullFence())
                        break;
                    else if (!displayed)
                    {

                        Log("debug: Waiting for database to finish initializing...");
                        displayed = true;
                    }
                    await Task.Delay(100);

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
                Log("Error: " + Global.ExMsg(ex));
            }
            finally
            {
                if (cw != null)
                    cw.Dispose();
                this.LastListUpdate.Write(DateTime.Now);
                this.IsHistoryListUpdating.WriteFullFence(false);
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
            Global_GUI.InvokeIFRequired(this.label2, () => { this.label2.Text = $"Running"; });

            //only update charts if stats tab is open

            if (this.tabControl1.SelectedIndex == 1)
            {
                Global_GUI.InvokeIFRequired(this, () => { this.UpdatePieChart(); this.UpdateTimeline(); this.UpdateConfidenceChart(); });
            }

            //load updated cameara stats info in camera tab if a camera is selected
            Global_GUI.InvokeIFRequired(this, () =>
            {
                if (this.list2.SelectedItems.Count > 0)
                {
                    //load only stats from Camera.cs object

                    //all camera objects are stored in the list CameraList, so firstly the position (stored in the second column for each entry) is gathered
                    Camera cam = AITOOL.GetCamera(this.list2.SelectedItems[0].Text);

                    //load cameras stats
                    string stats = $"Alerts: {cam.stats_alerts.ToString()} | Irrelevant Alerts: {cam.stats_irrelevant_alerts.ToString()} | False Alerts: {cam.stats_false_alerts.ToString()}";
                    if (cam.maskManager.MaskingEnabled)
                    {
                        stats += $" | Mask History Count: {cam.maskManager.LastPositionsHistory.Count()} | Current Dynamic Masks: {cam.maskManager.MaskedPositions.Count()}";
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

                //start by getting last selected camera if any
                string oldnamecameras = "";
                if (this.list2.SelectedItems != null && this.list2.SelectedItems.Count > 0)
                    oldnamecameras = this.list2.SelectedItems[0].Text;

                //if nothing was selected, then select the last saved camera:
                if (string.IsNullOrEmpty(oldnamecameras))
                    oldnamecameras = Global.GetSetting("LastSelectedCamera", "");

                string oldnamefilters = "";
                if (this.comboBox_filter_camera.Items.Count > 0)
                    oldnamefilters = this.comboBox_filter_camera.Text;

                string oldnamestats = "";
                if (this.comboBox1.Items.Count > 0)
                    oldnamestats = this.comboBox1.Text;

                this.list2.Items.Clear();
                this.comboBox1.Items.Clear();
                this.comboBox1.Items.Add("All Cameras");
                this.comboBox_filter_camera.Items.Clear();
                this.comboBox_filter_camera.Items.Add("All Cameras");

                int i = 0;
                int oldidxcameras = 0;
                int oldidxfilters = 0;
                int oldidxstats = 0;
                foreach (Camera cam in AppSettings.Settings.CameraList)
                {
                    //Add loaded camera to list2
                    ListViewItem item = new ListViewItem(new string[] { cam.name });
                    if (!cam.enabled)
                    {
                        item.ForeColor = System.Drawing.Color.Gray;
                    }
                    //item.Tag = file; //tag is not used anywhere I can see
                    this.list2.Items.Add(item);
                    //add camera to combobox on overview tab and to camera filter combobox in the History tab 
                    this.comboBox1.Items.Add($"   {cam.name}");
                    this.comboBox_filter_camera.Items.Add($"   {cam.name}");
                    if (string.Equals(oldnamecameras.Trim(), cam.name.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        oldidxcameras = i;
                    }
                    if (string.Equals(oldnamefilters.Trim(), cam.name.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        oldidxfilters = i + 1;
                    }
                    if (string.Equals(oldnamestats.Trim(), cam.name.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        oldidxstats = i + 1;
                    }
                    i++;

                }

                //select first camera, or last selected camera
                if (this.list2.Items.Count > 0 && this.list2.Items.Count >= oldidxcameras)
                {
                    this.list2.Items[oldidxcameras].Selected = true;
                }

                if (this.comboBox_filter_camera.Items.Count > 0)
                    this.comboBox_filter_camera.SelectedIndex = oldidxfilters;

                if (this.comboBox1.Items.Count > 0)
                    this.comboBox1.SelectedIndex = oldidxstats;


            }
            catch
            {
                Log("ERROR LoadCameras() failed.");
                MessageBox.Show("ERROR LoadCameras() failed.");
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
                if (string.Equals(c.name.Trim(), cam.name.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show($"ERROR: Camera name must be unique,{cam.name} already exists.");
                    return ($"ERROR: Camera name must be unique,{cam.name} already exists.");
                }
            }

            //check if name is empty
            if (cam.name == "")
            {
                MessageBox.Show($"ERROR: Camera name may not be empty.");
                return ($"ERROR: Camera name may not be empty.");
            }


            if (BlueIrisInfo.Result == BlueIrisResult.Valid)
            {
                //http://10.0.1.99:81/admin?trigger&camera=BACKFOSCAM&user=AITools&pw=haha&memo=[summary]
                cam.trigger_urls_as_string = $"[BlueIrisURL]/admin?trigger&camera=[camera]&user=[Username]&pw=[Password]&flagalert=1&memo=[summary]&jpeg=[ImagePathEscaped]";
            }

            //I dont think this is used anywhere
            cam.triggering_objects = Global.Split(cam.triggering_objects_as_string, ",").ToArray();   //triggering_objects_as_string.Split(','); //split the row of triggering objects between every ','

            //Split by cr/lf or other common delimiters
            cam.trigger_urls = Global.Split(cam.trigger_urls_as_string, "\r\n|;,").ToArray();  //all trigger urls in an array
            cam.cancel_urls = Global.Split(cam.cancel_urls_as_string, "\r\n|;,").ToArray();  //all trigger urls in an array

            AppSettings.Settings.CameraList.Add(cam); //add created camera object to CameraList

            this.LoadCameras();

            return ($"SUCCESS: {cam.name} created.");
        }



        //remove camera
        private void RemoveCamera(string name)
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            Log($"Removing camera {name}...");
            if (this.list2.Items.Count > 0) //if list is empty, nothing can be deleted
            {
                if (AppSettings.Settings.CameraList.Exists(x => string.Equals(x.name, name, StringComparison.OrdinalIgnoreCase))) //check if camera with specified name exists in list
                {

                    //find index of specified camera in list
                    int index = -1;

                    //check for each camera in the cameralist if its name equals the name of the camera that is selected to be deleted
                    for (int i = 0; i < AppSettings.Settings.CameraList.Count; i++)
                    {
                        if (AppSettings.Settings.CameraList[i].name.Equals(name, StringComparison.OrdinalIgnoreCase))
                        {
                            index = i;

                        }
                    }

                    if (index != -1) //only delete camera if index is known (!= its default value -1)
                    {
                        //AppSettings.Settings.CameraList[index].Delete(); //delete settings file of specified camera

                        //move all cameras following the specified camera one position forward in the list
                        //the position of the specified camera is overridden with the following camera, the position of the following camera is overridden with its follower, and so on
                        for (int i = index; i < AppSettings.Settings.CameraList.Count - 1; i++)
                        {
                            AppSettings.Settings.CameraList[i] = AppSettings.Settings.CameraList[i + 1];
                        }

                        AppSettings.Settings.CameraList.Remove(AppSettings.Settings.CameraList[AppSettings.Settings.CameraList.Count - 1]); //lastly, remove camera from list

                        //remove list2 entry
                        var item = this.list2.FindItemWithText(name);
                        this.list2.Items[this.list2.Items.IndexOf(item)].Remove();

                        //remove camera from combobox on overview tab and from camera filter combobox in the History tab 
                        this.comboBox1.Items.Remove($"   {name}");
                        this.comboBox_filter_camera.Items.Remove($"   {name}");

                        //select first camera
                        if (this.list2.Items.Count > 0)
                        {
                            this.list2.Items[0].Selected = true;
                        }

                        //if list2 is empty, clear settings fields (to prevent that values of a deleted camera are shown)
                        if (this.list2.Items.Count == 0)
                        {
                            this.tbName.Text = "";
                            this.tbPrefix.Text = "";
                            this.cb_enabled.Checked = false;
                            CheckBox[] cbarray = new CheckBox[] { this.cb_airplane, this.cb_bear, this.cb_bicycle, this.cb_bird, this.cb_boat, this.cb_bus, this.cb_car, this.cb_cat, this.cb_cow, this.cb_dog, this.cb_horse, this.cb_motorcycle, this.cb_person, this.cb_sheep, this.cb_truck };
                            foreach (CheckBox c in cbarray)
                            {
                                c.Checked = false;
                            }
                            //tbTriggerUrl.Text = "";
                            //cb_telegram.Checked = false;
                            //disable camera settings if there are no cameras setup yet
                            //tableLayoutPanel6.Enabled = false;  //this stops us from being able to add a new camera
                        }
                    }
                    else
                    {
                        Log("ERROR: Can't find the selected camera, camera wasn't deleted.");
                    }


                }
            }
        }

        //display camera settings for selected camera
        private void DisplayCameraSettings()
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            if (this.list2.SelectedItems.Count > 0)
            {

                this.tableLayoutPanel6.Enabled = true;

                this.tbName.Text = this.list2.SelectedItems[0].Text; //load name textbox from name in list2


                //load remaining settings from Camera.cs object

                //all camera objects are stored in the list CameraList, so firstly the position (stored in the second column for each entry) is gathered
                Camera cam = AITOOL.GetCamera(this.tbName.Text);   //int i = AppSettings.Settings.CameraList.FindIndex(x => x.name.Trim().ToLower() == list2.SelectedItems[0].Text.Trim().ToLower());

                //load cameras stats

                string stats = $"Alerts: {cam.stats_alerts.ToString()} | Irrelevant Alerts: {cam.stats_irrelevant_alerts.ToString()} | False Alerts: {cam.stats_false_alerts.ToString()}";

                if (cam.maskManager.MaskingEnabled)
                {
                    stats += $" | Mask History Count: {cam.maskManager.LastPositionsHistory.Count()} | Current Dynamic Masks: {cam.maskManager.MaskedPositions.Count()}";
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
                this.tbPrefix.Text = cam.prefix; //load 'input file begins with'
                this.lbl_prefix.Text = this.tbPrefix.Text + ".××××××.jpg"; //prefix live preview

                this.cmbcaminput.Text = cam.input_path;
                this.cmbcaminput.Items.Clear();
                foreach (string pth in BlueIrisInfo.ClipPaths)
                {
                    this.cmbcaminput.Items.Add(pth);
                    //try to automatically pick the path that starts with AI if not already set
                    //if ((pth.ToLower().Contains(tbName.Text.ToLower()) || pth.ToLower().Contains(tbPrefix.Text.ToLower())) && string.IsNullOrWhiteSpace(cmbcaminput.Text))
                    //{
                    //    cmbcaminput.Text = pth;
                    //}
                }
                this.cb_monitorCamInputfolder.Checked = cam.input_path_includesubfolders;

                this.tb_threshold_lower.Text = cam.threshold_lower.ToString(); //load lower threshold value
                this.tb_threshold_upper.Text = cam.threshold_upper.ToString(); // load upper threshold value

                //load is masking enabled 
                this.cb_masking_enabled.Checked = cam.maskManager.MaskingEnabled;



                //load triggering objects
                //first create arrays with all checkboxes stored in
                CheckBox[] cbarray = new CheckBox[] { this.cb_airplane, this.cb_bear, this.cb_bicycle, this.cb_bird, this.cb_boat, this.cb_bus, this.cb_car, this.cb_cat, this.cb_cow, this.cb_dog, this.cb_horse, this.cb_motorcycle, this.cb_person, this.cb_sheep, this.cb_truck };
                //create array with strings of the triggering_objects related to the checkboxes in the same order
                string[] cbstringarray = new string[] { "Airplane", "Bear", "Bicycle", "Bird", "Boat", "Bus", "Car", "Cat", "Cow", "Dog", "Horse", "Motorcycle", "Person", "Sheep", "Truck" };

                //clear all checkmarks
                foreach (CheckBox cb in cbarray)
                {
                    cb.Checked = false;
                }

                //check for every triggering_object string if it is active in the settings file. If yes, check according checkbox
                for (int j = 0; j < cbarray.Length; j++)
                {
                    if (cam.triggering_objects_as_string.IndexOf(cbstringarray[j], StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        cbarray[j].Checked = true;
                    }
                }

                this.tbAdditionalRelevantObjects.Text = cam.additional_triggering_objects_as_string;


            }
        }



        // SPECIAL METHODS

        //input file begins with live preview
        private void tbPrefix_TextChanged(object sender, EventArgs e)
        {
            this.lbl_prefix.Text = this.tbPrefix.Text + ".××××××.jpg";
        }



        //event: camera list another item selected
        private void list2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.list2.SelectedItems.Count > 0)
            {
                Global.SaveSetting("LastSelectedCamera", this.list2.SelectedItems[0].Text);
            }

            this.DisplayCameraSettings(); //display new item's settings
        }

        //event: camera add button
        private void btnCameraAdd_Click(object sender, EventArgs e)
        {

            using (Frm_CameraAdd frm = new Frm_CameraAdd())
            {

                //add only cameras not already installed
                foreach (string camstr in BlueIrisInfo.Cameras)
                {
                    if (GetCamera(camstr, false) == null)
                        frm.checkedListBoxCameras.Items.Add(camstr, false);

                }

                if (frm.ShowDialog() == DialogResult.OK)
                {
                    int added = 0;
                    List<string> cams = Global.Split(frm.tb_Cameras.Text, "\r\n|;,");
                    foreach (var cs in cams)
                    {
                        if (GetCamera(cs, false) == null)
                        {
                            Camera cam = new Camera(cs);
                            string camresult = this.AddCamera(cam);
                            Log(camresult);
                            if (camresult.StartsWith("success", StringComparison.OrdinalIgnoreCase))
                            {
                                added++;
                            }

                        }
                        else
                        {
                            Log($"Error: Camera already existed {cs}.");
                        }

                    }

                    MessageBox.Show($"Added {added} out of {cams.Count()}.  See log for details.");

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

            if (this.list2.Items.Count > 0)
            {
                //check if name is empty
                if (String.IsNullOrWhiteSpace(this.tbName.Text))
                {
                    this.DisplayCameraSettings(); //reset displayed settings
                    MessageBox.Show($"WARNING: Camera name may not be empty.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!string.Equals(this.list2.SelectedItems[0].Text.Trim(), this.tbName.Text.Trim(), StringComparison.OrdinalIgnoreCase))
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
                    else
                    {
                        Log($"SUCCESS: Camera {this.list2.SelectedItems[0].Text} was updated to {this.tbName.Text}.");
                    }
                }


                Camera cam = AITOOL.GetCamera(this.list2.SelectedItems[0].Text, false);

                if (cam == null)
                {
                    //should not happen, but...
                    MessageBox.Show($"WARNING: Camera not found???  '{this.list2.SelectedItems[0].Text}'", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                CheckBox[] cbarray = new CheckBox[] { this.cb_airplane, this.cb_bear, this.cb_bicycle, this.cb_bird, this.cb_boat, this.cb_bus, this.cb_car, this.cb_cat, this.cb_cow, this.cb_dog, this.cb_horse, this.cb_motorcycle, this.cb_person, this.cb_sheep, this.cb_truck };
                //create array with strings of the triggering_objects related to the checkboxes in the same order
                string[] cbstringarray = new string[] { "Airplane", "Bear", "Bicycle", "Bird", "Boat", "Bus", "Car", "Cat", "Cow", "Dog", "Horse", "Motorcycle", "Person", "Sheep", "Truck" };

                //go through all checkboxes and write all triggering_objects in one string
                cam.triggering_objects_as_string = "";
                for (int i = 0; i < cbarray.Length; i++)
                {
                    if (cbarray[i].Checked == true)
                    {
                        cam.triggering_objects_as_string += $"{cbstringarray[i].Trim()}, ";
                    }
                }

                //get lower and upper threshold values from textboxes
                cam.threshold_lower = Convert.ToInt32(this.tb_threshold_lower.Text.Trim());
                cam.threshold_upper = Convert.ToInt32(this.tb_threshold_upper.Text.Trim());

                cam.triggering_objects = Global.Split(cam.triggering_objects_as_string, ",").ToArray();   //triggering_objects_as_string.Split(','); //split the row of triggering objects between every ','

                cam.additional_triggering_objects_as_string = this.tbAdditionalRelevantObjects.Text.Trim();

                cam.trigger_urls = Global.Split(cam.trigger_urls_as_string, "\r\n|;,").ToArray();  //all trigger urls in an array
                cam.cancel_urls = Global.Split(cam.cancel_urls_as_string, "\r\n|;,").ToArray();

                if (cam.name != this.tbName.Text.Trim())
                {
                    string Oldmaskfile = GetMaskFile(cam.name);
                    if (!string.IsNullOrEmpty(Oldmaskfile) && File.Exists(Oldmaskfile))
                    {
                        string ext = Path.GetExtension(Oldmaskfile);
                        string pth = Path.GetDirectoryName(Oldmaskfile);
                        string NewMaskFile = Path.Combine(pth, this.tbName.Text.Trim() + ext);
                        File.Move(Oldmaskfile, NewMaskFile);
                    }
                    cam.name = this.tbName.Text.Trim();  //just in case we needed to rename it
                }

                cam.prefix = this.tbPrefix.Text.Trim();
                cam.enabled = this.cb_enabled.Checked;
                cam.maskManager.MaskingEnabled = this.cb_masking_enabled.Checked;
                cam.input_path = this.cmbcaminput.Text.Trim();
                cam.input_path_includesubfolders = this.cb_monitorCamInputfolder.Checked;

                int ccnt = 0;

                if (SaveTo)
                {
                    using (Frm_ApplyCameraTo frm = new Frm_ApplyCameraTo())
                    {
                        foreach (Camera ccam in AppSettings.Settings.CameraList)
                        {
                            if (ccam.name != cam.name)
                                frm.checkedListBoxCameras.Items.Add(ccam.name, false);
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

                                        Log($"Updating camera '{cam.name}' with settings from '{icam.name}'...");

                                        if (frm.cb_apply_confidence_limits.Checked)
                                        {
                                            icam.threshold_lower = cam.threshold_lower;
                                            icam.threshold_upper = cam.threshold_upper;
                                        }
                                        if (frm.cb_apply_objects.Checked)
                                        {
                                            icam.triggering_objects_as_string = cam.triggering_objects_as_string;
                                            icam.additional_triggering_objects_as_string = cam.additional_triggering_objects_as_string;
                                            icam.triggering_objects = Global.Split(icam.triggering_objects_as_string, ",").ToArray();   //triggering_objects_as_string.Split(','); //split the row of triggering objects between every ','
                                        }
                                        if (frm.cb_apply_actions.Checked)
                                        {
                                            icam.trigger_urls_as_string = cam.trigger_urls_as_string;
                                            icam.trigger_urls = cam.trigger_urls;
                                            icam.cancel_urls_as_string = cam.cancel_urls_as_string;
                                            icam.cancel_urls = cam.cancel_urls;
                                            icam.cooldown_time = cam.cooldown_time;
                                            icam.telegram_enabled = cam.telegram_enabled;
                                            icam.telegram_caption = cam.telegram_caption;
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
                                            icam.maskManager.Objects = cam.maskManager.Objects;

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

                AppSettings.SaveAsync();

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
        //event: delete camera button
        private void btnCameraDel_Click(object sender, EventArgs e)
        {
            if (this.list2.Items.Count > 0)
            {
                using (var form = new InputForm($"Delete camera {this.list2.SelectedItems[0].Text} ?", "Delete Camera?", false))
                {
                    var result = form.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        //Log("about to del cam");
                        this.RemoveCamera(this.list2.SelectedItems[0].Text);
                    }
                }
            }
        }

        //event: DELETE key pressed
        private void list2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (this.list2.Items.Count > 0)
                {
                    using (var form = new InputForm($"Delete camera {this.list2.SelectedItems[0].Text} ?", "Delete Camera?", false))
                    {
                        var result = form.ShowDialog();
                        if (result == DialogResult.OK)
                        {
                            this.RemoveCamera(this.list2.SelectedItems[0].Text);
                        }
                    }
                }
            }
        }

        //event: leaving empty lower confidence limit textbox
        private void tb_threshold_lower_Leave(object sender, EventArgs e)
        {
            if (this.tb_threshold_lower.Text == "")
            {
                this.tb_threshold_lower.Text = "0";
            }
        }

        //event: leaving empty upper confidence limit textbox
        private void tb_threshold_upper_Leave(object sender, EventArgs e)
        {
            if (this.tb_threshold_upper.Text == "")
            {
                this.tb_threshold_upper.Text = "100";
            }
        }



        //----------------------------------------------------------------------------------------------------------
        //SETTING TAB
        //----------------------------------------------------------------------------------------------------------


        //settings save button
        private async void BtnSettingsSave_Click_1(object sender, EventArgs e)
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            Log($"Saving settings to {AppSettings.Settings.SettingsFileName}");
            //save inputted settings into App.settings
            AppSettings.Settings.input_path = this.cmbInput.Text.Trim();
            AppSettings.Settings.input_path_includesubfolders = this.cb_inputpathsubfolders.Checked;
            AppSettings.Settings.deepstack_url = this.tbDeepstackUrl.Text.Trim();
            AppSettings.Settings.deepstack_urls_are_queued = this.cb_DeepStackURLsQueued.Checked;
            AppSettings.Settings.telegram_chatids = Global.Split(this.tb_telegram_chatid.Text, "|;,", true, true);
            AppSettings.Settings.telegram_token = this.tb_telegram_token.Text.Trim();
            AppSettings.Settings.telegram_cooldown_minutes = Convert.ToDouble(this.tb_telegram_cooldown.Text.Trim());
            AppSettings.Settings.send_errors = this.cb_send_errors.Checked;
            AppSettings.Settings.startwithwindows = this.cbStartWithWindows.Checked;
            AppSettings.Settings.minimizetotray = this.minimizeToTrayCheckbox.Checked;

            AppSettings.Settings.DefaultUserName = this.tb_username.Text.Trim();
            AppSettings.Settings.DefaultPasswordEncrypted = Global.EncryptString(this.tb_password.Text.Trim());

            AppSettings.Settings.BlueIrisServer = this.tb_BlueIrisServer.Text.Trim();

            Global.Startup(AppSettings.Settings.startwithwindows);

            //let the image loop (running in another thread) know to recheck ai server url settings.
            AIURLSettingsChanged.WriteFullFence(true);

            if (await AppSettings.SaveAsync())
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

            //update fswatcher to watch new input folder
            UpdateWatchers(true);

            //Update blue iris info
            await BlueIrisInfo.RefreshBIInfoAsync(AppSettings.Settings.BlueIrisServer);


            this.cmbInput.Items.Clear();
            foreach (string pth in BlueIrisInfo.ClipPaths)
                this.cmbInput.Items.Add(pth);


            if (BlueIrisInfo.Result != BlueIrisResult.Valid)
                MessageBox.Show($"Error: Could not connect to BlueIris server: '{BlueIrisInfo.Result}'.  See log for more detail.", "Error", MessageBoxButtons.OK, icon: MessageBoxIcon.Error);

            bool noneg = false;
            if (AppSettings.Settings.telegram_chatids.Count > 0)
            {
                foreach (string id in AppSettings.Settings.telegram_chatids)
                {
                    if (!string.IsNullOrWhiteSpace(id) && !id.TrimStart().StartsWith("-"))
                    {
                        noneg = true;
                    }
                }
            }

            if (noneg)
            {
                MessageBox.Show("Please note that the Telegram Chat ID may need to start with a negative sign. -1234567890", "Telegram Chat ID format", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

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
        private void Shell_FormClosing(object sender, FormClosingEventArgs e)
        {
            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            Log($"------Closing------- CloseReason: {e.CloseReason}");

            IsClosing.WriteFullFence(true);

            try
            {
                if (e.CloseReason != CloseReason.WindowsShutDown && AppSettings.Settings.close_instantly <= 0) //if it's either enabled or not set  -1 = not set | 0 = ask for confirmation | 1 = don't ask
                {
                    using (var form = new InputForm($"Stop and close AI Tool?", "AI Tool", false))
                    {
                        var result = form.ShowDialog();
                        if (AppSettings.Settings.close_instantly == -1)
                        {
                            //if it's the first time, ask if the confirmation dialog should ever appear again
                            using (var form1 = new InputForm($"Confirm closing AI Tool every time?", "AI Tool", false, "NO, Never!", "YES"))
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


                Global_GUI.SaveWindowState(this);

                AppSettings.SaveAsync();  //save settings in any case

                if (!AppSettings.AlreadyRunning)
                    Global.SaveSetting("LastShutdownState", "graceful shutdown");

                MasterCTS.Cancel();



            }
            catch { }



        }

        private void SaveDeepStackTab()
        {

            if (DeepStackServerControl == null)
                DeepStackServerControl = new DeepStack(AppSettings.Settings.deepstack_adminkey, AppSettings.Settings.deepstack_apikey, AppSettings.Settings.deepstack_mode, AppSettings.Settings.deepstack_sceneapienabled, AppSettings.Settings.deepstack_faceapienabled, AppSettings.Settings.deepstack_detectionapienabled, AppSettings.Settings.deepstack_port);

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
            AppSettings.Settings.deepstack_debug = this.Chk_DSDebug.Checked;
            AppSettings.Settings.deepstack_highpriority = this.chk_HighPriority.Checked;
            AppSettings.Settings.deepstack_adminkey = this.Txt_AdminKey.Text.Trim();
            AppSettings.Settings.deepstack_apikey = this.Txt_APIKey.Text.Trim();
            AppSettings.Settings.deepstack_installfolder = this.Txt_DeepStackInstallFolder.Text.Trim();
            AppSettings.Settings.deepstack_port = this.Txt_Port.Text.Trim();


            AppSettings.SaveAsync();


            if (DeepStackServerControl.IsInstalled)
            {
                if (DeepStackServerControl.IsStarted)
                {


                    if (DeepStackServerControl.IsActivated)
                    {
                        MethodInvoker LabelUpdate = delegate { this.Lbl_BlueStackRunning.Text = "*RUNNING*"; };
                        this.Invoke(LabelUpdate);

                        this.Btn_Start.Enabled = false;
                        this.Btn_Stop.Enabled = true;
                    }
                    else
                    {
                        MethodInvoker LabelUpdate = delegate { this.Lbl_BlueStackRunning.Text = "*NOT ACTIVATED, RUNNING*"; };
                        this.Invoke(LabelUpdate);

                        this.Btn_Start.Enabled = false;
                        this.Btn_Stop.Enabled = true;
                    }
                }
                else
                {
                    MethodInvoker LabelUpdate = delegate { this.Lbl_BlueStackRunning.Text = "*NOT RUNNING*"; };
                    this.Invoke(LabelUpdate);

                    this.Btn_Start.Enabled = true;
                    this.Btn_Stop.Enabled = false;
                }
            }
            else
            {
                this.Btn_Start.Enabled = false;
                this.Btn_Stop.Enabled = false;
                MethodInvoker LabelUpdate = delegate { this.Lbl_BlueStackRunning.Text = "*NOT INSTALLED*"; };
                this.Invoke(LabelUpdate);


            }

            DeepStackServerControl.Update(AppSettings.Settings.deepstack_adminkey, AppSettings.Settings.deepstack_apikey, AppSettings.Settings.deepstack_mode, AppSettings.Settings.deepstack_sceneapienabled, AppSettings.Settings.deepstack_faceapienabled, AppSettings.Settings.deepstack_detectionapienabled, AppSettings.Settings.deepstack_port);

        }

        private async Task LoadDeepStackTab(bool StartIfNeeded)
        {

            try
            {
                if (DeepStackServerControl == null)
                    DeepStackServerControl = new DeepStack(AppSettings.Settings.deepstack_adminkey, AppSettings.Settings.deepstack_apikey, AppSettings.Settings.deepstack_mode, AppSettings.Settings.deepstack_sceneapienabled, AppSettings.Settings.deepstack_faceapienabled, AppSettings.Settings.deepstack_detectionapienabled, AppSettings.Settings.deepstack_port);


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

                //have seen a few cases nothing is checked but it is required
                if (!this.Chk_DetectionAPI.Checked && !this.Chk_FaceAPI.Checked && !this.Chk_SceneAPI.Checked)
                {
                    this.Chk_DetectionAPI.Checked = true;
                    DeepStackServerControl.DetectionAPIEnabled = true;
                }

                this.Chk_AutoStart.Checked = AppSettings.Settings.deepstack_autostart;
                this.Chk_DSDebug.Checked = AppSettings.Settings.deepstack_debug;
                this.chk_HighPriority.Checked = AppSettings.Settings.deepstack_highpriority;
                this.Txt_AdminKey.Text = DeepStackServerControl.AdminKey;
                this.Txt_APIKey.Text = DeepStackServerControl.APIKey;
                this.Txt_DeepStackInstallFolder.Text = DeepStackServerControl.DeepStackFolder;
                this.Txt_Port.Text = DeepStackServerControl.Port;

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
                    if (DeepStackServerControl.IsStarted && !DeepStackServerControl.HasError)
                    {
                        if (DeepStackServerControl.IsActivated && (DeepStackServerControl.VisionDetectionRunning || DeepStackServerControl.DetectionAPIEnabled))
                        {

                            MethodInvoker LabelUpdate = delegate { this.Lbl_BlueStackRunning.Text = "*RUNNING*"; };
                            this.Invoke(LabelUpdate);

                            this.Btn_Start.Enabled = false;
                            this.Btn_Stop.Enabled = true;
                        }
                        else if (!DeepStackServerControl.IsActivated)
                        {
                            MethodInvoker LabelUpdate = delegate { this.Lbl_BlueStackRunning.Text = "*NOT ACTIVATED, RUNNING*"; };
                            this.Invoke(LabelUpdate);

                            this.Btn_Start.Enabled = false;
                            this.Btn_Stop.Enabled = true;
                        }
                        else if (!DeepStackServerControl.VisionDetectionRunning || DeepStackServerControl.DetectionAPIEnabled)
                        {
                            MethodInvoker LabelUpdate = delegate { this.Lbl_BlueStackRunning.Text = "*DETECTION API NOT RUNNING*"; };
                            this.Invoke(LabelUpdate);

                            this.Btn_Start.Enabled = false;
                            this.Btn_Stop.Enabled = true;
                        }

                    }
                    else if (DeepStackServerControl.HasError)
                    {
                        MethodInvoker LabelUpdate = delegate { this.Lbl_BlueStackRunning.Text = "*ERROR*"; };
                        this.Invoke(LabelUpdate);

                        this.Btn_Start.Enabled = false;
                        this.Btn_Stop.Enabled = true;
                    }
                    else
                    {
                        MethodInvoker LabelUpdate = delegate { this.Lbl_BlueStackRunning.Text = "*NOT RUNNING*"; };
                        this.Invoke(LabelUpdate);

                        this.Btn_Start.Enabled = true;
                        this.Btn_Stop.Enabled = false;
                        if (this.Chk_AutoStart.Checked && StartIfNeeded)
                        {
                            if (await DeepStackServerControl.StartAsync())
                            {
                                if (DeepStackServerControl.IsStarted && !DeepStackServerControl.HasError)
                                {
                                    LabelUpdate = delegate { this.Lbl_BlueStackRunning.Text = "*RUNNING*"; };
                                    this.Invoke(LabelUpdate);
                                    this.Btn_Start.Enabled = false;
                                    this.Btn_Stop.Enabled = true;
                                }
                                else if (DeepStackServerControl.HasError)
                                {
                                    LabelUpdate = delegate { this.Lbl_BlueStackRunning.Text = "*ERROR*"; };
                                    this.Invoke(LabelUpdate);

                                    this.Btn_Start.Enabled = false;
                                    this.Btn_Stop.Enabled = true;
                                }

                            }
                            else
                            {
                                LabelUpdate = delegate { this.Lbl_BlueStackRunning.Text = "*ERROR*"; };
                                this.Invoke(LabelUpdate);

                                this.Btn_Start.Enabled = false;
                                this.Btn_Stop.Enabled = true;
                            }
                        }
                    }
                }
                else
                {
                    this.Btn_Start.Enabled = false;
                    this.Btn_Stop.Enabled = false;
                    MethodInvoker LabelUpdate = delegate { this.Lbl_BlueStackRunning.Text = "*NOT INSTALLED*"; };
                    this.Invoke(LabelUpdate);


                }

            }
            catch (Exception ex)
            {

                Log(Global.ExMsg(ex));
            }
        }

        private async void Btn_Start_Click(object sender, EventArgs e)
        {
            this.Lbl_BlueStackRunning.Text = "STARTING...";
            this.Btn_Start.Enabled = false;
            this.Btn_Stop.Enabled = false;
            this.SaveDeepStackTab();
            await DeepStackServerControl.StartAsync();
            this.LoadDeepStackTab(true);
        }

        private void Btn_Save_Click(object sender, EventArgs e)
        {
            this.SaveDeepStackTab();
        }

        private async void Btn_Stop_Click(object sender, EventArgs e)
        {
            this.Lbl_BlueStackRunning.Text = "STOPPING...";
            this.Btn_Start.Enabled = false;
            this.Btn_Stop.Enabled = false;
            await DeepStackServerControl.StopAsync();
            this.LoadDeepStackTab(false);
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
            using (Frm_DynamicMasking frm = new Frm_DynamicMasking())
            {
                Camera cam = AITOOL.GetCamera(this.list2.SelectedItems[0].Text);
                frm.cam = cam;
                frm.Text = "Dynamic Masking Settings - " + cam.name;

                //Merge ClassObject's code
                frm.num_history_mins.Value = cam.maskManager.HistorySaveMins;//load minutes to retain history objects that have yet to become masks
                frm.num_mask_create.Value = cam.maskManager.HistoryThresholdCount; // load mask create counter
                frm.num_mask_remove.Value = cam.maskManager.MaskRemoveMins; //load mask remove counter
                frm.num_percent_var.Value = (decimal)cam.maskManager.PercentMatch;
                frm.numMaskThreshold.Value = cam.maskManager.MaskRemoveThreshold;

                frm.cb_enabled.Checked = this.cb_masking_enabled.Checked;

                frm.tb_objects.Text = cam.maskManager.Objects;

                if (frm.ShowDialog() == DialogResult.OK)
                {
                    ////get masking values from textboxes
                    Int32.TryParse(frm.num_history_mins.Text, out int history_mins);
                    Int32.TryParse(frm.num_mask_create.Text, out int mask_create_counter);
                    Int32.TryParse(frm.num_mask_remove.Text, out int mask_remove_mins);
                    Int32.TryParse(frm.numMaskThreshold.Text, out int maskRemoveThreshold);
                    Int32.TryParse(frm.num_percent_var.Text, out int variance);

                    cam.maskManager.HistorySaveMins = history_mins;
                    cam.maskManager.HistoryThresholdCount = mask_create_counter;
                    cam.maskManager.MaskRemoveMins = mask_remove_mins;
                    cam.maskManager.MaskRemoveThreshold = maskRemoveThreshold;
                    cam.maskManager.PercentMatch = variance;

                    this.cb_masking_enabled.Checked = frm.cb_enabled.Checked;
                    cam.maskManager.MaskingEnabled = this.cb_masking_enabled.Checked;
                    cam.maskManager.Objects = frm.tb_objects.Text.Trim();

                    AppSettings.SaveAsync();
                }
            }
        }

        private void btnDetails_Click(object sender, EventArgs e)
        {
            this.ShowMaskDetailsDialog(this.list2.SelectedItems[0].Text);
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
            this.ShowEditImageMaskDialog(this.list2.SelectedItems[0].Text);
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

            using (Frm_LegacyActions frm = new Frm_LegacyActions())
            {

                if (this.list2.SelectedItems.Count == 0)
                    return;

                Camera cam = AITOOL.GetCamera(this.list2.SelectedItems[0].Text);

                frm.cam = cam;

                frm.tbTriggerUrl.Text = string.Join("\r\n", Global.Split(cam.trigger_urls_as_string, "\r\n|;,"));
                frm.tbCancelUrl.Text = string.Join("\r\n", Global.Split(cam.cancel_urls_as_string, "\r\n|;,"));
                frm.tb_cooldown.Text = cam.cooldown_time.ToString(); //load cooldown time
                //load telegram image sending on/off option
                frm.cb_telegram.Checked = cam.telegram_enabled;
                frm.tb_telegram_caption.Text = cam.telegram_caption;
                frm.tb_telegram_triggering_objects.Text = cam.telegram_triggering_objects;

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

                frm.cb_queue_actions.Checked = cam.Action_queued;

                frm.cb_mergeannotations.Checked = cam.Action_image_merge_detections;

                frm.tb_jpeg_merge_quality.Text = cam.Action_image_merge_jpegquality.ToString();

                if (frm.cb_mergeannotations.Checked)
                    frm.tb_jpeg_merge_quality.Enabled = true;
                else
                    frm.tb_jpeg_merge_quality.Enabled = false;

                if (frm.ShowDialog() == DialogResult.OK)
                {
                    cam.trigger_urls_as_string = string.Join(",", Global.Split(frm.tbTriggerUrl.Text.Trim(), "\r\n|;,"));
                    cam.trigger_urls = Global.Split(cam.trigger_urls_as_string, "\r\n|;,").ToArray();
                    cam.cancel_urls_as_string = string.Join(",", Global.Split(frm.tbCancelUrl.Text.Trim(), "\r\n|;,"));
                    cam.cancel_urls = Global.Split(cam.cancel_urls_as_string, "\r\n|;,").ToArray();

                    cam.cooldown_time = Convert.ToDouble(frm.tb_cooldown.Text.Trim());
                    cam.telegram_enabled = frm.cb_telegram.Checked;
                    cam.telegram_caption = frm.tb_telegram_caption.Text.Trim();
                    cam.telegram_triggering_objects = frm.tb_telegram_triggering_objects.Text;

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

                    cam.Action_image_merge_detections = frm.cb_mergeannotations.Checked;

                    cam.Action_image_merge_jpegquality = Convert.ToInt64(frm.tb_jpeg_merge_quality.Text);

                    cam.Action_queued = frm.cb_queue_actions.Checked;

                    AppSettings.SaveAsync();

                }
            }
        }

        private void tbDeepstackUrl_TextChanged(object sender, EventArgs e)
        {

        }

        private void tabLog_Click(object sender, EventArgs e)
        {

        }

        private void folv_history_SelectionChanged(object sender, EventArgs e)
        {

            if (IsClosing.ReadFullFence())
                return;

            try
            {
                if (this.folv_history.SelectedObjects != null && this.folv_history.SelectedObjects.Count > 0)
                {
                    History hist = (History)this.folv_history.SelectedObjects[0];

                    if (hist == null)
                        return;

                    if (!String.IsNullOrEmpty(hist.Filename) && hist.Filename.Contains("\\") && File.Exists(hist.Filename))
                    {
                        using (var img = new Bitmap(hist.Filename))
                        {
                            this.pictureBox1.BackgroundImage = new Bitmap(img); //load actual image as background, so that an overlay can be added as the image
                        }
                        this.showHideMask();
                        this.lbl_objects.Text = hist.Detections;
                    }
                    else
                    {
                        Log("Removing missing file from database: " + hist.Filename);
                        HistoryDB.DeleteHistoryQueue(hist.Filename);
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

                }
                else
                {
                    this.lbl_objects.Text = "No selection";
                    this.pictureBox1.BackgroundImage = null;
                    this.toolStripButtonDetails.Enabled = false;
                    this.toolStripButtonEditImageMask.Enabled = false;
                    this.toolStripButtonMaskDetails.Enabled = false;
                }

            }
            catch (Exception ex)
            {
                Log($"Error: {Global.ExMsg(ex)}");

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
            // Log("Error: " & ExMsg(ex))
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

            LogMan.ErrorCount.WriteFullFence(0);

            AppSettings.SaveAsync();

            this.UpdatePieChart(); this.UpdateTimeline(); this.UpdateConfidenceChart();

            this.UpdateStats();
        }

        private async void cb_filter_skipped_CheckedChanged(object sender, EventArgs e)
        {
            await this.LoadHistoryAsync(true, AppSettings.Settings.HistoryFollow).ConfigureAwait(false);
        }

        private async void HistoryUpdateListTimer_Tick(object sender, EventArgs e)
        {
            if (IsClosing.ReadFullFence())
                return;

            if (!AppSettings.AlreadyRunning)
                Global.SaveSetting("LastShutdownState", $"checkpoint: HistoryUpdateTimer: {DateTime.Now}");

            await this.UpdateHistoryAddedRemoved();

            this.UpdateStats();
        }

        private void folv_history_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void toolStripStatusErrors_Click(object sender, EventArgs e)
        {
            this.ShowErrors();
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

            AppSettings.SaveAsync();
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
            AppSettings.SaveAsync();
        }

        private async void cb_filter_person_Click(object sender, EventArgs e)
        {
            SaveFilters();
            await this.LoadHistoryAsync(true, AppSettings.Settings.HistoryFollow).ConfigureAwait(false);
            AppSettings.SaveAsync();
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
            AppSettings.SaveAsync();
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
            AppSettings.SaveAsync();
            this.HistoryStartStop();

        }

        private void cb_showObjects_Click(object sender, EventArgs e)
        {
            AppSettings.Settings.HistoryShowObjects = this.cb_showObjects.Checked;
            AppSettings.SaveAsync();
            this.pictureBox1.Refresh();
        }

        private void cb_follow_Click(object sender, EventArgs e)
        {
            AppSettings.Settings.HistoryFollow = this.cb_follow.Checked;
            AppSettings.SaveAsync();
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

                foreach (History hist in this.folv_history.SelectedObjects)
                {
                    List<ClsPrediction> predictions = Global.SetJSONString<List<ClsPrediction>>(hist.PredictionsJSON);

                    if (predictions != null && predictions.Count > 0)
                    {
                        allpredictions.AddRange(predictions);
                    }
                    else
                    {
                        Log($"debug: No predictions for image {hist.Filename}: Json='{hist.PredictionsJSON}'");
                    }

                }

                Frm_ObjectDetail frm = new Frm_ObjectDetail();
                frm.PredictionObjectDetail = allpredictions;
                frm.Show();

            }
            catch (Exception ex)
            {

                Log("Error: " + Global.ExMsg(ex));
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
                        string ext = Path.GetExtension(hist.Filename);
                        string testfile = Path.Combine(folder, $"{filename}_AITOOLTEST_{DateTime.Now.TimeOfDay.TotalSeconds}{ext}");
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
            foreach (Camera cam in AppSettings.Settings.CameraList)
            {
                if (!cam.telegram_enabled)
                {
                    cam.telegram_enabled = true;
                    Log($"Enabled Telegram on camera '{cam.name}'.");
                }
            }
            AppSettings.SaveAsync();
        }

        private void btn_disabletelegram_Click(object sender, EventArgs e)
        {
            foreach (Camera cam in AppSettings.Settings.CameraList)
            {
                if (cam.telegram_enabled)
                {
                    cam.telegram_enabled = false;
                    Log($"Disabled Telegram on camera '{cam.name}'.");
                }
            }
            AppSettings.SaveAsync();
        }

        private void storeFalseAlertsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AppSettings.Settings.HistoryStoreFalseAlerts = this.storeFalseAlertsToolStripMenuItem.Checked;
            AppSettings.SaveAsync();
        }

        private void storeMaskedAlertsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AppSettings.Settings.HistoryStoreMaskedAlerts = this.storeMaskedAlertsToolStripMenuItem.Checked;
            AppSettings.SaveAsync();

        }

        private void showOnlyRelevantObjectsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AppSettings.Settings.HistoryOnlyDisplayRelevantObjects = this.showOnlyRelevantObjectsToolStripMenuItem.Checked;
            AppSettings.SaveAsync();
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
            if (IsLoading.ReadFullFence())
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
            if (IsLoading.ReadFullFence())
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
                    string date = Global.GetWordBetween(fi.FullName, "[", "]");
                    //AITool.[2020-10-19.1].log.zip
                    //        ----------
                    date = Global.GetWordBetween(date, "", ".");

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
                                        imagemaskkey = "key=" + Global.GetWordBetween(CLI.Detail, "key=", ",| ");
                                        Log("Debug: " + imagemaskkey);
                                    }
                                    //   Found 'Person' (Key=191457) in last_positions_history: key=198932, name=Person, xmin=1108
                                    else if (CLI.Detail.IndexOf("last_positions_history: key=", StringComparison.OrdinalIgnoreCase) >= 0)
                                    {
                                        matchedmaskkey = "key=" + Global.GetWordBetween(CLI.Detail, "history: key=", ",| ");
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

                Log("Error: " + Global.ExMsg(ex));
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

            if (IsLoading.ReadFullFence())
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

                if (!IsLoading.ReadFullFence() && Global.IsRegexPatternValid(this.ToolStripComboBoxSearch.Text))
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

            if (IsLoading.ReadFullFence())
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

                if (!IsLoading.ReadFullFence())
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
            if (IsLoading.ReadFullFence())
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
            LogMan.ErrorCount.WriteFullFence(0);
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
            if (IsLoading.ReadFullFence())
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
            if (IsLoading.ReadFullFence())
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

                string LastFile = Global.GetSetting("LastLoadedLogFile", LogMan.GetCurrentLogFileName());

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
                    Global.SaveSetting("LastLoadedLogFile", ofd.FileName);
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
            if (IsLoading.ReadFullFence())
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

                        Log("Error: " + Global.ExMsg(ex));
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

                Log("Error: " + Global.ExMsg(ex));
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

                string LastFile = Global.GetSetting("LastLoadedImageFile", "");

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
                        Global.SaveSetting("LastLoadedImageFile", ofd.FileName);
                        AddImageToQueue(ofd.FileName);
                        //small delay
                        await Task.Delay(AppSettings.Settings.file_access_delay);
                    }
                }


            }
        }

        private void restrictThresholdAtSourceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AppSettings.Settings.HistoryRestrictMinThresholdAtSource = this.restrictThresholdAtSourceToolStripMenuItem.Checked;
            AppSettings.SaveAsync();
        }

    }

    //enhanced TableLayoutPanel loads faster
    public partial class DBLayoutPanel : TableLayoutPanel
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


