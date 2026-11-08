using Microsoft.EntityFrameworkCore;
using RaceDay.API.Models;

namespace RaceDay.API.Data;

public class RaceDayDbContext : DbContext
{
    public RaceDayDbContext(DbContextOptions<RaceDayDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Event> Events => Set<Event>();

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Enrolment> Enrolments => Set<Enrolment>();

    public DbSet<Result> Results => Set<Result>();

    public DbSet<Participant> Participants => Set<Participant>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Participant - Enrolment (One-to-Many)
        modelBuilder.Entity<Enrolment>()
            .HasOne(e => e.Participant)
            .WithMany(p => p.Enrolments)
            .HasForeignKey(e => e.ParticipantId)
            .OnDelete(DeleteBehavior.Restrict);

        // Event - Enrolment (One-to-Many)
        modelBuilder.Entity<Enrolment>()
            .HasOne(e => e.Event)
            .WithMany()
            .HasForeignKey(e => e.EventId)
            .OnDelete(DeleteBehavior.Restrict);

        // Category - Enrolment (One-to-Many)
        modelBuilder.Entity<Enrolment>()
            .HasOne(e => e.Category)
            .WithMany()
            .HasForeignKey(e => e.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Participant - Result (One-to-Many)
        modelBuilder.Entity<Result>()
            .HasOne(r => r.Participant)
            .WithMany(p => p.Results)
            .HasForeignKey(r => r.ParticipantId)
            .OnDelete(DeleteBehavior.Restrict);

        // Event - Result (One-to-Many)
        modelBuilder.Entity<Result>()
            .HasOne(r => r.Event)
            .WithMany()
            .HasForeignKey(r => r.EventId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}