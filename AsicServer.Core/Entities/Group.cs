using System;
using System.Collections.Generic;

namespace AsicServer.Core.Entities
{
    public partial class Group
    {
        public Group()
        {
            AttendeeGroups = new HashSet<AttendeeGroup>();
            Sessions = new HashSet<Session>();
        }

        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime? DateTimeCreated { get; set; }
        public int? TotalSession { get; set; }
        public bool Deleted { get; set; }

        public virtual ICollection<AttendeeGroup> AttendeeGroups { get; set; }
        public virtual ICollection<Session> Sessions { get; set; }
    }
}
