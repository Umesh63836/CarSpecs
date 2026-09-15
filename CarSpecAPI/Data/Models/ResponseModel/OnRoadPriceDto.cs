using CarSpecAPI.Data.Models.ServiceModel;
using CarSpecAPI.Services;

namespace CarSpecAPI.Data.Models.ResponseModel
{
    public class OnRoadPriceDto
    {
        public int VariantId { get; set; }
        public int StateId { get; set; }
        public decimal ExShowroomPrice { get; set; }
        public RegistrationParameters Registration { get; set; } = new();
        public decimal? Insurance { get; set; } = new();
        public decimal? Tcs { get; set; }
        public decimal Fastag { get; set; }
        public decimal TotalOnRoadPrice { get; set; }
    }
}
