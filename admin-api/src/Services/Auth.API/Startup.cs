using CorrelationId;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using API.Extensions;
using Utils;
using Boxed.AspNetCore;
using Swashbuckle.AspNetCore.Swagger;
using Admin.API.Infrastructure.Migrations;
using API.Filters;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;
using System.Reflection;

namespace API
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            HostingEnvironment = hostingEnvironment;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string dbType = "Oracle";
            services.AddMvc(x => x.Filters.Add(typeof(LogFilter))).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // Register Context
            switch (dbType)
            {
                case "MSSQL":
                    services.AddDbContext<ModelContext>(opts => opts.UseSqlServer(Configuration["ConnectionString:" + dbType]));
                    break;
                case "Oracle":
                    services.AddDbContext<ModelContext>(opts => opts.UseOracle(Configuration["ConnectionString:" + dbType]));
                    break;
                default:
                    services.AddDbContext<ModelContext>(opts => opts.UseSqlServer(Configuration["ConnectionString:" + dbType]));
                    break;
            }

            services
                .AddCustomCaching()
                .AddCustomOptions(Configuration)
                .AddCorrelationIdFluent()
                .AddCustomRouting()
                .AddResponseCaching()
                .AddCustomResponseCompression()
                .AddCustomStrictTransportSecurity()
                .AddHttpContextAccessor()
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .AddCustomApiVersioning()
                .AddMvcCore()
                .AddApiExplorer()
                .AddAuthorization()
                .AddDataAnnotations()
                .AddJsonFormatters()
                .AddCustomJsonOptions(HostingEnvironment)
                .AddCustomCors(Configuration)
                .AddCustomMvcOptions(HostingEnvironment)
                .Services
                .AddProjectServices()
                .BuildServiceProvider();

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {

                c.SwaggerDoc("v1", new Info { Title = "Auth API", Version = "v1" });
                var xmlFile = typeof(Startup).GetTypeInfo().Assembly.GetName().Name + ".xml";
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, xmlFile);
                c.IncludeXmlComments(xmlPath);
                c.EnableAnnotations();
                //c.OperationFilter<MyHeaderFilter>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Pass a GUID in a X-Correlation-ID HTTP header to set the HttpContext.TraceIdentifier.
            app.UseCorrelationId()
            .UseForwardedHeaders()
            .UseResponseCaching()
            .UseResponseCompression()
            .UseCors(CorsPolicyName.AllowAny)
            .UseDeveloperErrorPages()
            // .UseIf(
            //     this.hostingEnvironment.IsDevelopment(),
            //     x => x.UseDeveloperErrorPages())
            .UseStaticFilesWithCacheControl()
            .UseAuthentication()
            .UseIf(
                Configuration["Cors:AllowAll"] == "true",
                x => x.UseCors(CorsPolicyName.AllowAny))
            .UseIf(
                Configuration["Cors:AllowAll"] == "true",
                x => x.UseCors(CorsPolicyName.AllowFrontEnd).UseCors(CorsPolicyName.AllowThirdparty))
            .UseMvc();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auth API V1");
            });
        }
    }
}
