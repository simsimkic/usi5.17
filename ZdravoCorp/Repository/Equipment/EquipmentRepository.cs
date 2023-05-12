using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Domain.Equipment;
using ZdravoCorp.Domain;
using ZdravoCorp.Repository.Serializer;

namespace ZdravoCorp.Repository.Equipment
{
    public class EquipmentRepository
    {
        public const string EquipmentFilePath = "..\\..\\..\\Data\\Equipment\\equipment.csv";
        public static List<Domain.Equipment.Equipment> Equipment = new();
        public static Serializer<Domain.Equipment.Equipment> EquipmentSerializer = new();

        public EquipmentRepository()
        {
            Equipment = EquipmentSerializer.fromCSV(EquipmentFilePath);
        }

        public void Save()
        {
            EquipmentSerializer.toCSV(EquipmentFilePath, Equipment);
        }

        public static Domain.Equipment.Equipment GetEquipment(string id)
        {
            return Equipment.FirstOrDefault(item => item.Id == id);
        }
    }
}
