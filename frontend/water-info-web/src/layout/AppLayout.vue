<template>
  <div class="layout-shell">
    <SidebarNav />
    <div class="layout-main">
      <AppHeader />
      <main class="layout-content">
        <div class="layout-content__inner">
          <router-view v-slot="{ Component }">
            <transition name="page" mode="out-in">
              <component :is="Component" />
            </transition>
          </router-view>
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
  height: 100vh;
  min-height: 100vh;
  overflow: hidden;
}

.layout-main {
  display: flex;
  flex-direction: column;
  min-width: 0;
  min-height: 0;
  height: 100vh;
  overflow: hidden;
}

.layout-content {
  flex: 1;
  min-height: 0;
  padding: 0 var(--wi-app-page-padding-inline) var(--wi-app-page-padding-block);
  overflow-x: hidden;
  overflow-y: auto;
}

.layout-content__inner {
  width: 100%;
  min-height: 100%;
  max-width: var(--wi-app-content-max-width);
  margin: 0 auto;
  padding-top: var(--wi-space-4, 16px);
  padding-bottom: var(--wi-space-2, 8px);
}

@media (max-width: 1080px) {
  .layout-shell {
    grid-template-columns: 1fr;
    grid-template-rows: auto minmax(0, 1fr);
  }

  .layout-main {
    height: auto;
  }

  .layout-content {
    padding-inline: 18px;
    padding-bottom: 18px;
  }
}
</style>
