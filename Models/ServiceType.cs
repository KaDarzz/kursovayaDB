using System;
using System.Collections.Generic;

namespace TransportCompanyWithAuthorize.Models;

public partial class ServiceType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Services> Services { get; set; } = new List<Services>();
}
