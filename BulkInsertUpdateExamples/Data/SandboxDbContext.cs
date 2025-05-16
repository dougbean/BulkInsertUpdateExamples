using System;
using System.Collections.Generic;
using BulkInsertUpdateExamples.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols;
using System.Configuration;

namespace BulkInsertUpdateExamples.Data;

public partial class SandboxDbContext : DbContext
{
    private string connectionString = string.Empty;
    public SandboxDbContext()
    {
        connectionString = ConfigurationManager.ConnectionStrings["database"].ConnectionString;
    }

    public SandboxDbContext(DbContextOptions<SandboxDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Child> Children { get; set; }

    public virtual DbSet<Parent> Parents { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {   
            optionsBuilder.UseSqlServer(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Child>(entity =>
        {
            entity.ToTable("Child");

            entity.Property(e => e.Name).HasMaxLength(255);

            entity.HasOne(d => d.Parent).WithMany(p => p.Children)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK_Child_Mock");
        });

        modelBuilder.Entity<Parent>(entity =>
        {
            entity.ToTable("Parent");

            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Product__3214EC0743D54369");

            entity.ToTable("Product");

            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Price).HasColumnType("money");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
