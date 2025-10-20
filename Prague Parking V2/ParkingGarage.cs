using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prague_Parking_V2
{
    public class ParkingGarage
    {
        public List<ParkingSpace> ParkingSpaces { get; } = new(); // Lista över parkeringsplatser

        public ParkingGarage(int numberOfSpaces, int capacityPerSpace)  // Konstruktor för parkeringsgaraget
        {
            ParkingSpaces = Enumerable.Range(1, numberOfSpaces)
                .Select(i => new ParkingSpace(i, capacityPerSpace))
                .ToList();
            for (int i = 0; i < numberOfSpaces; i++)
            {
                ParkingSpaces.Add(new ParkingSpace(i, capacityPerSpace));
            }
        }

        public bool TryParkVehicle(Vehicle vehicle, out int spaceIndex) // Försök att parkera ett fordon
        {
            foreach (var space in ParkingSpaces)
            {
                if (space.CanVehicleFit(vehicle))
                {
                    space.ParkVehicle(vehicle);
                    spaceIndex = space.Index;
                    return true;
                }
            }
            spaceIndex = -1;
            return false; // No available space found 
        }

        public bool TryRemoveVehicle(string fixedRegNumber, out Vehicle? removedVehicle) // Försök att ta bort ett fordon från garaget
        {
            var regNumber = Vehicle.FixReg(fixedRegNumber);
            foreach (var space in ParkingSpaces)
            {
                var vehicle = space.Vehicles.FirstOrDefault(v => v.LicensePlate == regNumber);
                if (vehicle != null && space.RemoveVehicle(regNumber))
                {
                    removedVehicle = vehicle;
                    return true;
                }
            }
            removedVehicle = null;
            return false; // Fordonet hittades inte
        }

        public bool CheckoutVehicle(string fixedRegNumber, out int spaceIndex) // Ta bort ett fordon från garaget
        {
            var regNumber = Vehicle.FixReg(fixedRegNumber);

            foreach (var space in ParkingSpaces)
            {
                var vehicle = space.Vehicles.FirstOrDefault(v => v.LicensePlate == regNumber); // Hitta fordonet i parkeringsplatsen genom att matcha registreringsnumret med fordonets LicensePlate-egenskap
                if (vehicle != null && space.RemoveVehicle(regNumber))
                {
                    spaceIndex = space.Index;
                    return true;
                }
            }
            spaceIndex = -1;
            return false; // Fordonet hittades inte
        }

        public Vehicle? FindVehicle(string fixedRegNumber, out int spaceIndex) //Hitta ett fordon i garaget
        {
            var regNumber = Vehicle.FixReg(fixedRegNumber);

            foreach (var space in ParkingSpaces)
            {
                var vehicle = space.Vehicles.FirstOrDefault(vehicle => vehicle.LicensePlate == regNumber);
                if (vehicle != null)
                {
                    spaceIndex = space.Index;
                    return vehicle;
                }
            }
            spaceIndex = -1;
            return null; // Vehicle not found
        }

        public sealed class ParkingStats
        {
            public int TotalSpaces { get; }
            public int OccupiedSpaces { get; }
            public int FreeSpaces { get; }
            public int TotalVehiclesParked { get; }

            public ParkingStats(int totalSpaces, int occupiedSpaces, int totalVehiclesParked, int vehicles)
            {
                TotalSpaces = totalSpaces;
                OccupiedSpaces = occupiedSpaces;
                TotalVehiclesParked = totalVehiclesParked;
                FreeSpaces = vehicles;
            }
        }

        public ParkingStats GetParkingStats() // Hämta statistik om garaget
        {
            int totalSpaces = ParkingSpaces.Count;
            int occupiedSpaces = ParkingSpaces.Count(space => space.Vehicles.Count > 0);
            int totalVehiclesParked = ParkingSpaces.Sum(space => space.Vehicles.Count);
            int freeSpaces = totalSpaces - occupiedSpaces;
            return new ParkingStats(totalSpaces, occupiedSpaces, totalVehiclesParked, freeSpaces);
        }
    }
}
