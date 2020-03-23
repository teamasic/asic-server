using System;
using System.Collections.Generic;

namespace AsicServer.Core.Entities
{
    public partial class Sessions
    {
        public Sessions()
        {
            Records = new HashSet<Records>();
        }

        public int Id { get; set; }
        public int? GroupId { get; set; }
        public string Name { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string RoomName { get; set; }
        public string RtspString { get; set; }

        public virtual Groups Group { get; set; }
        public virtual ICollection<Records> Records { get; set; }
    }
}
