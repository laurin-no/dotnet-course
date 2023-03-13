using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DeviceManager;
using DeviceManager.Data;
using Test.Helpers;
using Xunit;
using Xunit.Abstractions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using AngleSharp.Html.Dom;
using System.Net;
using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;
using Test.Helpers;
using DeviceManager.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;

namespace test;

public class UnitTest
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly WebApplicationFactory<Program> _applicationFactory;
    private readonly HttpClient _client;
    private readonly ApplicationDbContext _db;

    public UnitTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _applicationFactory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    // ... Configure test services
                });

        _client = _applicationFactory.WithWebHostBuilder(builder =>
                    {
                        builder.ConfigureTestServices(services =>
                        {
                            services.AddAuthentication("Test")
                                .AddScheme<AuthenticationSchemeOptions, TestAuthHandlerUser>(
                                    "Test", options => { });
                        });
                    })
                    .CreateClient(new WebApplicationFactoryClientOptions
                    {
                        AllowAutoRedirect = false,
                    });

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Test");

        string connectionstring = "Data Source=app.db";

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlite(connectionstring);

        _db = new ApplicationDbContext(optionsBuilder.Options);
    }

    [Fact]
    public async Task Checkpoint05_01()
    {
        // Arrange
        var userId = "7951bc9f-c769-42b7-a601-9c0a9da6a809";
        var foreignDeviceId = 5;

        //Act
        var response = await _client.GetAsync($"/devices/details/{foreignDeviceId}");

        // Assert
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.StartsWith("/Home/Error",
            response.Headers.Location.OriginalString);
    }

    [Fact]
    public async Task Checkpoint05_02()
    {
        // Arrange
        Random rnd = new Random();
        var ownDeviceId = 4;
        var foreignDeviceId = 5;
        var dbDevice = await _db.Devices.FirstOrDefaultAsync(d => d.Id == foreignDeviceId);

        // Act
        var responseGetDelete = await _client.GetAsync($"/devices/delete/{ownDeviceId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, responseGetDelete.StatusCode);

        // Act 2
        var contentDelete = await HtmlHelpers.GetDocumentAsync(responseGetDelete);
        var form = (IHtmlFormElement)contentDelete.QuerySelector("form");
        form.Action = $"/devices/delete/{foreignDeviceId}";
        var responsePostDelete = await _client.SendAsync(
                form,
                new Dictionary<string, string>());
        var resultPageDelete = await responsePostDelete.Content.ReadAsStringAsync();
        _testOutputHelper.WriteLine(resultPageDelete);
        // Assert 2
        Assert.Equal(HttpStatusCode.Redirect, responsePostDelete.StatusCode);
        Assert.NotNull(await _db.Devices.FirstOrDefaultAsync(d => d.Id == foreignDeviceId));
        Assert.NotNull(await _db.Devices.FirstOrDefaultAsync(d => d.Id == ownDeviceId));
    }
}