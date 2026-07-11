using IsraelFootballFansEvents.WebApi.DTOs;
using IsraelFootballFansEvents.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace IsraelFootballFansEvents.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        private readonly SessionService _sessionService;
        private readonly RegistrationService _registrationService;

        public SessionController(
            SessionService sessionService,
            RegistrationService registrationService)
        {
            _sessionService = sessionService;
            _registrationService = registrationService;
        }

        [HttpPost]
        [Route("{id}/register")]
        public ActionResult<string> RegisterUserToSession(int id, RegistrationDto registrationDto)
        {
            try
            {
                string result = _registrationService.RegisterUserToSession(id, registrationDto);

                if (result == "User not found" || result == "Session not found")
                {
                    return NotFound(result);
                }

                if (result != "Registration added successfully")
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
        [Route("{id}/users")]
        [Route("{id}/user")]
        public ActionResult<List<SessionUserDto>> GetUsersBySessionId(int id)
        {
            try
            {
                List<SessionUserDto> users = _registrationService.GetUsersBySessionId(id);

                if (users == null || users.Count == 0)
                {
                    return NotFound("No users registered to this session");
                }

                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("{id}")]
        public ActionResult<SessionDto> GetSessionById(int id)
        {
            try
            {
                SessionDto? session = _sessionService.GetSessionById(id);

                if (session == null)
                {
                    return NotFound("Session not found");
                }

                return Ok(session);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("{id}")]
        public ActionResult<string> UpdateSession(int id, SessionDto sessionDto)
        {
            try
            {
                string result = _sessionService.UpdateSession(id, sessionDto);

                if (result == "Session not found" || result == "Event not found")
                {
                    return NotFound(result);
                }

                if (result != "Session updated successfully")
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
        public ActionResult<string> DeleteSession(int id)
        {
            try
            {
                string result = _sessionService.DeleteSession(id);

                if (result == "Session not found")
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
    }
}