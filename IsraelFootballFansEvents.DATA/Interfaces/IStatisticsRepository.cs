using IsraelFootballFansEvents.DATA.Models;

namespace IsraelFootballFansEvents.DATA.Interfaces
{
    public interface IStatisticsRepository
    {
        int GetTotalEvents();

        int GetTotalSessions();

        int GetTotalRegistrations();

        List<Event> GetAllEvents();

        List<SessionRegistration> GetAllRegistrationsWithSessions();
    }
}