using IsraelFootballFansEvents.WebApi.DTOs;
using IsraelFootballFansEvents.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace IsraelFootballFansEvents.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly StatisticsService _statisticsService;

        public StatisticsController(StatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        [HttpGet]
        public ActionResult<StatisticsDto> GetStatistics()
        {
            try
            {
                StatisticsDto statistics = _statisticsService.GetStatistics();

                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}