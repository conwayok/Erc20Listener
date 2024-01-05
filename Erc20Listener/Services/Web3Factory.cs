using Nethereum.JsonRpc.Client;
using Nethereum.Web3;

namespace Erc20Listener.Services;

public interface IWeb3Factory
{
    IWeb3 CreateWeb3(string rpcUrl);
}

public class Web3Factory(HttpClient httpClient) : IWeb3Factory
{
    public IWeb3 CreateWeb3(string rpcUrl)
    {
        return new Web3(new RpcClient(new Uri(rpcUrl), httpClient));
    }
}