using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security;
using System.Windows.Forms;
using MobileResizer.Helpers;

namespace MobileResizer
{
    public sealed partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            AllowDrop = true;
            DragEnter += Form1_DragEnter;
            DragDrop += Form1_DragDrop;
            InitView();
        }

        private void InitView()
        {
            panel1.BackColor = Color.FromArgb(255, 75, 75, 75);
            panel2.BackColor = Color.FromArgb(255, 75, 75, 75);
            for (var i = 0; i < checkedListBox.Items.Count; i++)
            {
                checkedListBox.SetItemChecked(i, true);
            }

            openFileDialog1.Filter =
                "Images (*.PNG;*.BMP;*.JPG;*.GIF)|*.PNG;*.BMP;*.JPG;*.GIF|" +
                "All files (*.*)|*.*";

            openFileDialog1.Multiselect = true;
            progressBar1.Visible = false;
            progressBar1.Maximum = 100;
            progressBar1.Step = 1;
        }

        void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            ResizeImage(files);
        }

        private void ResetProgressBar()
        {
            labelP.Visible = true;
            progressBar1.Visible = true;
            progressBar1.Value = 0;
            labelP.Text = progressBar1.Value + @"%";
        }

        private void ResizeImage(string[] files)
        {
            ResetProgressBar();
            foreach (var file in files)
            {
                Image image = Image.FromFile(file);
                ImageFormat format = image.RawFormat;
                ImageCodecInfo codec = ImageCodecInfo.GetImageDecoders().First(c => c.FormatID == format.Guid);

                string fileName = Path.GetFileName(file);
                if (!checkBoxKeepFileName.Checked)
                {
                    fileName = fileName.ToLowerInvariant().Replace(" ", "_");
                    fileName = fileName.Replace("-", "_");
                }

                var folderPath = Path.GetDirectoryName(file);
                if (folderPath != null)
                {
                    foreach (var checkedItem in checkedListBox.CheckedItems)
                    {
                        var drawable = checkedItem.ToString();
                        if (drawable.Contains("@"))
                        {
                            drawable = "iOS";
                        }
                        var newPath = Path.Combine(folderPath, drawable);
                        CreateFolder(newPath);
                        float[] rateXx = { 1f / 12f * 6, 1f / 12 * 8, 1f, 1f / 12 * 16, 1f / 12 * 8, 1f };
                        float[] rateXxx = { 1f / 16 * 6, 1f / 16 * 8, 1f / 16 * 12, 1f, 1f / 16 * 8, 1f / 16 * 12 };
                        var rates = radioButtonXX.Checked ? rateXx : rateXxx;

                        string tempFileName = fileName;
                        Image resizeImage = image;
                        switch (checkedItem.ToString())
                        {
                            case "drawable-hdpi":
                                resizeImage = ImageHelper.Resize(image, (int)(image.Width * rates[0]), Int32.MaxValue, false);
                                break;
                            case "drawable-xhdpi":
                                resizeImage = ImageHelper.Resize(image, (int)(image.Width * rates[1]), Int32.MaxValue, false);
                                break;
                            case "drawable-xxhdpi":
                                resizeImage = ImageHelper.Resize(image, (int)(image.Width * rates[2]), Int32.MaxValue, false);
                                break;
                            case "drawable-xxxhdpi":
                                resizeImage = ImageHelper.Resize(image, (int)(image.Width * rates[3]), Int32.MaxValue, false);
                                break;
                            case "@2x":
                                resizeImage = ImageHelper.Resize(image, (int)(image.Width * rates[4]), Int32.MaxValue, false);
                                tempFileName = tempFileName.Replace(".", "@2x.");
                                break;
                            case "@3x":
                                resizeImage = ImageHelper.Resize(image, (int)(image.Width * rates[5]), Int32.MaxValue, false);
                                tempFileName = tempFileName.Replace(".", "@3x.");
                                break;
                        }
                        ImageHelper.Save(Path.Combine(folderPath, drawable, tempFileName), resizeImage, codec);
                    }
                }
                while (progressBar1.Value < 100)
                {
                    progressBar1.Value += 1;
                    labelP.Text = progressBar1.Value + @"%";
                }
            }
        }

        private void CreateFolder(string path)
        {
            Directory.CreateDirectory(path);
        }


        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void OnUploadFiles(object sender, EventArgs e)
        {
            DialogResult dr = this.openFileDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                ResizeImage(openFileDialog1.FileNames);
            }
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
