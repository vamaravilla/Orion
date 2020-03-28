using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace SevenDays.Api.Models
{
    public partial class SevenDaysContext : DbContext
    {
        public SevenDaysContext()
        {
        }

        public SevenDaysContext(DbContextOptions<SevenDaysContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Inventory> Inventory { get; set; }
        public virtual DbSet<Liked> Liked { get; set; }
        public virtual DbSet<Movie> Movie { get; set; }
        public virtual DbSet<Rental> Rental { get; set; }
        public virtual DbSet<Sale> Sale { get; set; }
        public virtual DbSet<User> User { get; set; }

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Inventory>(entity =>
            {
                entity.HasOne(d => d.IdMovieNavigation)
                    .WithMany(p => p.Inventory)
                    .HasForeignKey(d => d.IdMovie)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MovieInventory");
            });

            modelBuilder.Entity<Liked>(entity =>
            {
                entity.HasKey(e => new { e.IdMovie, e.IdUser });

                entity.HasOne(d => d.IdMovieNavigation)
                    .WithMany(p => p.Liked)
                    .HasForeignKey(d => d.IdMovie)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MovieLiked");

                entity.HasOne(d => d.IdUserNavigation)
                    .WithMany(p => p.Liked)
                    .HasForeignKey(d => d.IdUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserLiked");
            });

            modelBuilder.Entity<Movie>(entity =>
            {
                entity.Property(e => e.Description).IsUnicode(false);

                entity.Property(e => e.Image).IsUnicode(false);

                entity.Property(e => e.Title).IsUnicode(false);
            });

            modelBuilder.Entity<Rental>(entity =>
            {
                entity.HasOne(d => d.IdInventoryNavigation)
                    .WithMany(p => p.Rental)
                    .HasForeignKey(d => d.IdInventory)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_InventoryRental");

                entity.HasOne(d => d.IdUserNavigation)
                    .WithMany(p => p.Rental)
                    .HasForeignKey(d => d.IdUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserRental");
            });

            modelBuilder.Entity<Sale>(entity =>
            {
                entity.HasKey(e => new { e.IdInventory, e.IdUser });

                entity.HasOne(d => d.IdInventoryNavigation)
                    .WithMany(p => p.Sale)
                    .HasForeignKey(d => d.IdInventory)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_InventorySale");

                entity.HasOne(d => d.IdUserNavigation)
                    .WithMany(p => p.Sale)
                    .HasForeignKey(d => d.IdUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserSale");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Email).IsUnicode(false);

                entity.Property(e => e.Name).IsUnicode(false);

                entity.Property(e => e.Password).IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
