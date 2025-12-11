using Microsoft.AspNetCore.Authorization;
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
        try
        {
            var events = await _eventService.GetUserEvents();
            return Ok(events);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while retrieving users event log. {ex.Message}");
        }
    }

    [HttpDelete("user-events")]
    public async Task<ActionResult> DeleteUserEvents()
    {
        try
        {
            await _eventService.DeleteUserEvents();
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while deleting users event log. {ex.Message}");
        }
      
    }

    [HttpGet("admin-events")]
    public async Task<ActionResult<IEnumerable<AdminEventLogDto>>> GetAdminEvents()
    {
        try
        {
            var adminEvents = await _eventService.GetAdminEvents();
            return Ok(adminEvents);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while retrieving admin event log. {ex.Message}");
        }
    }
    
    [HttpDelete("admin-events")]
    public async Task<ActionResult> DeleteAdminEventsAsync()
    {
        try
        {
            await _eventService.DeleteAdminEvents();
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while deleting admin event log. {ex.Message}");
        }        
    }
}
