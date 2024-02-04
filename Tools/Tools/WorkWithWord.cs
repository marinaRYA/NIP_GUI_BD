using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Xceed.Words.NET;
using System.Windows;
namespace Tools
{
    public class WorkWithWord
    {
       

        static string connectionString = "Data Source=NIP.db;Version=3;";
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
        public WorkWithWord()
        {
            List<Project> projects = GetProjects();

            string filePath = "ProjectAndEmployeesInfo.docx";

            using (DocX document = DocX.Create(filePath))
            {
                foreach (Project project in projects)
                {
                    AddProjectInfoToDocument(document, project);
                    List<Employee> employees = GetEmployees(project.ProjectName);
                    AddEmployeesInfoToDocument(document, employees);
                }
                document.Save();
                MessageBox.Show("Сохранено в ProjectAndEmployeesInfo.docx");
            }
        }
            static void AddProjectInfoToDocument(DocX document, Project project)
            {
                document.InsertParagraph($"Имя проекта: {project.ProjectName}");
                document.InsertParagraph($"Стоимость проекта: {project.ProjectCost:N2}");
                document.InsertParagraph($"Начальная дата: {project.StartDate.ToString("dd.MM.yyyy")}");
                document.InsertParagraph($"Конечная дата: {project.EndDate.ToString("dd.MM.yyyy")}");
                document.InsertParagraph($"Руководитель проекта: {project.ProjectManager}");
                document.InsertParagraph($"Контактное лицо: {project.ContactPerson}");
                document.InsertParagraph($"Телефон контактного лица: {project.ContactPersonPhone}");
                document.InsertParagraph($"Бонусная ставка: {project.BonusRate:N2}");
                document.InsertParagraph($"Заказчик: {project.Customer}");
                document.InsertParagraph();
            }

            static void AddEmployeesInfoToDocument(DocX document, List<Employee> employees)
            {
                document.InsertParagraph("Сотрудники на проекте:");
                foreach (Employee employee in employees)
                {
                    document.InsertParagraph($"ФИО: {employee.FullName}");
                    document.InsertParagraph($"Пол: {employee.Gender}");
                    document.InsertParagraph($"Дата рождения: {employee.DateOfBirth.ToString("dd.MM.yyyy")}");
                    document.InsertParagraph($"Адрес: {employee.Address}");
                    document.InsertParagraph($"Тип специализации: {employee.SpecializationType}");
                    document.InsertParagraph($"Опыт работы: {employee.WorkExperience}");
                    document.InsertParagraph($"Тип образования: {employee.EducationType}");
                    document.InsertParagraph($"Зарплата: {employee.Salary:N2}");
                    document.InsertParagraph($"Начальная дата: {employee.StartDate.ToString("dd.MM.yyyy")}");
                    document.InsertParagraph($"Конечная дата: {employee.EndDate.ToString("dd.MM.yyyy")}");
                    document.InsertParagraph();
                }

                document.InsertParagraph(); 
            }

        private  List<Employee> GetEmployees(string projectName)
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
