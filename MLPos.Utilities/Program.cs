using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Identity;
using MLPos.Core.Model;

namespace MLPos.Utilities
{
    public class Program
    {
        private static void CreateUser()
        {
            Console.Write("Username: ");
            string username = Console.ReadLine();
            Console.Write("Password: ");
            var pass = string.Empty;

            ConsoleKey key;
            do
            {
                var keyInfo = Console.ReadKey(intercept: true);
                key = keyInfo.Key;

                if (key == ConsoleKey.Backspace && pass.Length > 0)
                {
                    Console.Write("\b \b");
                    pass = pass[0..^1];
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    Console.Write("*");
                    pass += keyInfo.KeyChar;
                }
            } while (key != ConsoleKey.Enter);

            Console.WriteLine();

            var hasher = new PasswordHasher<User>();
            string hashed = hasher.HashPassword(new User(), pass);

            Console.WriteLine($"Username: {username} Hashed password: {hashed}");
        }

        static void Main(string[] args)
        {
            Console.WriteLine("[0]: CreateUser");
            Console.Write("Select function: ");
            string function = Console.ReadLine();
            switch (function)
            {
                case "0":
                    CreateUser();
                    break;
            }
        }
    }
}
