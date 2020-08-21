using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace api
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {

                    try
                    {
                        using var client = new System.Net.Http.HttpClient();
                        var request = new System.Net.Http.HttpRequestMessage();
                        request.RequestUri = new Uri("http://backend/");
                        var header = "azds-route-as";
                        if (context.Request.Headers.ContainsKey(header))
                        {
                            request.Headers.Add(header, context.Request.Headers[header] as IEnumerable<string>);
                        }
                        var response = await client.SendAsync(request);
                        if(response.IsSuccessStatusCode) {
                            await context.Response.WriteAsync($"API saw this --> {await response.Content.ReadAsStringAsync()}");
                        }
                        else {
                            await context.Response.WriteAsync("Couldn't connect...");
                        }                        
                    }
                    catch (Exception ex)
                    {
                        await context.Response.WriteAsync(ex.Message);
                    }

                });
            });
        }
    }
}
