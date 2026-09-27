using Microsoft.Extensions.DependencyInjection;
using WeatherBoard.Application.Favorites;

namespace WeatherBoard.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<FavoritesService>();
        return services;
    }
}
