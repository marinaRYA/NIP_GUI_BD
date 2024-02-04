using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using static Tools.WorkWithWord;
using Xceed.Words.NET;
using System.Windows;

namespace Tools
{

    public class WorkToExcel
    {
        
        private static ExcelPackage excelPackage;
        static string connectionString = "Data Source=NIP.db;Version=3;";
        
        static ExcelWorksheet currentWorksheet;
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
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }

        }
        public WorkToExcel()
        {
            List<Project> projects = GetProjects();

            string filePath = "ProjectAndEmployeesInfo.xlsx";
            using (excelPackage = new ExcelPackage())
            {
                foreach (Project project in projects)
                {
                    AddProjectInfoToExcel(project);
                    List<Employee> employees = GetEmployees(project.ProjectName);
                    AddEmployeesInfoToExcel(employees);
                }

                excelPackage.SaveAs(new System.IO.FileInfo(filePath));
                MessageBox.Show("Сохранено в ProjectAndEmployeesInfo.xlsx");
            }

            Console.WriteLine($"Документ сохранен: {filePath}");
        }
        public static void AddProjectInfoToExcel(Project project)
        {
            // Создаем новый лист для каждого проекта
            currentWorksheet = excelPackage.Workbook.Worksheets.Add(project.ProjectName);

            // Добавляем заголовки и данные для проекта
            currentWorksheet.Cells["A1"].Value = "Имя проекта";
            currentWorksheet.Cells["A2"].Value = project.ProjectName;

            currentWorksheet.Cells["B1"].Value = "Стоимость проекта";
            currentWorksheet.Cells["B2"].Value = project.ProjectCost;

            currentWorksheet.Cells["C1"].Value = "Начальная дата";
            currentWorksheet.Cells["C2"].Value = project.StartDate.ToString("dd.MM.yyyy");

            currentWorksheet.Cells["D1"].Value = "Конечная дата";
            currentWorksheet.Cells["D2"].Value = project.EndDate.ToString("dd.MM.yyyy");

            currentWorksheet.Cells["E1"].Value = "Руководитель проекта";
            currentWorksheet.Cells["E2"].Value = project.ProjectManager;

            currentWorksheet.Cells["F1"].Value = "Контактное лицо";
            currentWorksheet.Cells["F2"].Value = project.ContactPerson;

            currentWorksheet.Cells["G1"].Value = "Телефон контактного лица";
            currentWorksheet.Cells["G2"].Value = project.ContactPersonPhone;

            currentWorksheet.Cells["H1"].Value = "Бонусная ставка";
            currentWorksheet.Cells["H2"].Value = project.BonusRate;

            currentWorksheet.Cells["I1"].Value = "Заказчик";
            currentWorksheet.Cells["I2"].Value = project.Customer;

            
        }

        public static void AddEmployeesInfoToExcel(List<Employee> employees)
        {
           
            currentWorksheet.Cells["A3"].Value = "ФИО";
            currentWorksheet.Cells["A4"].Value = "Пол";
            currentWorksheet.Cells["A5"].Value = "Дата рождения";
            currentWorksheet.Cells["A6"].Value = "Адрес";
            currentWorksheet.Cells["A7"].Value = "Тип специализации";
            currentWorksheet.Cells["A8"].Value = "Опыт работы";
            currentWorksheet.Cells["A9"].Value = "Тип образования";
            currentWorksheet.Cells["A10"].Value = "Зарплата";
            currentWorksheet.Cells["A11"].Value = "Начальная дата";
            currentWorksheet.Cells["A12"].Value = "Конечная дата";


            int row = 1;
            
            foreach (Employee employee in employees)
            {
                char col = (char)('A' + row);
                currentWorksheet.Cells[$"{col}3"].Value = employee.FullName;
                currentWorksheet.Cells[$"{col}4"].Value = employee.Gender;
                currentWorksheet.Cells[$"{col}5"].Value = employee.DateOfBirth.ToString("dd.MM.yyyy");
                currentWorksheet.Cells[$"{col}6"].Value = employee.Address;
                currentWorksheet.Cells[$"{col}7"].Value = employee.SpecializationType;
                currentWorksheet.Cells[$"{col}8"].Value = employee.WorkExperience;
                currentWorksheet.Cells[$"{col}9"].Value = employee.EducationType;
                currentWorksheet.Cells[$"{col}10"].Value = employee.Salary;
                currentWorksheet.Cells[$"{col}11"].Value = employee.StartDate.ToString("dd.MM.yyyy");
                currentWorksheet.Cells[$"{col}12"].Value = employee.EndDate.ToString("dd.MM.yyyy");

               
                row++;
            }
        }
    

private List<Employee> GetEmployees(string projectName)
        {
            List<Employee> employees = new List<Employee>();
            string query = @"
            SELECT 
                Employees.EmployeeID, 
                Employees.FullName,
                Employees.Gender,
                Employees.DateOfBirth,
                Address.AddressName AS Address,
                SpecializationType.SpecializationName AS SpecializationType,
                Employees.WorkExperience,
                EducationType.EducationTypeName AS EducationType,
                Employees.Salary,
                TeamAssignment.StartDate,
                TeamAssignment.EndDate
            FROM 
                Employees
            INNER JOIN TeamAssignment ON Employees.EmployeeID = TeamAssignment.EmployeeID
            INNER JOIN ProjectTeam ON TeamAssignment.TeamID = ProjectTeam.TeamID
            INNER JOIN Project ON ProjectTeam.ProjectID = Project.ProjectID
            INNER JOIN Address ON Employees.AddressID = Address.AddressID
            INNER JOIN SpecializationType ON Employees.SpecializationTypeID = SpecializationType.SpecializationTypeID
            INNER JOIN EducationType ON Employees.EducationTypeID = EducationType.EducationTypeID
            WHERE 
                Project.ProjectName = @ProjectName;";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProjectName", projectName);

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
                                StartDate = Convert.ToDateTime(reader["StartDate"]),
                                EndDate = Convert.ToDateTime(reader["EndDate"])
                            };

                            employees.Add(employee);
                        }
                    }
                }
            }

            return employees;
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
    }

}
