﻿using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using MmoGameFramework.Models;
using Newtonsoft.Json;
using WebConsole.Models;

namespace WebConsole.Controllers;

public class HomeController : Controller
{
    private static readonly HttpClient client = new HttpClient();
    
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
    
    public async Task<IActionResult> Server()
    {
        // Format Request
        var response = await client.GetStringAsync("http://localhost:3000/api/server/status");
        
        // Deserialize Response
        var model = JsonConvert.DeserializeObject<ServerStatusModel>(response);
        
        // Pass model into View
        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}