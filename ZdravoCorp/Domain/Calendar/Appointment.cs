using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Domain.Users;
using ZdravoCorp.Presentation;
using ZdravoCorp.Repository;
using ZdravoCorp.Repository.Serializer;
using ZdravoCorp.Services;
using ZdravoCorp.Services.Rooms;
using ZdravoCorp.Domain.Room;
using static ZdravoCorp.Domain.Users.User;

namespace ZdravoCorp.Domain.Calendar
{
    public class Appointment : Serializable
    {
        public enum AppointmentType
        {
            Operation,
            Examination
        }

        public enum AppointmentStatus
        {
            Active,
            Canceled,
            Finished,
            CheckedIn
        }

        public int Id { get; set; }
        public TimeSlot TimeSlot { get; set; }
        public string DoctorUsername { get; set; }
        public string PatientUsername { get; set; }
        public AppointmentType Type { get; set; }
        public AppointmentStatus Status { get; set; }
        public string RoomName { get; set; }
        public bool HasPatientAppointed { get; set; }
        public bool HasPatientEdited { get; set; }
        public bool HasPatientCanceled { get; set; }
        public bool IsEmergency { get; set; }
        public Appointment()
        {
            AssignId();
            TimeSlot = new();
            DoctorUsername = "";
            PatientUsername = "";
            Type = new AppointmentType();
            Status = new AppointmentStatus();
            RoomName = "";
            HasPatientAppointed = false;
            HasPatientEdited = false;
            HasPatientCanceled = false;
            IsEmergency = false;
        }

        public Appointment(TimeSlot timeSlot, string doctorUsername, string patientUsername, AppointmentType type, AppointmentStatus status, string roomName, bool hasPatientAppointed, bool hasPatientEdited, bool hasPatientCanceled, bool isEmergency = false)
        {
            AssignId();
            TimeSlot = timeSlot;
            DoctorUsername = doctorUsername;
            PatientUsername = patientUsername;
            Type = type;
            Status = status;
            RoomName = roomName;
            HasPatientAppointed = hasPatientAppointed;
            HasPatientEdited = hasPatientEdited;
            HasPatientCanceled = hasPatientCanceled;
            IsEmergency = isEmergency;
        }
        public Appointment(int id, TimeSlot timeSlot, string doctorUsername, string patientUsername, AppointmentType type, AppointmentStatus status, string roomName, bool hasPatientAppointed, bool hasPatientEdited, bool hasPatientCanceled, bool isEmergency = false)
        {
            Id = id;
            TimeSlot = timeSlot;
            DoctorUsername = doctorUsername;
            PatientUsername = patientUsername;
            Type = type;
            Status = status;
            RoomName = roomName;
            HasPatientAppointed = hasPatientAppointed;
            HasPatientEdited = hasPatientEdited;
            HasPatientCanceled = hasPatientCanceled;
            IsEmergency = isEmergency;
        }

        public bool IsInNextFifteenMinutes()
        {
            return this.TimeSlot.Start < DateTime.Now.Add(new TimeSpan(0,15,0)) && this.Status == AppointmentStatus.Active;
        }
        public bool IsInNextTwoHours()
        {
            return this.TimeSlot.Start >= DateTime.Now && this.TimeSlot.Start < DateTime.Now.Add(new TimeSpan(2, 0, 0)) && this.Status == AppointmentStatus.Active;
        }

        public TimeSlot EarliestDelay()
        {
            TimeSpan appointmentDuration =  this.TimeSlot.End - this.TimeSlot.Start;
            TimeSlot delaySlot = new TimeSlot(DateTime.Now.AddHours(2), DateTime.Now.AddHours(2).AddMinutes(Convert.ToInt32(appointmentDuration.TotalMinutes)));

            while (true)
            {
                RoundDateTimeUpToNearestQuarterHour(delaySlot);

                Patient patient = PatientService.GetPatient(this.PatientUsername)!;
                Room.Room? room = GetRoom(delaySlot);
                Doctor? doctor = DoctorService.GetDoctor(this.DoctorUsername);

                if (doctor?.IsAvailable(delaySlot) == true && patient.IsAvailable(delaySlot) && room != null) return delaySlot;

                delaySlot.Start = delaySlot.Start.AddMinutes(15);
                delaySlot.End = delaySlot.End.AddMinutes(15);
                
            }
        }

