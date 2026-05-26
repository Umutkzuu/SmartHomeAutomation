using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartHome.Data.Context;

namespace SmartHome.Web.Controllers;

public class LogsController : Controller
{
    private readonly SmartHomeDbContext _context;

    public LogsController(SmartHomeDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var logs = await _context.DeviceLogs
            .Include(l => l.Device)
            .OrderByDescending(l => l.Timestamp)
            .ToListAsync();

        return View(logs);
    }
}