using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Domain;
using ZdravoCorp.Domain.Users;
using ZdravoCorp.Repository.Serializer;
using static ZdravoCorp.Domain.Users.Patient;

namespace ZdravoCorp.Repository
{
    public class PatientRepository
    {
        public const string PatientsFilePath = "..\\..\\..\\Data\\Patient\\patients.csv";

        public static List<Patient> Patients = new();
        public static Serializer<Patient> PatientSerializer = new();

        public PatientRepository()
        {
            Patients = PatientSerializer.fromCSV(PatientsFilePath);
        }

        public static void SaveRepository()
        {
            PatientSerializer.toCSV(PatientsFilePath, Patients);
        }

        public static List<Patient> GetAllActivePatients()
        {
            return Patients.Where(patient => patient.Status == BlockStatus.Active).ToList();
        }

        public static Patient GetPatient(string username)
        {
            foreach (var patient in Patients)
            {
                if (patient.Username == username) return patient;
            }

            throw new ArgumentException("Patient has not been found.");
        }

        public static void EditPatient(string oldUsername, string newUsername, string firstName, string lastName,
            bool isChecked)
        {
            var patient = GetPatient(oldUsername);
            if (patient == null) return; 

            patient!.Username = newUsername;
            patient.FirstName = firstName;
            patient.LastName = lastName;
            patient.Status = isChecked ? BlockStatus.Blocked : BlockStatus.Active;

            SaveRepository();
        }

        public static void DeletePatient(string username)
        {
            Patient patient = GetPatient(username)!;
            Patients.Remove(patient);
            SaveRepository();
        }
    }
}
