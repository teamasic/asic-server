using System;
using System.Collections.Generic;

namespace AsicServer.Core.Entities
{
    public partial class Records
    {
        public Records()
        {
            ChangeRequests = new HashSet<ChangeRequests>();
        }

        public long Id { get; set; }
        public long? AttendeeId { get; set; }
        public string AttendeeCode { get; set; }
        public int? SessionId { get; set; }
        public string SessionName { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool Present { get; set; }

        public virtual User Attendee { get; set; }
        public virtual Sessions Session { get; set; }
        public virtual ICollection<ChangeRequests> ChangeRequests { get; set; }
    }
}
