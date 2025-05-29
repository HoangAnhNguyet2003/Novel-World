using AutoMapper;
using NovelWorld.Dtos.Auth.UserDtos;
using NovelWorld.Entities.Auth;

namespace NovelWorld.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
        }
    }
}
