using System;
using System.Collections.Generic;

namespace AsicServer.Core.Entities
{
    public partial class ChangeRequests
    {
        public int Id { get; set; }
        public long? RecordId { get; set; }
        public string Comment { get; set; }
        public int Status { get; set; }

        public virtual Records Record { get; set; }
    }
}
