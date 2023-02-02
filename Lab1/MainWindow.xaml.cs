using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lab1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string fileContent = string.Empty;
        private string filePath = string.Empty;
        private int[] KEY = { 2, 0, 3, 1, 4 };
        public MainWindow()
        {
            InitializeComponent();
        }

        private void exitButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void chooseFileButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = "C:\\Users\\user\\Desktop\\Основы криптографии\\Lab12";
            openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                filePath = openFileDialog.FileName;

                var fileStream = openFileDialog.OpenFile();

                using (StreamReader reader = new StreamReader(fileStream))
                {
                    fileContent = reader.ReadToEnd();
                }
            }
            currentFileLabel.Content = filePath.ToString();
        }

        private async void encryptionButtonClick(object sender, RoutedEventArgs e)
        {
            var appDir = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string newFilePath = @"Зашифрованый.txt";
            var fullPath = System.IO.Path.Combine(appDir, newFilePath);
            string[] arrayText = ReadFile();
            string[,] result;

            //Запись
            using (FileStream fstream = new FileStream(fullPath, FileMode.OpenOrCreate))
            {
                result = new string[(int)Math.Sqrt(arrayText.Length)+1 , (int)Math.Sqrt(arrayText.Length)+1];
                int count = 0;

                for (int i = 0; i < result.GetLength(1); i++)
                {
                    for (int j = 0; j < result.GetLength(0); j++)
                    {
                        if (count < arrayText.Length)
                        {
                            result[i, j] = arrayText[count];
                            count++;
                        }
                    }
                }

                fstream.Close();
                count = 0;
                for (int i = 0; i < result.GetLength(1); i++) // столбик 
                {
                    for (int j = 0; j < result.GetLength(0); j++) // строка
                    {
                        if (count < arrayText.Length)
                        {
                            arrayText[count] = result[j, SerchKey(i)];
                            count++;
                        }
                    }
                }
                File.WriteAllText(fullPath, string.Join("", arrayText));
            }
        }

        private async void decodingButtonClick(object sender, RoutedEventArgs e)
        {
            var appDir = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string newFilePath = @"Расшифрованный.txt";
            var fullPath = System.IO.Path.Combine(appDir, newFilePath);
            string[] arrayText = ReadFile();
            string[,] result;

            //Запись
            using (FileStream fstream = new FileStream(fullPath, FileMode.OpenOrCreate))
            {
                result = new string[(int)Math.Sqrt(arrayText.Length) + 1, (int)Math.Sqrt(arrayText.Length) + 1];
                string[] serchKey = new string[result.GetLength(1)];
                int count = 0;

                for (int i = 0; i < result.GetLength(1); i++)
                {
                    for (int j = 0; j < result.GetLength(0); j++)
                    {
                        if (count < arrayText.Length)
                        {
                            result[j, i] = arrayText[count];
                            count++;
                        }
                    }
                }

                fstream.Close();

                for (int i = 0; i < result.GetLength(0); i++)
                {
                    serchKey[i] = result[0, i];
                }

                string[] key = decryptKey(serchKey);
                count = 0;

                for (int i = 0; i < result.GetLength(1); i ++)
                { 
                    for (int j = 0; j < result.GetLength(0); j++)
                    {
                        if (count < arrayText.Length)
                        {
                            arrayText[count] = result[i, Convert.ToInt32(key[j])];
                            count++;
                        }
                    }
                }
                File.WriteAllText(fullPath, string.Join("",arrayText));

            }
        }

        private string[] decryptKey(string[] serchKey)
        {
            string[] correct = { "С", "О", "В", "Е", "Р" };
            string[] key = new string[serchKey.Length];

            for (int i = 0; i < serchKey.Length; i++)
            {
                for (int j = 0; j < serchKey.Length; j++)
                {
                    if (correct[i].Equals(serchKey[j]))
                    {
                        key[i] = j.ToString();
                    }
                }
            }
            return key;
        }

        private int SerchKey(int column)
        {
            int keyElement = 0;
            for (int i = 0; i < KEY.Length; i++)
            {
                if (KEY[i] == column)
                {
                    keyElement = i;
                    return keyElement;
                }
            }
            return keyElement;
        }

        private string[] ReadFile()
        {
            byte[] buffer;
            string[] arrayText;

            //Чтение
            using (FileStream fstream = new FileStream(filePath, FileMode.Open))
            {
                buffer = new byte[fstream.Length];
                fstream.Read(buffer, 0, buffer.Length);
                string textFromFile = Encoding.UTF8.GetString(buffer);
                arrayText = new string[textFromFile.Length];

                for (int i = 0; i < textFromFile.Length; i++)
                {
                    arrayText[i] = textFromFile.ElementAt(i).ToString().ToUpper();
                }
            }
            return arrayText;
        }
    }

}
