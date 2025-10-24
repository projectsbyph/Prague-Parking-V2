using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prague_Parking_V2
{
    public class ConfigApp
    {
        public int DefaultSpaceCount { get; }
        public int DefaultSpaceCapacityUnits { get; }
        public int FreeMinutesBeforeCharge { get; set; } = 10;
        public int CarPricePerHour { get; set; } = 20;
        public int McPricePerHour { get; set; } = 10;
        public IReadOnlyDictionary<string, VehiclePricing> PricingByType { get; }
        public ConfigApp(int defaultSpaceCount, int defaultSpaceCapacityUnits, int freeMinutesBeforeCharge, IDictionary<string, VehiclePricing> pricingByType)
        {
            DefaultSpaceCount = defaultSpaceCount;
            DefaultSpaceCapacityUnits = defaultSpaceCapacityUnits;
            FreeMinutesBeforeCharge = freeMinutesBeforeCharge;
            PricingByType = new Dictionary<string, VehiclePricing>(pricingByType);
        }
    }

    public class VehiclePricing
    {
        public int SizeUnits { get; }
        public decimal HourlyRate { get; }
        public VehiclePricing(int sizeUnits, decimal hourlyRate)
        {
            SizeUnits = sizeUnits;
            HourlyRate = hourlyRate;
        }
    }
}
