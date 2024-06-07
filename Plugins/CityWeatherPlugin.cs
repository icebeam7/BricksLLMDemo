using System.ComponentModel;
using BricksAIDemo.Helpers;
using Microsoft.SemanticKernel;

namespace BricksAIDemo.Plugins
{
    // Native function: native code with custom logic
    public class CityWeatherPlugin
    {
        HttpClient client = new HttpClient()
        {
            BaseAddress = new(Constants.OpenWeatherMapEndpoint)
        };

        [KernelFunction]
        [Description("Describes the current weather of a city in JSON format")]
        public async Task<string> GetWeather(
            [Description("The name of the city ")] string city)
        {
            var query = $"{Constants.OpenWeatherMapCurrentApi}?q={city}&appid={Constants.OpenWeatherMapKey}";

            var result = await client.GetAsync(query);

            if (result.IsSuccessStatusCode)
                return await result.Content.ReadAsStringAsync();

            return "Error, the weather of the city is not available at this moment";
        }
    }
}
