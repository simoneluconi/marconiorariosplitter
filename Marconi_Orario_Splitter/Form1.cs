using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Marconi_Orario_Splitter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnSelezionaFile_Click(object sender, EventArgs e)
        {
            lstLog.Items.Clear();
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "File PDF (*.pdf)|*.pdf";
            fd.CheckFileExists = true;
            fd.CheckPathExists = true;

            if (fd.ShowDialog() == DialogResult.OK)
            {
                Log("Selezionato file : " + System.IO.Path.GetFileName(fd.FileName));
                string directoryOuput = System.IO.Path.GetDirectoryName(fd.FileName) + "/Marconi_Orario_Splitter";
                GestisciFile(directoryOuput, fd.FileName);
            }
        }

        private void GestisciFile(string path, string filePath)
        {
            string fileName = System.IO.Path.GetFileName(filePath);
            string typeContent = getTypeOfFileContents(filePath);

            if (typeContent.Equals("ORARIO_CLASSI"))
            {
                string directoryOutput = path + "/Classi";
                Directory.CreateDirectory(directoryOutput);
                Bitmap[] images = PdfToImage(filePath);
                Bitmap[] tuttiOrari = ConvertiPagine(images);
                string[] NomiClassi = ListaClassi(filePath);
                SalvaImmagini(directoryOutput, tuttiOrari, NomiClassi);
            }
            else if (typeContent.Equals("ORARIO_DOCENTE"))
            {
                string directoryOutput = path + "/Professori";
                Directory.CreateDirectory(directoryOutput);
                Bitmap[] images = PdfToImage(filePath);
                Bitmap[] tuttiOrari = ConvertiPagine(images);
                string[] NomiProf = ListaProfessori(filePath);
                SalvaImmagini(directoryOutput, tuttiOrari, NomiProf);
            }
            else if (typeContent.Equals("ORARIO_LABORATORI"))
            {
                string directoryOutput = path + "/Laboratori";
                Directory.CreateDirectory(directoryOutput);
                Bitmap[] images = PdfToImage(filePath);
                string[] NomiLab = ListaLaboratori(filePath);
                SalvaImmagini(directoryOutput, images, NomiLab);
            }
            else
            {
                Log("ERRORE! IMPOSSIBILE DETERMINARE IL CONTENUTO DEL FILE");
            }
        }


        private string getTypeOfFileContents(string path)
        {
            foreach (string text in LeggiPdf(path))
            {
                using (var reader = new StringReader(text))
                {
                    for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
                    {
                        if (line.ToLower().Contains("orario classe"))
                        {
                            return "ORARIO_CLASSI";
                        } else if (line.ToLower().Contains("orario docente"))
                        {
                            return "ORARIO_DOCENTE";
                        } else if (line.ToLower().Contains("occupazione aula"))
                        {
                            return "ORARIO_LABORATORI";
                        }
                    }
                }

            }

            return null;
        }
        

        private Bitmap[] ConvertiPagine(Bitmap[] images)
        {
            List<Bitmap> tuttiOrari = new List<Bitmap>();
            Log("Inizio divisione pagine");
            foreach (Bitmap img in images)
            {
                Bitmap[] splitted = SplittaPagina(img);
                tuttiOrari.AddRange(splitted);
            }

            Log("Divisione immagine finita");
            Log(tuttiOrari.Count + " orari trovati");
            return tuttiOrari.ToArray();

        }

        private Bitmap[] SplittaPagina(Image img)
        {
            Bitmap bmpImage = img as Bitmap;
            int offset = 120;
            Rectangle cropArea1 = new Rectangle(0, offset, img.Width, (img.Height / 2) - offset);
            Bitmap[] orari = new Bitmap[2];
            orari[0] = bmpImage.Clone(cropArea1, bmpImage.PixelFormat);
            Rectangle cropArea2 = new Rectangle(0, img.Height / 2, img.Width, (img.Height / 2) - offset);
            orari[1] = bmpImage.Clone(cropArea2, bmpImage.PixelFormat);
            return orari;
        }


        private Bitmap[] PdfToImage(string pdfPath)
        {
            Log("Inizio conversione PDF -> Immagine");
            List<Bitmap> images = new List<Bitmap>();

            SautinSoft.PdfFocus f = new SautinSoft.PdfFocus();

            string jpegDir = System.IO.Path.GetDirectoryName(pdfPath);

            f.OpenPdf(pdfPath);

            if (f.PageCount > 0)
            {
                f.ImageOptions.ImageFormat = ImageFormat.Jpeg;
                f.ImageOptions.Dpi = 300;
                f.ImageOptions.JpegQuality = 100;

                for (int page = 1; page <= f.PageCount; page++)
                {
                    byte[] imageData = f.ToImage(page);
                    var ms = new MemoryStream(imageData);
                    images.Add(new Bitmap(ms));
                    Log("Convertita immagine n°" + page);
                }
            }

            Log("Finita conversione PDF");

            return images.ToArray();
        }


        private string[] ListaClassi(string path)
        {
            Log("Cerco nomi delle classi");
            List<string> orarioClassi = new List<string>();

            foreach (string text in LeggiPdf(path))
            {
                using (var reader = new StringReader(text))
                {
                    for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
                    {
                        if (line.ToLower().Contains("orario classe"))
                        {
                            int index = line.IndexOf("^");
                            string anno = line.Substring(index - 1, 1);
                            string sezione = line.Substring(index + 1, 1);
                            string classe = "Classe_" + anno + sezione;
                            Log("Trovata classe: " + classe);
                            orarioClassi.Add(classe);
                        }
                    }
                }

            }

            Log("Finito di cercare i nomi delle classi");
            Log(orarioClassi.Count + " classi trovate");
            return orarioClassi.ToArray();

        }


        private string[] ListaProfessori(string path)
        {
            Log("Cerco nomi dei professori");
            List<string> orarioProf = new List<string>();

            foreach (string text in LeggiPdf(path))
            {
                using (var reader = new StringReader(text))
                {
                    for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
                    {
                        if (line.ToLower().Contains("orario docente"))
                        {
                            int index = line.IndexOf("-");
                            string nome = line.Substring(index +1, line.IndexOf("(") - index - 1).Trim();
                            nome = nome.Replace("/", "-");
                            nome = nome.Replace(".", "");
                            Log("Trovata nome: " + nome);
                            orarioProf.Add(nome);
                        }
                    }
                }

            }

            Log("Finito di cercare i nomi dei professori");
            Log(orarioProf.Count + " professori trovate");
            return orarioProf.ToArray();

        }

        private string[] ListaLaboratori(string path)
        {
            Log("Cerco nomi dei laboratori");
            List<string> orarioLaboratori = new List<string>();

            foreach (string text in LeggiPdf(path))
            {
                using (var reader = new StringReader(text))
                {
                    for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
                    {
                        if (line.ToLower().Contains("occupazione aula"))
                        {
                            int index = line.IndexOf("-");
                            string nome = line.Substring(index + 1, line.IndexOf("(") - index - 1).Trim();
                            nome = nome.Replace("/", "-");
                            nome = nome.Replace(".", "");
                            Log("Trovata aula: " + nome);
                            orarioLaboratori.Add(nome);
                        }
                    }
                }

            }

            Log("Finito di cercare i nomi dei laboratori");
            Log(orarioLaboratori.Count + " laboratori trovate");
            return orarioLaboratori.ToArray();

        }

        private string[] LeggiPdf(string path)
        {
            List<string> txtPages = new List<string>();
            var pdfReader = new PdfReader(path); //other filestream etc

            for (int i = 1; i <= pdfReader.NumberOfPages; i++)
            {
                string text = PdfTextExtractor.GetTextFromPage(pdfReader, i, new SimpleTextExtractionStrategy());
                txtPages.Add(text);
            }

            return txtPages.ToArray();
        }

        private void SalvaImmagini(string path, Bitmap[] orari, string[] desc)
        {
            Log("Inizio salvataggio immagini");
            for (int i = 0; i < desc.Length; i++)
            {
                string filename = path +"/" + desc[i] + ".jpg";
                Log("Salvo immagine: " + filename);
                orari[i].Save(filename, ImageFormat.Jpeg);
            }

            Log("Finito salvataggio classi");

            if (orari.Length != desc.Length)
                Log("ATTEZIONE: Orari: " + orari.Length + " - Desc: " + desc.Length);

            Log("Apro percorso: " + path);
            Process.Start(path);


        }

        private void Log(string s)
        {
            lstLog.Items.Add(s);
            lstLog.SelectedIndex = lstLog.Items.Count - 1;
        }

    }
}
