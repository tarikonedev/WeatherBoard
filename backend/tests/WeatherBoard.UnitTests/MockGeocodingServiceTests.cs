using WeatherBoard.Infrastructure.Geocoding;

namespace WeatherBoard.UnitTests;

public class MockGeocodingServiceTests
{
    private readonly MockGeocodingService _service = new();

    [Fact]
    public async Task SearchCitiesAsync_MatchesByNameCaseInsensitively()
    {
        var results = await _service.SearchCitiesAsync("kyiv");

        var city = Assert.Single(results);
        Assert.Equal("Kyiv", city.Name);
    }

    [Fact]
    public async Task SearchCitiesAsync_MatchesByCountry()
    {
        var results = await _service.SearchCitiesAsync("France");

        var city = Assert.Single(results);
        Assert.Equal("Paris", city.Name);
    }

    [Fact]
    public async Task SearchCitiesAsync_ReturnsEmpty_ForBlankQuery()
    {
        var results = await _service.SearchCitiesAsync("   ");

        Assert.Empty(results);
    }

    [Fact]
    public async Task SearchCitiesAsync_ReturnsEmpty_WhenNoMatch()
    {
        var results = await _service.SearchCitiesAsync("Atlantis");

        Assert.Empty(results);
    }
}
