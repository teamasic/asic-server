using AsicServer.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataService.Repository
{
    public interface IUserRepository : IBaseRepository<User>
    {
        User GetUserByEmail(string username);
        bool AddRangeIfNotInDb(List<User> users);
        Task<User> AddIfNotInDb(User user);
        bool IsExisted(string username);
        User GetByEmail(string email);
        List<User> GetByCodes(List<string> codes);
    }

    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public async Task<User> AddIfNotInDb(User user)
        {
            var userInDb = GetUserByEmail(user.Email);
            if(userInDb == null)
            {
                userInDb = user;
                await AddAsync(userInDb);
            }
            return userInDb;
        }

        public bool AddRangeIfNotInDb(List<User> users)
        {
            try
            {
                foreach (var user in users)
                {
                    var userInDb = GetByAttendeeCode(user.Code);
                    if(userInDb == null)
                    {
                        userInDb = GetByEmail(user.Email);
                        if(userInDb == null)
                        {
                            dbContext.Add(user);
                        }
                    }
                }
            } catch (Exception e)
            {
                return false;
            }
            dbContext.SaveChanges();
            return true;
        }

        public User GetByEmail(string email)
        {
            return Get(u => u.Email == email).FirstOrDefault();
        }

        public User GetByAttendeeCode(string code)
        {
            var user = Get(filter: acc => string.Equals(acc.Code, code),
                orderBy: null).FirstOrDefault();
            return user;
        }

        public User GetUserByEmail(string email)
        {
            var user = this.Get(filter: acc => string.Equals(acc.Email, email), 
                orderBy: null, "Role").FirstOrDefault();
            return user;
        }

        public bool IsExisted(string email)
        {
            var user = this.Get(acc => string.Equals(acc.Email, email)).FirstOrDefault();
            return user != null;
        }

        public List<User> GetByCodes(List<string> codes)
        {
            return Get(u => codes.Contains(u.Code)).ToList();
        }
    }
}
