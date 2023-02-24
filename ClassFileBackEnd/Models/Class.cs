using System;
using System.Collections.Generic;

namespace ClassFileBackEnd.Models;

public partial class Class
{
    public int Id { get; set; }

    public string? ClassName { get; set; }

    public int? TeacherAccountId { get; set; }

    public virtual ICollection<Post> Posts { get; } = new List<Post>();

    public virtual ICollection<Account> StudentAccounts { get; } = new List<Account>();
}
