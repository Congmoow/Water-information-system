<template>
  <aside class="sidebar">
    <div class="sidebar__inner">
      <div class="brand">
        <div class="brand__mark">
          <img :src="brandIcon" alt="水利信息系统图标" class="brand__icon" />
        </div>
        <div>
          <strong>水利信息系统</strong>
          <span>Hydrology Control Room</span>
        </div>
        <button
          type="button"
          class="sidebar__collapse-btn"
          :title="appStore.collapsed ? '展开侧栏' : '收起侧栏'"
          @click="appStore.toggleCollapsed()"
        >
          <el-icon><Fold /></el-icon>
        </button>
      </div>

      <div class="sidebar__menus">
        <p class="sidebar__section">业务导航</p>

        <nav class="sidebar__nav">
          <router-link
            v-for="item in appMenus"
            :key="item.path"
            :to="item.path"
            class="nav-item"
          >
            <el-icon><component :is="icons[item.icon]" /></el-icon>
            <span>{{ item.title }}</span>
          </router-link>
        </nav>
      </div>

      <div ref="settingsRef" class="sidebar__settings">
        <transition name="settings-popover">
          <div
            v-if="isSettingsOpen"
            class="settings-popover"
            role="menu"
            aria-label="设置菜单"
          >
            <div
              v-for="group in settingsMenuGroups"
              :key="group.id"
              class="settings-popover__group"
            >
              <template v-for="item in group.items" :key="item.name">
                <router-link
                  v-if="item.path"
                  :to="item.path"
                  class="settings-popover__item"
                  @click="handleNavigate"
                >
                  <span class="settings-popover__item-main">
                    <el-icon><component :is="icons[item.icon]" /></el-icon>
                    <span>{{ item.title }}</span>
                  </span>
                </router-link>
                <button
                  v-else
                  type="button"
                  class="settings-popover__item settings-popover__item--action"
                  @click="handleAction(item.action)"
                >
                  <span class="settings-popover__item-main">
                    <el-icon><component :is="icons[item.icon]" /></el-icon>
                    <span>{{ item.title }}</span>
                  </span>
                </button>
              </template>
            </div>
          </div>
        </transition>

        <button
          type="button"
          class="nav-item nav-item--trigger"
          :class="{ 'is-open': isSettingsOpen, 'is-active': isSettingsRoute }"
          :aria-expanded="isSettingsOpen"
          aria-haspopup="menu"
          @click="toggleSettings"
        >
          <span class="nav-item__content">
            <el-icon><Setting /></el-icon>
            <span>设置</span>
          </span>
          <el-icon class="nav-item__arrow">
            <ArrowDown />
          </el-icon>
        </button>
      </div>
    </div>
  </aside>
</template>

<script setup lang="ts">
import type { Component } from 'vue'
import { computed, onBeforeUnmount, onMounted, ref } from 'vue'
import { ElMessage } from 'element-plus'
import brandIcon from '@/assets/zjw.png'
import {
  ArrowDown,
  Bell,
  Collection,
  DataAnalysis,
  Fold,
  Guide,
  Histogram,
  Location,
  MapLocation,
  Setting,
  SwitchButton,
  User
} from '@element-plus/icons-vue'
import { useRoute, useRouter } from 'vue-router'
import { appMenus, settingsMenuGroups } from '@/constants/menu'
import { useAuthStore } from '@/stores/auth'
import { useAppStore } from '@/stores/app'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()
const appStore = useAppStore()
const settingsRef = ref<HTMLElement | null>(null)
const settingsExpanded = ref(route.name === 'userCenter')

const icons: Record<string, Component> = {
  Bell,
  Collection,
  DataAnalysis,
  Fold,
  Guide,
  Histogram,
  Location,
  MapLocation,
  Setting,
  SwitchButton,
  User
}

const isSettingsRoute = computed(() => route.name === 'userCenter')
const isSettingsOpen = computed(() => settingsExpanded.value)

function toggleSettings() {
  settingsExpanded.value = !settingsExpanded.value
}

function closeSettings() {
  settingsExpanded.value = false
}

function handleNavigate() {
  closeSettings()
}

