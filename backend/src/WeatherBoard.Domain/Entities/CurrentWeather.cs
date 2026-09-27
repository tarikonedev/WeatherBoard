namespace WeatherBoard.Domain.Entities;

public record CurrentWeather(
    double Temperature,
    double TodayHigh,
    double TodayLow,
    int HumidityPercent,
    double WindSpeedKph,
    int WeatherCode,
    bool IsDay,
    string Timezone,
    int UtcOffsetSeconds);
