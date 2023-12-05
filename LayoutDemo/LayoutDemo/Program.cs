using LayoutDemo.Client;
using LayoutDemo.Client.Pages;
using LayoutDemo.Components;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Resources;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddAntDesign();

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

app.MapWhen(ctx => ctx.Request.Path.StartsWithSegments("/Admin"), second =>
{
    second.UseStaticFiles();
    second.UseStaticFiles("/Admin");

    second.UseRouting();
    second.UseAntiforgery();
    second.UseEndpoints(endpoints =>
    {
        endpoints.MapRazorComponents<App2>()
         .AddInteractiveWebAssemblyRenderMode();
    });
});

app.MapWhen(ctx => ctx.Request.Path.StartsWithSegments("/Home"), first =>
{

    first.UseStaticFiles();
    first.UseStaticFiles("/Home");
    first.UseRouting();
    first.UseEndpoints(endpoints =>
    {
        endpoints.MapRazorComponents<App>();
    });
});

app.MapRazorComponents<App>().AddInteractiveWebAssemblyRenderMode();

app.Run();

