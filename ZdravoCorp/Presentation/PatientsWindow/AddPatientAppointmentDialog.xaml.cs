using System;
using System.Windows;
using ZdravoCorp.Domain.Calendar;
using ZdravoCorp.Domain.Common;
using ZdravoCorp.Domain.Users;
using ZdravoCorp.Presentation.Notifications;
using ZdravoCorp.Repository;
using ZdravoCorp.Services;
using static ZdravoCorp.Domain.Calendar.Appointment;

namespace ZdravoCorp.Presentation.PatientsWindow
{
    /// <summary>
    /// Interaction logic for AddPatientAppointmentDialog.xaml
    /// </summary>
    public partial class AddPatientAppointmentDialog : Window
    {
        private PatientWindow patientWindow;
        public AddPatientAppointmentDialog(PatientWindow patientWindow)
        {
            InitializeComponent();
            InitializeDefaultValues();
            this.patientWindow = patientWindow;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Appointment appointment = ParseAppointmentFromDialog();
                AppointmentService.ValidateAddAppointment(appointment);
                AppointmentService.AddOrEditAppointment(appointment);

                patientWindow.UpdateAppointmentsTable();

                if (AppointmentService.CountPatientAddedAppointments(Globals.LoggedUser.Username) >= 8)
                {
                    PatientService.BlockPatient(Globals.LoggedUser.Username);
                    patientWindow.LogOutBlockedPatient();
                    patientWindow.Close();
                }
                else
                {
                    Notification.ShowSuccessDialog("You have successfully created an appointment!");
                    this.Close();
                }
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

        private void InitializeDefaultValues()
        {
            foreach (Doctor doctor in DoctorService.GetAllDoctors())
            {
                doctorPickerComboBox.Items.Add(doctor.Username);
            }
            datePicker.SelectedDate = DateTime.Now;
            timePickerTextBox.Text = "08:00";
        }

        private TimeSlot ParseDatesFromDialog()
        {

            DateTime startDate = datePicker.SelectedDate.Value.Date;
            DateTime endDate = datePicker.SelectedDate.Value.Date;

            try
            {
                TimeOnly startTime = TimeOnly.Parse(timePickerTextBox.Text);

                startDate = startDate.AddHours(startTime.Hour).AddMinutes(startTime.Minute);
                endDate = startDate.AddMinutes(15);
            }
            catch
            {
                return null;
            }
           
            return new TimeSlot(startDate, endDate);
        }

        private Appointment ParseAppointmentFromDialog()
        {
            TimeSlot timeSlot = ParseDatesFromDialog();

            return new Appointment(timeSlot, doctorPickerComboBox.Text, Globals.LoggedUser.Username, AppointmentType.Examination, AppointmentStatus.Active, "",  true, false, false);
        }

        private void AddAppointmentWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            patientWindow.IsEnabled = true;
        }
    }
}
