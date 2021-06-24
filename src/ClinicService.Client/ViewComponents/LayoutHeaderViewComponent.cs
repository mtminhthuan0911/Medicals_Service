using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClinicService.Client.Constants;
using ClinicService.Client.Models;
using ClinicService.Client.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClinicService.Client.ViewComponents
{
    public class LayoutHeaderViewComponent : ViewComponent
    {
        private readonly IWebsiteSectionApiClient _websiteSectionApiClient;

        public LayoutHeaderViewComponent(IWebsiteSectionApiClient websiteSectionApiClient)
        {
            _websiteSectionApiClient = websiteSectionApiClient;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var parentNavigations = await _websiteSectionApiClient.GetChildrenByParentId(SystemsConstant.WebsiteSection.WEB_SEC_DIEU_HUONG);
            var viewModels = new List<NavigationViewModel>();

            foreach(var item in parentNavigations)
            {
                var viewModel = new NavigationViewModel
                {
                    Item = item,
                    Children = await GetChildrenByParentId(item.Id)
                };

                viewModels.Add(viewModel);
            }

            return View(viewModels);
        }

        private async Task<IEnumerable<NavigationViewModel>> GetChildrenByParentId(string parentId)
        {
            var childrenNavigations = await _websiteSectionApiClient.GetChildrenByParentId(parentId);

            if (childrenNavigations == null)
                return null;

            var viewModels = new List<NavigationViewModel>();

            foreach(var item in childrenNavigations)
            {
                var viewModel = new NavigationViewModel
                {
                    Item = item,
                    Children = await GetChildrenByParentId(item.Id)
                };

                viewModels.Add(viewModel);
            }

            return viewModels;
        }
    }
}
