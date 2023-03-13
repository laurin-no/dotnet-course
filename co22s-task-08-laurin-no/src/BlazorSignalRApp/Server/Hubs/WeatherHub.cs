using BlazorSignalRApp.Server.Data;
using BlazorSignalRApp.Shared;
using Microsoft.AspNetCore.SignalR;

namespace BlazorSignalRApp.Server.Hubs;

public class WeatherHub : Hub
{
    private readonly AppDataContext _db;

    public WeatherHub(AppDataContext db)
    {
        _db = db;
    }

    public async Task SubmitObservation(WeatherObservation observation)
    {
        _db.WeatherObservations.Add(observation);
        await _db.SaveChangesAsync();

        await Clients.Others.SendAsync("Forecast", observation);
    }
}