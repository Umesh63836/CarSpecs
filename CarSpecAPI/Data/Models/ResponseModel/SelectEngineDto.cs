namespace CarSpecAPI.Data.Models.ResponseModel
{
    public class SelectEngineDto
    {
            public int EngineId { get; set; }
            public string EngineName { get; set; } = null!;
            public bool IsTurbocharged { get; set; }
            public string EmissionStandard { get; set; } = null!;
    }
}
