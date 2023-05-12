using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Domain.Common;
using ZdravoCorp.Domain.Users;
using ZdravoCorp.Repository.Serializer;

namespace ZdravoCorp.Repository
{
    internal class UserRepository
    {
        private const string UsersFilePath = "..\\..\\..\\Data\\users.csv";
        public static List<User> Users = new();
        public static Serializer<User> UserSerializer = new();

        public UserRepository()
        {
            Users = UserSerializer.fromCSV(UsersFilePath);
        }

        public static void SaveRepository()
        {
            UserSerializer.toCSV(UsersFilePath, Users);
        }

        public static User? GetUser(string username)
        {
            return Users.FirstOrDefault(user => user.Username == username);
        }

        public static bool DoesUsernameAlreadyExist(string username)
        {
            return Users.Any(user => user.Username == username);
        }
        public static bool IsUsernameValidForModification(string oldUsername, string newUsername)
        {
            return Users.All(user => newUsername != user.Username || oldUsername == user.Username);
        }

        public static void EditUser(string oldUsername, string newUsername, string password, User.UserRole role)
        {
            User? user = GetUser(oldUsername);
            user.Username = newUsername;
            user.Password = password;
            user.Role = role;
            SaveRepository();
        }

        public static void DeleteUser(string username)
        {
            User user = GetUser(username)!;
            Users.Remove(user);
            SaveRepository();
        }
    }
}
