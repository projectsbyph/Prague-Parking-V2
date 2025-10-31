// Namespace: Prague_Parking_V2
using PragueParkingV2.Data;
using Spectre.Console;
using System;
using System.IO;
using System.Text.Json;

namespace Prague_Parking_V2
{
    internal class Program
    {
        private static ConfigApp _config = null!;

        static void Main(string[] args)
        {
            // Välkomstmeddelande
            AnsiConsole.Write(new FigletText("PRAGUE PARKING V2").Centered().Color(Color.Blue));
            AnsiConsole.MarkupLine("[slowblink]This application helps you park vehicles in Prague.[/]");

            // Läs in konfiguration och initiera applikationen
            var configPath = Path.Combine(AppContext.BaseDirectory, "configData.json"); // Standardväg för konfigurationsfilen
            var json = File.ReadAllText(configPath);
            var jsonOptions = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                PropertyNameCaseInsensitive = true
            };
            _config = JsonSerializer.Deserialize<ConfigApp>(json, jsonOptions)!;

            // Läs garage data från fil
            var storagePath = new MyFiles("../../../parkingData.json"); // Standardväg för lagringsfilen
            var savedGaragedto = storagePath.TryLoad();

            if (savedGaragedto != null && (savedGaragedto.SpaceCapacityUnits <= 0 || savedGaragedto.SpaceCount <= 0)) // Grundläggande validering av sparad data
            {
                AnsiConsole.MarkupLine("[red]Warning:[/] Invalid data in storage file. A new garage will be created using default configuration.");
                savedGaragedto = null;
            }

            var spaceCount = _config.DefaultSpaceCount > 0 ? _config.DefaultSpaceCount : 100;  // Säkerställ giltiga standardvärden
            var capPerSpace = _config.DefaultSpaceCapacityUnits > 0 ? _config.DefaultSpaceCapacityUnits : 4; // Säkerställ giltiga standardvärden

            // Skapa garage från sparad data eller ny med standardvärden
            var garage = savedGaragedto is not null
                ? Mapper.FromDto(savedGaragedto, _config)
                : new ParkingGarage(_config.DefaultSpaceCount, _config.DefaultSpaceCapacityUnits); // Nytt garage enligt config

            // Initialisera och kör menyn
            Menu.Init(garage, storagePath, _config);
            AnsiConsole.MarkupLine("[bold blue]Welcome to Prague Parking V2![/]");

            Menu.Run();
        }
    }
}
