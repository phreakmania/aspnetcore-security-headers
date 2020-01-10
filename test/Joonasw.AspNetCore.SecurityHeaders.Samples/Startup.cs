using Joonasw.AspNetCore.SecurityHeaders.Samples.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Joonasw.AspNetCore.SecurityHeaders.Samples
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Setup mapping from config file to configuration
            services.Configure<HstsOptions>(Configuration.GetSection("Hsts"));
            services.Configure<CspOptions>(Configuration.GetSection("Csp"));
            services.Configure<HpkpOptions>(Configuration.GetSection("Hpkp"));
            services.Configure<FeaturePolicyOptions>(Configuration.GetSection("FeaturePolicy"));

            // Add framework services.
            services.AddControllersWithViews();

            // Add CSP nonce support
            services.AddJoonaswCsp(nonceByteAmount: 32);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHttpsEnforcement();

                app.UseJoonaswHsts();
                // Manual configuration
                //app.UseJoonaswHsts(new HstsOptions(TimeSpan.FromDays(30), includeSubDomains: false, preload: false));

                app.UseJoonaswHpkp();
                // Manual configuration
                //app.UseJoonaswHpkp(hpkp =>
                //{
                //    hpkp.UseMaxAgeSeconds(30 * 24 * 60 * 60)
                //        .AddSha256Pin("nrmpk4ZI3wbRBmUZIT5aKAgP0LlKHRgfA2Snjzeg9iY=")
                //        .SetReportOnly()
                //        .ReportViolationsTo("/hpkp-report");
                //});
            }

            app.UseStaticFiles();

            app.UseJoonaswCsp();
            // Manual configuration
            //app.UseJoonaswCsp(csp =>
            //{
            //    //csp.EnableSandbox()
            //    //    .AllowScripts();
            //    csp.AllowScripts
            //        .FromSelf()
            //        .From("localhost:1591")
            //        .From("ajax.aspnetcdn.com")
            //        .AddNonce();

            //    csp.AllowStyles
            //        .FromSelf()
            //        .From("ajax.aspnetcdn.com")
            //        .AddNonce();

            //    csp.AllowImages
            //        .FromSelf();

            //    csp.AllowAudioAndVideo
            //        .FromNowhere();

            //    csp.AllowFrames
            //        .FromNowhere();

            //    csp.AllowWorkers
            //        .FromNowhere();

            //    csp.AllowConnections
            //        .To("ws://localhost:1591")
            //        .To("http://localhost:1591")
            //        .ToSelf();

            //    csp.AllowFonts
            //        .FromSelf()
            //        .From("ajax.aspnetcdn.com");

            //    csp.AllowPlugins
            //        .FromNowhere();

            //    csp.AllowFraming
            //        .FromNowhere();

            //    csp.RequireSri
            //        .ForScripts()
            //        .ForStyles(); // Require subresource integrity for scripts and stylesheets

            //    csp.SetReportOnly();
            //    csp.ReportViolationsTo("/csp-report");
            //    csp.SetUpgradeInsecureRequests(); //Upgrade HTTP URIs to HTTPS

            //    csp.OnSendingHeader = context =>
            //    {
            //        context.ShouldNotSend = context.HttpContext.Request.Path.StartsWithSegments("/api");
            //        return Task.CompletedTask;
            //    };
            //});

            app.UseJoonaswXFrameOptions();
            // Manual Configuration
            //app.UseJoonaswXFrameOptions(new XFrameOptionsOptions(XFrameOptionsOptions.XFrameOptionsValues.Deny));

            app.UseJoonaswXXssProtection();
            // Manual Configuration
            //app.UseJoonaswXXssProtection(new XXssProtectionOptions(false, false));

            app.UseJoonaswXContentTypeOptions();
            // Manual Configuration
            //app.UseJoonaswXContentTypeOptions(new XContentTypeOptionsOptions(true));

            app.UseJoonaswReferrerPolicy();
            // Manual Configuration
            //app.UseJoonaswReferrerPolicy(
            //    new ReferrerPolicyOptions(ReferrerPolicyOptions.ReferrerPolicyValues.NoReferrerWhenDowngrade));

            app.UseJoonaswExpectCT();
            // Manual Configuration
            //app.UseJoonaswExpectCT(
            //    new ExpectCTOptions(TimeSpan.FromSeconds(30), "/expect-ct-report", true));

            app.UseJoonaswFeaturePolicy();
            // Inline configuration
            //app.UseJoonaswFeaturePolicy(fp =>
            //{
            //    fp.AllowGeolocation
            //        .FromSelf()
            //        .From("https://google.com");

            //    fp.AllowMidi
            //        .FromAnywhere();

            //    fp.AllowNotifications
            //        .FromSelf();

            //    fp.AllowPush
            //        .FromNowhere();

            //    fp.AllowSyncXhr
            //        .FromNowhere();

            //    fp.AllowMicrophone
            //        .FromAnywhere();

            //    fp.AllowCamera
            //        .FromNowhere();

            //    fp.AllowMagnetometer
            //        .FromSelf();

            //    fp.AllowGyroscope
            //        .FromSelf()
            //        .From("https://google.com")
            //        .From("https://www.yahoo.com");

            //    fp.AllowSpeaker
            //        .FromNowhere();

            //    fp.AllowVibrate
            //        .FromNowhere();

            //    fp.AllowFullscreen
            //        .FromSelf();

            //    fp.AllowPayment.
            //        FromNowhere();

            //    fp.AllowOtherFeature("some-new-one")
            //        .FromNowhere();
            //});

            app.UseRouting();
            app.UseEndpoints(routes =>
            {
                routes.MapControllerRoute(
                    name: "csp-report",
                    pattern: "csp-report",
                    defaults: new { controller = "Report", action = "Csp" });

                routes.MapControllerRoute(
                    name: "hpkp-report",
                    pattern: "hpkp-report",
                    defaults: new { controller = "Report", action = "Hpkp" });

                routes.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
