using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Windows;
using static WpfPresent.BD.TeamCompositionBd;
using static System.Data.Entity.Infrastructure.Design.Executor;
using static WpfPresent.BD.SpecializationTypeBd;

namespace WpfPresent.BD
{
    public class TeamCompositionBd
    {
        public class TeamComposition
        {
            public int TeamId { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public string FullName { get; set; }
        }
        public List<TeamComposition> TeamCompositions;
        private string connectionString;
        private string teamName;

        public TeamCompositionBd(string bdName, string teamName)
        {
            connectionString = $"Data Source={bdName};Version=3;";
            this.teamName = teamName;
            TeamCompositions = GetTeamComposition();
            
        }

        public List<TeamComposition> GetTeamComposition()
        {
            List<TeamComposition> teamComposition = new List<TeamComposition>();
            string query = "SELECT Employees.EmployeeID, Employees.FullName, TeamAssignment.StartDate, TeamAssignment.EndDate " +
                           "FROM Employees " +
                           "INNER JOIN TeamAssignment ON Employees.EmployeeID = TeamAssignment.EmployeeID " +
                           "INNER JOIN ProjectTeam ON TeamAssignment.TeamID = ProjectTeam.TeamID " +
                           "INNER JOIN Project ON ProjectTeam.ProjectID = Project.ProjectID " +
                           "WHERE Project.ProjectName = @TeamName";




            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TeamName", teamName);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TeamComposition composition = new TeamComposition
                            {
                                StartDate = Convert.ToDateTime(reader["StartDate"]),
                                EndDate = Convert.ToDateTime(reader["EndDate"]),
                                FullName = Convert.ToString(reader["FullName"]),
                            };

                            teamComposition.Add(composition);
                        }
                    }
                }
            }

            return teamComposition;
        }

        public void AddEmployeeToTeam(TeamComposition newEmpInTeam)
        {
            string insertQuery = "INSERT INTO TeamAssignment (TeamID, EmployeeID, StartDate, EndDate) " +
                                 "VALUES (@TeamID, @EmployeeID, @StartDate, @EndDate)";

            string selectIdQuery = "SELECT last_insert_rowid();";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@TeamID", GetTeamIdByName(teamName));
                        command.Parameters.AddWithValue("@EmployeeID", GetEmployeeIdByFullName(newEmpInTeam.FullName));
                        command.Parameters.AddWithValue("@StartDate", newEmpInTeam.StartDate);
                        command.Parameters.AddWithValue("@EndDate", newEmpInTeam.EndDate);

                        command.ExecuteNonQuery();

                        using (SQLiteCommand selectIdCommand = new SQLiteCommand(selectIdQuery, connection))
                        {
                            newEmpInTeam.TeamId = Convert.ToInt32(selectIdCommand.ExecuteScalar());
                        }
                    }
                }

                TeamCompositions.Add(newEmpInTeam);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении работника: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void UpdateEmployeeInTeam(TeamComposition updatedEmpInTeam)
        {
            string updateQuery = "UPDATE TeamAssignment " +
                                 "SET StartDate = @StartDate, EndDate = @EndDate " +
                                 "WHERE TeamID = @TeamID AND EmployeeID = @EmployeeID";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@TeamID", GetTeamIdByName(teamName));
                        command.Parameters.AddWithValue("@EmployeeID", GetEmployeeIdByFullName(updatedEmpInTeam.FullName));
                        command.Parameters.AddWithValue("@StartDate", updatedEmpInTeam.StartDate);
                        command.Parameters.AddWithValue("@EndDate", updatedEmpInTeam.EndDate);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected == 0) throw new Exception("Не удалось обновить. Запись в TeamAssignment не найдена.");
                        }
                    }
                int index = TeamCompositions.FindIndex(team => team.TeamId == updatedEmpInTeam.TeamId);
                TeamCompositions[index] = updatedEmpInTeam;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при редактировании: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public void RemoveEmployeeFromTeam(string name)
        {
            string deleteQuery = "DELETE FROM TeamAssignment " +
                                 "WHERE TeamID = @TeamID AND EmployeeID = @EmployeeID";
            int index = GetEmployeeIdByFullName(name);
            int ind = TeamCompositions.FindIndex(team => team.TeamId == index);

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@TeamID", GetTeamIdByName(teamName));
                        command.Parameters.AddWithValue("@EmployeeID", index);

                        command.ExecuteNonQuery();
                        TeamCompositions.RemoveAt(ind);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

       
        private int GetTeamIdByName(string teamName)
        {
            string query = "SELECT TeamID " +
                "FROM ProjectTeam " +
                "INNER JOIN Project ON ProjectTeam.ProjectID = Project.ProjectID " +
                "WHERE Project.ProjectName = @ProjectName";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProjectName", teamName);

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
        public int GetEmployeeIdByFullName(string fullName)
        {
            string query = "SELECT EmployeeID FROM Employees WHERE FullName = @FullName";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FullName", fullName);

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
