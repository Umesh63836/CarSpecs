using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class Admin
{
    public int AdminId { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
