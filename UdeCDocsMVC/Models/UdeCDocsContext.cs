using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace UdeCDocsMVC.Models
{
    public partial class UdecDocsContext : DbContext
    {
        public UdecDocsContext()
        {
        }

        public UdecDocsContext(DbContextOptions<UdecDocsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Comment> Comments { get; set; } = null!;
        public virtual DbSet<Document> Documents { get; set; } = null!;
        public virtual DbSet<Faculty> Faculties { get; set; } = null!;
        public virtual DbSet<Field> Fields { get; set; } = null!;
        public virtual DbSet<Rol> Rols { get; set; } = null!;
        public virtual DbSet<TypeVote> TypeVotes { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<Vote> Votes { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasKey(e => e.Idcomment);

                entity.ToTable("Comment");

                entity.Property(e => e.Idcomment).HasColumnName("IDComment");

                entity.Property(e => e.Body).HasColumnName("body");

                entity.Property(e => e.UserW).HasColumnName("userW");

                entity.Property(e => e.Date)
                    .HasColumnType("datetime")
                    .HasColumnName("date");

                entity.Property(e => e.Iddocument).HasColumnName("IDDocument");

                entity.Property(e => e.Iduser).HasColumnName("IDUser");

                entity.HasOne(d => d.IddocumentNavigation)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.Iddocument)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Comment_Document");

                entity.HasOne(d => d.IduserNavigation)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.Iduser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Comment_User");
            });

            modelBuilder.Entity<Document>(entity =>
            {
                entity.HasKey(e => e.Iddocument);

                entity.ToTable("Document");

                entity.Property(e => e.Iddocument).HasColumnName("IDDocument");

                entity.Property(e => e.Abstract)
                    .IsUnicode(false)
                    .HasColumnName("abstract");

                entity.Property(e => e.Authors)
                    .IsUnicode(false)
                    .HasColumnName("authors");

                entity.Property(e => e.Direction)
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasColumnName("direction");

                entity.Property(e => e.Idfield).HasColumnName("IDField");

                entity.Property(e => e.Iduser).HasColumnName("IDUser");

                entity.Property(e => e.Keywords)
                    .IsUnicode(false)
                    .HasColumnName("keywords");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("name");

                entity.Property(e => e.PublicationDate)
                    .HasColumnType("datetime")
                    .HasColumnName("publicationDate");

                entity.HasOne(d => d.IdfieldNavigation)
                    .WithMany(p => p.Documents)
                    .HasForeignKey(d => d.Idfield)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Document_Field");

                entity.HasOne(d => d.IduserNavigation)
                    .WithMany(p => p.Documents)
                    .HasForeignKey(d => d.Iduser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Document_User");
            });

            modelBuilder.Entity<Faculty>(entity =>
            {
                entity.HasKey(e => e.Idfaculty);

                entity.ToTable("Faculty");

                entity.Property(e => e.Idfaculty).HasColumnName("IDFaculty");

                entity.Property(e => e.Faculty1)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("faculty");

                entity.Property(e => e.Description)
                    .HasMaxLength(maxLength: 500)
                    .IsUnicode(false)
                    .HasColumnName("description");
            });

            modelBuilder.Entity<Field>(entity =>
            {
                entity.HasKey(e => e.Idfield);

                entity.ToTable("Field");

                entity.Property(e => e.Idfield).HasColumnName("IDField");

                entity.Property(e => e.Field1)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("field");

                entity.Property(e => e.Idfaculty).HasColumnName("IDFaculty");

                entity.HasOne(d => d.IdfacultyNavigation)
                    .WithMany(p => p.Fields)
                    .HasForeignKey(d => d.Idfaculty)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Field_Faculty");
            });

            modelBuilder.Entity<Rol>(entity =>
            {
                entity.HasKey(e => e.Idrol)
                    .HasName("PK_Role");

                entity.ToTable("Rol");

                entity.Property(e => e.Idrol).HasColumnName("IDRol");

                entity.Property(e => e.Role)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("role");
            });

            modelBuilder.Entity<TypeVote>(entity =>
            {
                entity.HasKey(e => e.IdtypeVote)
                    .HasName("PK_TypeRole");

                entity.ToTable("TypeVote");

                entity.Property(e => e.IdtypeVote).HasColumnName("IDTypeVote");

                entity.Property(e => e.Type)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("type");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Iduser);

                entity.ToTable("User");

                entity.Property(e => e.Iduser).HasColumnName("IDUser");

                entity.Property(e => e.City)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("city");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.Idfaculty).HasColumnName("IDFaculty");

                entity.Property(e => e.Idrol).HasColumnName("IDRol");

                entity.Property(e => e.Institution)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("institution");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("name");

                entity.Property(e => e.Password)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("password");

                entity.HasOne(d => d.IdfacultyNavigation)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.Idfaculty)
                    .HasConstraintName("FK_User_Faculty");

                entity.HasOne(d => d.IdrolNavigation)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.Idrol)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_Rol1");
            });

            modelBuilder.Entity<Vote>(entity =>
            {
                entity.HasKey(e => e.Idvote);

                entity.ToTable("Vote");

                entity.Property(e => e.Idvote).HasColumnName("IDVote");

                entity.Property(e => e.Iddocument).HasColumnName("IDDocument");

                entity.Property(e => e.IdtypeVote).HasColumnName("IDTypeVote");

                entity.Property(e => e.Iduser).HasColumnName("IDUser");

                entity.Property(e => e.Value).HasColumnName("value");

                entity.HasOne(d => d.IddocumentNavigation)
                    .WithMany(p => p.Votes)
                    .HasForeignKey(d => d.Iddocument)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Vote_Document");

                entity.HasOne(d => d.IdtypeVoteNavigation)
                    .WithMany(p => p.Votes)
                    .HasForeignKey(d => d.IdtypeVote)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Vote_TypeVote1");

                entity.HasOne(d => d.IduserNavigation)
                    .WithMany(p => p.Votes)
                    .HasForeignKey(d => d.Iduser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Vote_User");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
