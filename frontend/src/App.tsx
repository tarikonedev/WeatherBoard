import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { Dashboard } from './components/Dashboard/Dashboard'
import { FavoritesProvider } from './context/FavoritesContext'

const queryClient = new QueryClient()

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <FavoritesProvider>
        <Dashboard />
      </FavoritesProvider>
    </QueryClientProvider>
  )
}

export default App
