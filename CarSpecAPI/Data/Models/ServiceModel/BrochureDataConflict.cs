namespace CarSpecAPI.Data.Models.ServiceModel
{
    public sealed class BrochureDataConflict
    {
        public string Category { get; set; } = "";

        public string Path { get; set; } = "";

        public string? TerraValue { get; set; }

        public string? SolValue { get; set; }

        public string Reason { get; set; } = "";
    }
}
