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
    public class PresenterSpecializationType : IPresenterCommon
    {
        private Window _view;
        private DataGrid _specializationTypeGrid;
        private SpecializationTypeBd _specializationTypeBd;

        public PresenterSpecializationType(Window view, string bd)
        {
            _specializationTypeBd = new SpecializationTypeBd(bd);
            _view = view;

            StackPanel gridPanel = _view.FindName("GridPanel") as StackPanel;
            gridPanel.Children.Clear();
            _specializationTypeGrid = new DataGrid();
            _specializationTypeGrid.Width = double.NaN;
            _specializationTypeGrid.CanUserAddRows = false;
            _specializationTypeGrid.AutoGenerateColumns = false;
            _specializationTypeGrid.IsReadOnly = true;

            DataGridTextColumn idColumn = new DataGridTextColumn();
            idColumn.Header = "ID";
            idColumn.Binding = new System.Windows.Data.Binding("SpecializationTypeID");

            DataGridTextColumn nameColumn = new DataGridTextColumn();
            nameColumn.Header = "Тип специализации";
            nameColumn.Binding = new System.Windows.Data.Binding("SpecializationName");

            _specializationTypeGrid.Columns.Add(idColumn);
            _specializationTypeGrid.Columns.Add(nameColumn);

            _specializationTypeGrid.ItemsSource = _specializationTypeBd.SpecializationTypes;
            gridPanel.Children.Add(_specializationTypeGrid);
        }

        public void AddObject()
        {
            string typeName = GetSpecializationTypeNameFromUser("Введите тип специализации:");

            if (!string.IsNullOrEmpty(typeName))
            {
                SpecializationTypeBd.SpecializationType newSpecializationType = new SpecializationTypeBd.SpecializationType
                {
                    SpecializationName = typeName
                };

                _specializationTypeBd.AddSpecializationType(newSpecializationType);
                _specializationTypeGrid.ItemsSource = _specializationTypeBd.SpecializationTypes;
                _specializationTypeGrid.Items.Refresh();
            }
        }

        public void EditObject()
        {
            if (_specializationTypeGrid.SelectedItem != null)
            {
                int index = _specializationTypeGrid.SelectedIndex;
                SpecializationTypeBd.SpecializationType selectedSpecializationType = _specializationTypeBd.SpecializationTypes[index];

                string newTypeName = GetSpecializationTypeNameFromUser("Введите новый тип специализации:", selectedSpecializationType.SpecializationName);

                if (!string.IsNullOrEmpty(newTypeName))
                {
                  

                    _specializationTypeBd.UpdateSpecializationType(selectedSpecializationType);
                    _specializationTypeGrid.ItemsSource = _specializationTypeBd.SpecializationTypes;
                    _specializationTypeGrid.Items.Refresh();
                }
            }
        }

        public void DeleteObject()
        {
            if (_specializationTypeGrid.SelectedItem != null)
            {
                MessageBoxResult result = MessageBox.Show("Точно хотите удалить тип специализации?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        int index = _specializationTypeGrid.SelectedIndex;
                        SpecializationTypeBd.SpecializationType selectedSpecializationType = _specializationTypeBd.SpecializationTypes[index];
                        _specializationTypeBd.DeleteSpecializationType(selectedSpecializationType.SpecializationTypeID);
                        _specializationTypeGrid.ItemsSource = _specializationTypeBd.SpecializationTypes;
                        _specializationTypeGrid.Items.Refresh();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении типа специализации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        public void Search(string searchTerm)
        {
            List<SpecializationTypeBd.SpecializationType> filteredList = _specializationTypeBd.SpecializationTypes
                .Where(specType =>
                    specType.GetType().GetProperties()
                        .Any(prop =>
                            prop.GetValue(specType)?.ToString()?.ToLower().Contains(searchTerm.ToLower()) ?? false
                        )
                )
                .ToList();

            _specializationTypeGrid.ItemsSource = filteredList;
            _specializationTypeGrid.Items.Refresh();
        }
        private string GetSpecializationTypeNameFromUser(string prompt, string defaultValue = "")
        {
            return Microsoft.VisualBasic.Interaction.InputBox(prompt, "User Input", defaultValue);
        }
    }

}
