using BlogApp.Web.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BlogApp.Tests.Integration;

public class BlogApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _connString;

    public BlogApplicationFactory(string connString)
    {
        var builder = new SqlConnectionStringBuilder(connString)
        {
            InitialCatalog = "BlogTest"
        };

        _connString = builder.ConnectionString;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<BlogContext>));
            services.AddSqlServer<BlogContext>(_connString);
        });

        builder.ConfigureTestServices(services =>
        {
            services.AddControllersWithViews(options => options.Filters.Add(new IgnoreAntiforgeryTokenAttribute()));
        });

        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Admin:Key"] = Environment.GetEnvironmentVariable("admin_key"),
                ["ConnectionStrings:BlogConnection"] = ""
            });
        });
    }

    public async Task ResetDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.MigrateAsync();
    }
}
