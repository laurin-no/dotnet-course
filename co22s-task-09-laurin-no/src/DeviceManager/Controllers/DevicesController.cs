using System.Security.Claims;
using DeviceManager.Data;
using DeviceManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeviceManager.Controllers;

[Authorize]
public class DevicesController : Controller
{
    private readonly ApplicationDbContext _context;

    public DevicesController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return View(await DevicesForUser(userId));
    }

    public async Task<IActionResult> Details(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var devices = await DevicesForUser(userId);
        var deviceDetail = devices.FirstOrDefault(item => item.Id == id);

        if (deviceDetail == null)
        {
            return RedirectToAction("Error", "Home");
        }

        return View(deviceDetail);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create([Bind("Name, Description")] DeviceViewModel device)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


        var id = _context.Devices.Select(item => item.Id).Max() + 1;
        var mappedDevice = new Device
        {
            Id = id,
            UserId = userId,
            Name = device.Name,
            Description = device.Description,
            DateAdded = DateTime.Now
        };

        _context.Add(mappedDevice);

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var devices = await DevicesForUser(userId);
        var device = devices.FirstOrDefault(d => d.Id == id);

        return View(device);
    }

    [HttpPost]
    public async Task<IActionResult> Edit([Bind("Name, Description, Id")] DeviceViewModel inputDevice)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var device = await _context.Devices
            .Where(item => item.Id == inputDevice.Id && item.UserId == userId).FirstOrDefaultAsync();

        if (device == null)
        {
            return RedirectToAction("Error", "Home");
        }

        device.Name = inputDevice.Name;
        device.Description = inputDevice.Description;

        _context.Devices.Update(device);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var devices = await DevicesForUser(userId);
        var deviceDetail = devices.FirstOrDefault(item => item.Id == id);

        if (deviceDetail == null)
        {
            return RedirectToAction("Error", "Home");
        }

        return View(deviceDetail);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed([FromRoute] int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var device = await _context.Devices
            .Where(item => item.Id == id && item.UserId == userId).FirstOrDefaultAsync();

        if (device == null)
        {
            return RedirectToAction("Error", "Home");
        }

        _context.Devices.Remove(device);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
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