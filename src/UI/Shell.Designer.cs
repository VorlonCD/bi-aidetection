namespace AITool
{
    partial class Shell
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Shell));
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series5 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series6 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series7 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series8 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint1 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(1D, 30D);
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint2 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(1D, 30D);
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint3 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(1D, 30D);
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            notifyIcon = new System.Windows.Forms.NotifyIcon(components);
            TraycontextMenuStrip = new System.Windows.Forms.ContextMenuStrip(components);
            pauseAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            resumeAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            pauseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            tabControl1 = new System.Windows.Forms.TabControl();
            tabOverview = new System.Windows.Forms.TabPage();
            tableLayoutPanel14 = new DBLayoutPanel(components);
            tableLayoutPanel15 = new DBLayoutPanel(components);
            pictureBox2 = new System.Windows.Forms.PictureBox();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            lbl_version = new System.Windows.Forms.Label();
            lbl_errors = new System.Windows.Forms.Label();
            lbl_info = new System.Windows.Forms.Label();
            lblQueue = new System.Windows.Forms.Label();
            tabStats = new System.Windows.Forms.TabPage();
            tableLayoutPanel16 = new DBLayoutPanel(components);
            tableLayoutPanel23 = new DBLayoutPanel(components);
            label8 = new System.Windows.Forms.Label();
            chart_confidence = new System.Windows.Forms.DataVisualization.Charting.Chart();
            timeline = new System.Windows.Forms.DataVisualization.Charting.Chart();
            label7 = new System.Windows.Forms.Label();
            tableLayoutPanel17 = new DBLayoutPanel(components);
            chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            comboBox1 = new System.Windows.Forms.ComboBox();
            btn_resetstats = new System.Windows.Forms.Button();
            tabHistory = new System.Windows.Forms.TabPage();
            toolStrip1 = new System.Windows.Forms.ToolStrip();
            comboBox_filter_camera = new System.Windows.Forms.ToolStripComboBox();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            toolStripDropDownButtonFilters = new System.Windows.Forms.ToolStripDropDownButton();
            cb_filter_success = new System.Windows.Forms.ToolStripMenuItem();
            cb_filter_nosuccess = new System.Windows.Forms.ToolStripMenuItem();
            cb_filter_person = new System.Windows.Forms.ToolStripMenuItem();
            cb_filter_animal = new System.Windows.Forms.ToolStripMenuItem();
            cb_filter_vehicle = new System.Windows.Forms.ToolStripMenuItem();
            cb_filter_skipped = new System.Windows.Forms.ToolStripMenuItem();
            cb_filter_masked = new System.Windows.Forms.ToolStripMenuItem();
            toolStripDropDownButtonOptions = new System.Windows.Forms.ToolStripDropDownButton();
            cb_showMask = new System.Windows.Forms.ToolStripMenuItem();
            cb_showObjects = new System.Windows.Forms.ToolStripMenuItem();
            showOnlyRelevantObjectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            cb_follow = new System.Windows.Forms.ToolStripMenuItem();
            automaticallyRefreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            storeFalseAlertsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            storeMaskedAlertsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            restrictThresholdAtSourceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            mergeDuplicatePredictionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripButtonDetails = new System.Windows.Forms.ToolStripButton();
            toolStripButtonMaskDetails = new System.Windows.Forms.ToolStripButton();
            toolStripButtonEditImageMask = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            toolStripButtonEditURL = new System.Windows.Forms.ToolStripButton();
            toolStripButtonAdjustAnno = new System.Windows.Forms.ToolStripButton();
            splitContainer2 = new System.Windows.Forms.SplitContainer();
            groupBox8 = new System.Windows.Forms.GroupBox();
            folv_history = new BrightIdeasSoftware.FastObjectListView();
            contextMenuStripHistory = new System.Windows.Forms.ContextMenuStrip(components);
            testDetectionAgainToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            detailsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            dynamicMaskDetailsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            locateInLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            manuallyAddImagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            viewImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            jumpToImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            lbl_objects = new System.Windows.Forms.Label();
            pictureBox1 = new System.Windows.Forms.PictureBox();
            toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            tabCameras = new System.Windows.Forms.TabPage();
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            splitContainer3 = new System.Windows.Forms.SplitContainer();
            FOLV_Cameras = new BrightIdeasSoftware.FastObjectListView();
            CameraImageList = new System.Windows.Forms.ImageList(components);
            pictureBoxCamera = new System.Windows.Forms.PictureBox();
            tableLayoutPanel6 = new DBLayoutPanel(components);
            tableLayoutPanel11 = new DBLayoutPanel(components);
            btnCameraAdd = new System.Windows.Forms.Button();
            btnCameraDel = new System.Windows.Forms.Button();
            btnSaveTo = new System.Windows.Forms.Button();
            btnCameraSave = new System.Windows.Forms.Button();
            btnPause = new System.Windows.Forms.Button();
            lbl_camstats = new System.Windows.Forms.Label();
            tableLayoutPanel7 = new DBLayoutPanel(components);
            label26 = new System.Windows.Forms.Label();
            label14 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            lblPrefix = new System.Windows.Forms.Label();
            lblName = new System.Windows.Forms.Label();
            tableLayoutPanel12 = new DBLayoutPanel(components);
            lbl_prefix = new System.Windows.Forms.Label();
            tbPrefix = new System.Windows.Forms.TextBox();
            tableLayoutPanel13 = new DBLayoutPanel(components);
            cb_enabled = new System.Windows.Forms.CheckBox();
            tbName = new System.Windows.Forms.TextBox();
            lblRelevantObjects = new System.Windows.Forms.Label();
            tableLayoutPanel26 = new System.Windows.Forms.TableLayoutPanel();
            cmbcaminput = new System.Windows.Forms.ComboBox();
            cb_monitorCamInputfolder = new System.Windows.Forms.CheckBox();
            button2 = new System.Windows.Forms.Button();
            tableLayoutPanel27 = new System.Windows.Forms.TableLayoutPanel();
            cb_masking_enabled = new System.Windows.Forms.CheckBox();
            BtnDynamicMaskingSettings = new System.Windows.Forms.Button();
            btnDetails = new System.Windows.Forms.Button();
            btnCustomMask = new System.Windows.Forms.Button();
            lblDrawMask = new System.Windows.Forms.Label();
            label25 = new System.Windows.Forms.Label();
            dbLayoutPanel6 = new DBLayoutPanel(components);
            tbBiCamName = new System.Windows.Forms.TextBox();
            dbLayoutPanel7 = new DBLayoutPanel(components);
            tb_camera_telegram_chatid = new System.Windows.Forms.TextBox();
            tbCustomMaskFile = new System.Windows.Forms.TextBox();
            label21 = new System.Windows.Forms.Label();
            label39 = new System.Windows.Forms.Label();
            dbLayoutPanel11 = new DBLayoutPanel(components);
            BtnRelevantObjects = new System.Windows.Forms.Button();
            lbl_RelevantObjects = new System.Windows.Forms.Label();
            label15 = new System.Windows.Forms.Label();
            dbLayoutPanel12 = new DBLayoutPanel(components);
            Lbl_PredictionTolerances = new System.Windows.Forms.Label();
            BtnPredictionSize = new System.Windows.Forms.Button();
            dbLayoutPanel13 = new DBLayoutPanel(components);
            Lbl_Actions = new System.Windows.Forms.Label();
            btnActions = new System.Windows.Forms.Button();
            tabSettings = new System.Windows.Forms.TabPage();
            tableLayoutPanel4 = new DBLayoutPanel(components);
            tableLayoutPanel5 = new DBLayoutPanel(components);
            lbl_input = new System.Windows.Forms.Label();
            lbl_telegram_token = new System.Windows.Forms.Label();
            tableLayoutPanel18 = new DBLayoutPanel(components);
            btn_input_path = new System.Windows.Forms.Button();
            cmbInput = new System.Windows.Forms.ComboBox();
            cb_inputpathsubfolders = new System.Windows.Forms.CheckBox();
            lbl_deepstackurl = new System.Windows.Forms.Label();
            dbLayoutPanel1 = new DBLayoutPanel(components);
            cb_DeepStackURLsQueued = new System.Windows.Forms.CheckBox();
            button1 = new System.Windows.Forms.Button();
            dbLayoutPanel2 = new DBLayoutPanel(components);
            tb_telegram_cooldown = new System.Windows.Forms.TextBox();
            label5 = new System.Windows.Forms.Label();
            tb_telegram_chatid = new System.Windows.Forms.TextBox();
            lbl_telegram_chatid = new System.Windows.Forms.Label();
            tb_telegram_token = new System.Windows.Forms.TextBox();
            label12 = new System.Windows.Forms.Label();
            dbLayoutPanel3 = new DBLayoutPanel(components);
            cb_send_telegram_errors = new System.Windows.Forms.CheckBox();
            cb_send_pushover_errors = new System.Windows.Forms.CheckBox();
            label4 = new System.Windows.Forms.Label();
            dbLayoutPanel4 = new DBLayoutPanel(components);
            label6 = new System.Windows.Forms.Label();
            tb_username = new System.Windows.Forms.TextBox();
            tb_password = new System.Windows.Forms.TextBox();
            label16 = new System.Windows.Forms.Label();
            label17 = new System.Windows.Forms.Label();
            label18 = new System.Windows.Forms.Label();
            dbLayoutPanel5 = new DBLayoutPanel(components);
            tb_BlueIrisServer = new System.Windows.Forms.TextBox();
            label19 = new System.Windows.Forms.Label();
            lbl_blueirisserver = new System.Windows.Forms.Label();
            label29 = new System.Windows.Forms.Label();
            dbLayoutPanel8 = new DBLayoutPanel(components);
            tb_Pushover_Cooldown = new System.Windows.Forms.TextBox();
            tb_Pushover_APIKey = new System.Windows.Forms.TextBox();
            label31 = new System.Windows.Forms.Label();
            label30 = new System.Windows.Forms.Label();
            tb_Pushover_UserKey = new System.Windows.Forms.TextBox();
            dbLayoutPanel9 = new DBLayoutPanel(components);
            cbStartWithWindows = new System.Windows.Forms.CheckBox();
            cbMinimizeToTray = new System.Windows.Forms.CheckBox();
            label13 = new System.Windows.Forms.Label();
            dbLayoutPanel10 = new DBLayoutPanel(components);
            button3 = new System.Windows.Forms.Button();
            BtnSettingsSave = new System.Windows.Forms.Button();
            bt_CheckUpdates = new System.Windows.Forms.Button();
            tabDeepStack = new System.Windows.Forms.TabPage();
            chk_AutoAdd = new System.Windows.Forms.CheckBox();
            label34 = new System.Windows.Forms.Label();
            label33 = new System.Windows.Forms.Label();
            label32 = new System.Windows.Forms.Label();
            txt_DeepstackNoMoreOftenThanMins = new System.Windows.Forms.TextBox();
            txt_DeepstackRestartFailCount = new System.Windows.Forms.TextBox();
            Chk_AutoReStart = new System.Windows.Forms.CheckBox();
            Btn_ViewLog = new System.Windows.Forms.Button();
            Btn_DeepstackReset = new System.Windows.Forms.Button();
            linkLabel1 = new System.Windows.Forms.LinkLabel();
            groupBox11 = new System.Windows.Forms.GroupBox();
            tb_DeepStackURLs = new System.Windows.Forms.TextBox();
            groupBox10 = new System.Windows.Forms.GroupBox();
            tb_DeepstackCommandLine = new System.Windows.Forms.TextBox();
            lbl_DeepstackType = new System.Windows.Forms.Label();
            lbl_Deepstackversion = new System.Windows.Forms.Label();
            lbl_deepstackname = new System.Windows.Forms.Label();
            label28 = new System.Windows.Forms.Label();
            label24 = new System.Windows.Forms.Label();
            label23 = new System.Windows.Forms.Label();
            label22 = new System.Windows.Forms.Label();
            chk_stopbeforestart = new System.Windows.Forms.CheckBox();
            chk_HighPriority = new System.Windows.Forms.CheckBox();
            Chk_DSDebug = new System.Windows.Forms.CheckBox();
            Lbl_BlueStackRunning = new System.Windows.Forms.Label();
            Btn_Save = new System.Windows.Forms.Button();
            label11 = new System.Windows.Forms.Label();
            groupBox1 = new System.Windows.Forms.GroupBox();
            Chk_DetectionAPI = new System.Windows.Forms.CheckBox();
            Chk_FaceAPI = new System.Windows.Forms.CheckBox();
            Chk_SceneAPI = new System.Windows.Forms.CheckBox();
            groupBox2 = new System.Windows.Forms.GroupBox();
            RB_High = new System.Windows.Forms.RadioButton();
            RB_Medium = new System.Windows.Forms.RadioButton();
            RB_Low = new System.Windows.Forms.RadioButton();
            groupBox4 = new System.Windows.Forms.GroupBox();
            Txt_DeepStackInstallFolder = new System.Windows.Forms.TextBox();
            groupBox3 = new System.Windows.Forms.GroupBox();
            label27 = new System.Windows.Forms.Label();
            Txt_Port = new System.Windows.Forms.TextBox();
            Chk_AutoStart = new System.Windows.Forms.CheckBox();
            Btn_Start = new System.Windows.Forms.Button();
            Btn_Stop = new System.Windows.Forms.Button();
            groupBoxCustomModel = new System.Windows.Forms.GroupBox();
            Chk_CustomModelAPI = new System.Windows.Forms.CheckBox();
            label38 = new System.Windows.Forms.Label();
            label9 = new System.Windows.Forms.Label();
            label37 = new System.Windows.Forms.Label();
            label36 = new System.Windows.Forms.Label();
            label35 = new System.Windows.Forms.Label();
            Txt_CustomModelMode = new System.Windows.Forms.TextBox();
            Txt_CustomModelPort = new System.Windows.Forms.TextBox();
            Txt_CustomModelName = new System.Windows.Forms.TextBox();
            Txt_CustomModelPath = new System.Windows.Forms.TextBox();
            tabLog = new System.Windows.Forms.TabPage();
            toolStrip2 = new System.Windows.Forms.ToolStrip();
            toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            ToolStripComboBoxSearch = new System.Windows.Forms.ToolStripComboBox();
            toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            mnu_Filter = new System.Windows.Forms.ToolStripMenuItem();
            mnu_Highlight = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            toolStripDropDownButtonSettings = new System.Windows.Forms.ToolStripDropDownButton();
            Chk_AutoScroll = new System.Windows.Forms.ToolStripMenuItem();
            clearRecentErrorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItemLogLevel = new System.Windows.Forms.ToolStripMenuItem();
            mnu_log_filter_off = new System.Windows.Forms.ToolStripMenuItem();
            mnu_log_filter_fatal = new System.Windows.Forms.ToolStripMenuItem();
            mnu_log_filter_error = new System.Windows.Forms.ToolStripMenuItem();
            mnu_log_filter_warn = new System.Windows.Forms.ToolStripMenuItem();
            mnu_log_filter_info = new System.Windows.Forms.ToolStripMenuItem();
            mnu_log_filter_debug = new System.Windows.Forms.ToolStripMenuItem();
            mnu_log_filter_trace = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            openToolStripButton = new System.Windows.Forms.ToolStripButton();
            toolStripButtonLoad = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            toolStripButtonReload = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            toolStripButtonPauseLog = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            chk_filterErrors = new System.Windows.Forms.ToolStripButton();
            chk_filterErrorsAll = new System.Windows.Forms.ToolStripButton();
            groupBox7 = new System.Windows.Forms.GroupBox();
            folv_log = new BrightIdeasSoftware.FastObjectListView();
            toolTip1 = new System.Windows.Forms.ToolTip(components);
            HistoryImageList = new System.Windows.Forms.ImageList(components);
            HistoryUpdateListTimer = new System.Windows.Forms.Timer(components);
            statusStrip1 = new System.Windows.Forms.StatusStrip();
            toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            toolStripStatusLabelHistoryItems = new System.Windows.Forms.ToolStripStatusLabel();
            toolStripStatusErrors = new System.Windows.Forms.ToolStripStatusLabel();
            toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            toolStripStatusLabelInfo = new System.Windows.Forms.ToolStripStatusLabel();
            LogUpdateListTimer = new System.Windows.Forms.Timer(components);
            colorDialog1 = new System.Windows.Forms.ColorDialog();
            TraycontextMenuStrip.SuspendLayout();
            tabControl1.SuspendLayout();
            tabOverview.SuspendLayout();
            tableLayoutPanel14.SuspendLayout();
            tableLayoutPanel15.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            tabStats.SuspendLayout();
            tableLayoutPanel16.SuspendLayout();
            tableLayoutPanel23.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)chart_confidence).BeginInit();
            ((System.ComponentModel.ISupportInitialize)timeline).BeginInit();
            tableLayoutPanel17.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)chart1).BeginInit();
            tabHistory.SuspendLayout();
            toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            groupBox8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)folv_history).BeginInit();
            contextMenuStripHistory.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            toolStripContainer1.SuspendLayout();
            tabCameras.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer3).BeginInit();
            splitContainer3.Panel1.SuspendLayout();
            splitContainer3.Panel2.SuspendLayout();
            splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)FOLV_Cameras).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxCamera).BeginInit();
            tableLayoutPanel6.SuspendLayout();
            tableLayoutPanel11.SuspendLayout();
            tableLayoutPanel7.SuspendLayout();
            tableLayoutPanel12.SuspendLayout();
            tableLayoutPanel13.SuspendLayout();
            tableLayoutPanel26.SuspendLayout();
            tableLayoutPanel27.SuspendLayout();
            dbLayoutPanel6.SuspendLayout();
            dbLayoutPanel7.SuspendLayout();
            dbLayoutPanel11.SuspendLayout();
            dbLayoutPanel12.SuspendLayout();
            dbLayoutPanel13.SuspendLayout();
            tabSettings.SuspendLayout();
            tableLayoutPanel4.SuspendLayout();
            tableLayoutPanel5.SuspendLayout();
            tableLayoutPanel18.SuspendLayout();
            dbLayoutPanel1.SuspendLayout();
            dbLayoutPanel2.SuspendLayout();
            dbLayoutPanel3.SuspendLayout();
            dbLayoutPanel4.SuspendLayout();
            dbLayoutPanel5.SuspendLayout();
            dbLayoutPanel8.SuspendLayout();
            dbLayoutPanel9.SuspendLayout();
            dbLayoutPanel10.SuspendLayout();
            tabDeepStack.SuspendLayout();
            groupBox11.SuspendLayout();
            groupBox10.SuspendLayout();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox4.SuspendLayout();
            groupBox3.SuspendLayout();
            groupBoxCustomModel.SuspendLayout();
            tabLog.SuspendLayout();
            toolStrip2.SuspendLayout();
            groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)folv_log).BeginInit();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // notifyIcon
            // 
            notifyIcon.BalloonTipText = "Running in the background";
            notifyIcon.BalloonTipTitle = "AI Tool";
            notifyIcon.ContextMenuStrip = TraycontextMenuStrip;
            notifyIcon.Icon = (System.Drawing.Icon)resources.GetObject("notifyIcon.Icon");
            notifyIcon.Text = "AI Tool";
            notifyIcon.MouseClick += notifyIcon_MouseClick;
            notifyIcon.MouseDoubleClick += notifyIcon_MouseDoubleClick;
            // 
            // TraycontextMenuStrip
            // 
            TraycontextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { pauseAllToolStripMenuItem, resumeAllToolStripMenuItem, pauseToolStripMenuItem, exitToolStripMenuItem });
            TraycontextMenuStrip.Name = "TraycontextMenuStrip";
            TraycontextMenuStrip.Size = new System.Drawing.Size(134, 92);
            // 
            // pauseAllToolStripMenuItem
            // 
            pauseAllToolStripMenuItem.Name = "pauseAllToolStripMenuItem";
            pauseAllToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            pauseAllToolStripMenuItem.Text = "Pause All";
            pauseAllToolStripMenuItem.Click += pauseAllToolStripMenuItem_Click;
            // 
            // resumeAllToolStripMenuItem
            // 
            resumeAllToolStripMenuItem.Name = "resumeAllToolStripMenuItem";
            resumeAllToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            resumeAllToolStripMenuItem.Text = "Resume All";
            resumeAllToolStripMenuItem.Click += resumeAllToolStripMenuItem_Click;
            // 
            // pauseToolStripMenuItem
            // 
            pauseToolStripMenuItem.Name = "pauseToolStripMenuItem";
            pauseToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            pauseToolStripMenuItem.Text = "Pause...";
            pauseToolStripMenuItem.Click += pauseToolStripMenuItem_Click;
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // tabControl1
            // 
            tabControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tabControl1.Controls.Add(tabOverview);
            tabControl1.Controls.Add(tabStats);
            tabControl1.Controls.Add(tabHistory);
            tabControl1.Controls.Add(tabCameras);
            tabControl1.Controls.Add(tabSettings);
            tabControl1.Controls.Add(tabDeepStack);
            tabControl1.Controls.Add(tabLog);
            tabControl1.Location = new System.Drawing.Point(0, 2);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(1107, 520);
            tabControl1.TabIndex = 4;
            tabControl1.SelectedIndexChanged += tabControl1_SelectedIndexChanged;
            tabControl1.MouseDown += tabControl1_MouseDown;
            // 
            // tabOverview
            // 
            tabOverview.Controls.Add(tableLayoutPanel14);
            tabOverview.Location = new System.Drawing.Point(4, 22);
            tabOverview.Name = "tabOverview";
            tabOverview.Size = new System.Drawing.Size(1099, 494);
            tabOverview.TabIndex = 4;
            tabOverview.Text = "Overview";
            tabOverview.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel14
            // 
            tableLayoutPanel14.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tableLayoutPanel14.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            tableLayoutPanel14.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            tableLayoutPanel14.ColumnCount = 1;
            tableLayoutPanel14.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel14.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            tableLayoutPanel14.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            tableLayoutPanel14.Controls.Add(tableLayoutPanel15, 0, 0);
            tableLayoutPanel14.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel14.Name = "tableLayoutPanel14";
            tableLayoutPanel14.RowCount = 1;
            tableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel14.Size = new System.Drawing.Size(1096, 479);
            tableLayoutPanel14.TabIndex = 3;
            // 
            // tableLayoutPanel15
            // 
            tableLayoutPanel15.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tableLayoutPanel15.ColumnCount = 1;
            tableLayoutPanel15.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel15.Controls.Add(pictureBox2, 0, 0);
            tableLayoutPanel15.Controls.Add(label2, 0, 2);
            tableLayoutPanel15.Controls.Add(label3, 0, 1);
            tableLayoutPanel15.Controls.Add(lbl_version, 0, 5);
            tableLayoutPanel15.Controls.Add(lbl_errors, 0, 3);
            tableLayoutPanel15.Controls.Add(lbl_info, 0, 5);
            tableLayoutPanel15.Controls.Add(lblQueue, 0, 4);
            tableLayoutPanel15.Location = new System.Drawing.Point(4, 4);
            tableLayoutPanel15.Name = "tableLayoutPanel15";
            tableLayoutPanel15.RowCount = 6;
            tableLayoutPanel15.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 47.14285F));
            tableLayoutPanel15.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 0.9523811F));
            tableLayoutPanel15.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 28.57143F));
            tableLayoutPanel15.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 13.80951F));
            tableLayoutPanel15.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.761905F));
            tableLayoutPanel15.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.761905F));
            tableLayoutPanel15.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            tableLayoutPanel15.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            tableLayoutPanel15.Size = new System.Drawing.Size(1088, 471);
            tableLayoutPanel15.TabIndex = 0;
            // 
            // pictureBox2
            // 
            pictureBox2.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            pictureBox2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            pictureBox2.Image = (System.Drawing.Image)resources.GetObject("pictureBox2.Image");
            pictureBox2.Location = new System.Drawing.Point(3, 78);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new System.Drawing.Size(1082, 131);
            pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            pictureBox2.TabIndex = 4;
            pictureBox2.TabStop = false;
            // 
            // label2
            // 
            label2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            label2.AutoEllipsis = true;
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label2.ForeColor = System.Drawing.Color.Green;
            label2.Location = new System.Drawing.Point(3, 216);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(1082, 30);
            label2.TabIndex = 3;
            label2.Text = "Running";
            label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            label3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            label3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            label3.Location = new System.Drawing.Point(63, 212);
            label3.Margin = new System.Windows.Forms.Padding(63, 0, 63, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(962, 2);
            label3.TabIndex = 5;
            label3.Text = "label3";
            // 
            // lbl_version
            // 
            lbl_version.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lbl_version.Location = new System.Drawing.Point(3, 427);
            lbl_version.Name = "lbl_version";
            lbl_version.Size = new System.Drawing.Size(1082, 21);
            lbl_version.TabIndex = 6;
            lbl_version.Text = "Version 1.67 preview 7  (VorlonCD MOD)";
            lbl_version.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl_errors
            // 
            lbl_errors.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lbl_errors.Font = new System.Drawing.Font("Segoe UI Semibold", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            lbl_errors.ForeColor = System.Drawing.Color.FromArgb(192, 0, 0);
            lbl_errors.Location = new System.Drawing.Point(3, 344);
            lbl_errors.Name = "lbl_errors";
            lbl_errors.Size = new System.Drawing.Size(1082, 62);
            lbl_errors.TabIndex = 7;
            lbl_errors.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lbl_errors.Visible = false;
            lbl_errors.Click += lbl_errors_Click;
            // 
            // lbl_info
            // 
            lbl_info.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lbl_info.AutoSize = true;
            lbl_info.Location = new System.Drawing.Point(3, 448);
            lbl_info.Name = "lbl_info";
            lbl_info.Size = new System.Drawing.Size(1082, 23);
            lbl_info.TabIndex = 8;
            lbl_info.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblQueue
            // 
            lblQueue.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lblQueue.AutoSize = true;
            lblQueue.ForeColor = System.Drawing.Color.DarkOrange;
            lblQueue.Location = new System.Drawing.Point(3, 406);
            lblQueue.Name = "lblQueue";
            lblQueue.Size = new System.Drawing.Size(1082, 21);
            lblQueue.TabIndex = 9;
            lblQueue.Text = "Images in Queue: 0";
            lblQueue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabStats
            // 
            tabStats.Controls.Add(tableLayoutPanel16);
            tabStats.Location = new System.Drawing.Point(4, 24);
            tabStats.Name = "tabStats";
            tabStats.Size = new System.Drawing.Size(1099, 492);
            tabStats.TabIndex = 5;
            tabStats.Text = "Stats";
            tabStats.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel16
            // 
            tableLayoutPanel16.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tableLayoutPanel16.ColumnCount = 2;
            tableLayoutPanel16.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            tableLayoutPanel16.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            tableLayoutPanel16.Controls.Add(tableLayoutPanel23, 0, 0);
            tableLayoutPanel16.Controls.Add(tableLayoutPanel17, 0, 0);
            tableLayoutPanel16.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel16.Name = "tableLayoutPanel16";
            tableLayoutPanel16.RowCount = 1;
            tableLayoutPanel16.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel16.Size = new System.Drawing.Size(1096, 478);
            tableLayoutPanel16.TabIndex = 0;
            // 
            // tableLayoutPanel23
            // 
            tableLayoutPanel23.ColumnCount = 1;
            tableLayoutPanel23.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel23.Controls.Add(label8, 0, 2);
            tableLayoutPanel23.Controls.Add(chart_confidence, 0, 2);
            tableLayoutPanel23.Controls.Add(timeline, 0, 1);
            tableLayoutPanel23.Controls.Add(label7, 0, 0);
            tableLayoutPanel23.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel23.Location = new System.Drawing.Point(331, 3);
            tableLayoutPanel23.Name = "tableLayoutPanel23";
            tableLayoutPanel23.RowCount = 3;
            tableLayoutPanel23.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            tableLayoutPanel23.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel23.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            tableLayoutPanel23.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel23.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            tableLayoutPanel23.Size = new System.Drawing.Size(762, 472);
            tableLayoutPanel23.TabIndex = 7;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Dock = System.Windows.Forms.DockStyle.Top;
            label8.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label8.Location = new System.Drawing.Point(3, 239);
            label8.Margin = new System.Windows.Forms.Padding(3);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(756, 29);
            label8.TabIndex = 9;
            label8.Text = "Frequencies of alert result confidences";
            label8.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // chart_confidence
            // 
            chart_confidence.BackColor = System.Drawing.Color.Transparent;
            chart_confidence.BorderlineColor = System.Drawing.Color.Transparent;
            chartArea1.AxisX.Interval = 10D;
            chartArea1.AxisX.MajorGrid.Interval = 6D;
            chartArea1.AxisX.MajorGrid.LineColor = System.Drawing.Color.LightGray;
            chartArea1.AxisX.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            chartArea1.AxisX.MajorTickMark.Interval = 1D;
            chartArea1.AxisX.Maximum = 100D;
            chartArea1.AxisX.Minimum = 0D;
            chartArea1.AxisX.Title = "Alert confidence";
            chartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.LightGray;
            chartArea1.AxisY.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
            chartArea1.AxisY.Title = "Frequency";
            chartArea1.BackColor = System.Drawing.Color.Transparent;
            chartArea1.Name = "ChartArea1";
            chart_confidence.ChartAreas.Add(chartArea1);
            chart_confidence.Dock = System.Windows.Forms.DockStyle.Fill;
            chart_confidence.Location = new System.Drawing.Point(3, 274);
            chart_confidence.Name = "chart_confidence";
            series1.BorderWidth = 4;
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series1.Color = System.Drawing.Color.Orange;
            series1.Name = "no alert";
            series2.BorderWidth = 3;
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series2.Color = System.Drawing.Color.Green;
            series2.Legend = "Legend1";
            series2.Name = "alert";
            chart_confidence.Series.Add(series1);
            chart_confidence.Series.Add(series2);
            chart_confidence.Size = new System.Drawing.Size(756, 195);
            chart_confidence.TabIndex = 8;
            // 
            // timeline
            // 
            timeline.BackColor = System.Drawing.Color.Transparent;
            timeline.BorderlineColor = System.Drawing.Color.Transparent;
            chartArea2.AxisX.Interval = 3D;
            chartArea2.AxisX.MajorGrid.Interval = 6D;
            chartArea2.AxisX.MajorGrid.LineColor = System.Drawing.Color.LightGray;
            chartArea2.AxisX.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            chartArea2.AxisX.MajorTickMark.Interval = 1D;
            chartArea2.AxisX.Maximum = 24D;
            chartArea2.AxisX.Minimum = 0D;
            chartArea2.AxisY.MajorGrid.LineColor = System.Drawing.Color.LightGray;
            chartArea2.AxisY.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
            chartArea2.AxisY.Title = "Number";
            chartArea2.BackColor = System.Drawing.Color.Transparent;
            chartArea2.Name = "ChartArea1";
            timeline.ChartAreas.Add(chartArea2);
            timeline.Dock = System.Windows.Forms.DockStyle.Fill;
            timeline.Location = new System.Drawing.Point(3, 38);
            timeline.Name = "timeline";
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.SplineArea;
            series3.Color = System.Drawing.Color.Silver;
            series3.Legend = "Legend1";
            series3.Name = "all";
            series4.BorderWidth = 3;
            series4.ChartArea = "ChartArea1";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series4.Color = System.Drawing.Color.OrangeRed;
            series4.Legend = "Legend1";
            series4.Name = "falses";
            series5.BorderWidth = 3;
            series5.ChartArea = "ChartArea1";
            series5.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series5.Color = System.Drawing.Color.Orange;
            series5.Legend = "Legend1";
            series5.Name = "irrelevant";
            series6.BorderWidth = 4;
            series6.ChartArea = "ChartArea1";
            series6.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series6.Color = System.Drawing.Color.Green;
            series6.Legend = "Legend1";
            series6.Name = "relevant";
            series7.ChartArea = "ChartArea1";
            series7.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series7.Color = System.Drawing.Color.Purple;
            series7.Name = "skipped";
            timeline.Series.Add(series3);
            timeline.Series.Add(series4);
            timeline.Series.Add(series5);
            timeline.Series.Add(series6);
            timeline.Series.Add(series7);
            timeline.Size = new System.Drawing.Size(756, 195);
            timeline.TabIndex = 6;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Dock = System.Windows.Forms.DockStyle.Top;
            label7.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label7.Location = new System.Drawing.Point(3, 3);
            label7.Margin = new System.Windows.Forms.Padding(3);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(756, 29);
            label7.TabIndex = 0;
            label7.Text = "Timeline";
            label7.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // tableLayoutPanel17
            // 
            tableLayoutPanel17.ColumnCount = 1;
            tableLayoutPanel17.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel17.Controls.Add(chart1, 0, 2);
            tableLayoutPanel17.Controls.Add(comboBox1, 0, 0);
            tableLayoutPanel17.Controls.Add(btn_resetstats, 0, 1);
            tableLayoutPanel17.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel17.Location = new System.Drawing.Point(3, 3);
            tableLayoutPanel17.Name = "tableLayoutPanel17";
            tableLayoutPanel17.RowCount = 3;
            tableLayoutPanel17.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel17.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel17.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel17.Size = new System.Drawing.Size(322, 472);
            tableLayoutPanel17.TabIndex = 3;
            // 
            // chart1
            // 
            chart1.BackColor = System.Drawing.Color.Transparent;
            chart1.BorderlineColor = System.Drawing.Color.DimGray;
            chart1.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            chartArea3.Area3DStyle.Enable3D = true;
            chartArea3.Area3DStyle.IsRightAngleAxes = false;
            chartArea3.Area3DStyle.LightStyle = System.Windows.Forms.DataVisualization.Charting.LightStyle.Realistic;
            chartArea3.Area3DStyle.Perspective = 10;
            chartArea3.Area3DStyle.WallWidth = 6;
            chartArea3.AxisX.LabelAutoFitStyle = System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.IncreaseFont | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.DecreaseFont | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.StaggeredLabels | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.LabelsAngleStep30 | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.LabelsAngleStep45 | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.WordWrap;
            chartArea3.AxisY.LabelAutoFitStyle = System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.IncreaseFont | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.DecreaseFont | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.StaggeredLabels | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.LabelsAngleStep30 | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.LabelsAngleStep45 | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.LabelsAngleStep90 | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.WordWrap;
            chartArea3.BackColor = System.Drawing.Color.Transparent;
            chartArea3.Name = "ChartArea1";
            chart1.ChartAreas.Add(chartArea3);
            chart1.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Alignment = System.Drawing.StringAlignment.Center;
            legend1.BackColor = System.Drawing.Color.Transparent;
            legend1.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
            legend1.IsTextAutoFit = false;
            legend1.LegendStyle = System.Windows.Forms.DataVisualization.Charting.LegendStyle.Column;
            legend1.Name = "Legend1";
            legend1.Title = "Legend";
            chart1.Legends.Add(legend1);
            chart1.Location = new System.Drawing.Point(3, 55);
            chart1.Name = "chart1";
            series8.ChartArea = "ChartArea1";
            series8.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;
            series8.IsValueShownAsLabel = true;
            series8.Legend = "Legend1";
            series8.Name = "s1";
            dataPoint1.IsVisibleInLegend = true;
            series8.Points.Add(dataPoint1);
            series8.Points.Add(dataPoint2);
            series8.Points.Add(dataPoint3);
            chart1.Series.Add(series8);
            chart1.Size = new System.Drawing.Size(316, 414);
            chart1.TabIndex = 2;
            chart1.Text = "chart1";
            title1.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            title1.Name = "Title1";
            title1.Text = "Input Rates";
            chart1.Titles.Add(title1);
            // 
            // comboBox1
            // 
            comboBox1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new System.Drawing.Point(3, 3);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new System.Drawing.Size(316, 21);
            comboBox1.TabIndex = 3;
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged_1;
            comboBox1.SelectionChangeCommitted += comboBox1_SelectionChangeCommitted;
            // 
            // btn_resetstats
            // 
            btn_resetstats.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            btn_resetstats.Location = new System.Drawing.Point(2, 29);
            btn_resetstats.Margin = new System.Windows.Forms.Padding(2);
            btn_resetstats.Name = "btn_resetstats";
            btn_resetstats.Size = new System.Drawing.Size(318, 21);
            btn_resetstats.TabIndex = 4;
            btn_resetstats.Text = "Reset Stats";
            btn_resetstats.UseVisualStyleBackColor = true;
            btn_resetstats.Click += btn_resetstats_Click;
            // 
            // tabHistory
            // 
            tabHistory.Controls.Add(toolStrip1);
            tabHistory.Controls.Add(splitContainer2);
            tabHistory.Controls.Add(toolStripContainer1);
            tabHistory.Location = new System.Drawing.Point(4, 22);
            tabHistory.Name = "tabHistory";
            tabHistory.Padding = new System.Windows.Forms.Padding(3);
            tabHistory.Size = new System.Drawing.Size(1099, 494);
            tabHistory.TabIndex = 0;
            tabHistory.Text = "History";
            tabHistory.UseVisualStyleBackColor = true;
            // 
            // toolStrip1
            // 
            toolStrip1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            toolStrip1.AutoSize = false;
            toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            toolStrip1.ImageScalingSize = new System.Drawing.Size(28, 28);
            toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { comboBox_filter_camera, toolStripSeparator1, toolStripDropDownButtonFilters, toolStripDropDownButtonOptions, toolStripButtonDetails, toolStripButtonMaskDetails, toolStripButtonEditImageMask, toolStripSeparator9, toolStripButtonEditURL, toolStripButtonAdjustAnno });
            toolStrip1.Location = new System.Drawing.Point(6, 3);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            toolStrip1.Size = new System.Drawing.Size(1090, 37);
            toolStrip1.TabIndex = 5;
            toolStrip1.Text = "toolStrip1";
            // 
            // comboBox_filter_camera
            // 
            comboBox_filter_camera.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBox_filter_camera.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            comboBox_filter_camera.Name = "comboBox_filter_camera";
            comboBox_filter_camera.Size = new System.Drawing.Size(173, 37);
            comboBox_filter_camera.DropDownClosed += comboBox_filter_camera_DropDownClosed;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(6, 37);
            // 
            // toolStripDropDownButtonFilters
            // 
            toolStripDropDownButtonFilters.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { cb_filter_success, cb_filter_nosuccess, cb_filter_person, cb_filter_animal, cb_filter_vehicle, cb_filter_skipped, cb_filter_masked });
            toolStripDropDownButtonFilters.Image = (System.Drawing.Image)resources.GetObject("toolStripDropDownButtonFilters.Image");
            toolStripDropDownButtonFilters.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripDropDownButtonFilters.Name = "toolStripDropDownButtonFilters";
            toolStripDropDownButtonFilters.Size = new System.Drawing.Size(120, 34);
            toolStripDropDownButtonFilters.Text = "History Filters";
            // 
            // cb_filter_success
            // 
            cb_filter_success.CheckOnClick = true;
            cb_filter_success.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            cb_filter_success.Name = "cb_filter_success";
            cb_filter_success.Size = new System.Drawing.Size(188, 22);
            cb_filter_success.Text = "Only Relevant";
            cb_filter_success.Click += cb_filter_success_Click;
            // 
            // cb_filter_nosuccess
            // 
            cb_filter_nosuccess.CheckOnClick = true;
            cb_filter_nosuccess.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            cb_filter_nosuccess.Name = "cb_filter_nosuccess";
            cb_filter_nosuccess.Size = new System.Drawing.Size(188, 22);
            cb_filter_nosuccess.Text = "Only False / Irrelevant";
            cb_filter_nosuccess.Click += cb_filter_nosuccess_Click;
            // 
            // cb_filter_person
            // 
            cb_filter_person.CheckOnClick = true;
            cb_filter_person.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            cb_filter_person.Name = "cb_filter_person";
            cb_filter_person.Size = new System.Drawing.Size(188, 22);
            cb_filter_person.Text = "Only People";
            cb_filter_person.Click += cb_filter_person_Click;
            // 
            // cb_filter_animal
            // 
            cb_filter_animal.CheckOnClick = true;
            cb_filter_animal.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            cb_filter_animal.Name = "cb_filter_animal";
            cb_filter_animal.Size = new System.Drawing.Size(188, 22);
            cb_filter_animal.Text = "Only Animals";
            cb_filter_animal.Click += cb_filter_animal_Click;
            // 
            // cb_filter_vehicle
            // 
            cb_filter_vehicle.CheckOnClick = true;
            cb_filter_vehicle.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            cb_filter_vehicle.Name = "cb_filter_vehicle";
            cb_filter_vehicle.Size = new System.Drawing.Size(188, 22);
            cb_filter_vehicle.Text = "Only Vehicles";
            cb_filter_vehicle.Click += cb_filter_vehicle_Click;
            // 
            // cb_filter_skipped
            // 
            cb_filter_skipped.CheckOnClick = true;
            cb_filter_skipped.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            cb_filter_skipped.Name = "cb_filter_skipped";
            cb_filter_skipped.Size = new System.Drawing.Size(188, 22);
            cb_filter_skipped.Text = "Only Skipped";
            cb_filter_skipped.Click += cb_filter_skipped_Click;
            // 
            // cb_filter_masked
            // 
            cb_filter_masked.CheckOnClick = true;
            cb_filter_masked.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            cb_filter_masked.Name = "cb_filter_masked";
            cb_filter_masked.Size = new System.Drawing.Size(188, 22);
            cb_filter_masked.Text = "Only Masked";
            cb_filter_masked.Click += cb_filter_masked_Click;
            // 
            // toolStripDropDownButtonOptions
            // 
            toolStripDropDownButtonOptions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { cb_showMask, cb_showObjects, showOnlyRelevantObjectsToolStripMenuItem, cb_follow, automaticallyRefreshToolStripMenuItem, storeFalseAlertsToolStripMenuItem, storeMaskedAlertsToolStripMenuItem, restrictThresholdAtSourceToolStripMenuItem, mergeDuplicatePredictionsToolStripMenuItem });
            toolStripDropDownButtonOptions.Image = (System.Drawing.Image)resources.GetObject("toolStripDropDownButtonOptions.Image");
            toolStripDropDownButtonOptions.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripDropDownButtonOptions.Name = "toolStripDropDownButtonOptions";
            toolStripDropDownButtonOptions.Size = new System.Drawing.Size(131, 34);
            toolStripDropDownButtonOptions.Text = "History Settings";
            // 
            // cb_showMask
            // 
            cb_showMask.CheckOnClick = true;
            cb_showMask.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            cb_showMask.Name = "cb_showMask";
            cb_showMask.Size = new System.Drawing.Size(223, 22);
            cb_showMask.Text = "Show Mask";
            cb_showMask.Click += cb_showMask_Click;
            // 
            // cb_showObjects
            // 
            cb_showObjects.Checked = true;
            cb_showObjects.CheckOnClick = true;
            cb_showObjects.CheckState = System.Windows.Forms.CheckState.Checked;
            cb_showObjects.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            cb_showObjects.Name = "cb_showObjects";
            cb_showObjects.Size = new System.Drawing.Size(223, 22);
            cb_showObjects.Text = "Show Objects";
            cb_showObjects.CheckedChanged += cb_showObjects_CheckedChanged;
            cb_showObjects.Click += cb_showObjects_Click;
            // 
            // showOnlyRelevantObjectsToolStripMenuItem
            // 
            showOnlyRelevantObjectsToolStripMenuItem.CheckOnClick = true;
            showOnlyRelevantObjectsToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            showOnlyRelevantObjectsToolStripMenuItem.Name = "showOnlyRelevantObjectsToolStripMenuItem";
            showOnlyRelevantObjectsToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            showOnlyRelevantObjectsToolStripMenuItem.Text = "Show Only Relevant Objects";
            showOnlyRelevantObjectsToolStripMenuItem.ToolTipText = resources.GetString("showOnlyRelevantObjectsToolStripMenuItem.ToolTipText");
            showOnlyRelevantObjectsToolStripMenuItem.Click += showOnlyRelevantObjectsToolStripMenuItem_Click;
            // 
            // cb_follow
            // 
            cb_follow.CheckOnClick = true;
            cb_follow.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            cb_follow.Name = "cb_follow";
            cb_follow.Size = new System.Drawing.Size(223, 22);
            cb_follow.Text = "Follow History List";
            cb_follow.ToolTipText = "Automatically select the latest history item in the list for every update";
            cb_follow.Click += cb_follow_Click;
            // 
            // automaticallyRefreshToolStripMenuItem
            // 
            automaticallyRefreshToolStripMenuItem.Checked = true;
            automaticallyRefreshToolStripMenuItem.CheckOnClick = true;
            automaticallyRefreshToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            automaticallyRefreshToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            automaticallyRefreshToolStripMenuItem.Name = "automaticallyRefreshToolStripMenuItem";
            automaticallyRefreshToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            automaticallyRefreshToolStripMenuItem.Text = "Automatically Refresh";
            automaticallyRefreshToolStripMenuItem.Click += automaticallyRefreshToolStripMenuItem_Click;
            // 
            // storeFalseAlertsToolStripMenuItem
            // 
            storeFalseAlertsToolStripMenuItem.CheckOnClick = true;
            storeFalseAlertsToolStripMenuItem.DoubleClickEnabled = true;
            storeFalseAlertsToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            storeFalseAlertsToolStripMenuItem.Name = "storeFalseAlertsToolStripMenuItem";
            storeFalseAlertsToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            storeFalseAlertsToolStripMenuItem.Text = "Store False Alerts";
            storeFalseAlertsToolStripMenuItem.ToolTipText = resources.GetString("storeFalseAlertsToolStripMenuItem.ToolTipText");
            storeFalseAlertsToolStripMenuItem.Click += storeFalseAlertsToolStripMenuItem_Click;
            // 
            // storeMaskedAlertsToolStripMenuItem
            // 
            storeMaskedAlertsToolStripMenuItem.CheckOnClick = true;
            storeMaskedAlertsToolStripMenuItem.DoubleClickEnabled = true;
            storeMaskedAlertsToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            storeMaskedAlertsToolStripMenuItem.Name = "storeMaskedAlertsToolStripMenuItem";
            storeMaskedAlertsToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            storeMaskedAlertsToolStripMenuItem.Text = "Store Masked Alerts";
            storeMaskedAlertsToolStripMenuItem.ToolTipText = "If disabled the database will be smaller, leave enabled for better troubleshooting";
            storeMaskedAlertsToolStripMenuItem.Click += storeMaskedAlertsToolStripMenuItem_Click;
            // 
            // restrictThresholdAtSourceToolStripMenuItem
            // 
            restrictThresholdAtSourceToolStripMenuItem.CheckOnClick = true;
            restrictThresholdAtSourceToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            restrictThresholdAtSourceToolStripMenuItem.Name = "restrictThresholdAtSourceToolStripMenuItem";
            restrictThresholdAtSourceToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            restrictThresholdAtSourceToolStripMenuItem.Text = "Restrict Threshold at Source";
            restrictThresholdAtSourceToolStripMenuItem.ToolTipText = resources.GetString("restrictThresholdAtSourceToolStripMenuItem.ToolTipText");
            restrictThresholdAtSourceToolStripMenuItem.Click += restrictThresholdAtSourceToolStripMenuItem_Click;
            // 
            // mergeDuplicatePredictionsToolStripMenuItem
            // 
            mergeDuplicatePredictionsToolStripMenuItem.Name = "mergeDuplicatePredictionsToolStripMenuItem";
            mergeDuplicatePredictionsToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            mergeDuplicatePredictionsToolStripMenuItem.Text = "Merge Duplicate Predictions";
            mergeDuplicatePredictionsToolStripMenuItem.Click += mergeDuplicatePredictionsToolStripMenuItem_Click;
            // 
            // toolStripButtonDetails
            // 
            toolStripButtonDetails.Enabled = false;
            toolStripButtonDetails.Image = (System.Drawing.Image)resources.GetObject("toolStripButtonDetails.Image");
            toolStripButtonDetails.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButtonDetails.Name = "toolStripButtonDetails";
            toolStripButtonDetails.Size = new System.Drawing.Size(131, 34);
            toolStripButtonDetails.Text = "Prediction Details";
            toolStripButtonDetails.Click += toolStripButtonDetails_Click;
            // 
            // toolStripButtonMaskDetails
            // 
            toolStripButtonMaskDetails.Image = (System.Drawing.Image)resources.GetObject("toolStripButtonMaskDetails.Image");
            toolStripButtonMaskDetails.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButtonMaskDetails.Name = "toolStripButtonMaskDetails";
            toolStripButtonMaskDetails.Size = new System.Drawing.Size(155, 34);
            toolStripButtonMaskDetails.Text = "Dynamic Mask Details";
            toolStripButtonMaskDetails.Click += toolStripButtonMaskDetails_Click;
            // 
            // toolStripButtonEditImageMask
            // 
            toolStripButtonEditImageMask.Image = (System.Drawing.Image)resources.GetObject("toolStripButtonEditImageMask.Image");
            toolStripButtonEditImageMask.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButtonEditImageMask.Name = "toolStripButtonEditImageMask";
            toolStripButtonEditImageMask.Size = new System.Drawing.Size(126, 34);
            toolStripButtonEditImageMask.Text = "Edit Image Mask";
            toolStripButtonEditImageMask.Click += toolStripButtonEditImageMask_Click;
            // 
            // toolStripSeparator9
            // 
            toolStripSeparator9.Name = "toolStripSeparator9";
            toolStripSeparator9.Size = new System.Drawing.Size(6, 37);
            // 
            // toolStripButtonEditURL
            // 
            toolStripButtonEditURL.Image = (System.Drawing.Image)resources.GetObject("toolStripButtonEditURL.Image");
            toolStripButtonEditURL.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButtonEditURL.Name = "toolStripButtonEditURL";
            toolStripButtonEditURL.Size = new System.Drawing.Size(108, 34);
            toolStripButtonEditURL.Text = "Edit AI Server";
            toolStripButtonEditURL.Click += toolStripButtonEditURL_Click;
            // 
            // toolStripButtonAdjustAnno
            // 
            toolStripButtonAdjustAnno.Image = (System.Drawing.Image)resources.GetObject("toolStripButtonAdjustAnno.Image");
            toolStripButtonAdjustAnno.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButtonAdjustAnno.Name = "toolStripButtonAdjustAnno";
            toolStripButtonAdjustAnno.Size = new System.Drawing.Size(141, 32);
            toolStripButtonAdjustAnno.Text = "Adjust Annotations";
            toolStripButtonAdjustAnno.Click += toolStripButtonAdjustAnno_Click;
            // 
            // splitContainer2
            // 
            splitContainer2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            splitContainer2.Location = new System.Drawing.Point(2, 43);
            splitContainer2.Margin = new System.Windows.Forms.Padding(2);
            splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(groupBox8);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(lbl_objects);
            splitContainer2.Panel2.Controls.Add(pictureBox1);
            splitContainer2.Panel2MinSize = 300;
            splitContainer2.Size = new System.Drawing.Size(1097, 461);
            splitContainer2.SplitterDistance = 284;
            splitContainer2.SplitterWidth = 2;
            splitContainer2.TabIndex = 4;
            // 
            // groupBox8
            // 
            groupBox8.Controls.Add(folv_history);
            groupBox8.Dock = System.Windows.Forms.DockStyle.Fill;
            groupBox8.Location = new System.Drawing.Point(0, 0);
            groupBox8.Margin = new System.Windows.Forms.Padding(2);
            groupBox8.Name = "groupBox8";
            groupBox8.Padding = new System.Windows.Forms.Padding(2);
            groupBox8.Size = new System.Drawing.Size(280, 457);
            groupBox8.TabIndex = 11;
            groupBox8.TabStop = false;
            groupBox8.Text = "History";
            // 
            // folv_history
            // 
            folv_history.ContextMenuStrip = contextMenuStripHistory;
            folv_history.Dock = System.Windows.Forms.DockStyle.Fill;
            folv_history.Location = new System.Drawing.Point(2, 17);
            folv_history.Margin = new System.Windows.Forms.Padding(2);
            folv_history.Name = "folv_history";
            folv_history.ShowGroups = false;
            folv_history.Size = new System.Drawing.Size(276, 438);
            folv_history.TabIndex = 10;
            folv_history.UseCellFormatEvents = true;
            folv_history.UseCompatibleStateImageBehavior = false;
            folv_history.UseFiltering = true;
            folv_history.View = System.Windows.Forms.View.Details;
            folv_history.VirtualMode = true;
            folv_history.FormatRow += folv_history_FormatRow;
            folv_history.SelectionChanged += folv_history_SelectionChanged;
            folv_history.SelectedIndexChanged += folv_history_SelectedIndexChanged;
            folv_history.MouseDoubleClick += folv_history_MouseDoubleClick;
            // 
            // contextMenuStripHistory
            // 
            contextMenuStripHistory.ImageScalingSize = new System.Drawing.Size(24, 24);
            contextMenuStripHistory.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { testDetectionAgainToolStripMenuItem, detailsToolStripMenuItem, refreshToolStripMenuItem, dynamicMaskDetailsToolStripMenuItem, locateInLogToolStripMenuItem, manuallyAddImagesToolStripMenuItem, viewImageToolStripMenuItem, jumpToImageToolStripMenuItem });
            contextMenuStripHistory.Name = "contextMenuStripHistory";
            contextMenuStripHistory.Size = new System.Drawing.Size(191, 180);
            // 
            // testDetectionAgainToolStripMenuItem
            // 
            testDetectionAgainToolStripMenuItem.Name = "testDetectionAgainToolStripMenuItem";
            testDetectionAgainToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            testDetectionAgainToolStripMenuItem.Text = "Test Detection Again";
            testDetectionAgainToolStripMenuItem.Click += testDetectionAgainToolStripMenuItem_Click;
            // 
            // detailsToolStripMenuItem
            // 
            detailsToolStripMenuItem.Name = "detailsToolStripMenuItem";
            detailsToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            detailsToolStripMenuItem.Text = "Prediction Details";
            detailsToolStripMenuItem.Click += detailsToolStripMenuItem_Click;
            // 
            // refreshToolStripMenuItem
            // 
            refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            refreshToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            refreshToolStripMenuItem.Text = "Refresh";
            refreshToolStripMenuItem.Click += refreshToolStripMenuItem_Click;
            // 
            // dynamicMaskDetailsToolStripMenuItem
            // 
            dynamicMaskDetailsToolStripMenuItem.Name = "dynamicMaskDetailsToolStripMenuItem";
            dynamicMaskDetailsToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            dynamicMaskDetailsToolStripMenuItem.Text = "Dynamic Mask Details";
            dynamicMaskDetailsToolStripMenuItem.Click += dynamicMaskDetailsToolStripMenuItem_Click;
            // 
            // locateInLogToolStripMenuItem
            // 
            locateInLogToolStripMenuItem.Name = "locateInLogToolStripMenuItem";
            locateInLogToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            locateInLogToolStripMenuItem.Text = "Locate in log";
            locateInLogToolStripMenuItem.Click += locateInLogToolStripMenuItem_Click;
            // 
            // manuallyAddImagesToolStripMenuItem
            // 
            manuallyAddImagesToolStripMenuItem.Name = "manuallyAddImagesToolStripMenuItem";
            manuallyAddImagesToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            manuallyAddImagesToolStripMenuItem.Text = "Manually Add Images";
            manuallyAddImagesToolStripMenuItem.Click += manuallyAddImagesToolStripMenuItem_Click;
            // 
            // viewImageToolStripMenuItem
            // 
            viewImageToolStripMenuItem.Name = "viewImageToolStripMenuItem";
            viewImageToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            viewImageToolStripMenuItem.Text = "View Image";
            viewImageToolStripMenuItem.Click += viewImageToolStripMenuItem_Click;
            // 
            // jumpToImageToolStripMenuItem
            // 
            jumpToImageToolStripMenuItem.Name = "jumpToImageToolStripMenuItem";
            jumpToImageToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            jumpToImageToolStripMenuItem.Text = "Jump To Image";
            jumpToImageToolStripMenuItem.Click += jumpToImageToolStripMenuItem_Click;
            // 
            // lbl_objects
            // 
            lbl_objects.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lbl_objects.BackColor = System.Drawing.SystemColors.Info;
            lbl_objects.Location = new System.Drawing.Point(1, 0);
            lbl_objects.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            lbl_objects.Name = "lbl_objects";
            lbl_objects.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
            lbl_objects.Size = new System.Drawing.Size(883, 20);
            lbl_objects.TabIndex = 14;
            lbl_objects.Text = "No selection";
            lbl_objects.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            pictureBox1.Location = new System.Drawing.Point(4, 23);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new System.Drawing.Size(879, 431);
            pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 6;
            pictureBox1.TabStop = false;
            pictureBox1.Click += pictureBox1_Click;
            pictureBox1.Paint += pictureBox1_Paint;
            // 
            // toolStripContainer1
            // 
            toolStripContainer1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            toolStripContainer1.BottomToolStripPanelVisible = false;
            // 
            // toolStripContainer1.ContentPanel
            // 
            toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(1095, 11);
            toolStripContainer1.LeftToolStripPanelVisible = false;
            toolStripContainer1.Location = new System.Drawing.Point(4, 4);
            toolStripContainer1.Name = "toolStripContainer1";
            toolStripContainer1.RightToolStripPanelVisible = false;
            toolStripContainer1.Size = new System.Drawing.Size(1095, 36);
            toolStripContainer1.TabIndex = 6;
            toolStripContainer1.Text = "toolStripContainer1";
            // 
            // tabCameras
            // 
            tabCameras.BackColor = System.Drawing.Color.WhiteSmoke;
            tabCameras.Controls.Add(splitContainer1);
            tabCameras.Location = new System.Drawing.Point(4, 24);
            tabCameras.Name = "tabCameras";
            tabCameras.Padding = new System.Windows.Forms.Padding(3);
            tabCameras.Size = new System.Drawing.Size(1099, 492);
            tabCameras.TabIndex = 2;
            tabCameras.Text = "Cameras";
            // 
            // splitContainer1
            // 
            splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            splitContainer1.Location = new System.Drawing.Point(3, 3);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(splitContainer3);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(tableLayoutPanel6);
            splitContainer1.Size = new System.Drawing.Size(1093, 486);
            splitContainer1.SplitterDistance = 153;
            splitContainer1.TabIndex = 1;
            // 
            // splitContainer3
            // 
            splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer3.Location = new System.Drawing.Point(0, 0);
            splitContainer3.Name = "splitContainer3";
            splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            splitContainer3.Panel1.Controls.Add(FOLV_Cameras);
            // 
            // splitContainer3.Panel2
            // 
            splitContainer3.Panel2.Controls.Add(pictureBoxCamera);
            splitContainer3.Size = new System.Drawing.Size(149, 482);
            splitContainer3.SplitterDistance = 287;
            splitContainer3.TabIndex = 1;
            // 
            // FOLV_Cameras
            // 
            FOLV_Cameras.Dock = System.Windows.Forms.DockStyle.Fill;
            FOLV_Cameras.LargeImageList = CameraImageList;
            FOLV_Cameras.Location = new System.Drawing.Point(0, 0);
            FOLV_Cameras.Name = "FOLV_Cameras";
            FOLV_Cameras.ShowGroups = false;
            FOLV_Cameras.Size = new System.Drawing.Size(149, 287);
            FOLV_Cameras.TabIndex = 0;
            FOLV_Cameras.UseCompatibleStateImageBehavior = false;
            FOLV_Cameras.View = System.Windows.Forms.View.Details;
            FOLV_Cameras.VirtualMode = true;
            FOLV_Cameras.FormatRow += FOLV_Cameras_FormatRow;
            FOLV_Cameras.SelectionChanged += FOLV_Cameras_SelectionChanged;
            // 
            // CameraImageList
            // 
            CameraImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            CameraImageList.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("CameraImageList.ImageStream");
            CameraImageList.TransparentColor = System.Drawing.Color.Transparent;
            CameraImageList.Images.SetKeyName(0, "camera-webcam.ico");
            CameraImageList.Images.SetKeyName(1, "camera-webcam_add.ico");
            CameraImageList.Images.SetKeyName(2, "camera-webcam_delete.ico");
            // 
            // pictureBoxCamera
            // 
            pictureBoxCamera.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            pictureBoxCamera.Dock = System.Windows.Forms.DockStyle.Fill;
            pictureBoxCamera.Location = new System.Drawing.Point(0, 0);
            pictureBoxCamera.Name = "pictureBoxCamera";
            pictureBoxCamera.Size = new System.Drawing.Size(149, 191);
            pictureBoxCamera.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            pictureBoxCamera.TabIndex = 0;
            pictureBoxCamera.TabStop = false;
            // 
            // tableLayoutPanel6
            // 
            tableLayoutPanel6.BackColor = System.Drawing.Color.WhiteSmoke;
            tableLayoutPanel6.ColumnCount = 1;
            tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel6.Controls.Add(tableLayoutPanel11, 0, 2);
            tableLayoutPanel6.Controls.Add(lbl_camstats, 0, 0);
            tableLayoutPanel6.Controls.Add(tableLayoutPanel7, 0, 1);
            tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel6.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel6.Name = "tableLayoutPanel6";
            tableLayoutPanel6.RowCount = 3;
            tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.82557F));
            tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 93.17443F));
            tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            tableLayoutPanel6.Size = new System.Drawing.Size(932, 482);
            tableLayoutPanel6.TabIndex = 1;
            // 
            // tableLayoutPanel11
            // 
            tableLayoutPanel11.BackColor = System.Drawing.Color.Transparent;
            tableLayoutPanel11.ColumnCount = 5;
            tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.15411F));
            tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 21.46066F));
            tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 21.46066F));
            tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 21.46066F));
            tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 21.46391F));
            tableLayoutPanel11.Controls.Add(btnCameraAdd, 0, 0);
            tableLayoutPanel11.Controls.Add(btnCameraDel, 1, 0);
            tableLayoutPanel11.Controls.Add(btnSaveTo, 2, 0);
            tableLayoutPanel11.Controls.Add(btnCameraSave, 4, 0);
            tableLayoutPanel11.Controls.Add(btnPause, 3, 0);
            tableLayoutPanel11.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel11.Location = new System.Drawing.Point(3, 444);
            tableLayoutPanel11.Name = "tableLayoutPanel11";
            tableLayoutPanel11.RowCount = 1;
            tableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel11.Size = new System.Drawing.Size(926, 35);
            tableLayoutPanel11.TabIndex = 3;
            // 
            // btnCameraAdd
            // 
            btnCameraAdd.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom;
            btnCameraAdd.Image = Properties.Resources.camera_webcam_add;
            btnCameraAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            btnCameraAdd.Location = new System.Drawing.Point(30, 3);
            btnCameraAdd.Name = "btnCameraAdd";
            btnCameraAdd.Size = new System.Drawing.Size(70, 30);
            btnCameraAdd.TabIndex = 31;
            btnCameraAdd.Text = "Add";
            btnCameraAdd.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            btnCameraAdd.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            btnCameraAdd.UseVisualStyleBackColor = true;
            btnCameraAdd.Click += btnCameraAdd_Click;
            // 
            // btnCameraDel
            // 
            btnCameraDel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom;
            btnCameraDel.Image = Properties.Resources.camera_webcam_delete;
            btnCameraDel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            btnCameraDel.Location = new System.Drawing.Point(195, 3);
            btnCameraDel.Name = "btnCameraDel";
            btnCameraDel.Size = new System.Drawing.Size(70, 30);
            btnCameraDel.TabIndex = 32;
            btnCameraDel.Text = "Delete";
            btnCameraDel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            btnCameraDel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            btnCameraDel.UseVisualStyleBackColor = true;
            btnCameraDel.Click += btnCameraDel_Click;
            // 
            // btnSaveTo
            // 
            btnSaveTo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom;
            btnSaveTo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            btnSaveTo.Location = new System.Drawing.Point(393, 3);
            btnSaveTo.Name = "btnSaveTo";
            btnSaveTo.Size = new System.Drawing.Size(70, 30);
            btnSaveTo.TabIndex = 33;
            btnSaveTo.Text = "Apply to";
            btnSaveTo.UseVisualStyleBackColor = false;
            btnSaveTo.Click += btnSaveTo_Click;
            // 
            // btnCameraSave
            // 
            btnCameraSave.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom;
            btnCameraSave.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            btnCameraSave.Location = new System.Drawing.Point(790, 3);
            btnCameraSave.Name = "btnCameraSave";
            btnCameraSave.Size = new System.Drawing.Size(70, 30);
            btnCameraSave.TabIndex = 34;
            btnCameraSave.Text = "Save";
            btnCameraSave.UseVisualStyleBackColor = false;
            btnCameraSave.Click += btnCameraSave_Click_1;
            // 
            // btnPause
            // 
            btnPause.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom;
            btnPause.Location = new System.Drawing.Point(591, 3);
            btnPause.Name = "btnPause";
            btnPause.Size = new System.Drawing.Size(70, 30);
            btnPause.TabIndex = 35;
            btnPause.Text = "Pause...";
            btnPause.UseVisualStyleBackColor = true;
            btnPause.Click += btnPause_Click;
            // 
            // lbl_camstats
            // 
            lbl_camstats.Anchor = System.Windows.Forms.AnchorStyles.Left;
            lbl_camstats.AutoSize = true;
            lbl_camstats.Location = new System.Drawing.Point(3, 8);
            lbl_camstats.Name = "lbl_camstats";
            lbl_camstats.Size = new System.Drawing.Size(32, 13);
            lbl_camstats.TabIndex = 4;
            lbl_camstats.Text = "Stats";
            // 
            // tableLayoutPanel7
            // 
            tableLayoutPanel7.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tableLayoutPanel7.BackColor = System.Drawing.Color.White;
            tableLayoutPanel7.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            tableLayoutPanel7.ColumnCount = 2;
            tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 17.60259F));
            tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 82.39741F));
            tableLayoutPanel7.Controls.Add(label26, 0, 8);
            tableLayoutPanel7.Controls.Add(label14, 0, 3);
            tableLayoutPanel7.Controls.Add(label1, 0, 6);
            tableLayoutPanel7.Controls.Add(lblPrefix, 0, 2);
            tableLayoutPanel7.Controls.Add(lblName, 0, 0);
            tableLayoutPanel7.Controls.Add(tableLayoutPanel12, 1, 2);
            tableLayoutPanel7.Controls.Add(tableLayoutPanel13, 1, 0);
            tableLayoutPanel7.Controls.Add(lblRelevantObjects, 0, 4);
            tableLayoutPanel7.Controls.Add(tableLayoutPanel26, 1, 3);
            tableLayoutPanel7.Controls.Add(tableLayoutPanel27, 1, 7);
            tableLayoutPanel7.Controls.Add(label25, 0, 1);
            tableLayoutPanel7.Controls.Add(dbLayoutPanel6, 1, 1);
            tableLayoutPanel7.Controls.Add(dbLayoutPanel7, 1, 8);
            tableLayoutPanel7.Controls.Add(label39, 0, 5);
            tableLayoutPanel7.Controls.Add(dbLayoutPanel11, 1, 4);
            tableLayoutPanel7.Controls.Add(label15, 0, 7);
            tableLayoutPanel7.Controls.Add(dbLayoutPanel12, 1, 5);
            tableLayoutPanel7.Controls.Add(dbLayoutPanel13, 1, 6);
            tableLayoutPanel7.Location = new System.Drawing.Point(3, 33);
            tableLayoutPanel7.Name = "tableLayoutPanel7";
            tableLayoutPanel7.RowCount = 10;
            tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.10862F));
            tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11363F));
            tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.10915F));
            tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.10915F));
            tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.10536F));
            tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11724F));
            tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.1134F));
            tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11097F));
            tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.1125F));
            tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            tableLayoutPanel7.Size = new System.Drawing.Size(926, 405);
            tableLayoutPanel7.TabIndex = 2;
            tableLayoutPanel7.Paint += tableLayoutPanel7_Paint;
            // 
            // label26
            // 
            label26.Anchor = System.Windows.Forms.AnchorStyles.Right;
            label26.AutoSize = true;
            label26.Location = new System.Drawing.Point(64, 351);
            label26.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label26.Name = "label26";
            label26.Size = new System.Drawing.Size(97, 13);
            label26.TabIndex = 19;
            label26.Text = "Custom Mask File";
            label26.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label14
            // 
            label14.Anchor = System.Windows.Forms.AnchorStyles.Right;
            label14.AutoSize = true;
            label14.Location = new System.Drawing.Point(89, 141);
            label14.Name = "label14";
            label14.Size = new System.Drawing.Size(71, 13);
            label14.TabIndex = 17;
            label14.Text = "Input Folder";
            // 
            // label1
            // 
            label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(115, 267);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(45, 13);
            label1.TabIndex = 9;
            label1.Text = "Actions";
            // 
            // lblPrefix
            // 
            lblPrefix.Anchor = System.Windows.Forms.AnchorStyles.Right;
            lblPrefix.AutoSize = true;
            lblPrefix.Location = new System.Drawing.Point(42, 99);
            lblPrefix.Name = "lblPrefix";
            lblPrefix.Size = new System.Drawing.Size(118, 13);
            lblPrefix.TabIndex = 2;
            lblPrefix.Text = "Input file begins with";
            lblPrefix.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblName
            // 
            lblName.Anchor = System.Windows.Forms.AnchorStyles.Right;
            lblName.AutoSize = true;
            lblName.Location = new System.Drawing.Point(45, 15);
            lblName.Name = "lblName";
            lblName.Size = new System.Drawing.Size(115, 13);
            lblName.TabIndex = 10;
            lblName.Text = "AI Tool Camera Name";
            // 
            // tableLayoutPanel12
            // 
            tableLayoutPanel12.ColumnCount = 2;
            tableLayoutPanel12.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel12.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel12.Controls.Add(lbl_prefix, 1, 0);
            tableLayoutPanel12.Controls.Add(tbPrefix, 0, 0);
            tableLayoutPanel12.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel12.Location = new System.Drawing.Point(166, 86);
            tableLayoutPanel12.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            tableLayoutPanel12.Name = "tableLayoutPanel12";
            tableLayoutPanel12.RowCount = 1;
            tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel12.Size = new System.Drawing.Size(757, 39);
            tableLayoutPanel12.TabIndex = 12;
            // 
            // lbl_prefix
            // 
            lbl_prefix.Anchor = System.Windows.Forms.AnchorStyles.None;
            lbl_prefix.AutoSize = true;
            lbl_prefix.Location = new System.Drawing.Point(567, 13);
            lbl_prefix.Name = "lbl_prefix";
            lbl_prefix.Size = new System.Drawing.Size(0, 13);
            lbl_prefix.TabIndex = 6;
            // 
            // tbPrefix
            // 
            tbPrefix.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tbPrefix.Location = new System.Drawing.Point(21, 8);
            tbPrefix.Margin = new System.Windows.Forms.Padding(21, 3, 21, 3);
            tbPrefix.Name = "tbPrefix";
            tbPrefix.Size = new System.Drawing.Size(336, 22);
            tbPrefix.TabIndex = 3;
            tbPrefix.TextChanged += tbPrefix_TextChanged;
            // 
            // tableLayoutPanel13
            // 
            tableLayoutPanel13.ColumnCount = 2;
            tableLayoutPanel13.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel13.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel13.Controls.Add(cb_enabled, 1, 0);
            tableLayoutPanel13.Controls.Add(tbName, 0, 0);
            tableLayoutPanel13.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel13.Location = new System.Drawing.Point(167, 2);
            tableLayoutPanel13.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            tableLayoutPanel13.Name = "tableLayoutPanel13";
            tableLayoutPanel13.RowCount = 1;
            tableLayoutPanel13.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel13.Size = new System.Drawing.Size(755, 39);
            tableLayoutPanel13.TabIndex = 13;
            // 
            // cb_enabled
            // 
            cb_enabled.Anchor = System.Windows.Forms.AnchorStyles.Left;
            cb_enabled.AutoSize = true;
            cb_enabled.Location = new System.Drawing.Point(398, 11);
            cb_enabled.Margin = new System.Windows.Forms.Padding(21, 3, 3, 3);
            cb_enabled.Name = "cb_enabled";
            cb_enabled.Size = new System.Drawing.Size(206, 17);
            cb_enabled.TabIndex = 1;
            cb_enabled.Text = "Enable AI Detection for this camera";
            cb_enabled.UseVisualStyleBackColor = true;
            // 
            // tbName
            // 
            tbName.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tbName.Location = new System.Drawing.Point(21, 8);
            tbName.Margin = new System.Windows.Forms.Padding(21, 3, 21, 3);
            tbName.Name = "tbName";
            tbName.Size = new System.Drawing.Size(335, 22);
            tbName.TabIndex = 0;
            // 
            // lblRelevantObjects
            // 
            lblRelevantObjects.Anchor = System.Windows.Forms.AnchorStyles.Right;
            lblRelevantObjects.AutoSize = true;
            lblRelevantObjects.Location = new System.Drawing.Point(67, 183);
            lblRelevantObjects.Name = "lblRelevantObjects";
            lblRelevantObjects.Size = new System.Drawing.Size(93, 13);
            lblRelevantObjects.TabIndex = 1;
            lblRelevantObjects.Text = "Relevant Objects";
            // 
            // tableLayoutPanel26
            // 
            tableLayoutPanel26.ColumnCount = 3;
            tableLayoutPanel26.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 62.5F));
            tableLayoutPanel26.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            tableLayoutPanel26.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            tableLayoutPanel26.Controls.Add(cmbcaminput, 0, 0);
            tableLayoutPanel26.Controls.Add(cb_monitorCamInputfolder, 1, 0);
            tableLayoutPanel26.Controls.Add(button2, 2, 0);
            tableLayoutPanel26.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel26.Location = new System.Drawing.Point(166, 128);
            tableLayoutPanel26.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            tableLayoutPanel26.Name = "tableLayoutPanel26";
            tableLayoutPanel26.RowCount = 1;
            tableLayoutPanel26.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel26.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 39F));
            tableLayoutPanel26.Size = new System.Drawing.Size(757, 39);
            tableLayoutPanel26.TabIndex = 18;
            // 
            // cmbcaminput
            // 
            cmbcaminput.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            cmbcaminput.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            cmbcaminput.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            cmbcaminput.FormattingEnabled = true;
            cmbcaminput.Location = new System.Drawing.Point(21, 8);
            cmbcaminput.Margin = new System.Windows.Forms.Padding(21, 2, 21, 2);
            cmbcaminput.Name = "cmbcaminput";
            cmbcaminput.Size = new System.Drawing.Size(431, 21);
            cmbcaminput.TabIndex = 4;
            // 
            // cb_monitorCamInputfolder
            // 
            cb_monitorCamInputfolder.Anchor = System.Windows.Forms.AnchorStyles.None;
            cb_monitorCamInputfolder.AutoSize = true;
            cb_monitorCamInputfolder.Location = new System.Drawing.Point(504, 11);
            cb_monitorCamInputfolder.Margin = new System.Windows.Forms.Padding(2);
            cb_monitorCamInputfolder.Name = "cb_monitorCamInputfolder";
            cb_monitorCamInputfolder.Size = new System.Drawing.Size(127, 17);
            cb_monitorCamInputfolder.TabIndex = 5;
            cb_monitorCamInputfolder.Text = "Monitor Subfolders";
            cb_monitorCamInputfolder.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            button2.Anchor = System.Windows.Forms.AnchorStyles.None;
            button2.Location = new System.Drawing.Point(674, 9);
            button2.Name = "button2";
            button2.Size = new System.Drawing.Size(70, 21);
            button2.TabIndex = 6;
            button2.Text = "Select...";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // tableLayoutPanel27
            // 
            tableLayoutPanel27.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tableLayoutPanel27.ColumnCount = 5;
            tableLayoutPanel27.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 29.63504F));
            tableLayoutPanel27.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.54895F));
            tableLayoutPanel27.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.23776F));
            tableLayoutPanel27.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.8986F));
            tableLayoutPanel27.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30.94406F));
            tableLayoutPanel27.Controls.Add(cb_masking_enabled, 0, 0);
            tableLayoutPanel27.Controls.Add(BtnDynamicMaskingSettings, 1, 0);
            tableLayoutPanel27.Controls.Add(btnDetails, 2, 0);
            tableLayoutPanel27.Controls.Add(btnCustomMask, 4, 0);
            tableLayoutPanel27.Controls.Add(lblDrawMask, 3, 0);
            tableLayoutPanel27.Location = new System.Drawing.Point(166, 296);
            tableLayoutPanel27.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            tableLayoutPanel27.Name = "tableLayoutPanel27";
            tableLayoutPanel27.RowCount = 1;
            tableLayoutPanel27.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel27.Size = new System.Drawing.Size(757, 39);
            tableLayoutPanel27.TabIndex = 20;
            // 
            // cb_masking_enabled
            // 
            cb_masking_enabled.Anchor = System.Windows.Forms.AnchorStyles.Left;
            cb_masking_enabled.AutoSize = true;
            cb_masking_enabled.Location = new System.Drawing.Point(21, 12);
            cb_masking_enabled.Margin = new System.Windows.Forms.Padding(21, 3, 5, 0);
            cb_masking_enabled.Name = "cb_masking_enabled";
            cb_masking_enabled.Size = new System.Drawing.Size(152, 17);
            cb_masking_enabled.TabIndex = 25;
            cb_masking_enabled.Text = "Enable dynamic masking";
            cb_masking_enabled.UseVisualStyleBackColor = true;
            // 
            // BtnDynamicMaskingSettings
            // 
            BtnDynamicMaskingSettings.Anchor = System.Windows.Forms.AnchorStyles.Left;
            BtnDynamicMaskingSettings.Location = new System.Drawing.Point(228, 7);
            BtnDynamicMaskingSettings.Margin = new System.Windows.Forms.Padding(5, 1, 5, 1);
            BtnDynamicMaskingSettings.Name = "BtnDynamicMaskingSettings";
            BtnDynamicMaskingSettings.Size = new System.Drawing.Size(70, 25);
            BtnDynamicMaskingSettings.TabIndex = 26;
            BtnDynamicMaskingSettings.Text = "Settings";
            BtnDynamicMaskingSettings.UseVisualStyleBackColor = true;
            BtnDynamicMaskingSettings.Click += BtnDynamicMaskingSettings_Click;
            // 
            // btnDetails
            // 
            btnDetails.Anchor = System.Windows.Forms.AnchorStyles.Left;
            btnDetails.Location = new System.Drawing.Point(330, 8);
            btnDetails.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
            btnDetails.Name = "btnDetails";
            btnDetails.Size = new System.Drawing.Size(70, 23);
            btnDetails.TabIndex = 27;
            btnDetails.Text = "Details";
            btnDetails.UseVisualStyleBackColor = true;
            btnDetails.Click += btnDetails_Click;
            // 
            // btnCustomMask
            // 
            btnCustomMask.Anchor = System.Windows.Forms.AnchorStyles.Left;
            btnCustomMask.Location = new System.Drawing.Point(522, 8);
            btnCustomMask.Margin = new System.Windows.Forms.Padding(1, 2, 5, 2);
            btnCustomMask.Name = "btnCustomMask";
            btnCustomMask.Size = new System.Drawing.Size(70, 23);
            btnCustomMask.TabIndex = 28;
            btnCustomMask.Text = "Custom";
            btnCustomMask.UseVisualStyleBackColor = true;
            btnCustomMask.Click += btnCustomMask_Click;
            // 
            // lblDrawMask
            // 
            lblDrawMask.Anchor = System.Windows.Forms.AnchorStyles.Right;
            lblDrawMask.Location = new System.Drawing.Point(454, 11);
            lblDrawMask.Margin = new System.Windows.Forms.Padding(0);
            lblDrawMask.Name = "lblDrawMask";
            lblDrawMask.Size = new System.Drawing.Size(67, 16);
            lblDrawMask.TabIndex = 25;
            lblDrawMask.Text = "Draw Mask";
            lblDrawMask.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label25
            // 
            label25.Anchor = System.Windows.Forms.AnchorStyles.Right;
            label25.AutoSize = true;
            label25.Location = new System.Drawing.Point(71, 57);
            label25.Name = "label25";
            label25.Size = new System.Drawing.Size(89, 13);
            label25.TabIndex = 10;
            label25.Text = "BI Camera Name";
            // 
            // dbLayoutPanel6
            // 
            dbLayoutPanel6.ColumnCount = 2;
            dbLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            dbLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            dbLayoutPanel6.Controls.Add(tbBiCamName, 0, 0);
            dbLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            dbLayoutPanel6.Location = new System.Drawing.Point(167, 44);
            dbLayoutPanel6.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            dbLayoutPanel6.Name = "dbLayoutPanel6";
            dbLayoutPanel6.RowCount = 1;
            dbLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            dbLayoutPanel6.Size = new System.Drawing.Size(755, 39);
            dbLayoutPanel6.TabIndex = 24;
            // 
            // tbBiCamName
            // 
            tbBiCamName.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tbBiCamName.Location = new System.Drawing.Point(21, 8);
            tbBiCamName.Margin = new System.Windows.Forms.Padding(21, 3, 21, 3);
            tbBiCamName.Name = "tbBiCamName";
            tbBiCamName.Size = new System.Drawing.Size(335, 22);
            tbBiCamName.TabIndex = 2;
            // 
            // dbLayoutPanel7
            // 
            dbLayoutPanel7.ColumnCount = 3;
            dbLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 42.62048F));
            dbLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.86747F));
            dbLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40.51205F));
            dbLayoutPanel7.Controls.Add(tb_camera_telegram_chatid, 2, 0);
            dbLayoutPanel7.Controls.Add(tbCustomMaskFile, 0, 0);
            dbLayoutPanel7.Controls.Add(label21, 1, 0);
            dbLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
            dbLayoutPanel7.Location = new System.Drawing.Point(167, 338);
            dbLayoutPanel7.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            dbLayoutPanel7.Name = "dbLayoutPanel7";
            dbLayoutPanel7.RowCount = 1;
            dbLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            dbLayoutPanel7.Size = new System.Drawing.Size(755, 39);
            dbLayoutPanel7.TabIndex = 25;
            // 
            // tb_camera_telegram_chatid
            // 
            tb_camera_telegram_chatid.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tb_camera_telegram_chatid.Location = new System.Drawing.Point(451, 8);
            tb_camera_telegram_chatid.Name = "tb_camera_telegram_chatid";
            tb_camera_telegram_chatid.Size = new System.Drawing.Size(301, 22);
            tb_camera_telegram_chatid.TabIndex = 30;
            toolTip1.SetToolTip(tb_camera_telegram_chatid, "This overrides the chatid in the settings tab.");
            // 
            // tbCustomMaskFile
            // 
            tbCustomMaskFile.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tbCustomMaskFile.Location = new System.Drawing.Point(3, 8);
            tbCustomMaskFile.Name = "tbCustomMaskFile";
            tbCustomMaskFile.Size = new System.Drawing.Size(315, 22);
            tbCustomMaskFile.TabIndex = 29;
            // 
            // label21
            // 
            label21.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            label21.Location = new System.Drawing.Point(323, 10);
            label21.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label21.Name = "label21";
            label21.Size = new System.Drawing.Size(123, 19);
            label21.TabIndex = 19;
            label21.Text = "Telegram Chat ID Override";
            label21.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label39
            // 
            label39.Anchor = System.Windows.Forms.AnchorStyles.Right;
            label39.AutoSize = true;
            label39.Location = new System.Drawing.Point(41, 225);
            label39.Name = "label39";
            label39.Size = new System.Drawing.Size(119, 13);
            label39.TabIndex = 15;
            label39.Text = "Prediction Tolerances:";
            // 
            // dbLayoutPanel11
            // 
            dbLayoutPanel11.ColumnCount = 2;
            dbLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15.50725F));
            dbLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 84.49275F));
            dbLayoutPanel11.Controls.Add(BtnRelevantObjects, 0, 0);
            dbLayoutPanel11.Controls.Add(lbl_RelevantObjects, 1, 0);
            dbLayoutPanel11.Dock = System.Windows.Forms.DockStyle.Fill;
            dbLayoutPanel11.Location = new System.Drawing.Point(167, 172);
            dbLayoutPanel11.Name = "dbLayoutPanel11";
            dbLayoutPanel11.RowCount = 1;
            dbLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            dbLayoutPanel11.Size = new System.Drawing.Size(755, 35);
            dbLayoutPanel11.TabIndex = 26;
            // 
            // BtnRelevantObjects
            // 
            BtnRelevantObjects.Anchor = System.Windows.Forms.AnchorStyles.Left;
            BtnRelevantObjects.Location = new System.Drawing.Point(21, 5);
            BtnRelevantObjects.Margin = new System.Windows.Forms.Padding(21, 2, 2, 2);
            BtnRelevantObjects.Name = "BtnRelevantObjects";
            BtnRelevantObjects.Size = new System.Drawing.Size(70, 25);
            BtnRelevantObjects.TabIndex = 23;
            BtnRelevantObjects.Text = "Settings";
            BtnRelevantObjects.UseVisualStyleBackColor = true;
            BtnRelevantObjects.Click += BtnRelevantObjects_Click;
            // 
            // lbl_RelevantObjects
            // 
            lbl_RelevantObjects.Anchor = System.Windows.Forms.AnchorStyles.Left;
            lbl_RelevantObjects.AutoSize = true;
            lbl_RelevantObjects.Location = new System.Drawing.Point(120, 4);
            lbl_RelevantObjects.Name = "lbl_RelevantObjects";
            lbl_RelevantObjects.Size = new System.Drawing.Size(632, 26);
            lbl_RelevantObjects.TabIndex = 24;
            lbl_RelevantObjects.Text = "person, face, bear, elephant, car, truck, pickup truck, SUV, van, bicycle, motorcycle, bus, dog, horse, kangaroo, boat, train, airplane, zebra, giraffe, cow, sheep, cat, bird";
            // 
            // label15
            // 
            label15.Anchor = System.Windows.Forms.AnchorStyles.Right;
            label15.AutoSize = true;
            label15.Location = new System.Drawing.Point(110, 309);
            label15.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label15.Name = "label15";
            label15.Size = new System.Drawing.Size(51, 13);
            label15.TabIndex = 19;
            label15.Text = "Masking";
            label15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dbLayoutPanel12
            // 
            dbLayoutPanel12.ColumnCount = 2;
            dbLayoutPanel12.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15.36232F));
            dbLayoutPanel12.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 84.63768F));
            dbLayoutPanel12.Controls.Add(Lbl_PredictionTolerances, 1, 0);
            dbLayoutPanel12.Controls.Add(BtnPredictionSize, 0, 0);
            dbLayoutPanel12.Dock = System.Windows.Forms.DockStyle.Fill;
            dbLayoutPanel12.Location = new System.Drawing.Point(167, 214);
            dbLayoutPanel12.Name = "dbLayoutPanel12";
            dbLayoutPanel12.RowCount = 1;
            dbLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            dbLayoutPanel12.Size = new System.Drawing.Size(755, 35);
            dbLayoutPanel12.TabIndex = 27;
            // 
            // Lbl_PredictionTolerances
            // 
            Lbl_PredictionTolerances.Anchor = System.Windows.Forms.AnchorStyles.Left;
            Lbl_PredictionTolerances.AutoSize = true;
            Lbl_PredictionTolerances.Location = new System.Drawing.Point(118, 11);
            Lbl_PredictionTolerances.Name = "Lbl_PredictionTolerances";
            Lbl_PredictionTolerances.Size = new System.Drawing.Size(116, 13);
            Lbl_PredictionTolerances.TabIndex = 24;
            Lbl_PredictionTolerances.Text = "high low percent, etc";
            // 
            // BtnPredictionSize
            // 
            BtnPredictionSize.Anchor = System.Windows.Forms.AnchorStyles.Left;
            BtnPredictionSize.Location = new System.Drawing.Point(21, 5);
            BtnPredictionSize.Margin = new System.Windows.Forms.Padding(21, 2, 2, 2);
            BtnPredictionSize.Name = "BtnPredictionSize";
            BtnPredictionSize.Size = new System.Drawing.Size(70, 25);
            BtnPredictionSize.TabIndex = 23;
            BtnPredictionSize.Text = "Settings";
            BtnPredictionSize.UseVisualStyleBackColor = true;
            BtnPredictionSize.Click += BtnPredictionSize_Click;
            // 
            // dbLayoutPanel13
            // 
            dbLayoutPanel13.ColumnCount = 2;
            dbLayoutPanel13.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15.50725F));
            dbLayoutPanel13.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 84.49275F));
            dbLayoutPanel13.Controls.Add(Lbl_Actions, 1, 0);
            dbLayoutPanel13.Controls.Add(btnActions, 0, 0);
            dbLayoutPanel13.Dock = System.Windows.Forms.DockStyle.Fill;
            dbLayoutPanel13.Location = new System.Drawing.Point(167, 256);
            dbLayoutPanel13.Name = "dbLayoutPanel13";
            dbLayoutPanel13.RowCount = 1;
            dbLayoutPanel13.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            dbLayoutPanel13.Size = new System.Drawing.Size(755, 35);
            dbLayoutPanel13.TabIndex = 28;
            // 
            // Lbl_Actions
            // 
            Lbl_Actions.Anchor = System.Windows.Forms.AnchorStyles.Left;
            Lbl_Actions.AutoSize = true;
            Lbl_Actions.Location = new System.Drawing.Point(120, 11);
            Lbl_Actions.Name = "Lbl_Actions";
            Lbl_Actions.Size = new System.Drawing.Size(153, 13);
            Lbl_Actions.TabIndex = 24;
            Lbl_Actions.Text = "Telegram, Pushover, URL, etc";
            // 
            // btnActions
            // 
            btnActions.Anchor = System.Windows.Forms.AnchorStyles.Left;
            btnActions.Location = new System.Drawing.Point(21, 5);
            btnActions.Margin = new System.Windows.Forms.Padding(21, 2, 2, 2);
            btnActions.Name = "btnActions";
            btnActions.Size = new System.Drawing.Size(70, 25);
            btnActions.TabIndex = 24;
            btnActions.Text = "Settings";
            btnActions.UseVisualStyleBackColor = true;
            btnActions.Click += btnActions_Click_1;
            // 
            // tabSettings
            // 
            tabSettings.Controls.Add(tableLayoutPanel4);
            tabSettings.Location = new System.Drawing.Point(4, 24);
            tabSettings.Name = "tabSettings";
            tabSettings.Size = new System.Drawing.Size(1099, 492);
            tabSettings.TabIndex = 3;
            tabSettings.Text = "Settings";
            tabSettings.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel4
            // 
            tableLayoutPanel4.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tableLayoutPanel4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            tableLayoutPanel4.BackColor = System.Drawing.Color.WhiteSmoke;
            tableLayoutPanel4.ColumnCount = 1;
            tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel4.Controls.Add(tableLayoutPanel5, 0, 0);
            tableLayoutPanel4.Controls.Add(dbLayoutPanel10, 0, 1);
            tableLayoutPanel4.Location = new System.Drawing.Point(3, 3);
            tableLayoutPanel4.Name = "tableLayoutPanel4";
            tableLayoutPanel4.RowCount = 2;
            tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            tableLayoutPanel4.Size = new System.Drawing.Size(1093, 473);
            tableLayoutPanel4.TabIndex = 5;
            // 
            // tableLayoutPanel5
            // 
            tableLayoutPanel5.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tableLayoutPanel5.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            tableLayoutPanel5.BackColor = System.Drawing.Color.White;
            tableLayoutPanel5.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            tableLayoutPanel5.ColumnCount = 2;
            tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 85F));
            tableLayoutPanel5.Controls.Add(lbl_input, 0, 0);
            tableLayoutPanel5.Controls.Add(lbl_telegram_token, 0, 2);
            tableLayoutPanel5.Controls.Add(tableLayoutPanel18, 1, 0);
            tableLayoutPanel5.Controls.Add(lbl_deepstackurl, 0, 1);
            tableLayoutPanel5.Controls.Add(dbLayoutPanel1, 1, 1);
            tableLayoutPanel5.Controls.Add(dbLayoutPanel2, 1, 2);
            tableLayoutPanel5.Controls.Add(label12, 0, 4);
            tableLayoutPanel5.Controls.Add(dbLayoutPanel3, 1, 4);
            tableLayoutPanel5.Controls.Add(label4, 0, 6);
            tableLayoutPanel5.Controls.Add(dbLayoutPanel4, 1, 6);
            tableLayoutPanel5.Controls.Add(label18, 0, 7);
            tableLayoutPanel5.Controls.Add(dbLayoutPanel5, 1, 7);
            tableLayoutPanel5.Controls.Add(label29, 0, 3);
            tableLayoutPanel5.Controls.Add(dbLayoutPanel8, 1, 3);
            tableLayoutPanel5.Controls.Add(dbLayoutPanel9, 1, 5);
            tableLayoutPanel5.Controls.Add(label13, 0, 5);
            tableLayoutPanel5.Location = new System.Drawing.Point(3, 3);
            tableLayoutPanel5.Name = "tableLayoutPanel5";
            tableLayoutPanel5.RowCount = 8;
            tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.42959F));
            tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.29149F));
            tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.54469F));
            tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5499F));
            tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.54591F));
            tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.54572F));
            tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.54632F));
            tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.54638F));
            tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            tableLayoutPanel5.Size = new System.Drawing.Size(1087, 427);
            tableLayoutPanel5.TabIndex = 3;
            // 
            // lbl_input
            // 
            lbl_input.Anchor = System.Windows.Forms.AnchorStyles.Right;
            lbl_input.AutoSize = true;
            lbl_input.Location = new System.Drawing.Point(58, 20);
            lbl_input.Name = "lbl_input";
            lbl_input.Size = new System.Drawing.Size(102, 13);
            lbl_input.TabIndex = 1;
            lbl_input.Text = "Default Input Path";
            // 
            // lbl_telegram_token
            // 
            lbl_telegram_token.Anchor = System.Windows.Forms.AnchorStyles.Right;
            lbl_telegram_token.AutoSize = true;
            lbl_telegram_token.Location = new System.Drawing.Point(73, 124);
            lbl_telegram_token.Name = "lbl_telegram_token";
            lbl_telegram_token.Size = new System.Drawing.Size(87, 13);
            lbl_telegram_token.TabIndex = 6;
            lbl_telegram_token.Text = "Telegram Token";
            // 
            // tableLayoutPanel18
            // 
            tableLayoutPanel18.ColumnCount = 3;
            tableLayoutPanel18.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 73.09702F));
            tableLayoutPanel18.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.45378F));
            tableLayoutPanel18.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.4492F));
            tableLayoutPanel18.Controls.Add(btn_input_path, 2, 0);
            tableLayoutPanel18.Controls.Add(cmbInput, 0, 0);
            tableLayoutPanel18.Controls.Add(cb_inputpathsubfolders, 1, 0);
            tableLayoutPanel18.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel18.Location = new System.Drawing.Point(167, 4);
            tableLayoutPanel18.Name = "tableLayoutPanel18";
            tableLayoutPanel18.RowCount = 1;
            tableLayoutPanel18.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel18.Size = new System.Drawing.Size(916, 45);
            tableLayoutPanel18.TabIndex = 12;
            // 
            // btn_input_path
            // 
            btn_input_path.Anchor = System.Windows.Forms.AnchorStyles.None;
            btn_input_path.Location = new System.Drawing.Point(819, 7);
            btn_input_path.Name = "btn_input_path";
            btn_input_path.Size = new System.Drawing.Size(70, 30);
            btn_input_path.TabIndex = 2;
            btn_input_path.Text = "Select...";
            btn_input_path.UseVisualStyleBackColor = true;
            btn_input_path.Click += btn_input_path_Click;
            // 
            // cmbInput
            // 
            cmbInput.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            cmbInput.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            cmbInput.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            cmbInput.FormattingEnabled = true;
            cmbInput.Location = new System.Drawing.Point(3, 11);
            cmbInput.Margin = new System.Windows.Forms.Padding(3, 2, 2, 2);
            cmbInput.Name = "cmbInput";
            cmbInput.Size = new System.Drawing.Size(664, 21);
            cmbInput.TabIndex = 0;
            // 
            // cb_inputpathsubfolders
            // 
            cb_inputpathsubfolders.Anchor = System.Windows.Forms.AnchorStyles.Left;
            cb_inputpathsubfolders.AutoSize = true;
            cb_inputpathsubfolders.Location = new System.Drawing.Point(680, 14);
            cb_inputpathsubfolders.Margin = new System.Windows.Forms.Padding(11, 2, 2, 2);
            cb_inputpathsubfolders.Name = "cb_inputpathsubfolders";
            cb_inputpathsubfolders.Size = new System.Drawing.Size(82, 17);
            cb_inputpathsubfolders.TabIndex = 1;
            cb_inputpathsubfolders.Text = "Subfolders";
            cb_inputpathsubfolders.UseVisualStyleBackColor = true;
            // 
            // lbl_deepstackurl
            // 
            lbl_deepstackurl.Anchor = System.Windows.Forms.AnchorStyles.Right;
            lbl_deepstackurl.AutoSize = true;
            lbl_deepstackurl.Location = new System.Drawing.Point(75, 72);
            lbl_deepstackurl.Name = "lbl_deepstackurl";
            lbl_deepstackurl.Size = new System.Drawing.Size(85, 13);
            lbl_deepstackurl.TabIndex = 4;
            lbl_deepstackurl.Text = "AI Server URL(s)";
            // 
            // dbLayoutPanel1
            // 
            dbLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            dbLayoutPanel1.ColumnCount = 3;
            dbLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 72.96037F));
            dbLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.51981F));
            dbLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.45F));
            dbLayoutPanel1.Controls.Add(cb_DeepStackURLsQueued, 1, 0);
            dbLayoutPanel1.Controls.Add(button1, 0, 0);
            dbLayoutPanel1.Location = new System.Drawing.Point(166, 55);
            dbLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
            dbLayoutPanel1.Name = "dbLayoutPanel1";
            dbLayoutPanel1.RowCount = 1;
            dbLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            dbLayoutPanel1.Size = new System.Drawing.Size(918, 47);
            dbLayoutPanel1.TabIndex = 18;
            // 
            // cb_DeepStackURLsQueued
            // 
            cb_DeepStackURLsQueued.Anchor = System.Windows.Forms.AnchorStyles.Left;
            cb_DeepStackURLsQueued.AutoSize = true;
            cb_DeepStackURLsQueued.Location = new System.Drawing.Point(681, 15);
            cb_DeepStackURLsQueued.Margin = new System.Windows.Forms.Padding(11, 2, 2, 2);
            cb_DeepStackURLsQueued.Name = "cb_DeepStackURLsQueued";
            cb_DeepStackURLsQueued.Size = new System.Drawing.Size(67, 17);
            cb_DeepStackURLsQueued.TabIndex = 4;
            cb_DeepStackURLsQueued.Text = "Queued";
            toolTip1.SetToolTip(cb_DeepStackURLsQueued, "When checked, all urls will take turns processing the images.\r\nWhen unchecked, the original order will always be used.");
            cb_DeepStackURLsQueued.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            button1.Location = new System.Drawing.Point(3, 8);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(70, 30);
            button1.TabIndex = 3;
            button1.Text = "Edit";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click_1;
            // 
            // dbLayoutPanel2
            // 
            dbLayoutPanel2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            dbLayoutPanel2.ColumnCount = 5;
            dbLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 32.75058F));
            dbLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.275059F));
            dbLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 32.05128F));
            dbLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.63636F));
            dbLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.63636F));
            dbLayoutPanel2.Controls.Add(tb_telegram_cooldown, 4, 0);
            dbLayoutPanel2.Controls.Add(label5, 3, 0);
            dbLayoutPanel2.Controls.Add(tb_telegram_chatid, 2, 0);
            dbLayoutPanel2.Controls.Add(lbl_telegram_chatid, 1, 0);
            dbLayoutPanel2.Controls.Add(tb_telegram_token, 0, 0);
            dbLayoutPanel2.Location = new System.Drawing.Point(166, 107);
            dbLayoutPanel2.Margin = new System.Windows.Forms.Padding(2);
            dbLayoutPanel2.Name = "dbLayoutPanel2";
            dbLayoutPanel2.RowCount = 1;
            dbLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            dbLayoutPanel2.Size = new System.Drawing.Size(918, 48);
            dbLayoutPanel2.TabIndex = 19;
            // 
            // tb_telegram_cooldown
            // 
            tb_telegram_cooldown.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tb_telegram_cooldown.Location = new System.Drawing.Point(794, 13);
            tb_telegram_cooldown.Name = "tb_telegram_cooldown";
            tb_telegram_cooldown.Size = new System.Drawing.Size(121, 22);
            tb_telegram_cooldown.TabIndex = 7;
            // 
            // label5
            // 
            label5.Anchor = System.Windows.Forms.AnchorStyles.Right;
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(702, 17);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(86, 13);
            label5.TabIndex = 11;
            label5.Text = "Cooldown Secs";
            // 
            // tb_telegram_chatid
            // 
            tb_telegram_chatid.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tb_telegram_chatid.Location = new System.Drawing.Point(377, 13);
            tb_telegram_chatid.Name = "tb_telegram_chatid";
            tb_telegram_chatid.Size = new System.Drawing.Size(287, 22);
            tb_telegram_chatid.TabIndex = 6;
            // 
            // lbl_telegram_chatid
            // 
            lbl_telegram_chatid.Anchor = System.Windows.Forms.AnchorStyles.Right;
            lbl_telegram_chatid.AutoSize = true;
            lbl_telegram_chatid.Location = new System.Drawing.Point(326, 17);
            lbl_telegram_chatid.Name = "lbl_telegram_chatid";
            lbl_telegram_chatid.Size = new System.Drawing.Size(45, 13);
            lbl_telegram_chatid.TabIndex = 7;
            lbl_telegram_chatid.Text = "Chat ID";
            // 
            // tb_telegram_token
            // 
            tb_telegram_token.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tb_telegram_token.Location = new System.Drawing.Point(3, 13);
            tb_telegram_token.Name = "tb_telegram_token";
            tb_telegram_token.Size = new System.Drawing.Size(293, 22);
            tb_telegram_token.TabIndex = 5;
            // 
            // label12
            // 
            label12.Anchor = System.Windows.Forms.AnchorStyles.Right;
            label12.AutoSize = true;
            label12.Location = new System.Drawing.Point(94, 230);
            label12.Name = "label12";
            label12.Size = new System.Drawing.Size(66, 13);
            label12.TabIndex = 13;
            label12.Text = "Send Errors";
            // 
            // dbLayoutPanel3
            // 
            dbLayoutPanel3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            dbLayoutPanel3.ColumnCount = 4;
            dbLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 26.16822F));
            dbLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 53.85514F));
            dbLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9.918678F));
            dbLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9.9018F));
            dbLayoutPanel3.Controls.Add(cb_send_telegram_errors, 0, 0);
            dbLayoutPanel3.Controls.Add(cb_send_pushover_errors, 1, 0);
            dbLayoutPanel3.Location = new System.Drawing.Point(167, 214);
            dbLayoutPanel3.Name = "dbLayoutPanel3";
            dbLayoutPanel3.RowCount = 1;
            dbLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            dbLayoutPanel3.Size = new System.Drawing.Size(916, 46);
            dbLayoutPanel3.TabIndex = 20;
            // 
            // cb_send_telegram_errors
            // 
            cb_send_telegram_errors.Anchor = System.Windows.Forms.AnchorStyles.Left;
            cb_send_telegram_errors.AutoSize = true;
            cb_send_telegram_errors.Location = new System.Drawing.Point(3, 14);
            cb_send_telegram_errors.Name = "cb_send_telegram_errors";
            cb_send_telegram_errors.Size = new System.Drawing.Size(72, 17);
            cb_send_telegram_errors.TabIndex = 11;
            cb_send_telegram_errors.Text = "Telegram";
            cb_send_telegram_errors.UseVisualStyleBackColor = true;
            // 
            // cb_send_pushover_errors
            // 
            cb_send_pushover_errors.Anchor = System.Windows.Forms.AnchorStyles.Left;
            cb_send_pushover_errors.AutoSize = true;
            cb_send_pushover_errors.Location = new System.Drawing.Point(243, 14);
            cb_send_pushover_errors.Name = "cb_send_pushover_errors";
            cb_send_pushover_errors.Size = new System.Drawing.Size(73, 17);
            cb_send_pushover_errors.TabIndex = 12;
            cb_send_pushover_errors.Text = "Pushover";
            cb_send_pushover_errors.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(55, 336);
            label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(106, 13);
            label4.TabIndex = 16;
            label4.Text = "Default Credentials";
            // 
            // dbLayoutPanel4
            // 
            dbLayoutPanel4.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            dbLayoutPanel4.ColumnCount = 5;
            dbLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            dbLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            dbLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            dbLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            dbLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 466F));
            dbLayoutPanel4.Controls.Add(label6, 0, 0);
            dbLayoutPanel4.Controls.Add(tb_username, 1, 0);
            dbLayoutPanel4.Controls.Add(tb_password, 3, 0);
            dbLayoutPanel4.Controls.Add(label16, 2, 0);
            dbLayoutPanel4.Controls.Add(label17, 4, 0);
            dbLayoutPanel4.Location = new System.Drawing.Point(167, 320);
            dbLayoutPanel4.Name = "dbLayoutPanel4";
            dbLayoutPanel4.RowCount = 1;
            dbLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            dbLayoutPanel4.Size = new System.Drawing.Size(916, 46);
            dbLayoutPanel4.TabIndex = 21;
            // 
            // label6
            // 
            label6.Anchor = System.Windows.Forms.AnchorStyles.Right;
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(11, 16);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(61, 13);
            label6.TabIndex = 0;
            label6.Text = "Username:";
            // 
            // tb_username
            // 
            tb_username.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tb_username.Location = new System.Drawing.Point(78, 12);
            tb_username.Name = "tb_username";
            tb_username.Size = new System.Drawing.Size(144, 22);
            tb_username.TabIndex = 17;
            // 
            // tb_password
            // 
            tb_password.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tb_password.Location = new System.Drawing.Point(303, 12);
            tb_password.Name = "tb_password";
            tb_password.Size = new System.Drawing.Size(144, 22);
            tb_password.TabIndex = 18;
            // 
            // label16
            // 
            label16.Anchor = System.Windows.Forms.AnchorStyles.Right;
            label16.AutoSize = true;
            label16.Location = new System.Drawing.Point(238, 16);
            label16.Name = "label16";
            label16.Size = new System.Drawing.Size(59, 13);
            label16.TabIndex = 0;
            label16.Text = "Password:";
            // 
            // label17
            // 
            label17.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            label17.AutoSize = true;
            label17.Location = new System.Drawing.Point(453, 16);
            label17.Name = "label17";
            label17.Size = new System.Drawing.Size(460, 13);
            label17.TabIndex = 3;
            label17.Text = "These will be used with the [Username] and [Password] variables in Camera Actions.";
            label17.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label18
            // 
            label18.Anchor = System.Windows.Forms.AnchorStyles.Right;
            label18.AutoSize = true;
            label18.Location = new System.Drawing.Point(48, 391);
            label18.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label18.Name = "label18";
            label18.Size = new System.Drawing.Size(113, 13);
            label18.TabIndex = 16;
            label18.Text = "BlueIris Server Name:";
            // 
            // dbLayoutPanel5
            // 
            dbLayoutPanel5.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            dbLayoutPanel5.ColumnCount = 3;
            dbLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 17.52337F));
            dbLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.76419F));
            dbLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 56.76856F));
            dbLayoutPanel5.Controls.Add(tb_BlueIrisServer, 0, 0);
            dbLayoutPanel5.Controls.Add(label19, 2, 0);
            dbLayoutPanel5.Controls.Add(lbl_blueirisserver, 1, 0);
            dbLayoutPanel5.Location = new System.Drawing.Point(167, 373);
            dbLayoutPanel5.Name = "dbLayoutPanel5";
            dbLayoutPanel5.RowCount = 1;
            dbLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            dbLayoutPanel5.Size = new System.Drawing.Size(916, 50);
            dbLayoutPanel5.TabIndex = 22;
            // 
            // tb_BlueIrisServer
            // 
            tb_BlueIrisServer.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tb_BlueIrisServer.Location = new System.Drawing.Point(3, 14);
            tb_BlueIrisServer.Name = "tb_BlueIrisServer";
            tb_BlueIrisServer.Size = new System.Drawing.Size(154, 22);
            tb_BlueIrisServer.TabIndex = 19;
            // 
            // label19
            // 
            label19.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            label19.AutoSize = true;
            label19.Location = new System.Drawing.Point(398, 0);
            label19.Name = "label19";
            label19.Size = new System.Drawing.Size(515, 50);
            label19.TabIndex = 3;
            label19.Text = resources.GetString("label19.Text");
            label19.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl_blueirisserver
            // 
            lbl_blueirisserver.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lbl_blueirisserver.AutoSize = true;
            lbl_blueirisserver.ForeColor = System.Drawing.Color.DodgerBlue;
            lbl_blueirisserver.Location = new System.Drawing.Point(163, 0);
            lbl_blueirisserver.Name = "lbl_blueirisserver";
            lbl_blueirisserver.Size = new System.Drawing.Size(229, 50);
            lbl_blueirisserver.TabIndex = 20;
            lbl_blueirisserver.Text = "BlueIris Web Server is configured to listen on 111.111.111.111:43";
            lbl_blueirisserver.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label29
            // 
            label29.Anchor = System.Windows.Forms.AnchorStyles.Right;
            label29.AutoSize = true;
            label29.Location = new System.Drawing.Point(67, 177);
            label29.Name = "label29";
            label29.Size = new System.Drawing.Size(93, 13);
            label29.TabIndex = 6;
            label29.Text = "Pushover API Key";
            // 
            // dbLayoutPanel8
            // 
            dbLayoutPanel8.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            dbLayoutPanel8.ColumnCount = 5;
            dbLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 32.75058F));
            dbLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.391608F));
            dbLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 31.81818F));
            dbLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.28671F));
            dbLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.63636F));
            dbLayoutPanel8.Controls.Add(tb_Pushover_Cooldown, 4, 0);
            dbLayoutPanel8.Controls.Add(tb_Pushover_APIKey, 0, 0);
            dbLayoutPanel8.Controls.Add(label31, 3, 0);
            dbLayoutPanel8.Controls.Add(label30, 1, 0);
            dbLayoutPanel8.Controls.Add(tb_Pushover_UserKey, 2, 0);
            dbLayoutPanel8.Location = new System.Drawing.Point(166, 160);
            dbLayoutPanel8.Margin = new System.Windows.Forms.Padding(2);
            dbLayoutPanel8.Name = "dbLayoutPanel8";
            dbLayoutPanel8.RowCount = 1;
            dbLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            dbLayoutPanel8.Size = new System.Drawing.Size(918, 48);
            dbLayoutPanel8.TabIndex = 23;
            // 
            // tb_Pushover_Cooldown
            // 
            tb_Pushover_Cooldown.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tb_Pushover_Cooldown.Location = new System.Drawing.Point(795, 13);
            tb_Pushover_Cooldown.Name = "tb_Pushover_Cooldown";
            tb_Pushover_Cooldown.Size = new System.Drawing.Size(120, 22);
            tb_Pushover_Cooldown.TabIndex = 10;
            // 
            // tb_Pushover_APIKey
            // 
            tb_Pushover_APIKey.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tb_Pushover_APIKey.Location = new System.Drawing.Point(3, 13);
            tb_Pushover_APIKey.Name = "tb_Pushover_APIKey";
            tb_Pushover_APIKey.Size = new System.Drawing.Size(295, 22);
            tb_Pushover_APIKey.TabIndex = 8;
            // 
            // label31
            // 
            label31.Anchor = System.Windows.Forms.AnchorStyles.Right;
            label31.AutoSize = true;
            label31.Location = new System.Drawing.Point(703, 17);
            label31.Name = "label31";
            label31.Size = new System.Drawing.Size(86, 13);
            label31.TabIndex = 11;
            label31.Text = "Cooldown Secs";
            // 
            // label30
            // 
            label30.Anchor = System.Windows.Forms.AnchorStyles.Right;
            label30.AutoSize = true;
            label30.Location = new System.Drawing.Point(322, 17);
            label30.Name = "label30";
            label30.Size = new System.Drawing.Size(53, 13);
            label30.TabIndex = 7;
            label30.Text = "User Key:";
            // 
            // tb_Pushover_UserKey
            // 
            tb_Pushover_UserKey.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tb_Pushover_UserKey.Location = new System.Drawing.Point(381, 13);
            tb_Pushover_UserKey.Name = "tb_Pushover_UserKey";
            tb_Pushover_UserKey.Size = new System.Drawing.Size(286, 22);
            tb_Pushover_UserKey.TabIndex = 9;
            // 
            // dbLayoutPanel9
            // 
            dbLayoutPanel9.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            dbLayoutPanel9.ColumnCount = 3;
            dbLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 26.82635F));
            dbLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 73.17365F));
            dbLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 92F));
            dbLayoutPanel9.Controls.Add(cbStartWithWindows, 0, 0);
            dbLayoutPanel9.Controls.Add(cbMinimizeToTray, 1, 0);
            dbLayoutPanel9.Location = new System.Drawing.Point(167, 267);
            dbLayoutPanel9.Name = "dbLayoutPanel9";
            dbLayoutPanel9.RowCount = 1;
            dbLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            dbLayoutPanel9.Size = new System.Drawing.Size(916, 46);
            dbLayoutPanel9.TabIndex = 24;
            // 
            // cbStartWithWindows
            // 
            cbStartWithWindows.Anchor = System.Windows.Forms.AnchorStyles.Left;
            cbStartWithWindows.AutoSize = true;
            cbStartWithWindows.Location = new System.Drawing.Point(2, 14);
            cbStartWithWindows.Margin = new System.Windows.Forms.Padding(2);
            cbStartWithWindows.Name = "cbStartWithWindows";
            cbStartWithWindows.Size = new System.Drawing.Size(199, 17);
            cbStartWithWindows.TabIndex = 15;
            cbStartWithWindows.Text = "Start with user login (non-service)";
            cbStartWithWindows.UseVisualStyleBackColor = true;
            // 
            // cbMinimizeToTray
            // 
            cbMinimizeToTray.Anchor = System.Windows.Forms.AnchorStyles.Left;
            cbMinimizeToTray.AutoSize = true;
            cbMinimizeToTray.Location = new System.Drawing.Point(224, 14);
            cbMinimizeToTray.Name = "cbMinimizeToTray";
            cbMinimizeToTray.Size = new System.Drawing.Size(109, 17);
            cbMinimizeToTray.TabIndex = 16;
            cbMinimizeToTray.Text = "Minimize to Tray";
            cbMinimizeToTray.UseVisualStyleBackColor = true;
            // 
            // label13
            // 
            label13.Anchor = System.Windows.Forms.AnchorStyles.Right;
            label13.AutoSize = true;
            label13.Location = new System.Drawing.Point(85, 283);
            label13.Name = "label13";
            label13.Size = new System.Drawing.Size(75, 13);
            label13.TabIndex = 13;
            label13.Text = "Misc Settings";
            // 
            // dbLayoutPanel10
            // 
            dbLayoutPanel10.ColumnCount = 3;
            dbLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            dbLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            dbLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            dbLayoutPanel10.Controls.Add(button3, 0, 0);
            dbLayoutPanel10.Controls.Add(BtnSettingsSave, 2, 0);
            dbLayoutPanel10.Controls.Add(bt_CheckUpdates, 1, 0);
            dbLayoutPanel10.Dock = System.Windows.Forms.DockStyle.Fill;
            dbLayoutPanel10.Location = new System.Drawing.Point(3, 436);
            dbLayoutPanel10.Name = "dbLayoutPanel10";
            dbLayoutPanel10.RowCount = 1;
            dbLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            dbLayoutPanel10.Size = new System.Drawing.Size(1087, 34);
            dbLayoutPanel10.TabIndex = 4;
            // 
            // button3
            // 
            button3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom;
            button3.Location = new System.Drawing.Point(146, 3);
            button3.Name = "button3";
            button3.Size = new System.Drawing.Size(69, 28);
            button3.TabIndex = 20;
            button3.Text = "Reset";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // BtnSettingsSave
            // 
            BtnSettingsSave.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom;
            BtnSettingsSave.Location = new System.Drawing.Point(871, 3);
            BtnSettingsSave.Name = "BtnSettingsSave";
            BtnSettingsSave.Size = new System.Drawing.Size(69, 28);
            BtnSettingsSave.TabIndex = 21;
            BtnSettingsSave.Text = "Save";
            BtnSettingsSave.UseVisualStyleBackColor = true;
            BtnSettingsSave.Click += BtnSettingsSave_Click_1;
            // 
            // bt_CheckUpdates
            // 
            bt_CheckUpdates.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom;
            bt_CheckUpdates.Location = new System.Drawing.Point(508, 3);
            bt_CheckUpdates.Name = "bt_CheckUpdates";
            bt_CheckUpdates.Size = new System.Drawing.Size(69, 28);
            bt_CheckUpdates.TabIndex = 21;
            bt_CheckUpdates.Text = "Updates";
            toolTip1.SetToolTip(bt_CheckUpdates, "Check for AITOOL updates");
            bt_CheckUpdates.UseVisualStyleBackColor = true;
            bt_CheckUpdates.Click += bt_CheckUpdates_Click;
            // 
            // tabDeepStack
            // 
            tabDeepStack.Controls.Add(chk_AutoAdd);
            tabDeepStack.Controls.Add(label34);
            tabDeepStack.Controls.Add(label33);
            tabDeepStack.Controls.Add(label32);
            tabDeepStack.Controls.Add(txt_DeepstackNoMoreOftenThanMins);
            tabDeepStack.Controls.Add(txt_DeepstackRestartFailCount);
            tabDeepStack.Controls.Add(Chk_AutoReStart);
            tabDeepStack.Controls.Add(Btn_ViewLog);
            tabDeepStack.Controls.Add(Btn_DeepstackReset);
            tabDeepStack.Controls.Add(linkLabel1);
            tabDeepStack.Controls.Add(groupBox11);
            tabDeepStack.Controls.Add(groupBox10);
            tabDeepStack.Controls.Add(lbl_DeepstackType);
            tabDeepStack.Controls.Add(lbl_Deepstackversion);
            tabDeepStack.Controls.Add(lbl_deepstackname);
            tabDeepStack.Controls.Add(label28);
            tabDeepStack.Controls.Add(label24);
            tabDeepStack.Controls.Add(label23);
            tabDeepStack.Controls.Add(label22);
            tabDeepStack.Controls.Add(chk_stopbeforestart);
            tabDeepStack.Controls.Add(chk_HighPriority);
            tabDeepStack.Controls.Add(Chk_DSDebug);
            tabDeepStack.Controls.Add(Lbl_BlueStackRunning);
            tabDeepStack.Controls.Add(Btn_Save);
            tabDeepStack.Controls.Add(label11);
            tabDeepStack.Controls.Add(groupBox1);
            tabDeepStack.Controls.Add(groupBox2);
            tabDeepStack.Controls.Add(groupBox4);
            tabDeepStack.Controls.Add(groupBox3);
            tabDeepStack.Controls.Add(Chk_AutoStart);
            tabDeepStack.Controls.Add(Btn_Start);
            tabDeepStack.Controls.Add(Btn_Stop);
            tabDeepStack.Controls.Add(groupBoxCustomModel);
            tabDeepStack.Location = new System.Drawing.Point(4, 24);
            tabDeepStack.Margin = new System.Windows.Forms.Padding(2);
            tabDeepStack.Name = "tabDeepStack";
            tabDeepStack.Size = new System.Drawing.Size(1099, 492);
            tabDeepStack.TabIndex = 6;
            tabDeepStack.Text = "DeepStack";
            tabDeepStack.UseVisualStyleBackColor = true;
            // 
            // chk_AutoAdd
            // 
            chk_AutoAdd.AutoSize = true;
            chk_AutoAdd.Location = new System.Drawing.Point(93, 361);
            chk_AutoAdd.Name = "chk_AutoAdd";
            chk_AutoAdd.Size = new System.Drawing.Size(75, 17);
            chk_AutoAdd.TabIndex = 27;
            chk_AutoAdd.Text = "Auto Add";
            toolTip1.SetToolTip(chk_AutoAdd, "If the local Deepstack server URL is not already in Settings > AI Server URL list, it will be added automatically.");
            chk_AutoAdd.UseVisualStyleBackColor = true;
            // 
            // label34
            // 
            label34.AutoSize = true;
            label34.Location = new System.Drawing.Point(459, 389);
            label34.Name = "label34";
            label34.Size = new System.Drawing.Size(32, 13);
            label34.TabIndex = 26;
            label34.Text = "Mins";
            // 
            // label33
            // 
            label33.AutoSize = true;
            label33.Location = new System.Drawing.Point(307, 390);
            label33.Name = "label33";
            label33.Size = new System.Drawing.Size(109, 13);
            label33.TabIndex = 25;
            label33.Text = "No more often than";
            // 
            // label32
            // 
            label32.AutoSize = true;
            label32.Location = new System.Drawing.Point(172, 390);
            label32.Name = "label32";
            label32.Size = new System.Drawing.Size(118, 13);
            label32.TabIndex = 24;
            label32.Text = "URL Failures in a row.";
            // 
            // txt_DeepstackNoMoreOftenThanMins
            // 
            txt_DeepstackNoMoreOftenThanMins.Location = new System.Drawing.Point(411, 386);
            txt_DeepstackNoMoreOftenThanMins.Name = "txt_DeepstackNoMoreOftenThanMins";
            txt_DeepstackNoMoreOftenThanMins.Size = new System.Drawing.Size(42, 22);
            txt_DeepstackNoMoreOftenThanMins.TabIndex = 17;
            // 
            // txt_DeepstackRestartFailCount
            // 
            txt_DeepstackRestartFailCount.Location = new System.Drawing.Point(124, 386);
            txt_DeepstackRestartFailCount.Name = "txt_DeepstackRestartFailCount";
            txt_DeepstackRestartFailCount.Size = new System.Drawing.Size(42, 22);
            txt_DeepstackRestartFailCount.TabIndex = 16;
            // 
            // Chk_AutoReStart
            // 
            Chk_AutoReStart.AutoSize = true;
            Chk_AutoReStart.Location = new System.Drawing.Point(9, 389);
            Chk_AutoReStart.Name = "Chk_AutoReStart";
            Chk_AutoReStart.Size = new System.Drawing.Size(117, 17);
            Chk_AutoReStart.TabIndex = 15;
            Chk_AutoReStart.Text = "Auto Restart after";
            toolTip1.SetToolTip(Chk_AutoReStart, "Note this feature only works if AUTO START is also enabled");
            Chk_AutoReStart.UseVisualStyleBackColor = true;
            // 
            // Btn_ViewLog
            // 
            Btn_ViewLog.ForeColor = System.Drawing.Color.Maroon;
            Btn_ViewLog.Location = new System.Drawing.Point(359, 416);
            Btn_ViewLog.Name = "Btn_ViewLog";
            Btn_ViewLog.Size = new System.Drawing.Size(70, 30);
            Btn_ViewLog.TabIndex = 22;
            Btn_ViewLog.Text = "stderr.txt";
            toolTip1.SetToolTip(Btn_ViewLog, "Open STDERR.TXT which contains any errors deepstack may have had.");
            Btn_ViewLog.UseVisualStyleBackColor = true;
            Btn_ViewLog.Click += Btn_ViewLog_Click;
            // 
            // Btn_DeepstackReset
            // 
            Btn_DeepstackReset.Location = new System.Drawing.Point(271, 416);
            Btn_DeepstackReset.Name = "Btn_DeepstackReset";
            Btn_DeepstackReset.Size = new System.Drawing.Size(70, 30);
            Btn_DeepstackReset.TabIndex = 21;
            Btn_DeepstackReset.Text = "Reset";
            toolTip1.SetToolTip(Btn_DeepstackReset, "Delete all Deepstack temp files");
            Btn_DeepstackReset.UseVisualStyleBackColor = true;
            Btn_DeepstackReset.Click += bt_DeepstackReset_Click;
            // 
            // linkLabel1
            // 
            linkLabel1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            linkLabel1.Location = new System.Drawing.Point(0, 26);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new System.Drawing.Size(1097, 13);
            linkLabel1.TabIndex = 19;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "https://docs.deepstack.cc/windows/index.html";
            linkLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            linkLabel1.LinkClicked += linkLabel1_LinkClicked;
            // 
            // groupBox11
            // 
            groupBox11.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            groupBox11.Controls.Add(tb_DeepStackURLs);
            groupBox11.Location = new System.Drawing.Point(511, 300);
            groupBox11.Name = "groupBox11";
            groupBox11.Size = new System.Drawing.Size(577, 118);
            groupBox11.TabIndex = 18;
            groupBox11.TabStop = false;
            groupBox11.Text = "URL";
            // 
            // tb_DeepStackURLs
            // 
            tb_DeepStackURLs.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tb_DeepStackURLs.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            tb_DeepStackURLs.Location = new System.Drawing.Point(3, 22);
            tb_DeepStackURLs.Multiline = true;
            tb_DeepStackURLs.Name = "tb_DeepStackURLs";
            tb_DeepStackURLs.ReadOnly = true;
            tb_DeepStackURLs.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            tb_DeepStackURLs.Size = new System.Drawing.Size(570, 93);
            tb_DeepStackURLs.TabIndex = 0;
            tb_DeepStackURLs.WordWrap = false;
            // 
            // groupBox10
            // 
            groupBox10.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            groupBox10.Controls.Add(tb_DeepstackCommandLine);
            groupBox10.Location = new System.Drawing.Point(510, 167);
            groupBox10.Name = "groupBox10";
            groupBox10.Size = new System.Drawing.Size(581, 118);
            groupBox10.TabIndex = 18;
            groupBox10.TabStop = false;
            groupBox10.Text = "Command line";
            // 
            // tb_DeepstackCommandLine
            // 
            tb_DeepstackCommandLine.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tb_DeepstackCommandLine.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            tb_DeepstackCommandLine.Location = new System.Drawing.Point(3, 22);
            tb_DeepstackCommandLine.Multiline = true;
            tb_DeepstackCommandLine.Name = "tb_DeepstackCommandLine";
            tb_DeepstackCommandLine.ReadOnly = true;
            tb_DeepstackCommandLine.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            tb_DeepstackCommandLine.Size = new System.Drawing.Size(575, 93);
            tb_DeepstackCommandLine.TabIndex = 0;
            tb_DeepstackCommandLine.WordWrap = false;
            // 
            // lbl_DeepstackType
            // 
            lbl_DeepstackType.AutoSize = true;
            lbl_DeepstackType.ForeColor = System.Drawing.Color.DodgerBlue;
            lbl_DeepstackType.Location = new System.Drawing.Point(558, 96);
            lbl_DeepstackType.Name = "lbl_DeepstackType";
            lbl_DeepstackType.Size = new System.Drawing.Size(10, 13);
            lbl_DeepstackType.TabIndex = 16;
            lbl_DeepstackType.Text = ".";
            // 
            // lbl_Deepstackversion
            // 
            lbl_Deepstackversion.AutoSize = true;
            lbl_Deepstackversion.ForeColor = System.Drawing.Color.DodgerBlue;
            lbl_Deepstackversion.Location = new System.Drawing.Point(558, 75);
            lbl_Deepstackversion.Name = "lbl_Deepstackversion";
            lbl_Deepstackversion.Size = new System.Drawing.Size(10, 13);
            lbl_Deepstackversion.TabIndex = 16;
            lbl_Deepstackversion.Text = ".";
            // 
            // lbl_deepstackname
            // 
            lbl_deepstackname.AutoSize = true;
            lbl_deepstackname.ForeColor = System.Drawing.Color.DodgerBlue;
            lbl_deepstackname.Location = new System.Drawing.Point(558, 56);
            lbl_deepstackname.Name = "lbl_deepstackname";
            lbl_deepstackname.Size = new System.Drawing.Size(10, 13);
            lbl_deepstackname.TabIndex = 16;
            lbl_deepstackname.Text = ".";
            // 
            // label28
            // 
            label28.AutoSize = true;
            label28.Location = new System.Drawing.Point(512, 115);
            label28.Name = "label28";
            label28.Size = new System.Drawing.Size(42, 13);
            label28.TabIndex = 16;
            label28.Text = "Status:";
            // 
            // label24
            // 
            label24.AutoSize = true;
            label24.Location = new System.Drawing.Point(518, 96);
            label24.Name = "label24";
            label24.Size = new System.Drawing.Size(33, 13);
            label24.TabIndex = 16;
            label24.Text = "Type:";
            // 
            // label23
            // 
            label23.AutoSize = true;
            label23.Location = new System.Drawing.Point(507, 75);
            label23.Name = "label23";
            label23.Size = new System.Drawing.Size(48, 13);
            label23.TabIndex = 16;
            label23.Text = "Version:";
            // 
            // label22
            // 
            label22.AutoSize = true;
            label22.Location = new System.Drawing.Point(514, 56);
            label22.Name = "label22";
            label22.Size = new System.Drawing.Size(39, 13);
            label22.TabIndex = 16;
            label22.Text = "Name:";
            // 
            // chk_stopbeforestart
            // 
            chk_stopbeforestart.AutoSize = true;
            chk_stopbeforestart.Location = new System.Drawing.Point(356, 361);
            chk_stopbeforestart.Margin = new System.Windows.Forms.Padding(2);
            chk_stopbeforestart.Name = "chk_stopbeforestart";
            chk_stopbeforestart.Size = new System.Drawing.Size(150, 17);
            chk_stopbeforestart.TabIndex = 14;
            chk_stopbeforestart.Text = "Always stop before start";
            toolTip1.SetToolTip(chk_stopbeforestart, "If deepstack exe files are running when a START is requested, stop them first.");
            chk_stopbeforestart.UseVisualStyleBackColor = true;
            // 
            // chk_HighPriority
            // 
            chk_HighPriority.AutoSize = true;
            chk_HighPriority.Location = new System.Drawing.Point(243, 361);
            chk_HighPriority.Margin = new System.Windows.Forms.Padding(2);
            chk_HighPriority.Name = "chk_HighPriority";
            chk_HighPriority.Size = new System.Drawing.Size(114, 17);
            chk_HighPriority.TabIndex = 13;
            chk_HighPriority.Text = "Run high priority";
            toolTip1.SetToolTip(chk_HighPriority, "Increase the process priority of deepstack, python, etc.   This may make deepstack slighly more responsive.");
            chk_HighPriority.UseVisualStyleBackColor = true;
            // 
            // Chk_DSDebug
            // 
            Chk_DSDebug.AutoSize = true;
            Chk_DSDebug.Location = new System.Drawing.Point(174, 361);
            Chk_DSDebug.Margin = new System.Windows.Forms.Padding(2);
            Chk_DSDebug.Name = "Chk_DSDebug";
            Chk_DSDebug.Size = new System.Drawing.Size(61, 17);
            Chk_DSDebug.TabIndex = 12;
            Chk_DSDebug.Text = "Debug";
            toolTip1.SetToolTip(Chk_DSDebug, "Show all output from Deepstack's python.exe, redis.exe and server.exe  (Windows version, installed on same machine)");
            Chk_DSDebug.UseVisualStyleBackColor = true;
            // 
            // Lbl_BlueStackRunning
            // 
            Lbl_BlueStackRunning.AutoSize = true;
            Lbl_BlueStackRunning.Location = new System.Drawing.Point(558, 115);
            Lbl_BlueStackRunning.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            Lbl_BlueStackRunning.Name = "Lbl_BlueStackRunning";
            Lbl_BlueStackRunning.Size = new System.Drawing.Size(93, 13);
            Lbl_BlueStackRunning.TabIndex = 13;
            Lbl_BlueStackRunning.Text = "*NOT RUNNING*";
            // 
            // Btn_Save
            // 
            Btn_Save.Location = new System.Drawing.Point(183, 416);
            Btn_Save.Margin = new System.Windows.Forms.Padding(2);
            Btn_Save.Name = "Btn_Save";
            Btn_Save.Size = new System.Drawing.Size(70, 30);
            Btn_Save.TabIndex = 20;
            Btn_Save.Text = "Save";
            Btn_Save.UseVisualStyleBackColor = true;
            Btn_Save.Click += Btn_Save_Click;
            // 
            // label11
            // 
            label11.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            label11.BackColor = System.Drawing.SystemColors.Info;
            label11.ForeColor = System.Drawing.Color.RoyalBlue;
            label11.Location = new System.Drawing.Point(0, 4);
            label11.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(1097, 22);
            label11.TabIndex = 0;
            label11.Text = "This tab is only for the WINDOWS version of Deepstack";
            label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(Chk_DetectionAPI);
            groupBox1.Controls.Add(Chk_FaceAPI);
            groupBox1.Controls.Add(Chk_SceneAPI);
            groupBox1.Location = new System.Drawing.Point(7, 258);
            groupBox1.Margin = new System.Windows.Forms.Padding(2);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new System.Windows.Forms.Padding(2);
            groupBox1.Size = new System.Drawing.Size(162, 88);
            groupBox1.TabIndex = 3;
            groupBox1.TabStop = false;
            groupBox1.Text = "API";
            // 
            // Chk_DetectionAPI
            // 
            Chk_DetectionAPI.Location = new System.Drawing.Point(11, 63);
            Chk_DetectionAPI.Margin = new System.Windows.Forms.Padding(2);
            Chk_DetectionAPI.Name = "Chk_DetectionAPI";
            Chk_DetectionAPI.Size = new System.Drawing.Size(147, 19);
            Chk_DetectionAPI.TabIndex = 6;
            Chk_DetectionAPI.Text = "Detection API";
            Chk_DetectionAPI.UseVisualStyleBackColor = true;
            // 
            // Chk_FaceAPI
            // 
            Chk_FaceAPI.Location = new System.Drawing.Point(11, 40);
            Chk_FaceAPI.Margin = new System.Windows.Forms.Padding(2);
            Chk_FaceAPI.Name = "Chk_FaceAPI";
            Chk_FaceAPI.Size = new System.Drawing.Size(147, 19);
            Chk_FaceAPI.TabIndex = 5;
            Chk_FaceAPI.Text = "Face API";
            Chk_FaceAPI.UseVisualStyleBackColor = true;
            // 
            // Chk_SceneAPI
            // 
            Chk_SceneAPI.Location = new System.Drawing.Point(11, 17);
            Chk_SceneAPI.Margin = new System.Windows.Forms.Padding(2);
            Chk_SceneAPI.Name = "Chk_SceneAPI";
            Chk_SceneAPI.Size = new System.Drawing.Size(147, 19);
            Chk_SceneAPI.TabIndex = 4;
            Chk_SceneAPI.Text = "Scene API";
            Chk_SceneAPI.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(RB_High);
            groupBox2.Controls.Add(RB_Medium);
            groupBox2.Controls.Add(RB_Low);
            groupBox2.Location = new System.Drawing.Point(174, 258);
            groupBox2.Margin = new System.Windows.Forms.Padding(2);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new System.Windows.Forms.Padding(2);
            groupBox2.Size = new System.Drawing.Size(154, 88);
            groupBox2.TabIndex = 4;
            groupBox2.TabStop = false;
            groupBox2.Text = "Mode";
            // 
            // RB_High
            // 
            RB_High.Location = new System.Drawing.Point(11, 63);
            RB_High.Margin = new System.Windows.Forms.Padding(2);
            RB_High.Name = "RB_High";
            RB_High.Size = new System.Drawing.Size(139, 19);
            RB_High.TabIndex = 9;
            RB_High.TabStop = true;
            RB_High.Text = "High";
            RB_High.UseVisualStyleBackColor = true;
            // 
            // RB_Medium
            // 
            RB_Medium.Location = new System.Drawing.Point(11, 40);
            RB_Medium.Margin = new System.Windows.Forms.Padding(2);
            RB_Medium.Name = "RB_Medium";
            RB_Medium.Size = new System.Drawing.Size(139, 19);
            RB_Medium.TabIndex = 8;
            RB_Medium.TabStop = true;
            RB_Medium.Text = "Medium";
            RB_Medium.UseVisualStyleBackColor = true;
            // 
            // RB_Low
            // 
            RB_Low.Location = new System.Drawing.Point(11, 17);
            RB_Low.Margin = new System.Windows.Forms.Padding(2);
            RB_Low.Name = "RB_Low";
            RB_Low.Size = new System.Drawing.Size(139, 19);
            RB_Low.TabIndex = 7;
            RB_Low.TabStop = true;
            RB_Low.Text = "Low";
            RB_Low.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(Txt_DeepStackInstallFolder);
            groupBox4.Location = new System.Drawing.Point(11, 47);
            groupBox4.Margin = new System.Windows.Forms.Padding(2);
            groupBox4.Name = "groupBox4";
            groupBox4.Padding = new System.Windows.Forms.Padding(2);
            groupBox4.Size = new System.Drawing.Size(483, 45);
            groupBox4.TabIndex = 9;
            groupBox4.TabStop = false;
            groupBox4.Text = "DeepStack Install Folder";
            // 
            // Txt_DeepStackInstallFolder
            // 
            Txt_DeepStackInstallFolder.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            Txt_DeepStackInstallFolder.Location = new System.Drawing.Point(7, 17);
            Txt_DeepStackInstallFolder.Margin = new System.Windows.Forms.Padding(2);
            Txt_DeepStackInstallFolder.Name = "Txt_DeepStackInstallFolder";
            Txt_DeepStackInstallFolder.ReadOnly = true;
            Txt_DeepStackInstallFolder.Size = new System.Drawing.Size(472, 22);
            Txt_DeepStackInstallFolder.TabIndex = 2;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(label27);
            groupBox3.Controls.Add(Txt_Port);
            groupBox3.Location = new System.Drawing.Point(341, 258);
            groupBox3.Margin = new System.Windows.Forms.Padding(2);
            groupBox3.Name = "groupBox3";
            groupBox3.Padding = new System.Windows.Forms.Padding(2);
            groupBox3.Size = new System.Drawing.Size(151, 88);
            groupBox3.TabIndex = 5;
            groupBox3.TabStop = false;
            groupBox3.Text = "Port(s)";
            // 
            // label27
            // 
            label27.ForeColor = System.Drawing.Color.DarkGray;
            label27.Location = new System.Drawing.Point(5, 41);
            label27.Name = "label27";
            label27.Size = new System.Drawing.Size(142, 45);
            label27.TabIndex = 1;
            label27.Text = "Enter one or more ports with commas between to start more than one instance.";
            // 
            // Txt_Port
            // 
            Txt_Port.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            Txt_Port.Location = new System.Drawing.Point(10, 19);
            Txt_Port.Margin = new System.Windows.Forms.Padding(2);
            Txt_Port.Name = "Txt_Port";
            Txt_Port.Size = new System.Drawing.Size(127, 22);
            Txt_Port.TabIndex = 10;
            toolTip1.SetToolTip(Txt_Port, resources.GetString("Txt_Port.ToolTip"));
            // 
            // Chk_AutoStart
            // 
            Chk_AutoStart.AutoSize = true;
            Chk_AutoStart.Location = new System.Drawing.Point(9, 361);
            Chk_AutoStart.Margin = new System.Windows.Forms.Padding(2);
            Chk_AutoStart.Name = "Chk_AutoStart";
            Chk_AutoStart.Size = new System.Drawing.Size(78, 17);
            Chk_AutoStart.TabIndex = 11;
            Chk_AutoStart.Text = "Auto Start";
            toolTip1.SetToolTip(Chk_AutoStart, "Automatically start Deepstack when AITOOL starts.");
            Chk_AutoStart.UseVisualStyleBackColor = true;
            // 
            // Btn_Start
            // 
            Btn_Start.Location = new System.Drawing.Point(7, 416);
            Btn_Start.Margin = new System.Windows.Forms.Padding(2);
            Btn_Start.Name = "Btn_Start";
            Btn_Start.Size = new System.Drawing.Size(70, 30);
            Btn_Start.TabIndex = 18;
            Btn_Start.Text = "Start";
            Btn_Start.UseVisualStyleBackColor = true;
            Btn_Start.Click += Btn_Start_Click;
            // 
            // Btn_Stop
            // 
            Btn_Stop.Location = new System.Drawing.Point(95, 416);
            Btn_Stop.Margin = new System.Windows.Forms.Padding(2);
            Btn_Stop.Name = "Btn_Stop";
            Btn_Stop.Size = new System.Drawing.Size(70, 30);
            Btn_Stop.TabIndex = 19;
            Btn_Stop.Text = "Stop";
            Btn_Stop.UseVisualStyleBackColor = true;
            Btn_Stop.Click += Btn_Stop_Click;
            // 
            // groupBoxCustomModel
            // 
            groupBoxCustomModel.Controls.Add(Chk_CustomModelAPI);
            groupBoxCustomModel.Controls.Add(label38);
            groupBoxCustomModel.Controls.Add(label9);
            groupBoxCustomModel.Controls.Add(label37);
            groupBoxCustomModel.Controls.Add(label36);
            groupBoxCustomModel.Controls.Add(label35);
            groupBoxCustomModel.Controls.Add(Txt_CustomModelMode);
            groupBoxCustomModel.Controls.Add(Txt_CustomModelPort);
            groupBoxCustomModel.Controls.Add(Txt_CustomModelName);
            groupBoxCustomModel.Controls.Add(Txt_CustomModelPath);
            groupBoxCustomModel.Location = new System.Drawing.Point(11, 96);
            groupBoxCustomModel.Margin = new System.Windows.Forms.Padding(2);
            groupBoxCustomModel.Name = "groupBoxCustomModel";
            groupBoxCustomModel.Padding = new System.Windows.Forms.Padding(2);
            groupBoxCustomModel.Size = new System.Drawing.Size(483, 158);
            groupBoxCustomModel.TabIndex = 17;
            groupBoxCustomModel.TabStop = false;
            // 
            // Chk_CustomModelAPI
            // 
            Chk_CustomModelAPI.AutoSize = true;
            Chk_CustomModelAPI.BackColor = System.Drawing.SystemColors.Control;
            Chk_CustomModelAPI.FlatStyle = System.Windows.Forms.FlatStyle.System;
            Chk_CustomModelAPI.Location = new System.Drawing.Point(7, -2);
            Chk_CustomModelAPI.Name = "Chk_CustomModelAPI";
            Chk_CustomModelAPI.Size = new System.Drawing.Size(107, 18);
            Chk_CustomModelAPI.TabIndex = 0;
            Chk_CustomModelAPI.Text = "Custom Model";
            Chk_CustomModelAPI.UseVisualStyleBackColor = false;
            Chk_CustomModelAPI.CheckedChanged += Chk_CustomModelAPI_CheckedChanged;
            // 
            // label38
            // 
            label38.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            label38.ForeColor = System.Drawing.Color.DarkGray;
            label38.Location = new System.Drawing.Point(8, 113);
            label38.Name = "label38";
            label38.Size = new System.Drawing.Size(469, 43);
            label38.TabIndex = 2;
            label38.Text = "If needed, specify more than one custom model instance by supplying an equal number of items for PATH/NAME/PORT.    For example PATH=C:\\PTH1, C:\\PTH2, NAME=DOGS,CATS, PORT=82,83";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new System.Drawing.Point(3, 94);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(51, 13);
            label9.TabIndex = 1;
            label9.Text = "Mode(s):";
            // 
            // label37
            // 
            label37.AutoSize = true;
            label37.Location = new System.Drawing.Point(11, 70);
            label37.Name = "label37";
            label37.Size = new System.Drawing.Size(42, 13);
            label37.TabIndex = 1;
            label37.Text = "Port(s):";
            // 
            // label36
            // 
            label36.AutoSize = true;
            label36.Location = new System.Drawing.Point(2, 46);
            label36.Name = "label36";
            label36.Size = new System.Drawing.Size(50, 13);
            label36.TabIndex = 1;
            label36.Text = "Name(s):";
            // 
            // label35
            // 
            label35.AutoSize = true;
            label35.Location = new System.Drawing.Point(8, 21);
            label35.Name = "label35";
            label35.Size = new System.Drawing.Size(44, 13);
            label35.TabIndex = 1;
            label35.Text = "Path(s):";
            // 
            // Txt_CustomModelMode
            // 
            Txt_CustomModelMode.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            Txt_CustomModelMode.Location = new System.Drawing.Point(56, 91);
            Txt_CustomModelMode.Margin = new System.Windows.Forms.Padding(2);
            Txt_CustomModelMode.Name = "Txt_CustomModelMode";
            Txt_CustomModelMode.Size = new System.Drawing.Size(423, 22);
            Txt_CustomModelMode.TabIndex = 3;
            Txt_CustomModelMode.TextChanged += Txt_CustomModelName_TextChanged;
            // 
            // Txt_CustomModelPort
            // 
            Txt_CustomModelPort.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            Txt_CustomModelPort.Location = new System.Drawing.Point(56, 67);
            Txt_CustomModelPort.Margin = new System.Windows.Forms.Padding(2);
            Txt_CustomModelPort.Name = "Txt_CustomModelPort";
            Txt_CustomModelPort.Size = new System.Drawing.Size(423, 22);
            Txt_CustomModelPort.TabIndex = 3;
            Txt_CustomModelPort.TextChanged += Txt_CustomModelName_TextChanged;
            // 
            // Txt_CustomModelName
            // 
            Txt_CustomModelName.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            Txt_CustomModelName.Location = new System.Drawing.Point(56, 43);
            Txt_CustomModelName.Margin = new System.Windows.Forms.Padding(2);
            Txt_CustomModelName.Name = "Txt_CustomModelName";
            Txt_CustomModelName.Size = new System.Drawing.Size(423, 22);
            Txt_CustomModelName.TabIndex = 2;
            toolTip1.SetToolTip(Txt_CustomModelName, "The custom model name");
            Txt_CustomModelName.TextChanged += Txt_CustomModelName_TextChanged;
            // 
            // Txt_CustomModelPath
            // 
            Txt_CustomModelPath.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            Txt_CustomModelPath.Location = new System.Drawing.Point(56, 18);
            Txt_CustomModelPath.Margin = new System.Windows.Forms.Padding(2);
            Txt_CustomModelPath.Name = "Txt_CustomModelPath";
            Txt_CustomModelPath.Size = new System.Drawing.Size(423, 22);
            Txt_CustomModelPath.TabIndex = 1;
            toolTip1.SetToolTip(Txt_CustomModelPath, "The custom model path not including filename");
            // 
            // tabLog
            // 
            tabLog.Controls.Add(toolStrip2);
            tabLog.Controls.Add(groupBox7);
            tabLog.Location = new System.Drawing.Point(4, 24);
            tabLog.Margin = new System.Windows.Forms.Padding(2);
            tabLog.Name = "tabLog";
            tabLog.Size = new System.Drawing.Size(1099, 492);
            tabLog.TabIndex = 7;
            tabLog.Text = "Log";
            tabLog.UseVisualStyleBackColor = true;
            tabLog.Click += tabLog_Click;
            // 
            // toolStrip2
            // 
            toolStrip2.ImageScalingSize = new System.Drawing.Size(24, 24);
            toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripLabel1, ToolStripComboBoxSearch, toolStripDropDownButton1, toolStripSeparator3, toolStripSeparator, toolStripDropDownButtonSettings, toolStripSeparator4, toolStripSeparator2, openToolStripButton, toolStripButtonLoad, toolStripSeparator7, toolStripButtonReload, toolStripSeparator5, toolStripButtonPauseLog, toolStripSeparator6, toolStripSeparator8, chk_filterErrors, chk_filterErrorsAll });
            toolStrip2.Location = new System.Drawing.Point(0, 0);
            toolStrip2.Name = "toolStrip2";
            toolStrip2.Size = new System.Drawing.Size(1099, 31);
            toolStrip2.TabIndex = 6;
            toolStrip2.Text = "toolStrip2";
            // 
            // toolStripLabel1
            // 
            toolStripLabel1.Name = "toolStripLabel1";
            toolStripLabel1.Size = new System.Drawing.Size(45, 28);
            toolStripLabel1.Text = "Search:";
            // 
            // ToolStripComboBoxSearch
            // 
            ToolStripComboBoxSearch.BackColor = System.Drawing.SystemColors.Info;
            ToolStripComboBoxSearch.ForeColor = System.Drawing.Color.FromArgb(0, 0, 192);
            ToolStripComboBoxSearch.Items.AddRange(new object[] { "person", "error|warn|fatal|problem|exception", "this | orthat", "imagefilename.jpg | key=1234" });
            ToolStripComboBoxSearch.Name = "ToolStripComboBoxSearch";
            ToolStripComboBoxSearch.Size = new System.Drawing.Size(200, 31);
            ToolStripComboBoxSearch.ToolTipText = "The search can be normal text OR a valid 'RegEx' statement.\r\n";
            ToolStripComboBoxSearch.Leave += ToolStripComboBoxSearch_Leave;
            ToolStripComboBoxSearch.TextChanged += ToolStripComboBoxSearch_TextChanged;
            // 
            // toolStripDropDownButton1
            // 
            toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { mnu_Filter, mnu_Highlight });
            toolStripDropDownButton1.Image = (System.Drawing.Image)resources.GetObject("toolStripDropDownButton1.Image");
            toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            toolStripDropDownButton1.Size = new System.Drawing.Size(37, 28);
            toolStripDropDownButton1.Text = "Filter or highlight search box";
            // 
            // mnu_Filter
            // 
            mnu_Filter.CheckOnClick = true;
            mnu_Filter.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            mnu_Filter.Name = "mnu_Filter";
            mnu_Filter.Size = new System.Drawing.Size(124, 22);
            mnu_Filter.Text = "Filter";
            mnu_Filter.CheckStateChanged += mnu_Filter_CheckStateChanged;
            mnu_Filter.Click += mnu_Filter_Click;
            // 
            // mnu_Highlight
            // 
            mnu_Highlight.CheckOnClick = true;
            mnu_Highlight.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            mnu_Highlight.Name = "mnu_Highlight";
            mnu_Highlight.Size = new System.Drawing.Size(124, 22);
            mnu_Highlight.Text = "Highlight";
            mnu_Highlight.CheckStateChanged += mnu_highlight_CheckStateChanged;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new System.Drawing.Size(6, 31);
            // 
            // toolStripSeparator
            // 
            toolStripSeparator.Name = "toolStripSeparator";
            toolStripSeparator.Size = new System.Drawing.Size(6, 31);
            // 
            // toolStripDropDownButtonSettings
            // 
            toolStripDropDownButtonSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { Chk_AutoScroll, clearRecentErrorsToolStripMenuItem, toolStripMenuItemLogLevel });
            toolStripDropDownButtonSettings.Image = (System.Drawing.Image)resources.GetObject("toolStripDropDownButtonSettings.Image");
            toolStripDropDownButtonSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripDropDownButtonSettings.Name = "toolStripDropDownButtonSettings";
            toolStripDropDownButtonSettings.Size = new System.Drawing.Size(109, 28);
            toolStripDropDownButtonSettings.Text = "Log Settings";
            // 
            // Chk_AutoScroll
            // 
            Chk_AutoScroll.CheckOnClick = true;
            Chk_AutoScroll.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            Chk_AutoScroll.Name = "Chk_AutoScroll";
            Chk_AutoScroll.Size = new System.Drawing.Size(204, 22);
            Chk_AutoScroll.Text = "Auto Scroll";
            Chk_AutoScroll.Click += Chk_AutoScroll_Click_1;
            // 
            // clearRecentErrorsToolStripMenuItem
            // 
            clearRecentErrorsToolStripMenuItem.Name = "clearRecentErrorsToolStripMenuItem";
            clearRecentErrorsToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            clearRecentErrorsToolStripMenuItem.Text = "Clear Recent Error Count";
            clearRecentErrorsToolStripMenuItem.Click += clearRecentErrorsToolStripMenuItem_Click;
            // 
            // toolStripMenuItemLogLevel
            // 
            toolStripMenuItemLogLevel.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { mnu_log_filter_off, mnu_log_filter_fatal, mnu_log_filter_error, mnu_log_filter_warn, mnu_log_filter_info, mnu_log_filter_debug, mnu_log_filter_trace });
            toolStripMenuItemLogLevel.Name = "toolStripMenuItemLogLevel";
            toolStripMenuItemLogLevel.Size = new System.Drawing.Size(204, 22);
            toolStripMenuItemLogLevel.Text = "Logging Level";
            // 
            // mnu_log_filter_off
            // 
            mnu_log_filter_off.CheckOnClick = true;
            mnu_log_filter_off.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            mnu_log_filter_off.Name = "mnu_log_filter_off";
            mnu_log_filter_off.Size = new System.Drawing.Size(109, 22);
            mnu_log_filter_off.Text = "Off";
            mnu_log_filter_off.CheckStateChanged += mnu_log_filter_off_CheckStateChanged;
            mnu_log_filter_off.Click += mnu_log_filter_off_Click;
            // 
            // mnu_log_filter_fatal
            // 
            mnu_log_filter_fatal.CheckOnClick = true;
            mnu_log_filter_fatal.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            mnu_log_filter_fatal.Name = "mnu_log_filter_fatal";
            mnu_log_filter_fatal.Size = new System.Drawing.Size(109, 22);
            mnu_log_filter_fatal.Text = "Fatal";
            mnu_log_filter_fatal.CheckStateChanged += mnu_log_filter_fatal_CheckStateChanged;
            // 
            // mnu_log_filter_error
            // 
            mnu_log_filter_error.CheckOnClick = true;
            mnu_log_filter_error.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            mnu_log_filter_error.Name = "mnu_log_filter_error";
            mnu_log_filter_error.Size = new System.Drawing.Size(109, 22);
            mnu_log_filter_error.Text = "Error";
            mnu_log_filter_error.CheckStateChanged += mnu_log_filter_error_CheckStateChanged;
            // 
            // mnu_log_filter_warn
            // 
            mnu_log_filter_warn.CheckOnClick = true;
            mnu_log_filter_warn.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            mnu_log_filter_warn.Name = "mnu_log_filter_warn";
            mnu_log_filter_warn.Size = new System.Drawing.Size(109, 22);
            mnu_log_filter_warn.Text = "Warn";
            mnu_log_filter_warn.CheckStateChanged += mnu_log_filter_warn_CheckStateChanged;
            // 
            // mnu_log_filter_info
            // 
            mnu_log_filter_info.CheckOnClick = true;
            mnu_log_filter_info.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            mnu_log_filter_info.Name = "mnu_log_filter_info";
            mnu_log_filter_info.Size = new System.Drawing.Size(109, 22);
            mnu_log_filter_info.Text = "Info";
            mnu_log_filter_info.CheckStateChanged += mnu_log_filter_info_CheckStateChanged;
            // 
            // mnu_log_filter_debug
            // 
            mnu_log_filter_debug.CheckOnClick = true;
            mnu_log_filter_debug.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            mnu_log_filter_debug.Name = "mnu_log_filter_debug";
            mnu_log_filter_debug.Size = new System.Drawing.Size(109, 22);
            mnu_log_filter_debug.Text = "Debug";
            mnu_log_filter_debug.CheckStateChanged += mnu_log_filter_debug_CheckStateChanged;
            // 
            // mnu_log_filter_trace
            // 
            mnu_log_filter_trace.CheckOnClick = true;
            mnu_log_filter_trace.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            mnu_log_filter_trace.Name = "mnu_log_filter_trace";
            mnu_log_filter_trace.Size = new System.Drawing.Size(109, 22);
            mnu_log_filter_trace.Text = "Trace";
            mnu_log_filter_trace.CheckStateChanged += mnu_log_filter_trace_CheckStateChanged;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new System.Drawing.Size(6, 31);
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new System.Drawing.Size(6, 31);
            // 
            // openToolStripButton
            // 
            openToolStripButton.Image = (System.Drawing.Image)resources.GetObject("openToolStripButton.Image");
            openToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            openToolStripButton.Name = "openToolStripButton";
            openToolStripButton.Size = new System.Drawing.Size(64, 28);
            openToolStripButton.Text = "Open";
            openToolStripButton.ToolTipText = "Open Log File in external editor";
            openToolStripButton.Click += openToolStripButton_Click;
            // 
            // toolStripButtonLoad
            // 
            toolStripButtonLoad.Image = (System.Drawing.Image)resources.GetObject("toolStripButtonLoad.Image");
            toolStripButtonLoad.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButtonLoad.Name = "toolStripButtonLoad";
            toolStripButtonLoad.Size = new System.Drawing.Size(61, 28);
            toolStripButtonLoad.Text = "Load";
            toolStripButtonLoad.ToolTipText = "Load a specific log file into the list";
            toolStripButtonLoad.Click += toolStripButtonLoad_Click;
            // 
            // toolStripSeparator7
            // 
            toolStripSeparator7.Name = "toolStripSeparator7";
            toolStripSeparator7.Size = new System.Drawing.Size(6, 31);
            // 
            // toolStripButtonReload
            // 
            toolStripButtonReload.Image = (System.Drawing.Image)resources.GetObject("toolStripButtonReload.Image");
            toolStripButtonReload.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButtonReload.Name = "toolStripButtonReload";
            toolStripButtonReload.Size = new System.Drawing.Size(71, 28);
            toolStripButtonReload.Text = "Reload";
            toolStripButtonReload.ToolTipText = "Reloads the entire current log file from file without limiting the max number of lines.  \r\nUse this after loading other files or filtering to reset view";
            toolStripButtonReload.Click += toolStripButtonReload_ClickAsync;
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new System.Drawing.Size(6, 31);
            // 
            // toolStripButtonPauseLog
            // 
            toolStripButtonPauseLog.CheckOnClick = true;
            toolStripButtonPauseLog.Image = (System.Drawing.Image)resources.GetObject("toolStripButtonPauseLog.Image");
            toolStripButtonPauseLog.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButtonPauseLog.Name = "toolStripButtonPauseLog";
            toolStripButtonPauseLog.Size = new System.Drawing.Size(66, 28);
            toolStripButtonPauseLog.Text = "Pause";
            toolStripButtonPauseLog.ToolTipText = "Pause log tab auto refresh";
            toolStripButtonPauseLog.Click += toolStripButtonPauseLog_Click;
            // 
            // toolStripSeparator6
            // 
            toolStripSeparator6.Name = "toolStripSeparator6";
            toolStripSeparator6.Size = new System.Drawing.Size(6, 31);
            // 
            // toolStripSeparator8
            // 
            toolStripSeparator8.Name = "toolStripSeparator8";
            toolStripSeparator8.Size = new System.Drawing.Size(6, 31);
            // 
            // chk_filterErrors
            // 
            chk_filterErrors.CheckOnClick = true;
            chk_filterErrors.Image = (System.Drawing.Image)resources.GetObject("chk_filterErrors.Image");
            chk_filterErrors.ImageTransparentColor = System.Drawing.Color.Magenta;
            chk_filterErrors.Name = "chk_filterErrors";
            chk_filterErrors.Size = new System.Drawing.Size(65, 28);
            chk_filterErrors.Text = "Errors";
            chk_filterErrors.ToolTipText = "Show errors from current loaded log";
            chk_filterErrors.Click += chk_filterErrors_Click_1;
            // 
            // chk_filterErrorsAll
            // 
            chk_filterErrorsAll.CheckOnClick = true;
            chk_filterErrorsAll.Image = (System.Drawing.Image)resources.GetObject("chk_filterErrorsAll.Image");
            chk_filterErrorsAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            chk_filterErrorsAll.Name = "chk_filterErrorsAll";
            chk_filterErrorsAll.Size = new System.Drawing.Size(90, 28);
            chk_filterErrorsAll.Text = "Errors (All)";
            chk_filterErrorsAll.ToolTipText = "Show errors from ALL logs";
            chk_filterErrorsAll.Click += chk_filterErrorsAll_Click;
            // 
            // groupBox7
            // 
            groupBox7.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            groupBox7.Controls.Add(folv_log);
            groupBox7.Location = new System.Drawing.Point(5, 36);
            groupBox7.Margin = new System.Windows.Forms.Padding(2);
            groupBox7.Name = "groupBox7";
            groupBox7.Padding = new System.Windows.Forms.Padding(2);
            groupBox7.Size = new System.Drawing.Size(1092, 443);
            groupBox7.TabIndex = 5;
            groupBox7.TabStop = false;
            groupBox7.Text = "Log";
            // 
            // folv_log
            // 
            folv_log.BackColor = System.Drawing.Color.Navy;
            folv_log.Dock = System.Windows.Forms.DockStyle.Fill;
            folv_log.ForeColor = System.Drawing.Color.White;
            folv_log.Location = new System.Drawing.Point(2, 17);
            folv_log.Name = "folv_log";
            folv_log.ShowGroups = false;
            folv_log.Size = new System.Drawing.Size(1088, 424);
            folv_log.TabIndex = 0;
            folv_log.UseCompatibleStateImageBehavior = false;
            folv_log.View = System.Windows.Forms.View.Details;
            folv_log.VirtualMode = true;
            folv_log.FormatCell += folv_log_FormatCell;
            folv_log.FormatRow += folv_log_FormatRow;
            // 
            // HistoryImageList
            // 
            HistoryImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            HistoryImageList.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("HistoryImageList.ImageStream");
            HistoryImageList.TransparentColor = System.Drawing.Color.Transparent;
            HistoryImageList.Images.SetKeyName(0, "error16");
            HistoryImageList.Images.SetKeyName(1, "person16");
            HistoryImageList.Images.SetKeyName(2, "nothing16");
            HistoryImageList.Images.SetKeyName(3, "detection16");
            HistoryImageList.Images.SetKeyName(4, "success16");
            HistoryImageList.Images.SetKeyName(5, "bear16");
            HistoryImageList.Images.SetKeyName(6, "cat16");
            HistoryImageList.Images.SetKeyName(7, "dog16");
            HistoryImageList.Images.SetKeyName(8, "horse16");
            HistoryImageList.Images.SetKeyName(9, "bird16");
            HistoryImageList.Images.SetKeyName(10, "alien16");
            HistoryImageList.Images.SetKeyName(11, "cow16");
            HistoryImageList.Images.SetKeyName(12, "car16");
            HistoryImageList.Images.SetKeyName(13, "truck16");
            HistoryImageList.Images.SetKeyName(14, "motorcycle16");
            HistoryImageList.Images.SetKeyName(15, "bicycle32.png");
            HistoryImageList.Images.SetKeyName(16, "airplane.png");
            HistoryImageList.Images.SetKeyName(17, "bear.png");
            HistoryImageList.Images.SetKeyName(18, "bicycle.png");
            HistoryImageList.Images.SetKeyName(19, "bird.png");
            HistoryImageList.Images.SetKeyName(20, "boat.png");
            HistoryImageList.Images.SetKeyName(21, "bus.png");
            HistoryImageList.Images.SetKeyName(22, "car.png");
            HistoryImageList.Images.SetKeyName(23, "cat.png");
            HistoryImageList.Images.SetKeyName(24, "cow.png");
            HistoryImageList.Images.SetKeyName(25, "dog.png");
            HistoryImageList.Images.SetKeyName(26, "horse.png");
            HistoryImageList.Images.SetKeyName(27, "motorcycle.png");
            HistoryImageList.Images.SetKeyName(28, "person.png");
            HistoryImageList.Images.SetKeyName(29, "sheep.png");
            HistoryImageList.Images.SetKeyName(30, "truck.png");
            HistoryImageList.Images.SetKeyName(31, "error32.png");
            HistoryImageList.Images.SetKeyName(32, "person32.png");
            HistoryImageList.Images.SetKeyName(33, "nothing32.png");
            HistoryImageList.Images.SetKeyName(34, "success32.png");
            HistoryImageList.Images.SetKeyName(35, "detection32.png");
            HistoryImageList.Images.SetKeyName(36, "bear32.png");
            HistoryImageList.Images.SetKeyName(37, "cat32.png");
            HistoryImageList.Images.SetKeyName(38, "dog32.png");
            HistoryImageList.Images.SetKeyName(39, "horse32.png");
            HistoryImageList.Images.SetKeyName(40, "bird32.png");
            HistoryImageList.Images.SetKeyName(41, "alien32.png");
            HistoryImageList.Images.SetKeyName(42, "cow32.png");
            HistoryImageList.Images.SetKeyName(43, "car32.png");
            HistoryImageList.Images.SetKeyName(44, "truck32.png");
            HistoryImageList.Images.SetKeyName(45, "motorcycle32.png");
            // 
            // HistoryUpdateListTimer
            // 
            HistoryUpdateListTimer.Interval = 3000;
            HistoryUpdateListTimer.Tick += HistoryUpdateListTimer_Tick;
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new System.Drawing.Size(28, 28);
            statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripProgressBar1, toolStripStatusLabelHistoryItems, toolStripStatusErrors, toolStripStatusLabel1, toolStripStatusLabelInfo });
            statusStrip1.Location = new System.Drawing.Point(0, 527);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 8, 0);
            statusStrip1.Size = new System.Drawing.Size(1107, 22);
            statusStrip1.TabIndex = 5;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripProgressBar1
            // 
            toolStripProgressBar1.Name = "toolStripProgressBar1";
            toolStripProgressBar1.Size = new System.Drawing.Size(100, 29);
            toolStripProgressBar1.Visible = false;
            // 
            // toolStripStatusLabelHistoryItems
            // 
            toolStripStatusLabelHistoryItems.ForeColor = System.Drawing.Color.DodgerBlue;
            toolStripStatusLabelHistoryItems.Name = "toolStripStatusLabelHistoryItems";
            toolStripStatusLabelHistoryItems.Size = new System.Drawing.Size(86, 17);
            toolStripStatusLabelHistoryItems.Text = "0 History Items";
            // 
            // toolStripStatusErrors
            // 
            toolStripStatusErrors.Name = "toolStripStatusErrors";
            toolStripStatusErrors.Size = new System.Drawing.Size(10, 17);
            toolStripStatusErrors.Text = ".";
            toolStripStatusErrors.Click += toolStripStatusErrors_Click;
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new System.Drawing.Size(10, 17);
            toolStripStatusLabel1.Text = ".";
            // 
            // toolStripStatusLabelInfo
            // 
            toolStripStatusLabelInfo.ForeColor = System.Drawing.Color.DarkOrange;
            toolStripStatusLabelInfo.Name = "toolStripStatusLabelInfo";
            toolStripStatusLabelInfo.Size = new System.Drawing.Size(26, 17);
            toolStripStatusLabelInfo.Text = "Idle";
            // 
            // LogUpdateListTimer
            // 
            LogUpdateListTimer.Interval = 2000;
            LogUpdateListTimer.Tick += LogUpdateListTimer_Tick;
            // 
            // Shell
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1107, 549);
            Controls.Add(tabControl1);
            Controls.Add(statusStrip1);
            Font = new System.Drawing.Font("Segoe UI", 8.25F);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MinimumSize = new System.Drawing.Size(755, 440);
            Name = "Shell";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Tag = "SAVE";
            Text = "AI Tool";
            Activated += Shell_Activated;
            FormClosing += Shell_FormClosing;
            Load += Shell_Load;
            DpiChanged += Shell_DpiChanged;
            Resize += Form1_Resize;
            TraycontextMenuStrip.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            tabOverview.ResumeLayout(false);
            tableLayoutPanel14.ResumeLayout(false);
            tableLayoutPanel15.ResumeLayout(false);
            tableLayoutPanel15.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            tabStats.ResumeLayout(false);
            tableLayoutPanel16.ResumeLayout(false);
            tableLayoutPanel23.ResumeLayout(false);
            tableLayoutPanel23.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)chart_confidence).EndInit();
            ((System.ComponentModel.ISupportInitialize)timeline).EndInit();
            tableLayoutPanel17.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)chart1).EndInit();
            tabHistory.ResumeLayout(false);
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            groupBox8.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)folv_history).EndInit();
            contextMenuStripHistory.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            toolStripContainer1.ResumeLayout(false);
            toolStripContainer1.PerformLayout();
            tabCameras.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            splitContainer3.Panel1.ResumeLayout(false);
            splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer3).EndInit();
            splitContainer3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)FOLV_Cameras).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxCamera).EndInit();
            tableLayoutPanel6.ResumeLayout(false);
            tableLayoutPanel6.PerformLayout();
            tableLayoutPanel11.ResumeLayout(false);
            tableLayoutPanel7.ResumeLayout(false);
            tableLayoutPanel7.PerformLayout();
            tableLayoutPanel12.ResumeLayout(false);
            tableLayoutPanel12.PerformLayout();
            tableLayoutPanel13.ResumeLayout(false);
            tableLayoutPanel13.PerformLayout();
            tableLayoutPanel26.ResumeLayout(false);
            tableLayoutPanel26.PerformLayout();
            tableLayoutPanel27.ResumeLayout(false);
            tableLayoutPanel27.PerformLayout();
            dbLayoutPanel6.ResumeLayout(false);
            dbLayoutPanel6.PerformLayout();
            dbLayoutPanel7.ResumeLayout(false);
            dbLayoutPanel7.PerformLayout();
            dbLayoutPanel11.ResumeLayout(false);
            dbLayoutPanel11.PerformLayout();
            dbLayoutPanel12.ResumeLayout(false);
            dbLayoutPanel12.PerformLayout();
            dbLayoutPanel13.ResumeLayout(false);
            dbLayoutPanel13.PerformLayout();
            tabSettings.ResumeLayout(false);
            tableLayoutPanel4.ResumeLayout(false);
            tableLayoutPanel5.ResumeLayout(false);
            tableLayoutPanel5.PerformLayout();
            tableLayoutPanel18.ResumeLayout(false);
            tableLayoutPanel18.PerformLayout();
            dbLayoutPanel1.ResumeLayout(false);
            dbLayoutPanel1.PerformLayout();
            dbLayoutPanel2.ResumeLayout(false);
            dbLayoutPanel2.PerformLayout();
            dbLayoutPanel3.ResumeLayout(false);
            dbLayoutPanel3.PerformLayout();
            dbLayoutPanel4.ResumeLayout(false);
            dbLayoutPanel4.PerformLayout();
            dbLayoutPanel5.ResumeLayout(false);
            dbLayoutPanel5.PerformLayout();
            dbLayoutPanel8.ResumeLayout(false);
            dbLayoutPanel8.PerformLayout();
            dbLayoutPanel9.ResumeLayout(false);
            dbLayoutPanel9.PerformLayout();
            dbLayoutPanel10.ResumeLayout(false);
            tabDeepStack.ResumeLayout(false);
            tabDeepStack.PerformLayout();
            groupBox11.ResumeLayout(false);
            groupBox11.PerformLayout();
            groupBox10.ResumeLayout(false);
            groupBox10.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            groupBoxCustomModel.ResumeLayout(false);
            groupBoxCustomModel.PerformLayout();
            tabLog.ResumeLayout(false);
            tabLog.PerformLayout();
            toolStrip2.ResumeLayout(false);
            toolStrip2.PerformLayout();
            groupBox7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)folv_log).EndInit();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabHistory;
        private System.Windows.Forms.TabPage tabCameras;
        private System.Windows.Forms.TabPage tabSettings;
        private DBLayoutPanel tableLayoutPanel4;
        private DBLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.Label lbl_deepstackurl;
        private System.Windows.Forms.Label lbl_input;
        private System.Windows.Forms.Button BtnSettingsSave;
        private System.Windows.Forms.Button btnCameraAdd;
        private DBLayoutPanel tableLayoutPanel7;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblPrefix;
        private System.Windows.Forms.Label lblRelevantObjects;
        private System.Windows.Forms.Label lblName;
        private DBLayoutPanel tableLayoutPanel11;
        private System.Windows.Forms.Button btnCameraSave;
        private System.Windows.Forms.Button btnCameraDel;
        private DBLayoutPanel tableLayoutPanel12;
        private System.Windows.Forms.TextBox tbPrefix;
        private System.Windows.Forms.Label lbl_prefix;
        private DBLayoutPanel tableLayoutPanel13;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.CheckBox cb_enabled;
        private System.Windows.Forms.Label lbl_telegram_token;
        private System.Windows.Forms.Label lbl_telegram_chatid;
        private System.Windows.Forms.TabPage tabOverview;
        private System.Windows.Forms.Label lbl_camstats;
        private DBLayoutPanel tableLayoutPanel15;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TabPage tabStats;
        private DBLayoutPanel tableLayoutPanel16;
        private DBLayoutPanel tableLayoutPanel17;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label lbl_version;
        private System.Windows.Forms.Label lbl_errors;
        private DBLayoutPanel tableLayoutPanel18;
        private System.Windows.Forms.Button btn_input_path;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lbl_info;
        private DBLayoutPanel tableLayoutPanel23;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart_confidence;
        private System.Windows.Forms.DataVisualization.Charting.Chart timeline;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.CheckBox cb_send_telegram_errors;
        private System.Windows.Forms.Label lbl_objects;
        private DBLayoutPanel tableLayoutPanel6;
        private DBLayoutPanel tableLayoutPanel14;
        private System.Windows.Forms.TabPage tabDeepStack;
        private System.Windows.Forms.CheckBox Chk_AutoStart;
        private System.Windows.Forms.Button Btn_Stop;
        private System.Windows.Forms.Button Btn_Start;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox Txt_Port;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton RB_High;
        private System.Windows.Forms.RadioButton RB_Medium;
        private System.Windows.Forms.RadioButton RB_Low;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox Chk_DetectionAPI;
        private System.Windows.Forms.CheckBox Chk_FaceAPI;
        private System.Windows.Forms.CheckBox Chk_SceneAPI;
        private System.Windows.Forms.TextBox Txt_DeepStackInstallFolder;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label Lbl_BlueStackRunning;
        private System.Windows.Forms.Button Btn_Save;
        private System.Windows.Forms.TabPage tabLog;
        private System.Windows.Forms.ComboBox cmbInput;
        private System.Windows.Forms.CheckBox cbStartWithWindows;
        private System.Windows.Forms.CheckBox cb_inputpathsubfolders;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel26;
        private System.Windows.Forms.ComboBox cmbcaminput;
        private System.Windows.Forms.CheckBox cb_monitorCamInputfolder;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckBox Chk_DSDebug;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox chk_HighPriority;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel27;
        protected internal System.Windows.Forms.CheckBox cb_masking_enabled;
        private System.Windows.Forms.Button BtnDynamicMaskingSettings;
        private System.Windows.Forms.Button btnDetails;
        private System.Windows.Forms.Label lblQueue;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Button btnCustomMask;
        private System.Windows.Forms.Label lblDrawMask;
        private System.Windows.Forms.Button btnActions;
        private DBLayoutPanel dbLayoutPanel1;
        private System.Windows.Forms.CheckBox cb_DeepStackURLsQueued;
        private DBLayoutPanel dbLayoutPanel2;
        private System.Windows.Forms.TextBox tb_telegram_cooldown;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tb_telegram_chatid;
        private System.Windows.Forms.TextBox tb_telegram_token;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private BrightIdeasSoftware.FastObjectListView folv_history;
        private System.Windows.Forms.Button btn_resetstats;
        private System.Windows.Forms.ImageList HistoryImageList;
        private System.Windows.Forms.Timer HistoryUpdateListTimer;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelHistoryItems;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusErrors;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripComboBox comboBox_filter_camera;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButtonFilters;
        private System.Windows.Forms.ToolStripMenuItem cb_filter_success;
        private System.Windows.Forms.ToolStripMenuItem cb_filter_nosuccess;
        private System.Windows.Forms.ToolStripMenuItem cb_filter_person;
        private System.Windows.Forms.ToolStripMenuItem cb_filter_animal;
        private System.Windows.Forms.ToolStripMenuItem cb_filter_vehicle;
        private System.Windows.Forms.ToolStripMenuItem cb_filter_skipped;
        private System.Windows.Forms.ToolStripMenuItem cb_filter_masked;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButtonOptions;
        private System.Windows.Forms.ToolStripMenuItem cb_showMask;
        private System.Windows.Forms.ToolStripMenuItem cb_showObjects;
        private System.Windows.Forms.ToolStripMenuItem cb_follow;
        private System.Windows.Forms.ToolStripMenuItem automaticallyRefreshToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelInfo;
        private System.Windows.Forms.ToolStripButton toolStripButtonDetails;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripHistory;
        private System.Windows.Forms.ToolStripMenuItem testDetectionAgainToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem detailsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButtonMaskDetails;
        private System.Windows.Forms.ToolStripMenuItem dynamicMaskDetailsToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButtonEditImageMask;
        private DBLayoutPanel dbLayoutPanel3;
        private System.Windows.Forms.ToolStripMenuItem storeFalseAlertsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem storeMaskedAlertsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showOnlyRelevantObjectsToolStripMenuItem;
        private System.Windows.Forms.Button btnSaveTo;
        private BrightIdeasSoftware.FastObjectListView folv_log;
        private System.Windows.Forms.Timer LogUpdateListTimer;
        private System.Windows.Forms.ToolStripMenuItem locateInLogToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox ToolStripComboBoxSearch;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem mnu_Filter;
        private System.Windows.Forms.ToolStripMenuItem mnu_Highlight;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButtonSettings;
        private System.Windows.Forms.ToolStripMenuItem Chk_AutoScroll;
        private System.Windows.Forms.ToolStripMenuItem clearRecentErrorsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemLogLevel;
        private System.Windows.Forms.ToolStripMenuItem mnu_log_filter_off;
        private System.Windows.Forms.ToolStripMenuItem mnu_log_filter_fatal;
        private System.Windows.Forms.ToolStripMenuItem mnu_log_filter_error;
        private System.Windows.Forms.ToolStripMenuItem mnu_log_filter_warn;
        private System.Windows.Forms.ToolStripMenuItem mnu_log_filter_info;
        private System.Windows.Forms.ToolStripMenuItem mnu_log_filter_debug;
        private System.Windows.Forms.ToolStripMenuItem mnu_log_filter_trace;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton openToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripButton toolStripButtonReload;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripButton toolStripButtonPauseLog;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripButton toolStripButtonLoad;
        private System.Windows.Forms.ToolStripButton chk_filterErrors;
        private System.Windows.Forms.ToolStripButton chk_filterErrorsAll;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.Label label4;
        private DBLayoutPanel dbLayoutPanel4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tb_username;
        private System.Windows.Forms.TextBox tb_password;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private DBLayoutPanel dbLayoutPanel5;
        private System.Windows.Forms.TextBox tb_BlueIrisServer;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.ToolStripMenuItem manuallyAddImagesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem restrictThresholdAtSourceToolStripMenuItem;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox tb_camera_telegram_chatid;
        private System.Windows.Forms.Label lbl_Deepstackversion;
        private System.Windows.Forms.Label lbl_deepstackname;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.GroupBox groupBoxCustomModel;
        private System.Windows.Forms.TextBox Txt_CustomModelPath;
        private System.Windows.Forms.Label lbl_DeepstackType;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label25;
        private DBLayoutPanel dbLayoutPanel6;
        private System.Windows.Forms.TextBox tbBiCamName;
        private System.Windows.Forms.Label label26;
        private DBLayoutPanel dbLayoutPanel7;
        private System.Windows.Forms.TextBox tbCustomMaskFile;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.TextBox tb_DeepstackCommandLine;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.GroupBox groupBox11;
        private System.Windows.Forms.TextBox tb_DeepStackURLs;
        private System.Windows.Forms.Button Btn_DeepstackReset;
        private System.Windows.Forms.Button Btn_ViewLog;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label29;
        private DBLayoutPanel dbLayoutPanel8;
        private System.Windows.Forms.TextBox tb_Pushover_Cooldown;
        private System.Windows.Forms.TextBox tb_Pushover_APIKey;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.TextBox tb_Pushover_UserKey;
        private System.Windows.Forms.CheckBox chk_stopbeforestart;
        private System.Windows.Forms.CheckBox cb_send_pushover_errors;
        private DBLayoutPanel dbLayoutPanel9;
        private System.Windows.Forms.CheckBox cbMinimizeToTray;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.TextBox txt_DeepstackRestartFailCount;
        private System.Windows.Forms.CheckBox Chk_AutoReStart;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.TextBox txt_DeepstackNoMoreOftenThanMins;
        private System.Windows.Forms.TextBox Txt_CustomModelName;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.TextBox Txt_CustomModelPort;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.CheckBox Chk_CustomModelAPI;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripButton toolStripButtonEditURL;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private DBLayoutPanel dbLayoutPanel10;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ToolStripMenuItem viewImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem jumpToImageToolStripMenuItem;
        private System.Windows.Forms.Label label39;
        private System.Windows.Forms.Button BtnPredictionSize;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private BrightIdeasSoftware.FastObjectListView FOLV_Cameras;
        private System.Windows.Forms.ImageList CameraImageList;
        private DBLayoutPanel dbLayoutPanel11;
        private System.Windows.Forms.Button BtnRelevantObjects;
        private System.Windows.Forms.Label lbl_RelevantObjects;
        private DBLayoutPanel dbLayoutPanel12;
        private System.Windows.Forms.Label Lbl_PredictionTolerances;
        private DBLayoutPanel dbLayoutPanel13;
        private System.Windows.Forms.Label Lbl_Actions;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStripButton toolStripButtonAdjustAnno;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.PictureBox pictureBoxCamera;
        private System.Windows.Forms.Label lbl_blueirisserver;
        private System.Windows.Forms.Button bt_CheckUpdates;
        private System.Windows.Forms.CheckBox chk_AutoAdd;
        private System.Windows.Forms.ToolStripMenuItem mergeDuplicatePredictionsToolStripMenuItem;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox Txt_CustomModelMode;
        private System.Windows.Forms.ContextMenuStrip TraycontextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pauseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pauseAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resumeAllToolStripMenuItem;
    }
}

