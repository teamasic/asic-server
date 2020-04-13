using System;
using System.Collections.Generic;

namespace AsicServer.Core.Entities
{
    public partial class User
    {
        public User()
        {
            AttendeeGroups = new HashSet<AttendeeGroup>();
        }

        public string Code { get; set; }
        public string Email { get; set; }
        public string Fullname { get; set; }
        public string Image { get; set; }
        public int RoleId { get; set; }

        public virtual Role Role { get; set; }
        public virtual ICollection<AttendeeGroup> AttendeeGroups { get; set; }
    }
}
