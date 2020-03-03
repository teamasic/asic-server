using System;
using System.Collections.Generic;

namespace AsicServer.Core.Entities
{
    public partial class UserRole
    {
        public long UserId { get; set; }
        public int RoleId { get; set; }

        public virtual Role Role { get; set; }
        public virtual User User { get; set; }
    }
}
