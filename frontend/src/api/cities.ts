import { apiFetch } from './client';
import type { City } from './types';

export async function searchCities(query: string): Promise<City[]> {
  return apiFetch<City[]>(`/api/cities/search?q=${encodeURIComponent(query)}`);
}
