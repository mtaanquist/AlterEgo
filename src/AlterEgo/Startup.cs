using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AlterEgo.Data;
using AlterEgo.Models;
using AlterEgo.Services;
using AspNet.Security.OAuth.BattleNet;
using Microsoft.AspNetCore.Localization;
using Sakura.AspNetCore.Mvc;

namespace AlterEgo
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("PostgreSqlConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.Cookies.ApplicationCookie.ExpireTimeSpan = TimeSpan.FromDays(28);
                    options.User.AllowedUserNameCharacters =
                        "aáàäbcdeéèfghiíìjklmnoóòöpqrstuüvwxyzæøåAÁÀÄBCDEÉÈFGHIÍÌJKLMNOÓÒÖPQRSTUÜVWXYÆØÅZ0123456789#";
                    options.Cookies.ApplicationCookie.LoginPath = "/account/login/";
                    options.Cookies.ApplicationCookie.ReturnUrlParameter = "returnUrl";
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc();
            services.AddRouting(options =>
            {
                options.AppendTrailingSlash = true;
                options.LowercaseUrls = true; 
            });

            services.AddBootstrapPagerGenerator(options => 
            {
                options.ConfigureDefault();
            });

            services.Configure<BattleNetOptions>(Configuration.GetSection("BattleNet"));

            // Add scoped services
            services.AddScoped<BattleNetApi>();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();

            // Seed data
            services.AddTransient<ApplicationDbContextSeedData>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, ApplicationDbContextSeedData seeder)
        {
            // Localisation
            var supportedCultures = new List<CultureInfo>
            {
                new CultureInfo("en-US"),
                new CultureInfo("en-AU"),
                new CultureInfo("en-GB"),
                new CultureInfo("es-ES"),
                new CultureInfo("ja-JP"),
                new CultureInfo("fr-FR"),
                new CultureInfo("zh"),
                new CultureInfo("zh-CN")
            };

            var options = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-GB"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            };

            app.UseRequestLocalization(options);

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/home/error");
            }

            app.Use(next => context =>
            {
                if (string.Equals(context.Request.Headers["X-Forwarded-Proto"], "https",
                    StringComparison.OrdinalIgnoreCase))
                {
                    context.Request.Scheme = "https";
                }

                return next(context);
            });

            app.UseStaticFiles();
            app.UseIdentity();

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715
            app.UseBattleNetAuthentication(new BattleNetAuthenticationOptions()
            {
                ClientId = Configuration.GetSection("BattleNet").GetValue<string>("ClientId"),
                ClientSecret = Configuration.GetSection("BattleNet").GetValue<string>("ClientSecret"),

                Region = BattleNetAuthenticationRegion.Europe,
                SaveTokens = true,

                Scope = { "wow.profile" }
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            seeder.SeedData();
        }
    }
}
