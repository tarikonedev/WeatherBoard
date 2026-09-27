using WeatherBoard.Application.Favorites;
using WeatherBoard.Domain.Entities;
using WeatherBoard.Infrastructure.Favorites;

namespace WeatherBoard.UnitTests;

public class FavoritesServiceTests
{
    private static FavoritesService CreateService() => new(new InMemoryFavoritesRepository());

    [Fact]
    public async Task GetAllAsync_ReturnsSeededKyivFavorite()
    {
        var service = CreateService();

        var favorites = await service.GetAllAsync();

        var kyiv = Assert.Single(favorites);
        Assert.Equal("Kyiv", kyiv.Name);
        Assert.Equal(0, kyiv.Order);
    }

    [Fact]
    public async Task AddAsync_AssignsNextOrderAfterExistingFavorites()
    {
        var service = CreateService();
        var london = new City("London", "United Kingdom", 51.5072, -0.1276, "Europe/London");

        var added = await service.AddAsync(london);

        Assert.Equal("London", added.Name);
        Assert.Equal(1, added.Order);

        var all = await service.GetAllAsync();
        Assert.Equal(2, all.Count);
    }

    [Fact]
    public async Task RemoveAsync_ReturnsFalse_WhenFavoriteDoesNotExist()
    {
        var service = CreateService();

        var removed = await service.RemoveAsync(Guid.NewGuid());

        Assert.False(removed);
    }

    [Fact]
    public async Task RemoveAsync_ReturnsTrue_AndRemovesFavorite()
    {
        var service = CreateService();
        var existing = (await service.GetAllAsync()).Single();

        var removed = await service.RemoveAsync(existing.Id);

        Assert.True(removed);
        Assert.Empty(await service.GetAllAsync());
    }

    [Fact]
    public async Task ReorderAsync_UpdatesOrder_WhenIdSetMatches()
    {
        var service = CreateService();
        var london = await service.AddAsync(new City("London", "United Kingdom", 51.5072, -0.1276, "Europe/London"));
        var kyiv = (await service.GetAllAsync()).Single(f => f.Name == "Kyiv");

        await service.ReorderAsync([london.Id, kyiv.Id]);

        var reordered = await service.GetAllAsync();
        Assert.Equal(london.Id, reordered[0].Id);
        Assert.Equal(kyiv.Id, reordered[1].Id);
    }

    [Fact]
    public async Task ReorderAsync_Throws_WhenIdSetDoesNotMatchExistingFavorites()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<ArgumentException>(() => service.ReorderAsync([Guid.NewGuid()]));
    }
}
