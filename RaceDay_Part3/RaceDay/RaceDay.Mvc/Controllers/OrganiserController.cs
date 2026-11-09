using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Contracts;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers;

/// <summary>
/// MVC controller for all organiser-facing pages:
/// dashboard, event CRUD, category management, enrolment status updates and result entry.
/// Every action first verifies that the current session belongs to an Organiser.
/// </summary>
public class OrganiserController : RaceDayControllerBase
{
    private readonly ApiService _api;

    public OrganiserController(ApiService api) => _api = api;

    /// <summary>Returns true when the current session role is "Organiser".</summary>
    private bool IsOrganiser() => HttpContext.Session.GetString("Role") == "Organiser";

    /// <summary>
    /// Guards every organiser action. Redirects to login if there is no session,
    /// or to the home page if the logged-in user is not an organiser.
    /// </summary>
    private IActionResult? RequireOrganiser()
    {
        if (HttpContext.Session.GetInt32("UserId") == null)
            return RedirectToAction("Login", "Account", new { returnUrl = Request.Path });
        if (!IsOrganiser()) return RedirectToAction("Index", "Home");
        return null;
    }

    // ========== Dashboard ==========

    /// <summary>Shows a summary of the organiser's own events.</summary>
    public async Task<IActionResult> Dashboard()
    {
        var auth = RequireOrganiser(); if (auth != null) return auth;
        var events = OrEmpty(await _api.GetMyEventsAsync(), "your events");
        return View(events);
    }

    // ========== Events ==========

    /// <summary>Lists all events owned by the current organiser.</summary>
    public async Task<IActionResult> Events()
    {
        var auth = RequireOrganiser(); if (auth != null) return auth;
        var events = OrEmpty(await _api.GetMyEventsAsync(), "your events");
        return View(events);
    }

    /// <summary>Shows the form for creating a new event (default date = 14 days from today).</summary>
    [HttpGet]
    public IActionResult CreateEvent()
    {
        var auth = RequireOrganiser(); if (auth != null) return auth;
        return View(new CreateEventRequest { EventDate = DateTime.Today.AddDays(14) });
    }

    /// <summary>
    /// Creates a new event and optionally uploads a banner image.
    /// Banner upload failure does not roll back the event creation; a warning is shown instead.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEvent(CreateEventRequest model, IFormFile? banner)
    {
        var auth = RequireOrganiser(); if (auth != null) return auth;
        if (!ModelState.IsValid) return View(model);

        var (success, data, error) = await _api.CreateEventAsync(model);
        if (!success)
        {
            ModelState.AddModelError("", error ?? "Failed to create event.");
            return View(model);
        }

        TempData["Success"] = "Event created successfully.";

        // Optional banner upload after the event has been created
        if (banner != null && banner.Length > 0 && data != null)
        {
            var (bannerOk, _, bannerError) = await _api.UploadEventBannerAsync(data.EventId, banner);
            if (!bannerOk)
                TempData["Error"] = "The event was created, but the banner could not be uploaded. " + bannerError;
        }

        return RedirectToAction("Events");
    }

    /// <summary>Loads an existing event into the edit form.</summary>
    [HttpGet]
    public async Task<IActionResult> EditEvent(int id)
    {
        var auth = RequireOrganiser(); if (auth != null) return auth;
        var (ok, ev, error) = await _api.GetEventAsync(id);
        if (!ok || ev == null)
        {
            TempData["Error"] = "Could not load the event. " + error;
            return RedirectToAction("Events");
        }
        ViewBag.EventId = id;
        return View(new UpdateEventRequest
        {
            Name = ev.Name,
            Description = ev.Description,
            EventDate = ev.EventDate,
            Location = ev.Location,
            DistanceKm = ev.DistanceKm,
            EventType = ev.EventType
        });
    }

    /// <summary>
    /// Updates event details and optionally replaces the banner image.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditEvent(int id, UpdateEventRequest model, IFormFile? banner)
    {
        var auth = RequireOrganiser(); if (auth != null) return auth;
        ViewBag.EventId = id;
        if (!ModelState.IsValid) return View(model);

        var (success, _, error) = await _api.UpdateEventAsync(id, model);
        if (!success)
        {
            ModelState.AddModelError("", error ?? "Update failed.");
            return View(model);
        }

        TempData["Success"] = "Event updated.";

        if (banner != null && banner.Length > 0)
        {
            var (bannerOk, _, bannerError) = await _api.UploadEventBannerAsync(id, banner);
            if (!bannerOk)
                TempData["Error"] = "The event was updated, but the banner could not be uploaded. " + bannerError;
        }

        return RedirectToAction("Events");
    }

