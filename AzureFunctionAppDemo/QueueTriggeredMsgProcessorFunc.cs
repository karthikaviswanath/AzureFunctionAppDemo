using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace AzureFunctionAppDemo
{
    //Message Processor Function (Queue Triggered) - To automatically monitor the queue (name : "message-queue")
    //and process the messages like sending an email based on that or saving it in a data store and in this case, just logging the message
    public class QueueTriggeredMsgProcessorFunc
    {
        [FunctionName("QueueTriggerMsgProcessorFunc")]
        public void Run([QueueTrigger("message-queue", Connection = "AzureWebJobsStorage")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
