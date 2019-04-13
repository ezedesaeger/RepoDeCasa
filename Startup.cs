using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repositories;
using System;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApp_OpenIDConnect_DotNet.Controllers;
using WebApp_OpenIDConnect_DotNet.Extensions;

namespace WebApp_OpenIDConnect_DotNet
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddAuthentication(AzureADDefaults.AuthenticationScheme)
                .AddAzureAD(options => Configuration.Bind("AzureAd", options));

            //Registering dependency injection
            Assembly.Load(Assembly.GetExecutingAssembly().GetName().Name)
              .GetTypes()
              .Where(t => t.GetInterfaces().Any(i => i.Name == "I" + t.Name)).ToList()
              .ForEach(t => services.AddTransient(t.GetInterface("I" + t.Name, false), t));

            Assembly.Load("Repositories")
              .GetTypes()
              .Where(t => t.GetInterfaces().Any(i => i.Name == "I" + t.Name)).ToList()
              .ForEach(t => services.AddTransient(t.GetInterface("I" + t.Name, false), t));


            services.Configure<OpenIdConnectOptions>(AzureADDefaults.OpenIdScheme, options =>
            {
                options.Authority = options.Authority + "/v2.0/";

                options.TokenValidationParameters.ValidateIssuer = false;
                options.Events = new OpenIdConnectEvents()
                {
                    OnTicketReceived = (context) =>
                    {
                        ClaimsIdentity claimsId = context.Principal.Identity as ClaimsIdentity;

                        claimsId.AddClaims(services.BuildServiceProvider().GetRequiredService<Imyservice>().GetClaims());

                        return Task.FromResult(0);
                    }
                };

            });
            services.AddDistributedMemoryCache();
            services.AddSession(options => options.Cookie.IsEssential = true);

            services.AddMvc(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);


            // CONFIGURACION DE SQLITE IN-MEMORY
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            services.AddDbContext<BaseRepoContext>(options =>
                options.UseSqlite(connection));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
            }
            else
            {
                //app.UseExceptionHandler("/Home/Error");
                //app.UseHsts();
            }

            var eze = serviceProvider.GetService<Imyservice>();
            app.ConfigureExceptionHandler(eze);
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
