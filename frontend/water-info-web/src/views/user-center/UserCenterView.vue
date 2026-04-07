<template>
  <PageCard title="用户中心" subtitle="下一阶段接入当前用户资料、角色说明和退出登录">
    <el-descriptions :column="1" border>
      <el-descriptions-item label="用户名">{{ authStore.user?.username ?? '--' }}</el-descriptions-item>
      <el-descriptions-item label="姓名">{{ authStore.user?.fullName ?? '--' }}</el-descriptions-item>
      <el-descriptions-item label="角色">{{ authStore.user?.role ?? '--' }}</el-descriptions-item>
      <el-descriptions-item label="创建时间">{{ authStore.user?.createdAt ?? '--' }}</el-descriptions-item>
    </el-descriptions>
  </PageCard>
</template>

<script setup lang="ts">
import PageCard from '@/components/common/PageCard.vue'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()
</script>
