using WeatherBoard.Domain.Entities;

namespace WeatherBoard.Infrastructure.Geocoding;

public static class MockCityCatalog
{
    public static readonly IReadOnlyList<City> Cities =
    [
        new City("Kyiv", "Ukraine", 50.4501, 30.5234, "Europe/Kyiv"),
        new City("London", "United Kingdom", 51.5072, -0.1276, "Europe/London"),
        new City("New York", "United States", 40.7128, -74.0060, "America/New_York"),
        new City("Tokyo", "Japan", 35.6762, 139.6503, "Asia/Tokyo"),
        new City("Sydney", "Australia", -33.8688, 151.2093, "Australia/Sydney"),
        new City("Paris", "France", 48.8566, 2.3522, "Europe/Paris"),
        new City("Cairo", "Egypt", 30.0444, 31.2357, "Africa/Cairo"),
        new City("Rio de Janeiro", "Brazil", -22.9068, -43.1729, "America/Sao_Paulo"),
        new City("Reykjavik", "Iceland", 64.1466, -21.9426, "Atlantic/Reykjavik"),
        new City("Nairobi", "Kenya", -1.2921, 36.8219, "Africa/Nairobi"),
    ];
}
