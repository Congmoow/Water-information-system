<template>
  <div class="page-shell">
    <section class="dashboard-overview panel">
      <div class="dashboard-overview__main">
        <p class="dashboard-overview__eyebrow">系统运行总览</p>
        <h2>优先关注站点在线率、水位变化与当日告警响应。</h2>
        <p class="dashboard-overview__copy">
          当前首页聚合工程资产、站点在线状态、趋势变化与最近告警，便于在答辩或日常巡检中快速判断系统运行态势。
        </p>
      </div>
      <div class="dashboard-overview__meta">
        <div>
          <span>在线覆盖率</span>
          <strong>{{ onlineCoverage }}</strong>
        </div>
        <div>
          <span>待关注告警</span>
          <strong>{{ alarmSnapshot.pendingCount }}</strong>
        </div>
        <div>
          <span>最近触发</span>
          <strong>{{ formatDateTime(alarmSnapshot.latestTriggeredAt) }}</strong>
        </div>
      </div>
    </section>

    <div class="page-grid metrics-grid">
      <MetricCard
        v-for="item in metrics"
        :key="item.title"
        :label="item.title"
        :value="item.value"
        :description="item.description"
        :highlight="item.highlight"
        :tone="item.tone"
      />
    </div>

    <div class="page-grid dashboard-focus-grid">
      <ChartSection
        class="dashboard-main-chart"
        :title="waterLevelMeta.label"
        description="作为首页主分析区，聚焦最近监测样本按日期聚合后的变化趋势。"
      >
        <template #actions>
          <div class="trend-summary-group">
            <div class="trend-summary-card">
              <span>最新水位</span>
              <strong>{{ formatValueWithUnit(waterTrendSummary.currentValue, waterLevelMeta.unit) }}</strong>
              <em :class="`trend-summary-card__delta trend-summary-card__delta--${waterTrendSummary.direction}`">
                {{ formatSignedValue(waterTrendSummary.delta, waterLevelMeta.unit) }}
              </em>
            </div>
            <div class="trend-summary-card trend-summary-card--muted">
              <span>最新雨量</span>
              <strong>{{ formatValueWithUnit(rainfallTrendSummary.currentValue, rainfallMeta.unit) }}</strong>
              <em :class="`trend-summary-card__delta trend-summary-card__delta--${rainfallTrendSummary.direction}`">
                {{ formatSignedValue(rainfallTrendSummary.delta, rainfallMeta.unit) }}
              </em>
            </div>
          </div>
        </template>
        <TrendLineChart :points="overview.waterLevelTrend" series-type="waterLevel" :unit="waterLevelMeta.unit" />
      </ChartSection>

      <SideInfoPanel title="告警关注" subtitle="首页响应摘要">
        <template #status>
          <StatusTag category="alarmStatus" :value="alarmSnapshot.pendingCount > 0 ? 'Pending' : 'Resolved'" />
        </template>
        <template #meta>
          <div class="dashboard-alert-meta">
            <div>
              <span>严重告警</span>
              <strong>{{ alarmSnapshot.criticalCount }}</strong>
            </div>
            <div>
              <span>预警告警</span>
              <strong>{{ alarmSnapshot.warningCount }}</strong>
            </div>
            <div>
              <span>待处理</span>
              <strong>{{ alarmSnapshot.pendingCount }}</strong>
            </div>
          </div>
        </template>
        <div class="dashboard-alert-copy">
          <p class="dashboard-alert-copy__title">当前关注重点</p>
          <p>
            今日累计触发 {{ overview.todayAlarmCount }} 条告警，其中待处理 {{ alarmSnapshot.pendingCount }} 条。
            首页下方列表保留最近事件，便于继续追踪。
          </p>
        </div>
      </SideInfoPanel>
    </div>

    <div class="page-grid dashboard-support-grid">
      <ChartSection :title="rainfallMeta.label" description="作为次级趋势区，展示最近雨量采样的累计变化。">
        <TrendLineChart :points="overview.rainfallTrend" series-type="rainfall" :unit="rainfallMeta.unit" />
      </ChartSection>

      <div class="dashboard-support-grid__stack">
        <ChartSection title="告警等级分布" description="按告警等级汇总，用于快速判断风险结构。">
          <StatBarChart :items="overview.alarmLevelStats" palette="alarmLevels" />
        </ChartSection>

        <ChartSection title="站点运行状态" description="在线、离线与预警站点分布。">
          <StatDonutChart :items="overview.stationStatusStats" palette="stationStatus" />
        </ChartSection>

        <SideInfoPanel title="空间分布入口" subtitle="首页空间摘要">
          <template #meta>
            <div class="dashboard-spatial-meta">
              <div>
                <span>水库对象</span>
                <strong>{{ spatialSnapshot.reservoirCount }}</strong>
              </div>
              <div>
                <span>河道对象</span>
                <strong>{{ spatialSnapshot.riverCount }}</strong>
              </div>
              <div>
                <span>监测站点</span>
                <strong>{{ spatialSnapshot.stationCount }}</strong>
              </div>
            </div>
          </template>
          <div class="dashboard-spatial-copy">
            <p class="dashboard-spatial-copy__title">空间维度</p>
            <p>首页保留空间分布摘要，便于从总览切换到地图工作台，继续查看点位分布与异常状态。</p>
          </div>
          <template #footer>
            <div class="entity-panel-footer">
              <div>
                <span>快速入口</span>
                <strong>{{ spatialSnapshot.emphasis }}</strong>
              </div>
              <el-button type="primary" @click="openMapView">查看地图</el-button>
            </div>
          </template>
        </SideInfoPanel>
      </div>
    </div>

    <TableSection
      title="最近告警"
      description="展示最新触发的告警记录，便于首页快速追踪。"
      :loading="loading"
      :has-data="overview.recentAlarms.length > 0"
      :total="overview.recentAlarms.length"
      empty-description="当前暂无最近告警"
    >
      <el-table :data="overview.recentAlarms" v-loading="loading" border>
        <el-table-column prop="stationName" label="站点名称" min-width="160" />
        <el-table-column label="等级" width="120">
          <template #default="{ row }">
            <StatusTag category="alarmLevel" :value="row.level" />
          </template>
        </el-table-column>
        <el-table-column label="状态" width="120">
          <template #default="{ row }">
            <StatusTag category="alarmStatus" :value="row.status" />
          </template>
        </el-table-column>
        <el-table-column prop="message" label="内容" min-width="260" show-overflow-tooltip />
        <el-table-column prop="triggeredAt" label="触发时间" min-width="180">
          <template #default="{ row }">{{ formatDateTime(row.triggeredAt) }}</template>
        </el-table-column>
      </el-table>
    </TableSection>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue'
