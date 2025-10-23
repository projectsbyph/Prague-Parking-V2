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
        public override int Size => 4;
        public override string Type => "Car"; // Typ av fordon 

        public Car(string licensePlate) : base(licensePlate) // Konstruktor som anropar basklassens konstruktor
        {
        }
    }
}
