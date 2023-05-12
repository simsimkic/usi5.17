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
using ZdravoCorp.Domain.Equipment;
using ZdravoCorp.Presentation.ManagerWindow.Inventory;
using ZdravoCorp.Presentation.Notifications;
using ZdravoCorp.Services;

namespace ZdravoCorp.Presentation.ManagerWindow.Orders
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class OrderItemDialog : Window
    {
        private ManagerWindow _managerWindow;
        private InventoryItem _item;

        public OrderItemDialog(ManagerWindow managerWindow,InventoryItem item)
        {
            _item = item;
            _managerWindow = managerWindow;
            InitializeComponent();
        }

        private void PreviewNumberInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = FilterByQuantityDialog.IsTextAllowed(e.Text);
        }


        private void SubmitOrderButton_Click(object sender, RoutedEventArgs e)
        {
            var quantity = 1;
            if (!string.IsNullOrEmpty(OrderQuantityTextBox.Text) && int.TryParse(OrderQuantityTextBox.Text, out _))
            {
                quantity = int.Parse(OrderQuantityTextBox.Text);
                if (quantity > 0)
                {
                    _item.Quantity = quantity;
                    _managerWindow.OrderItem(_item);
                    Close();
                }
                else
                {
                    Notification.ShowErrorDialog("Please select a valid quantity!");
                }
            }
        }
    }
}
