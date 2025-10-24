using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryPragueParking.Data
{
    public class DTO
    {
        //Här ned finns allt som behövs för att serialisera och deserialisera ParkingGarage objektet till och från JSON format.
        public class GarageDto
        {
            public int SpaceCapacityUnits { get; set; }
            public int SpaceCount { get; set; }
            public List<SpaceDto> Spaces { get; set; } = new();
        }
        
        public class SpaceDto //Denna klass representerar en parkeringsplats och dess egenskaper för serialisering.
        {
            public int Index { get; set; }
            public List<VehicleDto> Vehicle { get; set; } = new();
        }

        public class VehicleDto //Denna klass representerar ett fordon och dess egenskaper för serialisering.
        {
            public string Type { get; set; } = string.Empty; // "Car" eller "Mc"
            public string LicensePlate { get; set; } = string.Empty;
            public DateTime ParkedAtUtc { get; set; }
        }
    }

}
