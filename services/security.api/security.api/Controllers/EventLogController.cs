using Microsoft.AspNetCore.Mvc;
using security.business.Contracts;
using security.sharedUtils.Dtos.Event.Event;
[ApiController]
[Route("api/eventLog")]
public class EventLogController : ControllerBase
{
    private readonly IEventLogService _eventService;

    public EventLogController(IEventLogService eventService)
    {
        _eventService = eventService;
    }

    [HttpGet("user-events")]
    public async Task<ActionResult<IEnumerable<EventLogDto>>> GetUserEvents()
    {
        var events = await _eventService.GetEventsAsync();
        return Ok(events);
    }

    [HttpGet("admin-events")]
    public async Task<ActionResult<IEnumerable<AdminEventLogDto>>> GetAdminEvents()
    {
        var adminEvents = await _eventService.GetAdminEventsAsync();
        return Ok(adminEvents);
    }
}
