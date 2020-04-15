using System;
using System.Collections.Generic;

namespace AsicServer.Core.Entities
{
    public partial class Room
    {
        public Room()
        {
            Session = new HashSet<Session>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string CameraConnectionString { get; set; }

        public virtual ICollection<Session> Session { get; set; }
    }
}
