using DemoAPI.DbContexts;
using DemoAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace DemoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly StudentDbContext _context;

        public BooksController(StudentDbContext context)
        {
            _context = context;
        }

        private async void LoadBookData()
        {
            StreamReader streamReader = new StreamReader("JsonFiles/BookInfo.json");
            var jsonData = streamReader.ReadToEnd();
            var bookList = JsonConvert.DeserializeObject<List<Book>>(jsonData);

            foreach (var book in bookList)
            {
                _context.Books.Add(book);
            }

            await _context.SaveChangesAsync();
        }

        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            List<Book> bookData = await _context.Books.ToListAsync();
            if (bookData.Count() == 0)
            {
                LoadBookData();
                bookData = await _context.Books.ToListAsync();
            }
            return bookData;


        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBooks(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }

        // PUT: api/Students/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, Book Books)
        {
            if (id != Books.Id)
            {
                return BadRequest();
            }

            _context.Entry(Books).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BooksExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        // POST: api/Students
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Student>> PostBook(Book Books)
        {
            _context.Books.Add(Books);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (BooksExists(Books.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetBooks", new { id = Books.Id }, Books);
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooks(int id)
        {
            var Books = await _context.Books.FindAsync(id);
            if (Books == null)
            {
                return NotFound();
            }

            _context.Books.Remove(Books);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BooksExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}