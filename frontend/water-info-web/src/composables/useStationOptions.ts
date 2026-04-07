import { ref } from 'vue'
import { fetchStations } from '@/api/modules/station'
import type { StationItem } from '@/types/models'

export function useStationOptions() {
  const stations = ref<StationItem[]>([])
  const loading = ref(false)

  async function loadStations() {
    loading.value = true
    try {
      const result = await fetchStations({ page: 1, pageSize: 100 })
      stations.value = result.items
    } finally {
      loading.value = false
    }
  }

  return {
    stations,
    loading,
    loadStations
  }
}
