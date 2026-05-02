using WaterInfoSystem.Application.Interfaces.Security;

namespace WaterInfoSystem.Application.Tests.Fakes;

internal sealed class FakePasswordHasher : IPasswordHasher
{
    private readonly bool _isValid;

    public FakePasswordHasher(bool isValid)
    {
        _isValid = isValid;
    }

    public string HashPassword(string password)
    {
        return password;
    }

    public bool Verify(string password, string passwordHash)
    {
        return _isValid;
    }
}
