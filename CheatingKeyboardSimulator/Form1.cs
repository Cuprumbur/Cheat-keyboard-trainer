using CheatingKeyboardSimulator.ServiceReference1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tesseract;

namespace CheatingKeyboardSimulator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var path = @"C:\Users\ochir\Desktop\Снимок2.png";
            textBox1.Text = GetResult(path, "rus");
        }

        private string GetResult(string pathFile, string laguage)
        {
            SpellServiceSoapClient client = new SpellServiceSoapClient("SpellServiceSoap");
            
            var img = new Bitmap(pathFile);
            using (var ocr = new TesseractEngine("./tessdata", laguage, EngineMode.Default))
            {
                var page = ocr.Process(img);
                var resStr = page.GetText();
                var res = client.checkText(resStr,"ru",6,"plain");
                foreach (var item in res)
                {
                    resStr.Replace(item.word, item.s.ToString());
                }
                return resStr;
            }
        }
    }

}

