using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ZdravoCorp.Repository.Serializer;

namespace ZdravoCorp.Domain.Users
{
    public class User : Serializable
    {
        public enum UserRole
        {
            Patient, Doctor, Nurse, Manager
        }
        public string Username { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; }

        public User() { }
        public User(string username, string password, UserRole role)
        {
            Username = username;
            Password = password;
            Role = role;
        }

        public string[] ToCSV()
        {
            string[] csvValues =
            {
                Username,
                Password,
                Role.ToString()
            };
            return csvValues;
        }

        public void FromCSV(string[] values)
        {
            Username = values[0];
            Password = values[1];
            Role = (UserRole)Enum.Parse(typeof(UserRole), values[2]);
        }
    }
}
