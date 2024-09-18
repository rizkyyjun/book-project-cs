using Microsoft.EntityFrameworkCore;

namespace BookProject.Model
{
    public class BookProjectDbContext : DbContext
    {
        public BookProjectDbContext(DbContextOptions<BookProjectDbContext> options) : base(options) { }  

        public DbSet<Book> Books { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
