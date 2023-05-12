using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using ZdravoCorp.Domain.Users;
using ZdravoCorp.Repository;
using static ZdravoCorp.Domain.Users.Patient;

namespace ZdravoCorp.Services
{
    public static class PatientService
    {
        public static void SaveRepository()
        {
            PatientRepository.SaveRepository();
        }

        public static Patient? GetPatient(string username)
        {
            try
            {
                return PatientRepository.GetPatient(username);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<Patient> GetAllPatients()
        {
            return PatientRepository.Patients;
        }

        public static List<Patient> GetAllActivePatients()
        {
            return PatientRepository.GetAllActivePatients();
        }

        public static void BlockPatient(string patientUsername)
        {
            Patient patient = GetPatient(patientUsername);
            patient.Status = BlockStatus.Blocked;
            PatientRepository.SaveRepository();
            
        }

        public static bool IsPatientBlocked(string patientUsername)
        {
            Patient patient = GetPatient(patientUsername);
            return patient.Status == BlockStatus.Blocked;
        }

        public static void EditPatient(string oldUsername, string newUsername, string firstName, string lastName,
            bool isChecked)
        {
            PatientRepository.EditPatient(oldUsername,newUsername,firstName,lastName,isChecked);
        }
        public static void DeletePatient(string username)
        {
            PatientRepository.DeletePatient(username);
        }

        public static void AddPatient(Patient patient)
        {
            PatientRepository.Patients.Add(patient);
            PatientRepository.SaveRepository();
        }
    }


}
