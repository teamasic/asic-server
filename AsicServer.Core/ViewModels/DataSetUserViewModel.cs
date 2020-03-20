using System;
using System.Collections.Generic;
using System.Text;

namespace AsicServer.Core.ViewModels
{
    public class DataSetUserViewModel
    {
        public int Id { get; set; }
        public int DataSetId { get; set; }
        public int UserId { get; set; }
        public UserViewModel User { get; set; }
    }
}
