using System.Diagnostics;
using System.Threading.Tasks;
using AspNetCorePowerBI.Models;
using AspNetCorePowerBI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AspNetCorePowerBI.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly PowerBiService _service;
    public HomeController(ILogger<HomeController> logger, PowerBiService service)
    {
        _logger = logger;
        _service = service;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _service.GetPowerBiEmbedConfig();

        return View(result);
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}