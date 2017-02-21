using System.Collections.Generic;
using Nop.Core.Domain.Customers;
using Nop.Web.Models.Customer;

namespace Nop.Web.Factories
{
    public partial interface ICustomerModelFactory
    {
        IList<CustomerAttributeModel> PrepareCustomCustomerAttributes(Customer customer, string overrideAttributesXml = "");

        CustomerInfoModel PrepareCustomerInfoModel(CustomerInfoModel model, Customer customer, 
            bool excludeProperties, string overrideCustomCustomerAttributesXml = "");

        RegisterModel PrepareRegisterModel(RegisterModel model, bool excludeProperties, 
            string overrideCustomCustomerAttributesXml = "", bool setDefaultValues = false);

        LoginModel PrepareLoginModel();

        PasswordRecoveryModel PreparePasswordRecoveryModel();

        PasswordRecoveryConfirmModel PreparePasswordRecoveryConfirmModel();

        RegisterResultModel PrepareRegisterResultModel(int resultId);

        CustomerNavigationModel PrepareCustomerNavigationModel(int selectedTabId = 0);

        CustomerAddressListModel PrepareCustomerAddressListModel();

        ChangePasswordModel PrepareChangePasswordModel();

        CustomerAvatarModel PrepareCustomerAvatarModel(CustomerAvatarModel model);
    }
}
