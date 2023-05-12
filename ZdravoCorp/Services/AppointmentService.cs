using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Linq;
using ZdravoCorp.Domain.Calendar;
using ZdravoCorp.Domain.Common;
using ZdravoCorp.Domain.Room;
using ZdravoCorp.Presentation.Notifications;
using ZdravoCorp.Repository;
using ZdravoCorp.Domain.Users;
using static ZdravoCorp.Domain.Calendar.Appointment;
using ZdravoCorp.Domain.Room;
using ZdravoCorp.Services.Rooms;

namespace ZdravoCorp.Services
{
    public static class AppointmentService
    {
        public static Appointment GetAppointment(int id)
        {
            return AppointmentRepository.Appointments[id];
        }

        public static void CancelAppointment(int id)
        {
            GetAppointment(id).Cancel();
            AppointmentRepository.Save();
        }

        public static void PatientCancelAppointment(int id)
        {
            GetAppointment(id).PatientCancel();
            AppointmentRepository.Save();
        }

        public static List<Appointment> GetAppointmentsForNextThreeDays(DateTime date)
        {
            List<Appointment> appointments = new();
            foreach (Appointment appointment in AppointmentRepository.Appointments.Values)
            {
                if (appointment.IsInRange(date, date.AddDays(3)) && appointment.DoctorUsername == Globals.LoggedUser.Username)
                {
                    appointments.Add(appointment);
                }
            }
            return appointments;
        }

        public static List<Appointment> GetAllAppointments()
        {
            return AppointmentRepository.Appointments.Values.ToList();
        }

        public static List<Appointment> GetAllAppointmentsForPatient(string username)
        {
            return AppointmentRepository.GetAllAppointmentsForPatient(username);
        }
        public static List<Appointment> GetAllActiveAppointments()
        {
            List<Appointment> appointments = new();
            foreach (Appointment appointment in AppointmentRepository.Appointments.Values)
            {
                if (appointment.Status == AppointmentStatus.Active)
                {
                    appointments.Add(appointment);
                }
            }
            return appointments;
        }

        public static bool HasPatientBeenToDoctor(string patientUsername, string doctorUsername)
        {
            foreach (Appointment appointment in AppointmentRepository.Appointments.Values)
            {
                if (appointment.IsFinished() &&
                    appointment.DoctorUsername == doctorUsername &&
                    appointment.PatientUsername == patientUsername)
                {
                    return true;
                }
            }
            return false;
        }
        public static void AddOrEditAppointment(Appointment appointment)
        {
            appointment.AssignRoom();
            AppointmentRepository.Appointments[appointment.Id] = appointment;
            AppointmentRepository.Save();
        }

        public static void ValidateAddAppointment(Appointment appointment)
        {
            if (!appointment.TimeSlot.IsInFuture())
            {
                throw new ArgumentException("Time must be in future.");
            } 
            else if(!PatientService.GetPatient(appointment.PatientUsername).IsAvailable(appointment.TimeSlot))
            {
                throw new ArgumentException("Patient is not available at given time.");
            }
            else if (!DoctorService.GetDoctor(appointment.DoctorUsername).IsAvailable(appointment.TimeSlot))
            {
                throw new ArgumentException("Doctor is not available at given time.");
            }
        }


        public static void ValidateEditAppointment(Appointment appointment)
        {
            if (!appointment.TimeSlot.IsInFuture())
            {
                throw new ArgumentException("Time must be in future.");
            }
            else if (!PatientService.GetPatient(appointment.PatientUsername).IsAvailable(appointment.TimeSlot))
            {
                throw new ArgumentException("Patient is not available at given time.");
            }
            else if (!DoctorService.GetDoctor(appointment.DoctorUsername).IsAvailable(appointment.TimeSlot))
            {
                throw new ArgumentException("Doctor is not available at given time.");
            }
        }

        public static void ValidateBeforeEditOrCancel(Appointment appointment)
        {
            if (appointment.Status != Appointment.AppointmentStatus.Active)
            {
                throw new InvalidOperationException("You can't edit non-active appointments.");
            }
            if (DateTime.Now.AddDays(1) > appointment.TimeSlot.Start)
            {
                throw new ArgumentException("Minimum 1 day difference is required in order to edit or cancel an appointment!");
            }
        }

        public static int CountPatientCanceledAppointments(string patientUsername)
        {
            int appointments = 0;
            foreach (Appointment appointment in AppointmentRepository.Appointments.Values)
            {
                if (appointment.PatientUsername == patientUsername && appointment.HasPatientCanceled == true &&
                    appointment.TimeSlot.Start.AddDays(30) > DateTime.Now)
                {
                    appointments++;
                }
            }
            return appointments;
        }

        public static int CountPatientAddedAppointments(string patientUsername)
        {
            int appointments = 0;
            foreach (Appointment appointment in AppointmentService.GetAllAppointments())
            {
                if (appointment.PatientUsername == patientUsername && appointment.HasPatientAppointed == true &&
                    appointment.TimeSlot.Start.AddDays(30) > DateTime.Now)
                {
                    appointments++;
                }
            }
            return appointments;
        }

        public static int CountPatientEditedAppointments(string patientUsername)
        {
            int appointments = 0;
            foreach (Appointment appointment in AppointmentRepository.Appointments.Values)
            {
                if (appointment.PatientUsername == patientUsername && appointment.HasPatientEdited == true &&
                    appointment.TimeSlot.Start.AddDays(30) > DateTime.Now)
                {
                    appointments++;
                }
            }
            return appointments;
        }

