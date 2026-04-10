<template>
  <div class="page-shell">
    <PageCard title="地图展示" subtitle="展示工程点位与监测站点点位，点击后查看详细信息">
      <div class="map-layout">
        <div class="map-canvas-wrap">
          <div ref="mapRef" class="map-canvas"></div>
        </div>

        <div class="map-sidebar">
          <SideInfoPanel title="空间态势总览" subtitle="地图摘要">
            <template #meta>
              <div class="map-summary-grid">
                <div>
                  <span>工程点位</span>
                  <strong>{{ engineeringCount }}</strong>
                </div>
                <div>
                  <span>站点点位</span>
                  <strong>{{ stationCount }}</strong>
                </div>
              </div>
            </template>
            <ul class="legend-list">
              <li><i class="legend-dot legend-dot--reservoir"></i> 水库点位</li>
              <li><i class="legend-dot legend-dot--river"></i> 河道点位</li>
              <li><i class="legend-dot legend-dot--station"></i> 监测站点</li>
              <li><i class="legend-dot legend-dot--warning"></i> 预警站点</li>
              <li><i class="legend-dot legend-dot--offline"></i> 离线站点</li>
            </ul>
          </SideInfoPanel>

          <SideInfoPanel
            v-if="activePoint"
            :title="activePoint.name"
            :subtitle="pointTypeLabel(activePoint)"
          >
            <template #status>
              <StatusTag
                v-if="activePoint.status"
                :category="activePoint.status === 'Offline' ? 'stationStatus' : 'riskStatus'"
                :value="activePoint.status === 'Offline' ? activePoint.status : activePoint.status === 'Warning' ? 'Warning' : 'Normal'"
              />
            </template>
            <template #meta>
              <div class="detail-meta-grid">
                <div>
                  <span>坐标</span>
                  <strong>{{ activePoint.latitude.toFixed(4) }}, {{ activePoint.longitude.toFixed(4) }}</strong>
                </div>
                <div>
                  <span>来源</span>
                  <strong>{{ activePoint.source }}</strong>
                </div>
              </div>
            </template>
            <dl class="map-detail-list">
              <div>
                <dt>说明</dt>
                <dd>{{ activePoint.description || '暂无说明' }}</dd>
              </div>
            </dl>
          </SideInfoPanel>

          <section v-else class="map-sidebar__detail panel">
            <el-empty description="点击地图中的点位查看详细信息" />
          </section>
        </div>
      </div>
    </PageCard>
  </div>
</template>

<script setup lang="ts">
import { computed, nextTick, onBeforeUnmount, onMounted, ref } from 'vue'
import L from 'leaflet'
import 'leaflet/dist/leaflet.css'
import PageCard from '@/components/common/PageCard.vue'
import SideInfoPanel from '@/components/common/SideInfoPanel.vue'
import StatusTag from '@/components/common/StatusTag.vue'
import { visualizationTokens } from '@/theme/tokens'
import { fetchMapPoints } from '@/api/modules/map'
import type { MapPoint } from '@/types/models'

const mapRef = ref<HTMLDivElement | null>(null)
const points = ref<MapPoint[]>([])
const activePoint = ref<MapPoint | null>(null)
let mapInstance: L.Map | null = null
let markerLayer: L.LayerGroup | null = null

const engineeringCount = computed(() => points.value.filter((item) => item.source !== 'station').length)
const stationCount = computed(() => points.value.filter((item) => item.source === 'station').length)

function pointTypeLabel(point: MapPoint) {
  if (point.source === 'reservoir') return '水库工程'
  if (point.source === 'river') return '河道工程'
  return `监测站点 / ${point.type}`
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
.map-layout {
  display: grid;
  grid-template-columns: minmax(0, 1.35fr) minmax(320px, 0.65fr);
  gap: 18px;
}

.map-canvas-wrap {
  padding: 12px;
  border-radius: 28px;
  background: linear-gradient(180deg, var(--wi-surface-raised), var(--wi-surface-panel));
  border: 1px solid var(--wi-border-default);
}

.map-canvas {
  min-height: 560px;
  border-radius: 22px;
  overflow: hidden;
}

.map-sidebar {
  display: grid;
  gap: 18px;
}

.map-sidebar__summary,
.map-sidebar__detail {
  padding: 22px;
  background: var(--wi-map-panel-bg);
}

.map-sidebar__detail {
  display: flex;
  flex-direction: column;
  gap: 14px;
}

.map-summary-grid {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 14px;

  div {
    padding: 4px 0;
  }

  span {
    display: block;
    color: var(--wi-text-secondary);
    font-size: 13px;
  }

  strong {
    display: block;
    margin-top: 10px;
    font-size: 30px;
    color: var(--wi-primary-active);
  }
}

.detail-meta-grid {
  display: grid;
  gap: 14px;

  span {
    display: block;
    color: var(--wi-text-tertiary);
    font-size: 12px;
  }

  strong {
    display: block;
    margin-top: 6px;
    color: var(--wi-text-primary);
    font-size: 14px;
    line-height: 1.7;
  }
}

.map-detail-list {
  display: grid;
  gap: 16px;
  margin: 0;

  dt {
    margin-bottom: 6px;
    color: var(--wi-text-tertiary);
    font-size: 12px;
  }

  dd {
    margin: 0;
    line-height: 1.8;
    color: var(--wi-text-primary);
  }
}

.legend-list {
  margin: 18px 0 0;
  padding: 0;
  list-style: none;
  display: grid;
  gap: 12px;

  li {
    display: flex;
    align-items: center;
    gap: 10px;
    color: var(--wi-text-secondary);
  }
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

@media (max-width: 1180px) {
  .map-layout {
    grid-template-columns: 1fr;
  }

  .map-canvas {
    min-height: 420px;
  }
}
</style>
