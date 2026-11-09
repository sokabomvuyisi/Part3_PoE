using Microsoft.AspNetCore.Mvc;
using RaceDay.Contracts;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers;

/// <summary>
/// Handles user registration, login and logout for the MVC front-end.
/// All authentication is delegated to the RaceDay API via ApiService;
/// successful logins store UserId and Role in the MVC session.
/// </summary>
public class AccountController : Controller
{
    private readonly ApiService _api;

    public AccountController(ApiService api) => _api = api;

    /// <summary>Shows the registration form.</summary>
    [HttpGet]
    public IActionResult Register() => View();

    /// <summary>
    /// Processes a registration request.
    /// On success the user is redirected to the login page with a success message.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterRequest model)
    {
        if (!ModelState.IsValid) return View(model);

        var (success, data, error) = await _api.RegisterAsync(model);
        if (success)
        {
            TempData["Success"] = "Registration successful! Please log in.";
            return RedirectToAction("Login");
        }
        ModelState.AddModelError("", error ?? "Registration failed.");
        return View(model);
    }

    /// <summary>
    /// Shows the login form. An optional returnUrl is preserved so the user
    /// can be sent back to the page they originally requested after logging in.
    /// </summary>
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    /// <summary>
    /// Authenticates the user via the API.
    /// On success the session is populated and the user is redirected either
    /// to the original returnUrl or to the role-specific home page.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginRequest model, string? returnUrl = null)
    {
        if (!ModelState.IsValid) return View(model);

        var (success, data, error) = await _api.LoginAsync(model);
        if (success && data != null)
        {
            // Honour a local returnUrl when present
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            // Otherwise send the user to the correct dashboard for their role
            return data.Role == "Organiser"
                ? RedirectToAction("Dashboard", "Organiser")
                : RedirectToAction("Index", "Participant");
        }
        ModelState.AddModelError("", error ?? "Invalid email or password.");
        return View(model);
    }

    /// <summary>
    /// Logs the user out of both the API session and the MVC session,
    /// then returns them to the public home page.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _api.LogoutAsync();
        return RedirectToAction("Index", "Home");
    }
}
