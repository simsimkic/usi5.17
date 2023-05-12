using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using ZdravoCorp.Domain.Calendar;
using ZdravoCorp.Domain.Room;
using ZdravoCorp.Repository.Rooms;
using static ZdravoCorp.Domain.Room.Room;

namespace ZdravoCorp.Services.Rooms
{
    public static class RoomService
    {
        public static Room GetRoom(string roomName)
        {
            return RoomRepository.GetRoom(roomName);
        }

        public static Room GetFreeRoom(RoomType roomType, TimeSlot timeslot)
        {
            List<Room> rooms = RoomRepository.GetRooms(roomType);
            List<Appointment> overlappingAppointments = AppointmentService.GetActiveOverlappingAppointments(timeslot);
            foreach (Room room in rooms)
            {
                bool isFree = overlappingAppointments.All(appointment => appointment.RoomName != room.Name);

                if (isFree)
                {
                    return room;
                }
            }

            throw new InvalidOperationException("There is no free room for given time.");
        }

        public static List<Room> GetAllOtherRooms(string roomName)
        {
            return RoomRepository.Rooms.FindAll(item => item.Name != roomName);
        }

        public static Room GetFreeRoom(RoomType roomType, TimeSlot timeslot, int ignoreAppointmentId)
        {
            List<Room> rooms = RoomRepository.GetRooms(roomType);
            List<Appointment> overlappingAppointments = AppointmentService.GetActiveOverlappingAppointments(timeslot);
            foreach (Room room in rooms)
            {
                bool isFree = true;
                foreach (var appointment in overlappingAppointments)
                {
                    if (appointment.RoomName == room.Name && appointment.Id != ignoreAppointmentId)
                    {
                        isFree = false;
                        break;
                    }
                }
                if (isFree)
                {
                    return room;
                }
            }
            throw new InvalidOperationException("There is no free room for given time.");
        }
    }
}
