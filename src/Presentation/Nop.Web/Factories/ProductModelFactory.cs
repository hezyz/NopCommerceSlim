using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Seo;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Framework.Security.Captcha;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Common;
using Nop.Web.Models.Media;

namespace Nop.Web.Factories
{
    public partial class ProductModelFactory : IProductModelFactory
    {
        #region Fields
        
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IProductTemplateService _productTemplateService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IPictureService _pictureService;
        private readonly ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IProductTagService _productTagService;
        private readonly IAclService _aclService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IPermissionService _permissionService;
        private readonly IDownloadService _downloadService;
        private readonly MediaSettings _mediaSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly SeoSettings _seoSettings;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Constructors

        public ProductModelFactory(
            ICategoryService categoryService,
            IProductService productService,
            IProductTemplateService productTemplateService,
            IWorkContext workContext,
            IStoreContext storeContext,
            IPictureService pictureService,
            ILocalizationService localizationService,
            IWebHelper webHelper,
            IDateTimeHelper dateTimeHelper,
            IProductTagService productTagService,
            IAclService aclService,
            IStoreMappingService storeMappingService,
            IPermissionService permissionService,
            IDownloadService downloadService,
            MediaSettings mediaSettings,
            CatalogSettings catalogSettings,
            CustomerSettings customerSettings,
            CaptchaSettings captchaSettings,
            SeoSettings seoSettings,
            ICacheManager cacheManager)
        {
            this._categoryService = categoryService;
            this._productService = productService;
            this._productTemplateService = productTemplateService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._pictureService = pictureService;
            this._localizationService = localizationService;
            this._webHelper = webHelper;
            this._dateTimeHelper = dateTimeHelper;
            this._productTagService = productTagService;
            this._aclService = aclService;
            this._storeMappingService = storeMappingService;
            this._permissionService = permissionService;
            this._downloadService = downloadService;
            this._mediaSettings = mediaSettings;
            this._catalogSettings = catalogSettings;
            this._customerSettings = customerSettings;
            this._captchaSettings = captchaSettings;
            this._seoSettings = seoSettings;
            this._cacheManager = cacheManager;
        }

        #endregion

        #region Utilities

        protected virtual ProductReviewOverviewModel PrepareProductReviewOverviewModel(Product product)
        {
            ProductReviewOverviewModel productReview;

            if (_catalogSettings.ShowProductReviewsPerStore)
            {
                string cacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_REVIEWS_MODEL_KEY, product.Id, _storeContext.CurrentStore.Id);

                productReview = _cacheManager.Get(cacheKey, () =>
                {
                    return new ProductReviewOverviewModel
                    {
                        RatingSum = product.ProductReviews
                                .Where(pr => pr.IsApproved && pr.StoreId == _storeContext.CurrentStore.Id)
                                .Sum(pr => pr.Rating),
                        TotalReviews = product
                                .ProductReviews
                                .Count(pr => pr.IsApproved && pr.StoreId == _storeContext.CurrentStore.Id)
                    };
                });
            }
            else
            {
                productReview = new ProductReviewOverviewModel()
                {
                    RatingSum = product.ApprovedRatingSum,
                    TotalReviews = product.ApprovedTotalReviews
                };
            }
            if (productReview != null)
            {
                productReview.ProductId = product.Id;
                productReview.AllowCustomerReviews = product.AllowCustomerReviews;
            }
            return productReview;
        }

        protected virtual PictureModel PrepareProductOverviewPictureModel(Product product, int? productThumbPictureSize = null)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            var productName = product.GetLocalized(x => x.Name);
            //If a size has been set in the view, we use it in priority
            int pictureSize = productThumbPictureSize.HasValue
                ? productThumbPictureSize.Value
                : _mediaSettings.ProductThumbPictureSize;

