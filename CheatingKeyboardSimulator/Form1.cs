using CheatingKeyboardSimulator.ServiceReference1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tesseract;
using Yandex.Speller.Api.DataContract;

namespace CheatingKeyboardSimulator
{
    public partial class Form1 : Form
    {
        private SpellResult _result { get; set; }
        private Error _error { get; set; }
        CancellationTokenSource cts = new CancellationTokenSource();
        private void Form1_Load(object sender, EventArgs e)
        {
        }

        public Form1()
        {
            InitializeComponent();
        }
        private void buttonScreenShot_Click(object sender, EventArgs e)
        {
            Visible = false;

            Language lang;

            if (radioButtonRus.Checked)
                lang = Language.rus;
            else
                lang = Language.eng;

            
            using (var screenShot = Cheat.GetBitmap())
            {
                if (screenShot == null)
                {
                    richTextBox1.Clear();
                    listBox1.Items.Clear();
                    textBox1.Clear();
                    label1.Text = "Не выбран скрин";
                    Console.WriteLine("Не выбран скрин");
                }
                else
                {
                    string text;
                    text = Cheat.RecognizePicture(screenShot, lang);

                    // Чистка от переносов и двойных пробелов
                    text = text.Replace("“", "\"").Replace("‘","'").Replace("\n", " ").Replace("\n\r", " ").Replace(Environment.NewLine, " ").Replace("  ", " ");

                    richTextBox1.Text = text;
                    _result = Cheat.CheckSpelling(text, lang);

                    if (_result.Errors != null && _result.Errors.Count > 0)
                    {
                        _error = _result.Errors[0];
                        ShowError();
                    }
                    else
                    {
                        Console.WriteLine(_result.Errors == null);
                    }
                    textBox1.Focus();
                }
            }
            Visible = true;
        }

        private void ShowError()
        {
            // Clear format
            richTextBox1.Text = richTextBox1.Text.ToString();
            textBox1.Text = _error.Word;

            if (_error.Steer == null)
                return;
            listBox1.Items.Clear();
            foreach (var item in _error.Steer)
            {
                listBox1.Items.Add(item);
            }
            //Вывод информации
            var err = _result.Errors.Find(x => x.Pos == _error.Pos);
            int index = _result.Errors.IndexOf(err);
            label1.Text = $"Errors: {_result.Errors.Count}  ind: {index + 1}";
            
            // Подсветка неправильного текста
            int selectiontStart = richTextBox1.Text.IndexOf(_error.Word);
            if (selectiontStart == -1) return;
            richTextBox1.SelectionStart = selectiontStart;
            richTextBox1.SelectionLength = _error.Len;
            richTextBox1.SelectionColor = Color.Blue;
            richTextBox1.SelectionFont = new Font(richTextBox1.Font.FontFamily, richTextBox1.Font.Size + 1, FontStyle.Bold);
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            try
            {
                var err = _result.Errors.Find(x => x.Pos == _error.Pos);
                int index = _result.Errors.IndexOf(err);
                if (index > 0)
                    _error = _result.Errors[index - 1];
                ShowError();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Back " + ex.Message);
            }
        }

        private void buttonSkip_Click(object sender, EventArgs e)
        {
            try
            {
                var temp = _error;
                buttonNext.PerformClick();
                var err = _result.Errors.Find(x => x.Pos == temp.Pos);
                _result.Errors.Remove(err);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Back " + ex.Message);
            }
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            try
            {
                var err = _result.Errors.Find(x => x.Pos == _error.Pos);

                int index = _result.Errors.IndexOf(err);
                if (index < _result.Errors.Count + 1)
                    _error = _result.Errors[index + 1];
                ShowError();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Next " + ex.Message);
            }
        }

        private void Emulate_Click(object sender, EventArgs e)
        {
            int startPosotion = int.Parse(maskedTextBoxStartPosition.Text);
            int delay = int.Parse(maskedDelay.Text);
            Cheat.EmulateKeyboardInput(richTextBox1.Text, startPosotion, delay);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedItem = (sender as ListBox).SelectedItem;
            if (selectedItem != null)
                textBox1.Text = selectedItem.ToString();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                var text = (sender as TextBox).Text;
                if (text != string.Empty)
                    richTextBox1.Text = richTextBox1.Text.Replace(_error.Word, text);

            }
            if (e.Alt && e.KeyCode == Keys.Up)       // Ctrl-S Save
            {
                // Do what you want here
                if (listBox1.SelectedIndex > 0)
                {
                    listBox1.SelectedIndex--;
                }
                e.SuppressKeyPress = true;
            }
            if (e.Alt && e.KeyCode == Keys.Down)
            {
                // Do what you want here
                if (listBox1.SelectedIndex < listBox1.SelectedItems.Count)
                {
                    listBox1.SelectedIndex++;
                }
                e.SuppressKeyPress = true;
            }

            if (e.Alt && e.KeyCode == Keys.Right)
            {
                buttonNext.PerformClick();
                e.SuppressKeyPress = true;
            }
            if (e.Alt && e.KeyCode == Keys.Left)
            {
                buttonBack.PerformClick();
                e.SuppressKeyPress = true;
            }
            if (e.Control && e.KeyCode == Keys.Enter)
            {
                Emulate.PerformClick();
                e.SuppressKeyPress = true;
            }

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var text = (sender as TextBox).Text;
            if (text != string.Empty)
                richTextBox1.Text = richTextBox1.Text.Replace(_error.Word, text);
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void richTextBox1_Click(object sender, EventArgs e)
        {
            //maskedTextBoxStartPosition.Text = richTextBox1.SelectionStart.ToString();
        }
    }

}

