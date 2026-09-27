namespace WeatherBoard.Domain.Entities;

public record City(
    string Name,
    string Country,
    double Latitude,
    double Longitude,
    string Timezone);
