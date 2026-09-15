using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class SubDistrict
{
    public int SubDistrictCode { get; set; }

    public int DistrictCode { get; set; }

    public int? SubDistrictVersion { get; set; }

    public string SubDistrictName { get; set; } = null!;

    public virtual District DistrictCodeNavigation { get; set; } = null!;
}
