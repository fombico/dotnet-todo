using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Steeltoe.Extensions.Configuration;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi
{
    public class Startup
    {

        public IConfiguration Configuration { get; set; }
        public IHostingEnvironment Environment { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Console.WriteLine("Configure Services");

            services.AddDbContext<TodoContext>(opt =>
            {

                if (Environment.IsEnvironment("Integration Test"))
                {
                    opt.UseInMemoryDatabase("Integration Test");
                } 
                else
                {
                    var hostname = $@"{Configuration["vcap:services:cleardb:0:credentials:hostname"]}";
                    var name = $@"{Configuration["vcap:services:cleardb:0:credentials:name"]}";
                    var username = $@"{Configuration["vcap:services:cleardb:0:credentials:username"]}";
                    var password = $@"{Configuration["vcap:services:cleardb:0:credentials:password"]}";
                    var connectionString = $@"server={hostname};database={name};uid={username};pwd={password};";
                    Console.WriteLine("Connection: " + connectionString);
                    opt.UseMySql(connectionString);
                }
                
            });
            
            services.AddTransient(typeof(TodoService));
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            Console.WriteLine("Configure, env: " + env.EnvironmentName);

            app.UseCors(builder =>
                {
                    builder.AllowAnyHeader();
                    builder.AllowAnyMethod();
                    builder.AllowAnyOrigin();
                }
            );
            
            Configuration = new ConfigurationBuilder()
                            .SetBasePath(env.ContentRootPath)
                            .AddJsonFile("appsettings.json")
                            .AddCloudFoundry()
                            .AddEnvironmentVariables()
                            .Build();

            Environment = env;
            
            app.UseMvc();
        }
    }
}