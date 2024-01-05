// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace Erc20Listener.Config;

public class AppSettings
{
    public string SqliteConnectionString { get; init; } = default!;

    public List<ListenConfig> ListenConfigs { get; init; } = default!;

    public ListenConfig GetListenConfig(string network)
    {
        return ListenConfigs.First(x => x.Network == network);
    }
}

public class ListenConfig
{
    public string Network { get; init; } = default!;
    public string RpcUrl { get; init; } = default!;
    public int PollIntervalSeconds { get; init; } = default!;
    public int MaxQueryBlocks { get; init; } = default!;
    public int ConfirmationBlocks { get; init; } = default!;
    public List<string> TokenAddresses { get; init; } = default!;
}