    /// <summary>Deletes an event owned by the current organiser.</summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        var auth = RequireOrganiser(); if (auth != null) return auth;
        var (success, error) = await _api.DeleteEventAsync(id);
        if (success) TempData["Success"] = "Event deleted.";
        else TempData["Error"] = "The event could not be deleted. " + error;
        return RedirectToAction("Events");
    }

    // ========== Categories ==========

    /// <summary>Shows the categories belonging to a given event together with an "add" form.</summary>
    public async Task<IActionResult> Categories(int eventId)
    {
        var auth = RequireOrganiser(); if (auth != null) return auth;
        return View(await BuildCategoriesViewModel(eventId, new CreateCategoryRequest()));
    }

    /// <summary>Builds the view model used by the Categories page.</summary>
    private async Task<CategoriesPageViewModel> BuildCategoriesViewModel(int eventId, CreateCategoryRequest newCategory)
    {
        return new CategoriesPageViewModel
        {
            EventId = eventId,
            Categories = OrEmpty(await _api.GetCategoriesAsync(eventId), "the categories"),
            NewCategory = newCategory
        };
    }

    /// <summary>Adds a new category to the specified event.</summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddCategory(int eventId, [Bind(Prefix = "NewCategory")] CreateCategoryRequest model)
    {
        var auth = RequireOrganiser(); if (auth != null) return auth;

        if (!ModelState.IsValid)
            return View("Categories", await BuildCategoriesViewModel(eventId, model));

        var (success, _, error) = await _api.CreateCategoryAsync(eventId, model);
        if (success) TempData["Success"] = "Category added.";
        else TempData["Error"] = "The category could not be added. " + error;
        return RedirectToAction("Categories", new { eventId });
    }

    /// <summary>Deletes a category. The eventId is only used for the redirect.</summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCategory(int id, int eventId)
    {
        var auth = RequireOrganiser(); if (auth != null) return auth;
        var (success, error) = await _api.DeleteCategoryAsync(id);
        if (success) TempData["Success"] = "Category deleted.";
        else TempData["Error"] = "The category could not be deleted. " + error;
        return RedirectToAction("Categories", new { eventId });
    }

    // ========== Enrolments ==========

    /// <summary>Lists all enrolments for a specific event so the organiser can confirm or cancel them.</summary>
    public async Task<IActionResult> Enrolments(int eventId)
    {
        var auth = RequireOrganiser(); if (auth != null) return auth;
        ViewBag.EventId = eventId;
        var list = OrEmpty(await _api.GetEventEnrolmentsAsync(eventId), "the enrolments");
        return View(list);
    }

    /// <summary>Updates the status of a single enrolment (Pending / Confirmed / Cancelled).</summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateEnrolmentStatus(int id, int eventId, string status)
    {
        var auth = RequireOrganiser(); if (auth != null) return auth;
        var (success, error) = await _api.UpdateEnrolmentStatusAsync(id, status);
        if (success) TempData["Success"] = $"Enrolment marked as {status}.";
        else TempData["Error"] = "The enrolment could not be updated. " + error;
        return RedirectToAction("Enrolments", new { eventId });
    }

    // ========== Results ==========

    /// <summary>
    /// Shows enrolments and existing results for an event so the organiser can enter finish times.
    /// </summary>
    public async Task<IActionResult> Results(int eventId)
    {
        var auth = RequireOrganiser(); if (auth != null) return auth;
        ViewBag.EventId = eventId;
        var enrolments = OrEmpty(await _api.GetEventEnrolmentsAsync(eventId), "the enrolments");
        var results = OrEmpty(await _api.GetEventResultsAsync(eventId), "the results");
        ViewBag.Results = results;
        return View(enrolments);
    }

    /// <summary>Accepted finish-time formats (hours may be 1 or 2 digits, optional fractional seconds).</summary>
    private static readonly string[] FinishTimeFormats =
        { @"h\:mm\:ss", @"hh\:mm\:ss", @"h\:mm\:ss\.FFF", @"hh\:mm\:ss\.FFF" };

    /// <summary>
    /// Records a race result for an enrolment.
    /// Finish time is validated against a small set of common formats before being sent to the API.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddResult(int eventId, int enrolmentId, string? finishTime, int position)
    {
        var auth = RequireOrganiser(); if (auth != null) return auth;

        if (!TimeSpan.TryParseExact((finishTime ?? "").Trim(), FinishTimeFormats, CultureInfo.InvariantCulture, out var ts)
            || ts <= TimeSpan.Zero)
        {
            TempData["Error"] = $"\"{finishTime}\" is not a valid finish time. Use the format hh:mm:ss, for example 00:45:30.";
            return RedirectToAction("Results", new { eventId });
        }

        if (position < 1)
        {
            TempData["Error"] = "Position must be 1 or greater.";
            return RedirectToAction("Results", new { eventId });
        }

        var (success, _, error) = await _api.CreateResultAsync(new CreateResultRequest
        {
            EnrolmentId = enrolmentId,
            FinishTime = ts,
            Position = position
        });

        if (success) TempData["Success"] = "Result saved.";
        else TempData["Error"] = "The result could not be saved. " + error;
        return RedirectToAction("Results", new { eventId });
    }
}
