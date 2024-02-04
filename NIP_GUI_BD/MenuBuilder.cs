using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Data;
using System.Data.SQLite;
using System.Windows;
using WpfPresent.Present;
using static NIP_GUI_BD.MainWindow;
using System.Xml.Linq;

namespace NIP_GUI_BD
{
    public class MenuBuilder
    {

        public Menu menu;
        string bdName;
        struct MenuItemData
        {

            public int Id { get; set; }
            public int ParentMenuItemId { get; set; }
            public string MenuText { get; set; }
            public string DLLName { get; set; }
            public string FunctionName { get; set; }
            public int DisplayOrder { get; set; }
        }

        MainWindow MainWindow { get; set; }
        List<MenuItemData> Items;
        private string connectionString;
        public MenuBuilder(MainWindow main)
        {
            MainWindow = main;
            Items = new List<MenuItemData>();
            bdName = main.connectstring;
            connectionString = $"Data Source={bdName};Version=3;";
            menu = BuildMenu();

        }
        private int GetInt(object value)
        {
            return value == DBNull.Value ? 0 : Convert.ToInt32(value);
        }

        private string GetString(object value)
        {
            return value == DBNull.Value ? string.Empty : Convert.ToString(value);
        }
        private Root GetUserPermissions(int userId, int menuItemId)
        {
            Root permissions = new Root();

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = "SELECT R, W, E, D FROM User_Rights WHERE User_ID = @UserId AND Menu_Item_ID = @MenuItemId";
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@MenuItemId", menuItemId);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            permissions.R = Convert.ToInt32(reader["R"]);
                            permissions.W = Convert.ToInt32(reader["W"]);
                            permissions.E = Convert.ToInt32(reader["E"]);
                            permissions.D = Convert.ToInt32(reader["D"]);
                        }
                    }
                }
            }

            return permissions;
        }
        private void ReadBd()
        {
            
            string query = "SELECT Id, ParentMenuItemId, MenuText, DLLName, FunctionName, DisplayOrder FROM Menu";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MenuItemData menuItem = new MenuItemData
                            {
                                Id = GetInt(reader["Id"]),
                                ParentMenuItemId = GetInt(reader["ParentMenuItemId"]),
                                MenuText = GetString(reader["MenuText"]),

                                DLLName = GetString(reader["DLLName"]),
                                FunctionName = GetString(reader["FunctionName"]),
                                DisplayOrder = GetInt(reader["DisplayOrder"])
                            };


                            Items.Add(menuItem);
                        }
                    }
                    connection.Close();
                }

            }
        }
        private Menu BuildMenu()
        {
            ReadBd();


            Menu mainMenu = new Menu();


            var topLevelItems = Items.FindAll(item => item.ParentMenuItemId == 0);

            foreach (var topLevelItem in topLevelItems)
            {
                MenuItem menuItem = CreateMenuItem(topLevelItem);
                if (!string.IsNullOrEmpty(topLevelItem.DLLName) && topLevelItem.ParentMenuItemId != 0)
                {
                    string[] parts = topLevelItem.FunctionName.Split('.');
                    string className = parts[0];
                    string methodName = parts[1];

                    Assembly assembly = Assembly.LoadFrom(topLevelItem.DLLName);
                    Type type = assembly.GetType(topLevelItem.DLLName + "." + className);
                    ConstructorInfo constructor = type.GetConstructor(new Type[] { typeof(Window), typeof(string) });
                    object instance = constructor.Invoke(new object[] { MainWindow,bdName });
                    MethodInfo method = type.GetMethod(methodName);
                    MainWindow.root = GetUserPermissions((int)MainWindow.usernameID, topLevelItem.Id);
                    if (MainWindow.root.R == 1)
                    {
                        menuItem.Click += (sender, e) =>
                        {
                            try
                            {
                                MainWindow.presenter = method.Invoke(instance, null);

                            }
                            catch (Exception ex) { method.Invoke(instance, null); }
                        };
                    }
                }
                mainMenu.Items.Add(menuItem);
                AddSubMenuItems(menuItem, topLevelItem.Id);
                
            }
            return mainMenu;
        }
        private void AddSubMenuItems(MenuItem parentMenuItem, int parentId)
        {

            var subMenuItems = Items.FindAll(item => item.ParentMenuItemId == parentId);


            foreach (var subMenuItem in subMenuItems)
            {
                MenuItem menuItem = CreateMenuItem(subMenuItem);
                parentMenuItem.Items.Add(menuItem);


                AddSubMenuItems(menuItem, subMenuItem.Id);
            }
        }

        private MenuItem CreateMenuItem(MenuItemData menuItemData)
        {

            MenuItem menuItem = new MenuItem
            {
                Header = menuItemData.MenuText,
                Tag = menuItemData

            };
             if (!string.IsNullOrEmpty(menuItemData.FunctionName))
             {
                 string[] parts = menuItemData.FunctionName.Split('.');
                 string className = parts[0]; 
                 string methodName = parts[1]; 

                 Assembly assembly = Assembly.LoadFrom(menuItemData.DLLName);
                 Type type = assembly.GetType(menuItemData.DLLName+"."+className);
                 ConstructorInfo constructor = type.GetConstructor(new Type[] { typeof(Window), typeof(string) });
                 object instance = constructor.Invoke(new object[] { MainWindow, bdName });
                 MethodInfo method = type.GetMethod(methodName);
                MainWindow.root = GetUserPermissions((int)MainWindow.usernameID, menuItemData.Id);
                if (MainWindow.root.R == 1)
                {
                    menuItem.Click += (sender, e) =>
                 {
                     try
                     {
                         MainWindow.presenter = method.Invoke(instance, null);

                     }
                     catch (Exception ex) { method.Invoke(instance, null); }
                 };
                }
             }
            return menuItem;
        }
    }

}
