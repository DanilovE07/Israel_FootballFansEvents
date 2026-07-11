using IsraelFootballFansEvents.DATA.Models;

namespace IsraelFootballFansEvents.DATA.Interfaces
{
    public interface ISessionRepository
    {
        List<Session> GetSessionsByEventId(int eventId);
        Session? GetSessionById(int sessionId);
        void AddSession(Session newSession);
        void UpdateSession(Session sessionToUpdate);
        void DeleteSession(Session sessionToDelete);
    }
}