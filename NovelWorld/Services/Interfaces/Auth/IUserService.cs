using NovelWorld.Dtos.Auth.UserDtos;

namespace NovelWorld.Services.Interfaces.Auth
{
    public interface IUserService
    {
        Task CreateUserAsync(CreateUserDto input);
        Task DeleteUserAsync();
        Task<UserDto> GetUserAsync();
        Task UpdateUserAsync(UpdateUserDto input);
    }

}
