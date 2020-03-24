using System;
using System.Collections.Generic;
using System.Text;

namespace AsicServer.Core.Models
{
    public class CreateUser
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Fullname { get; set; }
        public string RollNumber { get; set; }
    }
}
