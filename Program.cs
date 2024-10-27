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
using Basement.Parser;
using Basement.TextChunker;
using System.Text;
using System.Text.Encodings.Web;

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


            var services = new ServiceCollection();
            services.AddLogging(builder => builder.AddConsole());
            services.AddHttpClient();

            var servicesProvider = services.BuildServiceProvider();

            var httpClientFactory = servicesProvider.GetRequiredService<IHttpClientFactory>();
            var logger = servicesProvider.GetRequiredService<ILogger<ZhipuAI>>();
            var mainLogger = servicesProvider.GetRequiredService<ILogger<Program>>();

            string samplePath = "C:\\Users\\Jonah\\Desktop\\Sample.txt";

            try
            {
                using (StreamReader reader = new StreamReader(samplePath))
                {
                    string text = reader.ReadToEnd();
                    Console.WriteLine(text);
                }
            }
            catch (Exception e)
            {
                mainLogger.LogWarning($"No file readed becauseof {e.Message}");
            }


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


            var _res=Zhipu.WithDataSchemaAsync(sampleInput, typeof(SampleList), temprature: (float)0.3);
            mainLogger.LogInformation($"Current task id :{_res.id},{_res.task_status}");
            var query = new ZhipuQueryContent(_res.id);

            var asyncResult=Zhipu.ResultQuery(query);
            contents = asyncResult.GetContent();
            foreach (var content in contents)
            {
                Console.WriteLine(content);
            }

            JsonParser jsonParser = new JsonParser();
            var result=jsonParser.ParseTo<SampleList>(contents[0]);
            result.PrintToConsole();


        }
    }
}
