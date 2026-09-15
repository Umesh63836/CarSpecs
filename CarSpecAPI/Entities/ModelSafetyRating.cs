using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class ModelSafetyRating
{
    public int SafetyRatingId { get; set; }

    public int ModelId { get; set; }

    public string Agency { get; set; } = null!;

    public decimal? Rating { get; set; }

    public short? TestYear { get; set; }

    public decimal? AdultOccupantScore { get; set; }

    public decimal? AdultOccupantMaxScore { get; set; }

    public decimal? ChildOccupantScore { get; set; }

    public decimal? ChildOccupantMaxScore { get; set; }

    public string? TestedConfiguration { get; set; }

    public string? SourceUrl { get; set; }

    public bool IsActive { get; set; }

    public virtual Model Model { get; set; } = null!;
}
