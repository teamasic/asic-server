using System;
using System.Collections.Generic;

namespace AsicServer.Core.Entities
{
    public partial class AttendeeGroup
    {
        public AttendeeGroup()
        {
            Records = new HashSet<Record>();
        }

        public int Id { get; set; }
        public string AttendeeCode { get; set; }
        public string GroupCode { get; set; }
        public bool? IsActive { get; set; }

        public virtual User Attendee { get; set; }
        public virtual Group Group { get; set; }
        public virtual ICollection<Record> Records { get; set; }
    }
}
