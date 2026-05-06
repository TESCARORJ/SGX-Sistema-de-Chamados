import { createApp } from 'vue'
import { createPinia } from 'pinia'
import { Quasar } from 'quasar'
import router from './router'
import App from './App.vue'

import 'quasar/src/css/index.sass'
import '@quasar/extras/material-icons/material-icons.css'
import './style.css'

createApp(App)
  .use(createPinia())
  .use(router)
  .use(Quasar, {
    config: {
      brand: {
        primary: '#0f766e',
        secondary: '#f97316',
        accent: '#0369a1',
      },
    },
  })
  .mount('#app')
