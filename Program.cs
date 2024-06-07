using System.Text;
using System.Text.Json;
using System.Net.Http.Json;

using BricksAIDemo.Models;
using BricksAIDemo.Helpers;
using BricksAIDemo.Plugins;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

var builder = Kernel.CreateBuilder();

builder.Services
    .AddAzureOpenAIChatCompletion(
        Constants.AzureOpenAIDeploymentName,
        Constants.BricksAIEndpoint,
        Constants.BricksAIKey);

builder.Plugins.AddFromType<CityWeatherPlugin>();

Kernel kernel = builder.Build();

var prompt = "Tell me a joke about cats and return today's date in the following format: year/month name/day.";
var result = await kernel.InvokePromptAsync(prompt);

Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine(prompt);
Console.WriteLine("AI assistant replied: ");
Console.WriteLine(result);
Console.WriteLine("Press any key to continue...");
Console.ReadLine();

////////////////////

OpenAIPromptExecutionSettings settings = new()
{
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
};

var city = "Prague";
var weatherPrompt = "Is it raining in Guanajuato or in Rome right now?";

KernelArguments weatherArguments = new(settings) { { "city", city } };
var weatherResult = await kernel.InvokePromptAsync(weatherPrompt, weatherArguments);

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine(weatherPrompt);
Console.WriteLine("AI assistant replied: ");
Console.WriteLine(weatherResult);
Console.WriteLine("Press any key to continue...");
Console.ReadLine();

/////////////////////////

var client = new HttpClient();
client.BaseAddress = new(Constants.BricksAIDashboardEndpoint);
client.DefaultRequestHeaders.Add("Authorization", $"{Constants.BricksAISecretKey}");

var currentTime = DateTimeOffset.Now.ToUnixTimeSeconds();

var start = currentTime - (3600 * 24); // 24 hours
var end = currentTime;
var increment = 300 * 6; // 30 minutes
var keysInfoApi = $"{Constants.BricksAIKeysApi}/{Constants.BricksAIKeyId}?includeMetrics=true&start={start}&end={end}&increment={increment}";

var keysResponse = await client.GetAsync(keysInfoApi);

if (keysResponse.IsSuccessStatusCode)
{
    var keysContent = await keysResponse.Content.ReadAsStringAsync();
    var keysBody = JsonSerializer.Deserialize<BricksAISecretKeysInfo>(keysContent);

    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("*** BricksAI Key Usage Metrics (last 24 hours) ***");
    Console.WriteLine($"Key: {keysBody?.keyId}");

    foreach (var dp in keysBody?.dataPoints)
    {
        Console.WriteLine($"Timestamp: {dp.timeStamp} " +
            $"\tSuccess/Requests: {dp.successCount}/{dp.numberOfRequests} " +
            $"\tCost(USD): {dp.costInUsd:C4} " +
            $"\tLatency(ms): {dp.latencyInMs} " +
            $"\tTokens(in): {dp.promptTokenCount} " +
            $"\tTokens(out): {dp.completionTokenCount} ");
    }

    Console.WriteLine("Press any key to continue...");
    Console.ReadLine();
}

/////////////////////////

var eventInfo = new BricksAIEventsInfo()
{
    start = start,
    end = end,
    keyIds = new() { Constants.BricksAIKeyId },
//    increment = 300
};

var json = JsonSerializer.Serialize(eventInfo);
var content = new StringContent(json, Encoding.UTF8, "application/json");

var eventResponse =
    //await client.PostAsync(Constants.BricksAIEventsApi, content);
    await client.PostAsync(Constants.BricksAIEventsByDayApi, content);

if (eventResponse.IsSuccessStatusCode)
{
    var eventsBody = await eventResponse.Content.ReadFromJsonAsync<BricksAISecretKeysInfo>();

    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine("*** BricksAI Key Usage Metrics (day) ***");

    foreach (var dp in eventsBody.dataPoints)
    {
        Console.WriteLine($"Key: {dp.keyId} \tModel: {dp.model}");

        Console.WriteLine($"Timestamp: {dp.timeStamp} " +
            $"\tRequests: {dp.numberOfRequests} " +
            $"\tSuccess: {dp.successCount} " +
            $"\tCost(USD): {dp.costInUsd:C4} " +
            $"\tLatency(ms): {dp.latencyInMs} " +
            $"\tTokens(in): {dp.promptTokenCount} " +
            $"\tTokens(out): {dp.completionTokenCount} ");
    }

    Console.WriteLine("Press any key to continue...");
    Console.ReadLine();
}