using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace InstagramManager.Models;

public partial class InstagramManagerDatabaseContext : DbContext
{
    public InstagramManagerDatabaseContext()
    {
    }

    public InstagramManagerDatabaseContext(DbContextOptions<InstagramManagerDatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<FollowForFollowTable> FollowForFollowTables { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Database=InstagramManagerDatabase;Username=postgres;Password=0415");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FollowForFollowTable>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("FollowForFollowTable_pkey");

            entity.ToTable("FollowForFollowTable");

            entity.Property(e => e.Id)
                .HasColumnType("character varying")
                .HasColumnName("id");
            entity.Property(e => e.Address)
                .HasColumnType("character varying")
                .HasColumnName("address");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.Description)
                .HasColumnType("character varying")
                .HasColumnName("description");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
