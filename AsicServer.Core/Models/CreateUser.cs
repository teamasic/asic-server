using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsicServer.Core.Models
{
    public class CreateUser
    {
        [Name("email")]
        public string Email { get; set; }
        [Name("fullname")]
        public string Fullname { get; set; }
        [Name("rollNumber")]
        public string RollNumber { get; set; }
        [Name("image")]
        public string Image { get; set; }
    }
}
