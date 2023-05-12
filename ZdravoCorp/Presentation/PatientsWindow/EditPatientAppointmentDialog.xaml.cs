using System;
using System.Windows;
using ZdravoCorp.Domain.Calendar;
using ZdravoCorp.Domain.Common;
using ZdravoCorp.Domain.Users;
using ZdravoCorp.Presentation.Notifications;
using ZdravoCorp.Repository;
using ZdravoCorp.Services;

namespace ZdravoCorp.Presentation.PatientsWindow
{
    /// <summary>
    /// Interaction logic for EditAppointmentWindow.xaml
    /// </summary>
    /// 

    public partial class EditPatientAppointmentDialog : Window
    {
        private PatientWindow patientWindow;
        private Appointment appointment;
        public EditPatientAppointmentDialog(PatientWindow patientWindow, Appointment appointment)
        {
            InitializeComponent();
            this.patientWindow = patientWindow;
            this.appointment = appointment;
            Load(appointment);
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Appointment editedAppointment = ApplyChangesFromDialog(appointment);
            try
            {
                AppointmentService.ValidateEditAppointment(editedAppointment);
                AppointmentService.AddOrEditAppointment(editedAppointment);
                patientWindow.UpdateAppointmentsTable();

                if (AppointmentService.CountPatientEditedAppointments(Globals.LoggedUser.Username) >= 5)
                {
                    PatientService.BlockPatient(Globals.LoggedUser.Username);

                    patientWindow.LogOutBlockedPatient();
                    patientWindow.Close();
                }
                else
                {
                    Notification.ShowSuccessDialog("You have successfully edited an appointment!");
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

        private void Load(Appointment appointment)
        {
            foreach(Doctor doctor in DoctorService.GetAllDoctors())
            {
                doctorPickerComboBox.Items.Add(doctor.Username);
            }

            datePicker.SelectedDate = appointment.TimeSlot.Start.Date;
            timePickerTextBox.Text = appointment.TimeSlot.Start.TimeOfDay.ToString();
            doctorPickerComboBox.SelectedItem = appointment.DoctorUsername;
        }

        private void EditAppointment_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            patientWindow.IsEnabled = true;
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

        private Appointment ApplyChangesFromDialog(Appointment editedAppointment)
        {
            TimeSlot timeSlot = ParseDatesFromDialog();
            editedAppointment.DoctorUsername = doctorPickerComboBox.Text;
            editedAppointment.TimeSlot = timeSlot;
            editedAppointment.HasPatientEdited = true;
            return editedAppointment;
        }

    }
}
