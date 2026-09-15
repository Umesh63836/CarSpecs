namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public sealed class BrochureImportValidationResult
    {
        public bool IsValid => Errors.Count == 0;
        public List<string> Errors { get; } = [];
        public List<string> Warnings { get; } = [];
    }
}
