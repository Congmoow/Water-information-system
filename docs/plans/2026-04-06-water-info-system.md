# 水利信息管理系统 Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** 构建一个符合项目文档要求、可运行、可演示的水利信息管理系统。

**Architecture:** 后端采用 ASP.NET Core Web API 五层分层结构，前端采用 Vue 3 + TypeScript 单体后台管理应用。所有业务围绕用户、工程、站点、监测数据、告警和统计地图展开，优先保证主流程和文档一致性。

**Tech Stack:** ASP.NET Core 8, EF Core, SQL Server, JWT, OpenAPI, Scalar, Vue 3, TypeScript, Vite, Pinia, Axios, Element Plus, ECharts, Leaflet

---

### Task 1: 初始化目录和解决方案

**Files:**
- Create: `backend/WaterInfoSystem.sln`
- Create: `backend/src/WaterInfoSystem.API/`
- Create: `backend/src/WaterInfoSystem.Application/`
- Create: `backend/src/WaterInfoSystem.Domain/`
- Create: `backend/src/WaterInfoSystem.Infrastructure/`
- Create: `backend/src/WaterInfoSystem.Shared/`
- Create: `frontend/water-info-web/`
- Create: `sql/init/`
- Create: `sql/backup/`
- Create: `assets/screenshots/`
- Create: `assets/diagrams/`
- Create: `assets/demo-data/`

**Step 1: 创建后端解决方案与项目**

Run: `dotnet new sln -n WaterInfoSystem`

**Step 2: 创建后端五层项目**

Run: `dotnet new webapi`, `dotnet new classlib`

**Step 3: 建立项目引用关系**

Run: `dotnet sln add ...`, `dotnet add reference ...`

**Step 4: 创建前端工程**

Run: `npm create vite@latest frontend/water-info-web -- --template vue-ts`

**Step 5: 验证脚手架成功**

Run: `dotnet sln list`, `npm install`

### Task 2: 后端基础设施与公共层

**Files:**
- Modify: `backend/src/WaterInfoSystem.API/Program.cs`
- Create: `backend/src/WaterInfoSystem.API/Extensions/`
- Create: `backend/src/WaterInfoSystem.API/Middleware/`
- Create: `backend/src/WaterInfoSystem.Shared/Results/`
- Create: `backend/src/WaterInfoSystem.Shared/Exceptions/`
- Create: `backend/src/WaterInfoSystem.Infrastructure/Persistence/`
- Create: `backend/src/WaterInfoSystem.Infrastructure/DependencyInjection/`
- Create: `backend/src/WaterInfoSystem.Infrastructure/Identity/`

**Step 1: 建立统一返回结构与分页模型**

**Step 2: 建立异常处理中间件**

**Step 3: 配置 EF Core、SQL Server、JWT、OpenAPI、Scalar**

**Step 4: 写最小健康运行验证**

Run: `dotnet build`

### Task 3: 认证与用户骨架

**Files:**
- Create: `backend/src/WaterInfoSystem.Domain/Entities/User.cs`
- Create: `backend/src/WaterInfoSystem.Domain/Enums/UserRole.cs`
- Create: `backend/src/WaterInfoSystem.Application/Contracts/Auth/`
- Create: `backend/src/WaterInfoSystem.Application/Interfaces/IAuthService.cs`
- Create: `backend/src/WaterInfoSystem.Application/Services/AuthService.cs`
- Create: `backend/src/WaterInfoSystem.Infrastructure/Repositories/UserRepository.cs`
- Create: `backend/src/WaterInfoSystem.API/Controllers/AuthController.cs`

**Step 1: 实现用户实体、DTO 和仓储接口**

**Step 2: 实现密码哈希与 JWT 令牌生成**

**Step 3: 实现登录、获取当前用户接口**

**Step 4: 加入初始化管理员和普通用户**

**Step 5: 验证认证主流程**

Run: `dotnet build`

### Task 4: 前端基础布局与登录流程

**Files:**
- Create: `frontend/water-info-web/src/api/http.ts`
- Create: `frontend/water-info-web/src/api/modules/`
- Create: `frontend/water-info-web/src/router/`
- Create: `frontend/water-info-web/src/stores/`
- Create: `frontend/water-info-web/src/layout/`
- Create: `frontend/water-info-web/src/views/auth/`
- Create: `frontend/water-info-web/src/views/dashboard/`
- Create: `frontend/water-info-web/src/types/`

**Step 1: 配置 Element Plus、Pinia、Router、Axios**

**Step 2: 实现登录页**

**Step 3: 实现登录状态存储、用户信息加载、路由守卫**

**Step 4: 实现后台主布局、侧边栏、顶部栏**

**Step 5: 验证前端可启动**

Run: `npm run build`

### Task 5: 基础信息管理模块

**Files:**
- Create: `backend/src/.../Reservoir*`
- Create: `backend/src/.../River*`
- Create: `backend/src/.../Station*`
- Create: `frontend/water-info-web/src/views/reservoir/`
- Create: `frontend/water-info-web/src/views/river/`
- Create: `frontend/water-info-web/src/views/station/`
- Create: `frontend/water-info-web/src/api/modules/reservoir.ts`
- Create: `frontend/water-info-web/src/api/modules/river.ts`
- Create: `frontend/water-info-web/src/api/modules/station.ts`

**Step 1: 实现三类实体、DTO、仓储、服务、控制器**

**Step 2: 实现列表、详情、新增、编辑、删除接口**

**Step 3: 实现三类管理页面和表单对话框**

**Step 4: 接入角色控制按钮**

**Step 5: 验证 CRUD 主流程**

Run: `dotnet build`, `npm run build`

### Task 6: 监测数据与告警模块

**Files:**
- Create: `backend/src/.../Monitoring*`
- Create: `backend/src/.../Alarm*`
- Create: `frontend/water-info-web/src/views/monitoring/`
- Create: `frontend/water-info-web/src/views/alarm/`
- Create: `frontend/water-info-web/src/components/charts/`

**Step 1: 实现监测数据录入与查询接口**

**Step 2: 实现阈值判断和自动生成告警**

**Step 3: 实现告警状态处理接口**

**Step 4: 实现监测数据页、趋势图、告警页**

**Step 5: 验证查询、筛选和处理闭环**

Run: `dotnet build`, `npm run build`

### Task 7: 统计分析、地图与完善

**Files:**
- Create: `backend/src/WaterInfoSystem.API/Controllers/DashboardController.cs`
- Create: `frontend/water-info-web/src/views/map/`
- Create: `frontend/water-info-web/src/views/user-center/`
- Create: `sql/init/01-schema.sql`
- Create: `sql/init/02-seed.sql`
- Modify: `README.md`

**Step 1: 实现统计聚合接口**

**Step 2: 实现首页卡片、图表、最近告警**

**Step 3: 实现地图点位和详情弹窗**

**Step 4: 实现用户中心**

**Step 5: 补齐 SQL 初始化脚本、运行说明和演示数据说明**

**Step 6: 最终验证**

Run: `dotnet build`
Run: `npm run build`

### 验收检查

- 登录、获取当前用户、退出前端流程可走通
- 管理员与普通用户菜单和写操作权限有区分
- 水库、河道、站点完成 CRUD
- 监测数据支持录入、站点筛选、时间范围筛选、历史展示
- 告警支持自动生成、列表查询、状态处理
- 首页图表和地图展示可用
- OpenAPI 和 Scalar 可访问
- 项目启动后有演示数据，不是空页面
