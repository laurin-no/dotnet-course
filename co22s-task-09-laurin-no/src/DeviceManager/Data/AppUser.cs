using Microsoft.AspNetCore.Identity;

namespace DeviceManager.Data;

public class AppUser : IdentityUser
{
    public List<Device> Devices { get; set; }
}