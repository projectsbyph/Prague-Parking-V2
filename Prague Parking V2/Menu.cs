using Spectre.Console;
using System;
using System.Collections.Generic;
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
                    var normal = Vehicle.IsValidReg(stringReg);
                    return Vehicle.NomalizaReg(stringReg)
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

    }
}






