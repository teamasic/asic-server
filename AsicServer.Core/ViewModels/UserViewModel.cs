using AsicServer.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsicServer.Core.ViewModels
{
    public class UserViewModel : BaseViewModel
    {
        public string Code { get; set; }
        public string Email { get; set; }
        public string Fullname { get; set; }
        public string Image { get; set; }
    }
}
