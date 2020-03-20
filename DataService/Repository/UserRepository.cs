using AsicServer.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataService.Repository
{
    public interface IUserRepository : IBaseRepository<User>
    {
        User GetUserByUsername(string username);

        User GetByRollNumber(string rollNumber);

        bool IsExisted(string username);
    }

    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public User GetByRollNumber(string rollNumber)
        {
            var user = this.Get(filter: acc => string.Equals(acc.RollNumber, rollNumber),
                orderBy: null).FirstOrDefault();
            return user;
        }

        public User GetUserByUsername(string username)
        {
            var user = this.Get(filter: acc => string.Equals(acc.Username, username), 
                orderBy: null, "UserRole.Role").FirstOrDefault();
            return user;
        }

        public bool IsExisted(string username)
        {
            var user = this.Get(acc => string.Equals(acc.Username, username)).FirstOrDefault();
            return user != null;
        }
    }
}
