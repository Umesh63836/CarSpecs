using CarSpecAPI.Data.Models.ServiceModel;

namespace CarSpecAPI.Services
{
    public class RegistrationCalculator : IRegistrationCalculator
    {
        private const decimal RegistrationFee = 600m;
        private const decimal HsrpCharge = 800m;

        public RegistrationParameters Calculate(int stateCode, VehicleInfo vehicle)
        {
            return stateCode switch
            {
                // STATES
                28 => CalculateAndhraPradesh(vehicle),
                12 => CalculateArunachalPradesh(vehicle),
                18 => CalculateAssam(vehicle),
                10 => CalculateBihar(vehicle),
                22 => CalculateChhattisgarh(vehicle),
                30 => CalculateGoa(vehicle),
                24 => CalculateGujarat(vehicle),
                6 => CalculateHaryana(vehicle),
                2 => CalculateHimachalPradesh(vehicle),
                20 => CalculateJharkhand(vehicle),
                29 => CalculateKarnataka(vehicle),
                32 => CalculateKerala(vehicle),
                23 => CalculateMadhyaPradesh(vehicle),
                27 => CalculateMaharashtra(vehicle),
                14 => CalculateManipur(vehicle),
                17 => CalculateMeghalaya(vehicle),
                15 => CalculateMizoram(vehicle),
                13 => CalculateNagaland(vehicle),
                21 => CalculateOdisha(vehicle),
                3 => CalculatePunjab(vehicle),
                8 => CalculateRajasthan(vehicle),
                11 => CalculateSikkim(vehicle),
                33 => CalculateTamilNadu(vehicle),
                36 => CalculateTelangana(vehicle),
                16 => CalculateTripura(vehicle),
                9 => CalculateUttarPradesh(vehicle),
                5 => CalculateUttarakhand(vehicle),
                19 => CalculateWestBengal(vehicle),

                // UNION TERRITORIES
                35 => CalculateAndamanNicobar(vehicle),
                4 => CalculateChandigarh(vehicle),
                38 => CalculateDadraNagarHaveliDamanDiu(vehicle),
                7 => CalculateDelhi(vehicle),
                1 => CalculateJammuKashmir(vehicle),
                37 => CalculateLadakh(vehicle),
                31 => CalculateLakshadweep(vehicle),
                34 => CalculatePuducherry(vehicle),

                _ => throw new NotSupportedException(
                    $"Registration calculation is not configured for StateCode {stateCode}.")
            };
        }


        private RegistrationParameters CreateResult(
            decimal roadTax,
            decimal roadTaxCess = 0m,
            decimal infrastructureCess = 0m,
            decimal greenTax = 0m,
            decimal otherCharges = 0m)
        {
            return new RegistrationParameters
            {
                RoadTax = Math.Round(roadTax),
                RoadTaxCess = Math.Round(roadTaxCess),
                InfrastructureCess = Math.Round(infrastructureCess),
                RegistrationFee = RegistrationFee,
                HsrpCharge = HsrpCharge,
                GreenTax = Math.Round(greenTax),
                OtherCharges = Math.Round(otherCharges)
            };
        }


        private decimal CalculateSlabTax(
            decimal price,
            params (decimal MaxPrice, decimal Rate)[] slabs)
        {
            foreach (var slab in slabs)
            {
                if (price <= slab.MaxPrice)
                    return price * slab.Rate;
            }

            return 0m;
        }


        
        // 1. ANDHRA PRADESH - 28
        

        private RegistrationParameters CalculateAndhraPradesh(
            VehicleInfo vehicle)
        {
            if (vehicle.IsElectric)
                return CreateResult(0);

            var tax = CalculateSlabTax(
                vehicle.ExShowroomPrice,

                (500000m, 0.13m),       // ₹5 lakh
                (1000000m, 0.14m),      // ₹10 lakh
                (2000000m, 0.17m),      // ₹20 lakh
                (decimal.MaxValue, 0.18m)
            );

            return CreateResult(tax);
        }


        
        // 2. ARUNACHAL PRADESH - 12
        

