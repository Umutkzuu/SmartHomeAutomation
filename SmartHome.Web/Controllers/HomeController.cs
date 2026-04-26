using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartHome.Data.Context;
using System.Data; // Hataları düzelten kritik satır
using MySqlConnector; // MySQL parametreleri için gerekli

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

    [HttpPost]
    public async Task<IActionResult> BulkRoomAction(int roomId, bool status)
    {
        if (status == false)
        {
            await _context.Database.ExecuteSqlRawAsync("CALL sp_TurnOffRoomDevices({0})", roomId);
        }
        else
        {
            await _context.Database.ExecuteSqlRawAsync("CALL sp_TurnOnRoomDevices({0})", roomId);
        }
        
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ActivateNightMode()
    {
        // Entity Framework üzerinden ham bağlantıya ulaşıyoruz
        var connection = _context.Database.GetDbConnection();
        
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = "sp_ActivateNightMode";
            command.CommandType = CommandType.StoredProcedure; 

            
            var outputParam = command.CreateParameter();
            outputParam.ParameterName = "p_StatusMessage";
            outputParam.DbType = DbType.String; 
            outputParam.Direction = ParameterDirection.Output; 
            outputParam.Size = 255;
            
            command.Parameters.Add(outputParam);

            
            await command.ExecuteNonQueryAsync();

            
            TempData["NightModeMessage"] = outputParam.Value?.ToString();
        }
        finally
        {
            
            await connection.CloseAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Privacy()
    {
        return View();
    }
}