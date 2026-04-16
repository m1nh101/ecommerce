using Microsoft.Extensions.Hosting;
using Redis.OM;

namespace Infrastructure.Carts;

internal sealed class RedisIndexCreationService(RedisConnectionProvider provider) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await provider.Connection.CreateIndexAsync(typeof(CartDocument));
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
