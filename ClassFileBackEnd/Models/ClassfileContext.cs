using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ClassFileBackEnd.Models;

public partial class ClassfileContext : DbContext
{
    public ClassfileContext()
    {
    }

    public ClassfileContext(DbContextOptions<ClassfileContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<File> Files { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("server=(local); database=Classfile; uid=sa; pwd=sa; TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable("account");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AccountType)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("account_type");
            entity.Property(e => e.Fullname)
                .HasMaxLength(200)
                .HasColumnName("fullname");
            entity.Property(e => e.Password)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("username");
        });

        modelBuilder.Entity<Class>(entity =>
        {
            entity.ToTable("class");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ClassCode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("class_code");
            entity.Property(e => e.ClassName)
                .HasMaxLength(50)
                .HasColumnName("class_name");
            entity.Property(e => e.TeacherAccountId).HasColumnName("teacher_account_id");

            entity.HasOne(d => d.TeacherAccount).WithMany(p => p.ClassesNavigation)
                .HasForeignKey(d => d.TeacherAccountId)
                .HasConstraintName("FK_class_account");

            entity.HasMany(d => d.Accounts).WithMany(p => p.Classes)
                .UsingEntity<Dictionary<string, object>>(
                    "AccountClass",
                    r => r.HasOne<Account>().WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_account_class_account"),
                    l => l.HasOne<Class>().WithMany()
                        .HasForeignKey("ClassId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_account_class_class"),
                    j =>
                    {
                        j.HasKey("ClassId", "AccountId").HasName("PK_student_class");
                        j.ToTable("account_class");
                        j.IndexerProperty<int>("ClassId").HasColumnName("class_id");
                        j.IndexerProperty<int>("AccountId").HasColumnName("account_id");
                    });
        });

        modelBuilder.Entity<File>(entity =>
        {
            entity.ToTable("file");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FileName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("file_name");
            entity.Property(e => e.FileNameRoot)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("file_name_root");
            entity.Property(e => e.FileType)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("file_type");
            entity.Property(e => e.PostId).HasColumnName("post_id");

            entity.HasOne(d => d.Post).WithMany(p => p.Files)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK_file_post");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.ToTable("post");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ClassId).HasColumnName("class_id");
            entity.Property(e => e.DateCreated)
                .HasColumnType("datetime")
                .HasColumnName("date_created");
            entity.Property(e => e.PostedAccountId).HasColumnName("posted_account_id");
            entity.Property(e => e.Title)
                .HasMaxLength(500)
                .HasColumnName("title");

            entity.HasOne(d => d.Class).WithMany(p => p.Posts)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK_post_class");

            entity.HasOne(d => d.PostedAccount).WithMany(p => p.Posts)
                .HasForeignKey(d => d.PostedAccountId)
                .HasConstraintName("FK_post_account");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
