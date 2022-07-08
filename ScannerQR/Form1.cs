using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video; // librarie ce ajută  la accesarea unor surse video
using AForge.Video.DirectShow; // librarie ce contine clase care permit accesarea surselor video folosind interfata DirectShow
using ZXing; //librarie pentru citirea codului QR

namespace ScannerQR
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        FilterInfoCollection filterInfoCollection;
        VideoCaptureDevice captureDevice;

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Identificarea camerei din laptop
            filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo filterInfo in filterInfoCollection)
                cboDevice.Items.Add(filterInfo.Name);
            cboDevice.SelectedIndex = 0;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            captureDevice = new VideoCaptureDevice(filterInfoCollection[cboDevice.SelectedIndex].MonikerString); //adaugarea numelui in comboBox
            // Adaugarea unui nou frame pentru a putea actualiza imaginile facute de camera in PictureBox
            captureDevice.NewFrame += CaptureDevice_NewFrame;
            captureDevice.Start();
            timer1.Start();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            
        }

        private void CaptureDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            pictureBox.Image = (Bitmap)eventArgs.Frame.Clone();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(captureDevice.IsRunning)
                captureDevice.Stop();
        }

        // Folosirea unui timer pentru a citi camera codul QR in fiecare secunda
        private void timer1_Tick(object sender, EventArgs e)
        {
            if(pictureBox.Image != null)
            {
                BarcodeReader barcodeReader = new BarcodeReader(); // folosirea clasei BarecodeReader pentru decodarea codului QR
                Result result = barcodeReader.Decode((Bitmap)pictureBox.Image);
                if(result != null) // daca decodarea a fost facuta, se vor opri timer-ul si camera
                {
                    txtQRCode.Text = result.ToString();
                    timer1.Stop();
                    if (captureDevice.IsRunning)
                        captureDevice.Stop();
                    // accesarea site-ului web din TextBox
                    var uri = txtQRCode.Text;
                    var psi = new System.Diagnostics.ProcessStartInfo();
                    psi.UseShellExecute = true;
                    psi.FileName = uri;
                    System.Diagnostics.Process.Start(psi);
                }
            }
        }
    }
}
