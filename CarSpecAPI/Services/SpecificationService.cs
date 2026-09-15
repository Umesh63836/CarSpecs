using CarSpecAPI.Data;
using CarSpecAPI.Data.Models.RequestModel;
using CarSpecAPI.Data.Models.ResponseModel;
using CarSpecAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarSpecAPI.Services
{
    public class SpecificationService : ISpecificationService
    {
        private readonly CarsDbContext carDbContext;

        public SpecificationService(CarsDbContext carDbContext)
        {
            this.carDbContext = carDbContext;
        }

        public async Task<VariantSpecsDto?> GetVariantSpecsAsync(int variantId)
        {
            var variant = await carDbContext.Variants
                .Include(v => v.Model)
                    .ThenInclude(m => m.Brand)
                .Include(v => v.Powertrain)
                    .ThenInclude(p => p.Engine)
                        .ThenInclude(e => e.EnginePerformances)
                            .ThenInclude(ep => ep.FuelType)
                .Include(v => v.Transmission)
                .Include(v => v.Drivetrain)
                .Include(v => v.VariantImages)
                .FirstOrDefaultAsync(v => v.VariantId == variantId);

            if (variant == null)
                return null;

            var engine = variant.Powertrain?.Engine;
            var maxPower = engine?.EnginePerformances
                .Where(ep => ep.MaxPower.HasValue)
                .Select(ep => ep.MaxPower)
                .Max();
            var maxTorque = engine?.EnginePerformances
                .Where(ep => ep.MaxTorque.HasValue)
                .Select(ep => ep.MaxTorque)
                .Max();
            var performance = engine?.EnginePerformances
                .Where(ep => ep.MaxPower.HasValue)
                .OrderByDescending(ep => ep.MaxPower)
                .FirstOrDefault();

            return new VariantSpecsDto
            {
                VariantId = variant.VariantId,
                Brand = variant.Model.Brand.BrandName,
                Model = variant.Model.ModelName,
                Variant = variant.VariantName,
                ExShowroomPrice = variant.ExShowroomPrice,
                Engine = engine?.EngineName ?? string.Empty,
                NoOfCyl = engine?.NumberOfCylinders,
                NoOfValves = engine?.NumberOfValves,
                Displacement = engine?.Displacement,
                MaxPower = maxPower,
                MaxTorque = maxTorque,
                isTurbocharged = engine?.IsTurbocharged ?? false,
                EmmissionStandard = engine?.EmissionStandard,
                FuelType = performance?.FuelType?.FuelType1 ?? string.Empty,
                TransmissionType = variant.Transmission.TransmissionType,
                NoOfGears = variant.Transmission.NumberOfGears,
                Drivetrain = variant.Drivetrain.DrivetrainType,
                VarientImageURL = variant.VariantImages.FirstOrDefault(i => i.IsPrimary)?.ImageUrl
            };
        }

        public async Task<List<SelectTransmissionDto>> GetAllTransmissionsAsync()
        {
            var result = await carDbContext.Transmissions
                .Select(t => new SelectTransmissionDto
                {
                    TransmissionId = t.TransmissionId,
                    TransmissionType = t.TransmissionType,
                    NumberOfGears = Convert.ToByte(t.NumberOfGears)
                }).ToListAsync();
            return result;
        }

        public async Task<List<SelectDrivetrainDto>> GetAllDrivetrainsAsync()
        {
            var result = await carDbContext.Drivetrains
                .Select(d => new SelectDrivetrainDto
                {
                    DrivetrainId = d.DrivetrainId,
                    DrivetrainType = d.DrivetrainType,
                }).ToListAsync();
            return result;
        }

        public async Task<List<SelectEngineDto>> GetAllEnginesAsync()
        {
            var result = await carDbContext.Engines
                .Select(e => new SelectEngineDto
                {
                    EngineId = e.EngineId,
                    EngineName = e.EngineName,
                    IsTurbocharged = e.IsTurbocharged
                }).ToListAsync();
            return result;
        }

        public async Task<List<SelectFuelTypeDto>> GetAllFuelTypesAsync()
        {
            var result = await carDbContext.FuelTypes
                .Select(f => new SelectFuelTypeDto
                {
                    FuelTypeId = f.FuelTypeId,
                    FuelType = f.FuelType1,
                }).ToListAsync();
            return result;
        }

        public async Task<SelectFuelTypeDto> CreateFuelTypeAsync(CreateFuelTypeDto dto)
        {
            var fuelType = new FuelType
            {
                FuelType1 = dto.FuelType
            };

            await carDbContext.FuelTypes.AddAsync(fuelType);
            await carDbContext.SaveChangesAsync();

            return new SelectFuelTypeDto
            {
                FuelTypeId = fuelType.FuelTypeId,
                FuelType = fuelType.FuelType1
            };
        }

        public async Task<EngineDto?> CreateEngineAsync(CreateEngineDto dto)
        {
            if (dto.PowertrainId <= 0 || dto.FuelTypeId <= 0)
                return null;

            var powertrainExists = await carDbContext.Powertrains.AnyAsync(x => x.PowertrainId == dto.PowertrainId);
            if (!powertrainExists)
                return null;

            var fuelTypeExists = await carDbContext.FuelTypes.AnyAsync(x => x.FuelTypeId == dto.FuelTypeId);
            if (!fuelTypeExists)
                return null;

            var engine = new Engine
            {
                PowertrainId = dto.PowertrainId,
                EngineName = dto.EngineName,
                NumberOfCylinders = dto.NumberOfCylinders,
                NumberOfValves = dto.NumberOfValves,
                Displacement = dto.Displacement,
                IsTurbocharged = dto.IsTurbocharged,
                EmissionStandard = dto.EmissionStandard,
                Aspiration = null,
                EngineType = null
            };

            await carDbContext.Engines.AddAsync(engine);
            await carDbContext.SaveChangesAsync();

            var enginePerformance = new EnginePerformance
            {
                EngineId = engine.EngineId,
                ModeName = "Default",
                FuelTypeId = dto.FuelTypeId,
                MaxPower = dto.MaxPower,
                MaxTorque = dto.MaxTorque,
            };

            await carDbContext.EnginePerformances.AddAsync(enginePerformance);
            await carDbContext.SaveChangesAsync();

            var createdEngine = await carDbContext.Engines.FirstAsync(x => x.EngineId == engine.EngineId);
            var createdPerformance = await carDbContext.EnginePerformances
                .Include(ep => ep.FuelType)
                .FirstAsync(ep => ep.EnginePerformanceId == enginePerformance.EnginePerformanceId);

            return new EngineDto
            {
                EngineId = createdEngine.EngineId,
                EngineName = createdEngine.EngineName,
                FuelType = createdPerformance.FuelType?.FuelType1 ?? string.Empty,
                NumberOfCylinders = Convert.ToByte(createdEngine.NumberOfCylinders ?? 0),
                NumberOfValves = Convert.ToByte(createdEngine.NumberOfValves ?? 0),
                Displacement = Convert.ToDecimal(createdEngine.Displacement ?? 0),
                MaxPower = createdPerformance.MaxPower ?? 0,
                MaxTorque = createdPerformance.MaxTorque ?? 0,
                IsTurbocharged = createdEngine.IsTurbocharged,
                EmissionStandard = createdEngine.EmissionStandard ?? string.Empty
            };
        }

        public async Task<TransmissionDto> CreateTransmissionAsync(CreateTransmissionDto dto)
        {
            var transmission = new Transmission
            {
                TransmissionType = dto.TransmissionType,
                NumberOfGears = dto.NumberOfGears
            };

            await carDbContext.Transmissions.AddAsync(transmission);
            await carDbContext.SaveChangesAsync();

            return new TransmissionDto
            {
                TransmissionId = transmission.TransmissionId,
                TransmissionType = transmission.TransmissionType,
                NumberOfGears = Convert.ToByte(transmission.NumberOfGears)
            };
        }

        public async Task<DrivetrainDto> CreateDrivetrainAsync(CreateDrivetrainDto dto)
        {
            var drivetrain = new Drivetrain
            {
                DrivetrainType = dto.DrivetrainType
            };

            await carDbContext.Drivetrains.AddAsync(drivetrain);
            await carDbContext.SaveChangesAsync();

            return new DrivetrainDto
            {
                DrivetrainId = drivetrain.DrivetrainId,
                DrivetrainType = drivetrain.DrivetrainType
            };
        }
    }
}
