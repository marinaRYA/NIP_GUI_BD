using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data.SQLite;


namespace WpfPresent.BD
{
    public class SpecializationTypeBd
    {
        public class SpecializationType
        {
            public int SpecializationTypeID { get; set; }
            public string SpecializationName { get; set; }
        }

        private string connectionString;
        public List<SpecializationType> SpecializationTypes;

        public SpecializationTypeBd(string bdName)
        {
            connectionString = $"Data Source={bdName};Version=3;";
            SpecializationTypes = GetSpecializationTypes();
        }

        private List<SpecializationType> GetSpecializationTypes()
        {
            List<SpecializationType> specializationTypes = new List<SpecializationType>();
            string query = "SELECT * FROM SpecializationType";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            SpecializationType specializationType = new SpecializationType
                            {
                                SpecializationTypeID = Convert.ToInt32(reader["SpecializationTypeID"]),
                                SpecializationName = Convert.ToString(reader["SpecializationName"])
                            };

                            specializationTypes.Add(specializationType);
                        }
                    }
                }
            }

            return specializationTypes;
        }

        public void AddSpecializationType(SpecializationType newSpecializationType)
        {
            string insertQuery = "INSERT INTO SpecializationType (SpecializationName) VALUES (@SpecializationName)";
            string selectIdQuery = "SELECT last_insert_rowid();";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@SpecializationName", newSpecializationType.SpecializationName);
                        command.ExecuteNonQuery();

                        using (SQLiteCommand selectIdCommand = new SQLiteCommand(selectIdQuery, connection))
                        {
                            newSpecializationType.SpecializationTypeID = Convert.ToInt32(selectIdCommand.ExecuteScalar());
                        }
                    }
                }

                SpecializationTypes.Add(newSpecializationType);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении типа специализации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void UpdateSpecializationType(SpecializationType updatedSpecializationType)
        {
            string updateQuery = "UPDATE SpecializationType SET SpecializationName = @SpecializationName WHERE SpecializationTypeID = @SpecializationTypeID";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@SpecializationName", updatedSpecializationType.SpecializationName);
                        command.Parameters.AddWithValue("@SpecializationTypeID", updatedSpecializationType.SpecializationTypeID);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0) throw new Exception("Не удалось обновить. Тип специализации с указанным SpecializationTypeID не найден.");
                    }
                    int index = SpecializationTypes.FindIndex(spec => spec.SpecializationTypeID == updatedSpecializationType.SpecializationTypeID);
                    SpecializationTypes[index] = updatedSpecializationType;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при обновлении типа специализации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void DeleteSpecializationType(int specializationTypeId)
        {
            string deleteQuery = "DELETE FROM SpecializationType WHERE SpecializationTypeID = @SpecializationTypeID";
            int index = SpecializationTypes.FindIndex(specType => specType.SpecializationTypeID == specializationTypeId);

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@SpecializationTypeID", specializationTypeId);
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0) throw new Exception("Удаление не произошло");
                        SpecializationTypes.RemoveAt(index);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении типа специализации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

}
