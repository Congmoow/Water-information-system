export const corePalette = {
  brand: {
    50: '#eef5fb',
    100: '#d9e9f7',
    200: '#b8d4ee',
    300: '#8db8e2',
    400: '#5d96d2',
    500: '#2f7dc1',
    600: '#1a6fb5',
    700: '#155b95',
    800: '#124a79',
    900: '#103b61'
  },
  neutral: {
    0: '#ffffff',
    50: '#f7fafc',
    100: '#f1f5f9',
    200: '#e2e8f0',
    300: '#cbd5e1',
    400: '#94a3b8',
    500: '#64748b',
    600: '#475569',
    700: '#334155',
    800: '#1f2937',
    900: '#0f172a',
    950: '#0a1220'
  },
  accent: {
    100: '#f6ead0',
    200: '#ecd7aa',
    300: '#dbbc74',
    400: '#c99b43',
    500: '#b9852f',
    600: '#966a26',
    700: '#72501f'
  },
  success: {
    100: '#dcf4e7',
    500: '#1f8a5b',
    600: '#176c47'
  },
  warning: {
    100: '#fce9cb',
    500: '#c97d1f',
    600: '#a46116'
  },
  danger: {
    100: '#fde2e2',
    500: '#c84b4b',
    600: '#a13c3c'
  },
  info: {
    100: '#ddeff7',
    500: '#4e7a95',
    600: '#3f6278'
  }
} as const

const decorativeAlphaTokens = {
  accentGlowSubtle: '0.08',
  heroOverlaySoft: '0.1',
  heroOverlayMedium: '0.15',
  brandGlow: '0.18',
  heroOverlayStrong: '0.2',
  brandGlowStrong: '0.22',
  heroParticle: '0.3'
} as const

const visualTokens = {
  gradient: {
    /** 主工作区背景：中性层级为主，避免强装饰径向/强调色铺陈（Design §2、§6） */
    pageBackground: `linear-gradient(180deg, ${corePalette.neutral[50]} 0%, #f5f8fb 48%, ${corePalette.neutral[100]} 100%)`,
    inverseHero: 'linear-gradient(135deg, #0e1e38, rgba(26, 54, 93, 0.94))'
  },
  glow: {
    brand: `rgba(93, 150, 210, ${decorativeAlphaTokens.brandGlow})`,
    brandStrong: `rgba(47, 125, 193, ${decorativeAlphaTokens.brandGlowStrong})`,
    accentSubtle: `rgba(201, 155, 67, ${decorativeAlphaTokens.accentGlowSubtle})`
  },
  overlay: {
    decorativeStrong: `rgba(247, 251, 253, ${decorativeAlphaTokens.heroOverlayStrong})`,
    decorativeMedium: `rgba(247, 251, 253, ${decorativeAlphaTokens.heroOverlayMedium})`,
    decorativeSoft: `rgba(247, 251, 253, ${decorativeAlphaTokens.heroOverlaySoft})`
  },
  shadow: {
    emphasis: '0 8px 20px rgba(26, 111, 181, 0.14)',
    inverseStrong: '0 10px 24px rgba(14, 30, 56, 0.28)',
    /** 卡片/浮层：轻阴影，层级主要靠边框与表面色差（Design §8.4） */
    card: '0 4px 16px rgba(15, 23, 42, 0.05)',
    overlay: '0 16px 48px rgba(15, 23, 42, 0.1)'
  }
} as const

const authTokens = {
  hero: {
    background: 'linear-gradient(135deg, #0e1e38 0%, #1a365d 50%, #2a5298 100%)',
    overlay: {
      strong: visualTokens.overlay.decorativeStrong,
      medium: visualTokens.overlay.decorativeMedium,
      soft: visualTokens.overlay.decorativeSoft
    },
    decorative: {
      badgeBackground: 'rgba(247, 251, 253, 0.1)',
      badgeBorder: 'rgba(217, 233, 247, 0.2)',
      titleShadow: '0 4px 12px rgba(10, 18, 32, 0.28)',
      particle: `rgba(247, 251, 253, ${decorativeAlphaTokens.heroParticle})`
    }
  },
  glassCard: {
    background: 'rgba(255, 255, 255, 0.95)',
    shadow: '0 20px 60px rgba(10, 18, 32, 0.3)',
    insetBorder: 'rgba(255, 255, 255, 0.4)'
  },
  input: {
    background: '#f8fafc',
    focusShadow: '0 0 0 3px rgba(26, 111, 181, 0.12)'
  }
} as const

