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
    /// Interaction logic for FilterByEquipmentTypeDialog.xaml
    /// </summary>
    public partial class FilterByEquipmentTypeDialog : Window
    {
        private ManagerWindow _managerWindow;
        public FilterByEquipmentTypeDialog(ManagerWindow managerWindow)
        {
            InitializeComponent();
            _managerWindow = managerWindow;
        }

        private void SubmitButtonEquipmentType_Click(object sender, RoutedEventArgs e)
        {
            if (EquipmentTypeComboBox.SelectedValue is not null)
            {
                string equipmentType = EquipmentTypeComboBox.SelectedValue.ToString();
                _managerWindow.UpdateInventoryTable(InventoryService.GetItemsFilteredByEquipment(equipmentType), _managerWindow.InventoryDataGrid);
                Close();
            }
            else
            {
                Notification.ShowErrorDialog("Please select an equipment type!");
            }
        }
    }
}
