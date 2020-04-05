using System;
using System.Collections.Generic;
using System.Text;

namespace AsicServer.Core.ViewModels
{
    public class ResponsePython
    {
        public bool Success { get; set; }
        public string Results { get; set; }
        public string Errors { get; set; }
    }
}
