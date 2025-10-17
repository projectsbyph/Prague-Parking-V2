using Prague_Parking_V2;
using Spectre.Console;

namespace PragueParkingV2;

internal class Program
{
    static void Main(string[] args)
    {
        AnsiConsole.MarkupLine("[bold blue]Welcome to Prague Parking V2![/]");
        AnsiConsole.MarkupLine("[slowblink]This application helps you park vehicles in Prague.[/]");
        Style first = new(

        foreground: Color.Blue,
        background: Color.White,
        decoration: Decoration.Underline | Decoration.SlowBlink);



        Menu.Run();
    }
}









//ANROPA MENU.CS OCH STARTAR PROGRAMMET. HÄR SKA ENDAST SWITCHCASE FINNAS SOM ANROPAR OLIKA METODER I MENU.CS



