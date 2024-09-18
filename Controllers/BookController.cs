using AutoMapper;
using BookProject.DTO;
using BookProject.Interface;
using BookProject.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;

namespace BookProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {

        private readonly ILogger<BookController> _logger;
        private readonly IBookService _bookService;

        public BookController(ILogger<BookController> logger, IBookService bookService)
        {
            _logger = logger;
            _bookService = bookService;
        }

        /// <summary>
        /// Get All Books
        /// </summary>
        /// <response code="200">All books returned successfully.</response>
        [HttpGet(Name = "GetAllBooks")]
        public async Task<ActionResult<IEnumerable<Book>>> GetAllBooks()
        {
            var books = await _bookService.GetAllBooks();
            return Ok(books);
        }

        /// <summary>
        /// Get a book by its title
        /// </summary>
        /// <param name="title">The title of the book.</param>
        /// <response code="200">The book was found and return successfully.</response>
        /// <response code="404">Book with given title was not found.</response>
        [HttpGet("title/{title}", Name ="GetBookByTitle")]
        public async Task<ActionResult<BookDTO>> GetBookByTitle(String title)
        {
            var book = await _bookService.GetBookByTitle(title);
            if (book == null)
            {
                return NotFound(new ErrorResponse { Message = "Book Not Found" });
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
        public async Task<ActionResult<IEnumerable<BookDTO>>> GetBookByAuthor(string author)
        {
            var books = await _bookService.GetBookByAuthor(author);
            if (!books.Any())
            {
                return NotFound(new ErrorResponse { Message = "There aren't any books written by this author" });
            }
            return Ok(books);
        }

        /// <summary>
        /// Get a book by its ISBN
        /// </summary>
        /// <param name="isbn">ISBN of the book</param>
        /// <response code="200">The book was found and return successfully.</response>
        /// <response code="404">Book with given ISBN was not found.</response>
        [HttpGet("isbn", Name = "GetBookByISBN")]
        public async Task<ActionResult<BookDTO>> GetBookByISBN([FromQuery] string isbn)
        {
            var book = await _bookService.GetBookByISBN(isbn);
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
        /// <response code="201">A book successfully created and added to the database.</response>
        /// <response code="400">The book object is not valid.</response>
        /// <response code="409">Book with given ISBN is already in the database.</response>
        [HttpPost("add", Name = "AddBook")]
        public async Task<ActionResult> AddBook([FromBody] BookDTO bookDto)
        {
            try
            {
                await _bookService.AddBook(bookDto);
                return CreatedAtRoute("GetBookByISBN", new { isbn = bookDto.ISBN }, new { message = "Book successfully added.", bookDto });

            }
            catch (InvalidOperationException e)
            {
                return Conflict(new ErrorResponse { Message = e.Message });
            }
        }

        /// <summary>
        /// Update a book from
        /// </summary>
        /// <param name="id">Book Id</param>
        /// <response code="200">A book successfully updated.</response>
        /// <response code="404">Book with given Id was not found.</response>
        /// <response code="409">Book with given Id is already in the list.</response>
        [HttpPut("edit/{id}", Name = "EditBook")]
        public async Task<ActionResult> EditBook(Guid id, [FromBody] BookDTO bookDto)
        {
            try
            {
                await _bookService.EditBook(id, bookDto);
                return Ok(new { message = "Book successfully updated.", bookDto });
            } 
            catch (InvalidOperationException e)
            {
                return Conflict(new ErrorResponse { Message = e.Message });
            }

        }

        /// <summary>
        /// Delete a book
        /// </summary>
        /// <param name="id">Book Id</param>
        /// <response code="200">A book successfully deleted.</response>
        /// <response code="404">Book with given Id was not found.</response>
        [HttpDelete("delete/{id}", Name = "DeleteBook")]
        public async Task<ActionResult> DeleteBook(Guid id)
        {
            var deletedBookDto = await _bookService.DeleteBook(id);

            if (deletedBookDto == null)
            {
                return NotFound(new ErrorResponse { Message = "Book with given Id is not found." });
            }

            // Return the deleted book DTO
            return Ok(new { message = "Book successfully deleted.", book = deletedBookDto });
        }

    }

}
