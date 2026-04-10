<template>
  <div class="page-shell map-page-shell">
    <div class="map-summary-layout">
      <div class="map-summary-layout__cards">
        <MetricCard
          label="空间点位总数"
          :value="overview.total"
          description="当前地图已加载的全部空间对象与监测站点数量。"
          highlight="全局分布"
          tone="info"
        />
        <MetricCard
          label="工程对象"
          :value="overview.engineeringCount"
          description="水库与河道等工程对象在地图上的分布规模。"
          highlight="空间对象"
          tone="info"
        />
        <MetricCard
          label="预警点位"
          :value="overview.warningCount"
          description="地图上当前显示为预警状态的监测站点数量。"
          :highlight="overview.warningCount > 0 ? '需关注' : '运行平稳'"
          :tone="overview.warningCount > 0 ? 'warning' : 'success'"
        />
        <MetricCard
          label="离线点位"
          :value="overview.offlineCount"
          description="地图上当前未在线的监测站点数量。"
          :highlight="overview.offlineCount > 0 ? '需排查' : '通信正常'"
          tone="info"
        />
      </div>

      <SideInfoPanel title="空间态势摘要" subtitle="地图工作台">
        <template #status>
          <StatusTag category="riskStatus" :value="overview.warningCount > 0 ? 'Warning' : 'Normal'" />
        </template>
        <template #meta>
          <div class="entity-facts-grid entity-facts-grid--2 map-summary-meta">
            <div class="entity-facts-grid__item">
              <span>监测站点</span>
              <strong>{{ overview.stationCount }}</strong>
            </div>
            <div class="entity-facts-grid__item">
              <span>水库对象</span>
              <strong>{{ overview.reservoirCount }}</strong>
            </div>
            <div class="entity-facts-grid__item">
              <span>河道对象</span>
              <strong>{{ overview.riverCount }}</strong>
            </div>
            <div class="entity-facts-grid__item">
              <span>当前焦点</span>
              <strong>{{ overview.activePointName }}</strong>
            </div>
          </div>
        </template>
        <div class="map-summary-copy">
          <p class="map-summary-copy__title">页面阅读顺序</p>
          <p>先看顶部摘要判断空间分布与异常数量，再在主画布定位对象，最后结合右侧图例和详情面板确认对象属性与当前状态。</p>
        </div>
      </SideInfoPanel>
    </div>

    <section class="map-workbench panel">
      <header class="map-workbench__header">
        <div class="map-workbench__copy">
          <h3>空间监测主画布</h3>
          <p>地图作为主视图呈现对象分布与状态差异，右侧侧栏负责解释图例规则、状态语义和当前选中对象的详细信息。</p>
        </div>
        <div class="map-workbench__actions">
          <span class="entity-pill">点位总数 {{ overview.total }}</span>
          <span class="entity-pill">当前焦点 {{ overview.activePointName }}</span>
        </div>
      </header>

      <div class="map-workbench__body">
        <div class="map-stage">
          <div class="map-stage__overlay">
            <span>空间分布闭环</span>
            <strong>点击地图点位查看右侧详情</strong>
          </div>
          <div ref="mapRef" class="map-canvas"></div>
        </div>

        <div class="map-side-stack">
          <SideInfoPanel title="图例与状态说明" subtitle="空间语义规则">
            <template #meta>
              <div class="entity-facts-grid entity-facts-grid--2 map-legend-meta">
                <div class="entity-facts-grid__item">
                  <span>预警点位</span>
                  <strong>{{ overview.warningCount }}</strong>
                </div>
                <div class="entity-facts-grid__item">
                  <span>离线点位</span>
                  <strong>{{ overview.offlineCount }}</strong>
                </div>
              </div>
            </template>
            <ul class="legend-list">
              <li><i class="legend-dot legend-dot--reservoir"></i> 水库工程</li>
              <li><i class="legend-dot legend-dot--river"></i> 河道工程</li>
              <li><i class="legend-dot legend-dot--station"></i> 监测站点</li>
              <li><i class="legend-dot legend-dot--warning"></i> 预警站点</li>
              <li><i class="legend-dot legend-dot--offline"></i> 离线站点</li>
            </ul>
          </SideInfoPanel>

          <SideInfoPanel
            v-if="activePoint && pointInsight"
            :title="activePoint.name"
            :subtitle="pointTypeLabel(activePoint)"
          >
            <template #status>
              <StatusTag :category="activeStatus.category" :value="activeStatus.value" />
            </template>
            <template #meta>
              <div :class="`entity-state-note entity-state-note--${pointInsight.tone}`">
                <span class="entity-state-note__eyebrow">状态摘要</span>
                <strong>{{ pointInsight.heading }}</strong>
                <p>{{ pointInsight.description }}</p>
              </div>
              <div class="entity-facts-grid entity-facts-grid--2 map-point-meta">
                <div class="entity-facts-grid__item">
                  <span>对象来源</span>
                  <strong>{{ sourceLabel(activePoint.source) }}</strong>
                </div>
                <div class="entity-facts-grid__item">
                  <span>对象类型</span>
                  <strong>{{ pointTypeLabel(activePoint) }}</strong>
                </div>
                <div class="entity-facts-grid__item">
                  <span>纬度</span>
                  <strong>{{ activePoint.latitude.toFixed(4) }}</strong>
                </div>
                <div class="entity-facts-grid__item">
                  <span>经度</span>
                  <strong>{{ activePoint.longitude.toFixed(4) }}</strong>
                </div>
              </div>
            </template>
            <dl class="entity-detail-list">
              <div>
                <dt>空间说明</dt>
                <dd>{{ activePoint.description || '暂无说明' }}</dd>
              </div>
              <div>
                <dt>状态提示</dt>
                <dd>{{ pointInsight.description }}</dd>
              </div>
              <div>
                <dt>阅读建议</dt>
                <dd>先结合主画布确认点位位置，再通过当前详情面板判断对象身份、状态语义与补充说明。</dd>
              </div>
            </dl>
            <template #footer>
              <div class="entity-panel-footer">
                <div>
                  <span>当前选中对象</span>
                  <strong>{{ overview.activePointName }}</strong>
                </div>
                <div>
                  <span>状态判定</span>
                  <strong>{{ activeStatusLabel }}</strong>
                </div>
              </div>
            </template>
          </SideInfoPanel>

          <section v-else class="map-empty panel">
            <el-empty description="点击地图中的点位查看空间对象详情" />
          </section>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
