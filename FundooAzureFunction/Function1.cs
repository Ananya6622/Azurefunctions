using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using System;

public static class Function1
{
    [FunctionName("Function1")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
        [ServiceBus("reset-password", Connection = "ConnectionString")] IAsyncCollector<string> messages,
        ILogger log)
    {
        log.LogInformation("C# HTTP trigger function processed a request.");

        try
        {
            
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string token = data?.Token;
            string email = data?.Email;

            
            string responseMessage = $"Token: {token}, Email: {email}";

            
            await messages.AddAsync($"Token for reset-password: {token}, Email: {email}");

            return new OkObjectResult(responseMessage);
        }
        catch (Exception ex)
        {
            log.LogError($"Error processing request: {ex.Message}");
            return new BadRequestObjectResult($"Error processing request: {ex.Message}");
        }
    }
}
