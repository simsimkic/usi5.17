using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Domain.Equipment;
using ZdravoCorp.Domain.Orders;
using ZdravoCorp.Domain.Room;
using ZdravoCorp.Repository.Equipment;
using ZdravoCorp.Repository.Orders;

namespace ZdravoCorp.Services.Equipment
{
    public static class InventoryService
    {
        public static List<InventoryItem> GetInventoryItems()
        {
            return InventoryRepository.InventoryItems;
        }

        public static List<InventoryItem> GetItemsNotInStorage()
        {
            return InventoryRepository.FilterNotInStorage();
        }

        public static List<InventoryItem> GetSearchItems(string search)
        {
            return search.Length > 0 ?
                InventoryRepository.FilterContainingSearch(search)
                : InventoryRepository.InventoryItems;
        }

        public static List<InventoryItem> GetItemsFilteredByEquipment(string equipmentType)
        {
            return InventoryRepository.FilterByEquipment(equipmentType);
        }

        public static List<InventoryItem> GetItemsFilteredByQuantityRange(int lowerRange, int upperRange)
        {
            return InventoryRepository.FilterByQuantityRange(lowerRange, upperRange);
        }

        public static List<InventoryItem> GetItemsFilteredByQuantity(int quantity)
        {
            return InventoryRepository.FilterByQuantity(quantity);
        }

        public static List<InventoryItem> GetItemsFilteredByRooms(string roomType)
        {
            return InventoryRepository.FilterByRoomType(roomType.Replace(" ", ""));
        }

        public static List<InventoryItem> GetMissingItems()
        {
            return InventoryRepository.FilterByQuantityRange(0, 5).FindAll(item => item.Equipment.IsDynamic);
        }

        public static List<InventoryItem> GetDynamicEquipmentInRoom(string roomName)
        {
            return InventoryRepository.FilterByRoom(roomName).Where(inventoryItem => inventoryItem.Equipment.IsDynamic).ToList();
        }

        public static void ReceiveOrder(InventoryItem inventoryItem)
        {
            InventoryItem itemInStorage = InventoryRepository.InventoryItems.Find(item =>
                Equals(item.Equipment, inventoryItem.Equipment) && Equals(item.Room.Type, Room.RoomType.Storage));
            if (itemInStorage == null)
            {
                inventoryItem.Room = new Room();
                InventoryRepository.InventoryItems.Add(inventoryItem);
            }
            else
            {
                itemInStorage.Quantity += inventoryItem.Quantity;
            }
            InventoryRepository.Save();
        }

        public static void SpendItem(InventoryItem inventoryItem, int spentQuantity)
        {
            inventoryItem.Quantity -= spentQuantity;
            InventoryRepository.Save();
        }
    }
}
 