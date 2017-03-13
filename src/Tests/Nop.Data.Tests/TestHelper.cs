using Nop.Core;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Configuration;
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
using System;

namespace Nop.Data.Tests
{
    public static class TestHelper
    {
        private static readonly Guid _customerGuid = Guid.NewGuid();
        private static readonly Guid _orderGuid = Guid.NewGuid();
        private static readonly Guid _downloadGuid = Guid.NewGuid();
        private static readonly Guid _newsLetterSubscriptionGuid = Guid.NewGuid();

        #region Blogs

        public static BlogComment GetTestBlogComment(this PersistenceTest test)
        {
            return new BlogComment
            {
                IsApproved = true,
                CreatedOnUtc = new DateTime(2010, 01, 03),
                Customer = test.GetTestCustomer(),
                Store = test.GetTestStore()
            };
        }

        public static BlogPost GetTestBlogPost(this PersistenceTest test)
        {
            return new BlogPost
            {
                Title = "Title 1",
                Body = "Body 1",
                BodyOverview = "BodyOverview 1",
                AllowComments = true,
                Tags = "Tags 1",
                StartDateUtc = new DateTime(2010, 01, 01),
                EndDateUtc = new DateTime(2010, 01, 02),
                CreatedOnUtc = new DateTime(2010, 01, 03),
                MetaTitle = "MetaTitle 1",
                MetaDescription = "MetaDescription 1",
                MetaKeywords = "MetaKeywords 1",
                LimitedToStores = true,
                Language = test.GetTestLanguage()
            };
        }

        #endregion

        #region Catalog

        public static Category GetTestCategory(this PersistenceTest test)
        {
            return new Category
            {
                Name = "Books",
                Description = "Description 1",
                CategoryTemplateId = 1,
                MetaKeywords = "Meta keywords",
                MetaDescription = "Meta description",
                MetaTitle = "Meta title",
                ParentCategoryId = 2,
                PictureId = 3,
                PageSize = 4,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "4, 2, 8, 12",
                ShowOnHomePage = false,
                IncludeInTopMenu = true,
                Published = true,
                SubjectToAcl = true,
                LimitedToStores = true,
                Deleted = false,
                DisplayOrder = 5,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02),
            };
        }

