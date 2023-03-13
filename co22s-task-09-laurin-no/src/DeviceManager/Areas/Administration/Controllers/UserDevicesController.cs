using DeviceManager.Data;
using DeviceManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeviceManager.Areas.Admin.Controllers;

[Authorize(Roles = "Admins")]
[Area("Administration")]
public class UserDevicesController : Controller
{
    private readonly ApplicationDbContext _context;

    public UserDevicesController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string userId)
    {
        return View(await DevicesForUser(userId));
    }

    public async Task<IActionResult> Edit(string userId, int id)
    {
        var devices = await DevicesForUser(userId);
        var device = devices.FirstOrDefault(d => d.Id == id);

        return View(device);
    }

    [HttpPost]
    public async Task<IActionResult> Edit([Bind("Name, Description, UserId, Id")] DeviceViewModel inputDevice)
    {
        var device = await _context.Devices
            .Where(item => item.Id == inputDevice.Id && item.UserId == inputDevice.UserId).FirstOrDefaultAsync();

        if (device == null)
        {
            return NotFound();
        }

        device.Name = inputDevice.Name;
        device.Description = inputDevice.Description;

        _context.Devices.Update(device);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index), new { userId = device.UserId });
    }

    public IActionResult Create(string userId)
    {
        return View(new DeviceViewModel { UserId = userId });
    }

    [HttpPost]
    public async Task<IActionResult> Create([Bind("Name, Description, UserId")] DeviceViewModel device)
    {
        var id = _context.Devices.Select(item => item.Id).Max() + 1;
        var mappedDevice = new Device
        {
            Id = id,
            UserId = device.UserId,
            Name = device.Name,
            Description = device.Description,
            DateAdded = DateTime.Now
        };

        _context.Add(mappedDevice);

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index), new { userId = device.UserId });
    }

    public async Task<IActionResult> Details(int id, string userId)
    {
        var devices = await DevicesForUser(userId);
        var deviceDetail = devices.FirstOrDefault(item => item.Id == id);

        if (deviceDetail == null)
        {
            return NotFound();
        }

        return View(deviceDetail);
    }

    public async Task<IActionResult> Delete(int id, string userId)
    {
        var devices = await DevicesForUser(userId);
        var deviceDetail = devices.FirstOrDefault(item => item.Id == id);

        if (deviceDetail == null)
        {
            return NotFound();
        }

        return View(deviceDetail);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id, string userId)
    {
        var device = await _context.Devices
            .Where(item => item.Id == id && item.UserId == userId).FirstOrDefaultAsync();

        if (device == null)
        {
            return NotFound();
        }

        _context.Devices.Remove(device);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index), new { userId = device.UserId });
    }

    private async Task<List<DeviceViewModel>> DevicesForUser(string userId)
    {
        var devices = await _context.Devices.Where(item => item.UserId == userId)
            .Select(item => MapToDeviceViewModel(item)).ToListAsync();

        return devices;
    }

    private static DeviceViewModel MapToDeviceViewModel(Device device)
    {
        return new DeviceViewModel
        {
            Id = device.Id,
            UserId = device.UserId,
            Name = device.Name,
            Description = device.Description,
            DateAdded = device.DateAdded
        };
    }
}