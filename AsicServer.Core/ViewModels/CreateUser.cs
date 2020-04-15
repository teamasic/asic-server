using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsicServer.Core.ViewModels
{
    public class CreateUser
    {
        [Name("email")]
        public string Email { get; set; }
        [Name("fullname")]
        public string Fullname { get; set; }
        [Name("code")]
        public string Code { get; set; }
        [Name("image")]
        public string Image { get; set; }
    }
}
