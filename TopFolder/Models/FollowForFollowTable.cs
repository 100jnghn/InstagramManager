using System;
using System.Collections.Generic;

namespace InstagramManager.Models;

public partial class FollowForFollowTable
{
    public string Id { get; set; } = null!;

    public string Address { get; set; } = null!;

    public int Date { get; set; }

    public string? Description { get; set; }
}