        private RegistrationParameters CalculateArunachalPradesh(
            VehicleInfo vehicle)
        {
            var price = vehicle.ExShowroomPrice;

            decimal rate;

            if (price < 300000m)             // ₹3 lakh
                rate = 0m;
            else if (price < 500000m)        // ₹5 lakh
                rate = 0.027m;
            else if (price < 1000000m)       // ₹10 lakh
                rate = 0.03m;
            else if (price < 1500000m)       // ₹15 lakh
                rate = 0.035m;
            else if (price < 1800000m)       // ₹18 lakh
                rate = 0.04m;
            else if (price < 2000000m)       // ₹20 lakh
                rate = 0.045m;
            else
                rate = 0.065m;

            return CreateResult(price * rate);
        }


        
        // 3. ASSAM - 18
        

        private RegistrationParameters CalculateAssam(
            VehicleInfo vehicle)
        {
            if (vehicle.IsElectric)
                return CreateResult(0);

            var tax = CalculateSlabTax(
                vehicle.ExShowroomPrice,

                (400000m, 0.05m),        // ₹4 lakh
                (600000m, 0.06m),        // ₹6 lakh
                (1200000m, 0.07m),       // ₹12 lakh
                (1500000m, 0.075m),      // ₹15 lakh
                (2000000m, 0.09m),       // ₹20 lakh
                (decimal.MaxValue, 0.12m)
            );

            return CreateResult(tax);
        }


        
        // 4. BIHAR - 10
        

        private RegistrationParameters CalculateBihar(
            VehicleInfo vehicle)
        {
            if (vehicle.IsElectric)
                return CreateResult(0);

            var tax = CalculateSlabTax(
                vehicle.ExShowroomPrice,

                (800000m, 0.09m),        // ₹8 lakh
                (1500000m, 0.10m),       // ₹15 lakh
                (decimal.MaxValue, 0.12m)
            );

            return CreateResult(tax);
        }


        
        // 5. CHHATTISGARH - 22
        

        private RegistrationParameters CalculateChhattisgarh(
            VehicleInfo vehicle)
        {
            if (vehicle.IsElectric)
                return CreateResult(0);

            var tax = vehicle.ExShowroomPrice < 500000m
                ? vehicle.ExShowroomPrice * 0.05m
                : vehicle.ExShowroomPrice * 0.06m;

            return CreateResult(tax);
        }


        
        // 6. GOA - 30
        

        private RegistrationParameters CalculateGoa(
            VehicleInfo vehicle)
        {
            var price = vehicle.ExShowroomPrice;

            decimal tax;

            if (price <= 600000m)             // ₹6 lakh
                tax = price * 0.09m;
            else if (price <= 1500000m)      // ₹15 lakh
                tax = price * 0.11m;
            else if (price <= 3500000m)      // ₹35 lakh)
                tax = price * 0.13m;
            else
                tax = price * 0.14m;

            decimal infrastructureCess = 0m;

            if (price > 1000000m && price <= 2000000m)
                infrastructureCess = 15000m;
            else if (price <= 4000000m)
                infrastructureCess = 50000m;
            else if (price <= 6000000m)
                infrastructureCess = 100000m;
            else if (price > 6000000m)
                infrastructureCess = 125000m;

            return CreateResult(
                tax,
                infrastructureCess: infrastructureCess);
        }


        
        // 7. GUJARAT - 24
        

        private RegistrationParameters CalculateGujarat(
            VehicleInfo vehicle)
        {
            var basePrice = vehicle.ExShowroomPrice;

            decimal tax = basePrice * 0.06m;

            if (vehicle.IsElectric)
                tax = basePrice * 0.01m;

            return CreateResult(tax);
        }


        
        // 8. HARYANA - 6
        

        private RegistrationParameters CalculateHaryana(
            VehicleInfo vehicle)
        {
            if (vehicle.IsElectric)
                return CreateResult(0);

            var tax = CalculateSlabTax(
                vehicle.ExShowroomPrice,

                (600000m, 0.05m),        // ₹6 lakh
                (2000000m, 0.08m),       // ₹20 lakh
                (decimal.MaxValue, 0.10m)
            );

            // 20% rebate for CNG
            if (vehicle.IsCng)
                tax *= 0.80m;

            return CreateResult(tax);
        }


        
        // 9. HIMACHAL PRADESH - 2
        

