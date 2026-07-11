namespace IsraelFootballFansEvents.WebApi.DTOs
{
    public class StatisticsDto
    {
        public int TotalEvents { get; set; }

        public int TotalSessions { get; set; }

        public int TotalRegistrations { get; set; }

        public double AverageRegistrationsPerSession { get; set; }

        public PopularSessionDto? MostPopularSession { get; set; }

        public List<EventByMonthDto> EventsByMonth { get; set; } = new List<EventByMonthDto>();
    }
}