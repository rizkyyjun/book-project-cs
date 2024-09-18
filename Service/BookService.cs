using AutoMapper;
using BookProject.DTO;
using BookProject.Interface;
using BookProject.Model;
using BookProject.Repository;
using Microsoft.EntityFrameworkCore;
using System.Runtime.ConstrainedExecution;

namespace BookProject.Service
{
    public class BookService : IBookService
    {
        
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRedisCacheService _redisCacheService;

        public BookService(IUnitOfWork unitOfWork, IMapper mapper, IRedisCacheService redisCacheService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _redisCacheService = redisCacheService;
        }

        public async Task<IEnumerable<Book>> GetAllBooks()
        {
            var cacheKey = "allBooks";
            var cachedBooks = await _redisCacheService.GetAsync<IEnumerable<Book>>(cacheKey);

            if (cachedBooks != null)
            {
                return cachedBooks;
            }

            var books = await _unitOfWork.BookRepository.GetAll().ToListAsync();

            await _redisCacheService.SaveAsync(cacheKey, books);
            return books;

            //var books = await _unitOfWork.BookRepository.GetAll().ToListAsync();
            //return _mapper.Map<IEnumerable<Book>>(books);
        }

        public async Task<BookDTO> GetBookByTitle(string title)
        {
            var cacheKey = $"book_title_{title}";
            var cachedBook = await _redisCacheService.GetAsync<BookDTO>(cacheKey);

            if (cachedBook != null)
            {
                return cachedBook;
            }

            var book = await _unitOfWork.BookRepository.GetSingleAsync(b =>
                b.Title.ToLower() == title.ToLower());
            if (book == null) return null;

            var bookDto = _mapper.Map<BookDTO>(book);
            await _redisCacheService.SaveAsync(cacheKey, bookDto);

            return bookDto;

            //var book = await _unitOfWork.BookRepository.GetSingleAsync(b =>
            //    b.Title.ToLower() == title.ToLower());
            //if (book == null) return null;
            //return _mapper.Map<BookDTO>(book);

        }

        public async Task<IEnumerable<BookDTO>> GetBookByAuthor(string author)
        {
            var cacheKey = $"book_author_{author}";
            var cachedBooks = await _redisCacheService.GetAsync<IEnumerable<BookDTO>>(cacheKey);

            if (cachedBooks != null)
            {
                return cachedBooks;
            }

            var books = await _unitOfWork.BookRepository.GetMany(b =>
                b.Author.ToLower() == author.ToLower()).ToListAsync();
            var bookDtos = _mapper.Map<IEnumerable<BookDTO>>(books);

            await _redisCacheService.SaveAsync(cacheKey, bookDtos);
            return bookDtos;

            //var books = await _unitOfWork.BookRepository.GetMany(b =>
            //    b.Author.ToLower() == author.ToLower()).ToListAsync();
            //return _mapper.Map<IEnumerable<BookDTO>>(books);
        }

        public async Task<BookDTO> GetBookByISBN(string isbn)
        {
            var cacheKey = $"book_isbn_{isbn}";
            var cachedBook = await _redisCacheService.GetAsync<BookDTO>(cacheKey);

            if (cachedBook != null)
            {
                return cachedBook;
            }

            var book = await _unitOfWork.BookRepository.GetSingleAsync(b => b.ISBN.ToLower() == isbn.ToLower());
            if (book == null) return null;

            var bookDto = _mapper.Map<BookDTO>(book);
            await _redisCacheService.SaveAsync(cacheKey, bookDto);

            return bookDto;

            //var book = await _unitOfWork.BookRepository.GetSingleAsync(b => b.ISBN.ToLower() == isbn.ToLower());
            //if (book == null) return null;
            //return _mapper.Map<BookDTO>(book);
        }

        public async Task AddBook(BookDTO bookDTO)
        {
            if (bookDTO == null) throw new ArgumentNullException(nameof(bookDTO));

            var existingBook = _unitOfWork.BookRepository.GetAll()
                .Any(b => b.ISBN.ToLower() == bookDTO.ISBN.ToLower());

            // If a book exists, return conflict response
            if (existingBook) throw new InvalidOperationException("A book with the same ISBN already exists");

            // Map DTO to entity
            var book = _mapper.Map<Book>(bookDTO);

            book.CreatedAt = DateTime.UtcNow;
            book.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.BookRepository.AddAsync(book);
            await _unitOfWork.SaveAsync();

            // Clear cache after adding a new book to ensure consistency
            await _redisCacheService.DeleteAsync("allBooks");
        }

        public async Task EditBook(Guid id, BookDTO bookDTO)
        {
            if (bookDTO == null) throw new ArgumentNullException(nameof(bookDTO));

            var existingBook = await _unitOfWork.BookRepository.GetByIdAsync(id);
            if (existingBook == null) throw new InvalidOperationException("Book not found");

            var bookSameIsbn = _unitOfWork.BookRepository.GetAll()
                .Any(b => b.ISBN.ToLower() == bookDTO.ISBN.ToLower() && b.Id != id);

            // If a book exists, return conflict response
            if (bookSameIsbn) throw new InvalidOperationException("A book with the same ISBN already exists");

            // Invalidate cache for consistency
            await _redisCacheService.DeleteAsync("allBooks");
            await _redisCacheService.DeleteAsync($"book_title_{existingBook.Title}");
            await _redisCacheService.DeleteAsync($"book_author_{existingBook.Author}");
            await _redisCacheService.DeleteAsync($"book_isbn_{existingBook.ISBN}");

            existingBook.Title = bookDTO.Title;
            existingBook.ISBN = bookDTO.ISBN;
            existingBook.Author = bookDTO.Author;
            existingBook.Description = bookDTO.Description;
            existingBook.Pages = bookDTO.Pages;

            existingBook.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.BookRepository.Edit(existingBook);
            await _unitOfWork.SaveAsync();

        }

        public async Task<BookDTO> DeleteBook(Guid id)
        {
            var book = await _unitOfWork.BookRepository.GetByIdAsync(id);
            if (book == null) return null;

            _unitOfWork.BookRepository.Delete(book);
            await _unitOfWork.SaveAsync();

            // Invalidate cache after deleting a book
            await _redisCacheService.DeleteAsync("allBooks");
            await _redisCacheService.DeleteAsync($"book_title_{book.Title}");
            await _redisCacheService.DeleteAsync($"book_author_{book.Author}");
            await _redisCacheService.DeleteAsync($"book_isbn_{book.ISBN}");


            return _mapper.Map<BookDTO>(book);
        }
    }
}
