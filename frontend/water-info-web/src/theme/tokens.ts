export const corePalette = {
  brand: {
    50: '#eff8fb',
    100: '#d9edf3',
    200: '#b7dce7',
    300: '#87c3d4',
    400: '#539fb7',
    500: '#2b7f9b',
    600: '#1f6782',
    700: '#1d546a',
    800: '#1d4758',
    900: '#1d3b49'
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
      base: '#f4f8fb',
      subtle: corePalette.neutral[50],
      muted: '#edf3f7'
    }
  },
  surface: {
    raised: 'rgba(255, 255, 255, 0.72)',
    panel: 'rgba(255, 255, 255, 0.92)',
    overlay: corePalette.neutral[0],
    strong: '#fbfdff',
    inverse: 'rgba(15, 23, 42, 0.92)'
  },
  text: {
    primary: '#183247',
    secondary: '#486276',
    tertiary: '#6c8192',
    disabled: '#94a3b8'
  },
  inverse: {
    surface: 'rgba(18, 42, 57, 0.94)',
    surfaceStrong: '#112d3f',
    surfaceSoft: 'rgba(247, 251, 253, 0.1)',
    surfaceMuted: 'rgba(247, 251, 253, 0.14)',
    textPrimary: '#f7fbfd',
    textSecondary: 'rgba(232, 242, 248, 0.78)'
  },
  border: {
    subtle: 'rgba(51, 84, 106, 0.08)',
    default: 'rgba(51, 84, 106, 0.14)',
    strong: 'rgba(51, 84, 106, 0.22)',
    inverse: 'rgba(229, 241, 247, 0.16)'
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
    focusRing: 'rgba(31, 103, 130, 0.22)',
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
    seriesSecondary: '#2f6f88',
    seriesAccent: '#76a9be',
    seriesNeutral: '#7d92a5',
    seriesSuccess: semanticTokens.state.success.default,
    seriesWarning: semanticTokens.state.warning.default,
    seriesDanger: semanticTokens.state.danger.default
  },
  map: {
    engineering: semanticTokens.brand.primary,
    river: '#3f839a',
    station: '#5b9ab0',
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
    area: 'rgba(31, 103, 130, 0.16)'
  },
  rainfall: {
    line: visualizationTokens.chart.seriesSecondary,
    area: 'rgba(63, 131, 154, 0.14)'
  }
} as const
