using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Prague_Parking_V2
{
    public abstract class Vehicle
    {
        private static readonly Regex RegLetters = new(@"^[A-Z0-9]{1,3}-[A-Z0-9]{1,2} [0-9]{1,4}$", RegexOptions.Compiled); //Kolla denna line

        private string LicensePlate { get; }
        public string Color { get; }
        public DateTime TimeParked { get; } = DateTime.Now;

        public abstract int Size { get; }
        public abstract string Type { get; }

        protected Vehicle(string licensePlate, string color)
        {
            if (!RegLetters.IsMatch(licensePlate))
            {
                throw new ArgumentException("Invalid license plate format.");
            }
            LicensePlate = licensePlate;
            Color = color;
        }

        public override string ToString()
        {
            return $"{Type} - License Plate: {LicensePlate}, Color: {Color}, Parked At: {TimeParked}";
        }

        //Hjälpare för UI att visa fordonets info
        public static string FixReg(string input) // Rensa och formatera registreringsnumret

            => input.Trim().Replace(" ", "").Replace("-", "").ToUpperInvariant(); // Ta bort mellanslag och bindestreck, gör om till versaler


        public static bool RegIsValid(string regNumber) // Kontrollera om registreringsnumret är giltigt
        {
            return RegLetters.IsMatch(regNumber); // Kontrollera om registreringsnumret matchar mönstret
        }



    }
}
