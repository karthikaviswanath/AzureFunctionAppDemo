using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace AzureFunctionAppDemo
{
    public static class DurableFunction
    {
        [FunctionName("DurableFunction")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = new List<string>();

            //Get input message from context
            var inputMessage = context.GetInput<string>();

            // Note: these are the entity functions invoked by this orchestrator function
            outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello), "Default entity function!!"));
            outputs.Add(await context.CallActivityAsync<string>(nameof(DisplayMessage), inputMessage));
            outputs.Add(await context.CallActivityAsync<string>(nameof(AddMessageToQueue), inputMessage));

            
            return outputs;
        }

        [FunctionName(nameof(SayHello))]
        public static string SayHello([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation("Saying hello to {name}.", name);
            return $"Hello from {name}!";
        }

        [FunctionName(nameof(DisplayMessage))]
        public static string DisplayMessage([ActivityTrigger] string inputMessage, ILogger log)
        {
            log.LogInformation("Message that needs to be added: {inputMessage}", inputMessage);
            return $"DisplayMessage function is complete for : {inputMessage}!";
        }

        [FunctionName(nameof(AddMessageToQueue))]
        public static string AddMessageToQueue([ActivityTrigger] string inputMessage, ILogger log,
             [Queue("durable-queue"), StorageAccount("AzureWebJobsStorage")] ICollector<string> messages
            )
        {
           log.LogInformation("AddMessageToQueue function started for : {inputMessage} ", inputMessage);
            messages.Add(inputMessage);
           return $"Added {inputMessage} to the queue!";
        }

        [FunctionName("DurableFunction_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            //Get request body
            string requestBody = await req.Content.ReadAsStringAsync();
            log.LogInformation("Request Body : {requestBody}", requestBody);

            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("DurableFunction", null,requestBody);

            log.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}