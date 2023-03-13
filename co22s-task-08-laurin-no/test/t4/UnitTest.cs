using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Components.Forms;
using AngleSharp.Dom;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Bunit;
using SignalR_UnitTestingSupportXUnit.Hubs;
using BlazorSignalRApp.Server;
using BlazorSignalRApp.Server.Hubs;
// using BlazorSignalRApp.Client;
using BlazorSignalRApp.Client.Pages;
using BlazorSignalRApp.Client.Models;
using BlazorSignalRApp.Shared;
using BlazorSignalRApp.Server.Data;
using Microsoft.AspNetCore.SignalR.Client;
using Test.Helpers.Infrastructure;
using Microsoft.AspNetCore.TestHost;

namespace test;

public class UnitTest : IClassFixture<WebApplicationFactoryFixture<Program>>
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly HttpClient _client;
    private readonly TestServer _server;

    private readonly HubUrls _hubUrls;

    public UnitTest(ITestOutputHelper testOutputHelper, WebApplicationFactoryFixture<Program> factoryFixture)
    {
        _testOutputHelper = testOutputHelper;
        _client = factoryFixture.CreateDefaultClient();
        _server = factoryFixture.Server;
        _hubUrls = new HubUrls
        {
            ChatHubUrl = $"{factoryFixture.ServerUrl}/chatter",
            WeatherHubUrl = $"{factoryFixture.ServerUrl}/observations"
        };
    }

    [Fact]
    public async Task Checkpoint04()
    {
        // Arrange
        using var ctx = new TestContext();
        ctx.Services.AddSingleton<HubUrls>(_hubUrls);
        System.Random r = new System.Random();
        WeatherObservation observation = new WeatherObservation
        {
            Date = System.DateTime.Now,
            ObservationText = $"observation-text-{r.Next()}",
            TemperatureC = r.Next(),
            Summary = $"summary-{r.Next()}"
        };
        WeatherForecast forecast = new WeatherForecast();
        WeatherForecast forecast2 = new WeatherForecast();

        var connection = new HubConnectionBuilder()
            .WithUrl(
                _hubUrls.WeatherHubUrl,
                o => o.HttpMessageHandlerFactory = _ => _server.CreateHandler())
            .Build();
        connection.On<WeatherForecast>("Forecast", msg =>
        {
            forecast = msg;
        });

        await connection.StartAsync();

        var connection2 = new HubConnectionBuilder()
            .WithUrl(
                _hubUrls.WeatherHubUrl,
                o => o.HttpMessageHandlerFactory = _ => _server.CreateHandler())
            .Build();
        connection2.On<WeatherForecast>("Forecast", msg =>
        {
            forecast2 = msg;
        });

        await connection2.StartAsync();

        // Act
        await connection.InvokeAsync("SubmitObservation", observation);
        var response = await _client.GetAsync($"/appdata/weatherobservations");

        // Assert
        Assert.Null(forecast.Summary);
        Assert.Equal(default(int), forecast.TemperatureC);
        Assert.Equal(default(System.DateTime), forecast.Date);
        Assert.Equal(observation.TemperatureC, forecast2.TemperatureC);
        Assert.Equal(observation.Date, forecast2.Date);
        Assert.True(response.IsSuccessStatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains(observation.ObservationText, content);
        Assert.Contains(observation.Summary, content);
    }
}