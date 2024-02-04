using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using WpfPresent.BD;

namespace WpfPresent.Present
{
    public class PresenterEmployees : IPresenterCommon
    {
        private Window _view;
        private DataGrid employeesGrid;
        private EmployeesBd employeesBd;
        private string BD;

        public PresenterEmployees(Window view, string bd)
        {
            BD = bd;
            employeesBd = new EmployeesBd(bd);
            _view = view;

            StackPanel gridPanel = _view.FindName("GridPanel") as StackPanel;
            gridPanel.Children.Clear();
            employeesGrid = new DataGrid();
            employeesGrid.Width = double.NaN;
            employeesGrid.CanUserAddRows = false;
            employeesGrid.AutoGenerateColumns = false;
            employeesGrid.IsReadOnly = true;

            DataGridTextColumn idColumn = new DataGridTextColumn();
            idColumn.Header = "ID";
            idColumn.Binding = new Binding("EmployeeID");

            DataGridTextColumn fullNameColumn = new DataGridTextColumn();
            fullNameColumn.Header = "ФИО";
            fullNameColumn.Binding = new Binding("FullName");

            DataGridTextColumn genderColumn = new DataGridTextColumn();
            genderColumn.Header = "Пол";
            genderColumn.Binding = new Binding("Gender");

            DataGridTextColumn dateOfBirthColumn = new DataGridTextColumn();
            dateOfBirthColumn.Header = "Дата рождения";
            dateOfBirthColumn.Binding = new Binding("DateOfBirth")
            {
                StringFormat = "dd.MM.yyyy" 
            };

            DataGridTextColumn addressColumn = new DataGridTextColumn();
            addressColumn.Header = "Адрес";
            addressColumn.Binding = new Binding("Address");

            DataGridTextColumn specializationTypeColumn = new DataGridTextColumn();
            specializationTypeColumn.Header = "Специализация";
            specializationTypeColumn.Binding = new Binding("SpecializationType");

            DataGridTextColumn workExperienceColumn = new DataGridTextColumn();
            workExperienceColumn.Header = "Опыт работы";
            workExperienceColumn.Binding = new Binding("WorkExperience");

            DataGridTextColumn educationTypeColumn = new DataGridTextColumn();
            educationTypeColumn.Header = "Образование";
            educationTypeColumn.Binding = new Binding("EducationType");

            DataGridTextColumn salaryColumn = new DataGridTextColumn();
            salaryColumn.Header = "Зарплата";
            salaryColumn.Binding = new Binding("Salary");
            DataGridTemplateColumn photoColumn = new DataGridTemplateColumn();
            photoColumn.Header = "Фото";
            FrameworkElementFactory imageFactory = new FrameworkElementFactory(typeof(Image));
            imageFactory.SetBinding(Image.SourceProperty, new Binding("Photo"));
            imageFactory.SetValue(FrameworkElement.WidthProperty, 50.0);
            imageFactory.SetValue(FrameworkElement.HeightProperty, 50.0);
            DataTemplate cellTemplate = new DataTemplate { VisualTree = imageFactory };
            photoColumn.CellTemplate = cellTemplate;



            employeesGrid.Columns.Add(idColumn);
            employeesGrid.Columns.Add(fullNameColumn);
            employeesGrid.Columns.Add(genderColumn);
            employeesGrid.Columns.Add(dateOfBirthColumn);
            employeesGrid.Columns.Add(addressColumn);
            employeesGrid.Columns.Add(specializationTypeColumn);
            employeesGrid.Columns.Add(workExperienceColumn);
            employeesGrid.Columns.Add(educationTypeColumn);
            employeesGrid.Columns.Add(salaryColumn);
            employeesGrid.Columns.Add(photoColumn);
            employeesGrid.ItemsSource = employeesBd.Employees;
            gridPanel.Children.Add(employeesGrid);
        }

        public void AddObject()
        {
            EmployeeWindow employeeWindow = new EmployeeWindow(new EmployeesBd.Employee(), BD);
            employeeWindow.ShowDialog();

            if (employeeWindow.DialogResult == true)
            {
                employeesBd.AddEmployee(employeeWindow.employee);
                employeesGrid.ItemsSource = employeesBd.Employees;
                employeesGrid.Items.Refresh();
            }
        }

        public void EditObject()
        {
            if (employeesGrid.SelectedItem != null)
            {
                int index = employeesGrid.SelectedIndex;
                EmployeesBd.Employee selectedEmployee = employeesGrid.SelectedItem as EmployeesBd.Employee;
                EmployeeWindow employeeWindow = new EmployeeWindow(selectedEmployee, BD);
                employeeWindow.ShowDialog();

                if (employeeWindow.DialogResult == true)
                {
                    EmployeesBd.Employee updatedEmployee = employeeWindow.employee;
                    employeesBd.UpdateEmployee(updatedEmployee);
                    employeesGrid.ItemsSource = employeesBd.Employees;
                    employeesGrid.Items.Refresh();
                }
            }
        }

        public void DeleteObject()
        {
            if (employeesGrid.SelectedItem != null)
            {
                MessageBoxResult result = MessageBox.Show("Точно хотите удалить сотрудника?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        int index = employeesGrid.SelectedIndex;
                        EmployeesBd.Employee selectedEmployee = employeesGrid.SelectedItem as EmployeesBd.Employee;
                        employeesBd.DeleteEmployee(selectedEmployee.EmployeeID);
                        employeesGrid.ItemsSource = employeesBd.Employees;
                        employeesGrid.Items.Refresh();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении сотрудника: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        public void Search(string searchTerm)
        {
            List<EmployeesBd.Employee> filteredList = employeesBd.Employees
                .Where(employee =>
                    employee.GetType().GetProperties()
                        .Any(prop =>
                            prop.GetValue(employee)?.ToString()?.ToLower().Contains(searchTerm.ToLower()) ?? false
                        )
                )
                .ToList();

            employeesGrid.ItemsSource = filteredList;
            employeesGrid.Items.Refresh();
        }
    }

}
