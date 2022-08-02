using HeroesCup.Web.ClubsModule.Security;
using HeroesCup.Web.Services;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.FileProviders;
using Piranha;
using Piranha.Manager;
using ManagerLocalizer = HeroesCup.Localization.ManagerLocalizer;

namespace HeroesCup.Web.ClubsModule;

public static class ClubsModuleExtensions
{
    public static IServiceCollection AddClubsModule(this IServiceCollection services)
    {
        services.Configure<RazorViewEngineOptions>(options =>
        {
            options.AreaViewLocationFormats.Clear();
            options.AreaViewLocationFormats.Add("/ClubsModule/Areas/ClubsModule/{1}/{0}.cshtml");
            options.AreaViewLocationFormats.Add("/ClubsModule/Areas/ClubsModule/Shared/{0}.cshtml");
            options.AreaViewLocationFormats.Add("/ClubsModule/Areas/Views/Shared/{0}.cshtml");
            options.AreaViewLocationFormats.Add("/ClubsModule/Areas/Manager/Views/{1}/{0}.cshtml");
            options.AreaViewLocationFormats.Add("/ClubsModule/Areas/Manager/Views/Shared/{0}.cshtml");
        });

        services.AddHttpContextAccessor();
        App.Modules.Register<Module>();

        services.AddLocalization(options =>
            options.ResourcesPath = "/ClubsModule/Resources"
        );
        services.AddScoped<IHeroesService, HeroesService>();
        services.AddScoped<IClubsService, ClubsService>();
        services.AddScoped<IImagesService, ImagesService>();
        services.AddScoped<IUserManager, UserManager>();
        services.AddScoped<IMissionsService, MissionsService>();
        services.AddScoped<IStoriesService, StoriesService>();
        services.AddTransient<ISchoolYearService, SchoolYearService>();
        services.AddScoped<IMissionIdeasService, MissionIdeasService>();
        services.AddScoped<IMissionContentsService, MissionContentsService>();

        // Add localization service
        services.AddScoped<ManagerLocalizer>();

        services.AddAuthorization(options =>
        {
            // Clubs policies
            options.AddPolicy(Permissions.Clubs, policy =>
            {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permissions.Clubs, Permissions.Clubs);
            });

            options.AddPolicy(Permissions.ClubsAdd, policy =>
            {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permissions.Clubs, Permissions.Clubs);
                policy.RequireClaim(Permissions.ClubsAdd, Permissions.ClubsAdd);
            });

            options.AddPolicy(Permissions.ClubsDelete, policy =>
            {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permissions.Clubs, Permissions.Clubs);
                policy.RequireClaim(Permissions.ClubsDelete, Permissions.ClubsDelete);
            });

