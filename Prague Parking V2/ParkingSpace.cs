using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prague_Parking_V2
{
    public class ParkingSpace
    {
        private readonly List<Vehicle> _vehicles = new(); // Lista över fordon parkerade i denna plats
        public int Index { get; }
        public int CapacitySpaces { get; }

        public IReadOnlyList<Vehicle> Vehicles => _vehicles.AsReadOnly(); // Exponerar fordonen som är parkerade i denna plats som en read-only lista

        public ParkingSpace(int index, int capacitySpaces = 1) // Konstruktor för parkeringsplatsen
        {
            Index = index;
            CapacitySpaces = capacitySpaces;
        }

        public bool CanVehicleFit(Vehicle vehicle)
            => _vehicles.Sum(x => x.Size) + vehicle.Size <= CapacitySpaces;

        public void ParkVehicle(Vehicle vehicle)
        {
            if (!CanVehicleFit(vehicle))
            {
                throw new InvalidOperationException("Vehicle cannot fit in this parking space.");
            }
            _vehicles.Add(vehicle);
        }

        public bool RemoveVehicle(string fixedRegNumber)
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
    }
}
