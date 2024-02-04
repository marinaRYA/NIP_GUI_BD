using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data.SQLite;
using static WpfPresent.BD.BankBd;

namespace WpfPresent.BD
{

    public class AddressBd
    {
        public class Address
        {
            public int AddressID { get; set; }
            public string Street { get; set; }
         
        }

        private string connectionString;

        public List<Address> Addresses;

        public AddressBd(string bdName)
        {
            connectionString = $"Data Source={bdName};Version=3;";
            Addresses = GetAddressData();
        }

        private List<Address> GetAddressData()
        {

            List<Address> addresses = new List<Address>();
            string query = "SELECT * FROM Address";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Address address = new Address
                            {
                                AddressID = Convert.ToInt32(reader["AddressID"]),
                                Street = Convert.ToString(reader["AddressName"]),
                               
                            };

                            addresses.Add(address);
                        }
                    }
                }
            }

            return addresses;
        }

        public void AddAddress(Address newAddress)
        {
            string insertQuery = "INSERT INTO Address (AddressName) VALUES (@Street)";
            string selectIdQuery = "SELECT last_insert_rowid();";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Street", newAddress.Street);
                       
                        command.ExecuteNonQuery();

                        using (SQLiteCommand selectIdCommand = new SQLiteCommand(selectIdQuery, connection))
                        {
                            newAddress.AddressID = Convert.ToInt32(selectIdCommand.ExecuteScalar());
                        }
                    }
                }

                Addresses.Add(newAddress);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении адреса: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void UpdateAddress(Address updatedAddress)
        {
            string updateQuery = "UPDATE Address SET AddressName = @Street WHERE AddressID = @AddressID";


            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Street", updatedAddress.Street);
                        command.Parameters.AddWithValue("@AddressID", updatedAddress.AddressID);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0) throw new Exception("Не удалось обновить. Адрес с указанным AddressID не найден.");
                    }
                    int index = Addresses.FindIndex(adr => adr.AddressID == updatedAddress.AddressID);
                    Addresses[index] = updatedAddress;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при обновлении адреса: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        public void DeleteAddress(int addressId)
        {
            string deleteQuery = "DELETE FROM Address WHERE AddressID = @AddressID";
            int index = Addresses.FindIndex(addr => addr.AddressID == addressId);


            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@AddressID", addressId);
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0) throw new Exception("Удаление не произошло");
                        Addresses.RemoveAt(index);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении адреса: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }


}
