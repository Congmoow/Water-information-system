<template>
  <div class="page-shell">
    <PageCard class="dashboard-hero" title="水利信息管理系统" subtitle="以工程、站点、监测和告警为核心的课程演示后台">
      <div class="dashboard-hero__content">
        <div>
          <p class="dashboard-hero__eyebrow">Control Room Snapshot</p>
          <h2>实时掌握工程态势、监测数据与风险告警</h2>
          <p class="dashboard-hero__copy">
            首页聚合展示工程总量、站点在线情况、水位与雨量趋势，以及最近告警处置动态，便于课程答辩时直接演示主业务链路。
          </p>
        </div>
        <div class="dashboard-hero__pulse">
          <span>在线站点</span>
          <strong>{{ overview.onlineStationCount }}</strong>
          <em>/ {{ overview.stationCount }}</em>
        </div>
      </div>
    </PageCard>

    <div class="page-grid metrics-grid">
      <PageCard v-for="item in metrics" :key="item.title">
        <div class="metric-tile">
          <span>{{ item.title }}</span>
          <strong>{{ item.value }}</strong>
          <p>{{ item.description }}</p>
        </div>
      </PageCard>
    </div>

    <div class="page-grid analytics-grid">
      <PageCard title="水位趋势" subtitle="最近监测样本按日期聚合">
        <TrendLineChart
          :points="overview.waterLevelTrend"
          color="#0f6c7b"
          area-color="rgba(15, 108, 123, 0.2)"
          unit=""
        />
      </PageCard>

      <PageCard title="雨量统计" subtitle="最近雨量采样按日期累计">
        <TrendLineChart
          :points="overview.rainfallTrend"
          color="#2d8f5a"
          area-color="rgba(45, 143, 90, 0.18)"
          unit=""
        />
      </PageCard>
    </div>

    <div class="page-grid analytics-grid analytics-grid--secondary">
      <PageCard title="告警数量统计" subtitle="按告警等级汇总">
        <StatBarChart :items="overview.alarmLevelStats" :colors="['#8aa2b4', '#d08c2e', '#c44d4d']" />
      </PageCard>

      <PageCard title="站点状态统计" subtitle="在线、离线与预警站点分布">
        <StatDonutChart :items="overview.stationStatusStats" :colors="['#2d8f5a', '#8aa2b4', '#d08c2e']" />
      </PageCard>
    </div>

    <PageCard title="最近告警" subtitle="展示最新触发的告警记录，便于首页快速追踪">
      <el-table :data="overview.recentAlarms" v-loading="loading" border>
        <el-table-column prop="stationName" label="站点名称" min-width="160" />
        <el-table-column label="等级" width="120">
          <template #default="{ row }">
            <el-tag :type="row.level === 'Critical' ? 'danger' : row.level === 'Warning' ? 'warning' : 'info'">
              {{ row.level }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="状态" width="120">
          <template #default="{ row }">
            <el-tag :type="row.status === 'Resolved' ? 'success' : row.status === 'Processing' ? 'warning' : 'danger'">
              {{ row.status }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="message" label="内容" min-width="260" show-overflow-tooltip />
        <el-table-column prop="triggeredAt" label="触发时间" min-width="180">
          <template #default="{ row }">{{ formatDateTime(row.triggeredAt) }}</template>
        </el-table-column>
      </el-table>
    </PageCard>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue'
import dayjs from 'dayjs'
import PageCard from '@/components/common/PageCard.vue'
import TrendLineChart from '@/components/charts/TrendLineChart.vue'
import StatBarChart from '@/components/charts/StatBarChart.vue'
import StatDonutChart from '@/components/charts/StatDonutChart.vue'
import { fetchDashboardOverview } from '@/api/modules/dashboard'
import type { DashboardOverview } from '@/types/models'

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

const metrics = computed(() => [
  { title: '水库总数', value: overview.reservoirCount, description: '纳入系统统一维护的水库工程' },
  { title: '河道总数', value: overview.riverCount, description: '纳入系统展示的主要河道' },
  { title: '站点总数', value: overview.stationCount, description: '支持监测与告警的站点总量' },
  { title: '今日告警', value: overview.todayAlarmCount, description: '今日自动触发的告警记录' }
])

function formatDateTime(value: string) {
  return dayjs(value).format('YYYY-MM-DD HH:mm')
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
.dashboard-hero {
  overflow: hidden;
  background:
    linear-gradient(135deg, rgba(9, 65, 77, 0.97), rgba(14, 94, 111, 0.92)),
    radial-gradient(circle at top left, rgba(201, 138, 61, 0.28), transparent 30%);
  color: #f5fbff;

  :deep(.page-card__header p) {
    color: rgba(245, 251, 255, 0.78);
  }

  :deep(.page-card__header h3) {
    color: #fff;
  }
}

.dashboard-hero__content {
  display: grid;
  grid-template-columns: 1.4fr 0.6fr;
  gap: 22px;
  align-items: center;
}

.dashboard-hero__eyebrow {
  margin: 0 0 10px;
  letter-spacing: 0.16em;
  text-transform: uppercase;
  color: rgba(245, 251, 255, 0.74);
  font-size: 12px;
}

.dashboard-hero h2 {
  margin: 0;
  font-size: clamp(28px, 3vw, 42px);
  line-height: 1.1;
}

.dashboard-hero__copy {
  margin: 14px 0 0;
  max-width: 720px;
  color: rgba(245, 251, 255, 0.82);
  line-height: 1.9;
}

.dashboard-hero__pulse {
  justify-self: end;
  min-width: 220px;
  padding: 24px;
  border-radius: 24px;
  background: rgba(255, 255, 255, 0.12);
  box-shadow: inset 0 0 0 1px rgba(255, 255, 255, 0.08);
  display: flex;
  flex-direction: column;
  gap: 8px;

  span,
  em {
    color: rgba(245, 251, 255, 0.78);
    font-style: normal;
  }

  strong {
    font-size: 54px;
    line-height: 1;
  }
}

.metrics-grid {
  grid-template-columns: repeat(4, minmax(0, 1fr));
}

.metric-tile {
  display: flex;
  flex-direction: column;
  gap: 12px;

  span {
    color: var(--wi-text-soft);
    font-size: 13px;
    letter-spacing: 0.06em;
    text-transform: uppercase;
  }

  strong {
    font-size: 40px;
    color: var(--wi-primary-strong);
  }

  p {
    margin: 0;
    color: var(--wi-text-soft);
    line-height: 1.7;
  }
}

.analytics-grid {
  grid-template-columns: repeat(2, minmax(0, 1fr));
}

@media (max-width: 1200px) {
  .dashboard-hero__content,
  .metrics-grid,
  .analytics-grid {
    grid-template-columns: 1fr;
  }

  .dashboard-hero__pulse {
    justify-self: stretch;
  }
}
</style>
