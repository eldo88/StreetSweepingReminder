import './assets/main.css'
import '@fortawesome/fontawesome-free/css/all.css'

import { createApp } from 'vue'
import { createPinia } from 'pinia'
import piniaPluginPersistedstate from 'pinia-plugin-persistedstate'

import App from './App.vue'
import router from './router'
import apiPlugin from './plugins/apiPlugin'

import Toast from 'vue-toastification'
import 'vue-toastification/dist/index.css'

import VCalendar from 'v-calendar'
import 'v-calendar/style.css'

const app = createApp(App)

app.use(Toast, {
  position: 'top-center',
  timeout: 3000,
  closeOnClick: true,
  pauseOnHover: true,
  draggable: true,
})

const pinia = createPinia()
pinia.use(piniaPluginPersistedstate)

app.use(pinia)
app.use(router)
app.use(apiPlugin)
app.use(VCalendar, {})

app.mount('#app')
