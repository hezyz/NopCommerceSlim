using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Settings
{
    public partial class ProductEditorSettingsModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.Configuration.Settings.ProductEditor.Id")]
        public bool Id { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.ProductEditor.ProductType")]
        public bool ProductType { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.ProductEditor.VisibleIndividually")]
        public bool VisibleIndividually { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.ProductEditor.ProductTemplate")]
        public bool ProductTemplate { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.ProductEditor.AdminComment")]
        public bool AdminComment { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.ProductEditor.Stores")]
        public bool Stores { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.ProductEditor.ACL")]
        public bool ACL { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.ProductEditor.ShowOnHomePage")]
        public bool ShowOnHomePage { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.ProductEditor.DisplayOrder")]
        public bool DisplayOrder { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.ProductEditor.AllowCustomerReviews")]
        public bool AllowCustomerReviews { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.ProductEditor.ProductTags")]
        public bool ProductTags { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.ProductEditor.MarkAsNew")]
        public bool MarkAsNew { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.ProductEditor.MarkAsNewStartDate")]
        public bool MarkAsNewStartDate { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.ProductEditor.MarkAsNewEndDate")]
        public bool MarkAsNewEndDate { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.ProductEditor.Published")]
        public bool Published { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.ProductEditor.CreatedOn")]
        public bool CreatedOn { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.ProductEditor.UpdatedOn")]
        public bool UpdatedOn { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.ProductEditor.RelatedProducts")]
        public bool RelatedProducts { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.ProductEditor.Seo")]
        public bool Seo { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.ProductEditor.OneColumnProductPage")]
        public bool OneColumnProductPage { get; set; }

    }
}