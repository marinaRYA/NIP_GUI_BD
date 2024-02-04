using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data.SQLite;
using static WpfPresent.BD.AddressBd;

namespace WpfPresent.BD
{
    public class EducationTypeBd
    {
        public class EducationType
        {
            public int EducationTypeID { get; set; }
            public string EducationTypeName { get; set; }
        }

        private string connectionString;
        public List<EducationType> EducationTypes;

        public EducationTypeBd(string bdName)
        {
            connectionString = $"Data Source={bdName};Version=3;";
            EducationTypes = GetEducationTypes();
        }

        private List<EducationType> GetEducationTypes()
        {
            List<EducationType> educationTypes = new List<EducationType>();
            string query = "SELECT * FROM EducationType";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            EducationType educationType = new EducationType
                            {
                                EducationTypeID = Convert.ToInt32(reader["EducationTypeID"]),
                                EducationTypeName = Convert.ToString(reader["EducationTypeName"])
                            };

                            educationTypes.Add(educationType);
                        }
                    }
                }
            }

            return educationTypes;
        }

        public void AddEducationType(EducationType newEducationType)
        {
            string insertQuery = "INSERT INTO EducationType (EducationTypeName) VALUES (@EducationTypeName)";
            string selectIdQuery = "SELECT last_insert_rowid();";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@EducationTypeName", newEducationType.EducationTypeName);
                        command.ExecuteNonQuery();

                        using (SQLiteCommand selectIdCommand = new SQLiteCommand(selectIdQuery, connection))
                        {
                            newEducationType.EducationTypeID = Convert.ToInt32(selectIdCommand.ExecuteScalar());
                        }
                    }
                }

                EducationTypes.Add(newEducationType);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении типа образования: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void UpdateEducationType(EducationType updatedEducationType)
        {
            string updateQuery = "UPDATE EducationType SET EducationTypeName = @EducationTypeName WHERE EducationTypeID = @EducationTypeID";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@EducationTypeName", updatedEducationType.EducationTypeName);
                        command.Parameters.AddWithValue("@EducationTypeID", updatedEducationType.EducationTypeID);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0) throw new Exception("Не удалось обновить. Тип образования с указанным EducationTypeID не найден.");
                    }
                    int index = EducationTypes.FindIndex(edu => edu.EducationTypeID == updatedEducationType.EducationTypeID);
                    EducationTypes[index] = updatedEducationType;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при обновлении типа образования: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void DeleteEducationType(int educationTypeId)
        {
            string deleteQuery = "DELETE FROM EducationType WHERE EducationTypeID = @EducationTypeID";
            int index = EducationTypes.FindIndex(eduType => eduType.EducationTypeID == educationTypeId);

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@EducationTypeID", educationTypeId);
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0) throw new Exception("Удаление не произошло");
                        EducationTypes.RemoveAt(index);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении типа образования: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

}
