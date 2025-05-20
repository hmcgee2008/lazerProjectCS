using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using AForge.Video;
using AForge.Video.DirectShow;
using Emgu.CV;
using Emgu.CV.Structure;


namespace lazerProjectCS
{
    public partial class Webcam : Form
    {
        FilterInfoCollection filterInfoCollection;
        VideoCaptureDevice videoCaptureDevice;
        public Stopwatch watch {  get; set; }
        public Webcam()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            filterInfoCollection=new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach(FilterInfo filterInfo in filterInfoCollection)
                cboCamera.Items.Add(filterInfo.Name);
            cboCamera.SelectedIndex = 0;
            videoCaptureDevice = new VideoCaptureDevice();
            watch = Stopwatch.StartNew();
            serialPort1.Open();
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            WriteToPort(new Point(e.X, e.Y));
        }

        public void WriteToPort(Point coordinates)
        {
            if (watch.ElapsedMilliseconds > 15)
            {
                watch = Stopwatch.StartNew();

                serialPort1.Write(String.Format("x{0}Y{1}",
                (coordinates.X / (Size.Width / 180)),
                (coordinates.Y / (Size.Width / 180))));

            }
            
        }

        private void pic_Click(object sender, EventArgs e)
        {

        }

        

        private void btnStart_Click(object sender, EventArgs e)
        {
            videoCaptureDevice=new VideoCaptureDevice(filterInfoCollection[cboCamera.SelectedIndex].MonikerString);
            videoCaptureDevice.NewFrame += VideoCaptureDevice_NewFrame;
            videoCaptureDevice.Start();
        }

        static readonly CascadeClassifier cascadeClassifier = new CascadeClassifier("cascade_226.xml");
        
        private void VideoCaptureDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
            Image<Rgb, byte> grayImage = new Image<Rgb, byte>(bitmap);
            Rectangle[] rectangles = cascadeClassifier.DetectMultiScale(grayImage, 1.2, 1);
            foreach(Rectangle rectangle in rectangles)
            {
                using(Graphics graphics=Graphics.FromImage(bitmap))
                {
                    using (Pen pen = new Pen(Color.Red, 1))
                    {
                        graphics.DrawRectangle(pen, rectangle);

                    }
                }
            }
            pic.Image = bitmap;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (videoCaptureDevice.IsRunning==true)
                videoCaptureDevice.Stop();
        }
    }
}
