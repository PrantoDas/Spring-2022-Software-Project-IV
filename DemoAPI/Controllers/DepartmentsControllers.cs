using DemoAPI.DbContexts;
using DemoAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace DemoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly StudentDbContext _context;

        public DepartmentsController(StudentDbContext context)
        {
            _context = context;
        }
        private async void LoadDepartmentData()
        {
            StreamReader streamReader = new StreamReader("JsonFiles/DepartmentInfo.json");
            var jsonData = streamReader.ReadToEnd();
            var DepartmentList = JsonConvert.DeserializeObject<List<Department>>(jsonData);

            foreach (var department in DepartmentList)
            {
                _context.Departments.Add(department);
            }

            await _context.SaveChangesAsync();
        }
        // GET: api/Departments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartments()
        {
            List<Department> DepartmentData = await _context.Departments.ToListAsync();
            if (DepartmentData.Count() == 0)
            {
                LoadDepartmentData();
                DepartmentData = await _context.Departments.ToListAsync();
            }
            return DepartmentData;
        }

        // GET: api/Departments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Department>> GetDepartments(int id)
        {
            var department = await _context.Departments.FindAsync(id);

            if (department == null)
            {
                return NotFound();
            }

            return department;
        }

        // PUT: api/Students/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDepartment(int id, Department Departments)
        {
            if (id != Departments.Id)
            {
                return BadRequest();
            }

            _context.Entry(Departments).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentsExists(id))
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
        public async Task<ActionResult<Student>> PostDepartment(Department Departments)
        {
            _context.Departments.Add(Departments);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (DepartmentsExists(Departments.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetDepartments", new { id = Departments.Id }, Departments);
        }

        // DELETE: api/Departments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartments(int id)
        {
            var Departments = await _context.Departments.FindAsync(id);
            if (Departments == null)
            {
                return NotFound();
            }

            _context.Departments.Remove(Departments);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DepartmentsExists(int id)
        {
            return _context.Departments.Any(e => e.Id == id);
        }
    }
}