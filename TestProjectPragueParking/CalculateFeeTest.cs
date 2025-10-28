using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prague_Parking_V2;
using LibraryPragueParking.Data;

namespace TestProjectPragueParking
{
    [TestClass]
    public class CalculateFeeTest
    {
        [TestInitialize]
        public void InitMenuConfig()
        {
            var garage = new ParkingGarage(spaceCount: 1, spaceCapacityUnits: 4);
            var storage = new MyFiles(System.IO.Path.Combine(AppContext.BaseDirectory, "testData.json"));

            var cfg = new ConfigApp
            {
                DefaultSpaceCount = 100,
                DefaultSpaceCapacityUnits = 4,
                FreeMinutesBeforeCharge = 10,
                VehicleTypes = new()
                {
                    new VehicleSpec { Type = "Car", CapacityUnits = 2, ChargePerHour = 20m },
                    new VehicleSpec { Type = "Mc", CapacityUnits = 1, ChargePerHour = 10m }
                }
            };

            Menu.Init(garage, storage, cfg);
        }

        [TestMethod]
        public void Fee_Is_Zero_When_Parked_Less_Than_Free_Minutes() // Test case where parked time is less than free minutes
        {
            var car = new Car("ABC123");
            car.ApplySpec(20m, 4);
            car.RestoreParkedAtUtc(DateTime.UtcNow.AddMinutes(-9));

            var cfg = new ConfigApp
            {
                FreeMinutesBeforeCharge = 10
            };

            var fee = Menu.CalculateParkingFee(car, DateTime.UtcNow, cfg, out var total);

            Assert.AreEqual(0m, fee, "The fee should be 0m when total time <= free time."); 
        }

        [TestMethod]
        public void Fee_Rounds_Up_When_Parked_Less()
        {
             var car = new Car("ABC123");
            car.ApplySpec(20m, 4);
            car.RestoreParkedAtUtc(DateTime.UtcNow.AddMinutes(-65));
            
            var cfg = new ConfigApp
            {
                FreeMinutesBeforeCharge = 10
            };
            var fee = Menu.CalculateParkingFee(car, DateTime.UtcNow, cfg, out var total);
            
            Assert.AreEqual(20m, fee, "55 minutes of billable time should be rounded up to 1 hour at 20m intervals."); // 2 hours rounded up
        }

        [TestMethod]
        public void Fee_Rounds_Up_Two_Hour_When_Over_60_Minutes()
        {
            var car = new Car("ABC123");
            car.ApplySpec(20m, 4);
            car.RestoreParkedAtUtc(DateTime.UtcNow.AddMinutes(-121));
            var cfg = new ConfigApp
            {
                FreeMinutesBeforeCharge = 10
            };
            var fee = Menu.CalculateParkingFee(car, DateTime.UtcNow, cfg, out var total);
            Assert.AreEqual(40m, fee, "111 min chargeable should be rounded up to 2 h á 20m = 40m"); // 3 hours rounded up
        }
    }
}
