using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Web.Models.Catalog;

namespace Nop.Web.Factories
{
    public partial interface IProductModelFactory
    {
        string PrepareProductTemplateViewPath(Product product);

        IEnumerable<ProductOverviewModel> PrepareProductOverviewModels(IEnumerable<Product> products,
            bool preparePictureModel = true,
            int? productThumbPictureSize = null);

        ProductDetailsModel PrepareProductDetailsModel(Product product);

        ProductReviewsModel PrepareProductReviewsModel(ProductReviewsModel model, Product product);
        
        CustomerProductReviewsModel PrepareCustomerProductReviewsModel(int? page);

        ProductEmailAFriendModel PrepareProductEmailAFriendModel(ProductEmailAFriendModel model, Product product, bool excludeProperties);
    }
}
