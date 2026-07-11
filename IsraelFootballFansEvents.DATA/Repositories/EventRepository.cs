using IsraelFootballFansEvents.DATA.Interfaces;
using IsraelFootballFansEvents.DATA.Models;
using Microsoft.EntityFrameworkCore;

namespace IsraelFootballFansEvents.DATA.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly EventSystemContext _context;

        public EventRepository(EventSystemContext context)
        {
            _context = context;
        }

        public List<Event> GetAllEvents()
        {
            return _context.Events
                .OrderBy(e => e.StartDate)
                .ToList();
        }

        public Event? GetEventById(int eventId)
        {
            return _context.Events
                .Include(e => e.Sessions)
                .FirstOrDefault(e => e.Id == eventId);
        }

        public void AddEvent(Event newEvent)
        {
            _context.Events.Add(newEvent);
            _context.SaveChanges();
        }

        public void UpdateEvent(Event eventToUpdate)
        {
            _context.Events.Update(eventToUpdate);
            _context.SaveChanges();
        }

        public void DeleteEvent(Event eventToDelete)
        {
            _context.Events.Remove(eventToDelete);
            _context.SaveChanges();
        }
    }
}