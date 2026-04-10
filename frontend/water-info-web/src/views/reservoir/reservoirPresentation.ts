import type { ReservoirDetail, ReservoirItem } from '@/types/models'

export function buildReservoirOverview(rows: ReservoirItem[]) {
  const totalCapacity = rows.reduce((sum, item) => sum + item.capacity, 0)
  const largestReservoir = rows.reduce<ReservoirItem | null>((current, item) => {
    if (!current || item.capacity > current.capacity) {
      return item
    }

    return current
  }, null)

  return {
    total: rows.length,
    totalCapacity,
    largestReservoirName: largestReservoir?.name ?? '--'
  }
}

export function getReservoirPanelMeta(detail: ReservoirDetail) {
  return {
    heading: '重点资产档案',
    description: '聚焦水库位置、容量和管理单位等关键档案信息，便于对象化管理。',
    keyFacts: [
      { label: '设计容量', value: `${detail.capacity} 万m³` },
      { label: '所在位置', value: detail.location },
      { label: '管理单位', value: detail.managementUnit }
    ]
  }
}

export function getReservoirFormSections() {
  return ['基础信息', '空间信息', '补充说明']
}
