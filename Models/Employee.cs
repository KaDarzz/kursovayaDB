using System;
using System.Collections.Generic;

namespace TransportCompanyWithAuthorize.Models;

public partial class Employee
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string? Position { get; set; }

    public string? Phone { get; set; }

    public virtual ICollection<EmployeeSchedule> EmployeeSchedules { get; set; } = new List<EmployeeSchedule>();

    public virtual ICollection<PerformedService> PerformedServices { get; set; } = new List<PerformedService>();
}
