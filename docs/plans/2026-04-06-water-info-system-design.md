# 水利信息管理系统设计说明

## 1. 设计依据

本设计严格依据以下文档，冲突时按优先级处理：

1. 《项目规格需求说明书》
2. 《项目功能说明书》
3. 《项目结构说明书》

## 2. 目标与范围

本项目实现一个适合课程答辩演示的“水利信息管理系统”，满足以下范围：

- 前后端分离
- 后端使用 ASP.NET Core Web API
- 前端使用 Vue 3 + TypeScript
- 数据库使用 SQL Server
- JWT 认证
- OpenAPI + Scalar 接口文档
- 支持用户权限、工程管理、站点管理、监测数据、告警、统计分析、地图展示、用户中心

不引入文档外的复杂架构，不扩展微服务、消息队列、Redis、CQRS、MediatR 等方案。

## 3. 总体架构

系统采用前后端分离架构：

- 前端负责页面渲染、交互、图表和地图展示。
- 后端负责认证、业务编排、数据访问协调和接口输出。
- SQL Server 负责业务数据持久化。

请求链路如下：

`Vue 3 + TypeScript -> Axios -> ASP.NET Core Web API -> EF Core -> SQL Server`

## 4. 后端设计

后端严格按文档分层组织：

- `WaterInfoSystem.API`
  - 负责控制器、中间件、扩展配置、OpenAPI 与 Scalar
- `WaterInfoSystem.Application`
  - 负责 DTO、服务接口、业务服务、映射、查询条件、业务编排
- `WaterInfoSystem.Domain`
  - 负责实体、枚举、阈值常量
- `WaterInfoSystem.Infrastructure`
  - 负责 `DbContext`、实体配置、仓储实现、JWT 服务、密码哈希、种子数据
- `WaterInfoSystem.Shared`
  - 负责统一返回结构、分页模型、异常类型、公共常量

约束如下：

- Controller 只处理入参、调用 Service、返回结果
- Service 不直接依赖 `DbContext`
- Repository 仅负责数据访问
- DTO 与 Entity 完全分离
- Domain 不依赖 Infrastructure
- Shared 不承载业务逻辑

## 5. 数据模型设计

核心实体如下：

- `User`
- `Reservoir`
- `River`
- `Station`
- `MonitoringData`
- `AlarmRecord`

辅助枚举如下：

- `UserRole`
- `StationType`
- `StationStatus`
- `MonitoringDataType`
- `AlarmLevel`
- `AlarmStatus`
- `MapPointType`

关键关系如下：

- 站点可关联水库或河道，用于地图展示和业务归属
- 监测数据从属于站点
- 告警记录基于监测数据与站点信息生成
- 用户仅保留管理员和普通用户两类角色

## 6. 业务规则设计

### 6.1 权限

- 管理员可执行新增、编辑、删除、处理告警
- 普通用户仅能查看、查询、详情浏览
- 前端按角色控制菜单和页面操作按钮
- 后端按角色控制写操作接口

### 6.2 监测与告警

- 新增监测数据时按数据类型和站点阈值进行判断
- 超阈值时自动生成告警记录
- 告警记录包含类型、等级、状态、时间、说明
- 支持更新告警处理状态和处理备注

### 6.3 统计

- 首页统计卡片展示工程总数、站点总数、今日告警数、在线站点数
- 图表展示水位趋势、雨量统计、告警数量、站点状态分布
- 数据来源全部走后端聚合接口

### 6.4 地图

- 地图页展示水库、河道关联工程点位和监测站点点位
- 不强依赖第三方在线地图能力，采用 Leaflet + 开源底图
- 点击点位后弹出详情卡片，显示名称、类型、状态、说明等信息

## 7. 前端设计

前端目录按文档组织：

- `api/modules`
- `components/common`
- `components/business`
- `components/charts`
- `composables`
- `layout`
- `router`
- `stores`
- `types`
- `views`

页面范围如下：

- 登录页
- 首页仪表盘
- 水库管理页
- 河道管理页
- 站点管理页
- 监测数据页
- 告警记录页
- 地图展示页
- 用户中心页

交互设计如下：

- 使用统一后台布局，包括侧边栏、顶部栏、内容区
- 使用 Element Plus 完成表格、表单、抽屉、对话框、状态标签
- 使用 ECharts 封装业务图表组件
- 使用 Leaflet 展示地图点位和详情弹窗

## 8. 运行与初始化策略

- 后端配置 SQL Server 连接字符串、JWT 参数、OpenAPI + Scalar
- 提供数据库初始化脚本与 EF Core 种子数据
- 前端提供 `.env.development` 和 `.env.production`
- 默认内置演示数据，避免空页面

## 9. 实施顺序

1. 初始化目录、解决方案、项目依赖、基础配置
2. 实现登录认证、用户信息、布局与路由守卫
3. 实现水库、河道、站点 CRUD
4. 实现监测数据录入、查询、趋势
5. 实现告警自动生成与处理
6. 实现首页统计和地图展示
7. 完成种子数据、异常处理、自测与运行说明

## 10. 风险与控制

- 仓库当前无现成代码，需要从零初始化，因此优先保证结构和主流程跑通
- 本地未发现 `sqllocaldb`，数据库将按标准 SQL Server 连接串配置，并提供可修改配置项
- 仓库当前不是 Git 工作树，无法按工作流提交设计文档，只能保留本地文件记录
