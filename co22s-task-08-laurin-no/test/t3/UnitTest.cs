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
using Test.Helpers.Infrastructure;
using Microsoft.AspNetCore.TestHost;

namespace test;

public class UnitTest : IClassFixture<WebApplicationFactoryFixture<Program>>
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly HttpClient _client;
    private readonly AppDataContext _db;

    public UnitTest(ITestOutputHelper testOutputHelper, WebApplicationFactoryFixture<Program> factoryFixture)
    {
        _testOutputHelper = testOutputHelper;
        _client = factoryFixture.CreateDefaultClient();
        
        string connectionstring = "db";

        var optionsBuilder = new DbContextOptionsBuilder<AppDataContext>();
        optionsBuilder.UseInMemoryDatabase(connectionstring);

        _db = new AppDataContext(optionsBuilder.Options);
    }

    [Fact]
    public void Checkpoint03_1()
    {
        // Arrange
        System.Random r = new System.Random();
        WeatherObservation observation = new WeatherObservation
        {
            Date = System.DateTime.Now,
            ObservationText = $"observation-text-{r.Next()}",
            TemperatureC = r.Next(),
            Summary = $"summary-{r.Next()}"
        };

        // Act

        // Assert
        Assert.IsAssignableFrom<WeatherForecast>(observation);
    }

    [Fact]
    public async Task Checkpoint03_2()
    {
        // Arrange
        System.Random r = new System.Random();
        WeatherObservation observation = new WeatherObservation
        {
            Date = System.DateTime.Now,
            ObservationText = $"observation-text-{r.Next()}",
            TemperatureC = r.Next(),
            Summary = $"summary-{r.Next()}"
        };

        // Act
        _db.Add(observation);
        var result = await _db.SaveChangesAsync();
        var o = await _db.WeatherObservations.FirstOrDefaultAsync();

        // Assert
        Assert.Equal(1, result);
        Assert.NotNull(o);
        Assert.Equal(observation.ObservationText, o.ObservationText);
    }

    [Fact]
    public async Task Checkpoint03_3()
    {
        // Arrange
        string url = $"/appdata/messages";
        string expectedResult = "[]";

        // Act
        var response = await _client.GetAsync(url);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal(expectedResult, content);
    }
}