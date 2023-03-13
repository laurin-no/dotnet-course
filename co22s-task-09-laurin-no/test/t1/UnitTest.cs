using Xunit;
using System;
using System.IO;
using System.Diagnostics;
using Xunit.Abstractions;
using DeviceManager.Data;
using System.ComponentModel.DataAnnotations;

namespace test;

public class UnitTest
{
    private readonly ITestOutputHelper output;
    public UnitTest(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public async void Checkpoint01()
    {
        // check data folder
        string path = "../../../../../src/DeviceManager/Data";
        bool exists = Directory.Exists(path);
        Assert.True(exists);
        FileInfo fi = new FileInfo($"{path}/Device.cs");
        Assert.NotNull(fi);
        Assert.True(fi.Exists);

        Device d = new Device();
        Assert.NotNull(d);
        Type t = d.GetType();
        Assert.NotNull(t);
        var pn = t.GetProperty("Name");
        Assert.NotNull(pn);
        Assert.NotNull(t.GetProperty("Id"));
        Assert.NotNull(t.GetProperty("UserId"));
        Assert.NotNull(t.GetProperty("Description"));
        Assert.NotNull(t.GetProperty("DateAdded"));

        Assert.True(Attribute.IsDefined(pn, typeof(RequiredAttribute)));
        Assert.True(Attribute.IsDefined(pn, typeof(MaxLengthAttribute)));
        MaxLengthAttribute attr =
            (MaxLengthAttribute) Attribute.GetCustomAttribute(pn, typeof (MaxLengthAttribute));
        Assert.NotNull(attr);
        Assert.Equal(50, attr.Length);
    }
}