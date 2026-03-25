using Microsoft.EntityFrameworkCore;

namespace Employee.Api.Model;

public class EmployeeDbContext : DbContext
{
    public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options)
        : base(options) { }

    public DbSet<EmployeeModel> Employees { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Designation> Designations { get; set; }
    public DbSet<SalaryModel> Salaries { get; set; }
    public DbSet<TaskModel> Tasks { get; set; }
    public DbSet<AnnouncementModel> Announcements { get; set; }
    public DbSet<LeaveModel> Leaves { get; set; }
    public DbSet<AttendanceModel> Attendances { get; set; }
    public DbSet<MessageModel> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EmployeeModel>().ToTable("employeeTbl");
        modelBuilder.Entity<Department>().ToTable("departmentTbl");
        modelBuilder.Entity<Designation>().ToTable("designationTbl");

        modelBuilder.Entity<SalaryModel>()
            .Property(s => s.BasicSalary).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<SalaryModel>()
            .Property(s => s.HRA).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<SalaryModel>()
            .Property(s => s.DA).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<SalaryModel>()
            .Property(s => s.Bonus).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<SalaryModel>()
            .Property(s => s.Deductions).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<SalaryModel>()
            .Property(s => s.NetSalary).HasColumnType("decimal(18,2)");

        // ✅ prevent cascade conflict on taskTbl
        modelBuilder.Entity<TaskModel>()
            .HasOne(t => t.AssignedTo)
            .WithMany()
            .HasForeignKey(t => t.AssignedToEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<LeaveModel>()
            .HasOne(l => l.Employee)
            .WithMany()
            .HasForeignKey(l => l.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AttendanceModel>()
            .HasOne(a => a.Employee)
            .WithMany()
            .HasForeignKey(a => a.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        // ✅ unique constraint — one attendance per employee per day
        modelBuilder.Entity<AttendanceModel>()
            .HasIndex(a => new { a.EmployeeId, a.Date })
            .IsUnique();
    }
}