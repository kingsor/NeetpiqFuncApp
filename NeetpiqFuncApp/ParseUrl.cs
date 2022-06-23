using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
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

        [Function("ParseUrl")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string responseMessage = "This HTTP triggered function executed successfully. Pass a URL in the query string for a personalized response.";

            string url = string.Empty;
            var query = HttpUtility.ParseQueryString(req.Url.Query);
            
            if(query != null && query.Count >= 1)
            {
                url = query["url"];
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            if (!string.IsNullOrEmpty(url))
            {
                SmartReader.Article article = await SmartReader.Reader.ParseArticleAsync(url);
                var options = new JsonSerializerOptions() { WriteIndented = true };
                responseMessage = JsonSerializer.Serialize<SmartReader.Article>(article, options);
                //await response.WriteAsJsonAsync(responseMessage);
            }

            await response.WriteStringAsync(responseMessage);

            return response;
        }
    }
}
