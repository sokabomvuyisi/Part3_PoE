using RaceDay.Contracts;

namespace RaceDay.Mvc.Models;

/// <summary>
/// View model for the organiser's "manage categories" page.
/// Holds the list of existing categories plus a form for creating a new one.
/// </summary>
public class CategoriesPageViewModel
{
    /// <summary>Id of the event whose categories are being managed.</summary>
    public int EventId { get; set; }

    /// <summary>Categories that already exist for the event.</summary>
    public List<CategoryDto> Categories { get; set; } = new();

    /// <summary>Empty request object bound to the "add category" form.</summary>
    public CreateCategoryRequest NewCategory { get; set; } = new();
}

/// <summary>
/// View model for the participant/organiser profile page.
/// Separates the read-only profile data from the editable form values.
/// </summary>
public class ProfilePageViewModel
{
    /// <summary>Current profile information returned by the API.</summary>
    public ProfileDto Profile { get; set; } = new();

    /// <summary>Form values used when the user submits an update.</summary>
    public UpdateProfileRequest Form { get; set; } = new();
}
