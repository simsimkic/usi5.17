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
using ZdravoCorp.Domain.Common;
using ZdravoCorp.Domain;
using ZdravoCorp.Presentation.Common;
using ZdravoCorp.Presentation.Nurse.Appointments;
using ZdravoCorp.Services;

namespace ZdravoCorp.Presentation.Nurse
{
    /// <summary>
    /// Interaction logic for NurseWindow.xaml
    /// </summary>
    public partial class NurseWindow : Window
    {
        public NurseWindow()
        {
            InitializeComponent();
            loggedUsernameLabel.Content = Globals.LoggedUser.Username;
        }

        private void LogOutBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new();
            mainWindow.Show();

            Globals.LoggedUser = null;
            this.Close();
        }

        private void UpdatePatientBtn_Click(object sender, RoutedEventArgs e)
        {
            CRUDPatientsWindow crudPatientsWindow = new();
            crudPatientsWindow.Show();

            this.Close();
        }

        private void CheckInPatientBtn_Click(object sender, RoutedEventArgs e)
        {
            PatientCheckInWindow patientCheckInWindow = new();
            patientCheckInWindow.Show();

            this.Close();
        }

        private void EmergencyAppointmentBtn_Click(object sender, RoutedEventArgs e)
        {
            EmergencyAppointmentWindow emergencyAppointmentWindow = new();
            emergencyAppointmentWindow.Show();

            this.Close();
        }
    }
}
