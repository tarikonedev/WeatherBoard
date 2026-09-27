using WeatherBoard.Application.Interfaces;

namespace WeatherBoard.Api.Endpoints;

public static class WeatherEndpoints
{
    public static void MapWeatherEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/weather/current", async (double lat, double lon, IWeatherService weatherService, CancellationToken cancellationToken) =>
        {
            var weather = await weatherService.GetCurrentWeatherAsync(lat, lon, cancellationToken);
            return Results.Ok(weather);
        })
        .WithName("GetCurrentWeather");
    }
}
