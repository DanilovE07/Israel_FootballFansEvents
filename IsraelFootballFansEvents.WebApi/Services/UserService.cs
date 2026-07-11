using IsraelFootballFansEvents.DATA.Interfaces;
using IsraelFootballFansEvents.DATA.Models;
using IsraelFootballFansEvents.WebApi.DTOs;

namespace IsraelFootballFansEvents.WebApi.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public List<UserDto> GetAllUsers()
        {
            List<User> users = _userRepository.GetAllUsers();

            List<UserDto> result = users.Select(u => new UserDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email
            }).ToList();

            return result;
        }
    }
}