import dayjs from 'dayjs'
import { useRouter } from 'vue-router'
import ChartSection from '@/components/common/ChartSection.vue'
import MetricCard from '@/components/common/MetricCard.vue'
import SideInfoPanel from '@/components/common/SideInfoPanel.vue'
import StatusTag from '@/components/common/StatusTag.vue'
import TableSection from '@/components/common/TableSection.vue'
import TrendLineChart from '@/components/charts/TrendLineChart.vue'
import StatBarChart from '@/components/charts/StatBarChart.vue'
import StatDonutChart from '@/components/charts/StatDonutChart.vue'
import { fetchDashboardOverview } from '@/api/modules/dashboard'
import {
  buildDashboardAlarmSnapshot,
  buildDashboardMetrics,
  buildDashboardSpatialSnapshot,
  buildTrendSummary,
  getDashboardMeasurementMeta
} from './dashboardPresentation'
import type { DashboardOverview } from '@/types/models'

const router = useRouter()
const loading = ref(false)
const overview = reactive<DashboardOverview>({
  reservoirCount: 0,
  riverCount: 0,
  stationCount: 0,
  onlineStationCount: 0,
  todayAlarmCount: 0,
  waterLevelTrend: [],
  rainfallTrend: [],
  alarmLevelStats: [],
  stationStatusStats: [],
  recentAlarms: []
})

const metrics = computed(() => buildDashboardMetrics(overview))
const alarmSnapshot = computed(() => buildDashboardAlarmSnapshot(overview))
const spatialSnapshot = computed(() => buildDashboardSpatialSnapshot(overview))
const waterTrendSummary = computed(() => buildTrendSummary(overview.waterLevelTrend))
const rainfallTrendSummary = computed(() => buildTrendSummary(overview.rainfallTrend))
const onlineCoverage = computed(() => {
  if (overview.stationCount === 0) return '0%'
  return `${Math.round((overview.onlineStationCount / overview.stationCount) * 100)}%`
})

const waterLevelMeta = getDashboardMeasurementMeta('waterLevel')
const rainfallMeta = getDashboardMeasurementMeta('rainfall')

function formatDateTime(value?: string) {
  return value ? dayjs(value).format('YYYY-MM-DD HH:mm') : '--'
}

function formatValueWithUnit(value: number, unit: string) {
  return `${value} ${unit}`.trim()
}

