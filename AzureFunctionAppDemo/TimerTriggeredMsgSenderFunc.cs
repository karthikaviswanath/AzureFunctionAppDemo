using System;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureFunctionAppDemo
{
    public class TimerTriggeredMsgSenderFunc
    {
        [FunctionName("TimerTriggeredMsgSenderFunc")]
        public void Run([TimerTrigger("*/10 * * * * *")]TimerInfo myTimer, ILogger log) //Note: */5 * * * * *  an expression to schedule trigger -> means trigger every 5 sec in every min in every hour in every day etc
        {
            var message = $"C# Timer trigger function executed at: {DateTime.Now}";
            //Sending message as an http request via the following api : http://localhost:7171/api/HttpTriggerFunction post method
            HttpClient client = new HttpClient();
            HttpRequestMessage requestMessage = new(HttpMethod.Post, "http://localhost:7171/api/HttpTriggerFunction") ;
            //Here, we are creating as string content version or serialized(json) version of the message
            requestMessage.Content = new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json") ;
            //send this request
            client.Send(requestMessage);
            log.LogInformation("TimerTriggeredMsgSenderFunc execution is complete!!!");
        }
    }
}
