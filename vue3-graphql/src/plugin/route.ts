import { createWebHistory, createRouter } from "vue-router"

import home from '../components/home.vue'
import simpleQuery from '../components/simpleQuery.vue'
import fileDefineQuery from '../components/fileDefineQuery/fileDefineQuery.vue'
import inComponentQuery from '../components/inComponentQuery.vue';
import fragmentParentQuery from '../components/fragmentQuery/fragmentParent.vue';


const routes = [
    { path: '/', component: home },
    { path: '/simple', component: simpleQuery },
    { path: '/fileDefine', component: fileDefineQuery },
    { path: '/inComponent', component: inComponentQuery },
    { path: '/fragment', component: fragmentParentQuery },
]

export const routePlugin = createRouter({
    history: createWebHistory(),
    routes
})