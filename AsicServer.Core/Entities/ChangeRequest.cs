using System;
using System.Collections.Generic;

namespace AsicServer.Core.Entities
{
    public partial class ChangeRequest
    {
        public int Id { get; set; }
        public int RecordId { get; set; }
        public string Comment { get; set; }
        public int Status { get; set; }

        public virtual Record Record { get; set; }
    }
}
