using System;
using System.Collections.Generic;

namespace AsicServer.Core.Entities
{
    public partial class DataSetUser
    {
        public int Id { get; set; }
        public int? DataSetId { get; set; }
        public long? UserId { get; set; }

        public virtual DataSet DataSet { get; set; }
        public virtual User User { get; set; }
    }
}
