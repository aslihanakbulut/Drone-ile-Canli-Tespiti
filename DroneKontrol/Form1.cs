using GMap.NET.MapProviders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;


namespace DroneKontrol
{

    public partial class Form1 : Form
    {
        private SerialPort serialPort; // port nesnesi olustur
        private string okunanVeri;
        public Form1()
        {
            InitializeComponent();
            serialPort = new SerialPort();
            serialPort.DataReceived += SerialPort_DataReceived; // Veri alindiginda cagrilacak olay

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] port = SerialPort.GetPortNames();

            foreach (string p in port)
            {
                cbox_Port.Items.Add(p);
            }

            if (port.Length > 0)
            {
                //   comboBox1.SelectedIndex = 0;    // form acildiginda secili olarak gelsin
            }
            cbox_brate.Items.Add("9600");
            cbox_brate.Items.Add("38400");
            cbox_brate.Items.Add("115200");


            serialPort.DataReceived += new SerialDataReceivedEventHandler(SerialPort_DataReceived);
           // konum.DragButton == MouseButtons.Left;
           //
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialPort.IsOpen == true)
                serialPort.Close();

           
        }

     

        private void buton_baglan_Click(object sender, EventArgs e)
        {

            if (serialPort.IsOpen)
            {
                // baglanti aciksa kapat
                serialPort.Close();
                buton_baglan.Text = "BAĞLAN";
                cbox_Port.Enabled = true;
                cbox_brate.Enabled = true;
                buton_baglan.BackColor = Color.Green;
                baglanti_durum.Text = "BAĞLANTI YOK";

                textBox1.Text = " ";
                textBox2.Text = " ";
                textBox3.Text = " ";
                textBox4.Text = " ";

            }
            else
            {
                // secilen portu ac
                string selectedPort = cbox_Port.SelectedItem.ToString();
                int baudRate = int.Parse(cbox_brate.SelectedItem.ToString());
                serialPort.PortName = selectedPort;
                serialPort.BaudRate = baudRate;

                try
                {
                    serialPort.Open();
                    buton_baglan.Text = "BAĞLANTIYI KES ";
                    cbox_Port.Enabled = false;
                    cbox_brate.Enabled = false;
                    buton_baglan.BackColor = Color.Red;
                    baglanti_durum.Text = "BAĞLANDI";

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
        }
   
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //Veri alındığında bu olay tetiklenir
            okunanVeri = serialPort.ReadLine();
            this.Invoke(new EventHandler(displaydata));
        }

        private void displaydata(object sender, EventArgs e)
        {
             string[] GosterilecekDeger = okunanVeri.Split('/');
            
             textBox1.Text = GosterilecekDeger[0];
             textBox2.Text = GosterilecekDeger[1];
             textBox3.Text = GosterilecekDeger[2];
             textBox4.Text = GosterilecekDeger[3];
    

            konum.MapProvider = GMapProviders.GoogleMap;
            double enlem = Convert.ToDouble(textBox3.Text);
            double boylam = Convert.ToDouble(textBox4.Text);
            konum.Position = new GMap.NET.PointLatLng(enlem, boylam);
            konum.MinZoom = 10;
            konum.MaxZoom = 200;
            konum.Zoom = 10;


        }
    }


}
