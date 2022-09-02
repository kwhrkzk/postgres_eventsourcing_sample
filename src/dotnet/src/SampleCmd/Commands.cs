namespace SampleCmd;

public class Commands: ConsoleAppBase
{
    private SampleContext Smp { get; }
    private EventstoreContext Ev { get; }
    private IEventInsert EventInsert { get; }

    public Commands(SampleContext smp, EventstoreContext ev, IEventInsert eventInsert)
    {
        Smp = smp;
        Ev = ev;
        EventInsert = eventInsert;
    }

    [RootCommand]
    public void Exec(
        [Option("e", "event name. add or edit or delete")]string eventname,
        [Option("n", "user name. Required for add or edit")]string? username = null,
        [Option("id", "guid. Required for edit or delete")]string? id = null
    )
    {
        var json = (eventname, username, id) switch
        {
            ("add", null, _) => throw new ArgumentException("user name is Required for add or edit"),
            ("add", string u, _) => UserAdd(u),
            ("edit", null, _) => throw new ArgumentException("user name is Required for add or edit"),
            ("edit", _, null) => throw new ArgumentException("guid is Required for edit or delete"),
            ("edit", string u, string i) => UserEdit(u, i),
            ("delete", _, null) => throw new ArgumentException("guid is Required for edit or delete"),
            ("delete", _, string i) => UserDelete(i),
            _ => throw new ArgumentException("invalid event name"),
        };

        Console.WriteLine(json);
    }

    private string UserAdd(string username)
    {
        var user = Domain.User.Create(Guid.NewGuid(), username);

        EventInsert.Insert(EventIdEnum.UserAdded, JsonSerializer.SerializeToDocument(new {
            id = user.GuidString,
            name = user.NameString,
        }));

        return JsonSerializer.Serialize(new {
            id = user.GuidString,
            name = user.NameString,
        });
    }

    private string UserEdit(string username, string id)
    {
        if (Domain.User.Create(id, username) is not Domain.User user)
            throw new ArgumentException($"no user. {id}");

        SampleCmd.DataBase.User? readUser = Smp.Users.SingleOrDefault(u => u.Id == user.Guid);
        if (readUser is null)
            throw new ArgumentException($"no user. {id}");

        EventInsert.Insert(EventIdEnum.UserEdited, JsonSerializer.SerializeToDocument(new {
            id = user.GuidString,
            name = user.NameString,
        }));

        return JsonSerializer.Serialize(new {
            id = user.GuidString,
            name = user.NameString,
        });
    }

    private string UserDelete(string id)
    {
        if(UserId.Create(id) is not UserId userid)
            throw new ArgumentException($"no user. {id}");

        SampleCmd.DataBase.User? readUser = Smp.Users.SingleOrDefault(u => u.Id == userid.ID);
        if (readUser is null)
            throw new ArgumentException($"no user. {id}");

        EventInsert.Insert(EventIdEnum.UserDeleted, JsonSerializer.SerializeToDocument(new {
            id = userid.GuidString,
        }));

        return JsonSerializer.Serialize(new {
            id = userid.GuidString,
        });
    }
}