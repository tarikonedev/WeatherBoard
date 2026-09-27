import { apiFetch } from './client'
import type { CurrentWeather } from './types'

export async function getCurrentWeather(latitude: number, longitude: number): Promise<CurrentWeather> {
  return apiFetch<CurrentWeather>(`/api/weather/current?lat=${latitude}&lon=${longitude}`)
}
