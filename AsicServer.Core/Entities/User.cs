using System;
using System.Collections.Generic;

namespace AsicServer.Core.Entities
{
    public partial class User
    {
        public User()
        {
            AttendeeGroups = new HashSet<AttendeeGroups>();
            Records = new HashSet<Records>();
        }

        public long Id { get; set; }
        public string Username { get; set; }
        public string RollNumber { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string Image { get; set; }
        public int? RoleId { get; set; }

        public virtual Role Role { get; set; }
        public virtual ICollection<AttendeeGroups> AttendeeGroups { get; set; }
        public virtual ICollection<Records> Records { get; set; }
    }
}
