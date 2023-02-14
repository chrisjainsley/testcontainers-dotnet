namespace Testcontainers.CosmosDb;

public sealed class CosmosDbContainerTest : IAsyncLifetime
{
    private readonly CosmosDbTestcontainer _cosmosDbContainer;

    static CosmosDbContainerTest()
    {
    }

    public CosmosDbContainerTest()
    {
        var config = new CosmosDbTestcontainerConfiguration();
        _cosmosDbContainer = new TestcontainersBuilder<CosmosDbTestcontainer>()
                .WithDatabase(config)
                .WithExposedPort(8081)
                .Build();
    }

    public Task InitializeAsync()
    {
        return _cosmosDbContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _cosmosDbContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ReadAccountReturnsHttpStatusCodeOk()
    {
        // Given
        var cosmosClientOptions = new CosmosClientOptions
        {
            ConnectionMode = ConnectionMode.Gateway,
            HttpClientFactory = () => _cosmosDbContainer.HttpClient,
        };

        var client = new CosmosClient(_cosmosDbContainer.ConnectionString, cosmosClientOptions);

        // When
        var account = await client.ReadAccountAsync()
            .ConfigureAwait(false);

        // Then
        Assert.NotNull(account.Id);
    }
}