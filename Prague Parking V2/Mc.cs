using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prague_Parking_V2
{
    //KONKRET KLASS SOM ÄRVER FRÅN Vehicle
    public class Mc : Vehicle // Motorcykel klassen ärver från Vehicle
    {
        public override int Size => 2; // Motorcyklar tar upp 1 enhet av parkeringsutrymmet
        public override string Type => "Mc"; 
        public Mc(string licensePlate) : base(licensePlate) // Konstruktor som anropar basklassens konstruktor
        {
        }

    }
}
