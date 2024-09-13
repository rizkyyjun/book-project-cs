using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace BookProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private static readonly List<Book> Books = new List<Book>
        {
            new Book {isbn = "12345", title = "book1", author = "a1", pages = 412},
            new Book {isbn = "23456", title = "book2", author = "a1", pages = 441, description = "history book"},
            new Book {isbn = "123141", title = "book3", author = "a2", pages = 121},
        };

        private readonly ILogger<BookController> _logger;

        public BookController(ILogger<BookController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Get All Books
        /// </summary>
        /// <response code="200">All books returned successfully.</response>
        [HttpGet(Name = "GetAllBooks")]
        public IEnumerable<Book> GetAllBooks()
        {
            //var allBooks = Books.Select(b => b);
            //return allBooks;
            return Books;
        }

        /// <summary>
        /// Get a book by its title
        /// </summary>
        /// <param name="title">The title of the book.</param>
        /// <response code="200">The book was found and return successfully.</response>
        /// <response code="404">Book with given title was not found.</response>
        [HttpGet("title/{title}", Name ="GetBookByTitle")]
        public ActionResult<Book> GetBookByTitle(string title)
        {
            var book = Books.FirstOrDefault(b => b.title.Equals(title, StringComparison.OrdinalIgnoreCase));
            if (book == null)
            {
                return NotFound(new ErrorResponse { Message = "Book Not Found"});
            }
            return Ok(book);
        }

        /// <summary>
        /// Get list of books with given author
        /// </summary>
        /// <param name="author">The author of the book.</param>
        /// <response code="200">The books were found and return successfully.</response>
        /// <response code="404">Books with given author was not found.</response>
        [HttpGet("author/{author}", Name = "GetBookByAuthor")]
        public ActionResult<IEnumerable<Book>> GetBookByAuthor(String author)
        {
            var filteredBooks = Books.Where(b =>
                b.author.Equals(author, StringComparison.OrdinalIgnoreCase));

            if (!filteredBooks.Any())
            {
                return NotFound(new ErrorResponse { Message = "There aren't any books written by this author" });
            }
            return Ok(filteredBooks);
           
        }

        /// <summary>
        /// Get a book by its ISBN
        /// </summary>
        /// <param name="isbn">ISBN of the book</param>
        /// <response code="200">The book was found and return successfully.</response>
        /// <response code="404">Book with given ISBN was not found.</response>
        [HttpGet("isbn", Name = "GetBookByISBN")]
        public ActionResult<IEnumerable<Book>> GetBookByISBN([FromQuery] String isbn)
        {
            var book = Books.FirstOrDefault(b => b.isbn.Equals(isbn, StringComparison.OrdinalIgnoreCase));
            if (book == null)
            {
                return NotFound(new ErrorResponse { Message = "Book with given ISBN not found" });
            };
            return Ok(book);
        }

        /// <summary>
        /// Add book to the list
        /// </summary>
        /// <param name="book">Book Object</param>
        /// <response code="201">A book successfully created and added to the list.</response>
        /// <response code="400">The book object is not valid.</response>
        /// <response code="409">Book with given ISBN is already in the list.</response>
        [HttpPost("add", Name = "AddBook")]
        public ActionResult<Book> AddBook([FromBody] Book book)
        {
            if (book == null)
            {
                return BadRequest("Book can't be null");
            }
            // ISBN should be unique, check it first
            var DuplicateBooks = Books.Where(b => b.isbn.Equals(book.isbn, StringComparison.OrdinalIgnoreCase));
            
            if (DuplicateBooks.Any())
            {
                return Conflict(new ErrorResponse { Message = "A book with the same ISBN already exists" });
            }
            
            Books.Add(book);
            return CreatedAtRoute("GetBookByISBN", new { isbn = book.isbn }, new { message = "Book successfully added." }); ;
        }

        /// <summary>
        /// Delete a book from the list
        /// </summary>
        /// <param name="isbn">Book ISBN</param>
        /// <response code="200">A book successfully deleted from the list.</response>
        /// <response code="404">Book with given ISBN was not found.</response>
        [HttpDelete("delete/isbn", Name ="DeleteBook")]
        public ActionResult<Object> DeleteBook([FromQuery] string isbn)
        {
            var book = Books.FirstOrDefault(b => b.isbn.Equals(isbn, StringComparison.OrdinalIgnoreCase));

            if (book == null)
            {
                return NotFound(new ErrorResponse { Message = "Book with given ISBN is not found." });
            }
            Books.Remove(book);
            return Ok(new { message = "Book successfully deleted.", book });
        }
    }

}