        private Room.Room? GetRoom(TimeSlot delaySlot)
        {
            try
            {
                Room.Room.RoomType roomType = Room.Room.RoomType.OperationRoom;
                if (this.Type == AppointmentType.Examination) roomType = Room.Room.RoomType.ExaminationRoom;

                Room.Room room = RoomService.GetFreeRoom(roomType, delaySlot);
                return room;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static void RoundDateTimeUpToNearestQuarterHour(TimeSlot delaySlot)
        {
            while (delaySlot.Start.Minute % 15 != 0)
            {
                delaySlot.Start = delaySlot.Start.AddMinutes(1);
                delaySlot.End = delaySlot.End.AddMinutes(1);
            }
        }

        public void Cancel()
        {
            if (Status != AppointmentStatus.Active)
            {
                throw new InvalidOperationException("This appointment cannot be canceled.");
            }

            Status = AppointmentStatus.Canceled;
        }

        public void PatientCancel()
        {
            if (Status != AppointmentStatus.Active)
            {
                throw new InvalidOperationException("This appointment cannot be canceled.");
            }

            Status = AppointmentStatus.Canceled;
            HasPatientCanceled = true;
        }

        public bool IsInRange(DateTime dateStart, DateTime dateEnd)
        {
            return TimeSlot.Start.Date >= dateStart && TimeSlot.Start.Date <= dateEnd;
        }

        public bool IsActive()
        {
            return Status == AppointmentStatus.Active;
        }
        public bool IsCanceled()
        {
            return Status == AppointmentStatus.Canceled;
        }
        public bool IsFinished()
        {
            return Status == AppointmentStatus.Finished;
        }
        public bool IsCheckedIn()
        {
            return Status == AppointmentStatus.CheckedIn;
        }

        public void AssignRoom()
        {
            if (Type == AppointmentType.Examination)
            {
                RoomName = RoomService.GetFreeRoom(Room.Room.RoomType.ExaminationRoom, TimeSlot).Name;
            }
            else if (Type == AppointmentType.Operation)
            {
                RoomName = RoomService.GetFreeRoom(Room.Room.RoomType.OperationRoom, TimeSlot).Name;
            }
        }
        private int GenerateId()
        {
            Random rnd = new Random();
            return rnd.Next(1, 99999);
        }

        private void AssignId()
        {
            do
            {
                Id = GenerateId();
            } while (AppointmentRepository.Appointments.ContainsKey(Id));
        }

        public string[] ToTable()
        {
            string[] tableValues =
            {
                Id.ToString(),
                TimeSlot.Start.ToString(),
                TimeSlot.End.ToString(),
                DoctorUsername,
                PatientUsername,
                Type.ToString(),
                Status.ToString(),
                RoomName
            };
            return tableValues;
        }

        public string[] ToCSV()
        {
            string[] csvValues =
            {
                Id.ToString(),
                TimeSlot.Start.ToString(),
                TimeSlot.End.ToString(),
                DoctorUsername,
                PatientUsername,
                Type.ToString(),
                Status.ToString(),
                RoomName,
                HasPatientAppointed.ToString(),
                HasPatientEdited.ToString(),
                HasPatientCanceled.ToString(),
                IsEmergency.ToString()
            };
            return csvValues;
        }

        public void FromCSV(string[] values)
        {
            Id = int.Parse(values[0]);
            TimeSlot.Start = DateTime.Parse(values[1]);
            TimeSlot.End = DateTime.Parse(values[2]);
            DoctorUsername = values[3];
            PatientUsername = values[4];
            Type = (AppointmentType)Enum.Parse(typeof(AppointmentType), values[5]);
            Status = (AppointmentStatus)Enum.Parse(typeof(AppointmentStatus), values[6]);
            RoomName = values[7];
            HasPatientAppointed = bool.Parse(values[8]);
            HasPatientEdited = bool.Parse(values[9]);
            HasPatientCanceled = bool.Parse(values[10]);
            IsEmergency = bool.Parse(values[11]);
        }
    }
}
