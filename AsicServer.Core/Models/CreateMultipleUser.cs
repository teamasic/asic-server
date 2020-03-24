using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsicServer.Core.Models
{
    public class CreateMultipleUser
    {
        public List<CreateUser> Users { get; set; }
        public IFormFile ZipFile { get; set; }
    }
}
