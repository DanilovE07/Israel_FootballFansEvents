using IsraelFootballFansEvents.DATA.Models;

namespace IsraelFootballFansEvents.DATA.Interfaces
{
    public interface IUserRepository
    {
        List<User> GetAllUsers();
        User? GetUserById(int userId);
    }
}