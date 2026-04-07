export interface ReservoirItem {
  id: string
  name: string
  location: string
  capacity: number
  managementUnit: string
  latitude: number
  longitude: number
  description: string
  updatedAt: string
}

export interface ReservoirDetail extends ReservoirItem {
  createdAt: string
}

export interface RiverItem {
  id: string
  name: string
  length: number
  basin: string
  latitude: number
  longitude: number
  description: string
  updatedAt: string
}

export interface RiverDetail extends RiverItem {
  createdAt: string
}

export interface StationItem {
  id: string
  name: string
  type: string
  longitude: number
  latitude: number
  status: string
  warningThreshold: number
  criticalThreshold: number
  description: string
  parentName?: string
  lastActiveAt?: string
  updatedAt: string
}

export interface StationDetail extends StationItem {
  reservoirId?: string
  reservoirName?: string
  riverId?: string
  riverName?: string
  createdAt: string
}

export interface MonitoringItem {
  id: string
  stationId: string
  stationName: string
  dataType: string
  value: number
  collectedAt: string
  triggeredAlarm: boolean
  remark?: string
}

export interface MonitoringCreateResult {
  id: string
  triggeredAlarm: boolean
  alarmId?: string
  message: string
}

export interface AlarmItem {
  id: string
  stationId: string
  stationName: string
  alarmType: string
  level: string
  status: string
  message: string
  triggeredAt: string
  handledAt?: string
}

export interface AlarmDetail extends AlarmItem {
  handleRemark?: string
  monitoringDataId?: string
  handledByUserId?: string
}

export interface TrendPoint {
  label: string
  value: number
}

export interface CategoryCount {
  name: string
  value: number
}

export interface DashboardOverview {
  reservoirCount: number
  riverCount: number
  stationCount: number
  onlineStationCount: number
  todayAlarmCount: number
  waterLevelTrend: TrendPoint[]
  rainfallTrend: TrendPoint[]
  alarmLevelStats: CategoryCount[]
  stationStatusStats: CategoryCount[]
  recentAlarms: RecentAlarm[]
}

export interface RecentAlarm {
  id: string
  stationName: string
  level: string
  status: string
  message: string
  triggeredAt: string
}

export interface MapPoint {
  id: string
  name: string
  type: string
  latitude: number
  longitude: number
  description: string
  status?: string
  source: string
}
