namespace CarSpecAPI.Data.Models.DataExtractionModel
{
    public sealed class StageValidationResult
    {
        public bool IsValid => Errors.Count == 0;

        public List<string> Errors { get; } = [];
    }
}
