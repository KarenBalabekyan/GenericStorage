using System;
using System.Collections.Generic;

namespace GenericStorageTests.Stubs
{
    [Serializable]
    public class CarFactory
    {
        public static IEnumerable<Car> Create()
        {
            var cars = new List<Car>
            {
                new Car {Brand = "Porsche", Model = "3-Series", Year = 2012},
                new Car {Brand = "Audi", Model = "Q-Series", Year = 2017},
                new Car {Brand = "Mercedes-Benz", Model = "CLA 63", Year = 2016}
            };

            return cars;
        }
    }
}