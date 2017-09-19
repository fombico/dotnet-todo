using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<TodoContext>(opt =>
            {
                var hostname = $@"{Configuration["vcap:services:cleardb:0:credentials:hostname"]}";
                var name = $@"{Configuration["vcap:services:cleardb:0:credentials:name"]}";
                var username = $@"{Configuration["vcap:services:cleardb:0:credentials:username"]}";
                var password = $@"{Configuration["vcap:services:cleardb:0:credentials:password"]}";
                var connectionString = $@"server={hostname};database={name};uid={username};pwd={password};";
                Console.WriteLine("Connection: " +  connectionString);
                opt.UseMySql(connectionString);
            });
            services.AddMvc();

            services.AddTransient(typeof(TodoService));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            Console.WriteLine("Configure, env: " + env.EnvironmentName);
//            if (env.IsDevelopment())
//            {
//                app.UseDeveloperExceptionPage();
//            }
            var builder = new ConfigurationBuilder()
                            .SetBasePath(env.ContentRootPath)
                            .AddJsonFile("appsettings.json")
                            .AddCloudFoundry();

            Configuration = builder.AddEnvironmentVariables().Build();
            
//            Console.WriteLine(" ============ Configuration ============");
//            foreach(var environ in Configuration.GetChildren())
//            {
//                Console.WriteLine($"{environ.Key}:{ environ.Value}");
//            }
//            Console.WriteLine(" ============ End of Configuration ============");
            
            app.UseMvc();
        }
    }
}