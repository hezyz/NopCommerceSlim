using System.Linq;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Catalog
{
    [TestFixture]
    public class ProductPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_product()
        {
            var product = this.GetTestProduct();

            var fromDb = SaveAndLoadEntity(this.GetTestProduct());
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(product);
        }

        [Test]
        public void Can_save_and_load_product_with_productCategories()
        {
            var product = this.GetTestProduct();

            var productCategory = this.GetTestProductCategory();
            productCategory.Product = product;
            productCategory.Category = this.GetTestCategory();

            product.ProductCategories.Add(productCategory);
            var fromDb = SaveAndLoadEntity(product);
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(this.GetTestProduct());

            fromDb.ProductCategories.ShouldNotBeNull();
            (fromDb.ProductCategories.Count == 1).ShouldBeTrue();
            fromDb.ProductCategories.First().PropertiesShouldEqual(this.GetTestProductCategory());

            fromDb.ProductCategories.First().Category.ShouldNotBeNull();
            fromDb.ProductCategories.First().Category.PropertiesShouldEqual(this.GetTestCategory());
        }

        [Test]
        public void Can_save_and_load_product_with_productPictures()
        {
            var product = this.GetTestProduct();
            product.ProductPictures.Add(this.GetTestProductPicture());
            var fromDb = SaveAndLoadEntity(product);
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(this.GetTestProduct());

            fromDb.ProductPictures.ShouldNotBeNull();
            (fromDb.ProductPictures.Count == 1).ShouldBeTrue();
            fromDb.ProductPictures.First().PropertiesShouldEqual(this.GetTestProductPicture());

            fromDb.ProductPictures.First().Picture.ShouldNotBeNull();
            fromDb.ProductPictures.First().Picture.PropertiesShouldEqual(this.GetTestPicture());
        }

        [Test]
        public void Can_save_and_load_product_with_productTags()
        {
            var product = this.GetTestProduct();
            product.ProductTags.Add(this.GetTestProductTag());
            var fromDb = SaveAndLoadEntity(product);
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(this.GetTestProduct());

            fromDb.ProductTags.ShouldNotBeNull();
            (fromDb.ProductTags.Count == 1).ShouldBeTrue();
            fromDb.ProductTags.First().PropertiesShouldEqual(this.GetTestProductTag());
        }
    }
}
