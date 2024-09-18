using BookProject.DTO;
using BookProject.Model;

namespace BookProject.Interface
{
    public interface IBookService
    {
        Task<IEnumerable<Book>> GetAllBooks();
        Task<BookDTO> GetBookByTitle(string title);
        Task<IEnumerable<BookDTO>> GetBookByAuthor(string author);
        Task<BookDTO> GetBookByISBN(string isbn);
        Task AddBook(BookDTO bookDTO);
        Task EditBook(Guid id, BookDTO bookDTO);
        Task<BookDTO> DeleteBook(Guid id);
    }
}
