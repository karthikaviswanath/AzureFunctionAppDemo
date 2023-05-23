# AzureFunctionAppDemo
This is a demo app for Azure functions :
Implemented a time triggered function to generate an http request every 10s. The message in the request body is recieved by an http triggered function and added to a storage queue using queue triggered function.
Also, implemented a demo durable orchestrator function to display a message and add that message to a storage queue.
