using System;
using System.Collections.Generic;

namespace AsicServer.Core.Entities
{
    public partial class Session
    {
        public Session()
        {
            Record = new HashSet<Record>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Status { get; set; }
        public string GroupCode { get; set; }
        public int RoomId { get; set; }

        public virtual Group GroupCodeNavigation { get; set; }
        public virtual Room Room { get; set; }
        public virtual ICollection<Record> Record { get; set; }
    }
}
