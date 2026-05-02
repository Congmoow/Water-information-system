/**
 * 从 tokens.ts 读取颜色值，动态写入 CSS 自定义属性。
 * tokens.ts 为颜色唯一来源；theme.scss 仅保留非颜色变量（间距、字体、动画等）。
 */
import { corePalette, semanticTokens, visualizationTokens } from '@/theme/tokens'

function setVar(el: HTMLElement, name: string, value: string) {
  el.style.setProperty(name, value)
}

/** 核心色板 */
const paletteVars: Record<string, string> = {
  '--wi-color-brand-50': corePalette.brand[50],
  '--wi-color-brand-100': corePalette.brand[100],
  '--wi-color-brand-200': corePalette.brand[200],
  '--wi-color-brand-300': corePalette.brand[300],
  '--wi-color-brand-400': corePalette.brand[400],
  '--wi-color-brand-500': corePalette.brand[500],
  '--wi-color-brand-600': corePalette.brand[600],
  '--wi-color-brand-700': corePalette.brand[700],
  '--wi-color-brand-800': corePalette.brand[800],
  '--wi-color-brand-900': corePalette.brand[900],
  '--wi-color-neutral-0': corePalette.neutral[0],
  '--wi-color-neutral-50': corePalette.neutral[50],
  '--wi-color-neutral-100': corePalette.neutral[100],
  '--wi-color-neutral-200': corePalette.neutral[200],
  '--wi-color-neutral-300': corePalette.neutral[300],
  '--wi-color-neutral-400': corePalette.neutral[400],
  '--wi-color-neutral-500': corePalette.neutral[500],
  '--wi-color-neutral-600': corePalette.neutral[600],
  '--wi-color-neutral-700': corePalette.neutral[700],
  '--wi-color-neutral-800': corePalette.neutral[800],
  '--wi-color-neutral-900': corePalette.neutral[900],
  '--wi-color-neutral-950': corePalette.neutral[950],
  '--wi-color-accent-100': corePalette.accent[100],
  '--wi-color-accent-200': corePalette.accent[200],
  '--wi-color-accent-300': corePalette.accent[300],
  '--wi-color-accent-400': corePalette.accent[400],
  '--wi-color-accent-500': corePalette.accent[500],
  '--wi-color-accent-600': corePalette.accent[600],
  '--wi-color-accent-700': corePalette.accent[700],
  '--wi-color-success-100': corePalette.success[100],
  '--wi-color-success-500': corePalette.success[500],
  '--wi-color-success-600': corePalette.success[600],
  '--wi-color-warning-100': corePalette.warning[100],
  '--wi-color-warning-500': corePalette.warning[500],
  '--wi-color-warning-600': corePalette.warning[600],
  '--wi-color-danger-100': corePalette.danger[100],
  '--wi-color-danger-500': corePalette.danger[500],
  '--wi-color-danger-600': corePalette.danger[600],
  '--wi-color-info-100': corePalette.info[100],
  '--wi-color-info-500': corePalette.info[500],
  '--wi-color-info-600': corePalette.info[600]
}

