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
    linear-gradient(180deg, rgba(8, 52, 72, 0.98), rgba(8, 42, 58, 0.98)),
    radial-gradient(circle at top, rgba(201, 138, 61, 0.22), transparent 28%);
  color: #f5fbff;
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
    color: rgba(245, 251, 255, 0.72);
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
  background: linear-gradient(135deg, rgba(201, 138, 61, 0.95), rgba(15, 108, 123, 0.95));
  font-size: 22px;
  font-weight: 800;
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
  color: rgba(245, 251, 255, 0.86);
  transition: all 0.2s ease;

  &.router-link-active {
    background: rgba(255, 255, 255, 0.14);
    color: #fff;
    box-shadow: inset 0 0 0 1px rgba(255, 255, 255, 0.1);
  }

  &:hover {
    transform: translateX(4px);
  }
}
</style>
