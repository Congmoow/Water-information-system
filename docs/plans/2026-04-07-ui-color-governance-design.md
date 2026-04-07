# 水利信息系统 UI 色彩系统化治理设计说明

## 1. 目标

在不改变现有页面结构、业务语义、交互逻辑和组件划分的前提下，对前端项目进行一次项目级色彩治理。目标不是“换一套更花的颜色”，而是建立可维护的 design tokens 和统一主题规则，修复强调色滥用、中性色不足、语义色混乱、局部对比度偏弱、图表与地图各自为政等问题。

本次治理后的视觉方向为：

- 专业
- 稳重
- 清晰
- 有水利行业特征
- 不浮夸

## 2. 现状问题

基于 `frontend/water-info-web/src` 的主题、布局、图表和业务页面审查，现有问题集中在以下几个方面：

### 2.1 品牌色和强调色并列主导

- 当前项目同时使用蓝青和金色作为高存在感主视觉。
- 侧边栏、首页 hero、头像、地图和图表中都存在金色与蓝青混合渐变。
- 强调色没有被限制在少量关键区域，导致页面重点不聚焦。

### 2.2 中性色体系薄弱

- 全局主题中仅存在少量 `--wi-*` 变量，且大多直接面向具体组件。
- 页面背景、卡片背景、分区背景、边框、文字、说明文字之间没有形成稳定层级。
- 层级区分过度依赖彩色背景，而不是依赖中性色和边框。

### 2.3 语义色和业务色混用

- `warning`、品牌强调和金色存在角色重叠。
- 图表、地图点位、标签、按钮、表格状态在不同区域各自定义，不共享统一语义。
- 告警体系没有与品牌体系完全解耦。

### 2.4 Element Plus 和业务组件主题脱节

- `theme.scss` 只覆盖了少量 Element Plus 变量。
- 布局组件、图表组件、地图组件和页面级组件存在大量硬编码颜色。
- 组件库和业务页面没有共享同一套 token 设计。

### 2.5 局部对比度和可读性不稳定

- 深色渐变区域的辅助文字对比度偏弱。
- 浅彩色背景中的说明文字、统计块、图例块存在可读性波动。
- 表格、边框、卡片之间的层级不够清晰。

## 3. 色彩治理原则

### 3.1 品牌色只保留一个主轴

- 主品牌色统一收敛到更稳的水利蓝青体系。
- 金色不再与品牌色并列主导，只作为少量强调色存在。

### 3.2 层级优先用中性色构建

- 页面背景、卡片、次级区域、边框、分割线、主次文字全部使用中性色体系统一管理。
- 大面积背景不再使用高饱和彩色。

### 3.3 语义色必须独立

- `success / warning / danger / info` 拥有独立语义色。
- `warning` 不再借用金色强调。
- 告警色与品牌色彻底拆分。

### 3.4 页面和可视化共享主题，但保留专属语义层

- 业务界面、Element Plus 组件、图表、地图共用同一套核心色板和语义 token。
- 图表和地图额外拥有独立的可视化语义 token，以适应数据表达场景。

## 4. 新的三层色彩架构

### 4.1 Core Palette

只定义原始色阶，不直接在业务组件中使用。

- 品牌蓝青阶梯：`brand-50` ~ `brand-900`
- 中性色阶：`neutral-0` ~ `neutral-950`
- 强调金阶梯：`accent-100` ~ `accent-700`
- 语义色阶：
  - `success-*`
  - `warning-*`
  - `danger-*`
  - `info-*`

### 4.2 Semantic Tokens

页面、组件和业务视图只依赖语义 token。

#### 背景与 surface

- `--wi-bg-base`
- `--wi-bg-subtle`
- `--wi-bg-muted`
- `--wi-surface-raised`
- `--wi-surface-panel`
- `--wi-surface-overlay`
- `--wi-surface-strong`
- `--wi-surface-inverse`

#### 文字

- `--wi-text-primary`
- `--wi-text-secondary`
- `--wi-text-tertiary`
- `--wi-text-disabled`
- `--wi-text-inverse-primary`
- `--wi-text-inverse-secondary`

#### 边框

- `--wi-border-subtle`
- `--wi-border-default`
- `--wi-border-strong`
- `--wi-border-inverse`

#### 品牌与交互

- `--wi-primary`
- `--wi-primary-hover`
- `--wi-primary-active`
- `--wi-primary-soft`
- `--wi-primary-contrast`
- `--wi-accent`
- `--wi-accent-soft`
- `--wi-focus-ring`
- `--wi-disabled-bg`
- `--wi-disabled-border`

