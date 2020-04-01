using System;
using System.Collections.Generic;
using System.Text;

namespace AsicServer.Core.Models
{
    public class CreateUsersResponse: CreateUser
    {
        public int NoImageSaved { get; set; }
    }
}
