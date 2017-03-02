using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Extensions;
using Nop.Admin.Helpers;
using Nop.Admin.Models.Customers;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;

namespace Nop.Admin.Controllers
{
    public partial class CustomerRoleController : BaseAdminController
	{
		#region Fields

		private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IPermissionService _permissionService;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly ICacheManager _cacheManager;

		#endregion

		#region Constructors

        public CustomerRoleController(ICustomerService customerService,
            ILocalizationService localizationService, 
            ICustomerActivityService customerActivityService,
            IPermissionService permissionService,
            IProductService productService,
            ICategoryService categoryService,
            IStoreService storeService,
            IWorkContext workContext, 
            ICacheManager cacheManager)
		{
            this._customerService = customerService;
            this._localizationService = localizationService;
            this._customerActivityService = customerActivityService;
            this._permissionService = permissionService;
            this._productService = productService;
            this._categoryService = categoryService;
            this._storeService = storeService;
            this._workContext = workContext;
            this._cacheManager = cacheManager;
		}

		#endregion 

        #region Utilities

        [NonAction]
        protected virtual CustomerRoleModel PrepareCustomerRoleModel(CustomerRole customerRole)
        {
            var model = customerRole.ToModel();
            return model;
        }

        #endregion

        #region Customer roles

        public virtual ActionResult Index()
        {
            return RedirectToAction("List");
        }

		public virtual ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();
            
			return View();
		}

		[HttpPost]
		public virtual ActionResult List(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedKendoGridJson();
            
            var customerRoles = _customerService.GetAllCustomerRoles(true);
            var gridModel = new DataSourceResult
			{
                Data = customerRoles.Select(PrepareCustomerRoleModel),
                Total = customerRoles.Count()
			};
            return Json(gridModel);
        }

        public virtual ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();
            
            var model = new CustomerRoleModel();
            //default values
            model.Active = true;
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult Create(CustomerRoleModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();
            
            if (ModelState.IsValid)
            {
                var customerRole = model.ToEntity();
                _customerService.InsertCustomerRole(customerRole);

                //activity log
                _customerActivityService.InsertActivity("AddNewCustomerRole", _localizationService.GetResource("ActivityLog.AddNewCustomerRole"), customerRole.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Customers.CustomerRoles.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = customerRole.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

		public virtual ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();
            
            var customerRole = _customerService.GetCustomerRoleById(id);
            if (customerRole == null)
                //No customer role found with the specified id
                return RedirectToAction("List");
		    
            var model = PrepareCustomerRoleModel(customerRole);
            return View(model);
		}

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult Edit(CustomerRoleModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();
            
            var customerRole = _customerService.GetCustomerRoleById(model.Id);
            if (customerRole == null)
                //No customer role found with the specified id
                return RedirectToAction("List");

            try
            {
                if (ModelState.IsValid)
                {
                    if (customerRole.IsSystemRole && !model.Active)
                        throw new NopException(_localizationService.GetResource("Admin.Customers.CustomerRoles.Fields.Active.CantEditSystem"));

                    if (customerRole.IsSystemRole && !customerRole.SystemName.Equals(model.SystemName, StringComparison.InvariantCultureIgnoreCase))
                        throw new NopException(_localizationService.GetResource("Admin.Customers.CustomerRoles.Fields.SystemName.CantEditSystem"));

                    customerRole = model.ToEntity(customerRole);
                    _customerService.UpdateCustomerRole(customerRole);

                    //activity log
                    _customerActivityService.InsertActivity("EditCustomerRole", _localizationService.GetResource("ActivityLog.EditCustomerRole"), customerRole.Name);

                    SuccessNotification(_localizationService.GetResource("Admin.Customers.CustomerRoles.Updated"));
                    return continueEditing ? RedirectToAction("Edit", new { id = customerRole.Id}) : RedirectToAction("List");
                }

                //If we got this far, something failed, redisplay form
                return View(model);
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("Edit", new { id = customerRole.Id });
            }
        }

        [HttpPost]
        public virtual ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();
            
            var customerRole = _customerService.GetCustomerRoleById(id);
            if (customerRole == null)
                //No customer role found with the specified id
                return RedirectToAction("List");

            try
            {
                _customerService.DeleteCustomerRole(customerRole);

                //activity log
                _customerActivityService.InsertActivity("DeleteCustomerRole", _localizationService.GetResource("ActivityLog.DeleteCustomerRole"), customerRole.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Customers.CustomerRoles.Deleted"));
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc.Message);
                return RedirectToAction("Edit", new { id = customerRole.Id });
            }

		}
		#endregion
    }
}
