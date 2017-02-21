using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Cms;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Polls;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Tasks;
using Nop.Core.Domain.Topics;
using Nop.Core.Infrastructure;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Seo;

namespace Nop.Services.Installation
{
    public partial class CodeFirstInstallationService : IInstallationService
    {
        #region Fields

        private readonly IRepository<Store> _storeRepository;
        private readonly IRepository<Language> _languageRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<CustomerPassword> _customerPasswordRepository;
        private readonly IRepository<CustomerRole> _customerRoleRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<UrlRecord> _urlRecordRepository;
        private readonly IRepository<RelatedProduct> _relatedProductRepository;
        private readonly IRepository<EmailAccount> _emailAccountRepository;
        private readonly IRepository<MessageTemplate> _messageTemplateRepository;
        private readonly IRepository<ForumGroup> _forumGroupRepository;
        private readonly IRepository<Forum> _forumRepository;
        private readonly IRepository<Country> _countryRepository;
        private readonly IRepository<StateProvince> _stateProvinceRepository;
        private readonly IRepository<BlogPost> _blogPostRepository;
        private readonly IRepository<Topic> _topicRepository;
        private readonly IRepository<NewsItem> _newsItemRepository;
        private readonly IRepository<Poll> _pollRepository;
        private readonly IRepository<ActivityLogType> _activityLogTypeRepository;
        private readonly IRepository<ActivityLog> _activityLogRepository;
        private readonly IRepository<ProductTag> _productTagRepository;
        private readonly IRepository<ProductTemplate> _productTemplateRepository;
        private readonly IRepository<CategoryTemplate> _categoryTemplateRepository;
        private readonly IRepository<TopicTemplate> _topicTemplateRepository;
        private readonly IRepository<ScheduleTask> _scheduleTaskRepository;
        private readonly IRepository<Address> _addressRepository;
        private readonly IRepository<SearchTerm> _searchTermRepository;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public CodeFirstInstallationService(IRepository<Store> storeRepository,
            IRepository<Language> languageRepository,
            IRepository<Customer> customerRepository,
            IRepository<CustomerPassword> customerPasswordRepository,
            IRepository<CustomerRole> customerRoleRepository,
            IRepository<Category> categoryRepository,
            IRepository<Product> productRepository,
            IRepository<UrlRecord> urlRecordRepository,
            IRepository<RelatedProduct> relatedProductRepository,
            IRepository<EmailAccount> emailAccountRepository,
            IRepository<MessageTemplate> messageTemplateRepository,
            IRepository<ForumGroup> forumGroupRepository,
            IRepository<Forum> forumRepository,
            IRepository<Country> countryRepository,
            IRepository<StateProvince> stateProvinceRepository,
            IRepository<BlogPost> blogPostRepository,
            IRepository<Topic> topicRepository,
            IRepository<NewsItem> newsItemRepository,
            IRepository<Poll> pollRepository,
            IRepository<ActivityLogType> activityLogTypeRepository,
            IRepository<ActivityLog> activityLogRepository,
            IRepository<ProductTag> productTagRepository,
            IRepository<ProductTemplate> productTemplateRepository,
            IRepository<CategoryTemplate> categoryTemplateRepository,
            IRepository<TopicTemplate> topicTemplateRepository,
            IRepository<ScheduleTask> scheduleTaskRepository,
            IRepository<Address> addressRepository,
            IRepository<SearchTerm> searchTermRepository,
            IGenericAttributeService genericAttributeService,
            IWebHelper webHelper)
        {
            this._storeRepository = storeRepository;
            this._languageRepository = languageRepository;
            this._customerRepository = customerRepository;
            this._customerPasswordRepository = customerPasswordRepository;
            this._customerRoleRepository = customerRoleRepository;
            this._categoryRepository = categoryRepository;
            this._productRepository = productRepository;
            this._urlRecordRepository = urlRecordRepository;
            this._relatedProductRepository = relatedProductRepository;
            this._emailAccountRepository = emailAccountRepository;
            this._messageTemplateRepository = messageTemplateRepository;
            this._forumGroupRepository = forumGroupRepository;
            this._forumRepository = forumRepository;
            this._countryRepository = countryRepository;
            this._stateProvinceRepository = stateProvinceRepository;
            this._blogPostRepository = blogPostRepository;
            this._topicRepository = topicRepository;
            this._newsItemRepository = newsItemRepository;
            this._pollRepository = pollRepository;
            this._activityLogTypeRepository = activityLogTypeRepository;
            this._activityLogRepository = activityLogRepository;
            this._productTagRepository = productTagRepository;
            this._productTemplateRepository = productTemplateRepository;
            this._categoryTemplateRepository = categoryTemplateRepository;
            this._topicTemplateRepository = topicTemplateRepository;
            this._scheduleTaskRepository = scheduleTaskRepository;
            this._addressRepository = addressRepository;
            this._searchTermRepository = searchTermRepository;
            this._genericAttributeService = genericAttributeService;
            this._webHelper = webHelper;
        }

        #endregion

        #region Utilities

        protected virtual void InstallStores()
        {
            //var storeUrl = "http://www.yourStore.com/";
            var storeUrl = _webHelper.GetStoreLocation(false);
            var stores = new List<Store>
            {
                new Store
                {
                    Name = "Your store name",
                    Url = storeUrl,
                    SslEnabled = false,
                    Hosts = "yourstore.com,www.yourstore.com",
                    DisplayOrder = 1,
                    //should we set some default company info?
                    CompanyName = "Your company name",
                    CompanyAddress = "your company country, state, zip, street, etc",
                    CompanyPhoneNumber = "(123) 456-78901",
                },
            };

            _storeRepository.Insert(stores);
        }

        protected virtual void InstallLanguages()
        {
            var language = new Language
            {
                Name = "English",
                LanguageCulture = "en-US",
                UniqueSeoCode = "en",
                FlagImageFileName = "us.png",
                Published = true,
                DisplayOrder = 1
            };
            _languageRepository.Insert(language);
        }

        protected virtual void InstallLocaleResources()
        {
            //'English' language
            var language = _languageRepository.Table.Single(l => l.Name == "English");

            //save resources
            foreach (var filePath in System.IO.Directory.EnumerateFiles(CommonHelper.MapPath("~/App_Data/Localization/"), "*.nopres.xml", SearchOption.TopDirectoryOnly))
            {
                var localesXml = File.ReadAllText(filePath);
                var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
                localizationService.ImportResourcesFromXml(language, localesXml);
            }

        }

