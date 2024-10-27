using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

internal class Program
{
    internal static IConfiguration Config { get; private set; }

    private static async Task Main(string[] args)
    {
        Config=new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

        var modelInfo = Config.GetSection("KernelInfo");


        var modelId = modelInfo.GetSection("modelId");
        var endpoint = modelInfo.GetSection("endpoint");
        var asyncEndpoint = modelInfo.GetSection("asyncEndpoint");
        var asyncQueryEndpoint = modelInfo.GetSection("asyncQueryEndpoint");
        var apiKey = modelInfo.GetSection("apiKey");

        var builder = Kernel.CreateBuilder().AddAzureOpenAIChatCompletion(modelId.Value,endpoint.Value,apiKey.Value);

        builder.Services.AddLogging(services => services.AddConsole().SetMinimumLevel(LogLevel.Trace));

        Kernel kernel = builder.Build();

        var chatCompletionService=kernel.GetRequiredService<IChatCompletionService>();

        kernel.Plugins.AddFromType<LightsPlugin>("Lights");

        OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new ()
        {
           FunctionChoiceBehavior= FunctionChoiceBehavior.Auto()
        };

        var history = new ChatHistory();

        string? UserInput;

        do
        {
            Console.Write("User >");

            UserInput = Console.ReadLine();

            history.AddUserMessage(UserInput);

            var result = await chatCompletionService.GetChatMessageContentAsync(history, executionSettings: openAIPromptExecutionSettings, kernel: kernel);

            Console.WriteLine("Assistant >" + result);

            history.AddMessage(result.Role, result.Content ?? string.Empty);
        } while (UserInput is not null);
    }


}

