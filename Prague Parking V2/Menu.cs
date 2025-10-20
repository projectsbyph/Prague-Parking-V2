using Spectre.Console;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prague_Parking_V2
{
    public static class Menu // Meny klass för att hantera användarinteraktion och val i konsolen (UI)
                             //Kommentarer kommer att vara på svenska och kod på engelska
    {
        //TODO: lägg in denna: private static readonly _garage = new (spotCount: 20, spotCapacity: 4); ) /

        private static readonly ParkingGarage _garage = new(20, 4); // Skapar en instans av ParkingGarage med 20 platser och kapacitet på 4 fordon per plats
        public static void Run()
        {
            var optionsMenu = new List<string> // Menyval i en lista för enkel hantering
        {
            "Park a vehicle",
            "Remove a vehicle",
            "List parked vehicles",
            "Find a vehicle",
            "View parking statistics",
            "Exit"
        };

            while (true)
            {
                string selectedOption = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                    .Title("Please select an option: ")
                    .PageSize(10)
                    .WrapAround()
                    .AddChoices(optionsMenu));

                switch (selectedOption) // Switch-case för att hantera användarens val
                {
                    case "Park a vehicle":
                        {
                            AnsiConsole.MarkupLine("[green]Parking a vehicle...[/]");
                            TryParkVehicle(); // Metod för att parkera ett fordon
                        }
                        break;
                    case "Remove a vehicle":
                        {
                            AnsiConsole.MarkupLine("[green]Removing a vehicle...[/]");
                            RemoveVehicle();
                        }
                        break;
                    case "List parked vehicles":
                        {
                            AnsiConsole.MarkupLine("[green]Listing parked vehicles...[/]");

                            ListVehicles();
                        }
                        break;
                    case "Find a vehicle":
                        {
                            AnsiConsole.MarkupLine("[green]Finding a vehicle...[/]");
                            FindVehicle();
                        }
                        break;
                    case "View parking statistics":
                        {
                            AnsiConsole.MarkupLine("[green]Viewing parking statistics...[/]");
                            ViewStatistics();
                        }
                        break;
                    case "Exit":
                        {
                            AnsiConsole.MarkupLine("[bold red]Exiting the application. Goodbye![/]");
                            return; // Avsluta programmet

                        }
                }

            }
        }

        public static void TryParkVehicle() // Metod för att parkera ett fordon
        {
            var type = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("What kind of Vehicle do you want to park?")
                .AddChoices("Car", "Mc", "Exit"));
            if (type == "Exit") { return; }

            // nedan sker UI validering (format) och modellen validerar igen i konstruktorn
            var regNumber = AnsiConsole.Prompt(
                new TextPrompt<string>("Registration number:")
                .Validate(stringReg =>
                {
                    if (string.IsNullOrWhiteSpace(stringReg))
                        return ValidationResult.Error("[red]Registration number can not be left empty[/]");
                    var normal = Vehicle.FixReg(stringReg); // Normalisera registreringsnumret
                    bool isValid = Vehicle.RegIsValid(normal); // Kontrollera om det normaliserade registreringsnumret är giltigt

                    return isValid
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Input a valid format[/]");
                }));

            var color = AnsiConsole.Prompt(new TextPrompt<string>("Color:")); // Fråga användaren om fordonets färg

            Vehicle vehicle;
            try // Skapar fordon baserat på användarens val
            {
                vehicle = type == "Car" ? new Car(regNumber, color) // Om användaren valde "Car", skapa en Car-instans
                    : new Mc(regNumber, color); // Annars skapa en Motorcycle-instans
            }
            catch (Exception exception) // Fångar eventuella undantag vid skapandet av fordonet
            {
                AnsiConsole.MarkupLine($"[red]{exception.Message}[/]"); // Visar felmeddelande i rött
                return;
            }

            if (_garage.TryParkVehicle(vehicle, out int spotNumber)) // Anropar garage objektets metod för att parkera fordonet

                AnsiConsole.MarkupLine($"[green]Vehicle parked successfully in spot {spotNumber}.[/]");

            else

                AnsiConsole.MarkupLine("[red]Parking failed. No available spots.[/]");

            Pause();
            // Fortsätt med nästa metod nedan
        }

        public static void RemoveVehicle() // Metod för att ta bort ett fordon
        {
            var regNumber = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter the registration number of the vehicle to remove:")
                .Validate(stringReg =>
                {
                    if (string.IsNullOrWhiteSpace(stringReg))
                        return ValidationResult.Error("[red]Registration number can not be left empty[/]");
                    var normal = Vehicle.FixReg(stringReg);
                    return Vehicle.RegIsValid(normal)
                            ? ValidationResult.Success()
                                : ValidationResult.Error("[red]Input a valid format[/]");
                }));
            if (_garage.TryRemoveVehicle(regNumber, out Vehicle removedVehicle)) // Anropar garage objektets metod för att ta bort fordonet
            {
                AnsiConsole.MarkupLine($"[green]Vehicle with registration number {removedVehicle.LicensePlate} removed successfully.[/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[red]No vehicle found with the provided registration number.[/]");
            }
            Pause();
        }

        public static void ListVehicles() // Metod för att lista parkerade fordon
        {
            var rows = new List<(int Spot, Vehicle vehicle)>(); // Lista för att lagra parkerade fordon och deras platsnummer
            foreach (var space in _garage.ParkingSpaces) // Loopar igenom alla parkeringsplatser i garaget
            {
                foreach (var vehicle in space.Vehicles) // Loopar igenom alla fordon på varje parkeringsplats
                {
                    rows.Add((space.Index, vehicle)); // Lägger till fordonet och dess platsnummer i listan
                }
            }

            if (rows.Count == 0) // Om inga fordon är parkerade
            {
                AnsiConsole.MarkupLine("[yellow]No vehicles are parked yet.[/]");
                Pause();
                return;
            }

            {
                var table = new Table().Border(TableBorder.Rounded); // Skapar en tabell för att visa fordonen
                table.AddColumn("Spot Number");
                table.AddColumn("Registration Number");
                table.AddColumn("Color");
                table.AddColumn("Type");
                table.AddColumn("Parked At");

                foreach (var (spot, vehicle) in rows) // Loopar igenom listan med parkerade fordon
                {
                    table.AddRow(
                        spot.ToString(),
                        vehicle.LicensePlate,
                        vehicle.Color,
                        vehicle.GetType().Name,
                        vehicle.TimeParked.ToLocalTime().ToString("yyyy-MM-dd HH:mm")); // Lägger till en rad i tabellen för varje fordon
                }

                {
                    AnsiConsole.Write(table);
                }
                Pause();
            }
        }

        public static void FindVehicle()
        {
            var regNumber = AnsiConsole.Prompt(new TextPrompt<string>("Search for reg number: "));
            var foundVehicles = _garage.FindVehicle(regNumber, out int spotNumber); // Anropar garage objektets metod för att hitta fordonet
            if (foundVehicles is null)
            {
                AnsiConsole.MarkupLine("[red]No vehicle found with the provided registration number.[/]");
            }
            else
            {
                AnsiConsole.MarkupLine($"[green]Vehicle found in spot {spotNumber}:[/]");
                var vehicle = foundVehicles;
                AnsiConsole.MarkupLine($"Registration Number: {vehicle.LicensePlate}, Color: {vehicle.Color}, Type: {vehicle.GetType().Name}, Parked At: {vehicle.TimeParked.ToLocalTime():yyyy-MM-dd HH:mm}");
            }
            Pause();
        }

        public static void ViewStatistics() // Metod för att visa parkeringsstatistik
        {
            var stats = _garage.GetParkingStats(); // Hämtar statistik från garage objektet
            AnsiConsole.MarkupLine("[bold underline]Parking Statistics:[/]");
            AnsiConsole.MarkupLine($"Total Spots: {stats.TotalSpaces}");
            AnsiConsole.MarkupLine($"Occupied Spots: {stats.OccupiedSpaces}");
            AnsiConsole.MarkupLine($"Available Spots: {stats.FreeSpaces}");
            AnsiConsole.MarkupLine($"Total Vehicles Parked: {stats.TotalVehiclesParked}");
            Pause();
        }

        private static void Pause() // Metod för att pausa programmet och vänta på användarens input
        {
            AnsiConsole.MarkupLine("[grey]Press any key to continue...[/]");
            Console.ReadKey(true);
        }

        private static class PricingToPark // Fixa parkeringsavgift
        {
            public static decimal CalculateParkingFee(Vehicle vehicle, TimeSpan duration) // Metod för att beräkna parkeringsavgift baserat på fordonstyp och parkeringstid
            {
                decimal ratePerHour = vehicle switch
                {
                    Car => 20m,
                    Mc => 10m,
                    //Bus => 50m,
                    //Bike => 5m,
                    _ => throw new ArgumentException("Unknown vehicle type")
                };
                decimal totalFee = ratePerHour * (decimal)Math.Ceiling(duration.TotalHours);
                return totalFee;
            }
        }
    }
}






