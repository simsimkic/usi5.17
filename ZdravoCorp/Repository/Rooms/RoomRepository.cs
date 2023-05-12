using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Domain.Equipment;
using ZdravoCorp.Domain;
using ZdravoCorp.Domain.Room;
using ZdravoCorp.Repository.Serializer;

namespace ZdravoCorp.Repository.Rooms
{
    public class RoomRepository
    {
        public const string RoomsFilePath = "..\\..\\..\\Data\\Rooms\\rooms.csv";
        public static List<Room> Rooms = new();
        public static Serializer<Room> RoomsSerializer = new();

        public RoomRepository()
        {
            Rooms = RoomsSerializer.fromCSV(RoomsFilePath);
        }

        public void Save()
        {
            RoomsSerializer.toCSV(RoomsFilePath, Rooms);
        }

        public static Room GetRoom(string name)
        {
            return Rooms.FirstOrDefault(room => room.Name == name);
        }

        public static List<Room> GetRooms(Room.RoomType roomType)
        {
            return Rooms.FindAll(room => room.Type == roomType);
        }
    }
}
