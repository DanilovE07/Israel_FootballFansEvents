using IsraelFootballFansEvents.DATA.Interfaces;
using IsraelFootballFansEvents.DATA.Models;
using IsraelFootballFansEvents.WebApi.DTOs;

namespace IsraelFootballFansEvents.WebApi.Services
{
    public class SessionService
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IRegistrationRepository _registrationRepository;

        public SessionService(ISessionRepository sessionRepository,IEventRepository eventRepository,IRegistrationRepository registrationRepository)
        {
            _sessionRepository = sessionRepository;
            _eventRepository = eventRepository;
            _registrationRepository = registrationRepository;
        }

        public List<SessionDto> GetSessionsByEventId(int eventId)
        {
            List<Session> sessions = _sessionRepository.GetSessionsByEventId(eventId);

            List<SessionDto> result = sessions.Select(s => new SessionDto
            {
                Id = s.Id,
                EventId = s.EventId,
                Title = s.Title,
                Description = s.Description,
                SpeakerName = s.SpeakerName,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                RoomName = s.RoomName
            }).ToList();

            return result;
        }

        public string AddSessionToEvent(int eventId, SessionDto sessionDto)
        {
            Event? eventFromDb = _eventRepository.GetEventById(eventId);

            if (eventFromDb == null)
            {
                return "Event not found";
            }

            if (string.IsNullOrWhiteSpace(sessionDto.Title))
            {
                return "Session title is required";
            }

            if (sessionDto.StartTime >= sessionDto.EndTime)
            {
                return "Session start time must be before end time";
            }

            if (sessionDto.StartTime < eventFromDb.StartDate ||
                sessionDto.EndTime > eventFromDb.EndDate)
            {
                return "Session must be inside the event date range";
            }

            List<Session> existingSessions = _sessionRepository.GetSessionsByEventId(eventId);

            bool sessionAlreadyExists = existingSessions.Any(s =>
                s.Title == sessionDto.Title &&
                s.StartTime == sessionDto.StartTime &&
                s.EndTime == sessionDto.EndTime &&
                s.RoomName == sessionDto.RoomName);

            if (sessionAlreadyExists)
            {
                return "Session already exists for this event";
            }

            bool hasRoomOverlap = existingSessions.Any(s =>
                s.RoomName == sessionDto.RoomName &&
                sessionDto.StartTime < s.EndTime &&
                sessionDto.EndTime > s.StartTime);

            if (hasRoomOverlap)
            {
                return "Room already has another session at the same time";
            }

            Session newSession = new Session
            {
                EventId = eventId,
                Title = sessionDto.Title,
                Description = sessionDto.Description,
                SpeakerName = sessionDto.SpeakerName,
                StartTime = sessionDto.StartTime,
                EndTime = sessionDto.EndTime,
                RoomName = sessionDto.RoomName
            };

            _sessionRepository.AddSession(newSession);

            return "Session added successfully";
        }

        public string UpdateSession(int sessionId, SessionDto sessionDto)
        {
            Session? sessionFromDb = _sessionRepository.GetSessionById(sessionId);

            if (sessionFromDb == null)
            {
                return "Session not found";
            }

            Event? eventFromDb = _eventRepository.GetEventById(sessionFromDb.EventId);

            if (eventFromDb == null)
            {
                return "Event not found";
            }

            if (string.IsNullOrWhiteSpace(sessionDto.Title))
            {
                return "Session title is required";
            }

            if (sessionDto.StartTime >= sessionDto.EndTime)
            {
                return "Session start time must be before end time";
            }

            if (sessionDto.StartTime < eventFromDb.StartDate ||
                sessionDto.EndTime > eventFromDb.EndDate)
            {
                return "Session must be inside the event date range";
            }

            List<Session> existingSessions = _sessionRepository.GetSessionsByEventId(sessionFromDb.EventId);

            bool hasRoomOverlap = existingSessions.Any(s =>
                s.Id != sessionId &&
                s.RoomName == sessionDto.RoomName &&
                sessionDto.StartTime < s.EndTime &&
                sessionDto.EndTime > s.StartTime);

            if (hasRoomOverlap)
            {
                return "Room already has another session at the same time";
            }

            sessionFromDb.Title = sessionDto.Title;
            sessionFromDb.Description = sessionDto.Description;
            sessionFromDb.SpeakerName = sessionDto.SpeakerName;
            sessionFromDb.StartTime = sessionDto.StartTime;
            sessionFromDb.EndTime = sessionDto.EndTime;
            sessionFromDb.RoomName = sessionDto.RoomName;

            _sessionRepository.UpdateSession(sessionFromDb);

            return "Session updated successfully";
        }

        public string DeleteSession(int sessionId)
        {
            Session? sessionFromDb = _sessionRepository.GetSessionById(sessionId);

            if (sessionFromDb == null)
            {
                return "Session not found";
            }

            _registrationRepository.DeleteRegistrationsBySessionId(sessionId);

            _sessionRepository.DeleteSession(sessionFromDb);

            return "Session deleted successfully";
        }

        public SessionDto? GetSessionById(int sessionId)
        {
            Session? sessionFromDb = _sessionRepository.GetSessionById(sessionId);

            if (sessionFromDb == null)
            {
                return null;
            }

            SessionDto result = new SessionDto
            {
                Id = sessionFromDb.Id,
                EventId = sessionFromDb.EventId,
                Title = sessionFromDb.Title,
                Description = sessionFromDb.Description,
                SpeakerName = sessionFromDb.SpeakerName,
                StartTime = sessionFromDb.StartTime,
                EndTime = sessionFromDb.EndTime,
                RoomName = sessionFromDb.RoomName
            };

            return result;
        }
    }
}