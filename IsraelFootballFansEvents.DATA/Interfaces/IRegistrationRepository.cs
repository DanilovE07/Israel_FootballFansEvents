using IsraelFootballFansEvents.DATA.Models;

namespace IsraelFootballFansEvents.DATA.Interfaces
{
    public interface IRegistrationRepository
    {
        User? GetUserById(int userId);

        Session? GetSessionById(int sessionId);

        SessionRegistration? GetRegistration(int sessionId, int userId);

        List<SessionRegistration> GetRegistrationsByUserId(int userId);

        List<SessionRegistration> GetRegistrationsBySessionId(int sessionId);

        void AddRegistration(SessionRegistration registration);

        void DeleteRegistrationsBySessionId(int sessionId);
    }
}