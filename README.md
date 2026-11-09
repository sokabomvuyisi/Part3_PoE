# RaceDay MVC – Part 3

## 1. Project Overview

RaceDay MVC is the web-based user interface for the RaceDay event management system. It is developed using ASP.NET Core MVC (.NET 8) and provides separate interfaces for event organisers and race participants.

The MVC application uses the Model-View-Controller (MVC) design pattern:

- **Models** hold the data used by the application and its pages.
- **Views** provide the user interface using Razor (`.cshtml`) pages.
- **Controllers** handle browser requests, validate user input, coordinate application actions and return the appropriate views.
- **ApiService** handles communication with the application's backend service. The MVC project does not connect directly to the database or file storage.

## 2. Main Features

### Account and Authentication
- User registration and login.
- Logout functionality.
- Session-based storage of the signed-in user's ID and role.
- Redirects users to the correct area based on their role.
- Protected organiser and participant pages.

### Organiser Area

Organisers can:
- View an organiser dashboard.
- View and manage their events.
- Create, edit and delete events.
- Upload or replace event banner images.
- Manage event categories.
- View enrolments and update enrolment statuses.
- Record race results, including finishing times and positions.

### Participant Area

Participants can:
- Browse upcoming events.
- Filter events by event type and location.
- View event details and available categories.
- Enrol in an event by selecting a category.
- View their own events and enrolment statuses.
- View their race results.
- View and update their profile.
- Upload a profile picture.

## 3. Technologies Used

| Technology | Purpose |
|---|---|
| C# | Application programming language |
| ASP.NET Core MVC (.NET 8) | Web application framework |
| Razor Views | Dynamic HTML pages |
| Bootstrap 5 | Responsive page layout and components |
| Bootstrap Icons | Interface icons |
| HTML and CSS | Page structure and styling |
| JavaScript | Client-side interface functionality |
| HttpClient | Communication with the backend service |
| ASP.NET Core Session | Maintaining the signed-in user's session |

## 4. Project Structure

The following structure shows the main folders and files in `RaceDay.Mvc`:

```text
RaceDay.Mvc/
├── Controllers/
│   ├── AccountController.cs
│   ├── HomeController.cs
│   ├── OrganiserController.cs
│   ├── ParticipantController.cs
│   └── RaceDayControllerBase.cs
│
├── Helpers/
│   └── NumberExtensions.cs
│
├── Models/
│   └── ViewModels.cs
│
├── Services/
│   └── ApiService.cs
│
├── Views/
│   ├── Account/
│   │   ├── Login.cshtml
│   │   └── Register.cshtml
│   ├── Home/
│   │   ├── Error.cshtml
│   │   └── Index.cshtml
│   ├── Organiser/
│   │   ├── Categories.cshtml
│   │   ├── CreateEvent.cshtml
│   │   ├── Dashboard.cshtml
│   │   ├── EditEvent.cshtml
│   │   ├── Enrolments.cshtml
│   │   ├── Events.cshtml
│   │   └── Results.cshtml
│   ├── Participant/
│   │   ├── EventDetail.cshtml
│   │   ├── Index.cshtml
│   │   ├── MyEvents.cshtml
│   │   ├── MyResults.cshtml
│   │   └── Profile.cshtml
│   ├── Shared/
│   │   ├── _Layout.cshtml
│   │   └── _ValidationScriptsPartial.cshtml
│   ├── _ViewImports.cshtml
│   └── _ViewStart.cshtml
│
├── wwwroot/
│   ├── css/
│   │   └── raceday.css
│   ├── images/
│   │   ├── cycle.jpg
│   │   ├── run.jpg
│   │   └── walk.jpg
│   └── js/
│
├── Program.cs
├── appsettings.json
├── appsettings.Development.json
├── Dockerfile
└── RaceDay.Mvc.csproj
```

## 5. Explanation of Important MVC Components

### Controllers

| Controller | Responsibility |
|---|---|
| `HomeController` | Displays the public landing page and redirects signed-in users to their role-specific home page. |
| `AccountController` | Handles registration, login and logout. |
| `OrganiserController` | Handles organiser dashboard functions, event management, categories, enrolments and results. |
| `ParticipantController` | Handles event browsing, enrolment, participant events, results and profile management. |
| `RaceDayControllerBase` | Provides shared controller functionality, including handling unsuccessful data requests with user-friendly messages. |

The organiser and participant controllers check the user's session and role before allowing access to their pages. This helps prevent users from accessing the wrong role-specific area through the MVC interface.

