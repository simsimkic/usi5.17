using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Domain.Equipment;
using ZdravoCorp.Domain.Orders;
using ZdravoCorp.Repository.Equipment;
using ZdravoCorp.Repository.Orders;
using ZdravoCorp.Services.Equipment;

namespace ZdravoCorp.Services.Orders
{
    public static class OrdersService
    {
        public static void GenerateUniqueId(OrderItem orderItem)
        {
            bool isUnique = false;
            string id = "";
            while (!isUnique)
            {
                id = RandomStringGenerator.GenerateRandomString(5);
                isUnique = OrdersRepository.OrderItems.FindAll(item => item.Id == id).Count==0;
            }

            orderItem.Id = id;
        }

        public static List<OrderItem> GetOrders()
        {
            return OrdersRepository.OrderItems;
        }

        public static void MakeOrder(InventoryItem inventoryItem)
        {
            OrderItem orderItem  = new OrderItem();
            orderItem.InventoryItem = inventoryItem;
            GenerateUniqueId(orderItem);
            OrdersRepository.OrderItems.Add(orderItem);
            OrdersRepository.Save();
        }

        public static void CheckOrders()
        {
            foreach (var item in OrdersRepository.OrderItems)
            {
                if (item.Status == OrderItem.OrderStatus.SENT)
                {
                    if (item.Time < DateTime.Now)
                    {
                        item.Status = OrderItem.OrderStatus.FINISHED;
                        InventoryService.ReceiveOrder(item.InventoryItem);
                    }
                }
            }
            OrdersRepository.Save();
        }
    }
}
