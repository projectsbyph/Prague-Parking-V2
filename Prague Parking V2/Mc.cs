using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prague_Parking_V2
{
    public class Mc : Vehicle // Motorcykel klassen ärver från Vehicle
    {
        public override int Size => 1; // Motorcyklar tar upp 1 enhet av parkeringsutrymmet
        public override string Type => "Mc"; 
        public Mc(string licensePlate, string color) : base(licensePlate, color) // Konstruktor som anropar basklassens konstruktor
        {
        }

    }
}
