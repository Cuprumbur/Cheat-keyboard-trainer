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
                EngineMode engineMode;
                switch (language)
                {
                    case Language.rus: engineMode = EngineMode.TesseractAndCube; break;
                    case Language.eng: engineMode = EngineMode.Default; break;
                    default: throw new ArgumentException("Язык только русский или английский", "language");
                }

                using (var ocr = new TesseractEngine("./tessdata", language.ToString(), engineMode))
                {
                    var page = ocr.Process(img);
                    var resStr = page.GetText();
                    return resStr;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
                default: throw new ArgumentException("Язык только русский или английский", "language");
            }

            IYandexSpeller speller = new YandexSpeller();
            SpellResult res = speller.CheckText(text, lang, options, textFormat);
            return res;
        }
        public static Bitmap GetBitmap()
        {
            Bitmap bitmap = null;
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

        public static void EmulateKeyboardInput(string text, int startPositon, int delay)
        {
            try
            {
                float k = (float)999 / (float)60 - (float)0.18;

                Thread.Sleep(1500);
                var start = DateTime.Now;
                DateTime dateTime = DateTime.Now;
                for (int i = startPositon; i < text.Length; i++)
                {
                    try
                    {
                        SendKeys.Send(text[i].ToString());
                        
                            float sec = (float)(DateTime.Now - start).TotalSeconds; ;
                            float d = (float)i / sec;
                            float kn= (float)d * (float)60;
                        var dif = kn - 999;
                        if (dif>7)
                                delay++;
                            
                            if (dif < -7)
                                delay--;
                        
                        Thread.Sleep(delay);
                        //if (i % 100 == 0)
                        //{
                        //    var diff = DateTime.Now - dateTime;
                        //    dateTime = DateTime.Now;
                        //    //var speed = (float)diff.Ticks / (float)startPositon;
                        //    Console.WriteLine(i + "speed " + diff);
                        //}

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                var finish = DateTime.Now;
                Console.WriteLine($"{finish - start}\t{ text.Length} {(double)text.Length / (double)(finish - start).TotalSeconds}");
                Console.WriteLine("delay=" + delay);
            }
            catch (Exception ex)
            {
                Console.WriteLine("EmulateKeyboardInput " + ex.Message);
            }
        }
    }



    enum Language
    {
        rus,
        eng
    }
}
