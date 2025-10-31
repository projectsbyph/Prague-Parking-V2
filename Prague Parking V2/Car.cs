using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prague_Parking_V2
{
    /// <summary>
    /// Konkret klass Car som ärver från Vehicle
    /// </summary>
    /// <remarks>
    /// Egenskaperna Size (storlek) och HourlyRate (timpris) sätts inte i Car klassen.
    /// Dem hämtas från config via basklassens ApplySpec metod när fordonet skapas i ParkingGarage.
    /// Detta för att möjliggöra flexibilitet och enkel ändring av fordonsspecifikationer utan att behöva modifiera själva Car klassen.
    /// 
    /// Det sker:
    /// 1. I Menu när användaren väljer att parkera en bil.
    ///     var spec = GetSpec("Car");
    ///     vehicle.ApplySpec(spec.ChargePerHour, spec.CapacityUnits);
    /// 2. Vid inläsning från sparfil i Mapper.FromDto metoden.
    ///     var spec = config.VehicleTypes.FirstOrDefault(v => v.Type == vehicle.Type);
    ///     vehicle.ApplySpec(spec.ChargePerHour, spec.CapacityUnits);
    /// </remarks>

    public class Car : Vehicle // Bil klassen ärver från Vehicle
    {
        public Car(string licensePlate) : base(licensePlate, "Car") { } // Konstruktor som anropar basklassens konstruktor
    }
}
