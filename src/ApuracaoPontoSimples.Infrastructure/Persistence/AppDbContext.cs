using ApuracaoPontoSimples.Domain.Entities;
using ApuracaoPontoSimples.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ApuracaoPontoSimples.Infrastructure.Persistence;

public sealed class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Employer> Employers => Set<Employer>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<ScheduleConfig> Schedules => Set<ScheduleConfig>();
    public DbSet<Holiday> Holidays => Set<Holiday>();
    public DbSet<TimeCard> TimeCards => Set<TimeCard>();
    public DbSet<DayEntry> DayEntries => Set<DayEntry>();
    public DbSet<Absence> Absences => Set<Absence>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Employee>()
            .HasOne(e => e.Schedule)
            .WithOne()
            .HasForeignKey<ScheduleConfig>("EmployeeId");

        builder.Entity<TimeCard>()
            .HasMany(t => t.Days)
            .WithOne()
            .HasForeignKey("TimeCardId");

        builder.Entity<DayEntry>()
            .HasOne(d => d.Absence)
            .WithOne()
            .HasForeignKey<Absence>("DayEntryId");

        builder.Entity<DayEntry>()
            .HasOne(d => d.Calculation)
            .WithOne()
            .HasForeignKey<DayCalculation>("DayEntryId");

        builder.Entity<TimeCard>()
            .HasOne(t => t.Totals)
            .WithOne()
            .HasForeignKey<TimeCardTotals>("TimeCardId");

        builder.Entity<DayEntry>().OwnsOne(d => d.Interval1);
        builder.Entity<DayEntry>().OwnsOne(d => d.Interval2);
        builder.Entity<DayEntry>().OwnsOne(d => d.Interval3);
    }
}
