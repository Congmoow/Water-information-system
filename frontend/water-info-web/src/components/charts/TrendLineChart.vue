<template>
  <div class="chart-card">
    <VChart class="chart-card__view" :option="option" autoresize />
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import VChart from 'vue-echarts'
import '@/components/charts/chartSetup'
import { lineChartTokens, visualizationTokens } from '@/theme/tokens'
import type { LineChartSeriesType } from '@/theme/tokens'
import type { TrendPoint } from '@/types/models'

const props = defineProps<{
  points: TrendPoint[]
  seriesType: LineChartSeriesType
  unit: string
}>()

const seriesTokens = computed(() => lineChartTokens[props.seriesType])

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
    axisLine: { lineStyle: { color: visualizationTokens.chart.axis } },
    axisLabel: { color: visualizationTokens.chart.text }
  },
  yAxis: {
    type: 'value',
    axisLine: { show: false },
    splitLine: { lineStyle: { color: visualizationTokens.chart.grid } },
    axisLabel: {
      color: visualizationTokens.chart.text,
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
        color: seriesTokens.value.line,
        width: 3
      },
      areaStyle: {
        color: seriesTokens.value.area
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
