using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Net.Http;
using System.Net.Http.Json;

namespace LayoutDemo.Client
{
    public class WeatherApiClient : IWeatherService
    {
        HttpClient _httpClient;
        private readonly PersistentComponentState _state;
        private readonly IWeatherService _service;

        public WeatherApiClient(PersistentComponentState state, IWeatherService service)
        {
            _state = state;
            _service = service;
        }

        public WeatherApiClient(PersistentComponentState state, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _state = state;
        }

        public async Task<IEnumerable<WeatherForecast>?> GetForecasts()
        {
            if (OperatingSystem.IsBrowser())
            {
                if (!_state.TryTakeFromJson<IEnumerable<WeatherForecast>>(nameof(GetForecasts), out var weathers))
                {
                    return weathers;
                }
                else
                {
                    return await _httpClient.GetFromJsonAsync<IEnumerable<WeatherForecast>>("/api/weather");
                }
            }
            else
            {
                if (_service == null)
                    return [];

                var forecasts = await _service.GetForecasts();

                PersistingComponentStateSubscription? subscription = null;

                subscription = _state.RegisterOnPersisting(() =>
                {
                    _state.PersistAsJson(nameof(GetForecasts), forecasts);
                    subscription?.Dispose();
                    return Task.CompletedTask;
                }, RenderMode.InteractiveWebAssembly);

                return forecasts;
            }
        }
    }
}
