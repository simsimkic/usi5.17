using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ZdravoCorp.Domain.Users;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Formatting = Newtonsoft.Json.Formatting;

namespace ZdravoCorp.Repository
{
    public class ReportHistoryRepository
    {
        public const string ReportHistoryFilePath = "..\\..\\..\\Data\\Patient\\report_history.json";
        public static Dictionary<string,Dictionary<int, string>> PatientsReportHistory = new();

        public ReportHistoryRepository()
        {
            if (!File.Exists(ReportHistoryFilePath)) return;

            string json = File.ReadAllText(ReportHistoryFilePath);
            PatientsReportHistory = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<int, string>>>(json);
        }

        public static void Save()
        {
            string json = JsonConvert.SerializeObject(PatientsReportHistory, Formatting.Indented);
            File.WriteAllText(ReportHistoryFilePath, json);
        }

        public static void AddReportHistory(string username)
        {
            PatientsReportHistory[username] = new Dictionary<int, string>();
            Save();
        }

        public static void UpdateReport(string username, int id, string reportContent)
        {
            PatientsReportHistory[username][id] = reportContent;
            Save();
        }

        public static void UpdateReportHistory(string username, Dictionary<int, string> reportHistory)
        {
            PatientsReportHistory[username] = reportHistory;
            Save();
        }
        private static DateTime RoundDate(DateTime reportCreationTime)
        {
            return new DateTime(reportCreationTime.Year, reportCreationTime.Month,
                reportCreationTime.Day, reportCreationTime.Hour, reportCreationTime.Minute, reportCreationTime.Second);
        }


    }
}
