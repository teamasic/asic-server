using System;
using System.Collections.Generic;

namespace AsicServer.Core.Entities
{
    public partial class AttendeeDataSet
    {
        public int Id { get; set; }
        public int AttendeeId { get; set; }
        public int DataSetId { get; set; }

        public virtual Attendee Attendee { get; set; }
        public virtual DataSet DataSet { get; set; }
    }
}
