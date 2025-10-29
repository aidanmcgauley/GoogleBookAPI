using GoogleBookAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace GoogleBookAPI.Data
{
    public class BookDbContext : DbContext
    {
        public BookDbContext(DbContextOptions<BookDbContext> options) : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<IndustryIdentifier> IndustryIdentifiers { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Many-to-many: Book ↔ Authors
            modelBuilder.Entity<Book>()
                .HasMany(b => b.Authors)
                .WithMany(a => a.Books);

            // Many-to-many: Book ↔ Categories
            modelBuilder.Entity<Book>()
                .HasMany(b => b.Categories)
                .WithMany(c => c.Books);

            modelBuilder.Entity<Book>()
                .HasMany(b => b.IndustryIdentifiers)
                .WithOne(ii => ii.Book)
                .HasForeignKey(ii => ii.BookId);

        }
    }
}