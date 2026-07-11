using IsraelFootballFansEvents.WebApi.DTOs;
using IsraelFootballFansEvents.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace IsraelFootballFansEvents.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly EventService _eventService;
        private readonly SessionService _sessionService;
        private readonly WeatherService _weatherService;

        public EventController(EventService eventService, SessionService sessionService, WeatherService weatherService)
        {
            _eventService = eventService;
            _sessionService = sessionService;
            _weatherService = weatherService;
        }

        [HttpGet]
        [Route("schedule")]
        public ActionResult<List<EventDto>> GetSchedule()
        {
            try
            {
                List<EventDto> events = _eventService.GetSchedule();

                if (events == null || events.Count == 0)
                {
                    return NotFound("No events found");
                }

                return Ok(events);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("{id}")]
        public ActionResult<EventDetailsDto> GetEventById(int id)
        {
            try
            {
                EventDetailsDto? eventDetails = _eventService.GetEventById(id);

                if (eventDetails == null)
                {
                    return NotFound("Event not found");
                }

                return Ok(eventDetails);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult<EventDto> AddEvent(EventDto eventDto)
        {
            try
            {
                EventDto createdEvent = _eventService.AddEvent(eventDto);

                return CreatedAtAction(
                    nameof(GetEventById),
                    new { id = createdEvent.Id },
                    createdEvent
                );
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("{id}")]
        public ActionResult<string> UpdateEvent(int id, EventDto eventDto)
        {
            try
            {
                string result = _eventService.UpdateEvent(id, eventDto);

                if (result == "Event not found")
                {
                    return NotFound(result);
                }

                if (result != "Event updated successfully")
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public ActionResult<string> DeleteEvent(int id)
        {
            try
            {
                string result = _eventService.DeleteEvent(id);

                if (result == "Event not found")
                {
                    return NotFound(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("{eventId}/session")]
        public ActionResult<string> AddSessionToEvent(int eventId, SessionDto sessionDto)
        {
            try
            {
                string result = _sessionService.AddSessionToEvent(eventId, sessionDto);

                if (result == "Event not found")
                {
                    return NotFound(result);
                }

                if (result != "Session added successfully")
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("{eventId}/sessions")]
        public ActionResult<List<SessionDto>> GetSessionsByEventId(int eventId)
        {
            try
            {
                List<SessionDto> sessions = _sessionService.GetSessionsByEventId(eventId);

                if (sessions == null || sessions.Count == 0)
                {
                    return NotFound("No sessions found for this event");
                }

                return Ok(sessions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("{id}/weather")]
        public async Task<ActionResult> GetWeatherByEventId(int id)
        {
            try
            {
                string? weatherJson = await _weatherService.GetWeatherByEventIdAsync(id);

                if (weatherJson == null)
                {
                    return NotFound("Event not found");
                }

                return Content(weatherJson, "application/json");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}