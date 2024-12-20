using System;
using System.Collections.Generic;

namespace studentsAPI.Models;

public partial class Subject
{
    public int Id { get; set; }

    public string SubjectName { get; set; } = null!;

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
