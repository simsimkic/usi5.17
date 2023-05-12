using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace ZdravoCorp.Services
{
    public static class RandomStringGenerator
    {
        private const string allowedCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private static Random random = new Random();
        public static string GenerateRandomString(int length)
        {
            return new string(Enumerable.Repeat(allowedCharacters, length)
                .Select(s => s[random.Next(s.Length)])
                .ToArray());
        }
    }
}
