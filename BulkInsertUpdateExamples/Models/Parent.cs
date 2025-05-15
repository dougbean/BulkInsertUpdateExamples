using System;
using System.Collections.Generic;

namespace BulkInsertUpdateExamples.Models;

public partial class Parent
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Child> Children { get; set; } = new List<Child>();
}