function handleDocumentPointerDown(event: PointerEvent) {
  if (!isSettingsOpen.value || !settingsRef.value) {
    return
  }

  const target = event.target
  if (target instanceof Node && !settingsRef.value.contains(target)) {
    closeSettings()
  }
}

function handleDocumentKeydown(event: KeyboardEvent) {
  if (event.key === 'Escape') {
    closeSettings()
  }
}

async function handleAction(action?: 'logout') {
  if (action !== 'logout') {
    return
  }

  closeSettings()
  await authStore.logout()
  ElMessage.success('已退出登录')
  await router.push('/login')
}

onMounted(() => {
  document.addEventListener('pointerdown', handleDocumentPointerDown)
  document.addEventListener('keydown', handleDocumentKeydown)
})

onBeforeUnmount(() => {
  document.removeEventListener('pointerdown', handleDocumentPointerDown)
  document.removeEventListener('keydown', handleDocumentKeydown)
})
</script>

<style scoped lang="scss">
.sidebar {
  height: 100vh;
  padding: 20px 16px;
  overflow: visible;
  background:
    radial-gradient(ellipse 120% 90% at 0% 0%, rgba(47, 125, 193, 0.14) 0%, transparent 55%),
    radial-gradient(ellipse 80% 60% at 100% 100%, rgba(14, 30, 56, 0.45) 0%, transparent 50%),
    var(--wi-app-sidebar-bg);
  color: var(--wi-text-inverse-primary);
  border-right: 1px solid var(--wi-app-sidebar-border);
  box-shadow: inset -1px 0 0 rgba(255, 255, 255, 0.04);
}

.sidebar__inner {
  display: flex;
  flex-direction: column;
  height: 100%;
  min-height: 0;
}

.sidebar__menus {
  display: flex;
  flex: 1;
  flex-direction: column;
  min-height: 0;
  overflow: hidden;
}

.brand {
  display: flex;
  align-items: center;
  gap: 14px;
  padding: 12px 12px 20px;
  margin: 0 2px 4px;
  border-radius: var(--wi-app-radius-md, 14px) var(--wi-app-radius-md, 14px) 0 0;
  border-bottom: 1px solid var(--wi-app-sidebar-border);
  background: linear-gradient(180deg, rgba(247, 251, 253, 0.07) 0%, transparent 92%);

  strong {
    display: block;
    font-size: 16px;
    line-height: 1.4;
    font-weight: 700;
    letter-spacing: 0.02em;
  }

  span {
    display: block;
    margin-top: 4px;
    color: var(--wi-app-sidebar-text-secondary);
    font-size: 11px;
    letter-spacing: 0.1em;
    text-transform: uppercase;
  }
}

.brand__mark {
  display: flex;
  flex: 0 0 auto;
  align-items: center;
  justify-content: center;
}

.brand__icon {
  display: block;
  width: 60px;
  height: 60px;
  object-fit: contain;
}

.sidebar__collapse-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  margin-left: auto;
  width: 32px;
  height: 32px;
  padding: 0;
  border: 1px solid transparent;
  border-radius: 8px;
  background: transparent;
  color: var(--wi-app-sidebar-text-secondary);
  cursor: pointer;
  transition:
    background-color 0.18s ease,
    color 0.18s ease,
    border-color 0.18s ease;

  &:hover {
    background: var(--wi-app-sidebar-surface-hover);
    color: var(--wi-text-inverse-primary);
    border-color: var(--wi-app-sidebar-border);
  }
}

.sidebar__section {
  margin: 18px 12px 12px;
  color: var(--wi-app-sidebar-text-secondary);
  font-size: 12px;
  letter-spacing: 0.08em;
  text-transform: uppercase;
}

.sidebar__nav {
  display: flex;
  flex: 1;
  flex-direction: column;
  gap: 8px;
  min-height: 0;
  overflow-y: auto;
  padding-right: 4px;
}

.sidebar__settings {
  position: relative;
  margin-top: auto;
  padding-top: 18px;
  border-top: 1px solid var(--wi-app-sidebar-border);
}

