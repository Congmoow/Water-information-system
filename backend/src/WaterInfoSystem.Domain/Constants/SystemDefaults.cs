namespace WaterInfoSystem.Domain.Constants;

public static class SystemDefaults
{
    public const int DefaultPage = 1;

    public const int DefaultPageSize = 10;

    public const decimal DefaultWaterLevelWarningThreshold = 18.5m;

    public const decimal DefaultWaterLevelCriticalThreshold = 20m;

    public const decimal DefaultRainfallWarningThreshold = 50m;

    public const decimal DefaultRainfallCriticalThreshold = 80m;

    public const decimal DefaultFlowWarningThreshold = 200m;

    public const decimal DefaultFlowCriticalThreshold = 300m;
}
