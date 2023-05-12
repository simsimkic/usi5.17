using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Domain.Calendar;
using ZdravoCorp.Repository;
using ZdravoCorp.Repository.Serializer;
using static ZdravoCorp.Domain.Users.User;

namespace ZdravoCorp.Domain.Users
{
    public enum DoctorSpecialization
    {
        Cardiology,
        Oncology,
        Neurology,
        Pediatrics,
        Gynecology,
        Surgeon,
        Physician
    }
    public class Doctor : Serializable
    {

        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public DoctorSpecialization Specialization;

        public Doctor(){}
        public Doctor(string username, string firstName, string lastName)
        {
            Username = username;
            FirstName = firstName;
            LastName = lastName;
        }

        public bool IsAvailable(TimeSlot timeSlot)
        {
            foreach (Appointment appointment in AppointmentRepository.Appointments.Values)
            {
                if (appointment.DoctorUsername == Username &&
                    appointment.Status == Appointment.AppointmentStatus.Active &&
                    appointment.TimeSlot.OverlapsWith(timeSlot))
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsEditAvailable(Appointment editedAppointment)
        {
            foreach (Appointment appointment in AppointmentRepository.Appointments.Values)
            {
                if (appointment.Id != editedAppointment.Id &&
                    appointment.DoctorUsername == Username &&
                    appointment.Status == Appointment.AppointmentStatus.Active &&
                    appointment.TimeSlot.OverlapsWith(editedAppointment.TimeSlot))
                {
                    return false;
                }
            }
            return true;
        }
        public string[] ToCSV()
        {
            string[] csvValues =
            {
                Username,
                FirstName,
                LastName,
                Specialization.ToString()
            };
            return csvValues;
        }

        public void FromCSV(string[] values)
        {
            Username = values[0];
            FirstName = values[1];
            LastName = values[2];
            Specialization = ParseDoctorSpecialization(values[3]);
        }
        public DoctorSpecialization ParseDoctorSpecialization(string specializationString)
        {
            return Enum.Parse<DoctorSpecialization>(specializationString);
        }
    }
}
