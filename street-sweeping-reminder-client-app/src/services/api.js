import axios from 'axios'

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
})

console.log('API Base URL:', import.meta.env.VITE_API_BASE_URL)
// Add a request interceptor to include the JWT token in the headers
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('jwtToken')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  (error) => Promise.reject(error),
)

export default api
