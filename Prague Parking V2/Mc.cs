using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prague_Parking_V2
{
    /// <summary>
    /// Konkret klass Mc som ärver från Vehicle
    /// </summary>
    /// <remarks>
    /// Egenskaperna Size (storlek) och HourlyRate (timpris) sätts inte i Mc klassen.
    /// Dem hämtas från config via basklassens ApplySpec metod när fordonet skapas i ParkingGarage.
    /// 
    /// Det sker:
    /// 1. I Menu när användaren väljer att parkera en bil.
    ///     var spec = GetSpec("Mc");
    ///     vehicle.ApplySpec(spec.ChargePerHour, spec.CapacityUnits);
    /// 2. Vid inläsning från sparfil i Mapper.FromDto metoden.
    ///     var spec = config.VehicleTypes.FirstOrDefault(v => v.Type == vehicle.Type);
    ///     vehicle.ApplySpec(spec.ChargePerHour, spec.CapacityUnits);
    /// </remarks>
    
    public class Mc : Vehicle // Motorcykel klassen ärver från Vehicle
    {
        public Mc(string licensePlate) : base(licensePlate, "Mc") // Konstruktor som anropar basklassens konstruktor
        {
        }

    }
}
