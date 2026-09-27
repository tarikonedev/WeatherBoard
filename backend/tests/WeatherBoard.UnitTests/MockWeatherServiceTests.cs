using WeatherBoard.Infrastructure.Weather;

namespace WeatherBoard.UnitTests;

public class MockWeatherServiceTests
{
    private readonly MockWeatherService _service = new();

    [Fact]
    public async Task GetCurrentWeatherAsync_ReturnsPreset_ForKnownCityCoordinates()
    {
        var weather = await _service.GetCurrentWeatherAsync(50.4501, 30.5234);

        Assert.Equal("Europe/Kyiv", weather.Timezone);
        Assert.Equal(3, weather.WeatherCode);
    }

    [Fact]
    public async Task GetCurrentWeatherAsync_ReturnsDefault_ForUnknownCoordinates()
    {
        var weather = await _service.GetCurrentWeatherAsync(10.0, 10.0);

        Assert.Equal(MockWeatherCatalog.Default, weather);
    }
}
