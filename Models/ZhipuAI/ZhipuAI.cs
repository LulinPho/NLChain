using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Transactions;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace Basement.Models.ZhipuAI
{
    public class ZhipuAI
    {

        private HttpClient _httpClient;
        private ILogger _logger;
        private string? apikey;
        private const string ZhipuApiEndpoint = "https://open.bigmodel.cn/api/paas/v4/chat/completions";
        private const string ZhipuApiAsyncEndpoint = "https://open.bigmodel.cn/api/paas/v4/async/chat/completions";
        private const string ZhipuApiAsyncQueryEndpoint = "https://open.bigmodel.cn/api/paas/v4/async-result/";
        public string APIKey { set => apikey = value; }

        public string Model { get; set; } = "glm-4";

        public ILogger Logger { get => _logger; set => _logger = value; }



        public ZhipuAI(HttpClient httpClient, ILogger<ZhipuAI> logger)
        {
            _logger = logger;
            _httpClient = httpClient;

            _httpClient.BaseAddress = new Uri(ZhipuApiEndpoint);
        }
        public ZhipuAI(string apiKey, HttpClient httpClient, ILogger<ZhipuAI> logger)
        {
            _logger = logger;

            APIKey = apiKey;

            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(ZhipuApiEndpoint);
            _httpClient.DefaultRequestHeaders.Add("Authorization", apikey);

        }

        public ZhipuAI(string apiKey, string modelName, HttpClient httpClient, ILogger<ZhipuAI> logger)
        {
            _logger = logger;

            APIKey = apiKey;

            Model = modelName;


            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(ZhipuApiEndpoint);
            _httpClient.DefaultRequestHeaders.Add("Authorization", apikey); ;
        }


        public ZhipuResponseContent Post(ZhipuRequestContent content)
        {
            using (var Request = new HttpRequestMessage())
            {
                Request.Method = HttpMethod.Post;

                var jsonContent = new StringContent(content.ToJson(), Encoding.UTF8, "application/json");

                Request.Content = jsonContent;
                var Response = _httpClient.Send(Request);

                if (Response.IsSuccessStatusCode)
                {
                    var _ = Response.Content.ReadAsStringAsync().Result;
                    var resContent = JsonSerializer.Deserialize<ZhipuResponseContent>(_);

                    if (resContent != null)
                        return resContent;
                    else
                        return new ZhipuResponseContent();
                }
                else
                {
                    _logger.LogError("Post request failed with status code:" + Response.StatusCode);
                    return new ZhipuResponseContent();
                }
            }

        }

        public ZhipuAsyncResponseContent PostAsync(ZhipuRequestContent content)
        {

            using (var Request = new HttpRequestMessage())
            {
                Request.Method = HttpMethod.Post;
                Request.RequestUri = new Uri(ZhipuApiAsyncEndpoint);
                var jsonContent = new StringContent(content.ToJson(), Encoding.UTF8, "application/json");

                Request.Content = jsonContent;
                var Response = _httpClient.Send(Request);

                if (Response.IsSuccessStatusCode)
                {
                    var _ = Response.Content.ReadAsStringAsync().Result;
                    var resContent = JsonSerializer.Deserialize<ZhipuAsyncResponseContent>(_);
                    if (resContent != null)
                        return resContent;
                    else
                        return new ZhipuAsyncResponseContent();
                }
                else
                {
                    _logger.LogError("No valid response received");

                    return new ZhipuAsyncResponseContent();
                }
            }

        }

        public List<string> Invoke(ZhipuRequestContent content)
        {
            var resContent = Post(content);

            if (resContent != null && resContent.choices != null)
            {
                List<string> outputs = new List<string>();

                foreach (var item in resContent.choices)
                {
                    outputs.Add(item.message.ToFormatString());
                }
                return outputs;
            }
            else
            {
                return new List<string>() { "No valid reply received." };
            }
        }

        public ZhipuAsyncResultContent ResultQuery(ZhipuQueryContent query)
        {
            for (int queryNum = 0; queryNum < 40; queryNum++)
            {
                using (var request = new HttpRequestMessage())
                {
                    _logger.LogInformation(query.id);

                    request.Method = HttpMethod.Get;
                    request.RequestUri = new Uri(ZhipuApiAsyncQueryEndpoint + query.id);
                    var _jsonContent = new StringContent(query.ToJson(), Encoding.UTF8, "application/json");
                    request.Content = _jsonContent;

                    var _response = _httpClient.Send(request);

                    if (_response.IsSuccessStatusCode)
                    {

                        var _res = _response.Content.ReadAsStringAsync().Result;
                        var _resContent = JsonSerializer.Deserialize<ZhipuAsyncResultContent>(_res);
                        if (_resContent != null && _resContent.task_status == "SUCCESS")
                        {
                            return _resContent;
                        }
                        else
                        {
                            _logger.LogInformation($"Task processing, current getNum {queryNum}");
                            Task.Delay(2000).Wait();
                        }
                    }
                    else
                    {
                        _logger.LogError($"HttpRequest failed. Please check message and try it again. Statuscode: {_response.StatusCode}");

                        return new ZhipuAsyncResultContent();
                    }

                }
            }

            return new ZhipuAsyncResultContent();

        }
    }

    public class ZhipuRequestContent : RequestContent
    {
        [Required]
        public string model { get; set; }
        [Required]
        public List<ZhipuMessage> messages { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? request_id { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? do_sample { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? stream { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public float? temperature { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public float? top_p { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? max_tokens { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? stop { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? user_id { get; set; }

        public ZhipuRequestContent(string _model, ZhipuMessage _messages, float? temperature = null)
        {
            model = _model;
            messages = new List<ZhipuMessage>();
            messages.Add(_messages);


            this.temperature = temperature ?? null;
        }

        public ZhipuRequestContent(string _model, List<ZhipuMessage> _messages, float? temperature = null)
        {
            model = _model;
            messages = _messages;

            this.temperature = temperature ?? null;
        }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }

    }

    public class ZhipuQueryContent
    {
        public string? id;

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }

        public ZhipuQueryContent(string? id)
        {
            this.id= id ?? string.Empty;
        }
    }

    public struct Choice
    {
        public int index { get; set; }
        public string finish_reason { get; set; }
        public AssistantMessage message { get; set; }
    }

    public struct Usage
    {
        public int prompt_tokens { get; set; }
        public int completion_tokens { get; set; }
        public int total_tokens { get; set; }
    }

    public struct Web_search
    {
        public string icon { get; set; }
        public string title { get; set; }
        public string link { get; set; }
        public string media { get; set; }
        public string content { get; set; }
    }

    public class ZhipuResponseContent : ResponseContent
    {
        public string? id { get; set; }
        public long? created { get; set; }

        public List<Choice>? choices { get; set; }
        public Usage? usage { get; set; }
        public Web_search? web_search { get; set; }

        public List<string> GetContent()
        {
            List<string> outputs = new List<string>();

            if (choices != null)
            {
                foreach (var item in choices)
                {
                    outputs.Add(item.message.ToFormatString());
                }
            }

            return outputs;
        }
    }

    public class ZhipuAsyncResponseContent : ResponseContent
    {
        public string? request_id { get; set; }
        public string? id { get; set; }
        public string? model { get; set; }
        public string? task_status { get; set; }
    }

    public class ZhipuAsyncResultContent : ResponseContent
    {
        public string? model { get; set; }
        public List<Choice>? choices { get; set; }
        public string? task_status { get; set; }
        public string? request_id { get; set; }
        public string? id { get; set; }
        public Usage? usage { get; set; }

        public ToolCalls? tool_calls { get; set; } = null;

        public List<string> GetContent()
        {
            List<string> outputs = new List<string>();

            if (choices != null)
            {
                foreach (var item in choices)
                {
                    outputs.Add(item.message.ToFormatString());
                }
            }

            return outputs;
        }
    }


    public static class ZhipuAIExtension
    {
        public static ZhipuResponseContent WithSystemPrompt(this ZhipuAI zhipu, string prompt)
        {
            var systemPrompt = new SystemMessage(prompt);
            return zhipu.Post(new ZhipuRequestContent(zhipu.Model, systemPrompt));

        }
        public static ZhipuResponseContent WithDataSchema(this ZhipuAI zhipu, string text, Type dataSchema, Dictionary<string, string>? examples = null, float? temprature = null)
        {
            List<ZhipuMessage> messages = new List<ZhipuMessage>();

            var systemPrompt = new SystemMessage();
            systemPrompt.content += "You should always follow the instructions and output a valid JSON object." +
                                    "The structure of the JSON object you can found in the instructions, use { \"answer\": $your_answer} as the default structure." +
                                    "Schema and template will be given to demostrate the structure that you should strictly follow." +
                                    "if you are not sure about the structure, fill the structure with \"NOT FOUND\"" +
                                    "And you should always start the block with a \"JSONREPLY```\" and end the block with a \"```\" to indicate the end of the JSON object." +
                                    "It is not allowed to reply any text other than the message replied to according to the template";



            var inputs = new UserMessage();
            inputs.content = "<instruction>" + text + "<instruction/>";

            var schemaPrompt = new SystemMessage();


            var schemaJson = string.Empty;

            schemaPrompt.content += "" +
                "Here is the schema:";
            foreach (var property in dataSchema.GetProperties())
            {
                schemaPrompt.content += "Type:" + property.PropertyType.Name + "\t";
                schemaPrompt.content += "JsonName:" + property.Name + "\n";
                if (Attribute.IsDefined(property, typeof(DescriptionAttribute)))
                {
                    try
                    {

                        schemaPrompt.content += "Description" + property.GetCustomAttribute<DescriptionAttribute>().Description;
                    }
                    catch (Exception e)
                    {
                        zhipu.Logger.LogError($"Cannot get valid description of given property {property.Name}, with exception message: {e.Message}");
                    }
                }
            }

            schemaPrompt.content += "Schema ends";

            zhipu.Logger.LogInformation(Tools.GetDefaultTemplate(dataSchema));

            schemaPrompt.content += "Here is the reply template:\n" + Tools.GetDefaultTemplate(dataSchema) + "Reply template Ends";




            var examplePrompt = new SystemMessage();

            if (examples != null)
            {
                examplePrompt.content += "Here is examples:";
                foreach (var example in examples)
                {

                    examplePrompt.content += $"input:{example.Key}";
                    examplePrompt.content += $"output:{example.Value}";
                }
            }

            messages.Add(systemPrompt);
            messages.Add(schemaPrompt);

            if (examples != null)
                messages.Add(examplePrompt);

            messages.Add(inputs);

            ZhipuRequestContent content = new ZhipuRequestContent(zhipu.Model, messages, temprature);

            return zhipu.Post(content);
        }

        public static ZhipuAsyncResponseContent WithDataSchemaAsync(this ZhipuAI zhipu, string text, Type dataSchema, Dictionary<string, string>? examples = null, float? temprature = null)
        {
            List<ZhipuMessage> messages = new List<ZhipuMessage>();

            var systemPrompt = new SystemMessage();
            systemPrompt.content += "You should always follow the instructions and output a valid JSON object." +
                                    "The structure of the JSON object you can found in the instructions, use { \"answer\": $your_answer} as the default structure." +
                                    "Schema and template will be given to demostrate the structure that you should strictly follow." +
                                    "if you are not sure about the structure, fill the structure with \"NOT FOUND\"" +
                                    "And you should always start the block with a \"JSONREPLY```\" and end the block with a \"```\" to indicate the end of the JSON object." +
                                    "It is not allowed to reply any text other than the message replied to according to the template";



            var inputs = new UserMessage();
            inputs.content = "<instruction>" + text + "<instruction/>";

            var schemaPrompt = new SystemMessage();


            var schemaJson = string.Empty;

            schemaPrompt.content += "" +
                "Here is the schema:";
            foreach (var property in dataSchema.GetProperties())
            {
                schemaPrompt.content += "Type:" + property.PropertyType.Name + "\t";
                schemaPrompt.content += "JsonName:" + property.Name + "\n";
            }

            schemaPrompt.content += "Schema ends";

            zhipu.Logger.LogInformation(Tools.GetDefaultTemplate(dataSchema));

            schemaPrompt.content += "Here is the reply template:\n" + Tools.GetDefaultTemplate(dataSchema) + "Reply template Ends";




            var examplePrompt = new SystemMessage();

            if (examples != null)
            {
                examplePrompt.content += "Here is examples:";
                foreach (var example in examples)
                {

                    examplePrompt.content += $"input:{example.Key}";
                    examplePrompt.content += $"output:{example.Value}";
                }
            }

            messages.Add(systemPrompt);
            messages.Add(schemaPrompt);

            if (examples != null)
                messages.Add(examplePrompt);

            messages.Add(inputs);

            ZhipuRequestContent content = new ZhipuRequestContent(zhipu.Model, messages, temprature);

            return zhipu.PostAsync(content);
        }
    }
}

