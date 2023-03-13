using Xunit;
using System;
using System.IO;
using System.Diagnostics;
using Xunit.Abstractions;
using DeviceManager.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;


namespace test;

public class UnitTest
{
    private readonly ITestOutputHelper output;
    public UnitTest(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public async void Checkpoint02_1()
    {
        // check data folder
        string path = "../../../../../src/DeviceManager/Data";
        bool exists = Directory.Exists(path);
        Assert.True(exists);
        FileInfo fi = new FileInfo($"{path}/AppUser.cs");
        Assert.NotNull(fi);
        Assert.True(fi.Exists);

        AppUser a = new AppUser();
        Assert.NotNull(a);
        Type t = a.GetType();
        Assert.NotNull(t);
        var pn = t.GetProperty("Devices");
        Assert.NotNull(pn);

        Assert.True(typeof(IdentityUser).IsAssignableFrom(typeof(AppUser)));
    }

    [Fact]
    public async void Checkpoint02_2()
    {
        // check data folder
        string path = "../../../../../src/DeviceManager/Data";
        bool exists = Directory.Exists(path);
        Assert.True(exists);
        FileInfo fi = new FileInfo($"{path}/ApplicationDbContext.cs");
        Assert.NotNull(fi);
        Assert.True(fi.Exists);
    }

    [Fact]
    public async void Checkpoint02_3()
    {
        // Arrange
        Type t = typeof(ApplicationDbContext);

        // Act
        var pn = t.GetProperty("Devices");

        // Assert
        Assert.NotNull(pn);
        Assert.Equal(1, t.GetConstructors().Length);
        Assert.True(typeof(IdentityDbContext<AppUser>).IsAssignableFrom(typeof(ApplicationDbContext)));
    }

    [Fact]
    public async void Checkpoint02_4()
    {
        // Arrange
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseInMemoryDatabase("data");

        // Act
        var db = new ApplicationDbContext(optionsBuilder.Options);

        // Assert
        Assert.NotNull(db);
        var devices = await db.Devices.ToListAsync();
        Assert.NotNull(devices);
    }

}