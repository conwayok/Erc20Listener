# Erc20Listener

Blockchain ERC20 transfer events listener written in .NET

Polls for ERC20 transfer events and saves them to a sqlite database

## How to run

1. Create and initialize a sqlite database with `Sql/Init.sql`.
2. Configure the `SqliteConnectionString` in `appsettings.json` to point to the newly created DB.

## Configuration

#### AppSettings

| name                   | type           | description                                                                   |
|------------------------|----------------|-------------------------------------------------------------------------------|
| SqliteConnectionString | string         | sqlite connection string, where transfer events and listen progress is stored |
| ListenConfigs          | ListenConfig[] | array of config objects for different blockchains                             |

#### ListenConfig

| name                | type     | description                                                                                   |
|---------------------|----------|-----------------------------------------------------------------------------------------------|
| Network             | string   | unique, self defined network name                                                             |
| RpcUrl              | string   | rpc url for blockchain                                                                        |
| PollIntervalSeconds | int      | polling interval for events                                                                   |
| PollIntervalSeconds | int      | polling interval for events                                                                   |
| MaxQueryBlocks      | int      | max amount of blocks per poll                                                                 |
| ConfirmationBlocks  | int      | how many blocks should pass before a tx is "confirmed", only confirmed transactions are saved |
| TokenAddresses      | string[] | ERC20 token addresses we are interested in                                                    |

### Example

```json
{
  "SqliteConnectionString": "Data Source=DbFiles/sqlite.db",
  "ListenConfigs": [
    {
      "Network": "Polygon",
      "RpcUrl": "https://rpc.ankr.com/polygon",
      "PollIntervalSeconds": 5,
      "MaxQueryBlocks": 10,
      "ConfirmationBlocks": 200,
      "TokenAddresses": [
        "0xc2132D05D31c914a87C6611C10748AEb04B58e8F",
        "0x3BA4c387f786bFEE076A58914F5Bd38d668B42c3",
        "0x3c499c542cEF5E3811e1192ce70d8cC03d5c3359"
      ]
    }
  ]
}
```