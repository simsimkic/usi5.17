using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentScheduler;
using ZdravoCorp.Presentation.Notifications;
using ZdravoCorp.Services.Orders;

namespace ZdravoCorp.Services.Equipment.InventoryTimer
{
    public class InventoryTimerJob : IJob
    {
        public void Execute()
        {
            Task.Run(OrdersService.CheckOrders);
            Task.Run(TransferItemService.CheckTransferRequests);
        }
    }
}
