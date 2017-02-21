using System.Web.Mvc;
using Nop.Web.Models.Common;

namespace Nop.Web.Factories
{
    public partial interface ICommonModelFactory
    {
        LogoModel PrepareLogoModel();

        LanguageSelectorModel PrepareLanguageSelectorModel();

        HeaderLinksModel PrepareHeaderLinksModel();

        AdminHeaderLinksModel PrepareAdminHeaderLinksModel();

        SocialModel PrepareSocialModel();

        FooterModel PrepareFooterModel();

        ContactUsModel PrepareContactUsModel(ContactUsModel model, bool excludeProperties);

        SitemapModel PrepareSitemapModel();

        string PrepareSitemapXml(UrlHelper url);

        StoreThemeSelectorModel PrepareStoreThemeSelectorModel();

        FaviconModel PrepareFaviconModel();

        string PrepareRobotsTextFile();
    }
}
