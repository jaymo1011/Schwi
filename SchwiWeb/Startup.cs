using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNet.Security.OAuth.Discord;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SchwiWeb
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // Authentication via Discord (and cookie)
            services.AddAuthentication(options => {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = DiscordAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = DiscordAuthenticationDefaults.AuthenticationScheme;
            })
                .AddCookie()
                .AddDiscord(options =>
                {
                    // Quite obviously debug because it's stored in user secrets but this will be swapped out for regular config eventually
                    options.ClientId = Configuration["DiscordClientId"];
                    options.ClientSecret = Configuration["DiscordClientSecret"];
                });

            services.AddRouting();

            services.AddHostedService<SchwiBot.SchwiBot>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}
