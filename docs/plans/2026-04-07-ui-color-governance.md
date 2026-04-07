# UI Color Governance Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Rebuild the frontend color system around maintainable tokens, consistent Element Plus theming, and visualization semantics without changing the product structure or business logic.

**Architecture:** The implementation will centralize raw colors into SCSS design tokens, map them into semantic and visualization tokens, then bind both the app shell and Element Plus to those tokens. Business pages will stop using hardcoded colors and instead consume shared aliases for navigation, hero surfaces, charts, map markers, and status states.

**Tech Stack:** Vue 3, TypeScript, Vite, SCSS, Element Plus, ECharts, Leaflet

---

### Task 1: Add token infrastructure and token tests

**Files:**
- Modify: `frontend/water-info-web/package.json`
- Create: `frontend/water-info-web/vitest.config.ts`
- Create: `frontend/water-info-web/src/theme/tokens.ts`
- Create: `frontend/water-info-web/src/theme/tokens.test.ts`
- Modify: `frontend/water-info-web/src/assets/styles/theme.scss`

**Step 1: Write the failing test**

Add tests in `frontend/water-info-web/src/theme/tokens.test.ts` that assert:
- semantic token groups exist for brand, neutral, text, border, surface, state, inverse, chart, and map
- warning and accent resolve to different values
- chart and map color aliases are defined independently from brand aliases

**Step 2: Run test to verify it fails**

Run: `npm run test -- src/theme/tokens.test.ts`
Expected: fail because the test runner and token module do not exist yet

**Step 3: Add minimal test infrastructure**

Add `vitest` script and config so token tests can run under Vite.

**Step 4: Write minimal implementation**

Create `src/theme/tokens.ts` with exported token maps for:
- core palette
- semantic token groups
- visualization token groups

Mirror the same structure into `theme.scss`.

**Step 5: Run test to verify it passes**

Run: `npm run test -- src/theme/tokens.test.ts`
Expected: pass

### Task 2: Bind global styles and Element Plus to the new semantic tokens

**Files:**
- Modify: `frontend/water-info-web/src/assets/styles/theme.scss`
- Modify: `frontend/water-info-web/src/assets/styles/base.scss`
- Modify: `frontend/water-info-web/src/main.ts`

**Step 1: Write the failing test**

Extend `src/theme/tokens.test.ts` with assertions for:
- primary, accent, warning, and danger usage boundaries
- inverse and surface token aliases exist
- chart axis/grid tokens and map marker tokens are present

**Step 2: Run test to verify it fails**

Run: `npm run test -- src/theme/tokens.test.ts`
Expected: fail because aliases are incomplete

**Step 3: Write minimal implementation**

Update `theme.scss` and `base.scss` to:
- define complete token variables
- override Element Plus variables for surfaces, text, borders, buttons, inputs, tags, tables, dialogs, drawers, pagination, empty states
- establish default panel, toolbar, table, focus ring, and inverse-surface styles

**Step 4: Run tests**

Run: `npm run test -- src/theme/tokens.test.ts`
Expected: pass

**Step 5: Run build**

Run: `npm run build`
Expected: build succeeds

### Task 3: Remove hardcoded colors from layout and dashboard surfaces

**Files:**
- Modify: `frontend/water-info-web/src/layout/components/AppHeader.vue`
- Modify: `frontend/water-info-web/src/layout/components/SidebarNav.vue`
- Modify: `frontend/water-info-web/src/components/common/PageCard.vue`
- Modify: `frontend/water-info-web/src/views/auth/LoginView.vue`
- Modify: `frontend/water-info-web/src/views/dashboard/DashboardView.vue`

**Step 1: Write the failing test**

Add token regression assertions in `src/theme/tokens.test.ts` for the expected aliases used by:
- navigation inverse surface
- hero surface
- metric surface
- brand emphasis without accent/warning collisions

**Step 2: Run test to verify it fails**

Run: `npm run test -- src/theme/tokens.test.ts`
Expected: fail because the new aliases are not yet exported

