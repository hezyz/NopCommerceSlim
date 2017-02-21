using System.Linq;
using Nop.Core.Domain.Directory;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Directory
{
    [TestFixture]
    public class CountryPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_country()
        {
            var country = new Country
            {
                Name = "United States",
                TwoLetterIsoCode = "US",
                ThreeLetterIsoCode = "USA",
                NumericIsoCode = 1,
                Published = true,
                DisplayOrder = 1,
                LimitedToStores = true
            };

            var fromDb = SaveAndLoadEntity(country);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("United States");
            fromDb.TwoLetterIsoCode.ShouldEqual("US");
            fromDb.ThreeLetterIsoCode.ShouldEqual("USA");
            fromDb.NumericIsoCode.ShouldEqual(1);
            fromDb.Published.ShouldEqual(true);
            fromDb.DisplayOrder.ShouldEqual(1);
            fromDb.LimitedToStores.ShouldEqual(true);
        }

        [Test]
        public void Can_save_and_load_country_with_states()
        {
            var country = new Country
            {
                Name = "United States",
                TwoLetterIsoCode = "US",
                ThreeLetterIsoCode = "USA",
                NumericIsoCode = 1,
                Published = true,
                DisplayOrder = 1
            };
            country.StateProvinces.Add
                (
                    new StateProvince
                    {
                        Name = "California",
                        Abbreviation = "CA",
                        DisplayOrder = 1
                    }
                );
            var fromDb = SaveAndLoadEntity(country);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("United States");

            fromDb.StateProvinces.ShouldNotBeNull();
            (fromDb.StateProvinces.Count == 1).ShouldBeTrue();
            fromDb.StateProvinces.First().Name.ShouldEqual("California");
        }

        [Test]
        public void Can_save_and_load_country_with_restrictions()
        {
            var country = new Country
            {
                Name = "United States",
                TwoLetterIsoCode = "US",
                ThreeLetterIsoCode = "USA",
                NumericIsoCode = 1,
                Published = true,
                DisplayOrder = 1
            };
           
            var fromDb = SaveAndLoadEntity(country);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("United States");
        }
    }
}
