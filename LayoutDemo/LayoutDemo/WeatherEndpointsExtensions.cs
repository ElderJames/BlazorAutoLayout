using AntDesign;
using Microsoft.AspNetCore.Mvc;

namespace LayoutDemo
{
    public static class WeatherEndpointsExtensions
    {
        public static void MapWeatherEndpoints(this IEndpointRouteBuilder routes)
        {
            routes.MapGet("/api/weather", async ([FromServices] IWeatherService service) =>
            {
                return await service.GetForecasts();
            })
                .WithName("GetAllWeather")
                .Produces<IEnumerable<WeatherForecast>>(StatusCodes.Status200OK);
        }
    }
}
