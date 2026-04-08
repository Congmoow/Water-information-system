import { readFileSync } from 'node:fs'
import path from 'node:path'
import { describe, expect, it } from 'vitest'

function readSource(relativePath: string) {
  return readFileSync(path.resolve(process.cwd(), relativePath), 'utf8')
}

function readStyleBlock(source: string) {
  const match = source.match(/<style[\s\S]*?>([\s\S]*?)<\/style>/)
  return match?.[1] ?? ''
}

function readPropsBlock(source: string) {
  const match = source.match(/defineProps<\{([\s\S]*?)\}>\(\)/)
  return match?.[1] ?? ''
}

describe('theme governance', () => {
  it('keeps login view colors routed through shared theme variables', () => {
    const source = readSource('src/views/auth/LoginView.vue')
    const styleBlock = readStyleBlock(source)

    expect(source).not.toMatch(/\sstyle="/)
    expect(styleBlock).not.toMatch(/--text-primary\s*:/)
    expect(styleBlock).not.toMatch(/--text-secondary\s*:/)
    expect(styleBlock).not.toMatch(/--border-color\s*:/)
    expect(styleBlock).not.toMatch(/--primary-color\s*:/)
    expect(styleBlock).not.toMatch(/#[0-9a-fA-F]{3,8}/)
    expect(styleBlock).not.toMatch(/rgba?\(/)
  })

  it('does not allow chart components to accept raw color props', () => {
    const barChart = readSource('src/components/charts/StatBarChart.vue')
    const donutChart = readSource('src/components/charts/StatDonutChart.vue')
    const trendChart = readSource('src/components/charts/TrendLineChart.vue')
    const barChartProps = readPropsBlock(barChart)
    const donutChartProps = readPropsBlock(donutChart)
    const trendChartProps = readPropsBlock(trendChart)

    expect(barChartProps).not.toMatch(/\bcolors\s*:/)
    expect(barChartProps).toMatch(/\bpalette\s*:/)
    expect(donutChartProps).not.toMatch(/\bcolors\s*:/)
    expect(donutChartProps).toMatch(/\bpalette\s*:/)
    expect(trendChartProps).not.toMatch(/\bcolor\s*:/)
    expect(trendChartProps).not.toMatch(/\bareaColor\s*:/)
    expect(trendChartProps).toMatch(/\bseriesType\s*:/)
  })

  it('does not let chart callers pass raw color props', () => {
    const dashboardView = readSource('src/views/dashboard/DashboardView.vue')
    const monitoringView = readSource('src/views/monitoring/MonitoringView.vue')

    expect(dashboardView).not.toContain(':colors=')
    expect(dashboardView).not.toContain(':color=')
    expect(dashboardView).not.toContain(':area-color=')
    expect(monitoringView).not.toContain(':color=')
    expect(monitoringView).not.toContain(':area-color=')
  })
})
