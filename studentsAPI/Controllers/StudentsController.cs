using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using studentsAPI.Models;

namespace studentsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly StudentContext _context;

        public StudentsController(StudentContext context)
        {
            _context = context;
        }

        // GET: api/Students
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetStudents()
        {
            var students = await _context.Students
                .Include(s => s.Class) // Include Class entity
                .Include(s => s.GradeLevel) // Include GradeLevel entity
                .Include(s => s.Subjects) // Include Subjects from the many-to-many relationship
                .Select(s => new
                {
                    Id = s.Id,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Email = s.Email,
                    BirthDate = s.BirthDate,
                    Address = s.Address,
                    ClassName = s.Class.ClassName, // Get ClassName
                    GradeLevel = s.GradeLevel.Level, // Get Grade Level
                    Subjects = s.Subjects.Select(sub => sub.SubjectName).ToList() // List of subject names
                })
                .ToListAsync();

            return Ok(students);
        }



        // GET: api/Students/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetStudent(int id)
        {
            var student = await _context.Students
                .Include(s => s.Class) // Include Class entity
                .Include(s => s.GradeLevel) // Include GradeLevel entity
                .Include(s => s.Subjects) // Include Subjects from the many-to-many relationship
                .Where(s => s.Id == id)
                .Select(s => new
                {
                    Id = s.Id,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Email = s.Email,
                    BirthDate = s.BirthDate,
                    Address = s.Address,
                    ClassName = s.Class.ClassName, // Get ClassName
                    GradeLevel = s.GradeLevel.Level, // Get Grade Level
                    Subjects = s.Subjects.Select(sub => sub.SubjectName).ToList() // List of subject names
                })
                .FirstOrDefaultAsync();

            if (student == null)
            {
                return NotFound();
            }

            return Ok(student);
        }


        // PUT: api/Students/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudent(int id, StudentDto studentDto)
        {
            // Step 1: Validate if the ID in the URL matches the ID in the DTO
            if (id != studentDto.Id)
            {
                return BadRequest("The provided student ID does not match the ID in the URL.");
            }

            // Step 2: Resolve ClassId by ClassName
            var classEntity = await _context.Classes.FirstOrDefaultAsync(c => c.ClassName == studentDto.ClassName);
            if (classEntity == null)
            {
                return BadRequest($"Class '{studentDto.ClassName}' does not exist.");
            }

            // Step 3: Resolve GradeLevelId by Level
            var gradeLevelEntity = await _context.GradeLevels.FirstOrDefaultAsync(g => g.Level == studentDto.GradeLevel);
            if (gradeLevelEntity == null)
            {
                return BadRequest($"Grade level '{studentDto.GradeLevel}' does not exist.");
            }

            // Step 4: Resolve SubjectIds by SubjectNames
            var subjectEntities = await _context.Subjects
                .Where(s => studentDto.Subjects.Contains(s.SubjectName))
                .ToListAsync();
            if (subjectEntities.Count != studentDto.Subjects.Count)
            {
                var missingSubjects = studentDto.Subjects.Except(subjectEntities.Select(s => s.SubjectName));
                return BadRequest($"The following subjects do not exist: {string.Join(", ", missingSubjects)}");
            }

            // Step 5: Retrieve the existing student
            var student = await _context.Students
                .Include(s => s.Subjects)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null)
            {
                return NotFound($"Student with ID {id} not found.");
            }

            // Step 6: Update the student details
            student.FirstName = studentDto.FirstName;
            student.LastName = studentDto.LastName;
            student.Email = studentDto.Email;
            student.BirthDate = studentDto.BirthDate;
            student.Address = studentDto.Address;
            student.ClassId = classEntity.Id;
            student.GradeLevelId = gradeLevelEntity.Id;

            // Step 7: Update Subjects (many-to-many relationship)
            // Clear existing subjects and add the new ones
            student.Subjects.Clear();
            foreach (var subject in subjectEntities)
            {
                student.Subjects.Add(subject);
            }

            // Step 8: Save changes
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(id))
                {
                    return NotFound($"Student with ID {id} no longer exists.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // Helper method to check if a student exists
        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.Id == id);
        }



        [HttpPost]
        public async Task<ActionResult> PostStudent(StudentDto studentDto)
        {
            // Step 1: Check for duplicate email
            var existingStudent = await _context.Students.FirstOrDefaultAsync(s => s.Email == studentDto.Email);
            if (existingStudent != null)
            {
                return BadRequest($"A student with the email '{studentDto.Email}' already exists.");
            }

            // Step 2: Resolve ClassId by ClassName
            var classEntity = await _context.Classes.FirstOrDefaultAsync(c => c.ClassName == studentDto.ClassName);
            if (classEntity == null)
            {
                return BadRequest($"Class '{studentDto.ClassName}' does not exist.");
            }

            // Step 3: Resolve GradeLevelId by Level
            var gradeLevelEntity = await _context.GradeLevels.FirstOrDefaultAsync(g => g.Level == studentDto.GradeLevel);
            if (gradeLevelEntity == null)
            {
                return BadRequest($"Grade level '{studentDto.GradeLevel}' does not exist.");
            }

            // Step 4: Resolve SubjectIds by SubjectNames
            var subjectEntities = await _context.Subjects
                .Where(s => studentDto.Subjects.Contains(s.SubjectName))
                .ToListAsync();
            if (subjectEntities.Count != studentDto.Subjects.Count)
            {
                var missingSubjects = studentDto.Subjects.Except(subjectEntities.Select(s => s.SubjectName));
                return BadRequest($"The following subjects do not exist: {string.Join(", ", missingSubjects)}");
            }

            // Step 5: Create the Student
            var student = new Student
            {
                FirstName = studentDto.FirstName,
                LastName = studentDto.LastName,
                Email = studentDto.Email,
                BirthDate = studentDto.BirthDate,
                Address = studentDto.Address,
                ClassId = classEntity.Id,
                GradeLevelId = gradeLevelEntity.Id,
                Subjects = subjectEntities // Directly assign resolved subjects
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            // Step 6: Return the created resource
            return CreatedAtAction("GetStudent", new { id = student.Id }, studentDto);
        }

        // GET: api/Students/search
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<StudentDto>>> SearchStudents(
            [FromQuery] string? firstName,
            [FromQuery] string? lastName,
            [FromQuery] string? className,
            [FromQuery] int? gradeLevel,
            [FromQuery] string? subject)
        {
            // Start with the base query
            var query = _context.Students
                .Include(s => s.Class)
                .Include(s => s.GradeLevel)
                .Include(s => s.Subjects)
                .AsQueryable();

            // Apply filters based on query parameters
            if (!string.IsNullOrEmpty(firstName))
            {
                query = query.Where(s => s.FirstName.Contains(firstName));
            }

            if (!string.IsNullOrEmpty(lastName))
            {
                query = query.Where(s => s.LastName.Contains(lastName));
            }

            if (!string.IsNullOrEmpty(className))
            {
                query = query.Where(s => s.Class.ClassName == className);
            }

            if (gradeLevel.HasValue)
            {
                query = query.Where(s => s.GradeLevel.Level == gradeLevel.Value);
            }

            if (!string.IsNullOrEmpty(subject))
            {
                query = query.Where(s => s.Subjects.Any(sub => sub.SubjectName == subject));
            }

            // Select the results into the DTO format
            var students = await query
                .Select(s => new StudentDto
                {
                    Id = s.Id,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Email = s.Email,
                    BirthDate = s.BirthDate,
                    Address = s.Address,
                    ClassName = s.Class.ClassName,
                    GradeLevel = s.GradeLevel.Level,
                    Subjects = s.Subjects.Select(sub => sub.SubjectName).ToList()
                })
                .ToListAsync();

            return Ok(students);
        }




        // DELETE: api/Students/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            // Step 1: Retrieve the student with their associated subjects
            var student = await _context.Students
                .Include(s => s.Subjects) // Include the many-to-many relationship
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null)
            {
                return NotFound($"Student with ID {id} not found.");
            }

            // Step 2: Remove the associated subjects manually
            student.Subjects.Clear();
            _context.Students.Remove(student);

            // Step 3: Save changes to the database
            await _context.SaveChangesAsync();

            return NoContent();
        }


    }
}
