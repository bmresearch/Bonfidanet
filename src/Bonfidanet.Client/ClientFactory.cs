using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace Bonfida.Client
{
    public static class ClientFactory
    {
        public static IClient GetClient(ILogger logger = null)
        {
#if DEBUG
            logger ??= LoggerFactory.Create(x =>
            {
                x.AddSimpleConsole(o =>
                    {
                        o.UseUtcTimestamp = true;
                        o.IncludeScopes = true;
                        o.ColorBehavior = LoggerColorBehavior.Enabled;
                        o.TimestampFormat = "HH:mm:ss ";
                    })
                    .SetMinimumLevel(LogLevel.Debug);
            }).CreateLogger<IClient>();
#endif
            return new BonfidaClient(logger);
        }

        public static IStreamingClient GetStreamingClient(ILogger logger = null)
        {
#if DEBUG
            logger ??= LoggerFactory.Create(x =>
            {
                x.AddSimpleConsole(o =>
                    {
                        o.UseUtcTimestamp = true;
                        o.IncludeScopes = true;
                        o.ColorBehavior = LoggerColorBehavior.Enabled;
                        o.TimestampFormat = "HH:mm:ss ";
                    })
                    .SetMinimumLevel(LogLevel.Debug);
            }).CreateLogger<IClient>();
#endif
            return new BonfidaStreamingClient(logger);
        }
    }
}