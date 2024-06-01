
namespace AITool
{
    partial class Frm_AIServerDeepstackEdit
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_AIServerDeepstackEdit));
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            lbl_type = new System.Windows.Forms.Label();
            tb_URL = new System.Windows.Forms.TextBox();
            bt_Save = new System.Windows.Forms.Button();
            label4 = new System.Windows.Forms.Label();
            tb_ActiveTimeRange = new System.Windows.Forms.TextBox();
            label5 = new System.Windows.Forms.Label();
            tb_ApplyToCams = new System.Windows.Forms.TextBox();
            chk_Enabled = new System.Windows.Forms.CheckBox();
            groupBox1 = new System.Windows.Forms.GroupBox();
            gb_AIServerQueue = new System.Windows.Forms.GroupBox();
            lbl_ImgQueueStats = new System.Windows.Forms.Label();
            lbl_QueueTimeStats = new System.Windows.Forms.Label();
            lbl_QueueLengthStats = new System.Windows.Forms.Label();
            label19 = new System.Windows.Forms.Label();
            label18 = new System.Windows.Forms.Label();
            cb_AllowAIServerBasedQueue = new System.Windows.Forms.CheckBox();
            tb_SkipIfImgQueueLengthLarger = new System.Windows.Forms.TextBox();
            tb_SkipIfAIQueueTimeOverSecs = new System.Windows.Forms.TextBox();
            tb_MaxQueueLength = new System.Windows.Forms.TextBox();
            label17 = new System.Windows.Forms.Label();
            label16 = new System.Windows.Forms.Label();
            cb_IgnoreOffline = new System.Windows.Forms.CheckBox();
            tb_Name = new System.Windows.Forms.TextBox();
            label14 = new System.Windows.Forms.Label();
            cb_OnlyLinked = new System.Windows.Forms.CheckBox();
            cb_TimeoutError = new System.Windows.Forms.CheckBox();
            label12 = new System.Windows.Forms.Label();
            label10 = new System.Windows.Forms.Label();
            groupBoxLinked = new System.Windows.Forms.GroupBox();
            cb_LinkedServers = new System.Windows.Forms.CheckBox();
            label13 = new System.Windows.Forms.Label();
            checkedComboBoxLinked = new CheckComboBoxTest.CheckedComboBox();
            groupBoxRefine = new System.Windows.Forms.GroupBox();
            cb_RefinementServer = new System.Windows.Forms.CheckBox();
            label11 = new System.Windows.Forms.Label();
            tb_RefinementObjects = new System.Windows.Forms.TextBox();
            tb_Upper = new System.Windows.Forms.TextBox();
            tb_LinkedRefineTimeout = new System.Windows.Forms.TextBox();
            tb_Lower = new System.Windows.Forms.TextBox();
            label3 = new System.Windows.Forms.Label();
            tb_timeout = new System.Windows.Forms.TextBox();
            tb_ImagesPerMonth = new System.Windows.Forms.TextBox();
            label15 = new System.Windows.Forms.Label();
            label9 = new System.Windows.Forms.Label();
            label8 = new System.Windows.Forms.Label();
            labelTimeout = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            toolTip1 = new System.Windows.Forms.ToolTip(components);
            linkHelpURL = new System.Windows.Forms.LinkLabel();
            btTest = new System.Windows.Forms.Button();
            bt_clear = new System.Windows.Forms.Button();
            timer1 = new System.Windows.Forms.Timer(components);
            groupBox1.SuspendLayout();
            gb_AIServerQueue.SuspendLayout();
            groupBoxLinked.SuspendLayout();
            groupBoxRefine.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(84, 16);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(33, 13);
            label1.TabIndex = 0;
            label1.Text = "Type:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(87, 64);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(30, 13);
            label2.TabIndex = 1;
            label2.Text = "URL:";
            // 
            // lbl_type
            // 
            lbl_type.AutoSize = true;
            lbl_type.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            lbl_type.ForeColor = System.Drawing.Color.DodgerBlue;
            lbl_type.Location = new System.Drawing.Point(121, 16);
            lbl_type.Name = "lbl_type";
            lbl_type.Size = new System.Drawing.Size(10, 15);
            lbl_type.TabIndex = 0;
            lbl_type.Text = ".";
            // 
            // tb_URL
            // 
            tb_URL.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tb_URL.Font = new System.Drawing.Font("Consolas", 8.25F);
            tb_URL.Location = new System.Drawing.Point(121, 60);
            tb_URL.Name = "tb_URL";
            tb_URL.Size = new System.Drawing.Size(508, 20);
            tb_URL.TabIndex = 1;
            tb_URL.TextChanged += tb_URL_TextChanged;
            // 
            // bt_Save
            // 
            bt_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            bt_Save.DialogResult = System.Windows.Forms.DialogResult.OK;
            bt_Save.Location = new System.Drawing.Point(576, 533);
            bt_Save.Name = "bt_Save";
            bt_Save.Size = new System.Drawing.Size(70, 30);
            bt_Save.TabIndex = 17;
            bt_Save.Text = "Save";
            bt_Save.UseVisualStyleBackColor = true;
            bt_Save.Click += bt_Save_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(14, 116);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(103, 13);
            label4.TabIndex = 1;
            label4.Text = "Active Time Range:";
            // 
            // tb_ActiveTimeRange
            // 
            tb_ActiveTimeRange.Font = new System.Drawing.Font("Consolas", 8.25F);
            tb_ActiveTimeRange.Location = new System.Drawing.Point(121, 112);
            tb_ActiveTimeRange.Name = "tb_ActiveTimeRange";
            tb_ActiveTimeRange.Size = new System.Drawing.Size(152, 20);
            tb_ActiveTimeRange.TabIndex = 3;
            toolTip1.SetToolTip(tb_ActiveTimeRange, "Active time range in form of 00:00:00-23:59:59");
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(18, 90);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(99, 13);
            label5.TabIndex = 1;
            label5.Text = "Apply to Cameras:";
            // 
            // tb_ApplyToCams
            // 
            tb_ApplyToCams.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tb_ApplyToCams.Font = new System.Drawing.Font("Consolas", 8.25F);
            tb_ApplyToCams.Location = new System.Drawing.Point(121, 86);
            tb_ApplyToCams.Name = "tb_ApplyToCams";
            tb_ApplyToCams.Size = new System.Drawing.Size(508, 20);
            tb_ApplyToCams.TabIndex = 2;
            toolTip1.SetToolTip(tb_ApplyToCams, "A comma separated list of cameras that this AI server will work with.\r\n\r\nLeave empty for ALL.");
            // 
            // chk_Enabled
            // 
            chk_Enabled.AutoSize = true;
            chk_Enabled.ForeColor = System.Drawing.Color.DodgerBlue;
            chk_Enabled.Location = new System.Drawing.Point(11, 0);
            chk_Enabled.Name = "chk_Enabled";
            chk_Enabled.Size = new System.Drawing.Size(68, 17);
            chk_Enabled.TabIndex = 0;
            chk_Enabled.Text = "Enabled";
            chk_Enabled.UseVisualStyleBackColor = true;
            chk_Enabled.CheckedChanged += chk_Enabled_CheckedChanged;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            groupBox1.Controls.Add(gb_AIServerQueue);
            groupBox1.Controls.Add(cb_IgnoreOffline);
            groupBox1.Controls.Add(tb_Name);
            groupBox1.Controls.Add(label14);
            groupBox1.Controls.Add(cb_OnlyLinked);
            groupBox1.Controls.Add(cb_TimeoutError);
            groupBox1.Controls.Add(label12);
            groupBox1.Controls.Add(label10);
            groupBox1.Controls.Add(groupBoxLinked);
            groupBox1.Controls.Add(groupBoxRefine);
            groupBox1.Controls.Add(tb_Upper);
            groupBox1.Controls.Add(tb_LinkedRefineTimeout);
            groupBox1.Controls.Add(tb_Lower);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(tb_timeout);
            groupBox1.Controls.Add(tb_ImagesPerMonth);
            groupBox1.Controls.Add(chk_Enabled);
            groupBox1.Controls.Add(tb_ApplyToCams);
            groupBox1.Controls.Add(label15);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(tb_ActiveTimeRange);
            groupBox1.Controls.Add(lbl_type);
            groupBox1.Controls.Add(label9);
            groupBox1.Controls.Add(label8);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(tb_URL);
            groupBox1.Controls.Add(labelTimeout);
            groupBox1.Controls.Add(label7);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(label5);
            groupBox1.Location = new System.Drawing.Point(5, 6);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(636, 522);
            groupBox1.TabIndex = 7;
            groupBox1.TabStop = false;
            // 
            // gb_AIServerQueue
            // 
            gb_AIServerQueue.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            gb_AIServerQueue.Controls.Add(lbl_ImgQueueStats);
            gb_AIServerQueue.Controls.Add(lbl_QueueTimeStats);
            gb_AIServerQueue.Controls.Add(lbl_QueueLengthStats);
            gb_AIServerQueue.Controls.Add(label19);
            gb_AIServerQueue.Controls.Add(label18);
            gb_AIServerQueue.Controls.Add(cb_AllowAIServerBasedQueue);
            gb_AIServerQueue.Controls.Add(tb_SkipIfImgQueueLengthLarger);
            gb_AIServerQueue.Controls.Add(tb_SkipIfAIQueueTimeOverSecs);
            gb_AIServerQueue.Controls.Add(tb_MaxQueueLength);
            gb_AIServerQueue.Controls.Add(label17);
            gb_AIServerQueue.Controls.Add(label16);
            gb_AIServerQueue.Location = new System.Drawing.Point(7, 389);
            gb_AIServerQueue.Name = "gb_AIServerQueue";
            gb_AIServerQueue.Size = new System.Drawing.Size(619, 127);
            gb_AIServerQueue.TabIndex = 26;
            gb_AIServerQueue.TabStop = false;
            // 
            // lbl_ImgQueueStats
            // 
            lbl_ImgQueueStats.AutoSize = true;
            lbl_ImgQueueStats.Font = new System.Drawing.Font("Segoe UI", 8.142858F);
            lbl_ImgQueueStats.Location = new System.Drawing.Point(280, 102);
            lbl_ImgQueueStats.Name = "lbl_ImgQueueStats";
            lbl_ImgQueueStats.Size = new System.Drawing.Size(10, 13);
            lbl_ImgQueueStats.TabIndex = 28;
            lbl_ImgQueueStats.Text = ".";
            // 
            // lbl_QueueTimeStats
            // 
            lbl_QueueTimeStats.AutoSize = true;
            lbl_QueueTimeStats.Font = new System.Drawing.Font("Segoe UI", 8.142858F);
            lbl_QueueTimeStats.Location = new System.Drawing.Point(281, 74);
            lbl_QueueTimeStats.Name = "lbl_QueueTimeStats";
            lbl_QueueTimeStats.Size = new System.Drawing.Size(10, 13);
            lbl_QueueTimeStats.TabIndex = 28;
            lbl_QueueTimeStats.Text = ".";
            // 
            // lbl_QueueLengthStats
            // 
            lbl_QueueLengthStats.AutoSize = true;
            lbl_QueueLengthStats.Font = new System.Drawing.Font("Segoe UI", 8.142858F);
            lbl_QueueLengthStats.Location = new System.Drawing.Point(281, 47);
            lbl_QueueLengthStats.Name = "lbl_QueueLengthStats";
            lbl_QueueLengthStats.Size = new System.Drawing.Size(10, 13);
            lbl_QueueLengthStats.TabIndex = 28;
            lbl_QueueLengthStats.Text = ".";
            // 
            // label19
            // 
            label19.AutoSize = true;
            label19.Location = new System.Drawing.Point(11, 74);
            label19.Name = "label19";
            label19.Size = new System.Drawing.Size(201, 13);
            label19.TabIndex = 27;
            label19.Text = "AI Server Queue Seconds Larger Than:";
            // 
            // label18
            // 
            label18.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            label18.AutoSize = true;
            label18.ForeColor = System.Drawing.Color.Gray;
            label18.Location = new System.Drawing.Point(11, 23);
            label18.Name = "label18";
            label18.Size = new System.Drawing.Size(531, 13);
            label18.TabIndex = 26;
            label18.Text = "Skip to the next AITOOL url (if more than one available) when any of the following conditions are met:";
            // 
            // cb_AllowAIServerBasedQueue
            // 
            cb_AllowAIServerBasedQueue.AutoSize = true;
            cb_AllowAIServerBasedQueue.ForeColor = System.Drawing.Color.DodgerBlue;
            cb_AllowAIServerBasedQueue.Location = new System.Drawing.Point(7, 0);
            cb_AllowAIServerBasedQueue.Name = "cb_AllowAIServerBasedQueue";
            cb_AllowAIServerBasedQueue.Size = new System.Drawing.Size(212, 17);
            cb_AllowAIServerBasedQueue.TabIndex = 23;
            cb_AllowAIServerBasedQueue.Text = "Allow AI Server Based Queue / MESH";
            toolTip1.SetToolTip(cb_AllowAIServerBasedQueue, resources.GetString("cb_AllowAIServerBasedQueue.ToolTip"));
            cb_AllowAIServerBasedQueue.UseVisualStyleBackColor = true;
            cb_AllowAIServerBasedQueue.CheckedChanged += cb_AllowAIServerBasedQueue_CheckedChanged;
            // 
            // tb_SkipIfImgQueueLengthLarger
            // 
            tb_SkipIfImgQueueLengthLarger.Location = new System.Drawing.Point(219, 97);
            tb_SkipIfImgQueueLengthLarger.Name = "tb_SkipIfImgQueueLengthLarger";
            tb_SkipIfImgQueueLengthLarger.Size = new System.Drawing.Size(56, 22);
            tb_SkipIfImgQueueLengthLarger.TabIndex = 25;
            toolTip1.SetToolTip(tb_SkipIfImgQueueLengthLarger, "If the AITOOL image queue is larger than this value, and the server\r\nhas at least 1 item in its queue, skip to the next server URL to give it a\r\nchance to help lower the queue.");
            // 
            // tb_SkipIfAIQueueTimeOverSecs
            // 
            tb_SkipIfAIQueueTimeOverSecs.Location = new System.Drawing.Point(219, 69);
            tb_SkipIfAIQueueTimeOverSecs.Name = "tb_SkipIfAIQueueTimeOverSecs";
            tb_SkipIfAIQueueTimeOverSecs.Size = new System.Drawing.Size(56, 22);
            tb_SkipIfAIQueueTimeOverSecs.TabIndex = 25;
            toolTip1.SetToolTip(tb_SkipIfAIQueueTimeOverSecs, "If the AI Server queue has been busy for over this number of seconds,\r\njump to the next AITOOL server url.");
            // 
            // tb_MaxQueueLength
            // 
            tb_MaxQueueLength.Location = new System.Drawing.Point(219, 42);
            tb_MaxQueueLength.Name = "tb_MaxQueueLength";
            tb_MaxQueueLength.Size = new System.Drawing.Size(56, 22);
            tb_MaxQueueLength.TabIndex = 25;
            toolTip1.SetToolTip(tb_MaxQueueLength, "CodeProjectAI internal default maximum is 1024.  Lower this number to force the server\r\nto be 'busy' so that the next AITOOL server url in the list is used.   ");
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.ForeColor = System.Drawing.SystemColors.ControlText;
            label17.Location = new System.Drawing.Point(41, 102);
            label17.Name = "label17";
            label17.Size = new System.Drawing.Size(171, 13);
            label17.TabIndex = 24;
            label17.Text = "AITOOL Img Queue Larger Than:";
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.ForeColor = System.Drawing.SystemColors.ControlText;
            label16.Location = new System.Drawing.Point(18, 47);
            label16.Name = "label16";
            label16.Size = new System.Drawing.Size(194, 13);
            label16.TabIndex = 24;
            label16.Text = "AI Server Queue Length Larger Than:";
            // 
            // cb_IgnoreOffline
            // 
            cb_IgnoreOffline.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            cb_IgnoreOffline.AutoSize = true;
            cb_IgnoreOffline.ForeColor = System.Drawing.Color.DodgerBlue;
            cb_IgnoreOffline.Location = new System.Drawing.Point(179, 142);
            cb_IgnoreOffline.Name = "cb_IgnoreOffline";
            cb_IgnoreOffline.Size = new System.Drawing.Size(109, 17);
            cb_IgnoreOffline.TabIndex = 22;
            cb_IgnoreOffline.Text = "Ignore if Offline";
            toolTip1.SetToolTip(cb_IgnoreOffline, "If we cant even ping the server (ie a machine is asleep part of the day) we ignore any errors and skip the URL.");
            cb_IgnoreOffline.UseVisualStyleBackColor = true;
            // 
            // tb_Name
            // 
            tb_Name.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tb_Name.Location = new System.Drawing.Point(121, 33);
            tb_Name.Name = "tb_Name";
            tb_Name.Size = new System.Drawing.Size(508, 22);
            tb_Name.TabIndex = 21;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.ForeColor = System.Drawing.SystemColors.ControlDark;
            label14.Location = new System.Drawing.Point(170, 266);
            label14.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label14.Name = "label14";
            label14.Size = new System.Drawing.Size(393, 13);
            label14.TabIndex = 20;
            label14.Text = "(This should be checked if ANOTHER server uses this one as a linked server)";
            // 
            // cb_OnlyLinked
            // 
            cb_OnlyLinked.AutoSize = true;
            cb_OnlyLinked.ForeColor = System.Drawing.Color.DodgerBlue;
            cb_OnlyLinked.Location = new System.Drawing.Point(16, 265);
            cb_OnlyLinked.Name = "cb_OnlyLinked";
            cb_OnlyLinked.Size = new System.Drawing.Size(156, 17);
            cb_OnlyLinked.TabIndex = 10;
            cb_OnlyLinked.Text = "Use ONLY as linked server";
            cb_OnlyLinked.UseVisualStyleBackColor = true;
            cb_OnlyLinked.CheckedChanged += cb_OnlyLinked_CheckedChanged;
            // 
            // cb_TimeoutError
            // 
            cb_TimeoutError.AutoSize = true;
            cb_TimeoutError.ForeColor = System.Drawing.Color.Firebrick;
            cb_TimeoutError.Location = new System.Drawing.Point(537, 355);
            cb_TimeoutError.Name = "cb_TimeoutError";
            cb_TimeoutError.Size = new System.Drawing.Size(51, 17);
            cb_TimeoutError.TabIndex = 14;
            cb_TimeoutError.Text = "Error";
            toolTip1.SetToolTip(cb_TimeoutError, "An error will show in the log if a timeout happens");
            cb_TimeoutError.UseVisualStyleBackColor = true;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new System.Drawing.Point(501, 357);
            label12.Name = "label12";
            label12.Size = new System.Drawing.Size(21, 13);
            label12.TabIndex = 18;
            label12.Text = "ms";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.ForeColor = System.Drawing.Color.Firebrick;
            label10.Location = new System.Drawing.Point(8, 357);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(433, 13);
            label10.TabIndex = 18;
            label10.Text = "Maximum time to wait for a LINKED or REFINEMENT server URL to become available:";
            // 
            // groupBoxLinked
            // 
            groupBoxLinked.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            groupBoxLinked.Controls.Add(cb_LinkedServers);
            groupBoxLinked.Controls.Add(label13);
            groupBoxLinked.Controls.Add(checkedComboBoxLinked);
            groupBoxLinked.Location = new System.Drawing.Point(8, 288);
            groupBoxLinked.Name = "groupBoxLinked";
            groupBoxLinked.Size = new System.Drawing.Size(619, 63);
            groupBoxLinked.TabIndex = 17;
            groupBoxLinked.TabStop = false;
            // 
            // cb_LinkedServers
            // 
            cb_LinkedServers.AutoSize = true;
            cb_LinkedServers.ForeColor = System.Drawing.Color.DodgerBlue;
            cb_LinkedServers.Location = new System.Drawing.Point(8, 0);
            cb_LinkedServers.Name = "cb_LinkedServers";
            cb_LinkedServers.Size = new System.Drawing.Size(238, 17);
            cb_LinkedServers.TabIndex = 11;
            cb_LinkedServers.Text = "Link / Combine Results with other servers";
            cb_LinkedServers.UseVisualStyleBackColor = true;
            cb_LinkedServers.CheckedChanged += cb_LinkedServers_CheckedChanged;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new System.Drawing.Point(6, 20);
            label13.Name = "label13";
            label13.Size = new System.Drawing.Size(609, 13);
            label13.TabIndex = 13;
            label13.Text = "Wait for results from all linked servers - Useful to combine output from normal Deepstack and custom trained models";
            // 
            // checkedComboBoxLinked
            // 
            checkedComboBoxLinked.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            checkedComboBoxLinked.CheckOnClick = true;
            checkedComboBoxLinked.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            checkedComboBoxLinked.DropDownHeight = 1;
            checkedComboBoxLinked.Font = new System.Drawing.Font("Consolas", 8.25F);
            checkedComboBoxLinked.FormattingEnabled = true;
            checkedComboBoxLinked.IntegralHeight = false;
            checkedComboBoxLinked.Location = new System.Drawing.Point(6, 36);
            checkedComboBoxLinked.Name = "checkedComboBoxLinked";
            checkedComboBoxLinked.Size = new System.Drawing.Size(607, 21);
            checkedComboBoxLinked.TabIndex = 12;
            checkedComboBoxLinked.Text = "Click dropdown to select";
            checkedComboBoxLinked.ValueSeparator = ", ";
            // 
            // groupBoxRefine
            // 
            groupBoxRefine.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            groupBoxRefine.Controls.Add(cb_RefinementServer);
            groupBoxRefine.Controls.Add(label11);
            groupBoxRefine.Controls.Add(tb_RefinementObjects);
            groupBoxRefine.Location = new System.Drawing.Point(8, 196);
            groupBoxRefine.Name = "groupBoxRefine";
            groupBoxRefine.Size = new System.Drawing.Size(619, 63);
            groupBoxRefine.TabIndex = 16;
            groupBoxRefine.TabStop = false;
            // 
            // cb_RefinementServer
            // 
            cb_RefinementServer.AutoSize = true;
            cb_RefinementServer.ForeColor = System.Drawing.Color.DodgerBlue;
            cb_RefinementServer.Location = new System.Drawing.Point(8, 0);
            cb_RefinementServer.Name = "cb_RefinementServer";
            cb_RefinementServer.Size = new System.Drawing.Size(155, 17);
            cb_RefinementServer.TabIndex = 8;
            cb_RefinementServer.Text = "Use as Refinement Server";
            cb_RefinementServer.UseVisualStyleBackColor = true;
            cb_RefinementServer.CheckedChanged += cb_RefinementServer_CheckedChanged;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new System.Drawing.Point(3, 20);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(376, 13);
            label11.TabIndex = 13;
            label11.Text = "Use this server ONLY if another server detects the following objects first:";
            // 
            // tb_RefinementObjects
            // 
            tb_RefinementObjects.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tb_RefinementObjects.Font = new System.Drawing.Font("Consolas", 8.25F);
            tb_RefinementObjects.Location = new System.Drawing.Point(8, 36);
            tb_RefinementObjects.Name = "tb_RefinementObjects";
            tb_RefinementObjects.Size = new System.Drawing.Size(605, 20);
            tb_RefinementObjects.TabIndex = 9;
            // 
            // tb_Upper
            // 
            tb_Upper.Font = new System.Drawing.Font("Consolas", 8.25F);
            tb_Upper.Location = new System.Drawing.Point(214, 165);
            tb_Upper.Name = "tb_Upper";
            tb_Upper.Size = new System.Drawing.Size(49, 20);
            tb_Upper.TabIndex = 7;
            tb_Upper.Text = "100";
            // 
            // tb_LinkedRefineTimeout
            // 
            tb_LinkedRefineTimeout.Font = new System.Drawing.Font("Consolas", 8.25F);
            tb_LinkedRefineTimeout.Location = new System.Drawing.Point(446, 353);
            tb_LinkedRefineTimeout.Name = "tb_LinkedRefineTimeout";
            tb_LinkedRefineTimeout.Size = new System.Drawing.Size(49, 20);
            tb_LinkedRefineTimeout.TabIndex = 13;
            tb_LinkedRefineTimeout.Tag = "";
            tb_LinkedRefineTimeout.Text = "5000";
            // 
            // tb_Lower
            // 
            tb_Lower.Font = new System.Drawing.Font("Consolas", 8.25F);
            tb_Lower.Location = new System.Drawing.Point(121, 165);
            tb_Lower.Name = "tb_Lower";
            tb_Lower.Size = new System.Drawing.Size(49, 20);
            tb_Lower.TabIndex = 6;
            tb_Lower.Tag = "";
            tb_Lower.Text = "0";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(18, 168);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(99, 13);
            label3.TabIndex = 10;
            label3.Text = "Confidence limits:";
            // 
            // tb_timeout
            // 
            tb_timeout.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tb_timeout.Font = new System.Drawing.Font("Consolas", 8.25F);
            tb_timeout.Location = new System.Drawing.Point(121, 140);
            tb_timeout.Name = "tb_timeout";
            tb_timeout.Size = new System.Drawing.Size(52, 20);
            tb_timeout.TabIndex = 5;
            tb_timeout.Text = "0";
            toolTip1.SetToolTip(tb_timeout, "If you set this to any value other than 0 it will override the default timeout");
            // 
            // tb_ImagesPerMonth
            // 
            tb_ImagesPerMonth.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tb_ImagesPerMonth.Font = new System.Drawing.Font("Consolas", 8.25F);
            tb_ImagesPerMonth.Location = new System.Drawing.Point(584, 112);
            tb_ImagesPerMonth.Name = "tb_ImagesPerMonth";
            tb_ImagesPerMonth.Size = new System.Drawing.Size(45, 20);
            tb_ImagesPerMonth.TabIndex = 4;
            tb_ImagesPerMonth.Text = "0";
            toolTip1.SetToolTip(tb_ImagesPerMonth, "Max images per month - 0 for unlimited.    Amazon has 5000 free images a month");
            // 
            // label15
            // 
            label15.AllowDrop = true;
            label15.AutoSize = true;
            label15.Location = new System.Drawing.Point(78, 36);
            label15.Name = "label15";
            label15.Size = new System.Drawing.Size(39, 13);
            label15.TabIndex = 0;
            label15.Text = "Name:";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new System.Drawing.Point(267, 168);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(329, 13);
            label9.TabIndex = 1;
            label9.Text = "Upper    (These will override the CAMERA setting if configured)";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(172, 168);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(38, 13);
            label8.TabIndex = 1;
            label8.Text = "Lower";
            // 
            // labelTimeout
            // 
            labelTimeout.AutoSize = true;
            labelTimeout.Location = new System.Drawing.Point(65, 144);
            labelTimeout.Name = "labelTimeout";
            labelTimeout.Size = new System.Drawing.Size(52, 13);
            labelTimeout.TabIndex = 1;
            labelTimeout.Text = "Timeout:";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(451, 116);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(127, 13);
            label7.TabIndex = 1;
            label7.Text = "Max Images Per Month:";
            // 
            // linkHelpURL
            // 
            linkHelpURL.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            linkHelpURL.AutoSize = true;
            linkHelpURL.Location = new System.Drawing.Point(5, 530);
            linkHelpURL.Name = "linkHelpURL";
            linkHelpURL.Size = new System.Drawing.Size(10, 13);
            linkHelpURL.TabIndex = 8;
            linkHelpURL.TabStop = true;
            linkHelpURL.Text = ".";
            linkHelpURL.LinkClicked += linkHelpURL_LinkClicked;
            // 
            // btTest
            // 
            btTest.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btTest.Location = new System.Drawing.Point(500, 533);
            btTest.Name = "btTest";
            btTest.Size = new System.Drawing.Size(70, 30);
            btTest.TabIndex = 16;
            btTest.Text = "Test";
            btTest.UseVisualStyleBackColor = true;
            btTest.Click += btTest_Click;
            // 
            // bt_clear
            // 
            bt_clear.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            bt_clear.Location = new System.Drawing.Point(424, 533);
            bt_clear.Name = "bt_clear";
            bt_clear.Size = new System.Drawing.Size(70, 30);
            bt_clear.TabIndex = 15;
            bt_clear.Text = "Clear Stats";
            bt_clear.UseVisualStyleBackColor = true;
            bt_clear.Click += bt_clear_Click;
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Interval = 500;
            timer1.Tick += timer1_Tick;
            // 
            // Frm_AIServerDeepstackEdit
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            AutoScroll = true;
            ClientSize = new System.Drawing.Size(649, 570);
            Controls.Add(bt_Save);
            Controls.Add(bt_clear);
            Controls.Add(btTest);
            Controls.Add(linkHelpURL);
            Controls.Add(groupBox1);
            Font = new System.Drawing.Font("Segoe UI", 8.25F);
            Name = "Frm_AIServerDeepstackEdit";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Edit AI Server";
            FormClosing += Frm_AIServerDeepstackEdit_FormClosing;
            Load += Frm_AIServerDeepstackEdit_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            gb_AIServerQueue.ResumeLayout(false);
            gb_AIServerQueue.PerformLayout();
            groupBoxLinked.ResumeLayout(false);
            groupBoxLinked.PerformLayout();
            groupBoxRefine.ResumeLayout(false);
            groupBoxRefine.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbl_type;
        private System.Windows.Forms.Button bt_Save;
        public System.Windows.Forms.TextBox tb_URL;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.TextBox tb_ActiveTimeRange;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.TextBox tb_ApplyToCams;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox chk_Enabled;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tb_ImagesPerMonth;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.LinkLabel linkHelpURL;
        private System.Windows.Forms.Button btTest;
        private System.Windows.Forms.Button bt_clear;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tb_Upper;
        private System.Windows.Forms.TextBox tb_Lower;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tb_RefinementObjects;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox cb_RefinementServer;
        private System.Windows.Forms.TextBox tb_timeout;
        private System.Windows.Forms.Label labelTimeout;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.CheckBox cb_LinkedServers;
        private CheckComboBoxTest.CheckedComboBox checkedComboBoxLinked;
        private System.Windows.Forms.GroupBox groupBoxLinked;
        private System.Windows.Forms.GroupBox groupBoxRefine;
        private System.Windows.Forms.CheckBox cb_TimeoutError;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox tb_LinkedRefineTimeout;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.CheckBox cb_OnlyLinked;
        private System.Windows.Forms.TextBox tb_Name;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.CheckBox cb_IgnoreOffline;
        private System.Windows.Forms.CheckBox cb_AllowAIServerBasedQueue;
        private System.Windows.Forms.TextBox tb_MaxQueueLength;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.GroupBox gb_AIServerQueue;
        private System.Windows.Forms.TextBox tb_SkipIfImgQueueLengthLarger;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox tb_SkipIfAIQueueTimeOverSecs;
        private System.Windows.Forms.Label lbl_QueueLengthStats;
        private System.Windows.Forms.Label lbl_ImgQueueStats;
        private System.Windows.Forms.Label lbl_QueueTimeStats;
        private System.Windows.Forms.Timer timer1;
    }
}