const appChromeTokens = {
  background: {
    canvas: '#f5f8fb',
    subtle: '#eef3f8'
  },
  surface: {
    primary: '#ffffff',
    secondary: '#f8fbfd',
    tertiary: '#f3f7fb'
  },
  border: {
    subtle: 'rgba(72, 98, 118, 0.08)',
    default: 'rgba(72, 98, 118, 0.12)',
    strong: 'rgba(72, 98, 118, 0.18)'
  },
  shadow: {
    xs: '0 1px 2px rgba(15, 23, 42, 0.04)',
    sm: '0 4px 14px rgba(15, 23, 42, 0.04)',
    md: '0 10px 28px rgba(15, 23, 42, 0.06)'
  },
  inverse: {
    sidebar: '#101b2d',
    surface: 'rgba(247, 251, 253, 0.06)',
    surfaceHover: 'rgba(247, 251, 253, 0.08)',
    surfaceActive: 'rgba(247, 251, 253, 0.12)',
    border: 'rgba(217, 233, 247, 0.12)',
    textSecondary: 'rgba(217, 233, 247, 0.72)'
  }
} as const

export const semanticTokens = {
  brand: {
    primary: corePalette.brand[600],
    primaryHover: corePalette.brand[500],
    primaryActive: corePalette.brand[700],
    primarySoft: corePalette.brand[100],
    primaryContrast: corePalette.neutral[0],
    accent: corePalette.accent[400],
    accentSoft: corePalette.accent[100]
  },
  neutral: {
    background: {
      base: '#f3f7fc',
      subtle: corePalette.neutral[50],
      muted: '#eaf1f8'
    }
  },
  surface: {
    raised: 'rgba(255, 255, 255, 0.72)',
    panel: 'rgba(255, 255, 255, 0.92)',
    overlay: corePalette.neutral[0],
    strong: '#fbfdff',
    inverse: 'rgba(26, 54, 93, 0.94)'
  },
  text: {
    primary: '#183247',
    secondary: '#486276',
    tertiary: '#6c8192',
    disabled: '#94a3b8'
  },
  inverse: {
    surface: 'rgba(26, 54, 93, 0.94)',
    surfaceStrong: '#0e1e38',
    surfaceSoft: 'rgba(247, 251, 253, 0.1)',
    surfaceMuted: 'rgba(247, 251, 253, 0.14)',
    textPrimary: '#eef5fb',
    textSecondary: 'rgba(217, 233, 247, 0.78)'
  },
  auth: {
    ...authTokens,
    heroBackground: authTokens.hero.background,
    heroBadgeBg: authTokens.hero.decorative.badgeBackground,
    heroBadgeBorder: authTokens.hero.decorative.badgeBorder,
    heroTitleShadow: authTokens.hero.decorative.titleShadow,
    heroOverlayStrong: authTokens.hero.overlay.strong,
    heroOverlayMedium: authTokens.hero.overlay.medium,
    heroOverlaySoft: authTokens.hero.overlay.soft,
    heroParticle: authTokens.hero.decorative.particle,
    cardBg: authTokens.glassCard.background,
    cardShadow: authTokens.glassCard.shadow,
    cardInsetBorder: authTokens.glassCard.insetBorder,
    inputBg: authTokens.input.background,
    inputFocusShadow: authTokens.input.focusShadow
  },
  app: appChromeTokens,
  border: {
    subtle: 'rgba(48, 79, 118, 0.08)',
    default: 'rgba(48, 79, 118, 0.14)',
    strong: 'rgba(48, 79, 118, 0.22)',
    inverse: 'rgba(217, 233, 247, 0.16)'
  },
  state: {
    success: {
      default: corePalette.success[500],
      soft: corePalette.success[100]
    },
    warning: {
      default: corePalette.warning[500],
      soft: corePalette.warning[100]
    },
    danger: {
      default: corePalette.danger[500],
      soft: corePalette.danger[100]
    },
    info: {
      default: corePalette.info[500],
      soft: corePalette.info[100]
    }
  },
  interactive: {
    focusRing: 'rgba(26, 111, 181, 0.22)',
    disabledBg: '#e8eef4',
    disabledBorder: '#d7e0e8'
  },
  visual: {
    decorativeAlpha: decorativeAlphaTokens,
    ...visualTokens
  },
  shadow: {
    card: visualTokens.shadow.card,
    overlay: visualTokens.shadow.overlay
  }
} as const

