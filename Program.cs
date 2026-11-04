using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommuneClient
{
    internal static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            string serverUrl = ServerConfig.EnsureConfigured();

            if (serverUrl == null)
                return;   // user cancelled the server address form

            Application.Run(new LoginForm(serverUrl));
        }
    }
}
