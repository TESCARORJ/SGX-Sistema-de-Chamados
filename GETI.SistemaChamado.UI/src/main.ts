import { createApp } from 'vue';
import { Quasar } from 'quasar';
import App from './App.vue';
import { router } from './router';

import 'quasar/src/css/index.sass';
import './styles/app.css';

createApp(App)
  .use(Quasar, {
    config: {
      brand: {
        primary: '#005A9C',
        secondary: '#1F8A70',
        accent: '#F39C12'
      }
    }
  })
  .use(router)
  .mount('#app');
