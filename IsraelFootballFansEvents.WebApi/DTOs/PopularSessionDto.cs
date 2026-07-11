namespace IsraelFootballFansEvents.WebApi.DTOs
{
    public class PopularSessionDto
    {
        public int SessionId { get; set; }

        public string? SessionTitle { get; set; }

        public int RegistrationsCount { get; set; }
    }
}