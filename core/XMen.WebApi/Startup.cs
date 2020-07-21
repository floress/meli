using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using XMen.Data;
using XMen.Tools;

namespace XMen.WebApi
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
                //.ConfigureApiBehaviorOptions(options =>
                //{
                //    options.SuppressMapClientErrors = true; //Removing ProblemDetails
                //});

            services.Configure<PostgresqlOptions>(Configuration.GetSection(PostgresqlOptions.Seccion));
            services.Configure<BigTableOptions>(Configuration.GetSection(BigTableOptions.Seccion));
            services.AddSingleton<IMutantDetector, MutantDetector>();
            services.AddTransient<IPostgresqlManager, PostgresqlManager>();
            services.AddTransient<IBigTableManager, BigTableManager>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            app.UseDeveloperExceptionPage();
            //}

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
