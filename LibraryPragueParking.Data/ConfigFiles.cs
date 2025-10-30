using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace LibraryPragueParking.Data
{
    

    public class ConfigFiles
    {
        // DTO-klasser för konfigurationsdata
        public sealed class ConfigAppDto // Applikationskonfiguration för dataöverföring
        {
            public int DefaultSpaceCount { get; set; } 
            public int DefaultSpaceCapacityUnits { get; set; }

            public int FreeMinutesBeforeCharge { get; set; }
            public List<VehicleTypeDto> VehicleTypes { get; set; } = new(); 
        }

        public sealed class VehicleTypeDto // Fordonstyp för dataöverföring
        {
            public string Type { get; set; } = string.Empty; // "Car" eller "Mc"
            public int CapacityUnits { get; set; }
            public decimal HourlyRate { get; set; }
        }

        // Metod för att hämta standardkonfiguration
        private static ConfigAppDto GetDefaultConfig() => DefaultConfig(); // Returnerar standardkonfigurationen till användning vid behov


        private readonly string _configFilePath;// Sökväg till konfigurationsfilen
        private static readonly JsonSerializerOptions _jsonOptions = new() // Inställningar för JSON serialisering och deserialisering
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        // Konstruktor som accepterar en valfri filväg
        public ConfigFiles(string configFilePath = null) 
        {
            _configFilePath = configFilePath ?? Path.Combine(AppContext.BaseDirectory, "configData.json"); // Standardväg för konfigurationsfilen
        }

        // Publika API för att ladda och spara konfigurationsdata
        public ConfigAppDto LoadOrDefault() // Laddar konfigurationen från fil eller returnerar standardkonfigurationen
        {
            if (!File.Exists(_configFilePath))
            {
                return GetDefaultConfig(); 
            }
            var json = File.ReadAllText(_configFilePath);
            var dto = JsonSerializer.Deserialize<ConfigAppDto>(json, _jsonOptions);
            return JsonSerializer.Deserialize<ConfigAppDto>(json, _jsonOptions) ?? GetDefaultConfig();
        }

        // Sparar konfigurationen till fil
        private static ConfigAppDto DefaultConfig() => new ConfigAppDto // Returnerar standardkonfigurationen till användning vid behov
        {
            DefaultSpaceCount = 100,
            DefaultSpaceCapacityUnits = 4,
            FreeMinutesBeforeCharge = 10,
            VehicleTypes = new() 
            {
                new VehicleTypeDto {Type = "Car", CapacityUnits = 4, HourlyRate = 20m},
                new VehicleTypeDto {Type = "Mc", CapacityUnits = 2, HourlyRate = 10m},

            }
        };
    }
}
