using LibraryPragueParking.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LibraryPragueParking.Data.DTO;

namespace Prague_Parking_V2
{
    internal class Mapper
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
        public static ParkingGarage FromDto(GarageDto dto)
        {
            var garage = new ParkingGarage(dto.SpaceCount, dto.SpaceCapacityUnits);
            
            foreach (var spaceDto in dto.Spaces)
            {
                var space = garage.ParkingSpaces.First(x => x.Index == spaceDto.Index);

                foreach (var vehicleDto in spaceDto.Vehicle)
                {
                    Vehicle vehicle = vehicleDto.Type == "Car"
                    ? new Car(vehicleDto.LicensePlate)
                    : new Mc(vehicleDto.LicensePlate);

                    vehicle.RestoreParkedAtUtc(vehicleDto.ParkedAtUtc);
                    space.AddLoadedVehicle(vehicle);
                }
            }
            return garage;
        }
    }
}