        private RegistrationParameters CalculateHimachalPradesh(
            VehicleInfo vehicle)
        {
            var rate = vehicle.ExShowroomPrice <= 1500000m
                ? 0.06m
                : 0.07m;

            var tax = vehicle.ExShowroomPrice * rate;

            return CreateResult(tax);
        }


        
        // 10. JHARKHAND - 20
        

        private RegistrationParameters CalculateJharkhand(
            VehicleInfo vehicle)
        {
            var basePrice = vehicle.ExShowroomPrice;

            var tax = basePrice < 700000m
                ? basePrice * 0.07m
                : basePrice * 0.09m;

            return CreateResult(tax);
        }


        
        // 11. KARNATAKA - 29
        

        private RegistrationParameters CalculateKarnataka(
            VehicleInfo vehicle)
        {
            if (vehicle.IsElectric)
                return CreateResult(0);

            var roadTax = CalculateSlabTax(
                vehicle.ExShowroomPrice,

                (500000m, 0.13m),        // ₹5 lakh
                (1000000m, 0.14m),       // ₹10 lakh
                (2000000m, 0.17m),       // ₹20 lakh
                (decimal.MaxValue, 0.18m)
            );

            // 11% of road tax
            var cess = roadTax * 0.11m;

            // Welfare cess
            var welfareCess = 1000m;

            return CreateResult(
                roadTax,
                roadTaxCess: cess,
                otherCharges: welfareCess);
        }


        
        // 12. KERALA - 32
        

        private RegistrationParameters CalculateKerala(
            VehicleInfo vehicle)
        {
            if (vehicle.IsElectric)
                return CreateResult(0);

            var tax = CalculateSlabTax(
                vehicle.ExShowroomPrice,

                (500000m, 0.10m),        // ₹5 lakh
                (1000000m, 0.13m),       // ₹10 lakh
                (1500000m, 0.15m),       // ₹15 lakh
                (2000000m, 0.17m),       // ₹20 lakh
                (decimal.MaxValue, 0.22m)
            );

            return CreateResult(
                tax,
                otherCharges: 200m);
        }


        
        // 13. MADHYA PRADESH - 23
        

        private RegistrationParameters CalculateMadhyaPradesh(
            VehicleInfo vehicle)
        {
            decimal rate;

            if (vehicle.IsDiesel)
            {
                if (vehicle.ExShowroomPrice <= 1000000m)
                    rate = 0.10m;
                else if (vehicle.ExShowroomPrice <= 2000000m)
                    rate = 0.12m;
                else
                    rate = 0.16m;
            }
            else
            {
                if (vehicle.ExShowroomPrice <= 1000000m)
                    rate = 0.08m;
                else if (vehicle.ExShowroomPrice <= 2000000m)
                    rate = 0.10m;
                else
                    rate = 0.14m;
            }

            return CreateResult(
                vehicle.ExShowroomPrice * rate);
        }


        
        // 14. MAHARASHTRA - 27
        

        private RegistrationParameters CalculateMaharashtra(
            VehicleInfo vehicle)
        {
            if (vehicle.IsElectric)
                return CreateResult(0);

            decimal rate;

            if (vehicle.IsDiesel)
            {
                rate = vehicle.ExShowroomPrice <= 1000000m
                    ? 0.13m
                    : vehicle.ExShowroomPrice <= 2000000m
                        ? 0.14m
                        : 0.15m;
            }
            else if (vehicle.IsCng)
            {
                rate = vehicle.ExShowroomPrice <= 1000000m
                    ? 0.08m
                    : vehicle.ExShowroomPrice <= 2000000m
                        ? 0.08m
                        : 0.09m;
            }
            else
            {
                rate = vehicle.ExShowroomPrice <= 1000000m
                    ? 0.11m
                    : vehicle.ExShowroomPrice <= 2000000m
                        ? 0.12m
                        : 0.13m;
            }

            var tax = vehicle.ExShowroomPrice * rate;

            return CreateResult(tax);
        }


        
        // 15. MANIPUR - 14
        