        protected virtual void InstallCountriesAndStates()
        {
            var cUsa = new Country
            {
                Name = "United States",
                TwoLetterIsoCode = "US",
                ThreeLetterIsoCode = "USA",
                NumericIsoCode = 840,
                DisplayOrder = 1,
                Published = true,
            };
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "AA (Armed Forces Americas)",
                Abbreviation = "AA",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "AE (Armed Forces Europe)",
                Abbreviation = "AE",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Alabama",
                Abbreviation = "AL",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Alaska",
                Abbreviation = "AK",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "American Samoa",
                Abbreviation = "AS",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "AP (Armed Forces Pacific)",
                Abbreviation = "AP",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Arizona",
                Abbreviation = "AZ",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Arkansas",
                Abbreviation = "AR",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "California",
                Abbreviation = "CA",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Colorado",
                Abbreviation = "CO",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Connecticut",
                Abbreviation = "CT",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Delaware",
                Abbreviation = "DE",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "District of Columbia",
                Abbreviation = "DC",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Federated States of Micronesia",
                Abbreviation = "FM",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Florida",
                Abbreviation = "FL",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Georgia",
                Abbreviation = "GA",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Guam",
                Abbreviation = "GU",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Hawaii",
                Abbreviation = "HI",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Idaho",
                Abbreviation = "ID",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Illinois",
                Abbreviation = "IL",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Indiana",
                Abbreviation = "IN",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Iowa",
                Abbreviation = "IA",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Kansas",
                Abbreviation = "KS",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Kentucky",
                Abbreviation = "KY",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Louisiana",
                Abbreviation = "LA",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Maine",
                Abbreviation = "ME",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Marshall Islands",
                Abbreviation = "MH",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Maryland",
                Abbreviation = "MD",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Massachusetts",
                Abbreviation = "MA",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Michigan",
                Abbreviation = "MI",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Minnesota",
                Abbreviation = "MN",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Mississippi",
                Abbreviation = "MS",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Missouri",
                Abbreviation = "MO",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Montana",
                Abbreviation = "MT",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Nebraska",
                Abbreviation = "NE",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Nevada",
                Abbreviation = "NV",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "New Hampshire",
                Abbreviation = "NH",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "New Jersey",
                Abbreviation = "NJ",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "New Mexico",
                Abbreviation = "NM",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "New York",
                Abbreviation = "NY",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "North Carolina",
                Abbreviation = "NC",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "North Dakota",
                Abbreviation = "ND",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Northern Mariana Islands",
                Abbreviation = "MP",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Ohio",
                Abbreviation = "OH",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Oklahoma",
                Abbreviation = "OK",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Oregon",
                Abbreviation = "OR",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Palau",
                Abbreviation = "PW",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Pennsylvania",
                Abbreviation = "PA",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Puerto Rico",
                Abbreviation = "PR",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Rhode Island",
                Abbreviation = "RI",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "South Carolina",
                Abbreviation = "SC",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "South Dakota",
                Abbreviation = "SD",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Tennessee",
                Abbreviation = "TN",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Texas",
                Abbreviation = "TX",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Utah",
                Abbreviation = "UT",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Vermont",
                Abbreviation = "VT",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Virgin Islands",
                Abbreviation = "VI",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Virginia",
                Abbreviation = "VA",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Washington",
                Abbreviation = "WA",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "West Virginia",
                Abbreviation = "WV",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Wisconsin",
                Abbreviation = "WI",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Wyoming",
                Abbreviation = "WY",
                Published = true,
                DisplayOrder = 1,
            });
            var cCanada = new Country
            {
                Name = "Canada",
               
                
                TwoLetterIsoCode = "CA",
                ThreeLetterIsoCode = "CAN",
                NumericIsoCode = 124,
                DisplayOrder = 100,
                Published = true,
            };
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "Alberta",
                Abbreviation = "AB",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "British Columbia",
                Abbreviation = "BC",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "Manitoba",
                Abbreviation = "MB",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "New Brunswick",
                Abbreviation = "NB",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "Newfoundland and Labrador",
                Abbreviation = "NL",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "Northwest Territories",
                Abbreviation = "NT",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "Nova Scotia",
                Abbreviation = "NS",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "Nunavut",
                Abbreviation = "NU",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "Ontario",
                Abbreviation = "ON",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "Prince Edward Island",
                Abbreviation = "PE",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "Quebec",
                Abbreviation = "QC",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "Saskatchewan",
                Abbreviation = "SK",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "Yukon Territory",
                Abbreviation = "YT",
                Published = true,
                DisplayOrder = 1,
            });
            var countries = new List<Country>
                                {
                                    cUsa,
                                    cCanada,
                                    //other countries
                                    new Country
                                    {
                                        Name = "Argentina",
                                       
                                        
                                        TwoLetterIsoCode = "AR",
                                        ThreeLetterIsoCode = "ARG",
                                        NumericIsoCode = 32,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Armenia",
                                       
                                        
                                        TwoLetterIsoCode = "AM",
                                        ThreeLetterIsoCode = "ARM",
                                        NumericIsoCode = 51,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Aruba",
                                       
                                        
                                        TwoLetterIsoCode = "AW",
                                        ThreeLetterIsoCode = "ABW",
                                        NumericIsoCode = 533,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Australia",
                                       
                                        
                                        TwoLetterIsoCode = "AU",
                                        ThreeLetterIsoCode = "AUS",
                                        NumericIsoCode = 36,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Austria",
                                       
                                        
                                        TwoLetterIsoCode = "AT",
                                        ThreeLetterIsoCode = "AUT",
                                        NumericIsoCode = 40,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Azerbaijan",
                                       
                                        
                                        TwoLetterIsoCode = "AZ",
                                        ThreeLetterIsoCode = "AZE",
                                        NumericIsoCode = 31,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Bahamas",
                                       
                                        
                                        TwoLetterIsoCode = "BS",
                                        ThreeLetterIsoCode = "BHS",
                                        NumericIsoCode = 44,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Bangladesh",
                                       
                                        
                                        TwoLetterIsoCode = "BD",
                                        ThreeLetterIsoCode = "BGD",
                                        NumericIsoCode = 50,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Belarus",
                                       
                                        
                                        TwoLetterIsoCode = "BY",
                                        ThreeLetterIsoCode = "BLR",
                                        NumericIsoCode = 112,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Belgium",
                                       
                                        
                                        TwoLetterIsoCode = "BE",
                                        ThreeLetterIsoCode = "BEL",
                                        NumericIsoCode = 56,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Belize",
                                       
                                        
                                        TwoLetterIsoCode = "BZ",
                                        ThreeLetterIsoCode = "BLZ",
                                        NumericIsoCode = 84,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Bermuda",
                                       
                                        
                                        TwoLetterIsoCode = "BM",
                                        ThreeLetterIsoCode = "BMU",
                                        NumericIsoCode = 60,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Bolivia",
                                       
                                        
                                        TwoLetterIsoCode = "BO",
                                        ThreeLetterIsoCode = "BOL",
                                        NumericIsoCode = 68,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Bosnia and Herzegowina",
                                       
                                        
                                        TwoLetterIsoCode = "BA",
                                        ThreeLetterIsoCode = "BIH",
                                        NumericIsoCode = 70,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Brazil",
                                       
                                        
                                        TwoLetterIsoCode = "BR",
                                        ThreeLetterIsoCode = "BRA",
                                        NumericIsoCode = 76,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Bulgaria",
                                       
                                        
                                        TwoLetterIsoCode = "BG",
                                        ThreeLetterIsoCode = "BGR",
                                        NumericIsoCode = 100,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Cayman Islands",
                                       
                                        
                                        TwoLetterIsoCode = "KY",
                                        ThreeLetterIsoCode = "CYM",
                                        NumericIsoCode = 136,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Chile",
                                       
                                        
                                        TwoLetterIsoCode = "CL",
                                        ThreeLetterIsoCode = "CHL",
                                        NumericIsoCode = 152,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "China",
                                       
                                        
                                        TwoLetterIsoCode = "CN",
                                        ThreeLetterIsoCode = "CHN",
                                        NumericIsoCode = 156,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Colombia",
                                       
                                        
                                        TwoLetterIsoCode = "CO",
                                        ThreeLetterIsoCode = "COL",
                                        NumericIsoCode = 170,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Costa Rica",
                                       
                                        
                                        TwoLetterIsoCode = "CR",
                                        ThreeLetterIsoCode = "CRI",
                                        NumericIsoCode = 188,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Croatia",
                                       
                                        
                                        TwoLetterIsoCode = "HR",
                                        ThreeLetterIsoCode = "HRV",
                                        NumericIsoCode = 191,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Cuba",
                                       
                                        
                                        TwoLetterIsoCode = "CU",
                                        ThreeLetterIsoCode = "CUB",
                                        NumericIsoCode = 192,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Cyprus",
                                       
                                        
                                        TwoLetterIsoCode = "CY",
                                        ThreeLetterIsoCode = "CYP",
                                        NumericIsoCode = 196,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Czech Republic",
                                       
                                        
                                        TwoLetterIsoCode = "CZ",
                                        ThreeLetterIsoCode = "CZE",
                                        NumericIsoCode = 203,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Denmark",
                                       
                                        
                                        TwoLetterIsoCode = "DK",
                                        ThreeLetterIsoCode = "DNK",
                                        NumericIsoCode = 208,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Dominican Republic",
                                       
                                        
                                        TwoLetterIsoCode = "DO",
                                        ThreeLetterIsoCode = "DOM",
                                        NumericIsoCode = 214,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "East Timor",
                                       
                                        
                                        TwoLetterIsoCode = "TL",
                                        ThreeLetterIsoCode = "TLS",
                                        NumericIsoCode = 626,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Ecuador",
                                       
                                        
                                        TwoLetterIsoCode = "EC",
                                        ThreeLetterIsoCode = "ECU",
                                        NumericIsoCode = 218,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Egypt",
                                       
                                        
                                        TwoLetterIsoCode = "EG",
                                        ThreeLetterIsoCode = "EGY",
                                        NumericIsoCode = 818,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Finland",
                                       
                                        
                                        TwoLetterIsoCode = "FI",
                                        ThreeLetterIsoCode = "FIN",
                                        NumericIsoCode = 246,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "France",
                                       
                                        
                                        TwoLetterIsoCode = "FR",
                                        ThreeLetterIsoCode = "FRA",
                                        NumericIsoCode = 250,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Georgia",
                                       
                                        
                                        TwoLetterIsoCode = "GE",
                                        ThreeLetterIsoCode = "GEO",
                                        NumericIsoCode = 268,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Germany",
                                       
                                        
                                        TwoLetterIsoCode = "DE",
                                        ThreeLetterIsoCode = "DEU",
                                        NumericIsoCode = 276,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Gibraltar",
                                       
                                        
                                        TwoLetterIsoCode = "GI",
                                        ThreeLetterIsoCode = "GIB",
                                        NumericIsoCode = 292,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Greece",
                                       
                                        
                                        TwoLetterIsoCode = "GR",
                                        ThreeLetterIsoCode = "GRC",
                                        NumericIsoCode = 300,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Guatemala",
                                       
                                        
                                        TwoLetterIsoCode = "GT",
                                        ThreeLetterIsoCode = "GTM",
                                        NumericIsoCode = 320,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Hong Kong",
                                       
                                        
                                        TwoLetterIsoCode = "HK",
                                        ThreeLetterIsoCode = "HKG",
                                        NumericIsoCode = 344,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Hungary",
                                       
                                        
                                        TwoLetterIsoCode = "HU",
                                        ThreeLetterIsoCode = "HUN",
                                        NumericIsoCode = 348,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "India",
                                       
                                        
                                        TwoLetterIsoCode = "IN",
                                        ThreeLetterIsoCode = "IND",
                                        NumericIsoCode = 356,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Indonesia",
                                       
                                        
                                        TwoLetterIsoCode = "ID",
                                        ThreeLetterIsoCode = "IDN",
                                        NumericIsoCode = 360,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Ireland",
                                       
                                        
                                        TwoLetterIsoCode = "IE",
                                        ThreeLetterIsoCode = "IRL",
                                        NumericIsoCode = 372,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Israel",
                                       
                                        
                                        TwoLetterIsoCode = "IL",
                                        ThreeLetterIsoCode = "ISR",
                                        NumericIsoCode = 376,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Italy",
                                       
                                        
                                        TwoLetterIsoCode = "IT",
                                        ThreeLetterIsoCode = "ITA",
                                        NumericIsoCode = 380,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Jamaica",
                                       
                                        
                                        TwoLetterIsoCode = "JM",
                                        ThreeLetterIsoCode = "JAM",
                                        NumericIsoCode = 388,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Japan",
                                       
                                        
                                        TwoLetterIsoCode = "JP",
                                        ThreeLetterIsoCode = "JPN",
                                        NumericIsoCode = 392,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Jordan",
                                       
                                        
                                        TwoLetterIsoCode = "JO",
                                        ThreeLetterIsoCode = "JOR",
                                        NumericIsoCode = 400,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Kazakhstan",
                                       
                                        
                                        TwoLetterIsoCode = "KZ",
                                        ThreeLetterIsoCode = "KAZ",
                                        NumericIsoCode = 398,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Korea, Democratic People's Republic of",
                                       
                                        
                                        TwoLetterIsoCode = "KP",
                                        ThreeLetterIsoCode = "PRK",
                                        NumericIsoCode = 408,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Kuwait",
                                       
                                        
                                        TwoLetterIsoCode = "KW",
                                        ThreeLetterIsoCode = "KWT",
                                        NumericIsoCode = 414,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Malaysia",
                                       
                                        
                                        TwoLetterIsoCode = "MY",
                                        ThreeLetterIsoCode = "MYS",
                                        NumericIsoCode = 458,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Mexico",
                                       
                                        
                                        TwoLetterIsoCode = "MX",
                                        ThreeLetterIsoCode = "MEX",
                                        NumericIsoCode = 484,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Netherlands",
                                       
                                        
                                        TwoLetterIsoCode = "NL",
                                        ThreeLetterIsoCode = "NLD",
                                        NumericIsoCode = 528,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "New Zealand",
                                       
                                        
                                        TwoLetterIsoCode = "NZ",
                                        ThreeLetterIsoCode = "NZL",
                                        NumericIsoCode = 554,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Norway",
                                       
                                        
                                        TwoLetterIsoCode = "NO",
                                        ThreeLetterIsoCode = "NOR",
                                        NumericIsoCode = 578,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Pakistan",
                                       
                                        
                                        TwoLetterIsoCode = "PK",
                                        ThreeLetterIsoCode = "PAK",
                                        NumericIsoCode = 586,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Palestine",
                                       
                                        
                                        TwoLetterIsoCode = "PS",
                                        ThreeLetterIsoCode = "PSE",
                                        NumericIsoCode = 275,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Paraguay",
                                       
                                        
                                        TwoLetterIsoCode = "PY",
                                        ThreeLetterIsoCode = "PRY",
                                        NumericIsoCode = 600,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Peru",
                                       
                                        
                                        TwoLetterIsoCode = "PE",
                                        ThreeLetterIsoCode = "PER",
                                        NumericIsoCode = 604,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Philippines",
                                       
                                        
                                        TwoLetterIsoCode = "PH",
                                        ThreeLetterIsoCode = "PHL",
                                        NumericIsoCode = 608,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Poland",
                                       
                                        
                                        TwoLetterIsoCode = "PL",
                                        ThreeLetterIsoCode = "POL",
                                        NumericIsoCode = 616,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Portugal",
                                       
                                        
                                        TwoLetterIsoCode = "PT",
                                        ThreeLetterIsoCode = "PRT",
                                        NumericIsoCode = 620,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Puerto Rico",
                                       
                                        
                                        TwoLetterIsoCode = "PR",
                                        ThreeLetterIsoCode = "PRI",
                                        NumericIsoCode = 630,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Qatar",
                                       
                                        
                                        TwoLetterIsoCode = "QA",
                                        ThreeLetterIsoCode = "QAT",
                                        NumericIsoCode = 634,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Romania",
                                       
                                        
                                        TwoLetterIsoCode = "RO",
                                        ThreeLetterIsoCode = "ROM",
                                        NumericIsoCode = 642,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Russian Federation",
                                       
                                        
                                        TwoLetterIsoCode = "RU",
                                        ThreeLetterIsoCode = "RUS",
                                        NumericIsoCode = 643,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Saudi Arabia",
                                       
                                        
                                        TwoLetterIsoCode = "SA",
                                        ThreeLetterIsoCode = "SAU",
                                        NumericIsoCode = 682,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Singapore",
                                       
                                        
                                        TwoLetterIsoCode = "SG",
                                        ThreeLetterIsoCode = "SGP",
                                        NumericIsoCode = 702,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Slovakia (Slovak Republic)",
                                       
                                        
                                        TwoLetterIsoCode = "SK",
                                        ThreeLetterIsoCode = "SVK",
                                        NumericIsoCode = 703,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Slovenia",
                                       
                                        
                                        TwoLetterIsoCode = "SI",
                                        ThreeLetterIsoCode = "SVN",
                                        NumericIsoCode = 705,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "South Africa",
                                       
                                        
                                        TwoLetterIsoCode = "ZA",
                                        ThreeLetterIsoCode = "ZAF",
                                        NumericIsoCode = 710,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Spain",
                                       
                                        
                                        TwoLetterIsoCode = "ES",
                                        ThreeLetterIsoCode = "ESP",
                                        NumericIsoCode = 724,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Sweden",
                                       
                                        
                                        TwoLetterIsoCode = "SE",
                                        ThreeLetterIsoCode = "SWE",
                                        NumericIsoCode = 752,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Switzerland",
                                       
                                        
                                        TwoLetterIsoCode = "CH",
                                        ThreeLetterIsoCode = "CHE",
                                        NumericIsoCode = 756,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Taiwan",
                                       
                                        
                                        TwoLetterIsoCode = "TW",
                                        ThreeLetterIsoCode = "TWN",
                                        NumericIsoCode = 158,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Thailand",
                                       
                                        
                                        TwoLetterIsoCode = "TH",
                                        ThreeLetterIsoCode = "THA",
                                        NumericIsoCode = 764,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Turkey",
                                       
                                        
                                        TwoLetterIsoCode = "TR",
                                        ThreeLetterIsoCode = "TUR",
                                        NumericIsoCode = 792,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Ukraine",
                                       
                                        
                                        TwoLetterIsoCode = "UA",
                                        ThreeLetterIsoCode = "UKR",
                                        NumericIsoCode = 804,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "United Arab Emirates",
                                       
                                        
                                        TwoLetterIsoCode = "AE",
                                        ThreeLetterIsoCode = "ARE",
                                        NumericIsoCode = 784,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "United Kingdom",
                                       
                                        
                                        TwoLetterIsoCode = "GB",
                                        ThreeLetterIsoCode = "GBR",
                                        NumericIsoCode = 826,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "United States minor outlying islands",
                                       
                                        
                                        TwoLetterIsoCode = "UM",
                                        ThreeLetterIsoCode = "UMI",
                                        NumericIsoCode = 581,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Uruguay",
                                       
                                        
                                        TwoLetterIsoCode = "UY",
                                        ThreeLetterIsoCode = "URY",
                                        NumericIsoCode = 858,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Uzbekistan",
                                       
                                        
                                        TwoLetterIsoCode = "UZ",
                                        ThreeLetterIsoCode = "UZB",
                                        NumericIsoCode = 860,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Venezuela",
                                       
                                        
                                        TwoLetterIsoCode = "VE",
                                        ThreeLetterIsoCode = "VEN",
                                        NumericIsoCode = 862,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Serbia",
                                       
                                        
                                        TwoLetterIsoCode = "RS",
                                        ThreeLetterIsoCode = "SRB",
                                        NumericIsoCode = 688,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Afghanistan",
                                       
                                        
                                        TwoLetterIsoCode = "AF",
                                        ThreeLetterIsoCode = "AFG",
                                        NumericIsoCode = 4,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Albania",
                                       
                                        
                                        TwoLetterIsoCode = "AL",
                                        ThreeLetterIsoCode = "ALB",
                                        NumericIsoCode = 8,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Algeria",
                                       
                                        
                                        TwoLetterIsoCode = "DZ",
                                        ThreeLetterIsoCode = "DZA",
                                        NumericIsoCode = 12,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "American Samoa",
                                       
                                        
                                        TwoLetterIsoCode = "AS",
                                        ThreeLetterIsoCode = "ASM",
                                        NumericIsoCode = 16,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Andorra",
                                       
                                        
                                        TwoLetterIsoCode = "AD",
                                        ThreeLetterIsoCode = "AND",
                                        NumericIsoCode = 20,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Angola",
                                       
                                        
                                        TwoLetterIsoCode = "AO",
                                        ThreeLetterIsoCode = "AGO",
                                        NumericIsoCode = 24,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Anguilla",
                                       
                                        
                                        TwoLetterIsoCode = "AI",
                                        ThreeLetterIsoCode = "AIA",
                                        NumericIsoCode = 660,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Antarctica",
                                       
                                        
                                        TwoLetterIsoCode = "AQ",
                                        ThreeLetterIsoCode = "ATA",
                                        NumericIsoCode = 10,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Antigua and Barbuda",
                                       
                                        
                                        TwoLetterIsoCode = "AG",
                                        ThreeLetterIsoCode = "ATG",
                                        NumericIsoCode = 28,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Bahrain",
                                       
                                        
                                        TwoLetterIsoCode = "BH",
                                        ThreeLetterIsoCode = "BHR",
                                        NumericIsoCode = 48,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Barbados",
                                       
                                        
                                        TwoLetterIsoCode = "BB",
                                        ThreeLetterIsoCode = "BRB",
                                        NumericIsoCode = 52,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Benin",
                                       
                                        
                                        TwoLetterIsoCode = "BJ",
                                        ThreeLetterIsoCode = "BEN",
                                        NumericIsoCode = 204,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Bhutan",
                                       
                                        
                                        TwoLetterIsoCode = "BT",
                                        ThreeLetterIsoCode = "BTN",
                                        NumericIsoCode = 64,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Botswana",
                                       
                                        
                                        TwoLetterIsoCode = "BW",
                                        ThreeLetterIsoCode = "BWA",
                                        NumericIsoCode = 72,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Bouvet Island",
                                       
                                        
                                        TwoLetterIsoCode = "BV",
                                        ThreeLetterIsoCode = "BVT",
                                        NumericIsoCode = 74,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "British Indian Ocean Territory",
                                       
                                        
                                        TwoLetterIsoCode = "IO",
                                        ThreeLetterIsoCode = "IOT",
                                        NumericIsoCode = 86,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Brunei Darussalam",
                                       
                                        
                                        TwoLetterIsoCode = "BN",
                                        ThreeLetterIsoCode = "BRN",
                                        NumericIsoCode = 96,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Burkina Faso",
                                       
                                        
                                        TwoLetterIsoCode = "BF",
                                        ThreeLetterIsoCode = "BFA",
                                        NumericIsoCode = 854,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Burundi",
                                       
                                        
                                        TwoLetterIsoCode = "BI",
                                        ThreeLetterIsoCode = "BDI",
                                        NumericIsoCode = 108,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Cambodia",
                                       
                                        
                                        TwoLetterIsoCode = "KH",
                                        ThreeLetterIsoCode = "KHM",
                                        NumericIsoCode = 116,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Cameroon",
                                       
                                        
                                        TwoLetterIsoCode = "CM",
                                        ThreeLetterIsoCode = "CMR",
                                        NumericIsoCode = 120,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Cape Verde",
                                       
                                        
                                        TwoLetterIsoCode = "CV",
                                        ThreeLetterIsoCode = "CPV",
                                        NumericIsoCode = 132,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Central African Republic",
                                       
                                        
                                        TwoLetterIsoCode = "CF",
                                        ThreeLetterIsoCode = "CAF",
                                        NumericIsoCode = 140,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Chad",
                                       
                                        
                                        TwoLetterIsoCode = "TD",
                                        ThreeLetterIsoCode = "TCD",
                                        NumericIsoCode = 148,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Christmas Island",
                                       
                                        
                                        TwoLetterIsoCode = "CX",
                                        ThreeLetterIsoCode = "CXR",
                                        NumericIsoCode = 162,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Cocos (Keeling) Islands",
                                       
                                        
                                        TwoLetterIsoCode = "CC",
                                        ThreeLetterIsoCode = "CCK",
                                        NumericIsoCode = 166,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Comoros",
                                       
                                        
                                        TwoLetterIsoCode = "KM",
                                        ThreeLetterIsoCode = "COM",
                                        NumericIsoCode = 174,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Congo",
                                       
                                        
                                        TwoLetterIsoCode = "CG",
                                        ThreeLetterIsoCode = "COG",
                                        NumericIsoCode = 178,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Congo (Democratic Republic of the)",
                                       
                                        
                                        TwoLetterIsoCode = "CD",
                                        ThreeLetterIsoCode = "COD",
                                        NumericIsoCode = 180,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Cook Islands",
                                       
                                        
                                        TwoLetterIsoCode = "CK",
                                        ThreeLetterIsoCode = "COK",
                                        NumericIsoCode = 184,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Cote D'Ivoire",
                                       
                                        
                                        TwoLetterIsoCode = "CI",
                                        ThreeLetterIsoCode = "CIV",
                                        NumericIsoCode = 384,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Djibouti",
                                       
                                        
                                        TwoLetterIsoCode = "DJ",
                                        ThreeLetterIsoCode = "DJI",
                                        NumericIsoCode = 262,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Dominica",
                                       
                                        
                                        TwoLetterIsoCode = "DM",
                                        ThreeLetterIsoCode = "DMA",
                                        NumericIsoCode = 212,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "El Salvador",
                                       
                                        
                                        TwoLetterIsoCode = "SV",
                                        ThreeLetterIsoCode = "SLV",
                                        NumericIsoCode = 222,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Equatorial Guinea",
                                       
                                        
                                        TwoLetterIsoCode = "GQ",
                                        ThreeLetterIsoCode = "GNQ",
                                        NumericIsoCode = 226,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Eritrea",
                                       
                                        
                                        TwoLetterIsoCode = "ER",
                                        ThreeLetterIsoCode = "ERI",
                                        NumericIsoCode = 232,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Estonia",
                                       
                                        
                                        TwoLetterIsoCode = "EE",
                                        ThreeLetterIsoCode = "EST",
                                        NumericIsoCode = 233,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Ethiopia",
                                       
                                        
                                        TwoLetterIsoCode = "ET",
                                        ThreeLetterIsoCode = "ETH",
                                        NumericIsoCode = 231,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Falkland Islands (Malvinas)",
                                       
                                        
                                        TwoLetterIsoCode = "FK",
                                        ThreeLetterIsoCode = "FLK",
                                        NumericIsoCode = 238,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Faroe Islands",
                                       
                                        
                                        TwoLetterIsoCode = "FO",
                                        ThreeLetterIsoCode = "FRO",
                                        NumericIsoCode = 234,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Fiji",
                                       
                                        
                                        TwoLetterIsoCode = "FJ",
                                        ThreeLetterIsoCode = "FJI",
                                        NumericIsoCode = 242,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "French Guiana",
                                       
                                        
                                        TwoLetterIsoCode = "GF",
                                        ThreeLetterIsoCode = "GUF",
                                        NumericIsoCode = 254,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "French Polynesia",
                                       
                                        
                                        TwoLetterIsoCode = "PF",
                                        ThreeLetterIsoCode = "PYF",
                                        NumericIsoCode = 258,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "French Southern Territories",
                                       
                                        
                                        TwoLetterIsoCode = "TF",
                                        ThreeLetterIsoCode = "ATF",
                                        NumericIsoCode = 260,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Gabon",
                                       
                                        
                                        TwoLetterIsoCode = "GA",
                                        ThreeLetterIsoCode = "GAB",
                                        NumericIsoCode = 266,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Gambia",
                                       
                                        
                                        TwoLetterIsoCode = "GM",
                                        ThreeLetterIsoCode = "GMB",
                                        NumericIsoCode = 270,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Ghana",
                                       
                                        
                                        TwoLetterIsoCode = "GH",
                                        ThreeLetterIsoCode = "GHA",
                                        NumericIsoCode = 288,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Greenland",
                                       
                                        
                                        TwoLetterIsoCode = "GL",
                                        ThreeLetterIsoCode = "GRL",
                                        NumericIsoCode = 304,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Grenada",
                                       
                                        
                                        TwoLetterIsoCode = "GD",
                                        ThreeLetterIsoCode = "GRD",
                                        NumericIsoCode = 308,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Guadeloupe",
                                       
                                        
                                        TwoLetterIsoCode = "GP",
                                        ThreeLetterIsoCode = "GLP",
                                        NumericIsoCode = 312,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Guam",
                                       
                                        
                                        TwoLetterIsoCode = "GU",
                                        ThreeLetterIsoCode = "GUM",
                                        NumericIsoCode = 316,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Guinea",
                                       
                                        
                                        TwoLetterIsoCode = "GN",
                                        ThreeLetterIsoCode = "GIN",
                                        NumericIsoCode = 324,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Guinea-bissau",
                                       
                                        
                                        TwoLetterIsoCode = "GW",
                                        ThreeLetterIsoCode = "GNB",
                                        NumericIsoCode = 624,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Guyana",
                                       
                                        
                                        TwoLetterIsoCode = "GY",
                                        ThreeLetterIsoCode = "GUY",
                                        NumericIsoCode = 328,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Haiti",
                                       
                                        
                                        TwoLetterIsoCode = "HT",
                                        ThreeLetterIsoCode = "HTI",
                                        NumericIsoCode = 332,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Heard and Mc Donald Islands",
                                       
                                        
                                        TwoLetterIsoCode = "HM",
                                        ThreeLetterIsoCode = "HMD",
                                        NumericIsoCode = 334,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Honduras",
                                       
                                        
                                        TwoLetterIsoCode = "HN",
                                        ThreeLetterIsoCode = "HND",
                                        NumericIsoCode = 340,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Iceland",
                                       
                                        
                                        TwoLetterIsoCode = "IS",
                                        ThreeLetterIsoCode = "ISL",
                                        NumericIsoCode = 352,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Iran (Islamic Republic of)",
                                       
                                        
                                        TwoLetterIsoCode = "IR",
                                        ThreeLetterIsoCode = "IRN",
                                        NumericIsoCode = 364,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Iraq",
                                       
                                        
                                        TwoLetterIsoCode = "IQ",
                                        ThreeLetterIsoCode = "IRQ",
                                        NumericIsoCode = 368,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Kenya",
                                       
                                        
                                        TwoLetterIsoCode = "KE",
                                        ThreeLetterIsoCode = "KEN",
                                        NumericIsoCode = 404,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Kiribati",
                                       
                                        
                                        TwoLetterIsoCode = "KI",
                                        ThreeLetterIsoCode = "KIR",
                                        NumericIsoCode = 296,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Korea",
                                       
                                        
                                        TwoLetterIsoCode = "KR",
                                        ThreeLetterIsoCode = "KOR",
                                        NumericIsoCode = 410,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Kyrgyzstan",
                                       
                                        
                                        TwoLetterIsoCode = "KG",
                                        ThreeLetterIsoCode = "KGZ",
                                        NumericIsoCode = 417,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Lao People's Democratic Republic",
                                       
                                        
                                        TwoLetterIsoCode = "LA",
                                        ThreeLetterIsoCode = "LAO",
                                        NumericIsoCode = 418,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Latvia",
                                       
                                        
                                        TwoLetterIsoCode = "LV",
                                        ThreeLetterIsoCode = "LVA",
                                        NumericIsoCode = 428,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Lebanon",
                                       
                                        
                                        TwoLetterIsoCode = "LB",
                                        ThreeLetterIsoCode = "LBN",
                                        NumericIsoCode = 422,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Lesotho",
                                       
                                        
                                        TwoLetterIsoCode = "LS",
                                        ThreeLetterIsoCode = "LSO",
                                        NumericIsoCode = 426,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Liberia",
                                       
                                        
                                        TwoLetterIsoCode = "LR",
                                        ThreeLetterIsoCode = "LBR",
                                        NumericIsoCode = 430,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Libyan Arab Jamahiriya",
                                       
                                        
                                        TwoLetterIsoCode = "LY",
                                        ThreeLetterIsoCode = "LBY",
                                        NumericIsoCode = 434,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Liechtenstein",
                                       
                                        
                                        TwoLetterIsoCode = "LI",
                                        ThreeLetterIsoCode = "LIE",
                                        NumericIsoCode = 438,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Lithuania",
                                       
                                        
                                        TwoLetterIsoCode = "LT",
                                        ThreeLetterIsoCode = "LTU",
                                        NumericIsoCode = 440,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Luxembourg",
                                       
                                        
                                        TwoLetterIsoCode = "LU",
                                        ThreeLetterIsoCode = "LUX",
                                        NumericIsoCode = 442,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Macau",
                                       
                                        
                                        TwoLetterIsoCode = "MO",
                                        ThreeLetterIsoCode = "MAC",
                                        NumericIsoCode = 446,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Macedonia",
                                       
                                        
                                        TwoLetterIsoCode = "MK",
                                        ThreeLetterIsoCode = "MKD",
                                        NumericIsoCode = 807,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Madagascar",
                                       
                                        
                                        TwoLetterIsoCode = "MG",
                                        ThreeLetterIsoCode = "MDG",
                                        NumericIsoCode = 450,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Malawi",
                                       
                                        
                                        TwoLetterIsoCode = "MW",
                                        ThreeLetterIsoCode = "MWI",
                                        NumericIsoCode = 454,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Maldives",
                                       
                                        
                                        TwoLetterIsoCode = "MV",
                                        ThreeLetterIsoCode = "MDV",
                                        NumericIsoCode = 462,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Mali",
                                       
                                        
                                        TwoLetterIsoCode = "ML",
                                        ThreeLetterIsoCode = "MLI",
                                        NumericIsoCode = 466,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Malta",
                                       
                                        
                                        TwoLetterIsoCode = "MT",
                                        ThreeLetterIsoCode = "MLT",
                                        NumericIsoCode = 470,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Marshall Islands",
                                       
                                        
                                        TwoLetterIsoCode = "MH",
                                        ThreeLetterIsoCode = "MHL",
                                        NumericIsoCode = 584,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Martinique",
                                       
                                        
                                        TwoLetterIsoCode = "MQ",
                                        ThreeLetterIsoCode = "MTQ",
                                        NumericIsoCode = 474,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Mauritania",
                                       
                                        
                                        TwoLetterIsoCode = "MR",
                                        ThreeLetterIsoCode = "MRT",
                                        NumericIsoCode = 478,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Mauritius",
                                       
                                        
                                        TwoLetterIsoCode = "MU",
                                        ThreeLetterIsoCode = "MUS",
                                        NumericIsoCode = 480,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Mayotte",
                                       
                                        
                                        TwoLetterIsoCode = "YT",
                                        ThreeLetterIsoCode = "MYT",
                                        NumericIsoCode = 175,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Micronesia",
                                       
                                        
                                        TwoLetterIsoCode = "FM",
                                        ThreeLetterIsoCode = "FSM",
                                        NumericIsoCode = 583,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Moldova",
                                       
                                        
                                        TwoLetterIsoCode = "MD",
                                        ThreeLetterIsoCode = "MDA",
                                        NumericIsoCode = 498,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Monaco",
                                       
                                        
                                        TwoLetterIsoCode = "MC",
                                        ThreeLetterIsoCode = "MCO",
                                        NumericIsoCode = 492,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Mongolia",
                                       
                                        
                                        TwoLetterIsoCode = "MN",
                                        ThreeLetterIsoCode = "MNG",
                                        NumericIsoCode = 496,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Montenegro",
                                       
                                        
                                        TwoLetterIsoCode = "ME",
                                        ThreeLetterIsoCode = "MNE",
                                        NumericIsoCode = 499,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Montserrat",
                                       
                                        
                                        TwoLetterIsoCode = "MS",
                                        ThreeLetterIsoCode = "MSR",
                                        NumericIsoCode = 500,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Morocco",
                                       
                                        
                                        TwoLetterIsoCode = "MA",
                                        ThreeLetterIsoCode = "MAR",
                                        NumericIsoCode = 504,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Mozambique",
                                       
                                        
                                        TwoLetterIsoCode = "MZ",
                                        ThreeLetterIsoCode = "MOZ",
                                        NumericIsoCode = 508,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Myanmar",
                                       
                                        
                                        TwoLetterIsoCode = "MM",
                                        ThreeLetterIsoCode = "MMR",
                                        NumericIsoCode = 104,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Namibia",
                                       
                                        
                                        TwoLetterIsoCode = "NA",
                                        ThreeLetterIsoCode = "NAM",
                                        NumericIsoCode = 516,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Nauru",
                                       
                                        
                                        TwoLetterIsoCode = "NR",
                                        ThreeLetterIsoCode = "NRU",
                                        NumericIsoCode = 520,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Nepal",
                                       
                                        
                                        TwoLetterIsoCode = "NP",
                                        ThreeLetterIsoCode = "NPL",
                                        NumericIsoCode = 524,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Netherlands Antilles",
                                       
                                        
                                        TwoLetterIsoCode = "AN",
                                        ThreeLetterIsoCode = "ANT",
                                        NumericIsoCode = 530,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "New Caledonia",
                                       
                                        
                                        TwoLetterIsoCode = "NC",
                                        ThreeLetterIsoCode = "NCL",
                                        NumericIsoCode = 540,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Nicaragua",
                                       
                                        
                                        TwoLetterIsoCode = "NI",
                                        ThreeLetterIsoCode = "NIC",
                                        NumericIsoCode = 558,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Niger",
                                       
                                        
                                        TwoLetterIsoCode = "NE",
                                        ThreeLetterIsoCode = "NER",
                                        NumericIsoCode = 562,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Nigeria",
                                       
                                        
                                        TwoLetterIsoCode = "NG",
                                        ThreeLetterIsoCode = "NGA",
                                        NumericIsoCode = 566,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Niue",
                                       
                                        
                                        TwoLetterIsoCode = "NU",
                                        ThreeLetterIsoCode = "NIU",
                                        NumericIsoCode = 570,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Norfolk Island",
                                       
                                        
                                        TwoLetterIsoCode = "NF",
                                        ThreeLetterIsoCode = "NFK",
                                        NumericIsoCode = 574,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Northern Mariana Islands",
                                       
                                        
                                        TwoLetterIsoCode = "MP",
                                        ThreeLetterIsoCode = "MNP",
                                        NumericIsoCode = 580,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Oman",
                                       
                                        
                                        TwoLetterIsoCode = "OM",
                                        ThreeLetterIsoCode = "OMN",
                                        NumericIsoCode = 512,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Palau",
                                       
                                        
                                        TwoLetterIsoCode = "PW",
                                        ThreeLetterIsoCode = "PLW",
                                        NumericIsoCode = 585,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Panama",
                                       
                                        
                                        TwoLetterIsoCode = "PA",
                                        ThreeLetterIsoCode = "PAN",
                                        NumericIsoCode = 591,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Papua New Guinea",
                                       
                                        
                                        TwoLetterIsoCode = "PG",
                                        ThreeLetterIsoCode = "PNG",
                                        NumericIsoCode = 598,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Pitcairn",
                                       
                                        
                                        TwoLetterIsoCode = "PN",
                                        ThreeLetterIsoCode = "PCN",
                                        NumericIsoCode = 612,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Reunion",
                                       
                                        
                                        TwoLetterIsoCode = "RE",
                                        ThreeLetterIsoCode = "REU",
                                        NumericIsoCode = 638,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Rwanda",
                                       
                                        
                                        TwoLetterIsoCode = "RW",
                                        ThreeLetterIsoCode = "RWA",
                                        NumericIsoCode = 646,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Saint Kitts and Nevis",
                                       
                                        
                                        TwoLetterIsoCode = "KN",
                                        ThreeLetterIsoCode = "KNA",
                                        NumericIsoCode = 659,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Saint Lucia",
                                       
                                        
                                        TwoLetterIsoCode = "LC",
                                        ThreeLetterIsoCode = "LCA",
                                        NumericIsoCode = 662,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Saint Vincent and the Grenadines",
                                       
                                        
                                        TwoLetterIsoCode = "VC",
                                        ThreeLetterIsoCode = "VCT",
                                        NumericIsoCode = 670,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Samoa",
                                       
                                        
                                        TwoLetterIsoCode = "WS",
                                        ThreeLetterIsoCode = "WSM",
                                        NumericIsoCode = 882,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "San Marino",
                                       
                                        
                                        TwoLetterIsoCode = "SM",
                                        ThreeLetterIsoCode = "SMR",
                                        NumericIsoCode = 674,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Sao Tome and Principe",
                                       
                                        
                                        TwoLetterIsoCode = "ST",
                                        ThreeLetterIsoCode = "STP",
                                        NumericIsoCode = 678,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Senegal",
                                       
                                        
                                        TwoLetterIsoCode = "SN",
                                        ThreeLetterIsoCode = "SEN",
                                        NumericIsoCode = 686,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Seychelles",
                                       
                                        
                                        TwoLetterIsoCode = "SC",
                                        ThreeLetterIsoCode = "SYC",
                                        NumericIsoCode = 690,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Sierra Leone",
                                       
                                        
                                        TwoLetterIsoCode = "SL",
                                        ThreeLetterIsoCode = "SLE",
                                        NumericIsoCode = 694,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Solomon Islands",
                                       
                                        
                                        TwoLetterIsoCode = "SB",
                                        ThreeLetterIsoCode = "SLB",
                                        NumericIsoCode = 90,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Somalia",
                                       
                                        
                                        TwoLetterIsoCode = "SO",
                                        ThreeLetterIsoCode = "SOM",
                                        NumericIsoCode = 706,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "South Georgia & South Sandwich Islands",
                                       
                                        
                                        TwoLetterIsoCode = "GS",
                                        ThreeLetterIsoCode = "SGS",
                                        NumericIsoCode = 239,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "South Sudan",
                                       
                                        
                                        TwoLetterIsoCode = "SS",
                                        ThreeLetterIsoCode = "SSD",
                                        NumericIsoCode = 728,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Sri Lanka",
                                       
                                        
                                        TwoLetterIsoCode = "LK",
                                        ThreeLetterIsoCode = "LKA",
                                        NumericIsoCode = 144,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "St. Helena",
                                       
                                        
                                        TwoLetterIsoCode = "SH",
                                        ThreeLetterIsoCode = "SHN",
                                        NumericIsoCode = 654,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "St. Pierre and Miquelon",
                                       
                                        
                                        TwoLetterIsoCode = "PM",
                                        ThreeLetterIsoCode = "SPM",
                                        NumericIsoCode = 666,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Sudan",
                                       
                                        
                                        TwoLetterIsoCode = "SD",
                                        ThreeLetterIsoCode = "SDN",
                                        NumericIsoCode = 736,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Suriname",
                                       
                                        
                                        TwoLetterIsoCode = "SR",
                                        ThreeLetterIsoCode = "SUR",
                                        NumericIsoCode = 740,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Svalbard and Jan Mayen Islands",
                                       
                                        
                                        TwoLetterIsoCode = "SJ",
                                        ThreeLetterIsoCode = "SJM",
                                        NumericIsoCode = 744,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Swaziland",
                                       
                                        
                                        TwoLetterIsoCode = "SZ",
                                        ThreeLetterIsoCode = "SWZ",
                                        NumericIsoCode = 748,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Syrian Arab Republic",
                                       
                                        
                                        TwoLetterIsoCode = "SY",
                                        ThreeLetterIsoCode = "SYR",
                                        NumericIsoCode = 760,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Tajikistan",
                                       
                                        
                                        TwoLetterIsoCode = "TJ",
                                        ThreeLetterIsoCode = "TJK",
                                        NumericIsoCode = 762,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Tanzania",
                                       
                                        
                                        TwoLetterIsoCode = "TZ",
                                        ThreeLetterIsoCode = "TZA",
                                        NumericIsoCode = 834,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Togo",
                                       
                                        
                                        TwoLetterIsoCode = "TG",
                                        ThreeLetterIsoCode = "TGO",
                                        NumericIsoCode = 768,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Tokelau",
                                       
                                        
                                        TwoLetterIsoCode = "TK",
                                        ThreeLetterIsoCode = "TKL",
                                        NumericIsoCode = 772,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Tonga",
                                       
                                        
                                        TwoLetterIsoCode = "TO",
                                        ThreeLetterIsoCode = "TON",
                                        NumericIsoCode = 776,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Trinidad and Tobago",
                                       
                                        
                                        TwoLetterIsoCode = "TT",
                                        ThreeLetterIsoCode = "TTO",
                                        NumericIsoCode = 780,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Tunisia",
                                       
                                        
                                        TwoLetterIsoCode = "TN",
                                        ThreeLetterIsoCode = "TUN",
                                        NumericIsoCode = 788,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Turkmenistan",
                                       
                                        
                                        TwoLetterIsoCode = "TM",
                                        ThreeLetterIsoCode = "TKM",
                                        NumericIsoCode = 795,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Turks and Caicos Islands",
                                       
                                        
                                        TwoLetterIsoCode = "TC",
                                        ThreeLetterIsoCode = "TCA",
                                        NumericIsoCode = 796,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Tuvalu",
                                       
                                        
                                        TwoLetterIsoCode = "TV",
                                        ThreeLetterIsoCode = "TUV",
                                        NumericIsoCode = 798,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Uganda",
                                       
                                        
                                        TwoLetterIsoCode = "UG",
                                        ThreeLetterIsoCode = "UGA",
                                        NumericIsoCode = 800,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Vanuatu",
                                       
                                        
                                        TwoLetterIsoCode = "VU",
                                        ThreeLetterIsoCode = "VUT",
                                        NumericIsoCode = 548,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Vatican City State (Holy See)",
                                       
                                        
                                        TwoLetterIsoCode = "VA",
                                        ThreeLetterIsoCode = "VAT",
                                        NumericIsoCode = 336,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Viet Nam",
                                       
                                        
                                        TwoLetterIsoCode = "VN",
                                        ThreeLetterIsoCode = "VNM",
                                        NumericIsoCode = 704,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Virgin Islands (British)",
                                       
                                        
                                        TwoLetterIsoCode = "VG",
                                        ThreeLetterIsoCode = "VGB",
                                        NumericIsoCode = 92,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Virgin Islands (U.S.)",
                                       
                                        
                                        TwoLetterIsoCode = "VI",
                                        ThreeLetterIsoCode = "VIR",
                                        NumericIsoCode = 850,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Wallis and Futuna Islands",
                                       
                                        
                                        TwoLetterIsoCode = "WF",
                                        ThreeLetterIsoCode = "WLF",
                                        NumericIsoCode = 876,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Western Sahara",
                                       
                                        
                                        TwoLetterIsoCode = "EH",
                                        ThreeLetterIsoCode = "ESH",
                                        NumericIsoCode = 732,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Yemen",
                                       
                                        
                                        TwoLetterIsoCode = "YE",
                                        ThreeLetterIsoCode = "YEM",
                                        NumericIsoCode = 887,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Zambia",
                                       
                                        
                                        TwoLetterIsoCode = "ZM",
                                        ThreeLetterIsoCode = "ZMB",
                                        NumericIsoCode = 894,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Zimbabwe",
                                       
                                        
                                        TwoLetterIsoCode = "ZW",
                                        ThreeLetterIsoCode = "ZWE",
                                        NumericIsoCode = 716,
                                        
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                };
            _countryRepository.Insert(countries);
        }

        protected virtual void InstallCustomersAndUsers(string defaultUserEmail, string defaultUserPassword)
        {
            var crAdministrators = new CustomerRole
            {
                Name = "Administrators",
                Active = true,
                IsSystemRole = true,
                SystemName = SystemCustomerRoleNames.Administrators,
            };
            var crForumModerators = new CustomerRole
            {
                Name = "Forum Moderators",
                Active = true,
                IsSystemRole = true,
                SystemName = SystemCustomerRoleNames.ForumModerators,
            };
            var crRegistered = new CustomerRole
            {
                Name = "Registered",
                Active = true,
                IsSystemRole = true,
                SystemName = SystemCustomerRoleNames.Registered,
            };
            var crGuests = new CustomerRole
            {
                Name = "Guests",
                Active = true,
                IsSystemRole = true,
                SystemName = SystemCustomerRoleNames.Guests,
            };
            var customerRoles = new List<CustomerRole>
                                {
                                    crAdministrators,
                                    crForumModerators,
                                    crRegistered,
                                    crGuests
                                };
            _customerRoleRepository.Insert(customerRoles);

            //default store 
            var defaultStore = _storeRepository.Table.FirstOrDefault();

            if (defaultStore == null)
                throw new Exception("No default store could be loaded");

            var storeId = defaultStore.Id;

            //admin user
            var adminUser = new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                Email = defaultUserEmail,
                Username = defaultUserEmail,
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeId
            };

            var defaultAdminUserAddress = new Address
            {
                FirstName = "John",
                LastName = "Smith",
                PhoneNumber = "12345678",
                Email = defaultUserEmail,
                FaxNumber = "",
                Company = "Nop Solutions Ltd",
                Address1 = "21 West 52nd Street",
                Address2 = "",
                City = "New York",
                StateProvince = _stateProvinceRepository.Table.FirstOrDefault(sp => sp.Name == "New York"),
                Country = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "USA"),
                ZipPostalCode = "10021",
                CreatedOnUtc = DateTime.UtcNow,
            };
            adminUser.Addresses.Add(defaultAdminUserAddress);

            adminUser.CustomerRoles.Add(crAdministrators);
            adminUser.CustomerRoles.Add(crForumModerators);
            adminUser.CustomerRoles.Add(crRegistered);

            _customerRepository.Insert(adminUser);
            //set default customer name
            _genericAttributeService.SaveAttribute(adminUser, SystemCustomerAttributeNames.FirstName, "John");
            _genericAttributeService.SaveAttribute(adminUser, SystemCustomerAttributeNames.LastName, "Smith");

            //set hashed admin password
            var customerRegistrationService = EngineContext.Current.Resolve<ICustomerRegistrationService>();
            customerRegistrationService.ChangePassword(new ChangePasswordRequest(defaultUserEmail, false,
                 PasswordFormat.Hashed, defaultUserPassword));

            //second user
            var secondUserEmail = "steve_gates@nopCommerce.com";
            var secondUser = new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                Email = secondUserEmail,
                Username = secondUserEmail,
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeId
            };
            var defaultSecondUserAddress = new Address
            {
                FirstName = "Steve",
                LastName = "Gates",
                PhoneNumber = "87654321",
                Email = secondUserEmail,
                FaxNumber = "",
                Company = "Steve Company",
                Address1 = "750 Bel Air Rd.",
                Address2 = "",
                City = "Los Angeles",
                StateProvince = _stateProvinceRepository.Table.FirstOrDefault(sp => sp.Name == "California"),
                Country = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "USA"),
                ZipPostalCode = "90077",
                CreatedOnUtc = DateTime.UtcNow,
            };
            secondUser.Addresses.Add(defaultSecondUserAddress);

            secondUser.CustomerRoles.Add(crRegistered);

            _customerRepository.Insert(secondUser);
            //set default customer name
            _genericAttributeService.SaveAttribute(secondUser, SystemCustomerAttributeNames.FirstName, defaultSecondUserAddress.FirstName);
            _genericAttributeService.SaveAttribute(secondUser, SystemCustomerAttributeNames.LastName, defaultSecondUserAddress.LastName);

            //set customer password
            _customerPasswordRepository.Insert(new CustomerPassword
            {
                Customer = secondUser,
                Password = "123456",
                PasswordFormat = PasswordFormat.Clear,
                PasswordSalt = string.Empty,
                CreatedOnUtc = DateTime.UtcNow
            });

            //third user
            var thirdUserEmail = "arthur_holmes@nopCommerce.com";
            var thirdUser = new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                Email = thirdUserEmail,
                Username = thirdUserEmail,
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeId
            };
            var defaultThirdUserAddress = new Address
            {
                FirstName = "Arthur",
                LastName = "Holmes",
                PhoneNumber = "111222333",
                Email = thirdUserEmail,
                FaxNumber = "",
                Company = "Holmes Company",
                Address1 = "221B Baker Street",
                Address2 = "",
                City = "London",
                Country = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "GBR"),
                ZipPostalCode = "NW1 6XE",
                CreatedOnUtc = DateTime.UtcNow,
            };
            thirdUser.Addresses.Add(defaultThirdUserAddress);

            thirdUser.CustomerRoles.Add(crRegistered);

            _customerRepository.Insert(thirdUser);
            //set default customer name
            _genericAttributeService.SaveAttribute(thirdUser, SystemCustomerAttributeNames.FirstName, defaultThirdUserAddress.FirstName);
            _genericAttributeService.SaveAttribute(thirdUser, SystemCustomerAttributeNames.LastName, defaultThirdUserAddress.LastName);

            //set customer password
            _customerPasswordRepository.Insert(new CustomerPassword
            {
                Customer = thirdUser,
                Password = "123456",
                PasswordFormat = PasswordFormat.Clear,
                PasswordSalt = string.Empty,
                CreatedOnUtc = DateTime.UtcNow
            });

            //fourth user
            var fourthUserEmail = "james_pan@nopCommerce.com";
            var fourthUser = new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                Email = fourthUserEmail,
                Username = fourthUserEmail,
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeId
            };
            var defaultFourthUserAddress = new Address
            {
                FirstName = "James",
                LastName = "Pan",
                PhoneNumber = "369258147",
                Email = fourthUserEmail,
                FaxNumber = "",
                Company = "Pan Company",
                Address1 = "St Katharine’s West 16",
                Address2 = "",
                City = "St Andrews",
                Country = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "GBR"),
                ZipPostalCode = "KY16 9AX",
                CreatedOnUtc = DateTime.UtcNow,
            };
            fourthUser.Addresses.Add(defaultFourthUserAddress);

            fourthUser.CustomerRoles.Add(crRegistered);

            _customerRepository.Insert(fourthUser);
            //set default customer name
            _genericAttributeService.SaveAttribute(fourthUser, SystemCustomerAttributeNames.FirstName, defaultFourthUserAddress.FirstName);
            _genericAttributeService.SaveAttribute(fourthUser, SystemCustomerAttributeNames.LastName, defaultFourthUserAddress.LastName);

            //set customer password
            _customerPasswordRepository.Insert(new CustomerPassword
            {
                Customer = fourthUser,
                Password = "123456",
                PasswordFormat = PasswordFormat.Clear,
                PasswordSalt = string.Empty,
                CreatedOnUtc = DateTime.UtcNow
            });

            //fifth user
            var fifthUserEmail = "brenda_lindgren@nopCommerce.com";
            var fifthUser = new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                Email = fifthUserEmail,
                Username = fifthUserEmail,
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeId
            };
            var defaultFifthUserAddress = new Address
            {
                FirstName = "Brenda",
                LastName = "Lindgren",
                PhoneNumber = "14785236",
                Email = fifthUserEmail,
                FaxNumber = "",
                Company = "Brenda Company",
                Address1 = "1249 Tongass Avenue, Suite B",
                Address2 = "",
                City = "Ketchikan",
                StateProvince = _stateProvinceRepository.Table.FirstOrDefault(sp => sp.Name == "Alaska"),
                Country = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "USA"),
                ZipPostalCode = "99901",
                CreatedOnUtc = DateTime.UtcNow,
            };
            fifthUser.Addresses.Add(defaultFifthUserAddress);

            fifthUser.CustomerRoles.Add(crRegistered);

            _customerRepository.Insert(fifthUser);
            //set default customer name
            _genericAttributeService.SaveAttribute(fifthUser, SystemCustomerAttributeNames.FirstName, defaultFifthUserAddress.FirstName);
            _genericAttributeService.SaveAttribute(fifthUser, SystemCustomerAttributeNames.LastName, defaultFifthUserAddress.LastName);

            //set customer password
            _customerPasswordRepository.Insert(new CustomerPassword
            {
                Customer = fifthUser,
                Password = "123456",
                PasswordFormat = PasswordFormat.Clear,
                PasswordSalt = string.Empty,
                CreatedOnUtc = DateTime.UtcNow
            });

            //sixth user
            var sixthUserEmail = "victoria_victoria@nopCommerce.com";
            var sixthUser = new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                Email = sixthUserEmail,
                Username = sixthUserEmail,
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeId
            };
            var defaultSixthUserAddress = new Address
            {
                FirstName = "Victoria",
                LastName = "Terces",
                PhoneNumber = "45612378",
                Email = sixthUserEmail,
                FaxNumber = "",
                Company = "Terces Company",
                Address1 = "201 1st Avenue South",
                Address2 = "",
                City = "Saskatoon",
                StateProvince = _stateProvinceRepository.Table.FirstOrDefault(sp => sp.Name == "Saskatchewan"),
                Country = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "CAN"),
                ZipPostalCode = "S7K 1J9",
                CreatedOnUtc = DateTime.UtcNow,
            };
            sixthUser.Addresses.Add(defaultSixthUserAddress);

            sixthUser.CustomerRoles.Add(crRegistered);

            _customerRepository.Insert(sixthUser);
            //set default customer name
            _genericAttributeService.SaveAttribute(sixthUser, SystemCustomerAttributeNames.FirstName, defaultSixthUserAddress.FirstName);
            _genericAttributeService.SaveAttribute(sixthUser, SystemCustomerAttributeNames.LastName, defaultSixthUserAddress.LastName);

            //set customer password
            _customerPasswordRepository.Insert(new CustomerPassword
            {
                Customer = sixthUser,
                Password = "123456",
                PasswordFormat = PasswordFormat.Clear,
                PasswordSalt = string.Empty,
                CreatedOnUtc = DateTime.UtcNow
            });

            //search engine (crawler) built-in user
            var searchEngineUser = new Customer
            {
                Email = "builtin@search_engine_record.com",
                CustomerGuid = Guid.NewGuid(),
                AdminComment = "Built-in system guest record used for requests from search engines.",
                Active = true,
                IsSystemAccount = true,
                SystemName = SystemCustomerNames.SearchEngine,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeId
            };
            searchEngineUser.CustomerRoles.Add(crGuests);
            _customerRepository.Insert(searchEngineUser);


            //built-in user for background tasks
            var backgroundTaskUser = new Customer
            {
                Email = "builtin@background-task-record.com",
                CustomerGuid = Guid.NewGuid(),
                AdminComment = "Built-in system record used for background tasks.",
                Active = true,
                IsSystemAccount = true,
                SystemName = SystemCustomerNames.BackgroundTask,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeId
            };
            backgroundTaskUser.CustomerRoles.Add(crGuests);
            _customerRepository.Insert(backgroundTaskUser);
        }

        protected virtual void InstallActivityLog(string defaultUserEmail)
        {
            //default customer/user
            var defaultCustomer = _customerRepository.Table.FirstOrDefault(x => x.Email == defaultUserEmail);
            if (defaultCustomer == null)
                throw new Exception("Cannot load default customer");

            _activityLogRepository.Insert(new ActivityLog()
            {
                ActivityLogType = _activityLogTypeRepository.Table.First(alt => alt.SystemKeyword.Equals("EditCategory")),
                Comment = "Edited a category ('Computers')",
                CreatedOnUtc = DateTime.UtcNow,
                Customer = defaultCustomer,
                IpAddress = "127.0.0.1"
            });
        }

        protected virtual void InstallSearchTerms()
        {
            //default store
            var defaultStore = _storeRepository.Table.FirstOrDefault();
            if (defaultStore == null)
                throw new Exception("No default store could be loaded");

            _searchTermRepository.Insert(new SearchTerm()
            {
                Count = 34,
                Keyword = "computer",
                StoreId = defaultStore.Id
            });
            _searchTermRepository.Insert(new SearchTerm()
            {
                Count = 30,
                Keyword = "camera",
                StoreId = defaultStore.Id
            });
            _searchTermRepository.Insert(new SearchTerm()
            {
                Count = 27,
                Keyword = "jewelry",
                StoreId = defaultStore.Id
            });
            _searchTermRepository.Insert(new SearchTerm()
            {
                Count = 26,
                Keyword = "shoes",
                StoreId = defaultStore.Id
            });
            _searchTermRepository.Insert(new SearchTerm()
            {
                Count = 19,
                Keyword = "jeans",
                StoreId = defaultStore.Id
            });
            _searchTermRepository.Insert(new SearchTerm()
            {
                Count = 10,
                Keyword = "gift",
                StoreId = defaultStore.Id
            });
        }

        protected virtual void InstallEmailAccounts()
        {
            var emailAccounts = new List<EmailAccount>
                               {
                                   new EmailAccount
                                       {
                                           Email = "test@mail.com",
                                           DisplayName = "Store name",
                                           Host = "smtp.mail.com",
                                           Port = 25,
                                           Username = "123",
                                           Password = "123",
                                           EnableSsl = false,
                                           UseDefaultCredentials = false
                                       },
                               };
            _emailAccountRepository.Insert(emailAccounts);
        }

        protected virtual void InstallMessageTemplates()
        {
            var eaGeneral = _emailAccountRepository.Table.FirstOrDefault();
            if (eaGeneral == null)
                throw new Exception("Default email account cannot be loaded");

            var messageTemplates = new List<MessageTemplate>
            {
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.BlogCommentNotification,
                    Subject = "%Store.Name%. New blog comment.",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}A new blog comment has been created for blog post \"%BlogComment.BlogPostTitle%\".{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.CustomerEmailValidationMessage,
                    Subject = "%Store.Name%. Email validation",
                    Body = string.Format("<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}To activate your account <a href=\"%Customer.AccountActivationURL%\">click here</a>.{0}<br />{0}<br />{0}%Store.Name%{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.CustomerEmailRevalidationMessage,
                    Subject = "%Store.Name%. Email validation",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}Hello %Customer.FullName%!{0}<br />{0}To validate your new email address <a href=\"%Customer.EmailRevalidationURL%\">click here</a>.{0}<br />{0}<br />{0}%Store.Name%{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.PrivateMessageNotification,
                    Subject = "%Store.Name%. You have received a new private message",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}You have received a new private message.{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.CustomerPasswordRecoveryMessage,
                    Subject = "%Store.Name%. Password recovery",
                    Body = string.Format("<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}To change your password <a href=\"%Customer.PasswordRecoveryURL%\">click here</a>.{0}<br />{0}<br />{0}%Store.Name%{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.CustomerWelcomeMessage,
                    Subject = "Welcome to %Store.Name%",
                    Body = string.Format("We welcome you to <a href=\"%Store.URL%\"> %Store.Name%</a>.{0}<br />{0}<br />{0}You can now take part in the various services we have to offer you. Some of these services include:{0}<br />{0}<br />{0}Permanent Cart - Any products added to your online cart remain there until you remove them, or check them out.{0}<br />{0}Address Book - We can now deliver your products to another address other than yours! This is perfect to send birthday gifts direct to the birthday-person themselves.{0}<br />{0}Order History - View your history of purchases that you have made with us.{0}<br />{0}Products Reviews - Share your opinions on products with our other customers.{0}<br />{0}<br />{0}For help with any of our online services, please email the store-owner: <a href=\"mailto:%Store.Email%\">%Store.Email%</a>.{0}<br />{0}<br />{0}Note: This email address was provided on our registration page. If you own the email and did not register on our site, please send an email to <a href=\"mailto:%Store.Email%\">%Store.Email%</a>.{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.NewForumPostMessage,
                    Subject = "%Store.Name%. New Post Notification.",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}A new post has been created in the topic <a href=\"%Forums.TopicURL%\">\"%Forums.TopicName%\"</a> at <a href=\"%Forums.ForumURL%\">\"%Forums.ForumName%\"</a> forum.{0}<br />{0}<br />{0}Click <a href=\"%Forums.TopicURL%\">here</a> for more info.{0}<br />{0}<br />{0}Post author: %Forums.PostAuthor%{0}<br />{0}Post body: %Forums.PostBody%{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.NewForumTopicMessage,
                    Subject = "%Store.Name%. New Topic Notification.",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}A new topic <a href=\"%Forums.TopicURL%\">\"%Forums.TopicName%\"</a> has been created at <a href=\"%Forums.ForumURL%\">\"%Forums.ForumName%\"</a> forum.{0}<br />{0}<br />{0}Click <a href=\"%Forums.TopicURL%\">here</a> for more info.{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.CustomerRegisteredNotification,
                    Subject = "%Store.Name%. New customer registration",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}A new customer registered with your store. Below are the customer's details:{0}<br />{0}Full name: %Customer.FullName%{0}<br />{0}Email: %Customer.Email%{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.NewsCommentNotification,
                    Subject = "%Store.Name%. New news comment.",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}A new news comment has been created for news \"%NewsComment.NewsTitle%\".{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.NewsletterSubscriptionActivationMessage,
                    Subject = "%Store.Name%. Subscription activation message.",
                    Body = string.Format("<p>{0}<a href=\"%NewsLetterSubscription.ActivationUrl%\">Click here to confirm your subscription to our list.</a>{0}</p>{0}<p>{0}If you received this email by mistake, simply delete it.{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.NewsletterSubscriptionDeactivationMessage,
                    Subject = "%Store.Name%. Subscription deactivation message.",
                    Body = string.Format("<p>{0}<a href=\"%NewsLetterSubscription.DeactivationUrl%\">Click here to unsubscribe from our newsletter.</a>{0}</p>{0}<p>{0}If you received this email by mistake, simply delete it.{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.ProductReviewNotification,
                    Subject = "%Store.Name%. New product review.",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}A new product review has been written for product \"%ProductReview.ProductName%\".{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.EmailAFriendMessage,
                    Subject = "%Store.Name%. Referred Item",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\"> %Store.Name%</a>{0}<br />{0}<br />{0}%EmailAFriend.Email% was shopping on %Store.Name% and wanted to share the following item with you.{0}<br />{0}<br />{0}<b><a target=\"_blank\" href=\"%Product.ProductURLForCustomer%\">%Product.Name%</a></b>{0}<br />{0}%Product.ShortDescription%{0}<br />{0}<br />{0}For more info click <a target=\"_blank\" href=\"%Product.ProductURLForCustomer%\">here</a>{0}<br />{0}<br />{0}<br />{0}%EmailAFriend.PersonalMessage%{0}<br />{0}<br />{0}%Store.Name%{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.ContactUsMessage,
                    Subject = "%Store.Name%. Contact us",
                    Body = string.Format("<p>{0}%ContactUs.Body%{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                }
            };
            _messageTemplateRepository.Insert(messageTemplates);
        }

        protected virtual void InstallTopics()
        {
            var defaultTopicTemplate =
                _topicTemplateRepository.Table.FirstOrDefault(tt => tt.Name == "Default template");
            if (defaultTopicTemplate == null)
                throw new Exception("Topic template cannot be loaded");

            var topics = new List<Topic>
                               {
                                   new Topic
                                       {
                                           SystemName = "AboutUs",
                                           IncludeInSitemap = false,
                                           IsPasswordProtected = false,
                                           IncludeInFooterColumn1 = true,
                                           DisplayOrder = 20,
                                           Published = true,
                                           Title = "About us",
                                           Body = "<p>Put your &quot;About Us&quot; information here. You can edit this in the admin site.</p>",
                                           TopicTemplateId = defaultTopicTemplate.Id
                                       },
                                   new Topic
                                       {
                                           SystemName = "ConditionsOfUse",
                                           IncludeInSitemap = false,
                                           IsPasswordProtected = false,
                                           IncludeInFooterColumn1 = true,
                                           DisplayOrder = 15,
                                           Published = true,
                                           Title = "Conditions of Use",
                                           Body = "<p>Put your conditions of use information here. You can edit this in the admin site.</p>",
                                           TopicTemplateId = defaultTopicTemplate.Id
                                       },
                                   new Topic
                                       {
                                           SystemName = "ContactUs",
                                           IncludeInSitemap = false,
                                           IsPasswordProtected = false,
                                           DisplayOrder = 1,
                                           Published = true,
                                           Title = "",
                                           Body = "<p>Put your contact information here. You can edit this in the admin site.</p>",
                                           TopicTemplateId = defaultTopicTemplate.Id
                                       },
                                   new Topic
                                       {
                                           SystemName = "ForumWelcomeMessage",
                                           IncludeInSitemap = false,
                                           IsPasswordProtected = false,
                                           DisplayOrder = 1,
                                           Published = true,
                                           Title = "Forums",
                                           Body = "<p>Put your welcome message here. You can edit this in the admin site.</p>",
                                           TopicTemplateId = defaultTopicTemplate.Id
                                       },
                                   new Topic
                                       {
                                           SystemName = "HomePageText",
                                           IncludeInSitemap = false,
                                           IsPasswordProtected = false,
                                           DisplayOrder = 1,
                                           Published = true,
                                           Title = "Welcome to our store",
                                           Body = "<p>Online shopping is the process consumers go through to purchase products or services over the Internet. You can edit this in the admin site.</p><p>If you have questions, see the <a href=\"http://www.nopcommerce.com/documentation.aspx\">Documentation</a>, or post in the <a href=\"http://www.nopcommerce.com/boards/\">Forums</a> at <a href=\"http://www.nopcommerce.com\">nopCommerce.com</a></p>",
                                           TopicTemplateId = defaultTopicTemplate.Id
                                       },
                                   new Topic
                                       {
                                           SystemName = "LoginRegistrationInfo",
                                           IncludeInSitemap = false,
                                           IsPasswordProtected = false,
                                           DisplayOrder = 1,
                                           Published = true,
                                           Title = "About login / registration",
                                           Body = "<p>Put your login / registration information here. You can edit this in the admin site.</p>",
                                           TopicTemplateId = defaultTopicTemplate.Id
                                       },
                                   new Topic
                                       {
                                           SystemName = "PrivacyInfo",
                                           IncludeInSitemap = false,
                                           IsPasswordProtected = false,
                                           IncludeInFooterColumn1 = true,
                                           DisplayOrder = 10,
                                           Published = true,
                                           Title = "Privacy notice",
                                           Body = "<p>Put your privacy policy information here. You can edit this in the admin site.</p>",
                                           TopicTemplateId = defaultTopicTemplate.Id
                                       },
                                   new Topic
                                       {
                                           SystemName = "PageNotFound",
                                           IncludeInSitemap = false,
                                           IsPasswordProtected = false,
                                           DisplayOrder = 1,
                                           Published = true,
                                           Title = "",
                                           Body = "<p><strong>The page you requested was not found, and we have a fine guess why.</strong></p><ul><li>If you typed the URL directly, please make sure the spelling is correct.</li><li>The page no longer exists. In this case, we profusely apologize for the inconvenience and for any damage this may cause.</li></ul>",
                                           TopicTemplateId = defaultTopicTemplate.Id
                                       },
                                  
        
                               };
            _topicRepository.Insert(topics);


            //search engine names
            foreach (var topic in topics)
            {
                _urlRecordRepository.Insert(new UrlRecord
                {
                    EntityId = topic.Id,
                    EntityName = "Topic",
                    LanguageId = 0,
                    IsActive = true,
                    Slug = topic.ValidateSeName("", !String.IsNullOrEmpty(topic.Title) ? topic.Title : topic.SystemName, true)
                });
            }

        }

        protected virtual void InstallSettings()
        {
            var settingService = EngineContext.Current.Resolve<ISettingService>();
            settingService.SaveSetting(new PdfSettings
            {
                LogoPictureId = 0,
                LetterPageSizeEnabled = false,
                RenderOrderNotes = true,
                FontFileName = "FreeSerif.ttf",
                InvoiceFooterTextColumn1 = null,
                InvoiceFooterTextColumn2 = null,
            });

            settingService.SaveSetting(new CommonSettings
            {
                UseSystemEmailForContactUsForm = true,
                UseStoredProceduresIfSupported = true,
                SitemapEnabled = true,
                SitemapIncludeCategories = true,
                SitemapIncludeProducts = false,
                DisplayJavaScriptDisabledWarning = false,
                UseFullTextSearch = false,
                FullTextMode = FulltextSearchMode.ExactMatch,
                Log404Errors = true,
                BreadcrumbDelimiter = "/",
                RenderXuaCompatible = false,
                XuaCompatibleValue = "IE=edge",
                BbcodeEditorOpenLinksInNewWindow = false
            });

            settingService.SaveSetting(new SeoSettings
            {
                PageTitleSeparator = ". ",
                PageTitleSeoAdjustment = PageTitleSeoAdjustment.PagenameAfterStorename,
                DefaultTitle = "Your store",
                DefaultMetaKeywords = "",
                DefaultMetaDescription = "",
                GenerateProductMetaDescription = true,
                ConvertNonWesternChars = false,
                AllowUnicodeCharsInUrls = true,
                CanonicalUrlsEnabled = false,
                WwwRequirement = WwwRequirement.NoMatter,
                //we disable bundling out of the box because it requires a lot of server resources
                EnableJsBundling = false,
                EnableCssBundling = false,
                TwitterMetaTags = true,
                OpenGraphMetaTags = true,
                ReservedUrlRecordSlugs = new List<string>
                {
                    "admin",
                    "install",
                    "recentlyviewedproducts",
                    "newproducts",
                    "setproductreviewhelpfulness",
                    "login",
                    "register",
                    "logout",
                    "contactus",
                    "passwordrecovery",
                    "subscribenewsletter",
                    "blog",
                    "boards",
                    "inboxupdate",
                    "sentupdate",
                    "news",
                    "sitemap",
                    "search",
                    "config",
                    "eucookielawaccept",
                    "page-not-found",
                    //system names are not allowed (anyway they will cause a runtime error),
                    "con",
                    "lpt1",
                    "lpt2",
                    "lpt3",
                    "lpt4",
                    "lpt5",
                    "lpt6",
                    "lpt7",
                    "lpt8",
                    "lpt9",
                    "com1",
                    "com2",
                    "com3",
                    "com4",
                    "com5",
                    "com6",
                    "com7",
                    "com8",
                    "com9",
                    "null",
                    "prn",
                    "aux"
                },
                CustomHeadTags = ""
            });

            settingService.SaveSetting(new AdminAreaSettings
            {
                DefaultGridPageSize = 15,
                PopupGridPageSize = 10,
                GridPageSizes = "10, 15, 20, 50, 100",
                RichEditorAdditionalSettings = null,
                RichEditorAllowJavaScript = false
            });


            settingService.SaveSetting(new ProductEditorSettings
            {
            });

            settingService.SaveSetting(new CatalogSettings
            {
                AllowViewUnpublishedProductPage = true,
                DisplayDiscontinuedMessageForUnpublishedProducts = true,
                ShowSkuOnProductDetailsPage = true,
                ShowSkuOnCatalogPages = false,
                AllowProductSorting = true,
                AllowProductViewModeChanging = true,
                DefaultViewMode = "grid",
                ShowProductsFromSubcategories = false,
                ShowCategoryProductNumber = false,
                ShowCategoryProductNumberIncludingSubcategories = false,
                CategoryBreadcrumbEnabled = true,
                ShowShareButton = true,
                PageShareCode = "<!-- AddThis Button BEGIN --><div class=\"addthis_toolbox addthis_default_style \"><a class=\"addthis_button_preferred_1\"></a><a class=\"addthis_button_preferred_2\"></a><a class=\"addthis_button_preferred_3\"></a><a class=\"addthis_button_preferred_4\"></a><a class=\"addthis_button_compact\"></a><a class=\"addthis_counter addthis_bubble_style\"></a></div><script type=\"text/javascript\" src=\"http://s7.addthis.com/js/250/addthis_widget.js#pubid=nopsolutions\"></script><!-- AddThis Button END -->",
                ProductReviewsMustBeApproved = false,
                DefaultProductRatingValue = 5,
                AllowAnonymousUsersToReviewProduct = false,
                NotifyStoreOwnerAboutNewProductReviews = false,
                EmailAFriendEnabled = true,
                AllowAnonymousUsersToEmailAFriend = false,
                RecentlyViewedProductsNumber = 3,
                RecentlyViewedProductsEnabled = true,
                NewProductsNumber = 6,
                NewProductsEnabled = true,
                ProductSearchAutoCompleteEnabled = true,
                ProductSearchAutoCompleteNumberOfProducts = 10,
                ProductSearchTermMinimumLength = 3,
                ShowProductImagesInSearchAutoComplete = false,
                SearchPageProductsPerPage = 6,
                SearchPageAllowCustomersToSelectPageSize = true,
                SearchPagePageSizeOptions = "6, 3, 9, 18",
                NumberOfProductTags = 15,
                ProductsByTagPageSize = 6,
                IncludeFeaturedProductsInNormalLists = false,
                IgnoreFeaturedProducts = false,
                IgnoreAcl = true,
                IgnoreStoreLimitations = true,
                ProductsByTagAllowCustomersToSelectPageSize = true,
                ProductsByTagPageSizeOptions = "6, 3, 9, 18",
                DefaultCategoryPageSizeOptions = "6, 3, 9",
                DefaultCategoryPageSize = 6,
                ShowProductReviewsTabOnAccountPage = true,
                ProductReviewsPageSizeOnAccountPage = 10,
                ExportImportUseDropdownlistsForAssociatedEntities = true
            });

            settingService.SaveSetting(new LocalizationSettings
            {
                DefaultAdminLanguageId = _languageRepository.Table.Single(l => l.Name == "English").Id,
                UseImagesForLanguageSelection = false,
                SeoFriendlyUrlsForLanguagesEnabled = false,
                AutomaticallyDetectLanguage = false,
                LoadAllLocaleRecordsOnStartup = true,
                LoadAllLocalizedPropertiesOnStartup = true,
                LoadAllUrlRecordsOnStartup = false,
                IgnoreRtlPropertyForAdminArea = false
            });

            settingService.SaveSetting(new CustomerSettings
            {
                UsernamesEnabled = false,
                CheckUsernameAvailabilityEnabled = false,
                AllowUsersToChangeUsernames = false,
                DefaultPasswordFormat = PasswordFormat.Hashed,
                HashedPasswordFormat = "SHA1",
                PasswordMinLength = 6,
                UnduplicatedPasswordsNumber = 4,
                PasswordRecoveryLinkDaysValid = 7,
                PasswordLifetime = 90,
                FailedPasswordAllowedAttempts = 0,
                FailedPasswordLockoutMinutes = 30,
                UserRegistrationType = UserRegistrationType.Standard,
                AllowCustomersToUploadAvatars = false,
                AvatarMaximumSizeBytes = 20000,
                DefaultAvatarEnabled = true,
                ShowCustomersLocation = false,
                ShowCustomersJoinDate = false,
                AllowViewingProfiles = false,
                NotifyNewCustomerRegistration = false,
                CustomerNameFormat = CustomerNameFormat.ShowFirstName,
                GenderEnabled = true,
                DateOfBirthEnabled = true,
                DateOfBirthRequired = false,
                DateOfBirthMinimumAge = null,
                CompanyEnabled = true,
                StreetAddressEnabled = false,
                StreetAddress2Enabled = false,
                ZipPostalCodeEnabled = false,
                CityEnabled = false,
                CountryEnabled = false,
                CountryRequired = false,
                StateProvinceEnabled = false,
                StateProvinceRequired = false,
                PhoneEnabled = false,
                FaxEnabled = false,
                AcceptPrivacyPolicyEnabled = false,
                NewsletterEnabled = true,
                NewsletterTickedByDefault = true,
                HideNewsletterBlock = false,
                NewsletterBlockAllowToUnsubscribe = false,
                OnlineCustomerMinutes = 20,
                StoreLastVisitedPage = false,
                SuffixDeletedCustomers = false,
                EnteringEmailTwice = false,
                DeleteGuestTaskOlderThanMinutes = 1440
            });

            settingService.SaveSetting(new AddressSettings
            {
                CompanyEnabled = true,
                StreetAddressEnabled = true,
                StreetAddressRequired = true,
                StreetAddress2Enabled = true,
                ZipPostalCodeEnabled = true,
                ZipPostalCodeRequired = true,
                CityEnabled = true,
                CityRequired = true,
                CountryEnabled = true,
                StateProvinceEnabled = true,
                PhoneEnabled = true,
                PhoneRequired = true,
                FaxEnabled = true,
            });

            settingService.SaveSetting(new MediaSettings
            {
                AvatarPictureSize = 120,
                ProductThumbPictureSize = 415,
                ProductDetailsPictureSize = 550,
                ProductThumbPictureSizeOnProductDetailsPage = 100,
                AssociatedProductPictureSize = 220,
                CategoryThumbPictureSize = 450,
                AutoCompleteSearchThumbPictureSize = 20,
                ImageSquarePictureSize = 32,
                MaximumImageSize = 1980,
                DefaultPictureZoomEnabled = false,
                DefaultImageQuality = 80,
                MultipleThumbDirectories = false,
                ImportProductImagesUsingHash = true
            });

            settingService.SaveSetting(new StoreInformationSettings
            {
                StoreClosed = false,
                DefaultStoreTheme = "DefaultClean",
                AllowCustomerToSelectTheme = false,
                DisplayMiniProfilerInPublicStore = false,
                DisplayEuCookieLawWarning = false,
                FacebookLink = "http://www.facebook.com/nopCommerce",
                TwitterLink = "https://twitter.com/nopCommerce",
                YoutubeLink = "http://www.youtube.com/user/nopCommerce",
                GooglePlusLink = "https://plus.google.com/+nopcommerce",
                HidePoweredByNopCommerce = false
            });

            settingService.SaveSetting(new ExternalAuthenticationSettings
            {
                AutoRegisterEnabled = true,
                RequireEmailValidation = false
            });

            settingService.SaveSetting(new MessageTemplatesSettings
            {
                CaseInvariantReplacement = false,
                Color1 = "#b9babe",
                Color2 = "#ebecee",
                Color3 = "#dde2e6",
            });

            settingService.SaveSetting(new SecuritySettings
            {
                ForceSslForAllPages = false,
                EncryptionKey = CommonHelper.GenerateRandomDigitCode(16),
                AdminAreaAllowedIpAddresses = null,
                EnableXsrfProtectionForAdminArea = true,
                EnableXsrfProtectionForPublicStore = true,
                HoneypotEnabled = false,
                HoneypotInputName = "hpinput"
            });

            settingService.SaveSetting(new DateTimeSettings
            {
                DefaultStoreTimeZoneId = "",
                AllowCustomersToSetTimeZone = false
            });

            settingService.SaveSetting(new BlogSettings
            {
                Enabled = true,
                PostsPageSize = 10,
                AllowNotRegisteredUsersToLeaveComments = true,
                NotifyAboutNewBlogComments = false,
                NumberOfTags = 15,
                ShowHeaderRssUrl = false,
                BlogCommentsMustBeApproved = false,
                ShowBlogCommentsPerStore = false
            });
            settingService.SaveSetting(new NewsSettings
            {
                Enabled = true,
                AllowNotRegisteredUsersToLeaveComments = true,
                NotifyAboutNewNewsComments = false,
                ShowNewsOnMainPage = true,
                MainPageNewsCount = 3,
                NewsArchivePageSize = 10,
                ShowHeaderRssUrl = false,
                NewsCommentsMustBeApproved = false,
                ShowNewsCommentsPerStore = false
            });

            settingService.SaveSetting(new ForumSettings
            {
                ForumsEnabled = false,
                RelativeDateTimeFormattingEnabled = true,
                AllowCustomersToDeletePosts = false,
                AllowCustomersToEditPosts = false,
                AllowCustomersToManageSubscriptions = false,
                AllowGuestsToCreatePosts = false,
                AllowGuestsToCreateTopics = false,
                AllowPostVoting = true,
                MaxVotesPerDay = 30,
                TopicSubjectMaxLength = 450,
                PostMaxLength = 4000,
                StrippedTopicMaxLength = 45,
                TopicsPageSize = 10,
                PostsPageSize = 10,
                SearchResultsPageSize = 10,
                ActiveDiscussionsPageSize = 50,
                LatestCustomerPostsPageSize = 10,
                ShowCustomersPostCount = true,
                ForumEditor = EditorType.BBCodeEditor,
                SignaturesEnabled = true,
                AllowPrivateMessages = false,
                ShowAlertForPM = false,
                PrivateMessagesPageSize = 10,
                ForumSubscriptionsPageSize = 10,
                NotifyAboutPrivateMessages = false,
                PMSubjectMaxLength = 450,
                PMTextMaxLength = 4000,
                HomePageActiveDiscussionsTopicCount = 5,
                ActiveDiscussionsFeedEnabled = false,
                ActiveDiscussionsFeedCount = 25,
                ForumFeedsEnabled = false,
                ForumFeedCount = 10,
                ForumSearchTermMinimumLength = 3,
            });

            var eaGeneral = _emailAccountRepository.Table.FirstOrDefault();
            if (eaGeneral == null)
                throw new Exception("Default email account cannot be loaded");
            settingService.SaveSetting(new EmailAccountSettings
            {
                DefaultEmailAccountId = eaGeneral.Id
            });

            settingService.SaveSetting(new WidgetSettings
            {
                ActiveWidgetSystemNames = new List<string> { "Widgets.NivoSlider" },
            });
        }

        protected virtual void InstallCategories()
        {
            //pictures
            var pictureService = EngineContext.Current.Resolve<IPictureService>();
            var sampleImagesPath = CommonHelper.MapPath("~/content/samples/");



            var categoryTemplateInGridAndLines = _categoryTemplateRepository
                .Table.FirstOrDefault(pt => pt.Name == "Products in Grid or Lines");
            if (categoryTemplateInGridAndLines == null)
                throw new Exception("Category template cannot be loaded");


            //categories
            var allCategories = new List<Category>();
            var categoryComputers = new Category
            {
                Name = "Computers",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                PictureId = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "category_computers.jpeg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName("Computers")).Id,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryComputers);
            _categoryRepository.Insert(categoryComputers);


            var categoryDesktops = new Category
            {
                Name = "Desktops",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                ParentCategoryId = categoryComputers.Id,
                PictureId = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "category_desktops.jpg"), MimeTypes.ImagePJpeg, pictureService.GetPictureSeName("Desktops")).Id,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryDesktops);
            _categoryRepository.Insert(categoryDesktops);


            var categoryNotebooks = new Category
            {
                Name = "Notebooks",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                ParentCategoryId = categoryComputers.Id,
                PictureId = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "category_notebooks.jpg"), MimeTypes.ImagePJpeg, pictureService.GetPictureSeName("Notebooks")).Id,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 2,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryNotebooks);
            _categoryRepository.Insert(categoryNotebooks);


            var categorySoftware = new Category
            {
                Name = "Software",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                ParentCategoryId = categoryComputers.Id,
                PictureId = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "category_software.jpg"), MimeTypes.ImagePJpeg, pictureService.GetPictureSeName("Software")).Id,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 3,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categorySoftware);
            _categoryRepository.Insert(categorySoftware);


            var categoryElectronics = new Category
            {
                Name = "Electronics",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                PictureId = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "category_electronics.jpeg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName("Electronics")).Id,
                IncludeInTopMenu = true,
                Published = true,
                ShowOnHomePage = true,
                DisplayOrder = 2,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryElectronics);
            _categoryRepository.Insert(categoryElectronics);


            var categoryCameraPhoto = new Category
            {
                Name = "Camera & photo",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                ParentCategoryId = categoryElectronics.Id,
                PictureId = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "category_camera_photo.jpeg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName("Camera, photo")).Id,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryCameraPhoto);
            _categoryRepository.Insert(categoryCameraPhoto);


            var categoryCellPhones = new Category
            {
                Name = "Cell phones",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                ParentCategoryId = categoryElectronics.Id,
                PictureId = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "category_cell_phones.jpeg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName("Cell phones")).Id,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 2,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryCellPhones);
            _categoryRepository.Insert(categoryCellPhones);


            var categoryOthers = new Category
            {
                Name = "Others",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                ParentCategoryId = categoryElectronics.Id,
                PictureId = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "category_accessories.jpg"), MimeTypes.ImagePJpeg, pictureService.GetPictureSeName("Accessories")).Id,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 3,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryOthers);
            _categoryRepository.Insert(categoryOthers);


            var categoryApparel = new Category
            {
                Name = "Apparel",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                PictureId = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "category_apparel.jpeg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName("Apparel")).Id,
                IncludeInTopMenu = true,
                Published = true,
                ShowOnHomePage = true,
                DisplayOrder = 3,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryApparel);
            _categoryRepository.Insert(categoryApparel);


            var categoryShoes = new Category
            {
                Name = "Shoes",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                ParentCategoryId = categoryApparel.Id,
                PictureId = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "category_shoes.jpeg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName("Shoes")).Id,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryShoes);
            _categoryRepository.Insert(categoryShoes);


            var categoryClothing = new Category
            {
                Name = "Clothing",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                ParentCategoryId = categoryApparel.Id,
                PictureId = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "category_clothing.jpeg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName("Clothing")).Id,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 2,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryClothing);
            _categoryRepository.Insert(categoryClothing);


            var categoryAccessories = new Category
            {
                Name = "Accessories",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                ParentCategoryId = categoryApparel.Id,
                PictureId = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "category_apparel_accessories.jpg"), MimeTypes.ImagePJpeg, pictureService.GetPictureSeName("Apparel Accessories")).Id,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 3,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryAccessories);
            _categoryRepository.Insert(categoryAccessories);


            var categoryDigitalDownloads = new Category
            {
                Name = "Digital downloads",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                PictureId = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "category_digital_downloads.jpeg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName("Digital downloads")).Id,
                IncludeInTopMenu = true,
                Published = true,
                ShowOnHomePage = true,
                DisplayOrder = 4,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryDigitalDownloads);
            _categoryRepository.Insert(categoryDigitalDownloads);


            var categoryBooks = new Category
            {
                Name = "Books",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                MetaKeywords = "Books, Dictionary, Textbooks",
                MetaDescription = "Books category description",
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                PictureId = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "category_book.jpeg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName("Book")).Id,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 5,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryBooks);
            _categoryRepository.Insert(categoryBooks);


            var categoryJewelry = new Category
            {
                Name = "Jewelry",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                PictureId = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "category_jewelry.jpeg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName("Jewelry")).Id,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 6,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryJewelry);
            _categoryRepository.Insert(categoryJewelry);

            //search engine names
            foreach (var category in allCategories)
            {
                _urlRecordRepository.Insert(new UrlRecord
                {
                    EntityId = category.Id,
                    EntityName = "Category",
                    LanguageId = 0,
                    IsActive = true,
                    Slug = category.ValidateSeName("", category.Name, true)
                });
            }
        }

        protected virtual void InstallProducts(string defaultUserEmail)
        {
            var productTemplateSimple = _productTemplateRepository.Table.FirstOrDefault(pt => pt.Name == "Simple product");
            if (productTemplateSimple == null)
                throw new Exception("Simple product template could not be loaded");
            var productTemplateGrouped = _productTemplateRepository.Table.FirstOrDefault(pt => pt.Name == "Grouped product (with variants)");
            if (productTemplateGrouped == null)
                throw new Exception("Grouped product template could not be loaded");

            //default customer/user
            var defaultCustomer = _customerRepository.Table.FirstOrDefault(x => x.Email == defaultUserEmail);
            if (defaultCustomer == null)
                throw new Exception("Cannot load default customer");

            //default store
            var defaultStore = _storeRepository.Table.FirstOrDefault();
            if (defaultStore == null)
                throw new Exception("No default store could be loaded");


            //pictures
            var pictureService = EngineContext.Current.Resolve<IPictureService>();
            var sampleImagesPath = CommonHelper.MapPath("~/content/samples/");

            //downloads
            var downloadService = EngineContext.Current.Resolve<IDownloadService>();
            var sampleDownloadsPath = CommonHelper.MapPath("~/content/samples/");

            //products
            var allProducts = new List<Product>();

            #region Desktops


            var productBuildComputer = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "Build your own computer",
                Sku = "COMP_CUST",
                ShortDescription = "Build it",
                FullDescription = "<p>Fight back against cluttered workspaces with the stylish IBM zBC12 All-in-One desktop PC, featuring powerful computing resources and a stunning 20.1-inch widescreen display with stunning XBRITE-HiColor LCD technology. The black IBM zBC12 has a built-in microphone and MOTION EYE camera with face-tracking technology that allows for easy communication with friends and family. And it has a built-in DVD burner and Sony's Movie Store software so you can create a digital entertainment library for personal viewing at your convenience. Easy to setup and even easier to use, this JS-series All-in-One includes an elegantly designed keyboard and a USB mouse.</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "build-your-own-computer",
                AllowCustomerReviews = true,

                Published = true,
                ShowOnHomePage = true,
                MarkAsNew = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Desktops"),
                        DisplayOrder = 1,
                    }
                }
            };
            allProducts.Add(productBuildComputer);
            productBuildComputer.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_Desktops_1.jpeg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productBuildComputer.Name)),
                DisplayOrder = 1,
            });
            productBuildComputer.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_Desktops_2.jpeg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productBuildComputer.Name)),
                DisplayOrder = 2,
            });
            _productRepository.Insert(productBuildComputer);





            var productDigitalStorm = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "Digital Storm VANQUISH 3 Custom Performance PC",
                Sku = "DS_VA3_PC",
                ShortDescription = "Digital Storm Vanquish 3 Desktop PC",
                FullDescription = "<p>Blow the doors off today’s most demanding games with maximum detail, speed, and power for an immersive gaming experience without breaking the bank.</p><p>Stay ahead of the competition, VANQUISH 3 is fully equipped to easily handle future upgrades, keeping your system on the cutting edge for years to come.</p><p>Each system is put through an extensive stress test, ensuring you experience zero bottlenecks and get the maximum performance from your hardware.</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "compaq-presario-sr1519x-pentium-4-desktop-pc-with-cdrw",
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Desktops"),
                        DisplayOrder = 1,
                    }
                }
            };
            allProducts.Add(productDigitalStorm);
            productDigitalStorm.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_DigitalStorm.jpeg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productDigitalStorm.Name)),
                DisplayOrder = 1,
            });
            _productRepository.Insert(productDigitalStorm);





            var productLenovoIdeaCentre = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "Lenovo IdeaCentre 600 All-in-One PC",
                Sku = "LE_IC_600",
                ShortDescription = "",
                FullDescription = "<p>The A600 features a 21.5in screen, DVD or optional Blu-Ray drive, support for the full beans 1920 x 1080 HD, Dolby Home Cinema certification and an optional hybrid analogue/digital TV tuner.</p><p>Connectivity is handled by 802.11a/b/g - 802.11n is optional - and an ethernet port. You also get four USB ports, a Firewire slot, a six-in-one card reader and a 1.3- or two-megapixel webcam.</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "hp-iq506-touchsmart-desktop-pc",
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Desktops"),
                        DisplayOrder = 1,
                    }
                }
            };
            allProducts.Add(productLenovoIdeaCentre);
            productLenovoIdeaCentre.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_LenovoIdeaCentre.jpeg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productLenovoIdeaCentre.Name)),
                DisplayOrder = 1,
            });
            _productRepository.Insert(productLenovoIdeaCentre);




            #endregion

            #region Notebooks

            var productAppleMacBookPro = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "Apple MacBook Pro 13-inch",
                Sku = "AP_MBP_13",
                ShortDescription = "A groundbreaking Retina display. A new force-sensing trackpad. All-flash architecture. Powerful dual-core and quad-core Intel processors. Together, these features take the notebook to a new level of performance. And they will do the same for you in everything you create.",
                FullDescription = "<p>With fifth-generation Intel Core processors, the latest graphics, and faster flash storage, the incredibly advanced MacBook Pro with Retina display moves even further ahead in performance and battery life.* *Compared with the previous generation.</p><p>Retina display with 2560-by-1600 resolution</p><p>Fifth-generation dual-core Intel Core i5 processor</p><p>Intel Iris Graphics</p><p>Up to 9 hours of battery life1</p><p>Faster flash storage2</p><p>802.11ac Wi-Fi</p><p>Two Thunderbolt 2 ports for connecting high-performance devices and transferring data at lightning speed</p><p>Two USB 3 ports (compatible with USB 2 devices) and HDMI</p><p>FaceTime HD camera</p><p>Pages, Numbers, Keynote, iPhoto, iMovie, GarageBand included</p><p>OS X, the world's most advanced desktop operating system</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "asus-eee-pc-1000ha-10-inch-netbook",
                AllowCustomerReviews = true,
                Published = true,
                ShowOnHomePage = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Notebooks"),
                        DisplayOrder = 1,
                    }
                },
            };
            allProducts.Add(productAppleMacBookPro);
            productAppleMacBookPro.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_macbook_1.jpeg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productAppleMacBookPro.Name)),
                DisplayOrder = 1,
            });
            productAppleMacBookPro.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_macbook_2.jpeg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productAppleMacBookPro.Name)),
                DisplayOrder = 2,
            });
            _productRepository.Insert(productAppleMacBookPro);





            var productAsusN551JK = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "Asus N551JK-XO076H Laptop",
                Sku = "AS_551_LP",
                ShortDescription = "Laptop Asus N551JK Intel Core i7-4710HQ 2.5 GHz, RAM 16GB, HDD 1TB, Video NVidia GTX 850M 4GB, BluRay, 15.6, Full HD, Win 8.1",
                FullDescription = "<p>The ASUS N550JX combines cutting-edge audio and visual technology to deliver an unsurpassed multimedia experience. A full HD wide-view IPS panel is tailor-made for watching movies and the intuitive touchscreen makes for easy, seamless navigation. ASUS has paired the N550JX’s impressive display with SonicMaster Premium, co-developed with Bang & Olufsen ICEpower® audio experts, for true surround sound. A quad-speaker array and external subwoofer combine for distinct vocals and a low bass that you can feel.</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "asus-eee-pc-900ha-89-inch-netbook-black",
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Notebooks"),
                        DisplayOrder = 1,
                    }
                }
            };
            allProducts.Add(productAsusN551JK);
            productAsusN551JK.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_asuspc_N551JK.jpeg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productAsusN551JK.Name)),
                DisplayOrder = 1,
            });
            _productRepository.Insert(productAsusN551JK);





            var productSamsungSeries = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "Samsung Series 9 NP900X4C Premium Ultrabook",
                Sku = "SM_900_PU",
                ShortDescription = "Samsung Series 9 NP900X4C-A06US 15-Inch Ultrabook (1.70 GHz Intel Core i5-3317U Processor, 8GB DDR3, 128GB SSD, Windows 8) Ash Black",
                FullDescription = "<p>Designed with mobility in mind, Samsung's durable, ultra premium, lightweight Series 9 laptop (model NP900X4C-A01US) offers mobile professionals and power users a sophisticated laptop equally suited for work and entertainment. Featuring a minimalist look that is both simple and sophisticated, its polished aluminum uni-body design offers an iconic look and feel that pushes the envelope with an edge just 0.58 inches thin. This Series 9 laptop also includes a brilliant 15-inch SuperBright Plus display with HD+ technology, 128 GB Solid State Drive (SSD), 8 GB of system memory, and up to 10 hours of battery life.</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "hp-pavilion-artist-edition-dv2890nr-141-inch-laptop",
                AllowCustomerReviews = true,
                Published = true,
                //ShowOnHomePage = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Notebooks"),
                        DisplayOrder = 1,
                    }
                }
            };
            allProducts.Add(productSamsungSeries);
            productSamsungSeries.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_SamsungNP900X4C.jpeg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productSamsungSeries.Name)),
                DisplayOrder = 1,
            });
            _productRepository.Insert(productSamsungSeries);





            var productHpSpectre = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "HP Spectre XT Pro UltraBook",
                Sku = "HP_SPX_UB",
                ShortDescription = "HP Spectre XT Pro UltraBook / Intel Core i5-2467M / 13.3 / 4GB / 128GB / Windows 7 Professional / Laptop",
                FullDescription = "<p>Introducing HP ENVY Spectre XT, the Ultrabook designed for those who want style without sacrificing substance. It's sleek. It's thin. And with Intel. Corer i5 processor and premium materials, it's designed to go anywhere from the bistro to the boardroom, it's unlike anything you've ever seen from HP.</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "hp-pavilion-elite-m9150f-desktop-pc",
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Notebooks"),
                        DisplayOrder = 1,
                    }
                },
            };
            allProducts.Add(productHpSpectre);
            productHpSpectre.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_HPSpectreXT_1.jpeg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productHpSpectre.Name)),
                DisplayOrder = 1,
            });
            productHpSpectre.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_HPSpectreXT_2.jpeg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productHpSpectre.Name)),
                DisplayOrder = 2,
            });
            _productRepository.Insert(productHpSpectre);



            var productHpEnvy = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "HP Envy 6-1180ca 15.6-Inch Sleekbook",
                Sku = "HP_ESB_15",
                ShortDescription = "HP ENVY 6-1202ea Ultrabook Beats Audio, 3rd generation Intel® CoreTM i7-3517U processor, 8GB RAM, 500GB HDD, Microsoft Windows 8, AMD Radeon HD 8750M (2 GB DDR3 dedicated)",
                FullDescription = "The UltrabookTM that's up for anything. Thin and light, the HP ENVY is the large screen UltrabookTM with Beats AudioTM. With a soft-touch base that makes it easy to grab and go, it's a laptop that's up for anything.<br><br><b>Features</b><br><br>- Windows 8 or other operating systems available<br><br><b>Top performance. Stylish design. Take notice.</b><br><br>- At just 19.8 mm (0.78 in) thin, the HP ENVY UltrabookTM is slim and light enough to take anywhere. It's the laptop that gets you noticed with the power to get it done.<br>- With an eye-catching metal design, it's a laptop that you want to carry with you. The soft-touch, slip-resistant base gives you the confidence to carry it with ease.<br><br><b>More entertaining. More gaming. More fun.</b><br><br>- Own the UltrabookTM with Beats AudioTM, dual speakers, a subwoofer, and an awesome display. Your music, movies and photo slideshows will always look and sound their best.<br>- Tons of video memory let you experience incredible gaming and multimedia without slowing down. Create and edit videos in a flash. And enjoy more of what you love to the fullest.<br>- The HP ENVY UltrabookTM is loaded with the ports you'd expect on a world-class laptop, but on a Sleekbook instead. Like HDMI, USB, RJ-45, and a headphone jack. You get all the right connections without compromising size.<br><br><b>Only from HP.</b><br><br>- Life heats up. That's why there's HP CoolSense technology, which automatically adjusts your notebook's temperature based on usage and conditions. It stays cool. You stay comfortable.<br>- With HP ProtectSmart, your notebook's data stays safe from accidental bumps and bruises. It senses motion and plans ahead, stopping your hard drive and protecting your entire digital life.<br>- Keep playing even in dimly lit rooms or on red eye flights. The optional backlit keyboard[1] is full-size so you don't compromise comfort. Backlit keyboard. Another bright idea.<br><br><b>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "hp-pavilion-g60-230us-160-inch-laptop",
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Notebooks"),
                        DisplayOrder = 1,
                    }
                },
            };
            allProducts.Add(productHpEnvy);
            productHpEnvy.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_HpEnvy6.jpeg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productHpEnvy.Name)),
                DisplayOrder = 1,
            });
            _productRepository.Insert(productHpEnvy);





            var productLenovoThinkpad = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "Lenovo Thinkpad X1 Carbon Laptop",
                Sku = "LE_TX1_CL",
                ShortDescription = "Lenovo Thinkpad X1 Carbon Touch Intel Core i7 14 Ultrabook",
                FullDescription = "<p>The X1 Carbon brings a new level of quality to the ThinkPad legacy of high standards and innovation. It starts with the durable, carbon fiber-reinforced roll cage, making for the best Ultrabook construction available, and adds a host of other new features on top of the old favorites. Because for 20 years, we haven't stopped innovating. And you shouldn't stop benefiting from that.</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "toshiba-satellite-a305-s6908-154-inch-laptop",
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Notebooks"),
                        DisplayOrder = 1,
                    }
                }
            };
            allProducts.Add(productLenovoThinkpad);
            productLenovoThinkpad.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_LenovoThinkpad.jpeg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productLenovoThinkpad.Name)),
                DisplayOrder = 1,
            });
            _productRepository.Insert(productLenovoThinkpad);

            #endregion

            #region Software


            var productAdobePhotoshop = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "Adobe Photoshop CS4",
                Sku = "AD_CS4_PH",
                ShortDescription = "Easily find and view all your photos",
                FullDescription = "<p>Adobe Photoshop CS4 software combines power and simplicity so you can make ordinary photos extraordinary; tell engaging stories in beautiful, personalized creations for print and web; and easily find and view all your photos. New Photoshop.com membership* works with Photoshop CS4 so you can protect your photos with automatic online backup and 2 GB of storage; view your photos anywhere you are; and share your photos in fun, interactive ways with invitation-only Online Albums.</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "adobe-photoshop-elements-7",
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Software"),
                        DisplayOrder = 1,
                    }
                }
            };
            allProducts.Add(productAdobePhotoshop);
            productAdobePhotoshop.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_AdobePhotoshop.jpeg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productAdobePhotoshop.Name)),
                DisplayOrder = 1,
            });
            _productRepository.Insert(productAdobePhotoshop);






            var productWindows8Pro = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "Windows 8 Pro",
                Sku = "MS_WIN_8P",
                ShortDescription = "Windows 8 is a Microsoft operating system that was released in 2012 as part of the company's Windows NT OS family. ",
                FullDescription = "<p>Windows 8 Pro is comparable to Windows 7 Professional and Ultimate and is targeted towards enthusiasts and business users; it includes all the features of Windows 8. Additional features include the ability to receive Remote Desktop connections, the ability to participate in a Windows Server domain, Encrypting File System, Hyper-V, and Virtual Hard Disk Booting, Group Policy as well as BitLocker and BitLocker To Go. Windows Media Center functionality is available only for Windows 8 Pro as a separate software package.</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "corel-paint-shop-pro-photo-x2",
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Software"),
                        DisplayOrder = 1,
                    }
                }
            };
            allProducts.Add(productWindows8Pro);
            productWindows8Pro.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_Windows8.jpeg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productWindows8Pro.Name)),
                DisplayOrder = 1,
            });
            _productRepository.Insert(productWindows8Pro);





            var productSoundForge = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "Sound Forge Pro 11 (recurring)",
                Sku = "SF_PRO_11",
                ShortDescription = "Advanced audio waveform editor.",
                FullDescription = "<p>Sound Forge™ Pro is the application of choice for a generation of creative and prolific artists, producers, and editors. Record audio quickly on a rock-solid platform, address sophisticated audio processing tasks with surgical precision, and render top-notch master files with ease. New features include one-touch recording, metering for the new critical standards, more repair and restoration tools, and exclusive round-trip interoperability with SpectraLayers Pro. Taken together, these enhancements make this edition of Sound Forge Pro the deepest and most advanced audio editing platform available.</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "major-league-baseball-2k9",
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Software"),
                        DisplayOrder = 1,
                    }
                }
            };
            allProducts.Add(productSoundForge);
            productSoundForge.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_SoundForge.jpeg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productSoundForge.Name)),
                DisplayOrder = 1,
            });
            _productRepository.Insert(productSoundForge);




            #endregion

            #region Camera, Photo


            //this one is a grouped product with two associated ones
            var productNikonD5500DSLR = new Product
            {
                ProductType = ProductType.GroupedProduct,
                VisibleIndividually = true,
                Name = "Nikon D5500 DSLR",
                Sku = "N5500DS_0",
                ShortDescription = "Slim, lightweight Nikon D5500 packs a vari-angle touchscreen",
                FullDescription = "Nikon has announced its latest DSLR, the D5500. A lightweight, compact DX-format camera with a 24.2MP sensor, it’s the first of its type to offer a vari-angle touchscreen. The D5500 replaces the D5300 in Nikon’s range, and while it offers much the same features the company says it’s a much slimmer and lighter prospect. There’s a deep grip for easier handling and built-in Wi-Fi that lets you transfer and share shots via your phone or tablet.",
                ProductTemplateId = productTemplateGrouped.Id,
                //SeName = "canon-digital-slr-camera",
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Camera & photo"),
                        DisplayOrder = 1,
                    }
                }
            };
            allProducts.Add(productNikonD5500DSLR);
            productNikonD5500DSLR.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_NikonCamera_1.jpeg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productNikonD5500DSLR.Name)),
                DisplayOrder = 1,
            });
            productNikonD5500DSLR.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_NikonCamera_2.jpeg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productNikonD5500DSLR.Name)),
                DisplayOrder = 2,
            });
            _productRepository.Insert(productNikonD5500DSLR);
            var productNikonD5500DSLR_associated_1 = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = false, //hide this products
                ParentGroupedProductId = productNikonD5500DSLR.Id,
                Name = "Nikon D5500 DSLR - Black",
                Sku = "N5500DS_B",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "canon-digital-slr-camera-black",
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allProducts.Add(productNikonD5500DSLR_associated_1);
            productNikonD5500DSLR_associated_1.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_NikonCamera_black.jpeg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName("Canon Digital SLR Camera - Black")),
                DisplayOrder = 1,
            });
            _productRepository.Insert(productNikonD5500DSLR_associated_1);
            var productNikonD5500DSLR_associated_2 = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = false, //hide this products
                ParentGroupedProductId = productNikonD5500DSLR.Id,
                Name = "Nikon D5500 DSLR - Red",
                Sku = "N5500DS_R",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "canon-digital-slr-camera-silver",
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allProducts.Add(productNikonD5500DSLR_associated_2);
            productNikonD5500DSLR_associated_2.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_NikonCamera_red.jpeg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName("Canon Digital SLR Camera - Silver")),
                DisplayOrder = 1,
            });
            _productRepository.Insert(productNikonD5500DSLR_associated_2);





            var productLeica = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "Leica T Mirrorless Digital Camera",
                Sku = "LT_MIR_DC",
                ShortDescription = "Leica T (Typ 701) Silver",
                FullDescription = "<p>The new Leica T offers a minimalist design that's crafted from a single block of aluminum.  Made in Germany and assembled by hand, this 16.3 effective mega pixel camera is easy to use.  With a massive 3.7 TFT LCD intuitive touch screen control, the user is able to configure and save their own menu system.  The Leica T has outstanding image quality and also has 16GB of built in memory.  This is Leica's first system camera to use Wi-Fi.  Add the T-App to your portable iOS device and be able to transfer and share your images (free download from the Apple App Store)</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "canon-vixia-hf100-camcorder",
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Camera & photo"),
                        DisplayOrder = 3,
                    }
                }
            };
            allProducts.Add(productLeica);
            productLeica.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_LeicaT.jpeg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productLeica.Name)),
                DisplayOrder = 1,
            });
            _productRepository.Insert(productLeica);






            var productAppleICam = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "Apple iCam",
                Sku = "APPLE_CAM",
                ShortDescription = "Photography becomes smart",
                FullDescription = "<p>A few months ago we featured the amazing WVIL camera, by many considered the future of digital photography. This is another very good looking concept, iCam is the vision of Italian designer Antonio DeRosa, the idea is to have a device that attaches to the iPhone 5, which then allows the user to have a camera with interchangeable lenses. The device would also feature a front-touch screen and a projector. Would be great if apple picked up on this and made it reality.</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "panasonic-hdc-sdt750k-high-definition-3d-camcorder",
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Camera & photo"),
                        DisplayOrder = 2,
                    }
                },
            };
            allProducts.Add(productAppleICam);
            productAppleICam.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_iCam.jpeg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productAppleICam.Name)),
                DisplayOrder = 1,
            });
            _productRepository.Insert(productAppleICam);




            #endregion

            #region Cell Phone

            var productHtcOne = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "HTC One M8 Android L 5.0 Lollipop",
                Sku = "M8_HTC_5L",
                ShortDescription = "HTC - One (M8) 4G LTE Cell Phone with 32GB Memory - Gunmetal (Sprint)",
                FullDescription = "<p><b>HTC One (M8) Cell Phone for Sprint:</b> With its brushed-metal design and wrap-around unibody frame, the HTC One (M8) is designed to fit beautifully in your hand. It's fun to use with amped up sound and a large Full HD touch screen, and intuitive gesture controls make it seem like your phone almost knows what you need before you do. <br><br>Sprint Easy Pay option available in store.</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "blackberry-bold-9000-phone-black-att",
                AllowCustomerReviews = true,
                Published = true,
                ShowOnHomePage = true,
                MarkAsNew = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Cell phones"),
                        DisplayOrder = 1,
                    }
                }
            };
            allProducts.Add(productHtcOne);
            productHtcOne.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_HTC_One_M8.jpeg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productHtcOne.Name)),
                DisplayOrder = 1,
            });
            _productRepository.Insert(productHtcOne);






            var productHtcOneMini = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "HTC One Mini Blue",
                Sku = "OM_HTC_BL",
                ShortDescription = "HTC One and HTC One Mini now available in bright blue hue",
                FullDescription = "<p>HTC One mini smartphone with 4.30-inch 720x1280 display powered by 1.4GHz processor alongside 1GB RAM and 4-Ultrapixel rear camera.</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "samsung-rugby-a837-phone-black-att",
                AllowCustomerReviews = true,
                Published = true,
                MarkAsNew = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Cell phones"),
                        DisplayOrder = 1,
                    }
                }
            };
            allProducts.Add(productHtcOneMini);
            productHtcOneMini.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_HTC_One_Mini_1.jpeg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productHtcOneMini.Name)),
                DisplayOrder = 1,
            });
            productHtcOneMini.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_HTC_One_Mini_2.jpeg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productHtcOneMini.Name)),
                DisplayOrder = 2,
            });
            _productRepository.Insert(productHtcOneMini);






            var productNokiaLumia = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "Nokia Lumia 1020",
                Sku = "N_1020_LU",
                ShortDescription = "Nokia Lumia 1020 4G Cell Phone (Unlocked)",
                FullDescription = "<p>Capture special moments for friends and family with this Nokia Lumia 1020 32GB WHITE cell phone that features an easy-to-use 41.0MP rear-facing camera and a 1.2MP front-facing camera. The AMOLED touch screen offers 768 x 1280 resolution for crisp visuals.</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "sony-dcr-sr85-1mp-60gb-hard-drive-handycam-camcorder",
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Cell phones"),
                        DisplayOrder = 1,
                    }
                }
            };
            allProducts.Add(productNokiaLumia);
            productNokiaLumia.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_Lumia1020.jpeg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productNokiaLumia.Name)),
                DisplayOrder = 1,
            });
            _productRepository.Insert(productNokiaLumia);


            #endregion

            #region Others



            var productBeatsPill = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "Beats Pill 2.0 Wireless Speaker",
                Sku = "BP_20_WSP",
                ShortDescription = "<b>Pill 2.0 Portable Bluetooth Speaker (1-Piece):</b> Watch your favorite movies and listen to music with striking sound quality. This lightweight, portable speaker is easy to take with you as you travel to any destination, keeping you entertained wherever you are. ",
                FullDescription = "<ul><li>Pair and play with your Bluetooth® device with 30 foot range</li><li>Built-in speakerphone</li><li>7 hour rechargeable battery</li><li>Power your other devices with USB charge out</li><li>Tap two Beats Pills™ together for twice the sound with Beats Bond™</li></ul>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "acer-aspire-one-89-mini-notebook-case-black",
                AllowCustomerReviews = true,
                Published = true,
                MarkAsNew = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Others"),
                        DisplayOrder = 1,
                    }
                }
            };
            allProducts.Add(productBeatsPill);
            productBeatsPill.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_PillBeats_1.jpeg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productBeatsPill.Name)),
                DisplayOrder = 1,
            });
            productBeatsPill.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_PillBeats_2.jpeg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productBeatsPill.Name)),
                DisplayOrder = 2,
            });
            _productRepository.Insert(productBeatsPill);





            var productUniversalTabletCover = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "Universal 7-8 Inch Tablet Cover",
                Sku = "TC_78I_UN",
                ShortDescription = "Universal protection for 7-inch & 8-inch tablets",
                FullDescription = "<p>Made of durable polyurethane, our Universal Cover is slim, lightweight, and strong, with protective corners that stretch to hold most 7 and 8-inch tablets securely. This tough case helps protects your tablet from bumps, scuffs, and dings.</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "apc-back-ups-rs-800va-ups-800-va-ups-battery-lead-acid-br800blk",
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Others"),
                        DisplayOrder = 1,
                    }
                }
            };
            allProducts.Add(productUniversalTabletCover);
            productUniversalTabletCover.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_TabletCover.jpeg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productUniversalTabletCover.Name)),
                DisplayOrder = 1,
            });
            _productRepository.Insert(productUniversalTabletCover);




            var productPortableSoundSpeakers = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "Portable Sound Speakers",
                Sku = "PT_SPK_SN",
                ShortDescription = "Universall portable sound speakers",
                FullDescription = "<p>Your phone cut the cord, now it's time for you to set your music free and buy a Bluetooth speaker. Thankfully, there's one suited for everyone out there.</p><p>Some Bluetooth speakers excel at packing in as much functionality as the unit can handle while keeping the price down. Other speakers shuck excess functionality in favor of premium build materials instead. Whatever path you choose to go down, you'll be greeted with many options to suit your personal tastes.</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "microsoft-bluetooth-notebook-mouse-5000-macwindows",
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductCategories =
                {
                    new ProductCategory
                    {
                        Category = _categoryRepository.Table.Single(c => c.Name == "Others"),
                        DisplayOrder = 1,
                    }
                }
            };
            allProducts.Add(productPortableSoundSpeakers);
            productPortableSoundSpeakers.ProductPictures.Add(new ProductPicture
            {
                Picture = pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "product_Speakers.jpeg"), MimeTypes.ImageJpeg, pictureService.GetPictureSeName(productPortableSoundSpeakers.Name)),
                DisplayOrder = 1,
            });
            _productRepository.Insert(productPortableSoundSpeakers);


            #endregion

            //search engine names
            foreach (var product in allProducts)
            {
                _urlRecordRepository.Insert(new UrlRecord
                {
                    EntityId = product.Id,
                    EntityName = "Product",
                    LanguageId = 0,
                    IsActive = true,
                    Slug = product.ValidateSeName("", product.Name, true)
                });
            }


            #region Product Tags

            //product tags
            AddProductTag(productBeatsPill, "computer");
            AddProductTag(productBeatsPill, "cool");
            AddProductTag(productAdobePhotoshop, "computer");
            AddProductTag(productAdobePhotoshop, "awesome");
            AddProductTag(productUniversalTabletCover, "computer");
            AddProductTag(productUniversalTabletCover, "cool");
            AddProductTag(productAppleMacBookPro, "compact");
            AddProductTag(productAppleMacBookPro, "awesome");
            AddProductTag(productAppleMacBookPro, "computer");
            AddProductTag(productAsusN551JK, "compact");
            AddProductTag(productAsusN551JK, "awesome");
            AddProductTag(productAsusN551JK, "computer");
            AddProductTag(productHtcOne, "cell");
            AddProductTag(productHtcOne, "compact");
            AddProductTag(productHtcOne, "awesome");
            AddProductTag(productBuildComputer, "awesome");
            AddProductTag(productBuildComputer, "computer");
            AddProductTag(productNikonD5500DSLR, "cool");
            AddProductTag(productNikonD5500DSLR, "camera");
            AddProductTag(productLeica, "camera");
            AddProductTag(productLeica, "cool");
            AddProductTag(productDigitalStorm, "cool");
            AddProductTag(productDigitalStorm, "computer");
            AddProductTag(productWindows8Pro, "awesome");
            AddProductTag(productWindows8Pro, "computer");
            AddProductTag(productLenovoIdeaCentre, "awesome");
            AddProductTag(productLenovoIdeaCentre, "computer");
            AddProductTag(productSamsungSeries, "nice");
            AddProductTag(productSamsungSeries, "computer");
            AddProductTag(productSamsungSeries, "compact");
            AddProductTag(productHpSpectre, "nice");
            AddProductTag(productHpSpectre, "computer");
            AddProductTag(productHpEnvy, "computer");
            AddProductTag(productHpEnvy, "cool");
            AddProductTag(productHpEnvy, "compact");
            AddProductTag(productSoundForge, "game");
            AddProductTag(productSoundForge, "computer");
            AddProductTag(productSoundForge, "cool");


            #endregion

            #region  Reviews

            //reviews
            var random = new Random();
            foreach (var product in allProducts)
            {
                if (product.ProductType != ProductType.SimpleProduct)
                    continue;

                //only 3 of 4 products will have reviews
                if (random.Next(4) == 3)
                    continue;

                //rating from 4 to 5
                var rating = random.Next(4, 6);
                product.ProductReviews.Add(new ProductReview
                {
                    CustomerId = defaultCustomer.Id,
                    ProductId = product.Id,
                    StoreId = defaultStore.Id,
                    IsApproved = true,
                    Title = "Some sample review",
                    ReviewText = string.Format("This sample review is for the {0}. I've been waiting for this product to be available. It is priced just right.", product.Name),
                    //random (4 or 5)
                    Rating = rating,
                    HelpfulYesTotal = 0,
                    HelpfulNoTotal = 0,
                    CreatedOnUtc = DateTime.UtcNow
                });
                product.ApprovedRatingSum = rating;
                product.ApprovedTotalReviews = product.ProductReviews.Count;

            }
            _productRepository.Update(allProducts);

            #endregion
        }

        protected virtual void InstallForums()
        {
            var forumGroup = new ForumGroup
            {
                Name = "General",
                DisplayOrder = 5,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            };

            _forumGroupRepository.Insert(forumGroup);

            var newProductsForum = new Forum
            {
                ForumGroup = forumGroup,
                Name = "New Products",
                Description = "Discuss new products and industry trends",
                NumTopics = 0,
                NumPosts = 0,
                LastPostCustomerId = 0,
                LastPostTime = null,
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            };
            _forumRepository.Insert(newProductsForum);

            var mobileDevicesForum = new Forum
            {
                ForumGroup = forumGroup,
                Name = "Mobile Devices Forum",
                Description = "Discuss the mobile phone market",
                NumTopics = 0,
                NumPosts = 0,
                LastPostCustomerId = 0,
                LastPostTime = null,
                DisplayOrder = 10,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            };
            _forumRepository.Insert(mobileDevicesForum);
        }

        protected virtual void InstallBlogPosts(string defaultUserEmail)
        {
            var defaultLanguage = _languageRepository.Table.FirstOrDefault();

            var blogPosts = new List<BlogPost>
                                {
                                    new BlogPost
                                        {
                                             AllowComments = true,
                                             Language = defaultLanguage,
                                             Title = "How a blog can help your growing e-Commerce business",
                                             BodyOverview = "<p>When you start an online business, your main aim is to sell the products, right? As a business owner, you want to showcase your store to more audience. So, you decide to go on social media, why? Because everyone is doing it, then why shouldn&rsquo;t you? It is tempting as everyone is aware of the hype that it is the best way to market your brand.</p><p>Do you know having a blog for your online store can be very helpful? Many businesses do not understand the importance of having a blog because they don&rsquo;t have time to post quality content.</p><p>Today, we will talk about how a blog can play an important role for the growth of your e-Commerce business. Later, we will also discuss some tips that will be helpful to you for writing business related blog posts.</p>",
                                             Body = "<p>When you start an online business, your main aim is to sell the products, right? As a business owner, you want to showcase your store to more audience. So, you decide to go on social media, why? Because everyone is doing it, then why shouldn&rsquo;t you? It is tempting as everyone is aware of the hype that it is the best way to market your brand.</p><p>Do you know having a blog for your online store can be very helpful? Many businesses do not understand the importance of having a blog because they don&rsquo;t have time to post quality content.</p><p>Today, we will talk about how a blog can play an important role for the growth of your e-Commerce business. Later, we will also discuss some tips that will be helpful to you for writing business related blog posts.</p><h3>1) Blog is useful in educating your customers</h3><p>Blogging is one of the best way by which you can educate your customers about your products/services that you offer. This helps you as a business owner to bring more value to your brand. When you provide useful information to the customers about your products, they are more likely to buy products from you. You can use your blog for providing tutorials in regard to the use of your products.</p><p><strong>For example:</strong> If you have an online store that offers computer parts. You can write tutorials about how to build a computer or how to make your computer&rsquo;s performance better. While talking about these things, you can mention products in the tutorials and provide link to your products within the blog post from your website. Your potential customers might get different ideas of using your product and will likely to buy products from your online store.</p><h3>2) Blog helps your business in Search Engine Optimization (SEO)</h3><p>Blog posts create more internal links to your website which helps a lot in SEO. Blog is a great way to have quality content on your website related to your products/services which is indexed by all major search engines like Google, Bing and Yahoo. The more original content you write in your blog post, the better ranking you will get in search engines. SEO is an on-going process and posting blog posts regularly keeps your site active all the time which is beneficial when it comes to search engine optimization.</p><p><strong>For example:</strong> Let&rsquo;s say you sell &ldquo;Sony Television Model XYZ&rdquo; and you regularly publish blog posts about your product. Now, whenever someone searches for &ldquo;Sony Television Model XYZ&rdquo;, Google will crawl on your website knowing that you have something to do with this particular product. Hence, your website will show up on the search result page whenever this item is being searched.</p><h3>3) Blog helps in boosting your sales by convincing the potential customers to buy</h3><p>If you own an online business, there are so many ways you can share different stories with your audience in regard your products/services that you offer. Talk about how you started your business, share stories that educate your audience about what&rsquo;s new in your industry, share stories about how your product/service was beneficial to someone or share anything that you think your audience might find interesting (it does not have to be related to your product). This kind of blogging shows that you are an expert in your industry and interested in educating your audience. It sets you apart in the competitive market. This gives you an opportunity to showcase your expertise by educating the visitors and it can turn your audience into buyers.</p><p><strong>Fun Fact:</strong> Did you know that 92% of companies who decided to blog acquired customers through their blog?</p><p><a href=\"http://www.nopcommerce.com/\">nopCommerce</a> is great e-Commerce solution that also offers a variety of CMS features including blog. A store owner has full access for managing the blog posts and related comments.</p>",
                                             Tags = "e-commerce, blog, moey",
                                             CreatedOnUtc = DateTime.UtcNow,
                                        },
                                    new BlogPost
                                        {
                                             AllowComments = true,
                                             Language = defaultLanguage,
                                             Title = "Why your online store needs a wish list",
                                             BodyOverview = "<p>What comes to your mind, when you hear the term&rdquo; wish list&rdquo;? The application of this feature is exactly how it sounds like: a list of things that you wish to get. As an online store owner, would you like your customers to be able to save products in a wish list so that they review or buy them later? Would you like your customers to be able to share their wish list with friends and family for gift giving?</p><p>Offering your customers a feature of wish list as part of shopping cart is a great way to build loyalty to your store site. Having the feature of wish list on a store site allows online businesses to engage with their customers in a smart way as it allows the shoppers to create a list of what they desire and their preferences for future purchase.</p>",
                                             Body = "<p>What comes to your mind, when you hear the term&rdquo; wish list&rdquo;? The application of this feature is exactly how it sounds like: a list of things that you wish to get. As an online store owner, would you like your customers to be able to save products in a wish list so that they review or buy them later? Would you like your customers to be able to share their wish list with friends and family for gift giving?</p><p>Offering your customers a feature of wish list as part of shopping cart is a great way to build loyalty to your store site. Having the feature of wish list on a store site allows online businesses to engage with their customers in a smart way as it allows the shoppers to create a list of what they desire and their preferences for future purchase.</p><p>Does every e-Commerce store needs a wish list? The answer to this question in most cases is yes, because of the following reasons:</p><p><strong>Understanding the needs of your customers</strong> - A wish list is a great way to know what is in your customer&rsquo;s mind. Try to think the purchase history as a small portion of the customer&rsquo;s preferences. But, the wish list is like a wide open door that can give any online business a lot of valuable information about their customer and what they like or desire.</p><p><strong>Shoppers like to share their wish list with friends and family</strong> - Providing your customers a way to email their wish list to their friends and family is a pleasant way to make online shopping enjoyable for the shoppers. It is always a good idea to make the wish list sharable by a unique link so that it can be easily shared though different channels like email or on social media sites.</p><p><strong>Wish list can be a great marketing tool</strong> &ndash; Another way to look at wish list is a great marketing tool because it is extremely targeted and the recipients are always motivated to use it. For example: when your younger brother tells you that his wish list is on a certain e-Commerce store. What is the first thing you are going to do? You are most likely to visit the e-Commerce store, check out the wish list and end up buying something for your younger brother.</p><p>So, how a wish list is a marketing tool? The reason is quite simple, it introduce your online store to new customers just how it is explained in the above example.</p><p><strong>Encourage customers to return to the store site</strong> &ndash; Having a feature of wish list on the store site can increase the return traffic because it encourages customers to come back and buy later. Allowing the customers to save the wish list to their online accounts gives them a reason return to the store site and login to the account at any time to view or edit the wish list items.</p><p><strong>Wish list can be used for gifts for different occasions like weddings or birthdays. So, what kind of benefits a gift-giver gets from a wish list?</strong></p><ul><li>It gives them a surety that they didn&rsquo;t buy a wrong gift</li><li>It guarantees that the recipient will like the gift</li><li>It avoids any awkward moments when the recipient unwraps the gift and as a gift-giver you got something that the recipient do not want</li></ul><p><strong>Wish list is a great feature to have on a store site &ndash; So, what kind of benefits a business owner gets from a wish list</strong></p><ul><li>It is a great way to advertise an online store as many people do prefer to shop where their friend or family shop online</li><li>It allows the current customers to return to the store site and open doors for the new customers</li><li>It allows store admins to track what&rsquo;s in customers wish list and run promotions accordingly to target specific customer segments</li></ul><p><a href=\"http://www.nopcommerce.com/\">nopCommerce</a> offers the feature of wish list that allows customers to create a list of products that they desire or planning to buy in future.</p>",
                                             Tags = "e-commerce, nopCommerce, sample tag, money",
                                             CreatedOnUtc = DateTime.UtcNow.AddSeconds(1),
                                        },
                                };
            _blogPostRepository.Insert(blogPosts);

            //search engine names
            foreach (var blogPost in blogPosts)
            {
                _urlRecordRepository.Insert(new UrlRecord
                {
                    EntityId = blogPost.Id,
                    EntityName = "BlogPost",
                    LanguageId = blogPost.LanguageId,
                    IsActive = true,
                    Slug = blogPost.ValidateSeName("", blogPost.Title, true)
                });
            }

            //comments
            var defaultCustomer = _customerRepository.Table.FirstOrDefault(x => x.Email == defaultUserEmail);
            if (defaultCustomer == null)
                throw new Exception("Cannot load default customer");

            //default store
            var defaultStore = _storeRepository.Table.FirstOrDefault();
            if (defaultStore == null)
                throw new Exception("No default store could be loaded");

            foreach (var blogPost in blogPosts)
            {
                blogPost.BlogComments.Add(new BlogComment
                {
                    BlogPostId = blogPost.Id,
                    CustomerId = defaultCustomer.Id,
                    CommentText = "This is a sample comment for this blog post",
                    IsApproved = true,
                    StoreId = defaultStore.Id,
                    CreatedOnUtc = DateTime.UtcNow
                });
            }
            _blogPostRepository.Update(blogPosts);
        }

        protected virtual void InstallNews(string defaultUserEmail)
        {
            var defaultLanguage = _languageRepository.Table.FirstOrDefault();

            var news = new List<NewsItem>
                                {
                                    new NewsItem
                                    {
                                         AllowComments = true,
                                         Language = defaultLanguage,
                                         Title = "About nopCommerce",
                                         Short = "It's stable and highly usable. From downloads to documentation, www.nopCommerce.com offers a comprehensive base of information, resources, and support to the nopCommerce community.",
                                         Full = "<p>For full feature list go to <a href=\"http://www.nopCommerce.com\">nopCommerce.com</a></p><p>Providing outstanding custom search engine optimization, web development services and e-commerce development solutions to our clients at a fair price in a professional manner.</p>",
                                         Published  = true,
                                         CreatedOnUtc = DateTime.UtcNow,
                                    },
                                    new NewsItem
                                    {
                                         AllowComments = true,
                                         Language = defaultLanguage,
                                         Title = "nopCommerce new release!",
                                         Short = "nopCommerce includes everything you need to begin your e-commerce online store. We have thought of everything and it's all included! nopCommerce is a fully customizable shopping cart",
                                         Full = "<p>nopCommerce includes everything you need to begin your e-commerce online store. We have thought of everything and it's all included!</p>",
                                         Published  = true,
                                         CreatedOnUtc = DateTime.UtcNow.AddSeconds(1),
                                    },
                                    new NewsItem
                                    {
                                         AllowComments = true,
                                         Language = defaultLanguage,
                                         Title = "New online store is open!",
                                         Short = "The new nopCommerce store is open now! We are very excited to offer our new range of products. We will be constantly adding to our range so please register on our site.",
                                         Full = "<p>Our online store is officially up and running. Stock up for the holiday season! We have a great selection of items. We will be constantly adding to our range so please register on our site, this will enable you to keep up to date with any new products.</p><p>All shipping is worldwide and will leave the same day an order is placed! Happy Shopping and spread the word!!</p>",
                                         Published  = true,
                                         CreatedOnUtc = DateTime.UtcNow.AddSeconds(2),
                                    },

                                };
            _newsItemRepository.Insert(news);

            //search engine names
            foreach (var newsItem in news)
            {
                _urlRecordRepository.Insert(new UrlRecord
                {
                    EntityId = newsItem.Id,
                    EntityName = "NewsItem",
                    LanguageId = newsItem.LanguageId,
                    IsActive = true,
                    Slug = newsItem.ValidateSeName("", newsItem.Title, true)
                });
            }

            //comments
            var defaultCustomer = _customerRepository.Table.FirstOrDefault(x => x.Email == defaultUserEmail);
            if (defaultCustomer == null)
                throw new Exception("Cannot load default customer");

            //default store
            var defaultStore = _storeRepository.Table.FirstOrDefault();
            if (defaultStore == null)
                throw new Exception("No default store could be loaded");

            foreach (var newsItem in news)
            {
                newsItem.NewsComments.Add(new NewsComment
                {
                    NewsItemId = newsItem.Id,
                    CustomerId = defaultCustomer.Id,
                    CommentTitle = "Sample comment title",
                    CommentText = "This is a sample comment...",
                    IsApproved = true,
                    StoreId = defaultStore.Id,
                    CreatedOnUtc = DateTime.UtcNow
                });
            }
            _newsItemRepository.Update(news);
        }

        protected virtual void InstallPolls()
        {
            var defaultLanguage = _languageRepository.Table.FirstOrDefault();
            var poll1 = new Poll
            {
                Language = defaultLanguage,
                Name = "Do you like nopCommerce?",
                SystemKeyword = "",
                Published = true,
                ShowOnHomePage = true,
                DisplayOrder = 1,
            };
            poll1.PollAnswers.Add(new PollAnswer
            {
                Name = "Excellent",
                DisplayOrder = 1,
            });
            poll1.PollAnswers.Add(new PollAnswer
            {
                Name = "Good",
                DisplayOrder = 2,
            });
            poll1.PollAnswers.Add(new PollAnswer
            {
                Name = "Poor",
                DisplayOrder = 3,
            });
            poll1.PollAnswers.Add(new PollAnswer
            {
                Name = "Very bad",
                DisplayOrder = 4,
            });
            _pollRepository.Insert(poll1);
        }

        protected virtual void InstallActivityLogTypes()
        {
            var activityLogTypes = new List<ActivityLogType>
            {
                //admin area activities
                new ActivityLogType
                {
                    SystemKeyword = "AddNewAddressAttribute",
                    Enabled = true,
                    Name = "Add a new address attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewAddressAttributeValue",
                    Enabled = true,
                    Name = "Add a new address attribute value"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewBlogPost",
                    Enabled = true,
                    Name = "Add a new blog post"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewCampaign",
                    Enabled = true,
                    Name = "Add a new campaign"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewCategory",
                    Enabled = true,
                    Name = "Add a new category"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewCountry",
                    Enabled = true,
                    Name = "Add a new country"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewCustomer",
                    Enabled = true,
                    Name = "Add a new customer"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewCustomerAttribute",
                    Enabled = true,
                    Name = "Add a new customer attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewCustomerAttributeValue",
                    Enabled = true,
                    Name = "Add a new customer attribute value"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewCustomerRole",
                    Enabled = true,
                    Name = "Add a new customer role"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewEmailAccount",
                    Enabled = true,
                    Name = "Add a new email account"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewLanguage",
                    Enabled = true,
                    Name = "Add a new language"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewNews",
                    Enabled = true,
                    Name = "Add a new news"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewProduct",
                    Enabled = true,
                    Name = "Add a new product"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewSetting",
                    Enabled = true,
                    Name = "Add a new setting"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewStateProvince",
                    Enabled = true,
                    Name = "Add a new state or province"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewStore",
                    Enabled = true,
                    Name = "Add a new store"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewTopic",
                    Enabled = true,
                    Name = "Add a new topic"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewWidget",
                    Enabled = true,
                    Name = "Add a new widget"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteActivityLog",
                    Enabled = true,
                    Name = "Delete activity log"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteAddressAttribute",
                    Enabled = true,
                    Name = "Delete an address attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteAddressAttributeValue",
                    Enabled = true,
                    Name = "Delete an address attribute value"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteBlogPost",
                    Enabled = true,
                    Name = "Delete a blog post"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteBlogPostComment",
                    Enabled = true,
                    Name = "Delete a blog post comment"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteCampaign",
                    Enabled = true,
                    Name = "Delete a campaign"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteCategory",
                    Enabled = true,
                    Name = "Delete category"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteCountry",
                    Enabled = true,
                    Name = "Delete a country"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteCustomer",
                    Enabled = true,
                    Name = "Delete a customer"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteCustomerAttribute",
                    Enabled = true,
                    Name = "Delete a customer attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteCustomerAttributeValue",
                    Enabled = true,
                    Name = "Delete a customer attribute value"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteCustomerRole",
                    Enabled = true,
                    Name = "Delete a customer role"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteEmailAccount",
                    Enabled = true,
                    Name = "Delete an email account"
                },

                new ActivityLogType
                {
                    SystemKeyword = "DeleteLanguage",
                    Enabled = true,
                    Name = "Delete a language"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteMessageTemplate",
                    Enabled = true,
                    Name = "Delete a message template"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteNews",
                    Enabled = true,
                    Name = "Delete a news"
                },
                 new ActivityLogType
                {
                    SystemKeyword = "DeleteNewsComment",
                    Enabled = true,
                    Name = "Delete a news comment"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteProduct",
                    Enabled = true,
                    Name = "Delete a product"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteProductReview",
                    Enabled = true,
                    Name = "Delete a product review"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteSetting",
                    Enabled = true,
                    Name = "Delete a setting"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteStateProvince",
                    Enabled = true,
                    Name = "Delete a state or province"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteStore",
                    Enabled = true,
                    Name = "Delete a store"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteTopic",
                    Enabled = true,
                    Name = "Delete a topic"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteWidget",
                    Enabled = true,
                    Name = "Delete a widget"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditActivityLogTypes",
                    Enabled = true,
                    Name = "Edit activity log types"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditAddressAttribute",
                    Enabled = true,
                    Name = "Edit an address attribute"
                },
                 new ActivityLogType
                {
                    SystemKeyword = "EditAddressAttributeValue",
                    Enabled = true,
                    Name = "Edit an address attribute value"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditBlogPost",
                    Enabled = true,
                    Name = "Edit a blog post"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditCampaign",
                    Enabled = true,
                    Name = "Edit a campaign"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditCategory",
                    Enabled = true,
                    Name = "Edit category"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditCountry",
                    Enabled = true,
                    Name = "Edit a country"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditCustomer",
                    Enabled = true,
                    Name = "Edit a customer"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditCustomerAttribute",
                    Enabled = true,
                    Name = "Edit a customer attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditCustomerAttributeValue",
                    Enabled = true,
                    Name = "Edit a customer attribute value"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditCustomerRole",
                    Enabled = true,
                    Name = "Edit a customer role"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditEmailAccount",
                    Enabled = true,
                    Name = "Edit an email account"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditLanguage",
                    Enabled = true,
                    Name = "Edit a language"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditMessageTemplate",
                    Enabled = true,
                    Name = "Edit a message template"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditNews",
                    Enabled = true,
                    Name = "Edit a news"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditPlugin",
                    Enabled = true,
                    Name = "Edit a plugin"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditProduct",
                    Enabled = true,
                    Name = "Edit a product"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditProductReview",
                    Enabled = true,
                    Name = "Edit a product review"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditPromotionProviders",
                    Enabled = true,
                    Name = "Edit promotion providers"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditSettings",
                    Enabled = true,
                    Name = "Edit setting(s)"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditStateProvince",
                    Enabled = true,
                    Name = "Edit a state or province"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditStore",
                    Enabled = true,
                    Name = "Edit a store"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditTask",
                    Enabled = true,
                    Name = "Edit a task"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditTopic",
                    Enabled = true,
                    Name = "Edit a topic"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditWidget",
                    Enabled = true,
                    Name = "Edit a widget"
                },
                new ActivityLogType
                {
                    SystemKeyword = "Impersonation.Started",
                    Enabled = true,
                    Name = "Customer impersonation session. Started"
                },
                new ActivityLogType
                {
                    SystemKeyword = "Impersonation.Finished",
                    Enabled = true,
                    Name = "Customer impersonation session. Finished"
                },
                new ActivityLogType
                {
                    SystemKeyword = "InstallNewPlugin",
                    Enabled = true,
                    Name = "Install a new plugin"
                },
                new ActivityLogType
                {
                    SystemKeyword = "UninstallPlugin",
                    Enabled = true,
                    Name = "Uninstall a plugin"
                },
                //public store activities
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.ViewCategory",
                    Enabled = false,
                    Name = "Public store. View a category"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.ViewProduct",
                    Enabled = false,
                    Name = "Public store. View a product"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.SendPM",
                    Enabled = false,
                    Name = "Public store. Send PM"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.ContactUs",
                    Enabled = false,
                    Name = "Public store. Use contact us form"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.Login",
                    Enabled = false,
                    Name = "Public store. Login"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.Logout",
                    Enabled = false,
                    Name = "Public store. Logout"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.AddProductReview",
                    Enabled = false,
                    Name = "Public store. Add product review"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.AddNewsComment",
                    Enabled = false,
                    Name = "Public store. Add news comment"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.AddBlogComment",
                    Enabled = false,
                    Name = "Public store. Add blog comment"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.AddForumTopic",
                    Enabled = false,
                    Name = "Public store. Add forum topic"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.EditForumTopic",
                    Enabled = false,
                    Name = "Public store. Edit forum topic"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.DeleteForumTopic",
                    Enabled = false,
                    Name = "Public store. Delete forum topic"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.AddForumPost",
                    Enabled = false,
                    Name = "Public store. Add forum post"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.EditForumPost",
                    Enabled = false,
                    Name = "Public store. Edit forum post"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.DeleteForumPost",
                    Enabled = false,
                    Name = "Public store. Delete forum post"
                }
            };
            _activityLogTypeRepository.Insert(activityLogTypes);
        }

        protected virtual void InstallProductTemplates()
        {
            var productTemplates = new List<ProductTemplate>
                               {
                                   new ProductTemplate
                                       {
                                           Name = "Simple product",
                                           ViewPath = "ProductTemplate.Simple",
                                           DisplayOrder = 10,
                                           IgnoredProductTypes = ((int)ProductType.GroupedProduct).ToString()
                                       },
                                   new ProductTemplate
                                       {
                                           Name = "Grouped product (with variants)",
                                           ViewPath = "ProductTemplate.Grouped",
                                           DisplayOrder = 100,
                                           IgnoredProductTypes = ((int)ProductType.SimpleProduct).ToString()
                                       }
                               };
            _productTemplateRepository.Insert(productTemplates);
        }

        protected virtual void InstallCategoryTemplates()
        {
            var categoryTemplates = new List<CategoryTemplate>
                               {
                                   new CategoryTemplate
                                       {
                                           Name = "Products in Grid or Lines",
                                           ViewPath = "CategoryTemplate.ProductsInGridOrLines",
                                           DisplayOrder = 1
                                       },
                               };
            _categoryTemplateRepository.Insert(categoryTemplates);
        }

        protected virtual void InstallTopicTemplates()
        {
            var topicTemplates = new List<TopicTemplate>
                               {
                                   new TopicTemplate
                                       {
                                           Name = "Default template",
                                           ViewPath = "TopicDetails",
                                           DisplayOrder = 1
                                       },
                               };
            _topicTemplateRepository.Insert(topicTemplates);
        }

        protected virtual void InstallScheduleTasks()
        {
            var tasks = new List<ScheduleTask>
            {
                new ScheduleTask
                {
                    Name = "Send emails",
                    Seconds = 60,
                    Type = "Nop.Services.Messages.QueuedMessagesSendTask, Nop.Services",
                    Enabled = true,
                    StopOnError = false,
                },
                new ScheduleTask
                {
                    Name = "Keep alive",
                    Seconds = 300,
                    Type = "Nop.Services.Common.KeepAliveTask, Nop.Services",
                    Enabled = true,
                    StopOnError = false,
                },
                new ScheduleTask
                {
                    Name = "Delete guests",
                    Seconds = 600,
                    Type = "Nop.Services.Customers.DeleteGuestsTask, Nop.Services",
                    Enabled = true,
                    StopOnError = false,
                },
                new ScheduleTask
                {
                    Name = "Clear cache",
                    Seconds = 600,
                    Type = "Nop.Services.Caching.ClearCacheTask, Nop.Services",
                    Enabled = false,
                    StopOnError = false,
                },
                new ScheduleTask
                {
                    Name = "Clear log",
                    //60 minutes
                    Seconds = 3600,
                    Type = "Nop.Services.Logging.ClearLogTask, Nop.Services",
                    Enabled = false,
                    StopOnError = false,
                },
            };

            _scheduleTaskRepository.Insert(tasks);
        }

        private void AddProductTag(Product product, string tag)
        {
            var productTag = _productTagRepository.Table.FirstOrDefault(pt => pt.Name == tag);
            if (productTag == null)
            {
                productTag = new ProductTag
                {
                    Name = tag,
                };
            }
            product.ProductTags.Add(productTag);
            _productRepository.Update(product);
        }

        #endregion

        #region Methods

        public virtual void InstallData(string defaultUserEmail,
            string defaultUserPassword, bool installSampleData = true)
        {
            InstallStores();
            InstallLanguages();
            InstallCountriesAndStates();
            InstallCustomersAndUsers(defaultUserEmail, defaultUserPassword);
            InstallEmailAccounts();
            InstallMessageTemplates();
            InstallSettings();
            InstallTopicTemplates();
            InstallTopics();
            InstallLocaleResources();
            InstallActivityLogTypes();
            InstallProductTemplates();
            InstallCategoryTemplates();
            InstallScheduleTasks();

            if (installSampleData)
            {
                InstallCategories();
                InstallProducts(defaultUserEmail);
                InstallForums();
                InstallBlogPosts(defaultUserEmail);
                InstallNews(defaultUserEmail);
                InstallPolls();
                InstallActivityLog(defaultUserEmail);
                InstallSearchTerms();
            }
        }

        #endregion
    }
}