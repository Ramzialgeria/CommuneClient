using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommuneClient
{
    /// <summary>
    /// Stores and validates the CommuneServer address typed by the user
    /// (first launch / installation). The address is kept in
    /// %LocalAppData%\CommuneClient\conf.ini, which is always writable,
    /// even when the application is installed in Program Files.
    /// </summary>
    public static class ServerConfig
    {
        public const int DefaultPort = 5050;

        private static string ConfigPath
        {
            get
            {
                //string dir = Path.Combine(
                //    Environment.GetFolderPath(
                //        Environment.SpecialFolder.LocalApplicationData),
                //    "CommuneClient");

                //Directory.CreateDirectory(dir);
                return Path.Combine(Application.StartupPath, "conf.ini");
            }
        }

        /// <summary>Returns "host" or "host:port", or null when not configured.</summary>
        public static string Load()
        {
            

            try
            {
                if (File.Exists(ConfigPath))
                {
                    string v = File.ReadAllText(ConfigPath).Trim();
                    if (v.Length > 0) return v;
                }

                // Old location (next to the exe) - migrate it once.
                string legacy = Path.Combine(Application.StartupPath, "conf.ini");
                if (File.Exists(legacy))
                {
                    string v = File.ReadAllText(legacy).Trim();
                    if (v.Length > 0)
                    {
                        Save(v);
                        return v;
                    }
                }
            }
            catch { }

            return null;
        }

        public static void Save(string address)
        {
            File.WriteAllText(ConfigPath, address);
        }

        /// <summary>
        /// Accepts "192.168.1.10", "PC-SERVER", "192.168.1.10:5050" or
        /// "http://192.168.1.10:5050/" and returns "host" / "host:port".
        /// Returns null if the text is not usable.
        /// </summary>
        public static string Normalize(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;

            string s = input.Trim();

            if (s.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
                s = s.Substring(7);
            else if (s.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                s = s.Substring(8);

            s = s.TrimEnd('/');

            if (s.Length == 0 || s.Contains("/") || s.Contains(" "))
                return null;

            Uri uri;
            if (!Uri.TryCreate("http://" + s + "/", UriKind.Absolute, out uri))
                return null;

            return s;
        }

        public static string BuildUrl(string address)
        {
            string s = address.Trim();
            // no explicit port -> default port
            if (s.LastIndexOf(':') < 0)
                s += ":" + DefaultPort;
            return "http://" + s + "/";
        }

        /// <summary>Calls GET api/server/ping (anonymous) on the CommuneServer.</summary>
        public static async Task<bool> PingAsync(string address)
        {
            try
            {
                using (var http = new HttpClient())
                {
                    http.Timeout = TimeSpan.FromSeconds(4);
                    var r = await http.GetAsync(BuildUrl(address) + "api/server/ping");
                    return r.IsSuccessStatusCode;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the server URL to use. Asks the user for the address
        /// if none is saved yet. Returns null if the user cancels.
        /// </summary>
        public static string EnsureConfigured()
        {
            string address = Load();

            if (string.IsNullOrWhiteSpace(address))
            {
                address = AskAddress(null);
                if (address == null) return null;
            }

            return BuildUrl(address);
        }

        /// <summary>Shows the address form and saves the result. Null = cancelled.</summary>
        public static string AskAddress(string current)
        {
            using (var form = new IpServerForm())
            {
                form.Prefill = current;

                if (form.ShowDialog() != DialogResult.OK ||
                    string.IsNullOrWhiteSpace(form.IpAdress))
                    return null;

                Save(form.IpAdress);
                return form.IpAdress;
            }
        }
    }
}
