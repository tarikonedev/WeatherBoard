using WeatherBoard.Application.Interfaces;
using WeatherBoard.Domain.Entities;

namespace WeatherBoard.Infrastructure.Geocoding;

public class MockGeocodingService : IGeocodingService
{
    public Task<IReadOnlyList<City>> SearchCitiesAsync(string query, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return Task.FromResult<IReadOnlyList<City>>([]);
        }

        var matches = MockCityCatalog.Cities
            .Where(city =>
                city.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                city.Country.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return Task.FromResult<IReadOnlyList<City>>(matches);
    }
}
