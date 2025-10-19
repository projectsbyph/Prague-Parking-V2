using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prague_Parking_V2
{
    public class ParkingSpace
    {
        private readonly List<Vehicle> _vehicles = new();
        public int CapacityIndex { get; }
        public int CapacitySpaces { get; }

        public ParkingSpace(int index, int capacitySpaces = 1)
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

        public void RemoceVehicle(string fixedRegNumber)
        {
            var i = _vehicles.FindIndexOf(vehicle => vehicle.RegNumber == fixedRegNumber);
            if (i >= 0)
            {
                _vehicles.RemoveAt(i);
            }
            else
            {
                throw new InvalidOperationException("Vehicle with the specified registration number not found in this parking space.");
            }
        }
    }
}
