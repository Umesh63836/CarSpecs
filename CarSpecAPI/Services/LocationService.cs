using CarSpecAPI.Data.Models.ResponseModel;
using CarSpecAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarSpecAPI.Services
{
    public class LocationService : ILocationService
    {
        private readonly CarsDbContext context;

        public LocationService(CarsDbContext context)
        {
            this.context = context;
        }

        public async Task<List<LocationSearchDto>> SearchLocationsAsync(string searchTerm, int limit = 10)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<LocationSearchDto>();

            searchTerm = searchTerm.Trim();

            if (limit <= 0)
                limit = 10;

            if (limit > 50)
                limit = 50;

            var states = await context.States.AsNoTracking().Where(s => s.StateName.StartsWith(searchTerm))
                .OrderBy(s => s.StateName).Take(limit).Select(s => new LocationSearchDto
                {
                    Type = "State",
                    Id = s.StateCode,
                    Name = s.StateName,
                    StateCode = s.StateCode,
                    StateName = s.StateName,
                    DistrictCode = null,
                    DistrictName = null
                })
                .ToListAsync();


            var districts = await context.Districts.AsNoTracking().Where(d => d.DistrictName.StartsWith(searchTerm))
                .OrderBy(d => d.DistrictName).Take(limit).Select(d => new LocationSearchDto
                {
                    Type = "District",
                    Id = d.DistrictCode,
                    Name = d.DistrictName,

                    StateCode = d.StateCode,

                    StateName = d.StateCodeNavigation.StateName,

                    DistrictCode = d.DistrictCode,
                    DistrictName = d.DistrictName
                })
                .ToListAsync();


            var cities = await context.SubDistricts.AsNoTracking().Where(sd => sd.SubDistrictName.StartsWith(searchTerm))
                .OrderBy(sd => sd.SubDistrictName).Take(limit).Select(sd => new LocationSearchDto
                {
                    Type = "City",
                    Id = sd.SubDistrictCode,
                    Name = sd.SubDistrictName,

                    StateCode = sd.DistrictCodeNavigation.StateCode,

                    StateName = sd.DistrictCodeNavigation.StateCodeNavigation.StateName,

                    DistrictCode = sd.DistrictCode,
                    DistrictName = sd.DistrictCodeNavigation.DistrictName
                })
                .ToListAsync();


            return states.Concat(districts).Concat(cities).OrderBy(x => x.Name).Take(limit).ToList();
        }
    }
}
