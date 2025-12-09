# Roboto SDK

A .NET SDK for consuming the Roboto API with built-in authentication and comprehensive service coverage.

## Features

- ✅ **Full API Coverage**: Authentication, Users, and Calendar Events
- ✅ **Automatic Token Management**: JWT tokens are handled automatically
- ✅ **Dependency Injection**: Native support for ASP.NET Core and .NET applications
- ✅ **Strong Typing**: All DTOs are strongly typed
- ✅ **Exception Handling**: Specific exception types for different error scenarios
- ✅ **Flexible Configuration**: Configure via appsettings.json or code
- ✅ **HttpClient Integration**: Uses IHttpClientFactory for proper HttpClient management

## Installation

Add a reference to the Roboto.Sdk project:

```bash
dotnet add reference path/to/Roboto.Sdk/Roboto.Sdk.csproj
```

Or if it's a NuGet package:

```bash
dotnet add package Roboto.Sdk
```

## Quick Start

### 1. Configuration

Add to your `appsettings.json`:

```json
{
  "RobotoApi": {
    "BaseUrl": "https://localhost:5001",
    "TimeoutSeconds": 30,
    "EnableRetry": true,
    "MaxRetryAttempts": 3
  }
}
```

### 2. Register Services

In your `Program.cs`:

```csharp
using Roboto.Sdk.Extensions;

builder.Services.AddRobotoApiClient(builder.Configuration);
```

### 3. Use the Client

```csharp
using Roboto.Sdk;
using Roboto.Models.Dto;

public class MyService
{
    private readonly RobotoApiClient _client;

    public MyService(RobotoApiClient client)
    {
        _client = client;
    }

    public async Task ExampleUsageAsync()
    {
        // Login
        var loginResponse = await _client.Authentication.LoginAsync(new LoginDto
        {
            UsernameOrEmail = "user@example.com",
            Password = "password123"
        });

        // Create an event
        var newEvent = await _client.CalendarEvents.CreateEventAsync(new CalendarEventCreateDto
        {
            UserId = 1,
            Title = "Team Meeting",
            StartDateTime = DateTime.Now.AddDays(1),
            Duration = 1.5,
            Details = "Weekly sync"
        });
    }
}
```

## Usage Examples

### Console Application

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Roboto.Sdk;
using Roboto.Sdk.Extensions;
using Roboto.Models.Dto;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddRobotoApiClient(context.Configuration);
    })
    .Build();

var client = host.Services.GetRequiredService<RobotoApiClient>();

// Register
var registerResponse = await client.Authentication.RegisterAsync(new RegisterDto
{
    Username = "newuser",
    Email = "newuser@example.com",
    Password = "Password123!",
    FirstName = "John",
    LastName = "Doe"
});

// Login
var loginResponse = await client.Authentication.LoginAsync(new LoginDto
{
    UsernameOrEmail = "newuser@example.com",
    Password = "Password123!"
});

// Get user
var user = await client.Users.GetUserByIdAsync(registerResponse.Id);
Console.WriteLine($"User: {user.FirstName} {user.LastName}");

// Create event
var calendarEvent = await client.CalendarEvents.CreateEventAsync(new CalendarEventCreateDto
{
    UserId = registerResponse.Id,
    Title = "Meeting",
    StartDateTime = DateTime.Now.AddDays(1),
    Duration = 1.0
});

// Get all events
var events = await client.CalendarEvents.GetEventsForUserAsync(registerResponse.Id);
foreach (var evt in events)
{
    Console.WriteLine($"  - {evt.Title}: {evt.StartDateTime:g}");
}
```

### ASP.NET Core Controller

```csharp
[ApiController]
[Route("api/[controller]")]
public class MyController : ControllerBase
{
    private readonly RobotoApiClient _robotoClient;

