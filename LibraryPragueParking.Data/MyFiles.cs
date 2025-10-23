using System.Text.Json;
using System.Text.Json.Serialization;
using static LibraryPragueParking.Data.DTO;


namespace LibraryPragueParking.Data
{
    public class MyFiles
    {
        string _filePath = "../../../parkingData.json";
        public static readonly JsonSerializerOptions Options = new()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public MyFiles(string filePath = null)
        {
            _filePath = filePath ?? Path.Combine(AppContext.BaseDirectory, "parkingData.json"); // Använd angiven sökväg eller standardväg
        }

        //Publika API för att ladda och spara ParkingGarage objekt
        public GarageDto? TryLoad()
        {
            if (!File.Exists(_filePath)) return null;
            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<GarageDto>(json, Options);
        }

        public void Save(GarageDto dto)
        {
            var json = JsonSerializer.Serialize(dto, Options);
            File.WriteAllText(_filePath, json);
        }
    }

}






    













