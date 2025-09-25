using BlogApp.Web.Data;
using BlogApp.Web.Utilities;
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

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost", 
                           "close-genuinely-seahorse.ngrok-free.app",
                           "http://192.168.0.7")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    //app.UseExceptionHandler("/Home/Error");
    //app.UseHsts();
    app.UseDeveloperExceptionPage();

    using var scope = app.Services.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();
    try
    {
        logger.LogDebug("Applying pending migrations");
        dbContext.Database.Migrate();
        logger.LogDebug("Ensuring database has been seeded");
        new DbInitializer().Initialize(dbContext);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while migrating or initializing the database.");
        return;
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Posts}/{action=Index}/{id?}");

app.Run();
