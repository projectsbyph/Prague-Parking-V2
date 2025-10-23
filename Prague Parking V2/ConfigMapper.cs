using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryPragueParking.Data;
using static LibraryPragueParking.Data.ConfigFiles;

namespace Prague_Parking_V2
{
    internal class ConfigMapper
    {
        public static ConfigApp ToModel(ConfigAppDto dto)
        {
            var pricingByType = dto.VehicleTypes.ToDictionary(
                vt => vt.Type,
                vt => new VehiclePricing(vt.SizeUnits, vt.HourlyRate)
            );
            return new ConfigApp(
                dto.DefaultSpaceCount,
                dto.DefaultSpaceCapacityUnits,
                dto.FreeMinutesBeforeCharge,
                pricingByType
            );
        }
    }
}
