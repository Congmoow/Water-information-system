<template>
  <el-tag :type="meta.type" effect="light" class="status-tag">
    {{ label ?? meta.label }}
  </el-tag>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { resolveStatusTagMeta, type StatusTagCategory } from '@/components/common/statusTag'

const props = defineProps<{
  category: StatusTagCategory
  value?: string
  label?: string
}>()

const meta = computed(() => resolveStatusTagMeta(props.category, props.value, props.label))
</script>

<style scoped lang="scss">
.status-tag {
  min-height: 28px;
  padding-inline: 10px;
  border-radius: 999px;
  font-weight: 600;
}
</style>
