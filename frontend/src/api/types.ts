export interface City {
  name: string
  country: string
  latitude: number
  longitude: number
  timezone: string
}

export interface Favorite {
  id: string
  name: string
  country: string
  latitude: number
  longitude: number
  timezone: string
  order: number
}

export interface CurrentWeather {
  temperature: number
  todayHigh: number
  todayLow: number
  humidityPercent: number
  windSpeedKph: number
  weatherCode: number
  isDay: boolean
  timezone: string
  utcOffsetSeconds: number
}
