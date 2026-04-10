<template>
  <aside class="sidebar">
    <div class="sidebar__inner">
      <div class="brand">
        <div class="brand__mark">水</div>
        <div>
          <strong>水利信息系统</strong>
          <span>Hydrology Control Room</span>
        </div>
      </div>

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

      <div class="sidebar__footer">
        <strong>统一监测工作台</strong>
        <span>面向工程、站点、告警与地图展示的统一入口</span>
      </div>
    </div>
  </aside>
</template>

<script setup lang="ts">
import type { Component } from 'vue'
import {
  Bell,
  Collection,
  DataAnalysis,
  Guide,
  Histogram,
  Location,
  MapLocation,
  User
} from '@element-plus/icons-vue'
import { appMenus } from '@/constants/menu'

const icons: Record<string, Component> = {
  Bell,
  Collection,
  DataAnalysis,
  Guide,
  Histogram,
  Location,
  MapLocation,
  User
}
</script>

<style scoped lang="scss">
.sidebar {
  position: sticky;
  top: 0;
  min-height: 100vh;
  padding: 22px 18px;
  background: var(--wi-app-sidebar-bg);
  color: var(--wi-text-inverse-primary);
  border-right: 1px solid var(--wi-app-sidebar-border);
}

.sidebar__inner {
  display: flex;
  flex-direction: column;
  min-height: calc(100vh - 44px);
}

.brand {
  display: flex;
  align-items: center;
  gap: 14px;
  padding: 10px 12px 18px;
  border-bottom: 1px solid var(--wi-app-sidebar-border);

  strong {
    display: block;
    font-size: 16px;
    line-height: 1.4;
  }

  span {
    display: block;
    margin-top: 4px;
    color: var(--wi-app-sidebar-text-secondary);
    font-size: 12px;
    letter-spacing: 0.08em;
    text-transform: uppercase;
  }
}

.brand__mark {
  width: 42px;
  height: 42px;
  border-radius: 14px;
  display: grid;
  place-items: center;
  background: var(--wi-app-sidebar-surface);
  border: 1px solid var(--wi-app-sidebar-border);
  font-size: 20px;
  font-weight: 700;
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
  flex-direction: column;
  gap: 8px;
  flex: 1;
}

.nav-item {
  position: relative;
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 12px 14px 12px 16px;
  border-radius: 14px;
  color: var(--wi-app-sidebar-text-secondary);
  transition:
    background-color 0.2s ease,
    color 0.2s ease,
    box-shadow 0.2s ease;

  &::before {
    content: '';
    position: absolute;
    left: 8px;
    top: 10px;
    bottom: 10px;
    width: 3px;
    border-radius: 999px;
    background: transparent;
  }

  &.router-link-active {
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

.sidebar__footer {
  margin-top: 18px;
  padding: 16px;
  border-radius: 16px;
  background: var(--wi-app-sidebar-surface);
  border: 1px solid var(--wi-app-sidebar-border);

  strong,
  span {
    display: block;
  }

  strong {
    font-size: 13px;
    color: var(--wi-text-inverse-primary);
  }

  span {
    margin-top: 8px;
    color: var(--wi-app-sidebar-text-secondary);
    font-size: 12px;
    line-height: 1.7;
  }
}

@media (max-width: 1080px) {
  .sidebar {
    position: static;
    min-height: auto;
    padding: 18px;
    border-right: none;
    border-bottom: 1px solid var(--wi-app-sidebar-border);
  }

  .sidebar__inner {
    min-height: auto;
  }

  .sidebar__nav {
    flex-direction: row;
    overflow-x: auto;
    padding-bottom: 4px;
  }

  .nav-item {
    min-width: max-content;
  }
}
</style>
