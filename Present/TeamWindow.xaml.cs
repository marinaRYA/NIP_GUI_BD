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
using static WpfPresent.BD.TeamCompositionBd;

namespace WpfPresent.Present
{
    /// <summary>
    /// Логика взаимодействия для TeamWindow.xaml
    /// </summary>
    public partial class TeamWindow : Window
    {
        private EmployeesBd employeesBd;
        public TeamCompositionBd.TeamComposition team;
        public TeamWindow(string bd, TeamComposition team)
        {
            employeesBd = new EmployeesBd(bd);
            this.team = team;
            InitializeComponent();
            EmployeeComboBox.ItemsSource = employeesBd.Employees;
            EmployeeComboBox.DisplayMemberPath = "FullName";
            DisplayTeamDetails();

        }
        private void DisplayTeamDetails()
        {
            if (team != null)
            {
                
                EmployeeComboBox.SelectedItem = employeesBd.Employees.FirstOrDefault(emp => emp.FullName == team.FullName);

               
                StartDatePicker.SelectedDate = team.StartDate;

                
                EndDatePicker.SelectedDate = team.EndDate;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (EmployeeComboBox.SelectedItem == null ||
                    !StartDatePicker.SelectedDate.HasValue ||
                    !EndDatePicker.SelectedDate.HasValue)
                {
                    MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                   
                    TeamComposition updatedTeam = new TeamComposition
                    {
                        TeamId = team.TeamId,
                        FullName = (EmployeeComboBox.SelectedItem as EmployeesBd.Employee)?.FullName,
                        StartDate = StartDatePicker.SelectedDate.Value,
                        EndDate = EndDatePicker.SelectedDate.Value
                    };

                    team = updatedTeam;
                  

                    
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
