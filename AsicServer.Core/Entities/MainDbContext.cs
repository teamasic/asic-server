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

        public virtual DbSet<AttendeeGroup> AttendeeGroup { get; set; }
        public virtual DbSet<ChangeRequest> ChangeRequest { get; set; }
        public virtual DbSet<Group> Group { get; set; }
        public virtual DbSet<Record> Record { get; set; }
        public virtual DbSet<RecordStaging> RecordStaging { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<Room> Room { get; set; }
        public virtual DbSet<Session> Session { get; set; }
        public virtual DbSet<User> User { get; set; }

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
                    .WithMany(p => p.AttendeeGroup)
                    .HasForeignKey(d => d.AttendeeCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AttendeeGroups_User");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.AttendeeGroup)
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
                    .WithMany(p => p.Record)
                    .HasForeignKey(d => d.AttendeeGroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Records_AttendeeGroups");

                entity.HasOne(d => d.Session)
                    .WithMany(p => p.Record)
                    .HasForeignKey(d => d.SessionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Records_0_0");
            });

            modelBuilder.Entity<RecordStaging>(entity =>
            {
                entity.Property(e => e.AttendeeCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.GroupCode).HasMaxLength(50);

                entity.Property(e => e.GroupCreateTime).HasColumnType("datetime");

                entity.Property(e => e.GroupName).HasMaxLength(255);

                entity.Property(e => e.RoomId).HasColumnType("int");

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

                entity.HasOne(d => d.GroupCodeNavigation)
                    .WithMany(p => p.Session)
                    .HasForeignKey(d => d.GroupCode)
                    .HasConstraintName("FK_Sessions_Groups");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.Session)
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

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Image).IsUnicode(false);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_Role");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