.nav-item {
  position: relative;
  display: flex;
  align-items: center;
  gap: 12px;
  width: 100%;
  padding: 12px 14px 12px 16px;
  border: 0;
  border-radius: var(--wi-app-radius-sm, 10px);
  background: transparent;
  color: var(--wi-app-sidebar-text-secondary);
  text-align: left;
  cursor: pointer;
  transition:
    background-color 0.18s ease,
    color 0.18s ease,
    box-shadow 0.18s ease,
    border-color 0.18s ease;

  &::before {
    content: '';
    position: absolute;
    top: 10px;
    bottom: 10px;
    left: 8px;
    width: 3px;
    border-radius: 999px;
    background: transparent;
  }

  &.router-link-active,
  &.is-active {
    background: var(--wi-app-sidebar-surface-active);
    color: var(--wi-text-inverse-primary);
    box-shadow: inset 0 0 0 1px var(--wi-app-sidebar-border);

    &::before {
      background: var(--wi-primary);
    }
  }

  &:hover {
    background: var(--wi-app-sidebar-surface-hover);
    color: var(--wi-text-inverse-primary);
  }
}

.nav-item--trigger {
  justify-content: space-between;
  font-size: 15px;
  font-weight: 600;
  line-height: 1.2;
}

.nav-item__content {
  display: inline-flex;
  align-items: center;
  gap: 12px;
}

.nav-item__arrow {
  transition: transform 0.18s ease;
}

.nav-item--trigger.is-open .nav-item__arrow {
  transform: rotate(180deg);
}

.settings-popover {
  position: absolute;
  right: 0;
  bottom: calc(100% - 6px);
  left: 42px;
  z-index: 10;
  display: grid;
  gap: 6px;
  padding: 8px 8px 10px;
  border: 1px solid var(--wi-app-sidebar-border);
  border-radius: 16px 16px 20px 20px;
  background: color-mix(in srgb, var(--wi-app-sidebar-bg) 88%, var(--wi-app-sidebar-surface) 12%);
  box-shadow: var(--wi-app-shadow-md);
  backdrop-filter: blur(8px);
}

.settings-popover__group {
  display: grid;
  gap: 2px;
}

.settings-popover__group + .settings-popover__group {
  margin-top: 4px;
  padding-top: 6px;
  border-top: 1px solid var(--wi-app-sidebar-border);
}

.settings-popover__item {
  display: flex;
  align-items: center;
  width: 100%;
  min-height: 38px;
  padding: 0 10px;
  border: 1px solid transparent;
  border-radius: 10px;
  background: transparent;
  color: var(--wi-text-inverse-primary);
  font-size: 13px;
  font-weight: 500;
  cursor: pointer;
  transition:
    background-color 0.18s ease,
    border-color 0.18s ease,
    box-shadow 0.18s ease,
    transform 0.18s ease;

  &:hover {
    background: var(--wi-app-sidebar-surface-hover);
    border-color: var(--wi-app-sidebar-border);
  }

  &.router-link-active {
    background: var(--wi-app-sidebar-surface-active);
    border-color: var(--wi-app-sidebar-border);
    box-shadow: var(--wi-app-shadow-sm);
  }
}

.settings-popover__item-main {
  display: inline-flex;
  align-items: center;
  gap: 8px;
}

.settings-popover__item--action {
  font-size: 13px;
  font-weight: 500;
  line-height: 1.2;
}

.settings-popover-enter-active,
.settings-popover-leave-active {
  transition:
    opacity 0.18s ease,
    transform 0.18s ease;
  transform-origin: bottom center;
}

.settings-popover-enter-from,
.settings-popover-leave-to {
  opacity: 0;
  transform: translateY(8px) scale(0.98);
}

@media (max-width: 1080px) {
  .sidebar {
    height: auto;
    padding: 18px;
    border-right: none;
    border-bottom: 1px solid var(--wi-app-sidebar-border);
  }

  .sidebar__inner,
  .sidebar__menus,
  .sidebar__nav {
    min-height: auto;
    height: auto;
  }

  .sidebar__nav {
    overflow: visible;
  }

  .settings-popover {
    position: static;
    margin-left: 12px;
    margin-bottom: 10px;
  }
}
</style>
