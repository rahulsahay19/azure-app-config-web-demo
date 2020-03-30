using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

namespace azure_web_app_demo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
             Host.CreateDefaultBuilder(args)
              .ConfigureWebHostDefaults(webBuilder =>
             webBuilder.ConfigureAppConfiguration((hostingContext, config) =>
             {
                 var settings = config.Build();
                //on local machine
                //config.AddAzureAppConfiguration(settings["ConnectionStrings:AppConfig"]);
                //against managed identity
                string appConfigurationEndpoint = settings["AzureAppConfigurationEndpoint"];
                 config.AddAzureAppConfiguration(options =>
                 {
                    //For local machine
                  //  options.Connect(settings["ConnectionStrings:AppConfig"])
                         //using managed identity
                        options.Connect(new Uri(appConfigurationEndpoint), new ManagedIdentityCredential())
                         .ConfigureRefresh(refresh =>
                         {
                             refresh.Register("TestApp:Settings:Refresh", refreshAll: true);
                             refresh.SetCacheExpiration(TimeSpan.FromSeconds(5));
                         });
                 });
             })
             .UseStartup<Startup>());
    }
}
