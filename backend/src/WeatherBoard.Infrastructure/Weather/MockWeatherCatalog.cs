using WeatherBoard.Domain.Entities;

namespace WeatherBoard.Infrastructure.Weather;

public static class MockWeatherCatalog
{
    public static readonly IReadOnlyDictionary<string, CurrentWeather> ByCityName = new Dictionary<string, CurrentWeather>(StringComparer.OrdinalIgnoreCase)
    {
        ["Kyiv"] = new CurrentWeather(18.5, 21.0, 12.0, 64, 14.2, 3, true, "Europe/Kyiv", 10800),
        ["London"] = new CurrentWeather(14.0, 16.0, 9.5, 78, 22.0, 61, true, "Europe/London", 3600),
        ["New York"] = new CurrentWeather(22.0, 25.0, 17.0, 55, 11.5, 0, true, "America/New_York", -14400),
        ["Tokyo"] = new CurrentWeather(19.5, 22.0, 16.0, 82, 8.0, 45, false, "Asia/Tokyo", 32400),
        ["Sydney"] = new CurrentWeather(16.0, 18.5, 11.0, 60, 19.0, 1, false, "Australia/Sydney", 39600),
        ["Paris"] = new CurrentWeather(17.0, 19.0, 13.0, 70, 26.5, 95, true, "Europe/Paris", 3600),
        ["Cairo"] = new CurrentWeather(31.0, 34.0, 24.0, 25, 9.5, 0, true, "Africa/Cairo", 7200),
        ["Rio de Janeiro"] = new CurrentWeather(27.0, 29.5, 22.0, 74, 15.0, 80, true, "America/Sao_Paulo", -10800),
        ["Reykjavik"] = new CurrentWeather(3.5, 5.0, 0.5, 85, 30.0, 71, false, "Atlantic/Reykjavik", 0),
        ["Nairobi"] = new CurrentWeather(21.0, 24.0, 14.0, 58, 10.0, 2, true, "Africa/Nairobi", 10800),
    };

    public static readonly CurrentWeather Default = new(20.0, 22.0, 15.0, 50, 10.0, 1, true, "UTC", 0);
}
