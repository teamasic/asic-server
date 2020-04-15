using AsicServer.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsicServer.Core.ViewModels
{
    public class GroupViewModel : BaseViewModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int? TotalSession { get; set; }
        public DateTime? DateTimeCreated { get; set; }
        public ICollection<AttendeeViewModel> Attendees { get; set; }
        public ICollection<SessionViewModel> Sessions { get; set; }
    }
    public class GroupInSyncData : BaseViewModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int? TotalSession { get; set; }
        public DateTime? DateTimeCreated { get; set; }
    }
}
