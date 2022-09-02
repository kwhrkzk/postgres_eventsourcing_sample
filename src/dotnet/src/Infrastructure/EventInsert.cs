using Infrastructure.DataBase;
using System.Text.Json;
using Domain;

namespace Infrastructure;

public class EventInsert: IEventInsert
{
    private EventstoreContext Ev { get; }
    public EventInsert(EventstoreContext ev) => Ev = ev;

    public void Insert(EventIdEnum eventId, JsonDocument data, JsonDocument? meta = null)
    {
        var ev = new Event()
        {
            EventId = (int)eventId,
            Data = data,
            Meta = meta,
        };

        Ev.Add(ev);

        Ev.SaveChanges();
    }
}
