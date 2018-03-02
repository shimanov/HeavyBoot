using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using HeavyBoot.Api.Model;
using Microsoft.EntityFrameworkCore;

namespace HeavyBoot.Api
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
            //TODO: Указать параметры подключения
            const string conn = "Server=; Database=;user id=;password=";
            services.AddDbContext<ConnectionToDb>(o => o.UseSqlServer(conn));
            //Сжатие ответа
            services.AddResponseCompression();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //Сжатие ответа
            app.UseResponseCompression();
            app.UseMvc();
        }
    }
}
