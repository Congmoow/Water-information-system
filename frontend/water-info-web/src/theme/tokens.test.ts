import { describe, expect, it } from 'vitest'
import { semanticTokens, visualizationTokens } from '@/theme/tokens'

describe('theme tokens', () => {
  it('defines semantic token groups for the app shell', () => {
    expect(semanticTokens.brand.primary).toBeTruthy()
    expect(semanticTokens.brand.primaryHover).toBeTruthy()
    expect(semanticTokens.brand.primaryActive).toBeTruthy()
    expect(semanticTokens.neutral.background.base).toBeTruthy()
    expect(semanticTokens.neutral.background.subtle).toBeTruthy()
    expect(semanticTokens.neutral.background.muted).toBeTruthy()
    expect(semanticTokens.text.primary).toBeTruthy()
    expect(semanticTokens.text.secondary).toBeTruthy()
    expect(semanticTokens.text.tertiary).toBeTruthy()
    expect(semanticTokens.surface.panel).toBeTruthy()
    expect(semanticTokens.surface.raised).toBeTruthy()
    expect(semanticTokens.surface.overlay).toBeTruthy()
    expect(semanticTokens.surface.strong).toBeTruthy()
    expect(semanticTokens.inverse.surface).toBeTruthy()
    expect(semanticTokens.inverse.surfaceSoft).toBeTruthy()
    expect(semanticTokens.inverse.textPrimary).toBeTruthy()
    expect(semanticTokens.border.default).toBeTruthy()
    expect(semanticTokens.border.strong).toBeTruthy()
    expect(semanticTokens.state.warning.default).toBeTruthy()
    expect(semanticTokens.interactive.focusRing).toBeTruthy()
  })

  it('keeps accent separate from warning semantics', () => {
    expect(semanticTokens.brand.accent).not.toBe(semanticTokens.state.warning.default)
    expect(semanticTokens.brand.accentSoft).not.toBe(semanticTokens.state.warning.soft)
  })

  it('defines dedicated visualization aliases for charts and maps', () => {
    expect(visualizationTokens.chart.axis).toBeTruthy()
    expect(visualizationTokens.chart.grid).toBeTruthy()
    expect(visualizationTokens.chart.text).toBeTruthy()
    expect(visualizationTokens.chart.seriesPrimary).toBeTruthy()
    expect(visualizationTokens.chart.seriesWarning).toBeTruthy()
    expect(visualizationTokens.chart.seriesDanger).toBeTruthy()
    expect(visualizationTokens.map.engineering).toBeTruthy()
    expect(visualizationTokens.map.river).toBeTruthy()
    expect(visualizationTokens.map.station).toBeTruthy()
    expect(visualizationTokens.map.warning).toBeTruthy()
    expect(visualizationTokens.map.offline).toBeTruthy()
    expect(visualizationTokens.chart.seriesPrimary).not.toBe(semanticTokens.brand.accent)
    expect(visualizationTokens.map.warning).not.toBe(semanticTokens.brand.accent)
  })

  it('keeps usage boundaries clear between brand, accent, warning, and danger', () => {
    expect(semanticTokens.brand.primary).not.toBe(semanticTokens.brand.accent)
    expect(semanticTokens.brand.primary).not.toBe(semanticTokens.state.warning.default)
    expect(semanticTokens.brand.primary).not.toBe(semanticTokens.state.danger.default)
    expect(semanticTokens.state.warning.default).not.toBe(semanticTokens.state.danger.default)
  })
})
