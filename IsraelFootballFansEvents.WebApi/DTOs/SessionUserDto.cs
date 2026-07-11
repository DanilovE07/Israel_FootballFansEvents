namespace IsraelFootballFansEvents.WebApi.DTOs
{
    public class SessionUserDto
    {
        public int UserId { get; set; }

        public string? FullName { get; set; }

        public string? Email { get; set; }

        public DateTime? RegistrationDate { get; set; }
    }
}