        private RegistrationParameters CalculateManipur(
            VehicleInfo vehicle)
        {
            decimal rate;

            if (vehicle.ExShowroomPrice < 500000m)
                rate = 0.05m;
            else if (vehicle.ExShowroomPrice < 1000000m)
                rate = 0.06m;
            else if (vehicle.ExShowroomPrice < 2000000m)
                rate = 0.07m;
            else
                rate = 0.08m;

            return CreateResult(
                vehicle.ExShowroomPrice * rate);
        }


        
        // 16. MEGHALAYA - 17
        

        private RegistrationParameters CalculateMeghalaya(
            VehicleInfo vehicle)
        {
            var tax = CalculateSlabTax(
                vehicle.ExShowroomPrice,

                (300000m, 0.04m),        // ₹3 lakh
                (1500000m, 0.06m),       // ₹15 lakh
                (2000000m, 0.08m),       // ₹20 lakh
                (decimal.MaxValue, 0.10m)
            );

            return CreateResult(tax);
        }


        
        // 17. MIZORAM - 15
        

        private RegistrationParameters CalculateMizoram(
            VehicleInfo vehicle)
        {
            // 6% of vehicle cost excluding GST.
            var basePrice = vehicle.ExShowroomPrice;

            var tax = basePrice * 0.06m;

            return CreateResult(tax);
        }


        
        // 18. NAGALAND - 13
        

        private RegistrationParameters CalculateNagaland(
            VehicleInfo vehicle)
        {
            var tax = vehicle.ExShowroomPrice * 0.06m;

            return CreateResult(tax);
        }


        
        // 19. ODISHA - 21
        

        private RegistrationParameters CalculateOdisha(
            VehicleInfo vehicle)
        {
            if (vehicle.IsElectric)
                return CreateResult(0);

            var price = vehicle.ExShowroomPrice;

            decimal rate;

            if (price < 500000m)
                rate = 0.06m;
            else if (price <= 1000000m)
                rate = 0.08m;
            else
                rate = 0.10m;

            return CreateResult(price * rate);
        }


        
        // 20. PUNJAB - 3
        

        private RegistrationParameters CalculatePunjab(
            VehicleInfo vehicle)
        {
            if (vehicle.IsElectric)
                return CreateResult(0);

            var tax = CalculateSlabTax(
                vehicle.ExShowroomPrice,

                (1500000m, 0.095m),       // ₹15 lakh
                (2500000m, 0.12m),        // ₹25 lakh
                (decimal.MaxValue, 0.13m)
            );

            var cowCess = 1000m;

            return CreateResult(
                tax,
                otherCharges: cowCess);
        }


        
        // 21. RAJASTHAN - 8
        

        private RegistrationParameters CalculateRajasthan(
            VehicleInfo vehicle)
        {
            decimal roadTax;

            var cc = vehicle.EngineCc;

            if (vehicle.IsDiesel)
            {
                if (cc <= 800)
                    roadTax = vehicle.ExShowroomPrice * 0.08m;
                else if (cc <= 1200)
                    roadTax = vehicle.ExShowroomPrice * 0.11m;
                else
                    roadTax = vehicle.ExShowroomPrice * 0.12m;
            }
            else
            {
                if (cc <= 800)
                    roadTax = vehicle.ExShowroomPrice * 0.06m;
                else if (cc <= 1200)
                    roadTax = vehicle.ExShowroomPrice * 0.09m;
                else
                    roadTax = vehicle.ExShowroomPrice * 0.10m;
            }

            // 12.5% surcharge
            var surcharge = roadTax * 0.125m;

            decimal greenTax = 0m;

            if (vehicle.IsDiesel)
            {
                if (cc <= 1500)
                    greenTax = 2500m;
                else if (cc <= 2000)
                    greenTax = 3500m;
                else if (vehicle.SeatingCapacity > 5)
                    greenTax = 7500m;
                else
                    greenTax = 5000m;
            }

            return CreateResult(
                roadTax,
                roadTaxCess: surcharge,
                greenTax: greenTax);
        }


        
        // 22. SIKKIM - 11
        

