CREATE NONCLUSTERED INDEX [IX_LocaleStringResource] ON [LocaleStringResource] ([ResourceName] ASC,  [LanguageId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Country_DisplayOrder] ON [Country] ([DisplayOrder] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_StateProvince_CountryId] ON [StateProvince] ([CountryId]) INCLUDE ([DisplayOrder])
GO

CREATE NONCLUSTERED INDEX [IX_Log_CreatedOnUtc] ON [Log] ([CreatedOnUtc] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Customer_Email] ON [Customer] ([Email] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Customer_Username] ON [Customer] ([Username] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Customer_CustomerGuid] ON [Customer] ([CustomerGuid] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Customer_SystemName] ON [Customer] ([SystemName] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_GenericAttribute_EntityId_and_KeyGroup] ON [GenericAttribute] ([EntityId] ASC, [KeyGroup] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_QueuedEmail_CreatedOnUtc] ON [QueuedEmail] ([CreatedOnUtc] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Language_DisplayOrder] ON [Language] ([DisplayOrder] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_BlogPost_LanguageId] ON [BlogPost] ([LanguageId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_BlogComment_BlogPostId] ON [BlogComment] ([BlogPostId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_News_LanguageId] ON [News] ([LanguageId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_NewsComment_NewsItemId] ON [NewsComment] ([NewsItemId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_NewsletterSubscription_Email_StoreId] ON [NewsLetterSubscription] ([Email] ASC, [StoreId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_PollAnswer_PollId] ON [PollAnswer] ([PollId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_ProductReview_ProductId] ON [ProductReview] ([ProductId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_RelatedProduct_ProductId1] ON [RelatedProduct] ([ProductId1] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Category_DisplayOrder] ON [Category] ([DisplayOrder] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Category_ParentCategoryId] ON [Category] ([ParentCategoryId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Forums_Group_DisplayOrder] ON [Forums_Group] ([DisplayOrder] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Forums_Forum_DisplayOrder] ON [Forums_Forum] ([DisplayOrder] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Forums_Forum_ForumGroupId] ON [Forums_Forum] ([ForumGroupId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Forums_Topic_ForumId] ON [Forums_Topic] ([ForumId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Forums_Post_TopicId] ON [Forums_Post] ([TopicId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Forums_Post_CustomerId] ON [Forums_Post] ([CustomerId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Forums_Subscription_ForumId] ON [Forums_Subscription] ([ForumId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Forums_Subscription_TopicId] ON [Forums_Subscription] ([TopicId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Product_Deleted_and_Published] ON [Product] ([Published] ASC, [Deleted] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Product_Published] ON [Product] ([Published] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Product_ShowOnHomepage] ON [Product] ([ShowOnHomePage] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Product_ParentGroupedProductId] ON [Product] ([ParentGroupedProductId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Product_VisibleIndividually] ON [Product] ([VisibleIndividually] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_PCM_Product_and_Category] ON [Product_Category_Mapping] ([CategoryId] ASC, [ProductId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_ProductTag_Name] ON [ProductTag] ([Name] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_ActivityLog_CreatedOnUtc] ON [ActivityLog] ([CreatedOnUtc] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_UrlRecord_Slug] ON [UrlRecord] ([Slug] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_UrlRecord_Custom_1] ON [UrlRecord] ([EntityId] ASC, [EntityName] ASC, [LanguageId] ASC, [IsActive] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_AclRecord_EntityId_EntityName] ON [AclRecord] ([EntityId] ASC, [EntityName] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_StoreMapping_EntityId_EntityName] ON [StoreMapping] ([EntityId] ASC, [EntityName] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Category_LimitedToStores] ON [Category] ([LimitedToStores] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Product_LimitedToStores] ON [Product] ([LimitedToStores] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Category_SubjectToAcl] ON [Category] ([SubjectToAcl] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Product_SubjectToAcl] ON [Product] ([SubjectToAcl] ASC)
GO