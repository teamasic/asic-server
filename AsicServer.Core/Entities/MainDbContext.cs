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

        public virtual DbSet<AttendeeGroup> AttendeeGroups { get; set; }
        public virtual DbSet<ChangeRequest> ChangeRequests { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<Record> Records { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<Session> Sessions { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<RecordStaging> RecordStaging { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AttendeeGroup>(entity =>
            {
                entity.HasIndex(e => new { e.AttendeeCode, e.GroupCode })
                    .HasName("UK_AttendeeGroups")
                    .IsUnique();

                entity.Property(e => e.AttendeeCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.GroupCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Attendee)
                    .WithMany(p => p.AttendeeGroups)
                    .HasForeignKey(d => d.AttendeeCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AttendeeGroups_User");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.AttendeeGroups)
                    .HasForeignKey(d => d.GroupCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AttendeeGroups_Groups");
            });

            modelBuilder.Entity<ChangeRequest>(entity =>
            {
                entity.HasIndex(e => e.RecordId)
                    .HasName("UK_ChangeRequests_RecordId")
                    .IsUnique();

                entity.Property(e => e.Comment).HasMaxLength(255);

                entity.HasOne(d => d.Record)
                    .WithOne(p => p.ChangeRequest)
                    .HasForeignKey<ChangeRequest>(d => d.RecordId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ChangeRequests_Records");
            });

            modelBuilder.Entity<Group>(entity =>
            {
                entity.HasKey(e => e.Code)
                    .HasName("PK_Groups");

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DateTimeCreated)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Name).HasMaxLength(100);
            });

            modelBuilder.Entity<Record>(entity =>
            {
                entity.Property(e => e.AttendeeCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.EndTime).HasColumnType("datetime");

                entity.Property(e => e.SessionName).HasMaxLength(50);

                entity.Property(e => e.StartTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.AttendeeGroup)
                    .WithMany(p => p.Records)
                    .HasForeignKey(d => d.AttendeeGroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Records_AttendeeGroups");

                entity.HasOne(d => d.Session)
                    .WithMany(p => p.Records)
                    .HasForeignKey(d => d.SessionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Records_0_0");
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

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.Property(e => e.CameraConnectionString)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Session>(entity =>
            {
                entity.Property(e => e.EndTime).HasColumnType("datetime");

                entity.Property(e => e.GroupCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.StartTime).HasColumnType("datetime");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.Sessions)
                    .HasForeignKey(d => d.GroupCode)
                    .HasConstraintName("FK_Sessions_Groups");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.Sessions)
                    .HasForeignKey(d => d.RoomId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Sessions_Rooms");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.HasIndex(e => e.Email)
                    .HasName("UK_User_Email")
                    .IsUnique();

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Fullname)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Image).IsUnicode(false);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_Role");
            });
        }
    }
}