#### 语义状态

- `--wi-success`
- `--wi-success-soft`
- `--wi-warning`
- `--wi-warning-soft`
- `--wi-danger`
- `--wi-danger-soft`
- `--wi-info`
- `--wi-info-soft`

#### 反色 token

用于深色导航、深色 hero、深色提示块等区域：

- `--wi-inverse-surface`
- `--wi-inverse-surface-strong`
- `--wi-text-inverse-primary`
- `--wi-text-inverse-secondary`
- `--wi-border-inverse`

### 4.3 Visualization Tokens

为图表和地图建立独立语义层，避免数据表达和页面品牌表达互相污染。

#### 图表

- `--wi-chart-axis`
- `--wi-chart-grid`
- `--wi-chart-text`
- `--wi-chart-series-primary`
- `--wi-chart-series-secondary`
- `--wi-chart-series-accent`
- `--wi-chart-series-neutral`
- `--wi-chart-series-success`
- `--wi-chart-series-warning`
- `--wi-chart-series-danger`

#### 地图

- `--wi-map-engineering`
- `--wi-map-river`
- `--wi-map-station`
- `--wi-map-warning`
- `--wi-map-offline`
- `--wi-map-marker-stroke`
- `--wi-map-panel-bg`

## 5. 使用边界定义

### 5.1 品牌色

用于：

- 主按钮
- 核心激活态
- 关键数据走势
- 品牌识别

不用于：

- 告警状态
- 删除操作
- 大面积高饱和背景铺色

### 5.2 强调色

用于：

- 少量徽记点缀
- 少量数据高光
- 极少量装饰性强调

不用于：

- 主按钮主色
- warning 语义
- 大面积导航和 hero 主背景
- 图表主序列

### 5.3 Warning

用于：

- 风险提醒
- 处理中状态
- 非致命异常

不用于：

- 品牌识别
- 普通强调
- 统计重点数字装饰

### 5.4 Danger

用于：

- 严重告警
- 删除操作
- 高风险状态

不用于：

- 普通高亮
- 核心主操作

## 6. 组件落地策略

### 6.1 全局主题与基础样式

优先改造：

- `frontend/water-info-web/src/assets/styles/theme.scss`
- `frontend/water-info-web/src/assets/styles/base.scss`

目标：

- 建立完整 tokens
- 覆盖 Element Plus 主题变量
- 统一 body、panel、分区、边框、默认文字层级

### 6.2 布局组件

改造：

- `AppHeader.vue`
- `SidebarNav.vue`
- `PageCard.vue`

目标：

- 侧边栏改为稳定深蓝青反色体系
- 激活态从高饱和渐变改为克制的选中层级
- Header 和卡片回归中性色主导

### 6.3 首页 hero 与数据面板

改造：

- `DashboardView.vue`

目标：

- hero 保留行业氛围，但减少金色存在感
- 数据卡与统计块从彩色块转向中性色层级 + 品牌点缀

### 6.4 图表系统

改造：

- `TrendLineChart.vue`
- `StatBarChart.vue`
- `StatDonutChart.vue`
- 页面中的传色逻辑

目标：

- 统一坐标轴、网格线、图例文字
- 用 visualization token 替换硬编码系列色
- 避免把 warning 和 accent 混用

### 6.5 地图系统

改造：

- `MapView.vue`

目标：

- 地图点位色、图例色、地图侧栏统计块全部转为可视化语义 token
- 地图面板和详情面板遵循统一 surface 规则

### 6.6 Element Plus 组件覆盖

重点覆盖：

- 按钮
- 输入框
- 选择器
- 表格
- 标签
- 分页
- 弹窗 / 抽屉
- 空状态
- 描述列表

目标：

- 让组件库与业务页面共享同一套视觉语言
- 优先靠全局变量和基础样式实现，不做大面积逐页补丁

## 7. 实施顺序

1. 建立完整 global tokens 与 visualization tokens
2. 扩展 Element Plus 全局主题覆盖
3. 清理导航、hero、地图、图表中的硬编码颜色
4. 统一表格、标签、分页、表单、弹窗层级
5. 做局部细节优化
6. 每完成一个阶段执行一次页面回归检查

## 8. 验证重点

每个阶段都重点检查以下区域：

- 顶部导航
- 侧边栏
- 首页 hero
- 图表
- 地图
- 表格
- 标签
- 整体文字与背景对比度

验证方式包括：

- `npm run build`
- 视觉回归检查
- 页面级人工对比

