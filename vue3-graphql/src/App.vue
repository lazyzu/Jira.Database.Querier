<script setup lang="ts">

import { computed } from 'vue';
import { useRoute } from 'vue-router';

const route = useRoute();
const isHome = computed(() => route.path === '/');

</script>

<template>
  <router-view />
  <div>
    <router-link v-if="!isHome" to="/">Home</router-link>
  </div>
</template>
