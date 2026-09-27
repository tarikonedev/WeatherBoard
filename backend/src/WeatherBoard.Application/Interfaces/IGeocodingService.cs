using WeatherBoard.Domain.Entities;

namespace WeatherBoard.Application.Interfaces;

public interface IGeocodingService
{
    Task<IReadOnlyList<City>> SearchCitiesAsync(string query, CancellationToken cancellationToken = default);
}
