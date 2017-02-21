using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Catalog
{
    public partial class ProductListModel : BaseNopModel
    {
        public ProductListModel()
        {
            AvailableCategories = new List<SelectListItem>();
            AvailableStores = new List<SelectListItem>();
            AvailableProductTypes = new List<SelectListItem>();
            AvailablePublishedOptions = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.Catalog.Products.List.SearchProductName")]
        [AllowHtml]
        public string SearchProductName { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Products.List.SearchCategory")]
        public int SearchCategoryId { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Products.List.SearchIncludeSubCategories")]
        public bool SearchIncludeSubCategories { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Products.List.SearchStore")]
        public int SearchStoreId { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Products.List.SearchProductType")]
        public int SearchProductTypeId { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Products.List.SearchPublished")]
        public int SearchPublishedId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.List.GoDirectlyToSku")]
        [AllowHtml]
        public string GoDirectlyToSku { get; set; }


        public IList<SelectListItem> AvailableCategories { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
        public IList<SelectListItem> AvailableProductTypes { get; set; }
        public IList<SelectListItem> AvailablePublishedOptions { get; set; }
    }
}