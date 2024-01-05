using System.Numerics;

namespace Erc20Listener.Models;

public class Erc20Transfer
{
    public required string Network { get; init; }
    public required string BlockHash { get; init; }
    public required string TransactionHash { get; init; }
    public required BigInteger LogIndex { get; init; }
    public required BigInteger BlockNumber { get; init; }
    public required string TokenAddress { get; init; }
    public required string FromAddress { get; init; }
    public required string ToAddress { get; init; }
    public required BigInteger Value { get; init; }
    public required DateTimeOffset UpdateTime { get; init; }
}