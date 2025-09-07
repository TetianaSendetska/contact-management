using System;
using System.Collections.Generic;

namespace contact_manager.Models;

public partial class Contact
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateOnly? DateOfBirth { get; set; }

    public bool? Married { get; set; }

    public string? Phone { get; set; }

    public decimal? Salary { get; set; }
}
