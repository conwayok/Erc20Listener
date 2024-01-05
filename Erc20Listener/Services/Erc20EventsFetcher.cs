using System.Numerics;
using Erc20Listener.Config;
using Erc20Listener.Models;
using Erc20Listener.Repositories.Databases;
using Erc20Listener.Repositories.Proxies;
using Microsoft.Extensions.Options;

namespace Erc20Listener.Services;

public interface IErc20EventsFetcher
{
    Task Fetch(string network);
}

public class Erc20EventsFetcher(
    ILogger<Erc20EventsFetcher> logger,
    IOptions<AppSettings> options,
    IBlockchainProxy blockchainProxy,
    IQueryProgressManager queryProgressManager,
    ICustomTimeProvider customTimeProvider,
    ISqliteRepo sqliteRepo) : IErc20EventsFetcher
{
    public async Task Fetch(string network)
    {
        var listenConfig = options.Value.GetListenConfig(network);
        var maxQueryBlocks = listenConfig.MaxQueryBlocks;

        var queryBlockRange = await queryProgressManager.GetNextRange(network, new BigInteger(maxQueryBlocks));

        logger.LogInformation("{Network}, query range = {@QueryBlockRange}", network, queryBlockRange);

        if (!queryBlockRange.MoveForward)
        {
            queryProgressManager.SetCurrentProgress(network, queryBlockRange.To);
            return;
        }

        var eventLogs =
            await blockchainProxy.GetErc20TransferEvents(network, queryBlockRange.From, queryBlockRange.To);

        var filtered = eventLogs
            .Where(x => listenConfig.TokenAddresses.Contains(x.Log.Address, StringComparer.OrdinalIgnoreCase)).ToList();

        logger.LogInformation(
            "{Network}, found total {EventLogsCount} erc20 transfers, containing {FilteredCount} transfers for tokens {@TokenAddresses}",
            network,
            eventLogs.Count, filtered.Count, listenConfig.TokenAddresses);

        sqliteRepo.UpsertErc20Transfers(filtered.Select(x => new Erc20Transfer
        {
            FromAddress = x.Event.From,
            ToAddress = x.Event.To,
            Value = x.Event.Value,
            TokenAddress = x.Log.Address,
            BlockNumber = x.Log.BlockNumber,
            TransactionHash = x.Log.TransactionHash,
            LogIndex = x.Log.LogIndex,
            BlockHash = x.Log.BlockHash,
            Network = network,
            UpdateTime = customTimeProvider.UtcNow
        }));

        queryProgressManager.SetCurrentProgress(network, queryBlockRange.To);
    }
}