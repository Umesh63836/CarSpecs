using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class RefreshToken
{
    public int RefreshTokenId { get; set; }

    public int AdminId { get; set; }

    public string TokenHash { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public virtual Admin Admin { get; set; } = null!;
}
