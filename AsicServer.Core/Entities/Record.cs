using System;
using System.Collections.Generic;

namespace AsicServer.Core.Entities
{
    public partial class Record
    {
        public int Id { get; set; }
        public string AttendeeCode { get; set; }
        public string SessionName { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool Present { get; set; }
        public int SessionId { get; set; }
        public int AttendeeGroupId { get; set; }

        public virtual AttendeeGroup AttendeeGroup { get; set; }
        public virtual Session Session { get; set; }
        public virtual ChangeRequest ChangeRequest { get; set; }
    }
}
