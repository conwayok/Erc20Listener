using System.Numerics;

namespace Erc20Listener.Models;

public class QueryBlockRange
{
    public required BigInteger From { get; init; }
    public required BigInteger To { get; init; }
    public required bool MoveForward { get; init; }
}