using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using ZdravoCorp.Domain.Calendar;
using ZdravoCorp.Domain.Common;
using ZdravoCorp.Domain.Room;
using ZdravoCorp.Domain.Users;
using ZdravoCorp.Presentation.Common;
using ZdravoCorp.Presentation.DoctorWindow.Appointments;
using ZdravoCorp.Presentation.Notifications;
using ZdravoCorp.Services;
using ZdravoCorp.Services.Rooms;
using static ZdravoCorp.Domain.Calendar.Appointment;

namespace ZdravoCorp.Presentation.Nurse.Appointments
{
    /// <summary>
    /// Interaction logic for PatientCheckInWindow.xaml
    /// </summary>
    public partial class ChooseAppointmentDialog : Window
    {
        private readonly List<(TimeSlot, Appointment)> _possibleAppointments;
        private Patient patient;
        private AppointmentType appointmentType;
        public ChooseAppointmentDialog(List<(TimeSlot, Appointment)> possibleAppointments)
        {
            _possibleAppointments = possibleAppointments;
            InitializeComponent();
            this.ShowDialog();
        }

        public ChooseAppointmentDialog(List<(TimeSlot, Appointment)> possibleAppointments, Patient patient, bool isOperation, List<Doctor> specializedDoctors, AppointmentType appointmentType)
        {
            _possibleAppointments = possibleAppointments;
            InitializeComponent();
            this.ShowDialog();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateAppointmentsTable();
        }
        public void UpdateAppointmentsTable()
        {
            DataTable dataTable = InitAppointmentsTableColumns();

            foreach (var pair in _possibleAppointments)
            {
                TimeSlot delayTimeSlot = pair.Item1;
                Appointment originalAppointment = pair.Item2;

                string[] row = AppointmentAndDelayPairToTable(originalAppointment, delayTimeSlot);

                dataTable.Rows.Add(row);
            }
            appointmentsDataGrid.ItemsSource = new DataView(dataTable);
        }

        private static string[] AppointmentAndDelayPairToTable(Appointment originalAppointment, TimeSlot delayTimeSlot)
        {
            string[] tableValues =
            {
                originalAppointment.Id.ToString(),
                originalAppointment.TimeSlot.Start.ToString(),
                originalAppointment.DoctorUsername,
                originalAppointment.PatientUsername,
                originalAppointment.RoomName,
                originalAppointment.Type.ToString(),
                delayTimeSlot.Start.ToString()
            };
            return tableValues;
        }

        private static DataTable InitAppointmentsTableColumns()
        {
            DataTable dt = new DataTable();
            
            dt.Columns.Add("Appointment ID", typeof(string));
            dt.Columns.Add("Starts on", typeof(string));
            dt.Columns.Add("Doctor username", typeof(string));
            dt.Columns.Add("Patient username", typeof(string));
            dt.Columns.Add("Room name", typeof(string));
            dt.Columns.Add("Type", typeof(string));
            dt.Columns.Add("Delay for", typeof(string));
            return dt;
        }


        private void ChoiceBtn_Click(object sender, RoutedEventArgs e)
        {
            if (appointmentsDataGrid.SelectedItem == null)
            {
                Notification.ShowWarningDialog("Please first select appointment which you want to delay!");
                return;
            }

            int appointmentId = GetSelectedAppointmentFromTable();

            DelaySelectedAppointment(appointmentId);


        }

        private void DelaySelectedAppointment(int appointmentId)
        {
            
            foreach (var pair in _possibleAppointments)
            {
                if (pair.Item2.Id == appointmentId)
                {

                    CreateEmergencyAppointment(pair.Item2);
                    DelayAppointment(pair);
                    MakeNotices(pair);
                    break;
                }
            }
            this.Close();
        }

        private static void CreateEmergencyAppointment(Appointment delayedAppointment)
        {
            TimeSlot appointmentTime = new TimeSlot(delayedAppointment.TimeSlot.Start, delayedAppointment.TimeSlot.Start.AddMinutes(15));

            Appointment emergencyAppointment = new Appointment(appointmentTime, delayedAppointment.DoctorUsername, delayedAppointment.PatientUsername, delayedAppointment.Type, AppointmentStatus.Active, delayedAppointment.RoomName, false, false, false, true);

            AppointmentService.AddOrEditAppointment(emergencyAppointment);

            Notification.ShowSuccessDialog("Emergency Appointment has been successfully scheduled for:" + emergencyAppointment.TimeSlot.Start);

        }

        private static void MakeNotices((TimeSlot, Appointment) delayedAppointmentPair)
        {
            Appointment appointment = delayedAppointmentPair.Item2;
            TimeSlot timeOfDelay = delayedAppointmentPair.Item1;

            string noticeMessage = "Appointment with ID: " + appointment.Id +" has been rescheduled to: " + timeOfDelay.Start.ToString();

            Notice doctorNotice = new Notice(DateTime.Now, noticeMessage, appointment.DoctorUsername);
            Notice patientNotice = new Notice(DateTime.Now, noticeMessage, appointment.PatientUsername);

            NoticeService.Add(doctorNotice);
            NoticeService.Add(patientNotice);
        }

        private static void DelayAppointment((TimeSlot, Appointment) appointmentAndDelayPair)
        {
            appointmentAndDelayPair.Item2.TimeSlot = appointmentAndDelayPair.Item1;
            AppointmentService.AddOrEditAppointment(appointmentAndDelayPair.Item2);
        }

        private int GetSelectedAppointmentFromTable()
        {
            DataRowView rowView = (appointmentsDataGrid.SelectedItem as DataRowView)!;
            return int.Parse(rowView.Row[0].ToString()!);
        }
    }
}
