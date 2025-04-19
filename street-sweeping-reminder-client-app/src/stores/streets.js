import { defineStore } from 'pinia'
import api from '@/services/api'

export const useStreetsStore = defineStore('streets', {
  state: () => ({
    streets: [],
    isLoading: false,
    schedule: [],
    streetId: null,
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

    async createSchedule(id, weekOfMonth, dayOfWeek, year) {
      try {
        const payload = {
          weekOfMonth: weekOfMonth,
          dayOfWeek: dayOfWeek,
          year: year,
        }
        const reponse = await api.post(`Street/${id}/schedule`, payload)
        if (reponse.data > 0) {
          return reponse.data
        } else {
          return null
        }
      } catch (error) {
        console.log('Failed to create schedule' + error)
      }
    },

    async getSchedule(id) {
      try {
        const response = await api.get(`Street/${id}/getSchedule`)
        if (response.data) {
          this.schedule = response.data
        } else {
          this.schedule = []
        }
      } catch (error) {
        console.error('Failed to get schedule' + error)
        this.schedule = []
      }
    },

    clearStreets() {
      this.streets = []
      this.isLoading = false
    },
  },
})
