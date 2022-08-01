using HeroesCup.Modules.ClubsModule;
using HeroesCup.Web.Common;
using HeroesCup.Web.Data;
using HeroesCup.Web.Services;
using Microsoft.EntityFrameworkCore;
using Piranha;
using Piranha.AttributeBuilder;
using Piranha.AspNetCore.Identity.SQLite;
using Piranha.Data.EF.SQLite;
using Piranha.Manager.Editor;

var builder = WebApplication.CreateBuilder(args);

builder.AddPiranha(options =>
{

    options.AddRazorRuntimeCompilation = true;

    options.UseCms();
    options.UseManager();

    options.UseFileStorage(naming: Piranha.Local.FileStorageNaming.UniqueFolderNames);
    options.UseImageSharp();
    options.UseTinyMCE();
    options.UseMemoryCache();

    var connectionString = builder.Configuration.GetConnectionString("piranha");
    options.UseEF<SQLiteDb>(db => db.UseSqlite(connectionString));
    
    options.UseIdentityWithSeed<IdentitySQLiteDb>(db => db.UseSqlite(connectionString));
});
builder.Services.AddClubsModule();
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
    options => options.UseSqlite("connectionString"));
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UsePiranha(options =>
{
    // Initialize Piranha
    App.Init(options.Api);

    // Build content types
    new ContentTypeBuilder(options.Api)
        .AddAssembly(typeof(Program).Assembly)
        .Build()
        .DeleteOrphans();

    // Configure Tiny MCE
    EditorConfig.FromFile("editorconfig.json");

    options.UseManager();
    options.UseTinyMCE();
    options.UseIdentity();
});

app.Run();