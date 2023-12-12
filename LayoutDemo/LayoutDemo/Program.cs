using LayoutDemo;
using LayoutDemo.Client;
using LayoutDemo.Components;
using LayoutDemo.Components.State;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddAntDesign();
builder.Services.AddScoped<IWeatherService>(sp=>new WeatherApiClient(sp.GetRequiredService<PersistentComponentState>(), ActivatorUtilities.CreateInstance<WeatherService>(sp)));

builder.Services.AddScoped<StateService>();

builder.Services.AddHttpClient("API");

builder.Services.AddTransient(sp =>
{
    var client = sp.GetRequiredService<IHttpClientFactory>().CreateClient("API");
    var httpContext = sp.GetService<IHttpContextAccessor>()?.HttpContext;
    if (httpContext == null)
    {
        client.BaseAddress = new Uri("https://localhost:80");
        return client;
    }

    var request = httpContext.Request;
    var host = request.Host.ToUriComponent();
    var scheme = request.Scheme;
    var baseAddress = $"{scheme}://{host}";
    return client;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();

    app.MapGet("/debug/routes", (IEnumerable<EndpointDataSource> endpointSources) =>
        string.Join("\n", endpointSources.SelectMany(source => source.Endpoints)));
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapWeatherEndpoints();

app.MapWhen(ctx => ctx.Request.Path.StartsWithSegments("/Admin"), second =>
{
    second.UseStaticFiles();
    second.UseStaticFiles("/Admin");

    second.UseRouting();
    second.UseAntiforgery();
    second.UseEndpoints(endpoints =>
    {
        endpoints.MapRazorComponents<App2>()
        .AddInteractiveServerRenderMode()
         .AddInteractiveWebAssemblyRenderMode();
    });
});

app.MapRazorComponents<App>()
    .AddAdditionalAssemblies(typeof(App2).Assembly)
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode();

app.Run();