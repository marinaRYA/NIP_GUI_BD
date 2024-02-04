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

namespace WpfPresent.Present
{
   
    public partial class OrderedWindow : Window
    {
        AddressBd addressBd;
        BankBd bankBd;
        public OrderedBd.Ordered ordered;
        public OrderedWindow(OrderedBd.Ordered ord, string bd)
        {
            ordered = ord;
            addressBd = new AddressBd(bd);
            bankBd = new BankBd(bd);    
            InitializeComponent();
            BankComboBox.ItemsSource = bankBd.Banks;
            BankComboBox.DisplayMemberPath = "BankName";

            AddressComboBox.ItemsSource = addressBd.Addresses;
            AddressComboBox.DisplayMemberPath = "Street";
            DisplayOrderedDetails();
        }
        private void DisplayOrderedDetails()
        {

            if (ordered != null)
            {
                CustomerTextBox.Text = ordered.Customer;
                AccountNumberTextBox.Text = ordered.AccountNumber;
                CustomerINNTextBox.Text = ordered.CustomerINN;
                BankBd.Bank selectedBank = bankBd.Banks.FirstOrDefault(bank => bank.BankName == ordered.BankName);
                BankComboBox.SelectedItem = selectedBank;
                AddressBd.Address selectedAddress = addressBd.Addresses.FirstOrDefault(address => address.Street == ordered.Street);
                AddressComboBox.SelectedItem = selectedAddress;
            }
        }
        private void CreateOrderButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CustomerTextBox.Text.Length == 0 ||
        AccountNumberTextBox.Text.Length == 0 ||
        CustomerINNTextBox.Text.Length == 0 ||
        BankComboBox.SelectedItem == null ||
        AddressComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    // Создание нового объекта Ordered
                    OrderedBd.Ordered newOrder = new OrderedBd.Ordered
                    {
                        OrderID = ordered.OrderID,
                        Customer = CustomerTextBox.Text,
                        AccountNumber = AccountNumberTextBox.Text,
                        CustomerINN = CustomerINNTextBox.Text,
                        BankName = (BankComboBox.SelectedItem as BankBd.Bank)?.BankName,
                        Street = (AddressComboBox.SelectedItem as AddressBd.Address)?.Street,
                        
                    };

                   
                    ordered = newOrder;

                   
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
