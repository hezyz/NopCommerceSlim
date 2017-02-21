using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Services.Helpers;

namespace Nop.Services.Customers
{
    /// <summary>
    /// Customer report service
    /// </summary>
    public partial class CustomerReportService : ICustomerReportService
    {
        #region Fields

        private readonly IRepository<Customer> _customerRepository;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        
        #endregion

        #region Ctor
        
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="customerRepository">Customer repository</param>
        /// <param name="customerService">Customer service</param>
        /// <param name="dateTimeHelper">Date time helper</param>
        public CustomerReportService(IRepository<Customer> customerRepository,
             ICustomerService customerService,
            IDateTimeHelper dateTimeHelper)
        {
            this._customerRepository = customerRepository;
            this._customerService = customerService;
            this._dateTimeHelper = dateTimeHelper;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a report of customers registered in the last days
        /// </summary>
        /// <param name="days">Customers registered in the last days</param>
        /// <returns>Number of registered customers</returns>
        public virtual int GetRegisteredCustomersReport(int days)
        {
            DateTime date = _dateTimeHelper.ConvertToUserTime(DateTime.Now).AddDays(-days);

            var registeredCustomerRole = _customerService.GetCustomerRoleBySystemName(SystemCustomerRoleNames.Registered);
            if (registeredCustomerRole == null)
                return 0;

            var query = from c in _customerRepository.Table
                        from cr in c.CustomerRoles
                        where !c.Deleted &&
                        cr.Id == registeredCustomerRole.Id &&
                        c.CreatedOnUtc >= date 
                        //&& c.CreatedOnUtc <= DateTime.UtcNow
                        select c;
            int count = query.Count();
            return count;
        }

        #endregion
    }
}