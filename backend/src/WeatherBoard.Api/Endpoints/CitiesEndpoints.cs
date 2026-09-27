using WeatherBoard.Application.Interfaces;

namespace WeatherBoard.Api.Endpoints;

public static class CitiesEndpoints
{
    public static void MapCitiesEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/cities/search", async (string? q, IGeocodingService geocodingService, CancellationToken cancellationToken) =>
        {
            var results = await geocodingService.SearchCitiesAsync(q ?? string.Empty, cancellationToken);
            return Results.Ok(results);
        })
        .WithName("SearchCities");
    }
}
