using System;
using System.Collections.Generic;

namespace AsicServer.Core.Entities
{
    public partial class Groups
    {
        public Groups()
        {
            AttendeeGroups = new HashSet<AttendeeGroups>();
            Sessions = new HashSet<Sessions>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime? DateTimeCreated { get; set; }
        public bool Deleted { get; set; }
        public int? MaxSessionCount { get; set; }

        public virtual ICollection<AttendeeGroups> AttendeeGroups { get; set; }
        public virtual ICollection<Sessions> Sessions { get; set; }
    }
}