/** 语义层 */
const semanticVars: Record<string, string> = {
  /* 背景 */
  '--wi-bg-base': semanticTokens.neutral.background.base,
  '--wi-bg-subtle': corePalette.neutral[50],
  '--wi-bg-muted': semanticTokens.neutral.background.muted,

  /* 表面 */
  '--wi-surface-raised': semanticTokens.surface.raised,
  '--wi-surface-panel': semanticTokens.surface.panel,
  '--wi-surface-overlay': semanticTokens.surface.overlay,
  '--wi-surface-strong': semanticTokens.surface.strong,
  '--wi-surface-inverse': semanticTokens.inverse.surface,
  '--wi-surface-inverse-strong': semanticTokens.inverse.surfaceStrong,
  '--wi-surface-inverse-soft': semanticTokens.inverse.surfaceSoft,
  '--wi-surface-inverse-muted': semanticTokens.inverse.surfaceMuted,

  /* 文本 */
  '--wi-text-primary': semanticTokens.text.primary,
  '--wi-text-secondary': semanticTokens.text.secondary,
  '--wi-text-tertiary': semanticTokens.text.tertiary,
  '--wi-text-disabled': semanticTokens.text.disabled,
  '--wi-text-inverse-primary': semanticTokens.inverse.textPrimary,
  '--wi-text-inverse-secondary': semanticTokens.inverse.textSecondary,

  /* 装饰 alpha */
  '--wi-decorative-alpha-accent-glow-subtle': semanticTokens.visual.decorativeAlpha.accentGlowSubtle,
  '--wi-decorative-alpha-hero-overlay-soft': semanticTokens.visual.decorativeAlpha.heroOverlaySoft,
  '--wi-decorative-alpha-hero-overlay-medium': semanticTokens.visual.decorativeAlpha.heroOverlayMedium,
  '--wi-decorative-alpha-brand-glow': semanticTokens.visual.decorativeAlpha.brandGlow,
  '--wi-decorative-alpha-hero-overlay-strong': semanticTokens.visual.decorativeAlpha.heroOverlayStrong,
  '--wi-decorative-alpha-brand-glow-strong': semanticTokens.visual.decorativeAlpha.brandGlowStrong,
  '--wi-decorative-alpha-hero-particle': semanticTokens.visual.decorativeAlpha.heroParticle,

  /* 渐变 */
  '--wi-gradient-page-background': semanticTokens.visual.gradient.pageBackground,
  '--wi-gradient-inverse-hero': semanticTokens.visual.gradient.inverseHero,

  /* 覆层 */
  '--wi-overlay-decorative-strong': semanticTokens.visual.overlay.decorativeStrong,
  '--wi-overlay-decorative-medium': semanticTokens.visual.overlay.decorativeMedium,
  '--wi-overlay-decorative-soft': semanticTokens.visual.overlay.decorativeSoft,

  /* 光晕 */
  '--wi-glow-brand': semanticTokens.visual.glow.brand,
  '--wi-glow-brand-strong': semanticTokens.visual.glow.brandStrong,
  '--wi-glow-accent-subtle': semanticTokens.visual.glow.accentSubtle,

  /* 阴影 */
  '--wi-shadow-emphasis': semanticTokens.visual.shadow.emphasis,
  '--wi-shadow-inverse-strong': semanticTokens.visual.shadow.inverseStrong,
  '--wi-shadow-card': semanticTokens.visual.shadow.card,
  '--wi-shadow-overlay': semanticTokens.visual.shadow.overlay,

  /* 认证 */
  '--wi-auth-hero-background': semanticTokens.auth.heroBackground,
  '--wi-auth-hero-decorative-badge-bg': semanticTokens.auth.heroBadgeBg,
  '--wi-auth-hero-decorative-badge-border': semanticTokens.auth.heroBadgeBorder,
  '--wi-auth-hero-decorative-title-shadow': semanticTokens.auth.heroTitleShadow,
  '--wi-auth-hero-overlay-strong': semanticTokens.auth.heroOverlayStrong,
  '--wi-auth-hero-overlay-medium': semanticTokens.auth.heroOverlayMedium,
  '--wi-auth-hero-overlay-soft': semanticTokens.auth.heroOverlaySoft,
  '--wi-auth-hero-decorative-particle': semanticTokens.auth.heroParticle,
  '--wi-auth-glass-card-bg': semanticTokens.auth.cardBg,
  '--wi-auth-glass-card-shadow': semanticTokens.auth.cardShadow,
  '--wi-auth-input-background': semanticTokens.auth.inputBg,
  '--wi-auth-input-focus-shadow': semanticTokens.auth.inputFocusShadow,
  /* 认证短别名 */
  '--wi-auth-hero-bg': semanticTokens.auth.heroBackground,
  '--wi-auth-hero-badge-bg': semanticTokens.auth.heroBadgeBg,
  '--wi-auth-hero-badge-border': semanticTokens.auth.heroBadgeBorder,
  '--wi-auth-hero-title-shadow': semanticTokens.auth.heroTitleShadow,
  '--wi-auth-hero-particle': semanticTokens.auth.heroParticle,
  '--wi-auth-card-bg': semanticTokens.auth.cardBg,
  '--wi-auth-card-shadow': semanticTokens.auth.cardShadow,
  '--wi-auth-input-bg': semanticTokens.auth.inputBg,

  /* 应用外壳 */
  '--wi-app-bg': semanticTokens.app.background.canvas,
  '--wi-app-bg-subtle': semanticTokens.app.background.subtle,
  '--wi-app-surface': semanticTokens.app.surface.primary,
  '--wi-app-surface-secondary': semanticTokens.app.surface.secondary,
  '--wi-app-surface-tertiary': semanticTokens.app.surface.tertiary,
  '--wi-app-border-subtle': semanticTokens.app.border.subtle,
  '--wi-app-border': semanticTokens.app.border.default,
  '--wi-app-border-strong': semanticTokens.app.border.strong,
  '--wi-app-sidebar-bg': semanticTokens.app.inverse.sidebar,
  '--wi-app-sidebar-surface': semanticTokens.app.inverse.surface,
  '--wi-app-sidebar-surface-hover': semanticTokens.app.inverse.surfaceHover,
  '--wi-app-sidebar-surface-active': semanticTokens.app.inverse.surfaceActive,
  '--wi-app-sidebar-border': semanticTokens.app.inverse.border,
  '--wi-app-sidebar-text-secondary': semanticTokens.app.inverse.textSecondary,

  /* 边框 */
  '--wi-border-subtle': semanticTokens.border.subtle,
  '--wi-border-default': semanticTokens.border.default,
  '--wi-border-strong': semanticTokens.border.strong,
  '--wi-border-inverse': semanticTokens.border.inverse,

  /* 品牌语义别名 */
  '--wi-primary': semanticTokens.brand.primary,
  '--wi-primary-hover': semanticTokens.brand.primaryHover,
  '--wi-primary-active': semanticTokens.brand.primaryActive,
  '--wi-primary-soft': semanticTokens.brand.primarySoft,
  '--wi-primary-contrast': semanticTokens.brand.primaryContrast,
  '--wi-accent': semanticTokens.brand.accent,
  '--wi-accent-soft': semanticTokens.brand.accentSoft,
  '--wi-success': semanticTokens.state.success.default,
  '--wi-success-soft': semanticTokens.state.success.soft,
  '--wi-warning': semanticTokens.state.warning.default,
  '--wi-warning-soft': semanticTokens.state.warning.soft,
  '--wi-danger': semanticTokens.state.danger.default,
  '--wi-danger-soft': semanticTokens.state.danger.soft,
  '--wi-info': semanticTokens.state.info.default,
  '--wi-info-soft': semanticTokens.state.info.soft,

  /* 交互 */
  '--wi-focus-ring': semanticTokens.interactive.focusRing,
  '--wi-disabled-bg': semanticTokens.interactive.disabledBg,
  '--wi-disabled-border': semanticTokens.interactive.disabledBorder,
  '--wi-selection-bg': 'rgba(26, 111, 181, 0.18)',
  '--wi-interactive-soft-hover': 'rgba(26, 111, 181, 0.04)',
  '--wi-interactive-soft-active': 'rgba(26, 111, 181, 0.06)',
  '--wi-shadow-primary': semanticTokens.visual.shadow.emphasis,

  /* 图表 */
  '--wi-chart-axis': visualizationTokens.chart.axis,
  '--wi-chart-grid': visualizationTokens.chart.grid,
  '--wi-chart-text': visualizationTokens.chart.text,
  '--wi-chart-comparison-primary': visualizationTokens.chart.comparison.primary,
  '--wi-chart-comparison-secondary': visualizationTokens.chart.comparison.secondary,
  '--wi-chart-comparison-secondary-muted': visualizationTokens.chart.comparisonSecondary.secondary,
  '--wi-chart-trend-primary-line': visualizationTokens.chart.trend.primary.line,
  '--wi-chart-trend-primary-area': visualizationTokens.chart.trend.primary.area,
  '--wi-chart-trend-secondary-line': visualizationTokens.chart.trend.secondary.line,
  '--wi-chart-trend-secondary-area': visualizationTokens.chart.trend.secondary.area,
  '--wi-chart-series-accent': visualizationTokens.chart.seriesAccent,
  '--wi-chart-series-neutral': visualizationTokens.chart.seriesNeutral,
  '--wi-chart-series-success': visualizationTokens.chart.seriesSuccess,
  '--wi-chart-series-info': visualizationTokens.chart.seriesInfo,
  '--wi-chart-series-warning': visualizationTokens.chart.seriesWarning,
  '--wi-chart-series-danger': visualizationTokens.chart.seriesDanger,

  /* 地图 */
  '--wi-map-river': visualizationTokens.map.river,
  '--wi-map-station': visualizationTokens.map.station,
  '--wi-map-marker-stroke': visualizationTokens.map.markerStroke,
  '--wi-map-panel-bg': visualizationTokens.map.panelBg,

  /* Element Plus 对接 */
  '--el-color-primary': semanticTokens.brand.primary,
  '--el-color-primary-dark-2': semanticTokens.brand.primaryActive,
  '--el-color-primary-light-3': corePalette.brand[400],
  '--el-color-primary-light-5': corePalette.brand[300],
  '--el-color-primary-light-7': corePalette.brand[200],
  '--el-color-primary-light-8': corePalette.brand[100],
  '--el-color-primary-light-9': corePalette.brand[50],
  '--el-color-success': semanticTokens.state.success.default,
  '--el-color-success-dark-2': corePalette.success[600],
  '--el-color-success-light-9': corePalette.success[100],
  '--el-color-warning': semanticTokens.state.warning.default,
  '--el-color-warning-dark-2': corePalette.warning[600],
  '--el-color-warning-light-9': corePalette.warning[100],
  '--el-color-danger': semanticTokens.state.danger.default,
  '--el-color-danger-dark-2': corePalette.danger[600],
  '--el-color-danger-light-9': corePalette.danger[100],
  '--el-color-info': semanticTokens.state.info.default,
  '--el-color-info-dark-2': corePalette.info[600],
  '--el-color-info-light-9': corePalette.info[100],
  '--el-bg-color': semanticTokens.surface.overlay,
  '--el-bg-color-page': semanticTokens.neutral.background.base,
  '--el-bg-color-overlay': semanticTokens.surface.overlay,
  '--el-fill-color-blank': semanticTokens.surface.overlay,
  '--el-fill-color-light': semanticTokens.neutral.background.muted,
  '--el-text-color-primary': semanticTokens.text.primary,
  '--el-text-color-regular': semanticTokens.text.secondary,
  '--el-text-color-secondary': semanticTokens.text.tertiary,
  '--el-text-color-placeholder': semanticTokens.text.tertiary,
  '--el-text-color-disabled': semanticTokens.text.disabled,
  '--el-border-color': semanticTokens.border.default,
  '--el-border-color-light': semanticTokens.border.subtle,
  '--el-box-shadow': semanticTokens.visual.shadow.overlay,
  '--el-box-shadow-light': semanticTokens.visual.shadow.card,
  '--el-disabled-bg-color': semanticTokens.interactive.disabledBg,
  '--el-disabled-border-color': semanticTokens.interactive.disabledBorder,
  '--el-disabled-text-color': semanticTokens.text.disabled
}

/**
 * 将 tokens.ts 中的颜色值写入 :root CSS 自定义属性。
 * 必须在 Vue 挂载前同步调用，确保组件渲染时变量已就绪。
 */
export function applyCssVariables(root: HTMLElement = document.documentElement) {
  for (const [name, value] of Object.entries(paletteVars)) {
    setVar(root, name, value)
  }
  for (const [name, value] of Object.entries(semanticVars)) {
    setVar(root, name, value)
  }
}
