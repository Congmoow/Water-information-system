<template>
  <div class="layout-shell">
    <SidebarNav />
    <div class="layout-main">
      <AppHeader />
      <main class="layout-content">
        <div class="layout-content__inner">
          <router-view />
        </div>
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
  display: grid;
  grid-template-columns: var(--wi-app-sidebar-width) minmax(0, 1fr);
}

.layout-main {
  min-width: 0;
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.layout-content {
  flex: 1;
  padding: 0 var(--wi-app-page-padding-inline) var(--wi-app-page-padding-block);
}

.layout-content__inner {
  width: 100%;
  max-width: var(--wi-app-content-max-width);
  margin: 0 auto;
}

@media (max-width: 1080px) {
  .layout-shell {
    grid-template-columns: 1fr;
  }

  .layout-content {
    padding-inline: 18px;
    padding-bottom: 18px;
  }
}
</style>
