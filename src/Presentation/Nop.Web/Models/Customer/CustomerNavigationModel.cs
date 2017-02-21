using System.Collections.Generic;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Customer
{
    public partial class CustomerNavigationModel : BaseNopModel
    {
        public CustomerNavigationModel()
        {
            CustomerNavigationItems = new List<CustomerNavigationItemModel>();
        }

        public IList<CustomerNavigationItemModel> CustomerNavigationItems { get; set; }

        public CustomerNavigationEnum SelectedTab { get; set; }
    }

    public class CustomerNavigationItemModel
    {
        public string RouteName { get; set; }
        public string Title { get; set; }
        public CustomerNavigationEnum Tab { get; set; }
        public string ItemClass { get; set; }
    }

    public enum CustomerNavigationEnum
    {
        Info = 0,
        Addresses = 10,
        ChangePassword = 70,
        Avatar = 80,
        ForumSubscriptions = 90,
        ProductReviews = 100,
    }
}