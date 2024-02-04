using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using WpfPresent.BD;

namespace WpfPresent.Present
{
    public class PresenterEducationType : IPresenterCommon
    {
        private Window _view;
        private DataGrid _educationTypeGrid;
        private EducationTypeBd _educationTypeBd;

        public PresenterEducationType(Window view, string bd)
        {
            _educationTypeBd = new EducationTypeBd(bd);
            _view = view;

            StackPanel gridPanel = _view.FindName("GridPanel") as StackPanel;
            gridPanel.Children.Clear();
           
            _educationTypeGrid = new DataGrid();
            _educationTypeGrid.Width = double.NaN;
            _educationTypeGrid.CanUserAddRows = false;
            _educationTypeGrid.AutoGenerateColumns = false;
            _educationTypeGrid.IsReadOnly = true;

            DataGridTextColumn idColumn = new DataGridTextColumn();
            idColumn.Header = "ID";
            idColumn.Binding = new System.Windows.Data.Binding("EducationTypeID");

            DataGridTextColumn nameColumn = new DataGridTextColumn();
            nameColumn.Header = "Тип образования";
            nameColumn.Binding = new System.Windows.Data.Binding("EducationTypeName");

            _educationTypeGrid.Columns.Add(idColumn);
            _educationTypeGrid.Columns.Add(nameColumn);

            _educationTypeGrid.ItemsSource = _educationTypeBd.EducationTypes;
            gridPanel.Children.Add(_educationTypeGrid);
        }

        public void AddObject()
        {
            string typeName = GetEducationTypeNameFromUser("Введите тип образования:");

            if (!string.IsNullOrEmpty(typeName))
            {
                EducationTypeBd.EducationType newEducationType = new EducationTypeBd.EducationType
                {
                    EducationTypeName = typeName
                };

                _educationTypeBd.AddEducationType(newEducationType);
                _educationTypeGrid.ItemsSource = _educationTypeBd.EducationTypes;
                _educationTypeGrid.Items.Refresh();
            }
        }

        public void EditObject()
        {
            if (_educationTypeGrid.SelectedItem != null)
            {
                int index = _educationTypeGrid.SelectedIndex;
                EducationTypeBd.EducationType selectedEducationType = _educationTypeBd.EducationTypes[index];

                string newTypeName = GetEducationTypeNameFromUser("Введите новый тип образования:", selectedEducationType.EducationTypeName);

                if (!string.IsNullOrEmpty(newTypeName))
                {
                   

                    _educationTypeBd.UpdateEducationType(selectedEducationType);
                    _educationTypeGrid.ItemsSource = _educationTypeBd.EducationTypes;
                    _educationTypeGrid.Items.Refresh();
                }
            }
        }

        public void DeleteObject()
        {
            if (_educationTypeGrid.SelectedItem != null)
            {
                MessageBoxResult result = MessageBox.Show("Точно хотите удалить тип образования?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        int index = _educationTypeGrid.SelectedIndex;
                        EducationTypeBd.EducationType selectedEducationType = _educationTypeBd.EducationTypes[index];
                        _educationTypeBd.DeleteEducationType(selectedEducationType.EducationTypeID);
                        _educationTypeGrid.ItemsSource = _educationTypeBd.EducationTypes;
                        _educationTypeGrid.Items.Refresh();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении типа образования: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        public void Search(string searchTerm)
        {
            List<EducationTypeBd.EducationType> filteredList = _educationTypeBd.EducationTypes
                .Where(educationType =>
                    educationType.GetType().GetProperties()
                        .Any(prop =>
                            prop.GetValue(educationType)?.ToString()?.ToLower().Contains(searchTerm.ToLower()) ?? false
                        )
                )
                .ToList();

            _educationTypeGrid.ItemsSource = filteredList;
            _educationTypeGrid.Items.Refresh();
        }

        private string GetEducationTypeNameFromUser(string prompt, string defaultValue = "")
        {
            return Microsoft.VisualBasic.Interaction.InputBox(prompt, "User Input", defaultValue);
        }
    }

}
