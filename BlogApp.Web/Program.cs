using BlogApp.Web.Data;
using BlogApp.Web.Interfaces;
using BlogApp.Web.Options;
using BlogApp.Web.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<BlogContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("BlogConnection") ?? 
        throw new ApplicationException("No connection string found in config.");
    options.UseSqlServer(connectionString);
});

builder.Services.AddScoped<IBlogService, BlogService>();
builder.Services.Configure<AdminOptions>(
    builder.Configuration.GetSection("Admin"));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost",
                           "http://tonysblog.duckdns.org")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    using var scope = app.Services.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();
    try
    {
        logger.LogDebug("Applying pending migrations.");
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while migrating or initializing the database.");
        return;
    }
}
//else
//{
//    app.UseExceptionHandler("/Error");
//}

app.UseStaticFiles();
app.UseRouting();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Blog}/{action=Index}/{id?}");

app.Run();

public partial class Program { }