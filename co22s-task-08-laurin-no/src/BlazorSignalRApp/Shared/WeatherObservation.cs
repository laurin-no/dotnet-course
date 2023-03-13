namespace BlazorSignalRApp.Shared;

public class WeatherObservation: WeatherForecast
{
    public Guid Id { get; set; }
    
    public string? ObservationText { get; set; }
}