            options.AddPolicy(Permissions.ClubsEdit, policy =>
            {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permissions.Clubs, Permissions.Clubs);
                policy.RequireClaim(Permissions.ClubsEdit, Permissions.ClubsEdit);
            });

            options.AddPolicy(Permissions.ClubsSave, policy =>
            {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permissions.Clubs, Permissions.Clubs);
                policy.RequireClaim(Permissions.ClubsSave, Permissions.ClubsSave);
            });

            // Heroes policies
            options.AddPolicy(Permissions.Heroes, policy =>
            {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permissions.Heroes, Permissions.Heroes);
            });

            options.AddPolicy(Permissions.HeroesAdd, policy =>
            {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permissions.Heroes, Permissions.Heroes);
                policy.RequireClaim(Permissions.HeroesAdd, Permissions.HeroesAdd);
            });

            options.AddPolicy(Permissions.HeroesDelete, policy =>
            {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permissions.Heroes, Permissions.Heroes);
                policy.RequireClaim(Permissions.HeroesDelete, Permissions.HeroesDelete);
            });

            options.AddPolicy(Permissions.HeroesEdit, policy =>
            {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permissions.Heroes, Permissions.Heroes);
                policy.RequireClaim(Permissions.HeroesEdit, Permissions.HeroesEdit);
            });

            options.AddPolicy(Permissions.HeroesSave, policy =>
            {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permissions.Heroes, Permissions.Heroes);
                policy.RequireClaim(Permissions.HeroesSave, Permissions.HeroesSave);
            });

            options.AddPolicy(Permissions.HeroesAddCoordinator, policy =>
            {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permissions.Heroes, Permissions.Heroes);
                policy.RequireClaim(Permissions.HeroesAddCoordinator, Permissions.HeroesAddCoordinator);
            });

            // Missions policies
            options.AddPolicy(Permissions.Missions, policy =>
            {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permissions.Missions, Permissions.Missions);
            });

            options.AddPolicy(Permissions.MissionsAdd, policy =>
            {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permissions.Missions, Permissions.Missions);
                policy.RequireClaim(Permissions.MissionsAdd, Permissions.MissionsAdd);
            });

            options.AddPolicy(Permissions.MissionsDelete, policy =>
            {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permissions.Missions, Permissions.Missions);
                policy.RequireClaim(Permissions.MissionsDelete, Permissions.MissionsDelete);
            });

            options.AddPolicy(Permissions.MissionsEdit, policy =>
            {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permissions.Missions, Permissions.Missions);
                policy.RequireClaim(Permissions.MissionsEdit, Permissions.MissionsEdit);
            });

            options.AddPolicy(Permissions.MissionsSave, policy =>
            {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permissions.Missions, Permissions.Missions);
                policy.RequireClaim(Permissions.MissionsSave, Permissions.MissionsSave);
            });

            options.AddPolicy(Permissions.MissionsStars, policy =>
            {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permissions.Missions, Permissions.Missions);
                policy.RequireClaim(Permissions.MissionsStars, Permissions.MissionsStars);
            });

            options.AddPolicy(Permissions.MissionsPublish, policy =>
            {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permissions.Missions, Permissions.Missions);
                policy.RequireClaim(Permissions.MissionsPublish, Permissions.MissionsPublish);
            });

            // Stories policies
            options.AddPolicy(Permissions.Stories, policy =>
            {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permissions.Stories, Permissions.Stories);
            });

            options.AddPolicy(Permissions.StoriesAdd, policy =>
            {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permissions.Stories, Permissions.Stories);
                policy.RequireClaim(Permissions.StoriesAdd, Permissions.StoriesAdd);
            });

            options.AddPolicy(Permissions.StoriesDelete, policy =>
            {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permissions.Stories, Permissions.Stories);
                policy.RequireClaim(Permissions.StoriesDelete, Permissions.StoriesDelete);
            });

            options.AddPolicy(Permissions.StoriesEdit, policy =>
            {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permissions.Stories, Permissions.Stories);
                policy.RequireClaim(Permissions.StoriesEdit, Permissions.StoriesEdit);
            });

            options.AddPolicy(Permissions.StoriesSave, policy =>
            {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permissions.Stories, Permissions.Stories);
                policy.RequireClaim(Permissions.StoriesSave, Permissions.StoriesSave);
            });

            options.AddPolicy(Permissions.StoriesPublish, policy =>
            {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permissions.Stories, Permissions.Stories);
                policy.RequireClaim(Permissions.StoriesPublish, Permissions.StoriesPublish);
            });

            // Mission Ideas policies
            options.AddPolicy(Permissions.MissionIdeas, policy =>
            {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permissions.MissionIdeas, Permissions.MissionIdeas);
            });

            options.AddPolicy(Permissions.MissionIdeasAdd, policy =>
            {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permissions.MissionIdeas, Permissions.MissionIdeas);
                policy.RequireClaim(Permissions.MissionIdeasAdd, Permissions.MissionIdeasAdd);
            });

            options.AddPolicy(Permissions.MissionIdeasDelete, policy =>
            {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permissions.MissionIdeas, Permissions.MissionIdeas);
                policy.RequireClaim(Permissions.MissionIdeasDelete, Permissions.MissionIdeasDelete);
            });

            options.AddPolicy(Permissions.MissionIdeasEdit, policy =>
            {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permissions.MissionIdeas, Permissions.MissionIdeas);
                policy.RequireClaim(Permissions.MissionIdeasEdit, Permissions.MissionIdeasEdit);
            });

            options.AddPolicy(Permissions.MissionIdeasSave, policy =>
            {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permissions.MissionIdeas, Permissions.MissionIdeas);
                policy.RequireClaim(Permissions.MissionIdeasSave, Permissions.MissionIdeasSave);
            });

            options.AddPolicy(Permissions.MissionIdeasPublish, policy =>
            {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permissions.MissionIdeas, Permissions.MissionIdeas);
                policy.RequireClaim(Permissions.MissionIdeasPublish, Permissions.MissionIdeasPublish);
            });
        });

        return services;
    }

    public static void MapClubsModule(this IEndpointRouteBuilder builder)
    {
        builder.MapRazorPages();
    }

    public static IApplicationBuilder UseClubsModule(this IApplicationBuilder builder,
        WebApplicationBuilder webApplicationBuilder)
    {
        // Manager resources
        builder.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                "default",
                "{controller=Home}/{action=Index}/{id?}");

            endpoints.MapClubsModule();
        });

        App.Modules.Get<Piranha.Manager.Module>().Scripts.Add("~/manager/clubsmodule/js/components/blocks/clubs.js");
        App.Modules.Get<Piranha.Manager.Module>().Styles.Add("~/manager/clubsmodule/css/styles.css");
        App.Modules.Get<Piranha.Manager.Module>().Styles
            .Add("https://cdn.jsdelivr.net/npm/summernote@0.8.18/dist/summernote-bs4.min.css");
        App.Modules.Get<Piranha.Manager.Module>().Scripts
            .Add("https://cdn.jsdelivr.net/npm/summernote@0.8.18/dist/summernote-bs4.min.js");
        App.Modules.Get<Piranha.Manager.Module>().Scripts
            .Add("https://cdnjs.cloudflare.com/ajax/libs/jquery-validate/1.19.2/jquery.validate.min.js");
        App.Modules.Get<Piranha.Manager.Module>().Styles
            .Add("https://cdn.jsdelivr.net/npm/@tarekraafat/autocomplete.js@7.2.0/dist/css/autoComplete.min.css");
        App.Modules.Get<Piranha.Manager.Module>().Scripts
            .Add("https://cdn.jsdelivr.net/npm/@tarekraafat/autocomplete.js@7.2.0/dist/js/autoComplete.min.js");
        return builder.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(
                Path.Combine(webApplicationBuilder.Environment.ContentRootPath, "ClubsModule/assets")),
            RequestPath = "/manager/clubsmodule"
        });
    }
}