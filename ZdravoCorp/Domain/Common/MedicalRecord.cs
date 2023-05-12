using System;
using System.Collections.Generic;
using System.Data;
using ZdravoCorp.Repository.Serializer;

namespace ZdravoCorp.Domain.Common
{
    public class MedicalRecord : Serializable
    {
        public string PatientUsername { get; set; }
        public double Weight { get; set; }
        public double Height { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public Dictionary<DateTime, string> ReportHistory { get; set; }
        public List<string> DiseaseHistory { get; set; }

        public MedicalRecord() { }
        public MedicalRecord(double weight, double height, string username, DateOnly dateOfBirth, List<string> diseaseHistory)
        {
            PatientUsername = username;
            Weight = weight;
            Height = height;
            DateOfBirth = dateOfBirth;
            DiseaseHistory = diseaseHistory;
        }

        private string DiseaseHistoryToString()
        {
            bool skipComma = true;
            string stringBuilder = string.Empty;

            foreach (var disease in DiseaseHistory)
            {
                if (!skipComma)
                {
                    stringBuilder += "," + disease;
                }
                else
                {
                    stringBuilder += disease;
                    skipComma = false;
                }
            }
            return stringBuilder;
        }

        private void StringToDiseaseHistory(string diseaseHistoryString)
        {
            DiseaseHistory = new List<string>(diseaseHistoryString.Split(','));
        }

        public void AddDisease(string disease)
        {
            DiseaseHistory.Add(disease);
        }

        public string[] ToCSV()
        {
            string[] csvValues =
            {
                PatientUsername,
                Math.Round(Weight, 2).ToString(),
                Math.Round(Height, 2).ToString(),
                DateOfBirth.ToString("O"),
                DiseaseHistoryToString()
            };
            return csvValues;
        }

        public void FromCSV(string[] values)
        {
            PatientUsername = values[0];
            Weight = double.Parse(values[1]);
            Height = double.Parse(values[2]);
            DateOfBirth = DateOnly.ParseExact(values[3], "O", null);
            StringToDiseaseHistory(values[4]);
        }

    }
}
