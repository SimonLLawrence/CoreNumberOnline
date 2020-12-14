using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreNumberAPI.Factory;
using CoreNumberAPI.Model;
using CoreNumberAPI.Processors;
using CoreNumberAPI.Repository;
using CoreNumberAPI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CoreNumberAPI
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
            var testSecret = new ApiSecrets
            {
                SecretId = Guid.Empty.ToString(),
                Key = Configuration["TestBinanceSecret:ExchangeKey"],
                Secret = Configuration["TestBinanceSecret:ExchangeSecret"],
                SubaccountName = Configuration["TestBinanceSecret:ExchangeAccount"]
            };

            services.AddSingleton(testSecret);

            services.AddControllers();
            services.AddSwaggerGen();
            services.AddTransient<IBotProcessManager, BotProcessManager>();
            services.AddTransient<IExchangeFactory, ExchangeFactory>();
            services.AddTransient<IBotProcessorFactory, BotProcessorFactory>();
            services.AddTransient<IBotProcessor, CoreNumberProcessor>();
            services.AddTransient<IExchange, BinanceService>();
            services.AddTransient<IInstanceConfigurationService, InstanceConfigurationService>();
            services.AddSingleton<ITradingViewAlertService, TradingViewAlertService>();
            services.AddSingleton<IBotInstanceDataRepository, MemoryBotInstanceDataRepository>();
            services.AddSingleton<ISecretDataRepository, MemorySecretDataRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

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
