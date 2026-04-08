<template>
  <div class="chart-card">
    <VChart class="chart-card__view" :option="option" autoresize />
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import VChart from 'vue-echarts'
import '@/components/charts/chartSetup'
import { chartSeries, visualizationTokens } from '@/theme/tokens'
import type { ChartSeriesPaletteKey } from '@/theme/tokens'
import type { CategoryCount } from '@/types/models'

const props = defineProps<{
  items: CategoryCount[]
  palette: ChartSeriesPaletteKey
}>()

const paletteColors = computed(() => chartSeries[props.palette])

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
    data: props.items.map((item) => item.name),
    axisLine: { lineStyle: { color: visualizationTokens.chart.axis } },
    axisLabel: { color: visualizationTokens.chart.text }
  },
  yAxis: {
    type: 'value',
    splitLine: { lineStyle: { color: visualizationTokens.chart.grid } },
    axisLabel: { color: visualizationTokens.chart.text }
  },
  series: [
    {
      type: 'bar',
      data: props.items.map((item, index) => ({
        value: item.value,
        itemStyle: {
          color: paletteColors.value[index % paletteColors.value.length],
          borderRadius: [12, 12, 0, 0]
        }
      })),
      barWidth: 36
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
