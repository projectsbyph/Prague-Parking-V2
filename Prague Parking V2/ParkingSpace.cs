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
        public int CapacitySpaces { get; } = 4; // Standardkapacitet för parkeringsplatsen är 4 enheter

        public IReadOnlyList<Vehicle> Vehicles => _vehicles; // Exponerar fordonen som är parkerade i denna plats som en read-only lista
        public int UsedSpaces => _vehicles.Sum(vehicle => vehicle.Size); // Beräknar det använda utrymmet i parkeringsplatsen



        // KONSTRUKTOR
        public ParkingSpace(int index, int capacitySpaces = 1) // Konstruktor för parkeringsplatsen
        {
            Index = index; // Sätt index för parkeringsplatsen
            CapacitySpaces = capacitySpaces; // Sätt kapacitet för parkeringsplatsen
        }

        // METODER
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

        // WRAPPER METOD FÖR ATT TA BORT FORDON
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
