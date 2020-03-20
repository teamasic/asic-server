using System;
using System.Collections.Generic;
using System.Text;

namespace AsicServer.Core.ViewModels
{
    public class CreateDataSetViewModel
    {
        public string Name { get; set; }
        public List<CreateDataSetUserViewModel> RollNumbers { get; set; }
    }
}
