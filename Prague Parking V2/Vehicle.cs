using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Prague_Parking_V2
{
    public abstract class Vehicle
    {
        //FÄLT
        private static readonly Regex RegLetters = new("^[A-Z0-9]{3,10}$", RegexOptions.Compiled); //Kolla denna line

        //EGENSKAPER
        public string LicensePlate { get; }
        public DateTime TimeParked { get; } = DateTime.Now;

        public abstract int Size { get; }
        public abstract string Type { get; }


        //KONSTRUKTOR
        protected Vehicle(string licensePlate)
        {
            var normalizedReg = FixReg(licensePlate);
            if (!RegIsValid(normalizedReg))
            {
                throw new ArgumentException("Invalid license plate format.");
            }
            LicensePlate = normalizedReg;
        }

        public override string ToString()
        {
            return $"{Type} - License Plate: {LicensePlate}, Parked At: {TimeParked}";
        }

        //METODER (Hjälpare för UI att visa fordonets info)
        public static string FixReg(string regNumber) // Rensa och formatera registreringsnumret

            => regNumber.Trim().Replace(" ", "").Replace("-", "").ToUpperInvariant(); // Ta bort mellanslag och bindestreck, gör om till versaler


        public static bool RegIsValid(string regNumber) // Kontrollera om registreringsnumret är giltigt
        {
            return !string.IsNullOrEmpty(regNumber) && RegLetters.IsMatch(regNumber);
        }



    }
}
