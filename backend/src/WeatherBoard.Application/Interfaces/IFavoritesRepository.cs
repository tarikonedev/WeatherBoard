using WeatherBoard.Domain.Entities;

namespace WeatherBoard.Application.Interfaces;

public interface IFavoritesRepository
{
    Task<IReadOnlyList<Favorite>> GetAllAsync(CancellationToken cancellationToken = default);

    Task AddAsync(Favorite favorite, CancellationToken cancellationToken = default);

    Task<bool> RemoveAsync(Guid id, CancellationToken cancellationToken = default);

    Task ReorderAsync(IReadOnlyList<Guid> orderedIds, CancellationToken cancellationToken = default);
}
