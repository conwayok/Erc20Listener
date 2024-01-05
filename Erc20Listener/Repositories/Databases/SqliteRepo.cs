using System.Numerics;
using Dapper;
using Erc20Listener.Config;
using Erc20Listener.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

// ReSharper disable RedundantAnonymousTypePropertyName

namespace Erc20Listener.Repositories.Databases;

public interface ISqliteRepo
{
    void SetCurrentProgressBlockNumber(string network, BigInteger blockNumber);
    BigInteger? GetCurrentProgressBlockNumber(string network);
    void UpsertErc20Transfers(IEnumerable<Erc20Transfer> erc20Transfers);
}

public class SqliteRepo(IOptions<AppSettings> options) : ISqliteRepo
{
    private readonly string _connectionString = options.Value.SqliteConnectionString;

    public void SetCurrentProgressBlockNumber(string network, BigInteger blockNumber)
    {
        const string sql = """
                           INSERT INTO blockchain_listen_progresses (network, current_block_number)
                           VALUES (@Network, @CurrentBlockNumber)
                           ON CONFLICT DO UPDATE SET current_block_number = excluded.current_block_number;
                           """;

        using var sqliteConnection = new SqliteConnection(_connectionString);

        sqliteConnection.Execute(sql, new
        {
            Network = network,
            CurrentBlockNumber = blockNumber.ToString()
        });
    }

    public BigInteger? GetCurrentProgressBlockNumber(string network)
    {
        const string sql = """
                           SELECT current_block_number
                           FROM blockchain_listen_progresses
                           WHERE network = @Network;
                           """;

        using var sqliteConnection = new SqliteConnection(_connectionString);

        var blockNumberStr = sqliteConnection.QueryFirstOrDefault<string>(sql, new
        {
            Network = network
        });

        if (string.IsNullOrEmpty(blockNumberStr))
        {
            return null;
        }

        return BigInteger.Parse(blockNumberStr);
    }

    public void UpsertErc20Transfers(IEnumerable<Erc20Transfer> erc20Transfers)
    {
        const string sql = """
                           INSERT INTO erc_20_transfers (network,
                                                         block_hash,
                                                         transaction_hash,
                                                         log_index,
                                                         update_time_ms,
                                                         block_number,
                                                         from_address,
                                                         to_address,
                                                         value,
                                                         token_address)
                           VALUES (@Network,
                                   @BlockHash,
                                   @TransactionHash,
                                   @LogIndex,
                                   @UpdateTimeMs,
                                   @BlockNumber,
                                   @FromAddress,
                                   @ToAddress,
                                   @Value,
                                   @TokenAddress)
                           ON CONFLICT (network, block_hash, transaction_hash, log_index)
                               DO UPDATE SET update_time_ms = excluded.update_time_ms,
                                             block_number  = excluded.block_number,
                                             from_address  = excluded.from_address,
                                             to_address    = excluded.to_address,
                                             value         = excluded.value,
                                             token_address = excluded.token_address;
                           """;

        using var sqliteConnection = new SqliteConnection(_connectionString);

        sqliteConnection.Execute(sql, erc20Transfers.Select(x => new
        {
            Network = x.Network,
            BlockHash = x.BlockHash,
            TransactionHash = x.TransactionHash,
            LogIndex = x.LogIndex.ToString(),
            UpdateTimeMs = x.UpdateTime.ToUnixTimeMilliseconds(),
            BlockNumber = x.BlockNumber.ToString(),
            FromAddress = x.FromAddress,
            ToAddress = x.ToAddress,
            Value = x.Value.ToString(),
            TokenAddress = x.TokenAddress
        }).ToList());
    }
}