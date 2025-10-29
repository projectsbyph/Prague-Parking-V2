using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Prague_Parking_V2
{
    public class ParkingGarage
    {
        public List<ParkingSpace> ParkingSpaces { get; } = new(); // Lista över parkeringsplatser
        public IReadOnlyList<ParkingSpace> Spaces => ParkingSpaces; // Exponerar parkeringsplatserna som en read-only lista



        // KONSTRUKTOR
        public ParkingGarage(int spaceCount, int spaceCapacityUnits)  // Konstruktor för parkeringsgaraget
        {
            if (spaceCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(spaceCount), "Space count must be greater than zero.");
            if (spaceCapacityUnits <= 0)
                throw new ArgumentOutOfRangeException(nameof(spaceCapacityUnits), "Space capacity must be greater than zero.");

            for (int i = 0; i < spaceCount; i++)
            {
                ParkingSpaces.Add(new ParkingSpace(i + 1, spaceCapacityUnits)); // Skapa och lägg till parkeringsplatser i garaget
            }
        }

        // PARKERA FORDON
        public bool TryParkVehicle(Vehicle vehicle, out int spaceIndex) // Försök att parkera ett fordon. spaceIndex representerar indexet för parkeringsplatsen där fordonet parkerades
        {
            var reg = vehicle.LicensePlate;
            foreach (var space in ParkingSpaces)
            {
                var existingVehicle = space.Vehicles.FirstOrDefault(vehicle => vehicle.LicensePlate == reg); // Kontrollera om fordonet redan är parkerat
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

        // FLYTTA FORDON
        public bool MoveVehicle(string fixedRegNumber, int targetSpaceIndex, out int fromSpaceIndex, out string? error) // Flytta ett fordon till en annan parkeringsplats
        {
            error = null; // Initialisera felmeddelandet som null
            fromSpaceIndex = -1; // Initialisera frånSpaceIndex som -1
            var regNumber = Vehicle.FixReg(fixedRegNumber);

            Vehicle? vehicleToMove = null;
            ParkingSpace? currentSpace = null;

            // Hitta fordonet och dess nuvarande parkeringsplats
            foreach (var space in ParkingSpaces)
            {
                var vehicle = space.Vehicles.FirstOrDefault(v => v.LicensePlate == regNumber);
                if (vehicle != null)
                {
                    vehicleToMove = vehicle;
                    currentSpace = space;
                    break;
                }
            }

            if (vehicleToMove == null || currentSpace == null)
            {
                error = "Vehicle not found in the garage.";
                return false; // Fordonet hittades inte
            }

            if (targetSpaceIndex == currentSpace.Index)
            {
                error = "The vehicle is already in the target parking space.";
                return false; // Fordonet är redan i målparkeringsplatsen
            }


            // Hämtar målparkeringsplatsen
            var targetSpace = ParkingSpaces.FirstOrDefault(s => s.Index == targetSpaceIndex);
            if (targetSpace == null || !targetSpace.CanVehicleFit(vehicleToMove))
            {
                error = "Target parking space is invalid or does not have enough space.";
                return false; // Målparkeringsplatsen är ogiltig eller har inte tillräckligt med utrymme
            }

            //Kolla om fordonet rymms i målparkeringsplatsen
            var usedUnits = targetSpace.Vehicles.Sum(v => v.Size);
            if (usedUnits + vehicleToMove.Size > targetSpace.CapacitySpaces)
            {
                error = "Not enough space in the target parking space.";
                return false; // Inte tillräckligt med utrymme i målparkeringsplatsen
            }

            // Ta bort fordonet från den nuvarande parkeringsplatsen och parkera det i målparkeringsplatsen
            var removed = currentSpace.RemoveVehicle(regNumber);
            if (removed)
            {
                targetSpace.ParkVehicle(vehicleToMove);
                fromSpaceIndex = currentSpace.Index;
                return true; // Fordonet flyttades framgångsrikt
            }
            else
            {
                error = "Failed to remove the vehicle from its current parking space.";
                return false; // Misslyckades med att ta bort fordonet från dess nuvarande parkeringsplats
            }

            /*Flyttar utan att ändra parkerings tid
            targetSpace.AddLoadedVehicle(vehicleToMove);
            return true;*/
        }

        public bool TryResizeSpace(int newSpaceCount, int newSpaceCapacity, out string? error) // Försök att ändra storleken på en parkeringsplats

        {
            if (newSpaceCount <= 0)
            {
                error = "New space count must be greater than zero.";
                return false; // Det nya antalet platser måste vara större än noll
            }
            if (newSpaceCapacity <= 0)
            {
                error = "New capacity per space must be greater than zero.";
                return false; // Den nya kapaciteten per plats måste vara större än noll
            }

            foreach (var space in ParkingSpaces)
            {
                var usedSpaces = space.Vehicles.Sum(vehicle => vehicle.Size);
                if (usedSpaces > newSpaceCapacity)
                {
                    error = $"Cannot resize space {space.Index} to {newSpaceCapacity} units because it currently has {usedSpaces} units used.";
                    return false; // Kan inte ändra storleken på platsen eftersom den för närvarande har mer använda enheter än den nya kapaciteten
                }
            }

            foreach (var space in ParkingSpaces)
            {
                space.SetCapacity(newSpaceCapacity);
            }

            int currentSpaceCount = ParkingSpaces.Count;
            if (newSpaceCount > currentSpaceCount)
            {
                for (int i = currentSpaceCount; i < newSpaceCount; i++)
                {
                    ParkingSpaces.Add(new ParkingSpace(i + 1, newSpaceCapacity)); // Lägg till nya parkeringsplatser
                }
            }
            else if (newSpaceCount < currentSpaceCount)
            {
                ParkingSpaces.RemoveRange(newSpaceCount, currentSpaceCount - newSpaceCount); // Ta bort överflödiga parkeringsplatser
            }
            error = null;
            return true;
        }
    }
}
