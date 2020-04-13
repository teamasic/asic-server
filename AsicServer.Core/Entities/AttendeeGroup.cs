using System;
using System.Collections.Generic;

namespace AsicServer.Core.Entities
{
    public partial class AttendeeGroup
    {
        public AttendeeGroup()
        {
            Record = new HashSet<Record>();
        }

        public int Id { get; set; }
        public string AttendeeCode { get; set; }
        public string GroupCode { get; set; }
        public bool? IsActive { get; set; }

        public virtual User AttendeeCodeNavigation { get; set; }
        public virtual Group GroupCodeNavigation { get; set; }
        public virtual ICollection<Record> Record { get; set; }
    }
}
