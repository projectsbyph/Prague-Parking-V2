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

            // 1) Läs config (direkt till ConfigApp)
            var configPath = Path.Combine(AppContext.BaseDirectory, "configData.json"); // Standardväg för konfigurationsfilen
            var json = File.ReadAllText(configPath);
            var jsonOptions = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                PropertyNameCaseInsensitive = true
            };
            _config = JsonSerializer.Deserialize<ConfigApp>(json, jsonOptions)!;
                     
            // 2) Läs garage-data (oförändrat)
            var storagePath = new MyFiles("../../../parkingData.json"); // Standardväg för lagringsfilen
            var savedGaragedto = storagePath.TryLoad();

            if (savedGaragedto != null && (savedGaragedto.SpaceCapacityUnits <= 0 || savedGaragedto.SpaceCount <= 0)) // Grundläggande validering av sparad data
            {
                AnsiConsole.MarkupLine("[red]Warning:[/] Invalid data in storage file. A new garage will be created using default configuration.");
                savedGaragedto = null;
            }

            var spaceCount = _config.DefaultSpaceCount > 0 ? _config.DefaultSpaceCount : 100;  // Säkerställ giltiga standardvärden
            var capPerSpace = _config.DefaultSpaceCapacityUnits > 0 ? _config.DefaultSpaceCapacityUnits : 4; // Säkerställ giltiga standardvärden

            // 3) Skapa garage enligt config
            var garage = savedGaragedto is not null
                ? Mapper.FromDto(savedGaragedto, _config)
                : new ParkingGarage(_config.DefaultSpaceCount, _config.DefaultSpaceCapacityUnits); // Nytt garage enligt config

            // 4) Initiera meny med garage + storage + config
            Menu.Init(garage, storagePath, _config);
            AnsiConsole.MarkupLine("[bold blue]Welcome to Prague Parking V2![/]");

            Menu.Run();
        }
    }
}
