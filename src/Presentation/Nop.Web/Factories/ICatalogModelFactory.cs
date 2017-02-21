using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Web.Models.Catalog;

namespace Nop.Web.Factories
{
    public partial interface ICatalogModelFactory
    {
        #region Categories

        CategoryModel PrepareCategoryModel(Category category, CatalogPagingFilteringModel command);

        string PrepareCategoryTemplateViewPath(int templateId);

        CategoryNavigationModel PrepareCategoryNavigationModel(int currentCategoryId,
            int currentProductId);

        TopMenuModel PrepareTopMenuModel();

        List<CategoryModel> PrepareHomepageCategoryModels();

        #endregion

        #region Product tags

        PopularProductTagsModel PreparePopularProductTagsModel();

        ProductsByTagModel PrepareProductsByTagModel(ProductTag productTag,
            CatalogPagingFilteringModel command);

        PopularProductTagsModel PrepareProductTagsAllModel();

        #endregion

        #region Searching

        SearchModel PrepareSearchModel(SearchModel model, CatalogPagingFilteringModel command);

        SearchBoxModel PrepareSearchBoxModel();

        #endregion
    }
}
