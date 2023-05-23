using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AzureFunctionAppDemo
{
    //Message Receiver Function (http triggered) - To add the messages received via an http request to a storage queue (name: "message-queue")
    public static class HttpTriggeredMsgReceiverFunc
    {
        //Note here,only post is mentioned as we are doing only a post operation, .i.e.,
        //whenever an http request is received, the request body (which is a message) will be added to a storage queue
        [FunctionName("HttpTriggerFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, 
            ILogger log,
            [Queue("message-queue"),StorageAccount("AzureWebJobsStorage")] ICollector<string> messages)    //Binding for a queue named "message-queue" with storage account having connection string in
                                                                                                      //key "AzureWebJobsStorage" from local.settings.json;
                                                                                                      //Also, ICollector is used because we are using aync function, else we just have to use string
        {
            log.LogInformation("C# HTTP trigger function processed a request.");


            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //Add message to storage queue
            messages.Add(requestBody);


            return new OkResult();
        }
    }
}
