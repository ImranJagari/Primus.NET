using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Primus.NET.Controllers;
using Primus.NET.Network;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace PrimusNetServer
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddMvc().AddApplicationPart(typeof(PrimusController).Assembly).AddControllersAsServices();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            MessageParser.Initialize();

            app.UseRouting();
            app.UseWebSockets(new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120)
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });

            ///--------------- Handle 101 http response on task ----------------------///
            app.Use(async (context, next) =>
            {
                var socketManager = context.WebSockets;
                if (socketManager.IsWebSocketRequest && context.Request.Query["transport"] == "websocket" && Guid.TryParse(context.Request.Query["sid"], out Guid clientId))
                {
                    context.Response.OnStarting(() =>
                    {
                        context.Response.StatusCode = StatusCodes.Status101SwitchingProtocols;

                        return Task.CompletedTask;
                    });
                }

                await next();
            });
            ///--------------- Handle 101 http response on task ----------------------///
        }
    }
}
