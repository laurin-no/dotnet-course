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
    public async Task Checkpoint02()
    {
        // Arrange
        using var ctx = new TestContext();
        ctx.Services.AddSingleton<HubUrls>(_hubUrls);

        System.Random r = new System.Random();
        var auser = $"user-{r.Next()}";
        var amessage = $"message-{r.Next()}";

        var connection = new HubConnectionBuilder()
            .WithUrl(
                _hubUrls.ChatHubUrl,
                o => o.HttpMessageHandlerFactory = _ => _server.CreateHandler())
            .Build();

        await connection.StartAsync();

        // Act
        var cut = ctx.RenderComponent<Chat>();
        cut.WaitForState(() => cut.FindAll("input").Count.Equals(2));
        var markup = cut.Markup;
        var userInput = cut.Find("#username");
        var messageInput = cut.Find("#themessage");
        var submitButton = cut.Find("#submitthemessage");
        
        //_testOutputHelper.WriteLine(_client.BaseAddress.ToString());
        //_testOutputHelper.WriteLine(markup);

        // Assert
        Assert.NotNull(userInput);
        Assert.NotNull(messageInput);
        Assert.NotNull(submitButton);
        
        // Act 2
        userInput.Change(auser);
        messageInput.Change(amessage);
        await submitButton.ClickAsync(new MouseEventArgs());

        var div = cut.Find("#messagelist");

        // read the data from API
        var response = await _client.GetAsync($"/appdata/messages");


        // Assert 2
        Assert.Contains(auser, div.InnerHtml);
        Assert.Contains(amessage, div.InnerHtml);
        Assert.True(response.IsSuccessStatusCode);
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains(auser, content);
        Assert.Contains(amessage, content);
    }
}