using System;
using System.Collections.Generic;

namespace AsicServer.Core.Entities
{
    public partial class Rooms
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string RtspString { get; set; }
    }
}
