namespace Erc20Listener.Models;

public class BlockchainEvent<T>
{
    public required T Event { get; init; }

    public required BlockchainEventLog Log { get; init; }
}