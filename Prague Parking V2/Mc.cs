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
        public Mc(string licensePlate) : base(licensePlate, "Mc") // Konstruktor som anropar basklassens konstruktor
        {
        }

    }
}
