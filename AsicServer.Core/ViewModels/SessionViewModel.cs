using AsicServer.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AsicServer.Core.ViewModels
{
    public class SessionViewModel : BaseViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int GroupId { get; set; }
        public RecordViewModel Record { get; set; }
        [JsonIgnore]
        public GroupViewModel Group { get; set; }
    }

    public class SessionInSyncData : BaseViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string RtspString { get; set; }
        public string RoomName { get; set; }
        public int GroupId { get; set; }
        
    }
}
