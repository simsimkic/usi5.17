using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using ZdravoCorp.Domain;
using ZdravoCorp.Domain.Users;
using ZdravoCorp.Repository;
using static ZdravoCorp.Domain.Users.User;

namespace ZdravoCorp.Services
{
    public static class UserService
    {
        public static void SaveRepository()
        {
            UserRepository.SaveRepository();
        }

        public static User? GetUser(string username)
        {
            return UserRepository.GetUser(username);
        }

        public static bool DoesUsernameAlreadyExist(string username)
        {
            return UserRepository.DoesUsernameAlreadyExist(username);
        }
        public static bool IsUsernameValidForModification(string oldUsername, string newUsername)
        {
            return UserRepository.IsUsernameValidForModification(oldUsername, newUsername);
        }

        public static void EditUser(string oldUsername, string newUsername, string password, User.UserRole role)
        {
            UserRepository.EditUser(oldUsername, newUsername, password, role);
        }

        public static void DeleteUser(string username)
        {
            UserRepository.DeleteUser(username);
        }

        public static void AddUser(User user)
        {
            UserRepository.Users.Add(user);
            UserRepository.SaveRepository();
        }

        public static User? GetLoginUser(string username, string password)
        {
            User? user = GetUser(username);

            if (user == null) return null;

            return user.Password != password ? null : user;
        }
    }


}
