using security.sharedUtils.Dtos.Event.Event;
namespace security.business.Contracts
{
    public interface IEventLogService
    {
        Task<IEnumerable<EventLogDto>> GetUserEvents();
        Task<IEnumerable<AdminEventLogDto>> GetAdminEvents();
        Task DeleteAdminEvents();
        Task DeleteUserEvents();
    }

}
