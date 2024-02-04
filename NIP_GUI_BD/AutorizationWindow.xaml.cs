﻿using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Data.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace NIP_GUI_BD
{
    /// <summary>
    /// Логика взаимодействия для AutorizationWindow.xaml
    /// </summary>
    public partial class AutorizationWindow : Window
    {
        public int usernameID;
        public string nameBd;
        private string ConnectionString;
        public AutorizationWindow()
        {
            nameBd = GetConnectionStringFromJson("pathInfo.json");
            ConnectionString = $"Data Source={nameBd};Version=3;";
            InitializeComponent();
        }
        public string GetConnectionStringFromJson(string jsonFilePath)
        {
            try
            {
                string jsonString = File.ReadAllText(jsonFilePath);
                string dataSource = JsonConvert.DeserializeObject<string>(jsonString);



                string connectionString = $"Data Source={dataSource};Version=3";

                return dataSource;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Запуск по умолчанию");
                return "NIP.db";
            }
        }
        public string CalculateMD5Hash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }
        private void Register_Click(object sender, RoutedEventArgs e)
        {
            string newUsername = txtNewUsername.Text;
            string newPassword = txtNewPassword.Password;
            string repeatPassword = txtRepeatPassword.Password;

            if (string.IsNullOrEmpty(newUsername) || string.IsNullOrEmpty(newPassword))
            {
                MessageBox.Show("Введите логин и пароль.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (newPassword != repeatPassword)
            {
                MessageBox.Show("Пароли не совпадают", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                string selectIdQuery = "SELECT last_insert_rowid();";
                using (var connection = new SQLiteConnection(ConnectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = "INSERT INTO Users (Username, Password) VALUES (@Username, @Password)";
                        command.Parameters.AddWithValue("@Username", newUsername);
                        command.Parameters.AddWithValue("@Password", CalculateMD5Hash(newPassword));
                        command.ExecuteNonQuery();
                        

                        using (SQLiteCommand selectIdCommand = new SQLiteCommand(selectIdQuery, connection))
                        {
                            usernameID = Convert.ToInt32(selectIdCommand.ExecuteScalar());
                        }
                    }


                    int newUserId;
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = "SELECT User_ID FROM Users WHERE Username = @Username";
                        command.Parameters.AddWithValue("@Username", newUsername);
                        newUserId = Convert.ToInt32(command.ExecuteScalar());
                    }


                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        
                        command.CommandText = "SELECT Id FROM Menu";

                        List<int> menuIds = new List<int>();

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                menuIds.Add(Convert.ToInt32(reader["Id"]));
                            }
                        }

                      
                        foreach (int menuId in menuIds)
                        {
                           if (menuId==7) command.CommandText = "INSERT INTO User_Rights (User_ID, Menu_Item_ID, R, W, E, D) VALUES (@UserId, @MenuItemId, 1, 0, 0, 0)";
                            else command.CommandText = "INSERT INTO User_Rights (User_ID, Menu_Item_ID, R, W, E, D) VALUES (@UserId, @MenuItemId, 0, 0, 0, 0)";
                            command.Parameters.AddWithValue("@UserId", newUserId);
                            command.Parameters.AddWithValue("@MenuItemId", menuId);
                            command.ExecuteNonQuery();

                           
                            command.Parameters.Clear();
                        }
                    }


                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при регистрации", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            

            MessageBox.Show("Пользователь успешно зарегистрирован.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Password;
            if (ValidateUser(username, password))
            {
                MessageBox.Show("Авторизация произошла успешно");
                username = txtUsername.Text;
                DialogResult = true;
            }
            else MessageBox.Show("Неправильный логин или пароль, повторите попытку.");
           
        }
        private bool ValidateUser(string username, string password)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = "SELECT User_ID FROM Users WHERE Username = @Username AND Password = @Password";
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", CalculateMD5Hash(password));

                    object result = command.ExecuteScalar();
                    connection.Close();
                    if (result != null) usernameID = Convert.ToInt32(result);
                   
                    return result != null;

                }
                
            }
        }
    }
}
