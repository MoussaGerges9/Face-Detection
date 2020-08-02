using System;
using System.Drawing;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using Emgu.CV;
using Emgu.CV.Structure;

namespace Face_Detection
{
    public partial class Form1 : Form
    {
        private FilterInfoCollection _filter;
        private VideoCaptureDevice _device;
        private static readonly CascadeClassifier cascadeClassifier = new CascadeClassifier("haarcascade_frontalface_alt_tree.xml");

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _filter=new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo device in _filter)
            {
                deviceComboBox.Items.Add(device.Name);
            }

            deviceComboBox.SelectedIndex = 0;
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            _device = new VideoCaptureDevice(_filter[deviceComboBox.SelectedIndex].MonikerString);
            _device.NewFrame += DeviceNewFrame;
            _device.Start();
        }

        private void DeviceNewFrame(object sender, NewFrameEventArgs eventargs)
        {
            Bitmap bitmap = (Bitmap)eventargs.Frame.Clone();
            Image<Bgr, byte> grayImage = new Image<Bgr, byte>(bitmap);
            Rectangle[] rectangles = cascadeClassifier.DetectMultiScale(grayImage, 1.2, 1);

            foreach (Rectangle rectangle in rectangles)
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    using (Pen pen = new Pen(Color.Red, 3))
                    {
                        graphics.DrawRectangle(pen, rectangle);
                    }
                }
            }

            pictureBox1.Image = bitmap;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(_device.IsRunning)
                _device.Stop();
        }
    }
}
