using AsicServer.Core.Infrastructure;
using AsicServer.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AsicServer.Core.ViewModels
{
    public class RecordViewModel : BaseViewModel
    {
        public int Id { get; set; }
        public AttendeeViewModel Attendee { get; set; }
        public SessionViewModel Session { get; set; }
        public bool Present { get; set; }
    }

    public class RecordInSyncData : BaseViewModel
    {
        public int Id { get; set; }
        [Required]
        public AttendeeViewModel Attendee { get; set; }
        [Required]
        public SessionInSyncData Session { get; set; }
        [Required]
        public GroupInSyncData Group { get; set; }
        [Required]
        public bool Present { get; set; }
        public bool IsEnrollInClass { get; set; } = true;
    }

    public class RecordStagingViewModel
    {
        //attendee
        public string AttendeeName { get; set; }
        public string AttendeeCode { get; set; }
        //session
        public string SessionName { get; set; }
        public DateTime SessionStartTime { get; set; }
        public DateTime SessionEndTime { get; set; }
        //group

        public string GroupCode { get; set; }
        public string GroupName { get; set; }
        public int MaxSessionCount { get; set; }
        public DateTime GroupCreateTime { get; set; }
        //record
        public bool Present { get; set; }
    }
}
