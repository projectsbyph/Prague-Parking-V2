using Prague_Parking_V2;
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



        Menu.Run(); //Anropar menyn som hanterar användarinteraktion
    }
}









//ANROPA MENU.CS OCH STARTAR PROGRAMMET. HÄR SKA ENDAST SWITCHCASE FINNAS SOM ANROPAR OLIKA METODER I MENU.CS



