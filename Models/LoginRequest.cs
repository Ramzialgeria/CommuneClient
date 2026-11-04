using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommuneClient.Models
{
    public class LoginRequest
    {
        public string UserName { get; set; }

        public string Password { get; set; }
    }

    public class LoginResponse
    {
        public string accessToken { get; set; }
        public string userName { get; set; }
        public string role { get; set; }
        public string codeStructure { get; set; }
    }
}
