import { defineStore } from 'pinia'
import api from '@/services/api'

export const useStreetsStore = defineStore('streets', {
  state: () => ({
    streets: [],
    isLoading: false,
    cachedStreets: {},
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

    async createSchedule(id, weekOfMonth, dayOfWeek, year, sideofStreet) {
      try {
        const payload = {
          weekOfMonth: weekOfMonth,
          dayOfWeek: dayOfWeek,
          year: year,
          sideofStreet: sideofStreet,
        }
        const reponse = await api.post(`Street/${id}/schedule`, payload)
        if (reponse.data > 0) {
          this.streetId = id
          return reponse.data
        } else {
          return null
        }
      } catch (error) {
        console.log('Failed to create schedule' + error)
      }
    },

    async getSchedule(id) {
      if (!id) {
        return null
      }
      try {
        const response = await api.get(`Street/${id}/getSchedule`)
        if (response.data) {
          return response.data
        } else {
          return null
        }
      } catch (error) {
        console.error('Failed to get schedule' + error)
        return null
      }
    },

    async getStreet(id) {
      if (!id) {
        console.warn('getStreet called with invalid ID:', id)
        return null
      }
      const key = String(id)
      if (this.cachedStreets[key] !== undefined) {
        return this.cachedStreets[key]
      }
      try {
        const response = await api.get(`Street/${id}`)
        if (response.data) {
          this.cachedStreets[key] = response.data
          return response.data
        } else {
          console.warn(`No data returned for street ID: ${key}`)
          return null
        }
      } catch (error) {
        console.error(`Failed to get street ${key}:`, error)
        return null
      }
    },

    clearStreets() {
      this.streets = []
      this.isLoading = false
    },
  },

  persist: {
    paths: ['streets', 'isLoading'],
  },
})
