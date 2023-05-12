using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZdravoCorp.Domain.Calendar
{
    public class TimeSlot
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public TimeSlot()
        {
            Start = new();
            End = new();
        }
        public TimeSlot(DateTime start, DateTime end)
        {
            if (end <= start)
            {
                throw new ArgumentException("End date can't be smaller than start date.");
            }

            Start = start;
            End = end;
        }

        public bool OverlapsWith(TimeSlot timeSlot)
        {
            return Start < timeSlot.End && timeSlot.Start < End;
        }

        public bool IsInFuture()
        {
            return Start >= DateTime.Now;
        }
    }
}
