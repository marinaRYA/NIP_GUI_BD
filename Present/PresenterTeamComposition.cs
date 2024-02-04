using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using WpfPresent.BD;
using static WpfPresent.BD.ProjectBd;
using System.Windows.Data;
using static WpfPresent.BD.TeamCompositionBd;

namespace WpfPresent.Present
{
    public class PresenterTeamComposition : IPresenterCommon
    {
        private Window _view;
        private DataGrid _teamCompositionGrid;
        private TeamCompositionBd TeamBd = null;
        private ProjectBd projectBd;
        private string BD;
        ComboBox projectComboBox;

        public PresenterTeamComposition(Window view, string bd)
        {
            BD = bd;
            projectBd = new ProjectBd(bd);
            _view = view;

            StackPanel gridPanel = _view.FindName("GridPanel") as StackPanel;
            gridPanel.Children.Clear();
            projectComboBox = new ComboBox();
            projectComboBox.Width = 600;
            projectComboBox.ItemsSource = projectBd.Projects;
            projectComboBox.DisplayMemberPath = "ProjectName";
            gridPanel.Children.Add(projectComboBox);
            Button applyButton = new Button();
            applyButton.Margin = new Thickness(250, 0, 250, 0);
            applyButton.Content = "Применить";
            applyButton.Click += ApplyButton_Click;
            gridPanel.Children.Add(applyButton);

            _teamCompositionGrid = new DataGrid();
            _teamCompositionGrid.Width = double.NaN;
            _teamCompositionGrid.CanUserAddRows = false;
            _teamCompositionGrid.AutoGenerateColumns = false;
            _teamCompositionGrid.IsReadOnly = true;

            DataGridTextColumn startDateColumn = new DataGridTextColumn();
            startDateColumn.Header = "Дата начала";
            startDateColumn.Binding = new Binding("StartDate")
            {
                StringFormat = "dd.MM.yyyy"
            };

            DataGridTextColumn endDateColumn = new DataGridTextColumn();
            endDateColumn.Header = "Дата окончания";
            endDateColumn.Binding = new Binding("EndDate")
            {
                StringFormat = "dd.MM.yyyy"
            };

            DataGridTextColumn fullNameColumn = new DataGridTextColumn();
            fullNameColumn.Header = "Полное имя";
            fullNameColumn.Binding = new Binding("FullName");

            _teamCompositionGrid.Columns.Add(fullNameColumn);
            _teamCompositionGrid.Columns.Add(startDateColumn);
            _teamCompositionGrid.Columns.Add(endDateColumn);

            gridPanel.Children.Add(_teamCompositionGrid);
        }

        public void AddObject()
        {
            if (TeamBd !=null)
            {
                
              
                TeamWindow teamWindow = new TeamWindow(BD, new TeamCompositionBd.TeamComposition());
                teamWindow.ShowDialog();

              
                if (teamWindow.DialogResult == true)
                {
                   TeamBd.AddEmployeeToTeam(teamWindow.team);
                    _teamCompositionGrid.ItemsSource = TeamBd.TeamCompositions;
                    _teamCompositionGrid.Items.Refresh();
                }
            }
            else
            {
                MessageBox.Show("Нет данных.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public void EditObject()
        {
            if ( _teamCompositionGrid.SelectedItems.Count > 0)
            {

                TeamComposition selectedItem = _teamCompositionGrid.SelectedItem as TeamComposition;

               
                TeamWindow teamWindow = new TeamWindow(BD, selectedItem);
                teamWindow.ShowDialog();

               
                if (teamWindow.DialogResult == true)
                {

                    TeamBd.UpdateEmployeeInTeam(teamWindow.team);
                     _teamCompositionGrid.ItemsSource = TeamBd.TeamCompositions;
                    _teamCompositionGrid.Items.Refresh();
                }
            }
            else
            {
                MessageBox.Show("Нет данных или не выбран элемент.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public void DeleteObject()
        {
            if ( _teamCompositionGrid.SelectedItems.Count > 0)
            {
                TeamComposition selectedItem = _teamCompositionGrid.SelectedItem as TeamComposition;
                MessageBoxResult result = MessageBox.Show("Точно хотите удалить этого соотрудника?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {

                    if (selectedItem != null)
                    {
                        try
                        {
                            TeamBd.RemoveEmployeeFromTeam(selectedItem.FullName);
                            _teamCompositionGrid.ItemsSource = TeamBd.TeamCompositions;
                            _teamCompositionGrid.Items.Refresh();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка при удалении типа специализации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    
                     }
            }
            else
            {
                MessageBox.Show("Нет данных или не выбран элемент.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        public void Search(string searchTerm)
        {
            if (_teamCompositionGrid.Items.Count > 0)
            {
                
                var filteredList = TeamBd.TeamCompositions.Where(team => team.FullName.Contains(searchTerm)).ToList();

               
                _teamCompositionGrid.ItemsSource = filteredList;
                _teamCompositionGrid.Items.Refresh();
            }
            else
            {
                MessageBox.Show("Нет данных.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        public void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            if (projectComboBox.SelectedItem != null)
            {
                Project selectedProject = projectComboBox.SelectedItem as Project;
                TeamBd = new TeamCompositionBd(BD, selectedProject.ProjectName);
                _teamCompositionGrid.ItemsSource = TeamBd.TeamCompositions;
                _teamCompositionGrid.Items.Refresh();
            }
            else
            {
                MessageBox.Show("Выберите проект перед применением.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }

}