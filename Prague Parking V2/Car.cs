using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prague_Parking_V2
{
    public class Car : Vehicle
    {
        // Car class implementation
        public override int Size => 1;
        public override string Type => "Car";

        public Car(string licensePlate, string color) : base(licensePlate, color) // Konstruktor som anropar basklassens konstruktor
        {
        }
    }
}