**Step 3: Write minimal implementation**

Replace hardcoded gradients and text colors in the layout and dashboard with semantic tokens. Keep the hero visually strong but make blue-green the dominant axis and reduce gold to subtle emphasis.

**Step 4: Run tests**

Run: `npm run test -- src/theme/tokens.test.ts`
Expected: pass

**Step 5: Run build**

Run: `npm run build`
Expected: build succeeds

**Step 6: Run regression check**

Run the local app and visually inspect:
- navigation
- header
- dashboard hero
- metric cards
- table contrast

### Task 4: Move charts and map to visualization tokens

**Files:**
- Modify: `frontend/water-info-web/src/components/charts/TrendLineChart.vue`
- Modify: `frontend/water-info-web/src/components/charts/StatBarChart.vue`
- Modify: `frontend/water-info-web/src/components/charts/StatDonutChart.vue`
- Modify: `frontend/water-info-web/src/views/dashboard/DashboardView.vue`
- Modify: `frontend/water-info-web/src/views/monitoring/MonitoringView.vue`
- Modify: `frontend/water-info-web/src/views/map/MapView.vue`
- Modify: `frontend/water-info-web/src/theme/tokens.ts`

**Step 1: Write the failing test**

Add tests for visualization token exports covering:
- chart axis/grid/text aliases
- chart series aliases
- map engineering, river, station, warning, offline aliases

**Step 2: Run test to verify it fails**

Run: `npm run test -- src/theme/tokens.test.ts`
Expected: fail because the visualization aliases are incomplete or unused

**Step 3: Write minimal implementation**

Refactor chart components and map color selection to read from shared token-backed helpers instead of hardcoded strings.

**Step 4: Run tests**

Run: `npm run test -- src/theme/tokens.test.ts`
Expected: pass

**Step 5: Run build**

Run: `npm run build`
Expected: build succeeds

**Step 6: Run regression check**

Visually inspect:
- line charts
- bar chart
- donut chart
- map markers
- map legend
- map side panels

### Task 5: Finish Element Plus consistency and page-level cleanup

**Files:**
- Modify: `frontend/water-info-web/src/views/alarm/AlarmView.vue`
- Modify: `frontend/water-info-web/src/views/monitoring/MonitoringView.vue`
- Modify: `frontend/water-info-web/src/views/station/StationView.vue`
- Modify: `frontend/water-info-web/src/views/reservoir/ReservoirView.vue`
- Modify: `frontend/water-info-web/src/views/river/RiverView.vue`
- Modify: `frontend/water-info-web/src/assets/styles/base.scss`

**Step 1: Write the failing test**

Add tests for status alias boundaries ensuring:
- warning differs from accent
- danger differs from primary
- info differs from primary and warning

**Step 2: Run test to verify it fails**

Run: `npm run test -- src/theme/tokens.test.ts`
Expected: fail until final aliases are aligned

**Step 3: Write minimal implementation**

Clean up remaining page-level color usage and align:
- tags
- pagination
- toolbars
- dialogs
- drawers
- table action links
- empty states

**Step 4: Run tests**

Run: `npm run test -- src/theme/tokens.test.ts`
Expected: pass

**Step 5: Run build**

Run: `npm run build`
Expected: build succeeds

**Step 6: Run regression check**

Visually inspect:
- table hierarchy
- tag semantics
- pagination
- form controls
- dialog and drawer contrast

### Acceptance Checklist

- Brand color is blue-green led and gold is limited to small emphasis
- Warning and danger are fully independent from brand and accent tokens
- Global semantic tokens exist for surface, inverse, border, text, and state layers
- Visualization tokens exist for charts and maps
- Navigation, hero, charts, and map no longer rely on raw hardcoded colors
- Element Plus surfaces and controls inherit the same token system
- `npm run test -- src/theme/tokens.test.ts` passes
- `npm run build` passes
- Page regression checks are completed for navigation, hero, charts, map, tables, tags, and contrast
