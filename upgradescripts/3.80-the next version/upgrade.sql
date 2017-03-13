--upgrade scripts from nopCommerce 3.80 to next version

--new locale resources
declare @resources xml
--a resource will be deleted if its value is empty
set @resources='
<Language>
  <LocaleResource Name="Admin.Configuration.Plugins.OfficialFeed.Instructions">
    <Value><![CDATA[<p>Here you can find third-party extensions and themes which are developed by our community and partners.They are also available in our <a href="http://www.nopcommerce.com/marketplace.aspx?utm_source=admin-panel&utm_medium=official-plugins&utm_campaign=admin-panel" target="_blank">marketplace</a></p>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.Captcha.Instructions">
    <Value><![CDATA[<p>A CAPTCHA is a program that can tell whether its user is a human or a computer.You''ve probably seen them — colorful images with distorted text at the bottom ofWeb registration forms. CAPTCHAs are used by many websites to prevent abuse from"bots," or automated programs usually written to generate spam. No computer programcan read distorted text as well as humans can, so bots cannot navigate sites protectedby CAPTCHAs. nopCommerce uses <a href="http://www.google.com/recaptcha" target="_blank">reCAPTCHA</a>.</p>]]></Value>
  </LocaleResource> 
 <LocaleResource Name="Admin.Configuration.Plugins.Description.DownloadMorePlugins">
    <Value><![CDATA[<p>You can download more nopCommerce plugins in our <a href="http://www.nopcommerce.com/marketplace.aspx?utm_source=admin-panel&utm_medium=plugins&utm_campaign=admin-panel" target="_blank">marketplace</a></p>]]></Value>
 </LocaleResource> 
 <LocaleResource Name="Admin.Configuration.Settings.Blog.BlockTitle.BlogComments">
    <Value>Blog comments</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Blog.BlockTitle.Common">
    <Value>Common</Value>
  </LocaleResource>
   <LocaleResource Name="Admin.Configuration.Settings.Catalog.BlockTitle.AdditionalSections">
    <Value>Additional sections</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.BlockTitle.CatalogPages">
    <Value>Catalog pages</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.BlockTitle.Performance">
    <Value>Performance</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.BlockTitle.ProductFields">
    <Value>Product fields</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.BlockTitle.ProductPage">
    <Value>Product page</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.BlockTitle.ProductSorting">
    <Value>Product sorting</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.BlockTitle.ProductReviews">
    <Value>Product reviews</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.BlockTitle.Search">
    <Value>Search</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.BlockTitle.Share">
    <Value>Share</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.BlockTitle.Tags">
    <Value>Tags</Value>
  </LocaleResource>
   <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.BlockTitle.Account">
    <Value>Account</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.BlockTitle.Common">
    <Value>Common</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.BlockTitle.DefaultFields">
    <Value>Default fields</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.BlockTitle.ExternalAuthentication">
    <Value>External authentication</Value>
  </LocaleResource>
 <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.BlockTitle.Password">
    <Value>Password and security</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.BlockTitle.Profile">
    <Value>Profile</Value>
  </LocaleResource>
 <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.BlockTitle.TimeZone">
    <Value>Time zone</Value>
  </LocaleResource> 
  <LocaleResource Name="Admin.Configuration.Settings.Forums.BlockTitle.Common">
   <Value>Common</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Forums.BlockTitle.Feeds">
     <Value>Feeds</Value>
  </LocaleResource>
   <LocaleResource Name="Admin.Configuration.Settings.Forums.BlockTitle.PageSizes">
    <Value>Page sizes</Value>
  </LocaleResource>
<LocaleResource Name="Admin.Configuration.Settings.Forums.BlockTitle.Permissions">
    <Value>Permissions</Value>
  </LocaleResource>
    <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.BlockTitle.FullText">
    <Value>Full-Text</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.BlockTitle.Captcha">
    <Value>CAPTCHA</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.BlockTitle.Common">
    <Value>Common</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.BlockTitle.Localization">
    <Value>Localization</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.BlockTitle.Pdf">
    <Value>Pdf</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.BlockTitle.Security">
    <Value>Security</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.BlockTitle.SEO">
    <Value>SEO</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.BlockTitle.Sitemap">
    <Value>Sitemap</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.BlockTitle.SocialMedia">
    <Value>Social media</Value>
  </LocaleResource>
   <LocaleResource Name="Admin.Configuration.Settings.Media.BlockTitle.Common">
    <Value>Common</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Media.BlockTitle.OtherPages">
    <Value>Other pages</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Media.BlockTitle.Product">
    <Value>Product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.News.BlockTitle.BlogComments">
    <Value>News comments</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.News.BlockTitle.Common">
    <Value>Common</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.ImportCategories">
    <Value>{0} categories were imported</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.ImportProducts">
    <Value>{0} products were imported</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.ImportStates">
    <Value>{0} states and provinces were imported</Value>
  </LocaleResource>
   <LocaleResource Name="Admin.ContentManagement.Topics.Fields.SystemName.Required">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.BlogPosts.List.SearchStore">
    <Value>Store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.BlogPosts.List.SearchStore.Hint">
    <Value>Search by a specific store.</Value>
  </LocaleResource>
   <LocaleResource Name="Admin.Configuration.Settings.Media.DefaultPictureZoomEnabled">
    <Value>Picture zoom</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Media.DefaultPictureZoomEnabled.Hint">
   <Value>Check to enable picture zooming.</Value>
  </LocaleResource>
    <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayBlogMenuItem">
    <Value>Display "Blog"</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayBlogMenuItem.Hint">
    <Value>Check if "Blog" menu item should be displayed in the top menu.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayForumsMenuItem">
    <Value>Display "Forums"</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayForumsMenuItem.Hint">
    <Value>Check if "Forums" menu item should be displayed in the top menu.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayContactUsMenuItem">
    <Value>Display "Contact us"</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayContactUsMenuItem.Hint">
    <Value>Check if "Contact us" menu item should be displayed in the top menu.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayCustomerInfoMenuItem">
    <Value>Display "My account"</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayCustomerInfoMenuItem.Hint">
    <Value>Check if "My account" menu item should be displayed in the top menu.</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayHomePageMenuItem">
    <Value>Display "Home page"</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayHomePageMenuItem.Hint">
    <Value>Check if "Home page" menu item should be displayed in the top menu.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayNewProductsMenuItem">
    <Value>Display "New products"</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayNewProductsMenuItem.Hint">
    <Value>Check if "New products" menu item should be displayed in the top menu.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayProductSearchMenuItem">
    <Value>Display "Search"</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayProductSearchMenuItem.Hint">
    <Value>Check if "Search" menu item should be displayed in the top menu.</Value>
  </LocaleResource> 
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.BlockTitle.TopMenuItems">
    <Value>Top menu items</Value>
  </LocaleResource> 
  <LocaleResource Name="Nop.Web.Framework.Validators.MaxDecimal">
    <Value>The value is out of range. Maximum value is {0}.99</Value>
  </LocaleResource>
</Language>
'

CREATE TABLE #LocaleStringResourceTmp
	(
		[ResourceName] [nvarchar](200) NOT NULL,
		[ResourceValue] [nvarchar](max) NOT NULL
	)

INSERT INTO #LocaleStringResourceTmp (ResourceName, ResourceValue)
SELECT	nref.value('@Name', 'nvarchar(200)'), nref.value('Value[1]', 'nvarchar(MAX)')
FROM	@resources.nodes('//Language/LocaleResource') AS R(nref)

--do it for each existing language
DECLARE @ExistingLanguageId int
DECLARE cur_existinglanguage CURSOR FOR
SELECT [Id]
FROM [Language]
OPEN cur_existinglanguage
FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageId
WHILE @@FETCH_STATUS = 0
BEGIN
	DECLARE @ResourceName nvarchar(200)
	DECLARE @ResourceValue nvarchar(MAX)
	DECLARE cur_localeresource CURSOR FOR
	SELECT ResourceName, ResourceValue
	FROM #LocaleStringResourceTmp
	OPEN cur_localeresource
	FETCH NEXT FROM cur_localeresource INTO @ResourceName, @ResourceValue
	WHILE @@FETCH_STATUS = 0
	BEGIN
		IF (EXISTS (SELECT 1 FROM [LocaleStringResource] WHERE LanguageId=@ExistingLanguageId AND ResourceName=@ResourceName))
		BEGIN
			UPDATE [LocaleStringResource]
			SET [ResourceValue]=@ResourceValue
			WHERE LanguageId=@ExistingLanguageId AND ResourceName=@ResourceName
		END
		ELSE 
		BEGIN
			INSERT INTO [LocaleStringResource]
			(
				[LanguageId],
				[ResourceName],
				[ResourceValue]
			)
			VALUES
			(
				@ExistingLanguageId,
				@ResourceName,
				@ResourceValue
			)
		END
		
		IF (@ResourceValue is null or @ResourceValue = '')
		BEGIN
			DELETE [LocaleStringResource]
			WHERE LanguageId=@ExistingLanguageId AND ResourceName=@ResourceName
		END
		
		FETCH NEXT FROM cur_localeresource INTO @ResourceName, @ResourceValue
	END
	CLOSE cur_localeresource
	DEALLOCATE cur_localeresource

	--fetch next language identifier
	FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageId
END
CLOSE cur_existinglanguage
DEALLOCATE cur_existinglanguage

DROP TABLE #LocaleStringResourceTmp
GO

 --new setting
 IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'adminareasettings.useisodatetimeconverterinjson')
 BEGIN
     INSERT [Setting] ([Name], [Value], [StoreId])
     VALUES (N'adminareasettings.useisodatetimeconverterinjson', N'True', 0)
 END
 GO

 --new activity type
 IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'ImportCategories')
 BEGIN
 	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
 	VALUES (N'ImportCategories', N'Categories were imported', N'True')
 END
 GO
 
 --new activity type
 IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'ImportProducts')
 BEGIN
 	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
 	VALUES (N'ImportProducts', N'Products were imported', N'True')
 END
 GO

