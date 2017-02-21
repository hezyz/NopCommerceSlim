using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Catalog
{
    public partial class BulkEditProductModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.Catalog.BulkEdit.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.BulkEdit.Fields.SKU")]
        [AllowHtml]
        public string Sku { get; set; }

        [NopResourceDisplayName("Admin.Catalog.BulkEdit.Fields.Published")]
        public bool Published { get; set; }
    }
}