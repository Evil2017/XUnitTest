using AutoMapper;
using XUnitTest.Models;

namespace XUnitTest.Mvc.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserEntity, UserDto>().ReverseMap().ForAllMembers(memberOptions =>
            {
                memberOptions.DoNotAllowNull();
            });
            CreateMap<UserEntity, UserVm>().ReverseMap().ForAllMembers(memberOptions =>
            {
                memberOptions.DoNotAllowNull();
            });
        }
    }
}
