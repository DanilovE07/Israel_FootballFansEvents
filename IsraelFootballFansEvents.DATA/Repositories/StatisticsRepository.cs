using IsraelFootballFansEvents.DATA.Interfaces;
using IsraelFootballFansEvents.DATA.Models;
using Microsoft.EntityFrameworkCore;

namespace IsraelFootballFansEvents.DATA.Repositories
{
    public class StatisticsRepository : IStatisticsRepository
    {
        private readonly EventSystemContext _context;

        public StatisticsRepository(EventSystemContext context)
        {
            _context = context;
        }

        public int GetTotalEvents()
        {
            return _context.Events.Count();
        }

        public int GetTotalSessions()
        {
            return _context.Sessions.Count();
        }

        public int GetTotalRegistrations()
        {
            return _context.SessionRegistrations.Count();
        }

        public List<Event> GetAllEvents()
        {
            return _context.Events.ToList();
        }

        public List<SessionRegistration> GetAllRegistrationsWithSessions()
        {
            return _context.SessionRegistrations
                .Include(r => r.Session)
                .ToList();
        }
    }
}