using System;
using System.Data;
using System.Reflection.Metadata.Ecma335;
using System.Windows.Automation;
using ZdravoCorp.Repository.Serializer;
using static ZdravoCorp.Domain.Users.User;

namespace ZdravoCorp.Domain.Equipment;

public class Equipment : Serializable
{
    public enum EquipmentType
    {
        Operation,
        Examination,
        Furniture,
        Hallway
    };

    public string Id { get; set; }
    public string Name { get; set; } 
    public EquipmentType Type { get; set; }
    public bool IsDynamic { get; set; }

    public Equipment()
    {
        Id = "";
        Name = "";
        Type = EquipmentType.Operation;
    }
    public Equipment(string id, string name, EquipmentType type, bool isDynamic)
    {
        Id = id;
        Name = name;
        Type = type;
        IsDynamic = isDynamic;
    }

    public Equipment(string id,string name, string type, bool isDynamic)
    {
        Id = id;
        Name = name;
        Type = (EquipmentType)Enum.Parse(typeof(EquipmentType), type);
        IsDynamic = isDynamic;
    }

    public string[] ToCSV()
    {
        string[] csvValues =
        {
            Id,
            Name,
            Type.ToString(),
            IsDynamic.ToString()
        };
        return csvValues;
    }

    public void FromCSV(string[] values)
    {
        Id = values[0];
        Name = values[1];
        Type = (EquipmentType)Enum.Parse(typeof(EquipmentType), values[2]);
        IsDynamic = Boolean.Parse(values[3]);
    }

    public bool Contains(string search)
    {
        var isInId = Id.Contains(search);
        var isInName =Name.Contains(search);
        var isInType = Type.ToString().Contains(search);
        var isDynamic = IsDynamic.ToString().Contains(search);
        return isInId || isInName || isInType || isDynamic;
    }
}