using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LibraryAPI.Models;

public partial class LibraryContext : DbContext
{
    public LibraryContext()
    {
    }

    public LibraryContext(DbContextOptions<LibraryContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.;Database=Library;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Author__3214EC07EFF252AD");

            entity.ToTable("Author");

            entity.Property(e => e.BirthDate).HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Book__3214EC07F46E9FC5");

            entity.ToTable("Book");

            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.Author).WithMany(p => p.Books)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Book__AuthorId__3B75D760");

            entity.HasMany(d => d.Categories).WithMany(p => p.Books)
                .UsingEntity<Dictionary<string, object>>(
                    "BookCategory",
                    r => r.HasOne<Category>().WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__BookCateg__Categ__3F466844"),
                    l => l.HasOne<Book>().WithMany()
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__BookCateg__BookI__3E52440B"),
                    j =>
                    {
                        j.HasKey("BookId", "CategoryId").HasName("PK__BookCate__9C7051A7ED48DA29");
                        j.ToTable("BookCategory");
                    });
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Category__3214EC07CD90BCBF");

            entity.ToTable("Category");

            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .IsUnicode(false);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC078365413C");

            entity.ToTable("User");

            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
