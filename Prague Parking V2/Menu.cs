using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prague_Parking_V2
{
    public static class Menu
    {
        //TODO: lägg in denna: private static readonly _garage = new (spotCount: 20, spotCapacity: 4); ) /

        public static void Run()
        {
            var optionsMenu = new List<string>
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

                switch (selectedOption)
                {
                    case "Park a vehicle":
                        {
                            var typeVehicle = AnsiConsole.Prompt(
                                new SelectionPrompt<string>()
                                .Title("Select vehicle type to park: ")
                                .AddChoices(new[] { "Car", "Motorcycle", "Bus", "Bike", "Exit" }));

                            if (typeVehicle == "Exit")
                            {
                                break;
                            }
                        }
                        break;
                    case "Remove a vehicle":
                        {
                            AnsiConsole.MarkupLine("[green]Removing a vehicle...[/]");
                            // Call the method to remove a vehicle
                        }
                        break;
                    case "List parked vehicles":
                        {
                            AnsiConsole.MarkupLine("[green]Listing parked vehicles...[/]");
                            // Call the method to list parked vehicles
                        }
                        break;
                    case "Find a vehicle":
                        {
                            AnsiConsole.MarkupLine("[green]Finding a vehicle...[/]");
                            // Call the method to find a vehicle
                        }
                        break;
                    case "View parking statistics":
                        {
                            AnsiConsole.MarkupLine("[green]Viewing parking statistics...[/]");
                            // Call the method to view parking statistics
                        }
                        break;
                    case "Exit":
                        {
                            AnsiConsole.MarkupLine("[bold red]Exiting the application. Goodbye![/]");
                            return;

                        }
                }

            }
        }

        private static void ParkVehicle()
        {
            var type = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("Vad vill du parkera?")
                .AddChoices("Car", "Mc", "Exit"));
            if (type == "Exit") { return; }

            // nedan sker UI validering (format) och modellen validerar igen i konstruktorn
            var regNumber = AnsiConsole.Prompt(
                new TextPrompt<string>("Registreringsnummer:")
                .Validate(stringReg =>
                {
                    if (string.IsNullOrWhiteSpace(stringReg))
                        return ValidationResult.Error("[red]Registreringsnummer får inte vara tomt[/]");
                    var normal = Vehicle.IsValidReg(stringReg);
                    return Vehicle.NomalizaReg(stringReg)
                            ? ValidationResult.Success()
                                : ValidationResult.Error("[red]Ange ett giltigt format");
                }));

            var color = AnsiConsole.Prompt(new TextPrompt<string>("Färg:").DefaultValue("Svart")); // default svart sätts för att undvika null för fordon utan färg

            Vehicle vehicle;
            try
            {
                vehicle = type == "Car" ? new Car(regNumber, color)
                    : new Motorcycle(regNumber, color);
            }
            catch (ArgumentException ex)
            {
                AnsiConsole.MarkupLine($"[red]{ex.Message}[/]");
                return;
            }

            if (_garage.TryParkVehicle(vehicle, out int spotNumber))
            
                AnsiConsole.MarkupLine($"[green]Vehicle parked successfully in spot {spotNumber}.[/]");
            
            else
            
                AnsiConsole.MarkupLine("[red]Parking failed. No available spots.[/]");

                Pause();
            





            
        }

    }
}






