using System;
using System.Collections.Generic;
using System.Data;
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
using ZdravoCorp.Domain.Common;
using ZdravoCorp.Domain.Equipment;
using ZdravoCorp.Domain.Orders;
using ZdravoCorp.Domain.Room;
using ZdravoCorp.Presentation.ManagerWindow.Inventory;
using ZdravoCorp.Presentation.ManagerWindow.Orders;
using ZdravoCorp.Presentation.Notifications;
using ZdravoCorp.Repository.Equipment;
using ZdravoCorp.Services.Equipment;
using ZdravoCorp.Services.Orders;
using System.Windows.Threading;
using FluentScheduler;

namespace ZdravoCorp.Presentation.ManagerWindow
{
    /// <summary>
    /// Interaction logic for ManagerWindow.xaml
    /// </summary>
    public partial class ManagerWindow : Window
    {
        private DispatcherTimer _datagGridTimer;
        private static readonly object itemSourceLock = new object();
        public ManagerWindow()
        {
            InitializeComponent();

            LoggedInLabel.Content = "Logged in: " + Globals.LoggedUser.Username;

            _datagGridTimer = new DispatcherTimer();
            _datagGridTimer.Interval = TimeSpan.FromSeconds(41);
            _datagGridTimer.Tick += DataGridTick;
            _datagGridTimer.Start();
        }

        private void DataGridTick(object sender, EventArgs e)
        {
            try
            {
                UpdateInventoryTable(InventoryService.GetInventoryItems(), InventoryDataGrid);
                UpdateInventoryTable(InventoryService.GetMissingItems(), MissingInventoryDataGrid);
                UpdateOrdersTable(OrdersService.GetOrders(), OrdersDataGrid);
            }
            catch (Exception ex)
            {
                return;
            }
        }


        public void UpdateInventoryTable(List<InventoryItem> inventoryItems, DataGrid dataGrid)
        {
            lock (itemSourceLock)
            {
                dataGrid.ItemsSource = inventoryItems;
            }
        }
        
        public void UpdateOrdersTable(List<OrderItem> orderItems, DataGrid dataGrid)
        {
            lock (itemSourceLock)
            {
                dataGrid.ItemsSource = orderItems;
            }
        }

        private void InventoryDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateInventoryTable(InventoryService.GetInventoryItems(),InventoryDataGrid);
        }

        private void FilterByRoomTypeButton_Click(object sender, RoutedEventArgs e)
        {
            FilterByRoomTypeDialog filterByRoomTypeDialog = new(this);
            filterByRoomTypeDialog.ShowDialog();
        }

        private void FilterByEquipmentTypeButton_Click(object sender, RoutedEventArgs e)
        {
            FilterByEquipmentTypeDialog filterByEquipmentTypeDialog = new(this);
            filterByEquipmentTypeDialog.ShowDialog();
        }

        private void FilterByQuantityButton_Click(object sender, RoutedEventArgs e)
        {
            FilterByQuantityDialog filterByQuantityDialog = new(this);
            filterByQuantityDialog.ShowDialog();
        }

        private void FilterNotInStorageButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateInventoryTable(InventoryService.GetItemsNotInStorage(), InventoryDataGrid);
        }

        private void ResetFilterButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateInventoryTable(InventoryService.GetInventoryItems(), InventoryDataGrid);
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateInventoryTable(InventoryService.GetSearchItems(SearchBox.Text), InventoryDataGrid);
        }

        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new();
            mainWindow.Show();
            Globals.LoggedUser = null;
            this.Close();
        }

        
        private void MissingInventoryDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateInventoryTable(InventoryService.GetMissingItems(), MissingInventoryDataGrid);
        }

        private void OrdersDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateOrdersTable(OrdersService.GetOrders(), OrdersDataGrid);
        }


        private InventoryItem? GetSelectedItem(DataGrid dataGrid)
        {
           return (InventoryItem)dataGrid.SelectedItem;
        }
        
        private void OrderMissingItemsButton_Click(object sender, RoutedEventArgs e)
        {
            InventoryItem item = GetSelectedItem(MissingInventoryDataGrid);
            if (item != null)
            {
                OrderItemDialog orderItemDialog = new OrderItemDialog(this,item);
                orderItemDialog.ShowDialog();
            }
            else
            {
                Notification.ShowErrorDialog("Please select an item to order!");
            }
        }

        public void OrderItem(InventoryItem item)
        {
            OrdersService.MakeOrder(item);
        }

        public void TransferItem(InventoryItem item,Room destinationRoom, DateTime moveTime)
        {
            TransferItemService.TransferItemRequest(item, destinationRoom, moveTime);
        }

        private void RefreshOrdersButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateOrdersTable(OrdersService.GetOrders(),OrdersDataGrid);
        }

        private void TransferItemButton_Click(object sender, RoutedEventArgs e)
        {
            InventoryItem item = GetSelectedItem(InventoryDataGrid);
            if (item != null)
            {
                TransferItemDialog orderItemDialog = new TransferItemDialog(this, item);
                orderItemDialog.ShowDialog();
            }
            else
            {
                Notification.ShowErrorDialog("Please select an item to transfer!");
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            JobManager.Stop();
        }
    }
}
