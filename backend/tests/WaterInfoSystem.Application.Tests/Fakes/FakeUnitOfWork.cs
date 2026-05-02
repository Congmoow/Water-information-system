using WaterInfoSystem.Application.Interfaces;

namespace WaterInfoSystem.Application.Tests.Fakes;

internal class FakeUnitOfWork : IUnitOfWork
{
    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
