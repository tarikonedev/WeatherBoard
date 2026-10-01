import { createContext, useContext, type ReactNode } from 'react';
import type { Favorite } from '../api/types';

interface FavoritesContextValue {
  favorites: Favorite[];
}

const FavoritesContext = createContext<FavoritesContextValue | undefined>(undefined);

export function FavoritesProvider({ children }: { children: ReactNode }) {
  const value: FavoritesContextValue = { favorites: [] };
  return <FavoritesContext.Provider value={value}>{children}</FavoritesContext.Provider>;
}

export function useFavorites(): FavoritesContextValue {
  const context = useContext(FavoritesContext);
  if (!context) {
    throw new Error('useFavorites must be used within a FavoritesProvider');
  }
  return context;
}
