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
        //EGENSKAPER
        public string LicensePlate { get; } // Registreringsnummer
        public DateTime TimeParked { get; protected set; } // Tidpunkt när fordonet parkerades (i UTC)
        public int Size { get; set; } // Storlek i enheter (t.ex. bil = 4, mc = 2)
        public decimal HourlyRate { get; private set; } // Timpris för parkering
        public string Type { get; set; } // Typ av fordon (t.ex. "Car", "Mc")


        //KONSTRUKTOR
        protected Vehicle(string licensePlate, string type)
        {
            LicensePlate = FixReg(licensePlate);
            Type = type;
            TimeParked = DateTime.UtcNow;

            var normalizedReg = FixReg(licensePlate);
            if (!RegIsValid(normalizedReg))
            {
                throw new ArgumentException("Invalid license plate format.");
            }
            LicensePlate = normalizedReg;
        }

        public void ApplySpec(decimal hourlyRate, int sizeUnits)
        {
            HourlyRate = hourlyRate;
            Size = sizeUnits;
        }

        //METODER (Hjälpare för UI att visa fordonets info)
        public static string FixReg(string regNumber) // Rensa och formatera registreringsnumret

            => regNumber.Trim().Replace(" ", "").Replace("-", "").ToUpperInvariant(); // Ta bort mellanslag och bindestreck, gör om till versaler


        public static bool RegIsValid(string regNumber) // Kontrollera om registreringsnumret är giltigt
        {
            if (regNumber.Length > 10) // Max 10 tecken
            {
                Console.Clear();
                AnsiConsole.MarkupLine("[red]Your license plate cannot contain more than 10 characters![/]\n");
                AnsiConsole.MarkupLine("\n\nPress any [slowblink]key[/] to go back to the [yellow]menu[/]...");
                Console.ReadKey();
                Console.Clear();
                return false;
            }
            if (regNumber.Length == 0) // Minst 1 tecken
            {
                Console.Clear();
                AnsiConsole.MarkupLine("[red]You must enter a license plate number![/]\n");
                AnsiConsole.MarkupLine("\n\nPress any [slowblink]key[/] to go back to the [yellow]menu[/]...");
                Console.ReadKey();
                Console.Clear();
                return false;
            }
            if (regNumber.Any(char.IsWhiteSpace)) // Inga mellanslag tillåtna
            {
                Console.Clear();
                AnsiConsole.MarkupLine("[red]Your license plate cannot contain spaces![/]\n");
                AnsiConsole.MarkupLine("\n\nPress any [slowblink]key[/] to go back to the [yellow]menu[/]...");
                Console.ReadKey();
                Console.Clear();
                return false;
            }
            return true;
        }

        public static bool RegExists(string regNumber, List<ParkingSpace> parkingSpaces) // Kontrollera om registreringsnumret redan finns i garaget
        {
            var fixedReg = FixReg(regNumber);
            foreach (var space in parkingSpaces)
            {
                var existingVehicle = space.Vehicles.FirstOrDefault(v => v.LicensePlate == fixedReg); // Kontrollera om fordonet redan är parkerat med Linq uttryck
                if (existingVehicle != null)
                {
                    Console.Clear();
                    AnsiConsole.MarkupLine($"[red]A vehicle with the license plate {fixedReg} is already parked in the garage![/]\n");
                    AnsiConsole.MarkupLine("\n\nPress any [slowblink]key[/] to go back to the [yellow]menu[/]...");
                    Console.ReadKey();
                    Console.Clear();
                    return true;
                }
            }
            return false;
        }

        //METODER (Hjälpare för att spara och ladda fordonets parkerings tidpunkt)
        public void RestoreParkedAtUtc(DateTime parkedAtUtc) // Återställ parkerad tidpunkt från sparad UTC tid
        {
            TimeParked = DateTime.SpecifyKind(parkedAtUtc, DateTimeKind.Utc);
        }

        public void MarkParkedNow() // Sätt parkerad tidpunkt till nuvarande UTC tid
        {
            TimeParked = DateTime.UtcNow;
        }
    }
}

