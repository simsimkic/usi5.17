using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Domain;
using ZdravoCorp.Domain.Equipment;
using ZdravoCorp.Domain.Room;
using ZdravoCorp.Repository.Rooms;
using ZdravoCorp.Repository.Serializer;
using static ZdravoCorp.Domain.Equipment.Equipment;



namespace ZdravoCorp.Repository.Equipment
{
    public class InventoryRepository
    {
        public const string InventoryItemsFilePath = "..\\..\\..\\Data\\Equipment\\inventoryItems.csv";
        public static List<InventoryItem> InventoryItems = new();
        public static Serializer<InventoryItem> InventorySerializer = new();
        private static readonly object SaveLock = new object();

        public InventoryRepository()
        {
            InventoryItems = InventorySerializer.fromCSV(InventoryItemsFilePath);
        }

        public static void Save()
        {
            lock (SaveLock)
            {
                InventorySerializer.toCSV(InventoryItemsFilePath, InventoryItems);
            }
        }

        public static List<InventoryItem> FilterByEquipment(string type)
        {
            var equipmentType = (EquipmentType)Enum.Parse(typeof(EquipmentType), type);
            return InventoryItems.FindAll(item => item.Equipment.Type == equipmentType);
        }

        public static List<InventoryItem> FilterByRoomType(string type)
        {
            
            var roomType = (Room.RoomType)Enum.Parse(typeof(Room.RoomType), type);
            return InventoryItems.FindAll(item => item.Room.Type == roomType);
        }

        public static List<InventoryItem> FilterByRoom(string roomName)
        {
            return InventoryItems.FindAll(item => item.Room.Name == roomName);
        }

        public static List<InventoryItem> FilterByQuantity(int quantity)
        {
            return InventoryItems.FindAll(item => item.Quantity == quantity);
        }

        public static List<InventoryItem> FilterByQuantityRange(int lowerRange, int upperRange)
        {
            return InventoryItems.FindAll(item => (item.Quantity >= lowerRange) && (item.Quantity <= upperRange));
        }

        public static List<InventoryItem> FilterNotInStorage()
        {
            return InventoryItems.FindAll(item => item.Room.Type != Room.RoomType.Storage);
        }
        
        public static List<InventoryItem> FilterContainingSearch(string search)
        {
            return InventoryItems.FindAll(item => item.Equipment.Contains(search)
                                                  || item.Room.Contains(search) || item.Quantity.ToString().Contains(search));
        }

        public static InventoryItem? GetItem(string equipmentName, string roomName)
        {
            return InventoryItems.FirstOrDefault(item =>
                Equals(item.Equipment.Name, equipmentName) &&
                Equals(item.Room.Name, roomName));
        }
    }
}
