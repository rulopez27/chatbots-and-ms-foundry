using Roboto.Sdk.Services;

namespace Roboto.Sdk
{
    /// <summary>
    /// Main client for interacting with the Roboto API
    /// </summary>
    public class RobotoApiClient
    {
        public IAuthenticationService Authentication { get; }
        public IUserService Users { get; }
        public ICalendarEventService CalendarEvents { get; }

        public RobotoApiClient(
            IAuthenticationService authenticationService,
            IUserService userService,
            ICalendarEventService calendarEventService)
        {
            Authentication = authenticationService;
            Users = userService;
            CalendarEvents = calendarEventService;
        }
    }
}
