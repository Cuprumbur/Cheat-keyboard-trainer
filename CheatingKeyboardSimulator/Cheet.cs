using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tesseract;
using Yandex.Speller.Api;
using Yandex.Speller.Api.DataContract;

namespace CheatingKeyboardSimulator
{
    class Cheat
    {
        public static string RecognizePicture(Bitmap img, Language language)
        {
            try
            {
                using (var ocr = new TesseractEngine("./tessdata", language.ToString(), EngineMode.TesseractAndCube))
                {
                    var page = ocr.Process(img);
                    var resStr = page.GetText();
                    return resStr;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine( ex.Message);
            return null;

            }
        }


        public static SpellResult CheckSpelling(string text, Language language, Options options = Options.Default, TextFormat textFormat = TextFormat.Plain)
        {
            Lang lang;
            switch (language)
            {
                case Language.rus: lang = Lang.Ru; break;
                case Language.eng: lang = Lang.En; break;
                default:throw new ArgumentException("Язык только русский или английский", "language");
            }
            
            IYandexSpeller speller = new YandexSpeller();
            SpellResult res = speller.CheckText(text, lang, options, textFormat);
            return res;
        }
        public static Bitmap GetBitmap()
        {
            Bitmap bitmap=null;
            using (var form = new FormScreenShot())
            {
                var res = form.ShowDialog();
                if (res == DialogResult.OK)
                {
                    bitmap = form.GetBitmap;
                }
            }
            return bitmap;
        }

        public static void EmulateKeyboardInput(string text,int startPositon)
        {
            try
            {
                Thread.Sleep(3000);
                
                for (int i = startPositon; i < text.Length; i++)
                {
                    try
                    {
                        SendKeys.Send(text[i].ToString());
                        Thread.Sleep(45);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            catch (Exception ex )
            {
                Console.WriteLine("EmulateKeyboardInput "+  ex.Message);
            }
        }
    }

    

    enum Language
    {
        rus,
        eng
    }
}
