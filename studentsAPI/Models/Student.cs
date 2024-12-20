using System;
using System.Collections.Generic;

namespace studentsAPI.Models;

public partial class Student
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateTime BirthDate { get; set; }

    public string? Address { get; set; }

    public int ClassId { get; set; }

    public int GradeLevelId { get; set; }

    public virtual Class Class { get; set; } = null!;

    public virtual GradeLevel GradeLevel { get; set; } = null!;

    public virtual ICollection<Subject> Subjects { get; set; } = new List<Subject>();
}

public class StudentDto
{
    public int Id { get; set; } // Include the student ID for PUT requests
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTime BirthDate { get; set; }
    public string? Address { get; set; }
    public string ClassName { get; set; } = null!; // Provided as name, not ID
    public int GradeLevel { get; set; } // Provided as level, not ID
    public List<string> Subjects { get; set; } = new(); // Provided as a list of names
}

