// Namespace: Prague_Parking_V2
using LibraryPragueParking.Data;
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
            AnsiConsole.Write(new FigletText("PRAGUE PARKING V2").Centered().Color(Color.Blue));
            AnsiConsole.MarkupLine("[slowblink]This application helps you park vehicles in Prague.[/]");

            // 1) Läs config (direkt till ConfigApp)
            var configPath = Path.Combine(AppContext.BaseDirectory, "configData.json");
            var json = File.ReadAllText(configPath);
            var opts = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                PropertyNameCaseInsensitive = true
            };
            _config = JsonSerializer.Deserialize<ConfigApp>(json, opts)!;
                     
            // 2) Läs garage-data (oförändrat)
            var storagePath = new MyFiles("../../../parkingData.json");
            var dto = storagePath.TryLoad();

            if (dto != null && (dto.SpaceCapacityUnits <= 0 || dto.SpaceCount <= 0))
            {
                AnsiConsole.MarkupLine("[red]Warning:[/] Invalid data in storage file. A new garage will be created using default configuration.");
                dto = null;
            }

            var spaceCount = _config.DefaultSpaceCount > 0 ? _config.DefaultSpaceCount : 100;  
            var capPerSpace = _config.DefaultSpaceCapacityUnits > 0 ? _config.DefaultSpaceCapacityUnits : 4;

            // 3) Skapa garage enligt config (inte hårdkodat)
            var garage = dto is not null
                ? Mapper.FromDto(dto, _config)
                : new ParkingGarage(_config.DefaultSpaceCount, _config.DefaultSpaceCapacityUnits);

            AnsiConsole.MarkupLine($"[grey]CFG cap/space={_config.DefaultSpaceCapacityUnits}[/]");
            AnsiConsole.MarkupLine($"[grey]GAR first cap={garage.ParkingSpaces[0].CapacitySpaces}[/]");

            // 4) Initiera meny med garage + storage + config
            Menu.Init(garage, storagePath, _config);
            AnsiConsole.MarkupLine("[bold blue]Welcome to Prague Parking V2![/]");

            Menu.Run();
        }
    }
}
