using System;
using System.Collections.Generic;

namespace BulkInsertUpdateExamples.Models;

public partial class Child
{
    public int Id { get; set; }

    public int ParentId { get; set; }

    public string Name { get; set; } = null!;

    public virtual Parent Parent { get; set; } = null!;
}
