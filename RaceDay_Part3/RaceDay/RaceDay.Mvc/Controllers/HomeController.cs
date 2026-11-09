using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace RaceDay.Mvc.Controllers;

/// <summary>
/// Public entry point of the MVC application.
/// Logged-in users are automatically redirected to their role-specific home page.
/// </summary>
public class HomeController : Controller
{
    /// <summary>
    /// Landing page. If a session already exists the user is sent to the
    /// Organiser dashboard or Participant index according to their role.
    /// </summary>
    public IActionResult Index()
    {
        var role = HttpContext.Session.GetString("Role");
        if (role == "Organiser") return RedirectToAction("Dashboard", "Organiser");
        if (role == "Participant") return RedirectToAction("Index", "Participant");
        return View();
    }

    /// <summary>
    /// Generic error page. Caching is disabled so a fresh error view is always shown.
    /// </summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}
