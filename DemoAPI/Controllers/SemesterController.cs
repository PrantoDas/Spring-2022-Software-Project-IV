using DemoAPI.DbContexts;
using DemoAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace DemoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SemestersController : ControllerBase
    {
        private readonly StudentDbContext _context;

        public SemestersController(StudentDbContext context)
        {
            _context = context;
        }

        private async void LoadSemesterData()
        {
            StreamReader streamReader = new StreamReader("JsonFiles/SemesterInfo.json");
            var jsonData = streamReader.ReadToEnd();
            var semesterList = JsonConvert.DeserializeObject<List<Semester>>(jsonData);

            foreach (var semester in semesterList)
            {
                _context.Semesters.Add(semester);
            }

            await _context.SaveChangesAsync();
        }

        // GET: api/Semesters
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Semester>>> GetSemesters()
        {
            List<Semester> semesterData = await _context.Semesters.ToListAsync();
            if (semesterData.Count() == 0)
            {
                LoadSemesterData();
                semesterData = await _context.Semesters.ToListAsync();
            }
            return semesterData;
            
        }

        // GET: api/Semesters/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Semester>> GetBooks(int id)
        {
            var Semester= await _context.Semesters.FindAsync(id);

            if (Semester == null)
            {
                return NotFound();
            }

            return Semester;
        }

        // PUT: api/Students/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSemester(int id, Semester Semesters)
        {
            if (id != Semesters.Id)
            {
                return BadRequest();
            }

            _context.Entry(Semesters).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SemestersExists(id))
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
        public async Task<ActionResult<Student>> PostBook(Semester Semesters)
        {
            _context.Semesters.Add(Semesters);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (SemestersExists(Semesters.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetSemesters", new { id = Semesters.Id }, Semesters);
        }

        // DELETE: api/Semesters/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooks(int id)
        {
            var Semesters = await _context.Semesters.FindAsync(id);
            if (Semesters == null)
            {
                return NotFound();
            }

            _context.Semesters.Remove(Semesters);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SemestersExists(int id)
        {
            return _context.Semesters.Any(e => e.Id == id);
        }
    }
}