import { computed, nextTick, onBeforeUnmount, onMounted, ref } from 'vue'
import L from 'leaflet'
import 'leaflet/dist/leaflet.css'
import MetricCard from '@/components/common/MetricCard.vue'
import SideInfoPanel from '@/components/common/SideInfoPanel.vue'
import StatusTag from '@/components/common/StatusTag.vue'
import { visualizationTokens } from '@/theme/tokens'
import { fetchMapPoints } from '@/api/modules/map'
import { buildMapOverview, getMapPointInsight, pointTypeLabel } from './mapPresentation'
import type { MapPoint } from '@/types/models'

const mapRef = ref<HTMLDivElement | null>(null)
const points = ref<MapPoint[]>([])
const activePoint = ref<MapPoint | null>(null)
let mapInstance: L.Map | null = null
let markerLayer: L.LayerGroup | null = null

const overview = computed(() => buildMapOverview(points.value, activePoint.value))
const pointInsight = computed(() => (activePoint.value ? getMapPointInsight(activePoint.value) : null))
const activeStatus = computed(() => {
  if (activePoint.value?.status === 'Offline') {
    return { category: 'stationStatus' as const, value: 'Offline' }
  }

  if (activePoint.value?.status === 'Warning') {
    return { category: 'riskStatus' as const, value: 'Warning' }
  }

  return { category: 'riskStatus' as const, value: 'Normal' }
})
const activeStatusLabel = computed(() => {
  if (activePoint.value?.status === 'Offline') return '离线'
  if (activePoint.value?.status === 'Warning') return '预警'
  return activePoint.value?.source === 'station' ? '正常' : '已纳入监测'
})

function sourceLabel(source: string) {
  if (source === 'reservoir') return '水库'
  if (source === 'river') return '河道'
  return '站点'
}

function markerColor(point: MapPoint) {
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
  const layers = points.value.map((point) => {
    const marker = L.circleMarker([point.latitude, point.longitude], {
      radius: point.source === 'station' ? 10 : 12,
      color: visualizationTokens.map.markerStroke,
      weight: 2,
      fillColor: markerColor(point),
      fillOpacity: 0.92
    })

    marker.on('click', () => {
      activePoint.value = point
      marker.bindPopup(`<strong>${point.name}</strong><br/>${pointTypeLabel(point)}`).openPopup()
    })

    marker.addTo(markerLayer!)
    return marker
  })

  if (layers.length > 0) {
    const group = L.featureGroup(layers)
    mapInstance.fitBounds(group.getBounds().pad(0.18))
  }
}

