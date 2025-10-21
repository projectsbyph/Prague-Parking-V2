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


        // KONSTRUKTOR
        public ParkingGarage(int spaceCount, int spaceCapacityUnits)  // Konstruktor för parkeringsgaraget
        {
            ParkingSpaces = Enumerable.Range(1, spaceCount)
                .Select(i => new ParkingSpace(i, spaceCapacityUnits))
                .ToList();
        }

        // PARKERA FORDON
        public bool TryParkVehicle(Vehicle vehicle, out int spaceIndex) // Försök att parkera ett fordon. spaceIndex representerar indexet för parkeringsplatsen där fordonet parkerades
        {
            var reg = vehicle.LicensePlate;
            foreach (var space in ParkingSpaces)
            {
                var existingVehicle = space.Vehicles.FirstOrDefault(v => v.LicensePlate == reg); // Kontrollera om fordonet redan är parkerat
                if (existingVehicle != null)
                {
                    spaceIndex = -1;
                    return false; // Fordonet är redan parkerat
                }
            }

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

        // TA BORT FORDON
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

        // CHECKOUT FORDON (SKA VARA KÄRNMETOD)
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

        // HITTA FORDON
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
    }
}
