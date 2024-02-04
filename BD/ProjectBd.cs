using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data.SQLite;
using System.Globalization;

namespace WpfPresent.BD
{
    public class ProjectBd
    {
        public class Project
        {
            public int ProjectID { get; set; }
            public string ProjectName { get; set; }
            public double ProjectCost { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public string ProjectManager { get; set; }
            public string ContactPerson { get; set; }
            public string ContactPersonPhone { get; set; }
            public double BonusRate { get; set; }
            public string Customer { get; set; }
        }
        private string connectionString;
        public List<Project> Projects;

        public ProjectBd(string bdName)
        {
            connectionString = $"Data Source={bdName};Version=3;";
            Projects = GetProjects();
        }

        private List<Project> GetProjects()
        {
            List<Project> projects = new List<Project>();
            string query = "SELECT Project.ProjectID, Project.ProjectName, Project.ProjectCost, " +
                           "Project.StartDate, Project.EndDate, Project.ProjectManager, " +
                           "Project.ContactPerson, Project.ContactPersonPhone, Project.BonusRate, " +
                           "Orderer.Customer " +
                           "FROM Project " +
                           "INNER JOIN Orderer ON Project.OrderID = Orderer.OrderID";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Project project = new Project
                            {
                                ProjectID = Convert.ToInt32(reader["ProjectID"]),
                                ProjectName = Convert.ToString(reader["ProjectName"]),
                                ProjectCost = Convert.ToDouble(reader["ProjectCost"]),
                                StartDate = Convert.ToDateTime(reader["StartDate"]),
                                EndDate = Convert.ToDateTime(reader["EndDate"]),
                                ProjectManager = Convert.ToString(reader["ProjectManager"]),
                                ContactPerson = Convert.ToString(reader["ContactPerson"]),
                                ContactPersonPhone = Convert.ToString(reader["ContactPersonPhone"]),
                                BonusRate = Convert.ToDouble(reader["BonusRate"]),
                                Customer = Convert.ToString(reader["Customer"]),
                            };
                           projects.Add(project);   
                        }
                    }
                }
            }

            return projects;
        }

        public void AddProject(Project newProject)
        {
            string insertQuery = "INSERT INTO Project (ProjectName, ProjectCost, StartDate, EndDate, " +
                                 "ProjectManager, ContactPerson, ContactPersonPhone, BonusRate, OrderID) " +
                                 "VALUES (@ProjectName, @ProjectCost, @StartDate, @EndDate, " +
                                 "@ProjectManager, @ContactPerson, @ContactPersonPhone, @BonusRate, @OrderID)";
            string selectIdQuery = "SELECT last_insert_rowid();";
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@ProjectName", newProject.ProjectName);
                        command.Parameters.AddWithValue("@ProjectCost", newProject.ProjectCost);
                        command.Parameters.AddWithValue("@StartDate", newProject.StartDate);
                        command.Parameters.AddWithValue("@EndDate", newProject.EndDate);
                        command.Parameters.AddWithValue("@ProjectManager", newProject.ProjectManager);
                        command.Parameters.AddWithValue("@ContactPerson", newProject.ContactPerson);
                        command.Parameters.AddWithValue("@ContactPersonPhone", newProject.ContactPersonPhone);
                        command.Parameters.AddWithValue("@BonusRate", newProject.BonusRate);
                        command.Parameters.AddWithValue("@OrderID", GetOrderIdByCustomer(newProject.Customer));

                        command.ExecuteNonQuery();
                        using (SQLiteCommand selectIdCommand = new SQLiteCommand(selectIdQuery, connection))
                        {
                            newProject.ProjectID = Convert.ToInt32(selectIdCommand.ExecuteScalar());
                        }
                    }
                }

                Projects.Add(newProject);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void UpdateProject(Project updatedProject)
        {
            string updateQuery = "UPDATE Project " +
                                 "SET ProjectName = @ProjectName, ProjectCost = @ProjectCost, StartDate = @StartDate, " +
                                 "EndDate = @EndDate, ProjectManager = @ProjectManager, " +
                                 "ContactPerson = @ContactPerson, ContactPersonPhone = @ContactPersonPhone, " +
                                 "BonusRate = @BonusRate, OrderID = @OrderID " +
                                 "WHERE ProjectID = @ProjectID";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@ProjectName", updatedProject.ProjectName);
                        command.Parameters.AddWithValue("@ProjectCost", updatedProject.ProjectCost);
                        command.Parameters.AddWithValue("@StartDate", updatedProject.StartDate);
                        command.Parameters.AddWithValue("@EndDate", updatedProject.EndDate);
                        command.Parameters.AddWithValue("@ProjectManager", updatedProject.ProjectManager);
                        command.Parameters.AddWithValue("@ContactPerson", updatedProject.ContactPerson);
                        command.Parameters.AddWithValue("@ContactPersonPhone", updatedProject.ContactPersonPhone);
                        command.Parameters.AddWithValue("@BonusRate", updatedProject.BonusRate);
                        command.Parameters.AddWithValue("@OrderID", GetOrderIdByCustomer(updatedProject.Customer));
                        command.Parameters.AddWithValue("@ProjectID", updatedProject.ProjectID);

                        command.ExecuteNonQuery();
                    }
                }

                int index = Projects.FindIndex(p => p.ProjectID == updatedProject.ProjectID);
                Projects[index] = updatedProject;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void DeleteProject(int projectId)
        {
            string deleteQuery = "DELETE FROM Project WHERE ProjectID = @ProjectID";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@ProjectID", projectId);
                        command.ExecuteNonQuery();
                    }
                }

                Projects.RemoveAll(p => p.ProjectID == projectId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private int GetOrderIdByCustomer(string customer)
        {
            string query = "SELECT OrderID FROM Orderer WHERE Customer = @Customer";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Customer", customer);

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
