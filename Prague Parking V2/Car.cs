using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prague_Parking_V2
{
    //KONKRET KLASS SOM ÄRVER FRÅN Vehicle
    public class Car : Vehicle // Bil klassen ärver från Vehicle
    {
        public Car(string licensePlate) : base(licensePlate, "Car") { } // Konstruktor som anropar basklassens konstruktor
        
        
    }
}