--new activity type
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'ImportStates')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'ImportStates', N'States and provinces were imported', N'True')
END
GO 

 --new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'adminareasettings.usericheditorinmessagetemplates')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'adminareasettings.usericheditorinmessagetemplates', N'False', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'mediasettings.azurecachecontrolheader')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'mediasettings.azurecachecontrolheader', N'', 0)
END
GO

--add stored procedure for getting category tree
IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[nop_padright]') AND xtype IN (N'FN', N'IF', N'TF'))
DROP FUNCTION  [dbo].[nop_padright]
GO

CREATE FUNCTION [dbo].[nop_padright]
(
    @source INT, 
    @symbol NVARCHAR(MAX), 
    @length INT
)
RETURNS NVARCHAR(MAX)
AS
BEGIN
    RETURN RIGHT(REPLICATE(@symbol, @length)+ RTRIM(CAST(@source AS NVARCHAR(MAX))), @length)
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[CategoryLoadAllPaged]') AND OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CategoryLoadAllPaged]
GO

CREATE PROCEDURE [dbo].[CategoryLoadAllPaged]
(
    @ShowHidden         BIT = 0,
    @Name               NVARCHAR(MAX) = NULL,
    @StoreId            INT = 0,
    @CustomerRoleIds	NVARCHAR(MAX) = NULL,
    @PageIndex			INT = 0,
	@PageSize			INT = 2147483644,
    @TotalRecords		INT = NULL OUTPUT
)
AS
BEGIN
	SET NOCOUNT ON

    --filter by customer role IDs (access control list)
	SET @CustomerRoleIds = ISNULL(@CustomerRoleIds, '')
	CREATE TABLE #FilteredCustomerRoleIds
	(
		CustomerRoleId INT NOT NULL
	)
	INSERT INTO #FilteredCustomerRoleIds (CustomerRoleId)
	SELECT CAST(data AS INT) FROM [nop_splitstring_to_table](@CustomerRoleIds, ',')
	DECLARE @FilteredCustomerRoleIdsCount INT = (SELECT COUNT(1) FROM #FilteredCustomerRoleIds)

    --ordered categories
    CREATE TABLE #OrderedCategoryIds
	(
		[Id] int IDENTITY (1, 1) NOT NULL,
		[CategoryId] int NOT NULL
	)
    
    --get max length of DisplayOrder and Id columns (used for padding Order column)
    DECLARE @lengthId INT = (SELECT LEN(MAX(Id)) FROM [Category])
    DECLARE @lengthOrder INT = (SELECT LEN(MAX(DisplayOrder)) FROM [Category])

    --get category tree
    ;WITH [CategoryTree]
    AS (SELECT [Category].[Id] AS [Id], dbo.[nop_padright] ([Category].[DisplayOrder], '0', @lengthOrder) + '-' + dbo.[nop_padright] ([Category].[Id], '0', @lengthId) AS [Order]
        FROM [Category] WHERE [Category].[ParentCategoryId] = 0
        UNION ALL
        SELECT [Category].[Id] AS [Id], [CategoryTree].[Order] + '|' + dbo.[nop_padright] ([Category].[DisplayOrder], '0', @lengthOrder) + '-' + dbo.[nop_padright] ([Category].[Id], '0', @lengthId) AS [Order]
        FROM [Category]
        INNER JOIN [CategoryTree] ON [CategoryTree].[Id] = [Category].[ParentCategoryId])
    INSERT INTO #OrderedCategoryIds ([CategoryId])
    SELECT [Category].[Id]
    FROM [CategoryTree]
    RIGHT JOIN [Category] ON [CategoryTree].[Id] = [Category].[Id]

    --filter results
    WHERE [Category].[Deleted] = 0
    AND (@ShowHidden = 1 OR [Category].[Published] = 1)
    AND (@Name IS NULL OR @Name = '' OR [Category].[Name] LIKE ('%' + @Name + '%'))
    AND (@ShowHidden = 1 OR @FilteredCustomerRoleIdsCount  = 0 OR [Category].[SubjectToAcl] = 0
        OR EXISTS (SELECT 1 FROM #FilteredCustomerRoleIds [roles] WHERE [roles].[CustomerRoleId] IN
            (SELECT [acl].[CustomerRoleId] FROM [AclRecord] acl WITH (NOLOCK) WHERE [acl].[EntityId] = [Category].[Id] AND [acl].[EntityName] = 'Category')
        )
    )
    AND (@StoreId = 0 OR [Category].[LimitedToStores] = 0
        OR EXISTS (SELECT 1 FROM [StoreMapping] sm WITH (NOLOCK)
			WHERE [sm].[EntityId] = [Category].[Id] AND [sm].[EntityName] = 'Category' AND [sm].[StoreId] = @StoreId
		)
    )
    ORDER BY ISNULL([CategoryTree].[Order], 1)

    --total records
    SET @TotalRecords = @@ROWCOUNT

    --paging
    SELECT [Category].* FROM #OrderedCategoryIds AS [Result] INNER JOIN [Category] ON [Result].[CategoryId] = [Category].[Id]
    WHERE ([Result].[Id] > @PageSize * @PageIndex AND [Result].[Id] <= @PageSize * (@PageIndex + 1))
    ORDER BY [Result].[Id]

    DROP TABLE #FilteredCustomerRoleIds
    DROP TABLE #OrderedCategoryIds
END
GO

 --new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'commonsettings.usestoredprocedureforloadingcategories')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'commonsettings.usestoredprocedureforloadingcategories', N'False', 0)
