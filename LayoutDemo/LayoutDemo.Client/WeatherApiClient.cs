using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace LayoutDemo.Client
{
    public class WeatherApiClient : IWeatherService
    {
        HttpClient _httpClient;
        private readonly PersistentComponentState _state;

        public WeatherApiClient(HttpClient httpClient, PersistentComponentState state)
        {
            _httpClient = httpClient;
            _state = state;
        }

        public async Task<IEnumerable<WeatherForecast>?> GetForecasts()
        {
            if (_state.TryTakeFromJson<IEnumerable<WeatherForecast>>(nameof(GetForecasts), out var weathers))
            {
                return weathers;
            }
            else
            {
                return await _httpClient.GetFromJsonAsync<IEnumerable<WeatherForecast>>("/api/weather");
            }
        }
    }
}
