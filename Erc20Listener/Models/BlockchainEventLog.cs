using System.Numerics;

namespace Erc20Listener.Models;

public class BlockchainEventLog
{
    public required string TransactionHash { get; init; }
    public required string BlockHash { get; init; }
    public required BigInteger LogIndex { get; init; }
    public required string Address { get; init; }
    public required BigInteger BlockNumber { get; init; }
}