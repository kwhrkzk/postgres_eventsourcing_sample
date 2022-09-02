namespace Projector;

public class Commands: ConsoleAppBase
{
    private SampleDBContext Smp { get; }
    private EventstoreContext Ev { get; }

    public Commands(SampleDBContext smp, EventstoreContext ev)
    {
        Smp = smp;
        Ev = ev;
    }

    public async Task Init()
    {
        Smp.Database.EnsureDeleted();
        Smp.Database.EnsureCreated();

        _ = await Smp.Database.ExecuteSqlRawAsync("SELECT 1");

        using var conn = (NpgsqlConnection)Smp.Database.GetDbConnection();
        conn.Open();

        using var cmd = new NpgsqlCommand("""
            CREATE FUNCTION "Users_Update_Timestamp_Function"() RETURNS TRIGGER LANGUAGE PLPGSQL AS $$
            BEGIN
                NEW."modified" := now();
                RETURN NEW;
            END;
            $$;

            CREATE TRIGGER "UpdateTimestamp"
                BEFORE UPDATE
                ON "Users"
                FOR EACH ROW
                    EXECUTE FUNCTION "Users_Update_Timestamp_Function"();
        """, conn);
        await cmd.ExecuteNonQueryAsync();

        conn.Close();

        foreach (var x in Ev.Events)
            Handle(x);

    }

    public async Task Subscription(EventstoreContext ev)
    {
        var conn = (NpgsqlConnection)ev.Database.GetDbConnection();
        conn.Open();

        try
        {
            conn.Notification += OnNotification; 
            using var cmd = new NpgsqlCommand("LISTEN event_channel", conn);
            cmd.ExecuteNonQuery();

            while (Context.CancellationToken.IsCancellationRequested == false)
                await conn.WaitAsync(Context.CancellationToken);
        }
        catch (Exception ex) when (!(ex is OperationCanceledException))
        {
            Console.WriteLine(ex.ToString());
        }
        finally
        {
            conn.Close();
        }
    }

    private void OnNotification(object sender, NpgsqlNotificationEventArgs e)
    {
        Console.WriteLine("event handled: " + e.Payload);
        Event? ev = JsonSerializer.Deserialize<Event>(e.Payload, new JsonSerializerOptions 
        {
            PropertyNameCaseInsensitive = true,
        });

        if (ev is not null)
            Handle(ev);
    }

    private void Handle(Event ev)
    {
        if (Enum.IsDefined(typeof(EventIdEnum), ev.EventId) == false)
            return;

        switch ((EventIdEnum)ev.EventId)
        {
            case EventIdEnum.UserAdded: UserUpsertHandler(ev.Data.RootElement); break;
            case EventIdEnum.UserEdited: UserUpsertHandler(ev.Data.RootElement); break;
            case EventIdEnum.UserDeleted: UserDeleteHandler(ev.Data.RootElement); break;
            default: throw new ArgumentException($"not implemented. {ev.EventId}");
        };
    }

    private void UserDeleteHandler(JsonElement e)
    {
        var id = e.EnumerateObject().First(child => child.Name == "id");

        Projector.DataBase.User? user = Smp.Users.Find(id.Value.GetGuid());
        if (user is not Projector.DataBase.User u)
            return;

        Smp.Users.Remove(u);

        Smp.SaveChanges();
    }

    private void UserUpsertHandler(JsonElement e)
    {
        JsonProperty? idn = e.EnumerateObject().FirstOrDefault(child => child.Name == "id");
        if (idn is not JsonProperty id)
            return;

        var guid = id.Value.GetGuid();
        Projector.DataBase.User? usern = Smp.Users.SingleOrDefault(u => u.Id == guid);

        if (usern is Projector.DataBase.User user)
        {
            UserUpsertHandlerEdit(user, e);
        }
        else
        {
            user = UserUpsertHandlerEdit(new Projector.DataBase.User()
            {
                Id = guid,
            }, e);
            Smp.Users.Add(user);
        }

        Smp.SaveChanges();
    }

    private Projector.DataBase.User UserUpsertHandlerEdit(Projector.DataBase.User user, JsonElement e)
    {
        foreach (JsonProperty child in e.EnumerateObject())
        {
            if (child.Name == "name")
                user.Name = child.Value.GetString() ?? "";
        }

        return user;
    }
}