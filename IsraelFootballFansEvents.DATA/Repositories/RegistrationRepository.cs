using IsraelFootballFansEvents.DATA.Interfaces;
using IsraelFootballFansEvents.DATA.Models;
using Microsoft.EntityFrameworkCore;

namespace IsraelFootballFansEvents.DATA.Repositories
{
    public class RegistrationRepository : IRegistrationRepository
    {
        private readonly EventSystemContext _context;

        public RegistrationRepository(EventSystemContext context)
        {
            _context = context;
        }

        public User? GetUserById(int userId)
        {
            return _context.Users
                .FirstOrDefault(u => u.Id == userId);
        }

        public Session? GetSessionById(int sessionId)
        {
            return _context.Sessions
                .Include(s => s.Event)
                .FirstOrDefault(s => s.Id == sessionId);
        }

        public SessionRegistration? GetRegistration(int sessionId, int userId)
        {
            return _context.SessionRegistrations
                .FirstOrDefault(r => r.SessionId == sessionId && r.UserId == userId);
        }

        public List<SessionRegistration> GetRegistrationsByUserId(int userId)
        {
            return _context.SessionRegistrations
                .Include(r => r.Session)
                .ThenInclude(s => s.Event)
                .Where(r => r.UserId == userId)
                .OrderBy(r => r.Session.StartTime)
                .ToList();
        }

        public List<SessionRegistration> GetRegistrationsBySessionId(int sessionId)
        {
            return _context.SessionRegistrations
                .Include(r => r.User)
                .Where(r => r.SessionId == sessionId)
                .OrderBy(r => r.User.FullName)
                .ToList();
        }

        public void AddRegistration(SessionRegistration registration)
        {
            _context.SessionRegistrations.Add(registration);
            _context.SaveChanges();
        }

        public void DeleteRegistrationsBySessionId(int sessionId)
        {
            List<SessionRegistration> registrations = _context.SessionRegistrations
                .Where(r => r.SessionId == sessionId)
                .ToList();

            _context.SessionRegistrations.RemoveRange(registrations);
            _context.SaveChanges();
        }
    }
}