using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using WpfPresent.BD;
using static WpfPresent.BD.BankBd;

namespace WpfPresent.Present
{
    public class PresenterBank : IPresenterCommon
    {
        private Window _view;
        private DataGrid _bankGrid;
        private BankBd _bankBd;

        public PresenterBank(Window view, string bd)
        {
            _bankBd = new BankBd(bd);
            _view = view;

            StackPanel gridPanel = _view.FindName("GridPanel") as StackPanel;
             gridPanel.Children.Clear();
            _bankGrid = new DataGrid();
            _bankGrid.Width = double.NaN;
            _bankGrid.CanUserAddRows = false;
            _bankGrid.AutoGenerateColumns = false;
            _bankGrid.IsReadOnly = true;

            DataGridTextColumn idColumn = new DataGridTextColumn();
            idColumn.Header = "ID";
            idColumn.Binding = new System.Windows.Data.Binding("BankID");

            DataGridTextColumn nameColumn = new DataGridTextColumn();
            nameColumn.Header = "Название банка";
            nameColumn.Binding = new System.Windows.Data.Binding("BankName");

            _bankGrid.Columns.Add(idColumn);
            _bankGrid.Columns.Add(nameColumn);

            _bankGrid.ItemsSource = _bankBd.Banks;
            gridPanel.Children.Add(_bankGrid);
        }

        public void AddObject()
        {
            string bankName = GetBankNameFromUser("Введите название банка:");

            if (!string.IsNullOrEmpty(bankName))
            {
                Bank newBank = new Bank
                {
                    BankName = bankName
                };

                _bankBd.AddBank(newBank);
                _bankGrid.ItemsSource = _bankBd.Banks;
                _bankGrid.Items.Refresh();
            }
        }

        public void EditObject()
        {
            if (_bankGrid.SelectedItem != null)
            {
                int index = _bankGrid.SelectedIndex;
                Bank selectedBank = _bankBd.Banks[index];

                string newBankName = GetBankNameFromUser("Введите новое название банка:", selectedBank.BankName);

                if (!string.IsNullOrEmpty(newBankName))
                {
                    

                    _bankBd.UpdateBank(selectedBank);
                    _bankGrid.ItemsSource = _bankBd.Banks;
                    _bankGrid.Items.Refresh();
                }
            }
        }

        public void DeleteObject()
        {
            if (_bankGrid.SelectedItem != null)
            {
                MessageBoxResult result = MessageBox.Show("Точно хотите удалить банк?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        int index = _bankGrid.SelectedIndex;
                        BankBd.Bank selectedBank = _bankBd.Banks[index];
                        _bankBd.DeleteBank(selectedBank.BankID);
                        _bankGrid.ItemsSource = _bankBd.Banks;
                        _bankGrid.Items.Refresh();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении банка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        public void Search(string searchTerm)
        {
            List<BankBd.Bank> filteredList = _bankBd.Banks
                .Where(bank =>
                    bank.GetType().GetProperties()
                        .Any(prop =>
                            prop.GetValue(bank)?.ToString()?.ToLower().Contains(searchTerm.ToLower()) ?? false
                        )
                )
                .ToList();

            _bankGrid.ItemsSource = filteredList;
            _bankGrid.Items.Refresh();
        }



        private string GetBankNameFromUser(string prompt, string defaultValue = "")
        {
            return Microsoft.VisualBasic.Interaction.InputBox(prompt, "User Input", defaultValue);
        }
    }

}
