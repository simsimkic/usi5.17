using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ZdravoCorp.Services.Equipment;

namespace ZdravoCorp.Presentation.ManagerWindow.Inventory
{
    /// <summary>
    /// Interaction logic for FilterByQuantityDialog.xaml
    /// </summary>
    public partial class FilterByQuantityDialog : Window
    {
        private ManagerWindow _managerWindow;
        public FilterByQuantityDialog(ManagerWindow managerWindow)
        {
            InitializeComponent();
            _managerWindow = managerWindow;
        }


        private void PreviewNumberInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = IsTextAllowed(e.Text);
        }

        public static bool IsTextAllowed(string text)
        {
            Regex regex = new("[^0-9]+");
            return regex.IsMatch(text);
        }
        
        private void SubmitButtonRangeScan_Click(object sender, RoutedEventArgs e)
        {
            var lowerRange = int.MinValue;
            var upperRange = int.MaxValue;
            if (!string.IsNullOrEmpty(LowerRangeTextBox.Text) && int.TryParse(LowerRangeTextBox.Text, out _))
            {
                lowerRange = int.Parse(LowerRangeTextBox.Text);
            }
            if (!string.IsNullOrEmpty(UpperRangeTextBox.Text) && int.TryParse(LowerRangeTextBox.Text, out _))
            {
                upperRange = int.Parse(UpperRangeTextBox.Text);
            }

            _managerWindow.UpdateInventoryTable(InventoryService.GetItemsFilteredByQuantityRange(lowerRange, upperRange), _managerWindow.InventoryDataGrid);
            Close();
        }

        private void SubmitButtonSingleScan_Click(object sender, RoutedEventArgs e)
        {
            var quantity = 0;
            if (!string.IsNullOrEmpty(SingleScanTextBox.Text) && int.TryParse(SingleScanTextBox.Text, out _))
            {
                quantity = int.Parse(SingleScanTextBox.Text);
            }
            _managerWindow.UpdateInventoryTable(InventoryService.GetItemsFilteredByQuantity(quantity), _managerWindow.InventoryDataGrid);
            Close();
        }
    }
}