        private RegistrationParameters CalculateSikkim(
            VehicleInfo vehicle)
        {
            decimal rate;

            if (vehicle.IsElectric)
            {
                rate = 0.01m;
            }
            else if (vehicle.ExShowroomPrice <= 200000m)       // ₹2 lakh
            {
                rate = 0.01m;
            }
            else if (vehicle.ExShowroomPrice <= 20000000m)    // ₹2 crore
            {
                rate = 0.04m;
            }
            else
            {
                rate = 0.05m;
            }

            return CreateResult(
                vehicle.ExShowroomPrice * rate);
        }


        
        // 23. TAMIL NADU - 33
        

        private RegistrationParameters CalculateTamilNadu(
            VehicleInfo vehicle)
        {
            if (vehicle.IsElectric)
                return CreateResult(0);

            var tax = CalculateSlabTax(
                vehicle.ExShowroomPrice,

                (500000m, 0.12m),        // ₹5 lakh
                (1000000m, 0.13m),       // ₹10 lakh
                (2000000m, 0.18m),       // ₹20 lakh
                (decimal.MaxValue, 0.20m)
            );

            var roadSafetyTax = 2250m;

            return CreateResult(
                tax,
                otherCharges: roadSafetyTax);
        }


        
        // 24. TELANGANA - 36
        

        private RegistrationParameters CalculateTelangana(
            VehicleInfo vehicle)
        {
            if (vehicle.IsElectric)
                return CreateResult(0);

            var price = vehicle.ExShowroomPrice;

            decimal rate;

            if (price < 500000m)
                rate = 0.13m;
            else if (price <= 1000000m)
                rate = 0.14m;
            else if (price <= 2000000m)
                rate = 0.18m;
            else if (price <= 5000000m)
                rate = 0.20m;
            else
                rate = 0.21m;

            var tax = price * rate;

            return CreateResult(
                tax,
                otherCharges: 5000m);
        }


        
        // 25. TRIPURA - 16
        

        private RegistrationParameters CalculateTripura(
            VehicleInfo vehicle)
        {
            var price = vehicle.ExShowroomPrice;

            decimal tax;

            if (price <= 300000m)             // ₹3 lakh
                tax = 4100m;
            else if (price <= 500000m)        // ₹5 lakh
                tax = 5500m;
            else if (price <= 1000000m)       // ₹10 lakh
                tax = 6850m;
            else if (price <= 1500000m)       // ₹15 lakh
                tax = 7550m;
            else
                tax = 8250m;

            return CreateResult(tax);
        }


        
        // 26. UTTAR PRADESH - 9
        

        private RegistrationParameters CalculateUttarPradesh(
            VehicleInfo vehicle)
        {
            if (vehicle.IsElectric)
                return CreateResult(0);

            var tax = vehicle.ExShowroomPrice <= 1000000m
                ? vehicle.ExShowroomPrice * 0.09m
                : vehicle.ExShowroomPrice * 0.11m;

            return CreateResult(tax);
        }


        
        // 27. UTTARAKHAND - 5
        

        private RegistrationParameters CalculateUttarakhand(
            VehicleInfo vehicle)
        {
            var price = vehicle.ExShowroomPrice;

            decimal rate;

            if (price <= 500000m)             // ₹5 lakh
                rate = 0.08m;
            else if (price <= 1000000m)      // ₹10 lakh
                rate = 0.09m;
            else
                rate = 0.10m;

            var tax = price * rate;

            decimal greenTax = 0m;

            if (vehicle.IsPetrol)
                greenTax = 1500m;
            else if (vehicle.IsDiesel)
                greenTax = 3000m;

            return CreateResult(
                tax,
                greenTax: greenTax);
        }


        
        // 28. WEST BENGAL - 19
        

