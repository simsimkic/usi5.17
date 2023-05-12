using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentScheduler;

namespace ZdravoCorp.Services.Equipment.InventoryTimer
{
    public class InventoryTimerRegistry:Registry
    {
        public InventoryTimerRegistry()
        {
            Schedule<InventoryTimerJob>().ToRunNow().AndEvery(60).Seconds();
        }
    }
}
