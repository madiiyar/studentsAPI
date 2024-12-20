using System;
using System.Collections.Generic;

namespace studentsAPI.Models;

public partial class GradeLevel
{
    public int Id { get; set; }

    public int Level { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