async function loadData() {
  points.value = await fetchMapPoints()
  activePoint.value = points.value[0] ?? null
  await nextTick()
  initMap()
  renderMarkers()
}

onMounted(loadData)

onBeforeUnmount(() => {
  markerLayer?.clearLayers()
  mapInstance?.remove()
})
</script>

<style scoped lang="scss">
.map-page-shell {
  gap: 24px;
}

.map-summary-layout {
  display: grid;
  grid-template-columns: minmax(0, 1.4fr) minmax(320px, 0.6fr);
  gap: 24px;
}

.map-summary-layout__cards {
  display: grid;
  grid-template-columns: repeat(4, minmax(0, 1fr));
  gap: 24px;
}

.map-summary-copy__title {
  margin: 0 0 8px;
  color: var(--wi-text-primary);
  font-size: 14px;
  font-weight: 600;
}

.map-summary-copy p:last-child {
  margin: 0;
  color: var(--wi-text-secondary);
  line-height: 1.8;
}

.map-workbench {
  padding: 22px 24px;
}

.map-workbench__header {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 16px;
  margin-bottom: 18px;
  padding-bottom: 16px;
  border-bottom: 1px solid var(--wi-app-border-subtle);
}

.map-workbench__copy h3 {
  margin: 0;
  color: var(--wi-text-primary);
  font-size: 18px;
}

.map-workbench__copy p {
  margin: 8px 0 0;
  color: var(--wi-text-secondary);
  font-size: 13px;
  line-height: 1.8;
}

.map-workbench__actions {
  display: flex;
  flex-wrap: wrap;
  justify-content: flex-end;
  gap: 10px;
}

.map-workbench__body {
  display: grid;
  grid-template-columns: minmax(0, 1.55fr) minmax(320px, 0.55fr);
  gap: 22px;
  align-items: flex-start;
}

.map-stage {
  position: relative;
  min-height: 660px;
  padding: 14px;
  border-radius: 28px;
  background: var(--wi-app-surface-secondary);
  border: 1px solid var(--wi-app-border-subtle);
}

.map-stage__overlay {
  position: absolute;
  top: 28px;
  left: 28px;
  z-index: 500;
  display: flex;
  flex-direction: column;
  gap: 6px;
  padding: 14px 16px;
  border-radius: var(--wi-app-radius-md);
  background: color-mix(in srgb, var(--wi-surface-panel) 92%, transparent);
  border: 1px solid var(--wi-app-border-subtle);
  box-shadow: var(--wi-shadow-sm);
}

.map-stage__overlay span,
.map-stage__overlay strong {
  display: block;
}

.map-stage__overlay span {
  color: var(--wi-text-tertiary);
  font-size: 12px;
  letter-spacing: 0.08em;
  text-transform: uppercase;
}

.map-stage__overlay strong {
  color: var(--wi-text-primary);
  font-size: 14px;
}

.map-canvas {
  min-height: 632px;
  border-radius: 22px;
  overflow: hidden;
}

.map-side-stack {
  display: grid;
  gap: 18px;
}

.map-summary-meta,
.map-legend-meta,
.map-point-meta {
  gap: 12px;
}

.legend-list {
  margin: 0;
  padding: 0;
  list-style: none;
  display: grid;
  gap: 12px;
}

.legend-list li {
  display: flex;
  align-items: center;
  gap: 10px;
  color: var(--wi-text-secondary);
}

.legend-dot {
  width: 14px;
  height: 14px;
  border-radius: 999px;
  display: inline-block;
}

.legend-dot--reservoir {
  background: var(--wi-map-engineering);
}

.legend-dot--river {
  background: var(--wi-map-river);
}

.legend-dot--station {
  background: var(--wi-map-station);
}

.legend-dot--warning {
  background: var(--wi-map-warning);
}

.legend-dot--offline {
  background: var(--wi-map-offline);
}

.map-empty {
  padding: 22px;
}

@media (max-width: 1200px) {
  .map-summary-layout,
  .map-summary-layout__cards,
  .map-workbench__body {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 960px) {
  .map-workbench {
    padding: 18px;
  }

  .map-workbench__header {
    flex-direction: column;
  }

  .map-workbench__actions {
    justify-content: flex-start;
  }

  .map-stage {
    min-height: 460px;
  }

  .map-canvas {
    min-height: 432px;
  }
}
</style>
