using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace studentsAPI.Models;

public partial class StudentContext : DbContext
{
    public StudentContext()
    {
    }

    public StudentContext(DbContextOptions<StudentContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<GradeLevel> GradeLevels { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=jihcsqlserver.database.windows.net;Initial Catalog=student;User ID=jihcsqkserver;Password=Madiyar777.;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Class__3214EC07994A29F5");

            entity.ToTable("Class");

            entity.HasIndex(e => e.ClassName, "UQ__Class__F8BF561B1B25F596").IsUnique();

            entity.Property(e => e.ClassName).HasMaxLength(10);
        });

        modelBuilder.Entity<GradeLevel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__GradeLev__3214EC0734B8E84B");

            entity.ToTable("GradeLevel");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Student__3214EC07B3BF68E5");

            entity.ToTable("Student");

            entity.HasIndex(e => e.Email, "UQ__Student__A9D1053411F1E326").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.ClassId).HasColumnName("ClassID");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.GradeLevelId).HasColumnName("GradeLevelID");
            entity.Property(e => e.LastName).HasMaxLength(50);

            entity.HasOne(d => d.Class).WithMany(p => p.Students)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK__Student__ClassID__66603565");

            entity.HasOne(d => d.GradeLevel).WithMany(p => p.Students)
                .HasForeignKey(d => d.GradeLevelId)
                .HasConstraintName("FK__Student__GradeLe__6754599E");

            entity.HasMany(d => d.Subjects).WithMany(p => p.Students)
                .UsingEntity<Dictionary<string, object>>(
                    "StudentsSubject",
                    r => r.HasOne<Subject>().WithMany()
                        .HasForeignKey("SubjectId")
                        .HasConstraintName("FK__StudentsS__Subje__6B24EA82"),
                    l => l.HasOne<Student>().WithMany()
                        .HasForeignKey("StudentId")
                        .HasConstraintName("FK__StudentsS__Stude__6A30C649"),
                    j =>
                    {
                        j.HasKey("StudentId", "SubjectId").HasName("PK__Students__A80491A300D7361A");
                        j.ToTable("StudentsSubjects");
                    });
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Subjects__3214EC072CE915D4");

            entity.HasIndex(e => e.SubjectName, "UQ__Subjects__4C5A7D552D6511B5").IsUnique();

            entity.Property(e => e.SubjectName).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
