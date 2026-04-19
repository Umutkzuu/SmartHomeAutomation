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
        
        var devices = await _context.Devices.ToListAsync();
        
        
        return View(devices);
    }

    public IActionResult Privacy()
    {
        return View();
    }
}