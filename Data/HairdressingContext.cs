using System;
using System.Collections.Generic;
using TransportCompanyWithAuthorize.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.ConstrainedExecution;

namespace TransportCompanyWithAuthorize.Data;

public partial class HairdressingContext : DbContext
{
    public HairdressingContext()
    {
    }

    public HairdressingContext(DbContextOptions<HairdressingContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<EmployeeSchedule> EmployeeSchedules { get; set; }

    public virtual DbSet<PerformedService> PerformedServices { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Services> Services { get; set; }

    public virtual DbSet<ServiceType> ServiceTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Services>(entity =>
        {
            entity.Property(e => e.ServiceTypeId).HasColumnName("service_type_id");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.Property(e => e.FullName).HasColumnName("full_name");
            entity.Property(e => e.TotalServicesCost).HasColumnName("total_services_cost");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.Property(e => e.FullName).HasColumnName("full_name");
        });

        modelBuilder.Entity<EmployeeSchedule>(entity =>
        {
            entity.ToTable("EmployeeSchedule");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.IsWorking).HasColumnName("is_working");
            entity.Property(e => e.WorkDate).HasColumnName("work_date");
        });

        modelBuilder.Entity<PerformedService>(entity =>
        {
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.ServiceDate).HasColumnName("service_date");
            entity.Property(e => e.ServiceId).HasColumnName("service_id");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.ReviewDate).HasColumnName("review_date");
            entity.Property(e => e.ReviewText).HasColumnName("review_text");
            entity.Property(e => e.ServiceId).HasColumnName("service_id");
        });
    }

}
