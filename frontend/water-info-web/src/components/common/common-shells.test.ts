import { createSSRApp, h } from 'vue'
import { renderToString } from 'vue/server-renderer'
import { describe, expect, it } from 'vitest'
import ChartSection from '@/components/common/ChartSection.vue'
import FilterBar from '@/components/common/FilterBar.vue'
import MetricCard from '@/components/common/MetricCard.vue'
import PageHeader from '@/components/common/PageHeader.vue'
import SideInfoPanel from '@/components/common/SideInfoPanel.vue'
import TableSection from '@/components/common/TableSection.vue'

async function render(component: unknown, props?: Record<string, unknown>, slots?: Record<string, () => unknown>) {
  const app = createSSRApp({
    render: () => h(component as never, props ?? {}, slots ?? {})
  })

  return renderToString(app)
}

describe('common logged-in shells', () => {
  it('renders page header breadcrumbs, title, description, and actions slot', async () => {
    const html = await render(
      PageHeader,
      {
        title: '告警记录',
        description: '聚焦预警分级、处置进度和告警闭环。',
        breadcrumbs: [
          { label: '系统工作台' },
          { label: '告警记录' }
        ]
      },
      {
        actions: () => h('button', '新增')
      }
    )

    expect(html).toContain('系统工作台')
    expect(html).toContain('告警记录')
    expect(html).toContain('聚焦预警分级、处置进度和告警闭环。')
    expect(html).toContain('新增')
  })

  it('renders filter bar fields and actions in separate regions', async () => {
    const html = await render(
      FilterBar,
      undefined,
      {
        default: () => [h('input', { placeholder: '关键词' }), h('select')],
        actions: () => h('button', '查询')
      }
    )

    expect(html).toContain('filter-bar__fields')
    expect(html).toContain('filter-bar__actions')
    expect(html).toContain('关键词')
    expect(html).toContain('查询')
  })

  it('renders table section metadata and empty state copy', async () => {
    const html = await render(TableSection, {
      title: '告警列表',
      description: '展示最近触发的告警记录。',
      total: 18,
      emptyDescription: '暂无告警数据',
      hasData: false,
      loading: false
    })

    expect(html).toContain('告警列表')
    expect(html).toContain('18')
    expect(html).toContain('暂无告警数据')
  })

  it('renders metric and chart shells with stable titles', async () => {
    const metricHtml = await render(MetricCard, {
      label: '在线站点',
      value: '18',
      description: '当前在线监测站点数量'
    })
    const chartHtml = await render(
      ChartSection,
      {
        title: '水位趋势',
        description: '最近监测样本按日期聚合'
      },
      {
        default: () => h('div', '图表区域')
      }
    )

    expect(metricHtml).toContain('在线站点')
    expect(metricHtml).toContain('18')
    expect(chartHtml).toContain('水位趋势')
    expect(chartHtml).toContain('图表区域')
  })

  it('renders side info panel with title, status, and meta slots', async () => {
    const html = await render(
      SideInfoPanel,
      {
        title: '武湖水库',
        subtitle: '水库工程'
      },
      {
        status: () => h('span', '预警'),
        meta: () => h('div', '坐标信息'),
        default: () => h('div', '说明内容')
      }
    )

    expect(html).toContain('武湖水库')
    expect(html).toContain('水库工程')
    expect(html).toContain('预警')
    expect(html).toContain('坐标信息')
    expect(html).toContain('说明内容')
  })
})