        private RegistrationParameters CalculateWestBengal(
            VehicleInfo vehicle)
        {
            var price = vehicle.ExShowroomPrice;
            var cc = vehicle.EngineCc;

            decimal percentageTax;
            decimal minimumTax;

            if (cc <= 800)
            {
                percentageTax = price * 0.10m;
                minimumTax = 40000m;
            }
            else if (cc <= 1490)
            {
                percentageTax = price * 0.10m;
                minimumTax = 55000m;
            }
            else if (cc <= 1999)
            {
                percentageTax = price * 0.10m;
                minimumTax = 80000m;
            }
            else
            {
                percentageTax = price * 0.10m;
                minimumTax = 1m;
            }

            var tax = Math.Max(
                percentageTax,
                minimumTax);

            return CreateResult(tax);
        }


        
        // 29. ANDAMAN & NICOBAR - 35
        

        private RegistrationParameters CalculateAndamanNicobar(
            VehicleInfo vehicle)
        {
            decimal rate;

            if (vehicle.IsElectric)
                rate = 0.02m;
            else
                rate = 0.06m;

            return CreateResult(
                vehicle.ExShowroomPrice * rate);
        }


        
        // 30. CHANDIGARH - 4
        

        private RegistrationParameters CalculateChandigarh(
            VehicleInfo vehicle)
        {
            var price = vehicle.ExShowroomPrice;

            decimal rate;

            if (price <= 1500000m)             // ₹15 lakh
                rate = 0.10m;
            else
                rate = 0.12m;

            if (vehicle.IsElectric)
                return CreateResult(0);

            var tax = price * rate;

            return CreateResult(
                tax,
                otherCharges: 1000m);
        }


        
        // 31. DNH & DAMAN & DIU - 38
        

        private RegistrationParameters
            CalculateDadraNagarHaveliDamanDiu(
                VehicleInfo vehicle)
        {
            var price = vehicle.ExShowroomPrice;

            decimal roadTax;

            if (price <= 1_000_000m)
            {
                roadTax = price * 0.025m;
            }
            else
            {
                roadTax = price * 0.03m;
            }

            return CreateResult(roadTax);
        }


        
        // 32. DELHI - 7
        

        private RegistrationParameters CalculateDelhi(
            VehicleInfo vehicle)
        {
            if (vehicle.IsElectric)
                return CreateResult(0);

            decimal rate;

            if (vehicle.ExShowroomPrice <= 600000m)       // ₹6 lakh
            {
                rate = vehicle.IsDiesel
                    ? 0.05m
                    : 0.04m;
            }
            else if (vehicle.ExShowroomPrice <= 1000000m) // ₹10 lakh
            {
                rate = vehicle.IsDiesel
                    ? 0.0875m
                    : 0.07m;
            }
            else
            {
                rate = vehicle.IsDiesel
                    ? 0.125m
                    : 0.10m;
            }

            var roadTax =
                vehicle.ExShowroomPrice * rate;

            // MCD parking charge
            var parkingFee =
                vehicle.ExShowroomPrice <= 400000m
                    ? 2000m
                    : 4000m;

            return CreateResult(
                roadTax,
                otherCharges: parkingFee);
        }


        
        // 33. JAMMU & KASHMIR - 1
        

        private RegistrationParameters CalculateJammuKashmir(
            VehicleInfo vehicle)
        {
            var tax =
                vehicle.ExShowroomPrice * 0.09m;

            return CreateResult(tax);
        }


        
        // 34. LADAKH - 37
        

        private RegistrationParameters CalculateLadakh(
            VehicleInfo vehicle)
        {
            decimal rate;

            if (vehicle.ExShowroomPrice <= 1000000m)      // ₹10 lakh
                rate = 0.06m;
            else
                rate = 0.09m;

            return CreateResult(
                vehicle.ExShowroomPrice * rate);
        }


        
        // 35. LAKSHADWEEP - 31
        

        private RegistrationParameters CalculateLakshadweep(
            VehicleInfo vehicle)
        {
            var tax =
                vehicle.ExShowroomPrice * 0.055m;

            return CreateResult(tax);
        }


        
        // 36. PUDUCHERRY - 34
        

        private RegistrationParameters CalculatePuducherry(
            VehicleInfo vehicle)
        {
            var price = vehicle.ExShowroomPrice;

            decimal tax;

            if (price <= 1000000m)       // ₹10 lakh
                tax = price * 0.06m;
            else
                tax = price * 0.07m;

            return CreateResult(tax);
        }
    }
}