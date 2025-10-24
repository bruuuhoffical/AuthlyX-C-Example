using AuthlyXClient;
using System;
using System.Text;
using System.Threading.Tasks;

namespace AuthlyX_CSharp_Example__Console_
{
    internal class Program
    {
        public static auth AuthlyXApp = new auth(
            ownerId: "469e4d9235d1",
            appName: "BASIC",
            version: "1.0.0",
            secret: "iqcmyagw1skGdgk6Nq7OxxpX5iAmC2Hlwq7iNwyG"
        );

        static async Task Main(string[] args)
        {
            Console.Title = "AuthlyX Client Tester";
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔══════════════════════════════════════╗");
            Console.WriteLine("║           AUTHLYX C# EXAMPLE         ║");
            Console.WriteLine("║             Console Test             ║");
            Console.WriteLine("╚══════════════════════════════════════╝");
            Console.ResetColor();

            Console.WriteLine("\n Starting...");
            //Console.WriteLine("\nInitializing AuthlyX Connection...");

            await AuthlyXApp.Init();

            if (!AuthlyXApp.response.success)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Couldn't Start: {AuthlyXApp.response.message}");
                Console.ResetColor();
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Connection Established Successfully!");
            Console.ResetColor();

            bool running = true;
            while (running)
            {
                ShowMainMenu();
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await TestLogin();
                        break;
                    case "2":
                        await TestRegister();
                        break;
                    case "3":
                        await TestLicenseLogin();
                        break;
                    case "4":
                        await TestVariables();
                        break;
                    case "5":
                        TestUserInfo();
                        break;
                    case "6":
                        await TestAllFeatures();
                        break;
                    case "0":
                        running = false;
                        Console.WriteLine("Thank you for using AuthlyX!");
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Invalid choice. Please try again.");
                        Console.ResetColor();
                        break;
                }

                if (running && choice != "0")
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
        }

        static void ShowMainMenu()
        {
            Console.WriteLine("\n" + new string('═', 50));
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("MAIN MENU - Choose an option:");
            Console.ResetColor();
            Console.WriteLine("1. Login (Username + Password)");
            Console.WriteLine("2. Register New Account");
            Console.WriteLine("3. License Login (License Key Only)");
            Console.WriteLine("4. Variable Operations");
            Console.WriteLine("5. View User Information");
            Console.WriteLine("6. Test All Features");
            Console.WriteLine("0. Exit");
            Console.WriteLine(new string('═', 50));
            Console.Write("Your choice: ");
        }

        static async Task TestLogin()
        {
            Console.WriteLine("\n" + new string('─', 40));
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("LOGIN TEST");
            Console.ResetColor();

            Console.Write("Enter Username: ");
            string username = Console.ReadLine();

            Console.Write("Enter Password: ");
            string password = ReadPassword();

            Console.WriteLine("\nAuthenticating...");
            await AuthlyXApp.Login(username, password);

            DisplayResult("Login", AuthlyXApp.response);
        }

        static async Task TestRegister()
        {
            Console.WriteLine("\n" + new string('─', 40));
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("REGISTRATION TEST");
            Console.ResetColor();

            Console.Write("Enter Username: ");
            string username = Console.ReadLine();

            Console.Write("Enter Password: ");
            string password = ReadPassword();

            Console.Write("Enter License Key: ");
            string licenseKey = Console.ReadLine();

            Console.Write("Enter Email (optional): ");
            string email = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(email))
                email = null;

            Console.WriteLine("\nRegistering account...");
            await AuthlyXApp.Register(username, password, licenseKey, email);

            DisplayResult("Registration", AuthlyXApp.response);
        }

        static async Task TestLicenseLogin()
        {
            Console.WriteLine("\n" + new string('─', 40));
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("LICENSE LOGIN TEST");
            Console.ResetColor();

            Console.Write("Enter License Key: ");
            string licenseKey = Console.ReadLine();

            Console.WriteLine("\nAuthenticating with license...");
            await AuthlyXApp.LicenseLogin(licenseKey);

            DisplayResult("License Login", AuthlyXApp.response);
        }

