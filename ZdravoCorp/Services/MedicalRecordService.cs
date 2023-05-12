using System.Collections.Generic;
using System;
using System.Linq;
using ZdravoCorp.Repository;
using ZdravoCorp.Domain.Common;

namespace ZdravoCorp.Services
{
    public static class MedicalRecordService
    {
        public static MedicalRecord? GetMedicalRecord(string patientUsername) {
            return MedicalRecordRepository.MedicalRecords.FirstOrDefault(record => record.PatientUsername == patientUsername);
        }

        public static void EditMedicalRecord(string oldUsername, string newUsername, double height, double weight,
            DateOnly dateOfBirth, List<string> diseaseHistory)
        {
            MedicalRecordRepository.EditMedicalRecord(oldUsername,newUsername,height,weight,dateOfBirth,diseaseHistory);
        }

        public static void EditMedicalRecord(MedicalRecord medicalRecord)
        {
            MedicalRecordRepository.EditMedicalRecord(medicalRecord);
        }

        public static void DeleteMedicalRecord(string patientsUsername)
        {
            MedicalRecordRepository.DeleteMedicalRecord(patientsUsername);
        }
        public static void SaveRepository()
        {
            MedicalRecordRepository.SaveRepository();
        }

        public static void AddMedicalRecord(MedicalRecord medicalRecord)
        {
            MedicalRecordRepository.MedicalRecords.Add(medicalRecord);
            MedicalRecordRepository.SaveRepository();
        }
    }
}