            //prepare picture model
            var cacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_DEFAULTPICTURE_MODEL_KEY,
                product.Id, pictureSize, true, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured(),
                _storeContext.CurrentStore.Id);

            PictureModel defaultPictureModel = _cacheManager.Get(cacheKey, () =>
            {
                var picture = _pictureService.GetPicturesByProductId(product.Id, 1).FirstOrDefault();
                var pictureModel = new PictureModel
                {
                    ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                    FullSizeImageUrl = _pictureService.GetPictureUrl(picture)
                };
                //"title" attribute
                pictureModel.Title = (picture != null && !string.IsNullOrEmpty(picture.TitleAttribute))
                    ? picture.TitleAttribute
                    : string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat"), productName);
                //"alt" attribute
                pictureModel.AlternateText = (picture != null && !string.IsNullOrEmpty(picture.AltAttribute))
                    ? picture.AltAttribute
                    : string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat"),
                        productName);

                return pictureModel;
            });

            return defaultPictureModel;
        }

        protected virtual ProductDetailsModel.ProductBreadcrumbModel PrepareProductBreadcrumbModel(Product product)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            var cacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_BREADCRUMB_MODEL_KEY,
                    product.Id,
                    _workContext.WorkingLanguage.Id,
                    string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                    _storeContext.CurrentStore.Id);
            var cachedModel = _cacheManager.Get(cacheKey, () =>
            {
                var breadcrumbModel = new ProductDetailsModel.ProductBreadcrumbModel
                {
                    Enabled = _catalogSettings.CategoryBreadcrumbEnabled,
                    ProductId = product.Id,
                    ProductName = product.GetLocalized(x => x.Name),
                    ProductSeName = product.GetSeName()
                };
                var productCategories = _categoryService.GetProductCategoriesByProductId(product.Id);
                if (productCategories.Any())
                {
                    var category = productCategories[0].Category;
                    if (category != null)
                    {
                        foreach (var catBr in category.GetCategoryBreadCrumb(_categoryService, _aclService, _storeMappingService))
                        {
                            breadcrumbModel.CategoryBreadcrumb.Add(new CategorySimpleModel
                            {
                                Id = catBr.Id,
                                Name = catBr.GetLocalized(x => x.Name),
                                SeName = catBr.GetSeName(),
                                IncludeInTopMenu = catBr.IncludeInTopMenu
                            });
                        }
                    }
                }
                return breadcrumbModel;
            });
            return cachedModel;
        }

        protected virtual IList<ProductTagModel> PrepareProductTagModels(Product product)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            var productTagsCacheKey = string.Format(ModelCacheEventConsumer.PRODUCTTAG_BY_PRODUCT_MODEL_KEY, product.Id, _workContext.WorkingLanguage.Id, _storeContext.CurrentStore.Id);
            var model = _cacheManager.Get(productTagsCacheKey, () =>
                product.ProductTags
                //filter by store
                .Where(x => _productTagService.GetProductCount(x.Id, _storeContext.CurrentStore.Id) > 0)
                .Select(x => new ProductTagModel
                {
                    Id = x.Id,
                    Name = x.GetLocalized(y => y.Name),
                    SeName = x.GetSeName(),
                    ProductCount = _productTagService.GetProductCount(x.Id, _storeContext.CurrentStore.Id)
                })
                .ToList());

            return model;
        }
        protected virtual dynamic PrepareProductDetailsPictureModel(Product product, bool isAssociatedProduct = false)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            //default picture size
            var defaultPictureSize = isAssociatedProduct ?
                _mediaSettings.AssociatedProductPictureSize :
                _mediaSettings.ProductDetailsPictureSize;

            //prepare picture models
            var productPicturesCacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_DETAILS_PICTURES_MODEL_KEY, product.Id, defaultPictureSize, isAssociatedProduct, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured(), _storeContext.CurrentStore.Id);
            var cachedPictures = _cacheManager.Get(productPicturesCacheKey, () =>
            {
                var productName = product.GetLocalized(x => x.Name);

                var pictures = _pictureService.GetPicturesByProductId(product.Id);
                var defaultPicture = pictures.FirstOrDefault();
                var defaultPictureModel = new PictureModel
                {
                    ImageUrl = _pictureService.GetPictureUrl(defaultPicture, defaultPictureSize, !isAssociatedProduct),
                    FullSizeImageUrl = _pictureService.GetPictureUrl(defaultPicture, 0, !isAssociatedProduct)
                };
                //"title" attribute
                defaultPictureModel.Title = (defaultPicture != null && !string.IsNullOrEmpty(defaultPicture.TitleAttribute)) ?
                    defaultPicture.TitleAttribute :
                    string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat.Details"), productName);
                //"alt" attribute
                defaultPictureModel.AlternateText = (defaultPicture != null && !string.IsNullOrEmpty(defaultPicture.AltAttribute)) ?
                    defaultPicture.AltAttribute :
                    string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat.Details"), productName);

                //all pictures
                var pictureModels = new List<PictureModel>();
                foreach (var picture in pictures)
                {
                    var pictureModel = new PictureModel
                    {
                        ImageUrl = _pictureService.GetPictureUrl(picture, defaultPictureSize, !isAssociatedProduct),
                        ThumbImageUrl = _pictureService.GetPictureUrl(picture, _mediaSettings.ProductThumbPictureSizeOnProductDetailsPage),
                        FullSizeImageUrl = _pictureService.GetPictureUrl(picture),
                        Title = string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat.Details"), productName),
                        AlternateText = string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat.Details"), productName),
                    };
                    //"title" attribute
                    pictureModel.Title = !string.IsNullOrEmpty(picture.TitleAttribute) ?
                        picture.TitleAttribute :
                        string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat.Details"), productName);
                    //"alt" attribute
                    pictureModel.AlternateText = !string.IsNullOrEmpty(picture.AltAttribute) ?
                        picture.AltAttribute :
                        string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat.Details"), productName);

                    pictureModels.Add(pictureModel);
                }

                return new { DefaultPictureModel = defaultPictureModel, PictureModels = pictureModels };
            });

            return cachedPictures;
        }

        #endregion

        #region Methods

        public virtual string PrepareProductTemplateViewPath(Product product)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            var templateCacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_TEMPLATE_MODEL_KEY, product.ProductTemplateId);
            var productTemplateViewPath = _cacheManager.Get(templateCacheKey, () =>
            {
                var template = _productTemplateService.GetProductTemplateById(product.ProductTemplateId);
                if (template == null)
                    template = _productTemplateService.GetAllProductTemplates().FirstOrDefault();
                if (template == null)
                    throw new Exception("No default template could be loaded");
                return template.ViewPath;
            });

            return productTemplateViewPath;
        }

        public virtual IEnumerable<ProductOverviewModel> PrepareProductOverviewModels(IEnumerable<Product> products,
            bool preparePictureModel = true,
            int? productThumbPictureSize = null)
        {
            if (products == null)
                throw new ArgumentNullException("products");

            var models = new List<ProductOverviewModel>();
            foreach (var product in products)
            {
                var model = new ProductOverviewModel
                {
                    Id = product.Id,
                    Name = product.GetLocalized(x => x.Name),
                    ShortDescription = product.GetLocalized(x => x.ShortDescription),
                    FullDescription = product.GetLocalized(x => x.FullDescription),
                    SeName = product.GetSeName(),
                    Sku = product.Sku,
                    ProductType = product.ProductType,
                    MarkAsNew = product.MarkAsNew &&
                        (!product.MarkAsNewStartDateTimeUtc.HasValue || product.MarkAsNewStartDateTimeUtc.Value < DateTime.UtcNow) &&
                        (!product.MarkAsNewEndDateTimeUtc.HasValue || product.MarkAsNewEndDateTimeUtc.Value > DateTime.UtcNow)
                };

                //picture
                if (preparePictureModel)
                {
                    model.DefaultPictureModel = PrepareProductOverviewPictureModel(product, productThumbPictureSize);
                }

                //reviews
                model.ReviewOverviewModel = PrepareProductReviewOverviewModel(product);

                models.Add(model);
            }
            return models;
        }

        public virtual ProductDetailsModel PrepareProductDetailsModel(Product product)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            //standard properties
            var model = new ProductDetailsModel
            {
                Id = product.Id,
                Name = product.GetLocalized(x => x.Name),
                ShortDescription = product.GetLocalized(x => x.ShortDescription),
                FullDescription = product.GetLocalized(x => x.FullDescription),
                MetaKeywords = product.GetLocalized(x => x.MetaKeywords),
                MetaDescription = product.GetLocalized(x => x.MetaDescription),
                MetaTitle = product.GetLocalized(x => x.MetaTitle),
                SeName = product.GetSeName(),
                ProductType = product.ProductType,
                ShowSku = _catalogSettings.ShowSkuOnProductDetailsPage,
                Sku = product.Sku,
                DisplayDiscontinuedMessage = !product.Published && _catalogSettings.DisplayDiscontinuedMessageForUnpublishedProducts
            };

            //automatically generate product description?
            if (_seoSettings.GenerateProductMetaDescription && String.IsNullOrEmpty(model.MetaDescription))
            {
                //based on short description
                model.MetaDescription = model.ShortDescription;
            }

            //email a friend
            model.EmailAFriendEnabled = _catalogSettings.EmailAFriendEnabled;
            //store name
            model.CurrentStoreName = _storeContext.CurrentStore.GetLocalized(x => x.Name);

            //page sharing
            if (_catalogSettings.ShowShareButton && !String.IsNullOrEmpty(_catalogSettings.PageShareCode))
            {
                var shareCode = _catalogSettings.PageShareCode;
                if (_webHelper.IsCurrentConnectionSecured())
                {
                    //need to change the addthis link to be https linked when the page is, so that the page doesnt ask about mixed mode when viewed in https...
                    shareCode = shareCode.Replace("http://", "https://");
                }
                model.PageShareCode = shareCode;
            }

            //breadcrumb
            if (_catalogSettings.CategoryBreadcrumbEnabled)
            {
                model.Breadcrumb = PrepareProductBreadcrumbModel(product);
            }

            //product tags
                model.ProductTags = PrepareProductTagModels(product);
            
           //pictures
            model.DefaultPictureZoomEnabled = _mediaSettings.DefaultPictureZoomEnabled;
            var pictureModels = PrepareProductDetailsPictureModel(product);
            model.DefaultPictureModel = pictureModels.DefaultPictureModel;
            model.PictureModels = pictureModels.PictureModels;

            //product review overview
            model.ProductReviewOverview = PrepareProductReviewOverviewModel(product);

            return model;
        }

        public virtual ProductReviewsModel PrepareProductReviewsModel(ProductReviewsModel model, Product product)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (product == null)
                throw new ArgumentNullException("product");

            model.ProductId = product.Id;
            model.ProductName = product.GetLocalized(x => x.Name);
            model.ProductSeName = product.GetSeName();

            var productReviews = _catalogSettings.ShowProductReviewsPerStore
                ? product.ProductReviews.Where(pr => pr.IsApproved && pr.StoreId == _storeContext.CurrentStore.Id).OrderBy(pr => pr.CreatedOnUtc)
                : product.ProductReviews.Where(pr => pr.IsApproved).OrderBy(pr => pr.CreatedOnUtc);
            foreach (var pr in productReviews)
            {
                var customer = pr.Customer;
                model.Items.Add(new ProductReviewModel
                {
                    Id = pr.Id,
                    CustomerId = pr.CustomerId,
                    CustomerName = customer.FormatUserName(),
                    AllowViewingProfiles = _customerSettings.AllowViewingProfiles && customer != null && !customer.IsGuest(),
                    Title = pr.Title,
                    ReviewText = pr.ReviewText,
                    ReplyText = pr.ReplyText,
                    Rating = pr.Rating,
                    Helpfulness = new ProductReviewHelpfulnessModel
                    {
                        ProductReviewId = pr.Id,
                        HelpfulYesTotal = pr.HelpfulYesTotal,
                        HelpfulNoTotal = pr.HelpfulNoTotal,
                    },
                    WrittenOnStr = _dateTimeHelper.ConvertToUserTime(pr.CreatedOnUtc, DateTimeKind.Utc).ToString("g"),
                });
            }

            model.AddProductReview.CanCurrentCustomerLeaveReview = _catalogSettings.AllowAnonymousUsersToReviewProduct || !_workContext.CurrentCustomer.IsGuest();
            model.AddProductReview.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnProductReviewPage;

            return model;
        }
        
        public virtual CustomerProductReviewsModel PrepareCustomerProductReviewsModel(int? page)
        {
            var pageSize = _catalogSettings.ProductReviewsPageSizeOnAccountPage;
            int pageIndex = 0;

            if (page > 0)
            {
                pageIndex = page.Value - 1;
            }

            var list = _productService.GetAllProductReviews(customerId: _workContext.CurrentCustomer.Id, 
                approved: null, 
                pageIndex: pageIndex, 
                pageSize: pageSize);

            var productReviews = new List<CustomerProductReviewModel>();

            foreach (var review in list)
            {
                var product = review.Product;
                var productReviewModel = new CustomerProductReviewModel
                {
                    Title = review.Title,
                    ProductId = product.Id,
                    ProductName = product.GetLocalized(p => p.Name),
                    ProductSeName = product.GetSeName(),
                    Rating = review.Rating,
                    ReviewText = review.ReviewText,
                    ReplyText = review.ReplyText,
                    WrittenOnStr = _dateTimeHelper.ConvertToUserTime(product.CreatedOnUtc, DateTimeKind.Utc).ToString("g")
                };

                if (_catalogSettings.ProductReviewsMustBeApproved)
                {
                    productReviewModel.ApprovalStatus = review.IsApproved
                        ? _localizationService.GetResource("Account.CustomerProductReviews.ApprovalStatus.Approved")
                        : _localizationService.GetResource("Account.CustomerProductReviews.ApprovalStatus.Pending");
                }
                productReviews.Add(productReviewModel);
            }

            var pagerModel = new PagerModel
            {
                PageSize = list.PageSize,
                TotalRecords = list.TotalCount,
                PageIndex = list.PageIndex,
                ShowTotalSummary = false,
                RouteActionName = "CustomerProductReviewsPaged",
                UseRouteLinks = true,
                RouteValues = new CustomerProductReviewsModel.CustomerProductReviewsRouteValues { page = pageIndex }
            };

            var model = new CustomerProductReviewsModel
            {
                ProductReviews = productReviews,
                PagerModel = pagerModel
            };

            return model;
        }

        public virtual ProductEmailAFriendModel PrepareProductEmailAFriendModel(ProductEmailAFriendModel model, Product product, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (product == null)
                throw new ArgumentNullException("product");

            model.ProductId = product.Id;
            model.ProductName = product.GetLocalized(x => x.Name);
            model.ProductSeName = product.GetSeName();
            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnEmailProductToFriendPage;
            if (!excludeProperties)
            {
                model.YourEmailAddress = _workContext.CurrentCustomer.Email;
            }

            return model;
        }

        #endregion
    }
}
