using AsicServer.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsicServer.Core.Models
{
    public class AccessTokenResponse
    {
        public UserViewModel User { get; set; }
        public string[] Roles { get; set; }
        public string AccessToken { get; set; }
    }
}
