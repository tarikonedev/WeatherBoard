namespace WeatherBoard.Domain.Entities;

public record Favorite(
    Guid Id,
    string Name,
    string Country,
    double Latitude,
    double Longitude,
    string Timezone,
    int Order);
