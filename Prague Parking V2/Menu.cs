using PragueParkingV2.Data;
using Prague_Parking_V2;
using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;
using ConsoleValidationResult = Spectre.Console.ValidationResult;

namespace Prague_Parking_V2
{
    // Kommentarer kommer att vara på svenska och kod på engelska
    // Jag har pluggat tillsammans med Noah och Hannes vid utförandet av denna inlämning

    // HUVUDKLASS FÖR MENYN
    public static class Menu
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
            "Show pricelist",
            "Park a vehicle",
            "Remove a vehicle",
            "List parked vehicles",
            "Find a vehicle",
            "Move vehicle to another spot",
            "Create new garage from config (restart of application needed)",
            "Apply config changes (with no reset of application and keep all vehicles)",
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
                    case "Show pricelist":
                        {
                            ShowPriceList();
                        }
                        break;
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
                    case "Create new garage from config (restart of application needed)":
                        var configStorage = new ConfigFiles("../../../configData.json"); // Skapar en instans av ConfigFiles med angiven sökväg
                        var configDto = configStorage.LoadOrDefault(); // Laddar konfigurationsdata från filen eller standardkonfigurationen

                        var confirmReset = AnsiConsole.Confirm("Are you sure you want to reset the garage? The garage will be renovated. This will remove all parked vehicles and update all values from the config file.");
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
                    case "Apply config changes (with no reset of application and keep all vehicles)":
                        {
                            var configPath = Path.Combine(AppContext.BaseDirectory, "../../../configData.json"); // Sökväg till konfigurationsfilen
                            var json = File.ReadAllText(configPath);
                            var options = new JsonSerializerOptions
                            {
                                AllowTrailingCommas = true,
                                ReadCommentHandling = JsonCommentHandling.Skip, // Ignorera kommentarer i JSON-filen
                                PropertyNameCaseInsensitive = true
                            };
                            var newConfig = JsonSerializer.Deserialize<ConfigApp>(json, options)
                                       ?? throw new InvalidOperationException("Failed to read config.");  // Deserialisera JSON till ConfigApp-objekt

                            int newSpaceCount = newConfig.DefaultSpaceCount;
                            int newSpaceCapacity = newConfig.DefaultSpaceCapacityUnits;

                            AnsiConsole.MarkupLine("[bold] Config changes preview: [/]");
                            AnsiConsole.MarkupLine($"Current space count: {_config.DefaultSpaceCount}, New space count: {newSpaceCount}"); // Visa nuvarande och nya värden för parkeringsplatser
                            AnsiConsole.MarkupLine($"Current space capacity units: {_config.DefaultSpaceCapacityUnits}, New space capacity units: {newSpaceCapacity}");
                            AnsiConsole.MarkupLine("[yellow] Applying config changes will attempt to resize parking spaces without removing existing vehicles. If the new configuration cannot accommodate the currently parked vehicles, the operation will fail and no changes will be made. [/]");
                            AnsiConsole.WriteLine();
                            var confirmApply = AnsiConsole.Confirm("Do you want to apply these config changes?");
                            if (!confirmApply) // Användaren avbryter ändringen
                            {
                                AnsiConsole.MarkupLine("[yellow]Config changes application cancelled.[/]");
                                Pause();
                                break;
                            }
                            if (_garage.TryResizeSpace(newSpaceCount, newSpaceCapacity, out var error)) // Försök att ändra storlek på parkeringsplatserna utan att ta bort fordon
                            {
                                _config = newConfig; // <- uppdatera aktiv config i minnet
                                _storage.Save(Mapper.ToDto(_garage));
                                AnsiConsole.MarkupLine("[green]Applied config without reset. All vehicles kept.[/]");
                            }
                            else
                            {
                                AnsiConsole.MarkupLine($"[red]Failed to apply config: {error}[/]");
                            }
                            Pause();
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

        // METOD FÖR ATT VISA PRISLISTA
        private static void ShowPriceList()
        {
            AnsiConsole.Write(new Rule("[yellow]Price list[/]").LeftJustified());

            // Visa gratis minuter innan avgift
            AnsiConsole.MarkupLine($"[green]Free minutes before charge: [bold]{_config.FreeMinutesBeforeCharge}[/] min[/]");
            AnsiConsole.WriteLine();

            // Bygg prislistan som en tabell
            var table = new Table().Border(TableBorder.Rounded);
            table.Title = new TableTitle("Vehicle pricing");
            table.AddColumn(new TableColumn("Vehicle type").LeftAligned());
            table.AddColumn(new TableColumn("Size units").Centered());
            table.AddColumn(new TableColumn("Price / hour").RightAligned());

            // Fyll tabellen med data från konfigurationen
            foreach (var spec in _config.VehicleTypes.OrderBy(v => v.Type))
            {
                table.AddRow(
                    spec.Type,
                    spec.CapacityUnits.ToString(),
                    $"{spec.ChargePerHour:0.##} CZK"
                );
            }

            AnsiConsole.Write(table);

            // Liten förklaring vartifrån priser och storlekar hämtas
            AnsiConsole.MarkupLine("[grey]Prices and sizes are read live from configData.json.[/]");
            Pause();
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

            // Nedan sker UI validering (format) och modellen validerar igen i konstruktorn
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
                vehicle.ApplySpec(spec.ChargePerHour, spec.CapacityUnits); // Här hämtas fordonsspecifikationer från config och appliceras på fordonet!!
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
        }

        // HJÄLPMETODER FÖR ATT HÄMTA AVGIFT OCH SPECIFIKATIONER
        private static double GetRate(string type) => // Hämta avgift per timme baserat på fordonstyp
            (double)_config.VehicleTypes
            .First(vehicle => vehicle.Type.Equals(type, StringComparison.OrdinalIgnoreCase))
            .ChargePerHour;

        private static VehicleSpec GetSpec(string type) => // Hämta fordonsspecifikationer baserat på fordonstyp
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
            if (confirm == "No") // Användaren avbryter borttagningen
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

        // HJÄLPMETOD FÖR ATT BESTÄMMA PARKERINGSPLATSENS FÄRG (LEDIG, DELVIS UPPTAGEN, FULL)
        private static string GetSpotColor(int used, int cap)
        {
            if (used <= 0) return "green";      // helt ledig
            if (used < cap) return "yellow";    // delvis upptagen
            return "red";                       // full
        }

        // METOD UI FÖR ATT LISTA PARKERADE FORDON
        public static void ListVehiclesUI() // Metod för att lista parkerade fordon
        {
            AnsiConsole.Write(new FigletText("GARAGE").Centered().Color(Color.Blue));

            var spaces = _garage.ParkingSpaces;
            var total = spaces.Count;

            // Bestäm antal kolumner baserat på konsolens bredd
            int bufWidth = Console.BufferWidth > 0 ? Console.BufferWidth : 120;
            int columnsPerRow = Math.Clamp(bufWidth / 12, 6, 14);

            var grid = new Table()
                .Expand()
                .Border(TableBorder.None);

            grid.ShowFooters = false;
            grid.ShowHeaders = false;

            for (int i = 0; i < columnsPerRow; i++)
            {
                grid.AddColumn(new TableColumn("").Centered());
            }

            // Bygg rader med parkeringsplatser
            var rowCells = new List<IRenderable>(columnsPerRow);
            int colIndex = 0;

            foreach (var space in spaces) // Loopar igenom varje parkeringsplats
            {
                int used = space.Vehicles.Sum(v => v.Size);
                int cap = space.CapacitySpaces;

                string color = GetSpotColor(used, cap);
                // Tvåradig kompakt cell: färgad plats-siffra + used/cap
                string cellText =
                    $"[{color}][bold]{space.Index}[/][/]\n" +
                    $"[grey]{used}/{cap}[/]";

                rowCells.Add(new Markup(cellText));

                colIndex++; // Öka kolumnindex
                if (colIndex == columnsPerRow)
                {
                    grid.AddRow(rowCells.ToArray());
                    rowCells.Clear();
                    colIndex = 0;
                }
            }


            if (rowCells.Count > 0)
            {
                while (rowCells.Count < columnsPerRow)
                    rowCells.Add(new Markup("")); // tomma celler
                grid.AddRow(rowCells.ToArray());
            }

            // Legend som förklarar färgerna
            var legend = new Markup(
                "[bold]Occupancy:[/] " +
                "[green]■[/] Free  " +
                "[yellow]■[/] Partial  " +
                "[red]■[/] Full\n"
            );

            // Rendera allt tillsammans i konsolen
            AnsiConsole.Write(legend);
            AnsiConsole.Write(new Rule());
            AnsiConsole.Write(grid);
            AnsiConsole.Write(new Rule());

            // En snabb summering av antal lediga, delvis upptagna och fulla platser
            int freeCount = spaces.Count(s => s.Vehicles.Sum(v => v.Size) == 0);
            int partialCount = spaces.Count(s =>
            {
                var u = s.Vehicles.Sum(v => v.Size);
                return u > 0 && u < s.CapacitySpaces;
            });
            int fullCount = spaces.Count - freeCount - partialCount;

            var summary = new Table().Border(TableBorder.Rounded); // Summeringstabell
            summary.AddColumn("Free");
            summary.AddColumn("Partial");
            summary.AddColumn("Full");
            summary.AddRow($"{freeCount}", $"{partialCount}", $"{fullCount}");
            AnsiConsole.Write(summary);

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
                    if (string.IsNullOrWhiteSpace(stringReg)) // Kontrollera om inmatningen är tom eller endast innehåller blanksteg
                        return ConsoleValidationResult.Error("[red]Registration number can not be left empty[/]");
                    var normal = Vehicle.FixReg(stringReg);
                    return Vehicle.RegIsValid(normal)
                            ? ConsoleValidationResult.Success()
                                : ConsoleValidationResult.Error("[red]Input a valid format[/]");
                }));

            var maxIndex = _garage.ParkingSpaces.Count; // Hämta det maximala indexet för parkeringsplatser
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






