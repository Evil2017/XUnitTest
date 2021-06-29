using XUnitTest.Models;

namespace XUnitTest.Interfaces
{
    public interface IUserRepository
    {
        bool Add(UserEntity user);
        int Count();
        UserEntity GetByName(string name);
    }
}
