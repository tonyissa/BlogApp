using Testcontainers.MsSql;

namespace BlogApp.Tests.Integration;

public class SqlServerContainerFixture : IAsyncLifetime
{
    public MsSqlContainer Container { get; private set; }
    private const string _image = "mcr.microsoft.com/mssql/server:2019-latest";

    public async Task InitializeAsync()
    {
        Container = new MsSqlBuilder()
            .WithImage(_image)
            .WithCleanUp(true)
            .Build();

        await Container.StartAsync();
    }

    public async Task DisposeAsync() => await Container.DisposeAsync();
}
