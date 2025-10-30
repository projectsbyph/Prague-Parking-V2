using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prague_Parking_V2
{
    public class ParkingSpace
    {
        // FÄLT
        private readonly List<Vehicle> _vehicles = new(); // Lista över fordon parkerade i denna plats

        // EGENSKAPER
        public int Index { get; }
        public int CapacitySpaces { get; private set; } // Standardkapacitet för parkeringsplatsen är 4 enheter

        public IReadOnlyList<Vehicle> Vehicles => _vehicles; // Exponerar fordonen som är parkerade i denna plats som en read-only lista. 
        public int UsedSpaces => _vehicles.Sum(vehicle => vehicle.Size); // Beräknar det använda utrymmet i parkeringsplatsen



        // KONSTRUKTOR
        public ParkingSpace(int index, int capacitySpaces) // Konstruktor för parkeringsplatsen
        {
            Index = index; // Sätt index för parkeringsplatsen
            if (capacitySpaces <= 0)
                throw new ArgumentOutOfRangeException(nameof(capacitySpaces), "Capacity must be greater than zero.");
            CapacitySpaces = capacitySpaces; // Sätt kapacitet för parkeringsplatsen
        }

        // METODER
        public void SetCapacity(int newCapacity) // Ändra kapaciteten för parkeringsplatsen
        {
            if (newCapacity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(newCapacity), "Capacity must be greater than zero.");
            }
            var usedSpaces = _vehicles.Sum(vehicle => vehicle.Size);
            if (usedSpaces > newCapacity)
            {
                throw new InvalidOperationException("Cannot set capacity lower than the currently used spaces.");
            }
            CapacitySpaces = newCapacity;
        }

        public bool CanVehicleFit(Vehicle vehicle) // Kontrollera om fordonet får plats i denna parkeringsplats
            => _vehicles.Sum(x => x.Size) + vehicle.Size <= CapacitySpaces;

        public void ParkVehicle(Vehicle vehicle) // Parkera ett fordon i denna parkeringsplats
        {
            if (!CanVehicleFit(vehicle))
            {
                throw new InvalidOperationException("Vehicle cannot fit in this parking space.");
            }
            vehicle.MarkParkedNow();
            _vehicles.Add(vehicle);
        }

 
        public bool RemoveVehicle(string fixedRegNumber) // Ta bort ett fordon från denna parkeringsplats baserat på registreringsnumret
        {
            var i = _vehicles.FindIndex(vehicle => vehicle.LicensePlate == fixedRegNumber); // Hitta indexet för fordonet med det angivna registreringsnumret
            if (i >= 0)
            {
                _vehicles.RemoveAt(i);
            }
            else
            {
                throw new InvalidOperationException("Vehicle with the specified registration number not found in this parking space.");
            }
            return i >= 0; // Returnerar true om fordonet togs bort, annars false
        }

        public void AddLoadedVehicle(Vehicle vehicle) // Lägg till ett fordon som redan är parkerat (används vid inläsning från sparfil)
        {
            _vehicles.Add(vehicle);
        }
    }
}
