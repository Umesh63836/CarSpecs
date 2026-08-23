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
            var result = await carDbContext.Variants
            .Where(v => v.VariantId == variantId)
            .Select(v => new VariantSpecsDto
            {
                EngineId = v.Engine.EngineId,
                Engine = v.Engine.EngineName,

                NoOfCyl = v.Engine.NumberOfCylinders,
                NoOfValves = v.Engine.NumberOfValves,
                Displacement = v.Engine.Displacement,
                MaxPower = v.Engine.MaxPower,
                MaxTorque = v.Engine.MaxTorque,

                isTurbocharged = v.Engine.IsTurbocharged,
                EmmissionStandard = v.Engine.EmissionStandard,

                FuelType = v.Engine.FuelType.FuelType1,
                TransmissionType = v.Transmission.TransmissionType,

                NoOfGears = v.Transmission.NumberOfGears,

                Drivetrain = v.Drivetrain.DrivetrainType,
                VarientImageURL = v.VariantImages.Where(i => i.IsPrimary).Select(i => i.ImageUrl).FirstOrDefault(),
            }).FirstOrDefaultAsync();
            return result;
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


        public async Task<EngineDto?> CreateEngineAsync(CreateEngineDto dto)
        {
            // Check that FuelType exists
            var fuelTypeExists = await carDbContext.FuelTypes.AnyAsync(x => x.FuelTypeId == dto.FuelTypeId);

            if (!fuelTypeExists)
                return null;

            var engine = new Engine
            {
                EngineName = dto.EngineName,
                FuelTypeId = dto.FuelTypeId,
                NumberOfCylinders = dto.NumberOfCylinders,
                NumberOfValves = dto.NumberOfValves,
                Displacement = dto.Displacement,
                MaxPower = dto.MaxPower,
                MaxTorque = dto.MaxTorque,
                IsTurbocharged = dto.IsTurbocharged,
                EmissionStandard = dto.EmissionStandard
            };

            await carDbContext.Engines.AddAsync(engine);
            await carDbContext.SaveChangesAsync();

            var createdEngine = await carDbContext.Engines.Include(f => f.FuelType).Where(x => x.EngineId == engine.EngineId).FirstAsync();

            return new EngineDto
            {
                EngineId = createdEngine.EngineId,
                EngineName = createdEngine.EngineName,
                FuelType = createdEngine.FuelType.FuelType1,
                NumberOfCylinders = Convert.ToByte(createdEngine.NumberOfCylinders),
                NumberOfValves = Convert.ToByte(createdEngine.NumberOfValves),
                Displacement = Convert.ToDecimal(createdEngine.Displacement),
                MaxPower = Convert.ToDecimal(createdEngine.MaxPower),
                MaxTorque = Convert.ToDecimal(createdEngine.MaxTorque),
                IsTurbocharged = createdEngine.IsTurbocharged,
                EmissionStandard = createdEngine.EmissionStandard
            };
        }


        public async Task<TransmissionDto> CreateTransmissionAsync(
            CreateTransmissionDto dto)
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


        public async Task<DrivetrainDto> CreateDrivetrainAsync(
            CreateDrivetrainDto dto)
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
