using Microsoft.AspNetCore.Mvc;

namespace RaceDay.Mvc.Controllers;

/// <summary>
/// Base class for MVC controllers that talk to the RaceDay API.
/// Provides a small helper that turns a failed API call into a friendly TempData error
/// so the UI can display a message instead of crashing.
/// </summary>
public abstract class RaceDayControllerBase : Controller
{
    /// <summary>
    /// Returns the list from an API call, or an empty list plus a friendly TempData["Error"]
    /// message if the call failed (API down, session expired, ...).
    /// </summary>
    protected List<T> OrEmpty<T>((bool Success, List<T>? Data, string? Error) result, string what)
    {
        if (!result.Success)
            TempData["Error"] = $"Could not load {what}. {result.Error}";
        return result.Data ?? new List<T>();
    }
}
