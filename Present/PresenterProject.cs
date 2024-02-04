using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using WpfPresent.BD;
using System.Windows.Data;

namespace WpfPresent.Present
{
    public class PresenterProject : IPresenterCommon
    {
        private Window _view;
        private DataGrid projectGrid;
        private ProjectBd projectBd;
        private string BD;

        public PresenterProject(Window view, string bd)
        {
            BD = bd;
            projectBd = new ProjectBd(bd);
            _view = view;

            StackPanel gridPanel = _view.FindName("GridPanel") as StackPanel;
            gridPanel.Children.Clear();
            projectGrid = new DataGrid();
            projectGrid.Width = double.NaN;
            projectGrid.CanUserAddRows = false;
            projectGrid.AutoGenerateColumns = false;
            projectGrid.IsReadOnly = true;

            DataGridTextColumn idColumn = new DataGridTextColumn();
            idColumn.Header = "ID";
            idColumn.Binding = new System.Windows.Data.Binding("ProjectID");

            DataGridTextColumn nameColumn = new DataGridTextColumn();
            nameColumn.Header = "Название проекта";
            nameColumn.Binding = new System.Windows.Data.Binding("ProjectName");

            DataGridTextColumn costColumn = new DataGridTextColumn();
            costColumn.Header = "Стоимость проекта";
            costColumn.Binding = new System.Windows.Data.Binding("ProjectCost");

            DataGridTextColumn startDateColumn = new DataGridTextColumn();
            startDateColumn.Header = "Дата начала";
            startDateColumn.Binding = new Binding("StartDate")
            {
                StringFormat = "dd.MM.yyyy"
            };

            DataGridTextColumn endDateColumn = new DataGridTextColumn();
            endDateColumn.Header = "Дата окончания";
            endDateColumn.Binding =new  Binding("EndDate")
            {
                StringFormat = "dd.MM.yyyy"
            };

            DataGridTextColumn managerColumn = new DataGridTextColumn();
            managerColumn.Header = "Руководитель проекта";
            managerColumn.Binding = new System.Windows.Data.Binding("ProjectManager");

            DataGridTextColumn contactPersonColumn = new DataGridTextColumn();
            contactPersonColumn.Header = "Контактное лицо";
            contactPersonColumn.Binding = new System.Windows.Data.Binding("ContactPerson");

            DataGridTextColumn contactPersonPhoneColumn = new DataGridTextColumn();
            contactPersonPhoneColumn.Header = "Телефон контактного лица";
            contactPersonPhoneColumn.Binding = new System.Windows.Data.Binding("ContactPersonPhone");

            DataGridTextColumn bonusRateColumn = new DataGridTextColumn();
            bonusRateColumn.Header = "Бонусная ставка";
            bonusRateColumn.Binding = new System.Windows.Data.Binding("BonusRate");

            DataGridTextColumn customerColumn = new DataGridTextColumn();
            customerColumn.Header = "Заказчик";
            customerColumn.Binding = new System.Windows.Data.Binding("Customer");

            projectGrid.Columns.Add(idColumn);
            projectGrid.Columns.Add(nameColumn);
            projectGrid.Columns.Add(costColumn);
            projectGrid.Columns.Add(startDateColumn);
            projectGrid.Columns.Add(endDateColumn);
            projectGrid.Columns.Add(managerColumn);
            projectGrid.Columns.Add(contactPersonColumn);
            projectGrid.Columns.Add(contactPersonPhoneColumn);
            projectGrid.Columns.Add(bonusRateColumn);
            projectGrid.Columns.Add(customerColumn);

            projectGrid.ItemsSource = projectBd.Projects;
            gridPanel.Children.Add(projectGrid);
        }

        public void AddObject()
        {
            ProjectWindow projectWindow = new ProjectWindow(new ProjectBd.Project(), BD);
            projectWindow.ShowDialog();

            if (projectWindow.DialogResult == true)
            {
                projectBd.AddProject(projectWindow.project);
                projectGrid.ItemsSource = projectBd.Projects;
                projectGrid.Items.Refresh();
            }
        }

        public void EditObject()
        {
            if (projectGrid.SelectedItem != null)
            {
                int index = projectGrid.SelectedIndex;
                ProjectBd.Project selectedProject = projectGrid.SelectedItem as ProjectBd.Project;
                ProjectWindow projectWindow = new ProjectWindow(selectedProject, BD);
                projectWindow.ShowDialog();

                if (projectWindow.DialogResult == true)
                {
                   
                    projectBd.UpdateProject(projectWindow.project);
                    projectGrid.ItemsSource = projectBd.Projects;
                    projectGrid.Items.Refresh();
                }
            }
        }

        public void DeleteObject()
        {
            if (projectGrid.SelectedItem != null)
            {
                MessageBoxResult result = MessageBox.Show("Точно хотите удалить проект?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        int index = projectGrid.SelectedIndex;
                        ProjectBd.Project selectedProject = projectGrid.SelectedItem as ProjectBd.Project;
                        projectBd.DeleteProject(selectedProject.ProjectID);
                        projectGrid.ItemsSource = projectBd.Projects;
                        projectGrid.Items.Refresh();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении проекта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        public void Search(string searchTerm)
        {
            List<ProjectBd.Project> filteredList = projectBd.Projects
                .Where(project =>
                    project.GetType().GetProperties()
                        .Any(prop =>
                            prop.GetValue(project)?.ToString()?.ToLower().Contains(searchTerm.ToLower()) ?? false
                        )
                )
                .ToList();

            projectGrid.ItemsSource = filteredList;
            projectGrid.Items.Refresh();
        }
    }

}
