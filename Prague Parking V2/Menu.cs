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
                            var typeVehicle = AnsiConsole.Prompt(
                                new SelectionPrompt<string>()
                                .Title("Select vehicle type to park: ")
                                .AddChoices(new[] { "Car", "Motorcycle", "Bus", "Bike", "Exit" })); // Val av fordonstyp att parkera 

                            if (typeVehicle == "Exit")
                            {
                                break;
                            }
                        }
                        break;
                    case "Remove a vehicle":
                        {
                            AnsiConsole.MarkupLine("[green]Removing a vehicle...[/]");
                            // Metod för att ta bort ett fordon
                        }
                        break;
                    case "List parked vehicles":
                        {
                            AnsiConsole.MarkupLine("[green]Listing parked vehicles...[/]");
                            // Metod för att lista parkerade fordon
                        }
                        break;
                    case "Find a vehicle":
                        {
                            AnsiConsole.MarkupLine("[green]Finding a vehicle...[/]");
                            // Metod för att hitta ett fordon
                        }
                        break;
                    case "View parking statistics":
                        {
                            AnsiConsole.MarkupLine("[green]Viewing parking statistics...[/]");
                            // Metod för att visa parkeringsstatistik
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

        private static void ParkVehicle() // Metod för att parkera ett fordon
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
                    var normal = Vehicle.RegIsValid(stringReg);
                    bool isValid = Vehicle.FixReg(normal);

                    return isValid
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Input a valid format");
                }));

            var color = AnsiConsole.Prompt(new TextPrompt<string>("Color:").DefaultValue("Black")); // default svart sätts för att undvika null för fordon utan färg

            Vehicle vehicle;
            try // Skapar fordon baserat på användarens val
            {
                vehicle = type == "Car" ? new Car(regNumber, color) // Om användaren valde "Car", skapa en Car-instans
                    : new Motorcycle(regNumber, color); // Annars skapa en Motorcycle-instans
            }
            catch (ArgumentException exceptions) // Fångar eventuella undantag vid skapandet av fordonet
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

        private static void RemoveVehicle() // Metod för att ta bort ett fordon
        {
            var regNumber = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter the registration number of the vehicle to remove:")
                .Validate(stringReg =>
                {
                    if (string.IsNullOrWhiteSpace(stringReg))
                        return ValidationResult.Error("[red]Registration number can not be left empty[/]");
                    var normal = Vehicle.IsValidReg(stringReg);
                    return Vehicle.NomalizeReg(stringReg)
                            ? ValidationResult.Success()
                                : ValidationResult.Error("[red]Input a valid format");
                }));
            if (_garage.TryRemoveVehicle(regNumber, out Vehicle removedVehicle)) // Anropar garage objektets metod för att ta bort fordonet
            {
                AnsiConsole.MarkupLine($"[green]Vehicle with registration number {removedVehicle.RegistrationNumber} removed successfully.[/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[red]No vehicle found with the provided registration number.[/]");
            }
            Pause();
        }

        private static void ListVehicles() // Metod för att lista parkerade fordon
        {
            var parkedVehicles = _garage.GetParkedVehicles(); // Hämtar listan över parkerade fordon från garage objektet
            if (parkedVehicles.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No vehicles are currently parked.[/]");
            }
            else
            {
                var table = new Table(); // Skapar en tabell för att visa fordonen
                table.AddColumn("Spot Number");
                table.AddColumn("Registration Number");
                table.AddColumn("Color");
                table.AddColumn("Type");
                foreach (var (spotNumber, vehicle) in parkedVehicles)
                {
                    table.AddRow($"#{stringReg.Index}", spotNumber.ToString(), vehicle.RegistrationNumber, vehicle.Color, vehicle.GetType().Name, vehicle.ParkedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm"));
                }
                AnsiConsole.Write(table); // Skriver ut tabellen i konsolen
            }

            if (table.Rows.Count == 0) // Om inga fordon är parkerade
            {
                AnsiConsole.MarkupLine("[yellow] No vehicles are parked yet.[/]");
            }
            else
            {
                AnsiConsole.Write(table);
            }
            Pause();
        }

        private static void FindVehiclee()
        {
            var regNumber = AnsiConsole.Prompt(new TextPrompt<string>("Search for reg number: "));
            var foundVehicles = _garage.FindVehicles(regNumber, out int spotNumber); // Anropar garage objektets metod för att hitta fordonet
            if (foundVehicles is null)
            {
                AnsiConsole.MarkupLine("[red]No vehicle found with the provided registration number.[/]");
            }
            else
            {
                AnsiConsole.MarkupLine($"[green]Vehicle found in spot {spotNumber}:[/]");
                var vehicle = foundVehicles;
                AnsiConsole.MarkupLine($"Registration Number: {vehicle.RegistrationNumber}, Color: {vehicle.Color}, Type: {vehicle.GetType().Name}, Parked At: {vehicle.ParkedAt.ToLocalTime():yyyy-MM-dd HH:mm}");
            }
            Pause();
        }

        private static void ViewStatistics() // Metod för att visa parkeringsstatistik
        {
            var stats = _garage.GetStatistics(); // Hämtar statistik från garage objektet
            AnsiConsole.MarkupLine("[bold underline]Parking Statistics:[/]");
            AnsiConsole.MarkupLine($"Total Spots: {stats.TotalSpots}");
            AnsiConsole.MarkupLine($"Occupied Spots: {stats.OccupiedSpots}");
            AnsiConsole.MarkupLine($"Available Spots: {stats.AvailableSpots}");
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
                    Motorcycle => 10m,
                    Bus => 50m,
                    Bike => 5m,
                    _ => throw new ArgumentException("Unknown vehicle type")
                };
                decimal totalFee = ratePerHour * (decimal)Math.Ceiling(duration.TotalHours);
                return totalFee;
            }
        }
    }
}






