using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Domain;
using ZdravoCorp.Domain.Common;
using ZdravoCorp.Repository.Serializer;

namespace ZdravoCorp.Repository
{
    public class MedicalRecordRepository
    {
        public const string MedicalRecordsFilePath = "..\\..\\..\\Data\\Patient\\medical_records.csv";

        public static List<MedicalRecord> MedicalRecords = new();
        public static Serializer<MedicalRecord> MedicalRecordSerializer = new();

        public MedicalRecordRepository()
        {
            MedicalRecords = MedicalRecordSerializer.fromCSV(MedicalRecordsFilePath);
        }

        public static void SaveRepository()
        {
            MedicalRecordSerializer.toCSV(MedicalRecordsFilePath, MedicalRecords);
        }
        public static MedicalRecord? GetMedicalRecord(string patientUsername)
        {
            return MedicalRecordRepository.MedicalRecords.FirstOrDefault(record => record.PatientUsername == patientUsername);
        }

        public static void EditMedicalRecord(string oldUsername, string newUsername, double height, double weight,
            DateOnly dateOfBirth, List<string> diseaseHistory)
        {
            MedicalRecord? medicalRecord = GetMedicalRecord(oldUsername);

            medicalRecord!.PatientUsername = newUsername;
            medicalRecord.Height = height;
            medicalRecord.Weight = weight;
            medicalRecord.DateOfBirth = dateOfBirth;
            medicalRecord.DiseaseHistory = diseaseHistory;

            SaveRepository();
        }

        public static void EditMedicalRecord(MedicalRecord medicalRecord)
        {
            MedicalRecord? newMedicalRecord = GetMedicalRecord(medicalRecord.PatientUsername);

            newMedicalRecord.Height = medicalRecord.Height;
            newMedicalRecord.Weight = medicalRecord.Weight;
            newMedicalRecord.DateOfBirth = medicalRecord.DateOfBirth;
            newMedicalRecord.DiseaseHistory = medicalRecord.DiseaseHistory;

            SaveRepository();
        }

        public static void DeleteMedicalRecord(string patientsUsername)
        {
            MedicalRecord medicalRecord = GetMedicalRecord(patientsUsername)!;
            MedicalRecordRepository.MedicalRecords.Remove(medicalRecord);
            MedicalRecordRepository.SaveRepository();
        }

    }
}