    public MyController(RobotoApiClient robotoClient)
    {
        _robotoClient = robotoClient;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
        try
        {
            var response = await _robotoClient.Authentication.LoginAsync(request);
            return Ok(response);
        }
        catch (RobotoAuthenticationException ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    [HttpGet("events/{userId}")]
    public async Task<IActionResult> GetEvents(int userId)
    {
        var events = await _robotoClient.CalendarEvents.GetEventsForUserAsync(userId);
        return Ok(events);
    }
}
```

### Blazor Application

```csharp
// Program.cs
builder.Services.AddRobotoApiClient(options =>
{
    options.BaseUrl = "https://localhost:5001";
});

// Component
@inject RobotoApiClient RobotoClient

@code {
    private List<CalendarEventDto> events = new();

    protected override async Task OnInitializedAsync()
    {
        try
        {
            events = (await RobotoClient.CalendarEvents.GetEventsForUserAsync(1)).ToList();
        }
        catch (RobotoApiException ex)
        {
            // Handle error
        }
    }
}
```

## Error Handling

The SDK provides specific exception types:

```csharp
try
{
    var user = await client.Users.GetUserByIdAsync(999);
}
catch (RobotoNotFoundException ex)
{
    // Handle 404 - resource not found
    Console.WriteLine($"Not found: {ex.Message}");
}
catch (RobotoAuthenticationException ex)
{
    // Handle 401 - authentication failed
    Console.WriteLine($"Auth error: {ex.Message}");
}
catch (RobotoValidationException ex)
{
    // Handle 400 - validation error
    Console.WriteLine($"Validation error: {ex.Message}");
}
catch (RobotoApiException ex)
{
    // Handle all other API errors
    Console.WriteLine($"API error ({ex.StatusCode}): {ex.Message}");
    Console.WriteLine($"Response: {ex.ResponseContent}");
}
```

## Authentication

The SDK manages authentication tokens automatically:

```csharp
// Login automatically sets the token
var loginResponse = await client.Authentication.LoginAsync(loginRequest);

// Check if authenticated
if (client.Authentication.IsAuthenticated())
{
    // Make authenticated requests
}

// Manually set token (e.g., from storage)
client.Authentication.SetToken("your-jwt-token");

// Clear token (logout)
client.Authentication.ClearToken();

// Get current token
var token = client.Authentication.GetToken();
```

## API Reference

### Authentication Service

- `LoginAsync(LoginDto)` - Login with credentials
- `RegisterAsync(RegisterDto)` - Register a new user
- `GetToken()` - Get the current JWT token
- `SetToken(string)` - Set the JWT token manually
- `ClearToken()` - Clear the JWT token
- `IsAuthenticated()` - Check if authenticated

### User Service

- `GetUserByIdAsync(int)` - Get user details by ID

### Calendar Event Service

- `CreateEventAsync(CalendarEventCreateDto)` - Create a new event
- `GetEventByIdAsync(int)` - Get event by ID
- `UpdateEventAsync(CalendarEventDto)` - Update an event
- `DeleteEventAsync(int)` - Delete an event
- `GetEventsForUserAsync(int)` - Get all events for a user
- `GetEventsInDateRangeAsync(int, CalendarEventsRangeDto)` - Get events in date range
- `GetCalendarConflictsAsync(int, CalendarEventsRangeDto)` - Get conflicting events

## Configuration Options

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| BaseUrl | string | - | API base URL (required) |
| TimeoutSeconds | int | 30 | Request timeout in seconds |
| EnableRetry | bool | true | Enable automatic retries |
| MaxRetryAttempts | int | 3 | Maximum retry attempts |

## Advanced Configuration

### Programmatic Configuration

```csharp
services.AddRobotoApiClient(options =>
{
    options.BaseUrl = "https://localhost:5001";
    options.TimeoutSeconds = 60;
    options.EnableRetry = true;
    options.MaxRetryAttempts = 5;
});
```

### Per-Service Configuration

```csharp
// Access individual services if needed
services.AddHttpClient<IAuthenticationService, AuthenticationService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:5001");
    client.DefaultRequestHeaders.Add("Custom-Header", "Value");
});
```

## Best Practices

1. **Use Dependency Injection**: Always register the SDK via DI for proper lifecycle management
2. **Handle Exceptions**: Always wrap API calls in try-catch blocks
3. **Token Management**: Let the SDK manage tokens automatically after login
4. **Configuration**: Use appsettings.json for different environments (dev, staging, prod)
5. **HttpClient**: The SDK uses IHttpClientFactory - don't create HttpClient instances manually

## License

MIT
