using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using ZdravoCorp.Domain.Calendar;
using ZdravoCorp.Domain.Common;
using ZdravoCorp.Presentation.Common;
using ZdravoCorp.Presentation.Notifications;
using ZdravoCorp.Repository;
using ZdravoCorp.Services;

namespace ZdravoCorp.Presentation.PatientsWindow
{
    /// <summary>
    /// Interaction logic for PatientWindow.xaml
    /// </summary>
    public partial class PatientWindow : Window
    {
        public PatientWindow()
        {
            InitializeComponent();
            loggedUsernameLabel.Content = Globals.LoggedUser.Username;
        }

        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new();
            mainWindow.Show();
            Globals.LoggedUser = null;
            this.Close();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddPatientAppointmentDialog addPatientAppointmentDialog = new(this);
            addPatientAppointmentDialog.Show();
            this.IsEnabled = false;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Appointment appointment = AppointmentService.GetAppointment(GetSelectedAppointmentId());
                AppointmentService.ValidateBeforeEditOrCancel(appointment);
                OpenEditAppointmentForm(appointment);
            }
            catch(Exception error)
            {
                Notification.ShowErrorDialog(error.Message);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int id = GetSelectedAppointmentId();
                Appointment appointment = AppointmentService.GetAppointment(id);

                AppointmentService.ValidateBeforeEditOrCancel(appointment);
                AppointmentService.PatientCancelAppointment(id);
                UpdateAppointmentsTable();

                if (AppointmentService.CountPatientCanceledAppointments(Globals.LoggedUser.Username) >= 5)
                {
                    PatientService.BlockPatient(Globals.LoggedUser.Username);
                    LogOutBlockedPatient();
                }
                else
                {
                    Notification.ShowSuccessDialog("Succesfully cancelled appointment!");
                }
            }
            catch (Exception error)
            {
                Notification.ShowErrorDialog(error.Message);
            }
        }

        public void LogOutBlockedPatient()
        {
            Globals.LoggedUser = null;
            MainWindow mainWindow = new();
            mainWindow.Show();

            Notification.ShowErrorDialog("Your account has been permanently disabled for canceling 5 or more appointments.");

            this.Close();
        }

        private void OpenEditAppointmentForm(Appointment selectedAppointment)
        {
            EditPatientAppointmentDialog editPatientAppointmentDialog = new(this, selectedAppointment);
                editPatientAppointmentDialog.Show();
                this.IsEnabled = false;
        }

        private int GetSelectedAppointmentId()
        {
            var selectedRow = appointmentsDataGrid.SelectedItem as DataRowView;
            if (selectedRow == null)
            {
                throw new InvalidOperationException("Please select a row.");
            }
            return int.Parse(selectedRow.Row.ItemArray[0].ToString());
        }

        private void patientWindow_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateAppointmentsTable();
            UpdateReportHistoryTable(false, null);
            UpdateNoticeTable();
        }

        private void UpdateNoticeTable()
        {
            noticeDataGrid.ItemsSource = NoticeService.GetAllUsersNotices(Globals.LoggedUser.Username);
        }


        public void UpdateAppointmentsTable()
        {
            DataTable dataTable = InitAppointmentTableColumns();

            foreach (Appointment app in AppointmentService.GetAllAppointments())
            {
                if (app.PatientUsername == Globals.LoggedUser.Username)
                {
                    dataTable.Rows.Add(app.ToTable());
                }
            }

            appointmentsDataGrid.ItemsSource = new DataView(dataTable);
        }

        private static DataTable InitAppointmentTableColumns()
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Id", typeof(string));
            dataTable.Columns.Add("Start", typeof(string));
            dataTable.Columns.Add("End", typeof(string));
            dataTable.Columns.Add("Doctor username", typeof(string));
            dataTable.Columns.Add("Patient username", typeof(string));
            dataTable.Columns.Add("Type", typeof(string));
            dataTable.Columns.Add("Status", typeof(string));
            dataTable.Columns.Add("Room name", typeof(string));

            return dataTable;
        }

        public void UpdateReportHistoryTable(bool filter, string searchText)
        {
            DataTable dataTable = InitReportHistoryTableColumns();

            Dictionary<int, string> reportHistory = ReportHistoryService.GetReportHistory(Globals.LoggedUser.Username);
            foreach (KeyValuePair<int, string> report in reportHistory)
            {
                Appointment appointment = AppointmentService.GetAppointment(report.Key);
                string[] tableValues =
                {
                    report.Key.ToString(),
                    appointment.TimeSlot.Start.ToString(),
                    appointment.DoctorUsername,
                    DoctorService.GetDoctor(appointment.DoctorUsername).Specialization.ToString(),
                    report.Value
                };
                if (!filter || (filter && report.Value.Contains(searchText)))
                {
                    dataTable.Rows.Add(tableValues);
                }
            }
            reportHistoryDataGrid.ItemsSource = new DataView(dataTable);
        }

        private static DataTable InitReportHistoryTableColumns()
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Id", typeof(string));
            dataTable.Columns.Add("Start", typeof(string));
            dataTable.Columns.Add("Doctor", typeof(string));
            dataTable.Columns.Add("Doctor specialization", typeof(string));
            dataTable.Columns.Add("Medical report", typeof(string));

            return dataTable;
        }

        private void advancedSearchButton_Click(object sender, RoutedEventArgs e)
        {
            AdvancedAddPatientAppointmentForm advancedAddAppointment = new AdvancedAddPatientAppointmentForm(this);
            advancedAddAppointment.ShowDialog();
        }

        private void AppointmentShowMedicalRecordButton_Click(object sender, RoutedEventArgs e)
        {
            MedicalRecordWindow medicalRecordWindow =
                new MedicalRecordWindow(MedicalRecordService.GetMedicalRecord(Globals.LoggedUser.Username));
            medicalRecordWindow.ShowDialog();
        }

        private void searchTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdateReportHistoryTable(true, searchTextBox.Text);
        }
    }
}