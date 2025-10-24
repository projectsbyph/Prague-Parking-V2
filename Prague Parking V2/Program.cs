using Prague_Parking_V2;
using LibraryPragueParking.Data;
using Spectre.Console;

namespace PragueParkingV2;

internal class Program //Kommentarer kommer att vara på svenska och kod på engelska
{
    static void Main(string[] args)
    {
        AnsiConsole.Write(new FigletText("PRAGUE PARKING V2").Centered().Color(Color.Blue));
        AnsiConsole.MarkupLine("[slowblink]This application helps you park vehicles in Prague.[/]");
        Style first = new(

        foreground: Color.Blue,
        background: Color.White,
        decoration: Decoration.Underline | Decoration.SlowBlink);

        //LÄS CONGIG
        var cfgStorage = new ConfigFiles("../../../configData.json"); //Skapar en instans av ConfigFiles med angiven sökväg)
        var configDto = cfgStorage.LoadOrDefault(); //Laddar konfigurationsdata från filen eller standardkonfigurationen
        var config = ConfigMapper.ToModel(configDto); //Mapper från DTO till ConfigApp modell

        //LÄS GARAGE DATA
        var storagePath = new MyFiles("../../../parkingData.json"); //Skapar en instans av MyFiles med angiven sökväg
        var dto = storagePath.TryLoad(); //Försöker ladda parkeringsgaraget från filen

        //SKAPA GARAGE FRÅN CONFIG
        var garage = dto is not null
            ? Mapper.FromDto(dto) //Om dto inte är null, mappa från DTO till ParkingGarage
            : new ParkingGarage(100, 4); //Annars skapa ett nytt ParkingGarage med 10 platser och kapacitet 4

        //INITIALISERA MENY MED GARAGE + LAGRING + CONFIG
        Menu.Init(garage, storagePath, config); //Initierar menyn med parkeringsgaraget och lagringsvägen
        AnsiConsole.MarkupLine("[bold blue]Welcome to Prague Parking V2![/]"); //Välkomstmeddelande detta finns redan ta bort?

        Menu.Run(); //Anropar menyn som hanterar användarinteraktion
    }
}









//ANROPA MENU.CS OCH STARTAR PROGRAMMET. HÄR SKA ENDAST SWITCHCASE FINNAS SOM ANROPAR OLIKA METODER I MENU.CS



