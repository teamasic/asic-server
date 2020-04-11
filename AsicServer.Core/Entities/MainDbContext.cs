using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AsicServer.Core.Entities
{
    public partial class MainDbContext : DbContext
    {
        public MainDbContext()
        {
        }

        public MainDbContext(DbContextOptions<MainDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AttendeeGroups> AttendeeGroups { get; set; }
        public virtual DbSet<ChangeRequests> ChangeRequests { get; set; }
        public virtual DbSet<Groups> Groups { get; set; }
        public virtual DbSet<Records> Records { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<Rooms> Rooms { get; set; }
        public virtual DbSet<Sessions> Sessions { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<RecordStaging> RecordStaging { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AttendeeGroups>(entity =>
            {
                entity.HasKey(e => new { e.AttendeeId, e.GroupId })
                    .HasName("sqlite_autoindex_AttendeeGroups_1");

                entity.HasOne(d => d.Attendee)
                    .WithMany(p => p.AttendeeGroups)
                    .HasForeignKey(d => d.AttendeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AttendeeGroups_User");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.AttendeeGroups)
                    .HasForeignKey(d => d.GroupId)
                    .HasConstraintName("FK_AttendeeGroups_0_0");
            });

            modelBuilder.Entity<ChangeRequests>(entity =>
            {
                entity.Property(e => e.Comment).HasMaxLength(255);

                entity.HasOne(d => d.Record)
                    .WithMany(p => p.ChangeRequests)
                    .HasForeignKey(d => d.RecordId)
                    .HasConstraintName("FK_ChangeRequests_0_0");
            });

            modelBuilder.Entity<Groups>(entity =>
            {
                entity.HasIndex(e => e.Code)
                    .HasName("UK_Groups_Code")
                    .IsUnique();

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DateTimeCreated).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(255);
            });

            modelBuilder.Entity<RecordStaging>(entity =>
            {
                entity.Property(e => e.AttendeeCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.AttendeeName).HasMaxLength(255);

                entity.Property(e => e.GroupCode).HasMaxLength(50);

                entity.Property(e => e.GroupCreateTime).HasColumnType("datetime");

                entity.Property(e => e.GroupName).HasMaxLength(255);

                entity.Property(e => e.RoomName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RtspString)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.SessionEndTime).HasColumnType("datetime");

                entity.Property(e => e.SessionName).HasMaxLength(50);

                entity.Property(e => e.SessionStartTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<Records>(entity =>
            {
                entity.Property(e => e.AttendeeCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.EndTime).HasColumnType("datetime");

                entity.Property(e => e.SessionName).HasMaxLength(50);

                entity.Property(e => e.StartTime).HasColumnType("datetime");

                entity.HasOne(d => d.Attendee)
                    .WithMany(p => p.Records)
                    .HasForeignKey(d => d.AttendeeId)
                    .HasConstraintName("FK_Records_User");

                entity.HasOne(d => d.Session)
                    .WithMany(p => p.Records)
                    .HasForeignKey(d => d.SessionId)
                    .HasConstraintName("FK_Records_0_0");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasMany(d => d.User)
                .WithOne(d => d.Role);

            });

            modelBuilder.Entity<Rooms>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RtspString)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Sessions>(entity =>
            {
                entity.Property(e => e.EndTime).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.RoomName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.RtspString)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.StartTime).HasColumnType("datetime");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.Sessions)
                    .HasForeignKey(d => d.GroupId)
                    .HasConstraintName("FK_Sessions_0_0");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.RollNumber)
                    .HasName("UK_RollNumber_Account")
                    .IsUnique();

                entity.HasIndex(e => e.Username)
                    .HasName("IX_Account")
                    .IsUnique();

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Fullname)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Image).IsUnicode(false);

                entity.Property(e => e.RollNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_User_Role");
            });
        }
    }
}
