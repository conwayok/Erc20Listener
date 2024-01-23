using Erc20Listener.Config;
using Erc20Listener.Repositories.Databases;
using Erc20Listener.Repositories.Proxies;
using Erc20Listener.Services;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("nlog.json");
LogManager.Configuration =
    new NLogLoggingConfiguration(builder.Configuration.GetRequiredSection("NLog"));
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Trace);
builder.Host.UseNLog();

builder.Services.AddOptions<AppSettings>().Bind(builder.Configuration);
builder.Services.AddHttpClient();
builder.Services.AddTransient<IWeb3Factory, Web3Factory>();
builder.Services.AddTransient<IBlockchainProxy, BlockchainProxy>();
builder.Services.AddTransient<IQueryProgressManager, QueryProgressManager>();
builder.Services.AddTransient<IErc20EventsFetcher, Erc20EventsFetcher>();
builder.Services.AddSingleton<ICustomTimeProvider, CustomTimeProvider>();
builder.Services.AddTransient<ISqliteRepo, SqliteRepo>();
builder.Services.AddHostedService<Worker>();

var app = builder.Build();

app.Run();