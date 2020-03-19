using System;
using System.Collections.Generic;

namespace AsicServer.Core.Entities
{
    public partial class DataSet
    {
        public DataSet()
        {
            AttendeeDataSet = new HashSet<AttendeeDataSet>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<AttendeeDataSet> AttendeeDataSet { get; set; }
    }
}
