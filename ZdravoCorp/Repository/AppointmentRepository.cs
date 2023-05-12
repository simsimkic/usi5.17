using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Domain;
using ZdravoCorp.Domain.Calendar;
using ZdravoCorp.Repository.Serializer;

namespace ZdravoCorp.Repository
{
    public class AppointmentRepository
    {
        public const string AppointmentsFilePath = "..\\..\\..\\Data\\appointments.csv";
        public static Dictionary<int, Appointment> Appointments = new();
        public static Serializer<Appointment> AppointmentSerializer = new();

        public AppointmentRepository()
        {
            List<Appointment> parsedAppointments = AppointmentSerializer.fromCSV(AppointmentsFilePath);
            Appointments = parsedAppointments.ToDictionary(app => app.Id, app => app);
        }

        public static List<Appointment> GetAllAppointmentsForPatient(string username)
        {
            List<Appointment> appointments = new List<Appointment>();
            foreach (Appointment appointment in Appointments.Values)
            {
                if (appointment.PatientUsername == username)
                {
                    appointments.Add(appointment);
                }
            }

            return appointments;
        }

        public static void Save()
        {
            AppointmentSerializer.toCSV(AppointmentsFilePath, Appointments.Values.ToList());
        }
    }
}
