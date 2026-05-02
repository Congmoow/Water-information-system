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
  return {
    basic: { title: '基础信息', description: '定义名称、所在位置、容量和管理单位等核心资产属性。' },
    spatial: { title: '空间信息', description: '维护资产空间坐标，保证对象在地图和档案中的位置一致。' },
    supplement: { title: '补充说明', description: '补充说明资产用途、现场情况或维护备注。' }
  } as const
}
