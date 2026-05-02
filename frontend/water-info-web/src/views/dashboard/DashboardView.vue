<template>
  <div class="page-shell">
    <section class="dashboard-map-section panel">
      <header class="dashboard-map-section__header">
        <div class="dashboard-map-section__copy">
          <h3>空间监测分布</h3>
          <p>实时展示水库、河道与监测站点的空间分布及状态。</p>
        </div>
      </header>
      <div class="dashboard-map-layout">
        <div class="dashboard-map-canvas">
          <div ref="mapRef" class="map-container"></div>
        </div>
        <div class="dashboard-map-sidebar">
          <SideInfoPanel title="站点状态" subtitle="在线、离线与预警站点分布">
            <div class="legend-donut-wrapper">
              <StatDonutChart :items="stationStatusStatsCN" palette="stationStatus" />
            </div>
          </SideInfoPanel>
          <SideInfoPanel
            v-if="activeMapPoint"
            :title="activeMapPoint.name"
            :subtitle="mapPointTypeLabel(activeMapPoint)"
          >
            <template #status>
              <StatusTag
                v-if="activeMapPoint.source === 'station'"
                category="stationStatus"
                :value="activeMapPoint.status || 'Normal'"
              />
              <span v-else class="entity-pill">{{ activeMapPoint.source === 'reservoir' ? '水库' : '河道' }}</span>
            </template>
            <template #meta>
              <div class="entity-facts-grid entity-facts-grid--2">
                <div class="entity-facts-grid__item">
                  <span>纬度</span>
                  <strong>{{ activeMapPoint.latitude.toFixed(4) }}</strong>
                </div>
                <div class="entity-facts-grid__item">
                  <span>经度</span>
                  <strong>{{ activeMapPoint.longitude.toFixed(4) }}</strong>
                </div>
              </div>
            </template>
            <p class="map-point-desc">{{ activeMapPoint.description || '暂无说明' }}</p>
          </SideInfoPanel>
          <section v-else class="map-empty panel">
            <el-empty description="点击地图点位查看详情" />
          </section>
        </div>
      </div>
    </section>

    <div class="page-grid dashboard-focus-grid">
      <ChartSection
        class="dashboard-main-chart"
        :title="waterLevelMeta.label"
        description="聚焦最近监测样本按日期聚合后的变化趋势。"
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
          </p>
        </div>
      </SideInfoPanel>
    </div>

    <div class="page-grid dashboard-support-grid">
      <ChartSection :title="rainfallMeta.label" description="展示最近雨量采样的累计变化。">
        <TrendLineChart :points="overview.rainfallTrend" series-type="rainfall" :unit="rainfallMeta.unit" />
      </ChartSection>

      <ChartSection title="告警等级分布" description="按告警等级汇总，用于快速判断风险结构。">
        <StatBarChart :items="overview.alarmLevelStats" palette="alarmLevels" />
      </ChartSection>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, nextTick, onBeforeUnmount, onMounted, reactive, ref } from 'vue'
import L from 'leaflet'
import 'leaflet/dist/leaflet.css'
import ChartSection from '@/components/common/ChartSection.vue'
import SideInfoPanel from '@/components/common/SideInfoPanel.vue'
import StatusTag from '@/components/common/StatusTag.vue'
import TrendLineChart from '@/components/charts/TrendLineChart.vue'
import StatBarChart from '@/components/charts/StatBarChart.vue'
import StatDonutChart from '@/components/charts/StatDonutChart.vue'
import { fetchDashboardOverview } from '@/api/modules/dashboard'
import { fetchMapPoints } from '@/api/modules/map'
import { visualizationTokens } from '@/theme/tokens'
import {
  buildDashboardAlarmSnapshot,
  buildTrendSummary,
  getDashboardMeasurementMeta
} from './dashboardPresentation'
import type { DashboardOverview, MapPoint } from '@/types/models'

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
const alarmSnapshot = computed(() => buildDashboardAlarmSnapshot(overview))
const waterTrendSummary = computed(() => buildTrendSummary(overview.waterLevelTrend))
const rainfallTrendSummary = computed(() => buildTrendSummary(overview.rainfallTrend))

const waterLevelMeta = getDashboardMeasurementMeta('waterLevel')
const rainfallMeta = getDashboardMeasurementMeta('rainfall')

const stationStatusLabels: Record<string, string> = {
  Normal: '在线',
  Offline: '离线',
  Warning: '预警'
}

const stationStatusStatsCN = computed(() =>
  overview.stationStatusStats.map((item) => ({
    ...item,
    name: stationStatusLabels[item.name] || item.name
  }))
)

function formatValueWithUnit(value: number, unit: string) {
  return `${value} ${unit}`.trim()
}

