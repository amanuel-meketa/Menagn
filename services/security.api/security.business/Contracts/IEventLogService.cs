using security.sharedUtils.Dtos.Event.Event;
namespace security.business.Contracts
{
    public interface IEventLogService
    {
        Task<IEnumerable<EventLogDto>> GetEventsAsync();
        Task<IEnumerable<AdminEventLogDto>> GetAdminEventsAsync();
        Task DeleteAdminEventsAsync();
    }

}
