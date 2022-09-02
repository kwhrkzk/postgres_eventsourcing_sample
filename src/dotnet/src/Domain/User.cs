namespace Domain;
public record User
{
    UserId ID { get; init; }
    UserName Name { get; init; }

    private User(UserId id, UserName name)
    {
        ID = id;
        Name = name;
    }

    public static User Create(Guid id, string name)
    => new(
        new UserId{ ID = id },
        new UserName(name)
    );

    public static User? Create(string id, string name)
    => Guid.TryParse(id, out Guid result)
        ? Create(result, name)
        : null;

    public Guid Guid => ID.ID;
    public string GuidString => ID.GuidString;
    public string NameString => Name.NameString;
}
