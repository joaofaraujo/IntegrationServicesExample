using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProcessData;
using ProcessData.Repositories;
using ProcessData.Repositories.SSIS;
using ProcessDomain.Domain;
using ProcessDomain.Interfaces.Repositories;
using ProcessDomain.Interfaces.Services;
using ProcessDomain.Services;

namespace APIExample
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
            services.AddControllers();

            var packageIntegrationServices = new PackageIntegrationServices();
            Configuration.Bind("PackageIntegrationServices", packageIntegrationServices);
            services.AddSingleton(packageIntegrationServices);
            
            services.AddScoped<ContextoSSISDB>();
            services.AddScoped<IExecucaoDtsxRepository, ExecucaoDtsxRepository>();
            services.AddScoped<IProcessFileService, ProcessFileService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
