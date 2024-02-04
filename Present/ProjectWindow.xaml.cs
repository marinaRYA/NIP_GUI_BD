using System;
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
using WpfPresent.BD;
using static WpfPresent.BD.AddressBd;
using static WpfPresent.BD.OrderedBd;
using static WpfPresent.BD.ProjectBd;

namespace WpfPresent.Present
{
    /// <summary>
    /// Логика взаимодействия для ProjectWindow.xaml
    /// </summary>
    public partial class ProjectWindow : Window
    {
        OrderedBd orderedBd;
       
        public Project project;
        public ProjectWindow(Project proj, string bd)
        {
            orderedBd = new OrderedBd(bd);
            project = proj;
            InitializeComponent();
            DisplayProjectDetails();
            CustomerComboBox.ItemsSource = orderedBd.Orders;
            CustomerComboBox.DisplayMemberPath = "Customer";
        }
        private void DisplayProjectDetails()
        {
            if (project != null)
            {
                ProjectNameTextBox.Text = project.ProjectName;
                ProjectCostTextBox.Text = project.ProjectCost.ToString();
                StartDatePicker.SelectedDate = project.StartDate;
                EndDatePicker.SelectedDate = project.EndDate;
                ProjectManagerComboBox.Text = project.ProjectManager;
                ContactPersonTextBox.Text = project.ContactPerson;
                ContactPersonPhoneTextBox.Text = project.ContactPersonPhone;
                BonusRateTextBox.Text = project.BonusRate.ToString();
                Ordered selectedOrder = orderedBd.Orders.FirstOrDefault(order => order.Customer == project.Customer);
                CustomerComboBox.SelectedItem = selectedOrder;



            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
           
            try
            {
                
                if (string.IsNullOrEmpty(ProjectNameTextBox.Text) ||
                    string.IsNullOrEmpty(ProjectCostTextBox.Text) ||
                    StartDatePicker.SelectedDate == null ||
                    EndDatePicker.SelectedDate == null ||
                    string.IsNullOrEmpty(ProjectManagerComboBox.Text) ||
                    string.IsNullOrEmpty(ContactPersonTextBox.Text) ||
                    string.IsNullOrEmpty(ContactPersonPhoneTextBox.Text) ||
                    string.IsNullOrEmpty(BonusRateTextBox.Text) ||
                    CustomerComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    Project newProject = new Project
                    {
                        ProjectID = project.ProjectID,
                        ProjectName = ProjectNameTextBox.Text,
                        ProjectCost = Convert.ToDouble(ProjectCostTextBox.Text),
                        StartDate = StartDatePicker.SelectedDate.Value,
                        EndDate = EndDatePicker.SelectedDate.Value,
                        ProjectManager = ProjectManagerComboBox.Text,
                        ContactPerson = ContactPersonTextBox.Text,
                        ContactPersonPhone = ContactPersonPhoneTextBox.Text,
                        BonusRate = Convert.ToDouble(BonusRateTextBox.Text),
                        Customer = (CustomerComboBox.SelectedItem as OrderedBd.Ordered)?.Customer
                    };
                    project = newProject;
                    DialogResult = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
