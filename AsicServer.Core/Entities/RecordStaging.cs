using System;
using System.Collections.Generic;

namespace AsicServer.Core.Entities
{
    public partial class RecordStaging
    {
        public long Id { get; set; }
        public string AttendeeCode { get; set; }
        public string AttendeeName { get; set; }
        public string SessionName { get; set; }
        public DateTime? SessionStartTime { get; set; }
        public DateTime? SessionEndTime { get; set; }
        public string RtspString { get; set; }
        public string RoomName { get; set; }
        public string GroupCode { get; set; }
        public string GroupName { get; set; }
        public DateTime? GroupCreateTime { get; set; }
        public int? MaxSessionCount { get; set; }
        public bool? Present { get; set; }
        public bool? IsEnrollInClass { get; set; }
    }
}
