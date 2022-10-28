using System.Globalization;
using HeroesCup.Web.ClubsModule;
using HeroesCup.Web.Common;
using HeroesCup.Web.Data;
using HeroesCup.Web.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Piranha;
using Piranha.AspNetCore.Identity.MySQL;
using Piranha.AttributeBuilder;
using Piranha.Data.EF.MySql;
using Piranha.Local;
using Piranha.Manager.Editor;


var builder = WebApplication.CreateBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("HEROESCUP_CONNECTIONSTRING") ?? builder.Configuration.GetConnectionString("piranha");
builder.AddPiranha(options =>
{
    options.AddRazorRuntimeCompilation = true;
    options.UseCms();
    options.UseManager();

    options.UseFileStorage(naming: FileStorageNaming.UniqueFolderNames);
    options.UseImageSharp();
    options.UseTinyMCE();
    options.UseMemoryCache();


    options.UseEF<MySqlDb>(db => db.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

    options.UseIdentityWithSeed<IdentityMySQLDb>(
        db => db.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)),
        identityOptions: i =>
        {
            
        },  a =>
        {
            a.ExpireTimeSpan = TimeSpan.MaxValue;
            a.SlidingExpiration = true;
            a.Cookie.Expiration = TimeSpan.MaxValue;
        } );
});

builder.Services.Configure<RequestLocalizationOptions>(o =>
{
    o.SetDefaultCulture("bg-BG");
    o.DefaultRequestCulture = new RequestCulture(new CultureInfo("bg-BG"));
});
builder.Services.AddTransient<IHeroesCupIdentitySeed, IdentitySeed>();
builder.Services.AddTransient<IPageInitializer, PageInitializer>();
builder.Services.AddTransient<ILeaderboardService, LeaderboardService>();
builder.Services.AddTransient<IStatisticsService, StatisticsService>();
builder.Services.AddTransient<IMissionsService, MissionsService>();
builder.Services.AddTransient<ISessionService, SessionService>();
builder.Services.AddTransient<ISearchServce, SearchService>();
builder.Services.AddTransient<IWebUtils, WebUtils>();
builder.Services.AddTransient<IVideoThumbnailParser, YouTubeVideoThumbnailParser>();
builder.Services.AddTransient<IMetaDataProvider, MetaDataProvider>();
builder.Services.AddPiranhaFileStorage();
builder.Services.AddDbContext<HeroesCupDbContext>(
    options =>
    {
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        options.EnableSensitiveDataLogging();
    });

builder.Services.AddClubsModule();

var app = builder.Build();

app.UseSession();
app.UseStaticFiles();
IList<CultureInfo> supportedCultures = new List<CultureInfo>
{
    new CultureInfo("bg-BG")
};
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(new CultureInfo("bg-BG")),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

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
app.Run();