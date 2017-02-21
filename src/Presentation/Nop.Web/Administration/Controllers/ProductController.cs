using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Nop.Admin.Extensions;
using Nop.Admin.Helpers;
using Nop.Admin.Infrastructure.Cache;
using Nop.Admin.Models.Catalog;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.ExportImport;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Controllers
{
    public partial class ProductController : BaseAdminController
    {
        #region Fields

        private readonly IProductService _productService;
        private readonly IProductTemplateService _productTemplateService;
        private readonly ICategoryService _categoryService;
        private readonly ICustomerService _customerService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IPictureService _pictureService;
        private readonly IProductTagService _productTagService;
        private readonly ICopyProductService _copyProductService;
        private readonly IExportManager _exportManager;
        private readonly IImportManager _importManager;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IPermissionService _permissionService;
        private readonly IAclService _aclService;
        private readonly IStoreService _storeService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly ICacheManager _cacheManager;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ISettingService _settingService;
        
        #endregion

        #region Constructors

        public ProductController(IProductService productService,
            IProductTemplateService productTemplateService,
            ICategoryService categoryService,
            ICustomerService customerService,
            IUrlRecordService urlRecordService,
            IWorkContext workContext,
            ILanguageService languageService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            IPictureService pictureService,
            IProductTagService productTagService,
            ICopyProductService copyProductService,
            IExportManager exportManager,
            IImportManager importManager,
            ICustomerActivityService customerActivityService,
            IPermissionService permissionService,
            IAclService aclService,
            IStoreService storeService,
            IStoreMappingService storeMappingService,
            ICacheManager cacheManager,
            IDateTimeHelper dateTimeHelper,
            ISettingService settingService)
        {
            this._productService = productService;
            this._productTemplateService = productTemplateService;
            this._categoryService = categoryService;
            this._customerService = customerService;
            this._urlRecordService = urlRecordService;
            this._workContext = workContext;
            this._languageService = languageService;
            this._localizationService = localizationService;
            this._localizedEntityService = localizedEntityService;
            this._pictureService = pictureService;
            this._productTagService = productTagService;
            this._copyProductService = copyProductService;
            this._exportManager = exportManager;
            this._importManager = importManager;
            this._customerActivityService = customerActivityService;
            this._permissionService = permissionService;
            this._aclService = aclService;
            this._storeService = storeService;
            this._storeMappingService = storeMappingService;
            this._cacheManager = cacheManager;
            this._dateTimeHelper = dateTimeHelper;
            this._settingService = settingService;
        }

        #endregion 

        #region Utilities

        [NonAction]
        protected virtual void UpdateLocales(Product product, ProductModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(product,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);
                _localizedEntityService.SaveLocalizedValue(product,
                                                               x => x.ShortDescription,
                                                               localized.ShortDescription,
                                                               localized.LanguageId);
                _localizedEntityService.SaveLocalizedValue(product,
                                                               x => x.FullDescription,
                                                               localized.FullDescription,
                                                               localized.LanguageId);
                _localizedEntityService.SaveLocalizedValue(product,
                                                               x => x.MetaKeywords,
                                                               localized.MetaKeywords,
                                                               localized.LanguageId);
                _localizedEntityService.SaveLocalizedValue(product,
                                                               x => x.MetaDescription,
                                                               localized.MetaDescription,
                                                               localized.LanguageId);
                _localizedEntityService.SaveLocalizedValue(product,
                                                               x => x.MetaTitle,
                                                               localized.MetaTitle,
                                                               localized.LanguageId);

                //search engine name
                var seName = product.ValidateSeName(localized.SeName, localized.Name, false);
                _urlRecordService.SaveSlug(product, seName, localized.LanguageId);
            }
        }

        [NonAction]
        protected virtual void UpdateLocales(ProductTag productTag, ProductTagModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(productTag,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);
            }
        }
        [NonAction]
        protected virtual void UpdatePictureSeoNames(Product product)
        {
            foreach (var pp in product.ProductPictures)
                _pictureService.SetSeoFilename(pp.PictureId, _pictureService.GetPictureSeName(product.Name));
        }

        [NonAction]
        protected virtual void PrepareAclModel(ProductModel model, Product product, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (!excludeProperties && product != null)
                model.SelectedCustomerRoleIds = _aclService.GetCustomerRoleIdsWithAccess(product).ToList();

            var allRoles = _customerService.GetAllCustomerRoles(true);
            foreach (var role in allRoles)
            {
                model.AvailableCustomerRoles.Add(new SelectListItem
                {
                    Text = role.Name,
                    Value = role.Id.ToString(),
                    Selected = model.SelectedCustomerRoleIds.Contains(role.Id)
                });
            }
        }

        [NonAction]
        protected virtual void SaveProductAcl(Product product, ProductModel model)
        {
            product.SubjectToAcl = model.SelectedCustomerRoleIds.Any();

            var existingAclRecords = _aclService.GetAclRecords(product);
            var allCustomerRoles = _customerService.GetAllCustomerRoles(true);
            foreach (var customerRole in allCustomerRoles)
            {
                if (model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                {
                    //new role
                    if (existingAclRecords.Count(acl => acl.CustomerRoleId == customerRole.Id) == 0)
                        _aclService.InsertAclRecord(product, customerRole.Id);
                }
                else
                {
                    //remove role
                    var aclRecordToDelete = existingAclRecords.FirstOrDefault(acl => acl.CustomerRoleId == customerRole.Id);
                    if (aclRecordToDelete != null)
                        _aclService.DeleteAclRecord(aclRecordToDelete);
                }
            }
        }

        [NonAction]
        protected virtual void PrepareStoresMappingModel(ProductModel model, Product product, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (!excludeProperties && product != null)
                model.SelectedStoreIds = _storeMappingService.GetStoresIdsWithAccess(product).ToList();

            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                model.AvailableStores.Add(new SelectListItem
                {
                    Text = store.Name,
                    Value = store.Id.ToString(),
                    Selected = model.SelectedStoreIds.Contains(store.Id)
                });
            }
        }

        [NonAction]
        protected virtual void SaveStoreMappings(Product product, ProductModel model)
        {
            product.LimitedToStores = model.SelectedStoreIds.Any();

            var existingStoreMappings = _storeMappingService.GetStoreMappings(product);
            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        _storeMappingService.InsertStoreMapping(product, store.Id);
                }
                else
                {
                    //remove store
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                        _storeMappingService.DeleteStoreMapping(storeMappingToDelete);
                }
            }
        }

        [NonAction]
        protected virtual void PrepareCategoryMappingModel(ProductModel model, Product product, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (!excludeProperties && product != null)
                model.SelectedCategoryIds = _categoryService.GetProductCategoriesByProductId(product.Id, true).Select(c => c.CategoryId).ToList();

            var allCategories = SelectListHelper.GetCategoryList(_categoryService, _cacheManager, true);
            foreach (var c in allCategories)
            {
                c.Selected = model.SelectedCategoryIds.Contains(int.Parse(c.Value));
                model.AvailableCategories.Add(c);
            }
        }

        [NonAction]
        protected virtual void SaveCategoryMappings(Product product, ProductModel model)
        {
            var existingProductCategories = _categoryService.GetProductCategoriesByProductId(product.Id, true);

            //delete categories
            foreach (var existingProductCategory in existingProductCategories)
                if (!model.SelectedCategoryIds.Contains(existingProductCategory.CategoryId))
                    _categoryService.DeleteProductCategory(existingProductCategory);

            //add categories
            foreach (var categoryId in model.SelectedCategoryIds)
                if (existingProductCategories.FindProductCategory(product.Id, categoryId) == null)
                {
                    //find next display order
                    var displayOrder = 1;
                    var existingCategoryMapping = _categoryService.GetProductCategoriesByCategoryId(categoryId, showHidden: true);
                    if (existingCategoryMapping.Any())
                        displayOrder = existingCategoryMapping.Max(x => x.DisplayOrder) + 1;
                    _categoryService.InsertProductCategory(new ProductCategory
                    {
                        ProductId = product.Id,
                        CategoryId = categoryId,
                        DisplayOrder = displayOrder
                    });
                }
        }

        [NonAction]
        protected virtual string[] ParseProductTags(string productTags)
        {
            var result = new List<string>();
            if (!String.IsNullOrWhiteSpace(productTags))
            {
                string[] values = productTags.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string val1 in values)
                    if (!String.IsNullOrEmpty(val1.Trim()))
                        result.Add(val1.Trim());
            }
            return result.ToArray();
        }
        
        [NonAction]
        protected virtual void PrepareProductModel(ProductModel model, Product product, bool setPredefinedValues, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (product != null)
            {
                var parentGroupedProduct = _productService.GetProductById(product.ParentGroupedProductId);

                model.CreatedOn = _dateTimeHelper.ConvertToUserTime(product.CreatedOnUtc, DateTimeKind.Utc);
                model.UpdatedOn = _dateTimeHelper.ConvertToUserTime(product.UpdatedOnUtc, DateTimeKind.Utc);
            }
            
            //copy product
            if (product != null)
            {
                model.CopyProductModel.Id = product.Id;
                model.CopyProductModel.Name = string.Format(_localizationService.GetResource("Admin.Catalog.Products.Copy.Name.New"), product.Name);
                model.CopyProductModel.Published = true;
                model.CopyProductModel.CopyImages = true;
            }

            //templates
            var templates = _productTemplateService.GetAllProductTemplates();
            foreach (var template in templates)
            {
                model.AvailableProductTemplates.Add(new SelectListItem
                {
                    Text = template.Name,
                    Value = template.Id.ToString()
                });
            }
            //supported product types
            foreach (var productType in ProductType.SimpleProduct.ToSelectList(false).ToList())
            {
                var productTypeId = int.Parse(productType.Value);
                model.ProductsTypesSupportedByProductTemplates.Add(productTypeId, new List<SelectListItem>());
                foreach (var template in templates)
                {
                    if (String.IsNullOrEmpty(template.IgnoredProductTypes) ||
                        !((IList<int>)TypeDescriptor.GetConverter(typeof(List<int>)).ConvertFrom(template.IgnoredProductTypes)).Contains(productTypeId))
                    {
                        model.ProductsTypesSupportedByProductTemplates[productTypeId].Add(new SelectListItem
                        {
                            Text = template.Name,
                            Value = template.Id.ToString()
                        });
                    }
                }
            }

            //product tags
            if (product != null)
            {
                var result = new StringBuilder();
                for (int i = 0; i < product.ProductTags.Count; i++)
                {
                    var pt = product.ProductTags.ToList()[i];
                    result.Append(pt.Name);
                    if (i != product.ProductTags.Count - 1)
                        result.Append(", ");
                }
                model.ProductTags = result.ToString();
            }

            //default values
            if (setPredefinedValues)
            {
                model.AllowCustomerReviews = true;
                model.Published = true;
                model.VisibleIndividually = true;
            }

            //editor settings
            var productEditorSettings = _settingService.LoadSetting<ProductEditorSettings>();
            model.ProductEditorSettingsModel = productEditorSettings.ToModel();
        }

        [NonAction]
        protected virtual List<int> GetChildCategoryIds(int parentCategoryId)
        {
            var categoriesIds = new List<int>();
            var categories = _categoryService.GetAllCategoriesByParentCategoryId(parentCategoryId, true);
            foreach (var category in categories)
            {
                categoriesIds.Add(category.Id);
                categoriesIds.AddRange(GetChildCategoryIds(category.Id));
            }
            return categoriesIds;
        }

        #endregion

        #region Methods

        #region Product list / create / edit / delete

        //list products
        public virtual ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var model = new ProductListModel();

            //categories
            model.AvailableCategories.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var categories = SelectListHelper.GetCategoryList(_categoryService, _cacheManager, true);
            foreach (var c in categories)
                model.AvailableCategories.Add(c);

            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });

           //product types
            model.AvailableProductTypes = ProductType.SimpleProduct.ToSelectList(false).ToList();
            model.AvailableProductTypes.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            //"published" property
            //0 - all (according to "ShowHidden" parameter)
            //1 - published only
            //2 - unpublished only
            model.AvailablePublishedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Catalog.Products.List.SearchPublished.All"), Value = "0" });
            model.AvailablePublishedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Catalog.Products.List.SearchPublished.PublishedOnly"), Value = "1" });
            model.AvailablePublishedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Catalog.Products.List.SearchPublished.UnpublishedOnly"), Value = "2" });

            return View(model);
        }

        [HttpPost]
        public virtual ActionResult ProductList(DataSourceRequest command, ProductListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();

            var categoryIds = new List<int> { model.SearchCategoryId };
            //include subcategories
            if (model.SearchIncludeSubCategories && model.SearchCategoryId > 0)
                categoryIds.AddRange(GetChildCategoryIds(model.SearchCategoryId));

            //0 - all (according to "ShowHidden" parameter)
            //1 - published only
            //2 - unpublished only
            bool? overridePublished = null;
            if (model.SearchPublishedId == 1)
                overridePublished = true;
            else if (model.SearchPublishedId == 2)
                overridePublished = false;

            var products = _productService.SearchProducts(
                categoryIds: categoryIds,
                storeId: model.SearchStoreId,
                productType: model.SearchProductTypeId > 0 ? (ProductType?)model.SearchProductTypeId : null,
                keywords: model.SearchProductName,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize,
                showHidden: true,
                overridePublished: overridePublished
            );
            var gridModel = new DataSourceResult();
            gridModel.Data = products.Select(x =>
            {
                var productModel = x.ToModel();
                //little performance optimization: ensure that "FullDescription" is not returned
                productModel.FullDescription = "";

                //picture
                var defaultProductPicture = _pictureService.GetPicturesByProductId(x.Id, 1).FirstOrDefault();
                productModel.PictureThumbnailUrl = _pictureService.GetPictureUrl(defaultProductPicture, 75, true);
                //product type
                productModel.ProductTypeName = x.ProductType.GetLocalizedEnum(_localizationService, _workContext);
                return productModel;
            });
            gridModel.Total = products.TotalCount;

            return Json(gridModel);
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("go-to-product-by-sku")]
        public virtual ActionResult GoToSku(ProductListModel model)
        {
            string sku = model.GoDirectlyToSku;

            //try to load a product entity
            var product = _productService.GetProductBySku(sku);

            if (product != null)
                return RedirectToAction("Edit", "Product", new { id = product.Id });

            //not found
            return List();
        }

        //create product
        public virtual ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var model = new ProductModel();

            PrepareProductModel(model, null, true, true);
            AddLocales(_languageService, model.Locales);
            PrepareAclModel(model, null, false);
            PrepareStoresMappingModel(model, null, false);
            PrepareCategoryMappingModel(model, null, false);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult Create(ProductModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {

                //product
                var product = model.ToEntity();
                product.CreatedOnUtc = DateTime.UtcNow;
                product.UpdatedOnUtc = DateTime.UtcNow;
                _productService.InsertProduct(product);
                //search engine name
                model.SeName = product.ValidateSeName(model.SeName, product.Name, true);
                _urlRecordService.SaveSlug(product, model.SeName, 0);
                //locales
                UpdateLocales(product, model);
                //categories
                SaveCategoryMappings(product, model);
                //ACL (customer roles)
                SaveProductAcl(product, model);
                //stores
                SaveStoreMappings(product, model);
                //tags
                _productTagService.UpdateProductTags(product, ParseProductTags(model.ProductTags));

                //activity log
                _customerActivityService.InsertActivity("AddNewProduct", _localizationService.GetResource("ActivityLog.AddNewProduct"), product.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Products.Added"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = product.Id });
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            PrepareProductModel(model, null, false, true);
            PrepareAclModel(model, null, true);
            PrepareStoresMappingModel(model, null, true);
            PrepareCategoryMappingModel(model, null, true);

            return View(model);
        }

        //edit product
        public virtual ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var product = _productService.GetProductById(id);
            if (product == null || product.Deleted)
                //No product found with the specified id
                return RedirectToAction("List");

            var model = product.ToModel();
            PrepareProductModel(model, product, false, false);
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = product.GetLocalized(x => x.Name, languageId, false, false);
                locale.ShortDescription = product.GetLocalized(x => x.ShortDescription, languageId, false, false);
                locale.FullDescription = product.GetLocalized(x => x.FullDescription, languageId, false, false);
                locale.MetaKeywords = product.GetLocalized(x => x.MetaKeywords, languageId, false, false);
                locale.MetaDescription = product.GetLocalized(x => x.MetaDescription, languageId, false, false);
                locale.MetaTitle = product.GetLocalized(x => x.MetaTitle, languageId, false, false);
                locale.SeName = product.GetSeName(languageId, false, false);
            });

            PrepareAclModel(model, product, false);
            PrepareStoresMappingModel(model, product, false);
            PrepareCategoryMappingModel(model, product, false);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult Edit(ProductModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var product = _productService.GetProductById(model.Id);

            if (product == null || product.Deleted)
                //No product found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {

                //product
                product = model.ToEntity(product);

                product.UpdatedOnUtc = DateTime.UtcNow;
                _productService.UpdateProduct(product);
                //search engine name
                model.SeName = product.ValidateSeName(model.SeName, product.Name, true);
                _urlRecordService.SaveSlug(product, model.SeName, 0);
                //locales
                UpdateLocales(product, model);
                //tags
                _productTagService.UpdateProductTags(product, ParseProductTags(model.ProductTags));
                //categories
                SaveCategoryMappings(product, model);
                //ACL (customer roles)
                SaveProductAcl(product, model);
                //stores
                SaveStoreMappings(product, model);
                //picture seo names
                UpdatePictureSeoNames(product);

                //activity log
                _customerActivityService.InsertActivity("EditProduct", _localizationService.GetResource("ActivityLog.EditProduct"), product.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Products.Updated"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = product.Id });
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            PrepareProductModel(model, product, false, true);
            PrepareAclModel(model, product, true);
            PrepareStoresMappingModel(model, product, true);
            PrepareCategoryMappingModel(model, product, true);

            return View(model);
        }

        //delete product
        [HttpPost]
        public virtual ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var product = _productService.GetProductById(id);
            if (product == null)
                //No product found with the specified id
                return RedirectToAction("List");

            _productService.DeleteProduct(product);

            //activity log
            _customerActivityService.InsertActivity("DeleteProduct", _localizationService.GetResource("ActivityLog.DeleteProduct"), product.Name);

            SuccessNotification(_localizationService.GetResource("Admin.Catalog.Products.Deleted"));
            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual ActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                _productService.DeleteProducts(_productService.GetProductsByIds(selectedIds.ToArray()).ToList());
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual ActionResult CopyProduct(ProductModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var copyModel = model.CopyProductModel;
            try
            {
                var originalProduct = _productService.GetProductById(copyModel.Id);

                var newProduct = _copyProductService.CopyProduct(originalProduct,
                    copyModel.Name, copyModel.Published, copyModel.CopyImages);
                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Products.Copied"));
                return RedirectToAction("Edit", new { id = newProduct.Id });
            }
            catch (Exception exc)
            {
                ErrorNotification(exc.Message);
                return RedirectToAction("Edit", new { id = copyModel.Id });
            }
        }

        #endregion

        #region Related products

        [HttpPost]
        public virtual ActionResult RelatedProductList(DataSourceRequest command, int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();

            var relatedProducts = _productService.GetRelatedProductsByProductId1(productId, true);
            var relatedProductsModel = relatedProducts
                .Select(x => new ProductModel.RelatedProductModel
                {
                    Id = x.Id,
                    //ProductId1 = x.ProductId1,
                    ProductId2 = x.ProductId2,
                    Product2Name = _productService.GetProductById(x.ProductId2).Name,
                    DisplayOrder = x.DisplayOrder
                })
                .ToList();

            var gridModel = new DataSourceResult
            {
                Data = relatedProductsModel,
                Total = relatedProductsModel.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public virtual ActionResult RelatedProductUpdate(ProductModel.RelatedProductModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var relatedProduct = _productService.GetRelatedProductById(model.Id);
            if (relatedProduct == null)
                throw new ArgumentException("No related product found with the specified id");

            relatedProduct.DisplayOrder = model.DisplayOrder;
            _productService.UpdateRelatedProduct(relatedProduct);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual ActionResult RelatedProductDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var relatedProduct = _productService.GetRelatedProductById(id);
            if (relatedProduct == null)
                throw new ArgumentException("No related product found with the specified id");

            var productId = relatedProduct.ProductId1;

            _productService.DeleteRelatedProduct(relatedProduct);

            return new NullJsonResult();
        }

        public virtual ActionResult RelatedProductAddPopup(int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var model = new ProductModel.AddRelatedProductModel();

            //categories
            model.AvailableCategories.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var categories = SelectListHelper.GetCategoryList(_categoryService, _cacheManager, true);
            foreach (var c in categories)
                model.AvailableCategories.Add(c);

            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });

            //product types
            model.AvailableProductTypes = ProductType.SimpleProduct.ToSelectList(false).ToList();
            model.AvailableProductTypes.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            return View(model);
        }

        [HttpPost]
        public virtual ActionResult RelatedProductAddPopupList(DataSourceRequest command, ProductModel.AddRelatedProductModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();

            var products = _productService.SearchProducts(
                categoryIds: new List<int> { model.SearchCategoryId },
                storeId: model.SearchStoreId,
                productType: model.SearchProductTypeId > 0 ? (ProductType?)model.SearchProductTypeId : null,
                keywords: model.SearchProductName,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize,
                showHidden: true
                );
            var gridModel = new DataSourceResult();
            gridModel.Data = products.Select(x => x.ToModel());
            gridModel.Total = products.TotalCount;

            return Json(gridModel);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual ActionResult RelatedProductAddPopup(string btnId, string formId, ProductModel.AddRelatedProductModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (model.SelectedProductIds != null)
            {
                foreach (int id in model.SelectedProductIds)
                {
                    var product = _productService.GetProductById(id);
                    if (product != null)
                    {
                        var existingRelatedProducts = _productService.GetRelatedProductsByProductId1(model.ProductId);
                        if (existingRelatedProducts.FindRelatedProduct(model.ProductId, id) == null)
                        {
                            _productService.InsertRelatedProduct(
                                new RelatedProduct
                                {
                                    ProductId1 = model.ProductId,
                                    ProductId2 = id,
                                    DisplayOrder = 1
                                });
                        }
                    }
                }
            }

            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;
            return View(model);
        }

        #endregion

        #region Associated products

        [HttpPost]
        public virtual ActionResult AssociatedProductList(DataSourceRequest command, int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();

            var associatedProducts = _productService.GetAssociatedProducts(parentGroupedProductId: productId,
                showHidden: true);
            var associatedProductsModel = associatedProducts
                .Select(x => new ProductModel.AssociatedProductModel
                {
                    Id = x.Id,
                    ProductName = x.Name,
                    DisplayOrder = x.DisplayOrder
                })
                .ToList();

            var gridModel = new DataSourceResult
            {
                Data = associatedProductsModel,
                Total = associatedProductsModel.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public virtual ActionResult AssociatedProductUpdate(ProductModel.AssociatedProductModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var associatedProduct = _productService.GetProductById(model.Id);
            if (associatedProduct == null)
                throw new ArgumentException("No associated product found with the specified id");

            associatedProduct.DisplayOrder = model.DisplayOrder;
            _productService.UpdateProduct(associatedProduct);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual ActionResult AssociatedProductDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var product = _productService.GetProductById(id);
            if (product == null)
                throw new ArgumentException("No associated product found with the specified id");


            product.ParentGroupedProductId = 0;
            _productService.UpdateProduct(product);

            return new NullJsonResult();
        }

        public virtual ActionResult AssociatedProductAddPopup(int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var model = new ProductModel.AddAssociatedProductModel();

            //categories
            model.AvailableCategories.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var categories = SelectListHelper.GetCategoryList(_categoryService, _cacheManager, true);
            foreach (var c in categories)
                model.AvailableCategories.Add(c);

            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });

            //product types
            model.AvailableProductTypes = ProductType.SimpleProduct.ToSelectList(false).ToList();
            model.AvailableProductTypes.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            return View(model);
        }

        [HttpPost]
        public virtual ActionResult AssociatedProductAddPopupList(DataSourceRequest command, ProductModel.AddAssociatedProductModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();

            
            var products = _productService.SearchProducts(
                categoryIds: new List<int> { model.SearchCategoryId },
                storeId: model.SearchStoreId,
                productType: model.SearchProductTypeId > 0 ? (ProductType?)model.SearchProductTypeId : null,
                keywords: model.SearchProductName,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize,
                showHidden: true
                );
            var gridModel = new DataSourceResult();
            gridModel.Data = products.Select(x =>
            {
                var productModel = x.ToModel();
                //display already associated products
                var parentGroupedProduct = _productService.GetProductById(x.ParentGroupedProductId);
                if (parentGroupedProduct != null)
                {
                    productModel.AssociatedToProductId = x.ParentGroupedProductId;
                    productModel.AssociatedToProductName = parentGroupedProduct.Name;
                }
                return productModel;
            });
            gridModel.Total = products.TotalCount;

            return Json(gridModel);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual ActionResult AssociatedProductAddPopup(string btnId, string formId, ProductModel.AddAssociatedProductModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (model.SelectedProductIds != null)
            {
                foreach (int id in model.SelectedProductIds)
                {
                    var product = _productService.GetProductById(id);
                    if (product != null)
                    {
                        product.ParentGroupedProductId = model.ProductId;
                        _productService.UpdateProduct(product);
                    }
                }
            }
            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;
            return View(model);
        }

        #endregion

        #region Product pictures

        [ValidateInput(false)]
        public virtual ActionResult ProductPictureAdd(int pictureId, int displayOrder,
            string overrideAltAttribute, string overrideTitleAttribute,
            int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (pictureId == 0)
                throw new ArgumentException();

            var product = _productService.GetProductById(productId);
            if (product == null)
                throw new ArgumentException("No product found with the specified id");

            var picture = _pictureService.GetPictureById(pictureId);
            if (picture == null)
                throw new ArgumentException("No picture found with the specified id");

            _pictureService.UpdatePicture(picture.Id,
                _pictureService.LoadPictureBinary(picture),
                picture.MimeType,
                picture.SeoFilename,
                overrideAltAttribute,
                overrideTitleAttribute);

            _pictureService.SetSeoFilename(pictureId, _pictureService.GetPictureSeName(product.Name));

            _productService.InsertProductPicture(new ProductPicture
            {
                PictureId = pictureId,
                ProductId = productId,
                DisplayOrder = displayOrder,
            });

            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public virtual ActionResult ProductPictureList(DataSourceRequest command, int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();

            var productPictures = _productService.GetProductPicturesByProductId(productId);
            var productPicturesModel = productPictures
                .Select(x =>
                {
                    var picture = _pictureService.GetPictureById(x.PictureId);
                    if (picture == null)
                        throw new Exception("Picture cannot be loaded");
                    var m = new ProductModel.ProductPictureModel
                    {
                        Id = x.Id,
                        ProductId = x.ProductId,
                        PictureId = x.PictureId,
                        PictureUrl = _pictureService.GetPictureUrl(picture),
                        OverrideAltAttribute = picture.AltAttribute,
                        OverrideTitleAttribute = picture.TitleAttribute,
                        DisplayOrder = x.DisplayOrder
                    };
                    return m;
                })
                .ToList();

            var gridModel = new DataSourceResult
            {
                Data = productPicturesModel,
                Total = productPicturesModel.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public virtual ActionResult ProductPictureUpdate(ProductModel.ProductPictureModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var productPicture = _productService.GetProductPictureById(model.Id);
            if (productPicture == null)
                throw new ArgumentException("No product picture found with the specified id");

            var picture = _pictureService.GetPictureById(productPicture.PictureId);
            if (picture == null)
                throw new ArgumentException("No picture found with the specified id");

            _pictureService.UpdatePicture(picture.Id,
                _pictureService.LoadPictureBinary(picture),
                picture.MimeType,
                picture.SeoFilename,
                model.OverrideAltAttribute,
                model.OverrideTitleAttribute);

            productPicture.DisplayOrder = model.DisplayOrder;
            _productService.UpdateProductPicture(productPicture);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual ActionResult ProductPictureDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var productPicture = _productService.GetProductPictureById(id);
            if (productPicture == null)
                throw new ArgumentException("No product picture found with the specified id");

            var productId = productPicture.ProductId;

            var pictureId = productPicture.PictureId;
            _productService.DeleteProductPicture(productPicture);

            var picture = _pictureService.GetPictureById(pictureId);
            if (picture == null)
                throw new ArgumentException("No picture found with the specified id");
            _pictureService.DeletePicture(picture);

            return new NullJsonResult();
        }

        #endregion

        #region Product tags

        public virtual ActionResult ProductTags()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProductTags))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public virtual ActionResult ProductTags(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProductTags))
                return AccessDeniedKendoGridJson();

            var tags = _productTagService.GetAllProductTags()
                //order by product count
                .OrderByDescending(x => _productTagService.GetProductCount(x.Id, 0))
                .Select(x => new ProductTagModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    ProductCount = _productTagService.GetProductCount(x.Id, 0)
                })
                .ToList();

            var gridModel = new DataSourceResult
            {
                Data = tags.PagedForCommand(command),
                Total = tags.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public virtual ActionResult ProductTagDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProductTags))
                return AccessDeniedView();

            var tag = _productTagService.GetProductTagById(id);
            if (tag == null)
                throw new ArgumentException("No product tag found with the specified id");
            _productTagService.DeleteProductTag(tag);

            return new NullJsonResult();
        }

        //edit
        public virtual ActionResult EditProductTag(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProductTags))
                return AccessDeniedView();

            var productTag = _productTagService.GetProductTagById(id);
            if (productTag == null)
                //No product tag found with the specified id
                return RedirectToAction("List");

            var model = new ProductTagModel
            {
                Id = productTag.Id,
                Name = productTag.Name,
                ProductCount = _productTagService.GetProductCount(productTag.Id, 0)
            };
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = productTag.GetLocalized(x => x.Name, languageId, false, false);
            });

            return View(model);
        }

        [HttpPost]
        public virtual ActionResult EditProductTag(string btnId, string formId, ProductTagModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProductTags))
                return AccessDeniedView();

            var productTag = _productTagService.GetProductTagById(model.Id);
            if (productTag == null)
                //No product tag found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                productTag.Name = model.Name;
                _productTagService.UpdateProductTag(productTag);
                //locales
                UpdateLocales(productTag, model);

                ViewBag.RefreshPage = true;
                ViewBag.btnId = btnId;
                ViewBag.formId = formId;
                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region Export / Import


        [HttpPost, ActionName("List")]
        [FormValueRequired("exportxml-all")]
        public virtual ActionResult ExportXmlAll(ProductListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var categoryIds = new List<int> { model.SearchCategoryId };
            //include subcategories
            if (model.SearchIncludeSubCategories && model.SearchCategoryId > 0)
                categoryIds.AddRange(GetChildCategoryIds(model.SearchCategoryId));

            //0 - all (according to "ShowHidden" parameter)
            //1 - published only
            //2 - unpublished only
            bool? overridePublished = null;
            if (model.SearchPublishedId == 1)
                overridePublished = true;
            else if (model.SearchPublishedId == 2)
                overridePublished = false;

            var products = _productService.SearchProducts(
                categoryIds: categoryIds,
                storeId: model.SearchStoreId,
                productType: model.SearchProductTypeId > 0 ? (ProductType?)model.SearchProductTypeId : null,
                keywords: model.SearchProductName,
                showHidden: true,
                overridePublished: overridePublished
            );

            try
            {
                var xml = _exportManager.ExportProductsToXml(products);
                return new XmlDownloadResult(xml, "products.xml");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual ActionResult ExportXmlSelected(string selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var products = new List<Product>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                products.AddRange(_productService.GetProductsByIds(ids));
            }

            var xml = _exportManager.ExportProductsToXml(products);
            return new XmlDownloadResult(xml, "products.xml");
        }


        [HttpPost, ActionName("List")]
        [FormValueRequired("exportexcel-all")]
        public virtual ActionResult ExportExcelAll(ProductListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var categoryIds = new List<int> { model.SearchCategoryId };
            //include subcategories
            if (model.SearchIncludeSubCategories && model.SearchCategoryId > 0)
                categoryIds.AddRange(GetChildCategoryIds(model.SearchCategoryId));

            //0 - all (according to "ShowHidden" parameter)
            //1 - published only
            //2 - unpublished only
            bool? overridePublished = null;
            if (model.SearchPublishedId == 1)
                overridePublished = true;
            else if (model.SearchPublishedId == 2)
                overridePublished = false;

            var products = _productService.SearchProducts(
                categoryIds: categoryIds,
                storeId: model.SearchStoreId,
                productType: model.SearchProductTypeId > 0 ? (ProductType?)model.SearchProductTypeId : null,
                keywords: model.SearchProductName,
                showHidden: true,
                overridePublished: overridePublished
            );
            try
            {
                var bytes = _exportManager.ExportProductsToXlsx(products);

                return File(bytes, MimeTypes.TextXlsx, "products.xlsx");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual ActionResult ExportExcelSelected(string selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var products = new List<Product>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                products.AddRange(_productService.GetProductsByIds(ids));
            }

            var bytes = _exportManager.ExportProductsToXlsx(products);

            return File(bytes, MimeTypes.TextXlsx, "products.xlsx");
        }

        [HttpPost]
        public virtual ActionResult ImportExcel()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();
            
            try
            {
                var file = Request.Files["importexcelfile"];
                if (file != null && file.ContentLength > 0)
                {
                   // _importManager.ImportProductsFromXlsx(file.InputStream);
                }
                else
                {
                    ErrorNotification(_localizationService.GetResource("Admin.Common.UploadFile"));
                    return RedirectToAction("List");
                }
                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Products.Imported"));
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }

        }

        #endregion

        #region Bulk editing

        public virtual ActionResult BulkEdit()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var model = new BulkEditListModel();
            //categories
            model.AvailableCategories.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var categories = SelectListHelper.GetCategoryList(_categoryService, _cacheManager, true);
            foreach (var c in categories)
                model.AvailableCategories.Add(c);

            //product types
            model.AvailableProductTypes = ProductType.SimpleProduct.ToSelectList(false).ToList();
            model.AvailableProductTypes.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            return View(model);
        }

        [HttpPost]
        public virtual ActionResult BulkEditSelect(DataSourceRequest command, BulkEditListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();
            
            var products = _productService.SearchProducts(categoryIds: new List<int> { model.SearchCategoryId },
                productType: model.SearchProductTypeId > 0 ? (ProductType?)model.SearchProductTypeId : null,
                keywords: model.SearchProductName,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize,
                showHidden: true);
            var gridModel = new DataSourceResult();
            gridModel.Data = products.Select(x =>
            {
                var productModel = new BulkEditProductModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Sku = x.Sku,
                    Published = x.Published
                };

                return productModel;
            });
            gridModel.Total = products.TotalCount;

            return Json(gridModel);
        }

        [HttpPost]
        public virtual ActionResult BulkEditUpdate(IEnumerable<BulkEditProductModel> products)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (products != null)
            {
                foreach (var pModel in products)
                {
                    //update
                    var product = _productService.GetProductById(pModel.Id);
                    if (product != null)
                    {

                        product.Name = pModel.Name;
                        product.Sku = pModel.Sku;
                        product.Published = pModel.Published;
                        product.UpdatedOnUtc = DateTime.UtcNow;
                        _productService.UpdateProduct(product);
                    }
                }
            }

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual ActionResult BulkEditDelete(IEnumerable<BulkEditProductModel> products)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (products != null)
            {
                foreach (var pModel in products)
                {
                    //delete
                    var product = _productService.GetProductById(pModel.Id);
                    if (product != null)
                    {
                        _productService.DeleteProduct(product);
                    }
                }
            }
            return new NullJsonResult();
        }

        #endregion


        #region Product editor settings

        [HttpPost]
        public virtual ActionResult SaveProductEditorSettings(ProductModel model, string returnUrl = "")
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();
            
            var productEditorSettings = _settingService.LoadSetting<ProductEditorSettings>();
            productEditorSettings = model.ProductEditorSettingsModel.ToEntity(productEditorSettings);
            _settingService.SaveSetting(productEditorSettings);

            //product list
            if (String.IsNullOrEmpty(returnUrl))
                return RedirectToAction("List");
            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                return RedirectToAction("List");
            return Redirect(returnUrl);
        }

        #endregion

        #endregion
    }
}
