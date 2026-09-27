using WeatherBoard.Application.Interfaces;
using WeatherBoard.Domain.Entities;

namespace WeatherBoard.Application.Favorites;

public class FavoritesService(IFavoritesRepository repository)
{
    public Task<IReadOnlyList<Favorite>> GetAllAsync(CancellationToken cancellationToken = default) =>
        repository.GetAllAsync(cancellationToken);

    public async Task<Favorite> AddAsync(City city, CancellationToken cancellationToken = default)
    {
        var existing = await repository.GetAllAsync(cancellationToken);
        var nextOrder = existing.Count == 0 ? 0 : existing.Max(f => f.Order) + 1;

        var favorite = new Favorite(
            Guid.NewGuid(),
            city.Name,
            city.Country,
            city.Latitude,
            city.Longitude,
            city.Timezone,
            nextOrder);

        await repository.AddAsync(favorite, cancellationToken);
        return favorite;
    }

    public Task<bool> RemoveAsync(Guid id, CancellationToken cancellationToken = default) =>
        repository.RemoveAsync(id, cancellationToken);

    public async Task ReorderAsync(IReadOnlyList<Guid> orderedIds, CancellationToken cancellationToken = default)
    {
        var existing = await repository.GetAllAsync(cancellationToken);
        var existingIds = existing.Select(f => f.Id).ToHashSet();

        if (orderedIds.Count != existingIds.Count || !existingIds.SetEquals(orderedIds))
        {
            throw new ArgumentException("Reorder request must contain exactly the current set of favorite ids.", nameof(orderedIds));
        }

        await repository.ReorderAsync(orderedIds, cancellationToken);
    }
}
