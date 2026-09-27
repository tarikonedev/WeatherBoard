using WeatherBoard.Domain.Entities;

namespace WeatherBoard.Application.Interfaces;

public interface IWeatherService
{
    Task<CurrentWeather> GetCurrentWeatherAsync(double latitude, double longitude, CancellationToken cancellationToken = default);
}
