using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AITool
{
    public partial class Frm_AIServerDeepstackEdit:Form
    {
        public ClsURLItem CurURL;

        public Frm_AIServerDeepstackEdit()
        {
            InitializeComponent();
        }

        private void Frm_AIServerDeepstackEdit_Load(object sender, EventArgs e)
        {
            //Global_GUI.RestoreWindowState(this);

            this.tb_URL.Text = this.CurURL.url;
            this.lbl_type.Text = this.CurURL.Type.ToString();
            this.tb_Name.Text = this.CurURL.Name;

            if (this.CurURL.Type == URLTypeEnum.AWSRekognition_Objects || this.CurURL.Type == URLTypeEnum.AWSRekognition_Faces)
                this.tb_URL.Enabled = false;
            else
                this.tb_URL.Enabled = true;

            this.tb_ActiveTimeRange.Text = this.CurURL.ActiveTimeRange;
            this.chk_Enabled.Checked = this.CurURL.Enabled;
            this.tb_ApplyToCams.Text = this.CurURL.Cameras;
            this.tb_ImagesPerMonth.Text = this.CurURL.MaxImagesPerMonth.ToString();
            //this.cb_ImageAdjustProfile.Text = this.CurURL.ImageAdjustProfile;
            this.linkHelpURL.Text = this.CurURL.HelpURL;
            this.tb_Lower.Text = this.CurURL.Threshold_Lower.ToString();
            this.tb_Upper.Text = this.CurURL.Threshold_Upper.ToString();
            this.tb_timeout.Text = this.CurURL.HttpClientTimeoutSeconds.ToString();
            //this.labelTimeout.Text = $"Timeout Seconds Override (Default={this.CurURL.GetTimeout().TotalSeconds}):";

            this.cb_RefinementServer.Checked = this.CurURL.UseAsRefinementServer;
            this.tb_RefinementObjects.Text = this.CurURL.RefinementObjects;

            this.cb_LinkedServers.Checked = this.CurURL.LinkServerResults;

            this.cb_TimeoutError.Checked = AppSettings.Settings.MaxWaitForAIServerTimeoutError;
            this.tb_LinkedRefineTimeout.Text = AppSettings.Settings.MaxWaitForAIServerMS.ToString();

            this.cb_OnlyLinked.Checked = this.CurURL.UseOnlyAsLinkedServer;

            this.cb_IgnoreOffline.Checked = this.CurURL.IgnoreOfflineError;

            this.cb_AllowAIServerBasedQueue.Checked = this.CurURL.AllowAIServerBasedQueue;

            this.tb_MaxQueueLength.Text = this.CurURL.AIMaxQueueLength.ToString();

            this.tb_SkipIfImgQueueLengthLarger.Text = this.CurURL.SkipIfImgQueueLengthLarger.ToString();

            this.tb_SkipIfAIQueueTimeOverSecs.Text = this.CurURL.SkipIfAIQueueTimeOverSecs.ToString();

            List<string> linked = this.CurURL.LinkedResultsServerList.SplitStr(",;|");



            //Add all servers except current one and refinement server
            int idx = 0;
            //make a case insensitive hashset:
            HashSet<string> dupes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (ClsURLItem url in AppSettings.Settings.AIURLList)
            {

                if (url.Enabled && !dupes.Contains(url.ToString()) && !url.UseAsRefinementServer && !string.Equals(this.CurURL.ToString(), url.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    dupes.Add(url.ToString());
                    this.checkedComboBoxLinked.Items.Add(url);
                    if (Global.IsInList(url.ToString(), this.CurURL.LinkedResultsServerList, TrueIfEmpty: false))
                        this.checkedComboBoxLinked.SetItemChecked(idx, true);
                    idx++;
                }

            }

            //foreach (ClsImageAdjust ia in AppSettings.Settings.ImageAdjustProfiles)
            //    this.cb_ImageAdjustProfile.Items.Add(ia.Name);

            //this.cb_ImageAdjustProfile.SelectedIndex = this.cb_ImageAdjustProfile.Items.IndexOf(this.CurURL.DefaultURL);

            Global_GUI.GroupboxEnableDisable(groupBox1, chk_Enabled);
            Global_GUI.GroupboxEnableDisable(groupBoxRefine, cb_RefinementServer);
            Global_GUI.GroupboxEnableDisable(groupBoxLinked, cb_LinkedServers);
            Global_GUI.GroupboxEnableDisable(gb_AIServerQueue, cb_AllowAIServerBasedQueue);


        }

        private void UpdateStats()
        {
            this.lbl_QueueLengthStats.Text = $"{this.CurURL.AIQueueLengthCalcs.ToStringShort()}";
            this.lbl_QueueTimeStats.Text = $"{this.CurURL.AIQueueTimeCalcs.ToStringShort()}";
            this.lbl_ImgQueueStats.Text = $"{AITOOL.scalc.ToStringShort()}";
        }

        private void Frm_AIServerDeepstackEdit_FormClosing(object sender, FormClosingEventArgs e)
        {
            Global_GUI.SaveWindowState(this);
        }


        private void tb_URL_TextChanged(object sender, EventArgs e)
        {
            //ValidateForm();
        }

        private void ValidateForm()
        {
            this.CurURL.url = tb_URL.Text.Trim();
            if (!this.CurURL.Update(false))
            {
                tb_URL.ForeColor = Color.White;
                tb_URL.BackColor = Color.Red;
                bt_Save.Enabled = false;
                btTest.Enabled = false;

            }
            else
            {
                if (this.CurURL.UrlFixed)
                    tb_URL.Text = this.CurURL.url;  //not sure if this is the right thing to do here...  May interfere with peoples typing

                tb_URL.ResetBackColor();
                tb_URL.ResetForeColor();
                bt_Save.Enabled = true;
                btTest.Enabled = true;
            }
        }

        private void bt_Save_Click(object sender, EventArgs e)
        {
            this.UpdateURL();
            AITOOL.UpdateAIURLs();
            AppSettings.SaveAsync(true);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void UpdateURL()
        {
            this.CurURL.url = this.tb_URL.Text.Trim();
            this.CurURL.Name = this.tb_Name.Text.Trim();
            this.CurURL.ActiveTimeRange = this.tb_ActiveTimeRange.Text.Trim();
            this.CurURL.Enabled = this.chk_Enabled.Checked;
            this.CurURL.Cameras = this.tb_ApplyToCams.Text.Trim();
            //this.CurURL.ImageAdjustProfile = this.cb_ImageAdjustProfile.Text;
            this.CurURL.MaxImagesPerMonth = this.tb_ImagesPerMonth.Text.ToInt();

            this.CurURL.Threshold_Lower = this.tb_Lower.Text.ToInt();
            this.CurURL.Threshold_Upper = this.tb_Upper.Text.ToInt();

            this.CurURL.RefinementObjects = this.tb_RefinementObjects.Text.Trim();
            this.CurURL.UseAsRefinementServer = this.cb_RefinementServer.Checked;

            this.CurURL.LinkServerResults = this.cb_LinkedServers.Checked;
            this.CurURL.LinkedResultsServerList = "";
            foreach (ClsURLItem url in checkedComboBoxLinked.CheckedItems)
            {
                this.CurURL.LinkedResultsServerList += url.ToString() + ", ";
                url.UseOnlyAsLinkedServer = true;
            }
            this.CurURL.LinkedResultsServerList = this.CurURL.LinkedResultsServerList.Trim(" ,".ToCharArray());

            this.CurURL.UseOnlyAsLinkedServer = this.cb_OnlyLinked.Checked;

            this.CurURL.HttpClientTimeoutSeconds = this.tb_timeout.Text.ToInt();

            this.CurURL.IgnoreOfflineError = this.cb_IgnoreOffline.Checked;

            this.CurURL.AllowAIServerBasedQueue = this.cb_AllowAIServerBasedQueue.Checked;

            this.CurURL.AIMaxQueueLength = this.tb_MaxQueueLength.Text.ToInt();

            this.CurURL.SkipIfImgQueueLengthLarger = this.tb_SkipIfImgQueueLengthLarger.Text.ToInt();

            this.CurURL.SkipIfAIQueueTimeOverSecs = this.tb_SkipIfAIQueueTimeOverSecs.Text.ToInt();

            if (!string.IsNullOrWhiteSpace(this.tb_LinkedRefineTimeout.Text) && this.tb_LinkedRefineTimeout.Text.ToInt() >= 20)
                AppSettings.Settings.MaxWaitForAIServerMS = this.tb_LinkedRefineTimeout.Text.ToInt();

            AppSettings.Settings.MaxWaitForAIServerTimeoutError = this.cb_TimeoutError.Checked;



        }
        private void linkHelpURL_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(linkHelpURL.Text);
        }

        private void btn_ImageAdjustEdit_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Not implemented yet.");
        }

        private async void btTest_Click(object sender, EventArgs e)
        {

            this.UpdateURL();

            AITOOL.UpdateAIURLs();

            string pth = Global.GetRegSetting("TestImage", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestImage.jpg"));

            OpenFileDialog ofd = new OpenFileDialog
            {
                InitialDirectory = Path.GetDirectoryName(pth),
                FileName = Path.GetFileName(pth),
                Title = "Select test image",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "jpg",
                Filter = "jpg files (*.jpg)|*.jpg",
                FilterIndex = 2,
                RestoreDirectory = true,
                ShowReadOnly = true
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                pth = ofd.FileName;
                Global.SaveRegSetting("TestImage", pth);

                if (File.Exists(pth))
                {
                    //must create a temp unique file every time because database key is the filename
                    Camera cam = AITOOL.GetCamera("default", true);
                    if (cam == null)
                        cam = new Camera("TEST_CAM");

                    string ext = Path.GetExtension(pth);
                    string tpth = Path.Combine(Global.GetTempFolder(), $"{cam.Name}.URLTEST.{DateTime.Now:yyyy-MM-dd_HH-mm-ss-fff}{ext}");
                    File.Copy(pth, tpth, true);

                    btTest.Enabled = false;
                    bt_Save.Enabled = false;
                    btTest.Text = "Working...";
                    this.UpdateURL();
                    ClsImageQueueItem CurImg = new ClsImageQueueItem(tpth, 0);

                    List<ClsURLItem> linked = new List<ClsURLItem> { this.CurURL };

                    if (this.CurURL.LinkServerResults && !string.IsNullOrEmpty(this.CurURL.LinkedResultsServerList))
                    {
                        linked.AddRange(await AITOOL.WaitForNextURL(cam, false, null, this.CurURL.LinkedResultsServerList));
                    }
                    if (linked.Count > 1)
                    {
                        AITOOL.Log($"Debug: ---- Found '{linked.Count}' linked AI URL's.");
                    }

                    AITOOL.DetectObjectsResult result = await AITOOL.DetectObjects(CurImg, linked, cam);

                    //make sure not stuck in use for the test:
                    foreach (var url in result.OutURLs)
                    {
                        url.DecrementQueue();
                    }

                    btTest.Enabled = true;
                    bt_Save.Enabled = true;
                    btTest.Text = "Test";
                    if (result.Success)
                    {

                        Frm_ObjectDetail frm = new Frm_ObjectDetail();
                        frm.PredictionObjectDetailsList = result.OutPredictions;
                        frm.ImageFileName = tpth;
                        frm.Show();

                        MessageBox.Show($"Success! Time={result.TimeMS}ms: {this.CurURL.LastResultMessage}", "Success");
                    }
                    else
                        MessageBox.Show($"Error! Time={result.TimeMS}ms: {this.CurURL.LastResultMessage}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    this.CurURL.ErrCount = 0;
                    this.CurURL.CurErrCount = 0;

                }
                else
                {
                    MessageBox.Show($"Test file does not exist:\r\n{pth}");
                }

            }

        }

        private void bt_clear_Click(object sender, EventArgs e)
        {
            this.CurURL.ErrCount = 0;
            this.CurURL.CurErrCount = 0;
            this.CurURL.ErrsInRowCount = 0;
            this.CurURL.ErrDisabled = false;
            this.CurURL.AITimeCalcs = new MovingCalcs(500, "AITime", true);
            this.CurURL.AIQueueLengthCalcs = new MovingCalcs(500, "AIQueueLength", false);
            this.CurURL.AIQueueTimeCalcs = new MovingCalcs(500, "AIQueueTime", true);
            this.CurURL.LastResultMessage = "";
            this.CurURL.NotReadyReason = "";
            this.CurURL.LastSkippedReason = "";
            this.CurURL.LastTimeMS = 0;
            this.CurURL.LastTestedTime = new ThreadSafe.DateTime(DateTime.MinValue, AppSettings.Settings.DateFormat);
            this.CurURL.LastUsedTime = new ThreadSafe.DateTime(DateTime.MinValue, AppSettings.Settings.DateFormat);
            this.CurURL.LastResultSuccess = false;
            this.CurURL.InUse = false;
            this.CurURL.AIQueueLength = 0;
            this.CurURL.AIQueueSkippedCount = 0;

            MessageBox.Show("Cleared error counts and stats.");

        }

        private void cb_RefinementServer_CheckedChanged(object sender, EventArgs e)
        {
            Global_GUI.GroupboxEnableDisable(groupBoxRefine, cb_RefinementServer);
        }

        private void chk_Enabled_CheckedChanged(object sender, EventArgs e)
        {
            Global_GUI.GroupboxEnableDisable(groupBox1, chk_Enabled);
        }

        private void cb_LinkedServers_CheckedChanged(object sender, EventArgs e)
        {
            Global_GUI.GroupboxEnableDisable(groupBoxLinked, cb_LinkedServers);
            if (!cb_LinkedServers.Checked)
                this.CurURL.LinkedResultsServerList = "";

        }

        private void cb_OnlyLinked_CheckedChanged(object sender, EventArgs e)
        {
            if (cb_OnlyLinked.Checked && cb_LinkedServers.Checked)
                cb_LinkedServers.Checked = false;
        }

        private void cb_AllowAIServerBasedQueue_CheckedChanged(object sender, EventArgs e)
        {
            Global_GUI.GroupboxEnableDisable(gb_AIServerQueue, cb_AllowAIServerBasedQueue);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateStats();
        }
    }
}
