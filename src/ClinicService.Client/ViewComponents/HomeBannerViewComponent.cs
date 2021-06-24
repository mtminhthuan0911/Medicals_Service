using ClinicService.Client.Models;
using ClinicService.Client.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ClinicService.Client.ViewComponents
{
    public class HomeBannerViewComponent : ViewComponent
    {
        private readonly IWebsiteSectionApiClient _websiteSectionApiClient;
        private readonly ILogger<HomeFrmThongTinViewComponent> _logger;

        public HomeBannerViewComponent(IWebsiteSectionApiClient websiteSectionApiClient, ILogger<HomeFrmThongTinViewComponent> logger)
        {
            _websiteSectionApiClient = websiteSectionApiClient;
            _logger = logger;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {
                return View(await _websiteSectionApiClient.GetById("BANNER"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View();
            }
        }
    }
}
