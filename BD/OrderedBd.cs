using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Windows;

namespace WpfPresent.BD
{
    public class OrderedBd
    {
        public class Ordered
        {
            public int OrderID { get; set; }
            public string Customer { get; set; }
            public string AccountNumber { get; set; }
            public string CustomerINN { get; set; }
            public string BankName { get; set; }
            public string Street { get; set; }
          
        }

        private string connectionString;
        public List<Ordered> Orders;

        public OrderedBd(string bdName)
        {
            connectionString = $"Data Source={bdName};Version=3;";
            Orders = GetOrders();
        }

        private List<Ordered> GetOrders()
        {
            List<Ordered> orders = new List<Ordered>();
            string query = "SELECT Orderer.OrderID, Orderer.Customer, Orderer.AccountNumber, Orderer.CustomerINN, Bank.BankName, Address.AddressName " +
                           "FROM Orderer " +
                           "INNER JOIN Bank ON Orderer.BankID = Bank.BankID " +
                           "INNER JOIN Address ON Orderer.AddressID = Address.AddressID";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Ordered order = new Ordered
                            {
                                OrderID = Convert.ToInt32(reader["OrderID"]),
                                Customer = Convert.ToString(reader["Customer"]),
                                AccountNumber = Convert.ToString(reader["AccountNumber"]),
                                CustomerINN = Convert.ToString(reader["CustomerINN"]),
                                BankName = Convert.ToString(reader["BankName"]),
                                Street = Convert.ToString(reader["AddressName"]),
                            };

                            orders.Add(order);
                        }
                    }
                }
            }

            return orders;
        }
        public void AddOrder(Ordered newOrder)
            {
                string insertQuery = "INSERT INTO Orderer (Customer, AccountNumber, CustomerINN, BankID, AddressID) " +
                                     "VALUES (@Customer, @AccountNumber, @CustomerINN, @BankID, @AddressID)";
            string selectIdQuery = "SELECT last_insert_rowid();";
            try
                {
                    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();

                        using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                        {
                            command.Parameters.AddWithValue("@Customer", newOrder.Customer);
                            command.Parameters.AddWithValue("@AccountNumber", newOrder.AccountNumber);
                            command.Parameters.AddWithValue("@CustomerINN", newOrder.CustomerINN);
                            command.Parameters.AddWithValue("@BankID", GetBankIDByName(newOrder.BankName));
                            command.Parameters.AddWithValue("@AddressID", GetAddressIDByName(newOrder.Street));

                            command.ExecuteNonQuery();
                        }
                    using (SQLiteCommand selectIdCommand = new SQLiteCommand(selectIdQuery, connection))
                    {
                        newOrder.OrderID = Convert.ToInt32(selectIdCommand.ExecuteScalar());
                    }
                }

                    Orders.Add(newOrder);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении заказа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            public void UpdateOrder(Ordered updatedOrder)
            {
                string updateQuery = "UPDATE Orderer " +
                                     "SET Customer = @Customer, AccountNumber = @AccountNumber, CustomerINN = @CustomerINN, " +
                                     "BankID = @BankID, AddressID = @AddressID " +
                                     "WHERE OrderID = @OrderID";

                try
                {
                    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();

                        using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
                        {
                            command.Parameters.AddWithValue("@Customer", updatedOrder.Customer);
                            command.Parameters.AddWithValue("@AccountNumber", updatedOrder.AccountNumber);
                            command.Parameters.AddWithValue("@CustomerINN", updatedOrder.CustomerINN);
                            command.Parameters.AddWithValue("@BankID", GetBankIDByName(updatedOrder.BankName));
                            command.Parameters.AddWithValue("@AddressID", GetAddressIDByName(updatedOrder.Street));
                            command.Parameters.AddWithValue("@OrderID", updatedOrder.OrderID);

                            command.ExecuteNonQuery();
                        }
                    }

                    int index = Orders.FindIndex(o => o.OrderID == updatedOrder.OrderID);
                    Orders[index] = updatedOrder;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при обновлении заказа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            public void DeleteOrder(int orderId)
            {
                string deleteQuery = "DELETE FROM Orderer WHERE OrderID = @OrderID";

                try
                {
                    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();

                        using (SQLiteCommand command = new SQLiteCommand(deleteQuery, connection))
                        {
                            command.Parameters.AddWithValue("@OrderID", orderId);
                            command.ExecuteNonQuery();
                        }
                    }

                    Orders.RemoveAll(o => o.OrderID == orderId);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении заказа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        private int GetBankIDByName(string bankName)
        {
            string query = "SELECT BankID FROM Bank WHERE BankName = @BankName";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@BankName", bankName);

                    object result = command.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        return Convert.ToInt32(result);
                    }
                    else
                    {
                        
                        return -1;
                    }
                }
            }
        }

        private int GetAddressIDByName(string street)
        {
            string query = "SELECT AddressID FROM Address WHERE AddressName = @AddressName";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@AddressName", street);

                    object result = command.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        return Convert.ToInt32(result);
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
        }


    }

}


