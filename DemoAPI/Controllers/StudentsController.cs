﻿using DemoAPI.DbContexts;
using DemoAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net;

namespace DemoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly StudentDbContext _context;

        public StudentsController(StudentDbContext context)
        {
            _context = context;
        }
        private async void LoadStudentData()
        {
            StreamReader streamReader = new StreamReader("JsonFiles/StudentInfo.json");
            var jsonData = streamReader.ReadToEnd();
            var studentList = JsonConvert.DeserializeObject<List<Student>>(jsonData);

            foreach (var student in studentList)
            {
                _context.Students.Add(student);
            }

            await _context.SaveChangesAsync();
        }

        // GET: api/Students
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
        {
            List<Student> studentData = await _context.Students.ToListAsync();
            if (studentData.Count() == 0)
            {
                LoadStudentData();
                studentData = await _context.Students.ToListAsync();
            }
            return studentData;
        }

        // GET: api/Students/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetStudent(string id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            return student;
        }

        // PUT: api/Students/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudent(string id, Student student)
        {
            if (id != student.Id)
            {
                return BadRequest();
            }

            _context.Entry(student).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(id))
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
        public async Task<ActionResult<Student>> PostStudent(Student student)
        {
            _context.Students.Add(student);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (StudentExists(student.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetStudent", new { id = student.Id }, student);
        }

        // DELETE: api/Students/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(string id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Search Students by name
        [HttpGet("GetStudentsByName")]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudentsByName(string studentName)
        {
            studentName = studentName.ToLower().Trim();
            var allStudent = await _context.Students.ToListAsync();
            var selectedStudent = allStudent.FindAll(s => s.Name.ToLower().Contains(studentName));
            return selectedStudent;
        }

        // GET: api/Students/5
        [HttpGet("cgpa/{cgpa}")]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudents(double cgpa)
        {
            List<Student> allstudent = await _context.Students.ToListAsync();
            List<Student> selectedStudent = allstudent.FindAll(x => x.CGPA >= cgpa);
            return selectedStudent;
        }

        // GET: api/Get Students An Specific Age Range
        [HttpGet("GetStudentsByAnSpecificAgeRange")]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudentsByAnSpecificAgeRange(int startAge,int endAge)
        {
            var allStudent = await _context.Students.ToListAsync();
            var selectedStudent = allStudent.FindAll(s => s.Age >= startAge && s.Age <= endAge);
            return selectedStudent;
        }

        private bool StudentExists(string id)
        {
            return _context.Students.Any(e => e.Id == id);
        }

        // GET: api/Students in specific section
        [HttpGet("GetStudentsBySection")]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudentsBySection(string section)
        {
            section = section.ToLower().Trim();
            var totalSection = await _context.Students.ToListAsync();
            var sameSection = totalSection.FindAll(s => s.Section.ToLower() == section);

            if (sameSection.Count() == 0)
            {
                return NotFound();
            }

            return sameSection;
        }
    }
}
