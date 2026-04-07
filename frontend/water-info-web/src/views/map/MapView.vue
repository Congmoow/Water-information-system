<template>
  <div class="page-shell">
    <PageCard title="地图展示" subtitle="展示工程点位与监测站点点位，点击后查看详细信息">
      <div class="map-layout">
        <div class="map-canvas-wrap">
          <div ref="mapRef" class="map-canvas"></div>
        </div>

        <div class="map-sidebar">
          <section class="map-sidebar__summary panel">
            <h4>空间态势总览</h4>
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
            <ul class="legend-list">
              <li><i class="legend-dot legend-dot--reservoir"></i> 水库点位</li>
              <li><i class="legend-dot legend-dot--river"></i> 河道点位</li>
              <li><i class="legend-dot legend-dot--station"></i> 监测站点</li>
            </ul>
          </section>

          <section class="map-sidebar__detail panel">
            <template v-if="activePoint">
              <p class="map-sidebar__eyebrow">{{ pointTypeLabel(activePoint) }}</p>
              <h3>{{ activePoint.name }}</h3>
              <el-tag v-if="activePoint.status" :type="activePoint.status === 'Warning' ? 'warning' : activePoint.status === 'Offline' ? 'info' : 'success'">
                {{ activePoint.status }}
              </el-tag>
              <dl>
                <div>
                  <dt>坐标</dt>
                  <dd>{{ activePoint.latitude.toFixed(4) }}, {{ activePoint.longitude.toFixed(4) }}</dd>
                </div>
                <div>
                  <dt>来源</dt>
                  <dd>{{ activePoint.source }}</dd>
                </div>
                <div>
                  <dt>说明</dt>
                  <dd>{{ activePoint.description || '暂无说明' }}</dd>
                </div>
              </dl>
            </template>
            <el-empty v-else description="点击地图中的点位查看详细信息" />
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
  if (point.source === 'reservoir') return '#0f6c7b'
  if (point.source === 'river') return '#2d8f5a'
  if (point.status === 'Warning') return '#d08c2e'
  if (point.status === 'Offline') return '#8aa2b4'
  return '#c98a3d'
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
      color: '#ffffff',
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
  background:
    radial-gradient(circle at top left, rgba(15, 108, 123, 0.14), transparent 30%),
    linear-gradient(180deg, rgba(255, 255, 255, 0.72), rgba(255, 255, 255, 0.9));
  border: 1px solid rgba(15, 108, 123, 0.1);
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
}

.map-sidebar__summary h4,
.map-sidebar__detail h3 {
  margin: 0;
}

.map-sidebar__detail {
  display: flex;
  flex-direction: column;
  gap: 14px;

  dl {
    display: grid;
    gap: 16px;
    margin: 0;
  }

  dt {
    color: var(--wi-text-soft);
    font-size: 13px;
    margin-bottom: 6px;
  }

  dd {
    margin: 0;
    line-height: 1.8;
  }
}

.map-sidebar__eyebrow {
  margin: 0;
  color: var(--wi-primary);
  font-size: 12px;
  letter-spacing: 0.12em;
  text-transform: uppercase;
}

.map-summary-grid {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 14px;
  margin-top: 18px;

  div {
    padding: 18px;
    border-radius: 18px;
    background: rgba(15, 108, 123, 0.06);
  }

  span {
    display: block;
    color: var(--wi-text-soft);
    font-size: 13px;
  }

  strong {
    display: block;
    margin-top: 10px;
    font-size: 30px;
    color: var(--wi-primary-strong);
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
    color: var(--wi-text-soft);
  }
}

.legend-dot {
  width: 14px;
  height: 14px;
  border-radius: 999px;
  display: inline-block;
}

.legend-dot--reservoir {
  background: #0f6c7b;
}

.legend-dot--river {
  background: #2d8f5a;
}

.legend-dot--station {
  background: #c98a3d;
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
