using LayoutDemo;
using LayoutDemo.Client;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAntDesign();

builder.Services.AddHttpClient("API", options =>
{
    options.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
});

builder.Services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("API"));

builder.Services.AddScoped<IWeatherService>(sp => new WeatherApiClient(sp.GetRequiredService<PersistentComponentState>(), sp.GetRequiredService<HttpClient>()));

CultureInfo.DefaultThreadCurrentUICulture= CultureInfo.GetCultureInfo("zh-CN");
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("zh-CN");

await builder.Build().RunAsync();
