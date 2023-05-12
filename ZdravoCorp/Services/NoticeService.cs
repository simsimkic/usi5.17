using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Domain.Common;
using ZdravoCorp.Repository;

namespace ZdravoCorp.Services
{
    public static class NoticeService
    {
        public static List<Notice> GetAllUsersNotices(string username)
        {
            return NoticeRepository.GetAllUsersNotices(username);
        }

        public static void Add(Notice notice)
        {
            NoticeRepository.Notices.Add(notice);
            NoticeRepository.SaveRepository();
        }
    }
}
