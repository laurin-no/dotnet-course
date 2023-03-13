using Microsoft.EntityFrameworkCore;
using RazorPhones.Data;
using RazorPhones.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddDbContext<PhonesContext>(options => 
       options.UseSqlite(builder.Configuration.GetConnectionString("PhonesContext")));

var app = builder.Build();

// seed the db
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    SeedData.Initialize(services);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();

// The Program class declaration below is needed for the automated tests. 
// DO NOT remove the following line!!!
public partial class Program { }