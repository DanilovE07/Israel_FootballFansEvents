using IsraelFootballFansEvents.DATA.Interfaces;
using IsraelFootballFansEvents.DATA.Models;
using Microsoft.EntityFrameworkCore;

namespace IsraelFootballFansEvents.DATA.Repositories
{
    public class SessionRepository : ISessionRepository
    {
        private readonly EventSystemContext _context;

        public SessionRepository(EventSystemContext context)
        {
            _context = context;
        }

        public List<Session> GetSessionsByEventId(int eventId)
        {
            return _context.Sessions
                .Where(s => s.EventId == eventId)
                .OrderBy(s => s.StartTime)
                .ToList();
        }

        public Session? GetSessionById(int sessionId)
        {
            return _context.Sessions
                .Include(s => s.Event)
                .FirstOrDefault(s => s.Id == sessionId);
        }

        public void AddSession(Session newSession)
        {
            _context.Sessions.Add(newSession);
            _context.SaveChanges();
        }

        public void UpdateSession(Session sessionToUpdate)
        {
            _context.Sessions.Update(sessionToUpdate);
            _context.SaveChanges();
        }

        public void DeleteSession(Session sessionToDelete)
        {
            _context.Sessions.Remove(sessionToDelete);
            _context.SaveChanges();
        }
    }
}