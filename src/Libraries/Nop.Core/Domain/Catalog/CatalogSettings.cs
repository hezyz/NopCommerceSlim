using System.Collections.Generic;
using Nop.Core.Configuration;

namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Catalog settings
    /// </summary>
    public class CatalogSettings : ISettings
    {
        public CatalogSettings()
        {
            ProductSortingEnumDisabled = new List<int>();
            ProductSortingEnumDisplayOrder= new Dictionary<int, int>();
        }

        /// <summary>
        /// Gets or sets a value indicating details pages of unpublished product details pages could be open (for SEO optimization)
        /// </summary>
        public bool AllowViewUnpublishedProductPage { get; set; }
        /// <summary>
        /// Gets or sets a value indicating customers should see "discontinued" message when visibting details pages of unpublished products (if "AllowViewUnpublishedProductPage" is "true)
        /// </summary>
        public bool DisplayDiscontinuedMessageForUnpublishedProducts { get; set; }
       
        /// <summary>
        /// Gets or sets a value indicating whether to display product SKU on the product details page
        /// </summary>
        public bool ShowSkuOnProductDetailsPage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display product SKU on catalog pages
        /// </summary>
        public bool ShowSkuOnCatalogPages { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether product sorting is enabled
        /// </summary>
        public bool AllowProductSorting { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether customers are allowed to change product view mode
        /// </summary>
        public bool AllowProductViewModeChanging { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether customers are allowed to change product view mode
        /// </summary>
        public string DefaultViewMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a category details page should include products from subcategories
        /// </summary>
        public bool ShowProductsFromSubcategories { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether number of products should be displayed beside each category
        /// </summary>
        public bool ShowCategoryProductNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether we include subcategories (when 'ShowCategoryProductNumber' is 'true')
        /// </summary>
        public bool ShowCategoryProductNumberIncludingSubcategories { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether category breadcrumb is enabled
        /// </summary>
        public bool CategoryBreadcrumbEnabled { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether a 'Share button' is enabled
        /// </summary>
        public bool ShowShareButton { get; set; }

        /// <summary>
        /// Gets or sets a share code (e.g. AddThis button code)
        /// </summary>
        public string PageShareCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating product reviews must be approved
        /// </summary>
        public bool ProductReviewsMustBeApproved { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the default rating value of the product reviews
        /// </summary>
        public int DefaultProductRatingValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to allow anonymous users write product reviews.
        /// </summary>
        public bool AllowAnonymousUsersToReviewProduct { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether notification of a store owner about new product reviews is enabled
        /// </summary>
        public bool NotifyStoreOwnerAboutNewProductReviews { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product reviews will be filtered per store
        /// </summary>
        public bool ShowProductReviewsPerStore { get; set; }

        /// <summary>
        /// Gets or sets a show product reviews tab on account page
        /// </summary>
        public bool ShowProductReviewsTabOnAccountPage { get; set; }

        /// <summary>
        /// Gets or sets the page size for product reviews in account page
        /// </summary>
        public int ProductReviewsPageSizeOnAccountPage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether product 'Email a friend' feature is enabled
        /// </summary>
        public bool EmailAFriendEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to allow anonymous users to email a friend.
        /// </summary>
        public bool AllowAnonymousUsersToEmailAFriend { get; set; }

        /// <summary>
        /// Gets or sets a number of "Recently viewed products"
        /// </summary>
        public int RecentlyViewedProductsNumber { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether "Recently viewed products" feature is enabled
        /// </summary>
        public bool RecentlyViewedProductsEnabled { get; set; }
        /// <summary>
        /// Gets or sets a number of products on the "New products" page
        /// </summary>
        public int NewProductsNumber { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether "New products" page is enabled
        /// </summary>
        public bool NewProductsEnabled { get; set; }

       
        /// <summary>
        /// Gets or sets a value indicating whether autocomplete is enabled
        /// </summary>
        public bool ProductSearchAutoCompleteEnabled { get; set; }
        /// <summary>
        /// Gets or sets a number of products to return when using "autocomplete" feature
        /// </summary>
        public int ProductSearchAutoCompleteNumberOfProducts { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to show product images in the auto complete search
        /// </summary>
        public bool ShowProductImagesInSearchAutoComplete { get; set; }
        /// <summary>
        /// Gets or sets a minimum search term length
        /// </summary>
        public int ProductSearchTermMinimumLength { get; set; }
        
        /// <summary>
        /// Gets or sets a number of products per page on the search products page
        /// </summary>
        public int SearchPageProductsPerPage { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether customers are allowed to select page size on the search products page
        /// </summary>
        public bool SearchPageAllowCustomersToSelectPageSize { get; set; }
        /// <summary>
        /// Gets or sets the available customer selectable page size options on the search products page
        /// </summary>
        public string SearchPagePageSizeOptions { get; set; }

        /// <summary>
        /// Gets or sets a number of product tags that appear in the tag cloud
        /// </summary>
        public int NumberOfProductTags { get; set; }

        /// <summary>
        /// Gets or sets a number of products per page on 'products by tag' page
        /// </summary>
        public int ProductsByTagPageSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether customers can select the page size for 'products by tag'
        /// </summary>
        public bool ProductsByTagAllowCustomersToSelectPageSize { get; set; }

        /// <summary>
        /// Gets or sets the available customer selectable page size options for 'products by tag'
        /// </summary>
        public string ProductsByTagPageSizeOptions { get; set; }

        /// <summary>
        /// An option indicating whether products on category pages should include featured products as well
        /// </summary>
        public bool IncludeFeaturedProductsInNormalLists { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether to ignore featured products (side-wide). It can significantly improve performance when enabled.
        /// </summary>
        public bool IgnoreFeaturedProducts { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to ignore ACL rules (side-wide). It can significantly improve performance when enabled.
        /// </summary>
        public bool IgnoreAcl { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to ignore "limit per store" rules (side-wide). It can significantly improve performance when enabled.
        /// </summary>
        public bool IgnoreStoreLimitations { get; set; }

        /// <summary>
        /// Gets or sets the default value to use for Category page size options (for new categories)
        /// </summary>
        public string DefaultCategoryPageSizeOptions { get; set; }
        /// <summary>
        /// Gets or sets the default value to use for Category page size (for new categories)
        /// </summary>
        public int DefaultCategoryPageSize { get; set; }
       
        /// <summary>
        /// Gets or sets a list of disabled values of ProductSortingEnum
        /// </summary>
        public List<int> ProductSortingEnumDisabled { get; set; }

        /// <summary>
        /// Gets or sets a display order of ProductSortingEnum values 
        /// </summary>
        public Dictionary<int, int> ProductSortingEnumDisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether need create dropdown list for export
        /// </summary>
        public bool ExportImportUseDropdownlistsForAssociatedEntities { get; set; }
    }
}