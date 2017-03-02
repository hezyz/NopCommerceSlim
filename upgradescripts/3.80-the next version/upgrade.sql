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