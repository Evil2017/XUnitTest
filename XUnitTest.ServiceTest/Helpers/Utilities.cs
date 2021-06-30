using System.Collections.Generic;
using XUnitTest.Data;
using XUnitTest.Models;

namespace XUnitTest.ServiceTest.Helpers
{
    public static class Utilities
    {
        #region snippet1
        public static void InitializeDbForTests(ApplicationDbContext db)
        {
            db.Users.AddRange(GetSeedingMessages());
            db.SaveChanges();
        }

        public static void ReinitializeDbForTests(ApplicationDbContext db)
        {
            db.Users.RemoveRange(db.Users);
            InitializeDbForTests(db);
        }

        public static List<UserEntity> GetSeedingMessages()
        {
            return new List<UserEntity>()
            {
                new UserEntity(){ Name = "梁爸爸",Age=22 },
                new UserEntity(){ Name = "梁妈妈" ,Age=33},
                new UserEntity(){ Name = "梁吵吵" ,Age=44}
            };
        }
        #endregion
    }
}
