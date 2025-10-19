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
                    spaceIndex = space.CapacityIndex;
                    return true;
                }
            }
            spaceIndex = -1;
            return false; // No available space found 
        }

        public bool CheckoutVehicle(string fixedRegNumber, out int spaceIndex) // Ta bort ett fordon från garaget
        {
            var regNumber = Vehicle.FixReg(fixedRegNumber);
            
            foreach (var space in ParkingSpaces)
            {
                var vehicle = space.Vehicles.FirstOrDefault(v => v.RegNumber == regNumber);
                if (vehicle != null && space.RemoveVehicle(regNumber))
                {
                    vehicle = vehicle;
                    spaceIndex = space.CapacityIndex;
                    return true;
                }
            }
            vehicle = null;
            spaceIndex = -1;
            return false; // Fordonet hittades inte
        }

        public Vehicle? FindVehicle(string fixedRegNumber, out int spaceIndex) //Hitta ett fordon i garaget
        {
            var regNumber = Vehicle.FixReg(fixedRegNumber);

            foreach (var space in ParkingSpaces)
            {
                var vehicle = space.Vehicles.FirstOrDefault(vehicle => vehicle.RegNumber == regNumber);
                if (vehicle != null)
                {
                    spaceIndex = space.CapacityIndex;
                    return vehicle;
                }
            }
            spaceIndex = -1;
            return null; // Vehicle not found
        }
    }
}