END
 --new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'storeinformationsettings.displayminiprofilerforadminonly')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'storeinformationsettings.displayminiprofilerforadminonly', N'False', 0)
END
  GO 

  --indicating whether to display default menu items
DECLARE @displayMenuItems bit
IF NOT EXISTS (SELECT 1 FROM [Category] where ParentCategoryId=0 and Deleted=0 and Published=1)
	set @displayMenuItems = N'True'
ELSE
    set @displayMenuItems = N'False'

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'displaydefaultmenuitemsettings.displayhomepagemenuitem')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'displaydefaultmenuitemsettings.displayhomepagemenuitem', @displayMenuItems, 0)
END

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'displaydefaultmenuitemsettings.displaynewproductsmenuitem')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'displaydefaultmenuitemsettings.displaynewproductsmenuitem', @displayMenuItems, 0)
END

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'displaydefaultmenuitemsettings.displayproductsearchmenuitem')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'displaydefaultmenuitemsettings.displayproductsearchmenuitem', @displayMenuItems, 0)
END

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'displaydefaultmenuitemsettings.displaycustomerinfomenuitem')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'displaydefaultmenuitemsettings.displaycustomerinfomenuitem', @displayMenuItems, 0)
END

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'displaydefaultmenuitemsettings.displayblogmenuitem')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'displaydefaultmenuitemsettings.displayblogmenuitem', @displayMenuItems, 0)
END

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'displaydefaultmenuitemsettings.displayforumsmenuitem')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'displaydefaultmenuitemsettings.displayforumsmenuitem', @displayMenuItems, 0)
END

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'displaydefaultmenuitemsettings.displaycontactusmenuitem ')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'displaydefaultmenuitemsettings.displaycontactusmenuitem ', @displayMenuItems, 0)
END
GO

