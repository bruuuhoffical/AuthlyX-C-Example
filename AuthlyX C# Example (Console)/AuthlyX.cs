using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace AuthlyXClient
{
    /// <summary>
    /// Provides authentication and licensing functionality for interacting with the AuthlyX API.
    /// </summary>
    public class auth
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeConsole();

        private static readonly HttpClient client = new HttpClient();
        private readonly string baseUrl = "https://authly.cc/api/v1";
        private string sessionId;
        private string ownerId;
        private string appName;
        private string version;
        private string secret;

        public ResponseStruct response = new ResponseStruct();
        public UserData userData = new UserData();
        public VariableData variableData = new VariableData();

        public class ResponseStruct
        {
            public bool success { get; set; }
            public string message { get; set; }
            public string raw { get; set; }
        }

        public class UserData
        {
            public string Username { get; set; }
            public string Email { get; set; }
            public string LicenseKey { get; set; }
            public string Subscription { get; set; }
            public string ExpiryDate { get; set; }
            public string LastLogin { get; set; }
            public string Hwid { get; set; }
            public string IpAddress { get; set; }
            //public string Plan { get; set; }
            //public string Role { get; set; }
            public string RegisteredAt { get; set; }
        }

        public class VariableData
        {
            public string VarKey { get; set; }
            public string VarValue { get; set; }
            public string UpdatedAt { get; set; }
        }

        /// <summary>
        /// Initializes a new instance of the AuthlyX API client with application credentials.
        /// </summary>
        /// <param name="ownerId">Owner ID for the application.</param>
        /// <param name="appName">Name of the application.</param>
        /// <param name="version">Version of the application.</param>
        /// <param name="secret">Application secret key.</param>
        public auth(string ownerId, string appName, string version, string secret)
        {
            this.ownerId = ownerId;
            this.appName = appName;
            this.version = version;
            this.secret = secret;
        }

        private async Task<string> PostJson(string endpoint, object payload)
        {
            try
            {
                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var res = await client.PostAsync($"{baseUrl}/{endpoint}", content);
                var result = await res.Content.ReadAsStringAsync();

                AuthlyLogger.Log($"[{endpoint.ToUpper()}] {result}");

                response.raw = result;
                var obj = JObject.Parse(result);
                response.success = obj["success"]?.ToString()?.ToLower() == "true";
                response.message = obj["message"]?.ToString() ?? "";

                LoadUserData(obj);
                LoadVariableData(obj);

                return result;
            }
            catch (Exception ex)
            {
                response.success = false;
                response.message = $"Exception: {ex.Message}";
                AuthlyLogger.Log($"[ERROR] {ex.Message}");
                return "";
            }
        }

        private void LoadUserData(JObject obj)
        {
            try
            {
                var lic = obj["license"];
                var info = obj["user"] ?? obj["info"];

                if (lic != null)
                {
                    userData.LicenseKey = lic["license_key"]?.ToString();
                    userData.Subscription = lic["subscription"]?.ToString();
                    userData.ExpiryDate = lic["expiry_date"]?.ToString();
                    userData.LastLogin = lic["last_login"]?.ToString();
                    userData.Email = lic["email"]?.ToString();
                }

                if (info != null)
                {
                    userData.Username = info["username"]?.ToString();
                    userData.Email = info["email"]?.ToString();
                    userData.Subscription = info["subscription"]?.ToString();
                    userData.ExpiryDate = info["expiry_date"]?.ToString();
                    userData.LastLogin = info["last_login"]?.ToString();
                    //userData.Plan = info["plan"]?.ToString();
                    //userData.Role = info["role"]?.ToString();
                    userData.RegisteredAt = info["created_at"]?.ToString();
                }

                userData.Hwid = GetSystemSid();
                userData.IpAddress = GetLocalIp();
            }
            catch (Exception ex)
            {
                AuthlyLogger.Log($"[PARSE_ERROR] Failed to parse user/license data: {ex.Message}");
            }
        }

        private void LoadVariableData(JObject obj)
        {
            try
            {
                var variable = obj["variable"];
                if (variable != null)
                {
                    variableData.VarKey = variable["var_key"]?.ToString();
                    variableData.VarValue = variable["var_value"]?.ToString();
                    variableData.UpdatedAt = variable["updated_at"]?.ToString();
                }
            }
            catch (Exception ex)
            {
                AuthlyLogger.Log($"[PARSE_ERROR] Failed to parse variable data: {ex.Message}");
            }
        }

        /// <summary>
        /// Initializes the connection with the AuthlyX API using the provided credentials.
        /// </summary>
        public async Task Init()
        {
            AuthlyLogger.AppName = appName;

            var result = await PostJson("init", new
            {
                owner_id = ownerId,
                app_name = appName,
                version = version,
                secret = secret
            });

            try
            {
                var obj = JObject.Parse(result);
                if (obj["session_id"] != null)
                {
                    sessionId = obj["session_id"].ToString();
                }
                else if (!response.success)
                {
                    // Allocate console for error display
                    AllocConsole();
                    Console.WriteLine($"Initialization Error: {response.message}");
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey();
                    FreeConsole();
                    Environment.Exit(1);
                }
            }
            catch (Exception ex)
            {
                // Allocate console for exception display
                AllocConsole();
                Console.WriteLine($"Initialization Exception: {ex.Message}");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                FreeConsole();
                AuthlyLogger.Log($"[INIT_ERROR] {ex.Message}");
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Authenticates a user using their username and password.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <param name="password">The password of the user.</param>
        public async Task Login(string username, string password)
        {
            string sid = GetSystemSid();
            string ip = GetLocalIp();

            await PostJson("login", new
            {
                session_id = sessionId,
                username = username,
                password = password,
                hwid = sid,
                ip = ip
            });
        }

        /// <summary>
        /// Registers a new user with the provided credentials and license key.
        /// </summary>
        /// <param name="username">The username for the new user.</param>
        /// <param name="password">The password for the new user.</param>
        /// <param name="key">The license key for registration.</param>
        /// <param name="email">The email address of the user (optional).</param>
        public async Task Register(string username, string password, string key, string email = null)
        {
            string sid = GetSystemSid();

            await PostJson("register", new
            {
                session_id = sessionId,
                username = username,
                password = password,
                key = key,
                email = email,
                hwid = sid
            });
        }

        /// <summary>
        /// Authenticates a user using a license key.
        /// </summary>
        /// <param name="licenseKey">The license key for authentication.</param>
        public async Task LicenseLogin(string licenseKey)
        {
            string sid = GetSystemSid();
            string ip = GetLocalIp();

            await PostJson("licenses", new
            {
                session_id = sessionId,
                license_key = licenseKey,
                hwid = sid,
                ip = ip
            });
        }

        /// <summary>
        /// Retrieves a cloud variable by its key and returns its value.
        /// </summary>
        /// <param name="varKey">The key of the variable to retrieve.</param>
        /// <returns>The value of the variable, or null if not found.</returns>
        public async Task<string> GetVariable(string varKey)
        {
            await PostJson("variables", new
            {
                session_id = sessionId,
                var_key = varKey
            });

            return variableData.VarValue;
        }

        /// <summary>
        /// Sets a cloud variable value. Creates the variable if it doesn't exist.
        /// </summary>
        /// <param name="varKey">The key of the variable to set.</param>
        /// <param name="varValue">The value to set for the variable.</param>
        public async Task SetVariable(string varKey, string varValue)
        {
            await PostJson("variables/set", new
            {
                session_id = sessionId,
                var_key = varKey,
                var_value = varValue
            });
        }

        private string GetSystemSid()
        {
            try
            {
                string sid = WindowsIdentity.GetCurrent()?.User?.Value;
                return !string.IsNullOrEmpty(sid) ? sid : "UNKNOWN_SID";
            }
            catch { return "UNKNOWN_SID"; }
        }

        private string GetLocalIp()
        {
            try
            {
                string hostName = Dns.GetHostName();
                var addresses = Dns.GetHostAddresses(hostName);
                foreach (var addr in addresses)
                {
                    if (addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        return addr.ToString();
                }
            }
            catch { }
            return "UNKNOWN_IP";
        }
    }

    public static class AuthlyLogger
    {
        public static bool Enabled = true;
        public static string AppName = "AuthlyX";

        public static void Log(string content)
        {
            if (!Enabled) return;

            try
            {
                string baseDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    "AuthlyX", "logs", AppName);

                Directory.CreateDirectory(baseDir);

                string logFile = Path.Combine(baseDir, $"{DateTime.Now:MMM_dd_yyyy}_log.txt");

                string redacted = Redact(content);
                File.AppendAllText(logFile, $"[{DateTime.Now:HH:mm:ss}] {redacted}{Environment.NewLine}");
            }
            catch { }
        }

        private static string Redact(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            string[] fields = { "session_id", "owner_id", "secret", "password", "key", "license_key" };
            foreach (var f in fields)
            {
                text = System.Text.RegularExpressions.Regex.Replace(
                    text,
                    $"\"{f}\":\\s*\"[^\"]*\"",
                    $"\"{f}\":\"REDACTED\"",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase
                );
            }
            return text;
        }
    }
}