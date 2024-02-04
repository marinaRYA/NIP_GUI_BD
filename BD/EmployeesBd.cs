using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Windows;

namespace WpfPresent.BD
{
    public class EmployeesBd
    {
        public class Employee
        {
            public int EmployeeID { get; set; }
            public string FullName { get; set; }
            public string Gender { get; set; }
            public DateTime DateOfBirth { get; set; }
            public string Address { get; set; }
            public string SpecializationType { get; set; }
            public int WorkExperience { get; set; }
            public string EducationType { get; set; }
            public double Salary { get; set; }
            public byte[] Photo { get; set; }
        }

        private string connectionString;
        public List<Employee> Employees;

        public EmployeesBd(string bdName)
        {
            connectionString = $"Data Source={bdName};Version=3;";
            Employees = GetEmployees();
        }

        private List<Employee> GetEmployees()
        {
            List<Employee> employees = new List<Employee>();
            string query = "SELECT Employees.EmployeeID, Employees.FullName, Employees.Gender, Employees.DateOfBirth, " +
                           "Address.AddressName AS Address, SpecializationType.SpecializationName AS SpecializationType, " +
                           "Employees.WorkExperience, EducationType.EducationTypeName AS EducationType, " +
                           "Employees.Salary, Employees.Photo " +
                           "FROM Employees " +
                           "INNER JOIN Address ON Employees.AddressID = Address.AddressID " +
                           "INNER JOIN SpecializationType ON Employees.SpecializationTypeID = SpecializationType.SpecializationTypeID " +
                           "INNER JOIN EducationType ON Employees.EducationTypeID = EducationType.EducationTypeID";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Employee employee = new Employee
                            {
                                EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                                FullName = Convert.ToString(reader["FullName"]),
                                Gender = Convert.ToString(reader["Gender"]),
                                DateOfBirth = Convert.ToDateTime(reader["DateOfBirth"]),
                                Address = Convert.ToString(reader["Address"]),
                                SpecializationType = Convert.ToString(reader["SpecializationType"]),
                                WorkExperience = Convert.ToInt32(reader["WorkExperience"]),
                                EducationType = Convert.ToString(reader["EducationType"]),
                                Salary = Convert.ToDouble(reader["Salary"]),
                                Photo = reader["Photo"] as byte[]
                            };

                            employees.Add(employee);
                        }
                    }
                }
            }

            return employees;
        }

        public void AddEmployee(Employee newEmployee)
        {
            string insertQuery = "INSERT INTO Employees (FullName, Gender, DateOfBirth, AddressID, SpecializationTypeID, WorkExperience, EducationTypeID, Salary, Photo) " +
                                 "VALUES (@FullName, @Gender, @DateOfBirth, @AddressID, @SpecializationTypeID, @WorkExperience, @EducationTypeID, @Salary, @Photo)";
            string selectIdQuery = "SELECT last_insert_rowid();";
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@FullName", newEmployee.FullName);
                        command.Parameters.AddWithValue("@Gender", newEmployee.Gender);
                        command.Parameters.AddWithValue("@DateOfBirth", newEmployee.DateOfBirth);
                        command.Parameters.AddWithValue("@AddressID", GetAddressIDByName(newEmployee.Address));
                        command.Parameters.AddWithValue("@SpecializationTypeID", GetSpecializationTypeIDByName(newEmployee.SpecializationType));
                        command.Parameters.AddWithValue("@WorkExperience", newEmployee.WorkExperience);
                        command.Parameters.AddWithValue("@EducationTypeID", GetEducationTypeIDByName(newEmployee.EducationType));
                        command.Parameters.AddWithValue("@Salary", newEmployee.Salary);
                        command.Parameters.AddWithValue("@Photo", newEmployee.Photo);

                        command.ExecuteNonQuery();
                        using (SQLiteCommand selectIdCommand = new SQLiteCommand(selectIdQuery, connection))
                        {
                            newEmployee.EmployeeID = Convert.ToInt32(selectIdCommand.ExecuteScalar());
                        }
                    }
                }

                Employees.Add(newEmployee);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding employee: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void UpdateEmployee(Employee updatedEmployee)
        {
            string updateQuery = "UPDATE Employees " +
                                 "SET FullName = @FullName, Gender = @Gender, DateOfBirth = @DateOfBirth, " +
                                 "AddressID = @AddressID, SpecializationTypeID = @SpecializationTypeID, " +
                                 "WorkExperience = @WorkExperience, EducationTypeID = @EducationTypeID, " +
                                 "Salary = @Salary, Photo = @Photo " +
                                 "WHERE EmployeeID = @EmployeeID";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@FullName", updatedEmployee.FullName);
                        command.Parameters.AddWithValue("@Gender", updatedEmployee.Gender);
                        command.Parameters.AddWithValue("@DateOfBirth", updatedEmployee.DateOfBirth);
                        command.Parameters.AddWithValue("@AddressID", GetAddressIDByName(updatedEmployee.Address));
                        command.Parameters.AddWithValue("@SpecializationTypeID", GetSpecializationTypeIDByName(updatedEmployee.SpecializationType));
                        command.Parameters.AddWithValue("@WorkExperience", updatedEmployee.WorkExperience);
                        command.Parameters.AddWithValue("@EducationTypeID", GetEducationTypeIDByName(updatedEmployee.EducationType));
                        command.Parameters.AddWithValue("@Salary", updatedEmployee.Salary);
                        command.Parameters.AddWithValue("@Photo", updatedEmployee.Photo);
                        command.Parameters.AddWithValue("@EmployeeID", updatedEmployee.EmployeeID);

                        command.ExecuteNonQuery();
                    }
                }

                int index = Employees.FindIndex(e => e.EmployeeID == updatedEmployee.EmployeeID);
                Employees[index] = updatedEmployee;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating employee: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void DeleteEmployee(int employeeId)
        {
            string deleteQuery = "DELETE FROM Employees WHERE EmployeeID = @EmployeeID";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@EmployeeID", employeeId);
                        command.ExecuteNonQuery();
                    }
                }

                Employees.RemoveAll(e => e.EmployeeID == employeeId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting employee: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private int GetSpecializationTypeIDByName(string specializationTypeName)
        {
            string query = "SELECT SpecializationTypeID FROM SpecializationType WHERE SpecializationName = @SpecializationTypeName";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SpecializationTypeName", specializationTypeName);

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

        private int GetEducationTypeIDByName(string educationTypeName)
        {
            string query = "SELECT EducationTypeID FROM EducationType WHERE EducationTypeName = @EducationTypeName";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EducationTypeName", educationTypeName);

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
