using BookProject.Interface;
using BookProject.Model;
using Microsoft.EntityFrameworkCore;

namespace BookProject.Repository
{
    public class BookRepository : BaseRepository<Book>, IBookRepository
    {
        private readonly BookProjectDbContext _dbContext;
        public BookRepository(BookProjectDbContext dbContext) : base(dbContext) { }

    }
}
