using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Domain.Equipment;
using ZdravoCorp.Domain.Room;
using ZdravoCorp.Repository.Equipment;
using ZdravoCorp.Repository.Rooms;

namespace ZdravoCorp.Services.Equipment
{
    public class TransferItemService
    {


        public static void DecreaseItemQuantity(TransferItemRequest transferRequest)
        {
            var transferItem = new InventoryItem(transferRequest.InventoryItem);
            InventoryItem oldInventoryItem = InventoryRepository.GetItem(transferItem.Equipment.Name, transferItem.Room.Name);
            if (oldInventoryItem.Quantity - transferItem.Quantity < 0)
            {
                throw new Exception("not enough items to be transfered");
            }
            else
            {
                InventoryService.SpendItem(oldInventoryItem,transferItem.Quantity);
            }
        }

        public static void TransferItemRequest(InventoryItem item, Room destinationRoom, DateTime moveTime)
        {
            TransferItemRequest moveRequest = new TransferItemRequest(item, destinationRoom, moveTime);
            GenerateUniqueId(moveRequest);
            TransferItemRequestsRepository.TransferRequests.Add(moveRequest);
            TransferItemRequestsRepository.Save();
        }

        public static void CheckTransferRequests()
        {
            foreach (var request in TransferItemRequestsRepository.TransferRequests)
            {
                if (request.Status == Domain.Equipment.TransferItemRequest.TransferStatus.SENT)
                {
                    if (request.TransferTime < DateTime.Now)
                    {
                        TransferItem(request);
                    }
                }
            }
            TransferItemRequestsRepository.Save();
        }

        public static void TransferItem(TransferItemRequest transferItemRequest)
        {
            try
            {
                DecreaseItemQuantity(transferItemRequest);
            }
            catch (Exception e)
            {
                transferItemRequest.CancelTransfer();
                TransferItemRequestsRepository.Save();
                return;
            }
            
            MoveItemToDestinationRoom(transferItemRequest);

            transferItemRequest.FinishTransfer();
            InventoryRepository.Save();
        }

        private static void MoveItemToDestinationRoom(TransferItemRequest transferItemRequest)
        {
            var itemInRoom = InventoryRepository.GetItem(transferItemRequest.InventoryItem.Equipment.Name,
                transferItemRequest.DestinationRoom.Name);
            var transferedItem = new InventoryItem(transferItemRequest.InventoryItem);

            if (itemInRoom == null)
            {
                transferedItem.Room = new Room(transferItemRequest.DestinationRoom);
                InventoryRepository.InventoryItems.Add(transferedItem);
            }
            else
            {
                itemInRoom.Quantity += transferedItem.Quantity;
            }
        }

        public static void GenerateUniqueId(TransferItemRequest moveRequest)
        {
            bool isUnique = false;
            string id = "";
            while (!isUnique)
            {
                id = RandomStringGenerator.GenerateRandomString(5);
                isUnique = TransferItemRequestsRepository.TransferRequests.FindAll(item => item.Id == id).Count == 0;
            }

            moveRequest.Id = id;
        }

    }
}
