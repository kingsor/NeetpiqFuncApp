using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Web;

namespace NeetpiqFuncApp
{
    public class ParseUrl
    {
        private readonly ILogger _logger;

        public ParseUrl(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ParseUrl>();
        }

        // public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "smartreader/{url:alpha}")] HttpRequestData req, string url)
        [Function("ParseUrl")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "SmartReader/ParseUrl")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string responseMessage = "This HTTP triggered function executed successfully. Pass a URL in the query string for a personalized response.";

            string urlParam = string.Empty;
            var query = HttpUtility.ParseQueryString(req.Url.Query);
            
            if(query != null && query.Count >= 1)
            {
                urlParam = query["url"] ?? string.Empty;
                _logger.LogInformation($"Parameter [url]=[{urlParam}]");
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            if (!string.IsNullOrEmpty(urlParam))
            {
                SmartReader.Article article = await SmartReader.Reader.ParseArticleAsync(urlParam);
                var options = new JsonSerializerOptions()
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = true
                };
                responseMessage = JsonSerializer.Serialize<SmartReader.Article>(article, options);
            }

            await response.WriteStringAsync(responseMessage);

            return response;
        }
    }
}
