<template>
  <div class="chart-card">
    <VChart class="chart-card__view" :option="option" autoresize />
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import VChart from 'vue-echarts'
import '@/components/charts/chartSetup'
import type { TrendPoint } from '@/types/models'

const props = defineProps<{
  points: TrendPoint[]
  color: string
  areaColor: string
  unit: string
}>()

const option = computed(() => ({
  tooltip: {
    trigger: 'axis'
  },
  grid: {
    left: 16,
    right: 16,
    top: 24,
    bottom: 8,
    containLabel: true
  },
  xAxis: {
    type: 'category',
    boundaryGap: false,
    data: props.points.map((item) => item.label),
    axisLine: { lineStyle: { color: 'rgba(23, 50, 77, 0.14)' } },
    axisLabel: { color: '#5c738d' }
  },
  yAxis: {
    type: 'value',
    axisLine: { show: false },
    splitLine: { lineStyle: { color: 'rgba(23, 50, 77, 0.08)' } },
    axisLabel: {
      color: '#5c738d',
      formatter: `{value}${props.unit}`
    }
  },
  series: [
    {
      type: 'line',
      smooth: true,
      showSymbol: false,
      data: props.points.map((item) => item.value),
      lineStyle: {
        color: props.color,
        width: 3
      },
      areaStyle: {
        color: props.areaColor
      }
    }
  ]
}))
</script>

<style scoped lang="scss">
.chart-card {
  min-height: 280px;
}

.chart-card__view {
  height: 280px;
}
</style>