function formatSignedValue(value: number, unit: string) {
  const prefix = value > 0 ? '+' : ''
  return `${prefix}${value} ${unit}`.trim()
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

// Map integration
const mapRef = ref<HTMLDivElement | null>(null)
const mapPoints = ref<MapPoint[]>([])
const activeMapPoint = ref<MapPoint | null>(null)
let mapInstance: L.Map | null = null
let markerLayer: L.LayerGroup | null = null

const mapOverview = computed(() => ({
  total: mapPoints.value.length,
  warningCount: mapPoints.value.filter(p => p.status === 'Warning').length,
  offlineCount: mapPoints.value.filter(p => p.status === 'Offline').length
}))

function mapPointTypeLabel(point: MapPoint) {
  if (point.source === 'reservoir') return '水库工程'
  if (point.source === 'river') return '河道工程'
  return '监测站点'
}

function mapMarkerColor(point: MapPoint) {
  if (point.source === 'reservoir') return visualizationTokens.map.engineering
  if (point.source === 'river') return visualizationTokens.map.river
  if (point.status === 'Warning') return visualizationTokens.map.warning
  if (point.status === 'Offline') return visualizationTokens.map.offline
  return visualizationTokens.map.station
}

function initMap() {
  if (!mapRef.value || mapInstance) return

  mapInstance = L.map(mapRef.value, {
    zoomControl: true
  }).setView([30.62, 114.34], 11)

  L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    attribution: '&copy; OpenStreetMap contributors'
  }).addTo(mapInstance)

  markerLayer = L.layerGroup().addTo(mapInstance)
}

function renderMarkers() {
  if (!mapInstance || !markerLayer) return

  markerLayer.clearLayers()
  const layers = mapPoints.value.map((point) => {
    const marker = L.circleMarker([point.latitude, point.longitude], {
      radius: point.source === 'station' ? 8 : 10,
      color: visualizationTokens.map.markerStroke,
      weight: 2,
      fillColor: mapMarkerColor(point),
      fillOpacity: 0.92
    })

    marker.on('click', () => {
      activeMapPoint.value = point
      marker.bindPopup(`<strong>${point.name}</strong>`).openPopup()
    })

    marker.addTo(markerLayer!)
    return marker
  })

  if (layers.length > 0) {
    const group = L.featureGroup(layers)
    mapInstance.fitBounds(group.getBounds().pad(0.18))
  }
}

async function loadMapData() {
  mapPoints.value = await fetchMapPoints()
  activeMapPoint.value = mapPoints.value[0] ?? null
  await nextTick()
  initMap()
  renderMarkers()
}

onMounted(loadMapData)

onBeforeUnmount(() => {
  markerLayer?.clearLayers()
  mapInstance?.remove()
})
</script>

<style scoped lang="scss">


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
  transition:
    border-color 0.2s ease,
    box-shadow 0.2s ease,
    transform 0.2s ease;

  &:hover {
    border-color: color-mix(in srgb, var(--wi-primary) 18%, var(--wi-app-border-subtle));
    box-shadow: var(--wi-app-shadow-xs);
    transform: translateY(-1px);
  }

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
  grid-template-columns: minmax(0, 1fr) minmax(0, 1fr);
}

.dashboard-support-grid__stack {
  display: grid;
  gap: 24px;
}

.dashboard-map-section {
  padding: 22px 24px;
}

.dashboard-map-section__header {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 16px;
  margin-bottom: 18px;
  padding-bottom: 16px;
  border-bottom: 1px solid var(--wi-app-border-subtle);
}

.dashboard-map-section__copy h3 {
  margin: 0;
  color: var(--wi-text-primary);
  font-size: 18px;
}

.dashboard-map-section__copy p {
  margin: 8px 0 0;
  color: var(--wi-text-secondary);
  font-size: 13px;
}

.dashboard-map-section__stats {
  display: flex;
  flex-wrap: wrap;
  justify-content: flex-end;
  gap: 10px;
}

.dashboard-map-layout {
  display: grid;
  grid-template-columns: minmax(0, 1fr) minmax(300px, 0.35fr);
  gap: 20px;
  min-height: 420px;
}

.dashboard-map-canvas {
  position: relative;
  border-radius: var(--wi-app-radius-md);
  overflow: hidden;
  background: var(--wi-bg-muted);
}

.map-container {
  width: 100%;
  height: 100%;
  min-height: 400px;
}

.dashboard-map-sidebar {
  display: flex;
  flex-direction: column;
  gap: 24px;
}

.legend-donut-wrapper {
  :deep(.chart-card__view) {
    height: 160px;
  }
  :deep(.chart-card) {
    min-height: 160px;
  }
}

.map-empty {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 200px;
}

.map-point-desc {
  margin: 12px 0 0;
  color: var(--wi-text-secondary);
  font-size: 13px;
  line-height: 1.6;
}

@media (max-width: 1200px) {
  .dashboard-overview,
  .metrics-grid,
  .dashboard-focus-grid,
  .dashboard-support-grid,
  .dashboard-map-layout {
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
