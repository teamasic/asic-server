using System;
using System.Collections.Generic;

namespace AsicServer.Core.Entities
{
    public partial class AttendeeGroups
    {
        public long AttendeeId { get; set; }
        public int GroupId { get; set; }

        public virtual User Attendee { get; set; }
        public virtual Groups Group { get; set; }
    }
}
