<template>
  <aside class="sidebar">
    <div class="brand">
      <div class="brand__mark">水</div>
      <div>
        <strong>水利信息管理系统</strong>
        <span>Hydrology Control Room</span>
      </div>
    </div>

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
  padding: 24px 18px;
  background:
    linear-gradient(180deg, var(--wi-surface-inverse-strong), var(--wi-surface-inverse)),
    radial-gradient(circle at top, var(--wi-glow-brand), transparent 26%);
  color: var(--wi-text-inverse-primary);
  border-right: 1px solid var(--wi-border-inverse);
}

.brand {
  display: flex;
  align-items: center;
  gap: 14px;
  padding: 10px 12px 22px;

  strong {
    display: block;
    font-size: 17px;
  }

  span {
    display: block;
    margin-top: 4px;
    color: var(--wi-text-inverse-secondary);
    font-size: 12px;
    letter-spacing: 0.08em;
    text-transform: uppercase;
  }
}

.brand__mark {
  width: 46px;
  height: 46px;
  border-radius: 16px;
  display: grid;
  place-items: center;
  position: relative;
  background: linear-gradient(135deg, var(--wi-primary-hover), var(--wi-primary-active));
  box-shadow: var(--wi-shadow-inverse-strong);
  font-size: 22px;
  font-weight: 800;

  &::after {
    content: '';
    position: absolute;
    top: 6px;
    right: 6px;
    width: 10px;
    height: 10px;
    border-radius: 999px;
    background: var(--wi-accent);
    box-shadow: var(--wi-ring-accent);
  }
}

.sidebar__nav {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.nav-item {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 14px 16px;
  border-radius: 18px;
  color: var(--wi-text-inverse-secondary);
  transition:
    background-color 0.2s ease,
    color 0.2s ease,
    transform 0.2s ease,
    box-shadow 0.2s ease;

  &.router-link-active {
    background: var(--wi-surface-inverse-muted);
    color: var(--wi-text-inverse-primary);
    box-shadow: inset 0 0 0 1px var(--wi-border-inverse);
  }

  &:hover {
    transform: translateX(3px);
    background: var(--wi-surface-inverse-soft);
    color: var(--wi-text-inverse-primary);
  }
}
</style>
