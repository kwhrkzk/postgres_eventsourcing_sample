using System;

namespace Domain;
public record UserId
{
    public Guid ID { get; init; }
    public string GuidString => ID.ToString();

    public static UserId? Create(string? id)
    => Guid.TryParse(id, out Guid result)
            ? new UserId { ID = result }
            : null;
}
