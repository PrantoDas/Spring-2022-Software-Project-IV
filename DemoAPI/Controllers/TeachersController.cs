using DemoAPI.DbContexts;
using DemoAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace DemoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeachersController : ControllerBase
    {
        private readonly StudentDbContext _context;

        public TeachersController(StudentDbContext context)
        {
            _context = context;
        }
        private async void LoadTeacherData()
        {
            StreamReader streamReader = new StreamReader("JsonFiles/TeacherInfo.json");
            var jsonData = streamReader.ReadToEnd();
            var teacherList = JsonConvert.DeserializeObject<List<Teacher>>(jsonData);

            foreach (var teacher in teacherList)
            {
                _context.Teachers.Add(teacher);
            }

            await _context.SaveChangesAsync();
        }
        // GET: api/Teacher
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Teacher>>> GetTeachers()
        {
            List<Teacher> teacherData = await _context.Teachers.ToListAsync();
            if (teacherData.Count() == 0)
            {
                LoadTeacherData();
                teacherData = await _context.Teachers.ToListAsync();
            }
            return teacherData;
        }

        // GET: api/Teachers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Teacher>> GetTeacher(string id)
        {
            var teacher = await _context.Teachers.FindAsync(id);

            if (teacher == null)
            {
                return NotFound();
            }

            return teacher;
        }

        // PUT: api/Teachers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTeacher(string id, Teacher teacher)
        {
            if (id != teacher.Id)
            {
                return BadRequest();
            }

            _context.Entry(teacher).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeacherExists(id))
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

        // POST: api/Teachers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Teacher>> PostTeacher(Teacher teacher)
        {
            _context.Teachers.Add(teacher);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TeacherExists(teacher.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetTeacher", new { id = teacher.Id }, teacher);
        }

        // DELETE: api/Teacehrs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeacher(string id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }

            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Get Teacher An Specific Age 
        [HttpGet("FindByOlderAge")]
        public async Task<ActionResult<IEnumerable<Teacher>>> FindByOlderAge(int age)
        {
            var allTeacher = await _context.Teachers.ToListAsync();
            var selectedTeacher = allTeacher.FindAll(s => s.Age > age);
            return selectedTeacher;
        }

        private bool TeacherExists(string id)
        {
            return _context.Teachers.Any(e => e.Id == id);
        }
    }
}
