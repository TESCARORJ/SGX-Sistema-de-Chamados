import { createApp } from 'vue'
import { createPinia } from 'pinia'
import { Dialog, Loading, Notify, Quasar } from 'quasar'
import router from './router'
import App from './App.vue'

import 'quasar/src/css/index.sass'
import '@quasar/extras/material-icons/material-icons.css'
import './style.css'

createApp(App)
  .use(createPinia())
  .use(router)
  .use(Quasar, {
    plugins: {
      Dialog,
      Loading,
      Notify,
    },
    config: {
      brand: {
        primary: '#0b5ed7',
        secondary: '#062f66',
        accent: '#0284c7',
        dark: '#031b38',
      },
      notify: {
        position: 'top-right',
        timeout: 3000,
      },
      loading: {
        delay: 250,
      },
    },
  })
  .mount('#app')
