﻿using System;
using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Extensions;
using Nop.Admin.Models.Templates;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Topics;
using Nop.Services.Catalog;
using Nop.Services.Security;
using Nop.Services.Topics;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Controllers
{
    public partial class TemplateController : BaseAdminController
    {
        #region Fields

        private readonly ICategoryTemplateService _categoryTemplateService;
        private readonly IProductTemplateService _productTemplateService;
        private readonly ITopicTemplateService _topicTemplateService;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Constructors

        public TemplateController(ICategoryTemplateService categoryTemplateService,
            IProductTemplateService productTemplateService,
            ITopicTemplateService topicTemplateService,
            IPermissionService permissionService)
        {
            this._categoryTemplateService = categoryTemplateService;
            this._productTemplateService = productTemplateService;
            this._topicTemplateService = topicTemplateService;
            this._permissionService = permissionService;
        }

        #endregion

        #region Category templates

        public virtual ActionResult CategoryTemplates()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public virtual ActionResult CategoryTemplates(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedKendoGridJson();

            var templatesModel = _categoryTemplateService.GetAllCategoryTemplates()
                .Select(x => x.ToModel())
                .ToList();
            var gridModel = new DataSourceResult
            {
                Data = templatesModel,
                Total = templatesModel.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public virtual ActionResult CategoryTemplateUpdate(CategoryTemplateModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });
            }

            var template = _categoryTemplateService.GetCategoryTemplateById(model.Id);
            if (template == null)
                throw new ArgumentException("No template found with the specified id");
            template = model.ToEntity(template);
            _categoryTemplateService.UpdateCategoryTemplate(template);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual ActionResult CategoryTemplateAdd([Bind(Exclude = "Id")] CategoryTemplateModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });
            }

            var template = new CategoryTemplate();
            template = model.ToEntity(template);
            _categoryTemplateService.InsertCategoryTemplate(template);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual ActionResult CategoryTemplateDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var template = _categoryTemplateService.GetCategoryTemplateById(id);
            if (template == null)
                throw new ArgumentException("No template found with the specified id");

            _categoryTemplateService.DeleteCategoryTemplate(template);

            return new NullJsonResult();
        }

        #endregion

        #region Product templates

        public virtual ActionResult ProductTemplates()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public virtual ActionResult ProductTemplates(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedKendoGridJson();

            var templatesModel = _productTemplateService.GetAllProductTemplates()
                .Select(x => x.ToModel())
                .ToList();
            var gridModel = new DataSourceResult
            {
                Data = templatesModel,
                Total = templatesModel.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public virtual ActionResult ProductTemplateUpdate(ProductTemplateModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });
            }

            var template = _productTemplateService.GetProductTemplateById(model.Id);
            if (template == null)
                throw new ArgumentException("No template found with the specified id");
            template = model.ToEntity(template);
            _productTemplateService.UpdateProductTemplate(template);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual ActionResult ProductTemplateAdd([Bind(Exclude = "Id")] ProductTemplateModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });
            }

            var template = new ProductTemplate();
            template = model.ToEntity(template);
            _productTemplateService.InsertProductTemplate(template);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual ActionResult ProductTemplateDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var template = _productTemplateService.GetProductTemplateById(id);
            if (template == null)
                throw new ArgumentException("No template found with the specified id");

            _productTemplateService.DeleteProductTemplate(template);

            return new NullJsonResult();
        }

        #endregion

        #region Topic templates

        public virtual ActionResult TopicTemplates()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public virtual ActionResult TopicTemplates(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedKendoGridJson();

            var templatesModel = _topicTemplateService.GetAllTopicTemplates()
                .Select(x => x.ToModel())
                .ToList();
            var gridModel = new DataSourceResult
            {
                Data = templatesModel,
                Total = templatesModel.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public virtual ActionResult TopicTemplateUpdate(TopicTemplateModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });
            }

            var template = _topicTemplateService.GetTopicTemplateById(model.Id);
            if (template == null)
                throw new ArgumentException("No template found with the specified id");
            template = model.ToEntity(template);
            _topicTemplateService.UpdateTopicTemplate(template);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual ActionResult TopicTemplateAdd([Bind(Exclude = "Id")] TopicTemplateModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });
            }

            var template = new TopicTemplate();
            template = model.ToEntity(template);
            _topicTemplateService.InsertTopicTemplate(template);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual ActionResult TopicTemplateDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var template = _topicTemplateService.GetTopicTemplateById(id);
            if (template == null)
                throw new ArgumentException("No template found with the specified id");

            _topicTemplateService.DeleteTopicTemplate(template);

            return new NullJsonResult();
        }

        #endregion
    }
}
