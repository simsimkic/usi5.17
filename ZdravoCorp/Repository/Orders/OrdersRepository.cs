using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Domain.Equipment;
using ZdravoCorp.Domain.Orders;
using ZdravoCorp.Repository.Equipment;
using ZdravoCorp.Repository.Serializer;

namespace ZdravoCorp.Repository.Orders
{
    public class OrdersRepository
    {
        public const string OrderRepositoryItemsFilePath = "..\\..\\..\\Data\\Orders\\orderItems.csv";
        public static List<OrderItem> OrderItems = new();
        public static Serializer<OrderItem> OrdersSerializer = new();
        private static readonly object SaveLock = new object();
        public OrdersRepository()
        {
            OrderItems = OrdersSerializer.fromCSV(OrderRepositoryItemsFilePath);
        }

        public static void Save()
        {
            lock (SaveLock)
            {
                OrdersSerializer.toCSV(OrderRepositoryItemsFilePath, OrderItems);
            }
        }
        
    }
}
