using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartHome.Data.Context;

namespace SmartHome.Web.Controllers;

public class HomeController : Controller
{
    private readonly SmartHomeDbContext _context;

    public HomeController(SmartHomeDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        
        var devices = await _context.Devices.OrderBy(d => d.RoomId).ToListAsync();

        
        ViewBag.TotalDevices = devices.Count;
        ViewBag.ActiveDevices = devices.Count(d => d.IsActive == true);
        ViewBag.OfflineDevices = devices.Count(d => d.IsOnline == false);

        return View(devices);
    }

    [HttpPost]
    public async Task<IActionResult> ToggleDevice(int id)
    {
        var device = await _context.Devices.FindAsync(id);
        
        if (device != null)
        {
            device.IsActive = (device.IsActive == true) ? false : true;
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Privacy()
    {
        return View();
    }
}