using Microsoft.Extensions.DependencyInjection;
using WeatherBoard.Application.Interfaces;
using WeatherBoard.Infrastructure.Favorites;
using WeatherBoard.Infrastructure.Geocoding;
using WeatherBoard.Infrastructure.Weather;

namespace WeatherBoard.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IFavoritesRepository, InMemoryFavoritesRepository>();
        services.AddSingleton<IGeocodingService, MockGeocodingService>();
        services.AddSingleton<IWeatherService, MockWeatherService>();
        return services;
    }
}
