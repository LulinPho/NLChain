using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Basement.Models.ZhipuAI;
using Basement.DataSchema;

namespace ZhipuAIConsoleApp
{
    class Program
    {

        public static void Main(string[] args)
        {

            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();

            var apiKey = configuration["APIKEY"];
            Console.WriteLine(apiKey);


            var services = new ServiceCollection();
            services.AddLogging(builder => builder.AddConsole());
            services.AddHttpClient();

            var servicesProvider = services.BuildServiceProvider();

            var httpClientFactory = servicesProvider.GetRequiredService<IHttpClientFactory>();
            var logger = servicesProvider.GetRequiredService<ILogger<ZhipuAI>>();
            var mainLogger = servicesProvider.GetRequiredService<ILogger<Program>>();

            var modelName = "glm-4";

            var Zhipu = new ZhipuAI(apiKey, modelName, httpClientFactory.CreateClient(), logger);

            mainLogger.LogInformation("Start Chat Test");


            var line = string.Empty;

            var sampleInput = "There is a list of student." +
                "Jack Quest, ID 00932, from England." +
                "His Brother Rode Quest, ID 00922 and ID 00823, Christine Aoi.";

            var res = Zhipu.WithDataSchema(sampleInput, typeof(SampleList), temprature: (float)0.3);
            var contents = res.GetContent();
            foreach (var content in contents)
            {
                Console.WriteLine(content);
            }

        }
    }
}
