using System.Numerics;
using Erc20Listener.Config;
using Erc20Listener.Models;
using Erc20Listener.Services;
using Microsoft.Extensions.Options;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace Erc20Listener.Repositories.Proxies;

public interface IBlockchainProxy
{
    Task<List<BlockchainEvent<Erc20TransferEventDto>>> GetErc20TransferEvents(
        string network,
        BigInteger fromBlock,
        BigInteger toBlock);

    Task<BlockNumbers> GetBlockNumbers(string network);
}

public class BlockchainProxy(IWeb3Factory web3Factory, IOptions<AppSettings> options) : IBlockchainProxy
{
    public async Task<List<BlockchainEvent<Erc20TransferEventDto>>> GetErc20TransferEvents(string network,
        BigInteger fromBlock, BigInteger toBlock)
    {
        var web3 = web3Factory.CreateWeb3(options.Value.GetListenConfig(network).RpcUrl);

        var e = web3.Eth.GetEvent<Erc20TransferEventDto>();

        var filterInput = e.CreateFilterInput(
            new BlockParameter(new HexBigInteger(fromBlock)),
            new BlockParameter(new HexBigInteger(toBlock)));

        var allChangesAsync = await e.GetAllChangesAsync(filterInput);

        var eventLogs = new List<BlockchainEvent<Erc20TransferEventDto>>();

        // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
        foreach (var eventLog in allChangesAsync)
        {
            var blockchainEventLog = new BlockchainEvent<Erc20TransferEventDto>
            {
                Event = eventLog.Event,
                Log = new BlockchainEventLog
                {
                    Address = eventLog.Log.Address,
                    BlockNumber = eventLog.Log.BlockNumber.Value,
                    LogIndex = eventLog.Log.LogIndex.Value,
                    TransactionHash = eventLog.Log.TransactionHash,
                    BlockHash = eventLog.Log.BlockHash
                }
            };
            eventLogs.Add(blockchainEventLog);
        }

        return eventLogs;
    }

    public async Task<BlockNumbers> GetBlockNumbers(string network)
    {
        var listenConfig = options.Value.GetListenConfig(network);

        var web3 = web3Factory.CreateWeb3(listenConfig.RpcUrl);
        var currentBlockNumHex = await web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();
        return new BlockNumbers
        {
            Confirmed = currentBlockNumHex.Value - new BigInteger(listenConfig.ConfirmationBlocks)
        };
    }
}