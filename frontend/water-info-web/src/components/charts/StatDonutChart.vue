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
    trigger: 'item'
  },
  legend: {
    bottom: 0,
    textStyle: {
      color: visualizationTokens.chart.text
    }
  },
  series: [
    {
      type: 'pie',
      radius: ['54%', '74%'],
      center: ['50%', '44%'],
      label: {
        formatter: '{b}\n{c}',
        color: visualizationTokens.chart.text
      },
      data: props.items.map((item, index) => ({
        name: item.name,
        value: item.value,
        itemStyle: {
          color: paletteColors.value[index % paletteColors.value.length]
        }
      }))
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
