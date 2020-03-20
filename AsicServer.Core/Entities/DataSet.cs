using System;
using System.Collections.Generic;

namespace AsicServer.Core.Entities
{
    public partial class DataSet
    {
        public DataSet()
        {
            DataSetUser = new HashSet<DataSetUser>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<DataSetUser> DataSetUser { get; set; }
    }
}
