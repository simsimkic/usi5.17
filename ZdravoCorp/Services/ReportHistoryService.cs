using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Repository;

namespace ZdravoCorp.Services
{
    internal class ReportHistoryService
    {
        public static Dictionary<int, string> GetReportHistory(string username)
        {
            return ReportHistoryRepository.PatientsReportHistory[username];
        }

        public static void InitializeReportHistory(string username)
        {
            ReportHistoryRepository.AddReportHistory(username);
        }

        public static void AddOrEditReport(string username, int id, string reportContent)
        {
            ReportHistoryRepository.UpdateReport(username, id, reportContent);
        }

        public static void AddOrEditReportHistory(string username, Dictionary<int, string> reportHistory)
        {
            ReportHistoryRepository.UpdateReportHistory(username, reportHistory);
        }
    }
}
