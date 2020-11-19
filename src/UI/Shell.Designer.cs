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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Shell));
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series9 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series10 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea5 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series11 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series12 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series13 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series14 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series15 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea6 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series16 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint4 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(1D, 30D);
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint5 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(1D, 30D);
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint6 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(1D, 30D);
            System.Windows.Forms.DataVisualization.Charting.Title title2 = new System.Windows.Forms.DataVisualization.Charting.Title();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabOverview = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel14 = new AITool.DBLayoutPanel(this.components);
            this.tableLayoutPanel15 = new AITool.DBLayoutPanel(this.components);
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lbl_version = new System.Windows.Forms.Label();
            this.lbl_errors = new System.Windows.Forms.Label();
            this.lbl_info = new System.Windows.Forms.Label();
            this.lblQueue = new System.Windows.Forms.Label();
            this.tabStats = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel16 = new AITool.DBLayoutPanel(this.components);
            this.tableLayoutPanel23 = new AITool.DBLayoutPanel(this.components);
            this.label8 = new System.Windows.Forms.Label();
            this.chart_confidence = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.timeline = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.label7 = new System.Windows.Forms.Label();
            this.tableLayoutPanel17 = new AITool.DBLayoutPanel(this.components);
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.btn_resetstats = new System.Windows.Forms.Button();
            this.tabHistory = new System.Windows.Forms.TabPage();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.comboBox_filter_camera = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripDropDownButtonFilters = new System.Windows.Forms.ToolStripDropDownButton();
            this.cb_filter_success = new System.Windows.Forms.ToolStripMenuItem();
            this.cb_filter_nosuccess = new System.Windows.Forms.ToolStripMenuItem();
            this.cb_filter_person = new System.Windows.Forms.ToolStripMenuItem();
            this.cb_filter_animal = new System.Windows.Forms.ToolStripMenuItem();
            this.cb_filter_vehicle = new System.Windows.Forms.ToolStripMenuItem();
            this.cb_filter_skipped = new System.Windows.Forms.ToolStripMenuItem();
            this.cb_filter_masked = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripDropDownButtonOptions = new System.Windows.Forms.ToolStripDropDownButton();
            this.cb_showMask = new System.Windows.Forms.ToolStripMenuItem();
            this.cb_showObjects = new System.Windows.Forms.ToolStripMenuItem();
            this.showOnlyRelevantObjectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cb_follow = new System.Windows.Forms.ToolStripMenuItem();
            this.automaticallyRefreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.storeFalseAlertsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.storeMaskedAlertsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.restrictThresholdAtSourceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButtonDetails = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonMaskDetails = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonEditImageMask = new System.Windows.Forms.ToolStripButton();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.folv_history = new BrightIdeasSoftware.FastObjectListView();
            this.contextMenuStripHistory = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.testDetectionAgainToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.detailsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dynamicMaskDetailsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.locateInLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manuallyAddImagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lbl_objects = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tabCameras = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel2 = new AITool.DBLayoutPanel(this.components);
            this.tableLayoutPanel3 = new AITool.DBLayoutPanel(this.components);
            this.list2 = new System.Windows.Forms.ListView();
            this.tableLayoutPanel6 = new AITool.DBLayoutPanel(this.components);
            this.tableLayoutPanel11 = new AITool.DBLayoutPanel(this.components);
            this.btnCameraSave = new System.Windows.Forms.Button();
            this.btnCameraAdd = new System.Windows.Forms.Button();
            this.btnCameraDel = new System.Windows.Forms.Button();
            this.btnSaveTo = new System.Windows.Forms.Button();
            this.lbl_camstats = new System.Windows.Forms.Label();
            this.tableLayoutPanel7 = new AITool.DBLayoutPanel(this.components);
            this.label14 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblPrefix = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.tableLayoutPanel12 = new AITool.DBLayoutPanel(this.components);
            this.lbl_prefix = new System.Windows.Forms.Label();
            this.tbPrefix = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel13 = new AITool.DBLayoutPanel(this.components);
            this.cb_enabled = new System.Windows.Forms.CheckBox();
            this.tbName = new System.Windows.Forms.TextBox();
            this.lblRelevantObjects = new System.Windows.Forms.Label();
            this.lbl_threshold = new System.Windows.Forms.Label();
            this.tableLayoutPanel26 = new System.Windows.Forms.TableLayoutPanel();
            this.cmbcaminput = new System.Windows.Forms.ComboBox();
            this.cb_monitorCamInputfolder = new System.Windows.Forms.CheckBox();
            this.button2 = new System.Windows.Forms.Button();
            this.label15 = new System.Windows.Forms.Label();
            this.tableLayoutPanel27 = new System.Windows.Forms.TableLayoutPanel();
            this.cb_masking_enabled = new System.Windows.Forms.CheckBox();
            this.BtnDynamicMaskingSettings = new System.Windows.Forms.Button();
            this.btnDetails = new System.Windows.Forms.Button();
            this.btnCustomMask = new System.Windows.Forms.Button();
            this.lblDrawMask = new System.Windows.Forms.Label();
            this.btnActions = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.lbl_threshold_lower = new System.Windows.Forms.Label();
            this.tb_threshold_lower = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.lbl_threshold_upper = new System.Windows.Forms.Label();
            this.tb_threshold_upper = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.tableLayoutPanel8 = new AITool.DBLayoutPanel(this.components);
            this.cb_person = new System.Windows.Forms.CheckBox();
            this.cb_bicycle = new System.Windows.Forms.CheckBox();
            this.cb_motorcycle = new System.Windows.Forms.CheckBox();
            this.cb_bear = new System.Windows.Forms.CheckBox();
            this.cb_cow = new System.Windows.Forms.CheckBox();
            this.cb_sheep = new System.Windows.Forms.CheckBox();
            this.cb_horse = new System.Windows.Forms.CheckBox();
            this.cb_bird = new System.Windows.Forms.CheckBox();
            this.cb_dog = new System.Windows.Forms.CheckBox();
            this.cb_cat = new System.Windows.Forms.CheckBox();
            this.cb_airplane = new System.Windows.Forms.CheckBox();
            this.cb_boat = new System.Windows.Forms.CheckBox();
            this.cb_bus = new System.Windows.Forms.CheckBox();
            this.cb_truck = new System.Windows.Forms.CheckBox();
            this.cb_car = new System.Windows.Forms.CheckBox();
            this.label20 = new System.Windows.Forms.Label();
            this.tbAdditionalRelevantObjects = new System.Windows.Forms.TextBox();
            this.tabSettings = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel4 = new AITool.DBLayoutPanel(this.components);
            this.BtnSettingsSave = new System.Windows.Forms.Button();
            this.tableLayoutPanel5 = new AITool.DBLayoutPanel(this.components);
            this.lbl_input = new System.Windows.Forms.Label();
            this.lbl_telegram_token = new System.Windows.Forms.Label();
            this.tableLayoutPanel18 = new AITool.DBLayoutPanel(this.components);
            this.btn_input_path = new System.Windows.Forms.Button();
            this.cmbInput = new System.Windows.Forms.ComboBox();
            this.cb_inputpathsubfolders = new System.Windows.Forms.CheckBox();
            this.label13 = new System.Windows.Forms.Label();
            this.cbStartWithWindows = new System.Windows.Forms.CheckBox();
            this.lbl_deepstackurl = new System.Windows.Forms.Label();
            this.dbLayoutPanel1 = new AITool.DBLayoutPanel(this.components);
            this.tbDeepstackUrl = new System.Windows.Forms.TextBox();
            this.cb_DeepStackURLsQueued = new System.Windows.Forms.CheckBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.dbLayoutPanel4 = new AITool.DBLayoutPanel(this.components);
            this.label6 = new System.Windows.Forms.Label();
            this.tb_username = new System.Windows.Forms.TextBox();
            this.tb_password = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.dbLayoutPanel5 = new AITool.DBLayoutPanel(this.components);
            this.tb_BlueIrisServer = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.dbLayoutPanel2 = new AITool.DBLayoutPanel(this.components);
            this.tb_telegram_cooldown = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tb_telegram_chatid = new System.Windows.Forms.TextBox();
            this.lbl_telegram_chatid = new System.Windows.Forms.Label();
            this.tb_telegram_token = new System.Windows.Forms.TextBox();
            this.dbLayoutPanel3 = new AITool.DBLayoutPanel(this.components);
            this.cb_send_errors = new System.Windows.Forms.CheckBox();
            this.btn_enabletelegram = new System.Windows.Forms.Button();
            this.btn_disabletelegram = new System.Windows.Forms.Button();
            this.MinimizeBehaviour = new System.Windows.Forms.Label();
            this.minimizeToTrayCheckbox = new System.Windows.Forms.CheckBox();
            this.tabDeepStack = new System.Windows.Forms.TabPage();
            this.chk_HighPriority = new System.Windows.Forms.CheckBox();
            this.Chk_DSDebug = new System.Windows.Forms.CheckBox();
            this.Lbl_BlueStackRunning = new System.Windows.Forms.Label();
            this.Btn_Save = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.Txt_APIKey = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Chk_DetectionAPI = new System.Windows.Forms.CheckBox();
            this.Chk_FaceAPI = new System.Windows.Forms.CheckBox();
            this.Chk_SceneAPI = new System.Windows.Forms.CheckBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.Txt_AdminKey = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.RB_High = new System.Windows.Forms.RadioButton();
            this.RB_Medium = new System.Windows.Forms.RadioButton();
            this.RB_Low = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.Txt_DeepStackInstallFolder = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.Txt_Port = new System.Windows.Forms.TextBox();
            this.Chk_AutoStart = new System.Windows.Forms.CheckBox();
            this.Btn_Start = new System.Windows.Forms.Button();
            this.Btn_Stop = new System.Windows.Forms.Button();
            this.tabLog = new System.Windows.Forms.TabPage();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.ToolStripComboBoxSearch = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.mnu_Filter = new System.Windows.Forms.ToolStripMenuItem();
            this.mnu_Highlight = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripDropDownButtonSettings = new System.Windows.Forms.ToolStripDropDownButton();
            this.Chk_AutoScroll = new System.Windows.Forms.ToolStripMenuItem();
            this.clearRecentErrorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemLogLevel = new System.Windows.Forms.ToolStripMenuItem();
            this.mnu_log_filter_off = new System.Windows.Forms.ToolStripMenuItem();
            this.mnu_log_filter_fatal = new System.Windows.Forms.ToolStripMenuItem();
            this.mnu_log_filter_error = new System.Windows.Forms.ToolStripMenuItem();
            this.mnu_log_filter_warn = new System.Windows.Forms.ToolStripMenuItem();
            this.mnu_log_filter_info = new System.Windows.Forms.ToolStripMenuItem();
            this.mnu_log_filter_debug = new System.Windows.Forms.ToolStripMenuItem();
            this.mnu_log_filter_trace = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.openToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonLoad = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonReload = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonPauseLog = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.chk_filterErrors = new System.Windows.Forms.ToolStripButton();
            this.chk_filterErrorsAll = new System.Windows.Forms.ToolStripButton();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.folv_log = new BrightIdeasSoftware.FastObjectListView();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.HistoryImageList = new System.Windows.Forms.ImageList(this.components);
            this.HistoryUpdateListTimer = new System.Windows.Forms.Timer(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabelHistoryItems = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusErrors = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.LogUpdateListTimer = new System.Windows.Forms.Timer(this.components);
            this.tabControl1.SuspendLayout();
            this.tabOverview.SuspendLayout();
            this.tableLayoutPanel14.SuspendLayout();
            this.tableLayoutPanel15.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.tabStats.SuspendLayout();
            this.tableLayoutPanel16.SuspendLayout();
            this.tableLayoutPanel23.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart_confidence)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.timeline)).BeginInit();
            this.tableLayoutPanel17.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.tabHistory.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.groupBox8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.folv_history)).BeginInit();
            this.contextMenuStripHistory.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tabCameras.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            this.tableLayoutPanel11.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            this.tableLayoutPanel12.SuspendLayout();
            this.tableLayoutPanel13.SuspendLayout();
            this.tableLayoutPanel26.SuspendLayout();
            this.tableLayoutPanel27.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel8.SuspendLayout();
            this.tabSettings.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.tableLayoutPanel18.SuspendLayout();
            this.dbLayoutPanel1.SuspendLayout();
            this.dbLayoutPanel4.SuspendLayout();
            this.dbLayoutPanel5.SuspendLayout();
            this.dbLayoutPanel2.SuspendLayout();
            this.dbLayoutPanel3.SuspendLayout();
            this.tabDeepStack.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tabLog.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.folv_log)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon
            // 
            this.notifyIcon.BalloonTipText = "Running in the background";
            this.notifyIcon.BalloonTipTitle = "AI Tool";
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "AI Tool";
            this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseDoubleClick);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabOverview);
            this.tabControl1.Controls.Add(this.tabStats);
            this.tabControl1.Controls.Add(this.tabHistory);
            this.tabControl1.Controls.Add(this.tabCameras);
            this.tabControl1.Controls.Add(this.tabSettings);
            this.tabControl1.Controls.Add(this.tabDeepStack);
            this.tabControl1.Controls.Add(this.tabLog);
            this.tabControl1.Location = new System.Drawing.Point(0, 2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1035, 481);
            this.tabControl1.TabIndex = 4;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            this.tabControl1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tabControl1_MouseDown);
            // 
            // tabOverview
            // 
            this.tabOverview.Controls.Add(this.tableLayoutPanel14);
            this.tabOverview.Location = new System.Drawing.Point(4, 22);
            this.tabOverview.Name = "tabOverview";
            this.tabOverview.Size = new System.Drawing.Size(1027, 455);
            this.tabOverview.TabIndex = 4;
            this.tabOverview.Text = "Overview";
            this.tabOverview.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel14
            // 
            this.tableLayoutPanel14.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel14.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel14.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel14.ColumnCount = 1;
            this.tableLayoutPanel14.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel14.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.tableLayoutPanel14.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.tableLayoutPanel14.Controls.Add(this.tableLayoutPanel15, 0, 0);
            this.tableLayoutPanel14.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel14.Name = "tableLayoutPanel14";
            this.tableLayoutPanel14.RowCount = 1;
            this.tableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel14.Size = new System.Drawing.Size(1023, 455);
            this.tableLayoutPanel14.TabIndex = 3;
            // 
            // tableLayoutPanel15
            // 
            this.tableLayoutPanel15.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel15.ColumnCount = 1;
            this.tableLayoutPanel15.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel15.Controls.Add(this.pictureBox2, 0, 0);
            this.tableLayoutPanel15.Controls.Add(this.label2, 0, 2);
            this.tableLayoutPanel15.Controls.Add(this.label3, 0, 1);
            this.tableLayoutPanel15.Controls.Add(this.lbl_version, 0, 5);
            this.tableLayoutPanel15.Controls.Add(this.lbl_errors, 0, 3);
            this.tableLayoutPanel15.Controls.Add(this.lbl_info, 0, 5);
            this.tableLayoutPanel15.Controls.Add(this.lblQueue, 0, 4);
            this.tableLayoutPanel15.Location = new System.Drawing.Point(4, 4);
            this.tableLayoutPanel15.Name = "tableLayoutPanel15";
            this.tableLayoutPanel15.RowCount = 6;
            this.tableLayoutPanel15.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 47.14285F));
            this.tableLayoutPanel15.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 0.9523811F));
            this.tableLayoutPanel15.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 28.57143F));
            this.tableLayoutPanel15.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 13.80951F));
            this.tableLayoutPanel15.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.761905F));
            this.tableLayoutPanel15.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.761905F));
            this.tableLayoutPanel15.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.tableLayoutPanel15.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel15.Size = new System.Drawing.Size(1015, 447);
            this.tableLayoutPanel15.TabIndex = 0;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox2.Image = global::AITool.Properties.Resources.logo;
            this.pictureBox2.Location = new System.Drawing.Point(3, 66);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(1009, 131);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 4;
            this.pictureBox2.TabStop = false;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoEllipsis = true;
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Green;
            this.label2.Location = new System.Drawing.Point(3, 204);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(1009, 32);
            this.label2.TabIndex = 3;
            this.label2.Text = "Running";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label3.Location = new System.Drawing.Point(63, 200);
            this.label3.Margin = new System.Windows.Forms.Padding(63, 0, 63, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(889, 2);
            this.label3.TabIndex = 5;
            this.label3.Text = "label3";
            // 
            // lbl_version
            // 
            this.lbl_version.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_version.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lbl_version.Location = new System.Drawing.Point(3, 403);
            this.lbl_version.Name = "lbl_version";
            this.lbl_version.Size = new System.Drawing.Size(1009, 20);
            this.lbl_version.TabIndex = 6;
            this.lbl_version.Text = "Version 1.67 preview 7  (VorlonCD MOD)";
            this.lbl_version.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // lbl_errors
            // 
            this.lbl_errors.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_errors.Font = new System.Drawing.Font("Segoe UI Semibold", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_errors.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lbl_errors.Location = new System.Drawing.Point(3, 325);
            this.lbl_errors.Name = "lbl_errors";
            this.lbl_errors.Size = new System.Drawing.Size(1009, 58);
            this.lbl_errors.TabIndex = 7;
            this.lbl_errors.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_errors.Visible = false;
            this.lbl_errors.Click += new System.EventHandler(this.lbl_errors_Click);
            // 
            // lbl_info
            // 
            this.lbl_info.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_info.AutoSize = true;
            this.lbl_info.Location = new System.Drawing.Point(3, 423);
            this.lbl_info.Name = "lbl_info";
            this.lbl_info.Size = new System.Drawing.Size(1009, 24);
            this.lbl_info.TabIndex = 8;
            this.lbl_info.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblQueue
            // 
            this.lblQueue.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblQueue.AutoSize = true;
            this.lblQueue.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblQueue.ForeColor = System.Drawing.Color.DarkOrange;
            this.lblQueue.Location = new System.Drawing.Point(3, 383);
            this.lblQueue.Name = "lblQueue";
            this.lblQueue.Size = new System.Drawing.Size(1009, 20);
            this.lblQueue.TabIndex = 9;
            this.lblQueue.Text = "Images in Queue: 0";
            this.lblQueue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabStats
            // 
            this.tabStats.Controls.Add(this.tableLayoutPanel16);
            this.tabStats.Location = new System.Drawing.Point(4, 22);
            this.tabStats.Name = "tabStats";
            this.tabStats.Size = new System.Drawing.Size(1027, 455);
            this.tabStats.TabIndex = 5;
            this.tabStats.Text = "Stats";
            this.tabStats.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel16
            // 
            this.tableLayoutPanel16.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel16.ColumnCount = 2;
            this.tableLayoutPanel16.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel16.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutPanel16.Controls.Add(this.tableLayoutPanel23, 0, 0);
            this.tableLayoutPanel16.Controls.Add(this.tableLayoutPanel17, 0, 0);
            this.tableLayoutPanel16.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel16.Name = "tableLayoutPanel16";
            this.tableLayoutPanel16.RowCount = 1;
            this.tableLayoutPanel16.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel16.Size = new System.Drawing.Size(1020, 454);
            this.tableLayoutPanel16.TabIndex = 0;
            // 
            // tableLayoutPanel23
            // 
            this.tableLayoutPanel23.ColumnCount = 1;
            this.tableLayoutPanel23.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel23.Controls.Add(this.label8, 0, 2);
            this.tableLayoutPanel23.Controls.Add(this.chart_confidence, 0, 2);
            this.tableLayoutPanel23.Controls.Add(this.timeline, 0, 1);
            this.tableLayoutPanel23.Controls.Add(this.label7, 0, 0);
            this.tableLayoutPanel23.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel23.Location = new System.Drawing.Point(309, 3);
            this.tableLayoutPanel23.Name = "tableLayoutPanel23";
            this.tableLayoutPanel23.RowCount = 3;
            this.tableLayoutPanel23.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel23.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel23.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel23.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel23.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.tableLayoutPanel23.Size = new System.Drawing.Size(708, 448);
            this.tableLayoutPanel23.TabIndex = 7;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Dock = System.Windows.Forms.DockStyle.Top;
            this.label8.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(3, 227);
            this.label8.Margin = new System.Windows.Forms.Padding(3);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(702, 29);
            this.label8.TabIndex = 9;
            this.label8.Text = "Frequencies of alert result confidences";
            this.label8.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // chart_confidence
            // 
            this.chart_confidence.BackColor = System.Drawing.Color.Transparent;
            this.chart_confidence.BorderlineColor = System.Drawing.Color.Transparent;
            chartArea4.AxisX.Interval = 10D;
            chartArea4.AxisX.MajorGrid.Interval = 6D;
            chartArea4.AxisX.MajorGrid.LineColor = System.Drawing.Color.LightGray;
            chartArea4.AxisX.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            chartArea4.AxisX.MajorTickMark.Interval = 1D;
            chartArea4.AxisX.Maximum = 100D;
            chartArea4.AxisX.Minimum = 0D;
            chartArea4.AxisX.Title = "Alert confidence";
            chartArea4.AxisX.TitleFont = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea4.AxisY.MajorGrid.LineColor = System.Drawing.Color.LightGray;
            chartArea4.AxisY.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
            chartArea4.AxisY.Title = "Frequency";
            chartArea4.AxisY.TitleFont = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea4.BackColor = System.Drawing.Color.Transparent;
            chartArea4.Name = "ChartArea1";
            this.chart_confidence.ChartAreas.Add(chartArea4);
            this.chart_confidence.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chart_confidence.Location = new System.Drawing.Point(3, 262);
            this.chart_confidence.Name = "chart_confidence";
            series9.BorderWidth = 4;
            series9.ChartArea = "ChartArea1";
            series9.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series9.Color = System.Drawing.Color.Orange;
            series9.Name = "no alert";
            series10.BorderWidth = 3;
            series10.ChartArea = "ChartArea1";
            series10.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series10.Color = System.Drawing.Color.Green;
            series10.Legend = "Legend1";
            series10.Name = "alert";
            this.chart_confidence.Series.Add(series9);
            this.chart_confidence.Series.Add(series10);
            this.chart_confidence.Size = new System.Drawing.Size(702, 183);
            this.chart_confidence.TabIndex = 8;
            // 
            // timeline
            // 
            this.timeline.BackColor = System.Drawing.Color.Transparent;
            this.timeline.BorderlineColor = System.Drawing.Color.Transparent;
            chartArea5.AxisX.Interval = 3D;
            chartArea5.AxisX.MajorGrid.Interval = 6D;
            chartArea5.AxisX.MajorGrid.LineColor = System.Drawing.Color.LightGray;
            chartArea5.AxisX.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            chartArea5.AxisX.MajorTickMark.Interval = 1D;
            chartArea5.AxisX.Maximum = 24D;
            chartArea5.AxisX.Minimum = 0D;
            chartArea5.AxisX.TitleFont = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea5.AxisY.MajorGrid.LineColor = System.Drawing.Color.LightGray;
            chartArea5.AxisY.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
            chartArea5.AxisY.Title = "Number";
            chartArea5.AxisY.TitleFont = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea5.BackColor = System.Drawing.Color.Transparent;
            chartArea5.Name = "ChartArea1";
            this.timeline.ChartAreas.Add(chartArea5);
            this.timeline.Dock = System.Windows.Forms.DockStyle.Fill;
            this.timeline.Location = new System.Drawing.Point(3, 38);
            this.timeline.Name = "timeline";
            series11.ChartArea = "ChartArea1";
            series11.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.SplineArea;
            series11.Color = System.Drawing.Color.Silver;
            series11.Legend = "Legend1";
            series11.Name = "all";
            series12.BorderWidth = 3;
            series12.ChartArea = "ChartArea1";
            series12.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series12.Color = System.Drawing.Color.OrangeRed;
            series12.Legend = "Legend1";
            series12.Name = "falses";
            series13.BorderWidth = 3;
            series13.ChartArea = "ChartArea1";
            series13.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series13.Color = System.Drawing.Color.Orange;
            series13.Legend = "Legend1";
            series13.Name = "irrelevant";
            series14.BorderWidth = 4;
            series14.ChartArea = "ChartArea1";
            series14.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series14.Color = System.Drawing.Color.Green;
            series14.Legend = "Legend1";
            series14.Name = "relevant";
            series15.ChartArea = "ChartArea1";
            series15.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series15.Color = System.Drawing.Color.Purple;
            series15.Name = "skipped";
            this.timeline.Series.Add(series11);
            this.timeline.Series.Add(series12);
            this.timeline.Series.Add(series13);
            this.timeline.Series.Add(series14);
            this.timeline.Series.Add(series15);
            this.timeline.Size = new System.Drawing.Size(702, 183);
            this.timeline.TabIndex = 6;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Dock = System.Windows.Forms.DockStyle.Top;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(3, 3);
            this.label7.Margin = new System.Windows.Forms.Padding(3);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(702, 29);
            this.label7.TabIndex = 0;
            this.label7.Text = "Timeline";
            this.label7.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // tableLayoutPanel17
            // 
            this.tableLayoutPanel17.ColumnCount = 1;
            this.tableLayoutPanel17.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel17.Controls.Add(this.chart1, 0, 2);
            this.tableLayoutPanel17.Controls.Add(this.comboBox1, 0, 0);
            this.tableLayoutPanel17.Controls.Add(this.btn_resetstats, 0, 1);
            this.tableLayoutPanel17.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel17.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel17.Name = "tableLayoutPanel17";
            this.tableLayoutPanel17.RowCount = 3;
            this.tableLayoutPanel17.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel17.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel17.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel17.Size = new System.Drawing.Size(300, 448);
            this.tableLayoutPanel17.TabIndex = 3;
            // 
            // chart1
            // 
            this.chart1.BackColor = System.Drawing.Color.Transparent;
            this.chart1.BorderlineColor = System.Drawing.Color.DimGray;
            this.chart1.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            chartArea6.Area3DStyle.Enable3D = true;
            chartArea6.Area3DStyle.IsRightAngleAxes = false;
            chartArea6.Area3DStyle.LightStyle = System.Windows.Forms.DataVisualization.Charting.LightStyle.Realistic;
            chartArea6.Area3DStyle.Perspective = 10;
            chartArea6.Area3DStyle.WallWidth = 6;
            chartArea6.AxisX.LabelAutoFitStyle = ((System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles)((((((System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.IncreaseFont | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.DecreaseFont) 
            | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.StaggeredLabels) 
            | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.LabelsAngleStep30) 
            | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.LabelsAngleStep45) 
            | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.WordWrap)));
            chartArea6.AxisY.LabelAutoFitStyle = ((System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles)(((((((System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.IncreaseFont | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.DecreaseFont) 
            | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.StaggeredLabels) 
            | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.LabelsAngleStep30) 
            | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.LabelsAngleStep45) 
            | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.LabelsAngleStep90) 
            | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.WordWrap)));
            chartArea6.BackColor = System.Drawing.Color.Transparent;
            chartArea6.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea6);
            this.chart1.Dock = System.Windows.Forms.DockStyle.Fill;
            legend2.Alignment = System.Drawing.StringAlignment.Center;
            legend2.BackColor = System.Drawing.Color.Transparent;
            legend2.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
            legend2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            legend2.IsTextAutoFit = false;
            legend2.LegendStyle = System.Windows.Forms.DataVisualization.Charting.LegendStyle.Column;
            legend2.Name = "Legend1";
            legend2.Title = "Legend";
            this.chart1.Legends.Add(legend2);
            this.chart1.Location = new System.Drawing.Point(3, 59);
            this.chart1.Name = "chart1";
            series16.ChartArea = "ChartArea1";
            series16.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;
            series16.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            series16.IsValueShownAsLabel = true;
            series16.Legend = "Legend1";
            series16.Name = "s1";
            dataPoint4.IsVisibleInLegend = true;
            series16.Points.Add(dataPoint4);
            series16.Points.Add(dataPoint5);
            series16.Points.Add(dataPoint6);
            this.chart1.Series.Add(series16);
            this.chart1.Size = new System.Drawing.Size(294, 386);
            this.chart1.TabIndex = 2;
            this.chart1.Text = "chart1";
            title2.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            title2.Name = "Title1";
            title2.Text = "Input Rates";
            this.chart1.Titles.Add(title2);
            // 
            // comboBox1
            // 
            this.comboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(3, 3);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(294, 25);
            this.comboBox1.TabIndex = 3;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged_1);
            this.comboBox1.SelectionChangeCommitted += new System.EventHandler(this.comboBox1_SelectionChangeCommitted);
            // 
            // btn_resetstats
            // 
            this.btn_resetstats.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_resetstats.Location = new System.Drawing.Point(2, 33);
            this.btn_resetstats.Margin = new System.Windows.Forms.Padding(2);
            this.btn_resetstats.Name = "btn_resetstats";
            this.btn_resetstats.Size = new System.Drawing.Size(296, 21);
            this.btn_resetstats.TabIndex = 4;
            this.btn_resetstats.Text = "Reset Stats";
            this.btn_resetstats.UseVisualStyleBackColor = true;
            this.btn_resetstats.Click += new System.EventHandler(this.btn_resetstats_Click);
            // 
            // tabHistory
            // 
            this.tabHistory.Controls.Add(this.toolStrip1);
            this.tabHistory.Controls.Add(this.splitContainer2);
            this.tabHistory.Location = new System.Drawing.Point(4, 22);
            this.tabHistory.Name = "tabHistory";
            this.tabHistory.Padding = new System.Windows.Forms.Padding(3);
            this.tabHistory.Size = new System.Drawing.Size(1027, 455);
            this.tabHistory.TabIndex = 0;
            this.tabHistory.Text = "History";
            this.tabHistory.UseVisualStyleBackColor = true;
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(28, 28);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.comboBox_filter_camera,
            this.toolStripSeparator1,
            this.toolStripDropDownButtonFilters,
            this.toolStripDropDownButtonOptions,
            this.toolStripButtonDetails,
            this.toolStripButtonMaskDetails,
            this.toolStripButtonEditImageMask});
            this.toolStrip1.Location = new System.Drawing.Point(3, 3);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(1021, 38);
            this.toolStrip1.TabIndex = 5;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // comboBox_filter_camera
            // 
            this.comboBox_filter_camera.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_filter_camera.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.comboBox_filter_camera.Name = "comboBox_filter_camera";
            this.comboBox_filter_camera.Size = new System.Drawing.Size(173, 38);
            this.comboBox_filter_camera.DropDownClosed += new System.EventHandler(this.comboBox_filter_camera_DropDownClosed);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 38);
            // 
            // toolStripDropDownButtonFilters
            // 
            this.toolStripDropDownButtonFilters.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cb_filter_success,
            this.cb_filter_nosuccess,
            this.cb_filter_person,
            this.cb_filter_animal,
            this.cb_filter_vehicle,
            this.cb_filter_skipped,
            this.cb_filter_masked});
            this.toolStripDropDownButtonFilters.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButtonFilters.Image")));
            this.toolStripDropDownButtonFilters.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButtonFilters.Name = "toolStripDropDownButtonFilters";
            this.toolStripDropDownButtonFilters.Size = new System.Drawing.Size(120, 35);
            this.toolStripDropDownButtonFilters.Text = "History Filters";
            // 
            // cb_filter_success
            // 
            this.cb_filter_success.CheckOnClick = true;
            this.cb_filter_success.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.cb_filter_success.Name = "cb_filter_success";
            this.cb_filter_success.Size = new System.Drawing.Size(188, 22);
            this.cb_filter_success.Text = "Only Relevant";
            this.cb_filter_success.Click += new System.EventHandler(this.cb_filter_success_Click);
            // 
            // cb_filter_nosuccess
            // 
            this.cb_filter_nosuccess.CheckOnClick = true;
            this.cb_filter_nosuccess.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.cb_filter_nosuccess.Name = "cb_filter_nosuccess";
            this.cb_filter_nosuccess.Size = new System.Drawing.Size(188, 22);
            this.cb_filter_nosuccess.Text = "Only False / Irrelevant";
            this.cb_filter_nosuccess.Click += new System.EventHandler(this.cb_filter_nosuccess_Click);
            // 
            // cb_filter_person
            // 
            this.cb_filter_person.CheckOnClick = true;
            this.cb_filter_person.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.cb_filter_person.Name = "cb_filter_person";
            this.cb_filter_person.Size = new System.Drawing.Size(188, 22);
            this.cb_filter_person.Text = "Only People";
            this.cb_filter_person.Click += new System.EventHandler(this.cb_filter_person_Click);
            // 
            // cb_filter_animal
            // 
            this.cb_filter_animal.CheckOnClick = true;
            this.cb_filter_animal.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.cb_filter_animal.Name = "cb_filter_animal";
            this.cb_filter_animal.Size = new System.Drawing.Size(188, 22);
            this.cb_filter_animal.Text = "Only Animals";
            this.cb_filter_animal.Click += new System.EventHandler(this.cb_filter_animal_Click);
            // 
            // cb_filter_vehicle
            // 
            this.cb_filter_vehicle.CheckOnClick = true;
            this.cb_filter_vehicle.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.cb_filter_vehicle.Name = "cb_filter_vehicle";
            this.cb_filter_vehicle.Size = new System.Drawing.Size(188, 22);
            this.cb_filter_vehicle.Text = "Only Vehicles";
            this.cb_filter_vehicle.Click += new System.EventHandler(this.cb_filter_vehicle_Click);
            // 
            // cb_filter_skipped
            // 
            this.cb_filter_skipped.CheckOnClick = true;
            this.cb_filter_skipped.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.cb_filter_skipped.Name = "cb_filter_skipped";
            this.cb_filter_skipped.Size = new System.Drawing.Size(188, 22);
            this.cb_filter_skipped.Text = "Only Skipped";
            this.cb_filter_skipped.Click += new System.EventHandler(this.cb_filter_skipped_Click);
            // 
            // cb_filter_masked
            // 
            this.cb_filter_masked.CheckOnClick = true;
            this.cb_filter_masked.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.cb_filter_masked.Name = "cb_filter_masked";
            this.cb_filter_masked.Size = new System.Drawing.Size(188, 22);
            this.cb_filter_masked.Text = "Only Masked";
            this.cb_filter_masked.Click += new System.EventHandler(this.cb_filter_masked_Click);
            // 
            // toolStripDropDownButtonOptions
            // 
            this.toolStripDropDownButtonOptions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cb_showMask,
            this.cb_showObjects,
            this.showOnlyRelevantObjectsToolStripMenuItem,
            this.cb_follow,
            this.automaticallyRefreshToolStripMenuItem,
            this.storeFalseAlertsToolStripMenuItem,
            this.storeMaskedAlertsToolStripMenuItem,
            this.restrictThresholdAtSourceToolStripMenuItem});
            this.toolStripDropDownButtonOptions.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButtonOptions.Image")));
            this.toolStripDropDownButtonOptions.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButtonOptions.Name = "toolStripDropDownButtonOptions";
            this.toolStripDropDownButtonOptions.Size = new System.Drawing.Size(131, 35);
            this.toolStripDropDownButtonOptions.Text = "History Settings";
            // 
            // cb_showMask
            // 
            this.cb_showMask.CheckOnClick = true;
            this.cb_showMask.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.cb_showMask.Name = "cb_showMask";
            this.cb_showMask.Size = new System.Drawing.Size(222, 22);
            this.cb_showMask.Text = "Show Mask";
            this.cb_showMask.Click += new System.EventHandler(this.cb_showMask_Click);
            // 
            // cb_showObjects
            // 
            this.cb_showObjects.Checked = true;
            this.cb_showObjects.CheckOnClick = true;
            this.cb_showObjects.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_showObjects.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.cb_showObjects.Name = "cb_showObjects";
            this.cb_showObjects.Size = new System.Drawing.Size(222, 22);
            this.cb_showObjects.Text = "Show Objects";
            this.cb_showObjects.CheckedChanged += new System.EventHandler(this.cb_showObjects_CheckedChanged);
            this.cb_showObjects.Click += new System.EventHandler(this.cb_showObjects_Click);
            // 
            // showOnlyRelevantObjectsToolStripMenuItem
            // 
            this.showOnlyRelevantObjectsToolStripMenuItem.CheckOnClick = true;
            this.showOnlyRelevantObjectsToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.showOnlyRelevantObjectsToolStripMenuItem.Name = "showOnlyRelevantObjectsToolStripMenuItem";
            this.showOnlyRelevantObjectsToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.showOnlyRelevantObjectsToolStripMenuItem.Text = "Show Only Relevant Objects";
            this.showOnlyRelevantObjectsToolStripMenuItem.ToolTipText = resources.GetString("showOnlyRelevantObjectsToolStripMenuItem.ToolTipText");
            this.showOnlyRelevantObjectsToolStripMenuItem.Click += new System.EventHandler(this.showOnlyRelevantObjectsToolStripMenuItem_Click);
            // 
            // cb_follow
            // 
            this.cb_follow.CheckOnClick = true;
            this.cb_follow.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.cb_follow.Name = "cb_follow";
            this.cb_follow.Size = new System.Drawing.Size(222, 22);
            this.cb_follow.Text = "Follow History List";
            this.cb_follow.ToolTipText = "Automatically select the latest history item in the list for every update";
            this.cb_follow.Click += new System.EventHandler(this.cb_follow_Click);
            // 
            // automaticallyRefreshToolStripMenuItem
            // 
            this.automaticallyRefreshToolStripMenuItem.Checked = true;
            this.automaticallyRefreshToolStripMenuItem.CheckOnClick = true;
            this.automaticallyRefreshToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.automaticallyRefreshToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.automaticallyRefreshToolStripMenuItem.Name = "automaticallyRefreshToolStripMenuItem";
            this.automaticallyRefreshToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.automaticallyRefreshToolStripMenuItem.Text = "Automatically Refresh";
            this.automaticallyRefreshToolStripMenuItem.Click += new System.EventHandler(this.automaticallyRefreshToolStripMenuItem_Click);
            // 
            // storeFalseAlertsToolStripMenuItem
            // 
            this.storeFalseAlertsToolStripMenuItem.CheckOnClick = true;
            this.storeFalseAlertsToolStripMenuItem.DoubleClickEnabled = true;
            this.storeFalseAlertsToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.storeFalseAlertsToolStripMenuItem.Name = "storeFalseAlertsToolStripMenuItem";
            this.storeFalseAlertsToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.storeFalseAlertsToolStripMenuItem.Text = "Store False Alerts";
            this.storeFalseAlertsToolStripMenuItem.ToolTipText = resources.GetString("storeFalseAlertsToolStripMenuItem.ToolTipText");
            this.storeFalseAlertsToolStripMenuItem.Click += new System.EventHandler(this.storeFalseAlertsToolStripMenuItem_Click);
            // 
            // storeMaskedAlertsToolStripMenuItem
            // 
            this.storeMaskedAlertsToolStripMenuItem.CheckOnClick = true;
            this.storeMaskedAlertsToolStripMenuItem.DoubleClickEnabled = true;
            this.storeMaskedAlertsToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.storeMaskedAlertsToolStripMenuItem.Name = "storeMaskedAlertsToolStripMenuItem";
            this.storeMaskedAlertsToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.storeMaskedAlertsToolStripMenuItem.Text = "Store Masked Alerts";
            this.storeMaskedAlertsToolStripMenuItem.ToolTipText = "If disabled the database will be smaller, leave enabled for better troubleshootin" +
    "g";
            this.storeMaskedAlertsToolStripMenuItem.Click += new System.EventHandler(this.storeMaskedAlertsToolStripMenuItem_Click);
            // 
            // restrictThresholdAtSourceToolStripMenuItem
            // 
            this.restrictThresholdAtSourceToolStripMenuItem.CheckOnClick = true;
            this.restrictThresholdAtSourceToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.restrictThresholdAtSourceToolStripMenuItem.Name = "restrictThresholdAtSourceToolStripMenuItem";
            this.restrictThresholdAtSourceToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.restrictThresholdAtSourceToolStripMenuItem.Text = "Restrict Threshold at Source";
            this.restrictThresholdAtSourceToolStripMenuItem.ToolTipText = resources.GetString("restrictThresholdAtSourceToolStripMenuItem.ToolTipText");
            this.restrictThresholdAtSourceToolStripMenuItem.Click += new System.EventHandler(this.restrictThresholdAtSourceToolStripMenuItem_Click);
            // 
            // toolStripButtonDetails
            // 
            this.toolStripButtonDetails.Enabled = false;
            this.toolStripButtonDetails.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonDetails.Image")));
            this.toolStripButtonDetails.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonDetails.Name = "toolStripButtonDetails";
            this.toolStripButtonDetails.Size = new System.Drawing.Size(131, 35);
            this.toolStripButtonDetails.Text = "Prediction Details";
            this.toolStripButtonDetails.Click += new System.EventHandler(this.toolStripButtonDetails_Click);
            // 
            // toolStripButtonMaskDetails
            // 
            this.toolStripButtonMaskDetails.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonMaskDetails.Image")));
            this.toolStripButtonMaskDetails.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonMaskDetails.Name = "toolStripButtonMaskDetails";
            this.toolStripButtonMaskDetails.Size = new System.Drawing.Size(155, 35);
            this.toolStripButtonMaskDetails.Text = "Dynamic Mask Details";
            this.toolStripButtonMaskDetails.Click += new System.EventHandler(this.toolStripButtonMaskDetails_Click);
            // 
            // toolStripButtonEditImageMask
            // 
            this.toolStripButtonEditImageMask.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonEditImageMask.Image")));
            this.toolStripButtonEditImageMask.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonEditImageMask.Name = "toolStripButtonEditImageMask";
            this.toolStripButtonEditImageMask.Size = new System.Drawing.Size(126, 35);
            this.toolStripButtonEditImageMask.Text = "Edit Image Mask";
            this.toolStripButtonEditImageMask.Click += new System.EventHandler(this.toolStripButtonEditImageMask_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.Location = new System.Drawing.Point(2, 43);
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(2);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.groupBox8);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.lbl_objects);
            this.splitContainer2.Panel2.Controls.Add(this.pictureBox1);
            this.splitContainer2.Panel2MinSize = 300;
            this.splitContainer2.Size = new System.Drawing.Size(1018, 407);
            this.splitContainer2.SplitterDistance = 284;
            this.splitContainer2.SplitterWidth = 2;
            this.splitContainer2.TabIndex = 4;
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.folv_history);
            this.groupBox8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox8.Location = new System.Drawing.Point(0, 0);
            this.groupBox8.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox8.Size = new System.Drawing.Size(280, 403);
            this.groupBox8.TabIndex = 11;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "History";
            // 
            // folv_history
            // 
            this.folv_history.ContextMenuStrip = this.contextMenuStripHistory;
            this.folv_history.Dock = System.Windows.Forms.DockStyle.Fill;
            this.folv_history.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.folv_history.HideSelection = false;
            this.folv_history.Location = new System.Drawing.Point(2, 15);
            this.folv_history.Margin = new System.Windows.Forms.Padding(2);
            this.folv_history.Name = "folv_history";
            this.folv_history.ShowGroups = false;
            this.folv_history.Size = new System.Drawing.Size(276, 386);
            this.folv_history.TabIndex = 10;
            this.folv_history.UseCellFormatEvents = true;
            this.folv_history.UseCompatibleStateImageBehavior = false;
            this.folv_history.UseFiltering = true;
            this.folv_history.View = System.Windows.Forms.View.Details;
            this.folv_history.VirtualMode = true;
            this.folv_history.FormatRow += new System.EventHandler<BrightIdeasSoftware.FormatRowEventArgs>(this.folv_history_FormatRow);
            this.folv_history.SelectionChanged += new System.EventHandler(this.folv_history_SelectionChanged);
            this.folv_history.SelectedIndexChanged += new System.EventHandler(this.folv_history_SelectedIndexChanged);
            this.folv_history.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.folv_history_MouseDoubleClick);
            // 
            // contextMenuStripHistory
            // 
            this.contextMenuStripHistory.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStripHistory.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.testDetectionAgainToolStripMenuItem,
            this.detailsToolStripMenuItem,
            this.refreshToolStripMenuItem,
            this.dynamicMaskDetailsToolStripMenuItem,
            this.locateInLogToolStripMenuItem,
            this.manuallyAddImagesToolStripMenuItem});
            this.contextMenuStripHistory.Name = "contextMenuStripHistory";
            this.contextMenuStripHistory.Size = new System.Drawing.Size(191, 136);
            // 
            // testDetectionAgainToolStripMenuItem
            // 
            this.testDetectionAgainToolStripMenuItem.Name = "testDetectionAgainToolStripMenuItem";
            this.testDetectionAgainToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.testDetectionAgainToolStripMenuItem.Text = "Test Detection Again";
            this.testDetectionAgainToolStripMenuItem.Click += new System.EventHandler(this.testDetectionAgainToolStripMenuItem_Click);
            // 
            // detailsToolStripMenuItem
            // 
            this.detailsToolStripMenuItem.Name = "detailsToolStripMenuItem";
            this.detailsToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.detailsToolStripMenuItem.Text = "Prediction Details";
            this.detailsToolStripMenuItem.Click += new System.EventHandler(this.detailsToolStripMenuItem_Click);
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.refreshToolStripMenuItem.Text = "Refresh";
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // dynamicMaskDetailsToolStripMenuItem
            // 
            this.dynamicMaskDetailsToolStripMenuItem.Name = "dynamicMaskDetailsToolStripMenuItem";
            this.dynamicMaskDetailsToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.dynamicMaskDetailsToolStripMenuItem.Text = "Dynamic Mask Details";
            this.dynamicMaskDetailsToolStripMenuItem.Click += new System.EventHandler(this.dynamicMaskDetailsToolStripMenuItem_Click);
            // 
            // locateInLogToolStripMenuItem
            // 
            this.locateInLogToolStripMenuItem.Name = "locateInLogToolStripMenuItem";
            this.locateInLogToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.locateInLogToolStripMenuItem.Text = "Locate in log";
            this.locateInLogToolStripMenuItem.Click += new System.EventHandler(this.locateInLogToolStripMenuItem_Click);
            // 
            // manuallyAddImagesToolStripMenuItem
            // 
            this.manuallyAddImagesToolStripMenuItem.Name = "manuallyAddImagesToolStripMenuItem";
            this.manuallyAddImagesToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.manuallyAddImagesToolStripMenuItem.Text = "Manually Add Images";
            this.manuallyAddImagesToolStripMenuItem.Click += new System.EventHandler(this.manuallyAddImagesToolStripMenuItem_Click);
            // 
            // lbl_objects
            // 
            this.lbl_objects.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_objects.BackColor = System.Drawing.SystemColors.Info;
            this.lbl_objects.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_objects.Location = new System.Drawing.Point(1, 0);
            this.lbl_objects.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this.lbl_objects.Name = "lbl_objects";
            this.lbl_objects.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this.lbl_objects.Size = new System.Drawing.Size(786, 20);
            this.lbl_objects.TabIndex = 14;
            this.lbl_objects.Text = "No selection";
            this.lbl_objects.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox1.Location = new System.Drawing.Point(4, 23);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(765, 377);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            // 
            // tabCameras
            // 
            this.tabCameras.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tabCameras.Controls.Add(this.tableLayoutPanel2);
            this.tabCameras.Location = new System.Drawing.Point(4, 22);
            this.tabCameras.Name = "tabCameras";
            this.tabCameras.Padding = new System.Windows.Forms.Padding(3);
            this.tabCameras.Size = new System.Drawing.Size(1027, 455);
            this.tabCameras.TabIndex = 2;
            this.tabCameras.Text = "Cameras";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 19.05679F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80.94321F));
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel6, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1021, 449);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.list2, 0, 0);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 443F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(188, 443);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // list2
            // 
            this.list2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.list2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.list2.GridLines = true;
            this.list2.HideSelection = false;
            this.list2.Location = new System.Drawing.Point(3, 3);
            this.list2.Name = "list2";
            this.list2.Size = new System.Drawing.Size(182, 437);
            this.list2.TabIndex = 1;
            this.list2.UseCompatibleStateImageBehavior = false;
            this.list2.View = System.Windows.Forms.View.Details;
            this.list2.SelectedIndexChanged += new System.EventHandler(this.list2_SelectedIndexChanged);
            this.list2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.list2_KeyDown);
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel6.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tableLayoutPanel6.ColumnCount = 1;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.Controls.Add(this.tableLayoutPanel11, 0, 2);
            this.tableLayoutPanel6.Controls.Add(this.lbl_camstats, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.tableLayoutPanel7, 0, 1);
            this.tableLayoutPanel6.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tableLayoutPanel6.Location = new System.Drawing.Point(197, 3);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 3;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.82557F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 93.17443F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(821, 443);
            this.tableLayoutPanel6.TabIndex = 1;
            // 
            // tableLayoutPanel11
            // 
            this.tableLayoutPanel11.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel11.ColumnCount = 4;
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 18.02243F));
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 27.32586F));
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 27.32586F));
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 27.32586F));
            this.tableLayoutPanel11.Controls.Add(this.btnCameraSave, 3, 0);
            this.tableLayoutPanel11.Controls.Add(this.btnCameraAdd, 0, 0);
            this.tableLayoutPanel11.Controls.Add(this.btnCameraDel, 1, 0);
            this.tableLayoutPanel11.Controls.Add(this.btnSaveTo, 2, 0);
            this.tableLayoutPanel11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel11.Location = new System.Drawing.Point(3, 405);
            this.tableLayoutPanel11.Name = "tableLayoutPanel11";
            this.tableLayoutPanel11.RowCount = 1;
            this.tableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel11.Size = new System.Drawing.Size(815, 35);
            this.tableLayoutPanel11.TabIndex = 3;
            // 
            // btnCameraSave
            // 
            this.btnCameraSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.btnCameraSave.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnCameraSave.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCameraSave.Location = new System.Drawing.Point(667, 3);
            this.btnCameraSave.Name = "btnCameraSave";
            this.btnCameraSave.Size = new System.Drawing.Size(70, 30);
            this.btnCameraSave.TabIndex = 26;
            this.btnCameraSave.Text = "Save";
            this.btnCameraSave.UseVisualStyleBackColor = false;
            this.btnCameraSave.Click += new System.EventHandler(this.btnCameraSave_Click_1);
            // 
            // btnCameraAdd
            // 
            this.btnCameraAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.btnCameraAdd.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCameraAdd.Location = new System.Drawing.Point(38, 3);
            this.btnCameraAdd.Name = "btnCameraAdd";
            this.btnCameraAdd.Size = new System.Drawing.Size(70, 30);
            this.btnCameraAdd.TabIndex = 24;
            this.btnCameraAdd.Text = "Add";
            this.btnCameraAdd.UseVisualStyleBackColor = true;
            this.btnCameraAdd.Click += new System.EventHandler(this.btnCameraAdd_Click);
            // 
            // btnCameraDel
            // 
            this.btnCameraDel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.btnCameraDel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCameraDel.Location = new System.Drawing.Point(222, 3);
            this.btnCameraDel.Name = "btnCameraDel";
            this.btnCameraDel.Size = new System.Drawing.Size(70, 30);
            this.btnCameraDel.TabIndex = 25;
            this.btnCameraDel.Text = "Delete";
            this.btnCameraDel.UseVisualStyleBackColor = true;
            this.btnCameraDel.Click += new System.EventHandler(this.btnCameraDel_Click);
            // 
            // btnSaveTo
            // 
            this.btnSaveTo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.btnSaveTo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnSaveTo.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveTo.Location = new System.Drawing.Point(444, 3);
            this.btnSaveTo.Name = "btnSaveTo";
            this.btnSaveTo.Size = new System.Drawing.Size(70, 30);
            this.btnSaveTo.TabIndex = 27;
            this.btnSaveTo.Text = "Apply to";
            this.btnSaveTo.UseVisualStyleBackColor = false;
            this.btnSaveTo.Click += new System.EventHandler(this.btnSaveTo_Click);
            // 
            // lbl_camstats
            // 
            this.lbl_camstats.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lbl_camstats.AutoSize = true;
            this.lbl_camstats.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_camstats.Location = new System.Drawing.Point(3, 5);
            this.lbl_camstats.Name = "lbl_camstats";
            this.lbl_camstats.Size = new System.Drawing.Size(38, 17);
            this.lbl_camstats.TabIndex = 4;
            this.lbl_camstats.Text = "Stats";
            // 
            // tableLayoutPanel7
            // 
            this.tableLayoutPanel7.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel7.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel7.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel7.ColumnCount = 2;
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 17.60259F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 82.39741F));
            this.tableLayoutPanel7.Controls.Add(this.label14, 0, 2);
            this.tableLayoutPanel7.Controls.Add(this.label1, 0, 6);
            this.tableLayoutPanel7.Controls.Add(this.lblPrefix, 0, 1);
            this.tableLayoutPanel7.Controls.Add(this.lblName, 0, 0);
            this.tableLayoutPanel7.Controls.Add(this.tableLayoutPanel12, 1, 1);
            this.tableLayoutPanel7.Controls.Add(this.tableLayoutPanel13, 1, 0);
            this.tableLayoutPanel7.Controls.Add(this.lblRelevantObjects, 0, 3);
            this.tableLayoutPanel7.Controls.Add(this.lbl_threshold, 0, 5);
            this.tableLayoutPanel7.Controls.Add(this.tableLayoutPanel26, 1, 2);
            this.tableLayoutPanel7.Controls.Add(this.label15, 0, 7);
            this.tableLayoutPanel7.Controls.Add(this.tableLayoutPanel27, 1, 7);
            this.tableLayoutPanel7.Controls.Add(this.btnActions, 1, 6);
            this.tableLayoutPanel7.Controls.Add(this.flowLayoutPanel1, 1, 5);
            this.tableLayoutPanel7.Controls.Add(this.tableLayoutPanel8, 1, 3);
            this.tableLayoutPanel7.Controls.Add(this.label20, 0, 4);
            this.tableLayoutPanel7.Controls.Add(this.tbAdditionalRelevantObjects, 1, 4);
            this.tableLayoutPanel7.Location = new System.Drawing.Point(3, 30);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.RowCount = 8;
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.3264F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.822679F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.07454F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 28.46057F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.32999F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.3264F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.33084F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.32858F));
            this.tableLayoutPanel7.Size = new System.Drawing.Size(815, 369);
            this.tableLayoutPanel7.TabIndex = 2;
            this.tableLayoutPanel7.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel7_Paint);
            // 
            // label14
            // 
            this.label14.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label14.Location = new System.Drawing.Point(65, 85);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(75, 15);
            this.label14.TabIndex = 17;
            this.label14.Text = "Input Folder";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(92, 302);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 15);
            this.label1.TabIndex = 9;
            this.label1.Text = "Actions";
            // 
            // lblPrefix
            // 
            this.lblPrefix.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblPrefix.AutoSize = true;
            this.lblPrefix.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblPrefix.Location = new System.Drawing.Point(15, 49);
            this.lblPrefix.Name = "lblPrefix";
            this.lblPrefix.Size = new System.Drawing.Size(125, 15);
            this.lblPrefix.TabIndex = 2;
            this.lblPrefix.Text = "Input file begins with";
            this.lblPrefix.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblName
            // 
            this.lblName.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblName.AutoSize = true;
            this.lblName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblName.Location = new System.Drawing.Point(100, 12);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(40, 15);
            this.lblName.TabIndex = 10;
            this.lblName.Text = "Name";
            // 
            // tableLayoutPanel12
            // 
            this.tableLayoutPanel12.ColumnCount = 2;
            this.tableLayoutPanel12.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel12.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel12.Controls.Add(this.lbl_prefix, 1, 0);
            this.tableLayoutPanel12.Controls.Add(this.tbPrefix, 0, 0);
            this.tableLayoutPanel12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel12.Location = new System.Drawing.Point(146, 41);
            this.tableLayoutPanel12.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel12.Name = "tableLayoutPanel12";
            this.tableLayoutPanel12.RowCount = 1;
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel12.Size = new System.Drawing.Size(666, 31);
            this.tableLayoutPanel12.TabIndex = 12;
            // 
            // lbl_prefix
            // 
            this.lbl_prefix.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbl_prefix.AutoSize = true;
            this.lbl_prefix.Location = new System.Drawing.Point(499, 8);
            this.lbl_prefix.Name = "lbl_prefix";
            this.lbl_prefix.Size = new System.Drawing.Size(0, 15);
            this.lbl_prefix.TabIndex = 6;
            // 
            // tbPrefix
            // 
            this.tbPrefix.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPrefix.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tbPrefix.Location = new System.Drawing.Point(21, 4);
            this.tbPrefix.Margin = new System.Windows.Forms.Padding(21, 3, 21, 3);
            this.tbPrefix.Name = "tbPrefix";
            this.tbPrefix.Size = new System.Drawing.Size(291, 23);
            this.tbPrefix.TabIndex = 3;
            this.tbPrefix.TextChanged += new System.EventHandler(this.tbPrefix_TextChanged);
            // 
            // tableLayoutPanel13
            // 
            this.tableLayoutPanel13.ColumnCount = 2;
            this.tableLayoutPanel13.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel13.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel13.Controls.Add(this.cb_enabled, 1, 0);
            this.tableLayoutPanel13.Controls.Add(this.tbName, 0, 0);
            this.tableLayoutPanel13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel13.Location = new System.Drawing.Point(147, 4);
            this.tableLayoutPanel13.Name = "tableLayoutPanel13";
            this.tableLayoutPanel13.RowCount = 1;
            this.tableLayoutPanel13.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel13.Size = new System.Drawing.Size(664, 31);
            this.tableLayoutPanel13.TabIndex = 13;
            // 
            // cb_enabled
            // 
            this.cb_enabled.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cb_enabled.AutoSize = true;
            this.cb_enabled.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cb_enabled.Location = new System.Drawing.Point(353, 6);
            this.cb_enabled.Margin = new System.Windows.Forms.Padding(21, 3, 3, 3);
            this.cb_enabled.Name = "cb_enabled";
            this.cb_enabled.Size = new System.Drawing.Size(211, 19);
            this.cb_enabled.TabIndex = 2;
            this.cb_enabled.Text = "Enable AI Detection for this camera";
            this.cb_enabled.UseVisualStyleBackColor = true;
            // 
            // tbName
            // 
            this.tbName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tbName.Location = new System.Drawing.Point(21, 4);
            this.tbName.Margin = new System.Windows.Forms.Padding(21, 3, 21, 3);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(290, 23);
            this.tbName.TabIndex = 1;
            // 
            // lblRelevantObjects
            // 
            this.lblRelevantObjects.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblRelevantObjects.AutoSize = true;
            this.lblRelevantObjects.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblRelevantObjects.Location = new System.Drawing.Point(38, 155);
            this.lblRelevantObjects.Name = "lblRelevantObjects";
            this.lblRelevantObjects.Size = new System.Drawing.Size(102, 15);
            this.lblRelevantObjects.TabIndex = 1;
            this.lblRelevantObjects.Text = "Relevant Objects";
            // 
            // lbl_threshold
            // 
            this.lbl_threshold.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lbl_threshold.AutoSize = true;
            this.lbl_threshold.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lbl_threshold.Location = new System.Drawing.Point(37, 264);
            this.lbl_threshold.Name = "lbl_threshold";
            this.lbl_threshold.Size = new System.Drawing.Size(103, 15);
            this.lbl_threshold.TabIndex = 15;
            this.lbl_threshold.Text = "Confidence limits";
            // 
            // tableLayoutPanel26
            // 
            this.tableLayoutPanel26.ColumnCount = 3;
            this.tableLayoutPanel26.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 62.5F));
            this.tableLayoutPanel26.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel26.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel26.Controls.Add(this.cmbcaminput, 0, 0);
            this.tableLayoutPanel26.Controls.Add(this.cb_monitorCamInputfolder, 1, 0);
            this.tableLayoutPanel26.Controls.Add(this.button2, 2, 0);
            this.tableLayoutPanel26.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel26.Location = new System.Drawing.Point(146, 77);
            this.tableLayoutPanel26.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel26.Name = "tableLayoutPanel26";
            this.tableLayoutPanel26.RowCount = 1;
            this.tableLayoutPanel26.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel26.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel26.Size = new System.Drawing.Size(666, 32);
            this.tableLayoutPanel26.TabIndex = 18;
            // 
            // cmbcaminput
            // 
            this.cmbcaminput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbcaminput.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbcaminput.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbcaminput.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbcaminput.FormattingEnabled = true;
            this.cmbcaminput.Location = new System.Drawing.Point(21, 4);
            this.cmbcaminput.Margin = new System.Windows.Forms.Padding(21, 2, 21, 2);
            this.cmbcaminput.Name = "cmbcaminput";
            this.cmbcaminput.Size = new System.Drawing.Size(374, 23);
            this.cmbcaminput.TabIndex = 7;
            // 
            // cb_monitorCamInputfolder
            // 
            this.cb_monitorCamInputfolder.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cb_monitorCamInputfolder.AutoSize = true;
            this.cb_monitorCamInputfolder.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cb_monitorCamInputfolder.Location = new System.Drawing.Point(435, 6);
            this.cb_monitorCamInputfolder.Margin = new System.Windows.Forms.Padding(2);
            this.cb_monitorCamInputfolder.Name = "cb_monitorCamInputfolder";
            this.cb_monitorCamInputfolder.Size = new System.Drawing.Size(128, 19);
            this.cb_monitorCamInputfolder.TabIndex = 5;
            this.cb_monitorCamInputfolder.Text = "Monitor Subfolders";
            this.cb_monitorCamInputfolder.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.button2.Location = new System.Drawing.Point(589, 3);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(70, 26);
            this.button2.TabIndex = 6;
            this.button2.Text = "Select...";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label15
            // 
            this.label15.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label15.Location = new System.Drawing.Point(88, 341);
            this.label15.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(53, 15);
            this.label15.TabIndex = 19;
            this.label15.Text = "Masking";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tableLayoutPanel27
            // 
            this.tableLayoutPanel27.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel27.ColumnCount = 5;
            this.tableLayoutPanel27.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 29.63504F));
            this.tableLayoutPanel27.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.54895F));
            this.tableLayoutPanel27.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.23776F));
            this.tableLayoutPanel27.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.8986F));
            this.tableLayoutPanel27.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30.94406F));
            this.tableLayoutPanel27.Controls.Add(this.cb_masking_enabled, 0, 0);
            this.tableLayoutPanel27.Controls.Add(this.BtnDynamicMaskingSettings, 1, 0);
            this.tableLayoutPanel27.Controls.Add(this.btnDetails, 2, 0);
            this.tableLayoutPanel27.Controls.Add(this.btnCustomMask, 4, 0);
            this.tableLayoutPanel27.Controls.Add(this.lblDrawMask, 3, 0);
            this.tableLayoutPanel27.Location = new System.Drawing.Point(146, 330);
            this.tableLayoutPanel27.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.tableLayoutPanel27.Name = "tableLayoutPanel27";
            this.tableLayoutPanel27.RowCount = 1;
            this.tableLayoutPanel27.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel27.Size = new System.Drawing.Size(666, 37);
            this.tableLayoutPanel27.TabIndex = 20;
            // 
            // cb_masking_enabled
            // 
            this.cb_masking_enabled.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cb_masking_enabled.AutoSize = true;
            this.cb_masking_enabled.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cb_masking_enabled.Location = new System.Drawing.Point(21, 10);
            this.cb_masking_enabled.Margin = new System.Windows.Forms.Padding(21, 3, 5, 0);
            this.cb_masking_enabled.Name = "cb_masking_enabled";
            this.cb_masking_enabled.Size = new System.Drawing.Size(158, 19);
            this.cb_masking_enabled.TabIndex = 21;
            this.cb_masking_enabled.Text = "Enable dynamic masking";
            this.cb_masking_enabled.UseVisualStyleBackColor = true;
            // 
            // BtnDynamicMaskingSettings
            // 
            this.BtnDynamicMaskingSettings.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.BtnDynamicMaskingSettings.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.BtnDynamicMaskingSettings.Location = new System.Drawing.Point(201, 3);
            this.BtnDynamicMaskingSettings.Margin = new System.Windows.Forms.Padding(5, 1, 5, 1);
            this.BtnDynamicMaskingSettings.Name = "BtnDynamicMaskingSettings";
            this.BtnDynamicMaskingSettings.Size = new System.Drawing.Size(70, 30);
            this.BtnDynamicMaskingSettings.TabIndex = 22;
            this.BtnDynamicMaskingSettings.Text = "Settings";
            this.BtnDynamicMaskingSettings.UseVisualStyleBackColor = true;
            this.BtnDynamicMaskingSettings.Click += new System.EventHandler(this.BtnDynamicMaskingSettings_Click);
            // 
            // btnDetails
            // 
            this.btnDetails.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnDetails.Location = new System.Drawing.Point(290, 3);
            this.btnDetails.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.btnDetails.Name = "btnDetails";
            this.btnDetails.Size = new System.Drawing.Size(70, 30);
            this.btnDetails.TabIndex = 23;
            this.btnDetails.Text = "Details";
            this.btnDetails.UseVisualStyleBackColor = true;
            this.btnDetails.Click += new System.EventHandler(this.btnDetails_Click);
            // 
            // btnCustomMask
            // 
            this.btnCustomMask.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnCustomMask.Location = new System.Drawing.Point(459, 3);
            this.btnCustomMask.Margin = new System.Windows.Forms.Padding(1, 2, 5, 2);
            this.btnCustomMask.Name = "btnCustomMask";
            this.btnCustomMask.Size = new System.Drawing.Size(70, 30);
            this.btnCustomMask.TabIndex = 24;
            this.btnCustomMask.Text = "Custom";
            this.btnCustomMask.UseVisualStyleBackColor = true;
            this.btnCustomMask.Click += new System.EventHandler(this.btnCustomMask_Click);
            // 
            // lblDrawMask
            // 
            this.lblDrawMask.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblDrawMask.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblDrawMask.Location = new System.Drawing.Point(391, 10);
            this.lblDrawMask.Margin = new System.Windows.Forms.Padding(0);
            this.lblDrawMask.Name = "lblDrawMask";
            this.lblDrawMask.Size = new System.Drawing.Size(67, 16);
            this.lblDrawMask.TabIndex = 25;
            this.lblDrawMask.Text = "Draw Mask";
            this.lblDrawMask.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnActions
            // 
            this.btnActions.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnActions.Location = new System.Drawing.Point(165, 294);
            this.btnActions.Margin = new System.Windows.Forms.Padding(21, 2, 2, 2);
            this.btnActions.Name = "btnActions";
            this.btnActions.Size = new System.Drawing.Size(70, 30);
            this.btnActions.TabIndex = 21;
            this.btnActions.Text = "Settings";
            this.btnActions.UseVisualStyleBackColor = true;
            this.btnActions.Click += new System.EventHandler(this.btnActions_Click_1);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.flowLayoutPanel1.Controls.Add(this.lbl_threshold_lower);
            this.flowLayoutPanel1.Controls.Add(this.tb_threshold_lower);
            this.flowLayoutPanel1.Controls.Add(this.label9);
            this.flowLayoutPanel1.Controls.Add(this.lbl_threshold_upper);
            this.flowLayoutPanel1.Controls.Add(this.tb_threshold_upper);
            this.flowLayoutPanel1.Controls.Add(this.label10);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(147, 257);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(500, 29);
            this.flowLayoutPanel1.TabIndex = 22;
            // 
            // lbl_threshold_lower
            // 
            this.lbl_threshold_lower.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lbl_threshold_lower.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lbl_threshold_lower.Location = new System.Drawing.Point(21, 6);
            this.lbl_threshold_lower.Margin = new System.Windows.Forms.Padding(21, 0, 3, 0);
            this.lbl_threshold_lower.Name = "lbl_threshold_lower";
            this.lbl_threshold_lower.Size = new System.Drawing.Size(40, 16);
            this.lbl_threshold_lower.TabIndex = 24;
            this.lbl_threshold_lower.Text = "Lower limit";
            // 
            // tb_threshold_lower
            // 
            this.tb_threshold_lower.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.tb_threshold_lower.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tb_threshold_lower.Location = new System.Drawing.Point(67, 3);
            this.tb_threshold_lower.Margin = new System.Windows.Forms.Padding(3, 3, 1, 3);
            this.tb_threshold_lower.MaxLength = 3;
            this.tb_threshold_lower.Name = "tb_threshold_lower";
            this.tb_threshold_lower.Size = new System.Drawing.Size(34, 23);
            this.tb_threshold_lower.TabIndex = 25;
            this.tb_threshold_lower.Leave += new System.EventHandler(this.tb_threshold_lower_Leave);
            // 
            // label9
            // 
            this.label9.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(103, 6);
            this.label9.Margin = new System.Windows.Forms.Padding(1, 0, 3, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(19, 17);
            this.label9.TabIndex = 28;
            this.label9.Text = "%";
            // 
            // lbl_threshold_upper
            // 
            this.lbl_threshold_upper.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lbl_threshold_upper.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lbl_threshold_upper.Location = new System.Drawing.Point(146, 6);
            this.lbl_threshold_upper.Margin = new System.Windows.Forms.Padding(21, 0, 1, 0);
            this.lbl_threshold_upper.Name = "lbl_threshold_upper";
            this.lbl_threshold_upper.Size = new System.Drawing.Size(40, 16);
            this.lbl_threshold_upper.TabIndex = 27;
            this.lbl_threshold_upper.Text = "Upper limit";
            // 
            // tb_threshold_upper
            // 
            this.tb_threshold_upper.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.tb_threshold_upper.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tb_threshold_upper.Location = new System.Drawing.Point(190, 3);
            this.tb_threshold_upper.Margin = new System.Windows.Forms.Padding(3, 3, 1, 3);
            this.tb_threshold_upper.MaxLength = 3;
            this.tb_threshold_upper.Name = "tb_threshold_upper";
            this.tb_threshold_upper.Size = new System.Drawing.Size(34, 23);
            this.tb_threshold_upper.TabIndex = 26;
            this.tb_threshold_upper.Leave += new System.EventHandler(this.tb_threshold_upper_Leave);
            // 
            // label10
            // 
            this.label10.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label10.Location = new System.Drawing.Point(226, 7);
            this.label10.Margin = new System.Windows.Forms.Padding(1, 0, 3, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(17, 15);
            this.label10.TabIndex = 29;
            this.label10.Text = "%";
            // 
            // tableLayoutPanel8
            // 
            this.tableLayoutPanel8.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel8.ColumnCount = 5;
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel8.Controls.Add(this.cb_person, 0, 0);
            this.tableLayoutPanel8.Controls.Add(this.cb_bicycle, 0, 1);
            this.tableLayoutPanel8.Controls.Add(this.cb_motorcycle, 0, 2);
            this.tableLayoutPanel8.Controls.Add(this.cb_bear, 4, 2);
            this.tableLayoutPanel8.Controls.Add(this.cb_cow, 4, 1);
            this.tableLayoutPanel8.Controls.Add(this.cb_sheep, 4, 0);
            this.tableLayoutPanel8.Controls.Add(this.cb_horse, 3, 2);
            this.tableLayoutPanel8.Controls.Add(this.cb_bird, 3, 1);
            this.tableLayoutPanel8.Controls.Add(this.cb_dog, 3, 0);
            this.tableLayoutPanel8.Controls.Add(this.cb_cat, 2, 2);
            this.tableLayoutPanel8.Controls.Add(this.cb_airplane, 2, 1);
            this.tableLayoutPanel8.Controls.Add(this.cb_boat, 2, 0);
            this.tableLayoutPanel8.Controls.Add(this.cb_bus, 1, 2);
            this.tableLayoutPanel8.Controls.Add(this.cb_truck, 1, 1);
            this.tableLayoutPanel8.Controls.Add(this.cb_car, 1, 0);
            this.tableLayoutPanel8.Location = new System.Drawing.Point(147, 115);
            this.tableLayoutPanel8.Name = "tableLayoutPanel8";
            this.tableLayoutPanel8.RowCount = 3;
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.tableLayoutPanel8.Size = new System.Drawing.Size(664, 96);
            this.tableLayoutPanel8.TabIndex = 14;
            // 
            // cb_person
            // 
            this.cb_person.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cb_person.AutoSize = true;
            this.cb_person.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cb_person.Location = new System.Drawing.Point(21, 6);
            this.cb_person.Margin = new System.Windows.Forms.Padding(21, 3, 3, 3);
            this.cb_person.Name = "cb_person";
            this.cb_person.Size = new System.Drawing.Size(62, 19);
            this.cb_person.TabIndex = 4;
            this.cb_person.Text = "Person";
            this.cb_person.UseVisualStyleBackColor = true;
            this.cb_person.CheckedChanged += new System.EventHandler(this.cb_person_CheckedChanged);
            // 
            // cb_bicycle
            // 
            this.cb_bicycle.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cb_bicycle.AutoSize = true;
            this.cb_bicycle.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cb_bicycle.Location = new System.Drawing.Point(21, 38);
            this.cb_bicycle.Margin = new System.Windows.Forms.Padding(21, 3, 3, 3);
            this.cb_bicycle.Name = "cb_bicycle";
            this.cb_bicycle.Size = new System.Drawing.Size(63, 19);
            this.cb_bicycle.TabIndex = 9;
            this.cb_bicycle.Text = "Bicycle";
            this.cb_bicycle.UseVisualStyleBackColor = true;
            // 
            // cb_motorcycle
            // 
            this.cb_motorcycle.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cb_motorcycle.AutoSize = true;
            this.cb_motorcycle.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cb_motorcycle.Location = new System.Drawing.Point(21, 70);
            this.cb_motorcycle.Margin = new System.Windows.Forms.Padding(21, 3, 3, 3);
            this.cb_motorcycle.Name = "cb_motorcycle";
            this.cb_motorcycle.Size = new System.Drawing.Size(86, 19);
            this.cb_motorcycle.TabIndex = 14;
            this.cb_motorcycle.Text = "Motorcycle";
            this.cb_motorcycle.UseVisualStyleBackColor = true;
            // 
            // cb_bear
            // 
            this.cb_bear.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cb_bear.AutoSize = true;
            this.cb_bear.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cb_bear.Location = new System.Drawing.Point(549, 70);
            this.cb_bear.Margin = new System.Windows.Forms.Padding(21, 3, 3, 3);
            this.cb_bear.Name = "cb_bear";
            this.cb_bear.Size = new System.Drawing.Size(49, 19);
            this.cb_bear.TabIndex = 18;
            this.cb_bear.Text = "Bear";
            this.cb_bear.UseVisualStyleBackColor = true;
            // 
            // cb_cow
            // 
            this.cb_cow.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cb_cow.AutoSize = true;
            this.cb_cow.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cb_cow.Location = new System.Drawing.Point(549, 38);
            this.cb_cow.Margin = new System.Windows.Forms.Padding(21, 3, 3, 3);
            this.cb_cow.Name = "cb_cow";
            this.cb_cow.Size = new System.Drawing.Size(50, 19);
            this.cb_cow.TabIndex = 13;
            this.cb_cow.Text = "Cow";
            this.cb_cow.UseVisualStyleBackColor = true;
            // 
            // cb_sheep
            // 
            this.cb_sheep.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cb_sheep.AutoSize = true;
            this.cb_sheep.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cb_sheep.Location = new System.Drawing.Point(549, 6);
            this.cb_sheep.Margin = new System.Windows.Forms.Padding(21, 3, 3, 3);
            this.cb_sheep.Name = "cb_sheep";
            this.cb_sheep.Size = new System.Drawing.Size(58, 19);
            this.cb_sheep.TabIndex = 8;
            this.cb_sheep.Text = "Sheep";
            this.cb_sheep.UseVisualStyleBackColor = true;
            // 
            // cb_horse
            // 
            this.cb_horse.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cb_horse.AutoSize = true;
            this.cb_horse.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cb_horse.Location = new System.Drawing.Point(417, 70);
            this.cb_horse.Margin = new System.Windows.Forms.Padding(21, 3, 3, 3);
            this.cb_horse.Name = "cb_horse";
            this.cb_horse.Size = new System.Drawing.Size(57, 19);
            this.cb_horse.TabIndex = 17;
            this.cb_horse.Text = "Horse";
            this.cb_horse.UseVisualStyleBackColor = true;
            // 
            // cb_bird
            // 
            this.cb_bird.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cb_bird.AutoSize = true;
            this.cb_bird.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cb_bird.Location = new System.Drawing.Point(417, 38);
            this.cb_bird.Margin = new System.Windows.Forms.Padding(21, 3, 3, 3);
            this.cb_bird.Name = "cb_bird";
            this.cb_bird.Size = new System.Drawing.Size(47, 19);
            this.cb_bird.TabIndex = 12;
            this.cb_bird.Text = "Bird";
            this.cb_bird.UseVisualStyleBackColor = true;
            // 
            // cb_dog
            // 
            this.cb_dog.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cb_dog.AutoSize = true;
            this.cb_dog.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cb_dog.Location = new System.Drawing.Point(417, 6);
            this.cb_dog.Margin = new System.Windows.Forms.Padding(21, 3, 3, 3);
            this.cb_dog.Name = "cb_dog";
            this.cb_dog.Size = new System.Drawing.Size(48, 19);
            this.cb_dog.TabIndex = 7;
            this.cb_dog.Text = "Dog";
            this.cb_dog.UseVisualStyleBackColor = true;
            // 
            // cb_cat
            // 
            this.cb_cat.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cb_cat.AutoSize = true;
            this.cb_cat.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cb_cat.Location = new System.Drawing.Point(285, 70);
            this.cb_cat.Margin = new System.Windows.Forms.Padding(21, 3, 3, 3);
            this.cb_cat.Name = "cb_cat";
            this.cb_cat.Size = new System.Drawing.Size(44, 19);
            this.cb_cat.TabIndex = 16;
            this.cb_cat.Text = "Cat";
            this.cb_cat.UseVisualStyleBackColor = true;
            // 
            // cb_airplane
            // 
            this.cb_airplane.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cb_airplane.AutoSize = true;
            this.cb_airplane.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cb_airplane.Location = new System.Drawing.Point(285, 38);
            this.cb_airplane.Margin = new System.Windows.Forms.Padding(21, 3, 3, 3);
            this.cb_airplane.Name = "cb_airplane";
            this.cb_airplane.Size = new System.Drawing.Size(70, 19);
            this.cb_airplane.TabIndex = 11;
            this.cb_airplane.Text = "Airplane";
            this.cb_airplane.UseVisualStyleBackColor = true;
            // 
            // cb_boat
            // 
            this.cb_boat.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cb_boat.AutoSize = true;
            this.cb_boat.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cb_boat.Location = new System.Drawing.Point(285, 6);
            this.cb_boat.Margin = new System.Windows.Forms.Padding(21, 3, 3, 3);
            this.cb_boat.Name = "cb_boat";
            this.cb_boat.Size = new System.Drawing.Size(50, 19);
            this.cb_boat.TabIndex = 6;
            this.cb_boat.Text = "Boat";
            this.cb_boat.UseVisualStyleBackColor = true;
            // 
            // cb_bus
            // 
            this.cb_bus.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cb_bus.AutoSize = true;
            this.cb_bus.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cb_bus.Location = new System.Drawing.Point(153, 70);
            this.cb_bus.Margin = new System.Windows.Forms.Padding(21, 3, 3, 3);
            this.cb_bus.Name = "cb_bus";
            this.cb_bus.Size = new System.Drawing.Size(45, 19);
            this.cb_bus.TabIndex = 15;
            this.cb_bus.Text = "Bus";
            this.cb_bus.UseVisualStyleBackColor = true;
            // 
            // cb_truck
            // 
            this.cb_truck.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cb_truck.AutoSize = true;
            this.cb_truck.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cb_truck.Location = new System.Drawing.Point(153, 38);
            this.cb_truck.Margin = new System.Windows.Forms.Padding(21, 3, 3, 3);
            this.cb_truck.Name = "cb_truck";
            this.cb_truck.Size = new System.Drawing.Size(54, 19);
            this.cb_truck.TabIndex = 10;
            this.cb_truck.Text = "Truck";
            this.cb_truck.UseVisualStyleBackColor = true;
            // 
            // cb_car
            // 
            this.cb_car.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cb_car.AutoSize = true;
            this.cb_car.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cb_car.Location = new System.Drawing.Point(153, 6);
            this.cb_car.Margin = new System.Windows.Forms.Padding(21, 3, 3, 3);
            this.cb_car.Name = "cb_car";
            this.cb_car.Size = new System.Drawing.Size(44, 19);
            this.cb_car.TabIndex = 5;
            this.cb_car.Text = "Car";
            this.cb_car.UseVisualStyleBackColor = true;
            // 
            // label20
            // 
            this.label20.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label20.Location = new System.Drawing.Point(24, 218);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(116, 30);
            this.label20.TabIndex = 1;
            this.label20.Text = "Additional Relevant Objects";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tbAdditionalRelevantObjects
            // 
            this.tbAdditionalRelevantObjects.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbAdditionalRelevantObjects.Location = new System.Drawing.Point(147, 222);
            this.tbAdditionalRelevantObjects.Name = "tbAdditionalRelevantObjects";
            this.tbAdditionalRelevantObjects.Size = new System.Drawing.Size(664, 23);
            this.tbAdditionalRelevantObjects.TabIndex = 23;
            this.toolTip1.SetToolTip(this.tbAdditionalRelevantObjects, "comma separated list of custom object names to be accepted as relevant.");
            // 
            // tabSettings
            // 
            this.tabSettings.Controls.Add(this.tableLayoutPanel4);
            this.tabSettings.Location = new System.Drawing.Point(4, 22);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Size = new System.Drawing.Size(1027, 455);
            this.tabSettings.TabIndex = 3;
            this.tabSettings.Text = "Settings";
            this.tabSettings.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel4.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Controls.Add(this.BtnSettingsSave, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel5, 0, 0);
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 2;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(1022, 447);
            this.tableLayoutPanel4.TabIndex = 5;
            // 
            // BtnSettingsSave
            // 
            this.BtnSettingsSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.BtnSettingsSave.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnSettingsSave.Location = new System.Drawing.Point(476, 410);
            this.BtnSettingsSave.Name = "BtnSettingsSave";
            this.BtnSettingsSave.Size = new System.Drawing.Size(70, 34);
            this.BtnSettingsSave.TabIndex = 2;
            this.BtnSettingsSave.Text = "Save";
            this.BtnSettingsSave.UseVisualStyleBackColor = true;
            this.BtnSettingsSave.Click += new System.EventHandler(this.BtnSettingsSave_Click_1);
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel5.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel5.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel5.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel5.ColumnCount = 2;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 85F));
            this.tableLayoutPanel5.Controls.Add(this.lbl_input, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.lbl_telegram_token, 0, 2);
            this.tableLayoutPanel5.Controls.Add(this.tableLayoutPanel18, 1, 0);
            this.tableLayoutPanel5.Controls.Add(this.label13, 0, 4);
            this.tableLayoutPanel5.Controls.Add(this.cbStartWithWindows, 1, 4);
            this.tableLayoutPanel5.Controls.Add(this.lbl_deepstackurl, 0, 1);
            this.tableLayoutPanel5.Controls.Add(this.dbLayoutPanel1, 1, 1);
            this.tableLayoutPanel5.Controls.Add(this.label12, 0, 3);
            this.tableLayoutPanel5.Controls.Add(this.label4, 0, 5);
            this.tableLayoutPanel5.Controls.Add(this.dbLayoutPanel4, 1, 5);
            this.tableLayoutPanel5.Controls.Add(this.label18, 0, 6);
            this.tableLayoutPanel5.Controls.Add(this.dbLayoutPanel5, 1, 6);
            this.tableLayoutPanel5.Controls.Add(this.dbLayoutPanel2, 1, 2);
            this.tableLayoutPanel5.Controls.Add(this.dbLayoutPanel3, 1, 3);
            this.tableLayoutPanel5.Controls.Add(this.MinimizeBehaviour, 0, 7);
            this.tableLayoutPanel5.Controls.Add(this.minimizeToTrayCheckbox, 1, 7);
            this.tableLayoutPanel5.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 8;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(1016, 401);
            this.tableLayoutPanel5.TabIndex = 3;
            // 
            // lbl_input
            // 
            this.lbl_input.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lbl_input.AutoSize = true;
            this.lbl_input.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_input.Location = new System.Drawing.Point(26, 17);
            this.lbl_input.Name = "lbl_input";
            this.lbl_input.Size = new System.Drawing.Size(123, 17);
            this.lbl_input.TabIndex = 1;
            this.lbl_input.Text = "Default Input Path";
            // 
            // lbl_telegram_token
            // 
            this.lbl_telegram_token.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lbl_telegram_token.AutoSize = true;
            this.lbl_telegram_token.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_telegram_token.Location = new System.Drawing.Point(43, 117);
            this.lbl_telegram_token.Name = "lbl_telegram_token";
            this.lbl_telegram_token.Size = new System.Drawing.Size(106, 17);
            this.lbl_telegram_token.TabIndex = 6;
            this.lbl_telegram_token.Text = "Telegram Token";
            // 
            // tableLayoutPanel18
            // 
            this.tableLayoutPanel18.ColumnCount = 3;
            this.tableLayoutPanel18.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 72.9927F));
            this.tableLayoutPanel18.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.43458F));
            this.tableLayoutPanel18.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.66822F));
            this.tableLayoutPanel18.Controls.Add(this.btn_input_path, 2, 0);
            this.tableLayoutPanel18.Controls.Add(this.cmbInput, 0, 0);
            this.tableLayoutPanel18.Controls.Add(this.cb_inputpathsubfolders, 1, 0);
            this.tableLayoutPanel18.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel18.Location = new System.Drawing.Point(156, 4);
            this.tableLayoutPanel18.Name = "tableLayoutPanel18";
            this.tableLayoutPanel18.RowCount = 1;
            this.tableLayoutPanel18.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel18.Size = new System.Drawing.Size(856, 43);
            this.tableLayoutPanel18.TabIndex = 12;
            // 
            // btn_input_path
            // 
            this.btn_input_path.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btn_input_path.Location = new System.Drawing.Point(762, 6);
            this.btn_input_path.Name = "btn_input_path";
            this.btn_input_path.Size = new System.Drawing.Size(70, 30);
            this.btn_input_path.TabIndex = 2;
            this.btn_input_path.Text = "Select...";
            this.btn_input_path.UseVisualStyleBackColor = true;
            this.btn_input_path.Click += new System.EventHandler(this.btn_input_path_Click);
            // 
            // cmbInput
            // 
            this.cmbInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbInput.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbInput.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbInput.FormattingEnabled = true;
            this.cmbInput.Location = new System.Drawing.Point(3, 11);
            this.cmbInput.Margin = new System.Windows.Forms.Padding(3, 2, 2, 2);
            this.cmbInput.Name = "cmbInput";
            this.cmbInput.Size = new System.Drawing.Size(619, 21);
            this.cmbInput.TabIndex = 3;
            // 
            // cb_inputpathsubfolders
            // 
            this.cb_inputpathsubfolders.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cb_inputpathsubfolders.AutoSize = true;
            this.cb_inputpathsubfolders.Location = new System.Drawing.Point(635, 13);
            this.cb_inputpathsubfolders.Margin = new System.Windows.Forms.Padding(11, 2, 2, 2);
            this.cb_inputpathsubfolders.Name = "cb_inputpathsubfolders";
            this.cb_inputpathsubfolders.Size = new System.Drawing.Size(76, 17);
            this.cb_inputpathsubfolders.TabIndex = 4;
            this.cb_inputpathsubfolders.Text = "Subfolders";
            this.cb_inputpathsubfolders.UseVisualStyleBackColor = true;
            // 
            // label13
            // 
            this.label13.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.label13.Location = new System.Drawing.Point(113, 217);
            this.label13.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(37, 17);
            this.label13.TabIndex = 16;
            this.label13.Text = "Start";
            // 
            // cbStartWithWindows
            // 
            this.cbStartWithWindows.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cbStartWithWindows.AutoSize = true;
            this.cbStartWithWindows.Location = new System.Drawing.Point(155, 217);
            this.cbStartWithWindows.Margin = new System.Windows.Forms.Padding(2);
            this.cbStartWithWindows.Name = "cbStartWithWindows";
            this.cbStartWithWindows.Size = new System.Drawing.Size(182, 17);
            this.cbStartWithWindows.TabIndex = 17;
            this.cbStartWithWindows.Text = "Start with user login (non-service)";
            this.cbStartWithWindows.UseVisualStyleBackColor = true;
            // 
            // lbl_deepstackurl
            // 
            this.lbl_deepstackurl.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lbl_deepstackurl.AutoSize = true;
            this.lbl_deepstackurl.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_deepstackurl.Location = new System.Drawing.Point(41, 67);
            this.lbl_deepstackurl.Name = "lbl_deepstackurl";
            this.lbl_deepstackurl.Size = new System.Drawing.Size(108, 17);
            this.lbl_deepstackurl.TabIndex = 4;
            this.lbl_deepstackurl.Text = "AI Server URL(s)";
            // 
            // dbLayoutPanel1
            // 
            this.dbLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dbLayoutPanel1.ColumnCount = 2;
            this.dbLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 72.99664F));
            this.dbLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 27.00337F));
            this.dbLayoutPanel1.Controls.Add(this.tbDeepstackUrl, 0, 0);
            this.dbLayoutPanel1.Controls.Add(this.cb_DeepStackURLsQueued, 1, 0);
            this.dbLayoutPanel1.Location = new System.Drawing.Point(155, 53);
            this.dbLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.dbLayoutPanel1.Name = "dbLayoutPanel1";
            this.dbLayoutPanel1.RowCount = 1;
            this.dbLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.dbLayoutPanel1.Size = new System.Drawing.Size(858, 45);
            this.dbLayoutPanel1.TabIndex = 18;
            // 
            // tbDeepstackUrl
            // 
            this.tbDeepstackUrl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbDeepstackUrl.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbDeepstackUrl.Location = new System.Drawing.Point(3, 10);
            this.tbDeepstackUrl.Name = "tbDeepstackUrl";
            this.tbDeepstackUrl.Size = new System.Drawing.Size(620, 25);
            this.tbDeepstackUrl.TabIndex = 6;
            this.toolTip1.SetToolTip(this.tbDeepstackUrl, "Enter SERVER:PORT for DeepStack server - Use ; to separate more than one URL for " +
        "parallel processing");
            // 
            // cb_DeepStackURLsQueued
            // 
            this.cb_DeepStackURLsQueued.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cb_DeepStackURLsQueued.AutoSize = true;
            this.cb_DeepStackURLsQueued.Location = new System.Drawing.Point(637, 14);
            this.cb_DeepStackURLsQueued.Margin = new System.Windows.Forms.Padding(11, 2, 2, 2);
            this.cb_DeepStackURLsQueued.Name = "cb_DeepStackURLsQueued";
            this.cb_DeepStackURLsQueued.Size = new System.Drawing.Size(64, 17);
            this.cb_DeepStackURLsQueued.TabIndex = 7;
            this.cb_DeepStackURLsQueued.Text = "Queued";
            this.toolTip1.SetToolTip(this.cb_DeepStackURLsQueued, "When checked, all urls will take turns processing the images.\r\nWhen unchecked, th" +
        "e original order will always be used.");
            this.cb_DeepStackURLsQueued.UseVisualStyleBackColor = true;
            // 
            // label12
            // 
            this.label12.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(31, 167);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(118, 17);
            this.label12.TabIndex = 13;
            this.label12.Text = "Send To Telegram";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.label4.Location = new System.Drawing.Point(23, 267);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(127, 17);
            this.label4.TabIndex = 16;
            this.label4.Text = "Default Credentials";
            // 
            // dbLayoutPanel4
            // 
            this.dbLayoutPanel4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dbLayoutPanel4.ColumnCount = 5;
            this.dbLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.dbLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.dbLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.dbLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.dbLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 406F));
            this.dbLayoutPanel4.Controls.Add(this.label6, 0, 0);
            this.dbLayoutPanel4.Controls.Add(this.tb_username, 1, 0);
            this.dbLayoutPanel4.Controls.Add(this.tb_password, 3, 0);
            this.dbLayoutPanel4.Controls.Add(this.label16, 2, 0);
            this.dbLayoutPanel4.Controls.Add(this.label17, 4, 0);
            this.dbLayoutPanel4.Location = new System.Drawing.Point(156, 254);
            this.dbLayoutPanel4.Name = "dbLayoutPanel4";
            this.dbLayoutPanel4.RowCount = 1;
            this.dbLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.dbLayoutPanel4.Size = new System.Drawing.Size(856, 43);
            this.dbLayoutPanel4.TabIndex = 21;
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(14, 15);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Username:";
            // 
            // tb_username
            // 
            this.tb_username.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_username.Location = new System.Drawing.Point(78, 11);
            this.tb_username.Name = "tb_username";
            this.tb_username.Size = new System.Drawing.Size(144, 20);
            this.tb_username.TabIndex = 1;
            // 
            // tb_password
            // 
            this.tb_password.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_password.Location = new System.Drawing.Point(303, 11);
            this.tb_password.Name = "tb_password";
            this.tb_password.Size = new System.Drawing.Size(144, 20);
            this.tb_password.TabIndex = 2;
            // 
            // label16
            // 
            this.label16.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(241, 15);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(56, 13);
            this.label16.TabIndex = 0;
            this.label16.Text = "Password:";
            // 
            // label17
            // 
            this.label17.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(453, 8);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(400, 26);
            this.label17.TabIndex = 3;
            this.label17.Text = "These will be used with the [Username] and [Password] variables in Camera Actions" +
    ".";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label18
            // 
            this.label18.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.label18.Location = new System.Drawing.Point(9, 317);
            this.label18.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(141, 17);
            this.label18.TabIndex = 16;
            this.label18.Text = "BlueIris Server Name:";
            // 
            // dbLayoutPanel5
            // 
            this.dbLayoutPanel5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dbLayoutPanel5.ColumnCount = 2;
            this.dbLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 26.28505F));
            this.dbLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 73.71495F));
            this.dbLayoutPanel5.Controls.Add(this.tb_BlueIrisServer, 0, 0);
            this.dbLayoutPanel5.Controls.Add(this.label19, 1, 0);
            this.dbLayoutPanel5.Location = new System.Drawing.Point(156, 304);
            this.dbLayoutPanel5.Name = "dbLayoutPanel5";
            this.dbLayoutPanel5.RowCount = 1;
            this.dbLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.dbLayoutPanel5.Size = new System.Drawing.Size(856, 43);
            this.dbLayoutPanel5.TabIndex = 22;
            // 
            // tb_BlueIrisServer
            // 
            this.tb_BlueIrisServer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_BlueIrisServer.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_BlueIrisServer.Location = new System.Drawing.Point(3, 9);
            this.tb_BlueIrisServer.Name = "tb_BlueIrisServer";
            this.tb_BlueIrisServer.Size = new System.Drawing.Size(219, 25);
            this.tb_BlueIrisServer.TabIndex = 9;
            // 
            // label19
            // 
            this.label19.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(228, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(625, 43);
            this.label19.TabIndex = 3;
            this.label19.Text = resources.GetString("label19.Text");
            this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dbLayoutPanel2
            // 
            this.dbLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dbLayoutPanel2.ColumnCount = 5;
            this.dbLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 32.75058F));
            this.dbLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.275059F));
            this.dbLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 31.93473F));
            this.dbLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.40326F));
            this.dbLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.63636F));
            this.dbLayoutPanel2.Controls.Add(this.tb_telegram_cooldown, 4, 0);
            this.dbLayoutPanel2.Controls.Add(this.label5, 3, 0);
            this.dbLayoutPanel2.Controls.Add(this.tb_telegram_chatid, 2, 0);
            this.dbLayoutPanel2.Controls.Add(this.lbl_telegram_chatid, 1, 0);
            this.dbLayoutPanel2.Controls.Add(this.tb_telegram_token, 0, 0);
            this.dbLayoutPanel2.Location = new System.Drawing.Point(155, 103);
            this.dbLayoutPanel2.Margin = new System.Windows.Forms.Padding(2);
            this.dbLayoutPanel2.Name = "dbLayoutPanel2";
            this.dbLayoutPanel2.RowCount = 1;
            this.dbLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.dbLayoutPanel2.Size = new System.Drawing.Size(858, 45);
            this.dbLayoutPanel2.TabIndex = 19;
            // 
            // tb_telegram_cooldown
            // 
            this.tb_telegram_cooldown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_telegram_cooldown.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_telegram_cooldown.Location = new System.Drawing.Point(742, 10);
            this.tb_telegram_cooldown.Name = "tb_telegram_cooldown";
            this.tb_telegram_cooldown.Size = new System.Drawing.Size(113, 25);
            this.tb_telegram_cooldown.TabIndex = 12;
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(632, 14);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(104, 17);
            this.label5.TabIndex = 11;
            this.label5.Text = "Cooldown Mins";
            // 
            // tb_telegram_chatid
            // 
            this.tb_telegram_chatid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_telegram_chatid.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_telegram_chatid.Location = new System.Drawing.Point(354, 10);
            this.tb_telegram_chatid.Name = "tb_telegram_chatid";
            this.tb_telegram_chatid.Size = new System.Drawing.Size(268, 25);
            this.tb_telegram_chatid.TabIndex = 10;
            // 
            // lbl_telegram_chatid
            // 
            this.lbl_telegram_chatid.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lbl_telegram_chatid.AutoSize = true;
            this.lbl_telegram_chatid.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_telegram_chatid.Location = new System.Drawing.Point(294, 14);
            this.lbl_telegram_chatid.Name = "lbl_telegram_chatid";
            this.lbl_telegram_chatid.Size = new System.Drawing.Size(54, 17);
            this.lbl_telegram_chatid.TabIndex = 7;
            this.lbl_telegram_chatid.Text = "Chat ID";
            // 
            // tb_telegram_token
            // 
            this.tb_telegram_token.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_telegram_token.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_telegram_token.Location = new System.Drawing.Point(3, 10);
            this.tb_telegram_token.Name = "tb_telegram_token";
            this.tb_telegram_token.Size = new System.Drawing.Size(274, 25);
            this.tb_telegram_token.TabIndex = 9;
            // 
            // dbLayoutPanel3
            // 
            this.dbLayoutPanel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dbLayoutPanel3.ColumnCount = 3;
            this.dbLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 84.57375F));
            this.dbLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15.42625F));
            this.dbLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 305F));
            this.dbLayoutPanel3.Controls.Add(this.cb_send_errors, 0, 0);
            this.dbLayoutPanel3.Controls.Add(this.btn_enabletelegram, 1, 0);
            this.dbLayoutPanel3.Controls.Add(this.btn_disabletelegram, 2, 0);
            this.dbLayoutPanel3.Location = new System.Drawing.Point(156, 154);
            this.dbLayoutPanel3.Name = "dbLayoutPanel3";
            this.dbLayoutPanel3.RowCount = 1;
            this.dbLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.dbLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 43F));
            this.dbLayoutPanel3.Size = new System.Drawing.Size(856, 43);
            this.dbLayoutPanel3.TabIndex = 20;
            // 
            // cb_send_errors
            // 
            this.cb_send_errors.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cb_send_errors.AutoSize = true;
            this.cb_send_errors.Location = new System.Drawing.Point(3, 13);
            this.cb_send_errors.Name = "cb_send_errors";
            this.cb_send_errors.Size = new System.Drawing.Size(150, 17);
            this.cb_send_errors.TabIndex = 12;
            this.cb_send_errors.Text = "Send Errors and Warnings";
            this.cb_send_errors.UseVisualStyleBackColor = true;
            // 
            // btn_enabletelegram
            // 
            this.btn_enabletelegram.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btn_enabletelegram.Location = new System.Drawing.Point(473, 6);
            this.btn_enabletelegram.Name = "btn_enabletelegram";
            this.btn_enabletelegram.Size = new System.Drawing.Size(70, 30);
            this.btn_enabletelegram.TabIndex = 13;
            this.btn_enabletelegram.Text = "Enable All";
            this.toolTip1.SetToolTip(this.btn_enabletelegram, "Enable Telegram on all cameras");
            this.btn_enabletelegram.UseVisualStyleBackColor = true;
            this.btn_enabletelegram.Click += new System.EventHandler(this.btn_enabletelegram_Click);
            // 
            // btn_disabletelegram
            // 
            this.btn_disabletelegram.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btn_disabletelegram.Location = new System.Drawing.Point(668, 6);
            this.btn_disabletelegram.Name = "btn_disabletelegram";
            this.btn_disabletelegram.Size = new System.Drawing.Size(70, 30);
            this.btn_disabletelegram.TabIndex = 13;
            this.btn_disabletelegram.Text = "Disable All";
            this.toolTip1.SetToolTip(this.btn_disabletelegram, "Disable Telegram on all Cameras");
            this.btn_disabletelegram.UseVisualStyleBackColor = true;
            this.btn_disabletelegram.Click += new System.EventHandler(this.btn_disabletelegram_Click);
            // 
            // MinimizeBehaviour
            // 
            this.MinimizeBehaviour.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.MinimizeBehaviour.AutoSize = true;
            this.MinimizeBehaviour.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimizeBehaviour.Location = new System.Drawing.Point(24, 367);
            this.MinimizeBehaviour.Name = "MinimizeBehaviour";
            this.MinimizeBehaviour.Size = new System.Drawing.Size(125, 17);
            this.MinimizeBehaviour.TabIndex = 24;
            this.MinimizeBehaviour.Text = "Window behaviour";
            // 
            // minimizeToTrayCheckbox
            // 
            this.minimizeToTrayCheckbox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.minimizeToTrayCheckbox.AutoSize = true;
            this.minimizeToTrayCheckbox.Location = new System.Drawing.Point(155, 367);
            this.minimizeToTrayCheckbox.Margin = new System.Windows.Forms.Padding(2);
            this.minimizeToTrayCheckbox.Name = "minimizeToTrayCheckbox";
            this.minimizeToTrayCheckbox.Size = new System.Drawing.Size(98, 17);
            this.minimizeToTrayCheckbox.TabIndex = 23;
            this.minimizeToTrayCheckbox.Text = "Minimize to tray";
            this.minimizeToTrayCheckbox.UseVisualStyleBackColor = true;
            // 
            // tabDeepStack
            // 
            this.tabDeepStack.Controls.Add(this.chk_HighPriority);
            this.tabDeepStack.Controls.Add(this.Chk_DSDebug);
            this.tabDeepStack.Controls.Add(this.Lbl_BlueStackRunning);
            this.tabDeepStack.Controls.Add(this.Btn_Save);
            this.tabDeepStack.Controls.Add(this.label11);
            this.tabDeepStack.Controls.Add(this.groupBox6);
            this.tabDeepStack.Controls.Add(this.groupBox1);
            this.tabDeepStack.Controls.Add(this.groupBox5);
            this.tabDeepStack.Controls.Add(this.groupBox2);
            this.tabDeepStack.Controls.Add(this.groupBox4);
            this.tabDeepStack.Controls.Add(this.groupBox3);
            this.tabDeepStack.Controls.Add(this.Chk_AutoStart);
            this.tabDeepStack.Controls.Add(this.Btn_Start);
            this.tabDeepStack.Controls.Add(this.Btn_Stop);
            this.tabDeepStack.Location = new System.Drawing.Point(4, 22);
            this.tabDeepStack.Margin = new System.Windows.Forms.Padding(2);
            this.tabDeepStack.Name = "tabDeepStack";
            this.tabDeepStack.Size = new System.Drawing.Size(1027, 455);
            this.tabDeepStack.TabIndex = 6;
            this.tabDeepStack.Text = "DeepStack";
            this.tabDeepStack.UseVisualStyleBackColor = true;
            // 
            // chk_HighPriority
            // 
            this.chk_HighPriority.AutoSize = true;
            this.chk_HighPriority.Location = new System.Drawing.Point(342, 301);
            this.chk_HighPriority.Margin = new System.Windows.Forms.Padding(2);
            this.chk_HighPriority.Name = "chk_HighPriority";
            this.chk_HighPriority.Size = new System.Drawing.Size(102, 17);
            this.chk_HighPriority.TabIndex = 15;
            this.chk_HighPriority.Text = "Run high priority";
            this.chk_HighPriority.UseVisualStyleBackColor = true;
            // 
            // Chk_DSDebug
            // 
            this.Chk_DSDebug.AutoSize = true;
            this.Chk_DSDebug.Location = new System.Drawing.Point(206, 301);
            this.Chk_DSDebug.Margin = new System.Windows.Forms.Padding(2);
            this.Chk_DSDebug.Name = "Chk_DSDebug";
            this.Chk_DSDebug.Size = new System.Drawing.Size(113, 17);
            this.Chk_DSDebug.TabIndex = 14;
            this.Chk_DSDebug.Text = "Debug Deepstack";
            this.toolTip1.SetToolTip(this.Chk_DSDebug, "Show all output from Deepstack\'s python.exe, redis.exe and server.exe  (Windows v" +
        "ersion, installed on same machine)");
            this.Chk_DSDebug.UseVisualStyleBackColor = true;
            // 
            // Lbl_BlueStackRunning
            // 
            this.Lbl_BlueStackRunning.AutoSize = true;
            this.Lbl_BlueStackRunning.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_BlueStackRunning.Location = new System.Drawing.Point(282, 348);
            this.Lbl_BlueStackRunning.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Lbl_BlueStackRunning.Name = "Lbl_BlueStackRunning";
            this.Lbl_BlueStackRunning.Size = new System.Drawing.Size(105, 13);
            this.Lbl_BlueStackRunning.TabIndex = 13;
            this.Lbl_BlueStackRunning.Text = "*NOT RUNNING*";
            // 
            // Btn_Save
            // 
            this.Btn_Save.Location = new System.Drawing.Point(191, 338);
            this.Btn_Save.Margin = new System.Windows.Forms.Padding(2);
            this.Btn_Save.Name = "Btn_Save";
            this.Btn_Save.Size = new System.Drawing.Size(70, 30);
            this.Btn_Save.TabIndex = 12;
            this.Btn_Save.Text = "Save";
            this.Btn_Save.UseVisualStyleBackColor = true;
            this.Btn_Save.Click += new System.EventHandler(this.Btn_Save_Click);
            // 
            // label11
            // 
            this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label11.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label11.Location = new System.Drawing.Point(6, 10);
            this.label11.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(727, 34);
            this.label11.TabIndex = 0;
            this.label11.Text = "When the WINDOWS version of DeepStack will be running on the same machine as AI T" +
    "ool, this tab can replace the DeepStack UI. (Deepstack.exe) - No control over Do" +
    "cker version yet.";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.Txt_APIKey);
            this.groupBox6.Location = new System.Drawing.Point(11, 146);
            this.groupBox6.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox6.Size = new System.Drawing.Size(483, 45);
            this.groupBox6.TabIndex = 11;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "API Key (Optional)";
            // 
            // Txt_APIKey
            // 
            this.Txt_APIKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Txt_APIKey.Location = new System.Drawing.Point(7, 17);
            this.Txt_APIKey.Margin = new System.Windows.Forms.Padding(2);
            this.Txt_APIKey.Name = "Txt_APIKey";
            this.Txt_APIKey.Size = new System.Drawing.Size(472, 20);
            this.Txt_APIKey.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.Chk_DetectionAPI);
            this.groupBox1.Controls.Add(this.Chk_FaceAPI);
            this.groupBox1.Controls.Add(this.Chk_SceneAPI);
            this.groupBox1.Location = new System.Drawing.Point(8, 203);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(162, 88);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "API";
            // 
            // Chk_DetectionAPI
            // 
            this.Chk_DetectionAPI.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Chk_DetectionAPI.Location = new System.Drawing.Point(11, 63);
            this.Chk_DetectionAPI.Margin = new System.Windows.Forms.Padding(2);
            this.Chk_DetectionAPI.Name = "Chk_DetectionAPI";
            this.Chk_DetectionAPI.Size = new System.Drawing.Size(147, 19);
            this.Chk_DetectionAPI.TabIndex = 2;
            this.Chk_DetectionAPI.Text = "Detection API";
            this.Chk_DetectionAPI.UseVisualStyleBackColor = true;
            // 
            // Chk_FaceAPI
            // 
            this.Chk_FaceAPI.Location = new System.Drawing.Point(11, 40);
            this.Chk_FaceAPI.Margin = new System.Windows.Forms.Padding(2);
            this.Chk_FaceAPI.Name = "Chk_FaceAPI";
            this.Chk_FaceAPI.Size = new System.Drawing.Size(147, 19);
            this.Chk_FaceAPI.TabIndex = 1;
            this.Chk_FaceAPI.Text = "Face API";
            this.Chk_FaceAPI.UseVisualStyleBackColor = true;
            // 
            // Chk_SceneAPI
            // 
            this.Chk_SceneAPI.Location = new System.Drawing.Point(11, 17);
            this.Chk_SceneAPI.Margin = new System.Windows.Forms.Padding(2);
            this.Chk_SceneAPI.Name = "Chk_SceneAPI";
            this.Chk_SceneAPI.Size = new System.Drawing.Size(147, 19);
            this.Chk_SceneAPI.TabIndex = 0;
            this.Chk_SceneAPI.Text = "Scene API";
            this.Chk_SceneAPI.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.Txt_AdminKey);
            this.groupBox5.Location = new System.Drawing.Point(11, 96);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox5.Size = new System.Drawing.Size(483, 45);
            this.groupBox5.TabIndex = 10;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Admin Key (Optional)";
            // 
            // Txt_AdminKey
            // 
            this.Txt_AdminKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Txt_AdminKey.Location = new System.Drawing.Point(7, 16);
            this.Txt_AdminKey.Margin = new System.Windows.Forms.Padding(2);
            this.Txt_AdminKey.Name = "Txt_AdminKey";
            this.Txt_AdminKey.Size = new System.Drawing.Size(472, 20);
            this.Txt_AdminKey.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.RB_High);
            this.groupBox2.Controls.Add(this.RB_Medium);
            this.groupBox2.Controls.Add(this.RB_Low);
            this.groupBox2.Location = new System.Drawing.Point(175, 203);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(154, 88);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Mode";
            // 
            // RB_High
            // 
            this.RB_High.Location = new System.Drawing.Point(11, 63);
            this.RB_High.Margin = new System.Windows.Forms.Padding(2);
            this.RB_High.Name = "RB_High";
            this.RB_High.Size = new System.Drawing.Size(139, 19);
            this.RB_High.TabIndex = 3;
            this.RB_High.TabStop = true;
            this.RB_High.Text = "High";
            this.RB_High.UseVisualStyleBackColor = true;
            // 
            // RB_Medium
            // 
            this.RB_Medium.Location = new System.Drawing.Point(11, 40);
            this.RB_Medium.Margin = new System.Windows.Forms.Padding(2);
            this.RB_Medium.Name = "RB_Medium";
            this.RB_Medium.Size = new System.Drawing.Size(139, 19);
            this.RB_Medium.TabIndex = 2;
            this.RB_Medium.TabStop = true;
            this.RB_Medium.Text = "Medium";
            this.RB_Medium.UseVisualStyleBackColor = true;
            // 
            // RB_Low
            // 
            this.RB_Low.Location = new System.Drawing.Point(11, 17);
            this.RB_Low.Margin = new System.Windows.Forms.Padding(2);
            this.RB_Low.Name = "RB_Low";
            this.RB_Low.Size = new System.Drawing.Size(139, 19);
            this.RB_Low.TabIndex = 1;
            this.RB_Low.TabStop = true;
            this.RB_Low.Text = "Low";
            this.RB_Low.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.Txt_DeepStackInstallFolder);
            this.groupBox4.Location = new System.Drawing.Point(11, 47);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox4.Size = new System.Drawing.Size(483, 45);
            this.groupBox4.TabIndex = 9;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "DeepStack Install Folder";
            // 
            // Txt_DeepStackInstallFolder
            // 
            this.Txt_DeepStackInstallFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Txt_DeepStackInstallFolder.Location = new System.Drawing.Point(7, 17);
            this.Txt_DeepStackInstallFolder.Margin = new System.Windows.Forms.Padding(2);
            this.Txt_DeepStackInstallFolder.Name = "Txt_DeepStackInstallFolder";
            this.Txt_DeepStackInstallFolder.ReadOnly = true;
            this.Txt_DeepStackInstallFolder.Size = new System.Drawing.Size(472, 20);
            this.Txt_DeepStackInstallFolder.TabIndex = 2;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.Txt_Port);
            this.groupBox3.Location = new System.Drawing.Point(342, 203);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox3.Size = new System.Drawing.Size(151, 88);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Port";
            // 
            // Txt_Port
            // 
            this.Txt_Port.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Txt_Port.Location = new System.Drawing.Point(10, 19);
            this.Txt_Port.Margin = new System.Windows.Forms.Padding(2);
            this.Txt_Port.Name = "Txt_Port";
            this.Txt_Port.Size = new System.Drawing.Size(127, 20);
            this.Txt_Port.TabIndex = 0;
            // 
            // Chk_AutoStart
            // 
            this.Chk_AutoStart.AutoSize = true;
            this.Chk_AutoStart.Location = new System.Drawing.Point(13, 301);
            this.Chk_AutoStart.Margin = new System.Windows.Forms.Padding(2);
            this.Chk_AutoStart.Name = "Chk_AutoStart";
            this.Chk_AutoStart.Size = new System.Drawing.Size(170, 17);
            this.Chk_AutoStart.TabIndex = 8;
            this.Chk_AutoStart.Text = "Automatically Start DeepStack";
            this.Chk_AutoStart.UseVisualStyleBackColor = true;
            // 
            // Btn_Start
            // 
            this.Btn_Start.Location = new System.Drawing.Point(8, 338);
            this.Btn_Start.Margin = new System.Windows.Forms.Padding(2);
            this.Btn_Start.Name = "Btn_Start";
            this.Btn_Start.Size = new System.Drawing.Size(70, 30);
            this.Btn_Start.TabIndex = 6;
            this.Btn_Start.Text = "Start";
            this.Btn_Start.UseVisualStyleBackColor = true;
            this.Btn_Start.Click += new System.EventHandler(this.Btn_Start_Click);
            // 
            // Btn_Stop
            // 
            this.Btn_Stop.Location = new System.Drawing.Point(99, 338);
            this.Btn_Stop.Margin = new System.Windows.Forms.Padding(2);
            this.Btn_Stop.Name = "Btn_Stop";
            this.Btn_Stop.Size = new System.Drawing.Size(70, 30);
            this.Btn_Stop.TabIndex = 7;
            this.Btn_Stop.Text = "Stop";
            this.Btn_Stop.UseVisualStyleBackColor = true;
            this.Btn_Stop.Click += new System.EventHandler(this.Btn_Stop_Click);
            // 
            // tabLog
            // 
            this.tabLog.Controls.Add(this.toolStrip2);
            this.tabLog.Controls.Add(this.groupBox7);
            this.tabLog.Location = new System.Drawing.Point(4, 22);
            this.tabLog.Margin = new System.Windows.Forms.Padding(2);
            this.tabLog.Name = "tabLog";
            this.tabLog.Size = new System.Drawing.Size(1027, 455);
            this.tabLog.TabIndex = 7;
            this.tabLog.Text = "Log";
            this.tabLog.UseVisualStyleBackColor = true;
            this.tabLog.Click += new System.EventHandler(this.tabLog_Click);
            // 
            // toolStrip2
            // 
            this.toolStrip2.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.ToolStripComboBoxSearch,
            this.toolStripDropDownButton1,
            this.toolStripSeparator3,
            this.toolStripSeparator,
            this.toolStripDropDownButtonSettings,
            this.toolStripSeparator4,
            this.toolStripSeparator2,
            this.openToolStripButton,
            this.toolStripButtonLoad,
            this.toolStripSeparator7,
            this.toolStripButtonReload,
            this.toolStripSeparator5,
            this.toolStripButtonPauseLog,
            this.toolStripSeparator6,
            this.toolStripSeparator8,
            this.chk_filterErrors,
            this.chk_filterErrorsAll});
            this.toolStrip2.Location = new System.Drawing.Point(0, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(1027, 31);
            this.toolStrip2.TabIndex = 6;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(45, 28);
            this.toolStripLabel1.Text = "Search:";
            // 
            // ToolStripComboBoxSearch
            // 
            this.ToolStripComboBoxSearch.BackColor = System.Drawing.SystemColors.Info;
            this.ToolStripComboBoxSearch.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.ToolStripComboBoxSearch.Items.AddRange(new object[] {
            "person",
            "error|warn|fatal|problem|exception",
            "this | orthat",
            "imagefilename.jpg | key=1234"});
            this.ToolStripComboBoxSearch.Name = "ToolStripComboBoxSearch";
            this.ToolStripComboBoxSearch.Size = new System.Drawing.Size(200, 31);
            this.ToolStripComboBoxSearch.ToolTipText = "The search can be normal text OR a valid \'RegEx\' statement.\r\n";
            this.ToolStripComboBoxSearch.Leave += new System.EventHandler(this.ToolStripComboBoxSearch_Leave);
            this.ToolStripComboBoxSearch.TextChanged += new System.EventHandler(this.ToolStripComboBoxSearch_TextChanged);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnu_Filter,
            this.mnu_Highlight});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(37, 28);
            this.toolStripDropDownButton1.Text = "Filter or highlight search box";
            // 
            // mnu_Filter
            // 
            this.mnu_Filter.CheckOnClick = true;
            this.mnu_Filter.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.mnu_Filter.Name = "mnu_Filter";
            this.mnu_Filter.Size = new System.Drawing.Size(124, 22);
            this.mnu_Filter.Text = "Filter";
            this.mnu_Filter.CheckStateChanged += new System.EventHandler(this.mnu_Filter_CheckStateChanged);
            this.mnu_Filter.Click += new System.EventHandler(this.mnu_Filter_Click);
            // 
            // mnu_Highlight
            // 
            this.mnu_Highlight.CheckOnClick = true;
            this.mnu_Highlight.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.mnu_Highlight.Name = "mnu_Highlight";
            this.mnu_Highlight.Size = new System.Drawing.Size(124, 22);
            this.mnu_Highlight.Text = "Highlight";
            this.mnu_Highlight.CheckStateChanged += new System.EventHandler(this.mnu_highlight_CheckStateChanged);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 31);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(6, 31);
            // 
            // toolStripDropDownButtonSettings
            // 
            this.toolStripDropDownButtonSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Chk_AutoScroll,
            this.clearRecentErrorsToolStripMenuItem,
            this.toolStripMenuItemLogLevel});
            this.toolStripDropDownButtonSettings.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButtonSettings.Image")));
            this.toolStripDropDownButtonSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButtonSettings.Name = "toolStripDropDownButtonSettings";
            this.toolStripDropDownButtonSettings.Size = new System.Drawing.Size(109, 28);
            this.toolStripDropDownButtonSettings.Text = "Log Settings";
            // 
            // Chk_AutoScroll
            // 
            this.Chk_AutoScroll.CheckOnClick = true;
            this.Chk_AutoScroll.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.Chk_AutoScroll.Name = "Chk_AutoScroll";
            this.Chk_AutoScroll.Size = new System.Drawing.Size(204, 22);
            this.Chk_AutoScroll.Text = "Auto Scroll";
            this.Chk_AutoScroll.Click += new System.EventHandler(this.Chk_AutoScroll_Click_1);
            // 
            // clearRecentErrorsToolStripMenuItem
            // 
            this.clearRecentErrorsToolStripMenuItem.Name = "clearRecentErrorsToolStripMenuItem";
            this.clearRecentErrorsToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.clearRecentErrorsToolStripMenuItem.Text = "Clear Recent Error Count";
            this.clearRecentErrorsToolStripMenuItem.Click += new System.EventHandler(this.clearRecentErrorsToolStripMenuItem_Click);
            // 
            // toolStripMenuItemLogLevel
            // 
            this.toolStripMenuItemLogLevel.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnu_log_filter_off,
            this.mnu_log_filter_fatal,
            this.mnu_log_filter_error,
            this.mnu_log_filter_warn,
            this.mnu_log_filter_info,
            this.mnu_log_filter_debug,
            this.mnu_log_filter_trace});
            this.toolStripMenuItemLogLevel.Name = "toolStripMenuItemLogLevel";
            this.toolStripMenuItemLogLevel.Size = new System.Drawing.Size(204, 22);
            this.toolStripMenuItemLogLevel.Text = "Logging Level";
            // 
            // mnu_log_filter_off
            // 
            this.mnu_log_filter_off.CheckOnClick = true;
            this.mnu_log_filter_off.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.mnu_log_filter_off.Name = "mnu_log_filter_off";
            this.mnu_log_filter_off.Size = new System.Drawing.Size(109, 22);
            this.mnu_log_filter_off.Text = "Off";
            this.mnu_log_filter_off.CheckStateChanged += new System.EventHandler(this.mnu_log_filter_off_CheckStateChanged);
            this.mnu_log_filter_off.Click += new System.EventHandler(this.mnu_log_filter_off_Click);
            // 
            // mnu_log_filter_fatal
            // 
            this.mnu_log_filter_fatal.CheckOnClick = true;
            this.mnu_log_filter_fatal.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.mnu_log_filter_fatal.Name = "mnu_log_filter_fatal";
            this.mnu_log_filter_fatal.Size = new System.Drawing.Size(109, 22);
            this.mnu_log_filter_fatal.Text = "Fatal";
            this.mnu_log_filter_fatal.CheckStateChanged += new System.EventHandler(this.mnu_log_filter_fatal_CheckStateChanged);
            // 
            // mnu_log_filter_error
            // 
            this.mnu_log_filter_error.CheckOnClick = true;
            this.mnu_log_filter_error.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.mnu_log_filter_error.Name = "mnu_log_filter_error";
            this.mnu_log_filter_error.Size = new System.Drawing.Size(109, 22);
            this.mnu_log_filter_error.Text = "Error";
            this.mnu_log_filter_error.CheckStateChanged += new System.EventHandler(this.mnu_log_filter_error_CheckStateChanged);
            // 
            // mnu_log_filter_warn
            // 
            this.mnu_log_filter_warn.CheckOnClick = true;
            this.mnu_log_filter_warn.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.mnu_log_filter_warn.Name = "mnu_log_filter_warn";
            this.mnu_log_filter_warn.Size = new System.Drawing.Size(109, 22);
            this.mnu_log_filter_warn.Text = "Warn";
            this.mnu_log_filter_warn.CheckStateChanged += new System.EventHandler(this.mnu_log_filter_warn_CheckStateChanged);
            // 
            // mnu_log_filter_info
            // 
            this.mnu_log_filter_info.CheckOnClick = true;
            this.mnu_log_filter_info.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.mnu_log_filter_info.Name = "mnu_log_filter_info";
            this.mnu_log_filter_info.Size = new System.Drawing.Size(109, 22);
            this.mnu_log_filter_info.Text = "Info";
            this.mnu_log_filter_info.CheckStateChanged += new System.EventHandler(this.mnu_log_filter_info_CheckStateChanged);
            // 
            // mnu_log_filter_debug
            // 
            this.mnu_log_filter_debug.CheckOnClick = true;
            this.mnu_log_filter_debug.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.mnu_log_filter_debug.Name = "mnu_log_filter_debug";
            this.mnu_log_filter_debug.Size = new System.Drawing.Size(109, 22);
            this.mnu_log_filter_debug.Text = "Debug";
            this.mnu_log_filter_debug.CheckStateChanged += new System.EventHandler(this.mnu_log_filter_debug_CheckStateChanged);
            // 
            // mnu_log_filter_trace
            // 
            this.mnu_log_filter_trace.CheckOnClick = true;
            this.mnu_log_filter_trace.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.mnu_log_filter_trace.Name = "mnu_log_filter_trace";
            this.mnu_log_filter_trace.Size = new System.Drawing.Size(109, 22);
            this.mnu_log_filter_trace.Text = "Trace";
            this.mnu_log_filter_trace.CheckStateChanged += new System.EventHandler(this.mnu_log_filter_trace_CheckStateChanged);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 31);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 31);
            // 
            // openToolStripButton
            // 
            this.openToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripButton.Image")));
            this.openToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openToolStripButton.Name = "openToolStripButton";
            this.openToolStripButton.Size = new System.Drawing.Size(64, 28);
            this.openToolStripButton.Text = "Open";
            this.openToolStripButton.ToolTipText = "Open Log File in external editor";
            this.openToolStripButton.Click += new System.EventHandler(this.openToolStripButton_Click);
            // 
            // toolStripButtonLoad
            // 
            this.toolStripButtonLoad.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonLoad.Image")));
            this.toolStripButtonLoad.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonLoad.Name = "toolStripButtonLoad";
            this.toolStripButtonLoad.Size = new System.Drawing.Size(61, 28);
            this.toolStripButtonLoad.Text = "Load";
            this.toolStripButtonLoad.ToolTipText = "Load a specific log file into the list";
            this.toolStripButtonLoad.Click += new System.EventHandler(this.toolStripButtonLoad_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(6, 31);
            // 
            // toolStripButtonReload
            // 
            this.toolStripButtonReload.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonReload.Image")));
            this.toolStripButtonReload.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonReload.Name = "toolStripButtonReload";
            this.toolStripButtonReload.Size = new System.Drawing.Size(71, 28);
            this.toolStripButtonReload.Text = "Reload";
            this.toolStripButtonReload.ToolTipText = "Reloads the entire current log file from file without limiting the max number of " +
    "lines.  \r\nUse this after loading other files or filtering to reset view";
            this.toolStripButtonReload.Click += new System.EventHandler(this.toolStripButtonReload_ClickAsync);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 31);
            // 
            // toolStripButtonPauseLog
            // 
            this.toolStripButtonPauseLog.CheckOnClick = true;
            this.toolStripButtonPauseLog.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonPauseLog.Image")));
            this.toolStripButtonPauseLog.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPauseLog.Name = "toolStripButtonPauseLog";
            this.toolStripButtonPauseLog.Size = new System.Drawing.Size(66, 28);
            this.toolStripButtonPauseLog.Text = "Pause";
            this.toolStripButtonPauseLog.ToolTipText = "Pause log tab auto refresh";
            this.toolStripButtonPauseLog.Click += new System.EventHandler(this.toolStripButtonPauseLog_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 31);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(6, 31);
            // 
            // chk_filterErrors
            // 
            this.chk_filterErrors.CheckOnClick = true;
            this.chk_filterErrors.Image = ((System.Drawing.Image)(resources.GetObject("chk_filterErrors.Image")));
            this.chk_filterErrors.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.chk_filterErrors.Name = "chk_filterErrors";
            this.chk_filterErrors.Size = new System.Drawing.Size(65, 28);
            this.chk_filterErrors.Text = "Errors";
            this.chk_filterErrors.ToolTipText = "Show errors from current loaded log";
            this.chk_filterErrors.Click += new System.EventHandler(this.chk_filterErrors_Click_1);
            // 
            // chk_filterErrorsAll
            // 
            this.chk_filterErrorsAll.CheckOnClick = true;
            this.chk_filterErrorsAll.Image = ((System.Drawing.Image)(resources.GetObject("chk_filterErrorsAll.Image")));
            this.chk_filterErrorsAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.chk_filterErrorsAll.Name = "chk_filterErrorsAll";
            this.chk_filterErrorsAll.Size = new System.Drawing.Size(90, 28);
            this.chk_filterErrorsAll.Text = "Errors (All)";
            this.chk_filterErrorsAll.ToolTipText = "Show errors from ALL logs";
            this.chk_filterErrorsAll.Click += new System.EventHandler(this.chk_filterErrorsAll_Click);
            // 
            // groupBox7
            // 
            this.groupBox7.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox7.Controls.Add(this.folv_log);
            this.groupBox7.Location = new System.Drawing.Point(5, 36);
            this.groupBox7.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox7.Size = new System.Drawing.Size(1015, 412);
            this.groupBox7.TabIndex = 5;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Log";
            // 
            // folv_log
            // 
            this.folv_log.BackColor = System.Drawing.Color.Navy;
            this.folv_log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.folv_log.ForeColor = System.Drawing.Color.White;
            this.folv_log.HideSelection = false;
            this.folv_log.Location = new System.Drawing.Point(2, 15);
            this.folv_log.Name = "folv_log";
            this.folv_log.ShowGroups = false;
            this.folv_log.Size = new System.Drawing.Size(1011, 395);
            this.folv_log.TabIndex = 0;
            this.folv_log.UseCompatibleStateImageBehavior = false;
            this.folv_log.View = System.Windows.Forms.View.Details;
            this.folv_log.VirtualMode = true;
            this.folv_log.FormatCell += new System.EventHandler<BrightIdeasSoftware.FormatCellEventArgs>(this.folv_log_FormatCell);
            this.folv_log.FormatRow += new System.EventHandler<BrightIdeasSoftware.FormatRowEventArgs>(this.folv_log_FormatRow);
            // 
            // HistoryImageList
            // 
            this.HistoryImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("HistoryImageList.ImageStream")));
            this.HistoryImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.HistoryImageList.Images.SetKeyName(0, "error16");
            this.HistoryImageList.Images.SetKeyName(1, "person16");
            this.HistoryImageList.Images.SetKeyName(2, "nothing16");
            this.HistoryImageList.Images.SetKeyName(3, "detection16");
            this.HistoryImageList.Images.SetKeyName(4, "success16");
            this.HistoryImageList.Images.SetKeyName(5, "bear16");
            this.HistoryImageList.Images.SetKeyName(6, "cat16");
            this.HistoryImageList.Images.SetKeyName(7, "dog16");
            this.HistoryImageList.Images.SetKeyName(8, "horse16");
            this.HistoryImageList.Images.SetKeyName(9, "bird16");
            this.HistoryImageList.Images.SetKeyName(10, "alien16");
            this.HistoryImageList.Images.SetKeyName(11, "cow16");
            this.HistoryImageList.Images.SetKeyName(12, "car16");
            this.HistoryImageList.Images.SetKeyName(13, "truck16");
            this.HistoryImageList.Images.SetKeyName(14, "motorcycle16");
            this.HistoryImageList.Images.SetKeyName(15, "bicycle32.png");
            this.HistoryImageList.Images.SetKeyName(16, "airplane.png");
            this.HistoryImageList.Images.SetKeyName(17, "bear.png");
            this.HistoryImageList.Images.SetKeyName(18, "bicycle.png");
            this.HistoryImageList.Images.SetKeyName(19, "bird.png");
            this.HistoryImageList.Images.SetKeyName(20, "boat.png");
            this.HistoryImageList.Images.SetKeyName(21, "bus.png");
            this.HistoryImageList.Images.SetKeyName(22, "car.png");
            this.HistoryImageList.Images.SetKeyName(23, "cat.png");
            this.HistoryImageList.Images.SetKeyName(24, "cow.png");
            this.HistoryImageList.Images.SetKeyName(25, "dog.png");
            this.HistoryImageList.Images.SetKeyName(26, "horse.png");
            this.HistoryImageList.Images.SetKeyName(27, "motorcycle.png");
            this.HistoryImageList.Images.SetKeyName(28, "person.png");
            this.HistoryImageList.Images.SetKeyName(29, "sheep.png");
            this.HistoryImageList.Images.SetKeyName(30, "truck.png");
            this.HistoryImageList.Images.SetKeyName(31, "error32.png");
            this.HistoryImageList.Images.SetKeyName(32, "person32.png");
            this.HistoryImageList.Images.SetKeyName(33, "nothing32.png");
            this.HistoryImageList.Images.SetKeyName(34, "success32.png");
            this.HistoryImageList.Images.SetKeyName(35, "detection32.png");
            this.HistoryImageList.Images.SetKeyName(36, "bear32.png");
            this.HistoryImageList.Images.SetKeyName(37, "cat32.png");
            this.HistoryImageList.Images.SetKeyName(38, "dog32.png");
            this.HistoryImageList.Images.SetKeyName(39, "horse32.png");
            this.HistoryImageList.Images.SetKeyName(40, "bird32.png");
            this.HistoryImageList.Images.SetKeyName(41, "alien32.png");
            this.HistoryImageList.Images.SetKeyName(42, "cow32.png");
            this.HistoryImageList.Images.SetKeyName(43, "car32.png");
            this.HistoryImageList.Images.SetKeyName(44, "truck32.png");
            this.HistoryImageList.Images.SetKeyName(45, "motorcycle32.png");
            // 
            // HistoryUpdateListTimer
            // 
            this.HistoryUpdateListTimer.Interval = 3000;
            this.HistoryUpdateListTimer.Tick += new System.EventHandler(this.HistoryUpdateListTimer_Tick);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(28, 28);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar1,
            this.toolStripStatusLabelHistoryItems,
            this.toolStripStatusErrors,
            this.toolStripStatusLabel1,
            this.toolStripStatusLabelInfo});
            this.statusStrip1.Location = new System.Drawing.Point(0, 497);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 8, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1035, 24);
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 27);
            this.toolStripProgressBar1.Visible = false;
            // 
            // toolStripStatusLabelHistoryItems
            // 
            this.toolStripStatusLabelHistoryItems.ForeColor = System.Drawing.Color.DodgerBlue;
            this.toolStripStatusLabelHistoryItems.Name = "toolStripStatusLabelHistoryItems";
            this.toolStripStatusLabelHistoryItems.Size = new System.Drawing.Size(86, 19);
            this.toolStripStatusLabelHistoryItems.Text = "0 History Items";
            // 
            // toolStripStatusErrors
            // 
            this.toolStripStatusErrors.Name = "toolStripStatusErrors";
            this.toolStripStatusErrors.Size = new System.Drawing.Size(10, 19);
            this.toolStripStatusErrors.Text = ".";
            this.toolStripStatusErrors.Click += new System.EventHandler(this.toolStripStatusErrors_Click);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(10, 19);
            this.toolStripStatusLabel1.Text = ".";
            // 
            // toolStripStatusLabelInfo
            // 
            this.toolStripStatusLabelInfo.Font = new System.Drawing.Font("Segoe UI", 9.857143F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripStatusLabelInfo.ForeColor = System.Drawing.Color.DarkOrange;
            this.toolStripStatusLabelInfo.Name = "toolStripStatusLabelInfo";
            this.toolStripStatusLabelInfo.Size = new System.Drawing.Size(34, 19);
            this.toolStripStatusLabelInfo.Text = "Idle";
            // 
            // LogUpdateListTimer
            // 
            this.LogUpdateListTimer.Interval = 2000;
            this.LogUpdateListTimer.Tick += new System.EventHandler(this.LogUpdateListTimer_Tick);
            // 
            // Shell
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1035, 521);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.statusStrip1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.142858F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(755, 440);
            this.Name = "Shell";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Tag = "SAVE";
            this.Text = "AI Tool";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Shell_FormClosing);
            this.Load += new System.EventHandler(this.Shell_Load);
            this.DpiChanged += new System.Windows.Forms.DpiChangedEventHandler(this.Shell_DpiChanged);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.tabControl1.ResumeLayout(false);
            this.tabOverview.ResumeLayout(false);
            this.tableLayoutPanel14.ResumeLayout(false);
            this.tableLayoutPanel15.ResumeLayout(false);
            this.tableLayoutPanel15.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.tabStats.ResumeLayout(false);
            this.tableLayoutPanel16.ResumeLayout(false);
            this.tableLayoutPanel23.ResumeLayout(false);
            this.tableLayoutPanel23.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart_confidence)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.timeline)).EndInit();
            this.tableLayoutPanel17.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.tabHistory.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.folv_history)).EndInit();
            this.contextMenuStripHistory.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tabCameras.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel6.PerformLayout();
            this.tableLayoutPanel11.ResumeLayout(false);
            this.tableLayoutPanel7.ResumeLayout(false);
            this.tableLayoutPanel7.PerformLayout();
            this.tableLayoutPanel12.ResumeLayout(false);
            this.tableLayoutPanel12.PerformLayout();
            this.tableLayoutPanel13.ResumeLayout(false);
            this.tableLayoutPanel13.PerformLayout();
            this.tableLayoutPanel26.ResumeLayout(false);
            this.tableLayoutPanel26.PerformLayout();
            this.tableLayoutPanel27.ResumeLayout(false);
            this.tableLayoutPanel27.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.tableLayoutPanel8.ResumeLayout(false);
            this.tableLayoutPanel8.PerformLayout();
            this.tabSettings.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.tableLayoutPanel18.ResumeLayout(false);
            this.tableLayoutPanel18.PerformLayout();
            this.dbLayoutPanel1.ResumeLayout(false);
            this.dbLayoutPanel1.PerformLayout();
            this.dbLayoutPanel4.ResumeLayout(false);
            this.dbLayoutPanel4.PerformLayout();
            this.dbLayoutPanel5.ResumeLayout(false);
            this.dbLayoutPanel5.PerformLayout();
            this.dbLayoutPanel2.ResumeLayout(false);
            this.dbLayoutPanel2.PerformLayout();
            this.dbLayoutPanel3.ResumeLayout(false);
            this.dbLayoutPanel3.PerformLayout();
            this.tabDeepStack.ResumeLayout(false);
            this.tabDeepStack.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tabLog.ResumeLayout(false);
            this.tabLog.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.folv_log)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private DBLayoutPanel tableLayoutPanel2;
        private DBLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Button btnCameraAdd;
        private DBLayoutPanel tableLayoutPanel7;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblPrefix;
        private System.Windows.Forms.Label lblRelevantObjects;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.ListView list2;
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
        private DBLayoutPanel tableLayoutPanel8;
        private System.Windows.Forms.CheckBox cb_person;
        private System.Windows.Forms.CheckBox cb_bicycle;
        private System.Windows.Forms.CheckBox cb_motorcycle;
        private System.Windows.Forms.CheckBox cb_bear;
        private System.Windows.Forms.CheckBox cb_cow;
        private System.Windows.Forms.CheckBox cb_sheep;
        private System.Windows.Forms.CheckBox cb_horse;
        private System.Windows.Forms.CheckBox cb_bird;
        private System.Windows.Forms.CheckBox cb_dog;
        private System.Windows.Forms.CheckBox cb_cat;
        private System.Windows.Forms.CheckBox cb_airplane;
        private System.Windows.Forms.CheckBox cb_boat;
        private System.Windows.Forms.CheckBox cb_bus;
        private System.Windows.Forms.CheckBox cb_truck;
        private System.Windows.Forms.CheckBox cb_car;
        private System.Windows.Forms.Label lbl_threshold;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.CheckBox cb_send_errors;
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
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.TextBox Txt_APIKey;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox Txt_AdminKey;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label Lbl_BlueStackRunning;
        private System.Windows.Forms.Button Btn_Save;
        private System.Windows.Forms.TabPage tabLog;
        private System.Windows.Forms.ComboBox cmbInput;
        private System.Windows.Forms.Label label13;
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
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label lbl_threshold_lower;
        private System.Windows.Forms.TextBox tb_threshold_upper;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lbl_threshold_upper;
        private System.Windows.Forms.TextBox tb_threshold_lower;
        private System.Windows.Forms.Label label10;
        private DBLayoutPanel dbLayoutPanel1;
        private System.Windows.Forms.TextBox tbDeepstackUrl;
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
        private System.Windows.Forms.Button btn_enabletelegram;
        private System.Windows.Forms.Button btn_disabletelegram;
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
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox tbAdditionalRelevantObjects;
        private System.Windows.Forms.ToolStripMenuItem restrictThresholdAtSourceToolStripMenuItem;
        private System.Windows.Forms.CheckBox minimizeToTrayCheckbox;
        private System.Windows.Forms.Label MinimizeBehaviour;
    }
}

