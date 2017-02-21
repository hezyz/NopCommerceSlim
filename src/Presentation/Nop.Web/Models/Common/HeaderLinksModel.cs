using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Common
{
    public partial class HeaderLinksModel : BaseNopModel
    {
        public bool IsAuthenticated { get; set; }
        public string CustomerName { get; set; }
        

        public bool AllowPrivateMessages { get; set; }
        public string UnreadPrivateMessages { get; set; }
        public string AlertMessage { get; set; }
    }
}