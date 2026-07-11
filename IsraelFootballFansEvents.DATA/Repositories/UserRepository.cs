using IsraelFootballFansEvents.DATA.Interfaces;
using IsraelFootballFansEvents.DATA.Models;

namespace IsraelFootballFansEvents.DATA.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly EventSystemContext _context;

        public UserRepository(EventSystemContext context)
        {
            _context = context;
        }

        public List<User> GetAllUsers()
        {
            return _context.Users
                .OrderBy(u => u.FullName)
                .ToList();
        }

        public User? GetUserById(int userId)
        {
            return _context.Users
                .FirstOrDefault(u => u.Id == userId);
        }
    }
}