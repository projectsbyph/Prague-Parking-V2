using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prague_Parking_V2
{
    public sealed class VehicleSpec
    {
        public string Type { get; set; } = "";
        public int CapacityUnits { get; set; }
        public decimal ChargePerHour { get; set; }
    }

    public class ConfigApp
    {
        public int DefaultSpaceCount { get; set; }
        public int DefaultSpaceCapacityUnits { get; set; }
        public int FreeMinutesBeforeCharge { get; set; }

        public List<VehicleSpec> VehicleTypes { get; set; } = new();
    }

    
}
