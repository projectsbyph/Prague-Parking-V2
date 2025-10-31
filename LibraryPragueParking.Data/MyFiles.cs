using System.Text.Json;
using System.Text.Json.Serialization;
using static PragueParkingV2.Data.ParkingGarageDto;


namespace PragueParkingV2.Data
{
    public class MyFiles //Hanterar filoperationer för att ladda och spara ParkingGarage data i JSON format.
    {
        string _filePath = "../../../parkingData.json";
        public static readonly JsonSerializerOptions Options = new() //Inställningar för JSON serialisering och deserialisering
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public MyFiles(string filePath = null) //Konstruktor som accepterar en valfri filväg
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

        public void Save(GarageDto dto) //Sparar ParkingGarage objektet till fil (den som används nu)
        {
            var json = JsonSerializer.Serialize(dto, Options);
            File.WriteAllText(_filePath, json);
        }
    }

}






    













