using IsraelFootballFansEvents.DATA.Models;

namespace IsraelFootballFansEvents.DATA.Interfaces
{
    public interface IEventRepository
    {
        List<Event> GetAllEvents();
        Event? GetEventById(int eventId);
        void AddEvent(Event newEvent);
        void UpdateEvent(Event eventToUpdate);
        void DeleteEvent(Event eventToDelete);
    }
}