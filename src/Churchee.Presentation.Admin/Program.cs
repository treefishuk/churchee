using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Extensibility;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.Helpers;
using Churchee.Data.EntityFramework;
using Churchee.EmailConfiguration.MailGun.Infrastructure;
using Churchee.Module.Identity.Entities;
using Churchee.Module.Identity.Infrastructure;
using Churchee.Module.Identity.Managers;
using Churchee.Module.Tenancy.Infrastructure;
using Churchee.Module.UI.Models;
using Churchee.Presentation.Admin.PipelineBehavoirs;
using Churchee.Presentation.Admin.Registrations;
using FluentValidation;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SignalR;
using Radzen;

namespace Churchee.Presentation.Admin
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.UseKestrel(option => option.AddServerHeader = false);

            var assemblies = AssemblyResolution.GetModuleAssemblies().Append(typeof(Program).Assembly).ToArray();

            builder.Services.AddRazorPages();
            builder.Services.AddRadzenComponents();

            builder.Services.AddScoped<ICurrentUser, CurrentUser>();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<ITenantResolver, ClaimTenantResolver>();

            builder.Services.Configure<HubOptions>(options =>
            {
                options.MaximumReceiveMessageSize = 10 * 1024 * 1024; // 10MB or use null
            });

            builder.Services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true; // Enable compression for HTTPS requests
                options.Providers.Add<GzipCompressionProvider>();
                options.Providers.Add<BrotliCompressionProvider>();

            });

            builder.Services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = System.IO.Compression.CompressionLevel.Fastest; // Set the compression level
            });

            builder.Services.RegisterSeedActions();
            builder.Services.AddHttpClient();

            // Add services to the container.
            builder.Services.AddRazorComponents()
                            .AddInteractiveServerComponents();

            builder.Services.AddCascadingAuthenticationState();

            builder.Services.AddServicesActions();
            builder.Services.AddScoped<CurrentPage>();

            string? emailService = builder.Configuration.GetSection("Email").GetValue<string>("Service");

            if (string.IsNullOrEmpty(emailService))
            {
                emailService = "Logger";
            }

            switch (emailService)
            {
                case "MailGun":
                    builder.Services.AddScoped<IEmailService, MailGunEmailService>();
                    break;
                default:
                    builder.Services.AddScoped<IEmailService, LoggerEmailService>();
                    break;
            }


            builder.Services.RegisterAllTypes<IMenuRegistration>(ServiceLifetime.Scoped);
            builder.Services.RegisterAllTypes<IScriptRegistrations>(ServiceLifetime.Scoped);

            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblies));
            builder.Services.AddValidatorsFromAssemblies(assemblies);
            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ExceptionHandlingBehavior<,>));
            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));

            builder.Services.RunSeedActions();

            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddSignInManager<ChurcheeSignInManager>()
                .AddDefaultTokenProviders();


            builder.Services.AddAntiforgery(options =>
            {
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });


            if (!builder.Environment.IsDevelopment())
            {
                builder.Services.AddHsts(options =>
                {
                    options.Preload = true;
                    options.IncludeSubDomains = true;
                    options.MaxAge = TimeSpan.FromDays(365);
                });
            }

            var app = builder.Build();

            app.UseExceptionHandler("/Error");
            app.UseStatusCodePagesWithReExecute("/errors/{0}");

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseHsts();
            }

            app.UseCookiePolicy(new CookiePolicyOptions
            {
                HttpOnly = HttpOnlyPolicy.Always,
                Secure = CookieSecurePolicy.Always
            });

            app.UseHttpsRedirection();

            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = (context) =>
                {
                    context.Context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
                    context.Context.Response.Headers.Append("Permissions-Policy", "accelerometer=(), autoplay=(), camera=(), cross-origin-isolated=(), display-capture=(), encrypted-media=(), fullscreen=(), geolocation=(), gyroscope=(), keyboard-map=(), magnetometer=(), microphone=(), midi=(), payment=(), picture-in-picture=(), publickey-credentials-get=(), screen-wake-lock=(), sync-xhr=(self), usb=(), web-share=(), xr-spatial-tracking=(), clipboard-read=(), clipboard-write=(), gamepad=(), hid=(), idle-detection=(), interest-cohort=(), serial=(), unload=()");
                }
            });


            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseAntiforgery();
            app.UseSecurityHeadersMiddleware();

            app.MapRazorComponents<Components.App>()
                .AddInteractiveServerRenderMode()
                .AddAdditionalAssemblies(AssemblyResolution.GetModuleAssemblies());

            app.MapRazorPages();

            app.UseChurcheeHangfireDashboard();

            app.Run();
        }
    }
}
