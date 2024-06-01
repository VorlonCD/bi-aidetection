
namespace AITool
{
    partial class Frm_AIServers
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_AIServers));
            FOLV_AIServers = new BrightIdeasSoftware.FastObjectListView();
            toolStrip1 = new System.Windows.Forms.ToolStrip();
            toolStripSplitButtonAdd = new System.Windows.Forms.ToolStripSplitButton();
            codeProjectAIObjectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            codeProjectAILicensePlateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            codeProjectAIFacesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            codeProjectAISceneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            codeProjectAICustomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            codeProjectAIIPCAMAnimalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            codeProjectAIIPCAMCombinedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            codeProjectAIIPCAMDarkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            codeProjectAIIPCAMGeneralToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            deepstackObjectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            deepstackCustomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            deepstackSceneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            deepstackFacesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            addDoodsServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            addAmazonObjectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            addAmazonFaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            sightHoundVehicleAIServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            sightHoundPersonAIServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            toolStripButtonEdit = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            toolStripButtonDelete = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            toolStripButtonUp = new System.Windows.Forms.ToolStripButton();
            toolStripButtonDown = new System.Windows.Forms.ToolStripButton();
            imageList1 = new System.Windows.Forms.ImageList(components);
            timer1 = new System.Windows.Forms.Timer(components);
            ((System.ComponentModel.ISupportInitialize)FOLV_AIServers).BeginInit();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // FOLV_AIServers
            // 
            FOLV_AIServers.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            FOLV_AIServers.CheckBoxes = true;
            FOLV_AIServers.Location = new System.Drawing.Point(0, 34);
            FOLV_AIServers.Name = "FOLV_AIServers";
            FOLV_AIServers.ShowGroups = false;
            FOLV_AIServers.ShowImagesOnSubItems = true;
            FOLV_AIServers.Size = new System.Drawing.Size(591, 122);
            FOLV_AIServers.TabIndex = 0;
            FOLV_AIServers.UseCompatibleStateImageBehavior = false;
            FOLV_AIServers.View = System.Windows.Forms.View.Details;
            FOLV_AIServers.VirtualMode = true;
            FOLV_AIServers.FormatCell += FOLV_AIServers_FormatCell;
            FOLV_AIServers.FormatRow += FOLV_AIServers_FormatRow;
            FOLV_AIServers.SelectionChanged += FOLV_AIServers_SelectionChanged;
            FOLV_AIServers.SelectedIndexChanged += FOLV_AIServers_SelectedIndexChanged;
            FOLV_AIServers.DoubleClick += FOLV_AIServers_DoubleClick;
            // 
            // toolStrip1
            // 
            toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripSplitButtonAdd, toolStripSeparator1, toolStripButtonEdit, toolStripSeparator2, toolStripButtonDelete, toolStripSeparator3, toolStripButtonUp, toolStripButtonDown });
            toolStrip1.Location = new System.Drawing.Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new System.Drawing.Size(591, 31);
            toolStrip1.TabIndex = 2;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolStripSplitButtonAdd
            // 
            toolStripSplitButtonAdd.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { codeProjectAIObjectsToolStripMenuItem, codeProjectAILicensePlateToolStripMenuItem, codeProjectAIFacesToolStripMenuItem, codeProjectAISceneToolStripMenuItem, codeProjectAICustomToolStripMenuItem, codeProjectAIIPCAMAnimalToolStripMenuItem, codeProjectAIIPCAMCombinedToolStripMenuItem, codeProjectAIIPCAMDarkToolStripMenuItem, codeProjectAIIPCAMGeneralToolStripMenuItem, deepstackObjectsToolStripMenuItem, deepstackCustomToolStripMenuItem, deepstackSceneToolStripMenuItem, deepstackFacesToolStripMenuItem, addDoodsServerToolStripMenuItem, addAmazonObjectsToolStripMenuItem, addAmazonFaceToolStripMenuItem, sightHoundVehicleAIServerToolStripMenuItem, sightHoundPersonAIServerToolStripMenuItem });
            toolStripSplitButtonAdd.Image = (System.Drawing.Image)resources.GetObject("toolStripSplitButtonAdd.Image");
            toolStripSplitButtonAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripSplitButtonAdd.Name = "toolStripSplitButtonAdd";
            toolStripSplitButtonAdd.Size = new System.Drawing.Size(69, 28);
            toolStripSplitButtonAdd.Text = "Add";
            toolStripSplitButtonAdd.ButtonClick += toolStripSplitButtonAdd_ButtonClick;
            // 
            // codeProjectAIObjectsToolStripMenuItem
            // 
            codeProjectAIObjectsToolStripMenuItem.Image = Properties.Resources.Codeproject_2023_06_03_15_45_28;
            codeProjectAIObjectsToolStripMenuItem.Name = "codeProjectAIObjectsToolStripMenuItem";
            codeProjectAIObjectsToolStripMenuItem.Size = new System.Drawing.Size(273, 30);
            codeProjectAIObjectsToolStripMenuItem.Text = "CodeProject AI (Objects)";
            codeProjectAIObjectsToolStripMenuItem.Click += codeProjectAIObjectsToolStripMenuItem_Click;
            // 
            // codeProjectAILicensePlateToolStripMenuItem
            // 
            codeProjectAILicensePlateToolStripMenuItem.Image = Properties.Resources.Codeproject_2023_06_03_15_45_28;
            codeProjectAILicensePlateToolStripMenuItem.Name = "codeProjectAILicensePlateToolStripMenuItem";
            codeProjectAILicensePlateToolStripMenuItem.Size = new System.Drawing.Size(273, 30);
            codeProjectAILicensePlateToolStripMenuItem.Text = "CodeProject AI (License Plate)";
            codeProjectAILicensePlateToolStripMenuItem.Click += codeProjectAILicensePlateToolStripMenuItem_Click;
            // 
            // codeProjectAIFacesToolStripMenuItem
            // 
            codeProjectAIFacesToolStripMenuItem.Image = Properties.Resources.Codeproject_2023_06_03_15_45_28;
            codeProjectAIFacesToolStripMenuItem.Name = "codeProjectAIFacesToolStripMenuItem";
            codeProjectAIFacesToolStripMenuItem.Size = new System.Drawing.Size(273, 30);
            codeProjectAIFacesToolStripMenuItem.Text = "CodeProject AI (Faces)";
            codeProjectAIFacesToolStripMenuItem.Click += codeProjectAIFacesToolStripMenuItem_Click;
            // 
            // codeProjectAISceneToolStripMenuItem
            // 
            codeProjectAISceneToolStripMenuItem.Image = Properties.Resources.Codeproject_2023_06_03_15_45_28;
            codeProjectAISceneToolStripMenuItem.Name = "codeProjectAISceneToolStripMenuItem";
            codeProjectAISceneToolStripMenuItem.Size = new System.Drawing.Size(273, 30);
            codeProjectAISceneToolStripMenuItem.Text = "CodeProject AI (Scene)";
            codeProjectAISceneToolStripMenuItem.Click += codeProjectAISceneToolStripMenuItem_Click;
            // 
            // codeProjectAICustomToolStripMenuItem
            // 
            codeProjectAICustomToolStripMenuItem.Image = Properties.Resources.Codeproject_2023_06_03_15_45_28;
            codeProjectAICustomToolStripMenuItem.Name = "codeProjectAICustomToolStripMenuItem";
            codeProjectAICustomToolStripMenuItem.Size = new System.Drawing.Size(273, 30);
            codeProjectAICustomToolStripMenuItem.Text = "CodeProject AI (Custom)";
            codeProjectAICustomToolStripMenuItem.Click += codeProjectAICustomToolStripMenuItem_Click;
            // 
            // codeProjectAIIPCAMAnimalToolStripMenuItem
            // 
            codeProjectAIIPCAMAnimalToolStripMenuItem.Image = Properties.Resources.Codeproject_2023_06_03_15_45_28;
            codeProjectAIIPCAMAnimalToolStripMenuItem.Name = "codeProjectAIIPCAMAnimalToolStripMenuItem";
            codeProjectAIIPCAMAnimalToolStripMenuItem.Size = new System.Drawing.Size(273, 30);
            codeProjectAIIPCAMAnimalToolStripMenuItem.Text = "CodeProject AI (IPCAM Animal)";
            codeProjectAIIPCAMAnimalToolStripMenuItem.Click += codeProjectAIIPCAMAnimalToolStripMenuItem_Click;
            // 
            // codeProjectAIIPCAMCombinedToolStripMenuItem
            // 
            codeProjectAIIPCAMCombinedToolStripMenuItem.Image = Properties.Resources.Codeproject_2023_06_03_15_45_28;
            codeProjectAIIPCAMCombinedToolStripMenuItem.Name = "codeProjectAIIPCAMCombinedToolStripMenuItem";
            codeProjectAIIPCAMCombinedToolStripMenuItem.Size = new System.Drawing.Size(273, 30);
            codeProjectAIIPCAMCombinedToolStripMenuItem.Text = "CodeProject AI (IPCAM Combined)";
            codeProjectAIIPCAMCombinedToolStripMenuItem.Click += codeProjectAIIPCAMCombinedToolStripMenuItem_Click;
            // 
            // codeProjectAIIPCAMDarkToolStripMenuItem
            // 
            codeProjectAIIPCAMDarkToolStripMenuItem.Image = Properties.Resources.Codeproject_2023_06_03_15_45_28;
            codeProjectAIIPCAMDarkToolStripMenuItem.Name = "codeProjectAIIPCAMDarkToolStripMenuItem";
            codeProjectAIIPCAMDarkToolStripMenuItem.Size = new System.Drawing.Size(273, 30);
            codeProjectAIIPCAMDarkToolStripMenuItem.Text = "CodeProject AI (IPCAM Dark)";
            codeProjectAIIPCAMDarkToolStripMenuItem.Click += codeProjectAIIPCAMDarkToolStripMenuItem_Click;
            // 
            // codeProjectAIIPCAMGeneralToolStripMenuItem
            // 
            codeProjectAIIPCAMGeneralToolStripMenuItem.Image = Properties.Resources.Codeproject_2023_06_03_15_45_28;
            codeProjectAIIPCAMGeneralToolStripMenuItem.Name = "codeProjectAIIPCAMGeneralToolStripMenuItem";
            codeProjectAIIPCAMGeneralToolStripMenuItem.Size = new System.Drawing.Size(273, 30);
            codeProjectAIIPCAMGeneralToolStripMenuItem.Text = "CodeProject AI (IPCAM General)";
            codeProjectAIIPCAMGeneralToolStripMenuItem.Click += codeProjectAIIPCAMGeneralToolStripMenuItem_Click;
            // 
            // deepstackObjectsToolStripMenuItem
            // 
            deepstackObjectsToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("deepstackObjectsToolStripMenuItem.Image");
            deepstackObjectsToolStripMenuItem.Name = "deepstackObjectsToolStripMenuItem";
            deepstackObjectsToolStripMenuItem.Size = new System.Drawing.Size(273, 30);
            deepstackObjectsToolStripMenuItem.Text = "Deepstack (Objects) AI Server";
            deepstackObjectsToolStripMenuItem.ToolTipText = "Detect regular objects such as person, car, truck, etc.";
            deepstackObjectsToolStripMenuItem.Click += deepstackToolStripMenuItem_Click;
            // 
            // deepstackCustomToolStripMenuItem
            // 
            deepstackCustomToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("deepstackCustomToolStripMenuItem.Image");
            deepstackCustomToolStripMenuItem.Name = "deepstackCustomToolStripMenuItem";
            deepstackCustomToolStripMenuItem.Size = new System.Drawing.Size(273, 30);
            deepstackCustomToolStripMenuItem.Text = "Deepstack (Custom) AI Server";
            deepstackCustomToolStripMenuItem.ToolTipText = "Use a custom trained model for detection";
            deepstackCustomToolStripMenuItem.Click += deepstackCustomToolStripMenuItem_Click;
            // 
            // deepstackSceneToolStripMenuItem
            // 
            deepstackSceneToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("deepstackSceneToolStripMenuItem.Image");
            deepstackSceneToolStripMenuItem.Name = "deepstackSceneToolStripMenuItem";
            deepstackSceneToolStripMenuItem.Size = new System.Drawing.Size(273, 30);
            deepstackSceneToolStripMenuItem.Text = "Deepstack (Scene) AI Server";
            deepstackSceneToolStripMenuItem.ToolTipText = "Detect the scene such as \"conference_room\".";
            deepstackSceneToolStripMenuItem.Click += deepstackSceneToolStripMenuItem_Click;
            // 
            // deepstackFacesToolStripMenuItem
            // 
            deepstackFacesToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("deepstackFacesToolStripMenuItem.Image");
            deepstackFacesToolStripMenuItem.Name = "deepstackFacesToolStripMenuItem";
            deepstackFacesToolStripMenuItem.Size = new System.Drawing.Size(273, 30);
            deepstackFacesToolStripMenuItem.Text = "Deepstack (Faces) AI Server";
            deepstackFacesToolStripMenuItem.ToolTipText = "Face detection";
            deepstackFacesToolStripMenuItem.Click += deepstackFacesToolStripMenuItem_Click;
            // 
            // addDoodsServerToolStripMenuItem
            // 
            addDoodsServerToolStripMenuItem.Image = Properties.Resources.network_server;
            addDoodsServerToolStripMenuItem.Name = "addDoodsServerToolStripMenuItem";
            addDoodsServerToolStripMenuItem.Size = new System.Drawing.Size(273, 30);
            addDoodsServerToolStripMenuItem.Text = "DOODS AI Server";
            addDoodsServerToolStripMenuItem.Click += addDoodsServerToolStripMenuItem_Click;
            // 
            // addAmazonObjectsToolStripMenuItem
            // 
            addAmazonObjectsToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("addAmazonObjectsToolStripMenuItem.Image");
            addAmazonObjectsToolStripMenuItem.Name = "addAmazonObjectsToolStripMenuItem";
            addAmazonObjectsToolStripMenuItem.Size = new System.Drawing.Size(273, 30);
            addAmazonObjectsToolStripMenuItem.Text = "AWS Rekognition (Objects) AI Server";
            addAmazonObjectsToolStripMenuItem.ToolTipText = "Detect regular objects such as person, car, truck, etc.";
            addAmazonObjectsToolStripMenuItem.Click += addAmazonReToolStripMenuItem_Click;
            // 
            // addAmazonFaceToolStripMenuItem
            // 
            addAmazonFaceToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("addAmazonFaceToolStripMenuItem.Image");
            addAmazonFaceToolStripMenuItem.Name = "addAmazonFaceToolStripMenuItem";
            addAmazonFaceToolStripMenuItem.Size = new System.Drawing.Size(273, 30);
            addAmazonFaceToolStripMenuItem.Text = "AWS Rekognition (Faces) AI Server";
            addAmazonFaceToolStripMenuItem.ToolTipText = "Person age, emotion and gender";
            addAmazonFaceToolStripMenuItem.Click += addAmazonFaceToolStripMenuItem_Click;
            // 
            // sightHoundVehicleAIServerToolStripMenuItem
            // 
            sightHoundVehicleAIServerToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("sightHoundVehicleAIServerToolStripMenuItem.Image");
            sightHoundVehicleAIServerToolStripMenuItem.Name = "sightHoundVehicleAIServerToolStripMenuItem";
            sightHoundVehicleAIServerToolStripMenuItem.Size = new System.Drawing.Size(273, 30);
            sightHoundVehicleAIServerToolStripMenuItem.Text = "SightHound (Vehicle) AI Server";
            sightHoundVehicleAIServerToolStripMenuItem.ToolTipText = "Car make model and license plate";
            sightHoundVehicleAIServerToolStripMenuItem.Click += sightHoundAIServerToolStripMenuItem_Click;
            // 
            // sightHoundPersonAIServerToolStripMenuItem
            // 
            sightHoundPersonAIServerToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("sightHoundPersonAIServerToolStripMenuItem.Image");
            sightHoundPersonAIServerToolStripMenuItem.Name = "sightHoundPersonAIServerToolStripMenuItem";
            sightHoundPersonAIServerToolStripMenuItem.Size = new System.Drawing.Size(273, 30);
            sightHoundPersonAIServerToolStripMenuItem.Text = "SightHound (Person) AI Server";
            sightHoundPersonAIServerToolStripMenuItem.ToolTipText = "Person age, emotion, gender";
            sightHoundPersonAIServerToolStripMenuItem.Click += sightHoundPersonAIServerToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(6, 31);
            // 
            // toolStripButtonEdit
            // 
            toolStripButtonEdit.Enabled = false;
            toolStripButtonEdit.Image = (System.Drawing.Image)resources.GetObject("toolStripButtonEdit.Image");
            toolStripButtonEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButtonEdit.Name = "toolStripButtonEdit";
            toolStripButtonEdit.Size = new System.Drawing.Size(55, 28);
            toolStripButtonEdit.Text = "Edit";
            toolStripButtonEdit.Click += toolStripButtonEdit_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new System.Drawing.Size(6, 31);
            // 
            // toolStripButtonDelete
            // 
            toolStripButtonDelete.Enabled = false;
            toolStripButtonDelete.Image = (System.Drawing.Image)resources.GetObject("toolStripButtonDelete.Image");
            toolStripButtonDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButtonDelete.Name = "toolStripButtonDelete";
            toolStripButtonDelete.Size = new System.Drawing.Size(68, 28);
            toolStripButtonDelete.Text = "Delete";
            toolStripButtonDelete.Click += toolStripButtonDelete_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new System.Drawing.Size(6, 31);
            // 
            // toolStripButtonUp
            // 
            toolStripButtonUp.Enabled = false;
            toolStripButtonUp.Image = (System.Drawing.Image)resources.GetObject("toolStripButtonUp.Image");
            toolStripButtonUp.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButtonUp.Name = "toolStripButtonUp";
            toolStripButtonUp.Size = new System.Drawing.Size(50, 28);
            toolStripButtonUp.Text = "Up";
            toolStripButtonUp.Click += toolStripButtonUp_Click;
            // 
            // toolStripButtonDown
            // 
            toolStripButtonDown.Enabled = false;
            toolStripButtonDown.Image = (System.Drawing.Image)resources.GetObject("toolStripButtonDown.Image");
            toolStripButtonDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButtonDown.Name = "toolStripButtonDown";
            toolStripButtonDown.Size = new System.Drawing.Size(66, 28);
            toolStripButtonDown.Text = "Down";
            toolStripButtonDown.Click += toolStripButtonDown_Click;
            // 
            // imageList1
            // 
            imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            imageList1.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("imageList1.ImageStream");
            imageList1.TransparentColor = System.Drawing.Color.Transparent;
            imageList1.Images.SetKeyName(0, "AWSRekognition.png");
            imageList1.Images.SetKeyName(1, "Deepstack.png");
            imageList1.Images.SetKeyName(2, "DOODS.png");
            imageList1.Images.SetKeyName(3, "SightHound.png");
            imageList1.Images.SetKeyName(4, "Codeproject.png");
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Interval = 1000;
            timer1.Tick += timer1_Tick;
            // 
            // Frm_AIServers
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(591, 159);
            Controls.Add(toolStrip1);
            Controls.Add(FOLV_AIServers);
            Font = new System.Drawing.Font("Segoe UI", 8.25F);
            Name = "Frm_AIServers";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Tag = "SAVE";
            Text = "AI Servers";
            FormClosing += Frm_AddAIServers_FormClosing;
            Load += Frm_AddAIServers_Load;
            ((System.ComponentModel.ISupportInitialize)FOLV_AIServers).EndInit();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        public BrightIdeasSoftware.FastObjectListView FOLV_AIServers;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonEdit;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitButtonAdd;
        private System.Windows.Forms.ToolStripMenuItem deepstackObjectsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addAmazonObjectsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addDoodsServerToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButtonDelete;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton toolStripButtonUp;
        private System.Windows.Forms.ToolStripButton toolStripButtonDown;
        private System.Windows.Forms.ToolStripMenuItem sightHoundVehicleAIServerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sightHoundPersonAIServerToolStripMenuItem;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolStripMenuItem addAmazonFaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deepstackCustomToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deepstackFacesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deepstackSceneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem codeProjectAIObjectsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem codeProjectAIFacesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem codeProjectAICustomToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem codeProjectAISceneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem codeProjectAILicensePlateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem codeProjectAIIPCAMAnimalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem codeProjectAIIPCAMDarkToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem codeProjectAIIPCAMGeneralToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem codeProjectAIIPCAMCombinedToolStripMenuItem;
        private System.Windows.Forms.Timer timer1;
    }
}