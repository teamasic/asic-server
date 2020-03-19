using System;
using System.Collections.Generic;

namespace AsicServer.Core.Entities
{
    public partial class Attendee
    {
        public Attendee()
        {
            AttendeeDataSet = new HashSet<AttendeeDataSet>();
        }

        public int Id { get; set; }
        public string Code { get; set; }

        public virtual ICollection<AttendeeDataSet> AttendeeDataSet { get; set; }
    }
}
