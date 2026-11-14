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
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

       
        modelBuilder.Entity<Enrolment>()
            .HasOne(e => e.Participant)
            .WithMany(u => u.Enrolments)
            .HasForeignKey(e => e.ParticipantId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Enrolment>()
            .HasOne(e => e.Event)
            .WithMany(e => e.Enrolments)
            .HasForeignKey(e => e.EventId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Enrolment>()
            .HasOne(e => e.Category)
            .WithMany(c => c.Enrolments)
            .HasForeignKey(e => e.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

       
        modelBuilder.Entity<Result>()
            .HasOne(r => r.Participant)
            .WithMany(u => u.Results)
            .HasForeignKey(r => r.ParticipantId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Result>()
            .HasOne(r => r.Event)
            .WithMany(e => e.Results)
            .HasForeignKey(r => r.EventId)
            .OnDelete(DeleteBehavior.Restrict);

        
        modelBuilder.Entity<Ticket>(entity =>
        {
            // Unique ticket number per event
            entity.HasIndex(t => t.TicketNumber)
                  .IsUnique();

            // Decimal precision for Price
            entity.Property(t => t.Price)
                  .HasPrecision(18, 2);

            // Constrain string columns
            entity.Property(t => t.TicketType).HasMaxLength(20).IsRequired();
            entity.Property(t => t.PaymentStatus).HasMaxLength(20).IsRequired();
            entity.Property(t => t.TicketNumber).HasMaxLength(50).IsRequired();

            // Ticket → Participant (User)
            entity.HasOne(t => t.Participant)
                  .WithMany()                    
                  .HasForeignKey(t => t.ParticipantId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Ticket → Event
            entity.HasOne(t => t.Event)
                  .WithMany()
                  .HasForeignKey(t => t.EventId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Ticket → Enrolment (1 : 1)
            entity.HasOne(t => t.Enrolment)
                  .WithOne()                     
                  .HasForeignKey<Ticket>(t => t.EnrolmentId)
                  .OnDelete(DeleteBehavior.Cascade);   
        });

       
        modelBuilder.Entity<Payment>(entity =>
        {
           
            entity.HasIndex(p => p.Reference)
                  .IsUnique();

            
            entity.Property(p => p.Amount)
                  .HasPrecision(18, 2);

            entity.Property(p => p.Reference).HasMaxLength(50).IsRequired();
            entity.Property(p => p.Status).HasMaxLength(20).IsRequired();

           
            entity.HasOne(p => p.Ticket)
                  .WithMany(t => t.Payments)
                  .HasForeignKey(p => p.TicketId)
                  .OnDelete(DeleteBehavior.Cascade);   
        });
    }
}