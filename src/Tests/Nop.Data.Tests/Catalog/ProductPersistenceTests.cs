using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
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
            var product = new Product
            {
                ProductType = ProductType.GroupedProduct,
                ParentGroupedProductId = 2,
                VisibleIndividually = true,
                Name = "Name 1",
                ShortDescription = "ShortDescription 1",
                FullDescription = "FullDescription 1",
                AdminComment = "AdminComment 1",
                ProductTemplateId = 2,
                ShowOnHomePage = false,
                MetaKeywords = "Meta keywords",
                MetaDescription = "Meta description",
                MetaTitle = "Meta title",
                AllowCustomerReviews = true,
                ApprovedRatingSum = 2,
                NotApprovedRatingSum = 3,
                ApprovedTotalReviews = 4,
                NotApprovedTotalReviews = 5,
                SubjectToAcl = true,
                LimitedToStores = true,
                Sku = "sku 1",
                MarkAsNew = true,
                MarkAsNewStartDateTimeUtc = new DateTime(2010, 01, 07),
                MarkAsNewEndDateTimeUtc = new DateTime(2010, 01, 08),
                DisplayOrder = 30,
                Published = true,
                Deleted = false,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02),
            };
            
            var fromDb = SaveAndLoadEntity(product);
            fromDb.ShouldNotBeNull();
            fromDb.ProductType.ShouldEqual(ProductType.GroupedProduct);
            fromDb.ParentGroupedProductId.ShouldEqual(2);
            fromDb.VisibleIndividually.ShouldEqual(true);
            fromDb.Name.ShouldEqual("Name 1");
            fromDb.ShortDescription.ShouldEqual("ShortDescription 1");
            fromDb.FullDescription.ShouldEqual("FullDescription 1");
            fromDb.AdminComment.ShouldEqual("AdminComment 1");
            fromDb.ProductTemplateId.ShouldEqual(2);
            fromDb.ShowOnHomePage.ShouldEqual(false);
            fromDb.MetaKeywords.ShouldEqual("Meta keywords");
            fromDb.MetaDescription.ShouldEqual("Meta description");
            fromDb.AllowCustomerReviews.ShouldEqual(true);
            fromDb.ApprovedRatingSum.ShouldEqual(2);
            fromDb.NotApprovedRatingSum.ShouldEqual(3);
            fromDb.ApprovedTotalReviews.ShouldEqual(4);
            fromDb.NotApprovedTotalReviews.ShouldEqual(5);
            fromDb.SubjectToAcl.ShouldEqual(true);
            fromDb.LimitedToStores.ShouldEqual(true);
            fromDb.ShouldNotBeNull();
            fromDb.Sku.ShouldEqual("sku 1");
            fromDb.MarkAsNew.ShouldEqual(true);
            fromDb.MarkAsNewStartDateTimeUtc.ShouldEqual(new DateTime(2010, 01, 07));
            fromDb.MarkAsNewEndDateTimeUtc.ShouldEqual(new DateTime(2010, 01, 08));
            fromDb.DisplayOrder.ShouldEqual(30);
            fromDb.Published.ShouldEqual(true);
            fromDb.Deleted.ShouldEqual(false);
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));
            fromDb.UpdatedOnUtc.ShouldEqual(new DateTime(2010, 01, 02));
        }

        [Test]
        public void Can_save_and_load_product_with_productCategories()
        {
            var product = new Product
                          {
                              Name = "Name 1",
                              Published = true,
                              Deleted = false,
                              CreatedOnUtc = new DateTime(2010, 01, 01),
                              UpdatedOnUtc = new DateTime(2010, 01, 02)
                          };
            product.ProductCategories.Add
                (
                    new ProductCategory
                    {
                        IsFeaturedProduct = true,
                        DisplayOrder = 1,
                        Category = new Category
                        {
                            Name = "Books",
                            Description = "Description 1",
                            MetaKeywords = "Meta keywords",
                            MetaDescription = "Meta description",
                            MetaTitle = "Meta title",
                            ParentCategoryId = 2,
                            PictureId = 3,
                            PageSize = 4,
                            ShowOnHomePage = false,
                            Published = true,
                            Deleted = false,
                            DisplayOrder = 5,
                            CreatedOnUtc = new DateTime(2010, 01, 01),
                            UpdatedOnUtc = new DateTime(2010, 01, 02),
                        }
                    }
                );
            var fromDb = SaveAndLoadEntity(product);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Name 1");

            fromDb.ProductCategories.ShouldNotBeNull();
            (fromDb.ProductCategories.Count == 1).ShouldBeTrue();
            fromDb.ProductCategories.First().IsFeaturedProduct.ShouldEqual(true);

            fromDb.ProductCategories.First().Category.ShouldNotBeNull();
            fromDb.ProductCategories.First().Category.Name.ShouldEqual("Books");
        }

        [Test]
        public void Can_save_and_load_product_with_productPictures()
        {
            var product = new Product
                          {
                              Name = "Name 1",
                              Published = true,
                              Deleted = false,
                              CreatedOnUtc = new DateTime(2010, 01, 01),
                              UpdatedOnUtc = new DateTime(2010, 01, 02)
                          };
            product.ProductPictures.Add
                (
                    new ProductPicture
                    {
                        DisplayOrder = 1,
                        Picture = new Picture
                        {
                            PictureBinary = new byte[] { 1, 2, 3 },
                            MimeType = MimeTypes.ImagePJpeg,
                            IsNew = true
                        }
                    }
                );
            var fromDb = SaveAndLoadEntity(product);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Name 1");

            fromDb.ProductPictures.ShouldNotBeNull();
            (fromDb.ProductPictures.Count == 1).ShouldBeTrue();
            fromDb.ProductPictures.First().DisplayOrder.ShouldEqual(1);

            fromDb.ProductPictures.First().Picture.ShouldNotBeNull();
            fromDb.ProductPictures.First().Picture.MimeType.ShouldEqual(MimeTypes.ImagePJpeg);
        }

        [Test]
        public void Can_save_and_load_product_with_productTags()
        {
            var product = new Product
            {
                Name = "Name 1",
                Published = true,
                Deleted = false,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02)
            };
            product.ProductTags.Add
                (
                    new ProductTag
                    {
                        Name = "Tag name 1",
                    }
                );
            var fromDb = SaveAndLoadEntity(product);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Name 1");


            fromDb.ProductTags.ShouldNotBeNull();
            (fromDb.ProductTags.Count == 1).ShouldBeTrue();
            fromDb.ProductTags.First().Name.ShouldEqual("Tag name 1");
        }

    }
}
