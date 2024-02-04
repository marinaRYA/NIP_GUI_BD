using Microsoft.Win32;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static WpfPresent.BD.AddressBd;
using static WpfPresent.BD.EducationTypeBd;
using static WpfPresent.BD.EmployeesBd;
using static WpfPresent.BD.OrderedBd;
using static WpfPresent.BD.SpecializationTypeBd;

namespace WpfPresent.BD
{
    /// <summary>
    /// Логика взаимодействия для EmployeeWindow.xaml
    /// </summary>
    public partial class EmployeeWindow : Window
    {
        public Employee employee;
        AddressBd addressBd;
        SpecializationTypeBd specializationTypeBd;
        EducationTypeBd educationTypeBd;
        public EmployeeWindow(Employee emp, string bd)
        {
            employee = emp;
            addressBd = new AddressBd(bd);
            specializationTypeBd = new SpecializationTypeBd(bd);
            educationTypeBd = new EducationTypeBd(bd);
            InitializeComponent();
            SpecializationTypeComboBox.ItemsSource = specializationTypeBd.SpecializationTypes;
            SpecializationTypeComboBox.DisplayMemberPath = "SpecializationName";
            EducationTypeComboBox.ItemsSource = educationTypeBd.EducationTypes;
            EducationTypeComboBox.DisplayMemberPath = "EducationTypeName";

            AddressComboBox.ItemsSource = addressBd.Addresses;
            AddressComboBox.DisplayMemberPath = "Street";
            DisplayEmployeeDetails();
        }
        private void DisplayEmployeeDetails()
        {
            FullNameTextBox.Text = employee.FullName;

            GenderComboBox.SelectedItem = GetComboBoxItemByContent(GenderComboBox, employee.Gender);
            DateOfBirthDatePicker.SelectedDate = employee.DateOfBirth;
            AddressComboBox.SelectedItem =addressBd.Addresses.FirstOrDefault(address => address.Street == employee.Address);
            SpecializationTypeComboBox.SelectedItem = specializationTypeBd.SpecializationTypes.FirstOrDefault(spec => spec.SpecializationName == employee.SpecializationType);
            WorkExperienceTextBox.Text = employee.WorkExperience.ToString();
            EducationTypeComboBox.SelectedItem = educationTypeBd.EducationTypes.FirstOrDefault(edu => edu.EducationTypeName == employee.EducationType);
            SalaryTextBox.Text = employee.Salary.ToString();
          
        }
        private ComboBoxItem GetComboBoxItemByContent(ComboBox comboBox, string content)
        {
            foreach (var item in comboBox.Items)
            {
                if ((item as ComboBoxItem)?.Content.ToString() == content)
                {
                    return item as ComboBoxItem;
                }
            }
            return null;
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(FullNameTextBox.Text) ||
                    GenderComboBox.SelectedItem == null ||
                    DateOfBirthDatePicker.SelectedDate == null ||
                    AddressComboBox.SelectedItem == null ||
                    SpecializationTypeComboBox.SelectedItem == null ||
                    string.IsNullOrWhiteSpace(WorkExperienceTextBox.Text) ||
                    EducationTypeComboBox.SelectedItem == null ||
                    string.IsNullOrWhiteSpace(SalaryTextBox.Text))
                {
                    MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                  
                    Employee newEmployee = new Employee
                    {
                        EmployeeID = employee.EmployeeID,
                        FullName = FullNameTextBox.Text,
                        Gender = (GenderComboBox.SelectedItem as ComboBoxItem)?.Content.ToString(),
                        DateOfBirth = DateOfBirthDatePicker.SelectedDate ?? DateTime.Now, 
                        Address = (AddressComboBox.SelectedItem as Address)?.Street,
                        SpecializationType = (SpecializationTypeComboBox.SelectedItem as SpecializationType)?.SpecializationName,
                        WorkExperience = int.Parse(WorkExperienceTextBox.Text),
                        EducationType = (EducationTypeComboBox.SelectedItem as EducationType)?.EducationTypeName,

                        Salary = double.Parse(SalaryTextBox.Text),
                        Photo = employee.Photo
                    };

                    employee = newEmployee;
                    DialogResult = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Add_Button(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|All files (*.*)|*.*";

                if (openFileDialog.ShowDialog() == true)
                {
                    string filePath = openFileDialog.FileName;

                    
                    byte[] imageBytes = File.ReadAllBytes(filePath);

                    
                    employee.Photo = imageBytes;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


    }
}
