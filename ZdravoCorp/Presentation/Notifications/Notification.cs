using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZdravoCorp.Presentation.Notifications
{
    public static class Notification
    {
        public static void ShowErrorDialog(string message)
        {
            ErrorDialog errorDialog = new(message);
            errorDialog.ShowDialog();
        }

        public static void ShowSuccessDialog(string message)
        {
            SuccessDialog successDialog = new(message);
            successDialog.ShowDialog();
        }

        public static void ShowWarningDialog(string message)
        {
            WarningDialog warningDialog = new(message);
            warningDialog.ShowDialog();
        }
    }
}
