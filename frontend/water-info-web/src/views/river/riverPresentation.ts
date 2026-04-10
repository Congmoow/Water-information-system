import type { RiverDetail, RiverItem } from '@/types/models'

export function buildRiverOverview(rows: RiverItem[]) {
  const totalLength = Number(rows.reduce((sum, item) => sum + item.length, 0).toFixed(1))
  const majorRiver = rows.reduce<RiverItem | null>((current, item) => {
    if (!current || item.length > current.length) {
      return item
    }

    return current
  }, null)

  return {
    total: rows.length,
    totalLength,
    majorRiverName: majorRiver?.name ?? '--'
  }
}

export function getRiverPanelMeta(detail: RiverDetail) {
  return {
    heading: '河道档案概览',
    description: '聚焦流域归属、河道长度与空间位置，形成稳定的线性对象档案视图。',
    keyFacts: [
      { label: '河道长度', value: `${detail.length} km` },
      { label: '所属流域', value: detail.basin },
      { label: '中心坐标', value: `${detail.latitude}, ${detail.longitude}` }
    ]
  }
}

export function getRiverFormSections() {
  return ['基础信息', '空间信息', '补充说明']
}
