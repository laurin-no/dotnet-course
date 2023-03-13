using Microsoft.EntityFrameworkCore;
using BlazorSignalRApp.Shared;

namespace BlazorSignalRApp.Server.Data;

public class AppDataContext : DbContext
{
    public AppDataContext(DbContextOptions<AppDataContext> options)
        : base(options)
    {
    }

    public DbSet<ChatMessageNotification> Messages { get; set; } = null!;

    public DbSet<WeatherObservation> WeatherObservations { get; set; } = null!;
}