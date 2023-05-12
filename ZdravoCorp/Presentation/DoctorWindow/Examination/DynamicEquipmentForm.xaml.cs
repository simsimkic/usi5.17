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
using ZdravoCorp.Presentation.Notifications;
using ZdravoCorp.Repository.Equipment;
using ZdravoCorp.Services;
using ZdravoCorp.Services.Equipment;

namespace ZdravoCorp.Presentation.DoctorWindow.Examination
{
    /// <summary>
    /// Interaction logic for DynamicEquipmentForm.xaml
    /// </summary>
    public partial class DynamicEquipmentForm : Window
    {
        private readonly string _roomName;
        private InventoryItem _selectedItem;
        public DynamicEquipmentForm(string roomName)
        {
            InitializeComponent();
            _roomName = roomName;
            _selectedItem = null;
        }

        private void SpendButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedItem == null)
                {
                    throw new InvalidOperationException("Equipment must be selected.");
                }

                int spentQuantity = int.Parse(spentQuantityTextBox.Text);
                if (spentQuantity > _selectedItem.Quantity)
                {
                    throw new InvalidOperationException("Not enough items to spend.");
                }

                InventoryService.SpendItem(_selectedItem, spentQuantity);
                currentQuantityLabel.Content = _selectedItem.Quantity.ToString();

            }
            catch (Exception error)
            {
                Notification.ShowErrorDialog(error.Message);
            }
            
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void FillEquipmentComboBox()
        {
            foreach (InventoryItem item in InventoryService.GetDynamicEquipmentInRoom(_roomName))
            {
                equipmentComboBox.Items.Add(item.Equipment.Name);
            }

        }

        private void EquipmentComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedItem = InventoryRepository.GetItem(equipmentComboBox.SelectedValue.ToString(), _roomName);
            currentQuantityLabel.Content = _selectedItem.Quantity.ToString();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FillEquipmentComboBox();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Notification.ShowSuccessDialog("Examination has been finished.");
        }
    }
}
