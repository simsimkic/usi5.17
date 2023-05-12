using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Domain.Room;
using ZdravoCorp.Repository.Equipment;
using ZdravoCorp.Repository.Rooms;
using ZdravoCorp.Repository.Serializer;
using static ZdravoCorp.Domain.Orders.OrderItem;

namespace ZdravoCorp.Domain.Equipment
{
    public class TransferItemRequest:Serializable
    {

        public enum TransferStatus
        {
            SENT,
            FINISHED,
            CANCELLED
        }
        public string Id { get; set; }
        public InventoryItem InventoryItem { get; set; }
        public DateTime TransferTime { get; set; }
        public Room.Room DestinationRoom { get; set; }
        public TransferStatus Status { get; set; }

        public TransferItemRequest()
        {
            Id = "";
            InventoryItem = new InventoryItem();
            TransferTime = DateTime.Now.AddDays(1);
            DestinationRoom = new Room.Room();
            Status = TransferStatus.SENT;
        }

        public TransferItemRequest(InventoryItem inventoryItem, Room.Room destinationRoom,DateTime dateTime)
        {
            Id = "";
            InventoryItem = new(inventoryItem);
            DestinationRoom = destinationRoom;
            Status = TransferStatus.SENT;
            TransferTime = inventoryItem.Equipment.IsDynamic ? DateTime.Now : dateTime;
        }

        public TransferItemRequest(string id,InventoryItem inventoryItem, DateTime transferTime, Room.Room destinationRoom, TransferStatus status)
        {
            Id = id;
            InventoryItem = new(inventoryItem);
            TransferTime = transferTime;
            DestinationRoom = destinationRoom;
            Status = status;
        }

        public string[] ToCSV()
        {
            string[] csvValues =
            {
                Id,
                InventoryItem.Equipment.Id,
                InventoryItem.Room.Name,
                InventoryItem.Quantity.ToString(),
                DestinationRoom.Name,
                TransferTime.ToString(),
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
            DestinationRoom  = RoomRepository.GetRoom(values[4]);
            TransferTime = DateTime.Parse(values[5]);
            Status = (TransferStatus)Enum.Parse(typeof(TransferStatus), values[6]);
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
                DestinationRoom.Name,
                TransferTime.ToString(),
                Status.ToString()
            };
            return rowValues;
        }

        public void CancelTransfer()
        {
            Status = TransferStatus.CANCELLED;
        }

        public void FinishTransfer()
        {
            Status = TransferStatus.FINISHED;
        }
    }
}
