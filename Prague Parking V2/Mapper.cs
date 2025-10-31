using PragueParkingV2.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PragueParkingV2.Data.ParkingGarageDto;

namespace Prague_Parking_V2
{
    internal class Mapper // Klass för att mappa mellan domänmodeller och DTOs
    {
        public static GarageDto ToDto(ParkingGarage garage) // Konverterar ett ParkingGarage objekt till dess motsvarande GarageDto representation för serialisering.
        {
            var saveModelGarage = new GarageDto
            {
                SpaceCount = garage.ParkingSpaces.Count,
                SpaceCapacityUnits = garage.ParkingSpaces.First().CapacitySpaces
            };

            foreach (var space in garage.ParkingSpaces) // Loopar igenom varje parkeringsplats i garaget
            {
                var spaceDto = new SpaceDto { Index = space.Index };
                
                foreach (var vehicle in space.Vehicles) // Loopar igenom varje fordon parkerat på den aktuella parkeringsplatsen
                {
                    spaceDto.Vehicle.Add(new VehicleDto
                    {
                        Type = vehicle is Car ? "Car" : "Mc",
                        LicensePlate = vehicle.LicensePlate,
                        ParkedAtUtc = vehicle.TimeParked
                    });
                }
                saveModelGarage.Spaces.Add(spaceDto);
            }
            return saveModelGarage; // Returnerar den fyllda GarageDto objektet
        }

        
        public static ParkingGarage FromDto(GarageDto saveModelParking, ConfigApp config) // Konverterar ett GarageDto objekt tillbaka till ett ParkingGarage objekt med hjälp av applikationskonfigurationen.
        {
            // Fallback värden om inget är specificerat i sparmodellen eller config
            const int DEFAULT_SPACE_COUNT_FALLBACK = 100;
            const int DEFAULT_SPACE_CAPACITY_FALLBACK = 4;

            var capacity = saveModelParking.SpaceCapacityUnits > 0
                ? saveModelParking.SpaceCapacityUnits
                : config.DefaultSpaceCapacityUnits > 0 ? config.DefaultSpaceCapacityUnits:
                DEFAULT_SPACE_CAPACITY_FALLBACK;

            var spaceCount = saveModelParking.SpaceCount > 0
                ? saveModelParking.SpaceCount
                : config.DefaultSpaceCount > 0 ? config.DefaultSpaceCount:
                DEFAULT_SPACE_COUNT_FALLBACK;

            // Skapa garage
            var garage = new ParkingGarage(spaceCount, capacity); // Använder kapacitet och antal från sparmodellen eller fallback

            foreach (var spaceDto in saveModelParking.Spaces) // Loopar igenom varje parkeringsplats i sparmodellen
            {
                var space = garage.ParkingSpaces.First(x => x.Index == spaceDto.Index);

                foreach (var vehicleDto in spaceDto.Vehicle) // Loopar igenom varje fordon på den aktuella parkeringsplatsen
                {
                    Vehicle vehicle = vehicleDto.Type.Equals("Car", System.StringComparison.OrdinalIgnoreCase)
                    ? new Car(vehicleDto.LicensePlate)
                    : new Mc(vehicleDto.LicensePlate);

                    // Applicera specifikationer från config
                    var vehicleSpec = config.VehicleTypes.First(v => v.Type.Equals(vehicleDto.Type, StringComparison.OrdinalIgnoreCase));
                    vehicle.ApplySpec(vehicleSpec.ChargePerHour, vehicleSpec.CapacityUnits);

                    vehicle.RestoreParkedAtUtc(vehicleDto.ParkedAtUtc);
                    space.AddLoadedVehicle(vehicle);
                }
            }
            return garage; // Returnerar det återställda ParkingGarage objektet
        }
    }
}
