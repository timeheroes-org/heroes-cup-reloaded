using HeroesCup.Web.ClubsModule;
using HeroesCup.Web.Common;
using HeroesCup.Web.Data;
using HeroesCup.Web.Services;
using Microsoft.EntityFrameworkCore;
using Piranha;
using Piranha.AspNetCore.Identity.SQLite;
using Piranha.AttributeBuilder;
using Piranha.Data.EF.SQLite;
using Piranha.Local;
using Piranha.Manager.Editor;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("piranha");
builder.AddPiranha(options =>
{
    options.AddRazorRuntimeCompilation = true;

    options.UseCms();
    options.UseManager();

    options.UseFileStorage(naming: FileStorageNaming.UniqueFolderNames);
    options.UseImageSharp();
    options.UseTinyMCE();
    options.UseMemoryCache();


    options.UseEF<SQLiteDb>(db => db.UseSqlite(connectionString));

    options.UseIdentityWithSeed<IdentitySQLiteDb>(db => db.UseSqlite(connectionString));
});

builder.Services.AddTransient<IHeroesCupIdentitySeed, IdentitySeed>();
builder.Services.AddTransient<IPageInitializer, PageInitializer>();
builder.Services.AddTransient<ILeaderboardService, LeaderboardService>();
builder.Services.AddTransient<IStatisticsService, StatisticsService>();
builder.Services.AddTransient<IMissionsService, MissionsService>();
builder.Services.AddTransient<ISessionService, SessionService>();
builder.Services.AddTransient<IWebUtils, WebUtils>();
builder.Services.AddTransient<IVideoThumbnailParser, YouTubeVideoThumbnailParser>();
builder.Services.AddTransient<IMetaDataProvider, MetaDataProvider>();
builder.Services.AddDbContext<HeroesCupDbContext>(
    options =>
    {
        options.UseSqlite(connectionString);
        options.EnableSensitiveDataLogging();
    });
builder.Services.AddClubsModule();

var app = builder.Build();

app.UseSession();

if (app.Environment.IsDevelopment()) app.UseDeveloperExceptionPage();

app.UsePiranha(options =>
{
    // Initialize Piranha
    App.Init(options.Api);

    // Build content types
    new ContentTypeBuilder(options.Api)
        .AddAssembly(typeof(Program).Assembly)
        .Build();
    // Configure Tiny MCE
    EditorConfig.FromFile("editorconfig.json");

    options.UseManager();
    options.UseTinyMCE();
    options.UseIdentity();
});
app.UseRouting();
app.UseClubsModule(builder);

SeedDefaultPages();

void SeedDefaultPages()
{
    var dbSeed = builder.Configuration["DbSeed"];
    if (dbSeed == "true")
    {
#pragma warning disable ASP0000
        var serviceProvider = builder.Services.BuildServiceProvider();
#pragma warning restore ASP0000

        var identitySeed = serviceProvider.GetService<IHeroesCupIdentitySeed>();
        identitySeed.SeedIdentityAsync();
        var pagesInitializer = serviceProvider.GetService<IPageInitializer>();

        pagesInitializer.SeedMissionsPageAsync().Wait();
        pagesInitializer.SeedResourcesPageAsync().Wait();
        pagesInitializer.SeedEventsPageAsync().Wait();
        pagesInitializer.SeedAboutPageAsync().Wait();
        pagesInitializer.SeedStarPageAsync().Wait();
    }
}

app.Run();