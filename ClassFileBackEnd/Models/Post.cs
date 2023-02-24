using System;
using System.Collections.Generic;

namespace ClassFileBackEnd.Models;

public partial class Post
{
    public int Id { get; set; }

    public int? ClassId { get; set; }

    public string? Title { get; set; }

    public int? PostedAccountId { get; set; }

    public DateTime? DateCreated { get; set; }

    public virtual Class? Class { get; set; }

    public virtual ICollection<File> Files { get; } = new List<File>();
}
