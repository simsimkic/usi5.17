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
using ZdravoCorp.Presentation.Notifications;
using ZdravoCorp.Services.Equipment;

namespace ZdravoCorp.Presentation.ManagerWindow.Inventory
{
    /// <summary>
    /// Interaction logic for FilterByRoomTypeDialog.xaml
    /// </summary>
    public partial class FilterByRoomTypeDialog : Window
    {
        private ManagerWindow _managerWindow;
        public FilterByRoomTypeDialog(ManagerWindow managerWindow)
        {
            InitializeComponent();
            _managerWindow = managerWindow;
        }



        private void SubmitButtonRoomType_Click(object sender, RoutedEventArgs e)
        {
            if (RoomTypeComboBox.SelectedValue is not null)
            {
                string roomType = RoomTypeComboBox.SelectedValue.ToString();
                _managerWindow.UpdateInventoryTable(InventoryService.GetItemsFilteredByRooms(roomType), _managerWindow.InventoryDataGrid);
                Close();
            }
            else
            {
                Notification.ShowErrorDialog("Please select a room type!");
            }

        }
    }
}
