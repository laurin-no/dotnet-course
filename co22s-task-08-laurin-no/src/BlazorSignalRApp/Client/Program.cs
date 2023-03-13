using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorSignalRApp.Client;
using BlazorSignalRApp.Client.Models;

namespace BlazorSignalRApp.Client;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

        // configure the hub urls.
        HubUrls urls = new HubUrls
        {
            ChatHubUrl = "https://localhost:5000/chatter",
            WeatherHubUrl = "https://localhost:5000/observations"
        };
        builder.Services.AddSingleton(urls);

        await builder.Build().RunAsync();
    }
}
