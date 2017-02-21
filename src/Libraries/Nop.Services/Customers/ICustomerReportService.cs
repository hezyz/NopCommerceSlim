
namespace Nop.Services.Customers
{
    /// <summary>
    /// Customer report service interface
    /// </summary>
    public partial interface ICustomerReportService
    {        
        /// <summary>
        /// Gets a report of customers registered in the last days
        /// </summary>
        /// <param name="days">Customers registered in the last days</param>
        /// <returns>Number of registered customers</returns>
        int GetRegisteredCustomersReport(int days);
    }
}