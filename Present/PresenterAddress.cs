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
    public class PresenterAddress : IPresenterCommon
    {
        private Window _view;
        private DataGrid addressGrid;
        private AddressBd addressBd;

        public PresenterAddress(Window view, string bd)
        {
            addressBd = new AddressBd(bd);
            _view = view;
            
            StackPanel GridPanel = _view.FindName("GridPanel") as StackPanel;
            GridPanel.Children.Clear();
            addressGrid = new DataGrid();
            addressGrid.Width = double.NaN;
            addressGrid.CanUserAddRows = false;
            addressGrid.AutoGenerateColumns = false;
            addressGrid.IsReadOnly = true;
            DataGridTextColumn idColumn = new DataGridTextColumn();
            idColumn.Header = "ID";
            idColumn.Binding = new System.Windows.Data.Binding("AddressID");

            DataGridTextColumn streetColumn = new DataGridTextColumn();
            streetColumn.Header = "Улица";
            streetColumn.Binding = new System.Windows.Data.Binding("Street");

           
          

            addressGrid.Columns.Add(idColumn);
            addressGrid.Columns.Add(streetColumn);
            

            addressGrid.ItemsSource = addressBd.Addresses;
            GridPanel.Children.Add(addressGrid);
        }

        public void AddObject()
        {
            string street = GetStreetFromUser("Введите улицу:");
          
            if (!string.IsNullOrEmpty(street))
            {
                AddressBd.Address newAddress = new AddressBd.Address
                {
                    Street = street,
                   
                };

                addressBd.AddAddress(newAddress);
                addressGrid.ItemsSource = addressBd.Addresses;
                addressGrid.Items.Refresh();
            }
        }

        public void EditObject()
        {
            if (addressGrid.SelectedItem != null)
            {
                int index = addressGrid.SelectedIndex;
                AddressBd.Address selectedAddress = addressBd.Addresses[index];

                string newStreet = GetStreetFromUser("Введите новую улицу:", selectedAddress.Street);
               

                if (!string.IsNullOrEmpty(newStreet))
                {
                    selectedAddress.Street = newStreet;
                    

                    addressBd.UpdateAddress(selectedAddress);
                    addressGrid.ItemsSource = addressBd.Addresses;
                    addressGrid.Items.Refresh();
                }
            }
        }

        public void DeleteObject()
        {
            if (addressGrid.SelectedItem != null)
            {
                MessageBoxResult result = MessageBox.Show("Точно хотите удалить адрес?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        int index = addressGrid.SelectedIndex;
                        AddressBd.Address selectedAddress = addressBd.Addresses[index];
                        addressBd.DeleteAddress(selectedAddress.AddressID);
                        addressGrid.ItemsSource = addressBd.Addresses;
                        addressGrid.Items.Refresh();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении адреса: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }



        public void Search(string searchTerm)
        {
            List<AddressBd.Address> filteredList = addressBd.Addresses
                .Where(address =>
                    address.GetType().GetProperties()
                        .Any(prop =>
                            prop.GetValue(address)?.ToString()?.ToLower().Contains(searchTerm.ToLower()) ?? false
                        )
                )
                .ToList();

            addressGrid.ItemsSource = filteredList;
            addressGrid.Items.Refresh();
        }

        private string GetStreetFromUser(string prompt, string defaultValue = "")
        {
            return Microsoft.VisualBasic.Interaction.InputBox(prompt, "User Input", defaultValue);
        }

        private string GetHouseNumberFromUser(string prompt, string defaultValue = "")
        {
            return Microsoft.VisualBasic.Interaction.InputBox(prompt, "User Input", defaultValue);
        }
    }

}