        public static void ValidateAppointmentBeforeExamination(Appointment appointment)
        {
            if (!appointment.IsActive())
            {
                throw new InvalidOperationException("This appointment is not active.");
            }
            else if (DateTime.Now < appointment.TimeSlot.Start.AddMinutes(-15))
            {
                throw new InvalidOperationException("Examination can only be started 15 minutes prior.");
            } 
            else if (appointment.PatientUsername == "")
            {
                throw new InvalidOperationException("Patient has no account.");
            }
        }

        public static List<Appointment> GetActiveOverlappingAppointments(TimeSlot timeslot)
        {
            return AppointmentRepository.Appointments.Values.Where(appointment => appointment.TimeSlot.OverlapsWith(timeslot)).ToList();
        }

        public static List<Appointment> GetAppointmentsInNextFifteenMinutes()
        {
            List<Appointment> appointments = new();
            foreach (Appointment appointment in AppointmentRepository.Appointments.Values)
            {
                if (!appointment.IsInNextFifteenMinutes()) continue;
                appointments.Add(appointment);
            }
            return appointments;
        }

        public static List<Appointment> GetAvailableAppointmentsInDateRange(Doctor doctor, DateTime endDate, TimeSlot timeslot)
        {
            List<Appointment> availableAppointments = new List<Appointment>();
            TimeSlot iterativeTimeSlot = new();
            iterativeTimeSlot.Start = timeslot.Start;
            iterativeTimeSlot.End = timeslot.End;

            while (iterativeTimeSlot.Start.Date <= endDate)
            {
                List<Appointment> appointmentsForDay = GetAvailableAppointmentsInTimeRange(doctor, iterativeTimeSlot);
                iterativeTimeSlot.Start = iterativeTimeSlot.Start.AddDays(1);
                iterativeTimeSlot.End = iterativeTimeSlot.End.AddDays(1);

                availableAppointments.AddRange(appointmentsForDay);
            }

            return availableAppointments;
        }

        public static List<Appointment> GetAvailableAppointmentsInTimeRange(Doctor doctor, TimeSlot timeRange)
        {
            List<Appointment> availableAppointments = new List<Appointment>();
            Patient patient = PatientRepository.GetPatient(Globals.LoggedUser.Username);
            DateTime startTime = timeRange.Start;

            while (startTime < timeRange.End)
            {
                try
                {
                    string roomName = RoomService.GetFreeRoom(Room.RoomType.ExaminationRoom, timeRange).Name;

                    TimeSlot timeSlot = new();
                    timeSlot.Start = startTime;
                    timeSlot.End = startTime.AddMinutes(15);

                    if (doctor.IsAvailable(timeSlot) && patient.IsAvailable(timeSlot) && timeSlot.Start > DateTime.Now)
                    {
                        Appointment appointment = new Appointment(timeSlot, doctor.Username, patient.Username, Appointment.AppointmentType.Examination, Appointment.AppointmentStatus.Active, roomName, true, false, false);
                        availableAppointments.Add(appointment);
                    }
                }
                catch
                {
                }
                startTime = startTime.AddMinutes(15);
            }

            return availableAppointments;
        }

        public static List<Appointment> GetAvailableAppointmentsPriorityTime(Doctor doctor, DateTime lastDate, TimeSlot timeslot)
        {
            List<Appointment> availableAppointments = new();
            foreach (Doctor doc in DoctorService.GetAllDoctors())
            {
                if (doc == doctor) //skip already occupied doctor
                {
                    continue;
                }
                availableAppointments = GetAvailableAppointmentsInDateRange(doc, lastDate, timeslot);
                if (availableAppointments.Count != 0)
                {
                    break;
                }
            }
            return availableAppointments;
        }

        public static List<Appointment> GetAvailableAppointmentsPriorityDoctor(Doctor doctor, DateTime lastDate, TimeSlot timeslot)
        {
            List<Appointment> availableAppointments = new();

            if (lastDate < timeslot.Start.Date) {
                lastDate = timeslot.Start.Date;
            }

            for(int i = 0; i < 7; i++)
            {
                availableAppointments = AppointmentService.GetAvailableAppointmentsInDateRange(doctor, lastDate, timeslot);

                if (availableAppointments.Count > 0)
                {
                    return availableAppointments;
                }

                lastDate = lastDate.AddDays(1);
                timeslot.Start = timeslot.Start.AddDays(1);
                timeslot.End = timeslot.End.AddDays(1);
            }
            return availableAppointments;

        }
        public static List<Appointment> GetAppointmentsInNextTwoHours()
        {
            List<Appointment> appointments = new();
            foreach (Appointment appointment in AppointmentRepository.Appointments.Values)
            {
                if (!appointment.IsInNextTwoHours()) continue;
                appointments.Add(appointment);
            }
            return appointments;
        }
        public static List<Appointment> GetThreeClosestAppointments(DateTime lastDate, TimeSlot timeslot)
        {
            List<Appointment> availableAppointments = new();
            if (lastDate < timeslot.Start.Date)
            {
                lastDate = timeslot.Start.Date;
            }

            while (availableAppointments.Count <= 3)
            {
                foreach (Doctor doctor in DoctorService.GetAllDoctors())
                {
                    availableAppointments.AddRange(GetAvailableAppointmentsInDateRange(doctor, lastDate, timeslot));
                }
                lastDate = lastDate.AddDays(1);
                timeslot.Start = timeslot.Start.AddDays(1);
                timeslot.End = timeslot.End.AddDays(1);
            }
            return availableAppointments.GetRange(0, 3);
        }
    }
}
