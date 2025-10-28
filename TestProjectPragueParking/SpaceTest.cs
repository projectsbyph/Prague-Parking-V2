using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Prague_Parking_V2
{
    [TestClass]
    public sealed class SpaceTest
    {
        [TestMethod]
        public void Car_Fits_When_Capacity_Is_4()
        {
            var space = new ParkingSpace(index: 1, capacitySpaces: 4);
            var car = new Car("ABC123");
            car.ApplySpec(hourlyRate: 20m, sizeUnits: 4);

            var canFit = space.CanVehicleFit(car);

            Assert.IsTrue(canFit);
        }

        [TestMethod]
        public void Mc_Doesnt_Fit_When_Space_Full()
        {
            var space = new ParkingSpace(index: 1, capacitySpaces: 4);
            var mc1 = new Mc("MC111"); mc1.ApplySpec(10m, 2);
            var mc2 = new Mc("MC222"); mc2.ApplySpec(10m, 2);
            space.ParkVehicle(mc1);
            space.ParkVehicle(mc2);


            var mc3 = new Mc("MC333"); mc3.ApplySpec(10m, 2);

            var canFit = space.CanVehicleFit(mc3);

            Assert.IsFalse(canFit);
        }
    }
}
