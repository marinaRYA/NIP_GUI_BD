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
    public class BankBd
    {
        public class Bank
        {
            public int BankID { get; set; }
            public string BankName { get; set; }
        }

        private string connectionString;
        public List<Bank> Banks;

        public BankBd(string bdName)
        {
            connectionString = $"Data Source={bdName};Version=3;";
            Banks = GetBankData();
        }

        private List<Bank> GetBankData()
        {
            List<Bank> banks = new List<Bank>();
            string query = "SELECT * FROM Bank";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Bank bank = new Bank
                            {
                                BankID = Convert.ToInt32(reader["BankID"]),
                                BankName = Convert.ToString(reader["BankName"])
                            };

                            banks.Add(bank);
                        }
                    }
                }
            }

            return banks;
        }

        public void AddBank(Bank newBank)
        {
            string insertQuery = "INSERT INTO Bank (BankName) VALUES (@BankName)";
            string selectIdQuery = "SELECT last_insert_rowid();";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@BankName", newBank.BankName);
                        command.ExecuteNonQuery();

                        using (SQLiteCommand selectIdCommand = new SQLiteCommand(selectIdQuery, connection))
                        {
                            newBank.BankID = Convert.ToInt32(selectIdCommand.ExecuteScalar());
                        }
                    }
                }
                Banks.Add(newBank);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении банка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void UpdateBank(Bank updatedBank)
        {
            string updateQuery = "UPDATE Bank SET BankName = @BankName WHERE BankID = @BankID";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@BankName", updatedBank.BankName);
                        command.Parameters.AddWithValue("@BankID", updatedBank.BankID);

                        int rowsAffected = command.ExecuteNonQuery();

                       
                    }
                    int index = Banks.FindIndex(bank => bank.BankID == updatedBank.BankID);
                    Banks[index] = updatedBank;

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при обновлении банка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }
        }

        public void DeleteBank(int bankId)
        {
            string deleteQuery = "DELETE FROM Bank WHERE BankID = @BankID";
            int index = Banks.FindIndex(b => b.BankID == bankId);

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@BankID", bankId);
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0) throw new Exception("Удаление не произошло");
                        Banks.RemoveAt(index);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении банка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

    }

}
