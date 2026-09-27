using WeatherBoard.Application.Interfaces;
using WeatherBoard.Domain.Entities;

namespace WeatherBoard.Infrastructure.Favorites;

public class InMemoryFavoritesRepository : IFavoritesRepository
{
    private readonly object _lock = new();
    private readonly List<Favorite> _favorites;

    public InMemoryFavoritesRepository()
    {
        _favorites =
        [
            new Favorite(Guid.NewGuid(), "Kyiv", "Ukraine", 50.4501, 30.5234, "Europe/Kyiv", 0)
        ];
    }

    public Task<IReadOnlyList<Favorite>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            IReadOnlyList<Favorite> snapshot = _favorites.OrderBy(f => f.Order).ToList();
            return Task.FromResult(snapshot);
        }
    }

    public Task AddAsync(Favorite favorite, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _favorites.Add(favorite);
        }

        return Task.CompletedTask;
    }

    public Task<bool> RemoveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var removed = _favorites.RemoveAll(f => f.Id == id) > 0;
            return Task.FromResult(removed);
        }
    }

    public Task ReorderAsync(IReadOnlyList<Guid> orderedIds, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            for (var index = 0; index < orderedIds.Count; index++)
            {
                var id = orderedIds[index];
                var existingIndex = _favorites.FindIndex(f => f.Id == id);
                if (existingIndex >= 0)
                {
                    _favorites[existingIndex] = _favorites[existingIndex] with { Order = index };
                }
            }
        }

        return Task.CompletedTask;
    }
}
