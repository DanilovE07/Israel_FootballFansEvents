using IsraelFootballFansEvents.DATA.Interfaces;
using IsraelFootballFansEvents.DATA.Models;
using Microsoft.Extensions.Caching.Memory;

namespace IsraelFootballFansEvents.WebApi.Services
{
    public class WeatherService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _cache;
        private readonly IEventRepository _eventRepository;
        private readonly ILogger<WeatherService> _logger;

        public WeatherService(
            IHttpClientFactory httpClientFactory,
            IMemoryCache cache,
            IEventRepository eventRepository,
            ILogger<WeatherService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _cache = cache;
            _eventRepository = eventRepository;
            _logger = logger;
        }

        public async Task<string?> GetWeatherByEventIdAsync(int eventId)
        {
            Event? eventFromDb = _eventRepository.GetEventById(eventId);

            if (eventFromDb == null)
            {
                _logger.LogWarning("Weather request failed. Event {EventId} was not found", eventId);
                return null;
            }

            string cacheKey = "weather-event-" + eventId;

            if (_cache.TryGetValue(cacheKey, out string cachedWeather))
            {
                _logger.LogInformation("Weather for event {EventId} returned from Cache", eventId);
                return cachedWeather;
            }

            _logger.LogInformation("Weather for event {EventId} not found in Cache. Calling external API", eventId);

            LocationCoordinates coordinates = GetCoordinatesByLocation(eventFromDb.Location);

            string url =
                $"https://api.open-meteo.com/v1/forecast" +
                $"?latitude={coordinates.Latitude}" +
                $"&longitude={coordinates.Longitude}" +
                $"&current=temperature_2m,wind_speed_10m,weather_code" +
                $"&timezone=auto";

            var client = _httpClientFactory.CreateClient();

            string json = await client.GetStringAsync(url);

            _cache.Set(cacheKey, json, TimeSpan.FromMinutes(5));

            _logger.LogInformation("Weather for event {EventId} saved in Cache for 5 minutes", eventId);

            return json;
        }

        private LocationCoordinates GetCoordinatesByLocation(string? location)
        {
            if (location == null)
            {
                return new LocationCoordinates
                {
                    Latitude = 32.0853,
                    Longitude = 34.7818
                };
            }

            if (location.Contains("Tel Aviv") || location.Contains("Bloomfield"))
            {
                return new LocationCoordinates
                {
                    Latitude = 32.0853,
                    Longitude = 34.7818
                };
            }

            if (location.Contains("Ruppin"))
            {
                return new LocationCoordinates
                {
                    Latitude = 32.3426,
                    Longitude = 34.9120
                };
            }

            if (location.Contains("Haifa"))
            {
                return new LocationCoordinates
                {
                    Latitude = 32.7940,
                    Longitude = 34.9896
                };
            }

            return new LocationCoordinates
            {
                Latitude = 32.0853,
                Longitude = 34.7818
            };
        }

        private class LocationCoordinates
        {
            public double Latitude { get; set; }

            public double Longitude { get; set; }
        }
    }
}