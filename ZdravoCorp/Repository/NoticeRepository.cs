using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Domain.Common;
using ZdravoCorp.Repository.Serializer;

namespace ZdravoCorp.Repository
{
    public class NoticeRepository
    {
        public const string NoticeFilePath = "..\\..\\..\\Data\\notice.csv";
        
        public static List<Notice> Notices = new();
        public static Serializer<Notice> NoticeSerializer = new();

        public NoticeRepository()
        {
            Notices = NoticeSerializer.fromCSV(NoticeFilePath);
        }

        public static void SaveRepository()
        {
            NoticeSerializer.toCSV(NoticeFilePath,Notices);
        }

        public static List<Notice> GetAllUsersNotices(string username)
        {
            return Notices.Where(notice => notice.NotifiedUserUsername == username).ToList();
        }
    }
}
