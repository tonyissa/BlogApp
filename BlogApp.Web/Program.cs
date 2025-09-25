using BlogApp.Web.Data;
using BlogApp.Web.Data.DTOs;
using BlogApp.Web.Interfaces;
using BlogApp.Web.Models;
using BlogApp.Web.Options;
using BlogApp.Web.Services;
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
builder.Services.AddScoped<IBlogService, BlogService>();

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

builder.Services.AddAutoMapper(config =>
{
    config.CreateMap<PostDTO, Post>().ReverseMap();
    config.CreateMap<CommentDTO, Comment>().ReverseMap();
});
builder.Services.AddSingleton<MapperService>();

builder.Services.Configure<AdminOptions>(
    builder.Configuration.GetSection("Admin"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseHsts();
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
else
{
    app.UseExceptionHandler("/Error");
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{action=Index}/{id?}",
    defaults: new { controller = "Home" });

app.Run();
