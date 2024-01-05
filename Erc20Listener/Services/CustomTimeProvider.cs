namespace Erc20Listener.Services;

public interface ICustomTimeProvider
{
    DateTimeOffset UtcNow { get; }
}

public class CustomTimeProvider : ICustomTimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}