using IsraelFootballFansEvents.DATA.Interfaces;
using IsraelFootballFansEvents.DATA.Models;
using IsraelFootballFansEvents.DATA.Repositories;
using IsraelFootballFansEvents.WebApi.DTOs;

namespace IsraelFootballFansEvents.WebApi.Services
{
    public class EventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly ISessionRepository _sessionRepository;
        private readonly IRegistrationRepository _registrationRepository;

        public EventService(IEventRepository eventRepository, ISessionRepository sessionRepository, IRegistrationRepository registrationRepository)
        {
            _eventRepository = eventRepository;
            _sessionRepository = sessionRepository;
            _registrationRepository = registrationRepository;
        }

        public List<EventDto> GetSchedule()
        {
            List<Event> events = _eventRepository.GetAllEvents();

            List<EventDto> result = events.Select(e => new EventDto
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                Location = e.Location,
                EventType = e.EventType
            }).ToList();

            return result;
        }

        public EventDetailsDto? GetEventById(int eventId)
        {
            Event? eventFromDb = _eventRepository.GetEventById(eventId);

            if (eventFromDb == null)
            {
                return null;
            }

            EventDetailsDto result = new EventDetailsDto
            {
                Id = eventFromDb.Id,
                Title = eventFromDb.Title,
                Description = eventFromDb.Description,
                StartDate = eventFromDb.StartDate,
                EndDate = eventFromDb.EndDate,
                Location = eventFromDb.Location,
                EventType = eventFromDb.EventType,

                Sessions = eventFromDb.Sessions
                    .OrderBy(s => s.StartTime)
                    .Select(s => new SessionDto
                    {
                        Id = s.Id,
                        EventId = s.EventId,
                        Title = s.Title,
                        Description = s.Description,
                        SpeakerName = s.SpeakerName,
                        StartTime = s.StartTime,
                        EndTime = s.EndTime,
                        RoomName = s.RoomName
                    })
                    .ToList()
            };

            return result;
        }

        public EventDto AddEvent(EventDto eventDto)
        {
            if (string.IsNullOrEmpty(eventDto.Title))
            {
                throw new ArgumentException("Event title is required");
            }

            if (string.IsNullOrEmpty(eventDto.Location))
            {
                throw new ArgumentException("Event location is required");
            }

            if (eventDto.StartDate >= eventDto.EndDate)
            {
                throw new ArgumentException("Event start date must be before end date");
            }

            Event newEvent = new Event
            {
                Title = eventDto.Title,
                Description = eventDto.Description,
                StartDate = eventDto.StartDate,
                EndDate = eventDto.EndDate,
                Location = eventDto.Location,
                EventType = eventDto.EventType
            };

            _eventRepository.AddEvent(newEvent);

            EventDto result = new EventDto
            {
                Id = newEvent.Id,
                Title = newEvent.Title,
                Description = newEvent.Description,
                StartDate = newEvent.StartDate,
                EndDate = newEvent.EndDate,
                Location = newEvent.Location,
                EventType = newEvent.EventType
            };

            return result;
        }

        public string UpdateEvent(int eventId, EventDto eventDto)
        {
            Event? eventFromDb = _eventRepository.GetEventById(eventId);

            if (eventFromDb == null)
            {
                return "Event not found";
            }

            if (string.IsNullOrWhiteSpace(eventDto.Title))
            {
                return "Event title is required";
            }

            if (string.IsNullOrWhiteSpace(eventDto.Location))
            {
                return "Event location is required";
            }

            if (eventDto.StartDate >= eventDto.EndDate)
            {
                return "Start date must be before end date";
            }

            eventFromDb.Title = eventDto.Title;
            eventFromDb.Description = eventDto.Description;
            eventFromDb.StartDate = eventDto.StartDate;
            eventFromDb.EndDate = eventDto.EndDate;
            eventFromDb.Location = eventDto.Location;
            eventFromDb.EventType = eventDto.EventType;

            _eventRepository.UpdateEvent(eventFromDb);

            return "Event updated successfully";
        }

        public string DeleteEvent(int eventId)
        {
            Event? eventFromDb = _eventRepository.GetEventById(eventId);

            if (eventFromDb == null)
            {
                return "Event not found";
            }

            List<Session> sessions = _sessionRepository.GetSessionsByEventId(eventId);

            foreach (Session session in sessions)
            {
                _registrationRepository.DeleteRegistrationsBySessionId(session.Id);
                _sessionRepository.DeleteSession(session);
            }

            _eventRepository.DeleteEvent(eventFromDb);

            return "Event deleted successfully";
        }
    }
}