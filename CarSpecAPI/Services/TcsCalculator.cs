namespace CarSpecAPI.Services
{
    public class TcsCalculator : ITcsCalculator
    {
        public decimal Calculate(decimal exShowroomPrice)
        {
            if (exShowroomPrice <= 1000000)
                return 0;

            return Math.Round(exShowroomPrice * 0.01m);
        }
    }
}
