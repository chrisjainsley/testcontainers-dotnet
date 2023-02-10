namespace Testcontainers.CosmosDb;

public sealed class CosmosDbContainerTest : IAsyncLifetime
{
    private readonly CosmosDbTestcontainer _cosmosDbContainer = new TestcontainersBuilder<CosmosDbTestcontainer>()
                .WithDatabase(new CosmosDbTestcontainerConfiguration())
        .WithExposedPort(8081)
                .Build();

    static CosmosDbContainerTest()
    {
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
    public async Task ListBucketsReturnsHttpStatusCodeOk()
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