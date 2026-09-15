using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class District
{
    public int DistrictCode { get; set; }

    public short StateCode { get; set; }

    public string DistrictName { get; set; } = null!;

    public virtual State StateCodeNavigation { get; set; } = null!;

    public virtual ICollection<SubDistrict> SubDistricts { get; set; } = new List<SubDistrict>();
}
