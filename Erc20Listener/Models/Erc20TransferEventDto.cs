using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Erc20Listener.Models;

[Event("Transfer")]
public class Erc20TransferEventDto : IEventDTO
{
    [Parameter("address", "from", 1, true)]
    public string From { get; init; } = default!;

    [Parameter("address", "to", 2, true)]
    public string To { get; init; } = default!;

    [Parameter("uint256", "value", 3, false)]
    public BigInteger Value { get; init; }
}