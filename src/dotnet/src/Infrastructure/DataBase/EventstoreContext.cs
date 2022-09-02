using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Infrastructure.DataBase
{
    public partial class EventstoreContext : DbContext
    {
        public EventstoreContext()
        {
        }

        public EventstoreContext(DbContextOptions<EventstoreContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Event> Events { get; set; } = null!;



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("pgcrypto")
                .HasPostgresExtension("uuid-ossp");

            modelBuilder.Entity<Event>(entity =>
            {
                entity.ToTable("events");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("gen_random_uuid()");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.Data)
                    .HasColumnType("jsonb")
                    .HasColumnName("data");

                entity.Property(e => e.EventId).HasColumnName("event_id");

                entity.Property(e => e.Meta)
                    .HasColumnType("jsonb")
                    .HasColumnName("meta")
                    .HasDefaultValueSql("'{}'::jsonb");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
