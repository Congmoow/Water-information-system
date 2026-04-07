# UI 色彩治理使用说明

## 1. 目的

本文件用于约束项目后续 UI 色彩的新增、修改和评审方式，避免重新出现颜色散落、语义混用、强调色滥用和页面风格失控的问题。

当前项目的色彩体系以以下文件为唯一主题源：

- `frontend/water-info-web/src/theme/tokens.ts`
- `frontend/water-info-web/src/assets/styles/theme.scss`

后续所有颜色相关改动，应优先落在这两个文件中，而不是直接在业务页面或组件中写死颜色值。

## 2. 必须遵守的规则

### 2.1 新颜色先加到 token，不要直接写 hex

新增颜色时，必须先判断是否属于以下类别：

- 品牌色
- 中性色
- 语义状态色
- surface / 背景层级色
- 边框色
- 文字色
- 图表语义色
- 地图语义色

如果属于以上任一类别，必须先补到：

- `tokens.ts`
- `theme.scss`

禁止直接在业务页面、布局组件、图表页面中新增裸 `hex / rgb / rgba` 颜色作为最终方案。

### 2.2 图表和地图只能使用 visualization token

图表和地图属于数据表达层，不得直接复用页面品牌色逻辑，不得在页面里传裸色值。

必须使用：

- `visualizationTokens.chart.*`
- `visualizationTokens.map.*`
- `chartSeries`
- `lineChartTokens`

适用范围包括但不限于：

- ECharts 折线、柱状图、环图
- 坐标轴颜色
- 网格线颜色
- 图例文字颜色
- 地图 marker
- 地图图例
- 地图统计块中的数据类别颜色

### 2.3 页面组件优先使用 semantic token 或 Element Plus 语义 type

页面组件的颜色来源优先级如下：

1. semantic token
2. Element Plus 语义 type
3. visualization token

不要在页面组件里重新发明新的状态色逻辑。

例如：

- 按钮、输入框、表格、分页、弹窗、抽屉、空状态，应优先依赖全局主题变量和 Element Plus 主题覆盖
- 标签、状态文本、提醒信息，应优先使用 `success / warning / danger / info`
- 深色导航、深色 hero 等反色区域，应优先使用 `inverse` 相关 token

### 2.4 不要把 accent 当 warning

`accent` 与 `warning` 的角色不同，不能混用。

- `accent`
  - 用于少量视觉强调
  - 可用于徽记、点缀、高光、少量装饰性强调
- `warning`
  - 只用于风险提醒、处理中状态、非致命异常

禁止：

- 用 `accent` 充当告警色
- 用金色代替 warning tag
- 在图表或地图中把强调色当作 warning / alarm 语义

## 3. 使用边界

### 3.1 品牌色

品牌色仅用于：

- 主按钮
- 关键激活态
- 核心趋势图主序列
- 品牌识别

品牌色不用于：

- 删除操作
- 严重告警
- 大面积高饱和背景铺色

### 3.2 强调色

强调色仅用于：

- 徽记高光
- 少量视觉点睛
- 局部数字强调

强调色不用于：

- warning
- danger
- 地图风险点位
- 图表告警序列

### 3.3 语义色

固定语义如下：

- `success`: 成功、正常、已完成
- `warning`: 风险、处理中、预警
- `danger`: 严重、删除、高风险、失败
- `info`: 中性状态、离线、说明性状态

同一语义在不同组件中必须保持一致。

## 4. 推荐做法

### 4.1 页面改色

优先检查是否已经有可用 token：

- `semanticTokens.brand`
- `semanticTokens.surface`
- `semanticTokens.text`
- `semanticTokens.border`
- `semanticTokens.state`
- `semanticTokens.inverse`

如果已有，就直接引用，不要重复新增。

### 4.2 新增图表

新增图表时：

- 系列色从 `chartSeries` 或 `visualizationTokens.chart.*` 中选
- 坐标轴、网格线、图例文字统一走图表语义 token
- 不要在页面里内联 `#0f6c7b`、`#d08c2e` 这类颜色

### 4.3 新增地图标记或图例

新增地图点位或图例时：

- 统一走 `visualizationTokens.map.*`
- 工程类、河道类、站点类、warning、offline 必须各自独立
- 不要临时使用 accent 或品牌色替代语义状态

## 5. Code Review 检查项

后续涉及前端 UI 的 code review，应明确检查以下内容：

### 5.1 颜色来源检查

- 业务文件中是否新增了裸色值
- 新颜色是否先进入了 `tokens.ts` 和 `theme.scss`
- 是否存在页面级重复定义颜色

### 5.2 语义边界检查

- 是否把 accent 当 warning 使用
- 是否把品牌色用于 danger / delete / alarm
- 是否出现 success / warning / danger / info 混用

### 5.3 可视化检查

- 图表是否使用 visualization token
- 地图是否使用 visualization token
- 页面是否又从页面组件里传裸色值到图表或地图

### 5.4 层级与可读性检查

- 背景、surface、边框、文字层级是否清晰
- 深色区域是否使用了 inverse token
- 表格、表单、标签、分页是否仍在统一主题之下

## 6. 禁止事项

以下做法默认视为不符合规范：

- 在业务页面中直接新增十六进制颜色
- 在图表页面里写死系列色
- 在地图页面里手写 marker 颜色
- 用 accent 充当 warning
- 用品牌色充当 danger
- 为了局部“好看”绕过 token 系统

## 7. 维护建议

- 如果颜色需求是全局性的，先改 token，再看组件表现
- 如果颜色需求只影响数据表达，优先改 visualization token
- 如果颜色需求只是局部问题，先检查是否能用现有 semantic token 解决
- 只有当现有 token 无法表达该语义时，才允许新增 token

当本文件与局部实现冲突时，以本文件和 `tokens.ts / theme.scss` 的主题治理原则为准。
