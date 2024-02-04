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
    public class PresenterOrdered : IPresenterCommon
    {
        private Window _view;
        private DataGrid orderGrid;
        private OrderedBd orderBd;
        private string BD;
        public PresenterOrdered(Window view, string bd)
        {
            BD = bd;
            orderBd = new OrderedBd(bd);
            _view = view;

            StackPanel gridPanel = _view.FindName("GridPanel") as StackPanel;
            gridPanel.Children.Clear();
            orderGrid = new DataGrid();
            orderGrid.Width = double.NaN;
            orderGrid.CanUserAddRows = false;
            orderGrid.AutoGenerateColumns = false;
            orderGrid.IsReadOnly = true;
            DataGridTextColumn idColumn = new DataGridTextColumn();
            idColumn.Header = "ID";
            idColumn.Binding = new System.Windows.Data.Binding("OrderID");

            DataGridTextColumn customerColumn = new DataGridTextColumn();
            customerColumn.Header = "Заказчик";
            customerColumn.Binding = new System.Windows.Data.Binding("Customer");

            DataGridTextColumn accountNumberColumn = new DataGridTextColumn();
            accountNumberColumn.Header = "Номер счета";
            accountNumberColumn.Binding = new System.Windows.Data.Binding("AccountNumber");

            DataGridTextColumn customerINNColumn = new DataGridTextColumn();
            customerINNColumn.Header = "ИНН заказчика";
            customerINNColumn.Binding = new System.Windows.Data.Binding("CustomerINN");

            DataGridTextColumn bankColumn = new DataGridTextColumn();
            bankColumn.Header = "Банк";
            bankColumn.Binding = new System.Windows.Data.Binding("BankName");

            DataGridTextColumn addressColumn = new DataGridTextColumn();
            addressColumn.Header = "Адрес";
            addressColumn.Binding = new System.Windows.Data.Binding("Street");
            DataGridTextColumn houseColumn = new DataGridTextColumn();
            

            orderGrid.Columns.Add(idColumn);
            orderGrid.Columns.Add(customerColumn);
            orderGrid.Columns.Add(accountNumberColumn);
            orderGrid.Columns.Add(customerINNColumn);
            orderGrid.Columns.Add(bankColumn);
            orderGrid.Columns.Add(addressColumn);
            orderGrid.ItemsSource = orderBd.Orders;
            gridPanel.Children.Add(orderGrid);
        }



        public void AddObject()
        {
            OrderedWindow orderedWindow = new OrderedWindow(new OrderedBd.Ordered(),BD);
            orderedWindow.ShowDialog();

            if (orderedWindow.DialogResult == true)
            {
                orderBd.AddOrder(orderedWindow.ordered);
                orderGrid.ItemsSource = orderBd.Orders;
                orderGrid.Items.Refresh();
            }
        }

        public void EditObject()
        {
            if (orderGrid.SelectedItem != null)
            {
                int index = orderGrid.SelectedIndex;
                OrderedBd.Ordered selectedOrdered = orderGrid.SelectedItem as OrderedBd.Ordered;
                OrderedWindow orderedWindow = new OrderedWindow(selectedOrdered, BD);
                orderedWindow.ShowDialog();

                if (orderedWindow.DialogResult == true)
                {
                    OrderedBd.Ordered updatedOrdered = orderedWindow.ordered;
                    orderBd.UpdateOrder(updatedOrdered);
                    orderGrid.ItemsSource = orderBd.Orders;
                    orderGrid.Items.Refresh();
                }
            }

        }


        public void DeleteObject()
        {
            if (orderGrid.SelectedItem != null)
            {
                MessageBoxResult result = MessageBox.Show("Точно хотите удалить заказчика?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        int index = orderGrid.SelectedIndex;
                        OrderedBd.Ordered selectedOrder = orderGrid.SelectedItem as OrderedBd.Ordered;
                        orderBd.DeleteOrder(selectedOrder.OrderID);
                        orderGrid.ItemsSource = orderBd.Orders;
                        orderGrid.Items.Refresh();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении заказчика: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        public void Search(string searchTerm)
        {
            List<OrderedBd.Ordered> filteredList = orderBd.Orders
                .Where(order =>
                    order.GetType().GetProperties()
                        .Any(prop =>
                            prop.GetValue(order)?.ToString()?.ToLower().Contains(searchTerm.ToLower()) ?? false
                        )
                )
                .ToList();

            orderGrid.ItemsSource = filteredList;
            orderGrid.Items.Refresh();
        }

    }
}


