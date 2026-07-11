namespace IsraelFootballFansEvents.WebApi.DTOs
{
    public class UserScheduleDto
    {
        public int SessionId { get; set; }

        public int EventId { get; set; }

        public string? EventTitle { get; set; }

        public string? SessionTitle { get; set; }

        public string? SpeakerName { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string? RoomName { get; set; }
    }
}