        public static CategoryTemplate GetTestCategoryTemplate(this PersistenceTest test)
        {
            return new CategoryTemplate
            {
                Name = "Name 1",
                ViewPath = "ViewPath 1",
                DisplayOrder = 1,
            };
        }

    
        public static Product GetTestProduct(this PersistenceTest test)
        {
            return new Product
            {
                ProductType = ProductType.GroupedProduct,
                ParentGroupedProductId = 2,
                VisibleIndividually = true,
                Name = "Name 1",
                ShortDescription = "ShortDescription 1",
                FullDescription = "FullDescription 1",
                AdminComment = "AdminComment 1",
                ProductTemplateId = 2,
                ShowOnHomePage = false,
                MetaKeywords = "Meta keywords",
                MetaDescription = "Meta description",
                MetaTitle = "Meta title",
                AllowCustomerReviews = true,
                ApprovedRatingSum = 2,
                NotApprovedRatingSum = 3,
                ApprovedTotalReviews = 4,
                NotApprovedTotalReviews = 5,
                SubjectToAcl = true,
                LimitedToStores = true,
                Sku = "sku 1",
                MarkAsNew = true,
                MarkAsNewStartDateTimeUtc = new DateTime(2010, 01, 07),
                MarkAsNewEndDateTimeUtc = new DateTime(2010, 01, 08),
                DisplayOrder = 30,
                Published = true,
                Deleted = false,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02),
            };
        }

        public static ProductCategory GetTestProductCategory(this PersistenceTest test)
        {
            return new ProductCategory
            {
                IsFeaturedProduct = true,
                DisplayOrder = 1
            };
        }
        

        public static ProductPicture GetTestProductPicture(this PersistenceTest test)
        {
            return new ProductPicture
            {
                DisplayOrder = 1,
                Picture = test.GetTestPicture()
            };
        }

        public static ProductTag GetTestProductTag(this PersistenceTest test)
        {
            return new ProductTag
            {
                Name = "Tag name 1",
            };
        }

        public static ProductTemplate GetTestProductTemplate(this PersistenceTest test)
        {
            return new ProductTemplate
            {
                Name = "Name 1",
                ViewPath = "ViewPath 1",
                DisplayOrder = 1,
            };
        }
        
        #endregion

        #region Common

        public static Address GetTestAddress(this PersistenceTest test)
        {
            return new Address
            {
                FirstName = "FirstName 1",
                LastName = "LastName 1",
                Email = "Email 1",
                Company = "Company 1",
                City = "City 1",
                Address1 = "Address1a",
                Address2 = "Address1a",
                ZipPostalCode = "ZipPostalCode 1",
                PhoneNumber = "PhoneNumber 1",
                FaxNumber = "FaxNumber 1",
                CreatedOnUtc = new DateTime(2010, 01, 01)
            };
        }

        public static AddressAttribute GetTestAddressAttribute(this PersistenceTest test)
        {
            return new AddressAttribute
            {
                Name = "Name 1",
                IsRequired = true,
                AttributeControlType = AttributeControlType.Datepicker,
                DisplayOrder = 2
            };
        }

        public static AddressAttributeValue GetTestAddressAttributeValue(this PersistenceTest test)
        {
            return new AddressAttributeValue
            {
                Name = "Name 2",
                IsPreSelected = true,
                DisplayOrder = 1,
            };
        }
       
        public static GenericAttribute GetTestGenericAttribute(this PersistenceTest test)
        {
            return new GenericAttribute
            {
                EntityId = 1,
                KeyGroup = "KeyGroup 1",
                Key = "Key 1",
                Value = "Value 1",
                StoreId = 2,
            };
        }

        public static SearchTerm GetTestSearchTerm(this PersistenceTest test)
        {
            return new SearchTerm
            {
                Keyword = "Keyword 1",
                StoreId = 1,
                Count = 2,
            };
        }

        #endregion

        #region Configuration

        public static Setting GetTestSetting(this PersistenceTest test)
        {
            return new Setting
            {
                Name = "Setting1",
                Value = "Value1",
                StoreId = 1,
            };
        }

        #endregion

        #region Customers

        public static Customer GetTestCustomer(this PersistenceTest test)
        {
            return new Customer
            {
                Username = "a@b.com",
                Email = "a@b.com",
                CustomerGuid = _customerGuid,
                AdminComment = "some comment here",
                RequireReLogin = true,
                Active = true,
                Deleted = false,
                IsSystemAccount = true,
                SystemName = "SystemName 1",
                LastIpAddress = "192.168.1.1",
                CreatedOnUtc = new DateTime(2010, 01, 01),
                LastLoginDateUtc = new DateTime(2010, 01, 02),
                LastActivityDateUtc = new DateTime(2010, 01, 03)
            };
        }

        public static CustomerAttribute GetTestCustomerAttribute(this PersistenceTest test)
        {
            return new CustomerAttribute
            {
                Name = "Name 1",
                IsRequired = true,
                AttributeControlType = AttributeControlType.Datepicker,
                DisplayOrder = 2
            };
        }

        public static CustomerAttributeValue GetTestCustomerAttributeValue(this PersistenceTest test)
        {
            return new CustomerAttributeValue
            {
                Name = "Name 2",
                IsPreSelected = true,
                DisplayOrder = 1,
            };
        }

        public static CustomerPassword GetTestCustomerPassword(this PersistenceTest test)
        {
            return new CustomerPassword
            {
                Password = "password",
                PasswordFormat = PasswordFormat.Clear,
                PasswordSalt = string.Empty,
                CreatedOnUtc = new DateTime(2010, 01, 01)
            };
        }

        public static CustomerRole GetTestCustomerRole(this PersistenceTest test)
        {
            return new CustomerRole
            {
                Name = "Administrators",
                Active = true,
                IsSystemRole = true,
                SystemName = "Administrators"
            };
        }

        public static ExternalAuthenticationRecord GetTestExternalAuthenticationRecord(this PersistenceTest test)
        {
            return new ExternalAuthenticationRecord
            {
                Email = "Email 1",
                ExternalIdentifier = "ExternalIdentifier 1",
                ExternalDisplayIdentifier = "ExternalDisplayIdentifier 1",
                OAuthToken = "OAuthToken 1",
                OAuthAccessToken = "OAuthAccessToken 1",
                ProviderSystemName = "ProviderSystemName 1"
            };
        }

        #endregion

        #region Directory

        public static Country GetTestCountry(this PersistenceTest test)
        {
            return new Country
            {
                Name = "United States",
                TwoLetterIsoCode = "US",
                ThreeLetterIsoCode = "USA",
                NumericIsoCode = 1,
                Published = true,
                DisplayOrder = 1,
                LimitedToStores = true
            };
        }

        public static StateProvince GetTestStateProvince(this PersistenceTest test)
        {
            return new StateProvince
            {
                Name = "California",
                Abbreviation = "CA",
                DisplayOrder = 1
            };
        }

        #endregion

        #region Forums

        public static Forum GetTestForum(this PersistenceTest test)
        {
            return new Forum
            {
                Name = "Forum 1",
                Description = "Forum 1 Description",
                DisplayOrder = 10,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02),
                NumPosts = 25,
                NumTopics = 15,
            };
        }

        public static ForumGroup GetTestForumGroup(this PersistenceTest test)
        {
            return new ForumGroup
            {
                Name = "Forum Group 1",
                DisplayOrder = 1,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02),
            };
        }

        public static ForumPost GetTestForumPost(this PersistenceTest test)
        {
            return new ForumPost
            {
                Text = "Forum Post 1 Text",
                IPAddress = "127.0.0.1",
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02)
            };
        }

        public static ForumSubscription GetTestForumSubscription(this PersistenceTest test)
        {
            return new ForumSubscription
            {
                CreatedOnUtc = new DateTime(2010, 01, 01),
                SubscriptionGuid = new Guid("11111111-2222-3333-4444-555555555555")
            };
        }

        public static ForumTopic GetTestForumTopic(this PersistenceTest test)
        {
            return new ForumTopic
            {
                Subject = "Forum Topic 1",
                TopicTypeId = (int)ForumTopicType.Sticky,
                Views = 123,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02),
                NumPosts = 100
            };
        }
        
        public static PrivateMessage GetTestPrivateMessage(this PersistenceTest test)
        {
            return new PrivateMessage
            {
                Subject = "Private Message 1 Subject",
                Text = "Private Message 1 Text",
                IsDeletedByAuthor = false,
                IsDeletedByRecipient = false,
                IsRead = false,
                CreatedOnUtc = new DateTime(2010, 01, 01),
            };
        }

        #endregion

        #region Localization

        public static Language GetTestLanguage(this PersistenceTest test)
        {
            return new Language
            {
                Name = "English",
                LanguageCulture = "en-Us",
                UniqueSeoCode = "en",
                FlagImageFileName = "us.png",
                Rtl = true,
                Published = true,
                LimitedToStores = true,
                DisplayOrder = 1
            };
        }

        public static LocaleStringResource GetTestLocaleStringResource(this PersistenceTest test)
        {
            return new LocaleStringResource
            {
                ResourceName = "ResourceName1",
                ResourceValue = "ResourceValue2"
            };
        }

        public static LocalizedProperty GetTestLocalizedProperty(this PersistenceTest test)
        {
            return new LocalizedProperty
            {
                EntityId = 1,
                LocaleKeyGroup = "LocaleKeyGroup 1",
                LocaleKey = "LocaleKey 1",
                LocaleValue = "LocaleValue 1"
            };
        }

        #endregion

        #region Logging

        public static ActivityLogType GetTestActivityLogType(this PersistenceTest test)
        {
            return new ActivityLogType
            {
                SystemKeyword = "SystemKeyword 1",
                Name = "Name 1",
                Enabled = true,
            };
        }

        public static Log GetTestLog(this PersistenceTest test)
        {
            return new Log
            {
                LogLevel = LogLevel.Error,
                ShortMessage = "ShortMessage1",
                FullMessage = "FullMessage1",
                IpAddress = "127.0.0.1",
                PageUrl = "http://www.someUrl1.com",
                ReferrerUrl = "http://www.someUrl2.com",
                CreatedOnUtc = new DateTime(2010, 01, 01)
            };
        }

        #endregion

        #region Media

        public static Download GetTestDownload(this PersistenceTest test)
        {
            return new Download
            {
                DownloadGuid = _downloadGuid,
                UseDownloadUrl = true,
                DownloadUrl = "http://www.someUrl.com/file.zip",
                DownloadBinary = new byte[] { 1, 2, 3 },
                ContentType = MimeTypes.ApplicationXZipCo,
                Filename = "file",
                Extension = ".zip",
                IsNew = true
            };
        }

        public static Picture GetTestPicture(this PersistenceTest test)
        {
            return new Picture
            {
                PictureBinary = new byte[] { 1, 2, 3 },
                MimeType = MimeTypes.ImagePJpeg,
                SeoFilename = "seo filename 1",
                AltAttribute = "AltAttribute 1",
                TitleAttribute = "TitleAttribute 1",
                IsNew = true
            };
        }
        
        #endregion

        #region Messages

        public static Campaign GetTestCampaign(this PersistenceTest test)
        {
            return new Campaign
            {
                Name = "Name 1",
                Subject = "Subject 1",
                Body = "Body 1",
                CreatedOnUtc = new DateTime(2010, 01, 02),
                DontSendBeforeDateUtc = new DateTime(2016, 2, 23),
                CustomerRoleId = 1,
                StoreId = 1
            };
        }

        public static EmailAccount GetTestEmailAccount(this PersistenceTest test)
        {
            return new EmailAccount
            {
                Email = "admin@yourstore.com",
                DisplayName = "Administrator",
                Host = "127.0.0.1",
                Port = 125,
                Username = "John",
                Password = "111",
                EnableSsl = true,
                UseDefaultCredentials = true
            };
        }

        public static MessageTemplate GetTestMessageTemplate(this PersistenceTest test)
        {
            return new MessageTemplate
            {
                Name = "Template1",
                BccEmailAddresses = "Bcc",
                Subject = "Subj",
                Body = "Some text",
                IsActive = true,
                AttachedDownloadId = 3,
                EmailAccountId = 1,
                LimitedToStores = true,
                DelayBeforeSend = 2,
                DelayPeriodId = 0
            };
        }

        public static NewsLetterSubscription GetTestNewsLetterSubscription(this PersistenceTest test)
        {
            return new NewsLetterSubscription
            {
                Email = "me@yourstore.com",
                NewsLetterSubscriptionGuid = _newsLetterSubscriptionGuid,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                StoreId = 1,
                Active = true
            };
        }

        public static QueuedEmail GettestQueuedEmail(this PersistenceTest test)
        {
            return new QueuedEmail
            {
                PriorityId = 5,
                From = "From",
                FromName = "FromName",
                To = "To",
                ToName = "ToName",
                ReplyTo = "ReplyTo",
                ReplyToName = "ReplyToName",
                CC = "CC",
                Bcc = "Bcc",
                Subject = "Subject",
                Body = "Body",
                AttachmentFilePath = "some file path",
                AttachmentFileName = "some file name",
                AttachedDownloadId = 3,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                SentTries = 5,
                SentOnUtc = new DateTime(2010, 02, 02),
                DontSendBeforeDateUtc = new DateTime(2016, 2, 23)
            };
        }

        #endregion

        #region News

        public static NewsComment GetTestNewsComment(this PersistenceTest test)
        {
            return new NewsComment
            {
                CommentText = "Comment text 1",
                IsApproved = false,
                CreatedOnUtc = new DateTime(2010, 01, 03),
                Customer = test.GetTestCustomer(),
                Store = test.GetTestStore()
            };
        }

        public static NewsItem GetTestNewsItem(this PersistenceTest test)
        {
            return new NewsItem
            {
                Title = "Title 1",
                Short = "Short 1",
                Full = "Full 1",
                Published = true,
                StartDateUtc = new DateTime(2010, 01, 01),
                EndDateUtc = new DateTime(2010, 01, 02),
                AllowComments = true,
                LimitedToStores = true,
                CreatedOnUtc = new DateTime(2010, 01, 03),
                MetaTitle = "MetaTitle 1",
                MetaDescription = "MetaDescription 1",
                MetaKeywords = "MetaKeywords 1"
            };
        }

        #endregion

        #region Polls

        public static Poll GetTestPoll(this PersistenceTest test)
        {
            return new Poll
            {
                Name = "Name 1",
                SystemKeyword = "SystemKeyword 1",
                Published = true,
                ShowOnHomePage = true,
                DisplayOrder = 1,
                StartDateUtc = new DateTime(2010, 01, 01),
                EndDateUtc = new DateTime(2010, 01, 02)
            };
        }

        public static PollAnswer GetTestPollAnswer(this PersistenceTest test)
        {
            return new PollAnswer
            {
                Name = "Answer 1",
                NumberOfVotes = 1,
                DisplayOrder = 2
            };
        }

        public static PollVotingRecord GetTestPollVotingRecord(this PersistenceTest test)
        {
            return new PollVotingRecord
            {
                Customer = test.GetTestCustomer(),
                CreatedOnUtc = DateTime.UtcNow
            };
        }

        #endregion

        #region Security

        public static AclRecord GetTestAclRecord(this PersistenceTest test)
        {
            return new AclRecord
            {
                EntityId = 1,
                EntityName = "EntityName 1"
            };
        }

        public static PermissionRecord GetTestPermissionRecord(this PersistenceTest test)
        {
            return new PermissionRecord
            {
                Name = "Name 1",
                SystemName = "SystemName 2",
                Category = "Category 4",
            };
        }
        
        #endregion

        #region Seo

        public static UrlRecord UrlRecord(this PersistenceTest test)
        {
            return new UrlRecord
            {
                EntityId = 1,
                EntityName = "EntityName 1",
                Slug = "Slug 1",
                LanguageId = 2
            };
        }

        #endregion

        #region Stores

        public static Store GetTestStore(this PersistenceTest test)
        {
            return new Store
            {
                Name = "Computer store",
                Url = "http://www.yourStore.com",
                Hosts = "yourStore.com,www.yourStore.com",
                DefaultLanguageId = 1,
                DisplayOrder = 2,
                CompanyName = "company name",
                CompanyAddress = "some address",
                CompanyPhoneNumber = "123456789",
            };
        }

        public static StoreMapping GetTestStoreMapping(this PersistenceTest test)
        {
            return new StoreMapping
            {
                EntityId = 1,
                EntityName = "EntityName 1"
            };
        }

        #endregion

        #region Tasks

        public static ScheduleTask GetTestScheduleTask(this PersistenceTest test)
        {
            return new ScheduleTask
            {
                Name = "Task 1",
                Seconds = 1,
                Type = "some type 1",
                Enabled = true,
                StopOnError = true,
                LeasedByMachineName = "LeasedByMachineName 1",
                LeasedUntilUtc = new DateTime(2009, 01, 01),
                LastStartUtc = new DateTime(2010, 01, 01),
                LastEndUtc = new DateTime(2010, 01, 02),
                LastSuccessUtc = new DateTime(2010, 01, 03)
            };
        }

        #endregion

        #region Topics

        public static Topic GetTestTopic(this PersistenceTest test)
        {
            return new Topic
            {
                SystemName = "SystemName 1",
                IncludeInSitemap = true,
                IncludeInTopMenu = true,
                IncludeInFooterColumn1 = true,
                IncludeInFooterColumn2 = true,
                IncludeInFooterColumn3 = true,
                DisplayOrder = 1,
                AccessibleWhenStoreClosed = true,
                IsPasswordProtected = true,
                Password = "password",
                Title = "Title 1",
                Body = "Body 1",
                Published = true,
                TopicTemplateId = 1,
                MetaKeywords = "Meta keywords",
                MetaDescription = "Meta description",
                MetaTitle = "Meta title",
                SubjectToAcl = true,
                LimitedToStores = true
            };
        }

        public static TopicTemplate GetTestTopicTemplate(this PersistenceTest test)
        {
            return new TopicTemplate
            {
                Name = "Name 1",
                ViewPath = "ViewPath 1",
                DisplayOrder = 1,
            };
        }

        #endregion
    }
}
