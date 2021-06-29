using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XUnitTest.Data;
using XUnitTest.Interfaces;
using XUnitTest.Models;

namespace XUnitTest.Implement
{
    public class UserRepository : IUserRepository
    {
        private UserDbContext dbContext;
        public UserRepository(UserDbContext db)
        {
            this.dbContext = db;
        }
        public bool Add(UserEntity user)
        {
           // using (var db = new UserDbContext())
            {
                dbContext.Add(user);
                var rows = dbContext.SaveChanges();
                if (rows > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public int Count()
        {
           // using (var db = new UserDbContext())
            {
                return dbContext.Users.Count();
            }
        }

        public UserEntity GetByName(string name)
        {
            //using (var db = new UserDbContext())
            {
                return dbContext.Users.Where(u => u.Name.Equals(name)).FirstOrDefault();
            }
        }
    }
}
