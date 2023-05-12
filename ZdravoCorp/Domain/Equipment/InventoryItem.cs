using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ZdravoCorp.Repository.Equipment;
using ZdravoCorp.Repository.Rooms;
using ZdravoCorp.Repository.Serializer;
using static System.Int32;
using static ZdravoCorp.Domain.Equipment.Equipment;

namespace ZdravoCorp.Domain.Equipment
{
    public class InventoryItem:Serializable
    {
        public Equipment Equipment { get; set; }
        public Room.Room Room { get; set; }
        public int Quantity { get; set; }

        public InventoryItem()
        {
            Equipment = new Equipment();
            Room = new Room.Room();
            Quantity = 0;
        }

        public InventoryItem(Equipment equipment, Room.Room room, int quantity)
        {
            Equipment = equipment;
            Room = room;
            Quantity = quantity;
        }

        public InventoryItem(InventoryItem item)
        {
            Equipment = item.Equipment;
            Room = item.Room;
            Quantity = item.Quantity;

        }

        public string[] ToCSV()
        {
            string[] csvValues =
            {
                Equipment.Id,
                Room.Name,
                Quantity.ToString()
            };
            return csvValues;
        }

        public void FromCSV(string[] values)
        {
            Equipment = EquipmentRepository.GetEquipment(values[0]);
            Room = RoomRepository.GetRoom(values[1]);
            Quantity = Parse(values[2]);
        }

        public string[] ToTable()
        {
            string[] rowValues =
            {
                Equipment.Name,
                Equipment.Type.ToString(),
                Equipment.IsDynamic.ToString(),
                Room.Name,
                Room.Type.ToString(),
                Quantity.ToString()
            };
            return rowValues;
        }
    }
}
