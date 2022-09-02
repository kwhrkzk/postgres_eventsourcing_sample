namespace Projector.DataBase;

public class SampleDBContext: DbContext 
{
    public SampleDBContext(DbContextOptions<SampleDBContext> options): base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .Property(b => b.Created)
            .HasDefaultValueSql("now()");

        modelBuilder.Entity<User>()
            .Property(b => b.Modified)
            .HasDefaultValueSql("now()");

        base.OnModelCreating(modelBuilder);         
    }
}