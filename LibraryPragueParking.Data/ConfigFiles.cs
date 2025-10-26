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
        public sealed class ConfigAppDto
        {
            public int DefaultSpaceCount { get; set; }
            public int DefaultSpaceCapacityUnits { get; set; }

            public int FreeMinutesBeforeCharge { get; set; }
            public List<VehicleTypeDto> VehicleTypes { get; set; } = new();
        }

        public sealed class VehicleTypeDto
        {
            public string Type { get; set; } = string.Empty; // "Car" eller "Mc"
            public int SizeUnits { get; set; }
            public decimal HourlyRate { get; set; }
        }

        private static ConfigAppDto GetDefaultConfig() => DefaultConfig();


        private readonly string _configFilePath;
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public ConfigFiles(string configFilePath = null)
        {
            _configFilePath = configFilePath ?? Path.Combine(AppContext.BaseDirectory, "configData.json");
        }

        public ConfigAppDto LoadOrDefault()
        {
            if (!File.Exists(_configFilePath))
            {
                return GetDefaultConfig(); 
            }
            var json = File.ReadAllText(_configFilePath);
            var dto = JsonSerializer.Deserialize<ConfigAppDto>(json, _jsonOptions);
            return JsonSerializer.Deserialize<ConfigAppDto>(json, _jsonOptions) ?? GetDefaultConfig();
        }

        public void Save(ConfigAppDto config)
        {
            var json = JsonSerializer.Serialize(config, _jsonOptions);
            File.WriteAllText(_configFilePath, json);
        }

        private static ConfigAppDto DefaultConfig() => new ConfigAppDto
        {
            DefaultSpaceCount = 100,
            DefaultSpaceCapacityUnits = 4,
            FreeMinutesBeforeCharge = 10,
            VehicleTypes = new()
            {
                new VehicleTypeDto {Type = "Car", SizeUnits = 4, HourlyRate = 20m}, 
                new VehicleTypeDto {Type = "Mc", SizeUnits = 2, HourlyRate = 10m},

            }
        };
    }
}
