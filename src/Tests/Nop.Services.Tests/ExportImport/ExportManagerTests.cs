using System;
using System.Collections.Generic;
using System.IO;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.ExportImport;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Stores;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Services.Tests.ExportImport
{
    [TestFixture]
    public class ExportManagerTests : ServiceTest
    {
        private ICategoryService _categoryService;
        private ICustomerService _customerService;
        private IPictureService _pictureService;
        private INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private IExportManager _exportManager;
        private IStoreService _storeService;
        private ProductEditorSettings _productEditorSettings;
        private IWorkContext _workContext;
        private IProductTemplateService _productTemplateService;
        private CatalogSettings _catalogSettings;
        private IGenericAttributeService _genericAttributeService;
        private ICustomerAttributeFormatter _customerAttributeFormatter;

        [SetUp]
        public new void SetUp()
        {
            _storeService = MockRepository.GenerateMock<IStoreService>();
            _categoryService = MockRepository.GenerateMock<ICategoryService>();
            _customerService = MockRepository.GenerateMock<ICustomerService>();
            _pictureService = MockRepository.GenerateMock<IPictureService>();
            _newsLetterSubscriptionService = MockRepository.GenerateMock<INewsLetterSubscriptionService>();
            _productEditorSettings = new ProductEditorSettings();
            _workContext = MockRepository.GenerateMock<IWorkContext>();
            _productTemplateService = MockRepository.GenerateMock<IProductTemplateService>();
            _catalogSettings=new CatalogSettings();
            _genericAttributeService = MockRepository.GenerateMock<IGenericAttributeService>();
            _customerAttributeFormatter = MockRepository.GenerateMock<ICustomerAttributeFormatter>();
            

            _exportManager = new ExportManager(_categoryService,
                _customerService,
                _pictureService, _newsLetterSubscriptionService,
                _storeService, _workContext, _productEditorSettings,
                 _productTemplateService,
                _catalogSettings,
                 _genericAttributeService, _customerAttributeFormatter);
        }

        protected Country GetTestCountry()
        {
            return new Country
            {
                Name = "United States",
                TwoLetterIsoCode = "US",
                ThreeLetterIsoCode = "USA",
                NumericIsoCode = 1,
                Published = true,
                DisplayOrder = 1
            };
        }

        protected Customer GetTestCustomer()
        {
            return new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                AdminComment = "some comment here",
                Active = true,
                Deleted = false,
                CreatedOnUtc = new DateTime(2010, 01, 01)
            };
        }
    }
}
