using System;
using System.Collections.Generic;
using ZdravoCorp.Domain.Calendar;
using ZdravoCorp.Repository;
using ZdravoCorp.Repository.Serializer;
using static ZdravoCorp.Domain.Calendar.Appointment;

namespace ZdravoCorp.Domain.Users
{
    public class Patient : Serializable
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }

        public enum BlockStatus
        {
            Active,
            Blocked
        }

        public BlockStatus Status { get; set; }

        public Patient() { }
        public Patient(string firstName, string lastName, string username)
        {
            FirstName = firstName;
            LastName = lastName;
            Username = username;
            Status = BlockStatus.Active;

        }

        public Patient(string firstName, string lastName, string username, bool isChecked)
        {
            FirstName = firstName;
            LastName = lastName;
            Username = username;
            Status = isChecked ? BlockStatus.Blocked : BlockStatus.Active;
        }

        public bool IsAvailable(TimeSlot timeSlot)
        {
            foreach (Appointment appointment in AppointmentRepository.Appointments.Values)
            {
                if (appointment.PatientUsername == Username &&
                    appointment.Status == AppointmentStatus.Active &&
                    appointment.TimeSlot.OverlapsWith(timeSlot))
                {
                    return false;
                }
            }
            return true;
        }

        public Appointment? GetPatientsAppointmentThatOverlapsWith(TimeSlot timeSlot)
        {
            foreach (Appointment appointment in AppointmentRepository.Appointments.Values)
            {
                if (appointment.PatientUsername == Username &&
                    appointment.Status == AppointmentStatus.Active &&
                    appointment.TimeSlot.OverlapsWith(timeSlot))
                {
                    return appointment;
                }
            }
            return null;
        }

        public bool IsEditAvailable(Appointment editedAppointment)
        {
            foreach (Appointment appointment in AppointmentRepository.Appointments.Values)
            {
                if (appointment.Id != editedAppointment.Id &&
                    appointment.PatientUsername == Username &&
                    appointment.Status == AppointmentStatus.Active &&
                    appointment.TimeSlot.OverlapsWith(editedAppointment.TimeSlot))
                {
                    return false;
                }
            }
            return true;
        }

        public void BlockAccess()
        {
            Status = BlockStatus.Blocked;
        }

        public string[] ToTable()
        {
            string[] tableValues =
            {
                Username,
                FirstName,
                LastName
            };
            return tableValues;
        }
        public string[] ToCSV()
        {
            string[] csvValues =
            {
                FirstName,
                LastName,
                Username,
                Status.ToString()
            };
            return csvValues;
        }

        public void FromCSV(string[] values)
        {
            FirstName = values[0];
            LastName = values[1];
            Username = values[2];
            Status = (BlockStatus)Enum.Parse(typeof(BlockStatus), values[3]);
        }
    }
}
