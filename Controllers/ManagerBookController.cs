using AutoMapper;
using GoogleBookAPI.Models.DTOs;
using GoogleBookAPI.Models.Entities;
using GoogleBookAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GoogleBookAPI.Controllers
{
    [ApiController]
    [Route("api/manager/books")]
    public class ManagerBookController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;

        public ManagerBookController(IBookRepository bookRepository, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
        }

        // GET: api/manager/books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ManagerBookDTO>>> GetAllBooks()
        {
            var books = await _bookRepository.GetAllAsync<Book>();
            var bookDtos = _mapper.Map<IEnumerable<ManagerBookDTO>>(books);
            return Ok(bookDtos);
        }

        // GET: api/manager/books/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ManagerBookDTO>> GetBookById(int id)
        {
            var book = await _bookRepository.GetBookWithDetailsAsync(id);
            if (book == null)
                return NotFound($"Book with ID {id} not found.");

            var bookDto = _mapper.Map<ManagerBookDTO>(book);
            return Ok(bookDto);
        }

        // GET: api/customer/books/category/fiction
        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<ManagerBookDTO>>> GetBooksByCategory(string category)
        {
            var books = await _bookRepository.GetBooksByCategoryAsync(category);
            var bookDtos = _mapper.Map<IEnumerable<ManagerBookDTO>>(books);

            return Ok(bookDtos);
        }

        // POST: api/manager/books
        [HttpPost]
        public async Task<ActionResult<ManagerBookDTO>> CreateBook([FromBody] ManagerBookDTO bookDto)
        {
            if (bookDto == null)
                return BadRequest("Book data is required.");

            var book = _mapper.Map<Book>(bookDto);

            await _bookRepository.AddAsync(book);
            await _bookRepository.SaveChangesAsync();

            var createdBookDto = _mapper.Map<ManagerBookDTO>(book);

            return CreatedAtAction(nameof(GetBookById), new { id = book.Id }, createdBookDto);
        }

        // PUT: api/manager/books/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] ManagerBookDTO bookDto)
        {
            if (bookDto == null)
                return BadRequest("Book data is required.");

            var existingBook = await _bookRepository.GetBookWithDetailsAsync(id);
            if (existingBook == null)
                return NotFound($"Book with ID {id} not found.");

            // Update fields
            _mapper.Map(bookDto, existingBook);
            _bookRepository.Update(existingBook);
            await _bookRepository.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/manager/books/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var existingBook = await _bookRepository.GetByIdAsync<Book>(id);
            if (existingBook == null)
                return NotFound($"Book with ID {id} not found.");

            _bookRepository.Delete(existingBook);
            await _bookRepository.SaveChangesAsync();

            return NoContent();
        }

    }
}
