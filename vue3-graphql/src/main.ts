import { createApp } from 'vue'
import './style.css'
import App from './App.vue'
import { apolloPlugin } from './plugin/apollo'
import { routePlugin } from './plugin/route'

const app = createApp(App)
app.use(routePlugin);
app.use(apolloPlugin);


app.mount('#app')
