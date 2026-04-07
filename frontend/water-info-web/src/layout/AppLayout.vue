<template>
  <div class="layout-shell">
    <SidebarNav />
    <div class="layout-main">
      <AppHeader />
      <main class="layout-content">
        <router-view />
      </main>
    </div>
  </div>
</template>

<script setup lang="ts">
import AppHeader from '@/layout/components/AppHeader.vue'
import SidebarNav from '@/layout/components/SidebarNav.vue'
</script>

<style scoped lang="scss">
.layout-shell {
  min-height: 100vh;
  display: grid;
  grid-template-columns: 268px 1fr;
}

.layout-main {
  min-width: 0;
}

.layout-content {
  padding: 24px 26px 28px;
}

@media (max-width: 1080px) {
  .layout-shell {
    grid-template-columns: 1fr;
  }
}
</style>
