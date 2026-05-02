export const framerMotion = {
  durations: {
    instant: '0.08s',
    fast: '0.15s',
    normal: '0.25s',
    slow: '0.4s',
    slower: '0.6s'
  },
  easings: {
    default: 'cubic-bezier(0.25, 0.1, 0.25, 1)',
    smooth: 'cubic-bezier(0.4, 0, 0.2, 1)',
    spring: 'cubic-bezier(0.34, 1.56, 0.64, 1)',
    bounce: 'cubic-bezier(0.68, -0.55, 0.265, 1.55)',
    sharp: 'cubic-bezier(0.4, 0, 0.6, 1)',
    exit: 'cubic-bezier(0.4, 0, 1, 1)',
    enter: 'cubic-bezier(0, 0, 0.2, 1)'
  },
  springs: {
    gentle: { stiffness: 120, damping: 14 },
    wobbly: { stiffness: 180, damping: 12 },
    stiff: { stiffness: 260, damping: 20 },
    soft: { stiffness: 100, damping: 18 }
  }
} as const

export const framerTheme = {
  color: {
    black: '#0a0a0a',
    white: '#ffffff',
    blue: '#0066ff',
    blueDark: '#0052cc',
    blueLight: '#3385ff',
    gray: {
      50: '#fafafa',
      100: '#f5f5f5',
      200: '#e5e5e5',
      300: '#d4d4d4',
      400: '#a3a3a3',
      500: '#737373',
      600: '#525252',
      700: '#404040',
      800: '#262626',
      900: '#171717'
    }
  },
  shadows: {
    small: '0 2px 8px rgba(0, 0, 0, 0.08)',
    medium: '0 8px 24px rgba(0, 0, 0, 0.12)',
    large: '0 20px 48px rgba(0, 0, 0, 0.16)',
    glow: '0 0 20px rgba(0, 102, 255, 0.3)',
    glowStrong: '0 0 40px rgba(0, 102, 255, 0.4)'
  },
  radii: {
    sm: '6px',
    md: '12px',
    lg: '18px',
    xl: '24px',
    full: '9999px'
  }
} as const

export const animationClasses = {
  pageEnter: 'page-enter-active',
  pageLeave: 'page-leave-active',
  fadeIn: 'fade-in',
  fadeInUp: 'fade-in-up',
  fadeInDown: 'fade-in-down',
  scaleIn: 'scale-in',
  slideInLeft: 'slide-in-left',
  slideInRight: 'slide-in-right',
  staggerItem: 'stagger-item',
  hoverLift: 'hover-lift',
  hoverScale: 'hover-scale',
  hoverGlow: 'hover-glow'
} as const

export default {
  framerMotion,
  framerTheme,
  animationClasses
}
