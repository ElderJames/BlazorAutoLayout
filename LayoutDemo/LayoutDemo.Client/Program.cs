using LayoutDemo;
using LayoutDemo.Client;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAntDesign();

builder.Services.AddHttpClient("API", options =>
{
    options.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
});

builder.Services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("API"));

builder.Services.AddScoped<IWeatherService, WeatherApiClient>();

await builder.Build().RunAsync();
