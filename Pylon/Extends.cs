using Aiursoft.Pylon.Interfaces;
using Aiursoft.Pylon.Middlewares;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.API.OAuthAddressModels;
using Aiursoft.Pylon.Services;
using Aiursoft.Pylon.Services.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Aiursoft.Pylon
{
    public static class Extends
    {
        private static CultureInfo[] GetSupportedLanguages()
        {
            var supportedCultures = new[]
            {
                new CultureInfo("en"),
                new CultureInfo("zh")
            };
            return supportedCultures;
        }

        public static IApplicationBuilder UseAiurAPIHandler(this IApplicationBuilder app, bool isDevelopment)
        {
            if (isDevelopment)
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseMiddleware<HandleRobotsMiddleware>();
                app.UseMiddleware<EnforceHttpsMiddleware>();
                app.UseMiddleware<APIFriendlyServerExceptionMiddeware>();
            }
            return app;
        }

        public static IApplicationBuilder UseAiurUserHandler(this IApplicationBuilder app, bool isDevelopment)
        {
            if (isDevelopment)
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseMiddleware<HandleRobotsMiddleware>();
                app.UseMiddleware<EnforceHttpsMiddleware>();
                app.UseMiddleware<UserFriendlyServerExceptionMiddeware>();
                app.UseMiddleware<UserFriendlyNotFoundMiddeware>();
            }
            return app;
        }

        /// <summary>
        /// Static files, routing, auth, language switcher, endpoints.
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseAiursoftDefault(this IApplicationBuilder app)
        {
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en"),
                SupportedCultures = GetSupportedLanguages(),
                SupportedUICultures = GetSupportedLanguages()
            });
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<SwitchLanguageMiddleware>();
            app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
            app.UseDocGenerator();
            return app;
        }

        public static IApplicationBuilder UseDocGenerator(this IApplicationBuilder app)
        {
            return app.UseMiddleware<APIDocGeneratorMiddleware>();
        }

        public static IActionResult SignOutRootServer(this Controller controller, string apiServerAddress, AiurUrl viewingUrl)
        {
            var request = controller.HttpContext.Request;
            string serverPosition = $"{request.Scheme}://{request.Host}{viewingUrl}";
            var toRedirect = new AiurUrl(apiServerAddress, "OAuth", "UserSignout", new UserSignoutAddressModel
            {
                ToRedirect = serverPosition
            });
            return controller.Redirect(toRedirect.ToString());
        }

        public static JsonResult Protocol(this Controller controller, ErrorType errorType, string errorMessage)
        {
            return controller.Json(new AiurProtocol
            {
                Code = errorType,
                Message = errorMessage
            });
        }

        public static void SetClientLang(this Controller controller, string culture)
        {
            controller.HttpContext.Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });
        }

        public static IHost MigrateDbContext<TContext>(this IHost host, Action<TContext, IServiceProvider> seeder = null) where TContext : DbContext
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<TContext>>();
                var context = services.GetService<TContext>();
                var configuration = services.GetService<IConfiguration>();
                var env = services.GetService<IWebHostEnvironment>();

                var connectionString = configuration.GetConnectionString("DatabaseConnection");
                try
                {
                    logger.LogInformation($"Migrating database associated with context {typeof(TContext).Name}");
                    logger.LogInformation($"Connection string is {connectionString}");
                    var retry = Policy.Handle<Exception>().WaitAndRetry(new[]
                    {
                        TimeSpan.FromSeconds(5),
                        TimeSpan.FromSeconds(10),
                        TimeSpan.FromSeconds(15),
                    });

                    retry.Execute(() =>
                    {
                        // Migrate even in production level.
                        context.Database.Migrate();
                        try
                        {
                            seeder?.Invoke(context, services);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, $"An error occurred while seeding the database used on context {typeof(TContext).Name}");
                        }
                    });
                    logger.LogInformation($"Migrated database associated with context {typeof(TContext).Name}");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"An error occurred while migrating the database used on context {typeof(TContext).Name}");
                }
            }

            return host;
        }

        public static IServiceCollection AddAiurMvc(this IServiceCollection services)
        {
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services
                .AddControllersWithViews()
                .AddNewtonsoftJson()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            return services;
        }

        public static IEnumerable<T> AddWith<T>(this IEnumerable<T> input, T toadd)
        {
            var list = input.ToList();
            list.Add(toadd);
            return list;
        }

        public static List<Type> AllAccessiableClass()
        {
            var entry = Assembly.GetEntryAssembly();
            return entry
                .GetReferencedAssemblies()
                .ToList()
                .Select(t => Assembly.Load(t))
                .AddWith(entry)
                .SelectMany(t => t.GetTypes())
                .Where(t => !t.IsAbstract)
                .Where(t => !t.IsNestedPrivate)
                .Where(t => !t.IsGenericType)
                .Where(t => !t.IsInterface)
                .Where(t => !(t.Namespace?.StartsWith("System") ?? true))
                .ToList();
        }

        public static IServiceCollection AddAiurDependencies<TUser>(this IServiceCollection services, string appName) where TUser : AiurUserBase, new()
        {
            services.AddScoped<UserImageGenerator<TUser>>();
            services.AddTransient<AuthService<TUser>>();
            services.AddAiurDependencies(appName);
            return services;
        }

        public static IServiceCollection AddAiurDependencies(this IServiceCollection services, string appName)
        {
            AppsContainer.CurrentAppName = appName;
            services.AddHttpClient();
            services.AddMemoryCache();
            var executingTypes = AllAccessiableClass();
            foreach (var item in executingTypes)
            {
                if (item.GetInterfaces().Contains(typeof(ISingletonDependency)))
                {
                    if (item.GetInterfaces().Contains(typeof(IHostedService)))
                    {
                        services.AddSingleton(typeof(IHostedService), item);
                    }
                    else
                    {
                        services.AddSingleton(item);
                    }
                    Console.WriteLine($"Service: {item.Name} - was successfully registered as a singleton service.");
                }
                else if (item.GetInterfaces().Contains(typeof(IScopedDependency)))
                {
                    if (item.GetInterfaces().Contains(typeof(IAuthProvider)))
                    {
                        services.AddScoped(typeof(IAuthProvider), item);
                    }
                    else
                    {
                        services.AddScoped(item);
                    }
                    Console.WriteLine($"Service: {item.Name} - was successfully registered as a scoped service.");
                }
                else if (item.GetInterfaces().Contains(typeof(ITransientDependency)))
                {
                    services.AddTransient(item);
                    Console.WriteLine($"Service: {item.Name} - was successfully registered as a transient service.");
                }
            }
            return services;
        }

        //regex from http://detectmobilebrowsers.com/
        private static readonly Regex _mobile = new Regex(@"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        private static readonly Regex _version = new Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.IgnoreCase | RegexOptions.Multiline);

        public static bool IsMobileBrowser(this HttpRequest request)
        {
            var userAgent = request.UserAgent();
            if ((_mobile.IsMatch(userAgent) || _version.IsMatch(userAgent.Substring(0, 4))))
            {
                return true;
            }

            return false;
        }

        public static string UserAgent(this HttpRequest request)
        {
            return request.Headers["User-Agent"];
        }

        public static Task ForEachParallel<T>(this IEnumerable<T> items, Func<T, Task> function)
        {
            var taskList = new List<Task>();
            foreach (var item in items)
            {
                taskList.Add(function(item));
            }
            return Task.WhenAll(taskList);
        }

        public static bool AllowTrack(this HttpContext httpContext)
        {
            var dntFlag =
                httpContext.Request.Headers.ContainsKey("dnt") ? httpContext.Request.Headers["dnt"].ToString() :
                string.Empty;
            bool dnt = !string.IsNullOrWhiteSpace(dntFlag) && dntFlag.Trim() == 1.ToString();
            return !dnt;
        }
    }
}
