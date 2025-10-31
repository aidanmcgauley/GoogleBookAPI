using AutoMapper;
using GoogleBookAPI.Models.DTOs;
using GoogleBookAPI.Models.Entities;
using GoogleBookAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GoogleBookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerBookController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;

        public CustomerBookController(IBookRepository bookRepository, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
        }

        // GET: api/customer/books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerBookDTO>>> GetAllBooks()
        {
            var books = await _bookRepository.GetAllAsync<Book>();
            var bookDtos = _mapper.Map<IEnumerable<CustomerBookDTO>>(books);
            return Ok(bookDtos);
        }

        // GET: api/customer/books/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CustomerBookDTO>> GetBookById(int id)
        {
            var book = await _bookRepository.GetBookWithDetailsAsync(id);
            if (book == null)
                return NotFound($"Book with ID {id} not found.");

            var bookDto = _mapper.Map<CustomerBookDTO>(book);
            return Ok(bookDto);
        }

        // GET: api/customer/books/search?term=example
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<CustomerBookDTO>>> SearchBooks([FromQuery] string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return BadRequest("Search term cannot be empty.");

            var books = await _bookRepository.SearchBooksAsync(term);
            var bookDtos = _mapper.Map<IEnumerable<CustomerBookDTO>>(books);

            return Ok(bookDtos);
        }

        // GET: api/customer/books/category/fiction
        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<CustomerBookDTO>>> GetBooksByCategory(string category)
        {
            var books = await _bookRepository.GetBooksByCategoryAsync(category);
            var bookDtos = _mapper.Map<IEnumerable<CustomerBookDTO>>(books);

            return Ok(bookDtos);
        }
    }
}
