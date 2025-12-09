# Roboto SDK - Learning Guide

This guide will teach you how to use the Roboto SDK to consume the Roboto.Service API in your applications.

## Table of Contents

1. [Understanding the SDK Architecture](#understanding-the-sdk-architecture)
2. [Setting Up Your First Project](#setting-up-your-first-project)
3. [Authentication Workflow](#authentication-workflow)
4. [Working with Calendar Events](#working-with-calendar-events)
5. [Error Handling Best Practices](#error-handling-best-practices)
6. [Advanced Scenarios](#advanced-scenarios)

---

## Understanding the SDK Architecture

The Roboto SDK follows a **clean architecture** pattern:

```diagram
Roboto.Sdk/
├── Configuration/          # API configuration options
│   └── RobotoApiOptions.cs
├── Models/                 # DTOs for requests/responses
│   ├── AuthModels.cs
│   ├── CalendarEventModels.cs
│   └── UserModels.cs
├── Exceptions/            # Custom exception types
│   └── RobotoApiException.cs
├── Services/              # Service interfaces and implementations
│   ├── IAuthenticationService.cs
│   ├── AuthenticationService.cs
│   ├── IUserService.cs
│   ├── UserService.cs
│   ├── ICalendarEventService.cs
│   └── CalendarEventService.cs
├── Extensions/            # Dependency injection extensions
│   └── ServiceCollectionExtensions.cs
└── RobotoApiClient.cs     # Main client (facade pattern)
```

### Key Concepts

1. **RobotoApiClient**: The main entry point that provides access to all services
2. **Services**: Separate services for different API areas (Auth, Users, Calendar)
3. **HttpClient**: Uses IHttpClientFactory for proper connection management
4. **Dependency Injection**: Fully integrated with .NET's DI container

---

## Setting Up Your First Project

### Step 1: Create a Console Application

```bash
dotnet new console -n MyRobotoApp
cd MyRobotoApp
```

### Step 2: Add Required Packages

```bash
dotnet add package Microsoft.Extensions.Hosting
dotnet add package Microsoft.Extensions.Configuration.Json
dotnet add reference ../Roboto.Sdk/Roboto.Sdk.csproj
```

### Step 3: Create appsettings.json

```json
{
  "RobotoApi": {
    "BaseUrl": "https://localhost:5001",
    "TimeoutSeconds": 30
  }
}
```

Make sure it's copied to output:

```xml
<!-- Add to .csproj -->
<ItemGroup>
  <None Update="appsettings.json">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </None>
</ItemGroup>
```

### Step 4: Set Up Dependency Injection

```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Roboto.Sdk;
using Roboto.Sdk.Extensions;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false);
    })
    .ConfigureServices((context, services) =>
    {
        // Register the Roboto SDK
        services.AddRobotoApiClient(context.Configuration);
    })
    .Build();

// Get the client from DI
var client = host.Services.GetRequiredService<RobotoApiClient>();

Console.WriteLine("Roboto SDK is ready to use!");
```

---

## Authentication Workflow

### Understanding JWT Authentication

The Roboto API uses JWT (JSON Web Tokens) for authentication:

1. User logs in with credentials
2. API returns a JWT token
3. SDK stores the token automatically
4. Token is sent with every subsequent request

### Example: Register and Login

```csharp
using Roboto.Sdk;
using Roboto.Models.Dto;
using Roboto.Sdk.Exceptions;

public class AuthenticationExample
{
    private readonly RobotoApiClient _client;

    public AuthenticationExample(RobotoApiClient client)
    {
        _client = client;
    }

    public async Task RunAsync()
    {
        // Step 1: Register a new user
        Console.WriteLine("Registering new user...");
        
        var registerRequest = new RegisterDto
        {
            Username = "johndoe",
            Email = "john@example.com",
            Password = "SecurePassword123!",
            FirstName = "John",
            LastName = "Doe"
        };

        try
        {
            var registerResponse = await _client.Authentication.RegisterAsync(registerRequest);
            Console.WriteLine($"✓ Registered user ID: {registerResponse.Id}");
        }
        catch (RobotoValidationException ex)
        {
            Console.WriteLine($"✗ Registration failed: {ex.Message}");
            // User might already exist
            return;
        }

        // Step 2: Login
        Console.WriteLine("\nLogging in...");
        
        var loginRequest = new LoginDto
        {
            UsernameOrEmail = "john@example.com",  // Can use email or username
            Password = "SecurePassword123!"
        };

        try
        {
            var loginResponse = await _client.Authentication.LoginAsync(loginRequest);
            Console.WriteLine($"✓ Login successful!");
            Console.WriteLine($"  Token: {loginResponse.Token[..20]}...");
            
            // The token is automatically stored and will be used for future requests
        }
        catch (RobotoAuthenticationException ex)
        {
            Console.WriteLine($"✗ Login failed: {ex.Message}");
            return;
        }

        // Step 3: Check authentication status
        if (_client.Authentication.IsAuthenticated())
        {
            Console.WriteLine("\n✓ Currently authenticated");
            Console.WriteLine($"  Token: {_client.Authentication.GetToken()}");
        }
    }
}
```

### Token Management

```csharp
// Automatically set after login
var loginResponse = await client.Authentication.LoginAsync(loginRequest);
// Token is now stored

// Manually set token (e.g., from secure storage)
client.Authentication.SetToken("eyJhbGciOiJIUzI1...");

// Check if authenticated
if (client.Authentication.IsAuthenticated())
{
    // Make authenticated requests
}

// Get current token (e.g., to save to storage)
var token = client.Authentication.GetToken();

// Logout (clear token)
client.Authentication.ClearToken();
```

---

## Working with Calendar Events

### Create an Event

```csharp
var newEvent = await client.CalendarEvents.CreateEventAsync(new CalendarEventCreateDto
{
    UserId = 1,
    Title = "Team Meeting",
    StartDateTime = DateTime.Now.AddDays(1).Date.AddHours(9),  // Tomorrow at 9 AM
    Duration = 1.5,  // 1.5 hours
    IsAllDay = false,
    BlockCalendar = true,
    Details = "Weekly team sync meeting"
});

Console.WriteLine($"Created event: {newEvent.Title} (ID: {newEvent.Id})");
```

### Get Events for a User

```csharp
var events = await client.CalendarEvents.GetEventsForUserAsync(userId: 1);

Console.WriteLine($"Found {events.Count()} events:");
foreach (var evt in events)
{
    Console.WriteLine($"  - {evt.Title}");
    Console.WriteLine($"    Start: {evt.StartDateTime:g}");
    Console.WriteLine($"    Duration: {evt.Duration} hours");
}
```

### Update an Event

```csharp
// Get the event first
var existingEvent = await client.CalendarEvents.GetEventByIdAsync(eventId: 1);

// Modify properties
existingEvent.Title = "Updated: Team Meeting";
existingEvent.Duration = 2.0;  // Extended to 2 hours
existingEvent.Details = "Updated meeting details";

// Save changes
var updatedEvent = await client.CalendarEvents.UpdateEventAsync(existingEvent);
Console.WriteLine($"Updated event: {updatedEvent.Title}");
```

### Delete an Event

```csharp
await client.CalendarEvents.DeleteEventAsync(eventId: 1);
Console.WriteLine("Event deleted successfully");
```

### Check for Conflicts

```csharp
var dateRange = new CalendarEventsRangeDto
{
    StartDate = DateTime.Today,
    EndDate = DateTime.Today.AddDays(7)
};

var conflicts = await client.CalendarEvents.GetCalendarConflictsAsync(
    userId: 1,
    dateRange: dateRange
);

if (conflicts.Any())
{
    Console.WriteLine($"Found {conflicts.Count()} conflicting events:");
    foreach (var conflict in conflicts)
    {
        Console.WriteLine($"  - {conflict.Title}: {conflict.StartDateTime:g}");
    }
}
else
{
    Console.WriteLine("No conflicts found");
}
```

### Get Events in Date Range

```csharp
var range = new CalendarEventsRangeDto
{
    StartDate = new DateTime(2025, 12, 1),
    EndDate = new DateTime(2025, 12, 31)
};

var decemberEvents = await client.CalendarEvents.GetEventsInDateRangeAsync(
    userId: 1,
    dateRange: range
);

Console.WriteLine($"Events in December: {decemberEvents.Count()}");
```

---

## Error Handling Best Practices

### Exception Hierarchy

```diagram
Exception
└── RobotoApiException (Base for all API errors)
    ├── RobotoAuthenticationException (401 Unauthorized)
    ├── RobotoNotFoundException (404 Not Found)
    └── RobotoValidationException (400 Bad Request)
```

### Comprehensive Error Handling

```csharp
try
{
    // Attempt to create an event
    var newEvent = await client.CalendarEvents.CreateEventAsync(eventDto);
    Console.WriteLine("✓ Event created successfully");
}
catch (RobotoAuthenticationException ex)
{
    Console.WriteLine("✗ Authentication error");
    Console.WriteLine($"  Message: {ex.Message}");
    Console.WriteLine("  Please login again");
    
    // Redirect to login
}
catch (RobotoValidationException ex)
{
    Console.WriteLine("✗ Validation error");
    Console.WriteLine($"  Message: {ex.Message}");
    Console.WriteLine($"  Response: {ex.ResponseContent}");
    
    // Show validation errors to user
}
catch (RobotoNotFoundException ex)
{
    Console.WriteLine("✗ Resource not found");
    Console.WriteLine($"  Message: {ex.Message}");
    
    // Resource doesn't exist
}
catch (RobotoApiException ex)
{
    Console.WriteLine($"✗ API error ({ex.StatusCode})");
    Console.WriteLine($"  Message: {ex.Message}");
    Console.WriteLine($"  Response: {ex.ResponseContent}");
    
    // General API error
}
catch (HttpRequestException ex)
{
    Console.WriteLine("✗ Network error");
    Console.WriteLine($"  Message: {ex.Message}");
    
    // Network connectivity issue
}
catch (Exception ex)
{
    Console.WriteLine("✗ Unexpected error");
    Console.WriteLine($"  Message: {ex.Message}");
    
    // Log and report
}
```

---

## Advanced Scenarios

### Scenario 1: ASP.NET Core Web API

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddRobotoApiClient(builder.Configuration);

var app = builder.Build();
app.MapControllers();
app.Run();

// Controllers/EventsController.cs
[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly RobotoApiClient _robotoClient;

    public EventsController(RobotoApiClient robotoClient)
    {
        _robotoClient = robotoClient;
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<IEnumerable<CalendarEventDto>>> GetUserEvents(int userId)
    {
        try
        {
            var events = await _robotoClient.CalendarEvents.GetEventsForUserAsync(userId);
            return Ok(events);
        }
        catch (RobotoNotFoundException)
        {
            return NotFound($"User {userId} not found");
        }
        catch (RobotoApiException ex)
        {
            return StatusCode((int)ex.StatusCode, ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<CalendarEventDto>> CreateEvent([FromBody] CalendarEventCreateDto dto)
    {
        try
        {
            var created = await _robotoClient.CalendarEvents.CreateEventAsync(dto);
            return CreatedAtAction(nameof(GetUserEvents), new { userId = dto.UserId }, created);
        }
        catch (RobotoValidationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
```

### Scenario 2: Background Service

```csharp
public class CalendarSyncService : BackgroundService
{
    private readonly RobotoApiClient _client;
    private readonly ILogger<CalendarSyncService> _logger;

    public CalendarSyncService(RobotoApiClient client, ILogger<CalendarSyncService> logger)
    {
        _client = client;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Login once at startup
        await _client.Authentication.LoginAsync(new LoginDto
        {
            UsernameOrEmail = "service@example.com",
            Password = Environment.GetEnvironmentVariable("SERVICE_PASSWORD")
        });

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Syncing calendar...");

                var events = await _client.CalendarEvents.GetEventsForUserAsync(1);
                
                // Process events
                foreach (var evt in events)
                {
                    // Sync to external calendar
                }

                _logger.LogInformation("Sync complete");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during sync");
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
```

### Scenario 3: Blazor WebAssembly

```csharp
// Program.cs
builder.Services.AddRobotoApiClient(options =>
{
    options.BaseUrl = builder.Configuration["RobotoApi:BaseUrl"] ?? "https://localhost:5001";
});

// Pages/Calendar.razor
@page "/calendar"
@inject RobotoApiClient Client
@inject NavigationManager Nav

<h3>My Calendar</h3>

@if (events == null)
{
    <p>Loading...</p>
}
else if (!events.Any())
{
    <p>No events found</p>
}
else
{
    <table class="table">
        <thead>
            <tr>
                <th>Title</th>
                <th>Start</th>
                <th>Duration</th>
            </tr>
        </thead>
        <tbody>
            @foreach (var evt in events)
            {
                <tr>
                    <td>@evt.Title</td>
                    <td>@evt.StartDateTime.ToString("g")</td>
                    <td>@evt.Duration hours</td>
                </tr>
            }
        </tbody>
    </table>
}

@code {
    private List<CalendarEventDto> events;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            events = (await Client.CalendarEvents.GetEventsForUserAsync(1)).ToList();
        }
        catch (RobotoAuthenticationException)
        {
            Nav.NavigateTo("/login");
        }
        catch (Exception ex)
        {
            // Show error message
        }
    }
}
```

### Scenario 4: Token Persistence

```csharp
public class TokenStorageService
{
    private const string TOKEN_KEY = "roboto_auth_token";
    private readonly RobotoApiClient _client;
    private readonly ILocalStorageService _localStorage;

    public TokenStorageService(RobotoApiClient client, ILocalStorageService localStorage)
    {
        _client = client;
        _localStorage = localStorage;
    }

    public async Task LoginAndStoreAsync(LoginDto request)
    {
        var response = await _client.Authentication.LoginAsync(request);
        await _localStorage.SetItemAsync(TOKEN_KEY, response.Token);
    }

    public async Task RestoreTokenAsync()
    {
        var token = await _localStorage.GetItemAsync<string>(TOKEN_KEY);
        if (!string.IsNullOrEmpty(token))
        {
            _client.Authentication.SetToken(token);
        }
    }

    public async Task LogoutAsync()
    {
        _client.Authentication.ClearToken();
        await _localStorage.RemoveItemAsync(TOKEN_KEY);
    }
}
```

---

## Summary

You've learned:

1. ✅ How to set up and configure the Roboto SDK
2. ✅ Authentication workflow and token management
3. ✅ Working with calendar events (CRUD operations)
4. ✅ Proper error handling techniques
5. ✅ Advanced integration scenarios (Web API, Blazor, Background Services)

## Next Steps

1. Explore the API documentation for more endpoints
2. Implement retry policies for resilience
3. Add logging and monitoring
4. Create integration tests
5. Build your own application using the SDK!

Happy coding! 🚀
