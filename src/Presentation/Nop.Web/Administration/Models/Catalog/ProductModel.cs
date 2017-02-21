using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Admin.Models.Settings;
using Nop.Admin.Validators.Catalog;
using Nop.Web.Framework;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Catalog
{
    [Validator(typeof(ProductValidator))]
    public partial class ProductModel : BaseNopEntityModel, ILocalizedModel<ProductLocalizedModel>
    {
        public ProductModel()
        {
            Locales = new List<ProductLocalizedModel>();
            ProductPictureModels = new List<ProductPictureModel>();
            CopyProductModel = new CopyProductModel();
            AddPictureModel = new ProductPictureModel();
            ProductEditorSettingsModel = new ProductEditorSettingsModel();
            
            AvailableProductTemplates = new List<SelectListItem>();
            ProductsTypesSupportedByProductTemplates = new Dictionary<int, IList<SelectListItem>>();

            SelectedStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();

            
            SelectedCategoryIds = new List<int>();
            AvailableCategories = new List<SelectListItem>();

            SelectedCustomerRoleIds = new List<int>();
            AvailableCustomerRoles = new List<SelectListItem>();

        }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.ID")]
        public override int Id { get; set; }

        //picture thumbnail
        [NopResourceDisplayName("Admin.Catalog.Products.Fields.PictureThumbnailUrl")]
        public string PictureThumbnailUrl { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.ProductType")]
        public int ProductTypeId { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Products.Fields.ProductType")]
        public string ProductTypeName { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.AssociatedToProductName")]
        public int AssociatedToProductId { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Products.Fields.AssociatedToProductName")]
        public string AssociatedToProductName { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.VisibleIndividually")]
        public bool VisibleIndividually { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.ProductTemplate")]
        public int ProductTemplateId { get; set; }
        public IList<SelectListItem> AvailableProductTemplates { get; set; }
        //<product type ID, list of supported product template IDs>
        public Dictionary<int, IList<SelectListItem>> ProductsTypesSupportedByProductTemplates { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.ShortDescription")]
        [AllowHtml]
        public string ShortDescription { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.FullDescription")]
        [AllowHtml]
        public string FullDescription { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.AdminComment")]
        [AllowHtml]
        public string AdminComment { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.ShowOnHomePage")]
        public bool ShowOnHomePage { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.MetaKeywords")]
        [AllowHtml]
        public string MetaKeywords { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.MetaDescription")]
        [AllowHtml]
        public string MetaDescription { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.MetaTitle")]
        [AllowHtml]
        public string MetaTitle { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.SeName")]
        [AllowHtml]
        public string SeName { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.AllowCustomerReviews")]
        public bool AllowCustomerReviews { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.ProductTags")]
        public string ProductTags { get; set; }




        [NopResourceDisplayName("Admin.Catalog.Products.Fields.Sku")]
        [AllowHtml]
        public string Sku { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.MarkAsNew")]
        public bool MarkAsNew { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Products.Fields.MarkAsNewStartDateTimeUtc")]
        [UIHint("DateTimeNullable")]
        public DateTime? MarkAsNewStartDateTimeUtc { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Products.Fields.MarkAsNewEndDateTimeUtc")]
        [UIHint("DateTimeNullable")]
        public DateTime? MarkAsNewEndDateTimeUtc { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.Published")]
        public bool Published { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.CreatedOn")]
        public DateTime? CreatedOn { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Products.Fields.UpdatedOn")]
        public DateTime? UpdatedOn { get; set; }

        public IList<ProductLocalizedModel> Locales { get; set; }



        //ACL (customer roles)
        [NopResourceDisplayName("Admin.Catalog.Products.Fields.AclCustomerRoles")]
        [UIHint("MultiSelect")]
        public IList<int> SelectedCustomerRoleIds { get; set; }
        public IList<SelectListItem> AvailableCustomerRoles { get; set; }

        //store mapping
        [NopResourceDisplayName("Admin.Catalog.Products.Fields.LimitedToStores")]
        [UIHint("MultiSelect")]
        public IList<int> SelectedStoreIds { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }

        //categories
        [NopResourceDisplayName("Admin.Catalog.Products.Fields.Categories")]
        [UIHint("MultiSelect")]
        public IList<int> SelectedCategoryIds { get; set; }
        public IList<SelectListItem> AvailableCategories { get; set; }

        //pictures
        public ProductPictureModel AddPictureModel { get; set; }
        public IList<ProductPictureModel> ProductPictureModels { get; set; }

        //copy product
        public CopyProductModel CopyProductModel { get; set; }

        //editor settings
        public ProductEditorSettingsModel ProductEditorSettingsModel { get; set; }

        #region Nested classes
        
        public partial class ProductPictureModel : BaseNopEntityModel
        {
            public int ProductId { get; set; }

            [UIHint("Picture")]
            [NopResourceDisplayName("Admin.Catalog.Products.Pictures.Fields.Picture")]
            public int PictureId { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Products.Pictures.Fields.Picture")]
            public string PictureUrl { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Products.Pictures.Fields.DisplayOrder")]
            public int DisplayOrder { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Products.Pictures.Fields.OverrideAltAttribute")]
            [AllowHtml]
            public string OverrideAltAttribute { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Products.Pictures.Fields.OverrideTitleAttribute")]
            [AllowHtml]
            public string OverrideTitleAttribute { get; set; }
        }

        public partial class RelatedProductModel : BaseNopEntityModel
        {
            public int ProductId2 { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Products.RelatedProducts.Fields.Product")]
            public string Product2Name { get; set; }
            
            [NopResourceDisplayName("Admin.Catalog.Products.RelatedProducts.Fields.DisplayOrder")]
            public int DisplayOrder { get; set; }
        }
        public partial class AddRelatedProductModel : BaseNopModel
        {
            public AddRelatedProductModel()
            {
                AvailableCategories = new List<SelectListItem>();
                AvailableStores = new List<SelectListItem>();
                AvailableProductTypes = new List<SelectListItem>();
            }

            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchProductName")]
            [AllowHtml]
            public string SearchProductName { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchCategory")]
            public int SearchCategoryId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchStore")]
            public int SearchStoreId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchProductType")]
            public int SearchProductTypeId { get; set; }

            public IList<SelectListItem> AvailableCategories { get; set; }
            public IList<SelectListItem> AvailableStores { get; set; }
            public IList<SelectListItem> AvailableProductTypes { get; set; }

            public int ProductId { get; set; }

            public int[] SelectedProductIds { get; set; }
        }

        public partial class AssociatedProductModel : BaseNopEntityModel
        {
            [NopResourceDisplayName("Admin.Catalog.Products.AssociatedProducts.Fields.Product")]
            public string ProductName { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.AssociatedProducts.Fields.DisplayOrder")]
            public int DisplayOrder { get; set; }
        }
        public partial class AddAssociatedProductModel : BaseNopModel
        {
            public AddAssociatedProductModel()
            {
                AvailableCategories = new List<SelectListItem>();
                AvailableStores = new List<SelectListItem>();
                AvailableProductTypes = new List<SelectListItem>();
            }

            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchProductName")]
            [AllowHtml]
            public string SearchProductName { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchCategory")]
            public int SearchCategoryId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchStore")]
            public int SearchStoreId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchProductType")]
            public int SearchProductTypeId { get; set; }

            public IList<SelectListItem> AvailableCategories { get; set; }
            public IList<SelectListItem> AvailableStores { get; set; }
            public IList<SelectListItem> AvailableProductTypes { get; set; }

            public int ProductId { get; set; }

            public int[] SelectedProductIds { get; set; }
        }

        #endregion
    }

    public partial class ProductLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.ShortDescription")]
        [AllowHtml]
        public string ShortDescription { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.FullDescription")]
        [AllowHtml]
        public string FullDescription { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.MetaKeywords")]
        [AllowHtml]
        public string MetaKeywords { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.MetaDescription")]
        [AllowHtml]
        public string MetaDescription { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.MetaTitle")]
        [AllowHtml]
        public string MetaTitle { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.SeName")]
        [AllowHtml]
        public string SeName { get; set; }
    }
}