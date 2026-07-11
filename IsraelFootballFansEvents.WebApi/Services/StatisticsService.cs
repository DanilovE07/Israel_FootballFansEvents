using IsraelFootballFansEvents.DATA.Interfaces;
using IsraelFootballFansEvents.DATA.Models;
using IsraelFootballFansEvents.WebApi.DTOs;

namespace IsraelFootballFansEvents.WebApi.Services
{
    public class StatisticsService
    {
        private readonly IStatisticsRepository _statisticsRepository;

        public StatisticsService(IStatisticsRepository statisticsRepository)
        {
            _statisticsRepository = statisticsRepository;
        }

        public StatisticsDto GetStatistics()
        {
            int totalEvents = _statisticsRepository.GetTotalEvents();
            int totalSessions = _statisticsRepository.GetTotalSessions();
            int totalRegistrations = _statisticsRepository.GetTotalRegistrations();

            double averageRegistrationsPerSession = 0;

            if (totalSessions > 0)
            {
                averageRegistrationsPerSession = (double)totalRegistrations / totalSessions;
            }

            List<Event> events = _statisticsRepository.GetAllEvents();

            List<EventByMonthDto> eventsByMonth = events
                .GroupBy(e => new
                {
                    Year = e.StartDate.Year,
                    Month = e.StartDate.Month
                })
                .Select(g => new EventByMonthDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    EventsCount = g.Count()
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToList();

            List<SessionRegistration> registrations =
                _statisticsRepository.GetAllRegistrationsWithSessions();

            PopularSessionDto? mostPopularSession = registrations
                .GroupBy(r => r.SessionId)
                .Select(g => new PopularSessionDto
                {
                    SessionId = g.Key,
                    SessionTitle = g.First().Session.Title,
                    RegistrationsCount = g.Count()
                })
                .OrderByDescending(x => x.RegistrationsCount)
                .FirstOrDefault();

            StatisticsDto result = new StatisticsDto
            {
                TotalEvents = totalEvents,
                TotalSessions = totalSessions,
                TotalRegistrations = totalRegistrations,
                AverageRegistrationsPerSession = averageRegistrationsPerSession,
                MostPopularSession = mostPopularSession,
                EventsByMonth = eventsByMonth
            };

            return result;
        }
    }
}