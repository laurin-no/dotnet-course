using System.ComponentModel.DataAnnotations;

namespace DeviceManager.Data;

public class Device
{
    [Required] public int Id { get; set; }
    [Required] public string UserId { get; set; }
    [Required] [MaxLength(50)] public string Name { get; set; }
    public string Description { get; set; }
    public DateTime DateAdded { get; set; }
    
    public AppUser AppUser { get; set; }
}