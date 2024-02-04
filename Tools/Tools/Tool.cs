using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Reflection;
using OfficeOpenXml;

namespace Tools
{
    public class Tool
    {
        string bd;
        System.Windows.Window _view;
        public Tool(System.Windows.Window view, string BD)
        {
            bd = BD;
            _view = view;
        }
        public void About()
        {
            AboutWindow about = new AboutWindow();
            about.Show();
        }
        public void ChangePassword()
        {

            string variableName = "usernameID";


            FieldInfo field = _view.GetType().GetField(variableName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field != null)
            {

                object fieldValue = field.GetValue(_view);

                if (fieldValue is int)
                {
                    ChangePasswordWindow passwordWindow = new ChangePasswordWindow(bd, (int)fieldValue);
                    passwordWindow.Show();
                }


            }
            else MessageBox.Show("Произошла ошибка");

        }
        public void GetNewPathFromUser()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Выберите новый файл базы данных",
                Filter = "SQLite Database Files (*.db)|*.db|All Files (*.*)|*.*",
                CheckFileExists = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedPath = openFileDialog.FileName;


                string pathInfo = selectedPath;


                string json = JsonConvert.SerializeObject(pathInfo);


                string jsonFilePath = "pathInfo.json";


                File.WriteAllText(jsonFilePath, json);


            }
            else
            {
                MessageBox.Show("Ошибка при сохранении");
            }
        }

        public void ExportToWord()
        {
           WorkWithWord w = new WorkWithWord();
        }

        public void ExportToExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            WorkToExcel w = new WorkToExcel();
        }


    }
}



    
