using IsraelFootballFansEvents.DATA.Interfaces;
using IsraelFootballFansEvents.DATA.Models;
using IsraelFootballFansEvents.WebApi.DTOs;

namespace IsraelFootballFansEvents.WebApi.Services
{
    public class RegistrationService
    {
        private readonly IRegistrationRepository _registrationRepository;

        public RegistrationService(IRegistrationRepository registrationRepository)
        {
            _registrationRepository = registrationRepository;
        }

        public string RegisterUserToSession(int sessionId, RegistrationDto registrationDto)
        {
            User? userFromDb = _registrationRepository.GetUserById(registrationDto.UserId);

            if (userFromDb == null)
            {
                return "User not found";
            }

            Session? sessionFromDb = _registrationRepository.GetSessionById(sessionId);

            if (sessionFromDb == null)
            {
                return "Session not found";
            }

            SessionRegistration? existingRegistration =
                _registrationRepository.GetRegistration(sessionId, registrationDto.UserId);

            if (existingRegistration != null)
            {
                return "User already registered to this session";
            }

            List<SessionRegistration> userRegistrations =
                _registrationRepository.GetRegistrationsByUserId(registrationDto.UserId);

            bool hasOverlap = userRegistrations.Any(r =>
                sessionFromDb.StartTime < r.Session.EndTime &&
                sessionFromDb.EndTime > r.Session.StartTime);

            if (hasOverlap)
            {
                return "User has another session at the same time";
            }

            SessionRegistration newRegistration = new SessionRegistration
            {
                SessionId = sessionId,
                UserId = registrationDto.UserId,
                RegistrationDate = DateTime.Now
            };

            _registrationRepository.AddRegistration(newRegistration);

            return "Registration added successfully";
        }

        public List<UserScheduleDto> GetUserSchedule(int userId)
        {
            List<SessionRegistration> registrations =
                _registrationRepository.GetRegistrationsByUserId(userId);

            List<UserScheduleDto> result = registrations.Select(r => new UserScheduleDto
            {
                SessionId = r.Session.Id,
                EventId = r.Session.EventId,
                EventTitle = r.Session.Event.Title,
                SessionTitle = r.Session.Title,
                SpeakerName = r.Session.SpeakerName,
                StartTime = r.Session.StartTime,
                EndTime = r.Session.EndTime,
                RoomName = r.Session.RoomName
            }).ToList();

            return result;
        }

        public List<SessionUserDto> GetUsersBySessionId(int sessionId)
        {
            List<SessionRegistration> registrations =
                _registrationRepository.GetRegistrationsBySessionId(sessionId);

            List<SessionUserDto> result = registrations.Select(r => new SessionUserDto
            {
                UserId = r.User.Id,
                FullName = r.User.FullName,
                Email = r.User.Email,
                RegistrationDate = r.RegistrationDate
            }).ToList();

            return result;
        }
    }
}