using IsraelFootballFansEvents.WebApi.DTOs;
using IsraelFootballFansEvents.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace IsraelFootballFansEvents.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly RegistrationService _registrationService;
        private readonly UserService _userService;

        public UserController(
            RegistrationService registrationService,
            UserService userService)
        {
            _registrationService = registrationService;
            _userService = userService;
        }

        [HttpGet]
        public ActionResult<List<UserDto>> GetAllUsers()
        {
            try
            {
                List<UserDto> users = _userService.GetAllUsers();

                if (users == null || users.Count == 0)
                {
                    return NotFound("No users found");
                }

                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("{userId}/schedule")]
        public ActionResult<List<UserScheduleDto>> GetUserSchedule(int userId)
        {
            try
            {
                List<UserScheduleDto> schedule = _registrationService.GetUserSchedule(userId);

                if (schedule == null || schedule.Count == 0)
                {
                    return NotFound("No schedule found for this user");
                }

                return Ok(schedule);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}