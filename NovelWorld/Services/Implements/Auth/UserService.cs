using Microsoft.EntityFrameworkCore;
using NovelWorld.Dbcontexts;
using NovelWorld.Dtos.Auth.UserDtos;
using NovelWorld.Entities.Auth;
using NovelWorld.Exceptions;
using NovelWorld.Services.Interfaces.Auth;
using NovelWorld.Utils;

namespace NovelWorld.Services.Implements.Auth
{
    public class UserService : IUserService
    {
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserFollowService _userFollowService;

        public UserService(ILogger<UserService> logger, ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor, IUserFollowService userFollowService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _userFollowService = userFollowService;
        }

        public async Task CreateUserAsync(CreateUserDto input)
        {
            if (await _dbContext.Users.AnyAsync(u => u.DisplayName == input.DisplayName))
            {
                throw new UserFriendlyException($"User \"{input.DisplayName}\" already exists");
            }
            string imagePath = null;
            if (input.Image != null)
            {
                imagePath = await UploadFile.SaveFileAsync(input.Image, "Users", "Images");
            }
            await _dbContext.Users.AddAsync(new User
            {
                DisplayName = input.DisplayName,
                BirthYear = input.BirthYear,
                Gender = input.Gender,
                Image = imagePath
            });

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteUserAsync()
        {
            int currentUserId = CommonUntils.GetCurrentUserId(_httpContextAccessor);
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == currentUserId);

            if (user != null)
            {
                UploadFile.DeleteFile(user.Image, "Users", "Images");

                _dbContext.Users.Remove(user);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                throw new UserFriendlyException("User not found");
            }
        }

        public async Task<UserDto> GetUserAsync()
        {
            int currentUserId = CommonUntils.GetCurrentUserId(_httpContextAccessor);
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == currentUserId);
            int followersCount = await _userFollowService.GetFollowersCountAsync(currentUserId);
            int followingCount = await _userFollowService.GetFollowingCountAsync(currentUserId);

            if (user != null)
            {
                return new UserDto
                {
                    Id = user.UserId,
                    DisplayName = user.DisplayName,
                    BirthYear = user.BirthYear,
                    Gender = user.Gender,
                    Image = UploadFile.GetFileUrl(user.Image, _httpContextAccessor),
                    FollowersCount = followersCount,
                    FollowingCount = followingCount
                };
            }

            throw new UserFriendlyException("User not found!");
        }

        public async Task UpdateUserAsync(UpdateUserDto input)
        {
            int currentUserId = CommonUntils.GetCurrentUserId(_httpContextAccessor);
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == currentUserId);
            string imagePath = null;

            if (input.Image != null)
            {
                imagePath = await UploadFile.SaveFileAsync(input.Image, "Users", "Images");
            }
            if (user != null)
            {
                if (await _dbContext.Users
                    .AnyAsync(u => u.DisplayName == input.DisplayName && u.UserId != currentUserId))
                {
                    throw new UserFriendlyException("Something wrong!");
                }

                user.DisplayName = input.DisplayName;
                user.BirthYear = input.BirthYear;
                user.Gender = input.Gender;
                user.Image = imagePath ?? user.Image;
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                throw new UserFriendlyException("User not found");
            }
        }
    }
}