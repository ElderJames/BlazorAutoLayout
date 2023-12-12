using LayoutDemo;
using LayoutDemo.Client;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAntDesign();

builder.Services.AddHttpClient("API", options =>
{
    options.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
});

builder.Services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("API"));

builder.Services.AddScoped<IWeatherService>(sp => new WeatherApiClient(sp.GetRequiredService<PersistentComponentState>(), sp.GetRequiredService<HttpClient>()));

await builder.Build().RunAsync();
