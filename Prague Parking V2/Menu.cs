using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prague_Parking_V2;
using LibraryPragueParking.Data;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations;
using ConsoleValidationResult = Spectre.Console.ValidationResult;
using System.Threading.Tasks.Sources;

namespace Prague_Parking_V2
{
    public static class Menu // Meny klass för att hantera användarinteraktion och val i konsolen (UI)
                             //Kommentarer kommer att vara på svenska och kod på engelska
    {
        // FÖR JSON LAGRING
        private static ParkingGarage _garage = null!;
        private static MyFiles _storage = null!;
        private static ConfigApp _config = null!;

        public static void Init(ParkingGarage garage, MyFiles storage, ConfigApp config) // Initierar menyn med parkeringsgaraget och lagringsvägen
        {
            _garage = garage; // Parkeringsgarage objekt
            _storage = storage; // Lagringsväg för att spara och ladda garagets tillstånd
            _config = config; // Konfigurationsobjekt
        }

        // METOD - HUVUDMENY
        public static void Run()
        {
            var optionsMenu = new List<string> // Menyval i en lista för enkel hantering
        {
            "Park a vehicle",
            "Remove a vehicle",
            "List parked vehicles",
            "Find a vehicle",
            "Move vehicle to another spot", // Framtida funktionalitet
            "Reset garage from config file",
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
                            ParkVehicleUI(); // Metod för att parkera ett fordon
                            _storage.Save(Mapper.ToDto(_garage)); // Spara garagets tillstånd efter att ett fordon har parkerats

                        }
                        break;
                    case "Remove a vehicle":
                        {
                            AnsiConsole.MarkupLine("[green]Removing a vehicle...[/]");
                            RemoveVehicleUI();
                            _storage.Save(Mapper.ToDto(_garage)); // Spara garagets tillstånd efter att ett fordon har tagits bort
                        }
                        break;
                    case "List parked vehicles":
                        {
                            AnsiConsole.MarkupLine("[green]Listing parked vehicles...[/]");

                            ListVehiclesUI();
                        }
                        break;
                    case "Find a vehicle":
                        {
                            AnsiConsole.MarkupLine("[green]Finding a vehicle...[/]");
                            FindVehicleUI();
                        }
                        break;
                    case "Move vehicle to another spot": // Framtida funktionalitet
                        {
                            AnsiConsole.MarkupLine("[green]Moving vehicle...[/]");
                            MoveVehicleUI();
                            _storage.Save(Mapper.ToDto(_garage)); // Spara garagets tillstånd efter att ett fordon har flyttats
                        }
                        break;
                    case "Reset garage from config file":
                        var cfgStorage = new ConfigFiles("../../../configData.json"); // Skapar en instans av ConfigFiles med angiven sökväg
                        var configDto = cfgStorage.LoadOrDefault(); // Laddar konfigurationsdata från filen eller standardkonfigurationen

                        var confirmReset = AnsiConsole.Confirm("Are you sure you want to reset the garage? This will remove all parked vehicles and update all values from the config file.");
                        if (confirmReset)
                        {
                            _garage = new ParkingGarage(_config.DefaultSpaceCount, _config.DefaultSpaceCapacityUnits);
                            AnsiConsole.MarkupLine("[green]Garage has been reset to default configuration.[/]");
                            _storage.Save(Mapper.ToDto(_garage)); // Spara garagets tillstånd efter återställning
                            Menu.Init(_garage, _storage, _config); // Re-initialisera menyn med det nya garaget
                            AnsiConsole.MarkupLine("[green]Menu re-initialized with the new garage.[/]");
                        }
                        else
                        {
                            AnsiConsole.MarkupLine("[yellow]Garage reset cancelled.[/]");
                        }
                        break;
                    case "Exit":
                        {
                            AnsiConsole.MarkupLine("[bold red]Exiting the application. Goodbye![/]");
                            _storage.Save(Mapper.ToDto(_garage)); // Spara garagets tillstånd innan programmet avslutas
                            return; // Avsluta programmet


                        }
                }

            }
        }

        // METOD UI FÖR ATT PARKERA FORDON
        public static void ParkVehicleUI() // Metod för att parkera ett fordon
        {
            var type = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("What kind of Vehicle do you want to park?")
                .AddChoices("Car", "Mc", "Bus", "Bike", "Exit"));
            if (type == "Exit")
            { return; }

            if (type == "Bus" || type == "Bike") // Hantera ogiltiga fordonstyper
            {
                AnsiConsole.MarkupLine("[yellow] Will be implemented in the next update.[/]");
                Pause();
                return;
            }

            // nedan sker UI validering (format) och modellen validerar igen i konstruktorn
            var regNumber = AnsiConsole.Prompt(
                new TextPrompt<string>("Registration number:")
                .Validate(stringReg =>
                {

                    var normalizedReg = Vehicle.FixReg(stringReg); // Normalisera registreringsnumret
                    bool isValid = Vehicle.RegIsValid(normalizedReg); // Kontrollera om det normaliserade registreringsnumret är giltigt

                    return isValid
                        ? ConsoleValidationResult.Success()
                        : ConsoleValidationResult.Error("[red]Input a valid format[/]");
                }));

            if (Vehicle.RegExists(regNumber, _garage.ParkingSpaces)) // Kontrollera om fordonet redan är parkerat
            {
                AnsiConsole.MarkupLine("[red]A vehicle with this registration number is already parked in the garage.[/]");
                Pause();
                return;
            }

            Vehicle vehicle;
            try // Skapar fordon baserat på användarens val
            {
                vehicle = type == "Car" ? new Car(regNumber) // Om användaren valde "Car", skapa en Car-instans
                    : new Mc(regNumber); // Annars skapa en Motorcycle-instans
                var spec = GetSpec(type);
                vehicle.ApplySpec(spec.ChargePerHour, spec.CapacityUnits); // Applicera fordonsspecifikationer från config
            }
            catch (Exception exception) // Fångar eventuella undantag vid skapandet av fordonet
            {
                AnsiConsole.MarkupLine($"[red]{exception.Message}[/]"); // Visar felmeddelande i rött
                return;
            }

            if (_garage.TryParkVehicle(vehicle, out int spotNumber)) // Anropar garage objektets metod för att parkera fordonet
            {
                AnsiConsole.MarkupLine($"[green]Vehicle parked successfully in spot {spotNumber}.[/]");
                _storage.Save(Mapper.ToDto(_garage)); // Spara garagets tillstånd efter att ett fordon har parkerats
            }

            else
            {
                AnsiConsole.MarkupLine("[red]Parking failed. No available spots.[/]");
            }

            Pause();
            // Fortsätt med nästa metod nedan
        }

        private static double GetRate(string type) =>
            (double)_config.VehicleTypes
            .First(vehicle => vehicle.Type.Equals(type, StringComparison.OrdinalIgnoreCase))
            .ChargePerHour;

        private static VehicleSpec GetSpec(string type) =>
           _config.VehicleTypes
           .First(vehicle => vehicle.Type.Equals(type, StringComparison.OrdinalIgnoreCase));


        // METOD UI FÖR ATT TA BORT FORDON
        public static void RemoveVehicleUI() // Metod för att ta bort ett fordon
        {
            var regNumber = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter the registration number of the vehicle to remove:")
                .Validate(stringReg =>
                {
                    if (string.IsNullOrWhiteSpace(stringReg))
                        return ConsoleValidationResult.Error("[red]Registration number can not be left empty[/]");
                    var normal = Vehicle.FixReg(stringReg);
                    return Vehicle.RegIsValid(normal)
                            ? ConsoleValidationResult.Success()
                                : ConsoleValidationResult.Error("[red]Input a valid format[/]");
                }));

            //Hitta forodnet först för att kunna visa kvitto
            var vehicle = _garage.FindVehicle(regNumber, out int spotNumber); // Anropar garage objektets metod för att hitta fordonet
            if (vehicle is null)
            {
                AnsiConsole.MarkupLine("[red]No vehicle found with the provided registration number.[/]");
                Pause();
                return;
            }

            //Beräkna pris innan borttagning
            var nowUtc = DateTime.UtcNow;
            var parkingFee = CalculateParkingFee(vehicle, nowUtc, _config, out var total);

            //Visa kvitto
            var receiptTable = new Table().Border(TableBorder.Rounded).BorderColor(Color.Cyan1);
            receiptTable.AddColumn("[bold]Field[/]");
            receiptTable.AddColumn("[bold]Receipt[/]");
            receiptTable.AddRow("Registration Number", vehicle.LicensePlate);
            receiptTable.AddRow("Vehicle Type", vehicle.GetType().Name);
            receiptTable.AddRow("From", vehicle.TimeParked.ToLocalTime().ToString("yyyy-MM-dd HH:mm"));
            receiptTable.AddRow("To", nowUtc.ToLocalTime().ToString("yyyy-MM-dd HH:mm"));
            receiptTable.AddRow("Total time parked", $"{(int)total.TotalHours} hours and {total.Minutes} minutes");
            receiptTable.AddRow("Free parking time", $"{_config.FreeMinutesBeforeCharge} minutes");
            receiptTable.AddRow("Price per hour", vehicle is Car ? $"{GetRate("Car")} CZK" : $"{GetRate("Mc")} CZK");
            receiptTable.AddRow("Parking Fee", $"{parkingFee} CZK");
            AnsiConsole.Write(new Rule("[yellow]Parking Receipt[/]"));
            AnsiConsole.Write(receiptTable);

            //Användaren bekräftar betalning
            var confirm = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("Confirm payment and remove vehicle?")
                .AddChoices("Yes", "No"));
            if (confirm == "No")
            {
                AnsiConsole.MarkupLine("[yellow]Payment cancelled. Vehicle not removed.[/]");
                Pause();
                return;
            }


            // Nedan anropas garagets metod för att ta bort fordonet
            if (_garage.TryRemoveVehicle(regNumber, out Vehicle removedVehicle)) // Anropar garage objektets metod för att ta bort fordonet
            {
                AnsiConsole.MarkupLine($"[green]Vehicle with registration number {removedVehicle.LicensePlate} removed successfully.[/]");
                _storage.Save(Mapper.ToDto(_garage)); // Spara garagets tillstånd efter att ett fordon har tagits bort
            }
            else
            {
                AnsiConsole.MarkupLine("[red]No vehicle found with the provided registration number.[/]");
            }
            Pause();
        }

        // METOD UI FÖR ATT LISTA PARKERADE FORDON
        public static void ListVehiclesUI() // Metod för att lista parkerade fordon
        {
            AnsiConsole.Write(new FigletText("GARAGE").Centered().Color(Color.Blue)); // Visar en figlet text "GARAGE" centrerad och i grön färg))

            //var rows = new List<(int Spot, Vehicle vehicle)>(); // Lista för att lagra parkerade fordon och deras platsnummer
            foreach (var space in _garage.ParkingSpaces) // Loopar igenom alla parkeringsplatser i garaget
            {
                // Lägger till varje parkerat fordon i listan med dess platsnummer och separerar dem
                var usedSpaceUnits = space.Vehicles.Sum(v => v.Size); // Beräknar det använda utrymmet i parkeringsplatsen
                var header = $"[yellow]Parking Spot {space.Index}[/] - Used: {usedSpaceUnits}/{space.CapacitySpaces} units"; // Skapar en rubrik för parkeringsplatsen med dess index och använda kapacitet
                var rule = new Rule(header) { Justification = Justify.Left }; // Skapar en regel med rubriken
                AnsiConsole.Write(rule); // Visar regeln i konsolen

                // Visuell representation av kapaciteten med gröna och grå block nedan
                var unitsBar = string.Concat(Enumerable.Range(1, space.CapacitySpaces).Select(i => i <= usedSpaceUnits ? "[green]█[/]" : "[grey]█[/]")); // Skapar en visuell representation av kapaciteten med gröna och grå block
                AnsiConsole.MarkupLine(unitsBar); // Visar kapacitetsbaren i konsolen

                if (space.Vehicles.Count == 0) // Om inga fordon är parkerade på platsen
                {
                    AnsiConsole.MarkupLine("[grey]-- No vehicles parked in this spot --[/]");
                    continue; // Hoppar till nästa iteration av loopen
                }

                // Visar tabellen med parkerade fordon
                var table = new Table().Border(TableBorder.Rounded); // Skapar en tabell för att visa fordonen
                table.AddColumn("Space units used");
                table.AddColumn("Registration number");
                table.AddColumn("Type");
                table.AddColumn("Parked at");

                // Lägger till rader i tabellen för varje parkerat fordon
                foreach (var v in space.Vehicles)
                {
                    table.AddRow(
                        v.Size.ToString(), // Använda utrymmes enheter
                        v.LicensePlate, // Registreringsnummer
                        v.GetType().Name, // Fordonstyp
                        v.TimeParked.ToLocalTime().ToString("yyyy-MM-dd HH:mm")); // Tidpunkt när fordonet parkerades
                }
                AnsiConsole.Write(table); // Visar tabellen i konsolen
            }
            AnsiConsole.Write(new Rule()); // Visar en avslutande regel i konsolen

            Pause();

        }

        // METOD UI FÖR ATT HITTA FORDON
        public static void FindVehicleUI()
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
                AnsiConsole.MarkupLine($"Registration Number: {vehicle.LicensePlate}, Type: {vehicle.GetType().Name}, Parked At: {vehicle.TimeParked.ToLocalTime():yyyy-MM-dd HH:mm}");
            }
            Pause();
        }

        // METOD UI FÖR ATT FLYTTA FORDON
        public static void MoveVehicleUI() // Metod för att flytta ett fordon till en annan parkeringsplats (framtida funktionalitet)
        {
            var regNumber = AnsiConsole.Prompt(new TextPrompt<string>("Enter the registration number of the vehicle to move: ")
                .Validate(stringReg =>
                {
                    if (string.IsNullOrWhiteSpace(stringReg))
                        return ConsoleValidationResult.Error("[red]Registration number can not be left empty[/]");
                    var normal = Vehicle.FixReg(stringReg);
                    return Vehicle.RegIsValid(normal)
                            ? ConsoleValidationResult.Success()
                                : ConsoleValidationResult.Error("[red]Input a valid format[/]");
                }));

            var maxIndex = _garage.ParkingSpaces.Count;
            var targetSpot = AnsiConsole.Prompt(
                new TextPrompt<int>($"Enter to which spot you want to move the vehicle " +
                $"(0 to {maxIndex}): ")
                .Validate(index =>
                {
                    return (index >= 1 && index <= maxIndex)
                        ? ConsoleValidationResult.Success()
                        : ConsoleValidationResult.Error($"[red]Please enter a valid spot index between 0 and {maxIndex}[/]");
                }));

            if (_garage.MoveVehicle(regNumber, targetSpot, out int fromIndex, out string? error)) // Anropar garage objektets metod för att flytta fordonet
            {
                AnsiConsole.MarkupLine($"[green]Vehicle with registration number {regNumber} moved successfully from spot {fromIndex} to spot {targetSpot}.[/]");
            }
            else
            {
                AnsiConsole.MarkupLine($"[red]Failed to move vehicle: {error}[/]");

            }

            Pause();
        }

        // HJÄLPMETOD FÖR ATT PAUSA PROGRAMMET
        private static void Pause() // Metod för att pausa programmet och vänta på användarens input
        {
            AnsiConsole.MarkupLine("[grey]Press any key to continue...[/]");
            Console.ReadKey(true);
        }

        // KLASS FÖR ATT BERÄKNA PARKERINGSAVGIFT 
        public static decimal CalculateParkingFee(Vehicle vehicle, DateTime checkoutUtc, ConfigApp cfg, out TimeSpan total) // Metod för att beräkna parkeringsavgift baserat på fordonstyp och parkeringstid
        {
            var startUtc = vehicle.TimeParked; // Tidpunkt när fordonet parkerades
            total = checkoutUtc - startUtc; // Total parkeringstid

            var totalMinutes = (decimal)total.TotalMinutes;
            if (total.TotalMinutes <= cfg.FreeMinutesBeforeCharge) // Om parkeringstiden är inom den fria tiden
            {
                return 0m; // Ingen avgift
            }

            var pricing = vehicle is Car ? GetRate("Car") : GetRate("Mc"); // Avgift per timme baserat på fordonstyp
            var billableMinutes = totalMinutes - cfg.FreeMinutesBeforeCharge; // Beräknar debiterbara minuter efter att ha dragit av den fria tiden
            var billableHours = Math.Ceiling(billableMinutes / 60m); // Runda upp till närmaste timme
            return billableHours * (decimal)pricing; // Beräknar total avgift
        }

    }
}






