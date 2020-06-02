using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.CompilerServices;

namespace Draco_Encoder_Decoder
{
    public partial class Form1 : Form
    {

        List<string> toEncodeFilePaths = null;
        List<string> toDecodeFilePaths = null;

        public Form1()
        {
            InitializeComponent();
            rbOBJ.Checked = true;
        }

        //void TempMethod()
        //{
        //    string sourcepath = @"D:\@TLL_Project\RnD\Draco\Draco-Repository\master\draco-master\testdata\bun_zipper.ply";
        //    string resultpath = @"D:\@TLL_Project\RnD\Draco\Draco-Repository\master\draco-master\testdata\_bunny.drc";
        //    Encode(sourcepath, resultpath, 10, 14);

        //    Thread.Sleep(1000);
        //    Decode(resultpath, "D:/my.ply");
        //}

        void Encode(string InputFilepath,string OutputFilepath,int cl,int qp)
        {           
            var argument = $"draco_encoder -i {InputFilepath} -o {OutputFilepath} -cl {cl} -qp {qp}";            

            var p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.Arguments = "/C " + argument;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.Start();            
        }
        void Decode(string InputFilepath, string OutputFilepath)
        {
            var argument = $"draco_decoder -i {InputFilepath} -o {OutputFilepath}";

            var p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.Arguments = "/C " + argument;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.Start();
        }
        private void btnCompress_Click(object sender, EventArgs e)
        {
            //Compression Level
            int cl = trkbarCompressionLevel.Value;

            //Compression Quality [Quantization Points]
            int qp = trkBarCompressionQuality.Value;

            if (toEncodeFilePaths == null)
            {
                MessageBox.Show("Please Select files to Encode", "No Files Selected", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (string.IsNullOrWhiteSpace(txtbxEncodingOutputFolder.Text))
            {
                MessageBox.Show("Please Select a output destination for encoded files.", "Output location Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var outputFolder = txtbxEncodingOutputFolder.Text;

            foreach (var filepath in toEncodeFilePaths)
            {
                var filename = Path.GetFileNameWithoutExtension(filepath);

                var outputFilepath = outputFolder + "\\" + filename + ".drc";

                Encode(filepath, outputFilepath, cl, qp);
            }

            var p = new Process();
            p.StartInfo.Arguments = outputFolder;
            p.StartInfo.FileName = "explorer.exe";
            p.Start();

            MessageBox.Show("Encoding Process Completed");
        }

        private void btnFilesToEncode_Click(object sender, EventArgs e)
        {
            //Filesnames & Path List 
            List<string> filePaths = new List<string>();


            //Initializing OpenFileDialog
            var ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = "3D files (*.obj;*.ply)|*.OBJ;*.PLY"; //Filter for OBJ and PLY files [Draco only supports OBJ and PLY formats]
            ofd.Title = "Select 3D files to Encode";

            
            //Showing Dialogbox and storing its result in dr;
            DialogResult dr = ofd.ShowDialog();


            //Storing selected file names in FilePaths List
            if(dr == DialogResult.OK)
            {
                filePaths = new List<string>(ofd.FileNames);
            }

            //Text to be shown on TextBox txtbxFilesToEncode 
            string fileNames = "";

            foreach (var filepath in filePaths) 
            {
                fileNames = fileNames +"\""+ Path.GetFileName(filepath)+"\" ";
            }

            //Setting Text containing all the files selected on TextBox 
            txtbxFilesToEncode.Text = fileNames;

            toEncodeFilePaths = filePaths;
        }

        private void btnEncodingOutputFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            DialogResult dr = fbd.ShowDialog();

            if (dr == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                txtbxEncodingOutputFolder.Text = fbd.SelectedPath;
            }
        }

        private void btnFilesToDecode_Click(object sender, EventArgs e)
        {
            //Filesnames & Path List 
            List<string> filePaths = new List<string>();


            //Initializing OpenFileDialog
            var ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = "Drc files (*.drc)|*.drc"; //Filter for DRC files
            ofd.Title = "Select 3D files to Decode";


            //Showing Dialogbox and storing its result in dr;
            DialogResult dr = ofd.ShowDialog();


            //Storing selected file names in FilePaths List
            if (dr == DialogResult.OK)
            {
                filePaths = new List<string>(ofd.FileNames);
            }

            //Text to be shown on TextBox txtbxFilesToEncode 
            string fileNames = "";

            foreach (var filepath in filePaths)
            {
                fileNames = fileNames + "\"" + Path.GetFileName(filepath) + "\" ";
            }

            //Setting Text containing all the files selected on TextBox 
            txtbxFilesToDecode.Text = fileNames;

            toDecodeFilePaths = filePaths;
        }

        private void btnDecodingOutputFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            DialogResult dr = fbd.ShowDialog();

            if (dr == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                txtbxDecodingOutputFolder.Text = fbd.SelectedPath;
            }
        }

        private void btnDecompress_Click(object sender, EventArgs e)
        {
            if (toDecodeFilePaths == null)
            {
                MessageBox.Show("Please Select files to Decode", "No Files Selected", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (string.IsNullOrWhiteSpace(txtbxDecodingOutputFolder.Text))
            {
                MessageBox.Show("Please Select a output destination for Decoded files.", "Output location Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var outputFolder = txtbxDecodingOutputFolder.Text;
            string extension;

            if (rbOBJ.Checked) extension = ".obj"; else extension = ".ply";

            foreach (var filepath in toDecodeFilePaths)
            {
                var filename = Path.GetFileNameWithoutExtension(filepath);

                var outputFilepath = outputFolder + "\\" + filename+extension;

                Decode(filepath, outputFilepath);
            }

            

            var p = new Process();
            p.StartInfo.Arguments = outputFolder;
            p.StartInfo.FileName = "explorer.exe";
            p.Start();

            MessageBox.Show("Decoding Process Done");

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