export const visualizationTokens = {
  chart: {
    axis: 'rgba(72, 98, 118, 0.26)',
    grid: 'rgba(72, 98, 118, 0.12)',
    text: semanticTokens.text.secondary,
    comparison: {
      primary: semanticTokens.brand.primary,
      secondary: corePalette.brand[400]
    },
    comparisonSecondary: {
      primary: corePalette.brand[400],
      secondary: '#7d92a5'
    },
    trend: {
      primary: {
        line: semanticTokens.brand.primary,
        area: 'rgba(26, 111, 181, 0.16)'
      },
      secondary: {
        line: corePalette.brand[400],
        area: 'rgba(93, 150, 210, 0.16)'
      }
    },
    statusDistribution: [
      semanticTokens.state.success.default,
      semanticTokens.state.info.default,
      semanticTokens.state.warning.default,
      semanticTokens.state.danger.default
    ],
    categorySequential: [
      semanticTokens.brand.primary,
      corePalette.brand[500],
      corePalette.brand[400],
      corePalette.brand[300],
      '#7d92a5'
    ],
    seriesPrimary: semanticTokens.brand.primary,
    seriesSecondary: corePalette.brand[400],
    seriesAccent: corePalette.brand[300],
    seriesNeutral: '#7d92a5',
    seriesSuccess: semanticTokens.state.success.default,
    seriesInfo: semanticTokens.state.info.default,
    seriesWarning: semanticTokens.state.warning.default,
    seriesDanger: semanticTokens.state.danger.default
  },
  map: {
    engineering: semanticTokens.brand.primary,
    river: '#4c89ca',
    station: '#78abd9',
    warning: semanticTokens.state.warning.default,
    offline: semanticTokens.state.info.default,
    markerStroke: '#ffffff',
    panelBg: 'rgba(255, 255, 255, 0.88)'
  }
} as const

export const chartSeries = {
  alarmLevels: [
    visualizationTokens.chart.seriesNeutral,
    visualizationTokens.chart.seriesWarning,
    visualizationTokens.chart.seriesDanger
  ],
  stationStatus: [
    visualizationTokens.chart.seriesSuccess,
    visualizationTokens.map.offline,
    visualizationTokens.chart.seriesWarning
  ],
  comparison: [
    visualizationTokens.chart.comparison.primary,
    visualizationTokens.chart.comparison.secondary,
    visualizationTokens.chart.seriesNeutral
  ],
  comparisonSecondary: [
    visualizationTokens.chart.comparisonSecondary.primary,
    visualizationTokens.chart.comparisonSecondary.secondary,
    visualizationTokens.chart.seriesAccent
  ],
  statusDistribution: visualizationTokens.chart.statusDistribution,
  categorySequential: visualizationTokens.chart.categorySequential
} as const

export const lineChartTokens = {
  trendPrimary: visualizationTokens.chart.trend.primary,
  trendSecondary: visualizationTokens.chart.trend.secondary,
  waterLevel: visualizationTokens.chart.trend.primary,
  rainfall: visualizationTokens.chart.trend.secondary
} as const

export const chartTokenKeys = {
  palettes: Object.keys(chartSeries) as Array<keyof typeof chartSeries>,
  lineSeries: Object.keys(lineChartTokens) as Array<keyof typeof lineChartTokens>
} as const

export type ChartSeriesPaletteKey = keyof typeof chartSeries
export type LineChartSeriesType = keyof typeof lineChartTokens
