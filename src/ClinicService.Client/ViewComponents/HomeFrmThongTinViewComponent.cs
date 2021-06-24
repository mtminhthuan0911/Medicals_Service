using ClinicService.Client.Models;
using ClinicService.Client.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicService.Client.ViewComponents
{
    public class HomeFrmThongTinViewComponent : ViewComponent
    {
        private readonly IWebsiteSectionApiClient _websiteSectionApiClient;
        private readonly IClinicBranchApiClient _clinicBranchApiClient;
        private readonly ILogger<HomeFrmThongTinViewComponent> _logger;

        public HomeFrmThongTinViewComponent(IWebsiteSectionApiClient websiteSectionApiClient, IClinicBranchApiClient clinicBranchApiClient, ILogger<HomeFrmThongTinViewComponent> logger)
        {
            _websiteSectionApiClient = websiteSectionApiClient;
            _clinicBranchApiClient = clinicBranchApiClient;
            _logger = logger;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {
                return View(new HomeFrmThongTinViewModel
                {
                    Parent = await _websiteSectionApiClient.GetById("FRM_THONG_TIN"),
                    Children = await _websiteSectionApiClient.GetChildrenByParentId("FRM_THONG_TIN"),
                    ClinicBranches = await _clinicBranchApiClient.GetAll()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View();
            }
        }
    }
}
