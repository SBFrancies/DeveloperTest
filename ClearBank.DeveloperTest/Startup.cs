using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Interface;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ClearBank.DeveloperTest
{
    public class Startup
    {
        private const string ApiTitle = "Developer Test API";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        { 
            services.AddControllers().AddJsonOptions(a =>
            {
                a.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                a.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                a.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            services.AddSwaggerGen();

            var settings = new PaymentSettings();
            Configuration.Bind("PaymentApi", settings);

            if(settings.DataStoreType == "Backup")
            {
                services.AddSingleton<IDataStore, BackupAccountDataStore>();
            }

            else
            {
                services.AddSingleton<IDataStore, AccountDataStore>();
            }

            services.AddSingleton<IPaymentService, PaymentService>();
            services.AddSingleton<IPaymentValidator, BacsPaymentValidator>();
            services.AddSingleton<IPaymentValidator, ChapsPaymentValidator>();
            services.AddSingleton<IPaymentValidator, FasterPaymentsValidator>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", ApiTitle);
                c.DocumentTitle = ApiTitle;
                c.EnableDeepLinking();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
