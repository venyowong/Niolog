﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ExtCore.WebApplication.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Niolog.Web
{
    public class Startup
    {
        private string extensionsPath;

        public IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment hostingEnvironment, IConfiguration configuration)
        {
            Configuration = configuration;
            this.extensionsPath = Path.Combine(hostingEnvironment.ContentRootPath, configuration["Extensions:Path"]);
        }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddExtCore(this.extensionsPath, true);

            services.AddMvc();

            services.AddOptions();
            services.Configure<AppSettings>(this.Configuration);

            services.AddCors(o => o.AddPolicy("Default", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var defaultFile = new DefaultFilesOptions();  
            defaultFile.DefaultFileNames.Clear();  
            defaultFile.DefaultFileNames.Add("index.html");  
            app.UseDefaultFiles(defaultFile);
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseCors("Default");

            app.UseExtCore();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}