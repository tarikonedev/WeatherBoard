import { apiFetch } from './client';
import type { City, Favorite } from './types';

export async function getFavorites(): Promise<Favorite[]> {
  return apiFetch<Favorite[]>('/api/favorites');
}

export async function addFavorite(city: City): Promise<Favorite> {
  return apiFetch<Favorite>('/api/favorites', { method: 'POST', body: JSON.stringify(city) });
}

export async function removeFavorite(id: string): Promise<void> {
  return apiFetch<void>(`/api/favorites/${id}`, { method: 'DELETE' });
}

export async function reorderFavorites(orderedIds: string[]): Promise<void> {
  return apiFetch<void>('/api/favorites/reorder', {
    method: 'PUT',
    body: JSON.stringify({ orderedIds }),
  });
}
