using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using ZXing;
using System.Drawing.Printing;
namespace printlabel
{
    public partial class Form1 : Form
    {
        HttpListener listener;
        private PrintDocument printDocument;
        public Form1()
        {
            InitializeComponent();
            listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:4555/");
            printDocument = new PrintDocument();
            printDocument.PrintPage += PrintDocument_PrintPage;
        }

        private void btn_start_server_Click(object sender, EventArgs e)
        {
            listener.Start();

            listener.BeginGetContext(new AsyncCallback(OnRequestReceive), null);

        }
        private void OnRequestReceive(IAsyncResult ar)
        {
            var context = listener.EndGetContext(ar);
            listener.BeginGetContext(new AsyncCallback(OnRequestReceive), null);

            if (context.Request.HttpMethod == "POST")
            {
                using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
                {
                    string content = reader.ReadToEnd();
                    // 在这里处理POST数据
                    Console.WriteLine(content);
                }
            }

            byte[] buffer = Encoding.UTF8.GetBytes("Request received!");
            context.Response.ContentLength64 = buffer.Length;
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            context.Response.Close();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            listener.Stop();
        }

        private void gen_barcode_print_Click(object sender, EventArgs e)
        {
            // Generate barcode data (e.g., a sample text)
            string barcodeData = "123456789";

            // Create an instance of BarcodeWriter
            BarcodeWriter barcodeWriter = new BarcodeWriter();
            barcodeWriter.Format = BarcodeFormat.CODE_128; // You can choose other barcode formats

            // Generate a barcode bitmap
            Bitmap barcodeBitmap = barcodeWriter.Write(barcodeData);

            // Display the barcode in PictureBox
            pictureBox1.Image = barcodeBitmap;



            // Show the PrintDialog to select the printer
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                // Set the printer to the selected printer
                printDocument.PrinterSettings.PrinterName = printDialog.PrinterSettings.PrinterName;

                // Print the document
                //printDocument.Print();
                printDocument.Print();
            }


        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            // Print the image in the PictureBox
            e.Graphics.DrawImage(pictureBox1.Image, 0, 0);
        }

    }
}
