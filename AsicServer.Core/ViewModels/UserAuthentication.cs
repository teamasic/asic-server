using System;
using System.Collections.Generic;
using System.Text;

namespace AsicServer.Core.ViewModels
{
    public class UserAuthentication
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirebaseToken { get; set; }
    }
}