function formatSignedValue(value: number, unit: string) {
  const prefix = value > 0 ? '+' : ''
  return `${prefix}${value} ${unit}`.trim()
}

async function openMapView() {
  await router.push('/map')
}

async function loadData() {
  loading.value = true
  try {
    Object.assign(overview, await fetchDashboardOverview())
  } finally {
    loading.value = false
  }
}

onMounted(loadData)
</script>

<style scoped lang="scss">
.dashboard-overview {
  padding: 24px;
  display: grid;
  grid-template-columns: minmax(0, 1.3fr) minmax(280px, 0.7fr);
  gap: 24px;
  align-items: center;
}

.dashboard-overview__eyebrow {
  margin: 0;
  color: var(--wi-text-tertiary);
  font-size: 12px;
  letter-spacing: 0.12em;
  text-transform: uppercase;
}

.dashboard-overview__main h2 {
  margin: 12px 0 0;
  max-width: 760px;
  font-size: clamp(24px, 2.6vw, 34px);
  line-height: 1.2;
  color: var(--wi-text-primary);
}

.dashboard-overview__copy {
  margin: 12px 0 0;
  max-width: 720px;
  color: var(--wi-text-secondary);
  line-height: 1.8;
}

.dashboard-overview__meta {
  display: grid;
  gap: 14px;
  padding: 18px;
  border-radius: var(--wi-app-radius-lg);
  background: var(--wi-app-surface-secondary);
  border: 1px solid var(--wi-app-border-subtle);

  span,
  strong {
    display: block;
  }

  span {
    color: var(--wi-text-tertiary);
    font-size: 12px;
  }

  strong {
    margin-top: 8px;
    color: var(--wi-text-primary);
    font-size: 18px;
    line-height: 1.5;
  }
}

.metrics-grid {
  grid-template-columns: repeat(4, minmax(0, 1fr));
}

.dashboard-focus-grid {
  grid-template-columns: minmax(0, 1.45fr) minmax(320px, 0.55fr);
}

.dashboard-main-chart :deep(.chart-card__view) {
  height: 320px;
}

.trend-summary-group {
  display: flex;
  gap: 12px;
}

.trend-summary-card {
  min-width: 126px;
  padding: 12px 14px;
  border-radius: var(--wi-app-radius-md);
  background: var(--wi-app-surface-secondary);
  border: 1px solid var(--wi-app-border-subtle);

  span,
  strong,
  em {
    display: block;
  }

  span {
    color: var(--wi-text-tertiary);
    font-size: 12px;
  }

  strong {
    margin-top: 8px;
    color: var(--wi-text-primary);
    font-size: 20px;
  }
}

.trend-summary-card__delta {
  margin-top: 6px;
  font-style: normal;
  font-size: 12px;
  font-weight: 600;
}

.trend-summary-card__delta--up {
  color: var(--wi-warning);
}

.trend-summary-card__delta--down {
  color: var(--wi-info);
}

.trend-summary-card__delta--flat {
  color: var(--wi-text-tertiary);
}

.trend-summary-card--muted strong {
  font-size: 18px;
}

.dashboard-alert-meta,
.dashboard-spatial-meta {
  display: grid;
  grid-template-columns: repeat(3, minmax(0, 1fr));
  gap: 14px;

  span,
  strong {
    display: block;
  }

  span {
    color: var(--wi-text-tertiary);
    font-size: 12px;
  }

  strong {
    margin-top: 8px;
    color: var(--wi-text-primary);
    font-size: 24px;
  }
}

.dashboard-alert-copy__title,
.dashboard-spatial-copy__title {
  margin: 0 0 8px;
  color: var(--wi-text-primary);
  font-size: 14px;
  font-weight: 600;
}

.dashboard-alert-copy p:last-child,
.dashboard-spatial-copy p:last-child {
  margin: 0;
  color: var(--wi-text-secondary);
  line-height: 1.8;
}

.dashboard-support-grid {
  grid-template-columns: minmax(0, 1.15fr) minmax(0, 0.85fr);
}

.dashboard-support-grid__stack {
  display: grid;
  gap: 24px;
}

@media (max-width: 1200px) {
  .dashboard-overview,
  .metrics-grid,
  .dashboard-focus-grid,
  .dashboard-support-grid {
    grid-template-columns: 1fr;
  }

  .trend-summary-group,
  .dashboard-alert-meta,
  .dashboard-spatial-meta {
    grid-template-columns: 1fr;
    display: grid;
  }

  .dashboard-support-grid__stack {
    gap: 24px;
  }
}
</style>
