using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Domain.Equipment;
using ZdravoCorp.Repository.Equipment;
using ZdravoCorp.Repository.Rooms;
using ZdravoCorp.Repository.Serializer;
using static ZdravoCorp.Domain.Equipment.Equipment;

namespace ZdravoCorp.Domain.Orders
{
    public class OrderItem : Serializable
    {
        public enum OrderStatus
        {
            SENT,
            CANCELLED,
            FINISHED
        }
        public InventoryItem InventoryItem { get; set; }
        public DateTime Time { get; set; }
        public string Id { get; set; }
        public OrderStatus Status { get; set; }

        public OrderItem(string id,InventoryItem inventoryItem, DateTime time, OrderStatus status)
        {
            Id = id;
            InventoryItem = inventoryItem;
            Time = time;
            Status = status;
        }

        public OrderItem(string id, InventoryItem inventoryItem, DateTime time, string status)
        {
            Id = id;
            InventoryItem = inventoryItem;
            Time = time;
            Status = (OrderStatus)Enum.Parse(typeof(OrderStatus), status);
        }

        public OrderItem()
        {
            Id = "";
            InventoryItem = new();
            Time = DateTime.Now.AddDays(1);
            Status = OrderItem.OrderStatus.SENT;
        }

        public string[] ToCSV()
        {
            string[] csvValues =
            {
                Id,
                InventoryItem.Equipment.Id,
                InventoryItem.Room.Name,
                InventoryItem.Quantity.ToString(),
                Time.ToString(),
                Status.ToString(),
            };
            return csvValues;
        }

        public void FromCSV(string[] values)
        {
            Id = values[0];
            InventoryItem.Equipment = EquipmentRepository.GetEquipment(values[1]);
            InventoryItem.Room = RoomRepository.GetRoom(values[2]);
            InventoryItem.Quantity = int.Parse(values[3]);
            Time = DateTime.Parse(values[4]);
            Status = (OrderStatus)Enum.Parse(typeof(OrderStatus), values[5]); 
        }

        public string[] ToTable()
        {
            string[] rowValues =
            {
                Id,
                InventoryItem.Equipment.Name,
                InventoryItem.Equipment.Type.ToString(),
                InventoryItem.Equipment.IsDynamic.ToString(),
                InventoryItem.Room.Name,
                InventoryItem.Room.Type.ToString(),
                InventoryItem.Quantity.ToString(),
                Time.ToString(),
                Status.ToString()
            };
            return rowValues;
        }
    }
}
