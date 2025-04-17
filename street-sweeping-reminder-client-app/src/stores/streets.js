import { defineStore } from 'pinia'
import api from '@/services/api'

export const useStreetsStore = defineStore('streets', {
  state: () => ({
    streets: [],
    isLoading: false,
  }),

  actions: {
    async searchStreets(query = '') {
      if (!query.trim()) {
        this.streets = []
        return
      }
      this.isLoading = true
      try {
        const encodedQuery = encodeURIComponent(query)
        const response = await api.get(`Street/search?query=${encodedQuery}`)

        if (response.data) {
          this.streets = response.data
        } else {
          this.streets = []
        }
      } catch (error) {
        console.error('Failed to get streets' + error)
        this.streets = []
      } finally {
        this.isLoading = false
      }
    },

    clearStreets() {
      this.streets = []
      this.isLoading = false
    },
  },
})
