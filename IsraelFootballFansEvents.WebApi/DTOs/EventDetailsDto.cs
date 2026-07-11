namespace IsraelFootballFansEvents.WebApi.DTOs
{
    public class EventDetailsDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Location { get; set; }
        public string? EventType { get; set; }

        public List<SessionDto> Sessions { get; set; } = new List<SessionDto>();
    }
}