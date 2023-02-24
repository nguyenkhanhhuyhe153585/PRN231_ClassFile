using System;
using System.Collections.Generic;

namespace ClassFileBackEnd.Models;

public partial class File
{
    public int Id { get; set; }

    public int? PostId { get; set; }

    public string? FileName { get; set; }

    public string? FileType { get; set; }

    public virtual Post? Post { get; set; }
}
