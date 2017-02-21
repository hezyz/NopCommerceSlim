using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Stores;
using Nop.Data;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Product service
    /// </summary>
    public partial class ProductService : IProductService
    {
        #region Constants
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// </remarks>
        private const string PRODUCTS_BY_ID_KEY = "Nop.product.id-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string PRODUCTS_PATTERN_KEY = "Nop.product.";
        #endregion

        #region Fields

        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<RelatedProduct> _relatedProductRepository;
        private readonly IRepository<LocalizedProperty> _localizedPropertyRepository;
        private readonly IRepository<AclRecord> _aclRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly IRepository<ProductPicture> _productPictureRepository;
        private readonly IRepository<ProductReview> _productReviewRepository;
        private readonly ILanguageService _languageService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IDataProvider _dataProvider;
        private readonly IDbContext _dbContext;
        private readonly ICacheManager _cacheManager;
        private readonly IWorkContext _workContext;
        private readonly LocalizationSettings _localizationSettings;
        private readonly CommonSettings _commonSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly IEventPublisher _eventPublisher;
        private readonly IAclService _aclService;
        private readonly IStoreMappingService _storeMappingService;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="productRepository">Product repository</param>
        /// <param name="relatedProductRepository">Related product repository</param>
        /// <param name="localizedPropertyRepository">Localized property repository</param>
        /// <param name="aclRepository">ACL record repository</param>
        /// <param name="storeMappingRepository">Store mapping repository</param>
        /// <param name="productPictureRepository">Product picture repository</param>
        /// <param name="productReviewRepository">Product review repository</param>
        /// <param name="languageService">Language service</param>
        /// <param name="workflowMessageService">Workflow message service</param>
        /// <param name="dataProvider">Data provider</param>
        /// <param name="dbContext">Database Context</param>
        /// <param name="workContext">Work context</param>
        /// <param name="localizationSettings">Localization settings</param>
        /// <param name="commonSettings">Common settings</param>
        /// <param name="catalogSettings">Catalog settings</param>
        /// <param name="eventPublisher">Event published</param>
        /// <param name="aclService">ACL service</param>
        /// <param name="storeMappingService">Store mapping service</param>
        public ProductService(ICacheManager cacheManager,
            IRepository<Product> productRepository,
            IRepository<RelatedProduct> relatedProductRepository,
            IRepository<ProductPicture> productPictureRepository,
            IRepository<LocalizedProperty> localizedPropertyRepository,
            IRepository<AclRecord> aclRepository,
            IRepository<StoreMapping> storeMappingRepository,
            IRepository<ProductReview>  productReviewRepository,
            ILanguageService languageService,
            IWorkflowMessageService workflowMessageService,
            IDataProvider dataProvider, 
            IDbContext dbContext,
            IWorkContext workContext,
            LocalizationSettings localizationSettings, 
            CommonSettings commonSettings,
            CatalogSettings catalogSettings,
            IEventPublisher eventPublisher,
            IAclService aclService,
            IStoreMappingService storeMappingService)
        {
            this._cacheManager = cacheManager;
            this._productRepository = productRepository;
            this._relatedProductRepository = relatedProductRepository;
            this._productPictureRepository = productPictureRepository;
            this._localizedPropertyRepository = localizedPropertyRepository;
            this._aclRepository = aclRepository;
            this._storeMappingRepository = storeMappingRepository;
            this._productReviewRepository = productReviewRepository;
            this._languageService = languageService;
            this._workflowMessageService = workflowMessageService;
            this._dataProvider = dataProvider;
            this._dbContext = dbContext;
            this._workContext = workContext;
            this._localizationSettings = localizationSettings;
            this._commonSettings = commonSettings;
            this._catalogSettings = catalogSettings;
            this._eventPublisher = eventPublisher;
            this._aclService = aclService;
            this._storeMappingService = storeMappingService;
        }

        #endregion
        
        #region Methods

        #region Products

        /// <summary>
        /// Delete a product
        /// </summary>
        /// <param name="product">Product</param>
        public virtual void DeleteProduct(Product product)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            product.Deleted = true;
            //delete product
            UpdateProduct(product);

            //event notification
            _eventPublisher.EntityDeleted(product);
        }

        /// <summary>
        /// Delete products
        /// </summary>
        /// <param name="products">Products</param>
        public virtual void DeleteProducts(IList<Product> products)
        {
            if (products == null)
                throw new ArgumentNullException("products");

            foreach (var product in products)
            {
                product.Deleted = true;
            }

            //delete product
            UpdateProducts(products);

            foreach (var product in products)
            {
                //event notification
                _eventPublisher.EntityDeleted(product);
            }
        }

        /// <summary>
        /// Gets all products displayed on the home page
        /// </summary>
        /// <returns>Products</returns>
        public virtual IList<Product> GetAllProductsDisplayedOnHomePage()
        {
            var query = from p in _productRepository.Table
                        orderby p.DisplayOrder, p.Id
                        where p.Published &&
                        !p.Deleted &&
                        p.ShowOnHomePage
                        select p;
            var products = query.ToList();
            return products;
        }
        
        /// <summary>
        /// Gets product
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>Product</returns>
        public virtual Product GetProductById(int productId)
        {
            if (productId == 0)
                return null;
            
            string key = string.Format(PRODUCTS_BY_ID_KEY, productId);
            return _cacheManager.Get(key, () => _productRepository.GetById(productId));
        }

        /// <summary>
        /// Get products by identifiers
        /// </summary>
        /// <param name="productIds">Product identifiers</param>
        /// <returns>Products</returns>
        public virtual IList<Product> GetProductsByIds(int[] productIds)
        {
            if (productIds == null || productIds.Length == 0)
                return new List<Product>();

            var query = from p in _productRepository.Table
                        where productIds.Contains(p.Id) && !p.Deleted
                        select p;
            var products = query.ToList();
            //sort by passed identifiers
            var sortedProducts = new List<Product>();
            foreach (int id in productIds)
            {
                var product = products.Find(x => x.Id == id);
                if (product != null)
                    sortedProducts.Add(product);
            }
            return sortedProducts;
        }

        /// <summary>
        /// Inserts a product
        /// </summary>
        /// <param name="product">Product</param>
        public virtual void InsertProduct(Product product)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            //insert
            _productRepository.Insert(product);

            //clear cache
            _cacheManager.RemoveByPattern(PRODUCTS_PATTERN_KEY);
            
            //event notification
            _eventPublisher.EntityInserted(product);
        }

        /// <summary>
        /// Updates the product
        /// </summary>
        /// <param name="product">Product</param>
        public virtual void UpdateProduct(Product product)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            //update
            _productRepository.Update(product);

            //cache
            _cacheManager.RemoveByPattern(PRODUCTS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(product);
        }

        public virtual void UpdateProducts(IList<Product> products)
        {
            if (products == null)
                throw new ArgumentNullException("products");

            //update
            _productRepository.Update(products);

            //cache
            _cacheManager.RemoveByPattern(PRODUCTS_PATTERN_KEY);

            //event notification
            foreach (var product in products)
            {
                _eventPublisher.EntityUpdated(product);
            }
        }

        /// <summary>
        /// Get number of product (published and visible) in certain category
        /// </summary>
        /// <param name="categoryIds">Category identifiers</param>
        /// <param name="storeId">Store identifier; 0 to load all records</param>
        /// <returns>Number of products</returns>
        public virtual int GetNumberOfProductsInCategory(IList<int> categoryIds = null, int storeId = 0)
        {
            //validate "categoryIds" parameter
            if (categoryIds != null && categoryIds.Contains(0))
                categoryIds.Remove(0);

            var query = _productRepository.Table;
            query = query.Where(p => !p.Deleted && p.Published && p.VisibleIndividually);

            //category filtering
            if (categoryIds != null && categoryIds.Any())
            {
                query = from p in query
                        from pc in p.ProductCategories.Where(pc => categoryIds.Contains(pc.CategoryId))
                        select p;
            }

            if (!_catalogSettings.IgnoreAcl)
            {
                //Access control list. Allowed customer roles
                var allowedCustomerRolesIds = _workContext.CurrentCustomer.GetCustomerRoleIds();

                query = from p in query
                        join acl in _aclRepository.Table
                        on new { c1 = p.Id, c2 = "Product" } equals new { c1 = acl.EntityId, c2 = acl.EntityName } into p_acl
                        from acl in p_acl.DefaultIfEmpty()
                        where !p.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                        select p;
            }

            if (storeId > 0 && !_catalogSettings.IgnoreStoreLimitations)
            {
                query = from p in query
                        join sm in _storeMappingRepository.Table
                        on new { c1 = p.Id, c2 = "Product" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into p_sm
                        from sm in p_sm.DefaultIfEmpty()
                        where !p.LimitedToStores || storeId == sm.StoreId
                        select p;
            }

            //only distinct products
            var result = query.Select(p => p.Id).Distinct().Count();
            return result;
        }

        /// <summary>
        /// Search products
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="categoryIds">Category identifiers</param>
        /// <param name="storeId">Store identifier; 0 to load all records</param>
        /// <param name="productType">Product type; 0 to load all records</param>
        /// <param name="markedAsNewOnly">A values indicating whether to load only products marked as "new"; "false" to load all records; "true" to load "marked as new" only</param>
        /// <param name="featuredProducts">A value indicating whether loaded products are marked as featured (relates only to categories). 0 to load featured products only, 1 to load not featured products only, null to load all products</param>
       /// <param name="productTagId">Product tag identifier; 0 to load all records</param>
        /// <param name="keywords">Keywords</param>
        /// <param name="searchDescriptions">A value indicating whether to search by a specified "keyword" in product descriptions</param>
        /// <param name="searchSku">A value indicating whether to search by a specified "keyword" in product SKU</param>
        /// <param name="searchProductTags">A value indicating whether to search by a specified "keyword" in product tags</param>
        /// <param name="languageId">Language identifier (search for text searching)</param>
        /// <param name="orderBy">Order by</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="overridePublished">
        /// null - process "Published" property according to "showHidden" parameter
        /// true - load only "Published" products
        /// false - load only "Unpublished" products
        /// </param>
        /// <returns>Products</returns>
        public virtual IPagedList<Product> SearchProducts(
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            IList<int> categoryIds = null,
            int storeId = 0,
            ProductType? productType = null,
            bool visibleIndividuallyOnly = false,
            bool markedAsNewOnly = false,
            bool? featuredProducts = null,
            int productTagId = 0,
            string keywords = null,
            bool searchDescriptions = false,
            bool searchSku = true,
            bool searchProductTags = false,
            int languageId = 0,
            ProductSortingEnum orderBy = ProductSortingEnum.Position,
            bool showHidden = false,
            bool? overridePublished = null)
        {
            //search by keyword
            bool searchLocalizedValue = false;
            if (languageId > 0)
            {
                if (showHidden)
                {
                    searchLocalizedValue = true;
                }
                else
                {
                    //ensure that we have at least two published languages
                    var totalPublishedLanguages = _languageService.GetAllLanguages().Count;
                    searchLocalizedValue = totalPublishedLanguages >= 2;
                }
            }

            //validate "categoryIds" parameter
            if (categoryIds !=null && categoryIds.Contains(0))
                categoryIds.Remove(0);

            //Access control list. Allowed customer roles
            var allowedCustomerRolesIds = _workContext.CurrentCustomer.GetCustomerRoleIds();

            if (_commonSettings.UseStoredProceduresIfSupported && _dataProvider.StoredProceduredSupported)
            {
                //stored procedures are enabled and supported by the database. 
                //It's much faster than the LINQ implementation below 

                #region Use stored procedure
                
                //pass category identifiers as comma-delimited string
                string commaSeparatedCategoryIds = categoryIds == null ? "" : string.Join(",", categoryIds);


                //pass customer role identifiers as comma-delimited string
                string commaSeparatedAllowedCustomerRoleIds = string.Join(",", allowedCustomerRolesIds);


                //some databases don't support int.MaxValue
                if (pageSize == int.MaxValue)
                    pageSize = int.MaxValue - 1;
                
                //prepare parameters
                var pCategoryIds = _dataProvider.GetParameter();
                pCategoryIds.ParameterName = "CategoryIds";
                pCategoryIds.Value = commaSeparatedCategoryIds;
                pCategoryIds.DbType = DbType.String;

                var pStoreId = _dataProvider.GetParameter();
                pStoreId.ParameterName = "StoreId";
                pStoreId.Value = !_catalogSettings.IgnoreStoreLimitations ? storeId : 0;
                pStoreId.DbType = DbType.Int32;

                var pProductTypeId = _dataProvider.GetParameter();
                pProductTypeId.ParameterName = "ProductTypeId";
                pProductTypeId.Value = productType.HasValue ? (object)productType.Value : DBNull.Value;
                pProductTypeId.DbType = DbType.Int32;

                var pMarkedAsNewOnly = _dataProvider.GetParameter();
                pMarkedAsNewOnly.ParameterName = "MarkedAsNewOnly";
                pMarkedAsNewOnly.Value = markedAsNewOnly;
                pMarkedAsNewOnly.DbType = DbType.Int32;

                var pProductTagId = _dataProvider.GetParameter();
                pProductTagId.ParameterName = "ProductTagId";
                pProductTagId.Value = productTagId;
                pProductTagId.DbType = DbType.Int32;

                var pFeaturedProducts = _dataProvider.GetParameter();
                pFeaturedProducts.ParameterName = "FeaturedProducts";
                pFeaturedProducts.Value = featuredProducts.HasValue ? (object)featuredProducts.Value : DBNull.Value;
                pFeaturedProducts.DbType = DbType.Boolean;

                var pKeywords = _dataProvider.GetParameter();
                pKeywords.ParameterName = "Keywords";
                pKeywords.Value = keywords != null ? (object)keywords : DBNull.Value;
                pKeywords.DbType = DbType.String;

                var pSearchDescriptions = _dataProvider.GetParameter();
                pSearchDescriptions.ParameterName = "SearchDescriptions";
                pSearchDescriptions.Value = searchDescriptions;
                pSearchDescriptions.DbType = DbType.Boolean;

                var pSearchSku = _dataProvider.GetParameter();
                pSearchSku.ParameterName = "SearchSku";
                pSearchSku.Value = searchSku;
                pSearchSku.DbType = DbType.Boolean;

                var pSearchProductTags = _dataProvider.GetParameter();
                pSearchProductTags.ParameterName = "SearchProductTags";
                pSearchProductTags.Value = searchProductTags;
                pSearchProductTags.DbType = DbType.Boolean;

                var pUseFullTextSearch = _dataProvider.GetParameter();
                pUseFullTextSearch.ParameterName = "UseFullTextSearch";
                pUseFullTextSearch.Value = _commonSettings.UseFullTextSearch;
                pUseFullTextSearch.DbType = DbType.Boolean;

                var pFullTextMode = _dataProvider.GetParameter();
                pFullTextMode.ParameterName = "FullTextMode";
                pFullTextMode.Value = (int)_commonSettings.FullTextMode;
                pFullTextMode.DbType = DbType.Int32;

                var pLanguageId = _dataProvider.GetParameter();
                pLanguageId.ParameterName = "LanguageId";
                pLanguageId.Value = searchLocalizedValue ? languageId : 0;
                pLanguageId.DbType = DbType.Int32;

                var pOrderBy = _dataProvider.GetParameter();
                pOrderBy.ParameterName = "OrderBy";
                pOrderBy.Value = (int)orderBy;
                pOrderBy.DbType = DbType.Int32;

                var pAllowedCustomerRoleIds = _dataProvider.GetParameter();
                pAllowedCustomerRoleIds.ParameterName = "AllowedCustomerRoleIds";
                pAllowedCustomerRoleIds.Value = !_catalogSettings.IgnoreAcl ? commaSeparatedAllowedCustomerRoleIds : "";
                pAllowedCustomerRoleIds.DbType = DbType.String;

                var pPageIndex = _dataProvider.GetParameter();
                pPageIndex.ParameterName = "PageIndex";
                pPageIndex.Value = pageIndex;
                pPageIndex.DbType = DbType.Int32;

                var pPageSize = _dataProvider.GetParameter();
                pPageSize.ParameterName = "PageSize";
                pPageSize.Value = pageSize;
                pPageSize.DbType = DbType.Int32;

                var pShowHidden = _dataProvider.GetParameter();
                pShowHidden.ParameterName = "ShowHidden";
                pShowHidden.Value = showHidden;
                pShowHidden.DbType = DbType.Boolean;

                var pOverridePublished = _dataProvider.GetParameter();
                pOverridePublished.ParameterName = "OverridePublished";
                pOverridePublished.Value = overridePublished != null ? (object)overridePublished.Value : DBNull.Value;
                pOverridePublished.DbType = DbType.Boolean;

                var pTotalRecords = _dataProvider.GetParameter();
                pTotalRecords.ParameterName = "TotalRecords";
                pTotalRecords.Direction = ParameterDirection.Output;
                pTotalRecords.DbType = DbType.Int32;

                //invoke stored procedure
                var products = _dbContext.ExecuteStoredProcedureList<Product>(
                    "ProductLoadAllPaged",
                    pCategoryIds,
                    pStoreId,
                    pProductTypeId,
                    pMarkedAsNewOnly,
                    pProductTagId,
                    pFeaturedProducts,
                    pKeywords,
                    pSearchDescriptions,
                    pSearchSku,
                    pSearchProductTags,
                    pUseFullTextSearch,
                    pFullTextMode,
                    pLanguageId,
                    pOrderBy,
                    pAllowedCustomerRoleIds,
                    pPageIndex,
                    pPageSize,
                    pShowHidden,
                    pOverridePublished,
                    pTotalRecords);
                //return products
                int totalRecords = (pTotalRecords.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecords.Value) : 0;
                return new PagedList<Product>(products, pageIndex, pageSize, totalRecords);

                #endregion
            }
            else
            {
                //stored procedures aren't supported. Use LINQ

                #region Search products

                //products
                var query = _productRepository.Table;
                query = query.Where(p => !p.Deleted);
                if (!overridePublished.HasValue)
                {
                    //process according to "showHidden"
                    if (!showHidden)
                    {
                        query = query.Where(p => p.Published);
                    }
                }
                else if (overridePublished.Value)
                {
                    //published only
                    query = query.Where(p => p.Published);
                }
                else if (!overridePublished.Value)
                {
                    //unpublished only
                    query = query.Where(p => !p.Published);
                }
                //The function 'CurrentUtcDateTime' is not supported by SQL Server Compact. 
                //That's why we pass the date value
                var nowUtc = DateTime.UtcNow;
                if (markedAsNewOnly)
                {
                    query = query.Where(p => p.MarkAsNew);
                    query = query.Where(p =>
                        (!p.MarkAsNewStartDateTimeUtc.HasValue || p.MarkAsNewStartDateTimeUtc.Value < nowUtc) &&
                        (!p.MarkAsNewEndDateTimeUtc.HasValue || p.MarkAsNewEndDateTimeUtc.Value > nowUtc));
                }
                if (productType.HasValue)
                {
                    var productTypeId = (int) productType.Value;
                    query = query.Where(p => p.ProductTypeId == productTypeId);
                }

                //searching by keyword
                if (!String.IsNullOrWhiteSpace(keywords))
                {
                    query = from p in query
                            join lp in _localizedPropertyRepository.Table on p.Id equals lp.EntityId into p_lp
                            from lp in p_lp.DefaultIfEmpty()
                            from pt in p.ProductTags.DefaultIfEmpty()
                            where (p.Name.Contains(keywords)) ||
                                  (searchDescriptions && p.ShortDescription.Contains(keywords)) ||
                                  (searchDescriptions && p.FullDescription.Contains(keywords)) ||
                                  //sku (exact match)
                                  (searchSku && p.Sku == keywords) ||
                                  //product tags (exact match)
                                  (searchProductTags && pt.Name == keywords) ||
                                  //localized values
                                  (searchLocalizedValue && lp.LanguageId == languageId && lp.LocaleKeyGroup == "Product" && lp.LocaleKey == "Name" && lp.LocaleValue.Contains(keywords)) ||
                                  (searchDescriptions && searchLocalizedValue && lp.LanguageId == languageId && lp.LocaleKeyGroup == "Product" && lp.LocaleKey == "ShortDescription" && lp.LocaleValue.Contains(keywords)) ||
                                  (searchDescriptions && searchLocalizedValue && lp.LanguageId == languageId && lp.LocaleKeyGroup == "Product" && lp.LocaleKey == "FullDescription" && lp.LocaleValue.Contains(keywords))
                            select p;
                }

                if (!showHidden && !_catalogSettings.IgnoreAcl)
                {
                    //ACL (access control list)
                    query = from p in query
                            join acl in _aclRepository.Table
                            on new { c1 = p.Id, c2 = "Product" } equals new { c1 = acl.EntityId, c2 = acl.EntityName } into p_acl
                            from acl in p_acl.DefaultIfEmpty()
                            where !p.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                            select p;
                }

                if (storeId > 0 && !_catalogSettings.IgnoreStoreLimitations)
                {
                    //Store mapping
                    query = from p in query
                            join sm in _storeMappingRepository.Table
                            on new { c1 = p.Id, c2 = "Product" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into p_sm
                            from sm in p_sm.DefaultIfEmpty()
                            where !p.LimitedToStores || storeId == sm.StoreId
                            select p;
                }

                //category filtering
                if (categoryIds != null && categoryIds.Any())
                {
                    query = from p in query
                            from pc in p.ProductCategories.Where(pc => categoryIds.Contains(pc.CategoryId))
                            where (!featuredProducts.HasValue || featuredProducts.Value == pc.IsFeaturedProduct)
                            select p;
                }
                //related products filtering
                //if (relatedToProductId > 0)
                //{
                //    query = from p in query
                //            join rp in _relatedProductRepository.Table on p.Id equals rp.ProductId2
                //            where (relatedToProductId == rp.ProductId1)
                //            select p;
                //}

                //tag filtering
                if (productTagId > 0)
                {
                    query = from p in query
                            from pt in p.ProductTags.Where(pt => pt.Id == productTagId)
                            select p;
                }

                //only distinct products (group by ID)
                //if we use standard Distinct() method, then all fields will be compared (low performance)
                //it'll not work in SQL Server Compact when searching products by a keyword)
                query = from p in query
                        group p by p.Id
                        into pGroup
                        orderby pGroup.Key
                        select pGroup.FirstOrDefault();

                //sort products
                if (orderBy == ProductSortingEnum.Position && categoryIds != null && categoryIds.Any())
                {
                    //category position
                    var firstCategoryId = categoryIds[0];
                    query = query.OrderBy(p => p.ProductCategories.FirstOrDefault(pc => pc.CategoryId == firstCategoryId).DisplayOrder);
                }
                else if (orderBy == ProductSortingEnum.Position)
                {
                    //otherwise sort by name
                    query = query.OrderBy(p => p.Name);
                }
                else if (orderBy == ProductSortingEnum.NameAsc)
                {
                    //Name: A to Z
                    query = query.OrderBy(p => p.Name);
                }
                else if (orderBy == ProductSortingEnum.NameDesc)
                {
                    //Name: Z to A
                    query = query.OrderByDescending(p => p.Name);
                }
                else if (orderBy == ProductSortingEnum.CreatedOn)
                {
                    //creation date
                    query = query.OrderByDescending(p => p.CreatedOnUtc);
                }
                else
                {
                    //actually this code is not reachable
                    query = query.OrderBy(p => p.Name);
                }

                var products = new PagedList<Product>(query, pageIndex, pageSize);


                //return products
                return products;

                #endregion
            }
        }

        /// <summary>
        /// Gets associated products
        /// </summary>
        /// <param name="parentGroupedProductId">Parent product identifier (used with grouped products)</param>
        /// <param name="storeId">Store identifier; 0 to load all records</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Products</returns>
        public virtual IList<Product> GetAssociatedProducts(int parentGroupedProductId,
            int storeId = 0, bool showHidden = false)
        {
            var query = _productRepository.Table;
            query = query.Where(x => x.ParentGroupedProductId == parentGroupedProductId);
            query = query.Where(x => !x.Deleted);
            query = query.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Id);

            var products = query.ToList();

            //ACL mapping
            if (!showHidden)
            {
                products = products.Where(x => _aclService.Authorize(x)).ToList();
            }
            //Store mapping
            if (!showHidden && storeId > 0)
            {
                products = products.Where(x => _storeMappingService.Authorize(x, storeId)).ToList();
            }

            return products;
        }
        
        /// <summary>
        /// Update product review totals
        /// </summary>
        /// <param name="product">Product</param>
        public virtual void UpdateProductReviewTotals(Product product)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            int approvedRatingSum = 0;
            int notApprovedRatingSum = 0; 
            int approvedTotalReviews = 0;
            int notApprovedTotalReviews = 0;
            var reviews = product.ProductReviews;
            foreach (var pr in reviews)
            {
                if (pr.IsApproved)
                {
                    approvedRatingSum += pr.Rating;
                    approvedTotalReviews ++;
                }
                else
                {
                    notApprovedRatingSum += pr.Rating;
                    notApprovedTotalReviews++;
                }
            }

            product.ApprovedRatingSum = approvedRatingSum;
            product.NotApprovedRatingSum = notApprovedRatingSum;
            product.ApprovedTotalReviews = approvedTotalReviews;
            product.NotApprovedTotalReviews = notApprovedTotalReviews;
            UpdateProduct(product);
        }

      
        /// <summary>
        /// Gets a product by SKU
        /// </summary>
        /// <param name="sku">SKU</param>
        /// <returns>Product</returns>
        public virtual Product GetProductBySku(string sku)
        {
            if (String.IsNullOrEmpty(sku))
                return null;

            sku = sku.Trim();

            var query = from p in _productRepository.Table
                        orderby p.Id
                        where !p.Deleted &&
                        p.Sku == sku
                        select p;
            var product = query.FirstOrDefault();
            return product;
        }

        /// <summary>
        /// Gets a products by SKU array
        /// </summary>
        /// <param name="skuArray">SKU array</param>
        /// <returns>Products</returns>
        public IList<Product> GetProductsBySku(string[] skuArray)
        {
            if (skuArray == null)
                throw new ArgumentNullException("skuArray");

            var query = _productRepository.Table;
            query = query.Where(p => !p.Deleted && skuArray.Contains(p.Sku));

            return query.ToList();
        }

        #endregion

        #region Related products

        /// <summary>
        /// Deletes a related product
        /// </summary>
        /// <param name="relatedProduct">Related product</param>
        public virtual void DeleteRelatedProduct(RelatedProduct relatedProduct)
        {
            if (relatedProduct == null)
                throw new ArgumentNullException("relatedProduct");

            _relatedProductRepository.Delete(relatedProduct);

            //event notification
            _eventPublisher.EntityDeleted(relatedProduct);
        }

        /// <summary>
        /// Gets related products by product identifier
        /// </summary>
        /// <param name="productId1">The first product identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Related products</returns>
        public virtual IList<RelatedProduct> GetRelatedProductsByProductId1(int productId1, bool showHidden = false)
        {
            var query = from rp in _relatedProductRepository.Table
                        join p in _productRepository.Table on rp.ProductId2 equals p.Id
                        where rp.ProductId1 == productId1 &&
                        !p.Deleted &&
                        (showHidden || p.Published)
                        orderby rp.DisplayOrder, rp.Id
                        select rp;
            var relatedProducts = query.ToList();

            return relatedProducts;
        }

        /// <summary>
        /// Gets a related product
        /// </summary>
        /// <param name="relatedProductId">Related product identifier</param>
        /// <returns>Related product</returns>
        public virtual RelatedProduct GetRelatedProductById(int relatedProductId)
        {
            if (relatedProductId == 0)
                return null;
            
            return _relatedProductRepository.GetById(relatedProductId);
        }

        /// <summary>
        /// Inserts a related product
        /// </summary>
        /// <param name="relatedProduct">Related product</param>
        public virtual void InsertRelatedProduct(RelatedProduct relatedProduct)
        {
            if (relatedProduct == null)
                throw new ArgumentNullException("relatedProduct");

            _relatedProductRepository.Insert(relatedProduct);

            //event notification
            _eventPublisher.EntityInserted(relatedProduct);
        }

        /// <summary>
        /// Updates a related product
        /// </summary>
        /// <param name="relatedProduct">Related product</param>
        public virtual void UpdateRelatedProduct(RelatedProduct relatedProduct)
        {
            if (relatedProduct == null)
                throw new ArgumentNullException("relatedProduct");

            _relatedProductRepository.Update(relatedProduct);

            //event notification
            _eventPublisher.EntityUpdated(relatedProduct);
        }

        #endregion

        #region Product pictures

        /// <summary>
        /// Deletes a product picture
        /// </summary>
        /// <param name="productPicture">Product picture</param>
        public virtual void DeleteProductPicture(ProductPicture productPicture)
        {
            if (productPicture == null)
                throw new ArgumentNullException("productPicture");

            _productPictureRepository.Delete(productPicture);

            //event notification
            _eventPublisher.EntityDeleted(productPicture);
        }

        /// <summary>
        /// Gets a product pictures by product identifier
        /// </summary>
        /// <param name="productId">The product identifier</param>
        /// <returns>Product pictures</returns>
        public virtual IList<ProductPicture> GetProductPicturesByProductId(int productId)
        {
            var query = from pp in _productPictureRepository.Table
                        where pp.ProductId == productId
                        orderby pp.DisplayOrder, pp.Id
                        select pp;
            var productPictures = query.ToList();
            return productPictures;
        }

        /// <summary>
        /// Gets a product picture
        /// </summary>
        /// <param name="productPictureId">Product picture identifier</param>
        /// <returns>Product picture</returns>
        public virtual ProductPicture GetProductPictureById(int productPictureId)
        {
            if (productPictureId == 0)
                return null;

            return _productPictureRepository.GetById(productPictureId);
        }

        /// <summary>
        /// Inserts a product picture
        /// </summary>
        /// <param name="productPicture">Product picture</param>
        public virtual void InsertProductPicture(ProductPicture productPicture)
        {
            if (productPicture == null)
                throw new ArgumentNullException("productPicture");

            _productPictureRepository.Insert(productPicture);

            //event notification
            _eventPublisher.EntityInserted(productPicture);
        }

        /// <summary>
        /// Updates a product picture
        /// </summary>
        /// <param name="productPicture">Product picture</param>
        public virtual void UpdateProductPicture(ProductPicture productPicture)
        {
            if (productPicture == null)
                throw new ArgumentNullException("productPicture");

            _productPictureRepository.Update(productPicture);

            //event notification
            _eventPublisher.EntityUpdated(productPicture);
        }

        /// <summary>
        /// Get the IDs of all product images 
        /// </summary>
        /// <param name="productsIds">Products IDs</param>
        /// <returns>All picture identifiers grouped by product ID</returns>
        public IDictionary<int, int[]> GetProductsImagesIds(int [] productsIds)
        {
            return _productPictureRepository.Table.Where(p => productsIds.Contains(p.ProductId))
                .GroupBy(p => p.ProductId).ToDictionary(p => p.Key, p => p.Select(p1 => p1.PictureId).ToArray());
        }

        #endregion

        #region Product reviews

        /// <summary>
        /// Gets all product reviews
        /// </summary>
        /// <param name="customerId">Customer identifier (who wrote a review); 0 to load all records</param>
        /// <param name="approved">A value indicating whether to content is approved; null to load all records</param> 
        /// <param name="fromUtc">Item creation from; null to load all records</param>
        /// <param name="toUtc">Item item creation to; null to load all records</param>
        /// <param name="message">Search title or review text; null to load all records</param>
        /// <param name="storeId">The store identifier; pass 0 to load all records</param>
        /// <param name="productId">The product identifier; pass 0 to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Reviews</returns>
        public virtual IPagedList<ProductReview> GetAllProductReviews(int customerId, bool? approved,
            DateTime? fromUtc = null, DateTime? toUtc = null,
            string message = null, int storeId = 0, int productId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _productReviewRepository.Table;
            if (approved.HasValue)
                query = query.Where(pr => pr.IsApproved == approved);
            if (customerId > 0)
                query = query.Where(pr => pr.CustomerId == customerId);
            if (fromUtc.HasValue)
                query = query.Where(pr => fromUtc.Value <= pr.CreatedOnUtc);
            if (toUtc.HasValue)
                query = query.Where(pr => toUtc.Value >= pr.CreatedOnUtc);
            if (!String.IsNullOrEmpty(message))
                query = query.Where(pr => pr.Title.Contains(message) || pr.ReviewText.Contains(message));
            if (storeId > 0)
                query = query.Where(pr => pr.StoreId == storeId);
            if (productId > 0)
                query = query.Where(pr => pr.ProductId == productId);

            query = query.OrderBy(pr => pr.CreatedOnUtc).ThenBy(pr => pr.Id);

            var productReviews = new PagedList<ProductReview>(query, pageIndex, pageSize);

            return productReviews;
        }

        /// <summary>
        /// Gets product review
        /// </summary>
        /// <param name="productReviewId">Product review identifier</param>
        /// <returns>Product review</returns>
        public virtual ProductReview GetProductReviewById(int productReviewId)
        {
            if (productReviewId == 0)
                return null;

            return _productReviewRepository.GetById(productReviewId);
        }

        /// <summary>
        /// Get product reviews by identifiers
        /// </summary>
        /// <param name="productReviewIds">Product review identifiers</param>
        /// <returns>Product reviews</returns>
        public virtual IList<ProductReview> GetProducReviewsByIds(int[] productReviewIds)
        {
            if (productReviewIds == null || productReviewIds.Length == 0)
                return new List<ProductReview>();

            var query = from pr in _productReviewRepository.Table
                        where productReviewIds.Contains(pr.Id)
                        select pr;
            var productReviews = query.ToList();
            //sort by passed identifiers
            var sortedProductReviews = new List<ProductReview>();
            foreach (int id in productReviewIds)
            {
                var productReview = productReviews.Find(x => x.Id == id);
                if (productReview != null)
                    sortedProductReviews.Add(productReview);
            }
            return sortedProductReviews;
        }

        /// <summary>
        /// Deletes a product review
        /// </summary>
        /// <param name="productReview">Product review</param>
        public virtual void DeleteProductReview(ProductReview productReview)
        {
            if (productReview == null)
                throw new ArgumentNullException("productReview");

            _productReviewRepository.Delete(productReview);

            _cacheManager.RemoveByPattern(PRODUCTS_PATTERN_KEY);
            //event notification
            _eventPublisher.EntityDeleted(productReview);
        }

        /// <summary>
        /// Deletes product reviews
        /// </summary>
        /// <param name="productReviews">Product reviews</param>
        public virtual void DeleteProductReviews(IList<ProductReview> productReviews)
        {
            if (productReviews == null)
                throw new ArgumentNullException("productReviews");

            _productReviewRepository.Delete(productReviews);

            _cacheManager.RemoveByPattern(PRODUCTS_PATTERN_KEY);
            //event notification
            foreach (var productReview in productReviews)
            {
                _eventPublisher.EntityDeleted(productReview);
            }
        }

        #endregion

        #endregion
    }
}
