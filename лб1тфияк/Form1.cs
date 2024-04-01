using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace лб1тфияк
{
    public partial class Form1 : Form
    {
        private int fileCounter = 1;
        private string openedFilePath;
        private Lexer lexer;
        private List<ParsingError> _parsingErrors = new List<ParsingError>();


        public Form1()
        {
            InitializeComponent();
        }





        private void создатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string folderPath = @"C:\Users\Ольга\source\repos\лб1тфияк\лб1тфияк\bin\Debug\netcoreapp3.1";
            try
            {
                string fileName = $"File{fileCounter}.txt";
                string filePath = Path.Combine(folderPath, fileName);
                // Создание файла
                File.WriteAllText(filePath, $"Содержимое файла {fileCounter}");
                MessageBox.Show($"Файл {fileName} успешно создан.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                fileCounter++;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private bool CheckForChanges()
        {
            if (richTextBox1.Modified)
            {
                DialogResult result = MessageBox.Show("Хотите сохранить изменения?", "Предупреждение", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                {
                    SaveChanges();
                }
                else if (result == DialogResult.Cancel)
                {
                    return false;
                }
            }
            return true;
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!CheckForChanges())
            {
                return;
            }
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;

                    try
                    {
                        // Считываем содержимое файла
                        string fileContent = File.ReadAllText(filePath);

                        // Устанавливаем текст в RichTextBox
                        richTextBox1.Text = fileContent;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при открытии файла: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }




        private void сохранитьКакToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(openedFilePath))
            {

                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        openedFilePath = saveFileDialog.FileName;
                    }
                    else
                    {
                        return; // Пользователь отменил сохранение
                    }
                }
            }

            try
            {
                // Сохраняем содержимое RichTextBox в тот же файл
                File.WriteAllText(openedFilePath, richTextBox1.Text);
                MessageBox.Show("Файл успешно сохранен", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении файла: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            richTextBox1.Modified = false;
        }

        private void выделитьВсеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectAll();
        }

        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.SelectionLength > 0)
            {
                richTextBox1.SelectedText = "";
            }
        }

        private void копироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.SelectionLength > 0)
            {
                Clipboard.SetText(richTextBox1.SelectedText);
            }
        }

        private void вставитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                richTextBox1.SelectedText = Clipboard.GetText();
            }
        }

        private void вырезатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.SelectionLength > 0)
            {
                Clipboard.SetText(richTextBox1.SelectedText);
                richTextBox1.SelectedText = "";
            }
        }

        private void отменитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.CanUndo)
            {
                richTextBox1.Undo();
            }
        }

        private void повторитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.CanRedo)
            {
                richTextBox1.Redo();
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            string folderPath = @"C:\Users\Ольга\source\repos\лб1тфияк\лб1тфияк\bin\Debug\netcoreapp3.1";
            try
            {
                string fileName = $"File{fileCounter}.txt";
                string filePath = Path.Combine(folderPath, fileName);
                // Создание файла
                File.WriteAllText(filePath, $"Содержимое файла {fileCounter}");
                MessageBox.Show($"Файл {fileName} успешно создан.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                fileCounter++;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (!CheckForChanges())
            {
                return;
            }
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    openedFilePath = openFileDialog.FileName;

                    try
                    {
                        // Считываем содержимое файла
                        string fileContent = File.ReadAllText(openedFilePath);

                        // Устанавливаем текст в RichTextBox
                        richTextBox1.Text = fileContent;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при открытии файла: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(openedFilePath))
                {
                    // Сохраняем содержимое RichTextBox в тот же файл
                    File.WriteAllText(openedFilePath, richTextBox1.Text);
                    MessageBox.Show("Файл успешно сохранен", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Если openedFilePath не установлена, используйте диалог сохранения
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        openedFilePath = saveFileDialog.FileName;
                        // Сохраняем содержимое RichTextBox в новый файл
                        File.WriteAllText(openedFilePath, richTextBox1.Text);
                        MessageBox.Show("Файл успешно сохранен", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении файла: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            richTextBox1.Modified = false;
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (richTextBox1.CanUndo)
            {
                richTextBox1.Undo();
            }
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {

            if (richTextBox1.CanRedo)
            {
                richTextBox1.Redo();
            }
        }

        private void toolStripButton11_Click(object sender, EventArgs e)
        {

            if (richTextBox1.SelectionLength > 0)
            {
                Clipboard.SetText(richTextBox1.SelectedText);
            }
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            if (richTextBox1.SelectionLength > 0)
            {
                Clipboard.SetText(richTextBox1.SelectedText);
                richTextBox1.SelectedText = "";
            }
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                richTextBox1.SelectedText = Clipboard.GetText();
            }
        }
        private bool IsRichTextBoxModified()
        {
            return richTextBox1.Modified;
        }

        private void PromptToSaveChanges()
        {
            if (IsRichTextBoxModified())
            {
                DialogResult result = MessageBox.Show("Сохранить изменения?", "Предупреждение", MessageBoxButtons.YesNoCancel);

                if (result == DialogResult.Yes)
                {
                    SaveChanges();
                    Close();
                }
                else if (result == DialogResult.Cancel)
                {
                    return;
                }
                else if (result == DialogResult.No)
                {
                    Close();
                }
            }
        }

        private void SaveChanges()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Текстовые файлы|*.txt|Все файлы|*.*";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                richTextBox1.SaveFile(saveFileDialog.FileName, RichTextBoxStreamType.PlainText);
                richTextBox1.Modified = false;
            }
        }
        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (IsRichTextBoxModified())
            {
                PromptToSaveChanges();
            }
            else
            {
                Close();
            }
        }

        private void richTextBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        private void Information()
        {
            const string filePath = @"C:\Users\Ольга\source\repos\лб1тфияк\лб1тфияк\bin\Debug\netcoreapp3.1\y.html";
            if (System.IO.File.Exists(filePath))
            {
                Process p = new Process();
                p.StartInfo = new ProcessStartInfo(filePath) { UseShellExecute = true };
                p.Start();
            }
        }
        private void вызовСправкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Information();


        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            Information();
        }
        private void About_program()
        {
            const string filePath = @"C:\Users\Ольга\source\repos\лб1тфияк\лб1тфияк\bin\Debug\netcoreapp3.1\about.html";
            if (System.IO.File.Exists(filePath))
            {
                Process p = new Process();
                p.StartInfo = new ProcessStartInfo(filePath) { UseShellExecute = true };
                p.Start();
            }
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About_program();
        }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            About_program();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (IsRichTextBoxModified())
            {
                PromptToSaveChanges();
            }
            else
            {
                Close();
            }

        }

        private void пускToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string inputText = richTextBox1.Text;
            var lexer = new Lexer(inputText);
            List<Token> tokens = lexer.Analyze();

            var buff = new List<Token>();

            foreach (var token in tokens)
            {
                if (token.Value != " Пробел ")
                {
                    buff.Add(token);
                }
            }
            
            var errors = new Parser();
            var Exceptions = errors.Parse(buff, inputText);
            
            // _parsingErrors = Exceptions.GetParsingErrors();
            DisplayParsingErrors(Exceptions);
            //inputText = FixErrors.Fix(inputText);
            
            //richTextBox1.Text = inputText;

            dataGridView1.Visible = true;
        }

        private void DisplayParsingErrors(List<ParsingError> errors)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            
            dataGridView1.Columns.Add("ErrorNumber", "Номер ошибки");
            dataGridView1.Columns.Add("ErrorToken", "Ошибочный токен");
            dataGridView1.Columns.Add("NeedToken", "Ожидаемый токен");
            dataGridView1.Columns.Add("Message", "Сообщение об ошибке");
            
            foreach (var error in errors)
            {
                string typeDescription = GetTypeString(error.NumberOfError);
                string location = $"с {error.StartIndex} до {error.EndIndex}";
                dataGridView1.Rows.Add(error.NumberOfError, error.ErrorToken, error.NeedToken, location, error.Message);
            }
        }
        
        private void DisplayTokensInDataGridView(List<Token> tokens)
        {
            dataGridView1.Rows.Clear();

            foreach (var token in tokens)
            {
                string typeDescription = GetTypeString(token.Type);
                string location = $"с {token.StartPos} по {token.StartPos + token.Value.Length - 1} символ";
                dataGridView1.Rows.Add(token.Type, typeDescription, token.Value, location);
            }
        }
        private string GetTypeString(int type)
        {
            switch (type)
            {
                case -1:
                    return "ERROR";
                case 1:
                    return "Ключевое слово";
                case 2:
                    return "Ключевое слово";
                case 3:
                    return "Ключевое слово";
                case 4:
                    return "Ключевое слово";
                case 5:
                    return "Ключевое слово";
                case 6:
                    return "Ключевое слово";
                case 7:
                    return "Ключевое слово";
                case 8:
                    return "Ключевое слово";
                case 9:
                    return "Ключевое слово";
                case 10:
                    return "Ключевое слово";
                case 11:
                    return "Разделитель (пробел)";
                case 12:
                    return "Название структуры/поля";
                case 13:
                    return "Символ";
                case 14:
                    return "Символ";
                case 15:
                    return "Конец оператора";
                default:
                    return "UNKNOWN";
            }
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            string inputText = richTextBox1.Text;
            var lexer = new Lexer(inputText);
            List<Token> tokens = lexer.Analyze();
            DisplayTokensInDataGridView(tokens);
            dataGridView1.Visible = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string inputText = richTextBox1.Text;
            inputText = FixErrors.Fix(inputText);
            var lexer = new Lexer(inputText);
            List<Token> tokens = lexer.Analyze();

            var buff = new List<Token>();

            foreach (var token in tokens)
            {
                if (token.Value != " Пробел ")
                {
                    buff.Add(token);
                }
            }
            richTextBox1.Text = inputText;
            var errors = new Parser();
            var Exceptions = errors.Parse(buff, inputText);

            // _parsingErrors = Exceptions.GetParsingErrors();
            DisplayParsingErrors(Exceptions);

        }
    }
}


        