        static async Task TestVariables()
        {
            Console.WriteLine("\n" + new string('─', 40));
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("VARIABLE OPERATIONS");
            Console.ResetColor();

            Console.WriteLine("1. Get Variable");
            Console.WriteLine("2. Set Variable");
            Console.Write("Choose operation: ");

            var choice = Console.ReadLine();

            if (choice == "1")
            {
                Console.Write("Enter Variable Key: ");
                string varKey = Console.ReadLine();

                Console.WriteLine("\nFetching variable...");
                string value = await AuthlyXApp.GetVariable(varKey);

                if (AuthlyXApp.response.success)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Variable '{varKey}': {value}");
                    Console.ResetColor();
                }
                else
                {
                    DisplayResult("Get Variable", AuthlyXApp.response);
                }
            }
            else if (choice == "2")
            {
                Console.Write("Enter Variable Key: ");
                string varKey = Console.ReadLine();

                Console.Write("Enter Variable Value: ");
                string varValue = Console.ReadLine();

                Console.WriteLine("\nSetting variable...");
                await AuthlyXApp.SetVariable(varKey, varValue);

                DisplayResult("Set Variable", AuthlyXApp.response);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid choice for variable operation.");
                Console.ResetColor();
            }
        }

        static void TestUserInfo()
        {
            Console.WriteLine("\n" + new string('─', 40));
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("USER INFORMATION");
            Console.ResetColor();

            if (string.IsNullOrEmpty(AuthlyXApp.userData.Username) &&
                string.IsNullOrEmpty(AuthlyXApp.userData.LicenseKey))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("No active session. Please login first.");
                Console.ResetColor();
                return;
            }

            DisplayUserInfo();
        }

        static async Task TestAllFeatures()
        {
            Console.WriteLine("\n" + new string('─', 40));
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("COMPREHENSIVE FEATURE TEST");
            Console.ResetColor();

            Console.WriteLine("\n1. Testing Login...");
            Console.Write("Enter test username: ");
            string user = Console.ReadLine();
            Console.Write("Enter test password: ");
            string pass = ReadPassword();

            await AuthlyXApp.Login(user, pass);
            DisplayResult("Login", AuthlyXApp.response);

            if (AuthlyXApp.response.success)
            {
                Console.WriteLine("\n2. Testing Variable Operations...");
                string testKey = "test_variable";
                string testValue = $"test_value_{DateTime.Now:HHmmss}";

                await AuthlyXApp.SetVariable(testKey, testValue);
                DisplayResult("Set Variable", AuthlyXApp.response);

                if (AuthlyXApp.response.success)
                {
                    await AuthlyXApp.GetVariable(testKey);
                    DisplayResult("Get Variable", AuthlyXApp.response);
                }

                Console.WriteLine("\n3. Final User Information:");
                DisplayUserInfo();
            }
        }

        static void DisplayResult(string operation, auth.ResponseStruct response)
        {
            Console.WriteLine("\n" + new string('─', 30));
            if (response.success)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{operation} SUCCESS");
                Console.ResetColor();
                Console.WriteLine($"Message: {response.message}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{operation} FAILED");
                Console.ResetColor();
                Console.WriteLine($"Message: {response.message}");
            }
            Console.WriteLine(new string('─', 30));
        }

        static void DisplayUserInfo()
        {
            var user = AuthlyXApp.userData;

            Console.WriteLine("\n" + new string('═', 50));
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("USER PROFILE");
            Console.ResetColor();
            Console.WriteLine(new string('═', 50));

            Console.WriteLine($"Username: {user.Username ?? "N/A"}");
            Console.WriteLine($"Email: {user.Email ?? "N/A"}");
            Console.WriteLine($"License Key: {user.LicenseKey ?? "N/A"}");
            Console.WriteLine($"Subscription: {user.Subscription ?? "N/A"}");
            //Console.WriteLine($"Plan: {user.Plan ?? "N/A"}");
            //Console.WriteLine($"Role: {user.Role ?? "N/A"}");
            Console.WriteLine($"Expiry Date: {user.ExpiryDate ?? "N/A"}");
            Console.WriteLine($"Last Login: {user.LastLogin ?? "N/A"}");
            Console.WriteLine($"Registered: {user.RegisteredAt ?? "N/A"}");
            Console.WriteLine($"HWID: {user.Hwid ?? "N/A"}");
            Console.WriteLine($"IP Address: {user.IpAddress ?? "N/A"}");
            Console.WriteLine(new string('═', 50));
        }

        static string ReadPassword()
        {
            var password = new StringBuilder();
            while (true)
            {
                var key = Console.ReadKey(intercept: true);
                if (key.Key == ConsoleKey.Enter)
                    break;
                if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password.Remove(password.Length - 1, 1);
                    Console.Write("\b \b");
                }
                else if (key.Key != ConsoleKey.Backspace)
                {
                    password.Append(key.KeyChar);
                    Console.Write("*");
                }
            }
            Console.WriteLine();
            return password.ToString();
        }
    }
}