### Models

`Models/ViewModels.cs` contains view models used to transfer and organise information for the Razor views. View models help the application present the information required by a page without placing page-specific presentation logic inside the views.

### Views

The `Views` folder contains the Razor pages displayed in the browser. Views are separated according to their purpose:

- `Account` contains the login and registration pages.
- `Home` contains the public landing page and error page.
- `Organiser` contains the pages used by event organisers.
- `Participant` contains the pages used by participants.
- `Shared` contains common layout and validation components.

The shared `_Layout.cshtml` file provides the common page structure, navigation, notifications and footer.

### ApiService

`Services/ApiService.cs` centralises HTTP communication with the backend. MVC controllers call this service when they need to retrieve or submit application data. It also handles session-related request information and converts unsuccessful responses into readable error messages.

### Program.cs

`Program.cs` configures the MVC application, including:

- MVC controllers and views.
- Distributed memory cache and session services.
- The configured HTTP client.
- Dependency injection for `ApiService`.
- Static files, routing and the default controller route.

## 6. User Interface Design

The application uses a sporting theme with navy, red and white as its main colours.

The custom stylesheet is located at:

```text
wwwroot/css/raceday.css
```

The interface includes:

- A shared navigation bar.
- Responsive layouts using Bootstrap.
- Event cards and summary cards.
- Styled forms and tables.
- Status badges for enrolments.
- Success and error notifications.
- Sporting images for the event experience.

## 7. Configuration

The MVC application reads the backend service address from the `ApiBaseUrl` setting in `appsettings.json` or `appsettings.Development.json`.

Example:

```json
{
  "ApiBaseUrl": "https://localhost:7203/"
}
```

If the backend is running on a different address or port, update this value to match the address available in your environment. The backend service must be running for features that load or save application data to work.

## 8. Running the MVC Application Locally

### Prerequisites

- .NET 8 SDK.
- Visual Studio 2022 or a compatible code editor.
- The RaceDay backend service running and accessible.
- The correct `ApiBaseUrl` configured for the local environment.

### Steps

1. Open the RaceDay solution in Visual Studio, or open a terminal in the solution root.
2. Confirm that `ApiBaseUrl` in `RaceDay.Mvc/appsettings.Development.json` points to the running backend service.
3. Restore the MVC project's dependencies:

   ```bash
   dotnet restore RaceDay.Mvc/RaceDay.Mvc.csproj
   ```

4. Run the MVC project:

   ```bash
   dotnet run --project RaceDay.Mvc/RaceDay.Mvc.csproj
   ```

5. Open the local HTTPS or HTTP address displayed in the terminal.

The application starts at the default route:

```text
/Home/Index
```

## 9. Running the MVC Application with Docker

A Dockerfile is included in the MVC project. The Docker image is built using the .NET 8 SDK and runs using the ASP.NET 8 runtime image.

Run the following commands from the solution root, where the `RaceDay.Mvc` folder is located:

```bash
docker build -f RaceDay.Mvc/Dockerfile -t raceday-mvc .
```

Start the container:

```bash
docker run -d -p 8080:8080 \
  --add-host=host.docker.internal:host-gateway \
  -e ApiBaseUrl=https://host.docker.internal:7203/ \
  --name raceday-mvc raceday-mvc
```

Open the application at:

```text
http://localhost:8080
```

Change the `ApiBaseUrl` port if the backend service is listening on a different port.

To stop and remove the container:

```bash
docker rm -f raceday-mvc
```

## 10. MVC Request Flow

A typical user action follows this flow:

1. The user opens a page or submits a form in the browser.
2. The relevant MVC controller receives the request.
3. The controller checks the user's session and validates the submitted information where required.
4. The controller calls `ApiService` when application data needs to be retrieved or changed.
5. `ApiService` sends the HTTP request to the configured backend service.
6. The controller processes the response and returns a Razor view or redirects the user.
7. The updated page or a success/error message is displayed in the browser.

This separation keeps the presentation layer responsible for the user experience while backend data operations remain outside the MVC project.

## Video Link

## Screenshot of Docker
<img width="1901" height="775" alt="image" src="https://github.com/user-attachments/assets/b5bb3bee-f9bf-4060-8c90-6922ec5edd2f" />

## CI screenshots
<img width="1862" height="851" alt="image" src="https://github.com/user-attachments/assets/0e56e672-e81c-4550-852c-b9c99cfec44e" />

