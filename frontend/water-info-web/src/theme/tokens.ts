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
    heroBackground: 'linear-gradient(135deg, #0e1e38 0%, #1a365d 50%, #2a5298 100%)',
    heroBadgeBg: 'rgba(247, 251, 253, 0.1)',
    heroBadgeBorder: 'rgba(217, 233, 247, 0.2)',
    heroTitleShadow: '0 4px 12px rgba(10, 18, 32, 0.28)',
    heroOverlayStrong: 'rgba(247, 251, 253, 0.2)',
    heroOverlayMedium: 'rgba(247, 251, 253, 0.15)',
    heroOverlaySoft: 'rgba(247, 251, 253, 0.1)',
    heroParticle: 'rgba(247, 251, 253, 0.3)',
    cardBg: 'rgba(255, 255, 255, 0.95)',
    cardShadow: '0 20px 60px rgba(10, 18, 32, 0.3)',
    inputBg: '#f8fafc',
    inputFocusShadow: '0 0 0 3px rgba(26, 111, 181, 0.12)'
  },
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
  shadow: {
    card: '0 18px 40px rgba(15, 23, 42, 0.08)',
    overlay: '0 24px 64px rgba(15, 23, 42, 0.14)'
  }
} as const

export const visualizationTokens = {
  chart: {
    axis: 'rgba(72, 98, 118, 0.26)',
    grid: 'rgba(72, 98, 118, 0.12)',
    text: semanticTokens.text.secondary,
    seriesPrimary: semanticTokens.brand.primary,
    seriesSecondary: corePalette.brand[400],
    seriesAccent: corePalette.brand[300],
    seriesNeutral: '#7d92a5',
    seriesSuccess: semanticTokens.state.success.default,
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
  ]
} as const

export const lineChartTokens = {
  waterLevel: {
    line: visualizationTokens.chart.seriesPrimary,
    area: 'rgba(26, 111, 181, 0.16)'
  },
  rainfall: {
    line: visualizationTokens.chart.seriesSecondary,
    area: 'rgba(93, 150, 210, 0.16)'
  }
} as const

export type ChartSeriesPaletteKey = keyof typeof chartSeries
export type LineChartSeriesType = keyof typeof lineChartTokens
