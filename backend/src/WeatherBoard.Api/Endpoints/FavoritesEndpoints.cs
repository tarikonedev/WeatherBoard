using WeatherBoard.Application.Favorites;
using WeatherBoard.Domain.Entities;

namespace WeatherBoard.Api.Endpoints;

public record ReorderRequest(IReadOnlyList<Guid> OrderedIds);

public static class FavoritesEndpoints
{
    public static void MapFavoritesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/favorites");

        group.MapGet("/", async (FavoritesService favoritesService, CancellationToken cancellationToken) =>
        {
            var favorites = await favoritesService.GetAllAsync(cancellationToken);
            return Results.Ok(favorites);
        })
        .WithName("GetFavorites");

        group.MapPost("/", async (City city, FavoritesService favoritesService, CancellationToken cancellationToken) =>
        {
            var favorite = await favoritesService.AddAsync(city, cancellationToken);
            return Results.Created($"/api/favorites/{favorite.Id}", favorite);
        })
        .WithName("AddFavorite");

        group.MapDelete("/{id:guid}", async (Guid id, FavoritesService favoritesService, CancellationToken cancellationToken) =>
        {
            var removed = await favoritesService.RemoveAsync(id, cancellationToken);
            return removed ? Results.NoContent() : Results.NotFound();
        })
        .WithName("RemoveFavorite");

        group.MapPut("/reorder", async (ReorderRequest request, FavoritesService favoritesService, CancellationToken cancellationToken) =>
        {
            try
            {
                await favoritesService.ReorderAsync(request.OrderedIds, cancellationToken);
                return Results.NoContent();
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        })
        .WithName("ReorderFavorites");
    }
}
