using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AITool
{
    public partial class Frm_AIServers:Form
    {

        ClsURLItem CurURL = null;

        public Frm_AIServers()
        {
            InitializeComponent();
        }

        private void Frm_AddAIServers_Load(object sender, EventArgs e)
        {
            using Working w = new Working();
            Global_GUI.RestoreWindowState(this);
            Global_GUI.ConfigureFOLV(FOLV_AIServers, typeof(ClsURLItem), null, this.imageList1, editmode: BrightIdeasSoftware.ObjectListView.CellEditActivateMode.F2Only);

            this.FOLV_AIServers.BooleanCheckStateGetter = delegate (Object rowObject)
            {
                return !rowObject.IsNull() && ((ClsURLItem)rowObject).Enabled;
            };

            this.FOLV_AIServers.BooleanCheckStatePutter = delegate (Object rowObject, bool newValue)
            {
                if (rowObject.IsNull())
                    return false;
                ((ClsURLItem)rowObject).Enabled = newValue;
                return newValue;
            };

            Global_GUI.UpdateFOLV(FOLV_AIServers, AppSettings.Settings.AIURLList);
        }

        private void Frm_AddAIServers_FormClosing(object sender, FormClosingEventArgs e)
        {
            using Working w = new Working();
            Global_GUI.SaveWindowState(this);
        }


        private void FOLV_AIServers_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void FOLV_AIServers_SelectionChanged(object sender, EventArgs e)
        {
            using Working w = new Working();
            try
            {
                if (this.FOLV_AIServers.SelectedObjects != null && this.FOLV_AIServers.SelectedObjects.Count > 0)
                {
                    this.CurURL = (ClsURLItem)this.FOLV_AIServers.SelectedObjects[0];
                }
                else
                {
                    this.CurURL = null;
                }

            }
            catch (Exception ex)
            {
                AITOOL.Log($"Error: {ex.Msg()}");

            }

            UpdateButtons();

        }

        private void UpdateButtons()
        {
            if (this.CurURL != null)
            {

                toolStripButtonDelete.Enabled = true;
                toolStripButtonDown.Enabled = true;
                toolStripButtonEdit.Enabled = true;
                toolStripButtonUp.Enabled = true;
            }
            else
            {
                toolStripButtonDelete.Enabled = false;
                toolStripButtonDown.Enabled = false;
                toolStripButtonEdit.Enabled = false;
                toolStripButtonUp.Enabled = false;
            }

        }

        private void deepstackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using Working w = new Working();
            ClsURLItem url = new ClsURLItem("", AppSettings.Settings.AIURLList.Count + 1, URLTypeEnum.DeepStack);
            this.CurURL = url;
            AppSettings.Settings.AIURLList.Add(url);
            Global_GUI.UpdateFOLV(FOLV_AIServers, AppSettings.Settings.AIURLList, UseSelected: true, SelectObject: this.CurURL, FullRefresh: true);
            UpdateButtons();

        }

        private void addAmazonReToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using Working w = new Working();
            ClsURLItem url = new ClsURLItem("", AppSettings.Settings.AIURLList.Count + 1, URLTypeEnum.AWSRekognition_Objects);
            if (!AppSettings.Settings.AIURLList.Contains(url))
            {
                this.CurURL = url;
                AppSettings.Settings.AIURLList.Add(url);
                Global_GUI.UpdateFOLV(FOLV_AIServers, AppSettings.Settings.AIURLList, UseSelected: true, SelectObject: this.CurURL, FullRefresh: true);
                AITOOL.UpdateAIURLs();

            }
            else
            {
                MessageBox.Show("Already exists");
            }
            UpdateButtons();

        }

        private void addDoodsServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using Working w = new Working();
            ClsURLItem url = new ClsURLItem("", AppSettings.Settings.AIURLList.Count + 1, URLTypeEnum.DOODS);
            if (!AppSettings.Settings.AIURLList.Contains(url))
            {
                this.CurURL = url;
                AppSettings.Settings.AIURLList.Add(url);
                Global_GUI.UpdateFOLV(FOLV_AIServers, AppSettings.Settings.AIURLList, UseSelected: true, SelectObject: this.CurURL, FullRefresh: true);
            }
            else
            {
                MessageBox.Show("Already exists");
            }
            UpdateButtons();

        }

        private void toolStripButtonDelete_Click(object sender, EventArgs e)
        {
            using Working w = new Working();
            if (AppSettings.Settings.AIURLList.Contains(this.CurURL))
            {
                AppSettings.Settings.AIURLList.Remove(this.CurURL);
                for (int i = 0; i < AppSettings.Settings.AIURLList.Count; i++)
                {
                    AppSettings.Settings.AIURLList[i].Order = i + 1;
                }
                Global_GUI.UpdateFOLV(FOLV_AIServers, AppSettings.Settings.AIURLList, UseSelected: true, FullRefresh: true);
            }
            UpdateButtons();

        }

        private void toolStripButtonEdit_Click(object sender, EventArgs e)
        {
            Edit();


        }

        private void Edit()
        {
            using (Frm_AIServerDeepstackEdit frm = new Frm_AIServerDeepstackEdit())
            {
                frm.CurURL = this.CurURL;
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    this.CurURL = frm.CurURL;  //this should update the list item by ref - prob not needed but makes me feel warm and fuzzy
                    Global_GUI.UpdateFOLV(FOLV_AIServers, AppSettings.Settings.AIURLList, UseSelected: true, SelectObject: this.CurURL, FullRefresh: true);
                }
            }
            UpdateButtons();
        }

        private void toolStripButtonUp_Click(object sender, EventArgs e)
        {
            using Working w = new Working();
            if (this.CurURL == null)
                return;

            int idx = AppSettings.Settings.AIURLList.IndexOf(this.CurURL);

            if (idx > -1)
            {
                AppSettings.Settings.AIURLList.Move(idx, idx - 1);

                for (int i = 0; i < AppSettings.Settings.AIURLList.Count; i++)
                {
                    AppSettings.Settings.AIURLList[i].Order = i + 1;
                }

                Global_GUI.UpdateFOLV(FOLV_AIServers, AppSettings.Settings.AIURLList, UseSelected: true, SelectObject: this.CurURL, FullRefresh: true, ForcedSelection: true);

            }
        }

        private void toolStripButtonDown_Click(object sender, EventArgs e)
        {
            using Working w = new Working();

            if (this.CurURL == null)
                return;

            int idx = AppSettings.Settings.AIURLList.IndexOf(this.CurURL);

            if (idx > -1)
            {
                AppSettings.Settings.AIURLList.Move(idx, idx + 1);

                for (int i = 0; i < AppSettings.Settings.AIURLList.Count; i++)
                {
                    AppSettings.Settings.AIURLList[i].Order = i + 1;
                }

                Global_GUI.UpdateFOLV(FOLV_AIServers, AppSettings.Settings.AIURLList, UseSelected: true, SelectObject: this.CurURL, FullRefresh: true);

            }
        }

        private void FOLV_AIServers_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Edit();
        }

        private void FOLV_AIServers_FormatRow(object sender, BrightIdeasSoftware.FormatRowEventArgs e)
        {
            FormatURLRow(sender, e);
        }

        private void FormatCell(object sender, BrightIdeasSoftware.FormatCellEventArgs e)
        {
            try
            {
                if (e.CellValue is DateTime)
                {
                    //26-Feb-2015 08:51:18.820
                    DateTime dt = ((DateTime)e.CellValue);
                    string NewText = "";
                    if (dt > DateTime.MinValue)
                    {
                        NewText = dt.ToString(AppSettings.Settings.DateFormat);  //"yyyy-MM-dd hh:mm:ss tt"
                    }
                    else
                    {
                        NewText = "";
                    }
                    //"yyyy-MM-dd HH:mm:ss.fff"
                    e.SubItem.Text = NewText;
                }
                else if (e.CellValue is ThreadSafe.DateTime)
                {
                    //26-Feb-2015 08:51:18.820
                    DateTime dt = ((ThreadSafe.DateTime)e.CellValue);
                    string NewText = "";
                    if (dt > DateTime.MinValue)
                    {
                        NewText = dt.ToString(AppSettings.Settings.DateFormat);  //"yyyy-MM-dd hh:mm:ss tt"
                    }
                    else
                    {
                        NewText = "";
                    }
                    //"yyyy-MM-dd HH:mm:ss.fff"
                    e.SubItem.Text = NewText;
                }

                //else if (e.CellValue is long)
                //{
                //    long cv = (long)e.CellValue;
                //    if (cv == 0)
                //    {
                //        e.SubItem.Text = "";
                //    }
                //    else
                //    {
                //        if (e.Column.Name == "DwgSize" || e.Column.Name.StartsWith("Mem"))
                //        {
                //            e.SubItem.Text = Shared.ToPrettySize(cv);
                //        }
                //    }
                //}
                else if (e.CellValue is int)
                {
                    int cv = (int)e.CellValue;
                    if (cv == 0)
                    {
                        e.SubItem.Text = "";
                    }
                }
                else if (e.CellValue is ThreadSafe.Integer)
                {
                    int cv = (ThreadSafe.Integer)e.CellValue;
                    if (cv == 0)
                    {
                        e.SubItem.Text = "";
                    }
                }
                else if (e.CellValue is TimeSpan)
                {
                    TimeSpan ts = (TimeSpan)e.CellValue;
                    if ((ts.TotalMilliseconds == 0) || (ts == TimeSpan.MinValue) || (ts == TimeSpan.MaxValue))
                    {
                        e.SubItem.Text = "";
                    }
                    else
                    {
                        e.SubItem.Text = string.Format("{0}d {1}h {2}m {3}s", ts.Duration().Days, ts.Duration().Hours, ts.Duration().Minutes, ts.Duration().Seconds);
                    }
                }
            }
            catch //(Exception ex)
            {
                //LogMsg("Error: " + ex.Message, , MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void FormatURLRow(object Sender, BrightIdeasSoftware.FormatRowEventArgs e)
        {
            using Working w = new Working();
            try
            {
                ClsURLItem url = (ClsURLItem)e.Model;

                // If SPI IsNot Nothing Then
                if (url.Enabled && url.CurErrCount == 0 && !url.UseAsRefinementServer && !url.UseOnlyAsLinkedServer)
                    e.Item.ForeColor = Color.Green;

                else if (url.Enabled && url.CurErrCount == 0 && url.UseAsRefinementServer)
                    e.Item.ForeColor = Color.DarkOrange;

                else if (url.Enabled && url.CurErrCount == 0 && url.UseOnlyAsLinkedServer)
                    e.Item.ForeColor = Color.DarkCyan;

                else if (url.Enabled && url.CurErrCount > 0)
                {
                    e.Item.ForeColor = Color.Black;
                    e.Item.BackColor = Color.Red;
                }
                else if (!url.Enabled)
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

        private void toolStripSplitButtonAdd_ButtonClick(object sender, EventArgs e)
        {
            toolStripSplitButtonAdd.ShowDropDown();
        }

        private void sightHoundAIServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using Working w = new Working();
            ClsURLItem url = new ClsURLItem("", AppSettings.Settings.AIURLList.Count + 1, URLTypeEnum.SightHound_Vehicle);
            if (!AppSettings.Settings.AIURLList.Contains(url))
            {
                this.CurURL = url;
                AppSettings.Settings.AIURLList.Add(url);
                Global_GUI.UpdateFOLV(FOLV_AIServers, AppSettings.Settings.AIURLList, UseSelected: true, SelectObject: this.CurURL, FullRefresh: true);
                AITOOL.UpdateAIURLs();
            }
            else
            {
                MessageBox.Show("Already exists");
            }
            UpdateButtons();
        }

        private void sightHoundPersonAIServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using Working w = new Working();
            ClsURLItem url = new ClsURLItem("", AppSettings.Settings.AIURLList.Count + 1, URLTypeEnum.SightHound_Person);
            if (!AppSettings.Settings.AIURLList.Contains(url))
            {
                this.CurURL = url;
                AppSettings.Settings.AIURLList.Add(url);
                Global_GUI.UpdateFOLV(FOLV_AIServers, AppSettings.Settings.AIURLList, UseSelected: true, SelectObject: this.CurURL, FullRefresh: true);
                AITOOL.UpdateAIURLs();
            }
            else
            {
                MessageBox.Show("Already exists");
            }
            UpdateButtons();
        }

        private void addAmazonFaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using Working w = new Working();
            ClsURLItem url = new ClsURLItem("", AppSettings.Settings.AIURLList.Count + 1, URLTypeEnum.AWSRekognition_Faces);
            if (!AppSettings.Settings.AIURLList.Contains(url))
            {
                this.CurURL = url;
                AppSettings.Settings.AIURLList.Add(url);
                Global_GUI.UpdateFOLV(FOLV_AIServers, AppSettings.Settings.AIURLList, UseSelected: true, SelectObject: this.CurURL, FullRefresh: true);
                AITOOL.UpdateAIURLs();

            }
            else
            {
                MessageBox.Show("Already exists");
            }
            UpdateButtons();
        }

        private void deepstackCustomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using Working w = new Working();
            ClsURLItem url = new ClsURLItem("", AppSettings.Settings.AIURLList.Count + 1, URLTypeEnum.DeepStack_Custom);
            this.CurURL = url;
            AppSettings.Settings.AIURLList.Add(url);
            Global_GUI.UpdateFOLV(FOLV_AIServers, AppSettings.Settings.AIURLList, UseSelected: true, SelectObject: this.CurURL, FullRefresh: true);
            UpdateButtons();
        }

        private void deepstackFacesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using Working w = new Working();
            ClsURLItem url = new ClsURLItem("", AppSettings.Settings.AIURLList.Count + 1, URLTypeEnum.DeepStack_Faces);
            this.CurURL = url;
            AppSettings.Settings.AIURLList.Add(url);
            Global_GUI.UpdateFOLV(FOLV_AIServers, AppSettings.Settings.AIURLList, UseSelected: true, SelectObject: this.CurURL, FullRefresh: true);
            UpdateButtons();

        }

        private void deepstackSceneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using Working w = new Working();
            ClsURLItem url = new ClsURLItem("", AppSettings.Settings.AIURLList.Count + 1, URLTypeEnum.DeepStack_Scene);
            this.CurURL = url;
            AppSettings.Settings.AIURLList.Add(url);
            Global_GUI.UpdateFOLV(FOLV_AIServers, AppSettings.Settings.AIURLList, UseSelected: true, SelectObject: this.CurURL, FullRefresh: true);
            UpdateButtons();

        }

        private void codeProjectAIObjectsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using Working w = new Working();
            ClsURLItem url = new ClsURLItem("", AppSettings.Settings.AIURLList.Count + 1, URLTypeEnum.CodeProject_AI);
            this.CurURL = url;
            AppSettings.Settings.AIURLList.Add(url);
            Global_GUI.UpdateFOLV(FOLV_AIServers, AppSettings.Settings.AIURLList, UseSelected: true, SelectObject: this.CurURL, FullRefresh: true);
            UpdateButtons();
        }

        private void codeProjectAILicensePlateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using Working w = new Working();
            ClsURLItem url = new ClsURLItem("", AppSettings.Settings.AIURLList.Count + 1, URLTypeEnum.CodeProject_AI_Plate);
            this.CurURL = url;
            AppSettings.Settings.AIURLList.Add(url);
            Global_GUI.UpdateFOLV(FOLV_AIServers, AppSettings.Settings.AIURLList, UseSelected: true, SelectObject: this.CurURL, FullRefresh: true);
            UpdateButtons();

        }

        private void codeProjectAICustomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using Working w = new Working();
            ClsURLItem url = new ClsURLItem("", AppSettings.Settings.AIURLList.Count + 1, URLTypeEnum.CodeProject_AI_Custom);
            this.CurURL = url;
            AppSettings.Settings.AIURLList.Add(url);
            Global_GUI.UpdateFOLV(FOLV_AIServers, AppSettings.Settings.AIURLList, UseSelected: true, SelectObject: this.CurURL, FullRefresh: true);
            UpdateButtons();

        }


        private void codeProjectAIIPCAMCombinedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using Working w = new Working();
            ClsURLItem url = new ClsURLItem("", AppSettings.Settings.AIURLList.Count + 1, URLTypeEnum.CodeProject_AI_IPCAM_Combined);
            this.CurURL = url;
            AppSettings.Settings.AIURLList.Add(url);
            Global_GUI.UpdateFOLV(FOLV_AIServers, AppSettings.Settings.AIURLList, UseSelected: true, SelectObject: this.CurURL, FullRefresh: true);
            UpdateButtons();

        }

        private void codeProjectAIIPCAMGeneralToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using Working w = new Working();
            ClsURLItem url = new ClsURLItem("", AppSettings.Settings.AIURLList.Count + 1, URLTypeEnum.CodeProject_AI_IPCAM_General);
            this.CurURL = url;
            AppSettings.Settings.AIURLList.Add(url);
            Global_GUI.UpdateFOLV(FOLV_AIServers, AppSettings.Settings.AIURLList, UseSelected: true, SelectObject: this.CurURL, FullRefresh: true);
            UpdateButtons();

        }

        private void codeProjectAIIPCAMDarkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using Working w = new Working();
            ClsURLItem url = new ClsURLItem("", AppSettings.Settings.AIURLList.Count + 1, URLTypeEnum.CodeProject_AI_IPCAM_Dark);
            this.CurURL = url;
            AppSettings.Settings.AIURLList.Add(url);
            Global_GUI.UpdateFOLV(FOLV_AIServers, AppSettings.Settings.AIURLList, UseSelected: true, SelectObject: this.CurURL, FullRefresh: true);
            UpdateButtons();

        }

        private void codeProjectAIIPCAMAnimalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using Working w = new Working();
            ClsURLItem url = new ClsURLItem("", AppSettings.Settings.AIURLList.Count + 1, URLTypeEnum.CodeProject_AI_IPCAM_Animal);
            this.CurURL = url;
            AppSettings.Settings.AIURLList.Add(url);
            Global_GUI.UpdateFOLV(FOLV_AIServers, AppSettings.Settings.AIURLList, UseSelected: true, SelectObject: this.CurURL, FullRefresh: true);
            UpdateButtons();

        }

        private void codeProjectAIFacesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using Working w = new Working();
            ClsURLItem url = new ClsURLItem("", AppSettings.Settings.AIURLList.Count + 1, URLTypeEnum.CodeProject_AI_Faces);
            this.CurURL = url;
            AppSettings.Settings.AIURLList.Add(url);
            Global_GUI.UpdateFOLV(FOLV_AIServers, AppSettings.Settings.AIURLList, UseSelected: true, SelectObject: this.CurURL, FullRefresh: true);
            UpdateButtons();

        }

        private void codeProjectAISceneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using Working w = new Working();

            ClsURLItem url = new ClsURLItem("", AppSettings.Settings.AIURLList.Count + 1, URLTypeEnum.CodeProject_AI_Scene);
            this.CurURL = url;
            AppSettings.Settings.AIURLList.Add(url);
            Global_GUI.UpdateFOLV(FOLV_AIServers, AppSettings.Settings.AIURLList, UseSelected: true, SelectObject: this.CurURL, FullRefresh: true);
            UpdateButtons();

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (_Working)
                return;

            //this.FOLV_AIServers.RefreshObjects(AppSettings.Settings.AIURLList);
            this.FOLV_AIServers.Refresh();
            //this.FOLV_AIServers.Update();
            //this.FOLV_AIServers.Invalidate();

        }

        private void FOLV_AIServers_DoubleClick(object sender, EventArgs e)
        {
            Edit();
        }

        public static bool _Working = true;
        //create a class that is disposable that simply sets a working bool to false when disposed
        public class Working:IDisposable
        {
            public Working()
            {
                _Working = true;
            }
            public void Dispose()
            {
                _Working = false;
            }
        }

        private void FOLV_AIServers_FormatCell(object sender, BrightIdeasSoftware.FormatCellEventArgs e)
        {
            FormatCell(sender, e);
        }
    }
}
