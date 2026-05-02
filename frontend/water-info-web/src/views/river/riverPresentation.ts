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
  return {
    basic: { title: '基础信息', description: '定义河道名称、长度及所属流域等基础属性。' },
    spatial: { title: '空间信息', description: '维护河道空间坐标，保证在地图和档案中的位置一致。' },
    supplement: { title: '补充说明', description: '补充说明河道特征、水文情况或维护备注。' }
  } as const
}
