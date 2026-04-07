namespace WaterInfoSystem.Shared.Results;

public record PagedResult<T>(IReadOnlyList<T> Items, int Total);
