using System.Numerics;
using Erc20Listener.Models;
using Erc20Listener.Repositories.Databases;
using Erc20Listener.Repositories.Proxies;

namespace Erc20Listener.Services;

public interface IQueryProgressManager
{
    Task<QueryBlockRange> GetNextRange(string network, BigInteger maxQueryBlocks);
    void SetCurrentProgress(string network, BigInteger currentBlockNumber);
}

public class QueryProgressManager(IBlockchainProxy blockchainProxy, ISqliteRepo sqliteRepo)
    : IQueryProgressManager
{
    public async Task<QueryBlockRange> GetNextRange(string network, BigInteger maxQueryBlocks)
    {
        var currentBlockNumber = sqliteRepo.GetCurrentProgressBlockNumber(network);

        var blockNumbers = await blockchainProxy.GetBlockNumbers(network);

        if (!currentBlockNumber.HasValue)
        {
            return new QueryBlockRange
            {
                From = blockNumbers.Confirmed,
                To = blockNumbers.Confirmed,
                MoveForward = true
            };
        }

        if (currentBlockNumber.Value == blockNumbers.Confirmed)
        {
            return new QueryBlockRange
            {
                From = blockNumbers.Confirmed,
                To = blockNumbers.Confirmed,
                MoveForward = false
            };
        }

        var from = currentBlockNumber.Value + new BigInteger(1);

        var to = from + maxQueryBlocks;

        if (to > blockNumbers.Confirmed)
        {
            to = blockNumbers.Confirmed;
        }

        return new QueryBlockRange
        {
            From = from,
            To = to,
            MoveForward = true
        };
    }

    public void SetCurrentProgress(string network, BigInteger currentBlockNumber)
    {
        sqliteRepo.SetCurrentProgressBlockNumber(network, currentBlockNumber);
    }
}