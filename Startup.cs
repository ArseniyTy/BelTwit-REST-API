using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using BelTwit_REST_API.Models;
using BelTwit_REST_API.Models.JWT_token;
using Microsoft.EntityFrameworkCore;
using BelTwit_REST_API.Additional;

namespace BelTwit_REST_API
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
            //устанавливаем секретный ключ для JWT из конфигурации
            Signature.SecretKey = Configuration["JWTSecretKey"];


            //ізвлекаем строку подключенія із файла конфігураціі
            string connectionStr = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<BelTwitContext>(options => options.UseSqlServer(connectionStr));


            //создаём options для созданія контекста бд в будущем
            var optionsBuilder = new DbContextOptionsBuilder<BelTwitContext>();
            //это наш созданный класс
            Helper.BelTwitDbOptions = optionsBuilder.UseSqlServer(connectionStr).Options;


            //Install-Package Microsoft.AspNetCore.Mvc.NewtonsoftJson -Version 3.0.0-preview8.19405.7
            services.AddControllers().AddNewtonsoftJson();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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
