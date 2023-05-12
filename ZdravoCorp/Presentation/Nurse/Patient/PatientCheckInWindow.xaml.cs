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
using ZdravoCorp.Domain;
using ZdravoCorp.Domain.Calendar;
using ZdravoCorp.Domain.Common;
using ZdravoCorp.Domain.Users;
using ZdravoCorp.Presentation.Common;
using ZdravoCorp.Presentation.DoctorWindow.Appointments;
using ZdravoCorp.Presentation.Notifications;
using ZdravoCorp.Repository;
using ZdravoCorp.Services;

namespace ZdravoCorp.Presentation.Nurse
{
    /// <summary>
    /// Interaction logic for PatientCheckInWindow.xaml
    /// </summary>
    public partial class PatientCheckInWindow : Window
    {
        private int _appointmentId;
        public PatientCheckInWindow()
        {
            InitializeComponent();
            loggedUsernameLabel.Content = Globals.LoggedUser.Username;
            this.Show();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            NurseWindow nurseWindow = new();
            nurseWindow.Show();
            this.Close();
        }

        private void LogOutBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new();
            mainWindow.Show();

            foreach (Window window in Application.Current.Windows)
            {
                if (window == mainWindow) continue;
                
                window.Close();
            }
            Globals.LoggedUser = null;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateAppointmentsTable();

        }

        public void UpdateAppointmentsTable()
        {
            DataTable dataTable = InitAppointmentTableColumns();

            foreach (Appointment app in AppointmentService.GetAppointmentsInNextFifteenMinutes())
            {
                dataTable.Rows.Add(app.ToTable());
            }

            appointmentsDataGrid.ItemsSource = new DataView(dataTable);
        }

        private static DataTable InitAppointmentTableColumns()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Id", typeof(string));
            dt.Columns.Add("Start", typeof(string));
            dt.Columns.Add("End", typeof(string));
            dt.Columns.Add("Doctor username", typeof(string));
            dt.Columns.Add("Patient username", typeof(string));
            dt.Columns.Add("Type", typeof(string));
            dt.Columns.Add("Status", typeof(string));
            dt.Columns.Add("Room name", typeof(string));

            return dt;
        }

        private void checkInBtn_Click(object sender, RoutedEventArgs e)
        {
            if (appointmentsDataGrid.SelectedItem is DataRowView selectedRow)
            { 
                string patientUsername = selectedRow.Row.ItemArray[4].ToString() ?? string.Empty;
                int appointmentId = int.Parse(selectedRow.Row.ItemArray[0].ToString());
                _appointmentId = int.Parse(selectedRow.Row.ItemArray[0]!.ToString()!);

                CheckPatientProfile(patientUsername, appointmentId);
                return;
            }
            
            Notification.ShowErrorDialog("You must select appointment for which you would like to do check in!");
        }

        private void CheckPatientProfile(string patientUsername, int appointmentId)
        {
            if (PatientService.GetPatient(patientUsername) != null)
            {
                ReportHistoryService.AddOrEditReport(patientUsername, appointmentId, "");

                EditMedicalRecordWindow editMedicalRecordWindow = new(MedicalRecordService.GetMedicalRecord(patientUsername)!, _appointmentId);
                editMedicalRecordWindow.ShowDialog();
                return;
            }

            Notification.ShowWarningDialog("Patient still doesn't have an account, create new account. ");
            _ = new AddPatientProfileDialog(true, _appointmentId);
        }
    }
}
