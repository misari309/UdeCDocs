using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace UdeCDocsMVC.Models
{
    public partial class UdeCDocsContext : DbContext
    {
        public UdeCDocsContext()
        {
        }

        public UdeCDocsContext(DbContextOptions<UdeCDocsContext> options)
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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=LAPTOP-96B65B8M\\SQLEXPRESS; Database=UdeCDocs; Trusted_Connection=true;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasKey(e => e.Idcomment);

                entity.ToTable("Comment");

                entity.Property(e => e.Idcomment).HasColumnName("IDComment");

                entity.Property(e => e.Body)
                    .HasMaxLength(256)
                    .HasColumnName("body")
                    .IsFixedLength();

                entity.Property(e => e.Date)
                    .HasColumnType("date")
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
                    .HasMaxLength(256)
                    .HasColumnName("abstract")
                    .IsFixedLength();

                entity.Property(e => e.Authors)
                    .HasMaxLength(256)
                    .HasColumnName("authors")
                    .IsFixedLength();

                entity.Property(e => e.Direction)
                    .HasMaxLength(256)
                    .HasColumnName("direction")
                    .IsFixedLength();

                entity.Property(e => e.Idfield).HasColumnName("IDField");

                entity.Property(e => e.Iduser).HasColumnName("IDUser");

                entity.Property(e => e.Keywords)
                    .HasMaxLength(256)
                    .HasColumnName("keywords")
                    .IsFixedLength();

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name")
                    .IsFixedLength();

                entity.Property(e => e.PublicationDate)
                    .HasColumnType("date")
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
                    .HasColumnName("faculty")
                    .IsFixedLength();
            });

            modelBuilder.Entity<Field>(entity =>
            {
                entity.HasKey(e => e.Idfield);

                entity.ToTable("Field");

                entity.Property(e => e.Idfield).HasColumnName("IDField");

                entity.Property(e => e.Field1)
                    .HasMaxLength(100)
                    .HasColumnName("field")
                    .IsFixedLength();

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
                    .HasMaxLength(15)
                    .HasColumnName("role")
                    .IsFixedLength();
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
                    .HasColumnName("city");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .HasColumnName("email");

                entity.Property(e => e.Idfaculty).HasColumnName("IDFaculty");

                entity.Property(e => e.Idrol).HasColumnName("IDRol");

                entity.Property(e => e.Institution)
                    .HasMaxLength(50)
                    .HasColumnName("institution");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.Password)
                    .HasMaxLength(100)
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

                entity.Property(e => e.Idvote)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("IDVote");

                entity.Property(e => e.Iddocument).HasColumnName("IDDocument");

                entity.Property(e => e.IdtypeVote).HasColumnName("IDTypeVote");

                entity.Property(e => e.Iduser).HasColumnName("IDUser");

                entity.Property(e => e.Value).HasColumnName("value");

                entity.HasOne(d => d.IddocumentNavigation)
                    .WithMany(p => p.Votes)
                    .HasForeignKey(d => d.Iddocument)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Vote_Document");

                entity.HasOne(d => d.IduserNavigation)
                    .WithMany(p => p.Votes)
                    .HasForeignKey(d => d.Iduser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Vote_User");

                entity.HasOne(d => d.IdvoteNavigation)
                    .WithOne(p => p.Vote)
                    .HasForeignKey<Vote>(d => d.Idvote)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Vote_TypeVote");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
