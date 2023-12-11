
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace LayoutDemo
{
    public class WeatherService : IWeatherService
    {
        private readonly PersistentComponentState _state;

        public WeatherService(PersistentComponentState state)
        {
            _state = state;
        }

        public async Task<IEnumerable<WeatherForecast>> GetForecasts()
        {
            // Simulate asynchronous loading to demonstrate streaming rendering
            await Task.Delay(500);

            var startDate = DateOnly.FromDateTime(DateTime.Now);
            var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
            var forecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = startDate.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = summaries[Random.Shared.Next(summaries.Length)]
            }).ToArray();

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
