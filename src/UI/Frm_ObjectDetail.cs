using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Forms;


using static AITool.AITOOL;

namespace AITool
{
    public partial class Frm_ObjectDetail:Form
    {
        public List<ClsPrediction> PredictionObjectDetailsList = null;
        public ClsPrediction CurPred = null;
        public string ImageFileName = "";
        // this tracks the transformation applied to the PictureBox's Graphics
        private Matrix transform = new Matrix();
        private float m_dZoomscale = 1.0f;
        public const float s_dScrollValue = 0.1f;
        private ClsImageQueueItem OriginalBMP = null;
        public Frm_ObjectDetail()
        {
            this.InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {


        }

        private void Frm_ObjectDetail_Load(object sender, EventArgs e)
        {
            Global_GUI.RestoreWindowState(this);


            this.Show();

            try
            {
                Global_GUI.ConfigureFOLV(this.folv_ObjectDetail, typeof(ClsPrediction), null, null);

                Global_GUI.UpdateFOLV(this.folv_ObjectDetail, this.PredictionObjectDetailsList);

                if (this.PredictionObjectDetailsList.Count > 0)
                    CurPred = this.PredictionObjectDetailsList[0];

                UpdateImage();
            }
            catch (Exception)
            {

                throw;
            }

        }

        private string _lastimage = "";
        private void UpdateImage()
        {
            if (this.ImageFileName == _lastimage)
                return;

            if (!String.IsNullOrEmpty(this.ImageFileName) && this.ImageFileName.Contains("\\") && File.Exists(this.ImageFileName))
            {
                OriginalBMP = new ClsImageQueueItem(this.ImageFileName, 0);
                this.pictureBox1.Image = OriginalBMP.ToImage(); //load actual image as background, so that an overlay can be added as the image
                _lastimage = this.ImageFileName;
            }

        }

        private void Frm_ObjectDetail_FormClosing(object sender, FormClosingEventArgs e)
        {
            Global_GUI.SaveWindowState(this);
        }

        private void folv_ObjectDetail_FormatRow(object sender, BrightIdeasSoftware.FormatRowEventArgs e)
        {
            this.FormatRow(sender, e);
        }

        private async void FormatRow(object Sender, BrightIdeasSoftware.FormatRowEventArgs e)
        {
            try
            {
                ClsPrediction OP = (ClsPrediction)e.Model;

                // If SPI IsNot Nothing Then
                if (OP.Result == ResultType.Relevant)
                    e.Item.ForeColor = Color.DarkGreen;   //AppSettings.Settings.RectRelevantColor;
                else if (OP.Result == ResultType.DynamicMasked || OP.Result == ResultType.ImageMasked || OP.Result == ResultType.StaticMasked)
                    e.Item.ForeColor = AppSettings.Settings.RectMaskedColor;
                else if (OP.Result == ResultType.Error)
                {
                    e.Item.ForeColor = Color.Black;
                    e.Item.BackColor = Color.Red;
                }
                else
                    e.Item.ForeColor = AppSettings.Settings.RectIrrelevantColor;
            }



            catch (Exception)
            {
            }
            finally
            {
            }
        }

        private void createStaticMasksToolStripMenuItem_Click(object sender, EventArgs e)
        {

            int cnt = 0;
            if (this.folv_ObjectDetail.SelectedObjects != null && this.folv_ObjectDetail.SelectedObjects.Count > 0)
            {
                foreach (ClsPrediction CP in this.folv_ObjectDetail.SelectedObjects)
                {
                    if (string.IsNullOrEmpty(CP.Camera))
                        Log("Error: Can only add newer history prediction items that include cameraname, imagewidth, imageheight.");
                    else
                    {
                        ObjectPosition OP = new ObjectPosition(CP.XMin, CP.XMax, CP.YMin, CP.YMax, CP.Label, CP.ImageHeight, CP.ImageWidth, CP.Camera, CP.Filename);
                        Camera cam = GetCamera(CP.Camera);
                        cam.maskManager.CreateDynamicMask(OP, true);
                        cnt++;
                    }

                }
            }
            Log($"Added/updated {cnt} masks.");
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {

            if (this.pictureBox1.Image.IsNull())
                return;

            if (this.folv_ObjectDetail.SelectedObjects != null && this.folv_ObjectDetail.SelectedObjects.Count > 0) //if checkbox button is enabled
            {

                try
                {

                    foreach (ClsPrediction pred in this.folv_ObjectDetail.SelectedObjects)
                    {
                        if (pred != null)
                        {
                            AITOOL.DrawAnnotation(e.Graphics,
                                                          pred,
                                                          this.pictureBox1.Image.Width,
                                                          this.pictureBox1.Image.Height,
                                                          this.pictureBox1.Width,
                                                          this.pictureBox1.Height);

                            pictureBox2.Image = AITOOL.CropImage(OriginalBMP, pred.GetRectangle());

                        }

                    }



                }
                catch (Exception ex)
                {

                }

            }

        }

        //        private void showObject(PaintEventArgs e, ClsPrediction pred)
        //        {
        //            try
        //            {
        //                if ((this.pictureBox1 != null) && (this.pictureBox1.Image != null))
        //                {

        //                    e.Graphics.Transform = transform;

        //                    System.Drawing.Color color = new System.Drawing.Color();
        //                    int BorderWidth = AppSettings.Settings.RectBorderWidth
        //;

        //                    if (pred.Result == ResultType.Relevant)
        //                    {
        //                        color = System.Drawing.Color.FromArgb(AppSettings.Settings.RectRelevantColorAlpha, AppSettings.Settings.RectRelevantColor);
        //                    }
        //                    else if (pred.Result == ResultType.DynamicMasked || pred.Result == ResultType.ImageMasked || pred.Result == ResultType.StaticMasked)
        //                    {
        //                        color = System.Drawing.Color.FromArgb(AppSettings.Settings.RectMaskedColorAlpha, AppSettings.Settings.RectMaskedColor);
        //                    }
        //                    else
        //                    {
        //                        color = System.Drawing.Color.FromArgb(AppSettings.Settings.RectIrrelevantColorAlpha, AppSettings.Settings.RectIrrelevantColor);
        //                    }

        //                    //1. get the padding between the image and the picturebox border

        //                    //get dimensions of the image and the picturebox
        //                    double imgWidth = this.pictureBox1.Image.Width;
        //                    double imgHeight = this.pictureBox1.Image.Height;
        //                    double boxWidth = this.pictureBox1.Width;
        //                    double boxHeight = this.pictureBox1.Height;
        //                    double clnWidth = this.pictureBox1.ClientSize.Width;
        //                    double clnHeight = this.pictureBox1.ClientSize.Height;
        //                    double rctWidth = this.pictureBox1.ClientRectangle.Width;
        //                    double rctHeight = this.pictureBox1.ClientRectangle.Height;

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
        //                    double xmin = (scale * pred.XMin) + absX;
        //                    double xmax = (scale * pred.XMax) + absX;
        //                    double ymin = (scale * pred.YMin) + absY;
        //                    double ymax = (scale * pred.YMax) + absY;

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

        //                    //pictureBox2.Image = AITOOL.CropImage(OriginalBMP, pred.GetRectangle());

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



        //                    System.Drawing.SizeF TextSize = e.Graphics.MeasureString(pred.ToString(), new Font(AppSettings.Settings.RectDetectionTextFont, AppSettings.Settings.RectDetectionTextSize)); //finds size of text to draw the background rectangle


        //                    //object name text below rectangle

        //                    double x = xmin - halfbrd;
        //                    double y = ymax + halfbrd;

        //                    //just for debugging:
        //                    //int timgWidth = (int)imgWidth;
        //                    //int tboxWidth = (int)boxWidth;
        //                    //int tsclWidth = (int)sclWidth;

        //                    //int timgHeight = (int)imgHeight;
        //                    //int tboxHeight = (int)boxHeight;
        //                    //int tsclHeight = (int)sclHeight;


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
        //                    if (AppSettings.Settings.RectDetectionTextBackColor != System.Drawing.Color.Gainsboro)
        //                        brush = new SolidBrush(AppSettings.Settings.RectDetectionTextBackColor);

        //                    Brush forecolor = Brushes.Black;
        //                    if (AppSettings.Settings.RectDetectionTextForeColor != System.Drawing.Color.Gainsboro)
        //                        forecolor = new SolidBrush(AppSettings.Settings.RectDetectionTextForeColor);

        //                    e.Graphics.FillRectangle(brush,
        //                                             x.ToInt(),
        //                                             y.ToInt(),
        //                                             TextSize.Width,
        //                                             TextSize.Height); //draw grey background rectangle for detection text

        //                    e.Graphics.DrawString(pred.ToString(), new Font(AppSettings.Settings.RectDetectionTextFont, AppSettings.Settings.RectDetectionTextSize), forecolor, rect); //draw detection text


        //                }

        //            }
        //            catch (Exception ex)
        //            {

        //                Log("Error: " + ex.Msg());
        //            }
        //        }

        private void folv_ObjectDetail_SelectionChanged(object sender, EventArgs e)
        {
            if (folv_ObjectDetail.SelectedObject != null)
            {
                CurPred = folv_ObjectDetail.SelectedObject as ClsPrediction;
                this.ImageFileName = CurPred.Filename;
                UpdateImage();
            }
            this.pictureBox1.Refresh();
        }
        //protected override void OnMouseWheel(MouseEventArgs mea)
        //{
        //    pictureBox1.Focus();
        //    if (pictureBox1.Focused == true && mea.Delta != 0)
        //    {
        //        // Map the Form-centric mouse location to the PictureBox client coordinate system
        //        Point pictureBoxPoint = pictureBox1.PointToClient(this.PointToScreen(mea.Location));
        //        ZoomScroll(pictureBoxPoint, mea.Delta > 0);
        //    }
        //}

        //private void ZoomScroll(Point location, bool zoomIn)
        //{
        //    // Figure out what the new scale will be. Ensure the scale factor remains between
        //    // 1% and 1000%
        //    float newScale = Math.Min(Math.Max(m_dZoomscale + (zoomIn ? s_dScrollValue : -s_dScrollValue), 0.1f), 10);

        //    if (newScale != m_dZoomscale)
        //    {
        //        float adjust = newScale / m_dZoomscale;
        //        m_dZoomscale = newScale;

        //        // Translate mouse point to origin
        //        transform.Translate(-location.X, -location.Y, MatrixOrder.Append);

        //        // Scale view
        //        transform.Scale(adjust, adjust, MatrixOrder.Append);

        //        // Translate origin back to original mouse point.
        //        transform.Translate(location.X, location.Y, MatrixOrder.Append);
        //        Size newSize = new Size((int)(OriginalBMP.Width * m_dZoomscale), (int)(OriginalBMP.Height * m_dZoomscale));
        //        Bitmap bmp = new Bitmap(OriginalBMP, newSize);
        //        this.pictureBox1.Image = bmp; //load actual image as background, so that an overlay can be added as the image

        //        pictureBox1.Invalidate();
        //        pictureBox1.Refresh();
        //    }
        //}

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void folv_ObjectDetail_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        //FUNCTION FOR MOUSE SCROL ZOOM-IN
        //private void ZoomScroll(Point location, bool zoomIn)
        //{
        //    // make zoom-point (cursor location) our origin
        //    transform.Translate(-location.X, -location.Y);

        //    // perform zoom (at origin)
        //    if (zoomIn)
        //        transform.Scale(s_dScrollValue, s_dScrollValue);
        //    else
        //        transform.Scale(1 / s_dScrollValue, 1 / s_dScrollValue);

        //    // translate origin back to cursor
        //    transform.Translate(location.X, location.Y);
        //    this.pictureBox1.Invalidate();
        //    this.pictureBox1.Refresh();
        //    //m_Picturebox_Canvas.Invalidate();
        //}
    }
}
