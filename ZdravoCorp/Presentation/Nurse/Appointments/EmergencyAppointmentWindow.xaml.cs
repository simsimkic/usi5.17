using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
    public partial class EmergencyAppointmentWindow : Window
    {
        public EmergencyAppointmentWindow()
        {
            InitializeComponent();
            loggedUsernameLabel.Content = Globals.LoggedUser!.Username;
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
            InitializePatientDataGrid();

        }

        private void InitializePatientDataGrid()
        {
            patientDataGrid.ItemsSource = PatientService.GetAllActivePatients();
        }

        private void EmergencyExaminationBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!IsPatientSelected()) return;

            var emergencyPatient = (patientDataGrid.SelectedItem as Patient)!;
            var specialization = (DoctorSpecialization)specializationComboBox.SelectionBoxItem;

            FindEmergencyAppointmentWithinTwoHours(emergencyPatient, specialization, false);
        }

        private void EmergencyOperationBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!IsPatientSelected()) return;

            var emergencyPatient = (patientDataGrid.SelectedItem as Patient)!;
            var specialization = (DoctorSpecialization)specializationComboBox.SelectionBoxItem;

            FindEmergencyAppointmentWithinTwoHours(emergencyPatient, specialization, true);
        }
        private bool IsPatientSelected()
        {
            if (patientDataGrid.SelectedItem != null) return true;

            Notification.ShowWarningDialog("Please select the patient first!");
            return false;

        }

        private void FindEmergencyAppointmentWithinTwoHours(Patient patient, DoctorSpecialization specialization, bool isOperation)
        {
            var appointmentTimes = GetAppointmentTimesWithinTwoHours();
            List<Doctor> specializedDoctors = DoctorService.GetAllSpecializedDoctors(specialization);

            AppointmentType appointmentType = GetAppointmentType(isOperation);

            foreach (DateTime appointmentTime in appointmentTimes)
            {
                if (IsAppointmentMadeSuccessfully(patient, isOperation, appointmentTime, specializedDoctors, appointmentType))
                    return;
            }

            new ChooseAppointmentDialog(GenerateFivePossibleAppointments(specialization, isOperation), patient, isOperation, specializedDoctors, appointmentType);
        }

        private List<(TimeSlot, Appointment)> GenerateFivePossibleAppointments(DoctorSpecialization doctorSpecialization, bool isOperation)
        {
            List<Appointment> appointments = AppointmentService.GetAppointmentsInNextTwoHours();
            List<(TimeSlot, Appointment)> possibleAppointments = new List<(TimeSlot, Appointment)>();

            foreach (Appointment appointment in appointments)
            {
                if (appointment.IsEmergency) continue;
                if (DoctorService.GetDoctor(appointment.DoctorUsername)!.Specialization != doctorSpecialization) continue;
                if (isOperation && appointment.Type == AppointmentType.Examination) continue;
                if (isOperation == false && appointment.Type == AppointmentType.Operation) continue;

                TimeSlot delayTimeSlot = appointment.EarliestDelay();
                possibleAppointments.Add((delayTimeSlot, appointment));
            }
            possibleAppointments = possibleAppointments.OrderBy(pair => pair.Item1.Start).ToList();
            
            return possibleAppointments.Take(5).ToList();

        }

        private static bool IsAppointmentMadeSuccessfully(Patient patient, bool isOperation, DateTime appointmentTime, List<Doctor> specializedDoctors,
            AppointmentType appointmentType)
        {
            TimeSlot appointmentTimeInterval = new TimeSlot(appointmentTime, appointmentTime.AddMinutes(15));

            Room.RoomType roomType = GetRoomType(isOperation);
            Room? room = GetFreeRoom(roomType, appointmentTimeInterval);

            Doctor? doctor = GetAvailableDoctor(specializedDoctors, appointmentTimeInterval);

            if (!IsAppointmentAvailable(patient, appointmentTimeInterval, room, doctor)) return false;

            Appointment emergencyAppointment = new Appointment(appointmentTimeInterval, doctor!.Username, patient.Username, appointmentType, AppointmentStatus.Active, room!.Name, false, false, false, true);

            AppointmentService.AddOrEditAppointment(emergencyAppointment);

            Notification.ShowSuccessDialog("Emergency appointment scheduled for: " + emergencyAppointment.TimeSlot.Start);

            return true;
        }

        private static Appointment.AppointmentType GetAppointmentType(bool isOperation)
        {
            return isOperation ? Appointment.AppointmentType.Operation : Appointment.AppointmentType.Examination;
        }

        private static bool IsAppointmentAvailable(Patient patient, TimeSlot appointmentTimeInterval, Room? room, Doctor? doctor)
        {
            if (!patient.IsAvailable(appointmentTimeInterval)) return false;
            if (room == null) return false;
            if (doctor == null) return false;
            return true;
        }

        private static Room? GetFreeRoom(Room.RoomType roomType, TimeSlot appointmentTimeInterval)
        {
            try
            {
                Room room = RoomService.GetFreeRoom(roomType, appointmentTimeInterval);
                return room;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static Room.RoomType GetRoomType(bool isOperation)
        {
            if (isOperation)
            {
                return Room.RoomType.OperationRoom;
            }
            else return Room.RoomType.ExaminationRoom;
        }

        private static Doctor? GetAvailableDoctor(List<Doctor> specializedDoctors, TimeSlot appointmentTimeInterval)
        {
            return specializedDoctors.FirstOrDefault(doctor => doctor.IsAvailable(appointmentTimeInterval));
        }

        private static List<DateTime> GetAppointmentTimesWithinTwoHours()
        {
            DateTime currentTime = DateTime.Now;
            DateTime timeInTwoHours = currentTime.AddHours(2);

            List<DateTime> appointmentTimes = new List<DateTime>();

            for (DateTime appointmentTime = currentTime; appointmentTime < timeInTwoHours; appointmentTime = appointmentTime.AddMinutes(1))
            {
                if (appointmentTime.Minute % 15 == 0)
                {
                    appointmentTimes.Add(appointmentTime);
                }
            }

            return appointmentTimes;
        }
    }
}
