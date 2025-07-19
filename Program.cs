using itRoot.Models;
using itRoot.Repos;
using itRoot.Repos.IRepos;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using itRoot.UnitOfWorks.IUnitOfWorks;
using itRoot.UnitOfWorks;
using itRoot.Services.IServices;
using itRoot.Services;
namespace itRoot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region langauges
            builder.Services.AddControllersWithViews()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);

            builder.Services.AddLocalization(op =>
            {
                op.ResourcesPath = "Resources";
            });

            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[] { "en-US", "ar-EG" };
                options.SetDefaultCulture(supportedCultures[0])
                    .AddSupportedCultures(supportedCultures)
                    .AddSupportedUICultures(supportedCultures);

                options.RequestCultureProviders = new List<IRequestCultureProvider>
                {
                    new CookieRequestCultureProvider(),
                    new QueryStringRequestCultureProvider(),
                    new AcceptLanguageHeaderRequestCultureProvider()
                };
            });

            #endregion

            #region DI
            builder.Services.AddDbContext<dbContext>(op => op.UseSqlServer(builder.Configuration.GetConnectionString("cs")));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            #endregion
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
             .AddCookie(options =>
             {
                 options.LoginPath = "/Account/Login";
                 options.LogoutPath = "/Account/Logout";
             })
             .AddGoogle(options =>
             {
                 options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                 options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
                 options.SaveTokens = true;
             });
            var app = builder.Build();
            //var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();
            //app.UseRequestLocalization(localizationOptions.Value);
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.UseRequestLocalization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");

            app.Run();
        }
    }
}
