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
    public async Task Checkpoint05_1()
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
        var connection = new HubConnectionBuilder()
            .WithUrl(
                _hubUrls.WeatherHubUrl,
                o => o.HttpMessageHandlerFactory = _ => _server.CreateHandler())
            .Build();
        await connection.StartAsync();

        // Act
        var cut = ctx.RenderComponent<Weather>();
        cut.WaitForState(() => cut.FindAll("input").Count.Equals(4));
        var markup = cut.Markup;

        var dateInput = cut.FindComponent<InputDate<System.DateTime>>().Find("input");
        var tempCInput = cut.FindComponent<InputNumber<int>>().Find("input");
        var summaryInput = cut.Find("#summary");
        var observationInput = cut.Find("#observationtext");
        var submitButton = cut.Find("#submit");

        dateInput.Change(observation.Date);
        tempCInput.Change(observation.TemperatureC);
        summaryInput.Change(observation.Summary);
        observationInput.Change(observation.ObservationText);
        await submitButton.ClickAsync(new MouseEventArgs());
        var table = cut.Find("table");

        // read the data from API
        var response = await _client.GetAsync($"/appdata/weatherobservations");

        //_testOutputHelper.WriteLine(_client.BaseAddress.ToString());
        //_testOutputHelper.WriteLine(markup);

        Assert.True(response.IsSuccessStatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains(observation.TemperatureC.ToString(), content);
        Assert.Contains(observation.ObservationText, content);
        Assert.NotNull(table);
        Assert.DoesNotContain(observation.TemperatureC.ToString(), table.InnerHtml);
        Assert.DoesNotContain(observation.Summary, table.InnerHtml);
    }
}