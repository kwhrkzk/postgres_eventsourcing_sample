using System.Text.Json;

namespace Domain;

public interface IEventInsert
{
    public void Insert(EventIdEnum eventId, JsonDocument data, JsonDocument? meta = null);
}