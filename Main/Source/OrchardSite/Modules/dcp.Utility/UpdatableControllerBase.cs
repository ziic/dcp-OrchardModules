using System;
using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.Localization;

namespace dcp.Utility
{
    public abstract class UpdatableControllerBase : Controller, IUpdateModel
    {
        public Localizer T { get; set; }

        #region IUpdateModel

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties)
        {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage)
        {
            ModelState.AddModelError(key, errorMessage.Text);
        }

        #endregion

        public void AddModelError(string key, LocalizedString errorMessage)
        {
            ((IUpdateModel)this).AddModelError(key, errorMessage);
        }

        public void AddModelError(string key, Exception e)
        {
            ((IUpdateModel)this).AddModelError(key, T(e.Message));
        }
    }
}