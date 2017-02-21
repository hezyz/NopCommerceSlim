using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;
using Nop.Services.Common;
using Nop.Services.Customers.Cache;
using Nop.Services.Localization;

namespace Nop.Services.Customers
{
    public static class CustomerExtensions
    {
        /// <summary>
        /// Get full name
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>Customer full name</returns>
        public static string GetFullName(this Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");
            var firstName = customer.GetAttribute<string>(SystemCustomerAttributeNames.FirstName);
            var lastName = customer.GetAttribute<string>(SystemCustomerAttributeNames.LastName);

            string fullName = "";
            if (!String.IsNullOrWhiteSpace(firstName) && !String.IsNullOrWhiteSpace(lastName))
                fullName = string.Format("{0} {1}", firstName, lastName);
            else
            {
                if (!String.IsNullOrWhiteSpace(firstName))
                    fullName = firstName;

                if (!String.IsNullOrWhiteSpace(lastName))
                    fullName = lastName;
            }
            return fullName;
        }
        /// <summary>
        /// Formats the customer name
        /// </summary>
        /// <param name="customer">Source</param>
        /// <param name="stripTooLong">Strip too long customer name</param>
        /// <param name="maxLength">Maximum customer name length</param>
        /// <returns>Formatted text</returns>
        public static string FormatUserName(this Customer customer, bool stripTooLong = false, int maxLength = 0)
        {
            if (customer == null)
                return string.Empty;

            if (customer.IsGuest())
            {
                return EngineContext.Current.Resolve<ILocalizationService>().GetResource("Customer.Guest");
            }

            string result = string.Empty;
            switch (EngineContext.Current.Resolve<CustomerSettings>().CustomerNameFormat)
            {
                case CustomerNameFormat.ShowEmails:
                    result = customer.Email;
                    break;
                case CustomerNameFormat.ShowUsernames:
                    result = customer.Username;
                    break;
                case CustomerNameFormat.ShowFullNames:
                    result = customer.GetFullName();
                    break;
                case CustomerNameFormat.ShowFirstName:
                    result = customer.GetAttribute<string>(SystemCustomerAttributeNames.FirstName);
                    break;
                default:
                    break;
            }

            if (stripTooLong && maxLength > 0)
            {
                result = CommonHelper.EnsureMaximumLength(result, maxLength);
            }

            return result;
        }

        /// <summary>
        /// Check whether password recovery token is valid
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="token">Token to validate</param>
        /// <returns>Result</returns>
        public static bool IsPasswordRecoveryTokenValid(this Customer customer, string token)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            var cPrt = customer.GetAttribute<string>(SystemCustomerAttributeNames.PasswordRecoveryToken);
            if (String.IsNullOrEmpty(cPrt))
                return false;

            if (!cPrt.Equals(token, StringComparison.InvariantCultureIgnoreCase))
                return false;

            return true;
        }
        /// <summary>
        /// Check whether password recovery link is expired
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="customerSettings">Customer settings</param>
        /// <returns>Result</returns>
        public static bool IsPasswordRecoveryLinkExpired(this Customer customer, CustomerSettings customerSettings)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            if (customerSettings == null)
                throw new ArgumentNullException("customerSettings");

            if (customerSettings.PasswordRecoveryLinkDaysValid == 0)
                return false;
            
            var geneatedDate = customer.GetAttribute<DateTime?>(SystemCustomerAttributeNames.PasswordRecoveryTokenDateGenerated);
            if (!geneatedDate.HasValue)
                return false;

            var daysPassed = (DateTime.UtcNow - geneatedDate.Value).TotalDays;
            if (daysPassed > customerSettings.PasswordRecoveryLinkDaysValid)
                return true;

            return false;
        }

        /// <summary>
        /// Get customer role identifiers
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="showHidden">A value indicating whether to load hidden records</param>
        /// <returns>Customer role identifiers</returns>
        public static int[] GetCustomerRoleIds(this Customer customer, bool showHidden = false)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            var customerRolesIds = customer.CustomerRoles
               .Where(cr => showHidden || cr.Active)
               .Select(cr => cr.Id)
               .ToArray();

            return customerRolesIds;
        }

        /// <summary>
        /// Check whether customer password is expired 
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>True if password is expired; otherwise false</returns>
        public static bool PasswordIsExpired(this Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            //the guests don't have a password
            if (customer.IsGuest())
                return false;

            //password lifetime is disabled for user
            if (!customer.CustomerRoles.Any(role => role.Active && role.EnablePasswordLifetime))
                return false;

            //setting disabled for all
            var customerSettings = EngineContext.Current.Resolve<CustomerSettings>();
            if (customerSettings.PasswordLifetime == 0)
                return false;

            //cache result between HTTP requests 
            var cacheManager = EngineContext.Current.ContainerManager.Resolve<ICacheManager>("nop_cache_static");
            var cacheKey = string.Format(CustomerCacheEventConsumer.CUSTOMER_PASSWORD_LIFETIME, customer.Id);
            //get current password usage time
            var currentLifetime = cacheManager.Get(cacheKey, () =>
            {
                var customerPassword = EngineContext.Current.Resolve<ICustomerService>().GetCurrentPassword(customer.Id);
                //password is not found, so return max value to force customer to change password
                if (customerPassword == null)
                    return int.MaxValue;

                return (DateTime.UtcNow - customerPassword.CreatedOnUtc).Days;
            });

            return currentLifetime >= customerSettings.PasswordLifetime;
        }
    }
}
