using LibraryPragueParking.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LibraryPragueParking.Data.ParkingGarageDto;

namespace Prague_Parking_V2
{
    internal class Mapper // Mapping domän -> DTO och DTO -> domän
    {
        public static GarageDto ToDto(ParkingGarage garage)
        {
            var dto = new GarageDto
            {
                SpaceCount = garage.ParkingSpaces.Count,
                SpaceCapacityUnits = garage.ParkingSpaces.First().CapacitySpaces
            };

            foreach (var space in garage.ParkingSpaces)
            {
                var spaceDto = new SpaceDto { Index = space.Index };
                
                foreach (var vehicle in space.Vehicles)
                {
                    spaceDto.Vehicle.Add(new VehicleDto
                    {
                        Type = vehicle is Car ? "Car" : "Mc",
                        LicensePlate = vehicle.LicensePlate,
                        ParkedAtUtc = vehicle.TimeParked
                    });
                }
                dto.Spaces.Add(spaceDto);
            }
            return dto;
        }

        //Mapping DTO -> domän
        public static ParkingGarage FromDto(GarageDto dto, ConfigApp cfg)
        {
            // Fallback
            const int DEFAULT_SPACE_COUNT_FALLBACK = 100;
            const int DEFAULT_SPACE_CAPACITY_FALLBACK = 4;

            var capacity = dto.SpaceCapacityUnits > 0
                ? dto.SpaceCapacityUnits
                : cfg.DefaultSpaceCapacityUnits > 0 ? cfg.DefaultSpaceCapacityUnits:
                DEFAULT_SPACE_CAPACITY_FALLBACK;

            var spaceCount = dto.SpaceCount > 0
                ? dto.SpaceCount
                : cfg.DefaultSpaceCount > 0 ? cfg.DefaultSpaceCount:
                DEFAULT_SPACE_COUNT_FALLBACK;

            // Skapa garage
            var garage = new ParkingGarage(spaceCount, capacity);
            
            foreach (var spaceDto in dto.Spaces)
            {
                var space = garage.ParkingSpaces.First(x => x.Index == spaceDto.Index);

                foreach (var vehicleDto in spaceDto.Vehicle)
                {
                    Vehicle vehicle = vehicleDto.Type.Equals("Car", System.StringComparison.OrdinalIgnoreCase)
                    ? new Car(vehicleDto.LicensePlate)
                    : new Mc(vehicleDto.LicensePlate);

                    // Applicera specifikationer från config
                    var spec = cfg.VehicleTypes.First(v => v.Type.Equals(vehicleDto.Type, StringComparison.OrdinalIgnoreCase));
                    vehicle.ApplySpec(spec.ChargePerHour, spec.CapacityUnits);

                    vehicle.RestoreParkedAtUtc(vehicleDto.ParkedAtUtc);
                    space.AddLoadedVehicle(vehicle);
                }
            }
            return garage;
        }
    }
}
