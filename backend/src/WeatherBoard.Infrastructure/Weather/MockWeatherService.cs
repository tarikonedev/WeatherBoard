using WeatherBoard.Application.Interfaces;
using WeatherBoard.Domain.Entities;
using WeatherBoard.Infrastructure.Geocoding;

namespace WeatherBoard.Infrastructure.Weather;

public class MockWeatherService : IWeatherService
{
    private const double ToleranceDegrees = 0.5;

    public Task<CurrentWeather> GetCurrentWeatherAsync(double latitude, double longitude, CancellationToken cancellationToken = default)
    {
        var nearestCity = MockCityCatalog.Cities.FirstOrDefault(city =>
            Math.Abs(city.Latitude - latitude) <= ToleranceDegrees &&
            Math.Abs(city.Longitude - longitude) <= ToleranceDegrees);

        var weather = nearestCity is not null && MockWeatherCatalog.ByCityName.TryGetValue(nearestCity.Name, out var preset)
            ? preset
            : MockWeatherCatalog.Default;

        return Task.FromResult(weather);
    }
}
